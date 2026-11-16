import { useEffect, useState, type FormEvent } from 'react';
import { catalogoApi } from '../services/api';
import type { Categoria, Producto } from '../types/ecommerce';

export function AdminCatalogoPage() {
  const [categorias, setCategorias] = useState<Categoria[]>([]);
  const [productos, setProductos] = useState<Producto[]>([]);
  const [nombreCategoria, setNombreCategoria] = useState('');

  // Formulario de producto
  const [editandoId, setEditandoId] = useState<number | null>(null);
  const [nombreProd, setNombreProd] = useState('');
  const [descripcionProd, setDescripcionProd] = useState('');
  const [precioProd, setPrecioProd] = useState('');
  const [stockProd, setStockProd] = useState('');
  const [idCategoriaProd, setIdCategoriaProd] = useState<number>(1);

  const [mensaje, setMensaje] = useState<string | null>(null);
  const [error, setError] = useState<string | null>(null);

  const recargarDatos = async () => {
    try {
      const [cats, prods] = await Promise.all([
        catalogoApi.listarCategorias(),
        catalogoApi.listarProductos(),
      ]);
      setCategorias(cats);
      setProductos(prods);
      if (cats.length > 0) {
        setIdCategoriaProd((prev) =>
          cats.some((c) => c.idCategoria === prev) ? prev : cats[0].idCategoria
        );
      }
    } catch (err) {
      setError(err instanceof Error ? err.message : 'Error al cargar datos del catálogo.');
    }
  };

  useEffect(() => {
    let activo = true;

    const inicializar = async () => {
      try {
        const [cats, prods] = await Promise.all([
          catalogoApi.listarCategorias(),
          catalogoApi.listarProductos(),
        ]);
        if (!activo) return;
        setCategorias(cats);
        setProductos(prods);
        if (cats.length > 0) {
          setIdCategoriaProd((prev) =>
            cats.some((c) => c.idCategoria === prev) ? prev : cats[0].idCategoria
          );
        }
      } catch (err) {
        if (activo) {
          setError(err instanceof Error ? err.message : 'Error al cargar datos del catálogo.');
        }
      }
    };

    inicializar();

    return () => {
      activo = false;
    };
  }, []);

  const limpiarFormularioProducto = () => {
    setEditandoId(null);
    setNombreProd('');
    setDescripcionProd('');
    setPrecioProd('');
    setStockProd('');
  };

  const guardarCategoria = async (e: FormEvent) => {
    e.preventDefault();
    setMensaje(null);
    setError(null);
    try {
      await catalogoApi.crearCategoria({ nombre: nombreCategoria });
      setNombreCategoria('');
      setMensaje('Categoría creada correctamente.');
      await recargarDatos();
    } catch (err) {
      setError(err instanceof Error ? err.message : 'Error al crear categoría.');
    }
  };

  const guardarProducto = async (e: FormEvent) => {
    e.preventDefault();
    setMensaje(null);
    setError(null);

    const payload = {
      nombre: nombreProd,
      descripcion: descripcionProd.trim() || undefined,
      precio: Number(precioProd),
      stock: Number(stockProd),
      idCategoria: Number(idCategoriaProd),
    };

    try {
      if (editandoId !== null) {
        await catalogoApi.actualizarProducto(editandoId, payload);
        setMensaje('Producto actualizado correctamente.');
      } else {
        await catalogoApi.crearProducto(payload);
        setMensaje('Producto registrado correctamente.');
      }
      limpiarFormularioProducto();
      await recargarDatos();
    } catch (err) {
      setError(err instanceof Error ? err.message : 'Error al guardar el producto.');
    }
  };

  const cargarParaEditar = (prod: Producto) => {
    setEditandoId(prod.idProducto);
    setNombreProd(prod.nombre);
    setDescripcionProd(prod.descripcion || '');
    setPrecioProd(String(prod.precio));
    setStockProd(String(prod.stock));
    setIdCategoriaProd(prod.idCategoria);
    setMensaje(null);
    setError(null);
  };

  const eliminarProducto = async (id: number) => {
    setMensaje(null);
    setError(null);
    try {
      await catalogoApi.eliminarProducto(id);
      setMensaje('Producto eliminado.');
      await recargarDatos();
    } catch (err) {
      setError(err instanceof Error ? err.message : 'No se pudo eliminar el producto.');
    }
  };

  return (
    <div>
      <div className="page-header">
        <div>
          <h1 className="page-title">Administración de Catálogo (HU-02)</h1>
          <p className="page-subtitle">Crea categorías y administra el inventario de productos en PostgreSQL.</p>
        </div>
      </div>

      {mensaje && <div className="alert alert-success">{mensaje}</div>}
      {error && <div className="alert alert-error">{error}</div>}

      <div className="grid-dos-columnas" style={{ marginBottom: '1.5rem' }}>
        {/* Crear Categoría */}
        <div className="card">
          <h3 style={{ marginBottom: '0.85rem' }}>Nueva Categoría</h3>
          <form onSubmit={guardarCategoria}>
            <div className="form-group">
              <label htmlFor="catNombre">Nombre de la categoría *</label>
              <input
                id="catNombre"
                type="text"
                className="form-control"
                value={nombreCategoria}
                onChange={(e) => setNombreCategoria(e.target.value)}
                required
                placeholder="Ej: Periféricos"
              />
            </div>
            <button type="submit" className="btn btn-primary">
              Guardar Categoría
            </button>
          </form>
        </div>

        {/* Crear / Editar Producto */}
        <div className="card">
          <h3 style={{ marginBottom: '0.85rem' }}>
            {editandoId !== null ? `Editando Producto #${editandoId}` : 'Registrar Producto'}
          </h3>
          <form onSubmit={guardarProducto}>
            <div className="form-group">
              <label htmlFor="prodNombre">Nombre del producto *</label>
              <input
                id="prodNombre"
                type="text"
                className="form-control"
                value={nombreProd}
                onChange={(e) => setNombreProd(e.target.value)}
                required
              />
            </div>

            <div className="form-group">
              <label htmlFor="prodDesc">Descripción</label>
              <input
                id="prodDesc"
                type="text"
                className="form-control"
                value={descripcionProd}
                onChange={(e) => setDescripcionProd(e.target.value)}
              />
            </div>

            <div className="grid-dos-columnas">
              <div className="form-group">
                <label htmlFor="prodPrecio">Precio (COP) *</label>
                <input
                  id="prodPrecio"
                  type="number"
                  min="1"
                  step="any"
                  className="form-control"
                  value={precioProd}
                  onChange={(e) => setPrecioProd(e.target.value)}
                  required
                />
              </div>

              <div className="form-group">
                <label htmlFor="prodStock">Stock inicial *</label>
                <input
                  id="prodStock"
                  type="number"
                  min="0"
                  className="form-control"
                  value={stockProd}
                  onChange={(e) => setStockProd(e.target.value)}
                  required
                />
              </div>
            </div>

            <div className="form-group">
              <label htmlFor="prodCat">Categoría *</label>
              <select
                id="prodCat"
                className="form-control"
                value={idCategoriaProd}
                onChange={(e) => setIdCategoriaProd(Number(e.target.value))}
              >
                {categorias.map((c) => (
                  <option key={c.idCategoria} value={c.idCategoria}>
                    {c.nombre}
                  </option>
                ))}
              </select>
            </div>

            <div style={{ display: 'flex', gap: '0.5rem' }}>
              <button type="submit" className="btn btn-primary">
                {editandoId !== null ? 'Actualizar Producto' : 'Crear Producto'}
              </button>
              {editandoId !== null && (
                <button
                  type="button"
                  className="btn btn-secondary"
                  onClick={limpiarFormularioProducto}
                >
                  Cancelar
                </button>
              )}
            </div>
          </form>
        </div>
      </div>

      {/* Tabla de Productos */}
      <div className="card">
        <h3 style={{ marginBottom: '0.85rem' }}>Inventario Actual</h3>
        <div className="table-responsive">
          <table className="tabla">
            <thead>
              <tr>
                <th>ID</th>
                <th>Nombre</th>
                <th>Categoría</th>
                <th>Precio</th>
                <th>Stock</th>
                <th>Acciones</th>
              </tr>
            </thead>
            <tbody>
              {productos.map((p) => (
                <tr key={p.idProducto}>
                  <td>{p.idProducto}</td>
                  <td>{p.nombre}</td>
                  <td>{p.nombreCategoria}</td>
                  <td>${p.precio.toLocaleString('es-CO')}</td>
                  <td>{p.stock}</td>
                  <td>
                    <div style={{ display: 'flex', gap: '0.4rem' }}>
                      <button
                        type="button"
                        className="btn btn-secondary btn-sm"
                        onClick={() => cargarParaEditar(p)}
                      >
                        Editar
                      </button>
                      <button
                        type="button"
                        className="btn btn-danger btn-sm"
                        onClick={() => eliminarProducto(p.idProducto)}
                      >
                        Eliminar
                      </button>
                    </div>
                  </td>
                </tr>
              ))}
            </tbody>
          </table>
        </div>
      </div>
    </div>
  );
}