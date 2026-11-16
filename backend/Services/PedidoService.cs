using TiendaEcommerce.Api.DTOs;
using TiendaEcommerce.Api.Models;
using TiendaEcommerce.Api.Repositories;

namespace TiendaEcommerce.Api.Services
{
    /// <summary>
    /// Contrato de reglas de negocio para la creación y seguimiento de pedidos (HU-03).
    /// </summary>
    public interface IPedidoService
    {
        Task<List<PedidoRespuestaDto>> ObtenerPedidosAsync(int? idUsuario = null);
        Task<PedidoRespuestaDto?> ObtenerPedidoPorIdAsync(int id);
        Task<(bool Exito, string Mensaje, PedidoRespuestaDto? Datos)> CrearPedidoAsync(CrearPedidoDto dto);
        Task<(bool Exito, string Mensaje, PedidoRespuestaDto? Datos)> ActualizarEstadoAsync(int id, ActualizarEstadoPedidoDto dto);
    }

    /// <summary>
    /// Valida existencia de usuario, disponibilidad de stock, calcula totales
    /// y gestiona cambios de estado de los pedidos.
    /// </summary>
    public class PedidoService : IPedidoService
    {
        private readonly IPedidoRepository _pedidoRepository;
        private readonly IUsuarioRepository _usuarioRepository;
        private readonly ICatalogoRepository _catalogoRepository;

        private static readonly HashSet<string> EstadosValidos = new(StringComparer.OrdinalIgnoreCase)
        {
            "Pendiente",
            "Pagado",
            "Enviado",
            "Entregado",
            "Cancelado"
        };

        public PedidoService(
            IPedidoRepository pedidoRepository,
            IUsuarioRepository usuarioRepository,
            ICatalogoRepository catalogoRepository)
        {
            _pedidoRepository = pedidoRepository;
            _usuarioRepository = usuarioRepository;
            _catalogoRepository = catalogoRepository;
        }

        public async Task<List<PedidoRespuestaDto>> ObtenerPedidosAsync(int? idUsuario = null)
        {
            var pedidos = await _pedidoRepository.ObtenerPedidosAsync(idUsuario);
            return pedidos.Select(MapearPedido).ToList();
        }

        public async Task<PedidoRespuestaDto?> ObtenerPedidoPorIdAsync(int id)
        {
            var pedido = await _pedidoRepository.ObtenerPedidoPorIdAsync(id);
            return pedido == null ? null : MapearPedido(pedido);
        }

        /// <summary>
        /// HU-03: Valida existencia del usuario, verifica disponibilidad de stock por producto,
        /// calcula el total de la orden y ejecuta el registro transaccional en base de datos.
        /// </summary>
        public async Task<(bool Exito, string Mensaje, PedidoRespuestaDto? Datos)> CrearPedidoAsync(CrearPedidoDto dto)
        {
            var usuario = await _usuarioRepository.ObtenerPorIdAsync(dto.IdUsuario);
            if (usuario == null)
            {
                return (false, $"El usuario con ID {dto.IdUsuario} no existe.", null);
            }

            if (dto.Items == null || dto.Items.Count == 0)
            {
                return (false, "El pedido debe incluir al menos un producto.", null);
            }

            // Agrupar ítems repetidos del mismo producto para validar el stock acumulado
            var itemsAgrupados = dto.Items
                .GroupBy(i => i.IdProducto)
                .Select(g => new { IdProducto = g.Key, Cantidad = g.Sum(x => x.Cantidad) })
                .ToList();

            var detalles = new List<DetallePedido>();
            decimal totalPedido = 0m;

            foreach (var item in itemsAgrupados)
            {
                if (item.Cantidad <= 0)
                {
                    return (false, $"La cantidad para el producto ID {item.IdProducto} debe ser mayor a cero.", null);
                }

                var producto = await _catalogoRepository.ObtenerProductoPorIdAsync(item.IdProducto);
                if (producto == null)
                {
                    return (false, $"El producto con ID {item.IdProducto} no existe.", null);
                }

                if (producto.Stock < item.Cantidad)
                {
                    return (false, $"Stock insuficiente para '{producto.Nombre}'. Disponible: {producto.Stock}, solicitado: {item.Cantidad}.", null);
                }

                var subtotal = producto.Precio * item.Cantidad;
                totalPedido += subtotal;

                detalles.Add(new DetallePedido
                {
                    IdProducto = producto.IdProducto,
                    Cantidad = item.Cantidad,
                    PrecioUnitario = producto.Precio
                });
            }

            var nuevoPedido = new Pedido
            {
                Fecha = DateTime.SpecifyKind(DateTime.Now, DateTimeKind.Unspecified),
                Total = totalPedido,
                Estado = "Pendiente",
                IdUsuario = dto.IdUsuario,
                Detalles = detalles
            };

            // Persiste el pedido, su detalle y descuenta el stock de forma atómica (transaccional)
            var pedidoCreado = await _pedidoRepository.CrearPedidoTransaccionalAsync(nuevoPedido);
            return (true, "Pedido registrado correctamente.", MapearPedido(pedidoCreado));
        }

        public async Task<(bool Exito, string Mensaje, PedidoRespuestaDto? Datos)> ActualizarEstadoAsync(int id, ActualizarEstadoPedidoDto dto)
        {
            var pedido = await _pedidoRepository.ObtenerPedidoPorIdAsync(id);
            if (pedido == null)
            {
                return (false, "Pedido no encontrado.", null);
            }

            if (string.IsNullOrWhiteSpace(dto.Estado) || !EstadosValidos.Contains(dto.Estado.Trim()))
            {
                return (false, "Estado no válido. Valores permitidos: Pendiente, Pagado, Enviado, Entregado, Cancelado.", null);
            }

            var estadoNormalizado = EstadosValidos.First(e => e.Equals(dto.Estado.Trim(), StringComparison.OrdinalIgnoreCase));

            if (pedido.Estado.Equals("Cancelado", StringComparison.OrdinalIgnoreCase) && estadoNormalizado != "Cancelado")
            {
                return (false, "No se puede reactivar un pedido que ya fue cancelado.", null);
            }

            // Si pasa a 'Cancelado' desde otro estado, se devuelve el inventario a los productos
            bool restaurarStock = !pedido.Estado.Equals("Cancelado", StringComparison.OrdinalIgnoreCase)
                                  && estadoNormalizado == "Cancelado";

            pedido.Estado = estadoNormalizado;
            await _pedidoRepository.ActualizarEstadoTransaccionalAsync(pedido, restaurarStock);

            return (true, $"Estado del pedido actualizado a '{estadoNormalizado}'.", MapearPedido(pedido));
        }

        private static PedidoRespuestaDto MapearPedido(Pedido p) => new()
        {
            IdPedido = p.IdPedido,
            Fecha = p.Fecha,
            Total = p.Total,
            Estado = p.Estado,
            IdUsuario = p.IdUsuario,
            NombreUsuario = p.Usuario?.Nombre ?? "Usuario desconocido",
            Detalles = p.Detalles.Select(d => new DetallePedidoRespuestaDto
            {
                IdDetalle = d.IdDetalle,
                IdProducto = d.IdProducto,
                NombreProducto = d.Producto?.Nombre ?? "Producto",
                Cantidad = d.Cantidad,
                PrecioUnitario = d.PrecioUnitario,
                Subtotal = d.PrecioUnitario * d.Cantidad
            }).ToList()
        };
    }
}