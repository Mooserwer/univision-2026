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
  [RoutePrefix("clients/v2")]
  public class Clients2Controller : ApiController
  {
    // GET tasks
    [Route("list")]
    public async Task<IHttpActionResult> Get([FromUri] string update_at = "", [FromUri] int from_row = 0, [FromUri] int page_size = 100)
    {
      try
      {
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

        var list = await rtr.SelectRememberClientListAsync_v2<r_client_full_v2>("", 0, from_row, page_size);

        foreach (var data in list.item)
        {
          data.am = await rtr.SelectRememberClientAmListAsync_v2(data.c_seq);
          data.contact = await rtr.SelectRememberClientContactListAsync_v2(data.c_seq);
          data.tax_contact = await rtr.SelectRememberClientTaxContactListAsync_v2(data.c_seq);
          data.memo = await rtr.SelectRememberClientMemoListAsync_v2(data.c_seq);
          data.file = await rtr.SelectRememberClientFileListAsync_v2(data.c_seq);
          data.contract = await rtr.SelectRememberClientContractListAsync_v2(data.c_seq);
          if (data.contract != null)
          {
            if (data.contract.fee_type == "A" && data.contract.cc_seq > 0)
            {
              data.contract.fee_rate = await rtr.SelectRememberContractIncomeListAsync_v2(data.c_seq, data.contract.cc_seq);
            }
            else if (data.contract.fee_type == "B" && data.contract.cc_seq > 0)
            {
              data.contract.fee_rate = await rtr.SelectRememberContractPositionListAsync_v2(data.c_seq, data.contract.cc_seq);
            }
          }
        }


        return Json(new
        {
          result = 1,
          message = "[Request Client list(ver.2) OK]" + (list.item.Count == 0 ? " But No Result." : ""),
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
          message = "[Request Client list(ver.2) FAIL]" + e.Message

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

        //var list = await rtr.SelectRememberClientListAsync_v2<r_client_full_v2>("", );

        var list = await rtr.SelectRememberClientListAsync_v2<r_client_full_v2>("", 0, 0, 99999);

        foreach (var data in list.item)
        {
          data.am = await rtr.SelectRememberClientAmListAsync_v2(data.c_seq);
          data.contact = await rtr.SelectRememberClientContactListAsync_v2(data.c_seq);
          data.tax_contact = await rtr.SelectRememberClientTaxContactListAsync_v2(data.c_seq);
          data.memo = await rtr.SelectRememberClientMemoListAsync_v2(data.c_seq);
          data.file = await rtr.SelectRememberClientFileListAsync_v2(data.c_seq);
          data.contract = await rtr.SelectRememberClientContractListAsync_v2(data.c_seq);
          if (data.contract != null)
          {
            if (data.contract.fee_type == "A" && data.contract.cc_seq > 0)
            {
              data.contract.fee_rate = await rtr.SelectRememberContractIncomeListAsync_v2(data.c_seq, data.contract.cc_seq);
            }
            else if (data.contract.fee_type == "B" && data.contract.cc_seq > 0)
            {
              data.contract.fee_rate = await rtr.SelectRememberContractPositionListAsync_v2(data.c_seq, data.contract.cc_seq);
            }
          }
        }

        return Json(new
        {
          result = 1,
          message = "[Request Client bulk(ver.2) OK]" + (list.item.Count == 0 ? " But No Result." : ""),
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
          message = "[Request Client bulk(ver.2) FAIL]" + e.Message

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

        //var list = await rtr.SelectRememberClientListAsync<r_client_full>("", id);

        var list = await rtr.SelectRememberClientListAsync_v2<r_client_full_v2>("", id);

        foreach (var data in list.item)
        {
          data.am = await rtr.SelectRememberClientAmListAsync_v2(data.c_seq);
          data.contact = await rtr.SelectRememberClientContactListAsync_v2(data.c_seq);
          data.tax_contact = await rtr.SelectRememberClientTaxContactListAsync_v2(data.c_seq);
          data.memo = await rtr.SelectRememberClientMemoListAsync_v2(data.c_seq);
          data.file = await rtr.SelectRememberClientFileListAsync_v2(data.c_seq);
          data.contract = await rtr.SelectRememberClientContractListAsync_v2(data.c_seq);
          if (data.contract != null)
          {
            if (data.contract.fee_type == "A" && data.contract.cc_seq > 0)
            {
              data.contract.fee_rate = await rtr.SelectRememberContractIncomeListAsync_v2(data.c_seq, data.contract.cc_seq);
            }
            else if (data.contract.fee_type == "B" && data.contract.cc_seq > 0)
            {
              data.contract.fee_rate = await rtr.SelectRememberContractPositionListAsync_v2(data.c_seq, data.contract.cc_seq);
            }
          }
        }

        return Json(new
        {
          result = 1,
          message = "[Request Client code(" + id.ToString() + ") (Ver.2) OK]" + (list.item.Count == 0 ? " But No Result." : ""),
          count = list.item.Count,
          data = list.item,
        });
      }
      catch (Exception e)
      {
        return Json(new
        {
          result = -1,
          message = "[Request Client code(" + id.ToString() + ") (Ver.2) FAIL]" + e.Message

        });
      }
    }

  }
}
