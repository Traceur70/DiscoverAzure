using Eut.Interface.WebAPI01.Models;
using Eut.Service.MSGraph;
using Eut.Service.MSSharePoint;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Cors;

namespace Eut.Interface.WebAPI01.Controllers
{
    //[Authorize]
    [EnableCors(origins: "*", headers: "*", methods: "*")]
    public class TodoListController : BaseController
    {

        // GET: api/TodoList
        public async Task<string> Get()
        {
            using (var ctx = new EutSharePointContext("https://techunit1.sharepoint.com", UserToken))
            {
                //return ctx.GetWebTitle();
                //return await ctx.Get("/_api/Web/Lists/GetByTitle('Documents')/Items?$select=Title");
                try
                {
                    return await ctx.HttpAddItem("/_api/web/getlist('/lists/test')", new Dictionary<string, object> { { "Title", "test 05 ok !!" } });
                }
                catch(Exception e)
                {
                    throw;
                }
        }
    }

        //// GET: api/TodoList/5
        //public Todo Get(int id)
        //{
        //    string owner = ClaimsPrincipal.Current.FindFirst(ClaimTypes.NameIdentifier).Value;
        //}

        // POST: api/TodoList
        public void Post(Todo todo)
        {
            string owner = ClaimsPrincipal.Current.FindFirst(ClaimTypes.NameIdentifier).Value;
        }

        // PUT: api/TodoList
        public void Put(Todo todo)
        {
            string owner = ClaimsPrincipal.Current.FindFirst(ClaimTypes.NameIdentifier).Value;
        }

        //// DELETE: api/TodoList/5
        //public void Delete(int id)
        //{
        //    string owner = ClaimsPrincipal.Current.FindFirst(ClaimTypes.NameIdentifier).Value;
        //}
    }
}
