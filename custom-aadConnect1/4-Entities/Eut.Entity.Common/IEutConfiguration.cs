using Microsoft.IdentityModel.Clients.ActiveDirectory;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Eut.Entity.Common
{
    public interface IEutConfiguration
    {
        string TenantName { get; set; }
        string TenantOnMicrosoft { get; set; }
        string AppClientId { get; set; }
        string TenantAuthorityWindows { get; set; }
        string TenantAuthorityMSOnline { get; set; } 
        ClientCredential ClientCredential { get; set; }
        ISecureClientSecret AppClientSecret { get; set; }
    }
}
