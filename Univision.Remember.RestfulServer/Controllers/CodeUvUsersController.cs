using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using Univision.Core.Repositories;
using Univision.Core.Models.DTO;
using System.Threading.Tasks;

namespace Univision.Remember.RestfulServer.Controllers
{
  [RoutePrefix("code-uv-users")]
  public class CodeUvUsersController : ApiController
  {
    // GET tasks
    [Route("")]
    public async Task<IHttpActionResult> Get()
    {
      try
      {
        AccountRepository ar = new AccountRepository();
        var list = await ar.SelectRememberUserListAsync();

        return Json(new 
        {
          result = 1,
          message = "[Request user list OK]" + (list.Count == 0 ? "But No Result." : ""),
          count = list.Count,
          data = list
        });
      } 
      catch(Exception e)
      {
        return Json(new
        {
          result = -1,
          message = "[Request user list FAIL]" + e.Message
          
        });
      }
      
      
    }

    // GET tasks/5
    [Route("{id:int}")]
    public async Task<IHttpActionResult> Get(int id)
    {
      try
      {
        AccountRepository ar = new AccountRepository();
        var list = await ar.SelectRememberUserListAsync(id);

        return Json(new
        {
          result = 1,
          message = "[Request user list Seq(" + id.ToString() + ") OK]" + (list.Count == 0 ? "But No Result." : ""),
          count = list.Count,
          data = list
        });
      } 
      catch (Exception e)
      {
        return Json(new
        {
          result = -1,
          message = "[Request user list Seq(" + id.ToString() + ") FAIL]" + e.Message
        });
      }
    }

  }
}
