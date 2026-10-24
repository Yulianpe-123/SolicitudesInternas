using System;
using System.ComponentModel.DataAnnotations;
using SolicitudesInternas.Models;

namespace SolicitudesInternas.Helpers
{
    ///Utilidades de presentación para las enumeraciones del dominio.
    public static class EnumExtensions
    {
        ///Devuelve el nombre legible (atributo Display) de un valor de enumeración.
        public static string Descripcion(this Enum valor)
        {
            var campo = valor.GetType().GetField(valor.ToString());
            if (campo == null)
            {
                return valor.ToString();
            }

            var atributo = (DisplayAttribute)Attribute.GetCustomAttribute(campo, typeof(DisplayAttribute));
            return atributo != null ? atributo.GetName() : valor.ToString();
        }

        ///Clase CSS (Bootstrap 3) para mostrar el estado como etiqueta.
        public static string ClaseCss(this EstadoSolicitud estado)
        {
            switch (estado)
            {
                case EstadoSolicitud.Creada: return "label label-default";
                case EstadoSolicitud.Asignada: return "label label-info";
                case EstadoSolicitud.EnProceso: return "label label-primary";
                case EstadoSolicitud.Resuelta: return "label label-success";
                case EstadoSolicitud.Cerrada: return "label label-default";
                default: return "label label-default";
            }
        }

        /// Clase CSS (Bootstrap 3) para mostrar la prioridad como etiqueta.
        public static string ClaseCss(this PrioridadSolicitud prioridad)
        {
            switch (prioridad)
            {
                case PrioridadSolicitud.Baja: return "label label-default";
                case PrioridadSolicitud.Media: return "label label-info";
                case PrioridadSolicitud.Alta: return "label label-warning";
                case PrioridadSolicitud.Critica: return "label label-danger";
                default: return "label label-default";
            }
        }
    }
}
