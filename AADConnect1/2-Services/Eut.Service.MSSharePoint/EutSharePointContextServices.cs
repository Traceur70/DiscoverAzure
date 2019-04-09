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

        public async Task<string> GetWebTitle()
        {
            _spContext.Load(_spContext.Web);
            await _spContext.ExecuteQueryAsync();
            return _spContext.Web.Title;
        }

        public async Task<string> Get(string endpoint) => await HttpRequest(endpoint, HttpMethod.Get);
        
        public async Task<string> HttpAddItem(string listEndpoint, IDictionary<string,object> itemProps)
        {
            try
            {
                HttpClient client = new HttpClient();
                HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Post, string.Concat(_webUrl, listEndpoint + "/Items"));
                request.Headers.Add("Accept", "application/json;odata=verbose");
                request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", _accessToken);
                var formDigest = await GetFormDigest();
                request.Headers.Add("X-RequestDigest", formDigest);
                var spDataTypeReq = HttpRequest(listEndpoint + "?$select=ListItemEntityTypeFullName", HttpMethod.Get);
                var spDataType = JObject.Parse(await spDataTypeReq).SelectToken("d.ListItemEntityTypeFullName").ToString();
                itemProps.Add("__metadata", new { type = spDataType });
                var stringContent = JsonConvert.SerializeObject(itemProps);
                var requestContent = new StringContent(stringContent);
                requestContent.Headers.ContentType = MediaTypeHeaderValue.Parse("application/json;odata=verbose");
                request.Content = requestContent;
                HttpResponseMessage response = await client.SendAsync(request);
                return response.IsSuccessStatusCode
                    ? await response.Content.ReadAsStringAsync()
                    : (response.StatusCode + " " + (await response.Content.ReadAsStringAsync()));
            }
            catch (Exception e) { throw new EutSharePointException("Error while adding item", e); }
        }



    }
}
