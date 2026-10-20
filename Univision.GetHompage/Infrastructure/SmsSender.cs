using System;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Xml.Linq;
using Univision.Core.Models.DTO.Response.Sms;

namespace Univision.GetHompage.Infrastructure
{
  // ─────────────────────────────────────────────────────────────
  //  Univision.Main.Infrastructure.SMS.SmsService.SendSmsAsyncNew 를 콘솔용으로 복사.
  //  계정정보는 소스에 두지 않고 App.config appSettings 에서 읽음:
  //    <add key="SmsServiceId" value="..." />
  //    <add key="SmsServicePw" value="..." />
  // ─────────────────────────────────────────────────────────────
  public class SmsSender
  {
    private const string serviceSmsUri = "http://rest.supersms.co:6200/sms/xml";

    public async Task<SmsServiceModel> SendAsync(string phoneSender, string phoneReceiver, string message)
    {
      SmsServiceModel resultDto = new SmsServiceModel();
      try
      {
        string serviceId = ConfigurationManager.AppSettings["SmsServiceId"];
        string servicePw = ConfigurationManager.AppSettings["SmsServicePw"];
        if (string.IsNullOrWhiteSpace(serviceId) || string.IsNullOrWhiteSpace(servicePw))
          throw new Exception("App.config 에 SmsServiceId / SmsServicePw 설정이 없습니다.");

        StringBuilder getData = new StringBuilder();
        getData.Append("id=" + WebUtility.UrlEncode(serviceId) + "&");
        getData.Append("pwd=" + WebUtility.UrlEncode(servicePw) + "&");
        getData.Append("from=" + WebUtility.UrlEncode(CheckPhoneNumber(phoneSender)) + "&");
        getData.Append("to_country=82&");
        getData.Append("to=" + WebUtility.UrlEncode(CheckPhoneNumber(phoneReceiver)) + "&");
        getData.Append("message=" + WebUtility.UrlEncode(message) + "&");
        getData.Append("report_req=1&");
        getData.Append("charset=1&");

        HttpWebRequest request = (HttpWebRequest)WebRequest.Create(new Uri(serviceSmsUri + "?" + getData.ToString()));
        request.Method = "GET";
        request.ContentType = "text/xml; encoding='utf-8'";

        using (HttpWebResponse response = (HttpWebResponse)await request.GetResponseAsync())
        {
          XDocument doc = XDocument.Load(response.GetResponseStream());
          var node = doc.Descendants("message").FirstOrDefault();
          var errCode = node == null ? null : node.Element("err_code");

          if (errCode == null)
          {
            resultDto.response_code = "R993";
            resultDto.response_desc = "필수파라미터없음";
            resultDto.msgid = "";
          }
          else
          {
            resultDto.response_code = errCode.Value;
            resultDto.response_desc = errCode.Value == "R000" ? "성공" : "발송 실패";
            resultDto.msgid = errCode.Value == "R000" && node.Element("msgid") != null ? node.Element("msgid").Value : "";
          }
        }
      }
      catch (Exception ex)
      {
        resultDto.response_code = "R999";
        resultDto.response_desc = "알려지지 않은 에러";
        resultDto.exception_message = ex.Message;
      }
      return resultDto;
    }

    // 휴대폰번호면 하이픈 제거 (Main.SmsService.CheckPhoneNumber 와 동일)
    private static string CheckPhoneNumber(string phone)
    {
      if (phone != null && Regex.IsMatch(phone, "01[016789]\\-\\d{2,4}\\-\\d{3,4}"))
        return phone.Replace("-", "");
      return phone;
    }
  }
}
