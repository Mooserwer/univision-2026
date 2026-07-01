using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Univision.Core.Lib;
using Univision.Core.Models;
using Univision.Core.Models.DTO;
using Univision.Core.Models.DTO.Request;
using Univision.Core.Models.DTO.Request.MyList;
using Univision.Core.Models.DTO.Response;
using Univision.Core.Repositories;
using Univision.Main.Infrastructure.Mailing;
using Univision.Main.Models.MyList;
using Univision.Main.Models.Vacation;
using Univision.Main.Models.Receipt;
using Univision.Security;
using Univision.Main.Infrastructure;
using Univision.Core.Models.DTO.Response.MyList;

namespace Univision.Main.Controllers
{
  public class MyListController : BaseController
  {
    // GET: MyList
    public ActionResult AttendUpload()
    {
      return View();
    }

    public async Task<ActionResult> ReceiptList(ReceiptSearchModel search, int page = 1)
    {
      MyListRepository mr = new MyListRepository();

      int totalCount = 0;
      search.uv_seq = AppIdentity.user_seq;
      
      if (search.searchYear == 0 || search.searchYear == null)
      {
        search.searchYear = Utils.NowKorea().Year;
      }
      //한 페이지당 몇개의 행을 보여줄 것인지
      int pageSize = 100;

      //var list = new List<receipt>();
      var list = mr.SelectReceiptList(search);
      if (list == null)
      {
        list = new List<receipt>();
      }
      var yearList = new List<YearMonthModel>();

      for (var i = Utils.NowKorea().Year; i >= Utils.NowKorea().Year - 5; i--)
      {
        yearList.Add(new YearMonthModel() { key = i, value = i.ToString() + "년" });
      }

      ReceiptModel model = new ReceiptModel()
      {
        search = search,
        yearList = yearList,
        list = list,
        PagingInfo = new PagingInfo()
        {
          CurrentPage = page
              ,
          ItemsPerPage = pageSize
              ,
          TotalItems = totalCount
        }
      };

      return View(model);
    }
    

    public async Task<PartialViewResult> ModalLinkedInReceipt(int r_seq)
    {
      MyListRepository mr = new MyListRepository();

      var r_user = await mr.SelectUserReceipt(r_seq, AppIdentity.user_seq);

      receipt_user model = r_user;

      return PartialView(model);
    }


    public async Task<PartialViewResult> ModalMobileReceipt(int r_seq)
    {
      MyListRepository mr = new MyListRepository();

      var r_user = await mr.SelectUserReceipt(r_seq, AppIdentity.user_seq);
            
      receipt_user model = r_user;

      return PartialView(model);
    }

    public async Task<PartialViewResult> ModalCocardReceipt(int r_seq, int ra_seq)
    {
      MyListRepository mr = new MyListRepository();

      var r_user = await mr.SelectUserReceipt(r_seq, AppIdentity.user_seq);
      var r_user_dtl = await mr.SelectUserDtlReceiptList(r_seq, ra_seq, AppIdentity.user_seq);
      ReceiptCocardModel model = new ReceiptCocardModel()
      {
        r_seq = r_user.r_seq,
        ra_seq = r_user.ra_seq,
        r_type_name = r_user.r_type_name,
        r_month = r_user.r_month,
        card_no = r_user.comment,
        list = r_user_dtl
      }; 

      return PartialView(model);
    }


    #region 휴가신청

