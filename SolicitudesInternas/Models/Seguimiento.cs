using System;
using System.ComponentModel.DataAnnotations;

namespace SolicitudesInternas.Models
{
    /// Registro histórico de un evento de la solicitud. Nunca se modifica ni se elimina (RN-07).
    public class Seguimiento
    {
        public int Id { get; set; }

        public int SolicitudId { get; set; }

        public DateTime Fecha { get; set; }

        public TipoSeguimiento Tipo { get; set; }

        [Required]
        [StringLength(100)]
        public string Autor { get; set; }

        [Required]
        [StringLength(1000)]
        public string Comentario { get; set; }

        public virtual Solicitud Solicitud { get; set; }
    }
}
