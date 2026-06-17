using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Web.Http;
using Univision.Core.Models.DTO;
using Univision.Core.Models.DTO.Remember;
using Univision.Core.Repositories;

namespace Univision.Remember.RestfulServer.Controllers
{
  [RoutePrefix("tasks")]
  public class TasksController : ApiController
  {
    // GET tasks
    [Route("")]
    public async Task<IHttpActionResult> Get([FromUri] int re = 0, [FromUri] string comment = "")
    {
      try
      {
        RememberTaskRepository rtr = new RememberTaskRepository();
        List<remember_task> list = new List<remember_task>();
        if (re == 1)
        {
          list = await rtr.FindTaskReList();
        }
        else
        {
          list = await rtr.FindTaskList();
        }

        if (list.Count > 0)
        {
          RememberTaskEntityRepository rter = new RememberTaskEntityRepository();
          if (re == 0)
          {
            await rter.BackupRequest(list);
          }

          await rter.UpdateRequest(list, 0, "", comment);
          rter.Dispose();
        }

        return Json(new
        {
          result = 1,
          message = "[Request Task list OK]" + (list.Count == 0 ? " But No Result." : ""),
          count = list.Count,
          data = list
        });
      }
      catch (Exception e)
      {
        return Json(new
        {
          result = -1,
          message = "[Request Task list FAIL] " + e.Message

        });
      }


    }

    // GET tasks/5
    [Route("{id:int}")]

    public async Task<IHttpActionResult> GetOne(int id, [FromUri] string comment = "")
    {
      try
      {
        if (id == 0)
        {
          throw new Exception("Not allowed 0 code");
        }
        
        RememberTaskRepository rtr = new RememberTaskRepository();
        var data = await rtr.FindTaskOneAsync(id);

        if (data != null)
        {
          RememberTaskEntityRepository rter = new RememberTaskEntityRepository();
          await rter.UpdateRequest(data.task_id, 0, "", comment);
          rter.Dispose();
        }

        return Json(new
        {
          result = 1,
          message = "[Request Task (" + id.ToString() + ") OK]" + (data == null ? " But No Result." : ""),
          count = (data == null ? 0 : 1),
          data = data
        });
      }
      catch (Exception e)
      {
        return Json(new
        {
          result = -1,
          message = "[Request Task (" + id.ToString() + ") FAIL]" + e.Message
        });
      }
    }

    // Get tasks
    [Route("{id:int}/candidates")]

    public async Task<IHttpActionResult> GetCandidates(int id)
    {
      try
      {
        if (id == 0)
        {
          throw new Exception("Not allowed 0 code");
        }

        RememberTaskRepository rtr = new RememberTaskRepository();
        var data = await rtr.FindTaskOneAsync(id);
        r_common_paging<r_candidate_full> list = new r_common_paging<r_candidate_full>();
        if (data != null)
        {
          SearchEngineRepository sr = new SearchEngineRepository();

          list = await rtr.SelectRemeberCandidateListAsync<r_candidate_full>(data.c_seq);

          foreach (var candidate in list.item)
          {
            candidate.can_school = await rtr.SelectRemeberCanSchoolListAsync(data.c_seq);
            candidate.can_career = await rtr.SelectRemeberCanCareerListAsync(data.c_seq);
            candidate.can_business = await rtr.SelectRemeberCanBusinessListAsync(data.c_seq);
            candidate.can_job = await rtr.SelectRemeberCanJobListAsync(data.c_seq);
            candidate.can_foreign_lan = await rtr.SelectRemeberCanLanguageListAsync(data.c_seq);
            candidate.can_activity = await rtr.SelectRemeberCanActivityListAsync(data.c_seq);
            candidate.can_resume = sr.SelectCandidateResumeOnly(data.c_seq);
          }


          RememberTaskEntityRepository rter = new RememberTaskEntityRepository();
          await rter.UpdateRequest(data.task_id, 1, "");
          rter.Dispose();
        }

        return Json(new
        {
          result = 1,
          message = "[Request Candidate (Task id: " + id.ToString() + ") OK]" + (data == null ? " But No Result." : ""),
          count = (data == null ? 0 : 1),
          task_id = data.task_id,
          iud_type = data.iud_type,
          data = list
        });
      }
      catch (Exception e)
      {
        return Json(new
        {
          result = -1,
          message = "[Request Candidate (Task id: " + id.ToString() + ") FAIL]" + e.Message
        });
      }
    }

    // POST tasks
    public void Post([FromBody] string value)
    {
    }

    // PUT tasks
    [Route("{id:int}")]
    public async Task<IHttpActionResult> Put(int id, [FromBody] string comment)
    {
      try
      {
        if (id == 0)
        {
          throw new Exception("Not allowed 0 code");
        }

        RememberTaskRepository rtr = new RememberTaskRepository();
        var data = await rtr.FindTaskOneAsync(id);

        if (data != null)
        {
          RememberTaskEntityRepository rter = new RememberTaskEntityRepository();
          await rter.UpdateRequest(data.task_id, 2, "", comment);
          rter.Dispose();
        }
        else
        {
          throw new Exception("No Task found");
        }

        return Json(new
        {
          result = 1,
          message = "[Request Task complete (" + id.ToString() + " OK]" + (data == null ? "But No Result." : "")
        });
      }
      catch (Exception e)
      {
        return Json(new
        {
          result = -1,
          message = "[Request Task complete (" + id.ToString() + " FAIL]" + e.Message
        });
      }
    }

    // DELETE tasks
    public void Delete(int id)
    {
    }

    [Route("{id:int}/candidates-only")]
    public async Task<IHttpActionResult> GetCandidatesOnly(int id)
    {
      try
      {
        if (id == 0)
        {
          throw new Exception("Not allowed 0 code");
        }

        RememberTaskRepository rtr = new RememberTaskRepository();
        var data = await rtr.FindTaskOneAsync(id);
        r_common_paging<r_candidate_full> list = new r_common_paging<r_candidate_full>();
        if (data != null)
        {
          SearchEngineRepository sr = new SearchEngineRepository();

          list = await rtr.SelectRemeberCandidateListAsync<r_candidate_full>(data.c_seq);

          RememberTaskEntityRepository rter = new RememberTaskEntityRepository();
          await rter.UpdateRequest(data.task_id, 1, "");
          rter.Dispose();
        }

        return Json(new
        {
          result = 1,
          message = "[Request Candidate Only (Task id: " + id.ToString() + ") OK]" + (data == null ? " But No Result." : ""),
          count = (data == null ? 0 : 1),
          task_id = data.task_id,
          iud_type = data.iud_type,
          data = list
        });
      }
      catch (Exception e)
      {
        return Json(new
        {
          result = -1,
          message = "[Request Candidate Only  (Task id: " + id.ToString() + ") FAIL]" + e.Message
        });
      }
    }
  }
}
