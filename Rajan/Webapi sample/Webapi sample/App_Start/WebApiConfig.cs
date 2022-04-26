using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;

namespace Webapi_sample
{
    public static class WebApiConfig
    {
        public static void Register(HttpConfiguration config)
        {
            // Web API configuration and services

            // Web API routes
            config.MapHttpAttributeRoutes();

            config.Routes.MapHttpRoute(
                name: "DefaultApi",
                routeTemplate: "api/{controller}/{id}/{FirstName}/{LastName}/{Gender}/{Salary}/{ColumnName}",
                defaults: new { id = RouteParameter.Optional,FirstName = RouteParameter.Optional, 
                    LastName = RouteParameter.Optional, 
                    Gender = RouteParameter.Optional, 
                    Salary =RouteParameter.Optional,
                    ColumnName = RouteParameter.Optional,
                    ValueForColumn = RouteParameter.Optional
                }
            );
        }
    }
}
