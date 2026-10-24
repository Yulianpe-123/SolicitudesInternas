using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using SolicitudesInternas.Data;
using SolicitudesInternas.Interfaces;
using SolicitudesInternas.Models;

namespace SolicitudesInternas.Repositories
{
    public class SolicitudRepository : RepositorioBase, ISolicitudRepository
    {
        public SolicitudRepository(SolicitudesContext contexto) : base(contexto)
        {
        }

        public Solicitud ObtenerPorId(int id)
        {
            return Contexto.Solicitudes
                .Include(s => s.Solicitante)
                .Include(s => s.TipoSolicitud)
                .Include(s => s.Responsable)
                .Include(s => s.Seguimientos)
                .FirstOrDefault(s => s.Id == id);
        }

        public IList<Solicitud> Consultar(int? responsableId, EstadoSolicitud? estado, PrioridadSolicitud? prioridad)
        {
            IQueryable<Solicitud> consulta = Contexto.Solicitudes
                .Include(s => s.Solicitante)
                .Include(s => s.TipoSolicitud)
                .Include(s => s.Responsable);

            if (responsableId.HasValue)
            {
                consulta = consulta.Where(s => s.ResponsableId == responsableId.Value);
            }

            if (estado.HasValue)
            {
                consulta = consulta.Where(s => s.Estado == estado.Value);
            }

            if (prioridad.HasValue)
            {
                consulta = consulta.Where(s => s.Prioridad == prioridad.Value);
            }

            return consulta.OrderByDescending(s => s.FechaCreacion).ThenByDescending(s => s.Id).ToList();
        }

        public IDictionary<EstadoSolicitud, int> ContarPorEstado()
        {
            var conteos = Contexto.Solicitudes
                .GroupBy(s => s.Estado)
                .Select(g => new { Estado = g.Key, Total = g.Count() })
                .ToList();

            var resultado = new Dictionary<EstadoSolicitud, int>();
            foreach (EstadoSolicitud estado in Enum.GetValues(typeof(EstadoSolicitud)))
            {
                var fila = conteos.FirstOrDefault(c => c.Estado == estado);
                resultado[estado] = fila != null ? fila.Total : 0;
            }

            return resultado;
        }

        public int ContarAbiertasPorResponsable(int responsableId)
        {
            return Contexto.Solicitudes.Count(s =>
                s.ResponsableId == responsableId && s.Estado != EstadoSolicitud.Cerrada);
        }

        public void Agregar(Solicitud solicitud)
        {
            Contexto.Solicitudes.Add(solicitud);
            GuardarCambios();
        }

        public void Actualizar(Solicitud solicitud)
        {
            GuardarEntidad(solicitud);
        }
    }
}
