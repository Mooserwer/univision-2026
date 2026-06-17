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
  [RoutePrefix("project-activities")]
  public class ProjectActivitiesController : ApiController
  {
    // GET tasks
    [Route("list")]
    public async Task<IHttpActionResult> Get([FromUri] string update_at="", [FromUri]int from_row = 0, [FromUri] int page_size = 1000)
    {
      try
      {
        if (String.IsNullOrEmpty(update_at))
        {
          throw new Exception("update_at 값이 필요합니다. 형식 : yyyy-MM-dd'T'HH:mm:ss.SSS");
        }

        if (page_size > 1000)
        {
          page_size = 1000;
        }

        if(from_row < 0)
        {
          from_row = 0;
        }

        RememberTaskRepository rtr = new RememberTaskRepository();
        SearchEngineRepository sr = new SearchEngineRepository();

        var list = await rtr.SelectRememberPjtActivityListAsync<r_project_candidate>(update_at, 0, 0, from_row, page_size);

       
        return Json(new 
        {
          result = 1,
          message = "[Request project-activity list OK]" + (list.item.Count == 0 ? " But No Result." : ""),
          from_row = from_row,
          count = list.item.Count,
          total_count = list.total_cnt,
          data = list.item,
        });
      } 
      catch(Exception e)
      {
        return Json(new
        {
          result = -1,
          message = "[Request project-activity list FAIL]" + e.Message
          
        });
      }
      
      
    }

    // GET tasks
    [Route("bulk")]
    public async Task<IHttpActionResult> Get()
    {
      try
      {
        RememberTaskRepository rtr = new RememberTaskRepository();
        SearchEngineRepository sr = new SearchEngineRepository();

        var list = await rtr.SelectRememberPjtActivityListAsync<r_project_candidate>("", 0, 0, 0, 999999);


        return Json(new
        {
          result = 1,
          message = "[Request project-activity bulk OK]" + (list.item.Count == 0 ? " But No Result." : ""),
          from_row = 0,
          count = list.item.Count,
          total_count = list.total_cnt,
          data = list.item,
        });
      }
      catch (Exception e)
      {
        return Json(new
        {
          result = -1,
          message = "[Request project-activity bulk FAIL]" + e.Message

        });
      }


    }

    // GET tasks/5
    [Route("{id:int}")]
    public async Task<IHttpActionResult> Get(int id)
    {
      try
      {
        RememberTaskRepository rtr = new RememberTaskRepository();
        SearchEngineRepository sr = new SearchEngineRepository();

        var list = await rtr.SelectRememberPjtActivityListAsync<r_project_candidate>("", id);

       
        return Json(new
        {
          result = 1,
          message = "[Request project-activity code(" + id.ToString() + ") OK]" + (list.item.Count == 0 ? " But No Result." : ""),
          count = list.item.Count,
          data = list.item,
        });
      }
      catch (Exception e)
      {
        return Json(new
        {
          result = -1,
          message = "[Request project-activity code(" + id.ToString() + ") FAIL]" + e.Message

        });
      }
    }

  }
}
