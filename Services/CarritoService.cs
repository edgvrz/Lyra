                                                                                using System.Text.Json;
using Lyra.Models;
using Microsoft.AspNetCore.Http;

namespace Lyra.Services
{
    public class CarritoService
    {
        private const string SESSION_KEY = "precisa_carrito";
        private readonly IHttpContextAccessor _http;

        public CarritoService(IHttpContextAccessor http) => _http = http;

        private HttpContext Ctx => _http.HttpContext!;

        public List<CarritoItem> ObtenerCarrito()
        {
            var json = Ctx.Session.GetString(SESSION_KEY);
            return string.IsNullOrEmpty(json)
                ? new List<CarritoItem>()
                : JsonSerializer.Deserialize<List<CarritoItem>>(json)!;
        }

        private void Guardar(List<CarritoItem> items)
        {
            Ctx.Session.SetString(SESSION_KEY,
                JsonSerializer.Serialize(items));
        }

        public void Agregar(CarritoItem nuevo)
        {
            var carrito = ObtenerCarrito();
            var existente = carrito.FirstOrDefault(i => i.PrendaId == nuevo.PrendaId);
            if (existente != null)
                existente.Cantidad++;
            else
                carrito.Add(nuevo);
            Guardar(carrito);
        }

        public void ActualizarCantidad(int prendaId, int cantidad)
        {
            var carrito = ObtenerCarrito();
            var item = carrito.FirstOrDefault(i => i.PrendaId == prendaId);
            if (item != null)
            {
                if (cantidad <= 0) carrito.Remove(item);
                else item.Cantidad = cantidad;
            }
            Guardar(carrito);
        }

        public void Quitar(int prendaId)
        {
            var carrito = ObtenerCarrito();
            carrito.RemoveAll(i => i.PrendaId == prendaId);
            Guardar(carrito);
        }

        public void Limpiar() => Ctx.Session.Remove(SESSION_KEY);

        public int ContarItems() => ObtenerCarrito().Sum(i => i.Cantidad);

        public decimal CalcularTotal() => ObtenerCarrito().Sum(i => i.Subtotal);
    }
}