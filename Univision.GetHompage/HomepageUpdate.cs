using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Univision.Core.Models.DTO;
using Univision.Core.Repositories;
using Univision.Core.Models.HomePage;
using Univision.Core.Repositories.Homepage;
using Univision.GetHompage.Infrastructure;

namespace Univision.GetHompage
{
  class HomepageUpdate
  {
    public DateTime _latest { get; set; }

    public HomepageUpdate()
    {
      _latest = DateTime.UtcNow.AddHours(-1);
    }
    public async Task UpldatePrivacy()
    {
      try
      {
        //지난 1시간 홈페이지 privacy 리포트 데이터 불러오기
        SmsEntityRepository ser = new SmsEntityRepository();
        HomepageSmsEntityRepository hser = new HomepageSmsEntityRepository();
        var privacy_list = await hser.SelectPrivacyAgreeListDateAsync(_latest);
        //데이터 loop
        foreach (var privacy in privacy_list)
        {
          var uni_privacy = await ser.SelectPrivacyAgreeOneAsnyc(privacy.pa_seq);
          //유니비전에 해당 id의 업데이트 데이터가 없으면
          if (uni_privacy != null && !uni_privacy.agree_dt.HasValue)
          {
            uni_privacy.agree_code1 = privacy.agree_code1;
            uni_privacy.agree_code2 = privacy.agree_code2;
            uni_privacy.agree_dt = privacy.agree_dt;
            uni_privacy.agree_ip = privacy.agree_ip;
            //업데이트
            await ser.UpdatePrivacyMst(uni_privacy);
          }
        }

      } 
      catch (Exception e)
      {
        throw e;
      }
      
    }

    /// <summary>
    /// [신규 PrivacyCreate 플로우] 홈페이지 n_privacy_agree 동의완료 & 미동기화(is_sync=0) 건을 가져와
    /// 1) 내부 n_privacy_agree/dtl 반영  2) R_PROFILE_MEMO 메모  3) 완료 알림(정보주체 SMS/Email + 발신자 Email)
    /// 4) 홈페이지 is_sync=1 처리.  건별 처리 — 한 건 실패가 다른 건을 막지 않음.
    /// </summary>
    public async Task UpdateNPrivacy()
    {
      HomepageNPrivacyEntityRepository hner = new HomepageNPrivacyEntityRepository();
      var agreed_list = await hner.SelectAgreedNotSyncedAsync();
      Console.WriteLine("  대상 " + agreed_list.Count + "건");

      foreach (var hmst in agreed_list)
      {
        try
        {
          NPrivacyEntityRepository ner = new NPrivacyEntityRepository();
          var mst = await ner.SelectOneAsync(hmst.pa_seq);

          // 내부에 이미 반영된 건(지난 실행에서 반영 후 is_sync 처리만 실패) → 알림/메모 중복 방지, 동기화 표시만
          if (mst != null && !mst.agree_dt.HasValue)
          {
            // 1) 내부 반영
            var hdtl = await hner.SelectDtlListAsync(hmst.pa_seq);
            var dtl = await ner.SelectDtlListAsync(hmst.pa_seq);
            foreach (var d in dtl)
            {
              // 홈페이지 상세 pad_seq 는 별도 채번이므로 agree_type 으로 매칭
              var hd = hdtl.FirstOrDefault(x => x.agree_type == d.agree_type);
              d.is_agree = (hd != null && hd.is_agree == 1) ? 1 : 0;
            }
            mst.agree_code1 = hmst.agree_code1;
            mst.agree_code2 = hmst.agree_code2;
            mst.agree_dt = hmst.agree_dt;
            mst.agree_ip = hmst.agree_ip;
            await ner.UpdateAgreeResult(mst, dtl);

            string agreeType = AgreeTypeText(dtl);
            string agreeDt = mst.agree_dt.Value.ToString("yyyy-MM-dd HH:mm");

            // 2) 프로필 메모  예) [수신완료(2026-10-20 13:46)] 수집 및 이용, 제공 https://www.unicosearch.com/agreesuccess/{key}
            if ((mst.profile_id ?? 0) > 0)
            {
              string memo = "[수신완료(" + agreeDt + ")] " + MemoTypeText(dtl) + " " + AgreeSuccessUrl(mst);
              ProfileMemoEntityRepository pmr = new ProfileMemoEntityRepository();
              await pmr.CreateMemoAsync(new profile_memo
              {
                profile_id = mst.profile_id.Value,
                content = memo,
                create_dt = DateTime.UtcNow.AddHours(9)
              });
            }

            // 3) 완료 알림 (실패해도 동기화는 진행 — 재실행 시 메모/알림 중복 방지)
            await NotifyNPrivacy(mst, agreeType, agreeDt);
          }

          // 4) 동기화 완료
          await hner.MarkSyncedAsync(hmst);
          Console.WriteLine("  pa_seq " + hmst.pa_seq + " 동기화 완료" + (mst == null ? " (내부 데이터 없음)" : ""));
        }
        catch (Exception e)
        {
          Console.WriteLine("  pa_seq " + hmst.pa_seq + " 처리 오류: " + e.Message);
        }
      }
    }

