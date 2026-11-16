using Microsoft.AspNetCore.Mvc;
using TiendaEcommerce.Api.DTOs;
using TiendaEcommerce.Api.Services;

namespace TiendaEcommerce.Api.Controllers
{
    /// <summary>
    /// HU-03: Controlador REST para la gestión de Pedidos, transacciones y control de stock.
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public class PedidosController : ControllerBase
    {
        private readonly IPedidoService _pedidoService;

        public PedidosController(IPedidoService pedidoService)
        {
            _pedidoService = pedidoService;
        }

        /// <summary>
        /// Endpoint GET para listar todos los pedidos registrados o filtrados por usuario.
        /// HU-03: Permite consultar el historial de compras.
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> ObtenerTodos([FromQuery] int? idUsuario = null)
        {
            var pedidos = await _pedidoService.ObtenerPedidosAsync(idUsuario);
            return Ok(pedidos);
        }

        /// <summary>
        /// Endpoint GET para consultar el detalle de un pedido específico por su ID.
        /// </summary>
        [HttpGet("{id:int}")]
        public async Task<IActionResult> ObtenerPorId(int id)
        {
            var pedido = await _pedidoService.ObtenerPedidoPorIdAsync(id);
            if (pedido == null)
            {
                return NotFound(new { mensaje = "Pedido no encontrado." });
            }
            return Ok(pedido);
        }

        /// <summary>
        /// Endpoint POST para crear un nuevo pedido. 
        /// HU-03: Valida el stock disponible, descuenta inventario y genera el registro transaccional.
        /// </summary>
        [HttpPost]
        public async Task<IActionResult> Crear([FromBody] CrearPedidoDto dto)
        {
            var resultado = await _pedidoService.CrearPedidoAsync(dto);
            if (!resultado.Exito)
            {
                return BadRequest(new { mensaje = resultado.Mensaje });
            }

            return CreatedAtAction(
                nameof(ObtenerPorId),
                new { id = resultado.Datos!.IdPedido },
                new { mensaje = resultado.Mensaje, pedido = resultado.Datos }
            );
        }

        /// <summary>
        /// Endpoint PUT para actualizar el estado logístico de un pedido (Pendiente, Pagado, Enviado, Entregado).
        /// </summary>
        [HttpPut("{id:int}/estado")]
        public async Task<IActionResult> ActualizarEstado(int id, [FromBody] ActualizarEstadoPedidoDto dto)
        {
            var resultado = await _pedidoService.ActualizarEstadoAsync(id, dto);
            if (!resultado.Exito)
            {
                return BadRequest(new { mensaje = resultado.Mensaje });
            }
            return Ok(new { mensaje = resultado.Mensaje });
        }
    }
}