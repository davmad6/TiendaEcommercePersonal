using Microsoft.EntityFrameworkCore;
using TiendaEcommerce.Api.Models;

namespace TiendaEcommerce.Api.Data
{
    /// <summary>
    /// Contexto principal de Entity Framework Core.
    /// Mapea las 5 tablas relacionales de la base de datos tienda_ecommerce_personal.
    /// </summary>
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        public DbSet<Usuario> Usuarios { get; set; }
        public DbSet<Categoria> Categorias { get; set; }
        public DbSet<Producto> Productos { get; set; }
        public DbSet<Pedido> Pedidos { get; set; }
        public DbSet<DetallePedido> DetallePedidos { get; set; }
    }
}