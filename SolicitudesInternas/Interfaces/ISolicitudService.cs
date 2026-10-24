using System.Collections.Generic;
using SolicitudesInternas.Models;

namespace SolicitudesInternas.Interfaces
{
    public interface ISolicitudService
    {
        // Consultas
        IList<Solicitud> Consultar(int? responsableId, EstadoSolicitud? estado, PrioridadSolicitud? prioridad);
        Solicitud ObtenerPorId(int id);
        IDictionary<EstadoSolicitud, int> ObtenerResumen();

        // Operaciones del proceso
        Solicitud Registrar(int solicitanteId, int tipoSolicitudId, string descripcion, PrioridadSolicitud prioridad);
        void Asignar(int id, int responsableId, string comentario);
        void CambiarPrioridad(int id, PrioridadSolicitud prioridad, string motivo);
        void IniciarProceso(int id, string comentario);
        void AgregarSeguimiento(int id, string autor, string comentario);
        void Resolver(int id, string solucion);
        void Cerrar(int id, string comentario);
    }
}
