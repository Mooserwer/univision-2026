using System;
using System.Linq;
using System.Net.Mail;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;

namespace Univision.Remember.RestfulServer.Infrastructure.Mailing
{
  // ─────────────────────────────────────────────────────────────
  //  Univision.Main 의 메일 인프라를 RestfulServer 용으로 복사한 것.
  //  (WeeklyMail 프로젝트와 동일하게 프로젝트별 자체 복사 패턴)
  //  전송은 IIS 픽업 디렉터리(C:\Inetpub\mailroot\Pickup) 방식 — Main 의 CDO 픽업과 동일한 배달.
  //  RestfulServer 에는 CDO/ADODB COM 참조가 없어 System.Net.Mail 픽업으로 대체(동작 동일).
  // ─────────────────────────────────────────────────────────────

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

  public class MailResult
  {
    public bool isSend { get; set; }
    public string message { get; set; }
  }

  public class MailService
  {
    private const string originId = "noreply@unicosearch.com";
    // 픽업 디렉터리 배달 방식이라 SMTP 비밀번호는 사용하지 않음.
    private const string pickupDir = @"C:\Inetpub\mailroot\Pickup";

    private string mId = originId;

    public static readonly Regex regMap = new Regex(@"\{\{[a-zA-Z0-9]+\}\}");

    public MailService()
    {
      mId = originId;
    }

    public MailService(string new_id)
    {
      mId = string.IsNullOrWhiteSpace(new_id) ? originId : new_id;
    }

    // 템플릿의 {{prop}} 자리표시자를 DTO 속성값으로 치환 (Main._mapMail 과 동일).
    private static string _mapMail(MailDto mdto, string template)
    {
      string mailOutput = template;
      MatchCollection matches = regMap.Matches(template);
      foreach (Match match in matches)
      {
        string propName = match.Value.Replace("{{", "").Replace("}}", "");
        PropertyInfo propInfo = mdto.GetType().GetProperties()
          .Where(pi => pi.CanRead && pi.Name.ToLower() == propName.ToLower())
          .FirstOrDefault();
        if (propInfo != null)
        {
          string propVal = propInfo.GetValue(mdto, null) as string;
          // null 값이면 String.Replace 가 예외를 던지므로 빈 문자열로 치환 (Main 대비 null 안전 보강).
          mailOutput = mailOutput.Replace(match.Value, propVal ?? "");
        }
        else
        {
          throw new Exception("메일 매핑 중 오류: 템플릿 자리표시자에 해당하는 속성이 없습니다. (" + propName + ")");
        }
      }
      return mailOutput;
    }

    /// <summary>
    /// 신규 인보이스 발행 알림 메일 발송. (Univision.Main.MailService.SendInvoiceCreateMail 과 동일한 템플릿/치환)
    /// </summary>
    public MailResult SendInvoiceCreateMail(NewInvoiceCreateDto data, NewInvoiceCreateTemplete template)
    {
      try
      {
        string mailSubject = _mapMail(data, template.MailSubject);
        string mailContentHtml = _mapMail(data, template.MailBody);

        using (MailMessage mailMsg = new MailMessage())
        {
          if (data.ToArr != null)
            foreach (var to in data.ToArr.Where(x => !string.IsNullOrWhiteSpace(x)))
              mailMsg.To.Add(to);
          if (data.CCArr != null)
            foreach (var cc in data.CCArr.Where(x => !string.IsNullOrWhiteSpace(x)))
              mailMsg.CC.Add(cc);
          if (data.BCCArr != null)
            foreach (var bcc in data.BCCArr.Where(x => !string.IsNullOrWhiteSpace(x)))
              mailMsg.Bcc.Add(bcc);

          mailMsg.From = data.From ?? new MailAddress(mId);
          mailMsg.Subject = mailSubject;
          mailMsg.Body = mailContentHtml;
          mailMsg.BodyEncoding = Encoding.UTF8;
          mailMsg.SubjectEncoding = Encoding.UTF8;
          mailMsg.IsBodyHtml = true;

          // IIS 픽업 디렉터리로 배달 (Main 의 CDO cdoSendUsingPickup 과 동일 경로).
          SmtpClient smtp = new SmtpClient
          {
            DeliveryMethod = SmtpDeliveryMethod.SpecifiedPickupDirectory,
            PickupDirectoryLocation = pickupDir
          };
          smtp.Send(mailMsg);
        }

        return new MailResult { isSend = true, message = "메일 발송 했습니다." };
      }
      catch (Exception e)
      {
        return new MailResult { isSend = false, message = e.Message };
      }
    }
  }

