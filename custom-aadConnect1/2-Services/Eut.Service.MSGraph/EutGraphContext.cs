/* 
*  Copyright (c) Microsoft. All rights reserved. Licensed under the MIT license. 
*  See LICENSE in the source repository root for complete license information. 
*/

using Eut.Entity.Common;
using Eut.Logic.Utilities;
using Microsoft.Graph;
using Newtonsoft.Json;
using System;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using ADAuthenticationContext = Microsoft.IdentityModel.Clients.ActiveDirectory.AuthenticationContext;

namespace Eut.Service.MSGraph
{
    public partial class EutGraphContext: IEutServiceContext
    {
        public void Dispose() { }
        GraphServiceClient _graphClient;

        //TODO dont user client secret ? or use ISecureClientSecret ?
        public EutGraphContext()
        {
            _graphClient = new GraphServiceClient(new AdalNaiveAuthenticationProvider(EutConfiguration.Current.TenantAuthorityMSOnline));
        }

        //TODO check exception handling https://github.com/AzureAD/azure-activedirectory-library-for-dotnet/wiki/Exceptions-in-ADAL.NET
        private class AdalNaiveAuthenticationProvider : IAuthenticationProvider
        {
            private ADAuthenticationContext _authenticationContext;

            public AdalNaiveAuthenticationProvider(string tenantAuthorityMSOnline)
            {
                _authenticationContext = new ADAuthenticationContext(tenantAuthorityMSOnline);
            }

            public async Task AuthenticateRequestAsync(HttpRequestMessage request)
            {
                var result = await _authenticationContext.AcquireTokenAsync("https://graph.microsoft.com", EutConfiguration.Current.ClientCredential);
                request.Headers.Add("Authorization", result.CreateAuthorizationHeader());
            }
        }

        internal async Task RunWithBetaVersion(Func<Task> todo)
        {
            _graphClient.BaseUrl = "https://graph.microsoft.com/beta";
            await todo();
            _graphClient.BaseUrl = "https://graph.microsoft.com/v1.0";
        }
    }
}
