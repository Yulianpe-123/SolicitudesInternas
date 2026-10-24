using System.Collections.Generic;
using SolicitudesInternas.Models;

namespace SolicitudesInternas.Interfaces
{
    public interface ITipoSolicitudService
    {
        IList<TipoSolicitud> ObtenerTodos(bool soloActivos);
        TipoSolicitud ObtenerPorId(int id);
        void Registrar(TipoSolicitud tipo);
        void Actualizar(TipoSolicitud tipo);
    }
}
