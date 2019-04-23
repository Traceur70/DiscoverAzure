using Eut.Entity.Common;
using Microsoft.IdentityModel.Clients.ActiveDirectory;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Configuration;
using System.Linq;
using System.Security;
using System.Text;
using System.Threading.Tasks;

namespace Eut.Logic.Utilities
{
    public class EutConfiguration: IEutConfiguration
    {
        //TODO dont user client secret ? or use ISecureClientSecret ?
        public string TenantName { get; set; }
        public string TenantOnMicrosoft { get; set; }
        public string AppClientId { get; set; }
        public string TenantAuthorityWindows { get; set; }
        public string TenantAuthorityMSOnline { get; set; }
        public ClientCredential ClientCredential { get; set; }
        public ISecureClientSecret AppClientSecret { get; set; }

        private static EutConfiguration NewEutConfiguration()
        {
            var securedStrClientSecret = new SecureString();
            ConfigurationManager.AppSettings["ida:Secret"].ToList().ForEach(c => securedStrClientSecret.AppendChar(c));
            var securedClientSecret = new SecureClientSecret(securedStrClientSecret);
            var appClientId = ConfigurationManager.AppSettings["ida:Audience"];
            return new EutConfiguration
            {
                ClientCredential = new ClientCredential(appClientId, securedClientSecret),
                AppClientId = ConfigurationManager.AppSettings["ida:Audience"],
                TenantName = ConfigurationManager.AppSettings["ida:Tenant"],
                TenantOnMicrosoft = string.Concat(ConfigurationManager.AppSettings["ida:Tenant"], ".onmicrosoft.com"),
                TenantAuthorityWindows = string.Concat("https://login.windows.net/", ConfigurationManager.AppSettings["ida:Tenant"], ".onmicrosoft.com"),
                TenantAuthorityMSOnline = string.Concat("https://login.microsoftonline.com/", ConfigurationManager.AppSettings["ida:Tenant"], ".onmicrosoft.com"),
                AppClientSecret = securedClientSecret
            };
        }

        //private static EutConfiguration _current;
        public static readonly EutConfiguration Current = NewEutConfiguration();
    }
}
