using System.Collections.Generic;
using System.Web.Mvc;
using SolicitudesInternas.Models;

namespace SolicitudesInternas.ViewModels
{
    ///Resultado del listado de solicitudes junto con los filtros aplicados.
    public class SolicitudListadoViewModel
    {
        public int? ResponsableId { get; set; }

        public EstadoSolicitud? Estado { get; set; }

        public PrioridadSolicitud? Prioridad { get; set; }

        public IList<Solicitud> Solicitudes { get; set; }

        public IEnumerable<SelectListItem> Responsables { get; set; }
    }
}
