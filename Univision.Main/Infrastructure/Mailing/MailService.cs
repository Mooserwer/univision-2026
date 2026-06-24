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




namespace Univision.Main.Infrastructure.Mailing
{
  public class MailService
  {
    private const string originId = "noreply@unicosearch.com";
    private const string originPw = "unico11$";
    private const string mServer = "smtp.mailplug.co.kr";
    private const int mPort = 465;
    private string mId = "noreply@unicosearch.com";
    private string mPassword = "unico11$";

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
    private static string _mapMail(MailDto mdto, string template)
    {
      string mailOutput = template;
      try
      {
        MatchCollection matches = regMap.Matches(template);
        foreach (Match match in matches)
        {
          string propName = match.Value.Replace("{{", "").Replace("}}", "");
          PropertyInfo propInfo = mdto.GetType().GetProperties().Where(pi => pi.CanRead == true && pi.Name.ToLower() == propName.ToLower()).FirstOrDefault();
          if (propInfo != null)
          {
            string propVal = propInfo.GetValue(mdto, null) as string;
            mailOutput = mailOutput.Replace(match.Value, propVal);
          }
          else
          {
            throw new Exception("Erreur rencontrée lors d'un mapping de mail, le mail n'a donc pas été envoyé");
          }
        }
      }
      catch (Exception e)
      {
        throw e;
      }
      return mailOutput;
    }

    /// <summary>
    /// 전자결재 알림 메일 발송 (다음 결재자 / 기안자).
    /// </summary>
    public MailResult SendApprNotifyMail(ApprNotifyDto data, TempleteDto template)
    {
      try
      {
        string mailSubject = _mapMail(data, template.MailSubject);
        string mailContentHtml = _mapMail(data, template.MailBody);

        CDO.Message message = new CDO.Message();
        CDO.IConfiguration configuration = message.Configuration;
        ADODB.Fields fields = configuration.Fields;

        ADODB.Field field = fields["http://schemas.microsoft.com/cdo/configuration/smtpserver"];
        field.Value = "localhost";
        field = fields["http://schemas.microsoft.com/cdo/configuration/smtpserverport"];
        field.Value = 25;
        field = fields["http://schemas.microsoft.com/cdo/configuration/sendusing"];
        field.Value = CDO.CdoSendUsing.cdoSendUsingPickup;
        field = fields["http://schemas.microsoft.com/cdo/configuration/smtpserverpickupdirectory"];
        field.Value = @"C:\Inetpub\mailroot\Pickup";
        fields.Update();

        message.To = String.Join(";", data.ToArr);
        message.From = mId;
        message.Subject = mailSubject;
        message.HTMLBody = mailContentHtml;
        message.Send();

        return new MailResult() { isSend = true, message = "메일 발송 했습니다." };
      }
      catch (Exception e)
      {
        return new MailResult() { isSend = false, message = e.Message };
      }
    }

