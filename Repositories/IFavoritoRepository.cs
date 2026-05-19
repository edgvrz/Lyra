using Lyra.Models;

namespace Lyra.Repositories
{
    public interface IFavoritoRepository
    {
        Task<List<Favorito>> ObtenerPorPerfilAsync(int perfilId);
        Task<bool>           ExisteAsync(int perfilId, int prendaId);
        Task                 AgregarAsync(int perfilId, int prendaId);
        Task                 EliminarAsync(int favoritoId);
        Task<int>            ContarPorPerfilAsync(int perfilId);
    }
}