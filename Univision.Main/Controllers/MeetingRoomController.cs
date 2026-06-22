using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Security.Claims;
using System.Threading.Tasks;
using Univision.Core.Models.DTO;
using Univision.Core.Repositories;
using Univision.Security;
using Univision.Main.Infrastructure.Mailing;
using System.Text;
using System.Collections;

namespace Univision.Main.Controllers
{
  public class MeetingRoomController : BaseController
  {
    /// <summary>
    /// 회의실 
    /// </summary>
    /// <returns></returns>
    public ActionResult MeetingRoomView(string date_str)
    {
      ViewBag.book_date = date_str;
      ViewBag.is_meetingroom_admin = (AppIdentity.ua_seq == 1 || AppIdentity.ua_seq == 2 || AppIdentity.ua_seq == 3 ? true : false);
      return View();
    }

    /// <summary>
    /// 스케쥴 수정(신규 등록) 화면
    /// </summary>
    /// <param name="evt"></param>
    /// <returns></returns>
    public async Task<ActionResult> BookMeetingRoom(meeting_room_schedule evt)
    {


      ViewBag.is_meetingroom_admin = (AppIdentity.ua_seq == 1 || AppIdentity.ua_seq == 2 || AppIdentity.ua_seq == 3 ? true : false);

      // 사용자 리스트
      if (ViewBag.is_meetingroom_admin)
      {
        AdminRepository adr = new AdminRepository();
        var list = await adr.SelectUserMailListAsync("", "", "");
        list.Add(new uv_user()
        {
          uv_seq = 1,
          name = "시스템"
        });
        ViewBag.userList = list;//new SelectList(list, "uv_seq", "name");
      }


      MeetingRoomRepository mr = new MeetingRoomRepository();

      //미팅룸 리스트
      var room_list = await mr.SelectMeetingRoomListAsync();
      ViewBag.roomList = room_list;

      var data = await mr.SelectEventOneAsync(evt.id);

      if (data == null)
      {
        data = evt;
        data.date_str = Utils.ConvertDateTimeToString(evt.dd_start);
        if (data.resourceId != null)
        {
          var room = await mr.SelectMeetingRoomOneAsync(evt.resourceId);
          data.room_notice = room.room_notice;
          data.resourceName = room.title;
          data.resrouceShortName = room.short_title;
        }
        data.req_user_seq = AppIdentity.user_seq;
        data.req_user_name = AppIdentity.name;

      }

      return PartialView(data);

    }


