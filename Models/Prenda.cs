using System.ComponentModel.DataAnnotations;
using Lyra.Enums;

namespace Lyra.Models
{
    public class Prenda
    {
        public int Id { get; set; }

        [Required]
        public string Nombre { get; set; } = string.Empty;

        public string? Descripcion { get; set; }

        [Required]
        public CategoriaRopa Categoria { get; set; }

        [Required]
        public string Color { get; set; } = string.Empty;

        [Range(0, 99999)]
        public decimal Precio { get; set; }

        public TallaPrenda Talla { get; set; }

        public EstadoPrenda Estado { get; set; } = EstadoPrenda.Disponible;

        public string? ImagenUrl { get; set; }

        public DateTime FechaPublicacion { get; set; } = DateTime.UtcNow;

        // Relaciones con tipos de cuerpo y tono de piel (almacenados como lista separada por comas)
        // Ejemplo: "Reloj de Arena,Triángulo"
        public string TiposCuerpoCompatibles { get; set; } = string.Empty;
        public string TonosPielCompatibles { get; set; } = string.Empty;

        public int? TiendaId { get; set; }
        public Tienda? Tienda { get; set; }

        // Navegación
        public ICollection<Favorito> Favoritos { get; set; } = new List<Favorito>();
    }
}