using Lyra.Data;
using Lyra.Enums;
using Lyra.Models;
using Lyra.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Lyra.Controllers
{
    [Authorize(Roles = UserRoles.Cliente)]
    public class ClienteController : Controller
    {
        private readonly ApplicationDbContext     _context;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly BodyTypeService          _bodyTypeService;

        public ClienteController(
            ApplicationDbContext context,
            UserManager<IdentityUser> userManager,
            BodyTypeService bodyTypeService)
        {
            _context         = context;
            _userManager     = userManager;
            _bodyTypeService = bodyTypeService;
        }

        // ═══════════════════════════════════════════
        //  DASHBOARD
        // ═══════════════════════════════════════════
        public async Task<IActionResult> Dashboard()
{
    var user = await _userManager.GetUserAsync(User);
    var perfil = await _context.UsuariosPerfil
        .Include(p => p.MedidasCorporales)
        .FirstOrDefaultAsync(p => p.UserId == user!.Id);

    if (perfil == null || !perfil.PerfilCompleto)
        return RedirectToAction("CompletarPerfil");

    ViewBag.TotalFavoritos = await _context.Favoritos
        .CountAsync(f => f.UsuarioPerfilId == perfil.Id);

    // Colorimetría
    var colorService = HttpContext.RequestServices
        .GetRequiredService<ColorimetriaService>();
    ViewBag.Paleta = colorService.ObtenerParaTono(perfil.TonoPiel);

    return View(perfil);
}

        // ═══════════════════════════════════════════
        //  COMPLETAR PERFIL
        // ═══════════════════════════════════════════
        [HttpGet]
        public async Task<IActionResult> CompletarPerfil()
        {
            var user = await _userManager.GetUserAsync(User);
            var perfil = await _context.UsuariosPerfil
                .Include(p => p.MedidasCorporales)
                .FirstOrDefaultAsync(p => p.UserId == user!.Id);

            // Si ya tiene perfil, lo manda con los datos para editar
            return View(perfil ?? new UsuarioPerfil());
        }

// ═══════════════════════════════════════════
//  DETALLE DE PRENDA
// ═══════════════════════════════════════════
[HttpGet]
public async Task<IActionResult> DetallePrenda(int id)
{
    var prenda = await _context.Prendas
        .Include(p => p.Tienda)
        .FirstOrDefaultAsync(p =>
            p.Id == id &&
            p.Estado == EstadoPrenda.Disponible);

    if (prenda == null) return NotFound();

    var user   = await _userManager.GetUserAsync(User);
    var perfil = await _context.UsuariosPerfil
        .FirstOrDefaultAsync(p => p.UserId == user!.Id);

    ViewBag.EsFavorito = perfil != null &&
        await _context.Favoritos
            .AnyAsync(f => f.UsuarioPerfilId == perfil.Id &&
                           f.PrendaId == id);

    ViewBag.PerfilId = perfil?.Id;
    return View(prenda);
}
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CompletarPerfil(
            string nombre,
            int edad,
            double peso,
            double estatura,
            TonoPielEnum tonoPiel,
            double hombros,
            double busto,
            double cintura,
            double cadera)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return Challenge();

            // Calcular tipo de cuerpo con el servicio mejorado
            var tipoCuerpo = _bodyTypeService.DetectarTipo(
                hombros, busto, cintura, cadera);

            var perfil = await _context.UsuariosPerfil
                .Include(p => p.MedidasCorporales)
                .FirstOrDefaultAsync(p => p.UserId == user.Id);

            if (perfil == null)
            {
                // ─── CREAR ───
                perfil = new UsuarioPerfil
                {
                    UserId         = user.Id,
                    Nombre         = nombre,
                    Edad           = edad,
                    Peso           = peso,
                    Estatura       = estatura,
                    TonoPiel       = tonoPiel,
                    TipoCuerpo     = tipoCuerpo,
                    PerfilCompleto = true
                };
                _context.UsuariosPerfil.Add(perfil);
                await _context.SaveChangesAsync();

                _context.MedidasCorporales.Add(new MedidasCorporales
                {
                    Hombros         = hombros,
                    Busto           = busto,
                    Cintura         = cintura,
                    Cadera          = cadera,
                    UsuarioPerfilId = perfil.Id
                });
            }
            else
            {
                // ─── ACTUALIZAR ───
                perfil.Nombre         = nombre;
                perfil.Edad           = edad;
                perfil.Peso           = peso;
                perfil.Estatura       = estatura;
                perfil.TonoPiel       = tonoPiel;
                perfil.TipoCuerpo     = tipoCuerpo;
                perfil.PerfilCompleto = true;

                if (perfil.MedidasCorporales == null)
                {
                    _context.MedidasCorporales.Add(new MedidasCorporales
                    {
                        Hombros         = hombros,
                        Busto           = busto,
                        Cintura         = cintura,
                        Cadera          = cadera,
                        UsuarioPerfilId = perfil.Id
                    });
                }
                else
                {
                    perfil.MedidasCorporales.Hombros  = hombros;
                    perfil.MedidasCorporales.Busto    = busto;
                    perfil.MedidasCorporales.Cintura  = cintura;
                    perfil.MedidasCorporales.Cadera   = cadera;
                }
            }

            await _context.SaveChangesAsync();
            TempData["Success"] = $"¡Perfil guardado! Tu tipo de cuerpo es: {tipoCuerpo}";
            return RedirectToAction("Dashboard");
        }

        // ═══════════════════════════════════════════
        //  RECOMENDACIONES
        // ═══════════════════════════════════════════
public async Task<IActionResult> Recomendaciones(
    CategoriaRopa? categoria = null,
    decimal? precioMax = null)
{
    var user   = await _userManager.GetUserAsync(User);
    var perfil = await _context.UsuariosPerfil
        .FirstOrDefaultAsync(p => p.UserId == user!.Id);

    if (perfil == null || !perfil.PerfilCompleto)
        return RedirectToAction("CompletarPerfil");

    var tipoCuerpo = perfil.TipoCuerpo ?? "";
    var tonoPiel   = perfil.TonoPiel.ToString();

    // Usar el servicio de recomendaciones
    var recomendacionService = HttpContext.RequestServices
        .GetRequiredService<RecomendacionService>();
    var prendas = await recomendacionService.ObtenerParaPerfilAsync(perfil);

    // Filtros opcionales en memoria
    if (categoria.HasValue)
        prendas = prendas.Where(p => p.Categoria == categoria.Value).ToList();

    if (precioMax.HasValue)
        prendas = prendas.Where(p => p.Precio <= precioMax.Value).ToList();

    var favoritosIds = await _context.Favoritos
        .Where(f => f.UsuarioPerfilId == perfil.Id)
        .Select(f => f.PrendaId)
        .ToListAsync();

    ViewBag.TipoCuerpo     = tipoCuerpo;
    ViewBag.TonoPiel       = tonoPiel;
    ViewBag.FavoritosIds   = favoritosIds;
    ViewBag.CategoriaActual = categoria;
    ViewBag.PrecioMax      = precioMax;
    return View(prendas);
}

        // ═══════════════════════════════════════════
        //  FAVORITOS
        // ═══════════════════════════════════════════
        public async Task<IActionResult> Favoritos()
        {
            var user = await _userManager.GetUserAsync(User);
            var perfil = await _context.UsuariosPerfil
                .FirstOrDefaultAsync(p => p.UserId == user!.Id);

            if (perfil == null) return RedirectToAction("CompletarPerfil");

            var favoritos = await _context.Favoritos
                .Include(f => f.Prenda)
                    .ThenInclude(p => p.Tienda)
                .Where(f => f.UsuarioPerfilId == perfil.Id)
                .OrderByDescending(f => f.FechaAgregado)
                .ToListAsync();

            return View(favoritos);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AgregarFavorito(int prendaId)
        {
            var user = await _userManager.GetUserAsync(User);
            var perfil = await _context.UsuariosPerfil
                .FirstOrDefaultAsync(p => p.UserId == user!.Id);

            if (perfil == null) return RedirectToAction("CompletarPerfil");

            var existe = await _context.Favoritos.AnyAsync(f =>
                f.UsuarioPerfilId == perfil.Id && f.PrendaId == prendaId);

            if (!existe)
            {
                _context.Favoritos.Add(new Favorito
                {
                    UsuarioPerfilId = perfil.Id,
                    PrendaId        = prendaId
                });
                await _context.SaveChangesAsync();
                TempData["Success"] = "¡Agregado a favoritos!";
            }

            return RedirectToAction("Recomendaciones");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> QuitarFavorito(int favoritoId)
        {
            var favorito = await _context.Favoritos.FindAsync(favoritoId);
            if (favorito != null)
            {
                _context.Favoritos.Remove(favorito);
                await _context.SaveChangesAsync();
                TempData["Success"] = "Eliminado de favoritos.";
            }
            return RedirectToAction("Favoritos");
        }
        // ═══════════════════════════════════════════
//  RESERVAR PRENDA
// ═══════════════════════════════════════════
[HttpPost]
[ValidateAntiForgeryToken]
public async Task<IActionResult> ReservarPrenda(int prendaId, bool esPresencial)
{
    var user   = await _userManager.GetUserAsync(User);
    var perfil = await _context.UsuariosPerfil
        .FirstOrDefaultAsync(p => p.UserId == user!.Id);

    if (perfil == null) return RedirectToAction("CompletarPerfil");

    // ¿Ya tiene reserva activa para esta prenda?
    var yaReservada = await _context.Reservas
        .AnyAsync(r => r.UsuarioPerfilId == perfil.Id &&
                       r.PrendaId        == prendaId  &&
                       r.Estado          == EstadoReserva.EnProceso);

    if (yaReservada)
    {
        TempData["Error"] = "Ya tienes una reserva activa para esta prenda.";
        return RedirectToAction("DetallePrenda", new { id = prendaId });
    }

    var prenda = await _context.Prendas.FindAsync(prendaId);

    if (prenda == null || prenda.Stock <= 0)
    {
        TempData["Error"] = "Sin stock disponible en este momento.";
        return RedirectToAction("DetallePrenda", new { id = prendaId });
    }

    // Crear reserva
    _context.Reservas.Add(new Reserva
    {
        PrendaId        = prendaId,
        UsuarioPerfilId = perfil.Id,
        Estado          = EstadoReserva.EnProceso,
        EsPresencial    = esPresencial,
        FechaReserva    = DateTime.UtcNow
    });

    // Descontar stock
    prenda.Stock--;
    if (prenda.Stock == 0)
        prenda.Estado = EstadoPrenda.Agotado;

    await _context.SaveChangesAsync();
    TempData["Success"] = "¡Reserva creada! La tienda se pondrá en contacto contigo.";
    return RedirectToAction("MisReservas");
}

// ═══════════════════════════════════════════
//  MIS RESERVAS (cliente)
// ═══════════════════════════════════════════
public async Task<IActionResult> MisReservas()
{
    var user   = await _userManager.GetUserAsync(User);
    var perfil = await _context.UsuariosPerfil
        .FirstOrDefaultAsync(p => p.UserId == user!.Id);

    if (perfil == null) return RedirectToAction("CompletarPerfil");

    var reservas = await _context.Reservas
        .Include(r => r.Prenda)
            .ThenInclude(p => p.Tienda)
        .Where(r => r.UsuarioPerfilId == perfil.Id)
        .OrderByDescending(r => r.FechaReserva)
        .ToListAsync();

    return View(reservas);
}

// ═══════════════════════════════════════════
//  CANCELAR RESERVA (cliente)
// ═══════════════════════════════════════════
[HttpPost]
[ValidateAntiForgeryToken]
public async Task<IActionResult> CancelarReserva(int reservaId)
{
    var user   = await _userManager.GetUserAsync(User);
    var perfil = await _context.UsuariosPerfil
        .FirstOrDefaultAsync(p => p.UserId == user!.Id);

    if (perfil == null) return RedirectToAction("CompletarPerfil");

    var reserva = await _context.Reservas
        .Include(r => r.Prenda)
        .FirstOrDefaultAsync(r => r.Id == reservaId &&
                                   r.UsuarioPerfilId == perfil.Id);

    if (reserva == null) return NotFound();

    if (reserva.Estado == EstadoReserva.EnProceso)
    {
        reserva.Estado              = EstadoReserva.Cancelada;
        reserva.FechaActualizacion  = DateTime.UtcNow;

        // Devolver stock
        reserva.Prenda.Stock++;
        if (reserva.Prenda.Estado == EstadoPrenda.Agotado)
            reserva.Prenda.Estado = EstadoPrenda.Disponible;

        await _context.SaveChangesAsync();
        TempData["Success"] = "Reserva cancelada. El stock fue devuelto.";
    }

    return RedirectToAction("MisReservas");
}
    }
}