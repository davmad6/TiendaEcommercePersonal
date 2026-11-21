/**
 * Componente de barra de navegación principal (Navbar).
 * Permite cambiar entre las vistas de la tienda, visualizar el contador de artículos
 * en el carrito y gestionar el acceso o cierre de sesión del usuario.
 */

import type { Usuario } from '../types/ecommerce';

/** Identificadores de las vistas disponibles en la aplicación. */
export type VistaTienda = 'catalogo' | 'carrito' | 'pedidos' | 'admin' | 'auth';

/** Propiedades recibidas desde el componente raíz App. */
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
      {/* Marca o logotipo de la tienda */}
      <div className="navbar-brand">
        Tienda<span>Ecommerce</span>Personal
      </div>

      {/* Enlaces principales de navegación entre módulos */}
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
          {/* Muestra la insignia numérica solo cuando hay productos agregados */}
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

      {/* Sección derecha: saludo al usuario autenticado o botón de inicio de sesión */}
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