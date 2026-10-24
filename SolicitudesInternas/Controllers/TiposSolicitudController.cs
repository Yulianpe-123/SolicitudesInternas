using System.Web.Mvc;
using SolicitudesInternas.Excepciones;
using SolicitudesInternas.Interfaces;
using SolicitudesInternas.Models;

namespace SolicitudesInternas.Controllers
{
    public class TiposSolicitudController : BaseController
    {
        private readonly ITipoSolicitudService _tipoService;

        public TiposSolicitudController(ITipoSolicitudService tipoService)
        {
            _tipoService = tipoService;
        }

        public ActionResult Index()
        {
            return View(_tipoService.ObtenerTodos(false));
        }

        public ActionResult Crear()
        {
            return View(new TipoSolicitud());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Crear([Bind(Include = "Nombre,Descripcion,Activo")] TipoSolicitud tipo)
        {
            if (!ModelState.IsValid)
            {
                return View(tipo);
            }

            try
            {
                _tipoService.Registrar(tipo);
                MensajeExito("La categoría '" + tipo.Nombre + "' fue registrada correctamente.");
                return RedirectToAction("Index");
            }
            catch (AplicacionException ex)
            {
                ModelState.AddModelError(string.Empty, ex.Message);
                return View(tipo);
            }
        }

        public ActionResult Editar(int id)
        {
            return View(_tipoService.ObtenerPorId(id));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Editar([Bind(Include = "Id,Nombre,Descripcion,Activo")] TipoSolicitud tipo)
        {
            if (!ModelState.IsValid)
            {
                return View(tipo);
            }

            try
            {
                _tipoService.Actualizar(tipo);
                MensajeExito("La categoría se actualizó correctamente.");
                return RedirectToAction("Index");
            }
            catch (AplicacionException ex)
            {
                ModelState.AddModelError(string.Empty, ex.Message);
                return View(tipo);
            }
        }
    }
}
