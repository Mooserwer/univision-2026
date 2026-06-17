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
}