    public async Task<ActionResult> MyVacationList(VacationSearchModel search, int page = 1)
    {
      MyListRepository mr = new MyListRepository();
      AccountRepository ar = new AccountRepository();



      int totalCount = 0;
      if (search.uv_seq == 0 || search.uv_seq == null)
      {
        search.uv_seq = AppIdentity.user_seq;
      }
      var ud_seq = AppIdentity.ud_seq;
      if (ud_seq == 4)
      {
        ud_seq = 0;
      }
      var userList = await ar.SelectUserListByDivision(ud_seq);

      if (search.searchYear == 0 || search.searchYear == null)
      {
        search.searchYear = Utils.NowKorea().Year;
      }
      //한 페이지당 몇개의 행을 보여줄 것인지
      int pageSize = AppPaging.PageLength10;
      var status = mr.SelectMyVacationStatus(search);
      if (status == null)
      {
        status = new uv_vacation();
      }
      var list = mr.SelectMyVacationList(search);
      if (list == null)
      {
        list = new List<uv_vacation_history>();
      }
      var yearList = new List<YearMonthModel>();

      for (var i = Utils.NowKorea().Year; i >= Utils.NowKorea().Year - 5; i--)
      {
        yearList.Add(new YearMonthModel() { key = i, value = i.ToString() + "년" });
      }

      MyVacationModel model = new MyVacationModel()
      {
        status = status,
        search = search,
        yearList = yearList,
        userList = userList,
        list = list,
        PagingInfo = new PagingInfo()
        {
          CurrentPage = page
              ,
          ItemsPerPage = pageSize
              ,
          TotalItems = totalCount
        }
      };

      return View(model);
    }
    
  [HttpPost]
    public async Task<JsonResult> CreateLinkedInReceipt(ReceiptCreateModel model)
    {
      try
      {
        string file_name = String.Empty; 
        string file_path = String.Empty; 
        if (model.files != null)
        {
          FileUpload fiUpload = new FileUpload();
          string path = Server.MapPath("~/UploadedFiles");
          var result = fiUpload.UploadReceipt(path, "Receipt/" + model.r_seq.ToString(), "링크드인_" + AppIdentity.name, model.files);


          if (!result.status)
            return Json(new
            {
              ok = false,
              message = result.statusMessage
            });

          file_name = result.name;
          file_path = result.dbPath;
        }
        MyListEntityRepository mer = new MyListEntityRepository();

        var rud = await mer.SelectReceiptUserDtlOneAsync(model.ra_seq);
        if (rud == null)
        {
          rud = new receipt_user_dtl()
          {
            r_seq = model.r_seq,
            ra_seq = model.ra_seq,
            create_dt = Utils.NowKorea(),
            create_user = AppIdentity.user_seq
          };
        }
        rud.file_name = file_name;
        rud.file_path = file_path;
        
        rud.money01 = model.money01;
        rud.money02 = model.money02;
        rud.modify_dt = Utils.NowKorea();
        rud.modify_user = AppIdentity.user_seq;


        var ru = await mer.SelectReceiptUserOneAsync(model.ra_seq);
        if (ru == null)
        {
          ru = new receipt_user();
        }
        ru.is_submit = 1;
        ru.sub_money1 = model.money01;
        ru.sub_money2 = model.money02;
        ru.submit_date = Utils.NowKorea();
        ru.modify_dt = Utils.NowKorea();
        ru.modify_user = AppIdentity.user_seq;

        await mer.CreateMobileReceipt(ru, rud);

        return Json(new
        {
          ok = true,
          message = "제경비를 등록 했습니다.",
        });
      }
      catch (Exception e)
      {
        return Json(new
        {
          ok = false,
          message = "알 수 없는 오류가 발생 했습니다." + e.Message
        });
      }
    }

    [HttpPost]
    public async Task<JsonResult> CreateMobileReceipt(ReceiptCreateModel model)
    {
      try
      {
        FileUpload fiUpload = new FileUpload();
        string path = Server.MapPath("~/UploadedFiles");
        var result = fiUpload.UploadReceipt(path, "Receipt/"+model.r_seq.ToString(), "통신비_" + AppIdentity.name, model.files);

        if (!result.status)
          return Json(new
          {
            ok = false,
            message = result.statusMessage
          });

        MyListEntityRepository mer = new MyListEntityRepository();

        var rud = await mer.SelectReceiptUserDtlOneAsync(model.ra_seq);
        if (rud == null)
        {
          rud = new receipt_user_dtl()
          {
            r_seq = model.r_seq,
            ra_seq = model.ra_seq,
            create_dt = Utils.NowKorea(),
            create_user = AppIdentity.user_seq
          };
        }
        rud.file_name = result.name;
        rud.file_path = result.dbPath;
        rud.money01 = model.money01;
        rud.money02 = model.money02;
        rud.modify_dt = Utils.NowKorea();
        rud.modify_user = AppIdentity.user_seq;


        var ru = await mer.SelectReceiptUserOneAsync(model.ra_seq);
        if (ru == null)
        {
          ru = new receipt_user();
        }
        ru.is_submit  = 1;
        ru.sub_money1 = model.money01;
        ru.sub_money2 = model.money02;
        ru.submit_date = Utils.NowKorea();
        ru.modify_dt = Utils.NowKorea();
        ru.modify_user = AppIdentity.user_seq;

        await mer.CreateMobileReceipt(ru, rud);

        return Json(new
        {
          ok = true,
          message = "제경비를 등록 했습니다.",
        });
      }
      catch (Exception e)
      {
        return Json(new
        {
          ok = false,
          message = "알 수 없는 오류가 발생 했습니다." + e.Message
        });
      }
    }

