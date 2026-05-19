namespace Lyra.DTOs
{
    public class RecomendacionDto
    {
        public string TipoCuerpo        { get; set; } = string.Empty;
        public string TonoPiel          { get; set; } = string.Empty;
        public List<PrendaDto> Prendas  { get; set; } = new();
        public int    TotalEncontradas  { get; set; }
        public bool   FiltroRelajado    { get; set; } // true si usó fallback
    }
}