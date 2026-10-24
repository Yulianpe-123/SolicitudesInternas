using System.Collections.Generic;
using System.Linq;
using SolicitudesInternas.Data;
using SolicitudesInternas.Interfaces;
using SolicitudesInternas.Models;

namespace SolicitudesInternas.Repositories
{
    public class TipoSolicitudRepository : RepositorioBase, ITipoSolicitudRepository
    {
        public TipoSolicitudRepository(SolicitudesContext contexto) : base(contexto)
        {
        }

        public IList<TipoSolicitud> ObtenerTodos(bool soloActivos)
        {
            IQueryable<TipoSolicitud> consulta = Contexto.TiposSolicitud;
            if (soloActivos)
            {
                consulta = consulta.Where(t => t.Activo);
            }

            return consulta.OrderBy(t => t.Nombre).ToList();
        }

        public TipoSolicitud ObtenerPorId(int id)
        {
            return Contexto.TiposSolicitud.FirstOrDefault(t => t.Id == id);
        }

        public bool ExisteNombre(string nombre, int excluirId)
        {
            return Contexto.TiposSolicitud.Any(t => t.Nombre == nombre && t.Id != excluirId);
        }

        public void Agregar(TipoSolicitud tipo)
        {
            Contexto.TiposSolicitud.Add(tipo);
            GuardarCambios();
        }

        public void Actualizar(TipoSolicitud tipo)
        {
            GuardarEntidad(tipo);
        }
    }
}
