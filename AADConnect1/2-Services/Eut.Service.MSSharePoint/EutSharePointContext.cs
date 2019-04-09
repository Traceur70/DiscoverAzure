using System;
using System.Threading.Tasks;
using System.Net.Http;
using System.Net.Http.Headers;
using Microsoft.IdentityModel.Clients.ActiveDirectory;
using System.Xml.Linq;
using Eut.Logic.Utilities;
using Microsoft.SharePoint.Client;
using AppForSharePointOnlineWebToolkit;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Collections;
using System.Collections.Generic;
using Eut.Entity.Common;

namespace Eut.Service.MSSharePoint
{
    public partial class EutSharePointContext: IEutServiceContext
    {
        private readonly string _o365Token;
        private readonly string _webUrl;
        private readonly string _accessToken;
        private readonly ClientContext _spContext;
        private string _formDigest;

        public EutSharePointContext(string webUrl, string o365Token): this(webUrl, o365Token, EutConfiguration.Current) { }
        public EutSharePointContext(string webUrl, string o365Token, IEutConfiguration config)
        {
            try
            {
                _webUrl = webUrl;
                _o365Token = o365Token;
                AuthenticationContext authContext = new AuthenticationContext(config.TenantAuthorityWindows);
                string userAccessToken = o365Token.Substring(o365Token.LastIndexOf(' ')).Trim();
                UserAssertion userAssertion = new UserAssertion(userAccessToken);
                var authenticationResult = authContext.AcquireTokenAsync("https://"+ config.TenantName+".sharepoint.com", config.ClientCredential, userAssertion).Result;
                _accessToken = authenticationResult.AccessToken;
                Console.WriteLine("Token Type: {0} \nExpires: {1} \nToken: {2}", authenticationResult.AccessTokenType, authenticationResult.ExpiresOn, _accessToken);
                _spContext = TokenHelper.GetClientContextWithAccessToken(webUrl, _accessToken);
            }
            catch(Exception e) { throw new EutSharePointException("Error while instanciating context", e); }
        }


        private async Task<string> GetFormDigest()
        {
            if (string.IsNullOrEmpty(_formDigest))
            {
                var reqWebInfo = await HttpRequest("/_api/contextinfo?$select=GetContextWebInformation", HttpMethod.Post);
                _formDigest = JObject.Parse(reqWebInfo).SelectToken("d.GetContextWebInformation.FormDigestValue").ToString();
            }
            return _formDigest;
        }

        public void Dispose() => _spContext.Dispose();
        
        private async Task<string> HttpRequest(string endpoint, HttpMethod method)
        {
            try
            {
                HttpClient client = new HttpClient();
                HttpRequestMessage request = new HttpRequestMessage(method, string.Concat(_webUrl, endpoint));
                request.Headers.Add("Accept", "application/json;odata=verbose");
                request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", _accessToken);
                HttpResponseMessage response = await client.SendAsync(request);
                return response.IsSuccessStatusCode
                    ? await response.Content.ReadAsStringAsync()
                    : (response.StatusCode + " " + (await response.Content.ReadAsStringAsync()));
            }
            catch (Exception e) { throw new EutSharePointException("Error while executing HTTP request", e); }
        }

    }
}
