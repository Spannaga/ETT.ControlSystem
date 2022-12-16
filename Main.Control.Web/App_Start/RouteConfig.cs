using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace Main.Control.Web
{
    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {

            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");
            routes.MapMvcAttributeRoutes();

            routes.MapRoute(
                name: "Default",
                url: "{controller}/{action}/{id}",


                defaults: new { controller = "Admin", action = "SignIn", id = UrlParameter.Optional }


            );


            routes.MapRoute(
              "Second", // Route name
              "{controller}/{action}/{id}/{id2}", // URL with parameters
              new { controller = "Admin", action = "SignIn", id = UrlParameter.Optional, id2 = UrlParameter.Optional } // Parameter defaults
          );

            routes.MapRoute(
               "Third", // Route name
               "{controller}/{action}/{id}/{id2}/{id3}", // URL with parameters
               new { controller = "Admin", action = "SignIn", id = UrlParameter.Optional, id2 = UrlParameter.Optional, id3 = UrlParameter.Optional } // Parameter defaults
           );

            routes.MapRoute(
               "Fourth", // Route name
               "{controller}/{action}/{id}/{id2}/{id3}/{id4}", // URL with parameters
               new { controller = "Admin", action = "SignIn", id = UrlParameter.Optional, id2 = UrlParameter.Optional, id3 = UrlParameter.Optional, id4 = UrlParameter.Optional } // Parameter defaults
           );

        }
    }
}
