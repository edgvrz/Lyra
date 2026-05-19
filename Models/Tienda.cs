using System.ComponentModel.DataAnnotations;
using Lyra.Enums;

namespace Lyra.Models
{
    public class Tienda
    {
        public int Id { get; set; }

        [Required]
        public string Nombre      { get; set; } = string.Empty;
        public string? Descripcion { get; set; }
        public string? LogoUrl     { get; set; }
        public string? Ciudad      { get; set; }
        public string? Telefono    { get; set; }
        public string? SitioWeb    { get; set; }

        [Required]
        public string UserId { get; set; } = string.Empty;

        public bool Activa { get; set; } = true;

        // ─── NUEVO ───
        public EstadoAprobacion EstadoAprobacion { get; set; } = EstadoAprobacion.Pendiente;
        public string? MotivoRechazo { get; set; }
        // ─────────────

        public DateTime FechaRegistro { get; set; } = DateTime.UtcNow;

        public ICollection<Prenda> Prendas { get; set; } = new List<Prenda>();
    }
}