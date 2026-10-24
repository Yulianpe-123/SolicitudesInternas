using System.Web.Mvc;
using System.Web.Routing;
using SolicitudesInternas.Infrastructure;

namespace SolicitudesInternas
{
    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            ControllerBuilder.Current.SetControllerFactory(new FabricaControladores());
        }
    }
}
