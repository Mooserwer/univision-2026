using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Univision.Main.Infrastructure.Mailing
{
    public class InOrderDirectorDto : MailDto
    {
        /// <summary>
        /// 고객사명
        /// </summary>
        public string companyName { get; set; }

        /// <summary>
        /// 오더 날짜
        /// </summary>
        public string orderDt { get; set; }

        /// <summary>
        /// 오더 타입
        /// </summary>
        public string orderType { get; set; }

        /// <summary>
        /// 담당자이름
        /// </summary>
        public string contactName { get; set; }

        /// <summary>
        /// /담당자 부서
        /// </summary>
        public string contactDept { get; set; }

        /// <summary>
        /// 담당자 직급
        /// </summary>
        public string contactPosition { get; set; }

        /// <summary>
        /// 담당자 전화번호
        /// </summary>
        public string contactPhone { get; set; }

        /// <summary>
        /// 담당자 이메일
        /// </summary>
        public string contactEmail { get; set; }

        /// <summary>
        /// 직무
        /// </summary>
        public string jobDesc { get; set; }

        /// <summary>
        /// 자격요건
        /// </summary>
        public string contents { get; set; }

        /// <summary>
        /// 리턴 URL
        /// </summary>
        public string url { get; set; }

        /// <summary>
        /// 등록일
        /// </summary>
        public string createDt { get; set; }

    }

    public class InOrderDirectorTemplete : TempleteDto
    {
        public InOrderDirectorTemplete()
        {
            this.MailSubject = "[Univision] InOrder 담당 컨설턴트 배정 안내 ";
            this.MailBody = @" <html> <body style='width:500px;'> <table style='border-collapse:collapse; width:80%; margin-bottom: 1rem; color:#212529; font-size:12px;'> <tbody> <tr>
<th style='padding: .75rem; vertical-align: top; border-top: 1px solid #dee2e6; border: 1px solid #dee2e6 !important; background-color: #007bff; color: #fff'>고객사명</th>
<td style='padding: .75rem; vertical-align: top; border-top: 1px solid #dee2e6; border: 1px solid #dee2e6 !important'>{{companyName}}</td>
</tr><tr>
<th style='padding: .75rem; vertical-align: top; border-top: 1px solid #dee2e6; border: 1px solid #dee2e6 !important; background-color: #007bff; color: #fff'>Order Date.</th>
<td style='padding: .75rem; vertical-align: top; border-top: 1px solid #dee2e6; border: 1px solid #dee2e6 !important'>{{orderDt}}</td>
</tr><tr>
<th style='padding: .75rem; vertical-align: top; border-top: 1px solid #dee2e6; border: 1px solid #dee2e6 !important; background-color: #007bff; color: #fff'>타입</th>
<td style='padding: .75rem; vertical-align: top; border-top: 1px solid #dee2e6; border: 1px solid #dee2e6 !important'>{{orderType}}</td>
</tr><tr>
<th style='padding: .75rem; vertical-align: top; border-top: 1px solid #dee2e6; border: 1px solid #dee2e6 !important; background-color: #007bff; color: #fff'>담당자 이름</th>
<td style='padding: .75rem; vertical-align: top; border-top: 1px solid #dee2e6; border: 1px solid #dee2e6 !important'>{{contactName}} {{contactPosition}}</td>
</tr><tr>
<th style='padding: .75rem; vertical-align: top; border-top: 1px solid #dee2e6; border: 1px solid #dee2e6 !important; background-color: #007bff; color: #fff'>담당자 부서</th>
<td style='padding: .75rem; vertical-align: top; border-top: 1px solid #dee2e6; border: 1px solid #dee2e6 !important'>{{contactDept}}</td>
</tr><tr>
<th style='padding: .75rem; vertical-align: top; border-top: 1px solid #dee2e6; border: 1px solid #dee2e6 !important; background-color: #007bff; color: #fff'>담당자 연락처</th>
<td style='padding: .75rem; vertical-align: top; border-top: 1px solid #dee2e6; border: 1px solid #dee2e6 !important'>{{contactPhone}} / {{contactEmail}}</td>
</tr><tr>
<th style='padding: .75rem; vertical-align: top; border-top: 1px solid #dee2e6; border: 1px solid #dee2e6 !important; background-color: #007bff; color: #fff'>직무</th>
<td style='padding: .75rem; vertical-align: top; border-top: 1px solid #dee2e6; border: 1px solid #dee2e6 !important'>{{jobDesc}}</td>
</tr>
<tr>
<th style='padding: .75rem; vertical-align: top; border-top: 1px solid #dee2e6; border: 1px solid #dee2e6 !important; background-color: #007bff; color: #fff'>자격요건</th>
<td style='padding: .75rem; vertical-align: top; border-top: 1px solid #dee2e6; border: 1px solid #dee2e6 !important'>{{contents}}</td>
</tr>
</tbody></table>
<span style='color:red'>※ 프로젝트 상세 정보는 Univision에서 확인 가능 합니다.</span><br />
<a href='{{url}}' target='_blank'>Go Univision</a></body></html> ";
        }
    }
}