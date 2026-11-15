// ================= USUARIOS (HU-01) =================

export interface Usuario {
  idUsuario: number;
  nombre: string;
  email: string;
  direccion?: string | null;
  telefono?: string | null;
}

export interface RegistroUsuarioPayload {
  nombre: string;
  email: string;
  contrasena: string;
  direccion?: string;
  telefono?: string;
}

export interface LoginUsuarioPayload {
  email: string;
  contrasena: string;
}

// ================= CATÁLOGO (HU-02) =================

export interface Categoria {
  idCategoria: number;
  nombre: string;
}

export interface GuardarCategoriaPayload {
  nombre: string;
}

export interface Producto {
  idProducto: number;
  nombre: string;
  descripcion?: string | null;
  precio: number;
  stock: number;
  idCategoria: number;
  nombreCategoria: string;
}

export interface GuardarProductoPayload {
  nombre: string;
  descripcion?: string;
  precio: number;
  stock: number;
  idCategoria: number;
}

// ================= PEDIDOS Y CARRITO (HU-03) =================

export interface ItemCarrito {
  producto: Producto;
  cantidad: number;
}

export interface CrearPedidoPayload {
  idUsuario: number;
  items: {
    idProducto: number;
    cantidad: number;
  }[];
}

export interface DetallePedido {
  idDetalle: number;
  idProducto: number;
  nombreProducto: string;
  cantidad: number;
  precioUnitario: number;
  subtotal: number;
}

export interface Pedido {
  idPedido: number;
  fecha: string;
  total: number;
  estado: string;
  idUsuario: number;
  nombreUsuario: string;
  detalles: DetallePedido[];
}