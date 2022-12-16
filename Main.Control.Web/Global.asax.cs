using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using Castle.Windsor;
using Castle.Windsor.Installer;
using Main.Control.Utilities;
using System.Net;

namespace Main.Control.Web
{
    // Note: For instructions on enabling IIS6 or IIS7 classic mode, 
    // visit http://go.microsoft.com/?LinkId=9394801

    public class MvcApplication : System.Web.HttpApplication
    {
        private static IWindsorContainer container;


        protected void Application_Start()
        {
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls12;
            AreaRegistration.RegisterAllAreas();
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);
            AuthConfig.RegisterAuth();
            ValidatesCertificate();
            BootstrapContainer();
        }
        #region To Avoid, Could not create SSL/TLS secure channel  exception in import service,and others web requests
        /// <summary>
        /// To Avoid, Could not create SSL/TLS secure channel exception in import service,and others web requests
        /// (System.Net.Sockets.SocketException: An existing connection was forcibly closed by the remote host)
        /// </summary>
        public static void ValidatesCertificate()
        {
            ServicePointManager.Expect100Continue = true;
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls11 | SecurityProtocolType.Tls12;
        }
        #endregion
        private void Application_Error()
        {
            Exception error = Server.GetLastError();

        }

        private static void BootstrapContainer()
        {
            container = new WindsorContainer().Install(
                new RepositoriesInstaller(),
                new ServiceInstaller(),
                FromAssembly.This()
                );
            var controllerFactory = new WindsorControllerFactory(container.Kernel);
            ControllerBuilder.Current.SetControllerFactory(controllerFactory);
        }
    }
}