    [HttpPost]
    public async Task<JsonResult> CreateCocardReceipt(ReceiptCreateModel model)
    {
      try
      {
        MyListEntityRepository mer = new MyListEntityRepository();

        if (model.list.Count > 0) {
          var rud_list = new List<receipt_user_dtl>();
          foreach (var data in model.list)
          {
            var rud = await mer.SelectReceiptUserDtlOneAsync(model.ra_seq, data.rad_seq);
            if (rud != null)
            {
              rud.comment1 = data.comment1;
              rud.modify_dt = Utils.NowKorea();
              rud.modify_user = AppIdentity.user_seq;
              rud_list.Add(rud);
            }

          }

          var ru = await mer.SelectReceiptUserOneAsync(model.ra_seq);
          if (ru == null)
          {
            ru = new receipt_user();
          }
          ru.is_submit = 1;
          ru.submit_date = Utils.NowKorea();
          ru.modify_dt = Utils.NowKorea();
          ru.modify_user = AppIdentity.user_seq;

          await mer.CreateCoCardReceipt(ru, rud_list);

        }

        return Json(new
        {
          ok = true,
          message = "제경비를 등록 했습니다.",
        });
      }
      catch (Exception e)
      {
        return Json(new
        {
          ok = false,
          message = "알 수 없는 오류가 발생 했습니다." + e.Message
        });
      }
    }

    // Shift 자택근무(v_type=11) 주간 한도 초과 여부 사전 체크 (경고용)
    [HttpGet]
    public JsonResult CheckShiftHomeWeekly(string date, decimal days = 1)
    {
      try
      {
        DateTime d = DateTime.Parse(date);
        int diff = ((int)d.DayOfWeek + 6) % 7;          // 월요일 기준(월=0)
        DateTime weekStart = d.Date.AddDays(-diff);
        DateTime weekEndNext = weekStart.AddDays(7);     // 다음 주 월요일(미포함 상한)

        MyListRepository mr = new MyListRepository();
        int limit = mr.SelectWeeklyShiftLimit(AppIdentity.user_seq);
        decimal used = mr.CountShiftHomeDaysInWeek(AppIdentity.user_seq,
                          weekStart.ToString("yyyy-MM-dd"), weekEndNext.ToString("yyyy-MM-dd"));

        string weekStr = weekStart.ToString("MM/dd") + "~" + weekStart.AddDays(6).ToString("MM/dd");
        bool over = (used + days) > limit;

        return Json(new
        {
          ok = true,
          over,
          used,
          limit,
          week = weekStr,
          message = string.Format("이번 주({0}) Shift 자택근무가 이미 {1}일 신청되어 주간 한도({2}일)를 초과합니다.<br/>그래도 계속 신청하시겠습니까?", weekStr, used, limit)
        }, JsonRequestBehavior.AllowGet);
      }
      catch (Exception e)
      {
        return Json(new { ok = false, message = e.Message }, JsonRequestBehavior.AllowGet);
      }
    }

