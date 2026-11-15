using TiendaEcommerce.Api.DTOs;
using TiendaEcommerce.Api.Models;
using TiendaEcommerce.Api.Repositories;

namespace TiendaEcommerce.Api.Services
{
    /// <summary>
    /// Contrato de reglas de negocio para el catálogo de categorías y productos (HU-02).
    /// </summary>
    public interface ICatalogoService
    {
        // Categorías
        Task<List<CategoriaRespuestaDto>> ObtenerCategoriasAsync();
        Task<CategoriaRespuestaDto?> ObtenerCategoriaPorIdAsync(int id);
        Task<(bool Exito, string Mensaje, CategoriaRespuestaDto? Datos)> CrearCategoriaAsync(GuardarCategoriaDto dto);
        Task<(bool Exito, string Mensaje, CategoriaRespuestaDto? Datos)> ActualizarCategoriaAsync(int id, GuardarCategoriaDto dto);
        Task<(bool Exito, string Mensaje)> EliminarCategoriaAsync(int id);

        // Productos
        Task<List<ProductoRespuestaDto>> ObtenerProductosAsync(int? idCategoria = null);
        Task<ProductoRespuestaDto?> ObtenerProductoPorIdAsync(int id);
        Task<(bool Exito, string Mensaje, ProductoRespuestaDto? Datos)> CrearProductoAsync(GuardarProductoDto dto);
        Task<(bool Exito, string Mensaje, ProductoRespuestaDto? Datos)> ActualizarProductoAsync(int id, GuardarProductoDto dto);
        Task<(bool Exito, string Mensaje)> EliminarProductoAsync(int id);
    }

    /// <summary>
    /// Valida integridad referencial, precios, inventario y transforma entidades en DTOs.
    /// </summary>
    public class CatalogoService : ICatalogoService
    {
        private readonly ICatalogoRepository _catalogoRepository;

        public CatalogoService(ICatalogoRepository catalogoRepository)
        {
            _catalogoRepository = catalogoRepository;
        }

        // ================= CATEGORÍAS =================

        public async Task<List<CategoriaRespuestaDto>> ObtenerCategoriasAsync()
        {
            var categorias = await _catalogoRepository.ObtenerCategoriasAsync();
            return categorias.Select(MapearCategoria).ToList();
        }

        public async Task<CategoriaRespuestaDto?> ObtenerCategoriaPorIdAsync(int id)
        {
            var categoria = await _catalogoRepository.ObtenerCategoriaPorIdAsync(id);
            return categoria == null ? null : MapearCategoria(categoria);
        }

        public async Task<(bool Exito, string Mensaje, CategoriaRespuestaDto? Datos)> CrearCategoriaAsync(GuardarCategoriaDto dto)
        {
            if (string.IsNullOrWhiteSpace(dto.Nombre))
            {
                return (false, "El nombre de la categoría es obligatorio.", null);
            }

            var nueva = new Categoria
            {
                Nombre = dto.Nombre.Trim()
            };

            var creada = await _catalogoRepository.CrearCategoriaAsync(nueva);
            return (true, "Categoría creada correctamente.", MapearCategoria(creada));
        }

        public async Task<(bool Exito, string Mensaje, CategoriaRespuestaDto? Datos)> ActualizarCategoriaAsync(int id, GuardarCategoriaDto dto)
        {
            var existente = await _catalogoRepository.ObtenerCategoriaPorIdAsync(id);
            if (existente == null)
            {
                return (false, "Categoría no encontrada.", null);
            }

            if (string.IsNullOrWhiteSpace(dto.Nombre))
            {
                return (false, "El nombre de la categoría es obligatorio.", null);
            }

            existente.Nombre = dto.Nombre.Trim();

            await _catalogoRepository.ActualizarCategoriaAsync(existente);
            return (true, "Categoría actualizada correctamente.", MapearCategoria(existente));
        }

        public async Task<(bool Exito, string Mensaje)> EliminarCategoriaAsync(int id)
        {
            var existente = await _catalogoRepository.ObtenerCategoriaPorIdAsync(id);
            if (existente == null)
            {
                return (false, "Categoría no encontrada.");
            }

            if (await _catalogoRepository.CategoriaTieneProductosAsync(id))
            {
                return (false, "No se puede eliminar la categoría porque tiene productos asociados.");
            }

            await _catalogoRepository.EliminarCategoriaAsync(existente);
            return (true, "Categoría eliminada correctamente.");
        }

