# Plan de Pruebas — Criterios de Aceptación (Sección 7, SRS)

Este documento mapea cada uno de los 7 criterios de aceptación definidos en el SRS contra las pruebas funcionales realizadas durante el desarrollo, ejecutadas manualmente contra una base de datos SQL Server real (no simulada), módulo por módulo.

## 1. Se emitan tickets QR únicos sin duplicidad

**Resultado: Cumplido.**

- Cada ticket recibe un número secuencial correlativo con prefijo configurable (`COM-2026-000001` a `COM-2026-000007` verificados), generado con bloqueo `UPDLOCK, HOLDLOCK` sobre la tabla `SecuenciasTicket` para evitar duplicados bajo concurrencia (Módulo 2).
- Cada ticket recibe además un `TicketUid` (GUID) y un `TokenValidacion` únicos e irrepetibles.
- Verificado en la pantalla de Reportes (Fase 5.5): 7 tickets con numeración correlativa, sin duplicados.

## 2. El despacho se valide exclusivamente mediante QR válido

**Resultado: Cumplido.**

- Cada ticket incluye una firma HMAC-SHA256 sobre sus datos críticos (Ticket ID, número, empleado, vehículo, cantidad, fechas), verificada por el despachador al escanear el QR desde la PWA (Módulo 3, RS-04).
- El endpoint de despacho en la Api está protegido por JWT y valida el hash antes de registrar el despacho.
- Verificado end-to-end: escaneo QR real desde la PWA → validación → confirmación visual → registro de despacho, con la fila de auditoría correspondiente (`Despacho | Despacho | 4 | Despacho de 70 galones para el ticket COM-2026-000006`).

## 3. El inventario se actualice en tiempo real

**Resultado: Cumplido.**

- `IMovimientoInventarioService` es el único punto de mutación de `Tanque.ExistenciaActual`, con bloqueo pesimista (`UPDLOCK, HOLDLOCK`) para evitar condiciones de carrera bajo despachos concurrentes (Módulo 4).
- Verificado en la pantalla de Inventario en Tiempo Real (RF-15) y en el Dashboard Ejecutivo (Fase 5.7), que muestra el inventario actual sumado en vivo desde los tanques activos (3,870.00 gal verificados).

## 4. Exista trazabilidad completa de las operaciones

**Resultado: Cumplido.**

- Auditoría (`AuditoriaLog`, RS-06) conectada a: Accesos/login, Despachos, movimientos de Inventario (recepciones, ajustes, transferencias), Usuarios, Empleados, Vehículos, Departamentos, Tanques, Solicitudes, Tickets (creación y anulación), y Cierres Diarios (Módulo 5, Fase 5.1-5.2).
- Verificado con más de 40 registros de auditoría reales generados durante las pruebas de todos los módulos, incluyendo un caso de auditoría sobre `Tanque` verificado explícitamente (creación + desactivación).

## 5. Los reportes sean exportables

**Resultado: Cumplido.**

- Reportes de tickets filtrables por fecha, empleado, vehículo, departamento, tipo de combustible y estado (RF-19, Fase 5.5).
- Exportación a Excel (ClosedXML), CSV (con BOM UTF-8 para acentos/ñ) y PDF (QuestPDF), los tres formatos probados con y sin filtros aplicados, confirmando que el archivo exportado siempre coincide con los resultados filtrados en pantalla (RF-20, Fase 5.6).

## 6. La aplicación móvil opere correctamente en producción

**Resultado: Cumplido en ambiente de desarrollo — pendiente validar en despliegue real.**

- La PWA fue probada de punta a punta: login seguro, escaneo QR con cámara real (`html5-qrcode`), confirmación visual, validación en línea, registro de despacho y sincronización inmediata contra la Api (Módulo 3).
- **Nota honesta:** esta prueba se realizó en ambiente de desarrollo local (`localhost`), no en un servidor de producción desplegado con dominio y certificado TLS reales. Antes de considerar este criterio 100% cumplido para un entorno productivo, correspondería repetir la prueba end-to-end contra un despliegue real.

## 7. Se cumplan los requisitos de seguridad establecidos

**Resultado: Cumplido, con decisiones de arquitectura documentadas.**