    [HttpPost]
    public async Task<JsonResult> MyVacationCreate(VacationCreateModel model)
    {
      try
      {

        MyListEntityRepository mler = new MyListEntityRepository();
        AccountRepository ar = new AccountRepository();
        ApiEntityRepository apier = new ApiEntityRepository();
        List<int> userList = new List<int>();
        List<string> ToArr = new List<string>();

        //신규, 수정 state 변수
        //string state = "";

        //신규
        if (model.data.v_seq == 0)
        {
          model.data.confirm_user = AppIdentity.s_confirm_seq;
          model.data.request_date = Utils.NowKorea();
          model.data.request_user = AppIdentity.user_seq;
          model.data.leader_seq = AppIdentity.leader_seq;
          model.data.s_leader_seq = AppIdentity.s_confirm_seq;

          if (AppIdentity.user_seq == AppIdentity.leader_seq)
          {
            model.data.leader_confirm = 1;
            model.data.leader_date = Utils.NowKorea();
          }
          else
          {
            if (userList.Count() == 0)
            {
              //알람 발송을 위해 uv_seq 담아주기.
              userList.Add(AppIdentity.leader_seq);
            }

          }

          if (AppIdentity.user_seq == AppIdentity.s_confirm_seq)
          {
            model.data.is_confirm = 1;
            model.data.confirm_date = Utils.NowKorea();
          }
          else
          {
            if (userList.Count() == 0)
            {
              //알람 발송을 위해 uv_seq 담아주기.
              userList.Add(AppIdentity.s_confirm_seq);
            }
          }

          if (AppIdentity.user_seq == AppIdentity.s_leader_seq)
          {
            model.data.s_leader_confirm = 1;
            model.data.s_leader_date = Utils.NowKorea();
          }
          else
          {
            //알람 발송을 위해 uv_seq 담아주기.

            if (userList.Count() == 0)
            {
              //알람 발송을 위해 uv_seq 담아주기.
              userList.Add(AppIdentity.s_leader_seq);
            }
          }



          //state = CommonCodes.Create;
        }

        List<alarm_user> aList = new List<alarm_user>();
        MailService mService = new MailService();
        if (userList.Count > 0)
        {
          alarm_message aMessage = new alarm_message()
          {
            href_url = "/Dashboard/Index",
            message = string.Format("승인이 필요한 휴가가 있습니다.[신청자:{0}][{1}~{2}][{3}]", AppIdentity.name, Utils.ConvertDateTimeToString(model.data.start_date), Utils.ConvertDateTimeToString(model.data.end_date), Utils.ReturnVacationTypeTxt(model.data.v_type)),
            create_dt = Utils.NowKorea()
          };

          foreach (var uv_seq in userList)
          {

            alarm_user alarm = new alarm_user()
            {
              uv_seq = uv_seq
                ,
              is_read = 0
            };

            aList.Add(alarm);

            //담당자 이메일 추가. (최대 3건이라 건별로 조회)
            var confirmUser = await ar.FindUserBySeqAsync(uv_seq);
            ToArr.Add(confirmUser.email);
          }

          await mler.CreateMyVacation(model, aMessage, aList);
          string remain_day = "";
          if(model.data.v_type == 1)
          {
            remain_day = @"
<tr>
<th style='padding: .75rem; vertical-align: top; border-top: 1px solid #dee2e6; border: 1px solid #dee2e6 !important; background-color: #007bff; color: #fff'>잔여연차(사용 시)</th>
<td style='padding: .75rem; vertical-align: top; border-top: 1px solid #dee2e6; border: 1px solid #dee2e6 !important'>" + model.data.cur_remain.ToString() + @" 일</td>
</tr>
";
          }
          //메일 Dto
          VactationCreateDto mailData = new VactationCreateDto()
          {
            ToArr = ToArr.ToArray()
              ,
            From = new System.Net.Mail.MailAddress("noreply@unicosearch.com", "noreply")
              ,
            title = aMessage.message
              ,
            name = AppIdentity.name
              ,
            vType = Utils.ReturnVacationTypeTxt(model.data.v_type)
              ,
            startDate = Utils.ConvertDateTimeToString(model.data.start_date)
              ,
            endDate = Utils.ConvertDateTimeToString(model.data.end_date)
              ,
            day = model.data.vacation_number.ToString()
              ,
            remainday = remain_day
              ,
            requestDate = Utils.ConvertDateTimeToString(Utils.NowKorea())
              ,
            content = model.data.comment
              ,
            url = "http://univision.unicosearch.com/"
              ,
            urlStatus = "http://univision.unicosearch.com/MyList/MyVacationList?search.uv_seq=" + AppIdentity.user_seq
          };

          var result = mService.SendVacationApprMail(mailData, new VactationApprTemplete());
          if (!result.isSend)
            return Json(new
            {
              ok = true //result.isSend
                ,
              message = "휴가 신청을 완료 했지만, 메일 발송에 실패 했습니다." + result.message
            });

          return Json(new
          {
            ok = true
              ,
            message = "휴가를 신청하고, 승인자에게 알림과 메일을 발송 했습니다."
          });
        }
        else
        {
          await mler.CreateMyVacation(model, null, null);
        }

        List<alarm_user> me = new List<alarm_user>();
        alarm_message aMessage_my = new alarm_message()
        {
          href_url = "/MyList/MyVacationList",
          message = string.Format("휴가 신청이 완료 되었습니다.[신청자:{0}][{1}~{2}][{3}]", AppIdentity.name, Utils.ConvertDateTimeToString(model.data.start_date), Utils.ConvertDateTimeToString(model.data.end_date), Utils.ReturnVacationTypeTxt(model.data.v_type)),
          create_dt = Utils.NowKorea()
        };
        me.Add(new alarm_user()
        {
          uv_seq = AppIdentity.user_seq
            ,
          is_read = 0
        });

        await apier.CreateAlarm(aMessage_my, me);

        //메일 Dto
        VactationCreateDto mailData_my = new VactationCreateDto()
        {
          ToArr = new string[] { AppIdentity.email }
            ,
          From = new System.Net.Mail.MailAddress("noreply@unicosearch.com", "noreply")
            ,
          title = aMessage_my.message
            ,
          name = AppIdentity.name
            ,
          vType = Utils.ReturnVacationTypeTxt(model.data.v_type)
            ,
          startDate = Utils.ConvertDateTimeToString(model.data.start_date)
            ,
          endDate = Utils.ConvertDateTimeToString(model.data.end_date)
            ,
          day = model.data.vacation_number.ToString()
            ,
          requestDate = Utils.ConvertDateTimeToString(Utils.NowKorea())
            ,
          content = model.data.comment
            ,
          url = "http://univision.unicosearch.com/"
          ,
          urlStatus = "http://univision.unicosearch.com/MyList/MyVacationList?search.uv_seq=" + AppIdentity.user_seq
      };

        var result_my = mService.SendVacationCreateMail(mailData_my, new VactationCreateTemplete());
        if (!result_my.isSend)
          return Json(new
          {
            ok = true //result.isSend
              ,
            message = "휴가 신청을 완료 했지만, 메일 발송에 실패 했습니다." + result_my.message
          });

        return Json(new
        {
          ok = true
            ,
          message = "휴가 신청 했습니다."
        });
      }
      catch (Exception e)
      {
        return Json(new
        {
          ok = false,
          message = e.Message
        });
      }
    }

