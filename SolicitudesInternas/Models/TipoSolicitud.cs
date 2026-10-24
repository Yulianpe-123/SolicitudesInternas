using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace SolicitudesInternas.Models
{
    ///Categoría de la solicitud (equipos, accesos, permisos, soporte, etc.).
    public class TipoSolicitud
    {
        public TipoSolicitud()
        {
            Activo = true;
            Solicitudes = new List<Solicitud>();
        }

        public int Id { get; set; }

        [Required(ErrorMessage = "El nombre de la categoría es obligatorio.")]
        [StringLength(60, ErrorMessage = "El nombre no puede superar los 60 caracteres.")]
        public string Nombre { get; set; }

        [StringLength(200, ErrorMessage = "La descripción no puede superar los 200 caracteres.")]
        public string Descripcion { get; set; }

        public bool Activo { get; set; }

        public virtual ICollection<Solicitud> Solicitudes { get; set; }
    }
}
