using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;

namespace Univision.WeeklyAppointment
{
  class Program
  {
    static void Main(string[] args)
    {
      try
      {
        Console.Write("=========================================\n");
        Console.Write("전체 CRT메일 발송 \n");

        SmtpClient sc = new SmtpClient("smtp-mail.outlook.com", 25);
        NetworkCredential credentials = new NetworkCredential("laptop_c@unicosearch.com", "unico11$");
        sc.Credentials = credentials;
        sc.EnableSsl = true;
        MailMessage msg = new MailMessage();



        msg.From = new MailAddress("noreply@unicosearch.com", "UnicoSearch");
        msg.To.Add(new MailAddress("unicousers@unicosearch.com"));
        msg.Subject = "[알림] 2시부터 CRT(집중 Call Time) 입니다~";
        msg.Body = @"매주 화요일 오후 2시부터 CRT(Client Relationship Time) 입니다.

* 무엇을? Consultant는 Dormant 고객에게 전화, Associate
Consultant는 추천한 후보자에게 사후관리 콜 

* 어떻게? 매주 같은 시간에 집중적으로 Client와 Candidate에게 전화 

* 왜? Consultant 는 단절된 고객과의 관계를 돈독히 하고 Job
Order를 받을 기회도 살리며 각종 정보 수집 기회. 

Associate Consultant는 추천한 후보자에게 사후관리를 하고
있다는 의미도 심어주고 우수 인재 추천 받을 기회로도 활용.

* 언제? 매주 화요일 오후 2시~3시 

* 누가? Consultant 와 Associate Consultant 전원 
";


        string alt_desc = @"<html xmlns:v='urn:schemas-microsoft-com:vml' 
	xmlns:o='urn:schemas-microsoft-com:office:office' xmlns:w='urn:schemas-mic
	rosoft-com:office:word' xmlns:m='http://schemas.microsoft.com/office/2004/
	12/omml' xmlns='http://www.w3.org/TR/REC-html40'><head><meta http-equiv=Co
	ntent-Type content='text/html\; charset=ks_c_5601-1987'><meta name=Generat
	or content='Microsoft Word 15 (filtered medium)'><style><!--\n/* Font Defi
	nitions */\n@font-face\n	{font-family:굴림\;\n	panose-1:2 11 6 0 0 1 1 1
	 1 1\;}\n@font-face\n	{font-family:'Cambria Math'\;\n	panose-1:2 4 5 3 5 4
	 6 3 2 4\;}\n@font-face\n	{font-family:'맑은 고딕'\;\n	panose-1:2 11 5
	 3 2 0 0 2 0 4\;}\n@font-face\n	{font-family:'\\@맑은 고딕'\;}\n@font-
	face\n	{font-family:'\\@굴림'\;\n	panose-1:2 11 6 0 0 1 1 1 1 1\;}\n/* S
	tyle Definitions */\np.MsoNormal\, li.MsoNormal\, div.MsoNormal\n	{margin:
	0cm\;\n	text-align:justify\;\n	text-justify:inter-ideograph\;\n	text-autos
	pace:none\;\n	word-break:break-hangul\;\n	font-size:10.0pt\;\n	font-family
	:'맑은 고딕'\;}\nspan.EmailStyle18\n	{mso-style-type:personal-compose\
	;\n	font-family:'맑은 고딕'\;\n	color:windowtext\;}\n.MsoChpDefault\n	
	{mso-style-type:export-only\;}\n@page WordSection1\n	{size:612.0pt 792.0pt
	\;\n	margin:3.0cm 72.0pt 72.0pt 72.0pt\;}\ndiv.WordSection1\n	{page:WordSe
	ction1\;}\n--></style><!--[if gte mso 9]><xml>\n<o:shapedefaults v:ext='ed
	it' spidmax='1026' />\n</xml><![endif]--><!--[if gte mso 9]><xml>\n<o:shap
	elayout v:ext='edit'>\n<o:idmap v:ext='edit' data='1' />\n</o:shapelayout>
	</xml><![endif]--></head><body lang=KO link='#0563C1' vlink='#954F72' styl
	e='word-wrap:break-word'><div class=WordSection1><p class=MsoNormal style=
	'margin-bottom:12.0pt'><span style='font-size:12.0pt\;font-family:굴림'>
	매주 화요일 오후<span lang=EN-US> 2</span>시부터<span lang=EN-US
	> CRT(Client Relationship Time) </span>입니다<span lang=EN-US>.</span><
	/span><span lang=EN-US><br><br>* </span>무엇을<span lang=EN-US>? Consul
	tant</span>는<span lang=EN-US> Dormant </span>고객에게 전화<span la
	ng=EN-US>\, Associate<br>Consultant</span>는 추천한 후보자에게 
	사후관리 콜<span lang=EN-US> <br><br>* </span>어떻게<span lang=EN-
	US>? </span>매주 같은 시간에 집중적으로<span lang=EN-US> Clien
	t</span>와<span lang=EN-US> Candidate</span>에게 전화<span lang=EN-US
	> <br><br>* </span>왜<span lang=EN-US>? Consultant </span>는 단절된 
	고객과의 관계를 돈독히 하고<span lang=EN-US> Job<br>Order</spa
	n>를 받을 기회도 살리며 각종 정보 수집 기회<span lang=EN-
	US>. <br><br>Associate Consultant</span>는 추천한 후보자에게 사
	후관리를 하고<span lang=EN-US><br></span>있다는 의미도 심어
	주고 우수 인재 추천 받을 기회로도 활용<span lang=EN-US>.<b
	r><br>* </span>언제<span lang=EN-US>? </span>매주 화요일 오후<spa
	n lang=EN-US> 2</span>시<span lang=EN-US>~3</span>시<span lang=EN-US> <b
	r><br>* </span>누가<span lang=EN-US>? Consultant </span>와<span lang=EN
	-US> Associate Consultant </span>전원 <span lang=EN-US><o:p></o:p></span
	></p><p class=MsoNormal><span lang=EN-US><o:p>&nbsp\;</o:p></span></p><p c
	lass=MsoNormal><span lang=EN-US><o:p>&nbsp\;</o:p></span></p><p class=MsoN
	ormal><span lang=EN-US><o:p>&nbsp\;</o:p></span></p></div></body></html>";

        string desc = @"매주 화요일 오후 2시부터 CRT(Client Relationship Time
	) 입니다.\n\n* 무엇을? Consultant는 Dormant 고객에게 전화\, A
	ssociate\nConsultant는 추천한 후보자에게 사후관리 콜 \n\n* 
	어떻게? 매주 같은 시간에 집중적으로 Client와 Candidate에
	게 전화 \n\n* 왜? Consultant 는 단절된 고객과의 관계를 돈
	독히 하고 Job\nOrder를 받을 기회도 살리며 각종 정보 수
	집 기회. \n\nAssociate Consultant는 추천한 후보자에게 사후
	관리를 하고\n있다는 의미도 심어주고 우수 인재 추천 
	받을 기회로도 활용.\n\n* 언제? 매주 화요일 오후 2시~3시
	 \n\n* 누가? Consultant 와 Associate Consultant 전원";

        var start_time = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 14, 00, 00);
        var end_time = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 15, 00, 00);

