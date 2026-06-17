using System.Web;
using System.Web.Mvc;

namespace Univision.Remember.RestfulServer
{
  public class FilterConfig
  {
    public static void RegisterGlobalFilters(GlobalFilterCollection filters)
    {
      filters.Add(new HandleErrorAttribute());
    }
  }
}
