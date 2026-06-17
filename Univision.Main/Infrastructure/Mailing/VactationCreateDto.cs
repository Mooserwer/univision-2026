using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Univision.Main.Infrastructure.Mailing
{
  public class VactationCreateDto : MailDto
  {
    /// <summary>
    /// 메일 제목
    /// </summary>
    public string title { get; set; }
    public string content { get; set; }
    /// <summary>
    /// 신청자 이름
    /// </summary>
    public string name { get; set; }

    /// <summary>
    /// 휴가 구분
    /// </summary>
    public string vType { get; set; }

    /// <summary>
    /// 시작일
    /// </summary>
    public string startDate { get; set; }

    /// <summary>
    /// 종료일
    /// </summary>
    public string endDate { get; set; }

    /// <summary>
    /// 사용일수
    /// </summary>
    public string day { get; set; }

    /// <summary>
    /// 잔여일수
    /// </summary>
    public string remainday { get; set; }


    /// <summary>
    /// 요청일
    /// </summary>
    public string requestDate { get; set; }

    /// <summary>
    /// Return url
    /// </summary>
    public string url { get; set; }

    public string urlStatus { get; set; }

  }

  public class VactationApprTemplete : TempleteDto
  {
    public VactationApprTemplete()
    {
      this.MailSubject = "[Univision] {{title}}";
      this.MailBody = @" <html> <body style='width:500px;'> <table style='border-collapse:collapse; width:80%; margin-bottom: 1rem; color:#212529; font-size:12px;'> <tbody> <tr>
<th style='padding: .75rem; vertical-align: top; border-top: 1px solid #dee2e6; border: 1px solid #dee2e6 !important; background-color: #007bff; color: #fff'>신청자</th>
<td style='padding: .75rem; vertical-align: top; border-top: 1px solid #dee2e6; border: 1px solid #dee2e6 !important'>{{name}}</td>
</tr><tr>
<th style='padding: .75rem; vertical-align: top; border-top: 1px solid #dee2e6; border: 1px solid #dee2e6 !important; background-color: #007bff; color: #fff'>구분</th>
<td style='padding: .75rem; vertical-align: top; border-top: 1px solid #dee2e6; border: 1px solid #dee2e6 !important'>{{vType}}</td>
</tr><tr>
<th style='padding: .75rem; vertical-align: top; border-top: 1px solid #dee2e6; border: 1px solid #dee2e6 !important; background-color: #007bff; color: #fff'>시작/종료일</th>
<td style='padding: .75rem; vertical-align: top; border-top: 1px solid #dee2e6; border: 1px solid #dee2e6 !important'>{{startDate}} ~ {{endDate}} ({{day}}일)</td>
</tr>
{{remainday}}
<tr>
<th style='padding: .75rem; vertical-align: top; border-top: 1px solid #dee2e6; border: 1px solid #dee2e6 !important; background-color: #007bff; color: #fff'>요청일</th>
<td style='padding: .75rem; vertical-align: top; border-top: 1px solid #dee2e6; border: 1px solid #dee2e6 !important'>{{requestDate}}</td>
</tr><tr>
<th style='padding: .75rem; vertical-align: top; border-top: 1px solid #dee2e6; border: 1px solid #dee2e6 !important; background-color: #007bff; color: #fff'>사유</th>
<td style='padding: .75rem; vertical-align: top; border-top: 1px solid #dee2e6; border: 1px solid #dee2e6 !important'>{{content}}</td>
</tr>
</tbody></table>
<span style='color:red'>※ 휴가 승인 정보는 Univision 메인 화면에서 확인 가능 합니다.</span><br />
<a href='{{url}}' target='_blank'>휴가 승인하러 가기</a><br/>
<a href='{{urlStatus}}' target='_blank'>신청자 휴가 내역 확인</a>
</body></html> ";
    }
  }

  public class VactationCreateTemplete : TempleteDto
  {
    public VactationCreateTemplete()
    {
      this.MailSubject = "[Univision] {{title}}";
      this.MailBody = @" <html> <body style='width:500px;'> <table style='border-collapse:collapse; width:80%; margin-bottom: 1rem; color:#212529; font-size:12px;'> <tbody> <tr>
<th style='padding: .75rem; vertical-align: top; border-top: 1px solid #dee2e6; border: 1px solid #dee2e6 !important; background-color: #007bff; color: #fff'>신청자</th>
<td style='padding: .75rem; vertical-align: top; border-top: 1px solid #dee2e6; border: 1px solid #dee2e6 !important'>{{name}}</td>
</tr><tr>
<th style='padding: .75rem; vertical-align: top; border-top: 1px solid #dee2e6; border: 1px solid #dee2e6 !important; background-color: #007bff; color: #fff'>구분</th>
<td style='padding: .75rem; vertical-align: top; border-top: 1px solid #dee2e6; border: 1px solid #dee2e6 !important'>{{vType}}</td>
</tr><tr>
<th style='padding: .75rem; vertical-align: top; border-top: 1px solid #dee2e6; border: 1px solid #dee2e6 !important; background-color: #007bff; color: #fff'>시작/종료일</th>
<td style='padding: .75rem; vertical-align: top; border-top: 1px solid #dee2e6; border: 1px solid #dee2e6 !important'>{{startDate}} ~ {{endDate}} ({{day}}일)</td>
</tr>
{{remainday}}
<tr>
<th style='padding: .75rem; vertical-align: top; border-top: 1px solid #dee2e6; border: 1px solid #dee2e6 !important; background-color: #007bff; color: #fff'>요청일</th>
<td style='padding: .75rem; vertical-align: top; border-top: 1px solid #dee2e6; border: 1px solid #dee2e6 !important'>{{requestDate}}</td>
</tr><tr>
<th style='padding: .75rem; vertical-align: top; border-top: 1px solid #dee2e6; border: 1px solid #dee2e6 !important; background-color: #007bff; color: #fff'>사유</th>
<td style='padding: .75rem; vertical-align: top; border-top: 1px solid #dee2e6; border: 1px solid #dee2e6 !important'>{{content}}</td>
</tr>
</tbody></table>
<span style='color:red'>※ 휴가 신청 정보는 Univision 휴가관리 화면에서 확인 가능 합니다.</span><br />
<a href='{{urlStatus}}' target='_blank'>휴가 내역 확인</a> ";
    }
  }
}