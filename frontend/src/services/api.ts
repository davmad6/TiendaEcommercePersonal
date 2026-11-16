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
  if (response.status === 204) {
    return undefined as T;
  }

  const data: unknown = await response.json().catch(() => null);
  if (!response.ok) {
    let mensajeError = `Error HTTP ${response.status}`;
    if (data && typeof data === 'object') {
      if ('mensaje' in data && typeof data.mensaje === 'string') {
        mensajeError = data.mensaje;
      } else if ('title' in data && typeof data.title === 'string') {
        mensajeError = data.title;
      }
    }
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
    const res = await fetch(`${API_BASE_URL}/usuarios`, {
      method: 'POST',
      headers: { 'Content-Type': 'application/json' },
      body: JSON.stringify(payload),
    });
    const data = await manejarRespuesta<Usuario | { mensaje?: string; usuario: Usuario }>(res);
    if (data && typeof data === 'object' && 'usuario' in data) {
      return {
        mensaje: data.mensaje ?? 'Usuario registrado correctamente.',
        usuario: data.usuario,
      };
    }
    return {
      mensaje: 'Usuario registrado correctamente.',
      usuario: data,
    };
  },

  login: async (payload: LoginUsuarioPayload): Promise<{ mensaje: string; usuario: Usuario }> => {
    const res = await fetch(`${API_BASE_URL}/usuarios`);
    const usuarios = await manejarRespuesta<Array<Usuario & { contrasena?: string }>>(res);
    const emailBuscado = payload.email.trim().toLowerCase();
    const encontrado = usuarios.find((u) => u.email.trim().toLowerCase() === emailBuscado);

    if (!encontrado) {
      throw new Error('No existe una cuenta registrada con ese correo electrónico.');
    }

    if (typeof encontrado.contrasena === 'string' && encontrado.contrasena !== payload.contrasena) {
      throw new Error('La contraseña ingresada es incorrecta.');
    }

    const usuarioLimpio: Usuario = {
      idUsuario: encontrado.idUsuario,
      nombre: encontrado.nombre,
      email: encontrado.email,
      direccion: encontrado.direccion,
      telefono: encontrado.telefono,
    };

    return {
      mensaje: 'Inicio de sesión exitoso.',
      usuario: usuarioLimpio,
    };
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
    const data = await manejarRespuesta<Pedido | { mensaje?: string; pedido: Pedido }>(res);
    if (data && typeof data === 'object' && 'pedido' in data) {
      return {
        mensaje: data.mensaje ?? 'Pedido registrado correctamente.',
        pedido: data.pedido,
      };
    }
    return {
      mensaje: 'Pedido registrado correctamente.',
      pedido: data,
    };
  },

  actualizarEstado: async (
    idPedido: number,
    estado: string
  ): Promise<{ mensaje: string }> => {
    const res = await fetch(`${API_BASE_URL}/pedidos/${idPedido}/estado`, {
      method: 'PUT',
      headers: { 'Content-Type': 'application/json' },
      body: JSON.stringify({ estado }),
    });
    const data = await manejarRespuesta<{ mensaje?: string } | undefined>(res);
    return {
      mensaje:
        data && typeof data === 'object' && typeof data.mensaje === 'string'
          ? data.mensaje
          : `Estado del pedido #${idPedido} actualizado a "${estado}".`,
    };
  },
};