using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Univision.Main.Infrastructure.Mailing
{
  public class InvoiceCreateDto : MailDto
  {
    /// <summary>
    /// 메일 제목
    /// </summary>
    public string title { get; set; }

    public string name { get; set; }

    public string comment { get; set; }

    public string candidatename { get; set; }
    public string candidatesourcetxt { get; set; }
    public string candidatepositiontxt { get; set; }
    public string income { get; set; }
    public string joindate { get; set; }
    public string feerate { get; set; }
    public string vattype { get; set; }
    public string fee { get; set; }

    public string amt { get; set; }

    public string vat { get; set; }
    
    public string pono { get; set; }

    public string clientname { get; set; }
    public string ceo { get; set; }
    public string address { get; set; }
    public string contactname { get; set; }

    public string contactemail { get; set; }


    public string contactphone { get; set; }


    public string etaxname { get; set; }


    public string etaxmail { get; set; }


    public string etaxphone { get; set; }


    public string feeshare { get; set; }


    public string requestDate { get; set; }

    /// <summary>
    /// Return url
    /// </summary>
    public string url { get; set; }

  }

  public class InvoiceCreateTemplete : TempleteDto
  {
    public InvoiceCreateTemplete()
    {
      this.MailSubject = "[Univision] {{vattype}} {{clientname}} - {{title}}";
      this.MailBody = @" <html> <body style='width:500px;'> <table style='border-collapse:collapse; width:80%; margin-bottom: 1rem; color:#212529; font-size:12px;'> <tbody> <tr>
<th style='padding: 5px; vertical-align: top; border-top: 1px solid #dee2e6; border: 1px solid #dee2e6 !important; background-color: #007bff; color: #fff'>신청자</th>
<td style='padding: 5px; vertical-align: top; border-top: 1px solid #dee2e6; border: 1px solid #dee2e6 !important'>{{name}}</td>
</tr>
<tr>
<th style='padding: 5px; vertical-align: top; border-top: 1px solid #dee2e6; border: 1px solid #dee2e6 !important; background-color: #007bff; color: #fff'>요청내용</th>
<td style='padding: 5px; vertical-align: top; border-top: 1px solid #dee2e6; border: 1px solid #dee2e6 !important'>{{comment}}</td>
</tr>
<tr>
<th style='padding: 5px; vertical-align: top; border-top: 1px solid #dee2e6; border: 1px solid #dee2e6 !important; background-color: #007bff; color: #fff'>후보자정보</th>
<td style='padding: 5px; vertical-align: top; border-top: 1px solid #dee2e6; border: 1px solid #dee2e6 !important'>
<br/>
＊후보자명 : {{candidatename}} {{joindate}}<br/><br/>
＊연봉 & Fee rate : {{income}}({{feerate}} %)<br/><br/>
＊수임료{{vattype}} : {{fee}} [공급가 : {{amt}} / VAT : {{vat}} ]<br/><br/>
{{pono}}
* 후보자 소스 : {{candidatesourcetxt}}<br/>
* 후보자 최종 직급 : {{candidatepositiontxt}}<br/>
</td>
</tr>
<tr>
<th style='padding: 5px; vertical-align: top; border-top: 1px solid #dee2e6; border: 1px solid #dee2e6 !important; background-color: #007bff; color: #fff'>고객사 정보</th>
<td style='padding: 5px; vertical-align: top; border-top: 1px solid #dee2e6; border: 1px solid #dee2e6 !important'>
<br/>
＊고객사명 : {{clientname}}<br/><br/>
＊대표자 : {{ceo}}<br/><br/>
＊주소 : {{address}}<br/>
</td>
</tr><tr>
<th style='padding: 5px; vertical-align: top; border-top: 1px solid #dee2e6; border: 1px solid #dee2e6 !important; background-color: #007bff; color: #fff'>인보이스 담당자 정보</th>
<td style='padding: 5px; vertical-align: top; border-top: 1px solid #dee2e6; border: 1px solid #dee2e6 !important'>
<br/>
＊이름 : {{contactname}}<br/><br/>
＊E-mail : <a href='mailto:{{contactemail}}'>{{contactemail}}</a><br/><br/>
＊연락처 : {{contactphone}}<br/>
</td>
</tr>
<tr>
<th style='padding: 5px; vertical-align: top; border-top: 1px solid #dee2e6; border: 1px solid #dee2e6 !important; background-color: #007bff; color: #fff'>세금계산서 담당자 정보</th>
<td style='padding: 5px; vertical-align: top; border-top: 1px solid #dee2e6; border: 1px solid #dee2e6 !important'>
<br/>
＊이름 : {{etaxname}}<br/><br/>
＊E-mail : <a href='mailto:{{etaxmail}}'>{{etaxmail}}</a><br/><br/>
＊연락처 : {{etaxphone}}<br/>
</td>
</tr>
<tr>
<th style='padding: 5px; vertical-align: top; border-top: 1px solid #dee2e6; border: 1px solid #dee2e6 !important; background-color: #007bff; color: #fff'>Fee sharing 정보</th>
<td style='padding: 5px; vertical-align: top; border-top: 1px solid #dee2e6; border: 1px solid #dee2e6 !important'>
<br/>
{{feeshare}}
<br/>
</td>
</tr>
</tbody></table>
<a href='{{url}}' target='_blank'>Go Univision</a></body></html> ";
    }
  }

}