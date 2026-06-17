using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Univision.Main.Infrastructure.Mailing
{
    public class ShareCandidateDto : MailDto
    {
        /// <summary>
        /// 메일 제목
        /// </summary>
        public string title { get; set; }

        /// <summary>
        /// 신청자 이름
        /// </summary>
        public string name { get; set; }

        /// <summary>
        /// 요청일
        /// </summary>
        public string createDate { get; set; }

        /// <summary>
        /// 내용
        /// </summary>
        public string Comment { get; set; }


        /// <summary>
        /// Return url
        /// </summary>
        public string url { get; set; }

    }

    public class ShareCandidateTemplete : TempleteDto
    {
        public ShareCandidateTemplete()
        {
            this.MailSubject = "[후보추천드려요] {{title}}";
            this.MailBody = @" 
<html> 
    <body style='width:80%;'> 
        <table style='border-collapse:collapse; width:100%; margin-bottom: 1rem; color:#212529; font-size:12px;'> 
            <tbody> 
                <tr>
                    <th style='width:100px;padding:10px; vertical-align: top; border-top: 1px solid #dee2e6; border: 1px solid #dee2e6 !important; background-color: #ccc; color: #111'>작성자</th>
                    <td style='padding:10px; vertical-align: top; border-top: 1px solid #dee2e6; border: 1px solid #dee2e6 !important'>{{name}}</td>
</tr><tr>
<th style='padding:10px; vertical-align: top; border-top: 1px solid #dee2e6; border: 1px solid #dee2e6 !important; background-color: #ccc; color: #111'>작성일</th>
<td style='padding:10px; vertical-align: top; border-top: 1px solid #dee2e6; border: 1px solid #dee2e6 !important'>{{createDate}}</td>
</tr><tr>
<th style='padding:10px; vertical-align: top; border-top: 1px solid #dee2e6; border: 1px solid #dee2e6 !important; background-color: #ccc; color: #111'>내용</th>
<td style='padding:10px; vertical-align: top; border-top: 1px solid #dee2e6; border: 1px solid #dee2e6 !important'>{{Comment}}</td>
</tr>
</tbody></table>
<span style='color:red'>※ 상세정보는 Univision [후보 추천드려요] 메뉴에서 확인 가능 합니다.</span><br />
<a href='{{url}}' target='_blank'>Go Univision</a></body></html> ";
        }
    }
}