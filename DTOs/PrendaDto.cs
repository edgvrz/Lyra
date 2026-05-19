using Lyra.Enums;

namespace Lyra.DTOs
{
    public class PrendaDto
    {
        public int          Id                     { get; set; }
        public string       Nombre                 { get; set; } = string.Empty;
        public string?      Descripcion            { get; set; }
        public CategoriaRopa Categoria             { get; set; }
        public string       Color                  { get; set; } = string.Empty;
        public decimal      Precio                 { get; set; }
        public TallaPrenda  Talla                  { get; set; }
        public EstadoPrenda Estado                 { get; set; }
        public string?      ImagenUrl              { get; set; }
        public string       TiposCuerpoCompatibles { get; set; } = string.Empty;
        public string       TonosPielCompatibles   { get; set; } = string.Empty;
        public string?      TiendaNombre           { get; set; }
    }
}