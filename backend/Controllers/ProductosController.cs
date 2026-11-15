using Microsoft.AspNetCore.Mvc;
using TiendaEcommerce.Api.DTOs;
using TiendaEcommerce.Api.Services;

namespace TiendaEcommerce.Api.Controllers
{
    /// <summary>
    /// HU-02: Controlador REST para el catálogo de productos (/api/productos).
    /// Soporta filtrado opcional por categoría mediante ?idCategoria=1.
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public class ProductosController : ControllerBase
    {
        private readonly ICatalogoService _catalogoService;

        public ProductosController(ICatalogoService catalogoService)
        {
            _catalogoService = catalogoService;
        }

        // GET: api/productos  o  GET: api/productos?idCategoria=1
        [HttpGet]
        public async Task<ActionResult<List<ProductoRespuestaDto>>> ObtenerTodos([FromQuery] int? idCategoria)
        {
            return Ok(await _catalogoService.ObtenerProductosAsync(idCategoria));
        }

        // GET: api/productos/1
        [HttpGet("{id:int}")]
        public async Task<ActionResult<ProductoRespuestaDto>> ObtenerPorId(int id)
        {
            var producto = await _catalogoService.ObtenerProductoPorIdAsync(id);
            if (producto == null)
            {
                return NotFound(new { mensaje = $"No existe un producto con ID {id}." });
            }
            return Ok(producto);
        }

        // POST: api/productos
        [HttpPost]
        public async Task<ActionResult<ProductoRespuestaDto>> Crear([FromBody] GuardarProductoDto dto)
        {
            var resultado = await _catalogoService.CrearProductoAsync(dto);
            if (!resultado.Exito)
            {
                return BadRequest(new { mensaje = resultado.Mensaje });
            }

            return CreatedAtAction(
                nameof(ObtenerPorId),
                new { id = resultado.Datos!.IdProducto },
                resultado.Datos);
        }

        // PUT: api/productos/1
        [HttpPut("{id:int}")]
        public async Task<ActionResult<ProductoRespuestaDto>> Actualizar(int id, [FromBody] GuardarProductoDto dto)
        {
            var resultado = await _catalogoService.ActualizarProductoAsync(id, dto);
            if (!resultado.Exito)
            {
                if (resultado.Mensaje.Contains("no encontrado"))
                {
                    return NotFound(new { mensaje = resultado.Mensaje });
                }
                return BadRequest(new { mensaje = resultado.Mensaje });
            }

            return Ok(resultado.Datos);
        }

        // DELETE: api/productos/1
        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Eliminar(int id)
        {
            var resultado = await _catalogoService.EliminarProductoAsync(id);
            if (!resultado.Exito)
            {
                return NotFound(new { mensaje = resultado.Mensaje });
            }

            return Ok(new { mensaje = resultado.Mensaje });
        }
    }
}