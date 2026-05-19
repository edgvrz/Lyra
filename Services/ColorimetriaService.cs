using Lyra.Enums;

namespace Lyra.Services
{
    public class ColorimetriaService
    {
        public record ColorInfo(string Nombre, string Hex);

        public record PaletaRecomendada(
            string          Subtono,
            List<ColorInfo> Favorecen,
            List<ColorInfo> Evitar,
            string          Tip
        );

        public PaletaRecomendada ObtenerParaTono(TonoPielEnum tono) => tono switch
        {
            TonoPielEnum.Claro => new(
                Subtono:   "Frío",
                Favorecen: new() {
                    new("Azul marino",   "#1a3a5c"),
                    new("Esmeralda",     "#2d6a4f"),
                    new("Borgoña",       "#722f37"),
                    new("Lavanda",       "#9b8dc8"),
                    new("Blanco roto",   "#f5f0e8"),
                    new("Azul cielo",    "#87ceeb"),
                },
                Evitar: new() {
                    new("Naranja neón",  "#ff6b35"),
                    new("Amarillo lima", "#c8e600"),
                    new("Beige oscuro",  "#c4a97a"),
                },
                Tip: "Tu subtono frío se beneficia de colores joya y pastel suave. El azul marino y el esmeralda son tus mejores aliados."
            ),

            TonoPielEnum.MedioClaro => new(
                Subtono:   "Cálido suave",
                Favorecen: new() {
                    new("Coral",         "#ff7f6e"),
                    new("Durazno",       "#ffcba4"),
                    new("Oro cálido",    "#d4a017"),
                    new("Camel",         "#c19a6b"),
                    new("Terracota",     "#c46a3e"),
                    new("Rosa suave",    "#f4c2c2"),
                },
                Evitar: new() {
                    new("Blanco puro",   "#ffffff"),
                    new("Negro intenso", "#0a0a0a"),
                    new("Gris frío",     "#9e9e9e"),
                },
                Tip: "Tu subtono cálido se lleva bien con tonos tierra. El coral y el camel realzan tu piel naturalmente."
            ),

            TonoPielEnum.Medio => new(
                Subtono:   "Neutro-cálido",
                Favorecen: new() {
                    new("Terracota",     "#c46a3e"),
                    new("Verde oliva",   "#6b7c3e"),
                    new("Mostaza",       "#d4a017"),
                    new("Rojo vino",     "#722f37"),
                    new("Azul cobalto",  "#0047ab"),
                    new("Blanco",        "#f8f8f8"),
                },
                Evitar: new() {
                    new("Beige similar", "#d2b48c"),
                    new("Café apagado",  "#8b6914"),
                },
                Tip: "Tu tono neutro te da versatilidad. Colores tierra y tonos joya vibrantes te quedan especialmente bien."
            ),

            TonoPielEnum.Oliva => new(
                Subtono:   "Cálido-dorado",
                Favorecen: new() {
                    new("Naranja quemado","#bf5700"),
                    new("Verde musgo",   "#4a5e3a"),
                    new("Oro",           "#d4a017"),
                    new("Coral oscuro",  "#cd5c5c"),
                    new("Rojo ladrillo", "#8b2020"),
                    new("Dorado",        "#c9a96e"),
                },
                Evitar: new() {
                    new("Pastel frío",   "#b0c4de"),
                    new("Rosa palo",     "#f4c2c2"),
                    new("Gris perla",    "#d3d3d3"),
                },
                Tip: "Tu piel oliva brilla con tonos tierra intensos. Evita pasteles fríos que apagan tu tono natural."
            ),

            TonoPielEnum.Moreno => new(
                Subtono:   "Cálido-intenso",
                Favorecen: new() {
                    new("Blanco",        "#f8f8f8"),
                    new("Azul eléctrico","#007fff"),
                    new("Fucsia",        "#ff1493"),
                    new("Amarillo vivo", "#ffd700"),
                    new("Esmeralda",     "#2d6a4f"),
                    new("Coral vivo",    "#ff6b6b"),
                },
                Evitar: new() {
                    new("Marrón similar","#8b5e3c"),
                    new("Beige apagado", "#d2b48c"),
                    new("Gris medio",    "#808080"),
                },
                Tip: "Tu tono rico admite colores vibrantes y contrastes fuertes. El blanco y los tonos joya intensos te hacen brillar."
            ),

            TonoPielEnum.Oscuro => new(
                Subtono:   "Profundo",
                Favorecen: new() {
                    new("Blanco puro",   "#ffffff"),
                    new("Rojo intenso",  "#cc0000"),
                    new("Amarillo oro",  "#ffd700"),
                    new("Azul rey",      "#4169e1"),
                    new("Verde lima",    "#32cd32"),
                    new("Naranja vivo",  "#ff8c00"),
                },
                Evitar: new() {
                    new("Negro puro",    "#0a0a0a"),
                    new("Marrón oscuro", "#5c3317"),
                    new("Gris oscuro",   "#404040"),
                },
                Tip: "Tu piel profunda resalta con colores brillantes. Evita tonos muy oscuros que se mezclen con tu piel."
            ),

            _ => new("Neutro", new(), new(), "Completa tu perfil para recibir recomendaciones de color.")
        };

        // Palabras clave para matching automático con prendas
        public static readonly Dictionary<TonoPielEnum, List<string>> PalabrasClave = new()
        {
            [TonoPielEnum.Claro]      = new() { "azul", "marino", "esmeralda", "borgoña", "lavanda", "blanco", "verde", "gris", "morado" },
            [TonoPielEnum.MedioClaro] = new() { "coral", "durazno", "oro", "camel", "terracota", "rosa", "beige", "salmón" },
            [TonoPielEnum.Medio]      = new() { "terracota", "oliva", "mostaza", "vino", "cobalto", "blanco", "rojo", "verde" },
            [TonoPielEnum.Oliva]      = new() { "naranja", "musgo", "oro", "coral", "ladrillo", "dorado", "verde" },
            [TonoPielEnum.Moreno]     = new() { "blanco", "azul", "fucsia", "amarillo", "esmeralda", "coral", "turquesa" },
            [TonoPielEnum.Oscuro]     = new() { "blanco", "rojo", "amarillo", "oro", "azul", "verde", "naranja", "fucsia" },
        };
    }
}