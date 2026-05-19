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

        // ─── Helper: obtener tienda del usuario logueado ───
        private async Task<Tienda?> GetMiTiendaAsync()
        {
            var user = await _userManager.GetUserAsync(User);
            return await _context.Tiendas
                .FirstOrDefaultAsync(t => t.UserId == user!.Id);
        }

        // ─── Helper: verificar aprobación antes de cada acción ───
        private async Task<IActionResult?> VerificarAprobacion()
        {
            var tienda = await GetMiTiendaAsync();

            if (tienda == null)
                return RedirectToAction("ConfigurarTienda");

            if (tienda.EstadoAprobacion == EstadoAprobacion.Pendiente)
                return View("Pendiente", tienda);

            if (tienda.EstadoAprobacion == EstadoAprobacion.Rechazada)
                return View("Rechazada", tienda);

            return null; // Aprobada → continuar
        }

        // ═══════════════════════════════════════════
        //  DASHBOARD
        // ═══════════════════════════════════════════
        public async Task<IActionResult> Dashboard()
        {
            var bloqueo = await VerificarAprobacion();
            if (bloqueo != null) return bloqueo;

            var tienda = await GetMiTiendaAsync();

            ViewBag.TotalPrendas   = await _context.Prendas
                .CountAsync(p => p.TiendaId == tienda!.Id);
            ViewBag.Disponibles    = await _context.Prendas
                .CountAsync(p => p.TiendaId == tienda!.Id && p.Estado == EstadoPrenda.Disponible);
            ViewBag.TotalFavoritos = await _context.Favoritos
                .CountAsync(f => f.Prenda.TiendaId == tienda!.Id);

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
                // Primera vez → crear con estado Pendiente
                tienda = new Tienda
                {
                    UserId            = user.Id,
                    Nombre            = nombre,
                    Descripcion       = descripcion,
                    Ciudad            = ciudad,
                    Telefono          = telefono,
                    SitioWeb          = sitioWeb,
                    Activa            = true,
                    EstadoAprobacion  = EstadoAprobacion.Pendiente
                };
                _context.Tiendas.Add(tienda);
                await _context.SaveChangesAsync();

                // Redirigir a vista de espera
                return View("Pendiente", tienda);
            }
            else
            {
                // Actualizar datos (solo si ya está aprobada)
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
        //  CREAR PRENDA — con subida de imagen
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
            IFormFile? imagenFile,
            List<string> tiposCuerpo, List<string> tonosPiel)
        {
            var bloqueo = await VerificarAprobacion();
            if (bloqueo != null) return bloqueo;

            var tienda = await GetMiTiendaAsync();
            if (tienda == null) return RedirectToAction("ConfigurarTienda");

            // Guardar imagen
            var imagenUrl = await _imagenService.GuardarPrendaAsync(imagenFile);

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
                Estado                 = EstadoPrenda.Disponible,
                TiendaId               = tienda.Id
            });

            await _context.SaveChangesAsync();
            TempData["Success"] = "Prenda publicada.";
            return RedirectToAction("MisPrendas");
        }

        // ═══════════════════════════════════════════
        //  EDITAR PRENDA — con subida de imagen
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
            IFormFile? imagenFile,
            EstadoPrenda estado,
            List<string> tiposCuerpo, List<string> tonosPiel)
        {
            var tienda = await GetMiTiendaAsync();
            var prenda = await _context.Prendas
                .FirstOrDefaultAsync(p => p.Id == id && p.TiendaId == tienda!.Id);

            if (prenda == null) return NotFound();

            // Si subió nueva imagen, guardar y borrar la anterior
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
            prenda.Estado                 = estado;
            prenda.TiposCuerpoCompatibles = string.Join(",", tiposCuerpo);
            prenda.TonosPielCompatibles   = string.Join(",", tonosPiel);

            await _context.SaveChangesAsync();
            TempData["Success"] = "Prenda actualizada.";
            return RedirectToAction("MisPrendas");
        }

        // ═══════════════════════════════════════════
        //  ELIMINAR / CAMBIAR ESTADO
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