using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using System.Net;
using System.Web;
using System.Configuration;
using Univision.Remember.RestfulServer.Infrastructure.CustomFilters;

namespace Univision.Remember.RestfulServer.Handlers
{
  public class AuthMessageHandler : DelegatingHandler
  {
    private const string _APIKeyToCheck = "2f061901-f84d-4f5e-a483-d7b4783efd83";

    static string _allowedIps = "";
    static object _obj = new object();

    public AuthMessageHandler()
    {
      if (string.IsNullOrEmpty(_allowedIps))
      {
        lock (_obj)
        {
          _allowedIps = ConfigurationManager.AppSettings["allowedIPs"].Replace(" ", "").Trim();
        }
      }
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

    private string GetClientIp(HttpRequestMessage request)
    {
      // Web-hosting
      if (request.Properties.ContainsKey("MS_HttpContext"))
      {
        HttpContextWrapper ctx =
            (HttpContextWrapper)request.Properties["MS_HttpContext"];
        if (ctx != null)
        {
          return ctx.Request.UserHostAddress;
        }
      }

      return null;
    }

    protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage httpRequestMessage, CancellationToken cancellationToken)
    {
      bool validKey = false;
      //bool validIP = false;
      IEnumerable<string> requestHeaders;
      string usersIpAddress = GetClientIp(httpRequestMessage);

      var checkApiKeyExists = httpRequestMessage.Headers.TryGetValues("APIKey", out requestHeaders);
      if (checkApiKeyExists)
      {
        if (requestHeaders.FirstOrDefault().Equals(_APIKeyToCheck))
        {
          validKey = true;
        }
      }

      if (!validKey)
      {
        return httpRequestMessage.CreateResponse(HttpStatusCode.Forbidden, "Invalid API Key");
      }



      if (!checkIp(usersIpAddress))
      {
        return httpRequestMessage.CreateResponse(HttpStatusCode.Forbidden, "Not allowed IP Address : " + usersIpAddress);
      }


      var response = await base.SendAsync(httpRequestMessage, cancellationToken);
      return response;
    }
  }
}