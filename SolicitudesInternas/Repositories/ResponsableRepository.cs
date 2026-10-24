using System.Collections.Generic;
using System.Linq;
using SolicitudesInternas.Data;
using SolicitudesInternas.Interfaces;
using SolicitudesInternas.Models;

namespace SolicitudesInternas.Repositories
{
    public class ResponsableRepository : RepositorioBase, IResponsableRepository
    {
        public ResponsableRepository(SolicitudesContext contexto) : base(contexto)
        {
        }

        public IList<Responsable> ObtenerTodos(bool soloActivos)
        {
            IQueryable<Responsable> consulta = Contexto.Responsables;
            if (soloActivos)
            {
                consulta = consulta.Where(r => r.Activo);
            }

            return consulta.OrderBy(r => r.Nombre).ToList();
        }

        public Responsable ObtenerPorId(int id)
        {
            return Contexto.Responsables.FirstOrDefault(r => r.Id == id);
        }

        public bool ExisteCorreo(string correo, int excluirId)
        {
            return Contexto.Responsables.Any(r => r.Correo == correo && r.Id != excluirId);
        }

        public void Agregar(Responsable responsable)
        {
            Contexto.Responsables.Add(responsable);
            GuardarCambios();
        }

        public void Actualizar(Responsable responsable)
        {
            GuardarEntidad(responsable);
        }
    }
}