  // Univision.Main.Infrastructure.Mailing.NewInvoiceCreateDto 복사본.
  public class NewInvoiceCreateDto : MailDto
  {
    public string title { get; set; }
    public string name { get; set; }
    public string invoicetype { get; set; }
    public string billingdt { get; set; }
    public string taxreqdt { get; set; }
    public string comment { get; set; }
    public string language { get; set; }
    public string invoicetitle { get; set; }
    public string invoicecontents { get; set; }
    public string bankaccount { get; set; }
    public string bankname { get; set; }
    public string candidateinfo { get; set; }
    public string billingamt { get; set; }
    public string retaineramt { get; set; }
    public string currency { get; set; }
    public string feerate { get; set; }
    public string vattype { get; set; }
    public string fee { get; set; }
    public string amt { get; set; }
    public string vat { get; set; }
    public string pono { get; set; }
    public string clientname { get; set; }
    public string ceo { get; set; }
    public string address { get; set; }
    public string bizcode { get; set; }
    public string contactname { get; set; }
    public string contactemail { get; set; }
    public string contactphone { get; set; }
    public string etaxname { get; set; }
    public string etaxmail { get; set; }
    public string etaxphone { get; set; }
    public string feeshare { get; set; }
    public string requestDate { get; set; }
    public string pjturl { get; set; }
    public string invurl { get; set; }
  }

