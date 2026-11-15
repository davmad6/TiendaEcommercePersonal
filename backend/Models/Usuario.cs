using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TiendaEcommerce.Api.Models
{
    /// <summary>
    /// HU-01: Entidad que representa la tabla 'usuarios' en PostgreSQL.
    /// Mapea cada propiedad de C# con su columna exacta en la base de datos.
    /// </summary>
    [Table("usuarios")]
    public class Usuario
    {
        [Key]
        [Column("id_usuario")]
        public int IdUsuario { get; set; }

        [Required]
        [MaxLength(100)]
        [Column("nombre")]
        public string Nombre { get; set; } = string.Empty;

        [Required]
        [MaxLength(100)]
        [Column("email")]
        public string Email { get; set; } = string.Empty;

        [Required]
        [MaxLength(255)]
        [Column("contrasena")]
        public string Contrasena { get; set; } = string.Empty;

        [MaxLength(150)]
        [Column("direccion")]
        public string? Direccion { get; set; }

        [MaxLength(20)]
        [Column("telefono")]
        public string? Telefono { get; set; }
    }
}