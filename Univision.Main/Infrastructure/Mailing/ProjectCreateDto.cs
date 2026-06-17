using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Univision.Main.Infrastructure.Mailing
{
    public class ProjectCreateDto : MailDto
    {
        /// <summary>
        /// 등록, 수정 타입.
        /// </summary>
        public string type { get; set; }

        /// <summary>
        /// 프로젝트 제목
        /// </summary>
        public string title { get; set; }

        /// <summary>
        /// 고객사명
        /// </summary>
        public string companyName { get; set; }

        /// <summary>
        /// 프로젝트 타입
        /// </summary>
        public string pjtType { get; set; }

        /// <summary>
        /// AM 목록
        /// </summary>
        public string amNames { get; set; }

        /// <summary>
        /// Searcher 목록
        /// </summary>
        public string searcherNames { get; set; }

        /// <summary>
        /// 리턴 URL
        /// </summary>
        public string url { get; set; }

        /// <summary>
        /// 등록일
        /// </summary>
        public string createDt { get; set; }

    }

    public class ProjectCreateTemplete : TempleteDto
    {
        public ProjectCreateTemplete()
        {
            this.MailSubject = "[Univision] 프로젝트 {{type}} 안내 : [{{title}}]";
            this.MailBody = @" <html> <body style='width:500px;'> <table style='border-collapse:collapse; width:80%; margin-bottom: 1rem; color:#212529; font-size:12px;'> <tbody> <tr>
<th style='padding: .75rem; vertical-align: top; border-top: 1px solid #dee2e6; border: 1px solid #dee2e6 !important; background-color: #007bff; color: #fff'>고객사명</th>
<td style='padding: .75rem; vertical-align: top; border-top: 1px solid #dee2e6; border: 1px solid #dee2e6 !important'>{{companyName}}</td>
</tr><tr>
<th style='padding: .75rem; vertical-align: top; border-top: 1px solid #dee2e6; border: 1px solid #dee2e6 !important; background-color: #007bff; color: #fff'>제목</th>
<td style='padding: .75rem; vertical-align: top; border-top: 1px solid #dee2e6; border: 1px solid #dee2e6 !important'>{{title}}</td>
</tr><tr>
<th style='padding: .75rem; vertical-align: top; border-top: 1px solid #dee2e6; border: 1px solid #dee2e6 !important; background-color: #007bff; color: #fff'>구분</th>
<td style='padding: .75rem; vertical-align: top; border-top: 1px solid #dee2e6; border: 1px solid #dee2e6 !important'>{{pjtType}}</td>
</tr><tr>
<th style='padding: .75rem; vertical-align: top; border-top: 1px solid #dee2e6; border: 1px solid #dee2e6 !important; background-color: #007bff; color: #fff'>AM</th>
<td style='padding: .75rem; vertical-align: top; border-top: 1px solid #dee2e6; border: 1px solid #dee2e6 !important'>{{amNames}}</td>
</tr><tr>
<th style='padding: .75rem; vertical-align: top; border-top: 1px solid #dee2e6; border: 1px solid #dee2e6 !important; background-color: #007bff; color: #fff'>SM</th>
<td style='padding: .75rem; vertical-align: top; border-top: 1px solid #dee2e6; border: 1px solid #dee2e6 !important'>{{searcherNames}}</td>
</tr><tr>
<th style='padding: .75rem; vertical-align: top; border-top: 1px solid #dee2e6; border: 1px solid #dee2e6 !important; background-color: #007bff; color: #fff'>{{type}}일</th>
<td style='padding: .75rem; vertical-align: top; border-top: 1px solid #dee2e6; border: 1px solid #dee2e6 !important'>{{createDt}}</td>
</tr>
</tbody></table>
<span style='color:red'>※ 프로젝트 상세 정보는 Univision에서 확인 가능 합니다.</span><br />
<a href='{{url}}' target='_blank'>Go Univision</a></body></html> ";
        }
    }
}