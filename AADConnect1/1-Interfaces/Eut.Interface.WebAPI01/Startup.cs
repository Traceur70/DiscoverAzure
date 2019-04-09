using Eut.Entity.Common;
using Eut.Logic.Utilities;
using Microsoft.IdentityModel.Tokens;
using Microsoft.Owin;
using Microsoft.Owin.Security.ActiveDirectory;
using Owin;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Web.Http.Cors;//TODO remove using and nuget package

[assembly: OwinStartup(typeof(Eut.Interface.WebAPI01.Startup))]

namespace Eut.Interface.WebAPI01
{
    public class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            app.UseWindowsAzureActiveDirectoryBearerAuthentication(
                new WindowsAzureActiveDirectoryBearerAuthenticationOptions
                {
                    Tenant = EutConfiguration.Current.TenantOnMicrosoft,
                    TokenValidationParameters = new TokenValidationParameters//System.IdentityModel.Tokens.TokenValidationParameters
                    {
                        ValidAudience = EutConfiguration.Current.AppClientId,
                    },
                });
        }        
    }
    public static class WebApiConfig
    {
        public static void Register(HttpConfiguration config)
        {
            var corsAttr = new EnableCorsAttribute("*", "*", "*");//TODO specify attributes
            config.EnableCors(corsAttr);
            config.MapHttpAttributeRoutes();
            config.Routes.MapHttpRoute("DefaultApi", "api/{controller}/{id}", new { id = RouteParameter.Optional });
        }
    }
}