namespace TiendaEcommerce.Api.DTOs
{
    /// <summary>
    /// HU-01: Datos requeridos para registrar un nuevo usuario desde el formulario.
    /// </summary>
    public class CrearUsuarioDto
    {
        public string Nombre { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Contrasena { get; set; } = string.Empty;
        public string? Direccion { get; set; }
        public string? Telefono { get; set; }
    }

    /// <summary>
    /// HU-01: Datos permitidos para actualizar un usuario existente.
    /// </summary>
    public class ActualizarUsuarioDto
    {
        public string Nombre { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string? Contrasena { get; set; }
        public string? Direccion { get; set; }
        public string? Telefono { get; set; }
    }

    /// <summary>
    /// HU-01: Respuesta segura enviada al cliente web (omite la contraseña).
    /// </summary>
    public class UsuarioRespuestaDto
    {
        public int IdUsuario { get; set; }
        public string Nombre { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string? Direccion { get; set; }
        public string? Telefono { get; set; }
    }
}