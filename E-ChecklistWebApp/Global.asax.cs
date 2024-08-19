using E_ChecklistWebApp.AuthApi;
using E_ChecklistWebApp.Controllers;
using E_ChecklistWebApp.MachineModeApi;
using MySqlUserEngineServices.DataAccess;
using MySqlUserEngineServices.MySqlUserService;
using System;
using System.Configuration;
using System.Net.Http;
using System.Web;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using Unity;
using Unity.AspNet.Mvc;
using Unity.Injection;

namespace E_ChecklistWebApp
{
    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            RegisterComponents();
            AreaRegistration.RegisterAllAreas();
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);

        }
        protected void Application_Error(object sender, EventArgs e)
        {
            var exception = Server.GetLastError();
            Response.Clear();

            var httpException = exception as HttpException;
            RouteData routeData = new RouteData();
            routeData.Values.Add("controller", "Error");

            if (httpException != null)
            {
                switch (httpException.GetHttpCode())
                {
                    case 404:
                        routeData.Values.Add("action", "NotFound");
                        break;
                    case 500:
                        routeData.Values.Add("action", "ServerError");
                        break;
                    default:
                        routeData.Values.Add("action", "General");
                        break;
                }
            }
            else
            {
                routeData.Values.Add("action", "General");
            }

            Server.ClearError();
            Response.TrySkipIisCustomErrors = true;
            IController errorController = new ErrorController();
            errorController.Execute(new RequestContext(new HttpContextWrapper(Context), routeData));
        }

        protected void Application_BeginRequest(Object sender, EventArgs e)
        {
            Response.ContentEncoding = System.Text.Encoding.UTF8;
        }


        //private void RegisterComponents()
        //{
        //    var container = new UnityContainer();
        //    container.RegisterType<IAuthAPI, AuthAPI>();

        //    //string connectionString = ConfigurationManager.ConnectionStrings["Db"].ConnectionString;
        //    string plant = HttpContext.Current.Session["Plant"] as string;
        //    container.RegisterType<IDataAccess, DataAccess>(
        //    new InjectionConstructor(plant));
        //    container.RegisterType<IMySqlUserService, MySqlUserService>();
        //    DependencyResolver.SetResolver(new UnityDependencyResolver(container));


        //}

        private void RegisterComponents()
        {
            var container = new UnityContainer();
            container.RegisterType<IAuthAPI, AuthAPI>();
            container.RegisterFactory<IMachineModelAPI>(c =>
            {
                HttpClient httpClient = new HttpClient();
                string plant = HttpContext.Current.Session["Plant"] as string;
                return new MachineModelAPI(httpClient,plant);
            });
            container.RegisterFactory<IDataAccess>(c =>
            {
                string plant = HttpContext.Current.Session["Plant"] as string;
                    return new DataAccess(plant);
            });
            container.RegisterType<IMySqlUserService, MySqlUserService>();
            DependencyResolver.SetResolver(new UnityDependencyResolver(container));
        }
    }
}
