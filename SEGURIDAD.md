# Seguridad — Estado de Cumplimiento de Requisitos de Seguridad (Sección 5, SRS)

Este documento deja constancia explícita de qué requisitos de seguridad (RS-01 a RS-06) están implementados en código, y cuáles se documentan como decisiones de arquitectura fuera del alcance de una implementación académica local, en vez de dejarlos como huecos silenciosos.

## RS-01 Autenticación — Implementado (parcial por diseño)

- **Usuario/contraseña**: implementado con ASP.NET Core Identity (Web y Api).
- **Gestión de sesiones**: implementado explícitamente.
  - Web: cookie de autenticación configurada con `ExpireTimeSpan = 8 horas` y `SlidingExpiration = true` (Módulo 6, Fase 6.3) — antes de este cambio, el sistema dependía del valor por defecto de ASP.NET Core (14 días), que nunca fue una decisión explícita.
  - Api: token JWT con expiración de 120 minutos y `ClockSkew = TimeSpan.Zero` (sin margen de tolerancia adicional).
- **MFA (autenticación multifactor)**: el SRS lo marca explícitamente como **opcional** ("MFA opcional"). No se implementó en este proyecto — se documenta como mejora futura fuera del alcance de las 24 semanas/módulos planeados.

## RS-02 Autorización — Implementado

Control RBAC (control de acceso basado en roles) mediante `[Authorize(Roles = ...)]` en todos los controllers administrativos: Usuarios (solo Administrador), Empleados/Vehículos/Departamentos/Tanques/Recepciones/Ajustes/Transferencias (Administrador + Supervisor), Cierres Diarios (Administrador + Supervisor, consulta también Auditor), Reportes/Dashboard/Notificaciones/Auditoría (Administrador + Supervisor + Auditor), Despacho (rol Despachador vía Api con JWT).

## RS-03 Cifrado — Parcialmente fuera del alcance de código de aplicación

- **Datos en tránsito (TLS 1.3)**: la versión del protocolo TLS la negocia el servidor que hospeda la aplicación (Kestrel, IIS, o un reverse proxy como Nginx), no el código de la aplicación en sí. En desarrollo local, ASP.NET Core usa el certificado de desarrollo (`dotnet dev-certs https`) sobre HTTPS. En un despliegue de producción real, forzar TLS 1.3 específicamente es una decisión de configuración del servidor/hosting, documentada aquí como el paso que correspondería tomar en un despliegue productivo, pero fuera del alcance de código de este proyecto académico.
- **Datos en reposo (AES-256)**: esto normalmente se resuelve con Transparent Data Encryption (TDE), una característica del motor de base de datos. TDE está disponible en SQL Server Standard/Enterprise y en Azure SQL, pero **no** en SQL Server Express (la edición típica usada en un entorno de desarrollo estudiantil, que es la que usa este proyecto). Se documenta como una decisión de arquitectura pendiente de la edición de SQL Server que se use en un despliegue de producción real, no como una omisión.

## RS-04 Seguridad de QR — Implementado

Cada ticket incluye firma mediante hash HMAC-SHA256 sobre sus datos críticos (Ticket ID, número, empleado, vehículo, cantidad, fechas) y un token de validación único, no reutilizable ni editable (Módulo 2).

## RS-05 Seguridad de APIs — Implementado (JWT); OAuth 2.0 interpretado como fuera de alcance

Se implementó autenticación mediante JWT Bearer (Módulo 3) para proteger los endpoints de la Api consumidos por la PWA. Un flujo completo de OAuth 2.0 (Authorization Server separado, `client_id`/`client_secret`, *scopes*, *refresh tokens*) agrega una complejidad de infraestructura considerable, pensada para escenarios con múltiples clientes de terceros — este proyecto tiene un único cliente (la PWA propia) consumiendo su propia Api, por lo que JWT Bearer directo cubre la necesidad real de autenticación sin la sobre-ingeniería de un flujo OAuth2 completo. Se documenta como interpretación deliberada del requisito, no como omisión.

## RS-06 Auditoría — Implementado (a nivel de aplicación)

Registro de auditoría (`AuditoriaLog`) para creaciones, modificaciones, despachos, ajustes, anulaciones y accesos, con usuario, fecha, hora e IP (Módulo 5). A nivel de aplicación es inalterable: no existe ningún endpoint ni pantalla que permita editar o eliminar un registro de auditoría. A nivel de base de datos pura no hay una protección adicional (como *triggers* que bloqueen `UPDATE`/`DELETE`, o permisos de solo-inserción a nivel de usuario de BD) — se documenta como mejora futura para un entorno de producción con separación de roles de base de datos.