        string start_str = Convert.ToDateTime(start_time).ToString("yyyyMMddTHHmmss");
        string end_str = Convert.ToDateTime(end_time).ToString("yyyyMMddTHHmmss");

        StringBuilder str = new StringBuilder();
        str.AppendLine("BEGIN:VCALENDAR");

        //PRODID: identifier for the product that created the Calendar object
        str.AppendLine("PRODID:-//Microsoft Corporation//Outlook 16.0 MIMEDIR//EN");
        str.AppendLine("VERSION:2.0");
        str.AppendLine("METHOD:REQUEST");
        str.AppendLine("X-MS-OLK-FORCEINSPECTOROPEN:TRUE");
        str.AppendLine("BEGIN:VTIMEZONE");
        str.AppendLine("TZID: Korea Standard Time");
        str.AppendLine("BEGIN:STANDARD");
        str.AppendLine("DTSTART:16010101T000000");
        
        str.AppendLine("TZOFFSETFROM:+0900");
        str.AppendLine("TZOFFSETTO: +0900");
        str.AppendLine("END: STANDARD");
        str.AppendLine("END:VTIMEZONE");
        str.AppendLine("BEGIN:VEVENT");
        str.AppendLine(string.Format("ATTENDEE;CN=\"{0}\";RSVP=FALSE:mailto:{1}", "UnicoSearch", "unicousers@unicosearch.com"));
        str.AppendLine("CLASS:PUBLIC");
        str.AppendLine(string.Format("DESCRIPTION:{0}", desc));
        str.AppendLine(string.Format("DTEND;TZID=\"Korea Standard Time\":{0:yyyyMMddTHHmmss}", end_str));//TimeZoneInfo.ConvertTimeToUtc("EndTime").ToString("yyyyMMddTHHmmssZ")));
        str.AppendLine(string.Format("DTSTART;TZID=\"Korea Standard Time\":{0:yyyyMMddTHHmmss}", start_str));//TimeZoneInfo.ConvertTimeToUtc("BeginTime").ToString("yyyyMMddTHHmmssZ")));
        //str.AppendLine(string.Format("DTSTAMP:{0:yyyyMMddTHHmmssZ}", DateTime.UtcNow));        
        str.AppendLine(string.Format("LOCATION: {0}", "(주)유니코써치"));
        str.AppendLine(string.Format("ORGANIZER;CN=\"UnicoSearch\":MAILTO:{0}", "noreply@unicosearch.com"));
        str.AppendLine("PRIORITY:5");
        str.AppendLine("SEQUENCE:0");
        str.AppendLine(string.Format("SUMMARY;LANGUAGE=ko:{0}", msg.Subject));
        str.AppendLine(string.Format("UID:{0}", Guid.NewGuid()));
        str.AppendLine("TRANSP:OPAQUE");
        str.AppendLine(string.Format("X-ALT-DESC;FMTTYPE=text/html:{0}", alt_desc));
        str.AppendLine("X-MICROSOFT-CDO-BUSYSTATUS:TENTATIVE");
        str.AppendLine("X-MICROSOFT-CDO-IMPORTANCE:1");
        str.AppendLine("X-MICROSOFT-CDO-INTENDEDSTATUS:BUSY");
        str.AppendLine("X-MICROSOFT-DISALLOW-COUNTER:TRUE");
        str.AppendLine("X-MS-OLK-AUTOSTARTCHECK:FALSE");
        str.AppendLine("X-MS-OLK-CONFTYPE:0");

        //str.AppendLine("STATUS:CONFIRMED");
        str.AppendLine("BEGIN:VALARM");
        str.AppendLine("TRIGGER:-PT0M");
        str.AppendLine("ACTION:DISPLAY");
        str.AppendLine("DESCRIPTION:Reminder");
        //str.AppendLine("X-MICROSOFT-CDO-BUSYSTATUS:BUSY");
        str.AppendLine("END:VALARM");
        str.AppendLine("END:VEVENT");
        str.AppendLine("END:VCALENDAR");
        System.Net.Mime.ContentType ct = new System.Net.Mime.ContentType("text/calendar");
        ct.Parameters.Add("method", "REQUEST");
        ct.Parameters.Add("name", "meeting.ics");
        AlternateView avCal = AlternateView.CreateAlternateViewFromString(str.ToString(), ct);
        msg.AlternateViews.Add(avCal);
        sc.DeliveryMethod = SmtpDeliveryMethod.Network;

        //sc.EnableSsl = true;
        sc.Send(msg);

        
        Console.Write(msg.Subject);
      }
      catch (Exception ex)
      {

        throw ex;
      }


    }
  }
}
