using System.ComponentModel.DataAnnotations;

namespace Lyra.Models
{
    public class MedidasCorporales
    {
        public int Id { get; set; }

        // TODAS en centímetros — circunferencias, no anchos
        [Range(50, 200)]
        public double Hombros { get; set; }  // Circunf. a la altura de escápulas

        [Range(50, 200)]
        public double Busto { get; set; }    // Circunf. en parte más voluminosa

        [Range(40, 180)]
        public double Cintura { get; set; }  // Circunf. parte más estrecha

        [Range(50, 200)]
        public double Cadera { get; set; }   // Circunf. parte más ancha (glúteos)

        public int UsuarioPerfilId { get; set; }
        public UsuarioPerfil UsuarioPerfil { get; set; } = null!;
    }
}