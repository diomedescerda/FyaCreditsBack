# Fya Credits Backend

Backend de ASP.NET Core Web API sobre .NET 10 con Clean Architecture, EF Core y PostgreSQL.

## Ramas

- **`main`**: configuración del **backend desplegado**.
- **`dev`**: configuración para correr el backend **localmente** (desarrollo).

Para ejecutar el backend localmente, cambia a la rama `dev` y sigue las instrucciones de su `README.md`.

## Backend desplegado

Los servicios desplegados son:

- **API:** `https://fyacreditsback-1cde.onrender.com`
- **Email renderer:** `https://fyacreditsback.onrender.com`
- **Base de datos:** PostgreSQL en Render (ya provisionada e inicializada)

### Estados

- Salud de la API: `https://fyacreditsback-1cde.onrender.com/health`
- Documentación OpenAPI: `https://fyacreditsback-1cde.onrender.com/openapi/v1.json`

### Credenciales semilla

El usuario por defecto es:

- **Correo:** `ana.comercial@fyasocialcapital.com`
- **Contraseña:** `FyaDev123!`

También puedes registrarte como usuario nuevo desde la app.

## Qué puedes probar desde la app

- **Registrar un crédito** con los datos del cliente y el comercial (tomado del usuario autenticado).
- **Consultar créditos** registrados.
- **Búsqueda unificada** por nombre del cliente, cédula o comercial.
- **Ordenar** por fecha o valor.
- **Paginación** de 15 registros por página.
- **Ver detalles** de cada crédito.
- **Recuperar contraseña**: la API envía un correo con un enlace que abre la app para restablecerla (usando el email renderer desplegado).

La autenticación usa JWT con ASP.NET Core Identity. No se requieren pasos de despliegue: la app se conecta a la API desplegada desde la rama `main`.
