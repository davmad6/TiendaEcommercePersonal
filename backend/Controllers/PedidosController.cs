using Microsoft.AspNetCore.Mvc;
using TiendaEcommerce.Api.DTOs;
using TiendaEcommerce.Api.Services;

namespace TiendaEcommerce.Api.Controllers
{
    /// <summary>
    /// HU-03: Endpoints REST para creación, consulta y gestión de estado de pedidos.
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

        // GET: api/pedidos?idUsuario=1
        [HttpGet]
        public async Task<IActionResult> ObtenerTodos([FromQuery] int? idUsuario = null)
        {
            var pedidos = await _pedidoService.ObtenerPedidosAsync(idUsuario);
            return Ok(pedidos);
        }

        // GET: api/pedidos/1
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

        // POST: api/pedidos
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

        // PUT: api/pedidos/1/estado
        [HttpPut("{id:int}/estado")]
        public async Task<IActionResult> ActualizarEstado(int id, [FromBody] ActualizarEstadoPedidoDto dto)
        {
            var resultado = await _pedidoService.ActualizarEstadoAsync(id, dto);
            if (!resultado.Exito)
            {
                return BadRequest(new { mensaje = resultado.Mensaje });
            }

            return Ok(new { mensaje = resultado.Mensaje, pedido = resultado.Datos });
        }
    }
}