namespace TiendaEcommerce.Api.DTOs
{
    /// <summary>
    /// Ítem individual enviado desde el carrito de compras al confirmar un pedido.
    /// </summary>
    public class ItemPedidoCrearDto
    {
        public int IdProducto { get; set; }
        public int Cantidad { get; set; }
    }

    /// <summary>
    /// HU-03: Solicitud para registrar un nuevo pedido con uno o más productos.
    /// </summary>
    public class CrearPedidoDto
    {
        public int IdUsuario { get; set; }
        public List<ItemPedidoCrearDto> Items { get; set; } = new();
    }

    /// <summary>
    /// Datos permitidos para cambiar el estado de un pedido (ej: Pendiente, Pagado, Enviado, Entregado, Cancelado).
    /// </summary>
    public class ActualizarEstadoPedidoDto
    {
        public string Estado { get; set; } = string.Empty;
    }

    /// <summary>
    /// Respuesta JSON de cada línea de producto dentro de un pedido.
    /// </summary>
    public class DetallePedidoRespuestaDto
    {
        public int IdDetalle { get; set; }
        public int IdProducto { get; set; }
        public string NombreProducto { get; set; } = string.Empty;
        public int Cantidad { get; set; }
        public decimal PrecioUnitario { get; set; }
        public decimal Subtotal { get; set; }
    }

    /// <summary>
    /// Respuesta JSON completa de un pedido con datos del cliente y su lista de productos.
    /// </summary>
    public class PedidoRespuestaDto
    {
        public int IdPedido { get; set; }
        public DateTime Fecha { get; set; }
        public decimal Total { get; set; }
        public string Estado { get; set; } = string.Empty;
        public int IdUsuario { get; set; }
        public string NombreUsuario { get; set; } = string.Empty;
        public List<DetallePedidoRespuestaDto> Detalles { get; set; } = new();
    }
}