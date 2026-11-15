using Microsoft.EntityFrameworkCore;
using TiendaEcommerce.Api.Data;
using TiendaEcommerce.Api.Models;

namespace TiendaEcommerce.Api.Repositories
{
    /// <summary>
    /// Contrato de acceso a datos para las tablas 'categorias' y 'productos'.
    /// </summary>
    public interface ICatalogoRepository
    {
        // Categorías
        Task<List<Categoria>> ObtenerCategoriasAsync();
        Task<Categoria?> ObtenerCategoriaPorIdAsync(int id);
        Task<bool> CategoriaTieneProductosAsync(int idCategoria);
        Task<Categoria> CrearCategoriaAsync(Categoria categoria);
        Task ActualizarCategoriaAsync(Categoria categoria);
        Task EliminarCategoriaAsync(Categoria categoria);

        // Productos
        Task<List<Producto>> ObtenerProductosAsync(int? idCategoria = null);
        Task<Producto?> ObtenerProductoPorIdAsync(int id);
        Task<Producto> CrearProductoAsync(Producto producto);
        Task ActualizarProductoAsync(Producto producto);
        Task EliminarProductoAsync(Producto producto);
    }

    /// <summary>
    /// Implementación con Entity Framework Core para gestionar categorías y productos en PostgreSQL.
    /// </summary>
    public class CatalogoRepository : ICatalogoRepository
    {
        private readonly AppDbContext _context;

        public CatalogoRepository(AppDbContext context)
        {
            _context = context;
        }

        // ================= CATEGORÍAS =================

        public async Task<List<Categoria>> ObtenerCategoriasAsync()
        {
            return await _context.Categorias
                .AsNoTracking()
                .OrderBy(c => c.IdCategoria)
                .ToListAsync();
        }

        public async Task<Categoria?> ObtenerCategoriaPorIdAsync(int id)
        {
            return await _context.Categorias
                .FirstOrDefaultAsync(c => c.IdCategoria == id);
        }

        public async Task<bool> CategoriaTieneProductosAsync(int idCategoria)
        {
            return await _context.Productos
                .AnyAsync(p => p.IdCategoria == idCategoria);
        }

        public async Task<Categoria> CrearCategoriaAsync(Categoria categoria)
        {
            await _context.Categorias.AddAsync(categoria);
            await _context.SaveChangesAsync();
            return categoria;
        }

        public async Task ActualizarCategoriaAsync(Categoria categoria)
        {
            _context.Categorias.Update(categoria);
            await _context.SaveChangesAsync();
        }

        public async Task EliminarCategoriaAsync(Categoria categoria)
        {
            _context.Categorias.Remove(categoria);
            await _context.SaveChangesAsync();
        }

        // ================= PRODUCTOS =================

        public async Task<List<Producto>> ObtenerProductosAsync(int? idCategoria = null)
        {
            var consulta = _context.Productos
                .AsNoTracking()
                .Include(p => p.Categoria)
                .AsQueryable();

            if (idCategoria.HasValue)
            {
                consulta = consulta.Where(p => p.IdCategoria == idCategoria.Value);
            }

            return await consulta
                .OrderBy(p => p.IdProducto)
                .ToListAsync();
        }

        public async Task<Producto?> ObtenerProductoPorIdAsync(int id)
        {
            return await _context.Productos
                .Include(p => p.Categoria)
                .FirstOrDefaultAsync(p => p.IdProducto == id);
        }

        public async Task<Producto> CrearProductoAsync(Producto producto)
        {
            await _context.Productos.AddAsync(producto);
            await _context.SaveChangesAsync();
            return await ObtenerProductoPorIdAsync(producto.IdProducto) ?? producto;
        }

        public async Task ActualizarProductoAsync(Producto producto)
        {
            _context.Productos.Update(producto);
            await _context.SaveChangesAsync();
        }

        public async Task EliminarProductoAsync(Producto producto)
        {
            _context.Productos.Remove(producto);
            await _context.SaveChangesAsync();
        }
    }
}