import type {
  Categoria,
  CrearPedidoPayload,
  GuardarCategoriaPayload,
  GuardarProductoPayload,
  LoginUsuarioPayload,
  Pedido,
  Producto,
  RegistroUsuarioPayload,
  Usuario,
} from '../types/ecommerce';

const API_BASE_URL = 'http://localhost:5028/api';

async function manejarRespuesta<T>(response: Response): Promise<T> {
  const data = await response.json().catch(() => null);
  if (!response.ok) {
    const mensajeError =
      data && typeof data === 'object' && 'mensaje' in data
        ? String((data as { mensaje: string }).mensaje)
        : `Error HTTP ${response.status}`;
    throw new Error(mensajeError);
  }
  return data as T;
}

// ================= SERVICIO DE USUARIOS =================

export const usuariosApi = {
  listar: async (): Promise<Usuario[]> => {
    const res = await fetch(`${API_BASE_URL}/usuarios`);
    return manejarRespuesta<Usuario[]>(res);
  },

  registrar: async (payload: RegistroUsuarioPayload): Promise<{ mensaje: string; usuario: Usuario }> => {
    const res = await fetch(`${API_BASE_URL}/usuarios/registro`, {
      method: 'POST',
      headers: { 'Content-Type': 'application/json' },
      body: JSON.stringify(payload),
    });
    return manejarRespuesta<{ mensaje: string; usuario: Usuario }>(res);
  },

  login: async (payload: LoginUsuarioPayload): Promise<{ mensaje: string; usuario: Usuario }> => {
    const res = await fetch(`${API_BASE_URL}/usuarios/login`, {
      method: 'POST',
      headers: { 'Content-Type': 'application/json' },
      body: JSON.stringify(payload),
    });
    return manejarRespuesta<{ mensaje: string; usuario: Usuario }>(res);
  },
};

// ================= SERVICIO DE CATÁLOGO =================

export const catalogoApi = {
  listarCategorias: async (): Promise<Categoria[]> => {
    const res = await fetch(`${API_BASE_URL}/categorias`);
    return manejarRespuesta<Categoria[]>(res);
  },

  crearCategoria: async (payload: GuardarCategoriaPayload): Promise<Categoria> => {
    const res = await fetch(`${API_BASE_URL}/categorias`, {
      method: 'POST',
      headers: { 'Content-Type': 'application/json' },
      body: JSON.stringify(payload),
    });
    return manejarRespuesta<Categoria>(res);
  },

  listarProductos: async (idCategoria?: number): Promise<Producto[]> => {
    const url = idCategoria
      ? `${API_BASE_URL}/productos?idCategoria=${idCategoria}`
      : `${API_BASE_URL}/productos`;
    const res = await fetch(url);
    return manejarRespuesta<Producto[]>(res);
  },

  crearProducto: async (payload: GuardarProductoPayload): Promise<Producto> => {
    const res = await fetch(`${API_BASE_URL}/productos`, {
      method: 'POST',
      headers: { 'Content-Type': 'application/json' },
      body: JSON.stringify(payload),
    });
    return manejarRespuesta<Producto>(res);
  },

  actualizarProducto: async (id: number, payload: GuardarProductoPayload): Promise<Producto> => {
    const res = await fetch(`${API_BASE_URL}/productos/${id}`, {
      method: 'PUT',
      headers: { 'Content-Type': 'application/json' },
      body: JSON.stringify(payload),
    });
    return manejarRespuesta<Producto>(res);
  },

  eliminarProducto: async (id: number): Promise<void> => {
    const res = await fetch(`${API_BASE_URL}/productos/${id}`, {
      method: 'DELETE',
    });
    if (!res.ok) {
      await manejarRespuesta<unknown>(res);
    }
  },
};

// ================= SERVICIO DE PEDIDOS =================

export const pedidosApi = {
  listar: async (idUsuario?: number): Promise<Pedido[]> => {
    const url = idUsuario
      ? `${API_BASE_URL}/pedidos?idUsuario=${idUsuario}`
      : `${API_BASE_URL}/pedidos`;
    const res = await fetch(url);
    return manejarRespuesta<Pedido[]>(res);
  },

  crear: async (payload: CrearPedidoPayload): Promise<{ mensaje: string; pedido: Pedido }> => {
    const res = await fetch(`${API_BASE_URL}/pedidos`, {
      method: 'POST',
      headers: { 'Content-Type': 'application/json' },
      body: JSON.stringify(payload),
    });
    return manejarRespuesta<{ mensaje: string; pedido: Pedido }>(res);
  },

  actualizarEstado: async (
    idPedido: number,
    estado: string
  ): Promise<{ mensaje: string; pedido: Pedido }> => {
    const res = await fetch(`${API_BASE_URL}/pedidos/${idPedido}/estado`, {
      method: 'PUT',
      headers: { 'Content-Type': 'application/json' },
      body: JSON.stringify({ estado }),
    });
    return manejarRespuesta<{ mensaje: string; pedido: Pedido }>(res);
  },
};