    public async Task<JsonResult> CreateVacationAppr(int v_seq, int appr = 1)
    {
      try
      {
        MyListEntityRepository mler = new MyListEntityRepository();
        var vacation_History = await mler.SelectVacationOneAsync(v_seq);
        string remain_day = "";
        
          if (vacation_History == null)
          return Json(new
          {
            ok = false,
            message = "승인/반려 하려는 휴가내역이 없습니다.."
          });

        if (vacation_History.leader_seq == AppIdentity.user_seq && vacation_History.leader_confirm != 1)
        {
          vacation_History.leader_confirm = appr;
          vacation_History.leader_date = Utils.NowKorea();
        }
        if (vacation_History.confirm_user == AppIdentity.user_seq && vacation_History.is_confirm != 1)
        {
          vacation_History.is_confirm = appr;
          vacation_History.confirm_date = Utils.NowKorea();
          if (vacation_History.s_leader_confirm != 1)
          {
            vacation_History.s_leader_confirm = appr;
            vacation_History.s_leader_date = Utils.NowKorea();
          }

        }
        if (vacation_History.s_leader_seq == AppIdentity.user_seq && vacation_History.s_leader_confirm != 1)
        {
          vacation_History.s_leader_confirm = appr;
          vacation_History.s_leader_date = Utils.NowKorea();
        }

        if (vacation_History.v_type == 1)
        {
          remain_day = @"
<tr>
<th style='padding: .75rem; vertical-align: top; border-top: 1px solid #dee2e6; border: 1px solid #dee2e6 !important; background-color: #007bff; color: #fff'>잔여연차(사용 시)</th>
<td style='padding: .75rem; vertical-align: top; border-top: 1px solid #dee2e6; border: 1px solid #dee2e6 !important'>" + vacation_History.cur_remain.ToString() + @" 일</td>
</tr>
";
        }

        alarm_message aMessage = new alarm_message()
        {
          href_url = "/MyList/MyVacationList",
          message = AppIdentity.name + "님이 휴가를 " + (appr == 1 ? "승인" : "반려") + "하였습니다.",
          create_dt = Utils.NowKorea()
        };

        List<alarm_user> aList = new List<alarm_user>();
        alarm_user alarm = new alarm_user()
        {
          uv_seq = vacation_History.request_user.Value
            ,
          is_read = 0
        };
        aList.Add(alarm);


        await mler.CreateVacationApprAlarm(vacation_History, aMessage, aList);

        MailService mService = new MailService();
        AccountRepository ar = new AccountRepository();
        var requestUser = await ar.FindUserBySeqAsync(vacation_History.request_user.Value);
        //모든 휴가 승인 시 신청자에게 메일 발송 및 일정표에 휴가 표시
        if ((vacation_History.is_confirm == 1 && vacation_History.leader_confirm == 1 && vacation_History.s_leader_confirm == 1) || appr == -1)
        {
          var confirmUser = await ar.FindUserBySeqAsync(vacation_History.request_user.Value);
          //메일 Dto
          VactationCreateDto mailData = new VactationCreateDto()
          {
            ToArr = new string[] { confirmUser.email }
              ,
            From = new System.Net.Mail.MailAddress("noreply@unicosearch.com", "noreply")
              ,
            title = "신청하신 휴가가 " + (appr == 1 ? "승인" : "반려") + "되었습니다."
              ,
            name = requestUser.name
              ,
            vType = Utils.ReturnVacationTypeTxt(vacation_History.v_type)
              ,
            startDate = Utils.ConvertDateTimeToString(vacation_History.start_date)
              ,
            endDate = Utils.ConvertDateTimeToString(vacation_History.end_date)
              ,
            day = vacation_History.vacation_number.ToString()
              ,
            remainday = remain_day
              ,
            requestDate = Utils.ConvertDateTimeToString(Utils.NowKorea())
              ,
            content = vacation_History.comment
              ,
            url = "http://univision.unicosearch.com/"
            ,
            urlStatus = "http://univision.unicosearch.com/MyList/MyVacationList"
          };

          var result = mService.SendVacationCreateMail(mailData, new VactationCreateTemplete());
          if (!result.isSend)
            return Json(new
            {
              ok = true //result.isSend
                ,
              message = "휴가 " + (appr == 1 ? "승인" : "반려") + " 처리를 완료 했지만, 다음 승인자에게 메일 발송이 실패 했습니다." + result.message
            });
        }
        else if (appr == 1 & vacation_History.confirm_date == null)
        {

          var confirmUser = await ar.FindUserBySeqAsync(vacation_History.confirm_user.Value);
          //메일 Dto
          VactationCreateDto mailData = new VactationCreateDto()
          {
            ToArr = new string[] { confirmUser.email }
              ,
            From = new System.Net.Mail.MailAddress("noreply@unicosearch.com", "noreply")
              ,
            title = string.Format("승인이 필요한 휴가가 있습니다.[신청자:{0}][{1}~{2}][{3}]", requestUser.name, Utils.ConvertDateTimeToString(vacation_History.start_date), Utils.ConvertDateTimeToString(vacation_History.end_date), Utils.ReturnVacationTypeTxt(vacation_History.v_type))
              ,
            name = requestUser.name
              ,
            vType = Utils.ReturnVacationTypeTxt(vacation_History.v_type)
              ,
            startDate = Utils.ConvertDateTimeToString(vacation_History.start_date)
              ,
            endDate = Utils.ConvertDateTimeToString(vacation_History.end_date)
              ,
            day = vacation_History.vacation_number.ToString()
              ,
            remainday = remain_day
              ,
            requestDate = Utils.ConvertDateTimeToString(Utils.NowKorea())
              ,
            content = vacation_History.comment
              ,
            url = "http://univision.unicosearch.com/"
            ,
            urlStatus = "http://univision.unicosearch.com/MyList/MyVacationList?search.uv_seq=" + requestUser.uv_seq
          };

          var result = mService.SendVacationApprMail(mailData, new VactationApprTemplete());
          if (!result.isSend)
            return Json(new
            {
              ok = true //result.isSend
                ,
              message = "휴가 " + (appr == 1 ? "승인" : "반려") + " 처리를 완료 했지만, 다음 승인자에게 메일 발송이 실패 했습니다." + result.message
            });
        }
        else if (appr == 1 & vacation_History.s_leader_date == null)
        {

          var confirmUser = await ar.FindUserBySeqAsync(vacation_History.s_leader_seq.Value);
          //메일 Dto
          VactationCreateDto mailData = new VactationCreateDto()
          {
            ToArr = new string[] { confirmUser.email }
              ,
            From = new System.Net.Mail.MailAddress("noreply@unicosearch.com", "noreply")
              ,
            title = string.Format("승인이 필요한 휴가가 있습니다.[신청자:{0}][{1}~{2}][{3}]", requestUser.name, Utils.ConvertDateTimeToString(vacation_History.start_date), Utils.ConvertDateTimeToString(vacation_History.end_date), Utils.ReturnVacationTypeTxt(vacation_History.v_type))
              ,
            name = requestUser.name
              ,
            vType = Utils.ReturnVacationTypeTxt(vacation_History.v_type)
              ,
            startDate = Utils.ConvertDateTimeToString(vacation_History.start_date)
              ,
            endDate = Utils.ConvertDateTimeToString(vacation_History.end_date)
              ,
            day = vacation_History.vacation_number.ToString()
            ,
            remainday = remain_day
            ,
            requestDate = Utils.ConvertDateTimeToString(Utils.NowKorea())
              ,
            content = vacation_History.comment
              ,
            url = "http://univision.unicosearch.com/"
            ,
            urlStatus = "http://univision.unicosearch.com/MyList/MyVacationList?search.uv_seq=" + requestUser.uv_seq
          };

          var result = mService.SendVacationApprMail(mailData, new VactationApprTemplete());
          if (!result.isSend)
            return Json(new
            {
              ok = true //result.isSend
                ,
              message = "휴가 " + (appr == 1 ? "승인" : "반려") + " 처리를 완료 했지만, 다음 승인자에게 메일 발송이 실패 했습니다." + result.message
            });
        }

        return Json(new
        {
          ok = true,
          message = "휴가 " + (appr == 1 ? "승인" : "반려") + " 처리가 완료 되었습니다."
        });
      }
      catch (Exception e)
      {
        return Json(new
        {
          ok = false,
          message = e.Message
        });
      }
    }

