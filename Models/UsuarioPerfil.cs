using System.ComponentModel.DataAnnotations;
using Lyra.Enums;

namespace Lyra.Models
{
    public class UsuarioPerfil
    {
        public int Id { get; set; }

        [Required]
        public string UserId { get; set; } = string.Empty;

        [Required]
        public string Nombre { get; set; } = string.Empty;

        public int Edad { get; set; }
        public double Peso { get; set; }      // kg
        public double Estatura { get; set; }  // cm

        public TonoPielEnum TonoPiel { get; set; }

        // Calculado automáticamente por BodyTypeService
        public string? TipoCuerpo { get; set; }

        // Si ya completó el perfil con medidas
        public bool PerfilCompleto { get; set; } = false;

        public DateTime FechaRegistro { get; set; } = DateTime.UtcNow;

        // Navegación
        public MedidasCorporales? MedidasCorporales { get; set; }
        public ICollection<Favorito> Favoritos { get; set; } = new List<Favorito>();
    }
}