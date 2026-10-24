using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;
using SolicitudesInternas.Models;

namespace SolicitudesInternas.ViewModels
{
    ///Datos del formulario de registro de una solicitud (validación de entrada).
    public class SolicitudCrearViewModel
    {
        public SolicitudCrearViewModel()
        {
            Prioridad = PrioridadSolicitud.Media;
        }

        [Required(ErrorMessage = "Debe seleccionar el solicitante.")]
        [Display(Name = "Solicitante")]
        public int? SolicitanteId { get; set; }

        [Required(ErrorMessage = "Debe seleccionar la categoría.")]
        [Display(Name = "Categoría")]
        public int? TipoSolicitudId { get; set; }

        [Required(ErrorMessage = "La descripción es obligatoria.")]
        [StringLength(1000, MinimumLength = 10, ErrorMessage = "La descripción debe tener entre 10 y 1000 caracteres.")]
        [Display(Name = "Descripción")]
        public string Descripcion { get; set; }

        [Display(Name = "Prioridad")]
        public PrioridadSolicitud Prioridad { get; set; }

        public IEnumerable<SelectListItem> Usuarios { get; set; }

        public IEnumerable<SelectListItem> Tipos { get; set; }
    }
}
