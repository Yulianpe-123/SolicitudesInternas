/* =====================================================================
   Sistema de Seguimiento de Solicitudes Internas
   Script de creación de base de datos (SQL Server / LocalDB)

   - Crea la base SolicitudesInternasDB, tablas, llaves, relaciones y restricciones.
   - Inserta datos iniciales para poder ejecutar y demostrar el sistema.
   - Es re-ejecutable: elimina y vuelve a crear las tablas (se pierden los datos).

   Valores de las enumeraciones (coinciden con el código C#):
     Estado     : 1 Creada, 2 Asignada, 3 EnProceso, 4 Resuelta, 5 Cerrada
     Prioridad  : 1 Baja, 2 Media, 3 Alta, 4 Critica
     Seguimiento: 1 Creacion, 2 Asignacion, 3 Reasignacion, 4 CambioEstado,
                  5 CambioPrioridad, 6 Comentario, 7 Solucion, 8 Cierre
   ===================================================================== */

IF DB_ID(N'SolicitudesInternasDB') IS NULL
    CREATE DATABASE SolicitudesInternasDB;
GO

USE SolicitudesInternasDB;
GO

/* ---------- Limpieza (orden inverso a las dependencias) ---------- */
IF OBJECT_ID(N'dbo.Seguimientos',   N'U') IS NOT NULL DROP TABLE dbo.Seguimientos;
IF OBJECT_ID(N'dbo.Solicitudes',    N'U') IS NOT NULL DROP TABLE dbo.Solicitudes;
IF OBJECT_ID(N'dbo.TiposSolicitud', N'U') IS NOT NULL DROP TABLE dbo.TiposSolicitud;
IF OBJECT_ID(N'dbo.Responsables',   N'U') IS NOT NULL DROP TABLE dbo.Responsables;
IF OBJECT_ID(N'dbo.Usuarios',       N'U') IS NOT NULL DROP TABLE dbo.Usuarios;
GO

/* ---------- Tablas de catálogo ---------- */
CREATE TABLE dbo.Usuarios (
    Id           INT IDENTITY(1,1) NOT NULL,
    Nombre       NVARCHAR(100) NOT NULL,
    Correo       NVARCHAR(150) NOT NULL,
    Departamento NVARCHAR(100) NOT NULL,
    Activo       BIT           NOT NULL CONSTRAINT DF_Usuarios_Activo DEFAULT (1),
    CONSTRAINT PK_Usuarios PRIMARY KEY (Id),
    CONSTRAINT UQ_Usuarios_Correo UNIQUE (Correo)
);

CREATE TABLE dbo.Responsables (
    Id           INT IDENTITY(1,1) NOT NULL,
    Nombre       NVARCHAR(100) NOT NULL,
    Correo       NVARCHAR(150) NOT NULL,
    Especialidad NVARCHAR(100) NOT NULL,
    Activo       BIT           NOT NULL CONSTRAINT DF_Responsables_Activo DEFAULT (1),
    CONSTRAINT PK_Responsables PRIMARY KEY (Id),
    CONSTRAINT UQ_Responsables_Correo UNIQUE (Correo)
);

CREATE TABLE dbo.TiposSolicitud (
    Id          INT IDENTITY(1,1) NOT NULL,
    Nombre      NVARCHAR(60)  NOT NULL,
    Descripcion NVARCHAR(200) NULL,
    Activo      BIT           NOT NULL CONSTRAINT DF_TiposSolicitud_Activo DEFAULT (1),
    CONSTRAINT PK_TiposSolicitud PRIMARY KEY (Id),
    CONSTRAINT UQ_TiposSolicitud_Nombre UNIQUE (Nombre)
);

