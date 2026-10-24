using System;
using System.Collections.Generic;
using SolicitudesInternas.Excepciones;
using SolicitudesInternas.Interfaces;
using SolicitudesInternas.Models;

namespace SolicitudesInternas.Services
{
    public class ResponsableService : IResponsableService
    {
        private readonly IResponsableRepository _responsables;
        private readonly ISolicitudRepository _solicitudes;

        public ResponsableService(IResponsableRepository responsables, ISolicitudRepository solicitudes)
        {
            _responsables = responsables;
            _solicitudes = solicitudes;
        }

        public IList<Responsable> ObtenerTodos(bool soloActivos)
        {
            return _responsables.ObtenerTodos(soloActivos);
        }

        public Responsable ObtenerPorId(int id)
        {
            var responsable = _responsables.ObtenerPorId(id);
            if (responsable == null)
            {
                throw new RecursoNoEncontradoException("El responsable solicitado no existe.");
            }

            return responsable;
        }

        public void Registrar(Responsable responsable)
        {
            Normalizar(responsable);

            if (_responsables.ExisteCorreo(responsable.Correo, 0))
            {
                throw new ReglaNegocioException("Ya existe un responsable registrado con el correo " + responsable.Correo + ".");
            }

            responsable.Id = 0;
            _responsables.Agregar(responsable);
        }

        public void Actualizar(Responsable responsable)
        {
            Normalizar(responsable);
            var existente = ObtenerPorId(responsable.Id);

            if (_responsables.ExisteCorreo(responsable.Correo, responsable.Id))
            {
                throw new ReglaNegocioException("Otro responsable ya utiliza el correo " + responsable.Correo + ".");
            }

            // RN-09: un responsable no puede desactivarse mientras tenga solicitudes sin cerrar.
            if (existente.Activo && !responsable.Activo)
            {
                var abiertas = _solicitudes.ContarAbiertasPorResponsable(existente.Id);
                if (abiertas > 0)
                {
                    throw new ReglaNegocioException(
                        "No se puede desactivar a " + existente.Nombre + " porque tiene " + abiertas +
                        " solicitud(es) sin cerrar. Reasígnelas o ciérrelas primero.");
                }
            }

            existente.Nombre = responsable.Nombre;
            existente.Correo = responsable.Correo;
            existente.Especialidad = responsable.Especialidad;
            existente.Activo = responsable.Activo;

            _responsables.Actualizar(existente);
        }

        private static void Normalizar(Responsable responsable)
        {
            if (responsable == null)
            {
                throw new ArgumentNullException("responsable");
            }

            if (string.IsNullOrWhiteSpace(responsable.Nombre) ||
                string.IsNullOrWhiteSpace(responsable.Correo) ||
                string.IsNullOrWhiteSpace(responsable.Especialidad))
            {
                throw new ReglaNegocioException("Nombre, correo y especialidad son obligatorios.");
            }

            responsable.Nombre = responsable.Nombre.Trim();
            responsable.Correo = responsable.Correo.Trim().ToLowerInvariant();
            responsable.Especialidad = responsable.Especialidad.Trim();
        }
    }
}
