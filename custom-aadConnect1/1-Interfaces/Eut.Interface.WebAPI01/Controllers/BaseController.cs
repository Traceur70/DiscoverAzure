using Eut.Entity.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Web;
using System.Web.Http;


namespace Eut.Interface.WebAPI01.Controllers
{
    public class BaseController : ApiController
    {


        private string _userLogin;
        private string _userNameIdentifier;
        private string _userToken;
        private string _requestOriginUrl;

        public string UserToken => (_userToken ?? (_userToken = Request.Headers.GetValues("Authorization").FirstOrDefault())); //TODO or HttpContext.Current.Request.Headers["Authorization"] ?
        public string RequestOriginUrl => (_requestOriginUrl ?? (_requestOriginUrl = Request.Headers.GetValues("Origin").FirstOrDefault()));
        public string UserLogin => (_userLogin ?? (_userLogin = ClaimsPrincipal.Current.Identity.Name));
        public string UserNameIdentifier => (_userNameIdentifier ?? (_userNameIdentifier = ClaimsPrincipal.Current.FindFirst(ClaimTypes.NameIdentifier)?.Value));

        public static string GetAccessToken()
        {
            var session = HttpContext.Current.Session;
            if (session != null)
            {
                var startTokenKey = "_O365#AccessToken#";
                var tokenKey = session.Keys.Cast<string>().SingleOrDefault(k => k.Contains(startTokenKey));

                if (tokenKey != null)
                {
                    var tokenObject = session[startTokenKey + "ServiceResourceId"];
                    //Hack to get value since CacheItem type isn't accessible
                    if (tokenObject != null)
                    {
                        var token = tokenObject.GetType().GetField("Value").GetValue(tokenObject).ToString();
                        return token;
                    }
                }
            }
            return null;
        }
    }
}