# 🛍️ MayaShop - E-Commerce de Productos Yucatecos

MayaShop es una plataforma de comercio electrónico local (Yucatán, México) construida con una arquitectura moderna de microservicios usando **ASP.NET Core (C#)**, **React con Vite** y **PostgreSQL**. Incorpora autenticación empresarial mediante **Microsoft Entra ID (Azure AD)**.

El sistema se divide en dos proyectos principales: un Backend API robusto y escalable fundado sobre los principios de Clean Architecture, y un Frontend interactivo orientado tanto a Clientes Finales como a Administradores del catálogo.

---

## 🏗️ Estado Actual del Desarrollo
### ✅ Backend API (Completado)
La arquitectura base del sistema se encuentra **implementada al 100% y funcional** usando .NET 10. Cuenta con los siguientes módulos operativos:
1. **Modelado Relacional con Entity Framework Core** (Migrations listas y Base de Datos generada).
2. **Capa Data-Access mediante Patrón Repository Genérico**.
3. **Servicios Core de Negocio Independientes** (Catálogo, Carrito y Compra).
4. **API Restful Protegida** (Endpoints).
5. **Autenticación OIDC vía Token con Microsoft Entra ID**.

### ⏳ Frontend React (Próximo paso)
La interfaz gráfica de usuario está pendiente de implementación. (Módulo 4).

---

## 🛠️ Stack Tecnológico
* **Backend Framework:** ASP.NET Core Web API 10.0
* **Base de Datos:** PostgreSQL
* **ORM:** Entity Framework Core (con Npgsql provider)
* **Identity Provider:** Microsoft Entra ID (Azure AD) Validando tokens JWT
* **Arquitectura:** Clean Architecture (Api, Core, Infrastructure) + Patrón Repository

---

## 📦 Arquitectura de Base de Datos (PostgreSQL)
Se ha implementado el siguiente diagrama relacional con características avanzadas como *Soft Delete* (Borrado lógico) para evitar la pérdida del historial de facturación.

* **👥 Users:** Identificados inyectando directamente el  `AzureOid` proveniente del token JWT. Roles: `admin` | `customer`.
* **🏷️ Categories:** Clasificadores del producto (Artesanías, Alimentos, Textil, etc).
* **🛒 Products:** Catálogo de la tienda. (Id, Nombre, Descripción, Precio MXN, Stock, `ImageUrl`, `IsActive` para soft deletion).
* **🛍️ Carts & CartItems:** Carrito de compras *persistente* anclado a la cuenta del usuario para poder retomar sus compras en otro dispositivo.
* **🧾 Orders & OrderItems:** Historial inmutable de compras y el cálculo fijo del precio (UnitPrice).

---

## 🚀 Guía de Instalación y Ejecución Local

### 1. Requisitos Previos
* .NET SDK 10.0 o superior
* PostgreSQL instalado (*para macOS: configurado preferentemente de forma local usando Homebrew o Docker*)
* Un [Tenant Tenant] de Microsoft Entra ID (Registrar nueva aplicación).

### 2. Configurar Variables de Entorno y Secrets
Dentro de la ruta `backend/MayaShop.Api/`, edita el archivo `appsettings.json`:
1. Asegúrate de que el **ConnectionString** a PostgreSQL sea correcto en tu máquina local.
2. Ingresa los identificadores de Azure AD (`TenantId` y `ClientId`/`Audience`).

### 3. Ejecutar Migraciones BD
Para crear las tablas o sincronizar cambios estructurales en PostgreSQL:
```bash
cd backend/MayaShop.Api
dotnet ef database update --project ../MayaShop.Infrastructure
```

### 4. Compilar y Correr la API
```bash
cd backend/MayaShop.Api
dotnet run
```
Al correr la API en desarrollo, puedes entrar en la ruta de tu localhost para visualizar el documento vivo de los Endpoints gracias a **Swagger UI** (Ej: `https://localhost:XXXX/swagger`).

---

## 🔐 Endpoints Protegidos Principales
Todos estos endpoints ya pueden ser invocados interceptando previamente un token de Microsoft.

#### 👤 Autenticación (Sincronización P2P)
- `POST /api/auth/sync` *(Recibe un Token OAuth de Azure, y actualiza/crea al usuario silenciosamente en tu PostgreSQL Local dando el alta en el comercio)*.

#### 📦 Catálogo
- `GET /api/products` *(Acceso Libre)*.
- `POST /api/products` *(Requiere rol Admin y Token Azure)*.
- `DELETE /api/products/{id}` *(Requiere rol Admin. Deshabilita el producto para compras nuevas conservando la base de tickets)*.

#### 🛒 Carrito Persistente
*(Requieren Token)*
- `GET /api/cart`
- `POST /api/cart`
- `PUT /api/cart/{itemId}` 

#### 🧾 Compras (Checkout)
*(Requieren Token)*
- `POST /api/orders` *(Toma tu carrito persistente actual, liquida el total, resta las unidades del Stock Real en PostgreSQL, y archiva el comprobante)*.

---

*Proyecto en proceso de construcción por fases. Fase actual: **Módulo 3 superado**.*