    #endregion

    #region 출근

    /// <summary>
    /// 출근기록 업로드
    /// </summary>
    /// <param name="files"></param>
    /// <returns></returns>
    [HttpPost]
    public async Task<JsonResult> AttendUpload(HttpPostedFileBase files)
    {
      try
      {
        Excel excel = new Excel();

        var data = excel.ReadToDataSet(files);

        List<uv_user_attend> attendList = new List<uv_user_attend>();

        var returnMsg = "출근 기록을 업로드 했습니다.<br/><br/>";

        //읽어온 엑셀의 행이 3줄을 초과해야 정상적인 데이터가 들어왔다고 판단함
        if (data.Tables[0].Rows.Count > 2)
        {
          AccountRepository ar = new AccountRepository();
          var userList = await ar.SelectAllUser();

          for (var i = 2; i < data.Tables[0].Rows.Count; i++)
          {
            var rows = data.Tables[0].Rows[i];

            //출근 데이터만.
            if (rows[6].ToString() == "출근")
            {
              int uv_seq = userList.Where(x => x.user_id == rows[3].ToString().Replace("!", "")).Select(x => x.uv_seq).FirstOrDefault();

              if (uv_seq == 0)
              {
                returnMsg += "오류 : " + (i + 2) + " 행 - 회원[" + rows[4].ToString() + "]을 DB에서 검색 할 수 없습니다.<br/>";
                continue;
              }


              var attendDate = new DateTime();

              if (!DateTime.TryParse(rows[0].ToString(), out attendDate))
              {
                returnMsg += "오류 : " + (i + 2) + " 행 - 회원[" + rows[4].ToString() + "]의 인증일시를 DateTime 으로 변환하는데 실패 했습니다.<br/>";
                continue;
              }

              uv_user_attend attend = new uv_user_attend()
              {
                uv_seq = uv_seq,
                attend_date = attendDate
              };

              attendList.Add(attend);

            }
          }

          MyListEntityRepository mer = new MyListEntityRepository();
          await mer.CreateOrDeleteUserAttend(attendList);
        }


        return Json(new
        {
          ok = true
            ,
          message = returnMsg
        });
      }
      catch (Exception e)
      {
        return Json(new
        {
          ok = false,
          message = e.Message
        });
      }
    }

