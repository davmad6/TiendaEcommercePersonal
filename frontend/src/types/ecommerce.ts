/**
 * Definición de interfaces y tipos de TypeScript para TiendaEcommercePersonal.
 * Asegura la integridad de los datos intercambiados entre React y la API REST en .NET.
 */

// ================= USUARIOS (HU-01) =================

/** Representa los datos públicos de un usuario autenticado en el sistema. */
export interface Usuario {
  idUsuario: number;
  nombre: string;
  email: string;
  direccion?: string | null;
  telefono?: string | null;
}

/** Datos requeridos para registrar una nueva cuenta de cliente. */
export interface RegistroUsuarioPayload {
  nombre: string;
  email: string;
  contrasena: string;
  direccion?: string;
  telefono?: string;
}

/** Credenciales necesarias para iniciar sesión. */
export interface LoginUsuarioPayload {
  email: string;
  contrasena: string;
}

// ================= CATÁLOGO (HU-02) =================

/** Representa una categoría del catálogo de productos. */
export interface Categoria {
  idCategoria: number;
  nombre: string;
}

/** Carga útil para crear o actualizar una categoría. */
export interface GuardarCategoriaPayload {
  nombre: string;
}

/** Representa un producto disponible en la tienda con su categoría asociada. */
export interface Producto {
  idProducto: number;
  nombre: string;
  descripcion?: string | null;
  precio: number;
  stock: number;
  idCategoria: number;
  nombreCategoria: string;
}

/** Datos enviados al backend para registrar o editar un producto. */
export interface GuardarProductoPayload {
  nombre: string;
  descripcion?: string;
  precio: number;
  stock: number;
  idCategoria: number;
}

// ================= PEDIDOS Y CARRITO (HU-03) =================

/** Elemento almacenado en el estado local del carrito de compras. */
export interface ItemCarrito {
  producto: Producto;
  cantidad: number;
}

/** Estructura enviada para confirmar y registrar una nueva orden de compra. */
export interface CrearPedidoPayload {
  idUsuario: number;
  items: {
    idProducto: number;
    cantidad: number;
  }[];
}

/** Línea individual de detalle dentro de un pedido registrado. */
export interface DetallePedido {
  idDetalle: number;
  idProducto: number;
  nombreProducto: string;
  cantidad: number;
  precioUnitario: number;
  subtotal: number;
}

/** Representa un pedido completo con su estado logístico y lista de detalles. */
export interface Pedido {
  idPedido: number;
  fecha: string;
  total: number;
  estado: string;
  idUsuario: number;
  nombreUsuario: string;
  detalles: DetallePedido[];
}