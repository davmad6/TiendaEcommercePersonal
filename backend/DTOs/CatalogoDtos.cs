namespace TiendaEcommerce.Api.DTOs
{
    // ================= DTOs DE CATEGORÍAS =================

    /// <summary>
    /// Datos requeridos para crear o actualizar una categoría.
    /// </summary>
    public class GuardarCategoriaDto
    {
        public string Nombre { get; set; } = string.Empty;
    }

    /// <summary>
    /// Respuesta JSON enviada al cliente web para una categoría.
    /// </summary>
    public class CategoriaRespuestaDto
    {
        public int IdCategoria { get; set; }
        public string Nombre { get; set; } = string.Empty;
    }

    // ================= DTOs DE PRODUCTOS =================

    /// <summary>
    /// Datos requeridos para registrar o editar un producto.
    /// </summary>
    public class GuardarProductoDto
    {
        public string Nombre { get; set; } = string.Empty;
        public string? Descripcion { get; set; }
        public decimal Precio { get; set; }
        public int Stock { get; set; }
        public int IdCategoria { get; set; }
    }

    /// <summary>
    /// Respuesta JSON enviada al cliente web con los datos del producto y el nombre de su categoría.
    /// </summary>
    public class ProductoRespuestaDto
    {
        public int IdProducto { get; set; }
        public string Nombre { get; set; } = string.Empty;
        public string? Descripcion { get; set; }
        public decimal Precio { get; set; }
        public int Stock { get; set; }
        public int IdCategoria { get; set; }
        public string NombreCategoria { get; set; } = string.Empty;
    }
}