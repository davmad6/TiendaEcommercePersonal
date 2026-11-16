import { useEffect, useState } from 'react';
import { pedidosApi } from '../services/api';
import type { Pedido, Usuario } from '../types/ecommerce';

interface PedidosPageProps {
  usuario: Usuario | null;
}

const ESTADOS_DISPONIBLES = ['Pendiente', 'Pagado', 'Enviado', 'Entregado', 'Cancelado'];

/**
 * Vista principal del módulo de Pedidos (HU-03).
 * Permite consultar el historial de órdenes, filtrar por usuario autenticado
 * y gestionar los cambios de estado logístico conectando con la API REST.
 */
export function PedidosPage({ usuario }: PedidosPageProps) {
  const [pedidos, setPedidos] = useState<Pedido[]>([]);
  const [soloMisPedidos, setSoloMisPedidos] = useState(false);
  const [cargando, setCargando] = useState(true);
  const [mensaje, setMensaje] = useState<string | null>(null);
  const [error, setError] = useState<string | null>(null);

  // Consulta los pedidos registrados al montar el componente o al alternar el filtro de usuario
  useEffect(() => {
    let activo = true;

    const obtenerPedidos = async () => {
      setError(null);
      try {
        const filtroUsuario = soloMisPedidos && usuario ? usuario.idUsuario : undefined;
        const data = await pedidosApi.listar(filtroUsuario);
        if (activo) {
          setPedidos(data);
        }
      } catch (err) {
        if (activo) {
          setError(err instanceof Error ? err.message : 'Error al consultar pedidos.');
        }
      } finally {
        if (activo) {
          setCargando(false);
        }
      }
    };

    obtenerPedidos();

    return () => {
      activo = false;
    };
  }, [soloMisPedidos, usuario]);

  /**
   * Envía petición PUT al backend para actualizar el estado de un pedido
   * y recarga el listado (si se cancela, el backend restaura el stock automáticamente).
   */
  const cambiarEstado = async (idPedido: number, nuevoEstado: string) => {
    setMensaje(null);
    setError(null);
    try {
      const res = await pedidosApi.actualizarEstado(idPedido, nuevoEstado);
      setMensaje(res.mensaje);
      const filtroUsuario = soloMisPedidos && usuario ? usuario.idUsuario : undefined;
      const actualizados = await pedidosApi.listar(filtroUsuario);
      setPedidos(actualizados);
    } catch (err) {
      setError(err instanceof Error ? err.message : 'No se pudo actualizar el estado.');
    }
  };

  return (
    <div>
      <div className="page-header">
        <div>
          <h1 className="page-title">Historial y Gestión de Pedidos (HU-03)</h1>
          <p className="page-subtitle">
            Consulta órdenes registradas y actualiza su estado (cancelar devuelve el stock automáticamente).
          </p>
        </div>

        {usuario && (
          <button
            type="button"
            className={`btn ${soloMisPedidos ? 'btn-primary' : 'btn-secondary'}`}
            onClick={() => {
              setCargando(true);
              setSoloMisPedidos(!soloMisPedidos);
            }}
          >
            {soloMisPedidos ? `Mostrando pedidos de ${usuario.nombre}` : 'Ver todos los pedidos'}
          </button>
        )}
      </div>

      {mensaje && <div className="alert alert-success">{mensaje}</div>}
      {error && <div className="alert alert-error">{error}</div>}

      {cargando ? (
        <div className="card">Cargando pedidos...</div>
      ) : pedidos.length === 0 ? (
        <div className="card">No hay pedidos registrados para mostrar.</div>
      ) : (
        <div style={{ display: 'flex', flexDirection: 'column', gap: '1rem' }}>
          {pedidos.map((pedido) => (
            <div key={pedido.idPedido} className="card">
              <div
                style={{
                  display: 'flex',
                  flexWrap: 'wrap',
                  justifyContent: 'space-between',
                  alignItems: 'center',
                  gap: '0.75rem',
                  marginBottom: '0.95rem',
                  paddingBottom: '0.75rem',
                  borderBottom: '1px solid var(--border)',
                }}
              >
                <div>
                  <strong>Pedido #{pedido.idPedido}</strong> — Cliente: {pedido.nombreUsuario}
                  <div className="page-subtitle">
                    Fecha: {new Date(pedido.fecha).toLocaleString('es-CO')}
                  </div>
                </div>

                <div style={{ display: 'flex', alignItems: 'center', gap: '0.75rem' }}>
                  <span className="badge-estado">{pedido.estado}</span>

                  <select
                    aria-label={`Estado pedido ${pedido.idPedido}`}
                    className="form-control"
                    style={{ width: 'auto' }}
                    value={pedido.estado}
                    disabled={pedido.estado === 'Cancelado'}
                    onChange={(e) => cambiarEstado(pedido.idPedido, e.target.value)}
                  >
                    {ESTADOS_DISPONIBLES.map((est) => (
                      <option key={est} value={est}>
                        {est}
                      </option>
                    ))}
                  </select>
                </div>
              </div>

              <div className="table-responsive">
                <table className="tabla">
                  <thead>
                    <tr>
                      <th>Producto</th>
                      <th>Cantidad</th>
                      <th>Precio Unitario</th>
                      <th>Subtotal</th>
                    </tr>
                  </thead>
                  <tbody>
                    {pedido.detalles.map((det) => (
                      <tr key={det.idDetalle}>
                        <td>{det.nombreProducto}</td>
                        <td>{det.cantidad}</td>
                        <td>${det.precioUnitario.toLocaleString('es-CO')}</td>
                        <td>${det.subtotal.toLocaleString('es-CO')}</td>
                      </tr>
                    ))}
                  </tbody>
                </table>
              </div>

              <div style={{ textAlign: 'right', marginTop: '0.75rem', fontSize: '1.1rem' }}>
                Total del pedido: <strong>${pedido.total.toLocaleString('es-CO')}</strong>
              </div>
            </div>
          ))}
        </div>
      )}
    </div>
  );
}