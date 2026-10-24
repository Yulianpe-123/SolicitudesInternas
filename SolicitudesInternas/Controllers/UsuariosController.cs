using System.Web.Mvc;
using SolicitudesInternas.Excepciones;
using SolicitudesInternas.Interfaces;
using SolicitudesInternas.Models;

namespace SolicitudesInternas.Controllers
{
    public class UsuariosController : BaseController
    {
        private readonly IUsuarioService _usuarioService;

        public UsuariosController(IUsuarioService usuarioService)
        {
            _usuarioService = usuarioService;
        }

        public ActionResult Index()
        {
            return View(_usuarioService.ObtenerTodos(false));
        }

        public ActionResult Crear()
        {
            return View(new Usuario());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Crear([Bind(Include = "Nombre,Correo,Departamento,Activo")] Usuario usuario)
        {
            if (!ModelState.IsValid)
            {
                return View(usuario);
            }

            try
            {
                _usuarioService.Registrar(usuario);
                MensajeExito("El usuario " + usuario.Nombre + " fue registrado correctamente.");
                return RedirectToAction("Index");
            }
            catch (AplicacionException ex)
            {
                ModelState.AddModelError(string.Empty, ex.Message);
                return View(usuario);
            }
        }

        public ActionResult Editar(int id)
        {
            return View(_usuarioService.ObtenerPorId(id));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Editar([Bind(Include = "Id,Nombre,Correo,Departamento,Activo")] Usuario usuario)
        {
            if (!ModelState.IsValid)
            {
                return View(usuario);
            }

            try
            {
                _usuarioService.Actualizar(usuario);
                MensajeExito("Los datos del usuario se actualizaron correctamente.");
                return RedirectToAction("Index");
            }
            catch (AplicacionException ex)
            {
                ModelState.AddModelError(string.Empty, ex.Message);
                return View(usuario);
            }
        }
    }
}
