using System;
using System.Collections.Generic;
using SolicitudesInternas.Excepciones;
using SolicitudesInternas.Helpers;
using SolicitudesInternas.Interfaces;
using SolicitudesInternas.Models;

namespace SolicitudesInternas.Services
{
    /// Reglas de negocio del proceso de solicitudes. El controlador solo coordina;
    /// toda decisión (qué se permite y qué no) se toma aquí.
    public class SolicitudService : ISolicitudService
    {
        private const int LongitudMinimaDescripcion = 10;
        private const int LongitudMinimaSolucion = 10;
        private const string AutorCoordinacion = "Coordinación";

        private readonly ISolicitudRepository _solicitudes;
        private readonly IUsuarioRepository _usuarios;
        private readonly ITipoSolicitudRepository _tipos;
        private readonly IResponsableRepository _responsables;

        public SolicitudService(
            ISolicitudRepository solicitudes,
            IUsuarioRepository usuarios,
            ITipoSolicitudRepository tipos,
            IResponsableRepository responsables)
        {
            _solicitudes = solicitudes;
            _usuarios = usuarios;
            _tipos = tipos;
            _responsables = responsables;
        }

        // Consultas

        public IList<Solicitud> Consultar(int? responsableId, EstadoSolicitud? estado, PrioridadSolicitud? prioridad)
        {
            if (estado.HasValue && !Enum.IsDefined(typeof(EstadoSolicitud), estado.Value))
            {
                throw new ReglaNegocioException("El estado indicado en el filtro no es válido.");
            }

            if (prioridad.HasValue)
            {
                ValidarPrioridad(prioridad.Value);
            }

            return _solicitudes.Consultar(responsableId, estado, prioridad);
        }

        public Solicitud ObtenerPorId(int id)
        {
            var solicitud = _solicitudes.ObtenerPorId(id);
            if (solicitud == null)
            {
                throw new RecursoNoEncontradoException("La solicitud #" + id + " no existe.");
            }

            return solicitud;
        }

        public IDictionary<EstadoSolicitud, int> ObtenerResumen()
        {
            return _solicitudes.ContarPorEstado();
        }

        //Registro

        public Solicitud Registrar(int solicitanteId, int tipoSolicitudId, string descripcion, PrioridadSolicitud prioridad)
        {
            // RN-01: toda solicitud debe tener solicitante, categoría y descripción.
            var solicitante = _usuarios.ObtenerPorId(solicitanteId);
            if (solicitante == null)
            {
                throw new ReglaNegocioException("Debe seleccionar un solicitante válido.");
            }

            if (!solicitante.Activo)
            {
                throw new ReglaNegocioException(
                    "El usuario " + solicitante.Nombre + " está inactivo y no puede registrar solicitudes.");
            }

            var tipo = _tipos.ObtenerPorId(tipoSolicitudId);
            if (tipo == null)
            {
                throw new ReglaNegocioException("Debe seleccionar una categoría válida.");
            }

            if (!tipo.Activo)
            {
                throw new ReglaNegocioException("La categoría '" + tipo.Nombre + "' está inactiva.");
            }

            if (string.IsNullOrWhiteSpace(descripcion) || descripcion.Trim().Length < LongitudMinimaDescripcion)
            {
                throw new ReglaNegocioException(
                    "La descripción es obligatoria y debe tener al menos " + LongitudMinimaDescripcion + " caracteres.");
            }

            // RN-06: la prioridad debe pertenecer a los valores definidos.
            ValidarPrioridad(prioridad);

            var solicitud = new Solicitud
            {
                SolicitanteId = solicitante.Id,
                TipoSolicitudId = tipo.Id,
                Descripcion = descripcion.Trim(),
                Prioridad = prioridad,
                Estado = EstadoSolicitud.Creada,
                FechaCreacion = DateTime.Now
            };

            solicitud.Seguimientos.Add(NuevoSeguimiento(
                TipoSeguimiento.Creacion,
                solicitante.Nombre,
                "Solicitud registrada en la categoría '" + tipo.Nombre + "' con prioridad " + prioridad.Descripcion() + "."));

            _solicitudes.Agregar(solicitud);
            return solicitud;
        }

