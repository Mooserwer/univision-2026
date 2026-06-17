using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using Univision.Core.Lib;
using Univision.Core.Models;
using Univision.Core.Models.DTO;
using Univision.Core.Models.DTO.Excel;
using Univision.Core.Models.DTO.Request;
using Univision.Core.Models.DTO.Request.Candidate;
using Univision.Core.Models.DTO.Response.Candidate;
using Univision.Core.Models.DTO.Response.Project;
using Univision.Core.Models.DTO.Response.Schedult;
using Univision.Core.Repositories;
using Univision.Main.Infrastructure;
using Univision.Main.Models.Api;
using Univision.Main.Models.Candidate;
using Univision.Main.Models.Project;
using Univision.Security;
using Xceed.Words.NET;

namespace Univision.Main.Controllers
{
  public class ScheduleController : BaseController
  {
    public async Task<ActionResult> VacationList()
    {
      ScheduleRepository sr = new ScheduleRepository();

      var list = await sr.FindAllVacationToday();

      VacationListViewModel model = new VacationListViewModel
      {          
        list = list
      };
      return View(model);
      
    }
    #region 스케줄 메인

    public ActionResult ScheduleList()
    {
      return View();
    }

    [HttpGet]
    public async Task<JsonResult> ScheduleEventList(string start, string end, string type = "", int sub_type = 0)
    {
      try
      {
        ScheduleRepository sr = new ScheduleRepository();

        var list = await sr.FindScheduleList(AppIdentity.user_seq, type, sub_type, start.Substring(0, 10), end.Substring(0, 10), AppIdentity.ud_seq);

        return Json(list, JsonRequestBehavior.AllowGet);
      }
      catch (Exception e)
      {
        return Json(new
        {
          ok = false
        }, JsonRequestBehavior.AllowGet);
      }
    }

    [HttpGet]
    public async Task<JsonResult> AllScheduleCount(string start, string end)
    {
      try
      {
        ScheduleRepository sr = new ScheduleRepository();

        var data = await sr.FindAllScheduleCountAsync(AppIdentity.user_seq, start.Substring(0, 10), end.Substring(0, 10), AppIdentity.ud_seq);

        return Json(data, JsonRequestBehavior.AllowGet);
      }
      catch (Exception e)
      {
        return Json(new
        {
          ok = false
        }, JsonRequestBehavior.AllowGet);
      }
    }

    public async Task<PartialViewResult> ScheduleView(int s_seq = 0, string date = "")
    {
      ScheduleRepository sr = new ScheduleRepository();

      var data = await sr.FindScheduleOneAsync(s_seq);
      if (data == null)
      {
        DateTime startDt = new DateTime();
        DateTime.TryParse(date, out startDt);

        data = new schedule();
        data.start_date = startDt.AddHours(9);
        data.end_date = startDt.AddHours(18);
      }



      return PartialView(data);
    }

