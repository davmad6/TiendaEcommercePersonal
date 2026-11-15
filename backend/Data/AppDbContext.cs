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

        // Representa la tabla 'usuarios' de la base de datos
        public DbSet<Usuario> Usuarios { get; set; }
    }
}