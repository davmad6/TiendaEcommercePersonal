/**
 * Vista de Autenticación y Registro de Usuarios (HU-01).
 * Alterna entre el formulario de inicio de sesión y el de creación de cuenta,
 * devolviendo el usuario autenticado al contenedor principal App.
 */

import { useState, type SyntheticEvent } from 'react';
import { usuariosApi } from '../services/api';
import type { Usuario } from '../types/ecommerce';

/** Propiedades recibidas desde App para notificar una autenticación exitosa. */
interface AuthPageProps {
  onAutenticado: (usuario: Usuario) => void;
}

export function AuthPage({ onAutenticado }: AuthPageProps) {
  // Estados para controlar el modo (login o registro) y los campos del formulario
  const [modoRegistro, setModoRegistro] = useState(false);
  const [nombre, setNombre] = useState('');
  const [email, setEmail] = useState('');
  const [contrasena, setContrasena] = useState('');
  const [direccion, setDireccion] = useState('');
  const [telefono, setTelefono] = useState('');
  const [cargando, setCargando] = useState(false);
  const [error, setError] = useState<string | null>(null);

  /** Procesa el envío del formulario ejecutando el registro o el inicio de sesión. */
  const manejarEnvio = async (e: SyntheticEvent) => {
    e.preventDefault();
    setError(null);
    setCargando(true);

    try {
      if (modoRegistro) {
        const res = await usuariosApi.registrar({
          nombre,
          email,
          contrasena,
          direccion: direccion.trim() || undefined,
          telefono: telefono.trim() || undefined,
        });
        onAutenticado(res.usuario);
      } else {
        const res = await usuariosApi.login({ email, contrasena });
        onAutenticado(res.usuario);
      }
    } catch (err) {
      setError(err instanceof Error ? err.message : 'Ocurrió un error al autenticar.');
    } finally {
      setCargando(false);
    }
  };

  return (
    <div className="auth-wrapper">
      <div className="card">
        {/* Pestañas para alternar entre Iniciar Sesión y Crear Cuenta */}
        <div className="auth-tabs">
          <button
            type="button"
            className={`btn ${!modoRegistro ? 'btn-primary' : 'btn-secondary'}`}
            onClick={() => {
              setModoRegistro(false);
              setError(null);
            }}
          >
            Iniciar Sesión
          </button>
          <button
            type="button"
            className={`btn ${modoRegistro ? 'btn-primary' : 'btn-secondary'}`}
            onClick={() => {
              setModoRegistro(true);
              setError(null);
            }}
          >
            Crear Cuenta
          </button>
        </div>

        <h2 style={{ marginBottom: '0.35rem' }}>
          {modoRegistro ? 'Registro de Cliente (HU-01)' : 'Acceso de Usuario (HU-01)'}
        </h2>
        <p className="page-subtitle" style={{ marginBottom: '1.1rem' }}>
          {modoRegistro
            ? 'Completa tus datos para comprar en la tienda.'
            : 'Ingresa con tu correo registrado para confirmar pedidos.'}
        </p>

        {error && <div className="alert alert-error">{error}</div>}

        <form onSubmit={manejarEnvio}>
          {/* Campo de nombre visible solo en modo registro */}
          {modoRegistro && (
            <div className="form-group">
              <label htmlFor="nombre">Nombre completo *</label>
              <input
                id="nombre"
                type="text"
                className="form-control"
                value={nombre}
                onChange={(e) => setNombre(e.target.value)}
                required
                placeholder="Ej: David Madrid"
              />
            </div>
          )}

          <div className="form-group">
            <label htmlFor="email">Correo electrónico *</label>
            <input
              id="email"
              type="email"
              className="form-control"
              value={email}
              onChange={(e) => setEmail(e.target.value)}
              required
              placeholder="david@tienda.com"
            />
          </div>

          <div className="form-group">
            <label htmlFor="contrasena">Contraseña *</label>
            <input
              id="contrasena"
              type="password"
              className="form-control"
              value={contrasena}
              onChange={(e) => setContrasena(e.target.value)}
              required
              placeholder="••••••••"
            />
          </div>

          {/* Campos opcionales de envío disponibles al crear cuenta */}
          {modoRegistro && (
            <>
              <div className="form-group">
                <label htmlFor="direccion">Dirección de envío</label>
                <input
                  id="direccion"
                  type="text"
                  className="form-control"
                  value={direccion}
                  onChange={(e) => setDireccion(e.target.value)}
                  placeholder="Calle 10 # 20-30, Medellín"
                />
              </div>

              <div className="form-group">
                <label htmlFor="telefono">Teléfono</label>
                <input
                  id="telefono"
                  type="text"
                  className="form-control"
                  value={telefono}
                  onChange={(e) => setTelefono(e.target.value)}
                  placeholder="3001234567"
                />
              </div>
            </>
          )}

          <button type="submit" className="btn btn-primary btn-block" disabled={cargando}>
            {cargando
              ? 'Procesando...'
              : modoRegistro
                ? 'Registrarme y entrar'
                : 'Ingresar'}
          </button>
        </form>
      </div>
    </div>
  );
}