using Microsoft.SharePoint.Client;
using SharePoint.Client;
using System;
using System.Linq;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security;
using Microsoft.Online.SharePoint.TenantAdministration;

namespace TestSPODotNetCore
{
    class Program
    {
        static void Main(string[] args)
        {
            using(var ctx = new ClientContext("https://techunit1.sharepoint.com"))
            {
                var s = new SecureString();
                "teKi1iKhAhit".ToList().ForEach(ss => s.AppendChar(ss));
                // Tenant.
                ctx.Credentials = new SharePointOnlineCredentials("nadda@techunit1.onmicrosoft.com", s);
                ctx.Load(ctx.Web);
                ctx.ExecuteQueryAsync();
                var r = ctx.Web.CurrentChangeToken.StringValue;


                Test3("https://techunit1.sharepoint.com/", "https://techunit1.sharepoint.com/_api/Web", "nadda@techunit1.onmicrosoft.com", "teKi1iKhAhit");
                Test2("https://techunit1-admin.sharepoint.com/_api/lists('3b059a78-a1dd-492b-88b9-d8a4c9d5f84e')/items?$select=SiteUrl", "nadda@techunit1.onmicrosoft.com", "teKi1iKhAhit");
                //HttpClient client = new HttpClient();
                //HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Post, "https://techunit1-admin.sharepoint.com/_api/lists('3b059a78-a1dd-492b-88b9-d8a4c9d5f84e')/items?$select=SiteUrl");
                //request.Headers.Add("Accept", "application/json;odata=verbose");
                //request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", r+"123");
                //var response = client.SendAsync(request).Result;

                //using (var client = new WebClient())
                //{
                //    client.Headers.Add("X-FORMS_BASED_AUTH_ACCEPTED", "f");
                //    client.Credentials = new NetworkCredential("nadda@techunit1.onmicrosoft.com", "teKi1iKhAhit");
                //    client.Headers.Add(HttpRequestHeader.ContentType, "application/json;odata=verbose");
                //    client.Headers.Add(HttpRequestHeader.Accept, "application/json;odata=verbose");
                //    //var endpointUri = new Uri("https://techunit1-admin.sharepoint.com/_api/lists('3b059a78-a1dd-492b-88b9-d8a4c9d5f84e')/items?$select=SiteUrl");
                //    var endpointUri = new Uri("https://techunit1.sharepoint.com/_api/web");
                //    var result = client.DownloadString(endpointUri);
                //    Console.WriteLine(ctx.Web.Title);
                //}

            }
            Console.WriteLine("Hello World!");
        }
        private static void GetAllSiteCollections()
        {
            SPOSitePropertiesEnumerable prop = null;

            var password = new SecureString();
            foreach (char c in "MyPassword".ToCharArray()) password.AppendChar(c);
            var credentials = new SharePointOnlineCredentials("admin@MyTenant.onmicrosoft.com", password);
            var ctx = new ClientContext("https://MyTenant-admin.sharepoint.com/");
            ctx.Credentials = credentials;

            Tenant tenant = new Tenant(ctx);
            prop = tenant.GetSiteProperties(0, true);
            ctx.Load(prop);
            ctx.ExecuteQuery();
            foreach (SiteProperties sp in prop)
            {
                Console.WriteLine(sp.Title + " => " + sp.Url);
                Console.WriteLine("---------------------------");
            }
        }


        public string GetAllSitesColl(string adminUrl, string spToken)
        {
            HttpClient client = new HttpClient();
            HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Post, "/_api/lists/getbytitle('DO_NOT_DELETE_SPLIST_TENANTADMIN_AGGREGATED_SITECOLLECTIONS')/items?$select=SiteUrl");
            request.Headers.Add("Accept", "application/json;odata=verbose");
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", spToken);
            return client.SendAsync(request).Result.Content.ReadAsStringAsync().Result;
        }

        public static void Test3(string webUri, string endpointUrl, string userName, string password)
        {
            using (var client = new SPHttpClient(new Uri(webUri), userName, password))
            {
                var listTitle = "Tasks";
                var data = client.ExecuteJson(endpointUrl);
                foreach (var item in data["value"])
                {
                    Console.WriteLine(item["Title"]);
                }
            }
        }

        public static void Test2(string endpoint, string user, string pass)
        {
                HttpWebRequest endpointRequest = (HttpWebRequest)WebRequest.Create(endpoint);

                endpointRequest.Method = "GET";
                endpointRequest.Accept = "application/json;odata=verbose";
                //NetworkCredential cred = new System.Net.NetworkCredential(user.Split('@')[0], pass, user.Split('@')[1]);
                NetworkCredential cred = new System.Net.NetworkCredential(user, pass);
                endpointRequest.Credentials = cred;
                endpointRequest.Headers.Add("X-FORMS_BASED_AUTH_ACCEPTED", "f");
                HttpWebResponse endpointResponse = (HttpWebResponse)endpointRequest.GetResponse();
                try
                {
                    WebResponse webResponse = endpointRequest.GetResponse();

                Stream webStream = webResponse.GetResponseStream();
                    StreamReader responseReader = new StreamReader(webStream);
                    string response = responseReader.ReadToEnd();
                    //JObject jobj = JObject.Parse(response);
                    //JArray jarr = (JArray)jobj["d"]["results"];
                    //foreach (JObject j in jarr)
                    //{
                    //    Console.WriteLine(j["Title"] + " " + j["Body"]);
                    //}

                    responseReader.Close();
                    Console.ReadLine();


                }
                catch (Exception e)
                {
                    Console.Out.WriteLine(e.Message); Console.ReadLine();
            }
        }
    }
}