    /// <summary>
    /// 출근 기록
    /// </summary>
    /// <returns></returns>
    public ActionResult UserAttend()
    {

      return View();
    }

    /// <summary>
    /// 출근기록 월 변경.
    /// </summary>
    /// <param name="calDate"></param>
    /// <returns></returns>
    public async Task<JsonResult> UserAttendUpdate(DateTime calDate)
    {
      try
      {
        string startDate = calDate.AddDays(1 - calDate.Day).ToString("yyyy-MM-dd");
        string endDate = calDate.AddMonths(1).AddDays(-1).ToString("yyyy-MM-dd");

        MyListRepository mr = new MyListRepository();
        var list = await mr.FindUserAttendListAsync(AppIdentity.user_seq, startDate, endDate);

        return Json(new
        {
          ok = true
            ,
          events = list
        });
      }
      catch (Exception e)
      {
        return Json(new
        {
          ok = false,
          message = e.Message
        });
      }
    }

    #endregion

    #region SMS

    public ActionResult SMSHistoryList(SmsHistorySearchModel search, int page = 1)
    {
      MyListRepository mr = new MyListRepository();

      int totalCount = 0;

      //한 페이지당 몇개의 행을 보여줄 것인지
      int pageSize = AppPaging.PageLength10;

      var list = mr.SelectSmsHistoryList(search, AppIdentity.user_seq, (page - 1) * pageSize, pageSize, out totalCount);

      SMSHistoryListViewModel model = new SMSHistoryListViewModel()
      {
        search = search,
        list = list,
        PagingInfo = new PagingInfo()
        {
          CurrentPage = page
              ,
          ItemsPerPage = pageSize
              ,
          TotalItems = totalCount
        }
      };

      return View(model);
    }



    #endregion

    #region 알람

    /// <summary>
    /// 알람 내역 
    /// </summary>
    /// <param name="search"></param>
    /// <param name="page"></param>
    /// <returns></returns>
    public ActionResult AlarmHistoryList(AlarmHistorySearchModel search, int page = 1)
    {
      MyListRepository mr = new MyListRepository();

      int totalCount = 0;

      //한 페이지당 몇개의 행을 보여줄 것인지
      int pageSize = AppPaging.PageLength20;

      var list = mr.SelectMyAlarmListAsync(search, AppIdentity.user_seq, (page - 1) * pageSize, pageSize, out totalCount);

      AlarmHistoryListViewModel model = new AlarmHistoryListViewModel()
      {
        search = search,
        list = list,
        PagingInfo = new PagingInfo()
        {
          CurrentPage = page
              ,
          ItemsPerPage = pageSize
              ,
          TotalItems = totalCount
        }
      };

      return View(model);
    }

    #endregion

  }
}