# Sistema de Seguimiento de Solicitudes Internas

Proyecto Final – Programación II · ASP.NET MVC 5 (.NET Framework 4.8) · C# · Entity Framework 6 · SQL Server

Permite registrar solicitudes de equipos, accesos, permisos y soporte; asignarlas a un responsable,
cambiar su prioridad, registrar seguimientos, resolverlas, cerrarlas y consultarlas por responsable,
estado y prioridad, conservando siempre el historial.

## Estructura

```
SolicitudesInternas.sln
Database/01_CrearBaseDeDatos.sql      Script de la base de datos (tablas, llaves, restricciones y datos)
Docs/GuionDemostracion.md             Guion para la demostración
Docs/InformeFinal_SolicitudesInternas.docx   Informe técnico
SolicitudesInternas/                  Proyecto MVC
├── Models/            Entidades y enumeraciones (Usuario, Solicitud, Seguimiento, ...)
├── Interfaces/        Contratos de repositorios y servicios (DIP)
├── Repositories/      Acceso a datos con EF6
├── Services/          Reglas de negocio y flujo de estados
├── Data/              DbContext (SolicitudesContext)
├── Controllers/       Reciben solicitudes, validan entrada e invocan Services
├── ViewModels/        Datos de entrada/salida de las vistas
├── Views/             Vistas Razor
├── Excepciones/       Excepciones propias (negocio, no encontrado, persistencia)
├── Infrastructure/    FabricaControladores (raíz de composición / inyección de dependencias)
└── Helpers/           Utilidades de presentación para enumeraciones
```

## Cómo ejecutarlo

Requisitos: Windows, Visual Studio 2019/2022 con la carga de trabajo **ASP.NET y desarrollo web**,
.NET Framework 4.8 y SQL Server LocalDB (se instala con Visual Studio) o SQL Server Express.

1. **Base de datos.** Abra `Database/01_CrearBaseDeDatos.sql` en Visual Studio
   (*Ver → Explorador de objetos de SQL Server*) o en SSMS, conéctese a `(localdb)\MSSQLLocalDB`
   y ejecútelo. Crea `SolicitudesInternasDB` con datos de ejemplo.
2. **Proyecto.** Abra `SolicitudesInternas.sln`. Visual Studio restaurará los paquetes NuGet
   (EntityFramework 6.4.4, Microsoft.AspNet.Mvc 5.2.9). Si no lo hace: clic derecho en la solución →
   *Restaurar paquetes NuGet*.
3. **Cadena de conexión.** Está en `Web.config` (`SolicitudesContext`). Si usa SQL Express,
   cambie `(localdb)\MSSQLLocalDB` por `.\SQLEXPRESS`.
4. Presione **F5**.

> Las hojas de estilo y los scripts de validación del cliente (Bootstrap, jQuery) se cargan desde un CDN,
> por lo que se necesita conexión a Internet para ver el diseño completo. Las validaciones del servidor
> funcionan sin Internet.

> Si Visual Studio no logra abrir el `.csproj`, cree un proyecto *ASP.NET Web Application (.NET Framework) →
> Empty + MVC* llamado `SolicitudesInternas`, instale los paquetes NuGet indicados en `packages.config`
> y copie dentro las carpetas y archivos de `SolicitudesInternas/`.

## Reglas de negocio implementadas

| Código | Regla | Dónde se aplica |
|---|---|---|
| RN-01 | Toda solicitud requiere solicitante activo, categoría activa y descripción (mín. 10 caracteres). | `SolicitudService.Registrar` |
| RN-02 | Flujo Creada → Asignada → En proceso → Resuelta → Cerrada; no se permiten saltos ni retrocesos. | `FlujoEstados`, usado por `SolicitudService` |
| RN-03 | Una solicitud cerrada no puede modificarse. | `SolicitudService.ObtenerEditable` |
| RN-04 | Toda asignación/reasignación identifica un responsable activo y distinto del actual. | `SolicitudService.Asignar` |
| RN-05 | Una solicitud resuelta debe contener una solución. | `SolicitudService.Resolver` |
| RN-06 | La prioridad debe ser Baja, Media, Alta o Crítica. | `SolicitudService.ValidarPrioridad` |
| RN-07 | El historial de seguimiento se conserva: cada acción genera un registro y no hay opción de eliminar. | `SolicitudService` (todas las operaciones) |
| RN-08 | El responsable no puede cerrar sin solución registrada. | `SolicitudService.Cerrar` |
| RN-09 | Un responsable inactivo no recibe solicitudes ni puede desactivarse con solicitudes sin cerrar. | `SolicitudService.Asignar`, `ResponsableService.Actualizar` |
| RN-10 | Correo único en usuarios y responsables; nombre único en categorías. | Servicios de catálogo |

La base de datos refuerza varias de ellas con restricciones `CHECK` (estado, prioridad, solución obligatoria
para Resuelta/Cerrada, responsable obligatorio desde Asignada).
