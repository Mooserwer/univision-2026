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
    private readonly string _originSuffix;

    /// <param name="origin">고정 허용 Origin ("*" 또는 정확한 origin 하나).</param>
    /// <param name="originSuffix">
    /// 서브도메인 와일드카드. 예 ".rememberapp.co.kr" 지정 시, 요청 Origin 이 https 이고
    /// host 가 이 suffix 로 끝나거나(=서브도메인) 베이스 도메인과 같으면 그 Origin 을 그대로 echo 한다.
    /// (CORS 표준상 Access-Control-Allow-Origin 에 와일드카드 서브도메인은 못 넣으므로 Origin 반영 방식 사용)
    /// </param>
    public AllowCorsAttribute(
        string origin = "*",
        string methods = "GET,POST,OPTIONS",
        string headers = "Content-Type,Accept",
        string originSuffix = null)
    {
      _origin = origin;
      _methods = methods;
      _headers = headers;
      _originSuffix = originSuffix;
    }

    public override void OnActionExecuting(ActionExecutingContext filterContext)
    {
      var req = filterContext.HttpContext.Request;
      var resp = filterContext.HttpContext.Response;

      string origin = req.Headers["Origin"];

      // Origin 헤더가 있으면 CORS 허용 헤더 추가
      if (!string.IsNullOrEmpty(origin))
      {
        string allowOrigin = _origin;

        // originSuffix 가 지정되면, 패턴에 맞는 요청 Origin 을 그대로 echo (서브도메인 와일드카드)
        if (!string.IsNullOrEmpty(_originSuffix) && IsAllowedBySuffix(origin, _originSuffix))
        {
          allowOrigin = origin;
          // Origin 별로 응답이 달라지므로 캐시 분리
          resp.AddHeader("Vary", "Origin");
        }

        resp.AddHeader("Access-Control-Allow-Origin", allowOrigin);
        resp.AddHeader("Access-Control-Allow-Methods", _methods);
        resp.AddHeader("Access-Control-Allow-Headers", _headers);
        // 인증 정보(쿠키 등)도 허용할 경우:
        // resp.AddHeader("Access-Control-Allow-Credentials", "true");
      }

      // Preflight 요청엔 여기서 200 응답하고 액션 실행 중단
      if (req.HttpMethod == "OPTIONS")
      {
        filterContext.Result = new HttpStatusCodeResult(200);
      }

      base.OnActionExecuting(filterContext);
    }

    // 요청 Origin 이 https 이고, host 가 suffix(예 ".rememberapp.co.kr")로 끝나거나 베이스 도메인과 같은지.
    private static bool IsAllowedBySuffix(string origin, string suffix)
    {
      Uri uri;
      if (!Uri.TryCreate(origin, UriKind.Absolute, out uri)) return false;
      if (!uri.Scheme.Equals("https", StringComparison.OrdinalIgnoreCase)) return false;

      string host = uri.Host;                        // 예: headhunting-pro.rememberapp.co.kr
      string baseDomain = suffix.TrimStart('.');     // 예: rememberapp.co.kr

      // suffix 앞의 '.' 덕분에 evil-rememberapp.co.kr 같은 유사도메인은 매칭되지 않음
      return host.Equals(baseDomain, StringComparison.OrdinalIgnoreCase)
          || host.EndsWith(suffix, StringComparison.OrdinalIgnoreCase);
    }
  }
}