/* ---------- Solicitudes ---------- */
CREATE TABLE dbo.Solicitudes (
    Id              INT IDENTITY(1,1) NOT NULL,
    SolicitanteId   INT            NOT NULL,
    TipoSolicitudId INT            NOT NULL,
    Descripcion     NVARCHAR(1000) NOT NULL,
    Prioridad       INT            NOT NULL CONSTRAINT DF_Solicitudes_Prioridad DEFAULT (2),
    Estado          INT            NOT NULL CONSTRAINT DF_Solicitudes_Estado DEFAULT (1),
    ResponsableId   INT            NULL,
    Solucion        NVARCHAR(2000) NULL,
    FechaCreacion   DATETIME2      NOT NULL,
    FechaResolucion DATETIME2      NULL,
    FechaCierre     DATETIME2      NULL,
    CONSTRAINT PK_Solicitudes PRIMARY KEY (Id),
    CONSTRAINT FK_Solicitudes_Usuarios      FOREIGN KEY (SolicitanteId)   REFERENCES dbo.Usuarios (Id),
    CONSTRAINT FK_Solicitudes_Tipos         FOREIGN KEY (TipoSolicitudId) REFERENCES dbo.TiposSolicitud (Id),
    CONSTRAINT FK_Solicitudes_Responsables  FOREIGN KEY (ResponsableId)   REFERENCES dbo.Responsables (Id),

    -- RN-06: la prioridad pertenece a los valores definidos.
    CONSTRAINT CK_Solicitudes_Prioridad CHECK (Prioridad BETWEEN 1 AND 4),
    -- RN-02: el estado pertenece al flujo definido.
    CONSTRAINT CK_Solicitudes_Estado CHECK (Estado BETWEEN 1 AND 5),
    -- RN-01: la descripción no puede quedar vacía.
    CONSTRAINT CK_Solicitudes_Descripcion CHECK (LEN(LTRIM(RTRIM(Descripcion))) > 0),
    -- Toda solicitud asignada o posterior tiene responsable.
    CONSTRAINT CK_Solicitudes_Responsable CHECK (Estado = 1 OR ResponsableId IS NOT NULL),
    -- RN-05 y RN-08: una solicitud resuelta o cerrada contiene una solución.
    CONSTRAINT CK_Solicitudes_Solucion CHECK (Estado < 4 OR (Solucion IS NOT NULL AND LEN(LTRIM(RTRIM(Solucion))) > 0))
);

CREATE INDEX IX_Solicitudes_Estado        ON dbo.Solicitudes (Estado);
CREATE INDEX IX_Solicitudes_Prioridad     ON dbo.Solicitudes (Prioridad);
CREATE INDEX IX_Solicitudes_ResponsableId ON dbo.Solicitudes (ResponsableId);
CREATE INDEX IX_Solicitudes_SolicitanteId ON dbo.Solicitudes (SolicitanteId);

/* ---------- Historial de seguimiento (RN-07) ---------- */
CREATE TABLE dbo.Seguimientos (
    Id          INT IDENTITY(1,1) NOT NULL,
    SolicitudId INT            NOT NULL,
    Fecha       DATETIME2      NOT NULL,
    Tipo        INT            NOT NULL,
    Autor       NVARCHAR(100)  NOT NULL,
    Comentario  NVARCHAR(1000) NOT NULL,
    CONSTRAINT PK_Seguimientos PRIMARY KEY (Id),
    CONSTRAINT FK_Seguimientos_Solicitudes FOREIGN KEY (SolicitudId) REFERENCES dbo.Solicitudes (Id),
    CONSTRAINT CK_Seguimientos_Tipo CHECK (Tipo BETWEEN 1 AND 8)
);

CREATE INDEX IX_Seguimientos_SolicitudId ON dbo.Seguimientos (SolicitudId);
GO

/* ---------- Datos iniciales ---------- */
INSERT INTO dbo.TiposSolicitud (Nombre, Descripcion, Activo) VALUES
 (N'Equipo',          N'Solicitud de computadoras, periféricos u otros equipos.', 1),
 (N'Acceso',          N'Acceso a sistemas, aplicaciones o instalaciones.', 1),
 (N'Permisos',        N'Permisos sobre carpetas, módulos o recursos compartidos.', 1),
 (N'Soporte técnico', N'Fallas, incidencias y ayuda técnica.', 1);

INSERT INTO dbo.Usuarios (Nombre, Correo, Departamento, Activo) VALUES
 (N'Ana López',      N'ana.lopez@empresa.com',      N'Contabilidad', 1),
 (N'Carlos Méndez',  N'carlos.mendez@empresa.com',  N'Ventas',       1),
 (N'Lucía Ramírez',  N'lucia.ramirez@empresa.com',  N'Recursos Humanos', 1),
 (N'Pedro Castillo', N'pedro.castillo@empresa.com', N'Operaciones',  1),
 (N'Marta Solís',    N'marta.solis@empresa.com',    N'Compras',      0);   -- inactiva (para demostrar la validación)

INSERT INTO dbo.Responsables (Nombre, Correo, Especialidad, Activo) VALUES
 (N'Jorge Herrera', N'jorge.herrera@empresa.com', N'Soporte técnico', 1),
 (N'Sofía Díaz',    N'sofia.diaz@empresa.com',    N'Seguridad y accesos', 1),
 (N'Luis Morales',  N'luis.morales@empresa.com',  N'Infraestructura', 1),
 (N'Raúl Ortiz',    N'raul.ortiz@empresa.com',    N'Soporte técnico', 0);  -- inactivo (para demostrar la validación)

DECLARE @ahora DATETIME2 = SYSDATETIME();

/* Solicitud 1: Creada (sin responsable) */
INSERT INTO dbo.Solicitudes (SolicitanteId, TipoSolicitudId, Descripcion, Prioridad, Estado, ResponsableId, Solucion, FechaCreacion, FechaResolucion, FechaCierre)
VALUES (1, 1, N'Solicito una laptop adicional para el equipo de auditoría.', 2, 1, NULL, NULL, DATEADD(HOUR, -3, @ahora), NULL, NULL);

