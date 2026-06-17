
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Univision.Core.Models.DTO;
using Univision.Core.Repositories;


namespace Univision.WeeklyMail
{
    class Program
    {
        static string GetDateTimeToString(DateTime dt)
        {
            return dt != null ? dt.ToString("yyyy-MM-dd") : string.Empty;
        }

        static void Main(string[] args)
        {

            try
            {
                //기준일 설정 : 월요일
                DateTime input = DateTime.UtcNow.AddHours(9);
                int delta = DayOfWeek.Monday - input.DayOfWeek;
                //if (delta > 0)
                //    delta -= 7;
                DateTime monday = input.AddDays(delta);

                string base_date = GetDateTimeToString(monday);

                if (String.IsNullOrEmpty(base_date))
                    throw new Exception("Base date doesn't Set");

                //데이터 작성
                Console.Write("Start : Create Mail Data - " + base_date + "\n");
                MailReportRepository mrr = new MailReportRepository();

                Console.Write("Running..." + "\n");
                string rtn_cd = mrr.CreateReportData(base_date, 0, "", 0);
                //string rtn_cd = "OK";
                Console.Write("Create Data Result :" + rtn_cd + "\n");

                if (rtn_cd != "OK")
                    throw new Exception("Data Creation Error");

                //생성 데이터 조회

                
                var data = mrr.SelectMailReportMstList(base_date);
                //LOOP를 돌려서 메일 제목, 내용 조합 하여 DB에 저장
                if(data.Count() > 0)
                {
                    MailService report_mail = new MailService();
                    int idx = 0;
                    foreach (var report in data)
                    {
                        if (idx++ >= 3)
                        {
                            idx = 1;
                        }
                        try
                        {
                            Console.Write("=========================================\n");
                            Console.Write(report.uv_name + "\n");

                            var mr_last_list = mrr.SelectMailReportLastList(report.mr_idx);
                            var mr_this_list = mrr.SelectMailReportThisList(report.mr_idx);
                            var mr_pjt_list = mrr.SelectMailReportProjectList(report.mr_idx);

                            TempleteDto wt = new TempleteDto();
                            wt.MailSubject = String.Format("주간 유니비전 리포트({0}) [{1} : {2}]", base_date, report.ud_name, report.uv_name);
                            wt.MailBody = CreateMailForm(wt.MailSubject, report, mr_last_list, mr_this_list, mr_pjt_list);

                            List<string> ToArr = new List<string>();
                            List<string> ccArr = new List<string>();
                            List<string> bccArr = new List<string>();
                            
                            Console.Write(" TO : " + report.to_addr + "\n");
                            
                            ToArr.Add(report.to_addr);
                            if (report.is_leader == 5 && !string.IsNullOrEmpty(report.cc_addr))
                            {
                                Console.Write("CC : " + report.cc_addr + "\n");
                                ccArr.Add(report.cc_addr);
                                
                            }
                            
                            //PM그룹은 bcc에 대표님 포함해야함 
                            
                            if(!String.IsNullOrEmpty(report.ud_name) && report.ud_name.Substring(0, 2) == "PM")
                            {
                                bccArr.Add("ivykim@unicosearch.com");
                            }
                            
                            //ToArr.Add("lee.hc@unicosearch.com");
                            //ccArr.Add("lee.hc@unicosearch.com");
                            //bccArr.Add("lee.hc@unicosearch.com");
                            

                            MailDto md = new MailDto()
                            {
                                ToArr = ToArr.ToArray()
                                ,
                                CCArr = ccArr.ToArray()
                                ,
                                BccArr = bccArr.ToArray()
                                ,
                                From = new System.Net.Mail.MailAddress("noreply@unicosearch.com", "noreply")
                            };

                            var result = report_mail.SendReportMail(md, wt);

                            Console.Write(wt.MailSubject +" // " + result.message + "\n");
                            //Delay(5000);
                        }
                        catch (Exception ex)
                        {

                            throw ex;
                        }
                        
                    }

                }


                //Loop를 돌려 메일 발송

                Console.Write("End");
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        private static DateTime Delay(int MS)
        {
            DateTime ThisMoment = DateTime.Now;
            TimeSpan duration = new TimeSpan(0, 0, 0, 0, MS);
            DateTime AfterWards = ThisMoment.Add(duration);

            while (AfterWards >= ThisMoment)
            {
                //System.Windows.Forms.Application.DoEvents();
                ThisMoment = DateTime.Now;
            }

            return DateTime.Now;
        }

        static string CreateMailForm(string title, mail_report_mst mrm, List<mail_report_last_list> mrll, List<mail_report_this_list> mrtl, List<mail_report_pjt_list> mrpl)
        {
            try
            {
                string univision_url = "http://univision.unicosearch.com/";

                StringBuilder sb = new StringBuilder();
                sb.Append("<html>");
                sb.Append("<head>");
                sb.Append("    <title> ::: " + title + "</title>");
                sb.Append("    <meta http-equiv='Content-Type' content='text/html; charset=UTF-8'>");
                sb.Append("    <style>");
                sb.Append("        TD {");
                sb.Append("            font-size: 9pt;");
                sb.Append("            font-family: '맑은 고딕';");
                sb.Append("            color: #333333;");
                sb.Append("            line-height: 12pt;");
                sb.Append("        }");
                sb.Append("    </style>");
                sb.Append("</head>");
                sb.Append("<body topmargin='10' leftmargin='10' marginheight='10' marginwidth='10'>");
                /*
                sb.Append("안녕하세요, 경영지원팀 이현창 입니다.<br/>");
                sb.Append("<br/>");
                sb.Append("현재 유니비전은 많은 분들의 의견을 바탕으로 편의성, 정확성, 정보의 다양성 면에서 정성, 정량적인 개선작업이 진행되었습니다.<br/>");
                sb.Append("업무의 효율성을 높이고자 유니비전을 통해 프로젝트, 고객사, 후보자 업데이트 현황을 weekly로 관리하고 진행하실 수 있도록 향후 매 월요일마다 아래와 같은 이메일 리포트를 발송드릴 예정입니다.<br/>");
                sb.Append("개개인의 주간 업무 현황을 쉽게 파악하고 active한 프로젝트 파악을 통해 업무의 우선순위를 한 눈에 정리할 수 있도록 지원하고자 하는 목적으로 개발된 기능이니 활용해 보시고 추가적인 개선 의견이나 불편사항을 전달주시면 반영하도록 하겠습니다.<br/>");
                sb.Append("<br/>");
                sb.Append("고맙습니다.<br/><hr>");
                */


                sb.Append("    <table width='750' border='0' cellspacing='1' cellpadding='0' bgcolor='#cccccc'>");
                sb.Append("        <tr>");
                sb.Append("            <td style='padding:10px' bgcolor='#ffffff'>");
                sb.Append("                <table width='100%' border='0' cellspacing='0' cellpadding='0' bgcolor='#93C9E3'>");
                sb.Append("                    <tr height='30'>");
                sb.Append("                        <td width='10' bgcolor='#037ABC' nowrap></td>");
                sb.Append("                        <td width='2' bgcolor='#ffffff' nowrap></td>");
                sb.Append("                        <td width='100%' style='padding-left:10px;font-size:12pt;'><b>" + title + "</b></td>");
                sb.Append("                    </tr>");
                sb.Append("                </table>");
                sb.Append("                <table height='10' border='0' cellspacing='0' cellpadding='0'><tr><td>&nbsp;</td></tr></table>");

                sb.Append("                <div style='font-size:11pt;'><b>[지난주 요약 (Last week)]<b></div>");
                sb.Append("                <table width='100%' border='0' cellspacing='1' cellpadding='0' bgcolor='#cccccc'>");
                sb.Append("                    <tr>");
                sb.Append("                        <td height='40' style='padding:10 20 10 20' bgcolor='#ffffff'>");

                var new_inv_cnt = mrll.Where(x => x.gubun == "인보이스");
                var new_pjt_cnt = mrll.Where(x => x.gubun == "신규 프로젝트");
                var new_rec_cnt = mrll.Where(x => x.gubun == "후보자 추천");
                if (new_inv_cnt.Count() > 0)
                {
                    foreach (var ll in new_inv_cnt)
                    {
                        sb.Append("                            <b>[인보이스 발행] : </b>" + ll.gubun_cnt.ToString() + " 건<br>");
                    }
                }
                else
                {
                    sb.Append("                            <b>[인보이스 발행] : </b> 0 건<br>");
                }

                if (new_pjt_cnt.Count() > 0)
                {
                    foreach (var ll in new_pjt_cnt)
                    {
                        sb.Append("                            <b>[신규 프로젝트] : </b>" + ll.gubun_cnt.ToString() + " 건<br>");
                    }
                }
                else
                {
                    sb.Append("                            <b>[신규 프로젝트] : </b> 0 건<br>");
                }
                if (new_rec_cnt.Count() > 0)
                {
                    foreach (var ll in new_rec_cnt)
                    {
                        sb.Append("                            <b>[후보자 추천] : </b>" + ll.gubun_cnt.ToString() + " 명<br>");
                    }
                }
                else
                {
                    sb.Append("                            <b>[후보자 추천] : </b> 0 명<br>");
                }
                    
                sb.Append("                        </td>");
                sb.Append("                    </tr>");
                sb.Append("                </table>");
                sb.Append("                <table height='10' border='0' cellspacing='0' cellpadding='0'><tr><td>&nbsp;</td></tr></table>");

                sb.Append("                <div style='font-size:11pt;'><b>[이번주 요약(This week)]<b></div>");
                sb.Append("                <table width='100%' border='0' cellspacing='1' cellpadding='0' bgcolor='#cccccc'>");
                sb.Append("                    <tr>");
                sb.Append("                        <td height='40' style='padding:10 20 10 20' bgcolor='#ffffff'>");
                if (mrtl.Count() > 0)
                {
                    
                    var join = mrtl.Where(x => x.gubun == "입사확정").OrderBy(x => x.schedule_date);
                    var interview = mrtl.Where(x => x.gubun == "면접").OrderBy(x => x.schedule_date);
                    var grt = mrtl.Where(x => x.gubun == "보증기간").OrderBy(x => x.schedule_date);
                    sb.Append("                <div style='font-size:10pt;'><b>[입사 예정자]<b> : " + mrm.new_join_cnt.ToString() + " 명</div>");
                    if (join.Count() > 0)
                    {
                        //sb.Append("                <div style='font-size:10pt;'><b>[입사 예정자]<b> : " + mrm.new_join_cnt.ToString() +" 명</div>");
                        int seq = 0;
                        foreach (var ll in join)
                        {
                            if (seq++ > 0)
                                sb.Append("                            <table height='5' border='0' cellspacing='0' cellpadding='0'><tr><td></td></tr></table>");
                            sb.Append("                            <span style='padding-left:10px;'>&nbsp;&nbsp;&nbsp;&nbsp;- <b>[<a href='" + univision_url+"Candidate/CandidateDetail?c_seq=" + ll.c_seq.ToString()+ "'>"+ll.can_name+"</a>] 님: </b> [" + GetDateTimeToString(ll.schedule_date.Value) + "] " + ll.gubun + " - &lt;" + ll.client_name + "&gt;</span><br>");
                        }
                        
                    }
                    sb.Append("                <table height='5' border='0' cellspacing='0' cellpadding='0'><tr><td></td></tr></table>");
                    sb.Append("                <div style='font-size:10pt;'><b>[보증기간만료]<b> : " + mrm.new_grt_cnt.ToString() + " 명</div>");
                    if (grt.Count() > 0)
                    {
                        int seq = 0;
                        foreach (var ll in grt)
                        {
                            if (seq++ > 0)
                                sb.Append("                            <table height='5' border='0' cellspacing='0' cellpadding='0'><tr><td></td></tr></table>");
                            sb.Append("                            <span style='padding-left:10px;'>&nbsp;&nbsp;&nbsp;&nbsp;- <b><b style='padding-left:10px;'>[<a href='" + univision_url + "Candidate/CandidateDetail?c_seq=" + ll.c_seq.ToString() + "'>" + ll.can_name + "</a>] 님: </b> [" + GetDateTimeToString(ll.schedule_date.Value) + "] " + ll.memo + " - &lt;" + ll.client_name + "&gt;</span><br>");
                        }
                    }
                    sb.Append("                <table height='5' border='0' cellspacing='0' cellpadding='0'><tr><td></td></tr></table>");
                    sb.Append("                <div style='font-size:10pt;'><b>[면접 예정]<b> : " + mrm.new_intv_cnt.ToString() + " 명</div>");
                    if (interview.Count() > 0)
                    {
                        int seq = 0;
                        foreach (var ll in interview)
                        {
                            if (seq++ > 0)
                                sb.Append("                            <table height='5' border='0' cellspacing='0' cellpadding='0'><tr><td></td></tr></table>");
                            sb.Append("                            <span style='padding-left:10px;'>&nbsp;&nbsp;&nbsp;&nbsp;- <b><b style='padding-left:10px;'>[<a href='" + univision_url + "Candidate/CandidateDetail?c_seq=" + ll.c_seq.ToString() + "'>" + ll.can_name + "</a>] 님: </b> [" + GetDateTimeToString(ll.schedule_date.Value) + "] " + ll.gubun + " - &lt;" + ll.client_name + "&gt;</span><br>");
                        }
                    }
                }
                else
                {
                    sb.Append("                <div style='font-size:10pt;'><b>[입사 예정자]<b> : " + mrm.new_join_cnt.ToString() + " 명</div>");
                    sb.Append("                <table height='5' border='0' cellspacing='0' cellpadding='0'><tr><td></td></tr></table>");
                    sb.Append("                <div style='font-size:10pt;'><b>[보증기간만료]<b> : " + mrm.new_grt_cnt.ToString() + " 명</div>");
                    sb.Append("                <table height='5' border='0' cellspacing='0' cellpadding='0'><tr><td></td></tr></table>");
                    sb.Append("                <div style='font-size:10pt;'><b>[면접 예정]<b> : " + mrm.new_intv_cnt.ToString() + " 명</div>");
                }

                sb.Append("                        </td>");
                sb.Append("                    </tr>");
                sb.Append("                </table>");
                sb.Append("                <table height='10' border='0' cellspacing='0' cellpadding='0'><tr><td>&nbsp;</td></tr></table>");


                if (mrpl.Count() > 0)
                {
                    sb.Append("                <table width='100%' height='10' border='0' cellspacing='0' cellpadding='0'>");
                    sb.Append("                    <tr>");
                    sb.Append("                        <td style='font-size:11pt;'><b>[주간 프로젝트 현황(" + GetDateTimeToString(mrm.base_dt.Value.AddDays(-7)) + " ~ " +  GetDateTimeToString(mrm.base_dt.Value.AddDays(-1)) +  ")]</b></td>");
                    sb.Append("                    </tr>");
                    sb.Append("                    <tr>");
                    sb.Append("                        <td style='font-size:8pt;color:red;text-align:right;'>(※ 현재 진행 중이거나 지난주 활동이 있었던 프로젝트 입니다. 잠재/추천/면접은 지난 한 주 동안의 <b>자신의 활동</b>만 표시됩니다.)</td>");
                    sb.Append("                    </tr>");
                    sb.Append("                </table>");

                    sb.Append("                <table width='100%' border='0' cellspacing='1' cellpadding='0' bgcolor='#aaaaaa'>");
                    sb.Append("                    <tr>");
                    sb.Append("                        <th style='border-bottom: 1px solid #aaaaaa;font-size:8pt;background:#F7F7F7 none repeat scroll 0 0;height:30px;'>#</th>");
                    sb.Append("                        <th style='border-bottom: 1px solid #aaaaaa;font-size:8pt;background:#F7F7F7 none repeat scroll 0 0;height:30px;'>상태</th>");
                    sb.Append("                        <th style='border-bottom: 1px solid #aaaaaa;font-size:8pt;background:#F7F7F7 none repeat scroll 0 0;height:30px;'>프로젝트</th>");
                    sb.Append("                        <th style='border-bottom: 1px solid #aaaaaa;font-size:8pt;background:#F7F7F7 none repeat scroll 0 0;height:30px;'>등록일</th>");
                    sb.Append("                        <th style='border-bottom: 1px solid #aaaaaa;font-size:8pt;background:#F7F7F7 none repeat scroll 0 0;height:30px;'>업데이트</th>");
                    sb.Append("                        <th style='border-bottom: 1px solid #aaaaaa;font-size:8pt;background:#F7F7F7 none repeat scroll 0 0;height:30px;'>잠재</th>");
                    sb.Append("                        <th style='border-bottom: 1px solid #aaaaaa;font-size:8pt;background:#F7F7F7 none repeat scroll 0 0;height:30px;'>추천</th>");
                    sb.Append("                        <th style='border-bottom: 1px solid #aaaaaa;font-size:8pt;background:#F7F7F7 none repeat scroll 0 0;height:30px;'>면접</th>");
                    sb.Append("                        <th style='border-bottom: 1px solid #aaaaaa;font-size:8pt;background:#F7F7F7 none repeat scroll 0 0;height:30px;'>채용</th>");
                    sb.Append("                    </tr>");
                    sb.Append("                    <tr>");
                    sb.Append("                        <th colspan='5' style='font-size:8pt;border-bottom: 1px solid #aaaaaa;background:#F7F7F7 none repeat scroll 0 0;height:30px;'>합계</th>");
                    sb.Append("                        <th width='5%' style='font-size:8pt;border-bottom: 1px solid #aaaaaa;text-align:right;padding-right:5;background:#F7F7F7 none repeat scroll 0 0;height:30px;'>" + mrm.all_interest_cnt+"</th>");
                    sb.Append("                        <th width='5%' style='font-size:8pt;border-bottom: 1px solid #aaaaaa;text-align:right;padding-right:5;background:#F7F7F7 none repeat scroll 0 0;height:30px;'>" + mrm.all_recommend_cnt+"</th>");
                    sb.Append("                        <th width='5%' style='font-size:8pt;border-bottom: 1px solid #aaaaaa;text-align:right;padding-right:5;background:#F7F7F7 none repeat scroll 0 0;height:30px;'>" + mrm.all_interview_cnt+"</th>");
                    sb.Append("                        <th width='4%' style='font-size:8pt;border-bottom: 1px solid #aaaaaa;text-align:right;padding-right:5;background:#F7F7F7 none repeat scroll 0 0;height:30px;'>" + mrm.all_hire_cnt +"</th>");
                    sb.Append("                    </tr>");

                    int seq = 1;
                    foreach (var ll in mrpl)
                    {
                        string pjt_name = ll.pjt_title.Length > 30 ? ll.pjt_title.Substring(0, 30) + ".." : ll.pjt_title;
                        string status_name = String.Empty;
                        switch (ll.pjt_status) {
                            case 5 :
                                status_name = "<span style='color:green;'><b>[" + ll.status_name + "]</b></span>";
                                break;
                            case 4:
                                status_name = "<span style='color:blue;'><b>[" + ll.status_name + "]</b></span>";
                                break;
                            case 3:
                            case 2:
                                status_name = "<span style='color:red;'><b>[" + ll.status_name + "]</b></span>";
                                break;
                            default:
                                status_name = "<span><b>[" + ll.status_name + "]</b></span>";
                                break;
                        }

                        sb.Append("                    <tr bgcolor='#aaaaaa' height='35'>");
                        sb.Append("                        <td width='8%' bgcolor='#FFFFFF' style='font-size:8pt;border-bottom: 1px solid #aaaaaa;padding-left:5;text-align:center;'>" + (seq++) + "</td>");
                        sb.Append("                        <td width='8%' bgcolor='#FFFFFF' style='font-size:8pt;border-bottom: 1px solid #aaaaaa;padding-left:5;text-align:center;'>" + status_name + "</td>");
                        sb.Append("                        <td bgcolor='#FFFFFF' style='font-size:8pt;border-bottom: 1px solid #aaaaaa;padding-left:5'>[" + ll.client_name + "] ("+ll.user_names + ")<br/><a href='"+univision_url + "Project/ProjectDetail?p_seq="+ll.p_seq+"'>" + pjt_name + "</a></td>");
                        sb.Append("                        <td width='10%' bgcolor='#FFFFFF' style='font-size:8pt;border-bottom: 1px solid #aaaaaa;padding-left:5;text-align:center;'>" + GetDateTimeToString(ll.create_dt.Value) + "</td>");
                        sb.Append("                        <td width='10%' bgcolor='#FFFFFF' style='font-size:8pt;border-bottom: 1px solid #aaaaaa;padding-left:5;text-align:center;'>" + GetDateTimeToString(ll.pjt_update.Value) + "</td>");

                        if (ll.pjt_status == 5)
                        {
                            sb.Append("                        <td width='20%' colspan='4' bgcolor='#FFFFFF' style='font-size:8pt;border-bottom: 1px solid #aaaaaa;padding-right:5;text-align:center;'> 성공 </td>");
                        }
                        else if (ll.pjt_status == 2 || ll.pjt_status == 3)
                        {
                            sb.Append("                        <td width='20%' colspan='4' bgcolor='#FFFFFF' style='font-size:8pt;border-bottom: 1px solid #aaaaaa;padding-right:5;text-align:center;'> " + ll.status_name+" </td>");
                        }
                        else if (ll.interest_cnt == 0 && ll.recommend_cnt == 0 && ll.interview_cnt == 0 && ll.hire_cnt == 0)
                        {
                            sb.Append("                        <td width='20%' colspan='4' bgcolor='#FFFFFF' style='font-size:8pt;border-bottom: 1px solid #aaaaaa;padding-right:5;text-align:center;'> 후보자 활동 없음 </td>");
                        }
                        else
                        {
                            sb.Append("                        <td width='5%' bgcolor='#FFFFFF' style='font-size:8pt;border-bottom: 1px solid #aaaaaa;padding-right:5;text-align:right;'>" + ll.interest_cnt + "</td>");
                            sb.Append("                        <td width='5%' bgcolor='#FFFFFF' style='font-size:8pt;border-bottom: 1px solid #aaaaaa;padding-right:5;text-align:right;'>" + ll.recommend_cnt + "</td>");
                            sb.Append("                        <td width='5%' bgcolor='#FFFFFF' style='font-size:8pt;border-bottom: 1px solid #aaaaaa;padding-right:5;text-align:right;'>" + ll.interview_cnt + "</td>");
                            sb.Append("                        <td width='4%' bgcolor='#FFFFFF' style='font-size:8pt;border-bottom: 1px solid #aaaaaa;padding-right:5;text-align:right;'>" + ll.hire_cnt + "</td>");
                        }
                            
                        
                        
                        sb.Append("                    </tr>");
                    }
                    
                    sb.Append("                </table>");

                }
                sb.Append("                <table height='10' border='0' cellspacing='0' cellpadding='0'><tr><td>&nbsp;</td></tr></table>");
                sb.Append("                <table width='100%' height='10' border='0' cellspacing='0' cellpadding='0'>");
                sb.Append("                    <tr>");
                sb.Append("                        <td style='font-size:11pt;'><b>[주간 후보자 등록 및 업데이트 현황]</b></td>");
                sb.Append("                    </tr>");
                sb.Append("                    <tr>");
                sb.Append("                        <td style='font-size:10pt;color:red;text-align:right;'>※ 주간 기준등록 수 : 각 5건(월 기준 : 각 20건)</td>");
                sb.Append("                    </tr>");
                sb.Append("                </table>");

                sb.Append("                <table width='100%' border='0' cellspacing='1' cellpadding='0' bgcolor='#aaaaaa'>");
                sb.Append("                    <tr>");
                sb.Append("                        <th style='border-bottom: 1px solid #aaaaaa;font-size:8pt;background:#F7F7F7 none repeat scroll 0 0;height:30px;'>후보자 등록</th>");
                sb.Append("                        <th style='border-bottom: 1px solid #aaaaaa;font-size:8pt;background:#F7F7F7 none repeat scroll 0 0;height:30px;'>후보자 업데이트</th>");
                sb.Append("                        <th style='border-bottom: 1px solid #aaaaaa;font-size:8pt;background:#F7F7F7 none repeat scroll 0 0;height:30px;'>메모장 등록</th>");
                sb.Append("                    </tr>");
                sb.Append("                    <tr bgcolor='#aaaaaa' height='35'>");
                sb.Append("                        <td width='33%' bgcolor='#FFFFFF' style='padding-right:5;text-align:right;'>" + mrm.week_new_can_cnt + "</td>");
                sb.Append("                        <td bgcolor='#FFFFFF' style='padding-right:5;text-align:right;'>" + mrm.week_update_can_cnt + "</td>");
                sb.Append("                        <td width='33%' bgcolor='#FFFFFF' style='padding-right:5;text-align:right;'>" + mrm.week_memo_cnt + "</td>");
                sb.Append("                    </tr>");
                sb.Append("                </table>");

                /*
                 <table width='100%' border='0' cellspacing='1' cellpadding='0' bgcolor='#cccccc'>
                    <tr><th rowspan='2' style='background:#F7F7F7 none repeat scroll 0 0;height:30px;'></th><th colspan='3' style='font-size:8pt;background:#F7F7F7 none repeat scroll 0 0;height:30px;'>저장소   </th></tr>
                    <tr><th style='font-size:8pt;background:#F7F7F7 none repeat scroll 0 0;height:30px;'>합계   </th><th style='font-size:8pt;background:#F7F7F7 none repeat scroll 0 0;height:30px;'>사용량   </th><th style='font-size:8pt;background:#F7F7F7 none repeat scroll 0 0;height:30px;'>남은량  </th></tr>
                    <tr height='25'>
                        <td width='140' align='right' bgcolor='#EAEAEA' style='padding-right:15px' nowrap><strong>2020-04-22 09:00</strong></td>
                        <td width='33%' bgcolor='#FFFFFF' style='padding-left:5'>98.2GB</td>
                        <td width='33%' bgcolor='#FFFFFF' style='padding-left:5'>40.83GB (42%)</td>
                        <td width='33%' bgcolor='#FFFFFF' style='padding-left:5'>8.27GB (8%)</td>
                    </tr>
                 </table>
                 */
















                sb.Append("            </td>");
                sb.Append("        </tr>");
                sb.Append("    </table>");







                sb.Append("</body>");
                sb.Append("</html>");
                return sb.ToString();
            }
            catch (Exception ex)
            {
                return String.Empty;
            }
        }
    }


}
