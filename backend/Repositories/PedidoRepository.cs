using Microsoft.EntityFrameworkCore;
using TiendaEcommerce.Api.Data;
using TiendaEcommerce.Api.Models;

namespace TiendaEcommerce.Api.Repositories
{
    /// <summary>
    /// Contrato de acceso a datos para la gestión transaccional de pedidos y sus detalles (HU-03).
    /// </summary>
    public interface IPedidoRepository
    {
        Task<List<Pedido>> ObtenerPedidosAsync(int? idUsuario = null);
        Task<Pedido?> ObtenerPedidoPorIdAsync(int id);
        Task<Pedido> CrearPedidoTransaccionalAsync(Pedido pedido);
        Task ActualizarEstadoTransaccionalAsync(Pedido pedido, bool restaurarStock);
    }

    /// <summary>
    /// Implementación con Entity Framework Core que garantiza integridad transaccional
    /// entre las tablas 'pedidos', 'detalle_pedidos' y el inventario en 'productos'.
    /// </summary>
    public class PedidoRepository : IPedidoRepository
    {
        private readonly AppDbContext _context;

        public PedidoRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<List<Pedido>> ObtenerPedidosAsync(int? idUsuario = null)
        {
            var consulta = _context.Pedidos
                .AsNoTracking()
                .Include(p => p.Usuario)
                .Include(p => p.Detalles)
                    .ThenInclude(d => d.Producto)
                .AsQueryable();

            if (idUsuario.HasValue)
            {
                consulta = consulta.Where(p => p.IdUsuario == idUsuario.Value);
            }

            return await consulta
                .OrderByDescending(p => p.IdPedido)
                .ToListAsync();
        }

        public async Task<Pedido?> ObtenerPedidoPorIdAsync(int id)
        {
            return await _context.Pedidos
                .Include(p => p.Usuario)
                .Include(p => p.Detalles)
                    .ThenInclude(d => d.Producto)
                .FirstOrDefaultAsync(p => p.IdPedido == id);
        }

        public async Task<Pedido> CrearPedidoTransaccionalAsync(Pedido pedido)
        {
            await using var transaccion = await _context.Database.BeginTransactionAsync();
            try
            {
                // 1. Insertar cabecera del pedido y líneas de detalle
                _context.Pedidos.Add(pedido);
                await _context.SaveChangesAsync();

                // 2. Descontar el stock de cada producto adquirido
                foreach (var detalle in pedido.Detalles)
                {
                    var producto = await _context.Productos.FindAsync(detalle.IdProducto);
                    if (producto != null)
                    {
                        producto.Stock -= detalle.Cantidad;
                    }
                }

                await _context.SaveChangesAsync();
                await transaccion.CommitAsync();

                // Retornar el pedido completo con relaciones cargadas
                return (await ObtenerPedidoPorIdAsync(pedido.IdPedido))!;
            }
            catch
            {
                await transaccion.RollbackAsync();
                throw;
            }
        }

        public async Task ActualizarEstadoTransaccionalAsync(Pedido pedido, bool restaurarStock)
        {
            await using var transaccion = await _context.Database.BeginTransactionAsync();
            try
            {
                if (restaurarStock)
                {
                    foreach (var detalle in pedido.Detalles)
                    {
                        var producto = await _context.Productos.FindAsync(detalle.IdProducto);
                        if (producto != null)
                        {
                            producto.Stock += detalle.Cantidad;
                        }
                    }
                }

                await _context.SaveChangesAsync();
                await transaccion.CommitAsync();
            }
            catch
            {
                await transaccion.RollbackAsync();
                throw;
            }
        }
    }
}