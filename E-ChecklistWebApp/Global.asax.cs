using E_ChecklistWebApp.AuthApi;
using MySqlUserEngineServices.DataAccess;
using MySqlUserEngineServices.MySqlUserService;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.Management;
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

        private void RegisterComponents()
        {
            var container = new UnityContainer();
            string connectionString = ConfigurationManager.ConnectionStrings["Db"].ConnectionString;
            container.RegisterType<IAuthAPI, AuthAPI>();
            container.RegisterType<IDataAccess, DataAccess>(
            new InjectionConstructor(connectionString));
            container.RegisterType<IMySqlUserService, MySqlUserService>();
            DependencyResolver.SetResolver(new UnityDependencyResolver(container));
        }
    }
}
