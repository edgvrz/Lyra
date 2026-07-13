namespace Lyra.Models
{
    public class CarritoItem
    {
        public int    PrendaId     { get; set; }
        public string Nombre       { get; set; } = string.Empty;
        public decimal Precio      { get; set; }
        public int    Cantidad     { get; set; } = 1;
        public string? ImagenUrl   { get; set; }
        public string? TiendaNombre { get; set; }
        public string? Talla       { get; set; }
        public decimal Subtotal    => Precio * Cantidad;
    }
}