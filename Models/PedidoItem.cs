namespace Lyra.Models
{
    public class PedidoItem
    {
        public int Id { get; set; }

        public int PedidoId { get; set; }
        public Pedido Pedido { get; set; } = null!;

        public int PrendaId { get; set; }
        public Prenda Prenda { get; set; } = null!;

        // Snapshot al momento de comprar
        public string  NombrePrenda    { get; set; } = string.Empty;
        public decimal PrecioUnitario  { get; set; }
        public int     Cantidad        { get; set; }
        public decimal Subtotal        { get; set; }
        public string? TiendaNombre    { get; set; }
        public string? Talla           { get; set; }
    }
}