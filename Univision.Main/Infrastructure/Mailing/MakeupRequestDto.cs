using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Univision.Main.Infrastructure.Mailing
{
    public class MakeupRequestDto : MailDto
    {
        /// <summary>
        /// 메일 제목
        /// </summary>
        public string title { get; set; }

        public string name { get; set; }

        public string candidateurl { get; set; }

        public string candidatename { get; set; }
        public string filelang { get; set; }

        public string fileurl { get; set; }

        public string filename { get; set; }

        public string projecturl { get; set; }
        public string pjttitle { get; set; }
        
    }

    public class MakeupRequesTemplete : TempleteDto
    {
        public MakeupRequesTemplete()
        {
            this.MailSubject = "[Univision] {{title}}";
            this.MailBody = @" <html> <body style='width:500px;'> <table style='border-collapse:collapse; width:80%; margin-bottom: 1rem; color:#212529; font-size:12px;'> <tbody> <tr>
<th style='padding: .75rem; vertical-align: top; border-top: 1px solid #dee2e6; border: 1px solid #dee2e6 !important; background-color: #007bff; color: #fff'>신청자</th>
<td style='padding: .75rem; vertical-align: top; border-top: 1px solid #dee2e6; border: 1px solid #dee2e6 !important'><br/>{{name}}<br/>&nbsp;</td>
</tr>
<tr>
<th style='padding: .75rem; vertical-align: top; border-top: 1px solid #dee2e6; border: 1px solid #dee2e6 !important; background-color: #007bff; color: #fff'>후보자정보</th>
<td style='padding: .75rem; vertical-align: top; border-top: 1px solid #dee2e6; border: 1px solid #dee2e6 !important'>
<br/>
＊후보자명 : <a href='{{candidateurl}}'>{{candidatename}}</a><br/><br/>
＊이력서종류: {{filelang}}<br/><br/>
＊이력서: <a href='{{fileurl}}'>{{filename}}</a><br/>&nbsp;
</td>
</tr>
<tr>
<th style='padding: .75rem; vertical-align: top; border-top: 1px solid #dee2e6; border: 1px solid #dee2e6 !important; background-color: #007bff; color: #fff'>프로젝트 정보</th>
<td style='padding: .75rem; vertical-align: top; border-top: 1px solid #dee2e6; border: 1px solid #dee2e6 !important'>
<br/>
＊프로젝트명 : <a href='{{projecturl}}'>{{pjttitle}}</a><br/>&nbsp;
</td>
</tr>
</tbody></table>";
        }
    }

    public class MakeupCancelDto : MailDto
    {
        /// <summary>
        /// 메일 제목
        /// </summary>
        public string title { get; set; }

        public string name { get; set; }

        public string candidatename { get; set; }
        public string filelang { get; set; }

        public string candidateurl { get; set; }

    }
    public class MakeupCancelTemplete : TempleteDto
    {
        public MakeupCancelTemplete()
        {
            this.MailSubject = "[Univision] {{title}}";
            this.MailBody = @" <html> <body style='width:500px;'> <table style='border-collapse:collapse; width:80%; margin-bottom: 1rem; color:#212529; font-size:12px;'> <tbody> <tr>
<th style='padding: .75rem; vertical-align: top; border-top: 1px solid #dee2e6; border: 1px solid #dee2e6 !important; background-color: #007bff; color: #fff'>취소 요청자</th>
<td style='padding: .75rem; vertical-align: top; border-top: 1px solid #dee2e6; border: 1px solid #dee2e6 !important'><br/>{{name}}<br/>&nbsp;</td>
</tr>
<tr>
<th style='padding: .75rem; vertical-align: top; border-top: 1px solid #dee2e6; border: 1px solid #dee2e6 !important; background-color: #007bff; color: #fff'>후보자정보</th>
<td style='padding: .75rem; vertical-align: top; border-top: 1px solid #dee2e6; border: 1px solid #dee2e6 !important'>
<br/>
＊후보자명 : <a href='{{candidateurl}}'>{{candidatename}}</a><br/><br/>
＊이력서종류: {{filelang}}<br/><br/>
</td>
</tr>
</tbody></table>";
        }
    }
}