using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Web;

namespace Univision.Main.Infrastructure.Mailing
{
  public class MailDto
  {
    public string[] ToArr { get; set; }

    public string[] CCArr { get; set; }

    public string[] BCCArr { get; set; }
    public MailAddress From { get; set; }
  }

  public class TempleteDto
  {
    public string MailSubject { get; set; }
    public string MailBody { get; set; }
    public string[] FilePathArr { get; set; }

  }

  // 전자결재 알림 메일 (placeholder 는 영숫자만 — _mapMail 정규식 제약)
  public class ApprNotifyDto : MailDto
  {
    public string recipient { get; set; } = "";
    public string actionkor { get; set; } = "";
    public string docno { get; set; } = "";
    public string title { get; set; } = "";
    public string drafter { get; set; } = "";
    public string opinion { get; set; } = "";
    public string detailurl { get; set; } = "";
  }
}