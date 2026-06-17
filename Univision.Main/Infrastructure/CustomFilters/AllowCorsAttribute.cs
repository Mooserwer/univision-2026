using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Univision.Main.Infrastructure.CustomFilters
{
  public class AllowCorsAttribute : ActionFilterAttribute
  {
    private readonly string _origin;
    private readonly string _methods;
    private readonly string _headers;

    public AllowCorsAttribute(
        string origin = "*",
        string methods = "GET,POST,OPTIONS",
        string headers = "Content-Type,Accept")
    {
      _origin = origin;
      _methods = methods;
      _headers = headers;
    }

    public override void OnActionExecuting(ActionExecutingContext filterContext)
    {
      var req = filterContext.HttpContext.Request;
      var resp = filterContext.HttpContext.Response;

      // Origin 헤더가 있으면 CORS 허용 헤더 추가
      if (!string.IsNullOrEmpty(req.Headers["Origin"]))
      {
        resp.AddHeader("Access-Control-Allow-Origin", _origin);
        resp.AddHeader("Access-Control-Allow-Methods", _methods);
        resp.AddHeader("Access-Control-Allow-Headers", _headers);
        // 인증 정보도 허용할 경우:
        // resp.AddHeader("Access-Control-Allow-Credentials", "true");
      }

      // Preflight 요청엔 여기서 200 응답하고 액션 실행 중단
      if (req.HttpMethod == "OPTIONS")
      {
        filterContext.Result = new HttpStatusCodeResult(200);
      }

      base.OnActionExecuting(filterContext);
    }
  }
}