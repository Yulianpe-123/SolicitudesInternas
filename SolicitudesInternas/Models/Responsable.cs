using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace SolicitudesInternas.Models
{
    ///Persona encargada de atender y resolver solicitudes.
    public class Responsable
    {
        public Responsable()
        {
            Activo = true;
            Solicitudes = new List<Solicitud>();
        }

        public int Id { get; set; }

        [Required(ErrorMessage = "El nombre es obligatorio.")]
        [StringLength(100, ErrorMessage = "El nombre no puede superar los 100 caracteres.")]
        public string Nombre { get; set; }

        [Required(ErrorMessage = "El correo es obligatorio.")]
        [EmailAddress(ErrorMessage = "Ingrese un correo electrónico válido.")]
        [StringLength(150, ErrorMessage = "El correo no puede superar los 150 caracteres.")]
        public string Correo { get; set; }

        [Required(ErrorMessage = "La especialidad es obligatoria.")]
        [StringLength(100, ErrorMessage = "La especialidad no puede superar los 100 caracteres.")]
        public string Especialidad { get; set; }

        public bool Activo { get; set; }

        public virtual ICollection<Solicitud> Solicitudes { get; set; }
    }
}