        //Asignación y reasignación

        public void Asignar(int id, int responsableId, string comentario)
        {
            var solicitud = ObtenerEditable(id);

            if (solicitud.Estado == EstadoSolicitud.Resuelta)
            {
                throw new ReglaNegocioException(
                    "La solicitud ya fue resuelta y no puede reasignarse; solo resta cerrarla.");
            }

            // RN-04: toda asignación/reasignación debe identificar al nuevo responsable.
            var nuevo = _responsables.ObtenerPorId(responsableId);
            if (nuevo == null)
            {
                throw new ReglaNegocioException("Debe identificar al responsable al que se asignará la solicitud.");
            }

            if (!nuevo.Activo)
            {
                throw new ReglaNegocioException(
                    "El responsable " + nuevo.Nombre + " está inactivo y no puede recibir solicitudes.");
            }

            if (solicitud.ResponsableId == nuevo.Id)
            {
                throw new ReglaNegocioException(
                    "La solicitud ya está asignada a " + nuevo.Nombre + ". Seleccione un responsable distinto.");
            }

            var nota = string.IsNullOrWhiteSpace(comentario) ? string.Empty : " Comentario: " + comentario.Trim();

            if (solicitud.Estado == EstadoSolicitud.Creada)
            {
                FlujoEstados.Validar(solicitud.Estado, EstadoSolicitud.Asignada);
                solicitud.Estado = EstadoSolicitud.Asignada;
                solicitud.Responsable = nuevo;
                solicitud.ResponsableId = nuevo.Id;
                solicitud.Seguimientos.Add(NuevoSeguimiento(
                    TipoSeguimiento.Asignacion,
                    AutorCoordinacion,
                    "Solicitud asignada a " + nuevo.Nombre + "." + nota));
            }
            else
            {
                var anterior = solicitud.Responsable != null ? solicitud.Responsable.Nombre : "sin responsable";
                solicitud.Responsable = nuevo;
                solicitud.ResponsableId = nuevo.Id;
                solicitud.Seguimientos.Add(NuevoSeguimiento(
                    TipoSeguimiento.Reasignacion,
                    AutorCoordinacion,
                    "Solicitud reasignada de " + anterior + " a " + nuevo.Nombre + "." + nota));
            }

            _solicitudes.Actualizar(solicitud);
        }

        //Prioridad

        public void CambiarPrioridad(int id, PrioridadSolicitud prioridad, string motivo)
        {
            var solicitud = ObtenerEditable(id);

            ValidarPrioridad(prioridad);

            if (solicitud.Prioridad == prioridad)
            {
                throw new ReglaNegocioException("La solicitud ya tiene prioridad " + prioridad.Descripcion() + ".");
            }

            if (string.IsNullOrWhiteSpace(motivo))
            {
                throw new ReglaNegocioException("Debe indicar el motivo del cambio de prioridad.");
            }

            var anterior = solicitud.Prioridad;
            solicitud.Prioridad = prioridad;
            solicitud.Seguimientos.Add(NuevoSeguimiento(
                TipoSeguimiento.CambioPrioridad,
                AutorCoordinacion,
                "Prioridad cambiada de " + anterior.Descripcion() + " a " + prioridad.Descripcion() +
                ". Motivo: " + motivo.Trim()));

            _solicitudes.Actualizar(solicitud);
        }

        //Atención

        public void IniciarProceso(int id, string comentario)
        {
            var solicitud = ObtenerEditable(id);

            FlujoEstados.Validar(solicitud.Estado, EstadoSolicitud.EnProceso);

            if (!solicitud.ResponsableId.HasValue)
            {
                throw new ReglaNegocioException("La solicitud no tiene responsable; asígnela antes de iniciar su atención.");
            }

            solicitud.Estado = EstadoSolicitud.EnProceso;

            var texto = "La solicitud pasó a estado En proceso.";
            if (!string.IsNullOrWhiteSpace(comentario))
            {
                texto += " " + comentario.Trim();
            }

            solicitud.Seguimientos.Add(NuevoSeguimiento(
                TipoSeguimiento.CambioEstado, NombreResponsable(solicitud), texto));

            _solicitudes.Actualizar(solicitud);
        }

