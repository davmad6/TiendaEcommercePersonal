# Bitácora de Construcción - TiendaEcommercePersonal
* **Aprendiz:** David Madrid Peña
* **Programa:** Tecnólogo en Análisis y Desarrollo de Software (ADSO) — SENA
* **Instructor:** Eumir Pulido
* **Evidencias:** AA3-EV01 y AA3-EV02

## Paso 1: Estructura Base, Git y Base de Datos
* **1.1 Estructura y Git:** Creación de carpetas (`backend`, `frontend`, `docs`), inicialización de Git en rama `main`, configuración de `.gitignore` y bitácora.
* **1.2 Base de Datos (PostgreSQL):** Creación de `tienda_ecommerce_personal` con las 5 tablas (`categorias`, `productos`, `usuarios`, `pedidos`, `detalle_pedidos`).

## Paso 2: Backend en C# (.NET) y Módulo de Usuarios (HU-01)
* **2.1 Web API en C#:** Proyecto creado en `backend/` con arquitectura por capas y conexión a PostgreSQL.
* **2.2 Módulo de Usuarios (`HU-01`):** Implementación en arquitectura por capas y validación CRUD real contra PostgreSQL.

## Paso 3: Catálogo e Inventario en Backend (HU-02)
* **3.1 Categorías y Productos:** Implementación de los módulos de `categorias` y `productos` (`HU-02`) alineados al esquema real de PostgreSQL.

## Paso 4: Pedidos Transaccionales en Backend (HU-03)
* **4.1 Pedidos y Detalle de Pedidos:** Implementación del módulo transaccional (`HU-03`) con descuento y reversión automática de inventario validado contra PostgreSQL.

## Paso 5: Inicialización del Frontend en React + TypeScript
* **5.1 Estructura Modular:** Creación del proyecto en `frontend/` con Vite y organización en carpetas (`types`, `services`, `components`, `pages`).

## Paso 6: Contratos TypeScript y Cliente HTTP REST
* **6.1 Tipado y Servicios:** Implementación de contratos (`types/ecommerce.ts`) y cliente HTTP (`services/api.ts`) conectados al backend en C#.

## Paso 7: Vistas Completas del Frontend
* **7.1 Interfaz de Usuario (`Commit #8`):** Implementación de `Navbar`, `AuthPage` (`HU-01`), `CatalogoPage` (`HU-02`), `CarritoPage` (`HU-03`), `PedidosPage` (`HU-03`) y `AdminCatalogoPage` (`HU-02`).

## Paso 8: Alineación REST, Pruebas en Vivo y Cierre Documental (15/11/2026)
* **8.1 Alineación de Contratos REST (`Commit #9`):** Inspección de rutas de controladores en C# (`api/[controller]`) y ajuste de `usuariosApi` para operar contra `POST /api/usuarios` y `GET /api/usuarios`.
* **8.2 Validación Integral en PostgreSQL:** Registro e inicio de sesión desde el navegador, flujo de compra con descuento de stock y creación de categorías (`Cables/Almacenamiento`) y productos (`Padmouse`).
* **8.3 Cierre Documental (`Commit #10`):** Redacción de `README.md` y actualización final de `BITACORA_PROYECTO.md` para entrega de las evidencias **AA3-EV01** y **AA3-EV02**.