    /// <summary>
    /// 리소스(미팅룸) 저장
    /// </summary>
    /// <param name="companyName"></param>
    /// <returns></returns>
    public async Task<JsonResult> SubmitSchedule(meeting_room_schedule evt)
    {
      try
      {
        MeetingRoomRepository mr = new MeetingRoomRepository();

        var start = ((DateTime)evt.dd_start).AddMinutes(1).ToString("yyyy-MM-dd HH:mm:ss");
        var end = ((DateTime)evt.dd_end).AddMinutes(-1).ToString("yyyy-MM-dd HH:mm:ss");

        var chkRoom = await mr.MeetingRoomAvailable(evt.resourceId, evt.id, start, end);
        if (!chkRoom)
          return Json(new
          {
            ok = false,
            message = "해당 시간대에 이미 예약된 내역이 있습니다."
          });

        MeetingRoomEntityRepository mer = new MeetingRoomEntityRepository();
        var schedule = await mer.FindMeetingRoomScheduleOneAsync(evt.id);

        var timeChange = false;
        var laptopChange = false;
        string newedit_str = String.Empty;
        //신규
        if (schedule == null)
        {
          schedule = new meeting_room_schedule()
          {
            req_user_seq = evt.req_user_seq,
            attend_user = evt.attend_user,
            resourceId = evt.resourceId,
            usage_cd = evt.usage_cd,
            date_str = evt.date_str,
            dd_start = evt.dd_start,
            dd_end = evt.dd_end,
            laptop = evt.laptop,
            comment = evt.comment,
            is_del = 0,
            is_schedule = evt.is_schedule,
            create_dt = Utils.NowKorea(),
            create_seq = AppIdentity.user_seq,
            modify_dt = Utils.NowKorea(),
            modify_seq = AppIdentity.user_seq
          };

          await mer.CreateOrUpdateMeetingRoom(schedule, CommonCodes.Create);

          timeChange = true;
          laptopChange = evt.laptop == 1 ? true : false;
        }
        //수정
        else
        {
          newedit_str = "(수정)";
          //회의실 변경, 시작시간 변경시, 담당자에게 메일 전송
          if (schedule.resourceId != evt.resourceId || ((DateTime)schedule.dd_start).ToString("HH:mm") != evt.start_time)
            timeChange = true;

          //노트북 사용여부 변경시, 담당자에게 메일 전송.
          if (schedule.laptop != evt.laptop)
            laptopChange = true;

          schedule.req_user_seq = evt.req_user_seq;
          schedule.req_user_seq = evt.req_user_seq;
          schedule.attend_user = evt.attend_user;
          schedule.resourceId = evt.resourceId;
          schedule.usage_cd = evt.usage_cd;
          schedule.date_str = evt.date_str;
          schedule.dd_start = evt.dd_start;
          schedule.dd_end = evt.dd_end;
          schedule.laptop = evt.laptop;
          schedule.comment = evt.comment;
          schedule.is_schedule = evt.is_schedule;
          schedule.modify_dt = Utils.NowKorea();
          schedule.modify_seq = AppIdentity.user_seq;

          await mer.CreateOrUpdateMeetingRoom(schedule, CommonCodes.Update);
        }

        //메일 발송 체크, 시작시간 및 회의실 변경, 노트북 사용여부 변경시.
        if (evt.is_mail_send == 1 || timeChange || laptopChange)
        {
          AccountRepository ar = new AccountRepository();
          var requestUser = await ar.FindUserBySeqAsync(evt.req_user_seq);

          List<string> ToArr = new List<string>();

          if (!ToArr.Contains(requestUser.email))
            ToArr.Add(requestUser.email);

          //메일 발송 체크시, 참석자들에 메일 발송.
          if (evt.is_mail_send == 1 && evt.attend_user != null)
          {
            foreach (var userId in evt.attend_user.Split(','))
            {
              var user = await ar.FindUserByIdAsync(userId);

              if (!string.IsNullOrWhiteSpace(user.email))
                ToArr.Add(user.email);
            }
          }

          //회의실 담당자 추가
          if (timeChange)
            //회의실 담당자가 참석자 목록에 없다면.
            if (!ToArr.Contains("suhee@unicosearch.com"))
              ToArr.Add("suhee@unicosearch.com");

          //노트북 담당자 추가
          if (laptopChange)
            //노트북 담당자가 참석자 목록에 없다면.
            if (!ToArr.Contains("lee.hc@unicosearch.com"))
              ToArr.Add("lee.hc@unicosearch.com");

          MeetingRoomReservationDto mailData = new MeetingRoomReservationDto()
          {
            ToArr = ToArr.ToArray()
              ,
            From = new System.Net.Mail.MailAddress("noreply@unicosearch.com", "noreply")
              ,
            newedit = newedit_str
              ,
            useType = Utils.returnMeetingRoomUseType(evt.usage_cd)
              ,
            dateStr = evt.date_str
              ,
            startTime = evt.start_time
              ,
            endTime = evt.end_time
              ,
            resourceName = evt.resourceName
              ,
            requestName = requestUser.name
              ,
            laptop = evt.laptop.HasValue ? "Y" : "N"
              ,
            comment = evt.comment
              ,
            url = "http://univision.unicosearch.com/MeetingRoom/MeetingRoomView"
          };

          MailService mService = new MailService();
          var result = mService.SendReservationRoomMail(mailData, new MeetingRoomReservationTemplete());

          if (!result.isSend)
            return Json(new
            {
              ok = result.isSend
                ,
              message = "회의실 예약을 완료 했지만, 메일 발송에 실패 했습니다."
            });
        }

        //스케줄 등록 체크 되어 있다면
        if (evt.is_schedule == 1)
        {
          ScheduleEntityRepository ser = new ScheduleEntityRepository();
          var sch = await ser.FindScheduleWithMeetingRoomOneAsync(schedule.id);

          string usage = Utils.returnMeetingRoomUseType(schedule.usage_cd);

          //신규
          if (sch == null)
          {
            sch = new schedule()
            {
              type = ScheduleType.share,
              sub_type = ScheduleSubType.normal,
              title = "[미팅룸 예약]" + usage,
              start_date = (DateTime)schedule.dd_start,
              end_date = (DateTime)schedule.dd_end,
              contents = schedule.comment,
              meeting_id = schedule.id,
              create_dt = Utils.NowKorea(),
              create_user = AppIdentity.user_seq,
              modify_dt = Utils.NowKorea(),
              modify_user = AppIdentity.user_seq,
              bg_color = ScheduleType.shareColor
            };

            var schAttendList = new List<schedule_attend>();

            if (evt.attend_user != null)
            {
              AccountRepository ar = new AccountRepository();
              foreach (var userId in evt.attend_user.Split(','))
              {
                var user = await ar.FindUserByIdAsync(userId);

                //user 정보가 없으면 일단 넘김.
                if (user == null)
                  continue;

                schedule_attend at = new schedule_attend()
                {
                  uv_seq = user.uv_seq
                    ,
                  create_dt = Utils.NowKorea()
                    ,
                  create_user = AppIdentity.user_seq
                    ,
                  modify_dt = Utils.NowKorea()
                    ,
                  modify_user = AppIdentity.user_seq
                };

                schAttendList.Add(at);
              }
            }

            await ser.CreateOrUpdateSchedule(sch, schAttendList, null, CommonCodes.Create);

          }
          else
          {
            sch.type = ScheduleType.share;
            sch.sub_type = ScheduleSubType.normal;
            sch.title = "[미팅룸 예약]" + usage;
            sch.start_date = (DateTime)schedule.dd_start;
            sch.end_date = (DateTime)schedule.dd_end;
            sch.contents = schedule.comment;
            sch.meeting_id = schedule.id;
            sch.create_dt = Utils.NowKorea();
            sch.create_user = AppIdentity.user_seq;
            sch.modify_dt = Utils.NowKorea();
            sch.modify_user = AppIdentity.user_seq;
            sch.bg_color = ScheduleType.shareColor;

            var beforeAttendList = await ser.FindScheduleAttendListAsync(sch.s_seq);

            var schAttendList = new List<schedule_attend>();

            if (evt.attend_user != null)
            {
              AccountRepository ar = new AccountRepository();
              foreach (var userId in evt.attend_user.Split(','))
              {
                var user = await ar.FindUserByIdAsync(userId);

                //user 정보가 없으면 일단 넘김.
                if (user == null)
                  continue;


                schedule_attend at = new schedule_attend()
                {
                  uv_seq = user.uv_seq
                    ,
                  create_dt = Utils.NowKorea()
                    ,
                  create_user = AppIdentity.user_seq
                    ,
                  modify_dt = Utils.NowKorea()
                    ,
                  modify_user = AppIdentity.user_seq
                };

                schAttendList.Add(at);
              }
            }

            await ser.CreateOrUpdateSchedule(sch, schAttendList, beforeAttendList, CommonCodes.Update);
          }


        }

        return Json(new
        {
          ok = true
            ,
          message = "예약이 완료 되었습니다. <br />수정/취소가 필요한 경우는 해당 이벤트를 눌러 수정/취소 하여 주시기 바랍니다."
        });
      }
      catch (Exception e)
      {
        return Json(new
        {
          ok = false
        });
      }
    }

