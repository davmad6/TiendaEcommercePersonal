using System.Security.Cryptography;
using System.Text;
using TiendaEcommerce.Api.DTOs;
using TiendaEcommerce.Api.Models;
using TiendaEcommerce.Api.Repositories;

namespace TiendaEcommerce.Api.Services
{
    /// <summary>
    /// Contrato de la lógica de negocio para la gestión de usuarios (HU-01).
    /// </summary>
    public interface IUsuarioService
    {
        Task<List<UsuarioRespuestaDto>> ObtenerTodosAsync();
        Task<UsuarioRespuestaDto?> ObtenerPorIdAsync(int id);
        Task<(bool Exito, string Mensaje, UsuarioRespuestaDto? Datos)> CrearAsync(CrearUsuarioDto dto);
        Task<(bool Exito, string Mensaje, UsuarioRespuestaDto? Datos)> ActualizarAsync(int id, ActualizarUsuarioDto dto);
        Task<(bool Exito, string Mensaje)> EliminarAsync(int id);
    }

    /// <summary>
    /// Servicio que aplica validaciones de negocio, cifrado SHA-256 de contraseñas
    /// y transformación segura entre entidades y DTOs.
    /// </summary>
    public class UsuarioService : IUsuarioService
    {
        private readonly IUsuarioRepository _usuarioRepository;

        public UsuarioService(IUsuarioRepository usuarioRepository)
        {
            _usuarioRepository = usuarioRepository;
        }

        public async Task<List<UsuarioRespuestaDto>> ObtenerTodosAsync()
        {
            var usuarios = await _usuarioRepository.ObtenerTodosAsync();
            return usuarios.Select(MapearARespuesta).ToList();
        }

        public async Task<UsuarioRespuestaDto?> ObtenerPorIdAsync(int id)
        {
            var usuario = await _usuarioRepository.ObtenerPorIdAsync(id);
            return usuario == null ? null : MapearARespuesta(usuario);
        }

        public async Task<(bool Exito, string Mensaje, UsuarioRespuestaDto? Datos)> CrearAsync(CrearUsuarioDto dto)
        {
            if (string.IsNullOrWhiteSpace(dto.Nombre) ||
                string.IsNullOrWhiteSpace(dto.Email) ||
                string.IsNullOrWhiteSpace(dto.Contrasena))
            {
                return (false, "Los campos nombre, email y contraseña son obligatorio.", null);
            }

            if (!dto.Email.Contains('@') || !dto.Email.Contains('.'))
            {
                return (false, "El formato del correo electrónico no es válido.", null);
            }

            if (dto.Contrasena.Length < 6)
            {
                return (false, "La contraseña debe tener al menos 6 caracteres.", null);
            }

            if (await _usuarioRepository.ExisteEmailAsync(dto.Email))
            {
                return (false, "El correo electrónico ya se encuentra registrado.", null);
            }

            var nuevoUsuario = new Usuario
            {
                Nombre = dto.Nombre.Trim(),
                Email = dto.Email.Trim().ToLower(),
                Contrasena = CifrarContrasena(dto.Contrasena),
                Direccion = string.IsNullOrWhiteSpace(dto.Direccion) ? null : dto.Direccion.Trim(),
                Telefono = string.IsNullOrWhiteSpace(dto.Telefono) ? null : dto.Telefono.Trim()
            };

            var creado = await _usuarioRepository.CrearAsync(nuevoUsuario);
            return (true, "Usuario registrado correctamente.", MapearARespuesta(creado));
        }

        public async Task<(bool Exito, string Mensaje, UsuarioRespuestaDto? Datos)> ActualizarAsync(int id, ActualizarUsuarioDto dto)
        {
            var usuarioExistente = await _usuarioRepository.ObtenerPorIdAsync(id);
            if (usuarioExistente == null)
            {
                return (false, "Usuario no encontrado.", null);
            }

            if (string.IsNullOrWhiteSpace(dto.Nombre) || string.IsNullOrWhiteSpace(dto.Email))
            {
                return (false, "Los campos nombre y email son obligatorios.", null);
            }

            if (await _usuarioRepository.ExisteEmailAsync(dto.Email, id))
            {
                return (false, "El correo electrónico ya está en uso por otro usuario.", null);
            }

            usuarioExistente.Nombre = dto.Nombre.Trim();
            usuarioExistente.Email = dto.Email.Trim().ToLower();
            usuarioExistente.Direccion = string.IsNullOrWhiteSpace(dto.Direccion) ? null : dto.Direccion.Trim();
            usuarioExistente.Telefono = string.IsNullOrWhiteSpace(dto.Telefono) ? null : dto.Telefono.Trim();

            if (!string.IsNullOrWhiteSpace(dto.Contrasena))
            {
                if (dto.Contrasena.Length < 6)
                {
                    return (false, "La nueva contraseña debe tener al menos 6 caracteres.", null);
                }
                usuarioExistente.Contrasena = CifrarContrasena(dto.Contrasena);
            }

            await _usuarioRepository.ActualizarAsync(usuarioExistente);
            return (true, "Usuario actualizado correctamente.", MapearARespuesta(usuarioExistente));
        }

        public async Task<(bool Exito, string Mensaje)> EliminarAsync(int id)
        {
            var usuario = await _usuarioRepository.ObtenerPorIdAsync(id);
            if (usuario == null)
            {
                return (false, "Usuario no encontrado.");
            }

            await _usuarioRepository.EliminarAsync(usuario);
            return (true, "Usuario eliminado correctamente.");
        }

        private static UsuarioRespuestaDto MapearARespuesta(Usuario u) => new()
        {
            IdUsuario = u.IdUsuario,
            Nombre = u.Nombre,
            Email = u.Email,
            Direccion = u.Direccion,
            Telefono = u.Telefono
        };

        private static string CifrarContrasena(string contrasena)
        {
            var bytes = SHA256.HashData(Encoding.UTF8.GetBytes(contrasena));
            return Convert.ToHexString(bytes).ToLower();
        }
    }
}