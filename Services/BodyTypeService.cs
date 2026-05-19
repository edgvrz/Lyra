using Lyra.Enums;

namespace Lyra.Services
{
    public class BodyTypeService
    {
        /// <summary>
        /// Detecta el tipo de cuerpo usando ratios porcentuales (no diferencias absolutas).
        /// Todas las medidas en centímetros — circunferencias.
        /// Fuente: Who What Wear, OmniCalculator, Acibadem Healthcare.
        /// </summary>
        public string DetectarTipo(
            double hombros,
            double busto,
            double cintura,
            double cadera)
        {
            // Usamos el mayor entre hombros y busto como referencia superior
            double superior = Math.Max(hombros, busto);

            // Ratios clave
            double ratioCinturaBusto   = cintura / busto;
            double ratioCinturaCadera  = cintura / cadera;
            double ratioCaderaSuperior = cadera / superior;
            double ratioSuperiorCadera = superior / cadera;

            // ─── RELOJ DE ARENA ───
            // Cadera y parte superior dentro del 5% entre sí
            // Cintura al menos 25% más estrecha que busto Y cadera
            if (ratioCaderaSuperior is >= 0.95 and <= 1.05 &&
                ratioCinturaBusto <= 0.75 &&
                ratioCinturaCadera <= 0.75)
            {
                return TipoCuerpoConst.RelojDeArena;
            }

            // ─── OVALADO (Manzana) ───
            // La cintura es igual o mayor que el busto → zona media dominante
            if (cintura >= busto)
            {
                return TipoCuerpoConst.Ovalado;
            }

            // ─── TRIÁNGULO INVERTIDO ───
            // Parte superior al menos 5% más ancha que la cadera
            if (ratioSuperiorCadera >= 1.05)
            {
                return TipoCuerpoConst.TrianguloInvert;
            }

            // ─── TRIÁNGULO (Pera) ───
            // Cadera al menos 5% más ancha que la parte superior
            if (ratioCaderaSuperior >= 1.05)
            {
                return TipoCuerpoConst.Triangulo;
            }

            // ─── RECTÁNGULO ───
            // Todo proporcional, sin cintura muy marcada
            return TipoCuerpoConst.Rectangulo;
        }

        /// <summary>
        /// Devuelve los tipos de cuerpo compatibles con una prenda (para el motor de recomendación).
        /// </summary>
        public List<string> ObtenerTiposCompatibles(string tipoCuerpo)
        {
            return tipoCuerpo switch
            {
                TipoCuerpoConst.RelojDeArena => new List<string>
                    { TipoCuerpoConst.RelojDeArena, TipoCuerpoConst.Rectangulo },

                TipoCuerpoConst.Triangulo => new List<string>
                    { TipoCuerpoConst.Triangulo, TipoCuerpoConst.RelojDeArena },

                TipoCuerpoConst.TrianguloInvert => new List<string>
                    { TipoCuerpoConst.TrianguloInvert, TipoCuerpoConst.Rectangulo },

                TipoCuerpoConst.Ovalado => new List<string>
                    { TipoCuerpoConst.Ovalado },

                TipoCuerpoConst.Rectangulo => new List<string>
                    { TipoCuerpoConst.Rectangulo, TipoCuerpoConst.TrianguloInvert,
                      TipoCuerpoConst.Triangulo },

                _ => new List<string> { tipoCuerpo }
            };
        }
    }
}