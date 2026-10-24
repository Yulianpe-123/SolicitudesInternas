using System.ComponentModel.DataAnnotations;
using SolicitudesInternas.Models;

namespace SolicitudesInternas.ViewModels
{
    // Un ViewModel por operación del proceso. Cada uno valida únicamente los datos de entrada;
    // las reglas de negocio se verifican después en SolicitudService.

    public class AsignarViewModel
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Debe seleccionar el responsable.")]
        public int? ResponsableId { get; set; }

        [StringLength(500, ErrorMessage = "El comentario no puede superar los 500 caracteres.")]
        public string Comentario { get; set; }
    }

    public class CambiarPrioridadViewModel
    {
        public int Id { get; set; }

        public PrioridadSolicitud Prioridad { get; set; }

        [Required(ErrorMessage = "Debe indicar el motivo del cambio de prioridad.")]
        [StringLength(500, ErrorMessage = "El motivo no puede superar los 500 caracteres.")]
        public string Motivo { get; set; }
    }

    public class IniciarProcesoViewModel
    {
        public int Id { get; set; }

        [StringLength(500, ErrorMessage = "El comentario no puede superar los 500 caracteres.")]
        public string Comentario { get; set; }
    }

    public class SeguimientoViewModel
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Debe indicar quién registra el seguimiento.")]
        [StringLength(100, ErrorMessage = "El autor no puede superar los 100 caracteres.")]
        public string Autor { get; set; }

        [Required(ErrorMessage = "El comentario del seguimiento es obligatorio.")]
        [StringLength(900, ErrorMessage = "El comentario no puede superar los 900 caracteres.")]
        public string Comentario { get; set; }
    }

    public class ResolverViewModel
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Debe registrar la solución para resolver la solicitud.")]
        [StringLength(2000, ErrorMessage = "La solución no puede superar los 2000 caracteres.")]
        public string Solucion { get; set; }
    }

    public class CerrarViewModel
    {
        public int Id { get; set; }

        [StringLength(500, ErrorMessage = "El comentario no puede superar los 500 caracteres.")]
        public string Comentario { get; set; }
    }
}
