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
  [RoutePrefix("code-schools")]
  public class CodeSchoolsController : ApiController
  {
    // GET tasks
    [Route("")]
    public async Task<IHttpActionResult> Get([FromUri]int from_row = 0, [FromUri] int page_size = 10, [FromUri] int f = 0)
    {
      try
      {
        ApiRepository ar = new ApiRepository();
        int total_count = 0;
        if (from_row < 0)
        {
          from_row = 0;
        }

        if (from_row >= 0)
        {
          total_count = await ar.SelectRememberCodeSchoolTotalCountAsync();
        }

        if (page_size < 0)
        {
          page_size = 10;
        } else if(page_size >= 100000)
        {
          page_size = 100000;
        }

        if (f == 1)
        {
          from_row = 0;
          page_size = 99999999;
        }
        

        var list = await ar.SelectRememberCodeSchoolListAsync(0, from_row, page_size);

        return Json(new 
        {
          result = 1,
          message = "[Request school code OK]" + (list.Count == 0 ? " But No Result." : ""),
          from_row = from_row,
          count = list.Count,
          total_count = total_count,
          data = list
        });
      } 
      catch(Exception e)
      {
        return Json(new
        {
          result = -1,
          message = "[Request school code FAIL]" + e.Message
          
        });
      }
      
      
    }

    // GET tasks/5
    [Route("{id:int}")]
    public async Task<IHttpActionResult> Get(int id)
    {
      try
      {
        if (id == -1)
        {
          throw new Exception(" Please use this ( /code-schools?f=1 ) instead of -1 ");
        }
        if (id == 0)
        {
          throw new Exception("Not allowed 0 code");
        }

        ApiRepository ar = new ApiRepository();
        var list = await ar.SelectRememberCodeSchoolListAsync(id);

        return Json(new
        {
          result = 1,
          message = "[Request school code(" + id.ToString() + ") OK]" + (list.Count == 0 ? "But No Result." : ""),
          count = list.Count,
          data = list
        });
      } 
      catch (Exception e)
      {
        return Json(new
        {
          result = -1,
          message = "[Request school code(" + id.ToString() + ") FAIL]" + e.Message
        });
      }
    }


  }
}
