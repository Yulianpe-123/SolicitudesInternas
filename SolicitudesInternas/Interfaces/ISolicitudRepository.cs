using System.Collections.Generic;
using SolicitudesInternas.Models;

namespace SolicitudesInternas.Interfaces
{
    public interface ISolicitudRepository
    {
        /// Devuelve la solicitud con solicitante, categoría, responsable e historial, o null si no existe.
        Solicitud ObtenerPorId(int id);

        /// Consulta solicitudes aplicando únicamente los filtros que tengan valor.
        IList<Solicitud> Consultar(int? responsableId, EstadoSolicitud? estado, PrioridadSolicitud? prioridad);

        /// Cantidad de solicitudes por estado (incluye los estados con cero).
        IDictionary<EstadoSolicitud, int> ContarPorEstado();

        /// Cantidad de solicitudes asignadas al responsable que aún no están cerradas.
        int ContarAbiertasPorResponsable(int responsableId);

        void Agregar(Solicitud solicitud);

        ///Guarda los cambios de la solicitud y de los seguimientos nuevos en una sola operación.
        void Actualizar(Solicitud solicitud);
    }
}
