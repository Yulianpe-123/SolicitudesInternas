using System.Collections.Generic;
using SolicitudesInternas.Models;

namespace SolicitudesInternas.Interfaces
{
    public interface ITipoSolicitudRepository
    {
        IList<TipoSolicitud> ObtenerTodos(bool soloActivos);
        TipoSolicitud ObtenerPorId(int id);
        bool ExisteNombre(string nombre, int excluirId);
        void Agregar(TipoSolicitud tipo);
        void Actualizar(TipoSolicitud tipo);
    }
}
