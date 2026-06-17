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
  [RoutePrefix("candidates")]
  public class CandidatesController : ApiController
  {
    // GET tasks
    [Route("list")]
    public async Task<IHttpActionResult> Get([FromUri]int from_row = 0, [FromUri] int page_size = 100)
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

        var list = await rtr.SelectRemeberCandidateListAsync<r_candidate_full>(0, from_row, page_size);

        foreach(var data in list.item)
        {
          data.can_school = await rtr.SelectRemeberCanSchoolListAsync(data.c_seq);
          data.can_career = await rtr.SelectRemeberCanCareerListAsync(data.c_seq);
          data.can_business = await rtr.SelectRemeberCanBusinessListAsync(data.c_seq);
          data.can_job = await rtr.SelectRemeberCanJobListAsync(data.c_seq);
          data.can_foreign_lan = await rtr.SelectRemeberCanLanguageListAsync(data.c_seq);
          data.can_activity = await rtr.SelectRemeberCanActivityListAsync(data.c_seq);
          data.can_resume_list = await rtr.SelectRemeberCanResumeListAsync(data.c_seq);
          try
          {
            data.can_resume = sr.SelectCandidateResumeOnly(data.c_seq);
          } 
          catch(Exception e)
          {
            throw e;
          }
        }

        return Json(new 
        {
          result = 1,
          message = "[Request candidate list OK]" + (list.item.Count == 0 ? " But No Result." : ""),
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
          message = "[Request candidate list FAIL]" + e.Message
          
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

        var list = await rtr.SelectRemeberCandidateListAsync<r_candidate_full>(id);

        foreach (var data in list.item)
        {
          data.can_school = await rtr.SelectRemeberCanSchoolListAsync(data.c_seq);
          data.can_career = await rtr.SelectRemeberCanCareerListAsync(data.c_seq);
          data.can_business = await rtr.SelectRemeberCanBusinessListAsync(data.c_seq);
          data.can_job = await rtr.SelectRemeberCanJobListAsync(data.c_seq);
          data.can_foreign_lan = await rtr.SelectRemeberCanLanguageListAsync(data.c_seq);
          data.can_activity = await rtr.SelectRemeberCanActivityListAsync(data.c_seq);
          data.can_resume_list = await rtr.SelectRemeberCanResumeListAsync(data.c_seq);
          try
          {
            data.can_resume = sr.SelectCandidateResumeOnly(data.c_seq);
          }
          catch (Exception e)
          {
            throw e;
          }
        }

        return Json(new
        {
          result = 1,
          message = "[Request candidate code(" + id.ToString() + ") OK]" + (list.item.Count == 0 ? " But No Result." : ""),
          count = list.item.Count,
          data = list.item,
        });
      }
      catch (Exception e)
      {
        return Json(new
        {
          result = -1,
          message = "[Request candidate code(" + id.ToString() + ") FAIL]" + e.Message

        });
      }
    }

    // GET tasks/5
    [Route("{id:int}/can-school")]
    public async Task<IHttpActionResult> GetCanSchool(int id)
    {
      try
      {
        RememberTaskRepository rtr = new RememberTaskRepository();
        //SearchEngineRepository sr = new SearchEngineRepository();

        //var list = await rtr.SelectRemeberCandidateListAsync<r_candidate_full>(id);

        var list = await rtr.SelectRemeberCanSchoolListAsync(id);

        return Json(new
        {
          result = 1,
          message = "[Request candidate school code(" + id.ToString() + ") OK]" + (list.Count == 0 ? " But No Result." : ""),
          count = list.Count,
          data = list,
        });
      }
      catch (Exception e)
      {
        return Json(new
        {
          result = -1,
          message = "[Request candidate school code(" + id.ToString() + ") FAIL]" + e.Message

        });
      }
    }

    // GET tasks/5
    [Route("{id:int}/can-career")]
    public async Task<IHttpActionResult> GetCanCareer(int id)
    {
      try
      {
        RememberTaskRepository rtr = new RememberTaskRepository();
        //SearchEngineRepository sr = new SearchEngineRepository();

        //var list = await rtr.SelectRemeberCandidateListAsync<r_candidate_full>(id);

        var list = await rtr.SelectRemeberCanCareerListAsync(id);

        return Json(new
        {
          result = 1,
          message = "[Request candidate career code(" + id.ToString() + ") OK]" + (list.Count == 0 ? " But No Result." : ""),
          count = list.Count,
          data = list,
        });
      }
      catch (Exception e)
      {
        return Json(new
        {
          result = -1,
          message = "[Request candidate career code(" + id.ToString() + ") FAIL]" + e.Message

        });
      }
    }




  }
}
