using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using Univision.Core.Repositories;
using Univision.Core.Models.DTO.Remember;
using System.Threading.Tasks;

namespace Univision.Remember.RestfulServer.Controllers
{
  [RoutePrefix("code-businesses")]
  public class CodeBusinessesController : ApiController
  {
    // GET tasks
    [Route("")]
    public async Task<IHttpActionResult> Get()
    {
      try
      {
        ApiRepository ar = new ApiRepository();
        var list = await ar.SelectRememberCodeBusinessListAsync();

        return Json(new 
        {
          result = 1,
          message = "[Request business code OK]" + (list.Count == 0 ? " But No Result." : ""),
          count = list.Count,
          data = list
        });
      } 
      catch(Exception e)
      {
        return Json(new
        {
          result = -1,
          message = "[Request business code FAIL]" + e.Message
          
        });
      }
      
      
    }

    // GET tasks/5
    [Route("{id:int}")]
    public async Task<IHttpActionResult> Get(int id)
    {
      try
      {
        if (id == 0)
        {
          throw new Exception("Not allowed 0 code");
        }

        ApiRepository ar = new ApiRepository();
        var list = await ar.SelectRememberCodeBusinessListAsync(id);

        return Json(new
        {
          result = 1,
          message = "[Request business code(" + id.ToString() + ") OK]" + (list.Count == 0 ? " But No Result." : ""),
          count = list.Count,
          data = list
        });
      } 
      catch (Exception e)
      {
        return Json(new
        {
          result = -1,
          message = "[Request business code(" + id.ToString() + ") FAIL] " + e.Message
        });
      }
    }

    // GET tasks/5
    [Route("{id:int}/{id2:int}")]
    public async Task<IHttpActionResult> Get(int id, int id2)
    {
      try
      {
        if (id == 0)
        {
          throw new Exception("Not allowed 0 code");
        }
        if (id2 == 0)
        {
          throw new Exception("Not allowed 0 code");
        }

        ApiRepository ar = new ApiRepository();
        var list = await ar.SelectRememberCodeBusinessListAsync(id, id2);

        return Json(new
        {
          result = 1,
          message = "[Request business code(" + id.ToString() + "-" + id2.ToString() + ") OK]" + (list.Count == 0 ? " But No Result." : ""),
          count = list.Count,
          data = list
        });
      }
      catch (Exception e)
      {
        return Json(new
        {
          result = -1,
          message = "[Request business code(" + id.ToString() + "-" + id2.ToString() + ") FAIL] " + e.Message
        });
      }
    }

  }
}
