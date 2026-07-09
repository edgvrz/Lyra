using System.ComponentModel.DataAnnotations;
using Lyra.Enums;

namespace Lyra.Models
{
    public class Reserva
    {
        public int Id { get; set; }

        public int PrendaId { get; set; }
        public Prenda Prenda { get; set; } = null!;

        public int UsuarioPerfilId { get; set; }
        public UsuarioPerfil UsuarioPerfil { get; set; } = null!;

        public EstadoReserva Estado { get; set; } = EstadoReserva.EnProceso;

        // true = cliente va a recoger presencialmente
        // false = cliente solicitó envío / contacto por otro medio
        public bool EsPresencial { get; set; } = true;

        public string? NotasCliente { get; set; }

        public DateTime FechaReserva       { get; set; } = DateTime.UtcNow;
        public DateTime? FechaActualizacion { get; set; }
    }
}