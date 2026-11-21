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

        /// <summary>
        /// Endpoint GET para listar todos los productos o filtrarlos por categoría (?idCategoria=1).
        /// </summary>
        [HttpGet]
        public async Task<ActionResult<List<ProductoRespuestaDto>>> ObtenerTodos([FromQuery] int? idCategoria)
        {
            return Ok(await _catalogoService.ObtenerProductosAsync(idCategoria));
        }

        /// <summary>
        /// Endpoint GET para consultar el detalle de un producto específico por su ID.
        /// </summary>
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

        /// <summary>
        /// Endpoint POST para registrar un nuevo producto en el inventario del catálogo.
        /// </summary>
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

        /// <summary>
        /// Endpoint PUT para actualizar los datos, precio o stock de un producto existente.
        /// </summary>
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

        /// <summary>
        /// Endpoint DELETE para eliminar un producto del catálogo por su ID.
        /// </summary>
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