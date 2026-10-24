using System.Web.Mvc;
using SolicitudesInternas.Excepciones;
using SolicitudesInternas.Interfaces;
using SolicitudesInternas.Models;

namespace SolicitudesInternas.Controllers
{
    public class ResponsablesController : BaseController
    {
        private readonly IResponsableService _responsableService;

        public ResponsablesController(IResponsableService responsableService)
        {
            _responsableService = responsableService;
        }

        public ActionResult Index()
        {
            return View(_responsableService.ObtenerTodos(false));
        }

        public ActionResult Crear()
        {
            return View(new Responsable());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Crear([Bind(Include = "Nombre,Correo,Especialidad,Activo")] Responsable responsable)
        {
            if (!ModelState.IsValid)
            {
                return View(responsable);
            }

            try
            {
                _responsableService.Registrar(responsable);
                MensajeExito("El responsable " + responsable.Nombre + " fue registrado correctamente.");
                return RedirectToAction("Index");
            }
            catch (AplicacionException ex)
            {
                ModelState.AddModelError(string.Empty, ex.Message);
                return View(responsable);
            }
        }

        public ActionResult Editar(int id)
        {
            return View(_responsableService.ObtenerPorId(id));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Editar([Bind(Include = "Id,Nombre,Correo,Especialidad,Activo")] Responsable responsable)
        {
            if (!ModelState.IsValid)
            {
                return View(responsable);
            }

            try
            {
                _responsableService.Actualizar(responsable);
                MensajeExito("Los datos del responsable se actualizaron correctamente.");
                return RedirectToAction("Index");
            }
            catch (AplicacionException ex)
            {
                ModelState.AddModelError(string.Empty, ex.Message);
                return View(responsable);
            }
        }
    }
}
