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
  [RoutePrefix("clients")]
  public class ClientsController : ApiController
  {
    // GET tasks
    [Route("list")]
    public async Task<IHttpActionResult> Get([FromUri] string update_at = "", [FromUri] int from_row = 0, [FromUri] int page_size = 100)
    {
      try
      {

        throw new Exception("update_at 값이 필요합니다. 형식 : yyyy-MM-dd'T'HH:mm:ss.SSS");


        if (page_size > 1000)
        {
          page_size = 1000;
        }

        if (from_row < 0)
        {
          from_row = 0;
        }

        RememberTaskRepository rtr = new RememberTaskRepository();
        SearchEngineRepository sr = new SearchEngineRepository();

        var list = await rtr.SelectRememberClientListAsync<r_client_full>(update_at, 0, from_row, page_size);

        foreach (var data in list.item)
        {
          if (data.fee_type == "A" && data.contract_seq.HasValue)
          {
            data.fee_rate = await rtr.SelectRememberContractIncomeListAsync(data.c_seq, data.contract_seq.Value);
          }
          else if (data.fee_type == "B" && data.contract_seq.HasValue)
          {
            data.fee_rate = await rtr.SelectRememberContractPositionListAsync(data.c_seq, data.contract_seq.Value);
          }

        }

        return Json(new
        {
          result = 1,
          message = "[Request Client list OK]" + (list.item.Count == 0 ? " But No Result." : ""),
          from_row = from_row,
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
          message = "[Request Client list FAIL]" + e.Message

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

        var list = await rtr.SelectRememberClientListAsync<r_client_full>("", 0, 0, 99999);

        foreach (var data in list.item)
        {
          if (data.fee_type == "A" && data.contract_seq.HasValue)
          {
            data.fee_rate = await rtr.SelectRememberContractIncomeListAsync(data.c_seq, data.contract_seq.Value);
          }
          else if (data.fee_type == "B" && data.contract_seq.HasValue)
          {
            data.fee_rate = await rtr.SelectRememberContractPositionListAsync(data.c_seq, data.contract_seq.Value);
          }
        }

        return Json(new
        {
          result = 1,
          message = "[Request Client bulk OK]" + (list.item.Count == 0 ? " But No Result." : ""),
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
          message = "[Request Client bulk FAIL]" + e.Message

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

        var list = await rtr.SelectRememberClientListAsync<r_client_full>("", id);

        foreach (var data in list.item)
        {
          if (data.fee_type == "A" && data.contract_seq.HasValue)
          {
            data.fee_rate = await rtr.SelectRememberContractIncomeListAsync(data.c_seq, data.contract_seq.Value);
          }
          else if (data.fee_type == "B" && data.contract_seq.HasValue)
          {
            data.fee_rate = await rtr.SelectRememberContractPositionListAsync(data.c_seq, data.contract_seq.Value);
          }
        }

        return Json(new
        {
          result = 1,
          message = "[Request Client code(" + id.ToString() + ") OK]" + (list.item.Count == 0 ? " But No Result." : ""),
          count = list.item.Count,
          data = list.item,
        });
      }
      catch (Exception e)
      {
        return Json(new
        {
          result = -1,
          message = "[Request Client code(" + id.ToString() + ") FAIL]" + e.Message

        });
      }
    }

  }
}
