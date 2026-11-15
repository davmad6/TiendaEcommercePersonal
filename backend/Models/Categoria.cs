using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TiendaEcommerce.Api.Models
{
    /// <summary>
    /// HU-02: Entidad que representa la tabla 'categorias' en PostgreSQL.
    /// </summary>
    [Table("categorias")]
    public class Categoria
    {
        [Key]
        [Column("id_categoria")]
        public int IdCategoria { get; set; }

        [Required]
        [MaxLength(100)]
        [Column("nombre")]
        public string Nombre { get; set; } = string.Empty;

        // Propiedad de navegación: una categoría agrupa múltiples productos
        public ICollection<Producto> Productos { get; set; } = new List<Producto>();
    }
}