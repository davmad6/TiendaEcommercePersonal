using Microsoft.AspNetCore.Mvc;
using TiendaEcommerce.Api.DTOs;
using TiendaEcommerce.Api.Services;

namespace TiendaEcommerce.Api.Controllers
{
    /// <summary>
    /// HU-01: Controlador REST para la gestión de usuarios (CRUD).
    /// Expone endpoints JSON bajo la ruta base /api/usuarios.
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public class UsuariosController : ControllerBase
    {
        private readonly IUsuarioService _usuarioService;

        public UsuariosController(IUsuarioService usuarioService)
        {
            _usuarioService = usuarioService;
        }

        // GET: api/usuarios
        [HttpGet]
        public async Task<ActionResult<List<UsuarioRespuestaDto>>> ObtenerTodos()
        {
            var usuarios = await _usuarioService.ObtenerTodosAsync();
            return Ok(usuarios);
        }

        // GET: api/usuarios/5
        [HttpGet("{id:int}")]
        public async Task<ActionResult<UsuarioRespuestaDto>> ObtenerPorId(int id)
        {
            var usuario = await _usuarioService.ObtenerPorIdAsync(id);
            if (usuario == null)
            {
                return NotFound(new { mensaje = $"No existe un usuario con ID {id}." });
            }
            return Ok(usuario);
        }

        // POST: api/usuarios
        [HttpPost]
        public async Task<ActionResult<UsuarioRespuestaDto>> Crear([FromBody] CrearUsuarioDto dto)
        {
            var resultado = await _usuarioService.CrearAsync(dto);
            if (!resultado.Exito)
            {
                return BadRequest(new { mensaje = resultado.Mensaje });
            }

            return CreatedAtAction(
                nameof(ObtenerPorId),
                new { id = resultado.Datos!.IdUsuario },
                resultado.Datos);
        }

        // PUT: api/usuarios/5
        [HttpPut("{id:int}")]
        public async Task<ActionResult<UsuarioRespuestaDto>> Actualizar(int id, [FromBody] ActualizarUsuarioDto dto)
        {
            var resultado = await _usuarioService.ActualizarAsync(id, dto);
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

        // DELETE: api/usuarios/5
        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Eliminar(int id)
        {
            var resultado = await _usuarioService.EliminarAsync(id);
            if (!resultado.Exito)
            {
                return NotFound(new { mensaje = resultado.Mensaje });
            }

            return Ok(new { mensaje = resultado.Mensaje });
        }
    }
}