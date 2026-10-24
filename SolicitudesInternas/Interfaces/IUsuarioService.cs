using System.Collections.Generic;
using SolicitudesInternas.Models;

namespace SolicitudesInternas.Interfaces
{
    public interface IUsuarioService
    {
        IList<Usuario> ObtenerTodos(bool soloActivos);
        Usuario ObtenerPorId(int id);
        void Registrar(Usuario usuario);
        void Actualizar(Usuario usuario);
    }
}
