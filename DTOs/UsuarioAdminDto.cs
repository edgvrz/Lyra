namespace Lyra.DTOs
{
    public class UsuarioAdminDto
    {
        public string Id            { get; set; } = string.Empty;
        public string Email         { get; set; } = string.Empty;
        public string Rol           { get; set; } = string.Empty;
        public string Nombre        { get; set; } = string.Empty;
        public string TipoCuerpo    { get; set; } = string.Empty;
        public bool   PerfilCompleto { get; set; }
    }
}