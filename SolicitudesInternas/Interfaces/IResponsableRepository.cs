using System.Collections.Generic;
using SolicitudesInternas.Models;

namespace SolicitudesInternas.Interfaces
{
    public interface IResponsableRepository
    {
        IList<Responsable> ObtenerTodos(bool soloActivos);
        Responsable ObtenerPorId(int id);
        bool ExisteCorreo(string correo, int excluirId);
        void Agregar(Responsable responsable);
        void Actualizar(Responsable responsable);
    }
}
