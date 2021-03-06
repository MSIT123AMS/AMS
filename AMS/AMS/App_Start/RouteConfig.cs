﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace AMS
{
    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");


            routes.MapRoute(
                name: "Default",
                url: "{controller}/{action}/{id}",
                defaults: new { controller = "Account", action = "Login", id = UrlParameter.Optional }

            );


            routes.MapRoute(
              name: "Default1",
              url: "{controller}/{action}/{id}/{id2}",
              defaults: new { controller = "Employees", action = "Listemp", id = UrlParameter.Optional, id2 = UrlParameter.Optional }
          );

            routes.MapRoute(
              name: "Default2",
              url: "{controller}/{action}/{type}/{status}/{month}",
              defaults: new { controller = "Employees", action = "Listemp", type = UrlParameter.Optional, status = UrlParameter.Optional, month =DateTime.Now.Month}
            );



        }
    }
}
