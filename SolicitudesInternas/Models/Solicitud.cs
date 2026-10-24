using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace SolicitudesInternas.Models
{
    /// Solicitud interna (equipo, acceso, permiso o soporte). Solo representa datos y estado;
    /// las reglas que gobiernan sus cambios viven en SolicitudService.
    public class Solicitud
    {
        public Solicitud()
        {
            Estado = EstadoSolicitud.Creada;
            Prioridad = PrioridadSolicitud.Media;
            Seguimientos = new List<Seguimiento>();
        }

        public int Id { get; set; }

        [Required]
        public int SolicitanteId { get; set; }

        [Required]
        public int TipoSolicitudId { get; set; }

        [Required(ErrorMessage = "La descripción es obligatoria.")]
        [StringLength(1000, ErrorMessage = "La descripción no puede superar los 1000 caracteres.")]
        public string Descripcion { get; set; }

        public PrioridadSolicitud Prioridad { get; set; }

        public EstadoSolicitud Estado { get; set; }

        public int? ResponsableId { get; set; }

        [StringLength(2000)]
        public string Solucion { get; set; }

        public DateTime FechaCreacion { get; set; }

        public DateTime? FechaResolucion { get; set; }

        public DateTime? FechaCierre { get; set; }

        public virtual Usuario Solicitante { get; set; }

        public virtual TipoSolicitud TipoSolicitud { get; set; }

        public virtual Responsable Responsable { get; set; }

        public virtual ICollection<Seguimiento> Seguimientos { get; set; }

        /// <summary>Indica si la solicitud ya no admite modificaciones (RN-03).</summary>
        public bool EstaCerrada
        {
            get { return Estado == EstadoSolicitud.Cerrada; }
        }

        /// <summary>Indica si ya se registró una solución (RN-05, RN-08).</summary>
        public bool TieneSolucion
        {
            get { return !string.IsNullOrWhiteSpace(Solucion); }
        }
    }
}
