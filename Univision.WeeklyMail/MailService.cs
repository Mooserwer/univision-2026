using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using Univision.Core.Models.DTO;

namespace Univision.WeeklyMail
{
  public class MailService
  {
    private const string originId = "laptop_c@unicosearch.com";
    private const string originPw = "unico11$";
    private const string mServer = "smtp-mail.outlook.com";
    private static string mId = "laptop_c@unicosearch.com";
    private static string mPassword = "unico11$";

    public static readonly Regex regMap = new Regex(@"\{\{[a-zA-Z0-9]+\}\}");

    public MailService()
    {
      mId = originId;
      mPassword = originPw;
    }

    public MailService(string new_id, string new_pw)
    {
      mId = new_id;
      mPassword = new_pw;
    }

    /// <summary>
    /// 회의실 예약 알림 메일 발송.
    /// </summary>
    /// <param name="data"></param>
    /// <param name="template"></param>
    /// <returns></returns>
    public MailResult SendReportMail(MailDto data, TempleteDto template)
    {
      try
      {
        string mailSubject = template.MailSubject;
        string mailContentHtml = template.MailBody;
        

        
        MailMessage mailMsg = new MailMessage();

        //var mailMsg = new MailMessage();
        
        //받는 사람 목록
        for (var i = 0; i < data.ToArr.Length; i++)
        {
            mailMsg.To.Add(data.ToArr[i]);
        }

        //cc 사람 목록
        for (var i = 0; i < data.CCArr.Length; i++)
        {
            mailMsg.CC.Add(data.CCArr[i]);
        }

        //bcc 사람 목록
        for (var i = 0; i < data.BccArr.Length; i++)
        {
            mailMsg.Bcc.Add(data.BccArr[i]);
        }
        
        //mailMsg.Bcc.Add("lee.hc@unicosearch.com");
        //mailMsg.To.Add("lee.hc@unicosearch.com");
        mailMsg.From = new MailAddress("noreply@unicosearch.com", "UnicoSearch");
        mailMsg.Subject = mailSubject;
        mailMsg.Body = mailContentHtml;
        mailMsg.BodyEncoding = Encoding.UTF8;
        mailMsg.IsBodyHtml = true;


        SmtpClient sc = new SmtpClient("smtp-mail.outlook.com", 25);
        NetworkCredential credentials = new NetworkCredential("laptop_c@unicosearch.com", "unico11$");
        sc.Credentials = credentials;
        sc.EnableSsl = true;
        

        sc.DeliveryMethod = SmtpDeliveryMethod.Network;

        //sc.EnableSsl = true;
        sc.Send(mailMsg);


        return new MailResult()
        {
          isSend = true,
          message = "메일 발송 했습니다."
        };
      }
      catch (Exception e)
      {
        return new MailResult()
        {
          isSend = false,
          message = e.Message
        };
      }
    }
  }


  public class MailResult
  {
    public bool isSend { get; set; }
    public string message { get; set; }
  }

  public class MailDto
  {
    public string[] ToArr { get; set; }
    public string[] CCArr { get; set; }
    public string[] BccArr { get; set; }
    public MailAddress From { get; set; }
  }

  public class TempleteDto
  {
    public string MailSubject { get; set; }
    public string MailBody { get; set; }
    public string[] FilePathArr { get; set; }
  }
}