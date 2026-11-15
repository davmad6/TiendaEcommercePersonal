using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TiendaEcommerce.Api.Models
{
    /// <summary>
    /// HU-03: Entidad maestra que representa la tabla 'pedidos' en PostgreSQL.
    /// Vincula cada orden de compra con el usuario cliente y sus líneas de detalle.
    /// </summary>
    [Table("pedidos")]
    public class Pedido
    {
        [Key]
        [Column("id_pedido")]
        public int IdPedido { get; set; }

        [Required]
        [Column("fecha", TypeName = "timestamp without time zone")]
        public DateTime Fecha { get; set; }

        [Required]
        [Column("total", TypeName = "decimal(10,2)")]
        public decimal Total { get; set; }

        [Required]
        [MaxLength(50)]
        [Column("estado")]
        public string Estado { get; set; } = "Pendiente";

        [Required]
        [Column("id_usuario")]
        public int IdUsuario { get; set; }

        // Propiedades de navegación
        [ForeignKey("IdUsuario")]
        public Usuario? Usuario { get; set; }

        public ICollection<DetallePedido> Detalles { get; set; } = new List<DetallePedido>();
    }
}