    private static string AgreeTypeText(List<n_privacy_agree_dtl> dtl)
    {
      var names = dtl.Where(x => x.is_agree == 1)
                     .OrderBy(x => x.agree_type)
                     .Select(x => x.agree_type == 1 ? "개인정보 수집·이용" : x.agree_type == 2 ? "개인정보 제3자 제공" : "기타")
                     .ToList();
      return names.Count > 0 ? string.Join(", ", names) : "동의 항목 없음";
    }

    // 메모용 간소화 표기 (개인정보 문구 제외)
    private static string MemoTypeText(List<n_privacy_agree_dtl> dtl)
    {
      var names = dtl.Where(x => x.is_agree == 1)
                     .OrderBy(x => x.agree_type)
                     .Select(x => x.agree_type == 1 ? "수집 및 이용" : x.agree_type == 2 ? "제공" : "기타")
                     .ToList();
      return names.Count > 0 ? string.Join(", ", names) : "동의 항목 없음";
    }

    // 홈페이지 동의 내역 확인 페이지
    private static string AgreeSuccessUrl(n_privacy_agree mst)
    {
      return "https://www.unicosearch.com/agreesuccess/" + mst.agree_code3;
    }

    private async Task NotifyNPrivacy(n_privacy_agree mst, string agreeType, string agreeDt)
    {
      string url = AgreeSuccessUrl(mst);
      PrivacyMailService mail = new PrivacyMailService();
      string msg;

      // 정보주체
      try
      {
        if (mst.send_type == "S")
        {
          string text = "[유니코써치] " + mst.can_name + "님, " + agreeType + " 동의가 완료되었습니다. (" + agreeDt + ")\n동의내역 확인: " + url;
          var sms = await new SmsSender().SendAsync(mst.send_addr, mst.can_contact, text);

          SmsEntityRepository ser = new SmsEntityRepository();
          await ser.CreateSms(new List<sms_history>
          {
            new sms_history()
            {
              msg_id = sms.msgid,
              sms_type = 0,
              send_number = mst.send_addr,
              send_type = 0,
              receive_number = mst.can_contact,
              contents = text,
              contents_len = 999,
              response_code = sms.response_code,
              response_desc = sms.response_desc,
              create_dt = DateTime.UtcNow.AddHours(9),
              create_user = 0
            }
          });
          Console.WriteLine("    정보주체 SMS: " + sms.response_code + " " + sms.exception_message);
        }
        else if (mst.send_type == "E")
        {
          bool ok = mail.Send(new[] { mst.can_contact }, "[유니코써치] 개인정보 수집·이용 및 제3자 제공 동의 내역 안내",
                              PrivacyMailService.SubjectBody(mst.can_name, agreeType, mst.agree_code1, agreeDt, url), out msg);
          Console.WriteLine("    정보주체 메일: " + (ok ? "성공" : msg));
        }
      }
      catch (Exception e)
      {
        Console.WriteLine("    정보주체 알림 오류: " + e.Message);
      }

      // 발신자(담당자)
      if (!string.IsNullOrWhiteSpace(mst.sender_email))
      {
        bool ok = mail.Send(new[] { mst.sender_email }, "[헤드헌팅 Pro] 개인정보 동의 완료 - " + mst.can_name,
                            PrivacyMailService.SenderBody(mst.can_name, mst.can_contact, mst.profile_id, agreeType, agreeDt, url), out msg);
        Console.WriteLine("    발신자 메일: " + (ok ? "성공" : msg));
      }
    }

    public async Task UpldateSMS()
    {
      

      
      
      
      try
      {
        //지난 1시간 홈페이지 sms 리포트 데이터 불러오기
        SmsEntityRepository ser = new SmsEntityRepository();
        HomepageSmsEntityRepository hser = new HomepageSmsEntityRepository();
        var sms_list = await hser.SelectSmsReportListDateAsync(_latest);
        //데이터 loop
        foreach (var sms in sms_list)
        {
          var uni_sms = await ser.SelectSmsReportOneAsnyc(sms.msg_id);
          //유니비전에 해당 id의 리포트 데이터가 없으면
          if (uni_sms == null)
          {
            uni_sms = new sms_report()
            {
              msg_id = sms.msg_id,
              create_dt = sms.create_dt,
              create_user = sms.create_user,
              net_work = sms.net_work,
              report_code = sms.report_code,
              report_desc = sms.report_code,
              rescnt = sms.rescnt,
              sent_date = sms.sent_date,
              to = sms.to,
              to_country = sms.to_country
            };
            //insert
            await ser.CreateSmsReport(uni_sms);
          }
        }

      } 
      catch (Exception e)
      {
        throw e;
      }

    }

  }
}
