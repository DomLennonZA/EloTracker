using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;

namespace WebRole1
{
    public static class WebApiConfig
    {
        public static void Register(HttpConfiguration config)
        {
            // Web API configuration and services

            // Web API routes
            config.MapHttpAttributeRoutes();

            config.Routes.MapHttpRoute(
                name: "PlayerApi",
                routeTemplate: "api/{controller}/{action}/{id}/{count}",
                defaults: new { id = RouteParameter.Optional, count = RouteParameter.Optional }
            );

            config.Routes.MapHttpRoute(
               name: "CreateMatchApi",
               routeTemplate: "api/{controller}/{action}/{p1ID}/{p2ID}"
           );

            config.Routes.MapHttpRoute(
               name: "ResultsApi",
               routeTemplate: "api/{controller}/{action}/{gameID}/{playerID}/{result}"
           );
        }
    }
}
