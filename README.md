# TiendaEcommercePersonal

Aplicación web full-stack de comercio electrónico desarrollada como proyecto formativo para el programa **Tecnólogo en Análisis y Desarrollo de Software (ADSO) - SENA.**

## Arquitectura y Stack Tecnológico

* **Base de Datos:** PostgreSQL (administrada con pgAdmin 4), modelo relacional normalizado de 5 tablas.
* **Backend:** C# con ASP.NET Core Web API (.NET), ejecutándose en `http://localhost:5028`.
* **Frontend:** React + TypeScript con Vite, ejecutándose en `http://localhost:5173`.
* **Control de Versiones:** Git (flujo incremental con bitácora técnica en `docs/`).

## Modelo Relacional (PostgreSQL)

1. **`usuarios`**: `id_usuario` (PK), `nombre`, `email` (UNIQUE), `contrasena`, `direccion`, `telefono`.
2. **`categorias`**: `id_categoria` (PK), `nombre`.
3. **`productos`**: `id_producto` (PK), `nombre`, `descripcion`, `precio`, `stock`, `id_categoria` (FK).
4. **`pedidos`**: `id_pedido` (PK), `fecha`, `total`, `estado`, `id_usuario` (FK).
5. **`detalle_pedidos`**: `id_detalle` (PK), `cantidad`, `precio_unitario`, `id_pedido` (FK), `id_producto` (FK).

## Módulos Funcionales Implementados

* **HU-01 — Gestión de Usuarios y Autenticación:** registro de clientes (`POST /api/usuarios`), inicio de sesión y persistencia de sesión activa en el cliente.
* **HU-02 — Catálogo e Inventario (CRUD):** consulta filtrada por categorías (`GET /api/productos`, `GET /api/categorias`) y panel administrativo para crear categorías y crear, editar o eliminar productos (`POST`, `PUT`, `DELETE`).
* **HU-03 — Carrito y Pedidos Transaccionales:** creación de órdenes (`POST /api/pedidos`) con validación de inventario y descuento automático de stock en PostgreSQL, historial de pedidos (`GET /api/pedidos`) y actualización de estado (`PUT /api/pedidos/{id}/estado`) con reversión automática de stock al cancelar.

## Instrucciones de Ejecución Local

### 1. Base de Datos (PostgreSQL)
1. Crear la base de datos `tienda_ecommerce_personal` en pgAdmin 4.
2. Ejecutar el script SQL de creación de tablas y llaves foráneas documentado en `docs/`.

### 2. Backend (C# / .NET)
1. Verificar la cadena de conexión a PostgreSQL en `backend/appsettings.json`.
2. Ejecutar la API desde Visual Studio o desde terminal:
   ```bash
   dotnet run --project backend
   ```
3. La API quedará disponible en `http://localhost:5028/api`.

### 3. Frontend (React + TypeScript)
1. Instalar dependencias y levantar el servidor de desarrollo Vite:
   ```bash
   npm --prefix frontend install
   npm --prefix frontend run dev
   ```
2. Abrir `http://localhost:5173` en el navegador.

## Autor

* **David Madrid Peña** - Aprendiz SENA (Análisis y Desarrollo de Software).