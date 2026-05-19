namespace Lyra.Models
{
    public class Favorito
    {
        public int Id { get; set; }

        public int UsuarioPerfilId { get; set; }
        public UsuarioPerfil UsuarioPerfil { get; set; } = null!;

        public int PrendaId { get; set; }
        public Prenda Prenda { get; set; } = null!;

        public DateTime FechaAgregado { get; set; } = DateTime.UtcNow;
    }
}