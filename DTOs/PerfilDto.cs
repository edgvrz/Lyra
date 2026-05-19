using Lyra.Enums;

namespace Lyra.DTOs
{
    public class PerfilDto
    {
        public string Nombre       { get; set; } = string.Empty;
        public int    Edad         { get; set; }
        public double Peso         { get; set; }
        public double Estatura     { get; set; }
        public TonoPielEnum TonoPiel { get; set; }
        public double Hombros      { get; set; }
        public double Busto        { get; set; }
        public double Cintura      { get; set; }
        public double Cadera       { get; set; }
    }
}