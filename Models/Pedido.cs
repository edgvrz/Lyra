using System.ComponentModel.DataAnnotations;
using Lyra.Enums;

namespace Lyra.Models
{
    public class Pedido
    {
        public int Id { get; set; }

        [Required]
        public string NumeroPedido { get; set; } = string.Empty;

        public int UsuarioPerfilId { get; set; }
        public UsuarioPerfil UsuarioPerfil { get; set; } = null!;

        public EstadoPedido Estado { get; set; } = EstadoPedido.Confirmado;

        // Simulación de pago
        public string MetodoPago  { get; set; } = string.Empty;
        public string? Notas      { get; set; }

        public decimal Total      { get; set; }
        public DateTime FechaPedido { get; set; } = DateTime.UtcNow;

        public ICollection<PedidoItem> Items { get; set; } = new List<PedidoItem>();
    }
}