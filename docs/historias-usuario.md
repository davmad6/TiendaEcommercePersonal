# Historias de Usuario — TiendaEcommercePersonal

Documento de trazabilidad funcional alineado con la arquitectura definitiva del proyecto (**React + TypeScript**, **API REST en C# / .NET** y **PostgreSQL**).

---

## HU-01: Registro e inicio de sesión de usuarios
* **Narrativa:** Como cliente de la tienda, quiero registrar una cuenta nueva o iniciar sesión con mis credenciales, para contar con un identificador de usuario activo (`idUsuario`) que me permita realizar y consultar mis compras.
* **Criterios de aceptación:**
  * El formulario valida los campos obligatorios antes de enviar la petición `POST` al controlador de usuarios.
  * Al autenticar o registrar al usuario, la aplicación conserva el `idUsuario` en el estado global (`App.tsx`) y muestra su nombre en la barra superior (`Navbar.tsx`).
  * El sistema impide confirmar una orden de compra si no existe una sesión de usuario activa.
* **Componentes relacionados:** `AuthPage.tsx`, `Navbar.tsx`, `App.tsx` | Controlador de usuarios (.NET) | Tabla `usuarios`.

---

## HU-02: Exploración de catálogo y gestión del carrito de compras
* **Narrativa:** Como comprador, quiero consultar los productos disponibles filtrados por categoría y agregarlos a un carrito ajustando sus cantidades, para preparar mi compra con el cálculo automático de subtotales y valor total.
* **Criterios de aceptación:**
  * La vista de catálogo consulta los productos (`GET /api/productos`) y categorías (`GET /api/categorias`), mostrando precio unitario y existencias en inventario.
  * El usuario puede filtrar el listado por categoría y añadir productos al carrito sin recargar la página, actualizando el contador en `Navbar.tsx`.
  * En la vista del carrito (`CarritoPage.tsx`), el cliente puede modificar cantidades o eliminar artículos recalculando los subtotales y el total en tiempo real.
* **Componentes relacionados:** `CatalogoPage.tsx`, `CarritoPage.tsx`, `Navbar.tsx` | Controladores de productos y categorías (.NET) | Tablas `productos` y `categorias`.

---

## HU-03: Creación de pedidos, actualización de estados y control de inventario
* **Narrativa:** Como usuario registrado, quiero confirmar la orden desde mi carrito, consultar mi historial de compras y actualizar el estado logístico de mis pedidos, para garantizar la trazabilidad de cada venta y el descuento o devolución automática del stock.
* **Criterios de aceptación:**
  * Al confirmar el carrito (`POST /api/pedidos`), el *backend* verifica que exista stock suficiente, guarda el pedido y su detalle en una transacción y descuenta las unidades del inventario; si falta stock, rechaza la operación con un mensaje claro.
  * La vista de pedidos (`PedidosPage.tsx`) consulta las órdenes (`GET /api/pedidos`) y permite filtrarlas por el usuario autenticado.
  * Al actualizar el estado del pedido (`Pendiente`, `Pagado`, `Enviado`, `Entregado` o `Cancelado`) mediante `PUT /api/pedidos/{id}/estado`, si el nuevo estado es `Cancelado`, el servidor reintegra automáticamente las unidades al inventario del producto.
* **Componentes relacionados:** `CarritoPage.tsx`, `PedidosPage.tsx` | `PedidosController`, `PedidoService`, `PedidoRepository` (.NET) | Tablas `pedidos`, `detalle_pedidos` y `productos`.

---

## HU-04: Administración de categorías y productos del inventario
* **Narrativa:** Como administrador de la tienda, quiero crear categorías y registrar, editar o eliminar productos desde un panel operativo, para mantener actualizado el catálogo y el inventario sin interferir con la vista pública de ventas.
* **Criterios de aceptación:**
  * El panel administrativo (`AdminCatalogoPage.tsx`) permite registrar nuevas categorías mediante operaciones `POST`.
  * Permite crear, editar y eliminar productos (`POST`, `PUT` y `DELETE`) definiendo su nombre, categoría asociada, precio y stock disponible.
  * Las modificaciones guardadas se reflejan de inmediato en el catálogo público (`CatalogoPage.tsx`) y en PostgreSQL.
* **Componentes relacionados:** `AdminCatalogoPage.tsx` | Controladores de productos y categorías (.NET) | Tablas `categorias` y `productos`.