using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using Univision.Security;

namespace Univision.Remember.RestfulServer.Infrastructure.CustomFilters
{
  /// <summary>
  /// 특정 아이피를 제외한 모든 아이피 login 화면으로 튕겨냄.
  /// </summary>
  public class IPBlockFilter : ActionFilterAttribute
  {
    //Base컨트롤러로 이동.
    public override void OnActionExecuting(ActionExecutingContext filterContext)
    {


      //접속 사용자 IP
      //string usersIpAddress = HttpContext.Current.Request.UserHostAddress;

      //if (!checkIp(usersIpAddress))
      //{
      //    //403 페이지로 리턴.
      //    filterContext.Result = new HttpStatusCodeResult(403);

      //}

      base.OnActionExecuting(filterContext);
    }

    public static bool checkIp(string usersIpAddress)
    {
      //web.config에 설정한 허용 ip string 가져옴.
      string allowedIps = ConfigurationManager.AppSettings["allowedIPs"].Replace(" ", "").Trim();

      //, 구분으로 허용 ip string에서 자름.
      string[] ips = allowedIps.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries);

      foreach (var ip in ips)
      {
        //허용된 IP의 경우 통과
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
  }
}