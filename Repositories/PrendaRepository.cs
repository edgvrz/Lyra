using Lyra.Data;
using Lyra.Enums;
using Lyra.Models;
using Microsoft.EntityFrameworkCore;
using Lyra.Services;

namespace Lyra.Repositories
{
    public class PrendaRepository : IPrendaRepository
    {
        private readonly ApplicationDbContext _context;
        public PrendaRepository(ApplicationDbContext context) => _context = context;

        public async Task<List<Prenda>> ObtenerTodosAsync() =>
    await _context.Prendas
        .Include(p => p.Tienda)
        .OrderByDescending(p => p.FechaPublicacion)
        .ToListAsync(); // Admin ve TODO, sin filtro de estado

        public async Task<List<Prenda>> ObtenerPorTiendaAsync(int tiendaId) =>
            await _context.Prendas
                .Where(p => p.TiendaId == tiendaId)
                .OrderByDescending(p => p.FechaPublicacion)
                .ToListAsync();

        public async Task<List<Prenda>> ObtenerRecomendacionesAsync(
    string tipoCuerpo, string tonoPiel)
{
    var todas = await _context.Prendas
        .Include(p => p.Tienda)
        .Where(p =>
            p.Estado == EstadoPrenda.Disponible &&
            p.Tienda != null &&
            p.Tienda.Activa &&
            p.Tienda.EstadoAprobacion == EstadoAprobacion.Aprobada)
        .ToListAsync();

    bool MatchExacto(string campo, string valor) =>
        campo.Split(',')
             .Select(v => v.Trim())
             .Contains(valor, StringComparer.OrdinalIgnoreCase);

    var resultado = todas
        .Where(p =>
            MatchExacto(p.TiposCuerpoCompatibles, tipoCuerpo) &&
            MatchExacto(p.TonosPielCompatibles,   tonoPiel))
        .ToList();

    if (!resultado.Any())
        resultado = todas
            .Where(p => MatchExacto(p.TiposCuerpoCompatibles, tipoCuerpo))
            .ToList();

    // Ordenar: prendas cuyo color coincide con la paleta recomendada van primero
    if (Enum.TryParse<TonoPielEnum>(tonoPiel, out var tonoEnum) &&
        ColorimetriaService.PalabrasClave.TryGetValue(tonoEnum, out var palabras))
    {
        resultado = resultado
            .OrderByDescending(p =>
                palabras.Any(c => p.Color.ToLower().Contains(c)) ? 1 : 0)
            .ToList();
    }

    return resultado;
}
        public async Task<Prenda?> ObtenerPorIdAsync(int id) =>
            await _context.Prendas
                .Include(p => p.Tienda)
                .FirstOrDefaultAsync(p => p.Id == id);

        public async Task AgregarAsync(Prenda prenda)
        {
            _context.Prendas.Add(prenda);
            await _context.SaveChangesAsync();
        }

        public async Task ActualizarAsync(Prenda prenda)
        {
            _context.Prendas.Update(prenda);
            await _context.SaveChangesAsync();
        }

        public async Task EliminarAsync(int id)
        {
            var prenda = await _context.Prendas.FindAsync(id);
            if (prenda != null)
            {
                _context.Prendas.Remove(prenda);
                await _context.SaveChangesAsync();
            }
        }
    }
}