- RS-01 (autenticación y sesiones), RS-02 (RBAC), RS-04 (seguridad de QR) y RS-06 (auditoría inalterable a nivel de aplicación) están completamente implementados y probados (Módulo 6).
- RS-03 (cifrado TLS 1.3 / AES-256 en reposo) y RS-05 (OAuth 2.0 completo) tienen decisiones de arquitectura documentadas explícitamente en `SEGURIDAD.md`, ya que exceden el alcance razonable de una implementación académica local (dependen de la edición del motor de base de datos y de infraestructura de despliegue, respectivamente).

## Prueba adicional: Portabilidad del sistema (Fase 7.2)

Aunque no es uno de los 7 criterios de aceptación formales del SRS, se realizó una prueba adicional para reforzar la confiabilidad del despliegue de cara a la sustentación: se clonó el repositorio en una carpeta limpia, simulando una instalación desde cero en una máquina distinta a la de desarrollo habitual.

**Procedimiento:**

1. Clonado del repositorio desde GitHub en una ubicación nueva (commit `8fd9df0`, cierre del Módulo 6).
2. `dotnet restore` + `dotnet build`: 0 errores, 0 advertencias — confirma que el código fuente no depende de nada externo al repositorio.
3. Configuración de `dotnet user-secrets` desde cero para los proyectos Web y Api, confirmando primero que un clon nuevo arranca sin ningún secret configurado (los `user-secrets` nunca viajan con `git clone`).
4. `dotnet ef database update` contra una base de datos SQL Server completamente nueva y vacía: las 8 migraciones (desde `InitialCreate` hasta `Modulo6_Notificaciones`) se aplicaron limpias y en orden.
5. Verificación del sembrado automático (seed) del usuario Administrador inicial vía `IdentitySeeder`, con inicio de sesión exitoso en el Web.
6. Verificación de arranque y conectividad de los tres ejecutables (Web, Api, Pwa) entre sí, incluyendo autenticación JWT desde la PWA contra la Api.

**Hallazgo real detectado y corregido:** la prueba reveló que el proyecto Api no registraba `INotificacionService` en su propio contenedor de Inyección de Dependencias. Ese registro se había agregado al proyecto Web durante el Módulo 6 (Fase 6.1a), pero nunca se replicó en la Api porque ese proyecto no se había vuelto a ejecutar desde entonces. Como `EmailService`, `TicketService` y `MovimientoInventarioService` dependen de `INotificacionService` desde la Fase 6.1c, la Api no lograba arrancar (`System.AggregateException` al construir el contenedor de servicios). Se corrigió agregando el registro faltante en `Program.cs` de la Api — tanto en la copia de prueba de portabilidad como en el proyecto de desarrollo real, para que el defecto no reapareciera el día de la sustentación.

**Nota operativa documentada:** la Api debe iniciarse con el perfil de lanzamiento `https` (`dotnet run --project src\GestionTicketsCombustible.Api --launch-profile https`) para exponer el puerto `7262`, que es el que la PWA tiene configurado como `API_BASE_URL`. El perfil `http` (que `dotnet run` usa por defecto si no se especifica uno) solo expone el puerto `5275`, insuficiente para que la PWA se conecte. Al ejecutar el proyecto desde Visual Studio con F5, este detalle no se nota porque el IDE recuerda el último perfil usado (`https`).

**Resultado:** sistema portable confirmado. Un colaborador distinto — o el propio autor, desde otra instalación — puede clonar el repositorio, configurar sus propias credenciales, aplicar las migraciones y tener el sistema completo (Web + Api + Pwa) funcionando de principio a fin, sin depender de ninguna configuración previa específica de la máquina de desarrollo original.

## Resumen

| # | Criterio | Estado |
|---|----------|--------|
| 1 | Tickets QR únicos sin duplicidad | ✅ Cumplido |
| 2 | Despacho validado solo por QR | ✅ Cumplido |
| 3 | Inventario en tiempo real | ✅ Cumplido |
| 4 | Trazabilidad completa | ✅ Cumplido |
| 5 | Reportes exportables | ✅ Cumplido |
| 6 | App móvil en producción | ⚠️ Cumplido en desarrollo, pendiente validar en despliegue real |
| 7 | Requisitos de seguridad | ✅ Cumplido (con decisiones documentadas en SEGURIDAD.md) |
| — | Portabilidad del sistema (verificación adicional) | ✅ Cumplido — defecto real detectado y corregido durante la prueba |