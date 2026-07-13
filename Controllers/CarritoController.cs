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
    public class CarritoController : Controller
    {
        private readonly ApplicationDbContext      _context;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly CarritoService            _carrito;

        public CarritoController(
            ApplicationDbContext context,
            UserManager<IdentityUser> userManager,
            CarritoService carrito)
        {
            _context     = context;
            _userManager = userManager;
            _carrito     = carrito;
        }

        private async Task<UsuarioPerfil?> GetPerfilAsync()
        {
            var user = await _userManager.GetUserAsync(User);
            return await _context.UsuariosPerfil
                .FirstOrDefaultAsync(p => p.UserId == user!.Id);
        }

        // ═══════════════════════════════════════════
        //  VER CARRITO
        // ═══════════════════════════════════════════
        public IActionResult Index()
        {
            var items = _carrito.ObtenerCarrito();
            ViewBag.Total = _carrito.CalcularTotal();
            return View(items);
        }

        // ═══════════════════════════════════════════
        //  AGREGAR AL CARRITO
        // ═══════════════════════════════════════════
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Agregar(int prendaId)
        {
            var prenda = await _context.Prendas
                .Include(p => p.Tienda)
                .FirstOrDefaultAsync(p => p.Id == prendaId &&
                                          p.Estado == EstadoPrenda.Disponible);

            if (prenda == null)
            {
                TempData["Error"] = "Prenda no disponible.";
                return RedirectToAction("Recomendaciones", "Cliente");
            }

            if (prenda.Stock <= 0)
            {
                TempData["Error"] = "Sin stock disponible.";
                return RedirectToAction("DetallePrenda", "Cliente",
                    new { id = prendaId });
            }

            _carrito.Agregar(new CarritoItem
            {
                PrendaId     = prenda.Id,
                Nombre       = prenda.Nombre,
                Precio       = prenda.Precio,
                ImagenUrl    = prenda.ImagenUrl,
                TiendaNombre = prenda.Tienda?.Nombre,
                Talla        = prenda.Talla.ToString(),
                Cantidad     = 1
            });

            TempData["Success"] = $"'{prenda.Nombre}' agregada al carrito.";
            return RedirectToAction("Index");
        }

        // ═══════════════════════════════════════════
        //  ACTUALIZAR CANTIDAD
        // ═══════════════════════════════════════════
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult ActualizarCantidad(int prendaId, int cantidad)
        {
            _carrito.ActualizarCantidad(prendaId, cantidad);
            return RedirectToAction("Index");
        }

        // ═══════════════════════════════════════════
        //  QUITAR ITEM
        // ═══════════════════════════════════════════
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Quitar(int prendaId)
        {
            _carrito.Quitar(prendaId);
            TempData["Success"] = "Prenda eliminada del carrito.";
            return RedirectToAction("Index");
        }

        // ═══════════════════════════════════════════
        //  CHECKOUT
        // ═══════════════════════════════════════════
        [HttpGet]
        public IActionResult Checkout()
        {
            var items = _carrito.ObtenerCarrito();
            if (!items.Any())
                return RedirectToAction("Index");

            ViewBag.Items = items;
            ViewBag.Total = _carrito.CalcularTotal();
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Confirmar(
            string metodoPago, string? notas)
        {
            var items = _carrito.ObtenerCarrito();
            if (!items.Any())
                return RedirectToAction("Index");

            var perfil = await GetPerfilAsync();
            if (perfil == null)
                return RedirectToAction("CompletarPerfil", "Cliente");

            // Verificar stock y descontar
            foreach (var item in items)
            {
                var prenda = await _context.Prendas.FindAsync(item.PrendaId);
                if (prenda == null || prenda.Stock < item.Cantidad)
                {
                    TempData["Error"] =
                        $"Sin stock suficiente para '{item.Nombre}'. " +
                        "Revisa tu carrito.";
                    return RedirectToAction("Index");
                }

                prenda.Stock -= item.Cantidad;
                if (prenda.Stock == 0)
                    prenda.Estado = EstadoPrenda.Agotado;
            }

            // Crear número de pedido único
            var numero = $"PRC-{DateTime.Now:yyyyMMdd}-" +
                         $"{new Random().Next(1000, 9999)}";

            var pedido = new Pedido
            {
                NumeroPedido    = numero,
                UsuarioPerfilId = perfil.Id,
                MetodoPago      = metodoPago,
                Notas           = notas,
                Total           = _carrito.CalcularTotal(),
                Estado          = EstadoPedido.Confirmado,
                FechaPedido     = DateTime.UtcNow,
                Items           = items.Select(i => new PedidoItem
                {
                    PrendaId       = i.PrendaId,
                    NombrePrenda   = i.Nombre,
                    PrecioUnitario = i.Precio,
                    Cantidad       = i.Cantidad,
                    Subtotal       = i.Subtotal,
                    TiendaNombre   = i.TiendaNombre,
                    Talla          = i.Talla
                }).ToList()
            };

            _context.Pedidos.Add(pedido);
            await _context.SaveChangesAsync();

            // Limpiar carrito
            _carrito.Limpiar();

            return RedirectToAction("Confirmacion", new { id = pedido.Id });
        }

        // ═══════════════════════════════════════════
        //  CONFIRMACIÓN
        // ═══════════════════════════════════════════
        public async Task<IActionResult> Confirmacion(int id)
        {
            var perfil = await GetPerfilAsync();
            var pedido = await _context.Pedidos
                .Include(p => p.Items)
                .FirstOrDefaultAsync(p => p.Id == id &&
                                          p.UsuarioPerfilId == perfil!.Id);

            return pedido == null ? NotFound() : View(pedido);
        }

        // ═══════════════════════════════════════════
        //  MIS PEDIDOS
        // ═══════════════════════════════════════════
        public async Task<IActionResult> MisPedidos()
        {
            var perfil = await GetPerfilAsync();
            if (perfil == null)
                return RedirectToAction("CompletarPerfil", "Cliente");

            var pedidos = await _context.Pedidos
                .Include(p => p.Items)
                .Where(p => p.UsuarioPerfilId == perfil.Id)
                .OrderByDescending(p => p.FechaPedido)
                .ToListAsync();

            return View(pedidos);
        }
    }
}