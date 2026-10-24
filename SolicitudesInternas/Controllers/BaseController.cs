using System;
using System.Collections.Generic;
using System.Web.Mvc;
using SolicitudesInternas.Excepciones;

namespace SolicitudesInternas.Controllers
{
    /// Comportamiento común de los controladores: mensajes de confirmación/error,
    /// liberación de recursos y manejo centralizado de excepciones no controladas.
    public abstract class BaseController : Controller
    {
        private readonly List<IDisposable> _recursos = new List<IDisposable>();

        ///Registra un recurso (p. ej. el DbContext) para liberarlo al terminar la petición.
        public void RegistrarRecurso(IDisposable recurso)
        {
            _recursos.Add(recurso);
        }

        protected void MensajeExito(string mensaje)
        {
            TempData["Exito"] = mensaje;
        }

        protected void MensajeError(string mensaje)
        {
            TempData["Error"] = mensaje;
        }

        /// Última barrera: ninguna excepción técnica llega al usuario. Los errores propios de la
        /// aplicación muestran su mensaje; cualquier otro muestra un texto genérico.
        protected override void OnException(ExceptionContext filterContext)
        {
            if (filterContext.ExceptionHandled)
            {
                return;
            }

            var excepcion = filterContext.Exception;
            string mensaje;
            int codigo;

            if (excepcion is RecursoNoEncontradoException)
            {
                mensaje = excepcion.Message;
                codigo = 404;
            }
            else if (excepcion is AplicacionException)
            {
                mensaje = excepcion.Message;
                codigo = 400;
            }
            else
            {
                mensaje = "Ocurrió un error inesperado. Intente nuevamente o contacte al administrador del sistema.";
                codigo = 500;
            }

            var resultado = new ViewResult { ViewName = "Error" };
            resultado.ViewData["Mensaje"] = mensaje;

            filterContext.ExceptionHandled = true;
            filterContext.HttpContext.Response.Clear();
            filterContext.HttpContext.Response.StatusCode = codigo;
            filterContext.HttpContext.Response.TrySkipIisCustomErrors = true;
            filterContext.Result = resultado;
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                foreach (var recurso in _recursos)
                {
                    recurso.Dispose();
                }

                _recursos.Clear();
            }

            base.Dispose(disposing);
        }
    }
}
