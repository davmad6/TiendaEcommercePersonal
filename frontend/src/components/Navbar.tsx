import type { Usuario } from '../types/ecommerce';

export type VistaTienda = 'catalogo' | 'carrito' | 'pedidos' | 'admin' | 'auth';

interface NavbarProps {
  vistaActiva: VistaTienda;
  onCambiarVista: (vista: VistaTienda) => void;
  usuario: Usuario | null;
  cantidadCarrito: number;
  onCerrarSesion: () => void;
}

export function Navbar({
  vistaActiva,
  onCambiarVista,
  usuario,
  cantidadCarrito,
  onCerrarSesion,
}: NavbarProps) {
  return (
    <header className="navbar">
      <div className="navbar-brand">
        Tienda<span>Ecommerce</span>Personal
      </div>

      <nav className="nav-links">
        <button
          type="button"
          className={`nav-btn ${vistaActiva === 'catalogo' ? 'active' : ''}`}
          onClick={() => onCambiarVista('catalogo')}
        >
          Catálogo
        </button>

        <button
          type="button"
          className={`nav-btn ${vistaActiva === 'carrito' ? 'active' : ''}`}
          onClick={() => onCambiarVista('carrito')}
        >
          Carrito
          {cantidadCarrito > 0 && <span className="badge-count">{cantidadCarrito}</span>}
        </button>

        <button
          type="button"
          className={`nav-btn ${vistaActiva === 'pedidos' ? 'active' : ''}`}
          onClick={() => onCambiarVista('pedidos')}
        >
          Pedidos
        </button>

        <button
          type="button"
          className={`nav-btn ${vistaActiva === 'admin' ? 'active' : ''}`}
          onClick={() => onCambiarVista('admin')}
        >
          Gestión Catálogo
        </button>
      </nav>

      <div className="nav-user">
        {usuario ? (
          <>
            <span>Hola, <strong>{usuario.nombre}</strong></span>
            <button type="button" className="btn btn-secondary btn-sm" onClick={onCerrarSesion}>
              Salir
            </button>
          </>
        ) : (
          <button
            type="button"
            className="btn btn-primary btn-sm"
            onClick={() => onCambiarVista('auth')}
          >
            Iniciar Sesión
          </button>
        )}
      </div>
    </header>
  );
}