using System.Collections.Generic;
using SolicitudesInternas.Models;

namespace SolicitudesInternas.Interfaces
{
    public interface IUsuarioRepository
    {
        IList<Usuario> ObtenerTodos(bool soloActivos);
        Usuario ObtenerPorId(int id);
        bool ExisteCorreo(string correo, int excluirId);
        void Agregar(Usuario usuario);
        void Actualizar(Usuario usuario);
    }
}
