using System;
using System.Web.Mvc;
using System.Web.Routing;
using SolicitudesInternas.Controllers;
using SolicitudesInternas.Data;
using SolicitudesInternas.Repositories;
using SolicitudesInternas.Services;

namespace SolicitudesInternas.Infrastructure
{
    /// Raíz de composición: crea los controladores inyectando servicios y repositorios
    /// (Dependency Inversion). Cada petición usa un único DbContext, que se libera al finalizar.
    public class FabricaControladores : DefaultControllerFactory
    {
        protected override IController GetControllerInstance(RequestContext requestContext, Type controllerType)
        {
            if (controllerType == null)
            {
                return base.GetControllerInstance(requestContext, controllerType);
            }

            var contexto = new SolicitudesContext();

            var usuarioRepositorio = new UsuarioRepository(contexto);
            var responsableRepositorio = new ResponsableRepository(contexto);
            var tipoRepositorio = new TipoSolicitudRepository(contexto);
            var solicitudRepositorio = new SolicitudRepository(contexto);

            var usuarioService = new UsuarioService(usuarioRepositorio);
            var responsableService = new ResponsableService(responsableRepositorio, solicitudRepositorio);
            var tipoService = new TipoSolicitudService(tipoRepositorio);
            var solicitudService = new SolicitudService(
                solicitudRepositorio, usuarioRepositorio, tipoRepositorio, responsableRepositorio);

            BaseController controlador;

            if (controllerType == typeof(HomeController))
            {
                controlador = new HomeController(solicitudService);
            }
            else if (controllerType == typeof(SolicitudesController))
            {
                controlador = new SolicitudesController(solicitudService, usuarioService, responsableService, tipoService);
            }
            else if (controllerType == typeof(UsuariosController))
            {
                controlador = new UsuariosController(usuarioService);
            }
            else if (controllerType == typeof(ResponsablesController))
            {
                controlador = new ResponsablesController(responsableService);
            }
            else if (controllerType == typeof(TiposSolicitudController))
            {
                controlador = new TiposSolicitudController(tipoService);
            }
            else
            {
                contexto.Dispose();
                return base.GetControllerInstance(requestContext, controllerType);
            }

            controlador.RegistrarRecurso(contexto);
            return controlador;
        }
    }
}
