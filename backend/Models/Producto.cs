using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TiendaEcommerce.Api.Models
{
    /// <summary>
    /// HU-02: Entidad que representa la tabla 'productos' en PostgreSQL.
    /// Mapea exactamente las 6 columnas definidas en la base de datos.
    /// </summary>
    [Table("productos")]
    public class Producto
    {
        [Key]
        [Column("id_producto")]
        public int IdProducto { get; set; }

        [Required]
        [MaxLength(150)]
        [Column("nombre")]
        public string Nombre { get; set; } = string.Empty;

        [Column("descripcion")]
        public string? Descripcion { get; set; }

        [Required]
        [Column("precio", TypeName = "decimal(10,2)")]
        public decimal Precio { get; set; }

        [Required]
        [Column("stock")]
        public int Stock { get; set; }

        [Required]
        [Column("id_categoria")]
        public int IdCategoria { get; set; }

        // Propiedad de navegación hacia la categoría asociada
        [ForeignKey("IdCategoria")]
        public Categoria? Categoria { get; set; }
    }
}