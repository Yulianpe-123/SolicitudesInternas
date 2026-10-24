using System.Collections.Generic;
using System.Linq;
using SolicitudesInternas.Data;
using SolicitudesInternas.Interfaces;
using SolicitudesInternas.Models;

namespace SolicitudesInternas.Repositories
{
    public class UsuarioRepository : RepositorioBase, IUsuarioRepository
    {
        public UsuarioRepository(SolicitudesContext contexto) : base(contexto)
        {
        }

        public IList<Usuario> ObtenerTodos(bool soloActivos)
        {
            IQueryable<Usuario> consulta = Contexto.Usuarios;
            if (soloActivos)
            {
                consulta = consulta.Where(u => u.Activo);
            }

            return consulta.OrderBy(u => u.Nombre).ToList();
        }

        public Usuario ObtenerPorId(int id)
        {
            return Contexto.Usuarios.FirstOrDefault(u => u.Id == id);
        }

        public bool ExisteCorreo(string correo, int excluirId)
        {
            return Contexto.Usuarios.Any(u => u.Correo == correo && u.Id != excluirId);
        }

        public void Agregar(Usuario usuario)
        {
            Contexto.Usuarios.Add(usuario);
            GuardarCambios();
        }

        public void Actualizar(Usuario usuario)
        {
            GuardarEntidad(usuario);
        }
    }
}
