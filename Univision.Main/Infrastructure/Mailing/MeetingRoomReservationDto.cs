using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Univision.Main.Infrastructure.Mailing
{
    public class MeetingRoomReservationDto : MailDto
    {

        
        public string newedit { get; set; }
        /// <summary>
        /// 이용구분
        /// </summary>
        public string useType { get; set; }

        /// <summary>
        /// 시작 날짜
        /// </summary>
        public string dateStr { get; set; }

        /// <summary>
        /// 시작 시간
        /// </summary>
        public string startTime { get; set; }

        /// <summary>
        /// 종료 시간
        /// </summary>
        public string endTime { get; set; }

        /// <summary>
        /// 신청자명
        /// </summary>
        public string requestName { get; set; }

        /// <summary>
        /// 회의실 명
        /// </summary>
        public string resourceName { get; set; }

        /// <summary>
        /// 노트북 Y/N
        /// </summary>
        public string laptop { get; set; }

        /// <summary>
        /// 내용
        /// </summary>
        public string comment { get; set; }

        /// <summary>
        /// Return Url
        /// </summary>
        public string url { get; set; }
    }


    public class MeetingRoomReservationTemplete : TempleteDto
    {
        public MeetingRoomReservationTemplete()
        {
            this.MailSubject = "[Univision] 회의실 사용{{newedit}} 일정 안내 : [{{useType}}] / {{dateStr}} / {{startTime}} ~ {{endTime}}";
            this.MailBody = @" <html> <body style='width:500px;'> <table style='border-collapse:collapse; width:80%; margin-bottom: 1rem; color:#212529; font-size:12px;'> <tbody> <tr>
<th style='padding: .75rem; vertical-align: top; border-top: 1px solid #dee2e6; border: 1px solid #dee2e6 !important; background-color: #007bff; color: #fff'>신청자</th>
<td style='padding: .75rem; vertical-align: top; border-top: 1px solid #dee2e6; border: 1px solid #dee2e6 !important'>{{requestName}}</td>
</tr><tr>
<th style='padding: .75rem; vertical-align: top; border-top: 1px solid #dee2e6; border: 1px solid #dee2e6 !important; background-color: #007bff; color: #fff'>회의실</th>
<td style='padding: .75rem; vertical-align: top; border-top: 1px solid #dee2e6; border: 1px solid #dee2e6 !important'>{{resourceName}}</td>
</tr><tr>
<th style='padding: .75rem; vertical-align: top; border-top: 1px solid #dee2e6; border: 1px solid #dee2e6 !important; background-color: #007bff; color: #fff'>이용일자</th>
<td style='padding: .75rem; vertical-align: top; border-top: 1px solid #dee2e6; border: 1px solid #dee2e6 !important'>{{dateStr}}</td>
</tr><tr>
<th style='padding: .75rem; vertical-align: top; border-top: 1px solid #dee2e6; border: 1px solid #dee2e6 !important; background-color: #007bff; color: #fff'>시간</th>
<td style='padding: .75rem; vertical-align: top; border-top: 1px solid #dee2e6; border: 1px solid #dee2e6 !important'>{{startTime}} ~ {{endTime}}</td>
</tr><tr>
<th style='padding: .75rem; vertical-align: top; border-top: 1px solid #dee2e6; border: 1px solid #dee2e6 !important; background-color: #007bff; color: #fff'>노트북 사용</th>
<td style='padding: .75rem; vertical-align: top; border-top: 1px solid #dee2e6; border: 1px solid #dee2e6 !important'>{{laptop}}</td>
</tr><tr>
<th style='padding: .75rem; vertical-align: top; border-top: 1px solid #dee2e6; border: 1px solid #dee2e6 !important; background-color: #007bff; color: #fff'>이용구분</th>
<td style='padding: .75rem; vertical-align: top; border-top: 1px solid #dee2e6; border: 1px solid #dee2e6 !important'>{{useType}}</td>
</tr><tr>
<th style='padding: .75rem; vertical-align: top; border-top: 1px solid #dee2e6; border: 1px solid #dee2e6 !important; background-color: #007bff; color: #fff'>내용</th>
<td style='padding: .75rem; vertical-align: top; border-top: 1px solid #dee2e6; border: 1px solid #dee2e6 !important'>{{comment}}</td>
</tr>
</tbody></table>
<span style='color:red'>※ 예약 내역은 Univision에서 확인 가능 합니다.</span><br />
<a href='{{url}}' target='_blank'>Go Univision</a></body></html> ";
        }
    }
}