using System;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using Univision.Security;

namespace Univision.Main.Infrastructure.CustomFilters
{
    /// <summary>
    /// [감사용 / DUMMY] 로그인 세션 20분 유휴(미사용) 시 자동 만료 필터.
    ///
    /// 개인정보 보호 감사 증빙용 예시 코드입니다.
    /// 마지막 요청 시각을 세션에 저장하고, 20분 이상 활동이 없으면 세션을 폐기(Abandon)한 뒤
    /// 로그인 화면으로 리다이렉트합니다.
    ///
    /// 현재는 FilterConfig 및 어떤 컨트롤러에도 등록/적용되어 있지 않아 실제 동작에는 영향을 주지 않습니다.
    /// 실제 적용하려면 다음 중 하나를 수행하면 됩니다.
    ///   1) 컨트롤러/액션에 [SessionTimeoutFilter] 특성을 부착
    ///   2) App_Start/FilterConfig.cs 의 RegisterGlobalFilters 에
    ///      filters.Add(new SessionTimeoutFilter()); 추가 (전역 적용)
    /// </summary>
    public class SessionTimeoutFilter : ActionFilterAttribute
    {
        /// <summary>세션 유휴 타임아웃 (분).</summary>
        public const int SESSION_TIMEOUT_MINUTES = 20;

        /// <summary>마지막 활동 시각(UTC) 저장 세션 키.</summary>
        private const string LAST_ACTIVITY_KEY = "__LastActivityUtc";

        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            HttpContextBase http = filterContext.HttpContext;
            HttpSessionStateBase session = http != null ? http.Session : null;

            // 로그인 상태에서만 유휴시간을 검사한다.
            if (session != null && AppIdentity.user_seq != 0)
            {
                DateTime nowUtc = DateTime.UtcNow;
                object last = session[LAST_ACTIVITY_KEY];

                if (last is DateTime)
                {
                    double idleMinutes = (nowUtc - (DateTime)last).TotalMinutes;
                    if (idleMinutes >= SESSION_TIMEOUT_MINUTES)
                    {
                        // 20분 이상 유휴 → 세션 만료 처리 후 로그인 화면으로 이동.
                        session.Clear();
                        session.Abandon();

                        filterContext.Result = new RedirectToRouteResult(new RouteValueDictionary(new
                        {
                            controller = "Account",
                            action = "Index",
                            timeout = 1,
                            returnUrl = (http.Request != null && http.Request.Url != null)
                                ? http.Request.Url.ToString() : "/"
                        }));

                        base.OnActionExecuting(filterContext);
                        return;
                    }
                }

                // 정상 요청이면 마지막 활동 시각을 현재로 갱신한다.
                session[LAST_ACTIVITY_KEY] = nowUtc;
            }

            base.OnActionExecuting(filterContext);
        }
    }
}
