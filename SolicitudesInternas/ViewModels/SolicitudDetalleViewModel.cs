using System.Collections.Generic;
using System.Web.Mvc;
using SolicitudesInternas.Models;

namespace SolicitudesInternas.ViewModels
{
    ///Datos de la pantalla de detalle, que concentra las operaciones del proceso.
    public class SolicitudDetalleViewModel
    {
        public Solicitud Solicitud { get; set; }

        public IEnumerable<SelectListItem> Responsables { get; set; }

        public IEnumerable<SelectListItem> Prioridades { get; set; }
    }
}