/* Solicitud 2: Asignada */
INSERT INTO dbo.Solicitudes (SolicitanteId, TipoSolicitudId, Descripcion, Prioridad, Estado, ResponsableId, Solucion, FechaCreacion, FechaResolucion, FechaCierre)
VALUES (2, 2, N'Necesito acceso al sistema CRM con rol de consulta.', 3, 2, 2, NULL, DATEADD(DAY, -1, @ahora), NULL, NULL);

/* Solicitud 3: En proceso */
INSERT INTO dbo.Solicitudes (SolicitanteId, TipoSolicitudId, Descripcion, Prioridad, Estado, ResponsableId, Solucion, FechaCreacion, FechaResolucion, FechaCierre)
VALUES (3, 3, N'Requiero permiso de escritura en la carpeta compartida de Recursos Humanos.', 2, 3, 2, NULL, DATEADD(DAY, -2, @ahora), NULL, NULL);

/* Solicitud 4: Resuelta (pendiente de cierre) */
INSERT INTO dbo.Solicitudes (SolicitanteId, TipoSolicitudId, Descripcion, Prioridad, Estado, ResponsableId, Solucion, FechaCreacion, FechaResolucion, FechaCierre)
VALUES (4, 4, N'La impresora del área de operaciones no imprime desde ayer.', 4, 4, 1,
        N'Se reinstaló el controlador de la impresora y se reemplazó el cable de red.',
        DATEADD(DAY, -3, @ahora), DATEADD(DAY, -2, @ahora), NULL);

/* Solicitud 5: Cerrada */
INSERT INTO dbo.Solicitudes (SolicitanteId, TipoSolicitudId, Descripcion, Prioridad, Estado, ResponsableId, Solucion, FechaCreacion, FechaResolucion, FechaCierre)
VALUES (1, 4, N'No puedo conectarme a la VPN desde mi casa.', 1, 5, 1,
        N'Se restableció la contraseña de VPN y se actualizó el cliente.',
        DATEADD(DAY, -7, @ahora), DATEADD(DAY, -6, @ahora), DATEADD(DAY, -5, @ahora));

/* ---------- Historial de las solicitudes de ejemplo ---------- */
INSERT INTO dbo.Seguimientos (SolicitudId, Fecha, Tipo, Autor, Comentario) VALUES
 (1, DATEADD(HOUR, -3, @ahora), 1, N'Ana López', N'Solicitud registrada en la categoría ''Equipo'' con prioridad Media.'),

 (2, DATEADD(DAY, -1, @ahora),                  1, N'Carlos Méndez', N'Solicitud registrada en la categoría ''Acceso'' con prioridad Alta.'),
 (2, DATEADD(HOUR, -20, @ahora),                2, N'Coordinación',  N'Solicitud asignada a Sofía Díaz.'),

 (3, DATEADD(DAY, -2, @ahora),                  1, N'Lucía Ramírez', N'Solicitud registrada en la categoría ''Permisos'' con prioridad Media.'),
 (3, DATEADD(HOUR, -44, @ahora),                2, N'Coordinación',  N'Solicitud asignada a Sofía Díaz.'),
 (3, DATEADD(HOUR, -40, @ahora),                4, N'Sofía Díaz',    N'La solicitud pasó a estado En proceso.'),

 (4, DATEADD(DAY, -3, @ahora),                  1, N'Pedro Castillo', N'Solicitud registrada en la categoría ''Soporte técnico'' con prioridad Crítica.'),
 (4, DATEADD(HOUR, -70, @ahora),                2, N'Coordinación',   N'Solicitud asignada a Jorge Herrera.'),
 (4, DATEADD(HOUR, -60, @ahora),                4, N'Jorge Herrera',  N'La solicitud pasó a estado En proceso.'),
 (4, DATEADD(DAY, -2, @ahora),                  7, N'Jorge Herrera',  N'Se registró la solución y la solicitud fue marcada como Resuelta.'),

 (5, DATEADD(DAY, -7, @ahora),                  1, N'Ana López',     N'Solicitud registrada en la categoría ''Soporte técnico'' con prioridad Baja.'),
 (5, DATEADD(HOUR, -160, @ahora),               2, N'Coordinación',  N'Solicitud asignada a Jorge Herrera.'),
 (5, DATEADD(HOUR, -155, @ahora),               4, N'Jorge Herrera', N'La solicitud pasó a estado En proceso.'),
 (5, DATEADD(DAY, -6, @ahora),                  7, N'Jorge Herrera', N'Se registró la solución y la solicitud fue marcada como Resuelta.'),
 (5, DATEADD(DAY, -5, @ahora),                  8, N'Jorge Herrera', N'Solicitud cerrada.');
GO
