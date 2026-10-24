using System;
using System.Collections.Generic;
using SolicitudesInternas.Excepciones;
using SolicitudesInternas.Interfaces;
using SolicitudesInternas.Models;

namespace SolicitudesInternas.Services
{
    public class UsuarioService : IUsuarioService
    {
        private readonly IUsuarioRepository _usuarios;

        public UsuarioService(IUsuarioRepository usuarios)
        {
            _usuarios = usuarios;
        }

        public IList<Usuario> ObtenerTodos(bool soloActivos)
        {
            return _usuarios.ObtenerTodos(soloActivos);
        }

        public Usuario ObtenerPorId(int id)
        {
            var usuario = _usuarios.ObtenerPorId(id);
            if (usuario == null)
            {
                throw new RecursoNoEncontradoException("El usuario solicitado no existe.");
            }

            return usuario;
        }

        public void Registrar(Usuario usuario)
        {
            Normalizar(usuario);

            if (_usuarios.ExisteCorreo(usuario.Correo, 0))
            {
                throw new ReglaNegocioException("Ya existe un usuario registrado con el correo " + usuario.Correo + ".");
            }

            usuario.Id = 0;
            _usuarios.Agregar(usuario);
        }

        public void Actualizar(Usuario usuario)
        {
            Normalizar(usuario);
            var existente = ObtenerPorId(usuario.Id);

            if (_usuarios.ExisteCorreo(usuario.Correo, usuario.Id))
            {
                throw new ReglaNegocioException("Otro usuario ya utiliza el correo " + usuario.Correo + ".");
            }

            existente.Nombre = usuario.Nombre;
            existente.Correo = usuario.Correo;
            existente.Departamento = usuario.Departamento;
            existente.Activo = usuario.Activo;

            _usuarios.Actualizar(existente);
        }

        private static void Normalizar(Usuario usuario)
        {
            if (usuario == null)
            {
                throw new ArgumentNullException("usuario");
            }

            if (string.IsNullOrWhiteSpace(usuario.Nombre) ||
                string.IsNullOrWhiteSpace(usuario.Correo) ||
                string.IsNullOrWhiteSpace(usuario.Departamento))
            {
                throw new ReglaNegocioException("Nombre, correo y departamento son obligatorios.");
            }

            usuario.Nombre = usuario.Nombre.Trim();
            usuario.Correo = usuario.Correo.Trim().ToLowerInvariant();
            usuario.Departamento = usuario.Departamento.Trim();
        }
    }
}
