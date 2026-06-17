using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using Univision.Core.Repositories;
using Univision.Core.Models.DTO;
using System.Threading.Tasks;
using Univision.Remember.RestfulServer.Models;
using Univision.Remember.RestfulServer.Codes;

namespace Univision.Remember.RestfulServer.Controllers
{
  [RoutePrefix("code-candidate-reg-types")]
  public class CodeCanRegTypesController : ApiController
  {
    // GET tasks
    [Route("")]
    public IHttpActionResult Get()
    {
      try
      {
        RegTypeCode rtc = new RegTypeCode();
        List<CodeCandidateRegType> list = new List<CodeCandidateRegType>();

        foreach(var code in rtc.RegTypeCodes)
        {

          list.Add(new CodeCandidateRegType(code.Key, code.Value));
        }

        //var list = await ar.SelectBusinessCode2ListAsync();

        return Json(new 
        {
          result = 1,
          message = "[Request candidate registration type OK]" + (list.Count == 0 ? "But No Result." : ""),
          count = list.Count,
          data = list
        });
      } 
      catch(Exception e)
      {
        return Json(new
        {
          result = -1,
          message = "[Request candidate registration type FAIL]" + e.Message
          
        });
      }
      
      
    }

    // GET tasks/5
    [Route("{id:int}")]
    public IHttpActionResult Get(int id)
    {
      try
      {
        RegTypeCode rtc = new RegTypeCode();
        List<CodeCandidateRegType> list = new List<CodeCandidateRegType>();

        foreach (var code in rtc.RegTypeCodes)
        {
          if (code.Key == id)
          {
            list.Add(new CodeCandidateRegType(code.Key, code.Value));
          }
        }

        return Json(new
        {
          result = 1,
          message = "[Request candidate registration type code(" + id.ToString() + ") OK]" + (list.Count == 0 ? "But No Result." : ""),
          count = list.Count,
          data = list
        });
      } 
      catch (Exception e)
      {
        return Json(new
        {
          result = -1,
          message = "[Request candidate registration type code(" + id.ToString() + ") FAIL]" + e.Message
        });
      }
    }

  }
}