    [HttpPost]
    public async Task<JsonResult> CreateSchedule(schedule data)
    {
      try
      {
        ScheduleEntityRepository ser = new ScheduleEntityRepository();
        var sch = await ser.FindScheduleOneAsync(data.s_seq);
        string mode = "";

        //신규
        if (sch == null)
        {
          sch = new schedule()
          {
            title = data.title
              ,
            type = data.type
              ,
            sub_type = ScheduleSubType.normal
              ,
            ud_seq = data.type == ScheduleType.team ? AppIdentity.ud_seq : 0
              ,
            bg_color = Utils.returnScheduleColorReturn(data.type)
              ,
            start_date = data.start_date
              ,
            end_date = data.end_date
              ,
            contents = data.contents
              ,
            create_dt = Utils.NowKorea()
              ,
            create_user = AppIdentity.user_seq
              ,
            modify_dt = Utils.NowKorea()
              ,
            modify_user = AppIdentity.user_seq
          };

          List<schedule_attend> attendList = new List<schedule_attend>();

          //공유 스케줄의 경우
          if (data.type == ScheduleType.share)
          {
            if (!string.IsNullOrWhiteSpace(data.attend_user))
            {
              var attendSeqs = data.attend_user.Split(',');

              foreach (var attend in attendSeqs)
              {
                schedule_attend at = new schedule_attend()
                {
                  uv_seq = int.Parse(attend)
                    ,
                  create_dt = Utils.NowKorea()
                    ,
                  create_user = AppIdentity.user_seq
                    ,
                  modify_dt = Utils.NowKorea()
                    ,
                  modify_user = AppIdentity.user_seq
                };

                attendList.Add(at);
              }
            }
          }
          mode = CommonCodes.Create;
          await ser.CreateOrUpdateSchedule(sch, attendList, null, CommonCodes.Create);

        }
        //수정
        else
        {
          sch.title = data.title;
          sch.type = data.type;
          sch.ud_seq = data.type == ScheduleType.team ? AppIdentity.ud_seq : 0;
          sch.bg_color = Utils.returnScheduleColorReturn(data.type);
          sch.start_date = data.start_date;
          sch.end_date = data.end_date;
          sch.contents = data.contents;
          sch.modify_dt = Utils.NowKorea();
          sch.modify_user = AppIdentity.user_seq;

          var beforeAttendList = await ser.FindScheduleAttendListAsync(data.s_seq);

          List<schedule_attend> attendList = new List<schedule_attend>();

          if (data.type == ScheduleType.share)
          {
            if (!string.IsNullOrWhiteSpace(data.attend_user))
            {
              var attendSeqs = data.attend_user.Split(',');

              foreach (var attend in attendSeqs)
              {
                schedule_attend at = new schedule_attend()
                {
                  uv_seq = int.Parse(attend)
                    ,
                  create_dt = Utils.NowKorea()
                    ,
                  create_user = AppIdentity.user_seq
                    ,
                  modify_dt = Utils.NowKorea()
                    ,
                  modify_user = AppIdentity.user_seq
                };

                attendList.Add(at);
              }

              //이벤트 업데이트를 위해 비매핑컬럼 입력.
              sch.attend_cnt = attendList.Count();
            }
          }
          mode = CommonCodes.Update;
          await ser.CreateOrUpdateSchedule(sch, attendList, beforeAttendList, CommonCodes.Update);

          //후보자, 클라이언트, 프로젝트에 속한 스케줄의 경우 해당 데이터 같이 수정.
          if (sch.prc_seq.HasValue || sch.c_seq.HasValue || sch.cal_seq.HasValue)
          {
            //프로젝트 수정
            if (sch.prc_seq.HasValue)
            {
              ProjectEntityRepository per = new ProjectEntityRepository();

              var history = await per.SelectPjtRecHistory((int)sch.prc_seq);
              history.schedule_date = sch.start_date;
              history.contents = sch.contents;
              history.modify_dt = Utils.NowKorea();
              history.modify_user = AppIdentity.user_seq;

              await per.CreateOrUpdatePjtRecHistory(history, null, null, CommonCodes.Update, AppIdentity.user_seq, 1);
            }
            //후보자 수정
            else if (sch.c_seq.HasValue)
            {

            }
            //클라이언트 수정
            else
            {
              ClientEntityRepository cer = new ClientEntityRepository();

              var activity = await cer.selectActivityOneAsync((int)sch.cal_seq);
              activity.log_comment = sch.contents;
              activity.log_date = sch.start_date;
              activity.modify_dt = Utils.NowKorea();
              activity.modify_user = AppIdentity.user_seq;

              await cer.CreateOrUpdateActivity(activity, CommonCodes.Update, AppIdentity.user_seq);
            }
          }
        }

        //이벤트 업데이트를 위해 비매핑컬럼 입력.
        sch.id = sch.s_seq;
        sch.title = "[" + Utils.ReturnScheduleSubTypeTxt(sch.sub_type) + "]" + "[" + Utils.ReturnScheduleTypeTxt(sch.type) + "]" + sch.title;
        sch.start = sch.start_date.ToString("yyyy-MM-dd HH:mm:ss");
        sch.end = sch.end_date.ToString("yyyy-MM-dd HH:mm:ss");
        sch.borderColor = sch.bg_color;
        sch.backgroundColor = sch.bg_color;


        return Json(new
        {
          ok = true
            ,
          mode = mode
            ,
          schedule = sch
        });
      }
      catch (Exception e)
      {
        return Json(new
        {
          ok = false
            ,
          message = "알 수 없는 오류가 발생 했습니다."
        });
      }

    }

    [HttpPost]
    public async Task<JsonResult> DeleteSchedule(int s_seq)
    {
      try
      {

        ScheduleEntityRepository ser = new ScheduleEntityRepository();
        var schedule = await ser.FindScheduleOneAsync(s_seq);
        var attendList = await ser.FindScheduleAttendListAsync(s_seq);

        await ser.DeleteSchedule(schedule, attendList);

        return Json(new
        {
          ok = true
            ,
          message = "스케줄을 삭제 했습니다."
            ,
          s_seq = s_seq
        });
      }
      catch (Exception e)
      {
        return Json(new
        {
          ok = false
            ,
          message = "알 수 없는 오류가 발생 했습니다."
        });
      }
    }

    #endregion

  }
}