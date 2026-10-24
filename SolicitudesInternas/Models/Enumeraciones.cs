using System.ComponentModel.DataAnnotations;

namespace SolicitudesInternas.Models
{
    /// Ciclo de vida de una solicitud: Creada → Asignada → En proceso → Resuelta → Cerrada.
    public enum EstadoSolicitud
    {
        Creada = 1,
        Asignada = 2,
        [Display(Name = "En proceso")]
        EnProceso = 3,
        Resuelta = 4,
        Cerrada = 5
    }

    ///Valores de prioridad permitidos (RN-06).
    public enum PrioridadSolicitud
    {
        Baja = 1,
        Media = 2,
        Alta = 3,
        [Display(Name = "Crítica")]
        Critica = 4
    }

    ///Tipo de evento que se registra en el historial de una solicitud.
    public enum TipoSeguimiento
    {
        [Display(Name = "Creación")]
        Creacion = 1,
        [Display(Name = "Asignación")]
        Asignacion = 2,
        [Display(Name = "Reasignación")]
        Reasignacion = 3,
        [Display(Name = "Cambio de estado")]
        CambioEstado = 4,
        [Display(Name = "Cambio de prioridad")]
        CambioPrioridad = 5,
        Comentario = 6,
        [Display(Name = "Solución")]
        Solucion = 7,
        Cierre = 8
    }
}
