using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Univision.Core.Models.DTO;
using Univision.Core.Repositories;
using Univision.Core.Models.HomePage;
using Univision.Core.Repositories.Homepage;

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
