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
    [Authorize(Roles = UserRoles.Tienda)]
    public class TiendaController : Controller
    {
        private readonly ApplicationDbContext      _context;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly ImagenService             _imagenService;

        public TiendaController(
            ApplicationDbContext context,
            UserManager<IdentityUser> userManager,
            ImagenService imagenService)
        {
            _context       = context;
            _userManager   = userManager;
            _imagenService = imagenService;
        }

        private async Task<Tienda?> GetMiTiendaAsync()
        {
            var user = await _userManager.GetUserAsync(User);
            return await _context.Tiendas
                .FirstOrDefaultAsync(t => t.UserId == user!.Id);
        }

        private async Task<IActionResult?> VerificarAprobacion()
        {
            var tienda = await GetMiTiendaAsync();
            if (tienda == null)
                return RedirectToAction("ConfigurarTienda");
            if (tienda.EstadoAprobacion == EstadoAprobacion.Pendiente)
                return View("Pendiente", tienda);
            if (tienda.EstadoAprobacion == EstadoAprobacion.Rechazada)
                return View("Rechazada", tienda);
            return null;
        }

        // ═══════════════════════════════════════════
        //  DASHBOARD
        // ═══════════════════════════════════════════
        public async Task<IActionResult> Dashboard()
        {
            var bloqueo = await VerificarAprobacion();
            if (bloqueo != null) return bloqueo;

            var tienda = await GetMiTiendaAsync();

            ViewBag.TotalPrendas    = await _context.Prendas
                .CountAsync(p => p.TiendaId == tienda!.Id);
            ViewBag.Disponibles     = await _context.Prendas
                .CountAsync(p => p.TiendaId == tienda!.Id && p.Estado == EstadoPrenda.Disponible);
            ViewBag.TotalFavoritos  = await _context.Favoritos
                .CountAsync(f => f.Prenda.TiendaId == tienda!.Id);
            ViewBag.ReservasActivas = await _context.Reservas
                .CountAsync(r => r.Prenda.TiendaId == tienda!.Id &&
                                 r.Estado == EstadoReserva.EnProceso);

            return View(tienda);
        }

        // ═══════════════════════════════════════════
        //  CONFIGURAR TIENDA
        // ═══════════════════════════════════════════
        [HttpGet]
        public async Task<IActionResult> ConfigurarTienda()
        {
            var tienda = await GetMiTiendaAsync();
            return View(tienda ?? new Tienda());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ConfigurarTienda(
            string nombre, string? descripcion,
            string? ciudad, string? telefono, string? sitioWeb)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return Challenge();

            var tienda = await GetMiTiendaAsync();

            if (tienda == null)
            {
                tienda = new Tienda
                {
                    UserId           = user.Id,
                    Nombre           = nombre,
                    Descripcion      = descripcion,
                    Ciudad           = ciudad,
                    Telefono         = telefono,
                    SitioWeb         = sitioWeb,
                    Activa           = true,
                    EstadoAprobacion = EstadoAprobacion.Pendiente
                };
                _context.Tiendas.Add(tienda);
                await _context.SaveChangesAsync();
                return View("Pendiente", tienda);
            }
            else
            {
                tienda.Nombre       = nombre;
                tienda.Descripcion  = descripcion;
                tienda.Ciudad       = ciudad;
                tienda.Telefono     = telefono;
                tienda.SitioWeb     = sitioWeb;
                await _context.SaveChangesAsync();
                TempData["Success"] = "Tienda actualizada.";
                return RedirectToAction("Dashboard");
            }
        }

        // ═══════════════════════════════════════════
        //  MIS PRENDAS
        // ═══════════════════════════════════════════
        public async Task<IActionResult> MisPrendas()
        {
            var bloqueo = await VerificarAprobacion();
            if (bloqueo != null) return bloqueo;

            var tienda  = await GetMiTiendaAsync();
            var prendas = await _context.Prendas
                .Where(p => p.TiendaId == tienda!.Id)
                .OrderByDescending(p => p.FechaPublicacion)
                .ToListAsync();

            return View(prendas);
        }

        // ═══════════════════════════════════════════
        //  CREAR PRENDA — con stock + guía visual
        // ═══════════════════════════════════════════
        [HttpGet]
        public async Task<IActionResult> CrearPrenda()
        {
            var bloqueo = await VerificarAprobacion();
            if (bloqueo != null) return bloqueo;
            return View(new Prenda());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CrearPrenda(
            string nombre, string? descripcion,
            CategoriaRopa categoria, string color,
            decimal precio, TallaPrenda talla,
            int stock,
            IFormFile? imagenFile,
            List<string> tiposCuerpo,
            List<string> tonosPiel)
        {
            var bloqueo = await VerificarAprobacion();
            if (bloqueo != null) return bloqueo;

            var tienda = await GetMiTiendaAsync();
            if (tienda == null) return RedirectToAction("ConfigurarTienda");

            var imagenUrl = await _imagenService.GuardarPrendaAsync(imagenFile);

            _context.Prendas.Add(new Prenda
            {
                Nombre                 = nombre,
                Descripcion            = descripcion,
                Categoria              = categoria,
                Color                  = color,
                Precio                 = precio,
                Talla                  = talla,
                Stock                  = stock > 0 ? stock : 0,
                ImagenUrl              = imagenUrl,
                TiposCuerpoCompatibles = string.Join(",", tiposCuerpo),
                TonosPielCompatibles   = string.Join(",", tonosPiel),
                Estado                 = stock > 0 ? EstadoPrenda.Disponible : EstadoPrenda.Agotado,
                TiendaId               = tienda.Id
            });

            await _context.SaveChangesAsync();
            TempData["Success"] = "Prenda publicada.";
            return RedirectToAction("MisPrendas");
        }

        // ═══════════════════════════════════════════
        //  EDITAR PRENDA
        // ═══════════════════════════════════════════
        [HttpGet]
        public async Task<IActionResult> EditarPrenda(int id)
        {
            var bloqueo = await VerificarAprobacion();
            if (bloqueo != null) return bloqueo;

            var tienda = await GetMiTiendaAsync();
            var prenda = await _context.Prendas
                .FirstOrDefaultAsync(p => p.Id == id && p.TiendaId == tienda!.Id);

            return prenda == null ? NotFound() : View(prenda);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditarPrenda(
            int id, string nombre, string? descripcion,
            CategoriaRopa categoria, string color,
            decimal precio, TallaPrenda talla,
            int stock,
            IFormFile? imagenFile,
            EstadoPrenda estado,
            List<string> tiposCuerpo,
            List<string> tonosPiel)
        {
            var tienda = await GetMiTiendaAsync();
            var prenda = await _context.Prendas
                .FirstOrDefaultAsync(p => p.Id == id && p.TiendaId == tienda!.Id);

            if (prenda == null) return NotFound();

            if (imagenFile != null && imagenFile.Length > 0)
            {
                _imagenService.EliminarImagen(prenda.ImagenUrl);
                prenda.ImagenUrl = await _imagenService.GuardarPrendaAsync(imagenFile);
            }

            prenda.Nombre                 = nombre;
            prenda.Descripcion            = descripcion;
            prenda.Categoria              = categoria;
            prenda.Color                  = color;
            prenda.Precio                 = precio;
            prenda.Talla                  = talla;
            prenda.Stock                  = stock >= 0 ? stock : 0;
            prenda.Estado                 = stock > 0 ? estado : EstadoPrenda.Agotado;
            prenda.TiposCuerpoCompatibles = string.Join(",", tiposCuerpo);
            prenda.TonosPielCompatibles   = string.Join(",", tonosPiel);

            await _context.SaveChangesAsync();
            TempData["Success"] = "Prenda actualizada.";
            return RedirectToAction("MisPrendas");
        }

        // ═══════════════════════════════════════════
        //  MIS RESERVAS — gestión desde tienda
        // ═══════════════════════════════════════════
        public async Task<IActionResult> MisReservas()
        {
            var bloqueo = await VerificarAprobacion();
            if (bloqueo != null) return bloqueo;

            var tienda = await GetMiTiendaAsync();
            var reservas = await _context.Reservas
                .Include(r => r.Prenda)
                .Include(r => r.UsuarioPerfil)
                .Where(r => r.Prenda.TiendaId == tienda!.Id)
                .OrderByDescending(r => r.FechaReserva)
                .ToListAsync();

            return View(reservas);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> GestionarReserva(int id, EstadoReserva nuevoEstado)
        {
            var tienda = await GetMiTiendaAsync();
            var reserva = await _context.Reservas
                .Include(r => r.Prenda)
                .FirstOrDefaultAsync(r => r.Id == id &&
                                          r.Prenda.TiendaId == tienda!.Id);

            if (reserva == null) return NotFound();

            var estadoAnterior = reserva.Estado;
            reserva.Estado              = nuevoEstado;
            reserva.FechaActualizacion  = DateTime.UtcNow;

            // Si se cancela desde la tienda → devolver stock
            if (nuevoEstado == EstadoReserva.Cancelada &&
                estadoAnterior == EstadoReserva.EnProceso)
            {
                reserva.Prenda.Stock++;
                if (reserva.Prenda.Estado == EstadoPrenda.Agotado)
                    reserva.Prenda.Estado = EstadoPrenda.Disponible;
            }

            await _context.SaveChangesAsync();

            TempData["Success"] = nuevoEstado == EstadoReserva.Finalizada
                ? "✓ Entrega marcada como finalizada."
                : "Reserva cancelada. Stock actualizado.";

            return RedirectToAction("MisReservas");
        }

        // ═══════════════════════════════════════════
        //  ACTUALIZAR STOCK RÁPIDO
        // ═══════════════════════════════════════════
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ActualizarStock(int id, int stock)
        {
            var tienda = await GetMiTiendaAsync();
            var prenda = await _context.Prendas
                .FirstOrDefaultAsync(p => p.Id == id && p.TiendaId == tienda!.Id);

            if (prenda != null)
            {
                prenda.Stock  = stock >= 0 ? stock : 0;
                prenda.Estado = prenda.Stock > 0
                    ? EstadoPrenda.Disponible
                    : EstadoPrenda.Agotado;
                await _context.SaveChangesAsync();
                TempData["Success"] = "Stock actualizado.";
            }

            return RedirectToAction("MisPrendas");
        }

        // ═══════════════════════════════════════════
        //  ELIMINAR PRENDA
        // ═══════════════════════════════════════════
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EliminarPrenda(int id)
        {
            var tienda = await GetMiTiendaAsync();
            var prenda = await _context.Prendas
                .FirstOrDefaultAsync(p => p.Id == id && p.TiendaId == tienda!.Id);

            if (prenda != null)
            {
                // Cancelar reservas activas antes de eliminar
                var reservasActivas = await _context.Reservas
                    .Where(r => r.PrendaId == id && r.Estado == EstadoReserva.EnProceso)
                    .ToListAsync();

                foreach (var r in reservasActivas)
                {
                    r.Estado = EstadoReserva.Cancelada;
                    r.FechaActualizacion = DateTime.UtcNow;
                }

                _imagenService.EliminarImagen(prenda.ImagenUrl);
                _context.Prendas.Remove(prenda);
                await _context.SaveChangesAsync();
                TempData["Success"] = "Prenda eliminada.";
            }

            return RedirectToAction("MisPrendas");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CambiarEstado(int id, EstadoPrenda estado)
        {
            var tienda = await GetMiTiendaAsync();
            var prenda = await _context.Prendas
                .FirstOrDefaultAsync(p => p.Id == id && p.TiendaId == tienda!.Id);

            if (prenda != null)
            {
                prenda.Estado = estado;
                await _context.SaveChangesAsync();
            }

            return RedirectToAction("MisPrendas");
        }
    }
}