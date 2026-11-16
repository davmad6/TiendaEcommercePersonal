import { useState } from 'react';
import { Navbar, type VistaTienda } from './components/Navbar';
import { AdminCatalogoPage } from './pages/AdminCatalogoPage';
import { AuthPage } from './pages/AuthPage';
import { CarritoPage } from './pages/CarritoPage';
import { CatalogoPage } from './pages/CatalogoPage';
import { PedidosPage } from './pages/PedidosPage';
import type { ItemCarrito, Producto, Usuario } from './types/ecommerce';

export function App() {
  const [vistaActiva, setVistaActiva] = useState<VistaTienda>('catalogo');
  const [usuario, setUsuario] = useState<Usuario | null>(null);
  const [carrito, setCarrito] = useState<ItemCarrito[]>([]);

  const cantidadTotalCarrito = carrito.reduce((total, item) => total + item.cantidad, 0);

  const agregarAlCarrito = (producto: Producto) => {
    setCarrito((prev) => {
      const existente = prev.find((item) => item.producto.idProducto === producto.idProducto);
      if (existente) {
        if (existente.cantidad >= producto.stock) return prev;
        return prev.map((item) =>
          item.producto.idProducto === producto.idProducto
            ? { ...item, cantidad: item.cantidad + 1 }
            : item
        );
      }
      return [...prev, { producto, cantidad: 1 }];
    });
  };

  const actualizarCantidadCarrito = (idProducto: number, nuevaCantidad: number) => {
    if (nuevaCantidad <= 0) {
      eliminarDelCarrito(idProducto);
      return;
    }

    setCarrito((prev) =>
      prev.map((item) => {
        if (item.producto.idProducto !== idProducto) return item;
        const cantidadSegura = Math.min(nuevaCantidad, item.producto.stock);
        return { ...item, cantidad: cantidadSegura };
      })
    );
  };

  const eliminarDelCarrito = (idProducto: number) => {
    setCarrito((prev) => prev.filter((item) => item.producto.idProducto !== idProducto));
  };

  const vaciarCarrito = () => setCarrito([]);

  return (
    <div>
      <Navbar
        vistaActiva={vistaActiva}
        onCambiarVista={setVistaActiva}
        usuario={usuario}
        cantidadCarrito={cantidadTotalCarrito}
        onCerrarSesion={() => {
          setUsuario(null);
          setVistaActiva('catalogo');
        }}
      />

      <main className="container">
        {vistaActiva === 'catalogo' && (
          <CatalogoPage carrito={carrito} onAgregarAlCarrito={agregarAlCarrito} />
        )}

        {vistaActiva === 'carrito' && (
          <CarritoPage
            carrito={carrito}
            usuario={usuario}
            onActualizarCantidad={actualizarCantidadCarrito}
            onEliminarItem={eliminarDelCarrito}
            onVaciarCarrito={vaciarCarrito}
            onIrALogin={() => setVistaActiva('auth')}
            onPedidoConfirmado={() => setVistaActiva('pedidos')}
          />
        )}

        {vistaActiva === 'pedidos' && <PedidosPage usuario={usuario} />}

        {vistaActiva === 'admin' && <AdminCatalogoPage />}

        {vistaActiva === 'auth' && (
          <AuthPage
            onAutenticado={(usr) => {
              setUsuario(usr);
              setVistaActiva('catalogo');
            }}
          />
        )}
      </main>
    </div>
  );
}

export default App;