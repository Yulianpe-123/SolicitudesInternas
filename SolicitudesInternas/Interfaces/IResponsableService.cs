using System.Collections.Generic;
using SolicitudesInternas.Models;

namespace SolicitudesInternas.Interfaces
{
    public interface IResponsableService
    {
        IList<Responsable> ObtenerTodos(bool soloActivos);
        Responsable ObtenerPorId(int id);
        void Registrar(Responsable responsable);
        void Actualizar(Responsable responsable);
    }
}
