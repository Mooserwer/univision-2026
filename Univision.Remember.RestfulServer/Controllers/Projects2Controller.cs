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
  [RoutePrefix("projects/v2")]
  public class Projects2Controller : ApiController
  {
    // GET tasks
    [Route("list")]
    public async Task<IHttpActionResult> Get([FromUri] string update_at="", [FromUri]int from_row = 0, [FromUri] int page_size = 100)
    {
      try
      { 
        if(page_size > 1000)
        {
          page_size = 1000;
        }

        if(from_row < 0)
        {
          from_row = 0;
        }

        RememberTaskRepository rtr = new RememberTaskRepository();
        SearchEngineRepository sr = new SearchEngineRepository();

        var list = await rtr.SelectRememberProjectListAsync_v2<r_project_full_v2>(update_at, 0, from_row, page_size);

        foreach(var data in list.item)
        {
          data.am = await rtr.SelectRememberPjtUserListAsync(data.p_seq);
          data.sm = await rtr.SelectRememberPjtUserListAsync(data.p_seq, false);
          data.placeList = await rtr.SelectRememberPjtPlaceListAsync(data.p_seq);

          data.candidates = await rtr.SelectRemmemberPjtRecandidateListAsync(data.p_seq);

          if (data.candidates.Count > 0)
          {
            foreach (var candidate in data.candidates)
            {
              candidate.history = await rtr.SelectRemmemberPjtRecandidateHisListAsync(data.p_seq, candidate.pic_seq);
            }
          }

        }

        return Json(new 
        {
          result = 1,
          message = "[Request project list(Ver.2) OK]" + (list.item.Count == 0 ? " But No Result." : ""),
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
          message = "[Request project list(Ver.2) FAIL]" + e.Message
          
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

        var list = await rtr.SelectRememberProjectListAsync_v2<r_project_full_v2>("", 0, 0, 99999);

        foreach (var data in list.item)
        {
          data.am = await rtr.SelectRememberPjtUserListAsync(data.p_seq);
          data.sm = await rtr.SelectRememberPjtUserListAsync(data.p_seq, false);
          data.placeList = await rtr.SelectRememberPjtPlaceListAsync(data.p_seq);

          data.candidates = await rtr.SelectRemmemberPjtRecandidateListAsync(data.p_seq);

          if (data.candidates.Count > 0)
          {
            foreach (var candidate in data.candidates)
            {
              candidate.history = await rtr.SelectRemmemberPjtRecandidateHisListAsync(data.p_seq, candidate.pic_seq);
            }
          }
        }

        return Json(new
        {
          result = 1,
          message = "[Request project bulk(Ver.2) OK]" + (list.item.Count == 0 ? " But No Result." : ""),
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
          message = "[Request project bulk(Ver.2) FAIL]" + e.Message

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

        var list = await rtr.SelectRememberProjectListAsync_v2<r_project_full_v2>("", id);

        foreach (var data in list.item)
        {
          data.am = await rtr.SelectRememberPjtUserListAsync(data.p_seq);
          data.sm = await rtr.SelectRememberPjtUserListAsync(data.p_seq, false);
          data.placeList = await rtr.SelectRememberPjtPlaceListAsync(data.p_seq);

          data.candidates = await rtr.SelectRemmemberPjtRecandidateListAsync(data.p_seq);

          if (data.candidates.Count > 0)
          {
            foreach (var candidate in data.candidates)
            {
              candidate.history = await rtr.SelectRemmemberPjtRecandidateHisListAsync(data.p_seq, candidate.pic_seq);
            }
          }
        }

        return Json(new
        {
          result = 1,
          message = "[Request project code(" + id.ToString() + ") (Ver.2) OK]" + (list.item.Count == 0 ? " But No Result." : ""),
          count = list.item.Count,
          data = list.item,
        });
      }
      catch (Exception e)
      {
        return Json(new
        {
          result = -1,
          message = "[Request project code(" + id.ToString() + ") (Ver.2) FAIL]" + e.Message

        });
      }
    }

  }
}
