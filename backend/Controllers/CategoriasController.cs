using Microsoft.AspNetCore.Mvc;
using TiendaEcommerce.Api.DTOs;
using TiendaEcommerce.Api.Services;

namespace TiendaEcommerce.Api.Controllers
{
    /// <summary>
    /// HU-02: Controlador REST para la gestión de categorías (/api/categorias).
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public class CategoriasController : ControllerBase
    {
        private readonly ICatalogoService _catalogoService;

        public CategoriasController(ICatalogoService catalogoService)
        {
            _catalogoService = catalogoService;
        }

        // GET: api/categorias
        [HttpGet]
        public async Task<ActionResult<List<CategoriaRespuestaDto>>> ObtenerTodas()
        {
            return Ok(await _catalogoService.ObtenerCategoriasAsync());
        }

        // GET: api/categorias/1
        [HttpGet("{id:int}")]
        public async Task<ActionResult<CategoriaRespuestaDto>> ObtenerPorId(int id)
        {
            var categoria = await _catalogoService.ObtenerCategoriaPorIdAsync(id);
            if (categoria == null)
            {
                return NotFound(new { mensaje = $"No existe una categoría con ID {id}." });
            }
            return Ok(categoria);
        }

        // POST: api/categorias
        [HttpPost]
        public async Task<ActionResult<CategoriaRespuestaDto>> Crear([FromBody] GuardarCategoriaDto dto)
        {
            var resultado = await _catalogoService.CrearCategoriaAsync(dto);
            if (!resultado.Exito)
            {
                return BadRequest(new { mensaje = resultado.Mensaje });
            }

            return CreatedAtAction(
                nameof(ObtenerPorId),
                new { id = resultado.Datos!.IdCategoria },
                resultado.Datos);
        }

        // PUT: api/categorias/1
        [HttpPut("{id:int}")]
        public async Task<ActionResult<CategoriaRespuestaDto>> Actualizar(int id, [FromBody] GuardarCategoriaDto dto)
        {
            var resultado = await _catalogoService.ActualizarCategoriaAsync(id, dto);
            if (!resultado.Exito)
            {
                if (resultado.Mensaje.Contains("no encontrada"))
                {
                    return NotFound(new { mensaje = resultado.Mensaje });
                }
                return BadRequest(new { mensaje = resultado.Mensaje });
            }

            return Ok(resultado.Datos);
        }

        // DELETE: api/categorias/1
        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Eliminar(int id)
        {
            var resultado = await _catalogoService.EliminarCategoriaAsync(id);
            if (!resultado.Exito)
            {
                if (resultado.Mensaje.Contains("no encontrada"))
                {
                    return NotFound(new { mensaje = resultado.Mensaje });
                }
                return BadRequest(new { mensaje = resultado.Mensaje });
            }

            return Ok(new { mensaje = resultado.Mensaje });
        }
    }
}