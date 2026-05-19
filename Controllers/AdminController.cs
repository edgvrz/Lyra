using Lyra.Data;
using Lyra.Enums;
using Lyra.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Lyra.DTOs;

namespace Lyra.Controllers
{
    [Authorize(Roles = UserRoles.Admin)]
    public class AdminController : Controller
    {
        private readonly ApplicationDbContext      _context;
        private readonly UserManager<IdentityUser> _userManager;

        public AdminController(
            ApplicationDbContext context,
            UserManager<IdentityUser> userManager)
        {
            _context     = context;
            _userManager = userManager;
        }

        // ═══════════════════════════════════════════
        //  DASHBOARD — estadísticas globales
        // ═══════════════════════════════════════════
        public async Task<IActionResult> Dashboard()
        {
            ViewBag.TotalClientes  = (await _userManager.GetUsersInRoleAsync(UserRoles.Cliente)).Count;
            ViewBag.TotalTiendas   = (await _userManager.GetUsersInRoleAsync(UserRoles.Tienda)).Count;
            ViewBag.TotalPrendas   = await _context.Prendas.CountAsync();
            ViewBag.TotalFavoritos = await _context.Favoritos.CountAsync();
            return View();
        }

        // ═══════════════════════════════════════════
        //  USUARIOS
        // ═══════════════════════════════════════════
        public async Task<IActionResult> Usuarios()
        {
            var usuarios = _userManager.Users.ToList();
            var lista = new List<UsuarioAdminDto>();

            foreach (var u in usuarios)
            {
                var roles  = await _userManager.GetRolesAsync(u);
                var perfil = await _context.UsuariosPerfil
                    .FirstOrDefaultAsync(p => p.UserId == u.Id);

                lista.Add(new UsuarioAdminDto
{
    Id             = u.Id,
    Email          = u.Email ?? "",
    Rol            = roles.FirstOrDefault() ?? "Sin rol",
    Nombre         = perfil?.Nombre     ?? "—",
    TipoCuerpo     = perfil?.TipoCuerpo ?? "—",
    PerfilCompleto = perfil?.PerfilCompleto ?? false
});
ViewBag.Usuarios = lista;
            }

            ViewBag.Usuarios = lista;
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EliminarUsuario(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null) return NotFound();

            // Borrar perfil y medidas primero
            var perfil = await _context.UsuariosPerfil
                .Include(p => p.MedidasCorporales)
                .FirstOrDefaultAsync(p => p.UserId == userId);

            if (perfil != null) _context.UsuariosPerfil.Remove(perfil);
            await _context.SaveChangesAsync();
            await _userManager.DeleteAsync(user);

            TempData["Success"] = "Usuario eliminado.";
            return RedirectToAction("Usuarios");
        }

        // ═══════════════════════════════════════════
        //  TIENDAS
        // ═══════════════════════════════════════════
        public async Task<IActionResult> Tiendas()
        {
            var tiendas = await _context.Tiendas
                .Include(t => t.Prendas)
                .OrderByDescending(t => t.FechaRegistro)
                .ToListAsync();
            return View(tiendas);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ToggleTienda(int id)
        {
            var tienda = await _context.Tiendas.FindAsync(id);
            if (tienda != null)
            {
                tienda.Activa = !tienda.Activa;
                await _context.SaveChangesAsync();
                TempData["Success"] = tienda.Activa ? "Tienda activada." : "Tienda desactivada.";
            }
            return RedirectToAction("Tiendas");
        }

        // ═══════════════════════════════════════════
        //  PRENDAS (vista global)
        // ═══════════════════════════════════════════
        public async Task<IActionResult> Prendas()
        {
            var prendas = await _context.Prendas
                .Include(p => p.Tienda)
                .OrderByDescending(p => p.FechaPublicacion)
                .ToListAsync();
            return View(prendas);
        }

        [HttpGet]
        public IActionResult CrearPrenda() => View(new Prenda());

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CrearPrenda(
            string nombre,
            string? descripcion,
            CategoriaRopa categoria,
            string color,
            decimal precio,
            TallaPrenda talla,
            string? imagenUrl,
            List<string> tiposCuerpo,
            List<string> tonosPiel)
        {
            _context.Prendas.Add(new Prenda
            {
                Nombre                 = nombre,
                Descripcion            = descripcion,
                Categoria              = categoria,
                Color                  = color,
                Precio                 = precio,
                Talla                  = talla,
                ImagenUrl              = imagenUrl,
                TiposCuerpoCompatibles = string.Join(",", tiposCuerpo),
                TonosPielCompatibles   = string.Join(",", tonosPiel),
                Estado                 = EstadoPrenda.Disponible
            });

            await _context.SaveChangesAsync();
            TempData["Success"] = "Prenda creada.";
            return RedirectToAction("Prendas");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EliminarPrenda(int id)
        {
            var prenda = await _context.Prendas.FindAsync(id);
            if (prenda != null)
            {
                _context.Prendas.Remove(prenda);
                await _context.SaveChangesAsync();
                TempData["Success"] = "Prenda eliminada.";
            }
            return RedirectToAction("Prendas");
        }
        // ═══════════════════════════════════════════
//  APROBAR / RECHAZAR TIENDA
// ═══════════════════════════════════════════
[HttpPost]
[ValidateAntiForgeryToken]
public async Task<IActionResult> AprobarTienda(int id)
{
    var tienda = await _context.Tiendas.FindAsync(id);
    if (tienda != null)
    {
        tienda.EstadoAprobacion = EstadoAprobacion.Aprobada;
        tienda.MotivoRechazo    = null;
        await _context.SaveChangesAsync();
        TempData["Success"] = $"Tienda '{tienda.Nombre}' aprobada.";
    }
    return RedirectToAction("Tiendas");
}

[HttpPost]
[ValidateAntiForgeryToken]
public async Task<IActionResult> RechazarTienda(int id, string? motivo)
{
    var tienda = await _context.Tiendas.FindAsync(id);
    if (tienda != null)
    {
        tienda.EstadoAprobacion = EstadoAprobacion.Rechazada;
        tienda.MotivoRechazo    = motivo ?? "No cumple los requisitos.";
        await _context.SaveChangesAsync();
        TempData["Success"] = $"Tienda '{tienda.Nombre}' rechazada.";
    }
    return RedirectToAction("Tiendas");
}
    }
}