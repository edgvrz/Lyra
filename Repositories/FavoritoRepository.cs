using Lyra.Data;
using Lyra.Models;
using Microsoft.EntityFrameworkCore;

namespace Lyra.Repositories
{
    public class FavoritoRepository : IFavoritoRepository
    {
        private readonly ApplicationDbContext _context;
        public FavoritoRepository(ApplicationDbContext context) => _context = context;

        public async Task<List<Favorito>> ObtenerPorPerfilAsync(int perfilId) =>
            await _context.Favoritos
                .Include(f => f.Prenda).ThenInclude(p => p.Tienda)
                .Where(f => f.UsuarioPerfilId == perfilId)
                .OrderByDescending(f => f.FechaAgregado)
                .ToListAsync();

        public async Task<bool> ExisteAsync(int perfilId, int prendaId) =>
            await _context.Favoritos
                .AnyAsync(f => f.UsuarioPerfilId == perfilId && f.PrendaId == prendaId);

        public async Task AgregarAsync(int perfilId, int prendaId)
        {
            if (!await ExisteAsync(perfilId, prendaId))
            {
                _context.Favoritos.Add(new Favorito
                {
                    UsuarioPerfilId = perfilId,
                    PrendaId        = prendaId
                });
                await _context.SaveChangesAsync();
            }
        }

        public async Task EliminarAsync(int favoritoId)
        {
            var fav = await _context.Favoritos.FindAsync(favoritoId);
            if (fav != null)
            {
                _context.Favoritos.Remove(fav);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<int> ContarPorPerfilAsync(int perfilId) =>
            await _context.Favoritos.CountAsync(f => f.UsuarioPerfilId == perfilId);
    }
}