using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Web.Http;
using Microsoft.Owin.Security.OAuth;
using Newtonsoft.Json.Serialization;
using System.Web.Http.OData.Builder;
using System.Web.Http.OData.Extensions;
using WEBAPIODATAV3.Models;

namespace WEBAPIODATAV3
{
    public static class WebApiConfig
    {
        public static void Register(HttpConfiguration config)
        {
            // Web API configuration and services
            // Configure Web API to use only bearer token authentication.
            config.SuppressDefaultHostAuthentication();
            config.Filters.Add(new HostAuthenticationFilter(OAuthDefaults.AuthenticationType));


            config.Formatters.Remove(config.Formatters.XmlFormatter);

            // Web API routes
            config.MapHttpAttributeRoutes();

            config.Routes.MapHttpRoute(
                name: "DefaultApi",
                routeTemplate: "api/{controller}/{action}/{id}",
                defaults: new { id = RouteParameter.Optional }
            );
           

          //  ODataConventionModelBuilder builder = new ODataConventionModelBuilder();
          //  builder.EntitySet<BankDoarIncome>("BankDoarIncomes");
                    
          //  builder.EntitySet<Provider>("Providers");
         
          //  builder.EntitySet<TenderSection>("TenderSections");
          //  builder.EntitySet<TenderTemplatesBookletSection>("TenderTemplateEditor");



          //  config.EnableCors();
          //  //  builder.EntitySet<TenderBookletSection>("File");
          //  builder.EntitySet<Filename>("TenderFilename");
          //  builder.EntitySet<TenderBookletSection>("TenderBookletSections");
          //  builder.EntitySet<TenderTemplatesBookletSection>("TenderTemplatesBookletSections");
          ////  builder.Entity<string>();
          ////  builder.EntitySet<string>("File");
          //  config.Routes.MapODataServiceRoute("odata", "odata", builder.GetEdmModel());
        }
    }
}