        public void AgregarSeguimiento(int id, string autor, string comentario)
        {
            var solicitud = ObtenerEditable(id);

            if (string.IsNullOrWhiteSpace(autor))
            {
                throw new ReglaNegocioException("Debe indicar quién registra el seguimiento.");
            }

            if (string.IsNullOrWhiteSpace(comentario))
            {
                throw new ReglaNegocioException("El comentario del seguimiento no puede estar vacío.");
            }

            solicitud.Seguimientos.Add(NuevoSeguimiento(TipoSeguimiento.Comentario, autor.Trim(), comentario.Trim()));
            _solicitudes.Actualizar(solicitud);
        }

        //Resolución y cierre

        public void Resolver(int id, string solucion)
        {
            var solicitud = ObtenerEditable(id);

            FlujoEstados.Validar(solicitud.Estado, EstadoSolicitud.Resuelta);

            // RN-05: una solicitud resuelta debe contener una solución.
            if (string.IsNullOrWhiteSpace(solucion) || solucion.Trim().Length < LongitudMinimaSolucion)
            {
                throw new ReglaNegocioException(
                    "Para resolver la solicitud debe registrar una solución de al menos " + LongitudMinimaSolucion + " caracteres.");
            }

            solicitud.Solucion = solucion.Trim();
            solicitud.Estado = EstadoSolicitud.Resuelta;
            solicitud.FechaResolucion = DateTime.Now;
            solicitud.Seguimientos.Add(NuevoSeguimiento(
                TipoSeguimiento.Solucion, NombreResponsable(solicitud), "Se registró la solución y la solicitud fue marcada como Resuelta."));

            _solicitudes.Actualizar(solicitud);
        }

        public void Cerrar(int id, string comentario)
        {
            var solicitud = ObtenerEditable(id);

            // RN-08: el responsable no puede cerrar sin haber registrado una solución.
            if (!solicitud.TieneSolucion)
            {
                throw new ReglaNegocioException(
                    "No se puede cerrar la solicitud #" + id + ": el responsable debe registrar primero una solución.");
            }

            FlujoEstados.Validar(solicitud.Estado, EstadoSolicitud.Cerrada);

            solicitud.Estado = EstadoSolicitud.Cerrada;
            solicitud.FechaCierre = DateTime.Now;

            var texto = "Solicitud cerrada.";
            if (!string.IsNullOrWhiteSpace(comentario))
            {
                texto += " " + comentario.Trim();
            }

            solicitud.Seguimientos.Add(NuevoSeguimiento(TipoSeguimiento.Cierre, NombreResponsable(solicitud), texto));
            _solicitudes.Actualizar(solicitud);
        }

        //Apoyo interno

        ///Obtiene la solicitud y verifica que pueda modificarse (RN-03).
        private Solicitud ObtenerEditable(int id)
        {
            var solicitud = ObtenerPorId(id);
            if (solicitud.EstaCerrada)
            {
                throw new ReglaNegocioException(
                    "La solicitud #" + id + " está cerrada y no puede modificarse.");
            }

            return solicitud;
        }

        private static void ValidarPrioridad(PrioridadSolicitud prioridad)
        {
            if (!Enum.IsDefined(typeof(PrioridadSolicitud), prioridad))
            {
                throw new ReglaNegocioException(
                    "La prioridad indicada no es válida. Valores permitidos: Baja, Media, Alta y Crítica.");
            }
        }

        private static string NombreResponsable(Solicitud solicitud)
        {
            return solicitud.Responsable != null ? solicitud.Responsable.Nombre : AutorCoordinacion;
        }

        private static Seguimiento NuevoSeguimiento(TipoSeguimiento tipo, string autor, string comentario)
        {
            return new Seguimiento
            {
                Fecha = DateTime.Now,
                Tipo = tipo,
                Autor = autor,
                Comentario = comentario
            };
        }
    }
}
