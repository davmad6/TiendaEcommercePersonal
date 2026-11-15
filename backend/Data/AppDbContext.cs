using Microsoft.EntityFrameworkCore;
using TiendaEcommerce.Api.Models;

namespace TiendaEcommerce.Api.Data
{
    /// <summary>
    /// Contexto principal de Entity Framework Core.
    /// Administra la conexión y las consultas parametrizadas hacia PostgreSQL.
    /// </summary>
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        // Tablas mapeadas en la base de datos tienda_ecommerce_personal
        public DbSet<Usuario> Usuarios { get; set; }
        public DbSet<Categoria> Categorias { get; set; }
        public DbSet<Producto> Productos { get; set; }
    }
}