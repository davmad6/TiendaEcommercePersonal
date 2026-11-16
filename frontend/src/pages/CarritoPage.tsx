import { useState } from 'react';
import { pedidosApi } from '../services/api';
import type { ItemCarrito, Usuario } from '../types/ecommerce';

interface CarritoPageProps {
  carrito: ItemCarrito[];
  usuario: Usuario | null;
  onActualizarCantidad: (idProducto: number, nuevaCantidad: number) => void;
  onEliminarItem: (idProducto: number) => void;
  onVaciarCarrito: () => void;
  onIrALogin: () => void;
  onPedidoConfirmado: () => void;
}

export function CarritoPage({
  carrito,
  usuario,
  onActualizarCantidad,
  onEliminarItem,
  onVaciarCarrito,
  onIrALogin,
  onPedidoConfirmado,
}: CarritoPageProps) {
  const [procesando, setProcesando] = useState(false);
  const [error, setError] = useState<string | null>(null);

  const totalCompra = carrito.reduce(
    (acum, item) => acum + item.producto.precio * item.cantidad,
    0
  );

  const confirmarPedido = async () => {
    if (!usuario) {
      onIrALogin();
      return;
    }

    setError(null);
    setProcesando(true);

    try {
      await pedidosApi.crear({
        idUsuario: usuario.idUsuario,
        items: carrito.map((item) => ({
          idProducto: item.producto.idProducto,
          cantidad: item.cantidad,
        })),
      });
      onVaciarCarrito();
      onPedidoConfirmado();
    } catch (err) {
      setError(err instanceof Error ? err.message : 'No se pudo registrar el pedido.');
    } finally {
      setProcesando(false);
    }
  };

  return (
    <div>
      <div className="page-header">
        <div>
          <h1 className="page-title">Carrito de Compras</h1>
          <p className="page-subtitle">Revisa las cantidades antes de generar tu orden en PostgreSQL.</p>
        </div>

        {carrito.length > 0 && (
          <button type="button" className="btn btn-secondary" onClick={onVaciarCarrito}>
            Vaciar carrito
          </button>
        )}
      </div>

      {error && <div className="alert alert-error">{error}</div>}

      {carrito.length === 0 ? (
        <div className="card">
          <p>Tu carrito está vacío. Agrega productos desde el catálogo para iniciar un pedido.</p>
        </div>
      ) : (
        <div className="card">
          <div className="table-responsive">
            <table className="tabla">
              <thead>
                <tr>
                  <th>Producto</th>
                  <th>Precio Unitario</th>
                  <th>Cantidad</th>
                  <th>Subtotal</th>
                  <th>Acción</th>
                </tr>
              </thead>
              <tbody>
                {carrito.map(({ producto, cantidad }) => (
                  <tr key={producto.idProducto}>
                    <td>
                      <strong>{producto.nombre}</strong>
                      <div className="page-subtitle">{producto.nombreCategoria}</div>
                    </td>
                    <td>${producto.precio.toLocaleString('es-CO')}</td>
                    <td>
                      <div style={{ display: 'flex', alignItems: 'center', gap: '0.45rem' }}>
                        <button
                          type="button"
                          className="btn btn-secondary btn-sm"
                          onClick={() => onActualizarCantidad(producto.idProducto, cantidad - 1)}
                        >
                          -
                        </button>
                        <strong>{cantidad}</strong>
                        <button
                          type="button"
                          className="btn btn-secondary btn-sm"
                          disabled={cantidad >= producto.stock}
                          onClick={() => onActualizarCantidad(producto.idProducto, cantidad + 1)}
                        >
                          +
                        </button>
                      </div>
                    </td>
                    <td>
                      <strong>${(producto.precio * cantidad).toLocaleString('es-CO')}</strong>
                    </td>
                    <td>
                      <button
                        type="button"
                        className="btn btn-danger btn-sm"
                        onClick={() => onEliminarItem(producto.idProducto)}
                      >
                        Quitar
                      </button>
                    </td>
                  </tr>
                ))}
              </tbody>
            </table>
          </div>

          <div
            style={{
              marginTop: '1.35rem',
              display: 'flex',
              flexWrap: 'wrap',
              justifyContent: 'space-between',
              alignItems: 'center',
              gap: '1rem',
            }}
          >
            <div>
              <span className="page-subtitle">Total a pagar:</span>
              <div style={{ fontSize: '1.6rem', fontWeight: 700 }}>
                ${totalCompra.toLocaleString('es-CO')}
              </div>
            </div>

            {usuario ? (
              <button
                type="button"
                className="btn btn-primary"
                disabled={procesando}
                onClick={confirmarPedido}
              >
                {procesando
                  ? 'Registrando pedido...'
                  : `Confirmar pedido como ${usuario.nombre}`}
              </button>
            ) : (
              <button type="button" className="btn btn-primary" onClick={onIrALogin}>
                Inicia sesión para confirmar compra
              </button>
            )}
          </div>
        </div>
      )}
    </div>
  );
}