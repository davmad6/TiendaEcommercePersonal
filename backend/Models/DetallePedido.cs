using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TiendaEcommerce.Api.Models
{
    /// <summary>
    /// HU-03: Entidad transaccional que representa la tabla 'detalle_pedidos' en PostgreSQL.
    /// Registra cada producto adquirido, su cantidad y el precio unitario al momento de la compra.
    /// </summary>
    [Table("detalle_pedidos")]
    public class DetallePedido
    {
        [Key]
        [Column("id_detalle")]
        public int IdDetalle { get; set; }

        [Required]
        [Column("cantidad")]
        public int Cantidad { get; set; }

        [Required]
        [Column("precio_unitario", TypeName = "decimal(10,2)")]
        public decimal PrecioUnitario { get; set; }

        [Required]
        [Column("id_pedido")]
        public int IdPedido { get; set; }

        [Required]
        [Column("id_producto")]
        public int IdProducto { get; set; }

        // Propiedades de navegación
        [ForeignKey("IdPedido")]
        public Pedido? Pedido { get; set; }

        [ForeignKey("IdProducto")]
        public Producto? Producto { get; set; }
    }
}