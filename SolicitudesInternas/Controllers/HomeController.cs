using System.Web.Mvc;
using SolicitudesInternas.Interfaces;

namespace SolicitudesInternas.Controllers
{
    public class HomeController : BaseController
    {
        private readonly ISolicitudService _solicitudService;

        public HomeController(ISolicitudService solicitudService)
        {
            _solicitudService = solicitudService;
        }

        public ActionResult Index()
        {
            var resumen = _solicitudService.ObtenerResumen();
            return View(resumen);
        }
    }
}
