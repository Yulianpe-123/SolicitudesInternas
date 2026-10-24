using System;
using System.Collections.Generic;
using SolicitudesInternas.Excepciones;
using SolicitudesInternas.Interfaces;
using SolicitudesInternas.Models;

namespace SolicitudesInternas.Services
{
    public class TipoSolicitudService : ITipoSolicitudService
    {
        private readonly ITipoSolicitudRepository _tipos;

        public TipoSolicitudService(ITipoSolicitudRepository tipos)
        {
            _tipos = tipos;
        }

        public IList<TipoSolicitud> ObtenerTodos(bool soloActivos)
        {
            return _tipos.ObtenerTodos(soloActivos);
        }

        public TipoSolicitud ObtenerPorId(int id)
        {
            var tipo = _tipos.ObtenerPorId(id);
            if (tipo == null)
            {
                throw new RecursoNoEncontradoException("La categoría solicitada no existe.");
            }

            return tipo;
        }

        public void Registrar(TipoSolicitud tipo)
        {
            Normalizar(tipo);

            if (_tipos.ExisteNombre(tipo.Nombre, 0))
            {
                throw new ReglaNegocioException("Ya existe una categoría llamada '" + tipo.Nombre + "'.");
            }

            tipo.Id = 0;
            _tipos.Agregar(tipo);
        }

        public void Actualizar(TipoSolicitud tipo)
        {
            Normalizar(tipo);
            var existente = ObtenerPorId(tipo.Id);

            if (_tipos.ExisteNombre(tipo.Nombre, tipo.Id))
            {
                throw new ReglaNegocioException("Ya existe otra categoría llamada '" + tipo.Nombre + "'.");
            }

            existente.Nombre = tipo.Nombre;
            existente.Descripcion = tipo.Descripcion;
            existente.Activo = tipo.Activo;

            _tipos.Actualizar(existente);
        }

        private static void Normalizar(TipoSolicitud tipo)
        {
            if (tipo == null)
            {
                throw new ArgumentNullException("tipo");
            }

            if (string.IsNullOrWhiteSpace(tipo.Nombre))
            {
                throw new ReglaNegocioException("El nombre de la categoría es obligatorio.");
            }

            tipo.Nombre = tipo.Nombre.Trim();
            tipo.Descripcion = string.IsNullOrWhiteSpace(tipo.Descripcion) ? null : tipo.Descripcion.Trim();
        }
    }
}
