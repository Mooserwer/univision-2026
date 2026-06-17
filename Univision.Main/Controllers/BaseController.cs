using Elmah;
using Microsoft.Owin.Security;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using Univision.Core.Models.DTO;
using Univision.Core.Repositories;
using Univision.Main.Infrastructure.CustomFilters;
using Univision.Security;
using static Univision.Main.Infrastructure.JavascriptErrorHandler;

namespace Univision.Main.Controllers
{
  public class BaseController : Controller
  {
    static string _allowedIps = "";
    static object _obj = new object();

    public BaseController()
    {
      if (string.IsNullOrEmpty(_allowedIps))
      {
        lock (_obj)
        {
          _allowedIps = ConfigurationManager.AppSettings["allowedIPs"].Replace(" ", "").Trim();
        }
      }
    }
    IAuthenticationManager Authentication
    {
      get { return HttpContext.GetOwinContext().Authentication; }
    }

    public static bool checkIp(string usersIpAddress)
    {
      //, 구분으로 허용 ip string에서 자름.
      string[] ips = _allowedIps.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries);

      foreach (var ip in ips)
      {
        string[] addr = ip.Split('/');

        if (addr.Length != 2)
        {
          if (ip.Equals(usersIpAddress))
            return true;
        }
        else
        {
          if (new IPSubnet(ip).Contains(usersIpAddress))
            return true;

        }
      }

      //허용되지 않은 IP는 막음.
      return false;
    }

    /// <summary>
    /// 액션 실행 전, 외부접속 여부와 SMS인증 여부 체크.
    /// 외부접속이면서 인증 받지 않았다면 LogOut 페이지로 리턴.
    /// 로그인 여부도 체크함.
    /// </summary>
    /// <param name="context"></param>
    protected override void OnActionExecuting(ActionExecutingContext context)
    {
      string usersIpAddress = context.HttpContext.Request.UserHostAddress;

      //외부접속이면서 인증받지 않은 쿠키라면 logout 처리.
      if (!checkIp(usersIpAddress) && AppIdentity.isAuth == 0)
        context.Result = new RedirectToRouteResult(new RouteValueDictionary(new
        {
          controller = "Account",
          action = "LogOut",
          returnUrl = context.HttpContext.Request.Url.ToString()


        }));
      //RouteData.Values["controller"].ToString()

      //로그인 여부 체크.
      if (AppIdentity.user_seq == 0)
      {
        string return_url = String.Empty;
        if (context.RouteData.Values["controller"].ToString() != "Account" && context.RouteData.Values["controller"].ToString() != "Dashboard")
        {
          return_url = context.HttpContext.Request.Url.ToString();
        }

        context.Result = new RedirectToRouteResult(new RouteValueDictionary(new
        {
          controller = "Account",
          action = "Index",
          returnUrl = return_url

        }));
      } else
      {

      }


      base.OnActionExecuting(context);
    }


    [HttpPost]
    public void LogJavaScriptError(string message)
    {
      string msg = HttpUtility.UrlDecode(message);


      ErrorSignal.FromCurrentContext().Raise(new JavaScriptErrorException(msg));
    }

    //모바일 브라우저 여부 체크.
    public bool ChkMobileBrowser()
    {
      HttpBrowserCapabilitiesBase myBrowserCaps = Request.Browser;

      return myBrowserCaps.IsMobileDevice;
    }

    /// <summary>
    /// 개인정보 항목 열람 로그 저장.
    /// </summary>
    /// <param name="uv_seq"></param>
    /// <param name="element_seq"></param>
    /// <param name="menu_name"></param>
    /// <param name="item_name"></param>
    /// <returns></returns>
    [HttpPost]
    public async Task<JsonResult> SaveElementItemOpenLog(int uv_seq, int element_seq, string page_name, string table_name, string item_name, string etc)
    {
      try
      {
        element_open_log log = new element_open_log()
        {
          log_type = ExternalLogType.elementOpen,
          uv_seq = uv_seq,
          element_seq = element_seq,
          page_name = page_name,
          table_name = table_name,
          item_name = item_name,
          log_date = Utils.NowKorea(),
          etc = etc
        };

        ApiEntityRepository ar = new ApiEntityRepository();

        await ar.CreateMobileOpenLog(log);

        return Json(new
        {
          ok = true
        });
      }
      catch (Exception e)
      {
        return Json(new
        {
          ok = false,
          message = "개인정보 항목 열람 기록 저장 중, 오류가 발생 했습니다. " + e.Message
        });
      }
    }

    [HttpPost]
    public async Task<JsonResult> SaveMoveModifyLog(int uv_seq, int element_seq, string page_name, string returnUrl)
    {
      try
      {
        element_open_log log = new element_open_log()
        {
          log_type = ExternalLogType.moveModify,
          uv_seq = uv_seq,
          element_seq = element_seq,
          page_name = page_name,
          log_date = Utils.NowKorea(),
          etc = HttpUtility.UrlDecode(returnUrl)
        };

        ApiEntityRepository ar = new ApiEntityRepository();

        await ar.CreateMobileOpenLog(log);

        return Json(new
        {
          ok = true
        });
      }
      catch (Exception e)
      {
        return Json(new
        {
          ok = false,
          message = "수정페이지 이동 기록 저장 중, 오류가 발생 했습니다. " + e.Message
        });
      }

    }
  }

}