using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Lyra.Models;

namespace Lyra.Data
{
    public class ApplicationDbContext : IdentityDbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options) { }

        public DbSet<UsuarioPerfil>      UsuariosPerfil      { get; set; }
        public DbSet<MedidasCorporales>  MedidasCorporales   { get; set; }
        public DbSet<Tienda>             Tiendas             { get; set; }
        public DbSet<Prenda>             Prendas             { get; set; }
        public DbSet<Favorito>           Favoritos           { get; set; }
        public DbSet<Pedido>     Pedidos     { get; set; }
        public DbSet<PedidoItem> PedidoItems { get; set; }
        // Junto a los otros DbSet
public DbSet<Reserva> Reservas { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            // Favorito: clave única para evitar duplicados
            builder.Entity<Favorito>()
                .HasIndex(f => new { f.UsuarioPerfilId, f.PrendaId })
                .IsUnique();

            // UsuarioPerfil → un perfil por usuario
            builder.Entity<UsuarioPerfil>()
                .HasIndex(u => u.UserId)
                .IsUnique();

            // Tienda → un registro por usuario tienda
            builder.Entity<Tienda>()
                .HasIndex(t => t.UserId)
                .IsUnique();

            // Precio con 2 decimales
            builder.Entity<Prenda>()
                .Property(p => p.Precio)
                .HasColumnType("decimal(10,2)");
                // Reservas: índice compuesto para búsquedas rápidas
builder.Entity<Reserva>()
    .HasIndex(r => new { r.UsuarioPerfilId, r.PrendaId });
        }
    }
}