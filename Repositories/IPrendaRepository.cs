using Lyra.Models;

namespace Lyra.Repositories
{
    public interface IPrendaRepository
    {
        Task<List<Prenda>> ObtenerTodosAsync();
        Task<List<Prenda>> ObtenerPorTiendaAsync(int tiendaId);
        Task<List<Prenda>> ObtenerRecomendacionesAsync(string tipoCuerpo, string tonoPiel);
        Task<Prenda?>      ObtenerPorIdAsync(int id);
        Task               AgregarAsync(Prenda prenda);
        Task               ActualizarAsync(Prenda prenda);
        Task               EliminarAsync(int id);
    }
}