  // Univision.Main.Infrastructure.Mailing.NewInvoiceCreateTemplete 복사본 (템플릿 동일).
  public class NewInvoiceCreateTemplete : TempleteDto
  {
    public NewInvoiceCreateTemplete()
    {
      this.MailSubject = "[PMS] {{vattype}} {{clientname}} - {{title}}";
      this.MailBody = @"
<html>
  <body style='width:800px;'>
    <table style='border-collapse:collapse; width:80%; margin-bottom: 1rem; color:#212529; font-size:12px;'>
      <tbody>
        <tr>
          <th rowspan='4' style='padding: 5px; vertical-align: top; border-top: 1px solid #dee2e6; border: 1px solid #dee2e6 !important; background-color: #007bff; color: #fff'>발행기본정보</th>
          <th style='padding: 5px; vertical-align: top; border-top: 1px solid #dee2e6; border: 1px solid #dee2e6 !important; background-color: #007bff; color: #fff'>발행요청자</th>
          <td style='padding: 5px; vertical-align: top; border-top: 1px solid #dee2e6; border: 1px solid #dee2e6 !important'>{{name}}</td>
          <th style='padding: 5px; vertical-align: top; border-top: 1px solid #dee2e6; border: 1px solid #dee2e6 !important; background-color: #007bff; color: #fff'>종류</th>
          <td style='padding: 5px; vertical-align: top; border-top: 1px solid #dee2e6; border: 1px solid #dee2e6 !important'>{{invoicetype}}</td>
        </tr>
        <tr>
          <th style='padding: 5px; vertical-align: top; border-top: 1px solid #dee2e6; border: 1px solid #dee2e6 !important; background-color: #007bff; color: #fff'>인보이스신청일</th>
          <td style='padding: 5px; vertical-align: top; border-top: 1px solid #dee2e6; border: 1px solid #dee2e6 !important'>{{billingdt}}</td>
          <th style='padding: 5px; vertical-align: top; border-top: 1px solid #dee2e6; border: 1px solid #dee2e6 !important; background-color: #007bff; color: #fff'>발행 요청일</th>
          <td style='padding: 5px; vertical-align: top; border-top: 1px solid #dee2e6; border: 1px solid #dee2e6 !important'>{{taxreqdt}}</td>
        </tr>
        <tr>
          <th style='padding: 5px; vertical-align: top; border-top: 1px solid #dee2e6; border: 1px solid #dee2e6 !important; background-color: #007bff; color: #fff'>발행언어</th>
          <td style='padding: 5px; vertical-align: top; border-top: 1px solid #dee2e6; border: 1px solid #dee2e6 !important'>{{language}}</td>
          <th style='padding: 5px; vertical-align: top; border-top: 1px solid #dee2e6; border: 1px solid #dee2e6 !important; background-color: #007bff; color: #fff'>PO구분</th>
          <td style='padding: 5px; vertical-align: top; border-top: 1px solid #dee2e6; border: 1px solid #dee2e6 !important'>{{pono}}</td>
        </tr>
        <tr>
          <th style='padding: 5px; vertical-align: top; border-top: 1px solid #dee2e6; border: 1px solid #dee2e6 !important; background-color: #007bff; color: #fff'>요청내용</th>
          <td colspan='3' style='padding: 5px; vertical-align: top; border-top: 1px solid #dee2e6; border: 1px solid #dee2e6 !important'>{{comment}}</td>
        </tr>
        <tr>
          <td colspan='5' style='padding: 1px; vertical-align: top; border-top: 1px solid #dee2e6; border: 1px solid #dee2e6 !important'>&nbsp;</td>
        </tr>
        {{candidateinfo}}
        <tr>
          <th rowspan='9' style='padding: 5px; vertical-align: top; border-top: 1px solid #dee2e6; border: 1px solid #dee2e6 !important; background-color: #007bff; color: #fff'>수임료정보</th>
          <th style='padding: 5px; vertical-align: top; border-top: 1px solid #dee2e6; border: 1px solid #dee2e6 !important; background-color: #007bff; color: #fff'>VAT구분</th>
          <td colspan='3' style='padding: 5px; vertical-align: top; border-top: 1px solid #dee2e6; border: 1px solid #dee2e6 !important' >{{vattype}}</td>
        </tr>
        <tr>
          <th style='padding: 5px; vertical-align: top; border-top: 1px solid #dee2e6; border: 1px solid #dee2e6 !important; background-color: #007bff; color: #fff'>수수료율</th>
          <td colspan='3' style='padding: 5px; vertical-align: top; border-top: 1px solid #dee2e6; border: 1px solid #dee2e6 !important'>{{feerate}}</td>
        </tr>
        <tr>
          <th style='padding: 5px; vertical-align: top; border-top: 1px solid #dee2e6; border: 1px solid #dee2e6 !important; background-color: #007bff; color: #fff'>발행금액</th>
          <td colspan='3' style='padding: 5px; vertical-align: top; border-top: 1px solid #dee2e6; border: 1px solid #dee2e6 !important'>{{billingamt}} [{{currency}}]</td>
        </tr>
        <tr>
          <th style='padding: 5px; vertical-align: top; border-top: 1px solid #dee2e6; border: 1px solid #dee2e6 !important; background-color: #007bff; color: #fff'>공제금액(선수금)</th>
          <td colspan='3' style='padding: 5px; vertical-align: top; border-top: 1px solid #dee2e6; border: 1px solid #dee2e6 !important'>{{retaineramt}} [{{currency}}]</td>
        </tr>
        <tr>
          <th style='padding: 5px; vertical-align: top; border-top: 1px solid #dee2e6; border: 1px solid #dee2e6 !important; background-color: #007bff; color: #fff'>공급가</th>
          <td colspan='3' style='padding: 5px; vertical-align: top; border-top: 1px solid #dee2e6; border: 1px solid #dee2e6 !important'>{{amt}} [{{currency}}]</td>
        </tr>
        <tr>
          <th style='padding: 5px; vertical-align: top; border-top: 1px solid #dee2e6; border: 1px solid #dee2e6 !important; background-color: #007bff; color: #fff'>VAT</th>
          <td colspan='3' style='padding: 5px; vertical-align: top; border-top: 1px solid #dee2e6; border: 1px solid #dee2e6 !important'>{{vat}} [{{currency}}]</td>
        </tr>
        <tr>
          <th style='padding: 5px; vertical-align: top; border-top: 1px solid #dee2e6; border: 1px solid #dee2e6 !important; background-color: #007bff; color: #fff'>총계</th>
          <td colspan='3' style='padding: 5px; vertical-align: top; border-top: 1px solid #dee2e6; border: 1px solid #dee2e6 !important; font-weight:bold;'>{{fee}} [{{currency}}]</td>
        </tr>
        <tr>
          <td colspan='4' style='padding: 1px; vertical-align: top; border-top: 1px solid #dee2e6; border: 1px solid #dee2e6 !important'>&nbsp;</td>
        </tr>
        <tr>
          <th style='padding: 5px; vertical-align: top; border-top: 1px solid #dee2e6; border: 1px solid #dee2e6 !important; background-color: #007bff; color: #fff'>Fee sharing 정보</th>
          <td colspan='3' style='padding: 5px; vertical-align: top; border-top: 1px solid #dee2e6; border: 1px solid #dee2e6 !important'>
            {{feeshare}}
          </td>
        </tr>
        <tr>
          <td colspan='5' style='padding: 1px; vertical-align: top; border-top: 1px solid #dee2e6; border: 1px solid #dee2e6 !important'>&nbsp;</td>
        </tr>
        <tr>
          <th rowspan='3' style='padding: 5px; vertical-align: top; border-top: 1px solid #dee2e6; border: 1px solid #dee2e6 !important; background-color: #007bff; color: #fff'>고객사정보</th>
          <th style='padding: 5px; vertical-align: top; border-top: 1px solid #dee2e6; border: 1px solid #dee2e6 !important; background-color: #007bff; color: #fff'>회사명</th>
          <td style='padding: 5px; vertical-align: top; border-top: 1px solid #dee2e6; border: 1px solid #dee2e6 !important'>{{clientname}}</td>
          <th style='padding: 5px; vertical-align: top; border-top: 1px solid #dee2e6; border: 1px solid #dee2e6 !important; background-color: #007bff; color: #fff'>대표자</th>
          <td style='padding: 5px; vertical-align: top; border-top: 1px solid #dee2e6; border: 1px solid #dee2e6 !important'>{{ceo}}</td>
        </tr>
        <tr>
          <th style='padding: 5px; vertical-align: top; border-top: 1px solid #dee2e6; border: 1px solid #dee2e6 !important; background-color: #007bff; color: #fff'>사업자등록번호</th>
          <td colspan='3' style='padding: 5px; vertical-align: top; border-top: 1px solid #dee2e6; border: 1px solid #dee2e6 !important'>{{bizcode}}</td>
        </tr>
        <tr>
          <th style='padding: 5px; vertical-align: top; border-top: 1px solid #dee2e6; border: 1px solid #dee2e6 !important; background-color: #007bff; color: #fff'>인보이스주소</th>
          <td colspan='3' style='padding: 5px; vertical-align: top; border-top: 1px solid #dee2e6; border: 1px solid #dee2e6 !important'>{{address}}</td>
        </tr>
        <tr>
          <td colspan='5' style='padding: 1px; vertical-align: top; border-top: 1px solid #dee2e6; border: 1px solid #dee2e6 !important'>&nbsp;</td>
        </tr>
        <tr>
          <th rowspan='2' style='padding: 5px; vertical-align: top; border-top: 1px solid #dee2e6; border: 1px solid #dee2e6 !important; background-color: #007bff; color: #fff'>담당자정보</th>
          <th style='padding: 5px; vertical-align: top; border-top: 1px solid #dee2e6; border: 1px solid #dee2e6 !important; background-color: #007bff; color: #fff'>이름</th>
          <td colspan='3' style='padding: 5px; vertical-align: top; border-top: 1px solid #dee2e6; border: 1px solid #dee2e6 !important'>{{contactname}}</td>
        </tr>
        <tr>
          <th style='padding: 5px; vertical-align: top; border-top: 1px solid #dee2e6; border: 1px solid #dee2e6 !important; background-color: #007bff; color: #fff'>이메일</th>
          <td style='padding: 5px; vertical-align: top; border-top: 1px solid #dee2e6; border: 1px solid #dee2e6 !important'><a href='mailto:{{contactemail}}'>{{contactemail}}</a></td>
          <th style='padding: 5px; vertical-align: top; border-top: 1px solid #dee2e6; border: 1px solid #dee2e6 !important; background-color: #007bff; color: #fff'>전화</th>
          <td style='padding: 5px; vertical-align: top; border-top: 1px solid #dee2e6; border: 1px solid #dee2e6 !important'>{{contactphone}}</td>
        </tr>
        <tr>
          <td colspan='5' style='padding: 1px; vertical-align: top; border-top: 1px solid #dee2e6; border: 1px solid #dee2e6 !important'>&nbsp;</td>
        </tr>
        <tr>
          <th rowspan='2' style='padding: 5px; vertical-align: top; border-top: 1px solid #dee2e6; border: 1px solid #dee2e6 !important; background-color: #007bff; color: #fff'>계산서담당자정보</th>
          <th style='padding: 5px; vertical-align: top; border-top: 1px solid #dee2e6; border: 1px solid #dee2e6 !important; background-color: #007bff; color: #fff'>이름</th>
          <td colspan='3' style='padding: 5px; vertical-align: top; border-top: 1px solid #dee2e6; border: 1px solid #dee2e6 !important'>{{etaxname}}</td>
        </tr>
        <tr>
          <th style='padding: 5px; vertical-align: top; border-top: 1px solid #dee2e6; border: 1px solid #dee2e6 !important; background-color: #007bff; color: #fff'>이메일</th>
          <td style='padding: 5px; vertical-align: top; border-top: 1px solid #dee2e6; border: 1px solid #dee2e6 !important'><a href='mailto:{{etaxmail}}'>{{etaxmail}}</a></td>
          <th style='padding: 5px; vertical-align: top; border-top: 1px solid #dee2e6; border: 1px solid #dee2e6 !important; background-color: #007bff; color: #fff'>전화</th>
          <td style='padding: 5px; vertical-align: top; border-top: 1px solid #dee2e6; border: 1px solid #dee2e6 !important'>{{etaxphone}}</td>
        </tr>
        <tr>
          <td colspan='5' style='padding: 1px; vertical-align: top; border-top: 1px solid #dee2e6; border: 1px solid #dee2e6 !important'>&nbsp;</td>
        </tr>
        <tr>
          <th rowspan='3' style='padding: 5px; vertical-align: top; border-top: 1px solid #dee2e6; border: 1px solid #dee2e6 !important; background-color: #007bff; color: #fff'>추가정보</th>
          <th style='padding: 5px; vertical-align: top; border-top: 1px solid #dee2e6; border: 1px solid #dee2e6 !important; background-color: #007bff; color: #fff'>제목</th>
          <td colspan='3' style='padding: 5px; vertical-align: top; border-top: 1px solid #dee2e6; border: 1px solid #dee2e6 !important'>{{invoicetitle}}</td>
        </tr>
        <tr>
          <th style='padding: 5px; vertical-align: top; border-top: 1px solid #dee2e6; border: 1px solid #dee2e6 !important; background-color: #007bff; color: #fff'>내용</th>
          <td colspan='3' style='padding: 5px; vertical-align: top; border-top: 1px solid #dee2e6; border: 1px solid #dee2e6 !important'>{{invoicecontents}}</td>
        </tr>
        <tr>
          <th style='padding: 5px; vertical-align: top; border-top: 1px solid #dee2e6; border: 1px solid #dee2e6 !important; background-color: #007bff; color: #fff'>입금은행</th>
          <td style='padding: 5px; vertical-align: top; border-top: 1px solid #dee2e6; border: 1px solid #dee2e6 !important'>{{bankname}}</td>
          <th style='padding: 5px; vertical-align: top; border-top: 1px solid #dee2e6; border: 1px solid #dee2e6 !important; background-color: #007bff; color: #fff'>계좌번호</th>
          <td style='padding: 5px; vertical-align: top; border-top: 1px solid #dee2e6; border: 1px solid #dee2e6 !important'>{{bankaccount}}</td>
        </tr>
      </tbody>
    </table>
    <a href='{{pjturl}}' target='_blank'>PMS 프로젝트 보기</a><br/>
    <a href='{{invurl}}' target='_blank'>PMS 인보이스 보기</a>
  </body>
</html> ";
    }
  }
}
