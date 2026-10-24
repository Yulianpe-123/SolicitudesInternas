using System.Collections.Generic;
using System.Linq;
using SolicitudesInternas.Excepciones;
using SolicitudesInternas.Helpers;
using SolicitudesInternas.Models;

namespace SolicitudesInternas.Services
{
    /// Define qué transiciones de estado son válidas (RN-02). Es la única fuente de verdad
    /// del flujo: Creada → Asignada → En proceso → Resuelta → Cerrada.
    public static class FlujoEstados
    {
        private static readonly Dictionary<EstadoSolicitud, EstadoSolicitud[]> Permitidas =
            new Dictionary<EstadoSolicitud, EstadoSolicitud[]>
            {
                { EstadoSolicitud.Creada, new[] { EstadoSolicitud.Asignada } },
                { EstadoSolicitud.Asignada, new[] { EstadoSolicitud.EnProceso } },
                { EstadoSolicitud.EnProceso, new[] { EstadoSolicitud.Resuelta } },
                { EstadoSolicitud.Resuelta, new[] { EstadoSolicitud.Cerrada } },
                { EstadoSolicitud.Cerrada, new EstadoSolicitud[0] }
            };

        public static bool EsPermitida(EstadoSolicitud origen, EstadoSolicitud destino)
        {
            EstadoSolicitud[] destinos;
            return Permitidas.TryGetValue(origen, out destinos) && destinos.Contains(destino);
        }

        ///Lanza ReglaNegocioException si la transición no está permitida.
        public static void Validar(EstadoSolicitud origen, EstadoSolicitud destino)
        {
            if (EsPermitida(origen, destino))
            {
                return;
            }

            if (origen == EstadoSolicitud.Cerrada)
            {
                throw new ReglaNegocioException(
                    "La solicitud está cerrada y no admite más cambios de estado.");
            }

            var siguiente = Permitidas[origen].First();
            throw new ReglaNegocioException(string.Format(
                "No es posible pasar la solicitud de '{0}' a '{1}'. El siguiente paso permitido es '{2}'.",
                origen.Descripcion(), destino.Descripcion(), siguiente.Descripcion()));
        }
    }
}