    /// <summary>
    /// 회의실 예약 알림 메일 발송.
    /// </summary>
    /// <param name="data"></param>
    /// <param name="template"></param>
    /// <returns></returns>
    public MailResult SendReservationRoomMail(MeetingRoomReservationDto data, MeetingRoomReservationTemplete template)
    {
      try
      {
        string mailSubject = _mapMail(data, template.MailSubject);
        string mailContentHtml = _mapMail(data, template.MailBody);

        CDO.Message message = new CDO.Message();
        CDO.IConfiguration configuration = message.Configuration;
        ADODB.Fields fields = configuration.Fields;

        ADODB.Field field = fields["http://schemas.microsoft.com/cdo/configuration/smtpserver"];
        field.Value = "localhost";

        field = fields["http://schemas.microsoft.com/cdo/configuration/smtpserverport"];
        field.Value = 25;

        field = fields["http://schemas.microsoft.com/cdo/configuration/sendusing"];
        field.Value = CDO.CdoSendUsing.cdoSendUsingPickup;

        field = fields["http://schemas.microsoft.com/cdo/configuration/smtpserverpickupdirectory"];
        field.Value = @"C:\Inetpub\mailroot\Pickup";
        /*
        ADODB.Field field = fields["http://schemas.microsoft.com/cdo/configuration/smtpserver"];
        field.Value = mServer;

        field = fields["http://schemas.microsoft.com/cdo/configuration/smtpserverport"];
        field.Value = mPort;

        field = fields["http://schemas.microsoft.com/cdo/configuration/sendusing"];
        field.Value = CDO.CdoSendUsing.cdoSendUsingPort;

        field = fields["http://schemas.microsoft.com/cdo/configuration/smtpauthenticate"];
        field.Value = CDO.CdoProtocolsAuthentication.cdoBasic;

        field = fields["http://schemas.microsoft.com/cdo/configuration/sendusername"];
        field.Value = originId;

        field = fields["http://schemas.microsoft.com/cdo/configuration/sendpassword"];
        field.Value = originPw;

        field = fields["http://schemas.microsoft.com/cdo/configuration/smtpusessl"];
        field.Value = "true";
        */
        fields.Update();
        message.To = String.Join(";", data.ToArr);
        message.From = mId;
        //message.CC = CC;
        //message.BCC = BCC;
        message.Subject = mailSubject;
        message.HTMLBody = mailContentHtml;
        message.Send();
        /*
        var mailMsg = new MailMessage();

        //받는 사람 목록
        for (var i = 0; i < data.ToArr.Length; i++)
        {
            mailMsg.To.Add(data.ToArr[i]);
        }
        //mailMsg.Bcc.Add("lee.hc@unicosearch.com");
        mailMsg.From = data.From;
        mailMsg.Subject = mailSubject;
        mailMsg.Body = mailContentHtml;
        mailMsg.BodyEncoding = Encoding.UTF8;
        mailMsg.IsBodyHtml = true;

        SmtpClient smtpClient = new SmtpClient(mServer, mPort);
        NetworkCredential credentials = new NetworkCredential(mId, mPassword);
        smtpClient.Credentials = credentials;
        smtpClient.EnableSsl = true;
        smtpClient.Send(mailMsg);
        */
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

    /// <summary>
    /// 프로젝트 등록, 수정 메일 발송
    /// </summary>
    /// <param name="data"></param>
    /// <param name="template"></param>
    /// <returns></returns>
    public MailResult SendProjectCreateMail(ProjectCreateDto data, ProjectCreateTemplete template)
    {
      try
      {
        string mailSubject = _mapMail(data, template.MailSubject);
        string mailContentHtml = _mapMail(data, template.MailBody);

        CDO.Message message = new CDO.Message();
        CDO.IConfiguration configuration = message.Configuration;
        ADODB.Fields fields = configuration.Fields;

        ADODB.Field field = fields["http://schemas.microsoft.com/cdo/configuration/smtpserver"];
        field.Value = "localhost";

        field = fields["http://schemas.microsoft.com/cdo/configuration/smtpserverport"];
        field.Value = 25;

        field = fields["http://schemas.microsoft.com/cdo/configuration/sendusing"];
        field.Value = CDO.CdoSendUsing.cdoSendUsingPickup;

        field = fields["http://schemas.microsoft.com/cdo/configuration/smtpserverpickupdirectory"];
        field.Value = @"C:\Inetpub\mailroot\Pickup";
        /*
        ADODB.Field field = fields["http://schemas.microsoft.com/cdo/configuration/smtpserver"];
        field.Value = mServer;

        field = fields["http://schemas.microsoft.com/cdo/configuration/smtpserverport"];
        field.Value = mPort;

        field = fields["http://schemas.microsoft.com/cdo/configuration/sendusing"];
        field.Value = CDO.CdoSendUsing.cdoSendUsingPort;

        field = fields["http://schemas.microsoft.com/cdo/configuration/smtpauthenticate"];
        field.Value = CDO.CdoProtocolsAuthentication.cdoBasic;

        field = fields["http://schemas.microsoft.com/cdo/configuration/sendusername"];
        field.Value = originId;

        field = fields["http://schemas.microsoft.com/cdo/configuration/sendpassword"];
        field.Value = originPw;

        field = fields["http://schemas.microsoft.com/cdo/configuration/smtpusessl"];
        field.Value = "true";
        */
        fields.Update();
        message.To = String.Join(";", data.ToArr);
        message.From = mId;
        //objEmail.CC = CC;
        //objEmail.BCC = BCC;
        message.Subject = mailSubject;
        message.HTMLBody = mailContentHtml;
        message.Send();

        //var mailMsg = new MailMessage();
        /*
        //받는 사람 목록
        for (var i = 0; i < data.ToArr.Length; i++)
        {
            mailMsg.To.Add(data.ToArr[i]);
        }
        //mailMsg.Bcc.Add("lee.hc@unicosearch.com");
        mailMsg.From = data.From;
        mailMsg.Subject = mailSubject;
        mailMsg.Body = mailContentHtml;
        mailMsg.BodyEncoding = Encoding.UTF8;
        mailMsg.IsBodyHtml = true;

        SmtpClient smtpClient = new SmtpClient(mServer, 465);
        NetworkCredential credentials = new NetworkCredential(mId, mPassword);
        //smtpClient.Host = mServer;
        smtpClient.UseDefaultCredentials = false;
        smtpClient.DeliveryMethod = SmtpDeliveryMethod.Network;
        //smtpClient.EnableSsl = true;
        smtpClient.Credentials = credentials;
        smtpClient.Send(mailMsg);

        mailMsg.Dispose();
        */
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

    public MailResult SendShareCandidateMail(ShareCandidateDto data, ShareCandidateTemplete template)
    {
      try
      {
        string mailSubject = _mapMail(data, template.MailSubject);
        string mailContentHtml = _mapMail(data, template.MailBody);
        /*
        CDO.Message message = new CDO.Message();
        CDO.IConfiguration configuration = message.Configuration;
        ADODB.Fields fields = configuration.Fields;

        ADODB.Field field = fields["http://schemas.microsoft.com/cdo/configuration/smtpserver"];
        field.Value = "localhost";

        field = fields["http://schemas.microsoft.com/cdo/configuration/smtpserverport"];
        field.Value = 25;

        field = fields["http://schemas.microsoft.com/cdo/configuration/sendusing"];
        field.Value = CDO.CdoSendUsing.cdoSendUsingPickup;

        field = fields["http://schemas.microsoft.com/cdo/configuration/smtpserverpickupdirectory"];
        field.Value = @"C:\Inetpub\mailroot\Pickup";
        /*
        ADODB.Field field = fields["http://schemas.microsoft.com/cdo/configuration/smtpserver"];
        field.Value = mServer;

        field = fields["http://schemas.microsoft.com/cdo/configuration/smtpserverport"];
        field.Value = mPort;

        field = fields["http://schemas.microsoft.com/cdo/configuration/sendusing"];
        field.Value = CDO.CdoSendUsing.cdoSendUsingPort;

        field = fields["http://schemas.microsoft.com/cdo/configuration/smtpauthenticate"];
        field.Value = CDO.CdoProtocolsAuthentication.cdoBasic;

        field = fields["http://schemas.microsoft.com/cdo/configuration/sendusername"];
        field.Value = originId;

        field = fields["http://schemas.microsoft.com/cdo/configuration/sendpassword"];
        field.Value = originPw;

        field = fields["http://schemas.microsoft.com/cdo/configuration/smtpusessl"];
        field.Value = "true";
        
        fields.Update();
        


        message.To = String.Join(";", data.ToArr);
        message.From = mId;
        //objEmail.CC = CC;
        //objEmail.BCC = BCC;
        message.Subject = mailSubject;
        message.HTMLBody = mailContentHtml;
        message.Send();
        */
        
        var mailMsg = new MailMessage();

        //받는 사람 목록
        for (var i = 0; i < data.ToArr.Length; i++)
        {
          mailMsg.To.Add(data.ToArr[i]);
        }
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

    public MailResult SendMakeupRequestMail(MakeupRequestDto data, MakeupRequesTemplete template)
    {
      try
      {
        string mailSubject = _mapMail(data, template.MailSubject);
        string mailContentHtml = _mapMail(data, template.MailBody);

        CDO.Message message = new CDO.Message();
        CDO.IConfiguration configuration = message.Configuration;
        ADODB.Fields fields = configuration.Fields;

        ADODB.Field field = fields["http://schemas.microsoft.com/cdo/configuration/smtpserver"];
        field.Value = "localhost";

        field = fields["http://schemas.microsoft.com/cdo/configuration/smtpserverport"];
        field.Value = 25;

        field = fields["http://schemas.microsoft.com/cdo/configuration/sendusing"];
        field.Value = CDO.CdoSendUsing.cdoSendUsingPickup;

        field = fields["http://schemas.microsoft.com/cdo/configuration/smtpserverpickupdirectory"];
        field.Value = @"C:\Inetpub\mailroot\Pickup";
        /*
        ADODB.Field field = fields["http://schemas.microsoft.com/cdo/configuration/smtpserver"];
        field.Value = mServer;

        field = fields["http://schemas.microsoft.com/cdo/configuration/smtpserverport"];
        field.Value = mPort;

        field = fields["http://schemas.microsoft.com/cdo/configuration/sendusing"];
        field.Value = CDO.CdoSendUsing.cdoSendUsingPort;

        field = fields["http://schemas.microsoft.com/cdo/configuration/smtpauthenticate"];
        field.Value = CDO.CdoProtocolsAuthentication.cdoBasic;

        field = fields["http://schemas.microsoft.com/cdo/configuration/sendusername"];
        field.Value = originId;

        field = fields["http://schemas.microsoft.com/cdo/configuration/sendpassword"];
        field.Value = originPw;

        field = fields["http://schemas.microsoft.com/cdo/configuration/smtpusessl"];
        field.Value = "true";
        */
        fields.Update();
        message.To = String.Join(";", data.ToArr);
        message.From = mId;
        //objEmail.CC = CC;
        //objEmail.BCC = BCC;
        message.Subject = mailSubject;
        message.HTMLBody = mailContentHtml;
        message.Send();
        /*
        var mailMsg = new MailMessage();

        //받는 사람 목록
        for (var i = 0; i < data.ToArr.Length; i++)
        {
            mailMsg.To.Add(data.ToArr[i]);
        }
        //mailMsg.Bcc.Add("lee.hc@unicosearch.com");
        mailMsg.From = data.From;
        mailMsg.Subject = mailSubject;
        mailMsg.Body = mailContentHtml;
        mailMsg.BodyEncoding = Encoding.UTF8;
        mailMsg.IsBodyHtml = true;

        SmtpClient smtpClient = new SmtpClient(mServer, mPort);
        NetworkCredential credentials = new NetworkCredential(mId, mPassword);
        smtpClient.Credentials = credentials;
        smtpClient.EnableSsl = true;
        smtpClient.Send(mailMsg);
        */
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

    public MailResult SendMakeupCancelMail(MakeupCancelDto data, MakeupCancelTemplete template)
    {
      try
      {
        string mailSubject = _mapMail(data, template.MailSubject);
        string mailContentHtml = _mapMail(data, template.MailBody);
        CDO.Message message = new CDO.Message();
        CDO.IConfiguration configuration = message.Configuration;
        ADODB.Fields fields = configuration.Fields;

        ADODB.Field field = fields["http://schemas.microsoft.com/cdo/configuration/smtpserver"];
        field.Value = "localhost";

        field = fields["http://schemas.microsoft.com/cdo/configuration/smtpserverport"];
        field.Value = 25;

        field = fields["http://schemas.microsoft.com/cdo/configuration/sendusing"];
        field.Value = CDO.CdoSendUsing.cdoSendUsingPickup;

        field = fields["http://schemas.microsoft.com/cdo/configuration/smtpserverpickupdirectory"];
        field.Value = @"C:\Inetpub\mailroot\Pickup";
        /*
        ADODB.Field field = fields["http://schemas.microsoft.com/cdo/configuration/smtpserver"];
        field.Value = mServer;

        field = fields["http://schemas.microsoft.com/cdo/configuration/smtpserverport"];
        field.Value = mPort;

        field = fields["http://schemas.microsoft.com/cdo/configuration/sendusing"];
        field.Value = CDO.CdoSendUsing.cdoSendUsingPort;

        field = fields["http://schemas.microsoft.com/cdo/configuration/smtpauthenticate"];
        field.Value = CDO.CdoProtocolsAuthentication.cdoBasic;

        field = fields["http://schemas.microsoft.com/cdo/configuration/sendusername"];
        field.Value = originId;

        field = fields["http://schemas.microsoft.com/cdo/configuration/sendpassword"];
        field.Value = originPw;

        field = fields["http://schemas.microsoft.com/cdo/configuration/smtpusessl"];
        field.Value = "true";
        */
        fields.Update();
        message.To = String.Join(";", data.ToArr);
        message.From = mId;
        //message.CC = CC;
        //message.BCC = BCC;
        message.Subject = mailSubject;
        message.HTMLBody = mailContentHtml;
        message.Send();
        /*
        var mailMsg = new MailMessage();

        //받는 사람 목록
        for (var i = 0; i < data.ToArr.Length; i++)
        {
            mailMsg.To.Add(data.ToArr[i]);
        }
        //mailMsg.Bcc.Add("lee.hc@unicosearch.com");
        mailMsg.From = data.From;
        mailMsg.Subject = mailSubject;
        mailMsg.Body = mailContentHtml;
        mailMsg.BodyEncoding = Encoding.UTF8;
        mailMsg.IsBodyHtml = true;

        SmtpClient smtpClient = new SmtpClient(mServer, mPort);
        NetworkCredential credentials = new NetworkCredential(mId, mPassword);
        smtpClient.Credentials = credentials;
        smtpClient.EnableSsl = true;
        smtpClient.Send(mailMsg);
        */
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

    public MailResult SendInvoiceCreateMail(InvoiceCreateDto data, InvoiceCreateTemplete template)
    {
      try
      {
        string mailSubject = _mapMail(data, template.MailSubject);
        string mailContentHtml = _mapMail(data, template.MailBody);

        CDO.Message message = new CDO.Message();
        CDO.IConfiguration configuration = message.Configuration;
        ADODB.Fields fields = configuration.Fields;

        ADODB.Field field = fields["http://schemas.microsoft.com/cdo/configuration/smtpserver"];
        field.Value = "localhost";

        field = fields["http://schemas.microsoft.com/cdo/configuration/smtpserverport"];
        field.Value = 25;

        field = fields["http://schemas.microsoft.com/cdo/configuration/sendusing"];
        field.Value = CDO.CdoSendUsing.cdoSendUsingPickup;

        field = fields["http://schemas.microsoft.com/cdo/configuration/smtpserverpickupdirectory"];
        field.Value = @"C:\Inetpub\mailroot\Pickup";
        /*
        ADODB.Field field = fields["http://schemas.microsoft.com/cdo/configuration/smtpserver"];
        field.Value = mServer;

        field = fields["http://schemas.microsoft.com/cdo/configuration/smtpserverport"];
        field.Value = mPort;

        field = fields["http://schemas.microsoft.com/cdo/configuration/sendusing"];
        field.Value = CDO.CdoSendUsing.cdoSendUsingPort;

        field = fields["http://schemas.microsoft.com/cdo/configuration/smtpauthenticate"];
        field.Value = CDO.CdoProtocolsAuthentication.cdoBasic;

        field = fields["http://schemas.microsoft.com/cdo/configuration/sendusername"];
        field.Value = originId;

        field = fields["http://schemas.microsoft.com/cdo/configuration/sendpassword"];
        field.Value = originPw;

        field = fields["http://schemas.microsoft.com/cdo/configuration/smtpusessl"];
        field.Value = "true";
        */
        fields.Update();
        message.To = String.Join(";", data.ToArr);
        message.From = mId;
        //message.CC = CC;
        //message.BCC = "itsupport@unicosearch.com";
        message.Subject = mailSubject;
        message.HTMLBody = mailContentHtml;
        message.Send();
        /*
        var mailMsg = new MailMessage();

        //받는 사람 목록
        for (var i = 0; i < data.ToArr.Length; i++)
        {
            mailMsg.To.Add(data.ToArr[i]);
        }
        mailMsg.Bcc.Add("lee.hc@unicosearch.com");
        mailMsg.From = data.From;
        mailMsg.Subject = mailSubject;
        mailMsg.Body = mailContentHtml;
        mailMsg.BodyEncoding = Encoding.UTF8;
        mailMsg.IsBodyHtml = true;

        SmtpClient smtpClient = new SmtpClient(mServer, mPort);
        NetworkCredential credentials = new NetworkCredential(mId, mPassword);
        smtpClient.Credentials = credentials;
        smtpClient.EnableSsl = true;
        smtpClient.Send(mailMsg);
        */
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

    public MailResult SendInvoiceCreateMail(NewInvoiceCreateDto data, NewInvoiceCreateTemplete template)
    {
      try
      {
        string mailSubject = _mapMail(data, template.MailSubject);
        string mailContentHtml = _mapMail(data, template.MailBody);

        CDO.Message message = new CDO.Message();
        CDO.IConfiguration configuration = message.Configuration;
        ADODB.Fields fields = configuration.Fields;

        ADODB.Field field = fields["http://schemas.microsoft.com/cdo/configuration/smtpserver"];
        field.Value = "localhost";

        field = fields["http://schemas.microsoft.com/cdo/configuration/smtpserverport"];
        field.Value = 25;

        field = fields["http://schemas.microsoft.com/cdo/configuration/sendusing"];
        field.Value = CDO.CdoSendUsing.cdoSendUsingPickup;

        field = fields["http://schemas.microsoft.com/cdo/configuration/smtpserverpickupdirectory"];
        field.Value = @"C:\Inetpub\mailroot\Pickup";
        /*
        ADODB.Field field = fields["http://schemas.microsoft.com/cdo/configuration/smtpserver"];
        field.Value = mServer;

        field = fields["http://schemas.microsoft.com/cdo/configuration/smtpserverport"];
        field.Value = mPort;

        field = fields["http://schemas.microsoft.com/cdo/configuration/sendusing"];
        field.Value = CDO.CdoSendUsing.cdoSendUsingPort;

        field = fields["http://schemas.microsoft.com/cdo/configuration/smtpauthenticate"];
        field.Value = CDO.CdoProtocolsAuthentication.cdoBasic;

        field = fields["http://schemas.microsoft.com/cdo/configuration/sendusername"];
        field.Value = originId;

        field = fields["http://schemas.microsoft.com/cdo/configuration/sendpassword"];
        field.Value = originPw;

        field = fields["http://schemas.microsoft.com/cdo/configuration/smtpusessl"];
        field.Value = "true";
        */
        fields.Update();
        message.To = String.Join(";", data.ToArr);
        message.From = mId;
        //message.CC = CC;
        //message.BCC = "itsupport@unicosearch.com";
        message.Subject = mailSubject;
        message.HTMLBody = mailContentHtml;
        message.Send();
        /*
        var mailMsg = new MailMessage();

        //받는 사람 목록
        for (var i = 0; i < data.ToArr.Length; i++)
        {
            mailMsg.To.Add(data.ToArr[i]);
        }
        mailMsg.Bcc.Add("lee.hc@unicosearch.com");
        mailMsg.From = data.From;
        mailMsg.Subject = mailSubject;
        mailMsg.Body = mailContentHtml;
        mailMsg.BodyEncoding = Encoding.UTF8;
        mailMsg.IsBodyHtml = true;

        SmtpClient smtpClient = new SmtpClient(mServer, mPort);
        NetworkCredential credentials = new NetworkCredential(mId, mPassword);
        smtpClient.Credentials = credentials;
        smtpClient.EnableSsl = true;
        smtpClient.Send(mailMsg);
        */
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

    public MailResult SendPrivacyAgreeMail(PrivacyAgreeDto data, PrivacyAgreeTemplete template)
    {
      try
      {
        string mailSubject = template.MailSubject;
        string mailContentHtml = _mapMail(data, template.MailBody);

        CDO.Message message = new CDO.Message();
        CDO.IConfiguration configuration = message.Configuration;
        ADODB.Fields fields = configuration.Fields;

        ADODB.Field field = fields["http://schemas.microsoft.com/cdo/configuration/smtpserver"];
        field.Value = "localhost";

        field = fields["http://schemas.microsoft.com/cdo/configuration/smtpserverport"];
        field.Value = 25;

        field = fields["http://schemas.microsoft.com/cdo/configuration/sendusing"];
        field.Value = CDO.CdoSendUsing.cdoSendUsingPickup;

        field = fields["http://schemas.microsoft.com/cdo/configuration/smtpserverpickupdirectory"];
        field.Value = @"C:\Inetpub\mailroot\Pickup";
        /*
        ADODB.Field field = fields["http://schemas.microsoft.com/cdo/configuration/smtpserver"];
        field.Value = mServer;

        field = fields["http://schemas.microsoft.com/cdo/configuration/smtpserverport"];
        field.Value = mPort;

        field = fields["http://schemas.microsoft.com/cdo/configuration/sendusing"];
        field.Value = CDO.CdoSendUsing.cdoSendUsingPort;

        field = fields["http://schemas.microsoft.com/cdo/configuration/smtpauthenticate"];
        field.Value = CDO.CdoProtocolsAuthentication.cdoBasic;

        field = fields["http://schemas.microsoft.com/cdo/configuration/sendusername"];
        field.Value = originId;

        field = fields["http://schemas.microsoft.com/cdo/configuration/sendpassword"];
        field.Value = originPw;

        field = fields["http://schemas.microsoft.com/cdo/configuration/smtpusessl"];
        field.Value = "true";
        */
        fields.Update();
        message.To = String.Join(";", data.ToArr);
        message.From = mId;
        //message.CC = CC;        
        //message.BCC = mId + ";itsupport@unicosearch.com";
        message.Subject = mailSubject;
        message.HTMLBody = mailContentHtml;
        message.Send();

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
    /// <summary>
    /// 휴가 신청 메일 발송.
    /// </summary>
    /// <param name="data"></param>
    /// <param name="template"></param>
    /// <returns></returns>
    public MailResult SendVacationCreateMail(VactationCreateDto data, VactationCreateTemplete template)
    {
      try
      {
        string mailSubject = _mapMail(data, template.MailSubject);
        string mailContentHtml = _mapMail(data, template.MailBody);
        CDO.Message message = new CDO.Message();
        CDO.IConfiguration configuration = message.Configuration;
        ADODB.Fields fields = configuration.Fields;

        ADODB.Field field = fields["http://schemas.microsoft.com/cdo/configuration/smtpserver"];
        field.Value = "localhost";

        field = fields["http://schemas.microsoft.com/cdo/configuration/smtpserverport"];
        field.Value = 25;

        field = fields["http://schemas.microsoft.com/cdo/configuration/sendusing"];
        field.Value = CDO.CdoSendUsing.cdoSendUsingPickup;

        field = fields["http://schemas.microsoft.com/cdo/configuration/smtpserverpickupdirectory"];
        field.Value = @"C:\Inetpub\mailroot\Pickup";
        /*
        ADODB.Field field = fields["http://schemas.microsoft.com/cdo/configuration/smtpserver"];
        field.Value = mServer;

        field = fields["http://schemas.microsoft.com/cdo/configuration/smtpserverport"];
        field.Value = mPort;

        field = fields["http://schemas.microsoft.com/cdo/configuration/sendusing"];
        field.Value = CDO.CdoSendUsing.cdoSendUsingPort;

        field = fields["http://schemas.microsoft.com/cdo/configuration/smtpauthenticate"];
        field.Value = CDO.CdoProtocolsAuthentication.cdoBasic;

        field = fields["http://schemas.microsoft.com/cdo/configuration/sendusername"];
        field.Value = originId;

        field = fields["http://schemas.microsoft.com/cdo/configuration/sendpassword"];
        field.Value = originPw;

        field = fields["http://schemas.microsoft.com/cdo/configuration/smtpusessl"];
        field.Value = "true";
        */
        fields.Update();
        message.To = String.Join(";", data.ToArr);
        message.From = mId;
        //message.CC = CC;
        //message.BCC = BCC;
        message.Subject = mailSubject;
        message.HTMLBody = mailContentHtml;
        message.Send();
        /*
        var mailMsg = new MailMessage();

        //받는 사람 목록
        for (var i = 0; i < data.ToArr.Length; i++)
        {
            mailMsg.To.Add(data.ToArr[i]);
        }
        //mailMsg.Bcc.Add("lee.hc@unicosearch.com");
        mailMsg.From = data.From;
        mailMsg.Subject = mailSubject;
        mailMsg.Body = mailContentHtml;
        mailMsg.BodyEncoding = Encoding.UTF8;
        mailMsg.IsBodyHtml = true;

        SmtpClient smtpClient = new SmtpClient(mServer, mPort);
        NetworkCredential credentials = new NetworkCredential(mId, mPassword);
        smtpClient.Credentials = credentials;
        smtpClient.EnableSsl = true;
        smtpClient.Send(mailMsg);
        */
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

    public MailResult SendVacationApprMail(VactationCreateDto data, VactationApprTemplete template)
    {
      try
      {
        string mailSubject = _mapMail(data, template.MailSubject);
        string mailContentHtml = _mapMail(data, template.MailBody);
        CDO.Message message = new CDO.Message();
        CDO.IConfiguration configuration = message.Configuration;
        ADODB.Fields fields = configuration.Fields;

        ADODB.Field field = fields["http://schemas.microsoft.com/cdo/configuration/smtpserver"];
        field.Value = "localhost";

        field = fields["http://schemas.microsoft.com/cdo/configuration/smtpserverport"];
        field.Value = 25;

        field = fields["http://schemas.microsoft.com/cdo/configuration/sendusing"];
        field.Value = CDO.CdoSendUsing.cdoSendUsingPickup;

        field = fields["http://schemas.microsoft.com/cdo/configuration/smtpserverpickupdirectory"];
        field.Value = @"C:\Inetpub\mailroot\Pickup";
        /*
        ADODB.Field field = fields["http://schemas.microsoft.com/cdo/configuration/smtpserver"];
        field.Value = mServer;

        field = fields["http://schemas.microsoft.com/cdo/configuration/smtpserverport"];
        field.Value = mPort;

        field = fields["http://schemas.microsoft.com/cdo/configuration/sendusing"];
        field.Value = CDO.CdoSendUsing.cdoSendUsingPort;

        field = fields["http://schemas.microsoft.com/cdo/configuration/smtpauthenticate"];
        field.Value = CDO.CdoProtocolsAuthentication.cdoBasic;

        field = fields["http://schemas.microsoft.com/cdo/configuration/sendusername"];
        field.Value = originId;

        field = fields["http://schemas.microsoft.com/cdo/configuration/sendpassword"];
        field.Value = originPw;

        field = fields["http://schemas.microsoft.com/cdo/configuration/smtpusessl"];
        field.Value = "true";
        */
        fields.Update();
        message.To = String.Join(";", data.ToArr);
        message.From = mId;
        //message.CC = CC;
        //message.BCC = "itsupport@unicosearch.com";
        message.Subject = mailSubject;
        message.HTMLBody = mailContentHtml;
        message.Send();
        /*
        var mailMsg = new MailMessage();

        //받는 사람 목록
        for (var i = 0; i < data.ToArr.Length; i++)
        {
            mailMsg.To.Add(data.ToArr[i]);
        }
        mailMsg.Bcc.Add("lee.hc@unicosearch.com");
        mailMsg.From = data.From;
        mailMsg.Subject = mailSubject;
        mailMsg.Body = mailContentHtml;
        mailMsg.BodyEncoding = Encoding.UTF8;
        mailMsg.IsBodyHtml = true;

        SmtpClient smtpClient = new SmtpClient(mServer, mPort);
        NetworkCredential credentials = new NetworkCredential(mId, mPassword);
        smtpClient.Credentials = credentials;
        smtpClient.EnableSsl = true;
        smtpClient.Send(mailMsg);
        */
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

    /// <summary>
    /// 인오더 담당자 배정 메일
    /// </summary>
    /// <param name="data"></param>
    /// <param name="template"></param>
    /// <returns></returns>
    public MailResult SendInOrderDirectorMail(InOrderDirectorDto data, InOrderDirectorTemplete template)
    {
      try
      {
        string mailSubject = _mapMail(data, template.MailSubject);
        string mailContentHtml = _mapMail(data, template.MailBody);

        var mailMsg = new MailMessage();

        //받는 사람 목록
        for (var i = 0; i < data.ToArr.Length; i++)
        {
          mailMsg.To.Add(data.ToArr[i]);
        }
        mailMsg.Bcc.Add("itsupport@unicosearch.com");
        mailMsg.From = data.From;
        mailMsg.Subject = mailSubject;
        mailMsg.Body = mailContentHtml;
        mailMsg.BodyEncoding = Encoding.UTF8;
        mailMsg.IsBodyHtml = true;

        SmtpClient smtpClient = new SmtpClient(mServer, mPort);
        NetworkCredential credentials = new NetworkCredential(mId, mPassword);
        smtpClient.Credentials = credentials;
        smtpClient.EnableSsl = true;
        smtpClient.Send(mailMsg);

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

}