        // ================= PRODUCTOS =================

        public async Task<List<ProductoRespuestaDto>> ObtenerProductosAsync(int? idCategoria = null)
        {
            var productos = await _catalogoRepository.ObtenerProductosAsync(idCategoria);
            return productos.Select(MapearProducto).ToList();
        }

        public async Task<ProductoRespuestaDto?> ObtenerProductoPorIdAsync(int id)
        {
            var producto = await _catalogoRepository.ObtenerProductoPorIdAsync(id);
            return producto == null ? null : MapearProducto(producto);
        }

        public async Task<(bool Exito, string Mensaje, ProductoRespuestaDto? Datos)> CrearProductoAsync(GuardarProductoDto dto)
        {
            if (string.IsNullOrWhiteSpace(dto.Nombre))
            {
                return (false, "El nombre del producto es obligatorio.", null);
            }

            if (dto.Precio <= 0)
            {
                return (false, "El precio del producto debe ser mayor a cero.", null);
            }

            if (dto.Stock < 0)
            {
                return (false, "El stock no puede ser negativo.", null);
            }

            var categoria = await _catalogoRepository.ObtenerCategoriaPorIdAsync(dto.IdCategoria);
            if (categoria == null)
            {
                return (false, $"La categoría con ID {dto.IdCategoria} no existe.", null);
            }

            var nuevo = new Producto
            {
                Nombre = dto.Nombre.Trim(),
                Descripcion = string.IsNullOrWhiteSpace(dto.Descripcion) ? null : dto.Descripcion.Trim(),
                Precio = dto.Precio,
                Stock = dto.Stock,
                IdCategoria = dto.IdCategoria
            };

            var creado = await _catalogoRepository.CrearProductoAsync(nuevo);
            return (true, "Producto registrado correctamente.", MapearProducto(creado));
        }

        public async Task<(bool Exito, string Mensaje, ProductoRespuestaDto? Datos)> ActualizarProductoAsync(int id, GuardarProductoDto dto)
        {
            var existente = await _catalogoRepository.ObtenerProductoPorIdAsync(id);
            if (existente == null)
            {
                return (false, "Producto no encontrado.", null);
            }

            if (string.IsNullOrWhiteSpace(dto.Nombre))
            {
                return (false, "El nombre del producto es obligatorio.", null);
            }

            if (dto.Precio <= 0)
            {
                return (false, "El precio del producto debe ser mayor a cero.", null);
            }

            if (dto.Stock < 0)
            {
                return (false, "El stock no puede ser negativo.", null);
            }

            var categoria = await _catalogoRepository.ObtenerCategoriaPorIdAsync(dto.IdCategoria);
            if (categoria == null)
            {
                return (false, $"La categoría con ID {dto.IdCategoria} no existe.", null);
            }

            existente.Nombre = dto.Nombre.Trim();
            existente.Descripcion = string.IsNullOrWhiteSpace(dto.Descripcion) ? null : dto.Descripcion.Trim();
            existente.Precio = dto.Precio;
            existente.Stock = dto.Stock;
            existente.IdCategoria = dto.IdCategoria;
            existente.Categoria = categoria;

            await _catalogoRepository.ActualizarProductoAsync(existente);
            return (true, "Producto actualizado correctamente.", MapearProducto(existente));
        }

        public async Task<(bool Exito, string Mensaje)> EliminarProductoAsync(int id)
        {
            var existente = await _catalogoRepository.ObtenerProductoPorIdAsync(id);
            if (existente == null)
            {
                return (false, "Producto no encontrado.");
            }

            await _catalogoRepository.EliminarProductoAsync(existente);
            return (true, "Producto eliminado correctamente.");
        }

        private static CategoriaRespuestaDto MapearCategoria(Categoria c) => new()
        {
            IdCategoria = c.IdCategoria,
            Nombre = c.Nombre
        };

        private static ProductoRespuestaDto MapearProducto(Producto p) => new()
        {
            IdProducto = p.IdProducto,
            Nombre = p.Nombre,
            Descripcion = p.Descripcion,
            Precio = p.Precio,
            Stock = p.Stock,
            IdCategoria = p.IdCategoria,
            NombreCategoria = p.Categoria?.Nombre ?? "Sin categoría"
        };
    }
}