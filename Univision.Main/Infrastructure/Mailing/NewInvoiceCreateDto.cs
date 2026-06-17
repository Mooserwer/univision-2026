using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Univision.Main.Infrastructure.Mailing
{
  public class NewInvoiceCreateDto : MailDto
  {
    /// <summary>
    /// 메일 제목
    /// </summary>
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

    /// <summary>
    /// Return url
    /// </summary>
    public string url { get; set; }

  }

  public class NewInvoiceCreateTemplete : TempleteDto
  {
    public NewInvoiceCreateTemplete()
    {
      this.MailSubject = "[Univision] {{vattype}} {{clientname}} - {{title}}";
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
          <th style='padding: 5px; vertical-align: top; border-top: 1px solid #dee2e6; border: 1px solid #dee2e6 !important; background-color: #007bff; color: #fff'>인보이스빌링 요청일</th>
          <td style='padding: 5px; vertical-align: top; border-top: 1px solid #dee2e6; border: 1px solid #dee2e6 !important'>{{billingdt}}</td>
          <th style='padding: 5px; vertical-align: top; border-top: 1px solid #dee2e6; border: 1px solid #dee2e6 !important; background-color: #007bff; color: #fff'>세발 요청일</th>
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
    <a href='{{url}}' target='_blank'>Go Univision</a>
  </body>
</html> ";

      /*
@"
  <tr>
    <th rowspan='3' style='padding: 5px; vertical-align: top; border-top: 1px solid #dee2e6; border: 1px solid #dee2e6 !important; background-color: #007bff; color: #fff'>후보자정보</th>
    <th style='padding: 5px; vertical-align: top; border-top: 1px solid #dee2e6; border: 1px solid #dee2e6 !important; background-color: #007bff; color: #fff'>이름</th>
    <td style='padding: 5px; vertical-align: top; border-top: 1px solid #dee2e6; border: 1px solid #dee2e6 !important'>{{candidatename}}</td>
    <th style='padding: 5px; vertical-align: top; border-top: 1px solid #dee2e6; border: 1px solid #dee2e6 !important; background-color: #007bff; color: #fff'>입사일</th>
    <td style='padding: 5px; vertical-align: top; border-top: 1px solid #dee2e6; border: 1px solid #dee2e6 !important'>{{joindate}}</td>
  </tr>
  <tr>
    <th style='padding: 5px; vertical-align: top; border-top: 1px solid #dee2e6; border: 1px solid #dee2e6 !important; background-color: #007bff; color: #fff'>검색 소스</th>
    <td style='padding: 5px; vertical-align: top; border-top: 1px solid #dee2e6; border: 1px solid #dee2e6 !important'>{{candidatesourcetxt}}</td>
    <th style='padding: 5px; vertical-align: top; border-top: 1px solid #dee2e6; border: 1px solid #dee2e6 !important; background-color: #007bff; color: #fff'>최종 직급</th>
    <td style='padding: 5px; vertical-align: top; border-top: 1px solid #dee2e6; border: 1px solid #dee2e6 !important'>{{candidatepositiontxt}}</td>
  </tr>
  <tr>
    <th style='padding: 5px; vertical-align: top; border-top: 1px solid #dee2e6; border: 1px solid #dee2e6 !important; background-color: #007bff; color: #fff'>연봉</th>
    <td colspan='3' style='padding: 5px; vertical-align: top; border-top: 1px solid #dee2e6; border: 1px solid #dee2e6 !important'>{{income}}</td>
  </tr>
";
*/
    }
  }

}