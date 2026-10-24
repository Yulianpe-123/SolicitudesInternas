using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using SolicitudesInternas.Excepciones;
using SolicitudesInternas.Helpers;
using SolicitudesInternas.Interfaces;
using SolicitudesInternas.Models;
using SolicitudesInternas.ViewModels;

namespace SolicitudesInternas.Controllers
{
    /// Recibe las acciones del usuario, valida los datos de entrada, invoca al servicio
    /// y decide qué vista mostrar. No contiene reglas de negocio.
    public class SolicitudesController : BaseController
    {
        private readonly ISolicitudService _solicitudService;
        private readonly IUsuarioService _usuarioService;
        private readonly IResponsableService _responsableService;
        private readonly ITipoSolicitudService _tipoService;

        public SolicitudesController(
            ISolicitudService solicitudService,
            IUsuarioService usuarioService,
            IResponsableService responsableService,
            ITipoSolicitudService tipoService)
        {
            _solicitudService = solicitudService;
            _usuarioService = usuarioService;
            _responsableService = responsableService;
            _tipoService = tipoService;
        }

        // GET: Solicitudes  (consultas por responsable, estado y prioridad)
        public ActionResult Index(int? responsableId, EstadoSolicitud? estado, PrioridadSolicitud? prioridad)
        {
            var modelo = new SolicitudListadoViewModel
            {
                ResponsableId = responsableId,
                Estado = estado,
                Prioridad = prioridad,
                Responsables = new SelectList(_responsableService.ObtenerTodos(false), "Id", "Nombre", responsableId)
            };

            try
            {
                modelo.Solicitudes = _solicitudService.Consultar(responsableId, estado, prioridad);
            }
            catch (ReglaNegocioException ex)
            {
                MensajeError(ex.Message);
                modelo.Solicitudes = new List<Solicitud>();
            }

            return View(modelo);
        }

        // GET: Solicitudes/Crear
        public ActionResult Crear()
        {
            var modelo = new SolicitudCrearViewModel();
            CargarListas(modelo);
            return View(modelo);
        }

        // POST: Solicitudes/Crear
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Crear(SolicitudCrearViewModel modelo)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var solicitud = _solicitudService.Registrar(
                        modelo.SolicitanteId.Value,
                        modelo.TipoSolicitudId.Value,
                        modelo.Descripcion,
                        modelo.Prioridad);

                    MensajeExito("La solicitud #" + solicitud.Id + " fue registrada correctamente.");
                    return RedirectToAction("Detalle", new { id = solicitud.Id });
                }
                catch (AplicacionException ex)
                {
                    ModelState.AddModelError(string.Empty, ex.Message);
                }
            }

            CargarListas(modelo);
            return View(modelo);
        }

        // GET: Solicitudes/Detalle/5
        public ActionResult Detalle(int id)
        {
            var solicitud = _solicitudService.ObtenerPorId(id);

            var modelo = new SolicitudDetalleViewModel
            {
                Solicitud = solicitud,
                Responsables = new SelectList(_responsableService.ObtenerTodos(true), "Id", "Nombre"),
                Prioridades = Enum.GetValues(typeof(PrioridadSolicitud))
                    .Cast<PrioridadSolicitud>()
                    .Select(p => new SelectListItem
                    {
                        Value = ((int)p).ToString(),
                        Text = p.Descripcion(),
                        Selected = p == solicitud.Prioridad
                    })
                    .ToList()
            };

            return View(modelo);
        }

        // POST: Solicitudes/Asignar
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Asignar(AsignarViewModel modelo)
        {
            if (!ModelState.IsValid)
            {
                return VolverConErroresDeEntrada(modelo.Id);
            }

            return Ejecutar(
                modelo.Id,
                () => _solicitudService.Asignar(modelo.Id, modelo.ResponsableId.Value, modelo.Comentario),
                "La asignación se registró correctamente.");
        }

        // POST: Solicitudes/CambiarPrioridad
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CambiarPrioridad(CambiarPrioridadViewModel modelo)
        {
            if (!ModelState.IsValid)
            {
                return VolverConErroresDeEntrada(modelo.Id);
            }

            return Ejecutar(
                modelo.Id,
                () => _solicitudService.CambiarPrioridad(modelo.Id, modelo.Prioridad, modelo.Motivo),
                "La prioridad se actualizó correctamente.");
        }

        // POST: Solicitudes/IniciarProceso
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult IniciarProceso(IniciarProcesoViewModel modelo)
        {
            if (!ModelState.IsValid)
            {
                return VolverConErroresDeEntrada(modelo.Id);
            }

            return Ejecutar(
                modelo.Id,
                () => _solicitudService.IniciarProceso(modelo.Id, modelo.Comentario),
                "La solicitud pasó a estado En proceso.");
        }

        // POST: Solicitudes/AgregarSeguimiento
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AgregarSeguimiento(SeguimientoViewModel modelo)
        {
            if (!ModelState.IsValid)
            {
                return VolverConErroresDeEntrada(modelo.Id);
            }

            return Ejecutar(
                modelo.Id,
                () => _solicitudService.AgregarSeguimiento(modelo.Id, modelo.Autor, modelo.Comentario),
                "El seguimiento se registró correctamente.");
        }

        // POST: Solicitudes/Resolver
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Resolver(ResolverViewModel modelo)
        {
            if (!ModelState.IsValid)
            {
                return VolverConErroresDeEntrada(modelo.Id);
            }

            return Ejecutar(
                modelo.Id,
                () => _solicitudService.Resolver(modelo.Id, modelo.Solucion),
                "La solicitud fue marcada como Resuelta.");
        }

        // POST: Solicitudes/Cerrar
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Cerrar(CerrarViewModel modelo)
        {
            if (!ModelState.IsValid)
            {
                return VolverConErroresDeEntrada(modelo.Id);
            }

            return Ejecutar(
                modelo.Id,
                () => _solicitudService.Cerrar(modelo.Id, modelo.Comentario),
                "La solicitud fue cerrada correctamente.");
        }

        // Apoyo

        /// Ejecuta una operación del servicio y deja el resultado (éxito o error de negocio)
        /// como mensaje; siempre regresa a la pantalla de detalle.
        private ActionResult Ejecutar(int id, Action operacion, string mensajeExito)
        {
            try
            {
                operacion();
                MensajeExito(mensajeExito);
            }
            catch (AplicacionException ex)
            {
                MensajeError(ex.Message);
            }

            return RedirectToAction("Detalle", new { id = id });
        }

        private ActionResult VolverConErroresDeEntrada(int id)
        {
            var errores = ModelState.Values
                .SelectMany(v => v.Errors)
                .Select(e => e.ErrorMessage)
                .Where(m => !string.IsNullOrWhiteSpace(m))
                .Distinct();

            MensajeError(string.Join(" ", errores));
            return RedirectToAction("Detalle", new { id = id });
        }

        private void CargarListas(SolicitudCrearViewModel modelo)
        {
            modelo.Usuarios = new SelectList(_usuarioService.ObtenerTodos(true), "Id", "Nombre", modelo.SolicitanteId);
            modelo.Tipos = new SelectList(_tipoService.ObtenerTodos(true), "Id", "Nombre", modelo.TipoSolicitudId);
        }
    }
}