using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace ProjectFClean
{
    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            

            // Route cho action Details
            routes.MapRoute(
                name: "Details",
                url: "Home/Details/{HID}",
                defaults: new { controller = "Home", action = "Details", HID = UrlParameter.Optional }
            );
            // Route cho action Details
            routes.MapRoute(
                name: "DetailsRenter1",
                url: "Home/DetailsRenter/{RID}",
                defaults: new { controller = "Home", action = "DetailsRenter1", RID = UrlParameter.Optional }
            );

            // Route cho action Details PID
            routes.MapRoute(
                name: "DetailsPost",
                url: "Posts/Details/{pid}",
                defaults: new { controller = "Posts", action = "DetailsPost", pid = UrlParameter.Optional }
            );
            routes.MapRoute(
                name: "Default",
                url: "{controller}/{action}/{id}",
                defaults: new { controller = "Home", action = "Index", id = UrlParameter.Optional }
            );

        }
    }
}