    [HttpPost]
    public async Task<JsonResult> RemoveSchedule(int event_id)
    {
      try
      {
        MeetingRoomEntityRepository mer = new MeetingRoomEntityRepository();
        var schedule = await mer.FindMeetingRoomScheduleOneAsync(event_id);
        if (schedule == null)
          return Json(new
          {
            ok = false,
            message = "삭제하려는 회의실 예약 정보를 찾을 수 없습니다."
          });

        //스케줄 등록된 미팅 룸의 경우.
        if (schedule.is_schedule == 1)
        {
          ScheduleEntityRepository ser = new ScheduleEntityRepository();
          var sch = await ser.FindScheduleWithMeetingRoomOneAsync(schedule.id);

          if (sch != null)
          {
            var beforeAttendList = await ser.FindScheduleAttendListAsync(sch.s_seq);

            await ser.DeleteSchedule(sch, beforeAttendList);
          }
        }

        await mer.CreateOrUpdateMeetingRoom(schedule, CommonCodes.Delete);


        return Json(new
        {
          ok = true,
          message = "회의실 예약을 삭제 했습니다."
        });
      }
      catch (Exception e)
      {
        return Json(new
        {
          ok = false,
          message = "회의실 삭제 오류 : " + e.Message
        });
      }
    }

    /// <summary>
    /// 리소스(미팅룸) 리스트
    /// </summary>
    /// <param name="companyName"></param>
    /// <returns></returns>
    public async Task<JsonResult> MeetingRoomList()
    {

      try
      {
        MeetingRoomRepository mr = new MeetingRoomRepository();

        var list = await mr.SelectMeetingRoomListAsync();


        return Json(list);
      }
      catch (Exception e)
      {
        return Json(new
        {
          ok = false
        });
      }
    }



    /// <summary>
    /// 이벤트(스케쥴) 리스트
    /// </summary>
    /// <param name="start"></param>
    /// <param name="end"></param>
    /// <returns></returns>
    public async Task<JsonResult> MeetingRoomEventList(string start, string end)
    {

      try
      {
        MeetingRoomRepository mr = new MeetingRoomRepository();

        var list = await mr.SelectEventListAsync(start.Substring(0, 10), end.Substring(0, 10));


        return Json(list);
      }
      catch (Exception e)
      {
        return Json(new
        {
          ok = false
        });
      }
    }

    public async Task<JsonResult> RoomCheck(string resource_id, int? event_id, string start_dt, string end_dt)
    {

      try
      {
        MeetingRoomRepository mr = new MeetingRoomRepository();

        var rst = await mr.MeetingRoomAvailable(resource_id, event_id, start_dt, end_dt);


        return Json(new
        {
          is_usable = rst
        });
      }
      catch (Exception e)
      {
        return Json(new
        {
          is_usable = false
        });
      }
    }
  }
}