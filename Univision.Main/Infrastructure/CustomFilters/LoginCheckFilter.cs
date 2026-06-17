using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using Univision.Security;

namespace Univision.Main.Infrastructure.CustomFilters
{
    public class LoginCheckFilter : ActionFilterAttribute, IActionFilter
    {
        void IActionFilter.OnActionExecuting(ActionExecutingContext context)
        {
            
            OnActionExecuting(context);
        }
    }

    public class GoLoginFilter : ActionFilterAttribute, IActionFilter
    {
        //Base컨트롤러로 이동.
        void IActionFilter.OnActionExecuting(ActionExecutingContext context)
        {
            if (AppIdentity.user_seq == 0)
                context.Result = new RedirectToRouteResult(new RouteValueDictionary(new
                {
                    controller = "Account",
                    action = "Index",
                    returnUrl = context.HttpContext.Request.Url.ToString()

                }));

            OnActionExecuting(context);
        }
    }
}