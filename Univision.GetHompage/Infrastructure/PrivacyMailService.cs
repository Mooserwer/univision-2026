using System;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Text;

namespace Univision.GetHompage.Infrastructure
{
  // ─────────────────────────────────────────────────────────────
  //  개인정보 동의완료 알림 메일 (정보주체 / 발신자).
  //  Univision.Main 메일 인프라를 콘솔용으로 복사 (RestfulServer InvoiceMailService 와 동일 패턴).
  //  전송은 IIS 픽업 디렉터리(C:\Inetpub\mailroot\Pickup) 방식 — Main 의 CDO 픽업과 동일 배달.
  // ─────────────────────────────────────────────────────────────
  public class PrivacyMailService
  {
    private const string originId = "noreply@unicosearch.com";
    private const string pickupDir = @"C:\Inetpub\mailroot\Pickup";

    public bool Send(string[] toArr, string subject, string body, out string message)
    {
      try
      {
        using (MailMessage mailMsg = new MailMessage())
        {
          foreach (var to in toArr.Where(x => !string.IsNullOrWhiteSpace(x)))
            mailMsg.To.Add(to);

          mailMsg.From = new MailAddress(originId, "유니코써치");
          mailMsg.Subject = subject;
          mailMsg.Body = body;
          mailMsg.BodyEncoding = Encoding.UTF8;
          mailMsg.SubjectEncoding = Encoding.UTF8;
          mailMsg.IsBodyHtml = true;

          SmtpClient smtp = new SmtpClient
          {
            DeliveryMethod = SmtpDeliveryMethod.SpecifiedPickupDirectory,
            PickupDirectoryLocation = pickupDir
          };
          smtp.Send(mailMsg);
        }
        message = "메일 발송 했습니다.";
        return true;
      }
      catch (Exception e)
      {
        message = e.Message;
        return false;
      }
    }

    private static string H(string s)
    {
      return WebUtility.HtmlEncode(s ?? "");
    }

    /// <summary>정보주체용 동의완료 안내 (홈페이지 _api/privacy/mail_privacy_agree.html 과 동일 템플릿)</summary>
    public static string SubjectBody(string canName, string agreeType, string agreeName, string agreeDt, string url)
    {
      return @"<!DOCTYPE html PUBLIC '-//W3C//DTD XHTML 1.0 Transitional//EN' 'http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd'>
<html xmlns='http://www.w3.org/1999/xhtml' lang='ko' xml:lang='ko'>
<head>
<meta http-equiv='Content-Type' content='text/html; charset=utf-8' />
<meta http-equiv='Cache-Control' content='no-cache' />
<title>유니코써치</title>
</head>
<body style='margin:0;padding:0;font-family:NanumGothic,ng,dotum;font-size:12px;color:#666;line-height:18px;white-space:normal'>
<div style='position:relative;'>
	<div style='width:740px; margin:0 auto;'>
		<div><img src='https://www.unicosearch.com/_images/account/t_04.gif' /></div>
		<div style='float:left;'><img src='https://www.unicosearch.com/_images/account/04top_bg.gif' alt='탑이미지' /></div>
		<div style='padding:15px'>
			<div style='border:1px solid #ccc; border-radius:15px; padding:20px; margin-bottom:40px;'>
				<div style='padding:30px;'>
					안녕하세요? <span style='font-weight:bold'>" + H(canName) + @" 님</span>
					<br/><br/>
					인재추천 및 채용절차 진행을 위한 아래와 같이 개인정보 수집·이용 및 제3자 제공에 동의하신 내역을 안내 드립니다.<br/><br/>
					<div style='background:#f4f4f4; border:1px solid #e2e1e1; padding:10px; font-weight:bold;font-size:13px;'>
						- 동의구분 : " + H(agreeType) + @" <br/>
						- 동의자 : " + H(agreeName) + @"<br/>
						- 동의일시 : " + H(agreeDt) + @" <br/>
					</div>
					<br/><br/>
					제공해 주신 " + H(canName) + @"님의 소중한 개인정보는 채용을 위한 절차 이외에 어떤 목적으로도 사용되지 않으며 당사의 개인정보보호 정책에 따라 기술적·물리적인 방법으로 적극 보호되고 있습니다.
					<div style='padding:20px 0;'>
						<span style='color:#28757c; font-weight:bold;'>아래의 링크주소를 클릭하시면 개인정보 수집 및 제공 동의 내역을 자세히 확인 하실 수 있습니다.</span>
					</div>
					<div style='background:#f4f4f4; border:1px solid #e2e1e1; padding:10px; font-weight:bold;font-size:13px;'>
						동의 내역 확인하기 : <a href='" + H(url) + @"'>" + H(url) + @"</a>
					</div>
				</div>
			</div>
			<div style='padding-top:15px'>
				<div style='padding-left:5px'>
					<ul style='list-style:none; margin:0; padding:0;'>
						<li>
							<a href='https://www.unicosearch.com/insight/news/2040' target='_blank'><img src='https://www.unicosearch.com/_images/account/footermark_01.gif' alt='고용서비스 우수기관 선정' style='border:none;padding-right:6px;' /></a>
							<a href='https://www.unicosearch.com/insight/news/2017' target='_blank'><img src='https://www.unicosearch.com/_images/account/footermark_04.gif' alt='대한민국 일하기 좋은 300대 기업 선정' style='border:none;' /></a>
						</li>
					</ul>
				</div>
				<div style='padding:10px 0 0 5px;'><img src='https://www.unicosearch.com/_images/account/footer_mail_2020.png' alt='서울특별시 강남구 테헤란로87길 36 도심공항타워 17층, 25층  TEL : 82-2-551-2300  /  Fax : 82-2-551-4959' /></div>
			</div>
		</div>
	</div>
</div>
</body>
</html>";
    }

    /// <summary>발신자(담당자)용 동의완료 통지</summary>
    public static string SenderBody(string canName, string canContact, int? profileId, string agreeType, string agreeDt, string agreeUrl)
    {
      string td = "padding:6px; border:1px solid #dee2e6;";
      string th = td + " background-color:#007bff; color:#fff; text-align:left; width:120px;";
      string profileCell = "-";
      if ((profileId ?? 0) > 0)
      {
        string profileUrl = "https://headhunting-pro.rememberapp.co.kr/internal-db/" + profileId.Value;
        profileCell = "<a href='" + H(profileUrl) + "' target='_blank'>" + profileId.Value + "</a>";
      }
      return @"<html>
  <body style='font-size:12px; color:#212529;'>
    <p><b>내부인재풀 개인정보 동의가 완료되었습니다.</b></p>
    <table style='border-collapse:collapse; width:600px;'>
      <tr><th style='" + th + "'>대상자</th><td style='" + td + "'>" + H(canName) + " (" + H(canContact) + @")</td></tr>
      <tr><th style='" + th + "'>프로필 ID</th><td style='" + td + "'>" + profileCell + @"</td></tr>
      <tr><th style='" + th + "'>동의구분</th><td style='" + td + "'>" + H(agreeType) + @"</td></tr>
      <tr><th style='" + th + "'>동의일시</th><td style='" + td + "'>" + H(agreeDt) + @"</td></tr>
      <tr><th style='" + th + "'>동의정보</th><td style='" + td + "'><a href='" + H(agreeUrl) + @"' target='_blank'>동의 내역 확인하기</a></td></tr>
    </table>
    <p style='color:#6c757d;'>※ 프로필에 실제 수신정보가 반영되는데 시간이 소요될 수 있습니다.</p>
  </body>
</html>";
    }
  }
}
