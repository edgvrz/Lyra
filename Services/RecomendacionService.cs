using Lyra.Models;
using Lyra.Repositories;

namespace Lyra.Services
{
    public class RecomendacionService
    {
        private readonly IPrendaRepository _prendaRepo;

        public RecomendacionService(IPrendaRepository prendaRepo)
            => _prendaRepo = prendaRepo;

        /// <summary>
        /// Devuelve prendas ordenadas por relevancia para el perfil dado.
        /// Prioridad: tipo cuerpo + tono piel > solo tipo cuerpo.
        /// </summary>
        public async Task<List<Prenda>> ObtenerParaPerfilAsync(UsuarioPerfil perfil)
        {
            if (perfil.TipoCuerpo == null) return new List<Prenda>();

            return await _prendaRepo.ObtenerRecomendacionesAsync(
                perfil.TipoCuerpo,
                perfil.TonoPiel.ToString());
        }
    }
}