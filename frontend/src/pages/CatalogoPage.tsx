/**
 * Vista pública del Catálogo de Productos (HU-02).
 * Consulta los productos y categorías desde la API, permite filtrar por categoría,
 * verifica la disponibilidad de stock y añade artículos al carrito.
 */

import { useEffect, useState } from 'react';
import { catalogoApi } from '../services/api';
import type { Categoria, ItemCarrito, Producto } from '../types/ecommerce';

/** Propiedades recibidas desde App para sincronizar el carrito activo. */
interface CatalogoPageProps {
  carrito: ItemCarrito[];
  onAgregarAlCarrito: (producto: Producto) => void;
}

export function CatalogoPage({ carrito, onAgregarAlCarrito }: CatalogoPageProps) {
  const [categorias, setCategorias] = useState<Categoria[]>([]);
  const [productos, setProductos] = useState<Producto[]>([]);
  const [categoriaSeleccionada, setCategoriaSeleccionada] = useState<number | undefined>(undefined);
  const [cargando, setCargando] = useState(true);
  const [error, setError] = useState<string | null>(null);

  // Carga el listado de categorías disponibles al montar el componente
  useEffect(() => {
    const cargarCategorias = async () => {
      try {
        const data = await catalogoApi.listarCategorias();
        setCategorias(data);
      } catch (err) {
        setError(err instanceof Error ? err.message : 'Error al cargar categorías.');
      }
    };
    cargarCategorias();
  }, []);

  // Consulta los productos cada vez que cambia la categoría seleccionada
  useEffect(() => {
    const cargarProductos = async () => {
      setCargando(true);
      setError(null);
      try {
        const data = await catalogoApi.listarProductos(categoriaSeleccionada);
        setProductos(data);
      } catch (err) {
        setError(err instanceof Error ? err.message : 'No se pudo conectar con la API de productos.');
      } finally {
        setCargando(false);
      }
    };
    cargarProductos();
  }, [categoriaSeleccionada]);

  /** Retorna cuántas unidades de un producto específico ya están agregadas en el carrito. */
  const obtenerCantidadEnCarrito = (idProducto: number) => {
    const item = carrito.find((i) => i.producto.idProducto === idProducto);
    return item ? item.cantidad : 0;
  };

  return (
    <div>
      <div className="page-header">
        <div>
          <h1 className="page-title">Catálogo de Productos</h1>
          <p className="page-subtitle">Explora nuestros artículos disponibles y agrégalos a tu carrito.</p>
        </div>
      </div>

      {/* Barra de filtro por categoría (HU-02) */}
      <div className="filtros-bar">
        <button
          type="button"
          className={`btn ${categoriaSeleccionada === undefined ? 'btn-primary' : 'btn-secondary'}`}
          onClick={() => setCategoriaSeleccionada(undefined)}
        >
          Todas las categorías
        </button>

        {categorias.map((cat) => (
          <button
            key={cat.idCategoria}
            type="button"
            className={`btn ${categoriaSeleccionada === cat.idCategoria ? 'btn-primary' : 'btn-secondary'}`}
            onClick={() => setCategoriaSeleccionada(cat.idCategoria)}
          >
            {cat.nombre}
          </button>
        ))}
      </div>

      {error && <div className="alert alert-error">{error}</div>}

      {/* Cuadrícula de tarjetas de productos con control de inventario */}
      {cargando ? (
        <div className="card">Cargando productos desde PostgreSQL...</div>
      ) : productos.length === 0 ? (
        <div className="card">No hay productos registrados en esta categoría.</div>
      ) : (
        <div className="grid-productos">
          {productos.map((prod) => {
            const enCarrito = obtenerCantidadEnCarrito(prod.idProducto);
            const sinStockDisponible = prod.stock <= 0 || enCarrito >= prod.stock;

            return (
              <article key={prod.idProducto} className="card producto-card">
                <div>
                  <span className="producto-categoria">{prod.nombreCategoria}</span>
                  <h3 style={{ marginBottom: '0.35rem' }}>{prod.nombre}</h3>
                  <p className="page-subtitle" style={{ marginBottom: '0.85rem' }}>
                    {prod.descripcion || 'Sin descripción adicional.'}
                  </p>
                </div>

                <div>
                  <div
                    style={{
                      display: 'flex',
                      justifyContent: 'space-between',
                      alignItems: 'center',
                      marginBottom: '0.85rem',
                    }}
                  >
                    <span className="producto-precio">
                      ${prod.precio.toLocaleString('es-CO')}
                    </span>
                    <span className={`badge-stock ${prod.stock > 0 ? 'stock-ok' : 'stock-out'}`}>
                      {prod.stock > 0 ? `Stock: ${prod.stock}` : 'Agotado'}
                    </span>
                  </div>

                  <button
                    type="button"
                    className="btn btn-primary btn-block"
                    disabled={sinStockDisponible}
                    onClick={() => onAgregarAlCarrito(prod)}
                  >
                    {prod.stock <= 0
                      ? 'Sin inventario'
                      : enCarrito >= prod.stock
                        ? `Máximo en carrito (${enCarrito})`
                        : enCarrito > 0
                          ? `Agregar otro (${enCarrito} en carrito)`
                          : 'Agregar al carrito'}
                  </button>
                </div>
              </article>
            );
          })}
        </div>
      )}
    </div>
  );
}