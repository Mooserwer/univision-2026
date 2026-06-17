using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
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
using Univision.Core.Models.DTO.Response.Statistics;
using Univision.Core.Models.HomePage;
using Univision.Core.Repositories;
using Univision.Core.Repositories.Homepage;
using Univision.Main.Infrastructure;
using Univision.Main.Infrastructure.Mailing;
using Univision.Main.Models.Api;
using Univision.Main.Models.Candidate;
using Univision.Main.Models.Project;
using Univision.Security;
using Xceed.Document.NET;
using Xceed.Words.NET;

namespace Univision.Main.Controllers
{
  public class ProjectController : BaseController
  {
    /*
    #region 인오더

    /// <summary>
    /// 마이프로젝트 리스트 채용
    /// </summary>
    /// <param name="search"></param>
    /// <param name="page"></param>
    /// <returns></returns>
    public async Task<ActionResult> InorderList(InorderSearchModel search, int page = 1)
    {
      ProjectRepository pr = new ProjectRepository();

      int totalCount = 0;

      //한 페이지당 몇개의 행을 보여줄 것인지
      int pageSize = AppPaging.PageLength20;

      var list = pr.SelectInorderList(search, (page - 1) * pageSize, pageSize, out totalCount);

      InorderListModel model = new InorderListModel()
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

    public async Task<ActionResult> InorderCreate(int i_seq = 0)
    {
      ProjectRepository pr = new ProjectRepository();
      string uploadTmpFolder = Utils.ReturnUniqueValue(AppIdentity.user_seq);
      var data = await pr.SelectInorderOneAsync(i_seq);
      if (data == null)
      {
        data = new inorder();
        data.inorder_dt = DateTime.Now;

      }

      InorderCreateModel model = new InorderCreateModel()
      {
        i_seq = data.i_seq
          ,
        p_seq = data.p_seq
          ,
        c_seq = data.c_seq
          ,
        cc_seq = data.cc_seq
          ,
        inorder_dt = data.inorder_dt
          ,
        inorder_type = data.inorder_type
          ,
        inorder_status = data.inorder_status
          ,
        clt_name = data.clt_name
          ,
        clt_busi_type = data.clt_busi_type
          ,
        clt_url = data.clt_url
          ,
        cc_name = data.cc_name
          ,
        cc_gender = data.cc_gender
          ,
        cc_position = data.cc_position
          ,
        cc_division = data.cc_division
          ,
        cc_phone = data.cc_phone
          ,
        cc_cell_phone = data.cc_cell_phone
          ,
        cc_email = data.cc_email
          ,
        cc_reason = data.cc_reason
          ,
        can_dept = data.can_dept
          ,
        can_pos = data.can_pos
          ,
        can_location = data.can_location
          ,
        can_jobdesc = data.can_jobdesc
          ,
        can_contents = data.can_contents
          ,
        create_dt = data.create_dt
          ,
        create_user = data.create_user
          ,
        modify_dt = data.modify_dt
          ,
        modify_user = data.modify_user
          ,
        client_name = data.client_name
          ,
        contact_name = data.contact_name
          ,
        project_title = data.project_title
          ,
        director_name = data.director_name


      };

      model.drectorList = await pr.SelectInorderDirectorListAsync(i_seq);
      //model.memoList = await pr.SelectProjectSearcherListAsync(i_seq);
      //model.fileList = await pr.SelectProjectJobListAsync(i_seq);

      ViewBag.uploadFolder = uploadTmpFolder;

      return View(model);
    }

    [HttpPost]
    public async Task<JsonResult> InorderCreate(InorderCreateModel data)
    {
      try
      {

        ProjectEntityRepository per = new ProjectEntityRepository();

        InorderCreateUpdateModel model = new InorderCreateUpdateModel();

        //신규, 수정 state 변수
        string state = "";

        //신규
        if (data.i_seq == 0)
        {
          //기본 정보
          model.data = new inorder()
          {
            p_seq = data.p_seq
              ,
            c_seq = data.c_seq
              ,
            cc_seq = data.cc_seq
              ,
            inorder_dt = data.inorder_dt
              ,
            inorder_type = data.inorder_type
              ,
            inorder_status = data.inorder_status
              ,
            clt_name = data.clt_name
              ,
            clt_busi_type = data.clt_busi_type
              ,
            clt_url = data.clt_url
              ,
            cc_name = data.cc_name
              ,
            cc_gender = data.cc_gender
              ,
            cc_position = data.cc_position
              ,
            cc_division = data.cc_division
              ,
            cc_phone = data.cc_phone
              ,
            cc_cell_phone = data.cc_cell_phone
              ,
            cc_email = data.cc_email
              ,
            cc_reason = data.cc_reason
              ,
            can_dept = data.can_dept
              ,
            can_pos = data.can_pos
              ,
            can_location = data.can_location
              ,
            can_jobdesc = data.can_jobdesc
              ,
            can_contents = data.can_contents
              ,
            create_dt = Utils.NowKorea()
              ,
            create_user = AppIdentity.user_seq
          };

          state = CommonCodes.Create;
        }
        //수정
        else
        {
          model.data = await per.SelectInorderOneAsync(data.i_seq);
          model.data.p_seq = data.p_seq;
          model.data.c_seq = data.c_seq;
          model.data.cc_seq = data.cc_seq;
          model.data.inorder_dt = data.inorder_dt;
          model.data.inorder_type = data.inorder_type;
          model.data.inorder_status = data.inorder_status;
          model.data.clt_name = data.clt_name;
          model.data.clt_busi_type = data.clt_busi_type;
          model.data.clt_url = data.clt_url;
          model.data.cc_name = data.cc_name;
          model.data.cc_gender = data.cc_gender;
          model.data.cc_position = data.cc_position;
          model.data.cc_division = data.cc_division;
          model.data.cc_phone = data.cc_phone;
          model.data.cc_cell_phone = data.cc_cell_phone;
          model.data.cc_email = data.cc_email;
          model.data.cc_reason = data.cc_reason;
          model.data.can_dept = data.can_dept;
          model.data.can_pos = data.can_pos;
          model.data.can_location = data.can_location;
          model.data.can_jobdesc = data.can_jobdesc;
          model.data.can_contents = data.can_contents;
          model.data.modify_dt = Utils.NowKorea();
          model.data.modify_user = AppIdentity.user_seq;


          model.drList = await per.SelectInorderDirectorListAsync(data.i_seq);

          state = CommonCodes.Update;
        }

        await per.CreateOrUpdateInorder(model, state, AppIdentity.user_seq, 1);

        if (data.fileList != null && data.fileList.Count > 0)
        {
          FileUpload fi = new FileUpload();
          //이력서 임시파일 정식으로 이동
          foreach (var file in data.fileList)
          {
            var result = fi.MoveFile(file.file_origin_path, Path.Combine(Server.MapPath("~/UploadedFiles"), "inorder/" + model.data.i_seq));
            if (!result.status)
              return Json(new
              {
                ok = false
                  ,
                message = result.statusMessage
              });

            inorder_file inf = new inorder_file()
            {
              i_seq = model.data.i_seq
                ,
              file_dir = Path.Combine(Utils.GetRootUrl(Request), result.dbPath)
                ,
              file_origin_path = result.filePath
                ,
              file_path = result.name
                ,
              file_extension = result.extension
                ,
              create_dt = Utils.NowKorea()
                ,
              create_user = AppIdentity.user_seq
            };

            await per.CreateOrDeleteInorderFile(inf, CommonCodes.Create);
          }
        }

        return Json(new
        {
          ok = true,
          i_seq = model.data.i_seq
        });
      }
      catch (Exception e)
      {
        return Json(new
        {
          ok = false,
          message = "알 수 없는 오류가 발생 했습니다."
        });
      }
    }

    public async Task<ActionResult> InorderDetail(int i_seq = 0)
    {
      ProjectRepository pr = new ProjectRepository();

      var data = await pr.SelectInorderOneAsync(i_seq);
      if (data == null)
        data = new inorder();

      InorderCreateModel model = new InorderCreateModel()
      {
        i_seq = data.i_seq
          ,
        p_seq = data.p_seq
          ,
        c_seq = data.c_seq
          ,
        cc_seq = data.cc_seq
          ,
        log_cnt = data.log_cnt
          ,
        inorder_dt = data.inorder_dt
          ,
        inorder_type = data.inorder_type
          ,
        inorder_status = data.inorder_status
          ,
        clt_name = data.clt_name
          ,
        clt_busi_type = data.clt_busi_type
          ,
        clt_url = data.clt_url
          ,
        cc_name = data.cc_name
          ,
        cc_gender = data.cc_gender
          ,
        cc_position = data.cc_position
          ,
        cc_division = data.cc_division
          ,
        cc_phone = data.cc_phone
          ,
        cc_cell_phone = data.cc_cell_phone
          ,
        cc_email = data.cc_email
          ,
        cc_reason = data.cc_reason
          ,
        can_dept = data.can_dept
          ,
        can_pos = data.can_pos
          ,
        can_location = data.can_location
          ,
        can_jobdesc = data.can_jobdesc
          ,
        can_contents = data.can_contents
          ,
        create_dt = data.create_dt
          ,
        create_user = data.create_user
          ,
        modify_dt = data.modify_dt
          ,
        modify_user = data.modify_user
          ,
        client_name = data.client_name
          ,
        contact_name = data.contact_name
          ,
        project_title = data.project_title
          ,
        director_name = data.director_name
          ,
        director_seq = data.director_seq
          ,
        director_tel = data.director_tel
          ,
        director_cp = data.director_cp
          ,
        director_email = data.director_email
          ,
        am_names = data.am_names
          ,
        searcher_names = data.searcher_names
          ,
        pjt_status = data.pjt_status
          ,
        project_dt = data.project_dt
      };

      //model.drectorList = await pr.SelectInorderDirectorListAsync(i_seq);
      model.fileList = await pr.SelectInorderFileListAsync(i_seq);

      return View(model);
    }


    public PartialViewResult InOrderMemoList(int i_seq, int page = 1)
    {
      ProjectRepository pr = new ProjectRepository();

      int totalCount = 0;

      //한 페이지당 몇개의 행을 보여줄 것인지
      int pageSize = 20;

      var list = pr.SelectInOrderMemoList(i_seq, (page - 1) * pageSize, pageSize, out totalCount);

      InOrderMemoListModel model = new InOrderMemoListModel()
      {
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

      return PartialView(model);
    }

    public async Task<JsonResult> InorderRemove(int i_seq)
    {
      try
      {
        ProjectEntityRepository per = new ProjectEntityRepository();
        InorderCreateUpdateModel model = new InorderCreateUpdateModel();

        model.data = await per.SelectInorderOneAsync(i_seq);

        if (model.data == null)
          return Json(new
          {
            ok = false,
            message = "삭제하려는 오더를 찾을 수 없습니다."
          });

        model.fileList = await per.SelectInorderFileListAsync(i_seq);

        await per.DeleteInorder(model, CommonCodes.Delete, AppIdentity.user_seq, 1);

        return Json(new
        {
          ok = true,
          message = "오더를 삭제 했습니다."
        });

      }
      catch (Exception e)
      {
        return Json(new
        {
          ok = false,
          message = "알 수 없는 오류가 발생 했습니다.<br />Error : " + e.Message
        });
      }
    }

    [HttpPost]
    public async Task<JsonResult> InOrderReject(int i_seq, string rejectReason, string rejectComment)
    {
      try
      {
        ProjectEntityRepository per = new ProjectEntityRepository();
        InorderCreateUpdateModel model = new InorderCreateUpdateModel();

        model.data = await per.SelectInorderOneAsync(i_seq);

        if (model.data == null)
          return Json(new
          {
            ok = false,
            message = "반려하려는 오더를 찾을 수 없습니다."
          });

        AccountRepository ar = new AccountRepository();
        var user = await ar.FindUserBySeqAsync((int)model.data.director_seq);
        if (user == null)
          return Json(new
          {
            ok = false,
            message = "반려 하려는 담당 컨설턴트를 찾을 수 없습니다."
          });

        model.data.director_seq = 0;

        await per.CreateOrUpdateInorder(model, CommonCodes.Update, AppIdentity.user_seq);

        alarm_message aMessage = new alarm_message()
        {
          message = string.Format("{0} Order 담당 컨설턴트 [{1}]님이 반려 하셨습니다.<br />반려사유 : {2}<br />상세내용 : {3}", model.data.clt_name, user.name, rejectReason, rejectComment),
          create_dt = Utils.NowKorea(),
          href_url = "/Project/InorderDetail?i_seq=" + model.data.i_seq
        };
        List<alarm_user> aList = new List<alarm_user>();
        alarm_user aUser = new alarm_user()
        {
          uv_seq = (int)model.data.create_user,
          is_read = 0
        };
        aList.Add(aUser);

        ApiEntityRepository aer = new ApiEntityRepository();
        await aer.CreateAlarm(aMessage, aList);

        inorder_memo iMemo = new inorder_memo()
        {
          i_seq = i_seq,
          memo = "[" + user.name + "]님께서 오더를 반려 했습니다.",
          create_dt = Utils.NowKorea(),
          create_user = AppIdentity.user_seq,
          modify_dt = Utils.NowKorea(),
          modify_user = AppIdentity.user_seq
        };

        await per.CreateOrUpdateInorderMemo(iMemo, CommonCodes.Create);

        return Json(new
        {
          ok = true,
          message = "오더를 반려 했습니다."
        });

      }
      catch (Exception e)
      {
        return Json(new
        {
          ok = false,
          message = "알 수 없는 오류가 발생 했습니다.<br />Error : " + e.Message
        });
      }
    }

    /// <summary>
    /// 인오더 메모 등록
    /// </summary>
    /// <param name="i_seq"></param>
    /// <param name="memo"></param>
    /// <returns></returns>
    [HttpPost]
    public async Task<JsonResult> CreateInOrderMemo(int i_seq, string memo)
    {
      try
      {
        ProjectEntityRepository per = new ProjectEntityRepository();
        var inorder = await per.SelectInorderOneAsync(i_seq);

        if (inorder == null)
          return Json(new
          {
            ok = false,
            message = "메모 등록하려는 인오더를 찾을 수 없습니다."
          });

        inorder_memo iMemo = new inorder_memo()
        {
          i_seq = i_seq,
          memo = memo,
          create_dt = Utils.NowKorea(),
          create_user = AppIdentity.user_seq,
          modify_dt = Utils.NowKorea(),
          modify_user = AppIdentity.user_seq
        };

        await per.CreateOrUpdateInorderMemo(iMemo, CommonCodes.Create);

        return Json(new
        {
          ok = true,
          message = "담당 컨설턴트를 저장 했습니다."
        });

      }
      catch (Exception e)
      {
        return Json(new
        {
          ok = false,
          message = "알 수 없는 오류가 발생 했습니다.<br />Error : " + e.Message
        });
      }
    }

    /// <summary>
    /// 인오더 메모 수정.
    /// </summary>
    /// <param name="im_seq"></param>
    /// <param name="memo"></param>
    /// <returns></returns>
    [HttpPost]
    public async Task<JsonResult> ModifyInOrderMemo(int im_seq, string memo)
    {
      try
      {
        ProjectEntityRepository per = new ProjectEntityRepository();
        var iMemo = await per.SelectInorderMemoOneAsync(im_seq);

        if (iMemo == null)
          return Json(new
          {
            ok = false,
            message = "수정하려는 메모를 찾을 수 없습니다."
          });

        iMemo.memo = HttpUtility.UrlDecode(memo);
        iMemo.modify_user = AppIdentity.user_seq;
        iMemo.modify_dt = Utils.NowKorea();

        await per.CreateOrUpdateInorderMemo(iMemo, CommonCodes.Update);

        return Json(new
        {
          ok = true,
          message = "메모를 저장 했습니다."
        });

      }
      catch (Exception e)
      {
        return Json(new
        {
          ok = false,
          message = "알 수 없는 오류가 발생 했습니다.<br />Error : " + e.Message
        });
      }
    }

    /// <summary>
    /// 인오더 메모 삭제.
    /// </summary>
    /// <param name="im_seq"></param>
    /// <returns></returns>
    [HttpPost]
    public async Task<JsonResult> DeleteInOrderMemo(int im_seq)
    {
      try
      {
        ProjectEntityRepository per = new ProjectEntityRepository();
        var iMemo = await per.SelectInorderMemoOneAsync(im_seq);

        if (iMemo == null)
          return Json(new
          {
            ok = false,
            message = "삭제하려는 메모를 찾을 수 없습니다."
          });

        await per.CreateOrUpdateInorderMemo(iMemo, CommonCodes.Delete);

        return Json(new
        {
          ok = true,
          message = "메모를 삭제 했습니다."
        });

      }
      catch (Exception e)
      {
        return Json(new
        {
          ok = false,
          message = "알 수 없는 오류가 발생 했습니다.<br />Error : " + e.Message
        });
      }
    }

    /// <summary>
    /// 인오더 파일 등록
    /// </summary>
    /// <param name="i_seq"></param>
    /// <param name="file"></param>
    /// <returns></returns>
    [HttpPost]
    public async Task<JsonResult> CreateInorderFile(int i_seq, HttpPostedFileBase file)
    {
      try
      {
        FileUpload fiUpload = new FileUpload();
        string path = Server.MapPath("~/UploadedFiles");
        string subPath = "inorder\\" + i_seq;

        var result = fiUpload.UploadCommon(path, subPath, file);

        if (!result.status)
          return Json(new
          {
            ok = false,
            message = result.statusMessage
          });

        ProjectEntityRepository per = new ProjectEntityRepository();

        inorder_file inf = new inorder_file()
        {
          i_seq = i_seq
            ,
          file_dir = Path.Combine(Utils.GetRootUrl(Request), result.dbPath)
            ,
          file_origin_path = result.filePath
            ,
          file_path = result.name
            ,
          file_extension = result.extension
            ,
          create_dt = Utils.NowKorea()
            ,
          create_user = AppIdentity.user_seq
        };

        await per.CreateOrDeleteInorderFile(inf, CommonCodes.Create);

        return Json(new
        {
          ok = true,
          message = "오더 파일을 등록 했습니다.",
          result = inf
        });
      }
      catch (Exception e)
      {
        return Json(new
        {
          ok = false,
          message = "알 수 없는 오류가 발생 했습니다."
        });
      }
    }

    /// <summary>
    /// 인오더 파일 삭제.
    /// </summary>
    /// <param name="if_seq"></param>
    /// <returns></returns>
    [HttpPost]
    public async Task<JsonResult> DeleteInorderFile(int if_seq)
    {
      try
      {
        ProjectEntityRepository per = new ProjectEntityRepository();

        var iFile = await per.SelectInorderFileOneAsync(if_seq);

        if (iFile == null)
        {
          return Json(new
          {
            ok = false,
            message = "삭제하고자 하는 파일을 찾지 못했습니다."
          });
        }

        iFile.remove_dt = Utils.NowKorea();
        iFile.remove_user = AppIdentity.user_seq;
        await per.CreateOrDeleteInorderFile(iFile, CommonCodes.Delete);

        return Json(new
        {
          ok = true,
          message = "삭제했습니다."
        });

      }
      catch (Exception e)
      {
        return Json(new
        {
          ok = false,
          message = "알 수 없는 오류가 발생 했습니다."
        });
      }
    }

    /// <summary>
    /// 인오더 임시 파일  등록(이현창)
    /// </summary>
    /// <param name="uploadFolder"></param>
    /// <param name="file"></param>
    /// <returns></returns>
    [HttpPost]
    public async Task<JsonResult> CreateTempInorderFile(string uploadFolder, HttpPostedFileBase file)
    {
      try
      {
        FileUpload fiUpload = new FileUpload();
        string path = Server.MapPath("~/UploadedFiles");
        var rst = fiUpload.UploadTemp(path, "Temp", uploadFolder, file);
        //TODO : 검색엔진에서 파일을 읽어서 주요 정보 추출 프로세스 추가 예정

        inorder_file inf = new inorder_file()
        {

          file_dir = rst.dbPath
            ,
          file_origin_path = rst.filePath
            ,
          file_path = rst.name
            ,
          file_extension = rst.extension
        };

        return Json(new
        {
          ok = true,
          message = "Upload Complete.",
          result = inf
        });

      }
      catch (Exception e)
      {
        return Json(new
        {
          ok = false,
          message = "[Error]알 수 없는 오류가 발생 했습니다."
        });
      }
    }

    /// <summary>
    /// 인오더 임시 파일 삭제
    /// </summary>
    /// <param name="file_origin_path"></param>
    /// <returns></returns>
    [HttpPost]
    public async Task<JsonResult> DeleteInorderTempFile(string file_origin_path)
    {
      try
      {
        FileUpload fiUpload = new FileUpload();

        string path = Server.MapPath("~/UploadedFiles");

        //실 파일 삭제
        if (!fiUpload.DeleteFile(file_origin_path))
          return Json(new
          {
            ok = false
              ,
            message = "경로에서 삭제하려는 파일을 찾을 수 없습니다."
          });

        return Json(new
        {
          ok = true,
          message = "파일을 삭제 했습니다."
        });

      }
      catch (Exception e)
      {
        return Json(new
        {
          ok = false,
          message = "알 수 없는 오류가 발생 했습니다."
        });
      }
    }

    /// <summary>
    /// 인오더 담당 컨설턴트 등록
    /// </summary>
    /// <param name="i_seq"></param>
    /// <param name="director_seq"></param>
    /// <returns></returns>
    [HttpPost]
    public async Task<JsonResult> SaveInorderDirector(int i_seq, int director_seq)
    {
      try
      {
        bool isOk = true;

        AccountRepository ar = new AccountRepository();
        var user = await ar.FindUserBySeqAsync(director_seq);
        if (user == null)
          return Json(new
          {
            ok = false,
            message = "등록하려는 담당 컨설턴트를 찾을 수 없습니다."
          });


        ProjectEntityRepository per = new ProjectEntityRepository();
        InorderCreateUpdateModel model = new InorderCreateUpdateModel();

        model.data = await per.SelectInorderOneAsync(i_seq);
        if (model.data == null)
          return Json(new
          {
            ok = false,
            message = "담당 컨설턴트를 등록하려는 오더를 찾을 수 없습니다."
          });

        model.data.director_seq = director_seq;
        model.data.director_dt = Utils.NowKorea();

        await per.CreateOrUpdateInorder(model, CommonCodes.Update, AppIdentity.user_seq);

        string message = "담당 컨설턴트를 저장 했습니다.";
        string failMessage = "담당 컨설턴트를 저장 했지만";

        string url = "http://univision.unicosearch.com/Project/InorderDetail?i_seq=" + model.data.i_seq;
        //이메일, 알림 발송. 메모 남김 작성 해야함. start
        List<string> toArr = new List<string>();
        toArr.Add(user.email);

        MailService service = new MailService();
        var mailData = new InOrderDirectorDto()
        {
          ToArr = toArr.ToArray(),
          From = new System.Net.Mail.MailAddress("noreply@unicosearch.com", "noreply"),
          companyName = model.data.clt_name,
          orderDt = Utils.ConvertDateTimeToString(model.data.inorder_dt),
          orderType = Utils.ReturnInOrderTypeTxt(model.data.inorder_type),
          contactName = model.data.cc_name,
          contactDept = model.data.cc_division,
          contactPosition = model.data.cc_position,
          contactPhone = string.IsNullOrWhiteSpace(model.data.cc_cell_phone) ? model.data.cc_phone : model.data.cc_cell_phone,
          contactEmail = model.data.cc_email,
          jobDesc = model.data.can_jobdesc,
          contents = model.data.can_contents,
          url = url,
          createDt = Utils.ConvertDateTimeToString(model.data.create_dt)
        };
        var mailResult = service.SendInOrderDirectorMail(mailData, new InOrderDirectorTemplete());

        //메일 발송 실패시.
        if (!mailResult.isSend)
        {
          isOk = false;
          failMessage += " 메일 발송에 실패 했습니다.";
        }

        alarm_message aMessage = new alarm_message()
        {
          message = model.data.clt_name + " Order 담당 컨설턴트로 등록 되었습니다.",
          create_dt = Utils.NowKorea(),
          href_url = "/Project/InorderDetail?i_seq=" + model.data.i_seq
        };
        List<alarm_user> aList = new List<alarm_user>();
        alarm_user aUser = new alarm_user()
        {
          uv_seq = director_seq,
          is_read = 0
        };
        aList.Add(aUser);

        ApiEntityRepository aer = new ApiEntityRepository();
        await aer.CreateAlarm(aMessage, aList);

        inorder_memo iMemo = new inorder_memo()
        {
          i_seq = model.data.i_seq,
          memo = "[" + user.name + "]님이" + model.data.clt_name + " Order 담당 컨설턴트로 등록 되었습니다.",
          create_dt = Utils.NowKorea(),
          create_user = AppIdentity.user_seq,
          modify_dt = Utils.NowKorea(),
          modify_user = AppIdentity.user_seq
        };

        await per.CreateOrUpdateInorderMemo(iMemo, CommonCodes.Create);

        //이메일, 알림 발송. 메모 남김 작성 해야함. end

        if (isOk)
          return Json(new
          {
            ok = isOk,
            message = "담당 컨설턴트를 저장 했습니다."
          });
        else
          return Json(new
          {
            ok = isOk,
            message = failMessage
          });

      }
      catch (Exception e)
      {
        return Json(new
        {
          ok = false,
          message = "알 수 없는 오류가 발생 했습니다.<br />Error : " + e.Message
        });
      }
    }

    public async Task<ActionResult> CreateInorderProject(int i_seq)
    {
      ProjectRepository pr = new ProjectRepository();
      var inorder = await pr.SelectInorderOneAsync(i_seq);

      ProjectCreateModel model = new ProjectCreateModel()
      {
        p_seq = 0,
        c_seq = (int)inorder.c_seq,
        company_name = inorder.client_name,
        pjt_type = 1,
        recruit_number = 1,
        assign_task = inorder.can_jobdesc,
        requirement = inorder.can_contents,
        is_kor_resume = 1,
        is_eng_resume = 1,
        i_seq = i_seq
      };

      if (model.placeList.Count == 0)
      {
        model.placeList.Add(new pjt_place()
        {
          code1 = "101000",
          code2 = "101000",
          area1 = "서울",
          area2 = "서울전체"
        });
      }

      if (model.amList.Count == 0)
      {
        model.amList.Add(new pjt_director()
        {
          uv_seq = (int)inorder.director_seq,
          name = inorder.director_name
        });
      }

      ApiRepository ar = new ApiRepository();
      model.positionList = await ar.SelectPositionCodeListAsync();
      model.type = "";

      return View("ProjectCreate", model);
    }

    #endregion
    */

    #region 프로젝트 목록

    public async Task<ActionResult> NewAllProjectList(ProjectSearchModel search, int CurrentPage = 1, int ItemsPerPage = 20)
    {
      ApiRepository ar = new ApiRepository();
      search.positionList = await ar.SelectPositionCodeListAsync();

      ProjectListViewModel model = new ProjectListViewModel
      {
        search = search
        ,
        PagingInfo = new PagingInfo()
        {
          CurrentPage = CurrentPage,
          ItemsPerPage = ItemsPerPage,
        }
      };

      return View(model);
    }

    public async Task<ActionResult> NewMyProjectList(ProjectSearchModel search, int CurrentPage = 1, int ItemsPerPage = 20)
    {
      ApiRepository ar = new ApiRepository();
      search.positionList = await ar.SelectPositionCodeListAsync();

      ProjectListViewModel model = new ProjectListViewModel
      {
        search = search
        ,
        PagingInfo = new PagingInfo()
        {
          CurrentPage = CurrentPage,
          ItemsPerPage = ItemsPerPage,
        }
      };

      return View(model);
    }

    public async Task<ActionResult> NewMyProjectAjxList(ProjectSearchModel search, int CurrentPage = 1, int ItemsPerPage = 20)
    {
      search.replace_search_text();


      ProjectRepository pr = new ProjectRepository();


      var data = await pr.SelectProjectList(search, (CurrentPage - 1) * ItemsPerPage, ItemsPerPage, AppIdentity.user_seq);
      ProjectStatusCntModel cntData = new ProjectStatusCntModel();
      int totalCnt = 0;
      foreach (var group in data.group_count)
      {
        if (group.code == 1)
          cntData.progressCnt = group.count;
        else if (group.code == 2)
          cntData.waitCnt = group.count;
        else if (group.code == 3)
          cntData.failCnt = group.count;
        else if (group.code == 4)
          cntData.completeCnt = group.count;
        else if (group.code == 5)
          cntData.successCnt = group.count;

        totalCnt += group.count;
      }

      cntData.allCnt = totalCnt;

      ProjectListViewModel model = new ProjectListViewModel
      {
        search = search,
        list = data.Items,
        cntData = cntData,
        PagingInfo = new PagingInfo()
        {
          CurrentPage = CurrentPage,
          ItemsPerPage = ItemsPerPage,
          TotalItems = data.totalCount
        }
      };


      //var list = ser.SelectCandidateList(search, page, pageSize, out totalCount);

      return View(model);
    }


    public async Task<ActionResult> NewMyProjectExcelList(ProjectSearchModel search, List<string> exl_pjt_sub)
    {
      try
      {

        search.replace_search_text();

        ProjectRepository per = new ProjectRepository();

        var excel = new Excel<ProjectListNewExcelModel>();

        var list = await per.SelectProjectExcelList(search, AppIdentity.user_seq);

        var data = excel.WriteExcel(list, exl_pjt_sub, "MyProjectList");

        return File(data.FileContents, data.ContentType, data.FileDownloadName);
      }
      catch (Exception e)
      {
        throw e;
      }


      ProjectListViewModel model = new ProjectListViewModel();


      //var list = ser.SelectCandidateList(search, page, pageSize, out totalCount);

      return View(model);
    }

    public async Task<PartialViewResult> PartialProjectSearchBusiList(ProjectSearchModel search)
    {

      return PartialView(search);
    }

    public async Task<PartialViewResult> PartialProjectSearchJobList(ProjectSearchModel search)
    {

      return PartialView(search);
    }

    public async Task<ActionResult> NewAllProjectExcelList(ProjectSearchModel search, List<string> exl_pjt_sub)
    {
      try
      {

        search.replace_search_text();

        SearchEngineRepository ser = new SearchEngineRepository();

        var excel = new Excel<ProjectListNewExcelModel>();

        var list = await ser.SelectProjectList(search, 1, 999999, AppIdentity.name);
        List<ProjectListNewExcelModel> excelData = new List<ProjectListNewExcelModel>();
        foreach (var d in list.Items)
        {
          excelData.Add(new ProjectListNewExcelModel()
          {
            프로젝트_코드 = d.p_seq.ToString(),
            고객사_코드 = d.c_seq.ToString(),
            고객사_국문명 = d.company_name,
            고객사_영문명 = d.company_name_eng,
            프로젝트_구분 = d.pjt_type_str,
            프로젝트_진행상태 = d.pjt_status_str,
            구인제목 = d.title,
            예상_연봉 = d.exp_salary_won_str,
            수수료율 = d.fee_rate_str,
            포지션명 = d.position_str,
            직급분류 = d.position_name,
            주_산업_대분류 = d.business_names1,
            주_산업_소분류 = d.business_names2,
            보조_산업_대분류 = d.sub_business_names1,
            보조_산업_소분류 = d.sub_business_names2,
            주_직무_대분류 = d.job_names1,
            주_직무_소분류 = d.job_names2,
            보조_직무_대분류 = d.sub_job_names1,
            보조_직무_소분류 = d.sub_job_names2,
            AM = d.am_names,
            SM = d.searcher_names.Replace("\n", "\n,"),
            최초등록일 = Utils.ConvertDateTimeToString(d.create_dt),
            최종수정일 = Utils.ConvertDateTimeToString(d.modify_dt),
            종료일 = Utils.ConvertDateTimeToString(d.close_dt),
            채용후보자정보 = d.hire_info,
            PE_구분 = (d.is_pe == 1 ? "PE" : "")

          });
        }
        var data = excel.WriteExcel(excelData, exl_pjt_sub, "ProjectList");

        return File(data.FileContents, data.ContentType, data.FileDownloadName);
      }
      catch (Exception e)
      {
        throw e;
      }


      ProjectListViewModel model = new ProjectListViewModel();


      //var list = ser.SelectCandidateList(search, page, pageSize, out totalCount);

      return View(model);
    }

    public async Task<ActionResult> NewAllProjectAjxList(ProjectSearchModel search, int CurrentPage = 1, int ItemsPerPage = 20)
    {
      search.replace_search_text();


      SearchEngineRepository ser = new SearchEngineRepository();
      CandidateRepository cr = new CandidateRepository();

      var data = await ser.SelectProjectList(search, CurrentPage, ItemsPerPage, AppIdentity.name);
      ProjectStatusCntModel cntData = new ProjectStatusCntModel();
      int totalCnt = 0;
      foreach (var group in data.group_count)
      {
        if (group.code == 1)
          cntData.progressCnt = group.count;
        else if (group.code == 2)
          cntData.waitCnt = group.count;
        else if (group.code == 3)
          cntData.failCnt = group.count;
        else if (group.code == 4)
          cntData.completeCnt = group.count;
        else if (group.code == 5)
          cntData.successCnt = group.count;

        totalCnt += group.count;
      }

      cntData.allCnt = totalCnt;

      ProjectListViewModel model = new ProjectListViewModel
      {
        search = search,
        list = data.Items,
        cntData = cntData,
        PagingInfo = new PagingInfo()
        {
          CurrentPage = CurrentPage,
          ItemsPerPage = ItemsPerPage,
          TotalItems = data.totalCount
        }
      };


      //var list = ser.SelectCandidateList(search, page, pageSize, out totalCount);

      return View(model);
    }


    public async Task<ActionResult> AllProjectList(MyProjectSearchModel search, int page = 1)
    {
      ProjectRepository pr = new ProjectRepository();

      if (search.excelDown)
      {
        search.excelDown = false;
        var excel = new Excel<AllProjectListExcelModel>();
        var excelData = await pr.SelectAllProjectListWithoutCountAsnyc(search);
        return excel.WriteExcel(excelData, "전체 프로젝트 목록");
      }

      ApiRepository ar = new ApiRepository();
      search.positionList = await ar.SelectPositionCodeListAsync();


      int totalCount = 0;

      //한 페이지당 몇개의 행을 보여줄 것인지
      int pageSize = AppPaging.PageLength10;

      var cntData = await pr.SelectAllProjectStatusCountAsync(search);
      var list = pr.SelectAllProjectList(search, (page - 1) * pageSize, pageSize, out totalCount);

      MyProjectListModel model = new MyProjectListModel()
      {
        search = search,
        cntData = cntData,
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

      //  return View();
    }

    /// <summary>
    /// 마이프로젝트 목록
    /// </summary>
    /// <returns></returns>
    public async Task<ActionResult> MyProject()
    {
      ProjectRepository pr = new ProjectRepository();

      var kpiData = await pr.SelectMyProjectKPIList(AppIdentity.user_seq);

      MyProjectListModel model = new MyProjectListModel()
      {
        kpiData = kpiData,

      };


      return View(model);
    }

    /// <summary>
    /// 마이프로젝트 리스트 채용
    /// </summary>
    /// <param name="search"></param>
    /// <param name="page"></param>
    /// <returns></returns>
    public async Task<ActionResult> MyProjectList(MyProjectSearchModel search, int page = 1)
    {
      ProjectRepository pr = new ProjectRepository();

      if (search.excelDown)
      {
        //return await MyProjectListExcelDown(search);
        return RedirectToAction("MyProjectListExcelDown", search);
      }

      ApiRepository ar = new ApiRepository();
      search.positionList = await ar.SelectPositionCodeListAsync();

      int totalCount = 0;

      //한 페이지당 몇개의 행을 보여줄 것인지
      int pageSize = AppPaging.PageLength10;

      var cntData = await pr.SelectProjectStatusCountAsync(search, AppIdentity.user_seq);
      var list = pr.SelectMyProjectList(search, AppIdentity.user_seq, (page - 1) * pageSize, pageSize, out totalCount);

      MyProjectListModel model = new MyProjectListModel()
      {
        search = search,
        cntData = cntData,
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

    /// <summary>
    /// 마이 프로젝트 리스트 평판조회
    /// </summary>
    /// <param name="search"></param>
    /// <param name="page"></param>
    /// <returns></returns>
    public async Task<ActionResult> MyProjectRepList(MyProjectSearchModel search, int page = 1)
    {
      ProjectRepository pr = new ProjectRepository();

      int totalCount = 0;

      //한 페이지당 몇개의 행을 보여줄 것인지
      int pageSize = AppPaging.PageLength10;

      var cntData = await pr.SelectProjectRepStatusCountAsync(search, AppIdentity.user_seq);
      var list = pr.SelectMyProjectRepList(search, AppIdentity.user_seq, (page - 1) * pageSize, pageSize, out totalCount);

      MyProjectListModel model = new MyProjectListModel()
      {
        search = search,
        cntData = cntData,
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

    /// <summary>
    /// 공유 프로젝트 생성 팝업
    /// </summary>
    /// <param name="p_seq"></param>
    /// <returns></returns>
    public PartialViewResult ShareProjectCreate(int p_seq)
    {
      ViewBag.p_seq = p_seq;
      return PartialView();
    }

    /// <summary>
    /// 공유 프로젝트 등록
    /// </summary>
    /// <param name="p_seq"></param>
    /// <param name="share_comments"></param>
    /// <param name="share_fee_rate"></param>
    /// <param name="secret_info"></param>
    /// <returns></returns>
    [HttpPost]
    public async Task<JsonResult> ShareProjectCreate(int p_seq, string share_comments, int share_fee_rate, string secret_info)
    {
      try
      {
        ProjectEntityRepository per = new ProjectEntityRepository();

        var project = await per.SelectProjectOneAsync(p_seq);
        if (project == null)
          return Json(new
          {
            ok = false,
            message = "해당 프로젝트를 찾을 수 없습니다."
          });

        project.is_share_pjt = 1;
        project.share_comments = share_comments;
        project.share_fee_rate = share_fee_rate;
        project.secret_info = secret_info;
        project.share_dt = Utils.NowKorea();
        project.modify_dt = Utils.NowKorea();
        project.modify_user = AppIdentity.user_seq;

        await per.UpdateProjectAsync(project, "U", AppIdentity.user_seq, 1);

        return Json(new
        {
          ok = true,
          message = "공유 프로젝트로 등록 했습니다."
        });
      }
      catch (Exception e)
      {
        return Json(new
        {
          ok = false,
          message = "알 수 없는 오류가 발생 했습니다."
        });
      }
    }

    [HttpPost]
    public async Task<JsonResult> ShareProjectCancel(int p_seq)
    {
      try
      {
        ProjectEntityRepository per = new ProjectEntityRepository();

        var project = await per.SelectProjectOneAsync(p_seq);
        if (project == null)
          return Json(new
          {
            ok = false,
            message = "해당 프로젝트를 찾을 수 없습니다."
          });

        project.is_share_pjt = 0;
        project.modify_dt = Utils.NowKorea();
        project.modify_user = AppIdentity.user_seq;

        await per.UpdateProjectAsync(project, "U", AppIdentity.user_seq, 1);

        return Json(new
        {
          ok = true,
          message = "공유 프로젝트를 취소 했습니다."
        });
      }
      catch (Exception e)
      {
        return Json(new
        {
          ok = false,
          message = "알 수 없는 오류가 발생 했습니다."
        });
      }
    }

    [HttpPost]
    public async Task<JsonResult> CoworkProjectCancel(int p_seq)
    {
      try
      {
        ProjectEntityRepository per = new ProjectEntityRepository();

        var project = await per.SelectProjectOneAsync(p_seq);
        if (project == null)
          return Json(new
          {
            ok = false,
            message = "해당 프로젝트를 찾을 수 없습니다."
          });

        project.is_cowork = 0;
        project.modify_dt = Utils.NowKorea();
        project.modify_user = AppIdentity.user_seq;

        await per.UpdateProjectAsync(project, "U", AppIdentity.user_seq, 1);

        return Json(new
        {
          ok = true,
          message = "코웍 프로젝트를 취소 했습니다."
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

    /// <summary>
    /// 공유 프로젝트 리스트
    /// </summary>
    /// <param name="search"></param>
    /// <param name="page"></param>
    /// <returns></returns>
    public async Task<ActionResult> ShareProjectList(MyProjectSearchModel search, int page = 1)
    {
      ProjectRepository pr = new ProjectRepository();

      if (search.excelDown)
      {
        search.excelDown = false;
        var excel = new Excel<ProjectListExcelModel>();
        var excelData = await pr.SelectShareProjectListWithoutCountAsync(search);
        return excel.WriteExcel(excelData, "공유 프로젝트 목록");
      }

      int totalCount = 0;

      //한 페이지당 몇개의 행을 보여줄 것인지
      int pageSize = AppPaging.PageLength10;

      var cntData = await pr.SelectShareProjectStatusCountAsync(search);
      var list = pr.SelectShareProjectList(search, (page - 1) * pageSize, pageSize, out totalCount);

      MyProjectListModel model = new MyProjectListModel()
      {
        search = search,
        cntData = cntData,
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

    /// <summary>
    /// 전담코웤 프로젝트 모델
    /// </summary>
    /// <param name="search"></param>
    /// <param name="page"></param>
    /// <returns></returns>
    public async Task<ActionResult> CoWorkProjectList(MyProjectSearchModel search, int page = 1)
    {
      ProjectRepository pr = new ProjectRepository();

      if (search.excelDown)
      {
        search.excelDown = false;
        var excel = new Excel<ProjectListExcelModel>();
        var excelData = await pr.SelectCoWorkProjectListWithoutCountAsnyc(search);
        return excel.WriteExcel(excelData, "전담 프로젝트 목록");
      }

      int totalCount = 0;

      //한 페이지당 몇개의 행을 보여줄 것인지
      int pageSize = AppPaging.PageLength10;

      var cntData = await pr.SelectCoWorkProjectStatusCountAsync(search);
      var list = pr.SelectCoWorkProjectList(search, (page - 1) * pageSize, pageSize, out totalCount);

      MyProjectListModel model = new MyProjectListModel()
      {
        search = search,
        cntData = cntData,
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

    /// <summary>
    /// 전담코웤 프로젝트 알림 발송.
    /// </summary>
    /// <param name="p_seq"></param>
    /// <returns></returns>
    [HttpPost]
    public async Task<JsonResult> CreateCoWorkAlram(int p_seq)
    {
      try
      {
        ProjectEntityRepository per = new ProjectEntityRepository();
        var project = await per.SelectProjectOneAsync(p_seq);

        if (project == null)
          return Json(new
          {
            ok = false,
            message = "알림 보내려는 프로젝트를 찾을 수 없습니다."
          });

        project.is_cowork = 1;
        project.cowork_dt = Utils.NowKorea();
        project.modify_dt = Utils.NowKorea();
        project.modify_user = AppIdentity.user_seq;

        alarm_message aMessage = new alarm_message()
        {
          href_url = "/Project/ProjectDetail?p_seq=" + p_seq,
          message = "[" + project.title + "] 프로젝트가 Co-work 프로젝트로 등록 되었습니다.",
          create_dt = Utils.NowKorea()
        };

        List<alarm_user> aList = new List<alarm_user>();
        AccountRepository ar = new AccountRepository();

        var userList = await ar.SelectAllUser();

        foreach (var user in userList)
        {
          alarm_user alarm = new alarm_user()
          {
            uv_seq = user.uv_seq
              ,
            is_read = 0
          };

          aList.Add(alarm);
        }

        await per.CreateCoWorkProjectAlarm(project, aMessage, aList);

        return Json(new
        {
          ok = true,
          message = "전담코웤 알림을 보냈습니다."
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

    #endregion

    /// <summary>
    /// 키프로젝트 생성
    /// </summary>
    /// <param name="p_seq"></param>
    /// <returns></returns>
    [HttpPost]
    public async Task<JsonResult> CreateKeyProject(int p_seq)
    {
      try
      {
        ProjectEntityRepository per = new ProjectEntityRepository();
        var p_mykey = await per.SelectProjectMyKeyOneAsync(p_seq, AppIdentity.user_seq);

        if (p_mykey != null)
          return Json(new
          {
            ok = false,
            message = "이미 키 프로젝트로 등록된 프로젝트 입니다."
          });

        p_mykey = new pjt_mykey()
        {
          p_seq = p_seq,
          uv_seq = AppIdentity.user_seq,
          create_dt = Utils.NowKorea()
        };


        await per.CreateOrRemoveMyKey(p_mykey, "C");

        return Json(new
        {
          ok = true,
          message = "Key 프로젝트 등록 완료."
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

    /// <summary>
    /// 키프로젝트 제거
    /// </summary>
    /// <param name="p_seq"></param>
    /// <returns></returns>
    [HttpPost]
    public async Task<JsonResult> RemoveKeyProject(int p_seq)
    {
      try
      {
        ProjectEntityRepository per = new ProjectEntityRepository();
        var p_mykey = await per.SelectProjectMyKeyOneAsync(p_seq, AppIdentity.user_seq);

        if (p_mykey == null)
          return Json(new
          {
            ok = false,
            message = "해제 할 키 프로젝트가 없습니다."
          });

        await per.CreateOrRemoveMyKey(p_mykey, "D");

        return Json(new
        {
          ok = true,
          message = "Key 프로젝트 해제 완료."
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


    public async Task<JsonResult> MyKeyProjectListAjx(KeyProjectSearchModel search)
    {
      try
      {
        search.uv_seq = AppIdentity.user_seq;

        StatisticsRepository sr = new StatisticsRepository();
        var list = await sr.SelectKeyProject(search);

        return Json(new
        {
          ok = true,
          list = list
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

    public async Task<JsonResult> MyInvoiceListAjx(MyInvoiceSearchModel search)
    {
      try
      {
        search.uv_seq = AppIdentity.user_seq;

        StatisticsRepository sr = new StatisticsRepository();
        var list = await sr.SelectMyInvoice(search);

        return Json(new
        {
          ok = true,
          list = list
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

    public async Task<ActionResult> MyKeyProjectList(KeyProjectSearchModel search)
    {
      KeyProjectListModel model = new KeyProjectListModel()
      {
        search = search
        //  ,
        //list = list
      };

      return View(model);
    }

    public async Task<ActionResult> MyInvoiceList(KeyProjectSearchModel search)
    {
      KeyProjectListModel model = new KeyProjectListModel()
      {
        search = search
        //  ,
        //list = list
      };

      return View(model);
    }

    #region 프로젝트 등록

    /// <summary>
    /// 프로젝트 등록 - 채용
    /// </summary>
    /// <param name="p_seq"></param>
    /// <returns></returns>
    public async Task<ActionResult> ProjectCreate(int p_seq = 0, int c_seq = 0, string type = "")
    {
      ProjectRepository pr = new ProjectRepository();

      var data = await pr.SelectProjectOneAsync(p_seq);
      if (data == null)
        data = new project()
        {
          recruit_number = 1,
          is_kor_resume = 1,
          is_eng_resume = 1
        };


      ProjectCreateModel model = new ProjectCreateModel()
      {
        p_seq = p_seq,
        log_cnt = data.log_cnt,
        c_seq = data.c_seq,
        company_name = data.company_name,
        pjt_type = data.pjt_type,
        recruit_number = data.recruit_number,
        is_posting = data.is_posting,
        title = data.title,
        position_seq = data.position_seq,
        position_str = data.position_str,
        experience_type = data.experience_type,
        expreience_number = data.expreience_number,
        edu_code = data.edu_code,
        edu_name = data.edu_name,
        foreign_lang = data.foreign_lang,
        foreign_lang_name = data.foreign_lang_name,
        foreign_level = data.foreign_level,
        assign_task = data.assign_task,
        requirement = data.requirement,
        internal_contents = data.internal_contents,
        is_kor_resume = data.is_kor_resume,
        is_eng_resume = data.is_eng_resume,
        is_portfolio = data.is_portfolio,
        is_self_introduction = data.is_self_introduction,
        is_etc = data.is_etc,
        etc_comment = data.etc_comment,
        is_pre_interview = data.is_pre_interview,
        is_number = data.is_number,
        interview_number = data.interview_number,
        is_personality_test = data.is_personality_test,
        gender_type = data.gender_type,
        start_age = data.start_age,
        end_age = data.end_age,
        target_school_nm = data.target_school_nm,
        //target_school_name = data.target_school_name,
        //target_school_campus = data.target_school_campus,
        target_major_nm = data.target_major_nm,
        //target_major_name = data.target_major_name,
        target_company_nm = data.target_company_nm,
        //target_company_name = data.target_company_name,
        confidentiallity = data.confidentiallity,
        exp_salary = data.exp_salary,
        exp_salary_won = data.exp_salary_won,
        currency_cd = data.currency_cd,
        fee_rate = data.fee_rate,
        business_code1 = data.business_code1,
        business_code2 = data.business_code2,
        business_name1 = data.business_names1,
        business_name2 = data.business_names2,
        sub_business_code1 = data.sub_business_code1,
        sub_business_code2 = data.sub_business_code2,
        sub_business_name1 = data.sub_business_names1,
        sub_business_name2 = data.sub_business_names2,
        no_business = data.no_business,
        no_job = data.no_job,
        job_code1 = data.job_code1,
        job_code2 = data.job_code2,
        job_name1 = data.job_names1,
        job_name2 = data.job_names2,
        sub_job_code1 = data.sub_job_code1,
        sub_job_code2 = data.sub_job_code2,
        sub_job_name1 = data.sub_job_names1,
        sub_job_name2 = data.sub_job_names2,
        is_pe = data.is_pe
      };

      //client에서 넘어온 project 생성의 경우.
      if (c_seq != 0 && p_seq == 0)
      {
        ClientRepository cr = new ClientRepository();

        var client = await cr.SelectClientOneAsync(c_seq);

        model.p_seq = 0;
        model.c_seq = client.c_seq;
        model.company_name = client.kor_name;

        var clientAmList = await cr.SelectClientAmListAsync(c_seq);

        model.amList = clientAmList.Select(x => new pjt_director
        {
          p_seq = 0
            ,
          uv_seq = x.uv_seq
            ,
          name = x.am_name
            ,
          ud_name = x.ud_name
        }).ToList();

        model.business_code1 = client.biz_industry_code1;
        model.business_code2 = client.biz_industry_code2;
        model.business_name2 = client.biz_industry_name2;
      }
      //프로젝트 수정의 경우.
      else if (p_seq != 0)
      {
        model.amList = await pr.SelectProjectAmListAsync(p_seq);
        model.searcherList = await pr.SelectProjectSearcherListAsync(p_seq);
        //model.jobList = await pr.SelectProjectJobListAsync(p_seq);
        model.placeList = await pr.SelectProjectPlaceListAsync(p_seq);
        model.keywordList = await pr.SelectProjectKeywordListAsync(p_seq);
      }

      if (model.placeList.Count == 0)
      {
        model.placeList.Add(new pjt_place()
        {
          code1 = "101000",
          code2 = "101000",
          area1 = "서울",
          area2 = "서울전체"
        });
      }
      if (model.searcherList.Count == 0)
      {
        model.searcherList.Add(new pjt_manager()
        {
          uv_seq = AppIdentity.user_seq,
          name = AppIdentity.name
        });
      }

      ApiRepository ar = new ApiRepository();
      model.positionList = await ar.SelectPositionCodeListAsync();

      model.type = type;

      return View(model);
    }

    /// <summary>
    /// 프로젝트 등록 - 채용
    /// </summary>
    /// <param name="data"></param>
    /// <returns></returns>
    [HttpPost]
    public async Task<JsonResult> ProjectCreate(ProjectCreateModel data)
    {
      try
      {
        //throw new Exception();

        ProjectEntityRepository per = new ProjectEntityRepository();

        ProjectCreateUpdateModel model = new ProjectCreateUpdateModel();

        //신규, 수정 변수
        string state = "";

        //신규
        if (data.p_seq == 0 || data.type == "pjtCopy")
        {
          model.data = new project()
          {
            pjt_type = data.pjt_type,
            c_seq = data.c_seq,
            recruit_number = data.recruit_number,
            is_posting = data.is_posting,
            is_pe = data.is_pe,
            title = data.title,
            position_seq = data.position_seq,
            position_str = data.position_str,
            experience_type = data.experience_type,
            expreience_number = data.expreience_number,
            edu_code = data.edu_code,
            edu_name = data.edu_name,
            foreign_lang = data.foreign_lang,
            foreign_lang_name = data.foreign_lang_name,
            foreign_level = data.foreign_level,
            assign_task = HttpUtility.UrlDecode(data.assign_task),
            requirement = HttpUtility.UrlDecode(data.requirement),
            internal_contents = HttpUtility.UrlDecode(data.internal_contents),
            is_kor_resume = data.is_kor_resume,
            is_eng_resume = data.is_eng_resume,
            is_portfolio = data.is_portfolio,
            is_self_introduction = data.is_self_introduction,
            is_etc = data.is_etc,
            etc_comment = data.etc_comment,
            is_pre_interview = data.is_pre_interview,
            is_number = data.is_number,
            interview_number = data.interview_number,
            is_personality_test = data.is_personality_test,
            gender_type = data.gender_type,
            start_age = data.start_age,
            end_age = data.end_age,
            target_school_nm = data.target_school_nm,
            target_major_nm = data.target_major_nm,
            target_company_nm = data.target_company_nm,
            exp_salary = data.exp_salary,
            exp_salary_won = data.exp_salary_won,
            currency_cd = data.currency_cd,
            fee_rate = data.fee_rate,
            pjt_status = ProjectStatusCode.progress,
            is_share_pjt = 0,
            business_code1 = data.business_code1,
            business_code2 = data.business_code2,
            job_code1 = data.job_code1,
            job_code2 = data.job_code2,
            sub_business_code1 = data.sub_business_code1,
            sub_business_code2 = data.sub_business_code2,
            sub_job_code1 = data.sub_job_code1,
            sub_job_code2 = data.sub_job_code2,
            no_business = data.no_business,
            no_job = data.no_job,
            create_dt = Utils.NowKorea(),
            open_dt = Utils.NowKorea(),
            create_user = AppIdentity.user_seq,
            modify_dt = Utils.NowKorea(),
            modify_user = AppIdentity.user_seq
          };

          state = CommonCodes.Create;
        }
        else
        {

          model.data = await per.SelectProjectOneAsync(data.p_seq);
          if (model.data == null)
            return Json(new
            {
              ok = false
                ,
              message = "해당 프로젝트를 찾을 수 없습니다."
            });
          model.data.c_seq = data.c_seq;
          model.data.pjt_type = data.pjt_type;
          model.data.recruit_number = data.recruit_number;
          model.data.is_posting = data.is_posting;
          model.data.is_pe = data.is_pe;
          model.data.title = data.title;
          model.data.position_seq = data.position_seq;
          model.data.position_str = data.position_str;
          model.data.experience_type = data.experience_type;
          model.data.expreience_number = data.expreience_number;
          model.data.edu_code = data.edu_code;
          model.data.edu_name = data.edu_name;
          model.data.foreign_lang = data.foreign_lang;
          model.data.foreign_lang_name = data.foreign_lang_name;
          model.data.foreign_level = data.foreign_level;
          model.data.assign_task = HttpUtility.UrlDecode(data.assign_task);
          model.data.requirement = HttpUtility.UrlDecode(data.requirement);
          model.data.internal_contents = HttpUtility.UrlDecode(data.internal_contents);
          model.data.is_kor_resume = data.is_kor_resume;
          model.data.is_eng_resume = data.is_eng_resume;
          model.data.is_portfolio = data.is_portfolio;
          model.data.is_self_introduction = data.is_self_introduction;
          model.data.is_etc = data.is_etc;
          model.data.etc_comment = data.etc_comment;
          model.data.is_pre_interview = data.is_pre_interview;
          model.data.is_number = data.is_number;
          model.data.interview_number = data.interview_number;
          model.data.is_personality_test = data.is_personality_test;
          model.data.gender_type = data.gender_type;
          model.data.start_age = data.start_age;
          model.data.end_age = data.end_age;
          model.data.target_school_nm = data.target_school_nm;
          model.data.target_major_nm = data.target_major_nm;
          model.data.target_company_nm = data.target_company_nm;
          model.data.exp_salary = data.exp_salary;
          model.data.exp_salary_won = data.exp_salary_won;
          model.data.currency_cd = data.currency_cd;
          model.data.fee_rate = data.fee_rate;
          model.data.modify_dt = Utils.NowKorea();
          model.data.modify_user = AppIdentity.user_seq;
          model.data.no_business = data.no_business;
          model.data.no_job = data.no_job;
          model.data.business_code1 = data.business_code1;
          model.data.business_code2 = data.business_code2;
          model.data.job_code1 = data.job_code1;
          model.data.job_code2 = data.job_code2;
          model.data.sub_business_code1 = data.sub_business_code1;
          model.data.sub_business_code2 = data.sub_business_code2;
          model.data.sub_job_code1 = data.sub_job_code1;
          model.data.sub_job_code2 = data.sub_job_code2;

          model.deleteAmList = await per.SelectPjtDirectorListAsync(model.data.p_seq);
          model.deleteSearcherList = await per.SelectPjtContactListAsync(model.data.p_seq);
          //model.deleteBusiList = await per.SelectPjtBusiCodeListAsync(model.data.p_seq);
          //model.deleteJobList = await per.SelectPjtJobCodeListAsync(model.data.p_seq);
          model.deletePlaceList = await per.SelectPjtPlaceListAsync(model.data.p_seq);
          model.deleteKeywordList = await per.SelectPjtKeywordListAsync(model.data.p_seq);

          state = CommonCodes.Update;
        }

        //AM
        foreach (var am in data.amList)
        {

          var am_entry_date = Utils.NowKorea();
          if (model.deleteAmList.Where(x => x.uv_seq == am.uv_seq).Count() > 0)
          {
            am_entry_date = model.deleteAmList.Where(x => x.uv_seq == am.uv_seq).FirstOrDefault().entry_date.Value;
          }
          if (am_entry_date == null)
          {
            am_entry_date = Utils.NowKorea();
          }
          var director = new pjt_director()
          {
            p_seq = model.data.p_seq
              ,
            uv_seq = am.uv_seq
            ,
            entry_date = am_entry_date
          };

          model.amList.Add(director);
        }

        //Searcher
        foreach (var searcher in data.searcherList)
        {
          var sc_entry_date = Utils.NowKorea();
          if (model.deleteSearcherList.Where(x => x.uv_seq == searcher.uv_seq).Count() > 0)
          {
            sc_entry_date = model.deleteSearcherList.Where(x => x.uv_seq == searcher.uv_seq).FirstOrDefault().entry_date.Value;
          }

          var contact = new pjt_manager()
          {
            p_seq = model.data.p_seq
              ,
            uv_seq = searcher.uv_seq
            ,
            entry_date = sc_entry_date
          };

          model.searcherList.Add(contact);
        }

        //Place
        foreach (var place in data.placeList)
        {
          var pPlace = new pjt_place()
          {
            p_seq = model.data.p_seq,
            code1 = place.code1,
            area1 = place.area1,
            code2 = place.code2,
            area2 = place.area2
          };

          model.placeList.Add(pPlace);
        }

        //Keyword
        foreach (var keyword in data.keywordList)
        {
          var pKey = new pjt_keyword()
          {
            p_seq = model.data.p_seq,
            pk_name = keyword.pk_name
          };

          model.keywordList.Add(pKey);
        }

        await per.CreateOrUpdateProject(model, state, AppIdentity.user_seq, 1);

        //인오더 등록 프로젝트의 경우.
        if (data.i_seq != 0)
        {
          InorderCreateUpdateModel inorder = new InorderCreateUpdateModel();
          inorder.data = await per.SelectInorderOneAsync(data.i_seq);

          if (inorder.data != null)
          {
            //프로젝트 seq 넣어줌.
            inorder.data.p_seq = model.data.p_seq;
            await per.CreateOrUpdateInorder(inorder, CommonCodes.Update, AppIdentity.user_seq);

            inorder_memo memo = new inorder_memo()
            {
              i_seq = inorder.data.i_seq,
              memo = "[" + AppIdentity.name + "]님께서 Order 프로젝트를 등록 했습니다.",
              create_dt = Utils.NowKorea(),
              create_user = AppIdentity.user_seq,
              modify_dt = Utils.NowKorea(),
              modify_user = AppIdentity.user_seq
            };

            await per.CreateOrUpdateInorderMemo(memo, CommonCodes.Create);
          }
        }

        //홈페이지 프로젝트 업데이트.
        try
        {
          await HomepageUpdate(model.data.p_seq);
        }
        catch
        {

        }


        //수정, 등록 모두 사용 가능하지만 등록에서만 메일 보내도록 변경함.
        if (state == CommonCodes.Create)
        {
          //프로젝트 등록(수정) 알림 및 메일 발송.
          var returnJson = await SendAMSearcherAlarmMail(model.data.p_seq, state == CommonCodes.Create ? "등록" : "수정");

          return returnJson;
        }
        else
        {
          return Json(new
          {
            ok = true
              ,
            p_seq = model.data.p_seq
              ,
            message = "프로젝트를 수정 했습니다."
          });
        }
      }
      catch (Exception e)
      {
        return Json(new
        {
          ok = false,
          message = "프로젝트 등록 에러 : " + e.Message
        });
      }
    }

    /// <summary>
    /// 프로젝트 등록 - 평판조회
    /// </summary>
    /// <param name="p_seq"></param>
    /// <returns></returns>
    public async Task<ActionResult> ProjectRepCreate(int p_seq = 0, int c_seq = 0, string type = "")
    {
      ProjectRepository pr = new ProjectRepository();

      var data = await pr.SelectProjectRepOneAsync(p_seq);
      if (data == null)
        data = new project()
        {
          exp_salary = 0,
          fee_rate = 100
        };

      ProjectCreateRepModel model = new ProjectCreateRepModel()
      {
        p_seq = data.p_seq,
        c_seq = data.c_seq,
        company_name = data.company_name,
        cc_seq = data.cc_seq,
        ctc_seq = data.ctc_seq,
        pjt_type = data.pjt_type,
        title = data.title,
        exp_salary = data.exp_salary,
        exp_salary_won = data.exp_salary_won,
        currency_cd = data.currency_cd,
        fee_rate = data.fee_rate,
        client_require = data.client_require,
        internal_note = data.internal_note,
        tax_division = data.tax_division,
        tax_name = data.tax_name,
        tax_email = data.tax_email,
        tax_phone = data.tax_phone,
        tax_cell_phone = data.tax_cell_phone,
        tax_deposit_manager = data.tax_deposit_manager,
        tax_deposit_email = data.tax_deposit_email,
        contact_name = data.contact_name,
        contact_gender = data.contact_gender,
        contact_email = data.contact_email,
        contact_phone = data.contact_phone,
        contact_cell_phone = data.contact_cell_phone,
        contact_division = data.contact_division,
        contact_position = data.contact_position,
        business_code1 = data.business_code1,
        business_code2 = data.business_code2,
        business_name1 = data.business_names1,
        business_name2 = data.business_names2,
        job_code1 = data.job_code1,
        job_code2 = data.job_code2,
        job_name1 = data.job_names1,
        job_name2 = data.job_names2

      };

      //client에서 넘어온 project 생성의 경우.
      if (c_seq != 0 && p_seq == 0)
      {
        ClientRepository cr = new ClientRepository();

        var client = await cr.SelectClientOneAsync(c_seq);

        model.c_seq = client.c_seq;
        model.company_name = client.kor_name;
        var clientAmList = await cr.SelectClientAmListAsync(c_seq);
        model.amList = clientAmList.Select(x => new pjt_director
        {
          p_seq = 0
            ,
          uv_seq = x.uv_seq
            ,
          name = x.am_name
            ,
          ud_name = x.ud_name
        }).ToList();
      }

      //프로젝트 수정의 경우.
      if (p_seq != 0)
      {
        model.amList = await pr.SelectProjectAmListAsync(p_seq);
        model.searcherList = await pr.SelectProjectSearcherListAsync(p_seq);
      }

      model.type = type;

      return View(model);
    }

    /// <summary>
    /// 프로젝트 등록 - 평판조회
    /// </summary>
    /// <param name="data"></param>
    /// <returns></returns>
    [HttpPost]
    public async Task<JsonResult> ProjectRepCreate(ProjectCreateRepModel data)
    {
      try
      {
        ProjectEntityRepository per = new ProjectEntityRepository();

        ProjectRepCreateUpdateModel model = new ProjectRepCreateUpdateModel();

        //신규, 수정 변수
        string state = "";

        //신규
        if (data.p_seq == 0)
        {
          model.data = new project()
          {
            pjt_type = data.pjt_type,
            c_seq = data.c_seq,
            title = data.title,
            exp_salary = data.exp_salary,
            exp_salary_won = data.exp_salary_won,
            currency_cd = data.currency_cd,
            fee_rate = 100,
            client_require = data.client_require,
            internal_note = data.internal_note,
            ctc_seq = data.ctc_seq,
            cc_seq = data.cc_seq,
            business_code1 = data.business_code1,
            business_code2 = data.business_code2,
            job_code1 = data.job_code1,
            job_code2 = data.job_code2,
            pjt_status = ProjectStatusCode.progress,
            open_dt = Utils.NowKorea(),
            create_dt = Utils.NowKorea(),
            create_user = AppIdentity.user_seq,
            modify_dt = Utils.NowKorea(),
            modify_user = AppIdentity.user_seq
          };

          state = CommonCodes.Create;
        }
        else
        {
          model.data = await per.SelectProjectOneAsync(data.p_seq);
          if (model.data == null)
            return Json(new
            {
              ok = false
                ,
              message = "해당 프로젝트를 찾을 수 없습니다."
            });
          model.data.c_seq = data.c_seq;
          model.data.business_code1 = data.business_code1;
          model.data.business_code2 = data.business_code2;
          model.data.job_code1 = data.job_code1;
          model.data.job_code2 = data.job_code2;

          model.data.pjt_type = data.pjt_type;
          model.data.title = data.title;
          model.data.exp_salary = data.exp_salary;
          model.data.exp_salary_won = data.exp_salary_won;
          model.data.currency_cd = data.currency_cd;
          model.data.fee_rate = 100;
          model.data.client_require = data.client_require;
          model.data.internal_note = data.internal_note;
          model.data.ctc_seq = data.ctc_seq;
          model.data.cc_seq = data.cc_seq;
          model.data.modify_dt = Utils.NowKorea();
          model.data.modify_user = AppIdentity.user_seq;

          model.deleteAmList = await per.SelectPjtDirectorListAsync(model.data.p_seq);
          model.deleteSearcherList = await per.SelectPjtContactListAsync(model.data.p_seq);

          state = CommonCodes.Update;
        }

        //AM
        foreach (var am in data.amList)
        {
          var director = new pjt_director()
          {
            p_seq = model.data.p_seq
              ,
            uv_seq = am.uv_seq
          };

          model.amList.Add(director);
        }

        //Searcher
        foreach (var searcher in data.searcherList)
        {
          var contact = new pjt_manager()
          {
            p_seq = model.data.p_seq
              ,
            uv_seq = searcher.uv_seq
          };

          model.searcherList.Add(contact);
        }

        await per.CreateOrUpdateProjectRep(model, state, AppIdentity.user_seq, 1);

        //프로젝트 등록(수정) 알림 및 메일 발송.
        var returnJson = await SendAMSearcherAlarmMail(model.data.p_seq, state == CommonCodes.Create ? "등록" : "수정");

        return returnJson;
      }
      catch (Exception e)
      {
        return Json(new
        {
          ok = false,
          message = "알 수 없는 오류가 발생 했습니다."
        });
      }
    }

    /// <summary>
    /// 프로젝트, 등록 및 수정 알림과 메일 발송.
    /// </summary>
    /// <param name="p_seq">프로젝트 p_seq</param>
    /// <param name="type">등록, 수정</param>
    /// <returns></returns>
    public async Task<JsonResult> SendAMSearcherAlarmMail(int p_seq, string type)
    {
      try
      {
        ProjectRepository pr = new ProjectRepository();
        var project = await pr.SelectProjectWithMailOneAsync(p_seq);

        //프로젝트 am
        var amList = await pr.SelectProjectAM(p_seq);
        if (amList.Count == 0)
          return Json(new
          {
            ok = true
              ,
            p_seq = p_seq
              ,
            message = string.Format("프로젝트 {0}을 완료 했지만, 알림 및 메일을 발송 할 AM을 찾지 못했습니다.", type)
          });

        //프로젝트 searcher
        var searcherList = await pr.SelectProjectSearcher(p_seq);
        if (searcherList.Count == 0)
          return Json(new
          {
            ok = true
              ,
            p_seq = p_seq
              ,
            message = string.Format("프로젝트 {0}을 완료 했지만, 알림 및 메일을 발송 할 Searcher를 찾지 못했습니다.", type)
          });

        //프로젝트 url 바인딩
        string url = string.Empty;
        if (project.pjt_type == ProjectTypeCode.hire)
          url = "/Project/ProjectDetail?p_seq=" + p_seq;
        else
          url = "/Project/ProjectRepDetail?p_seq=" + p_seq;

        //메일발송 담당자 list
        List<string> ToArr = new List<string>();

        //알림 메세지 작성
        alarm_message aMessage = new alarm_message()
        {
          href_url = url,
          message = "[" + project.title + "] 프로젝트가 등록 되었습니다.",
          create_dt = Utils.NowKorea()
        };

        //알림 보낼 유저 목록 생성
        var userList = new List<uv_user>();
        userList.AddRange(amList);
        userList.AddRange(searcherList);

        var uvSeqs = userList.Select(y => y.uv_seq).ToArray();
        //알림 보낼 유저 바인딩.
        List<alarm_user> aUserList = new List<alarm_user>();
        foreach (var user in userList)
        {
          //이미 리스트에 포함된 유저의 경우 continue. (AM이면서 Searcher인 경우가 있을 수 있으므로)
          if (aUserList.Where(x => x.uv_seq == user.uv_seq).FirstOrDefault() != null)
            continue;

          alarm_user aUser = new alarm_user()
          {
            uv_seq = user.uv_seq
              ,
            is_read = 0
              ,
            read_date = null
          };

          //메일 발송 유저 리스트 바인딩
          ToArr.Add(user.email);

          //알림 유저 리스트 바인딩
          aUserList.Add(aUser);
        }

        ApiEntityRepository aer = new ApiEntityRepository();
        //알림 발송.
        await aer.CreateAlarm(aMessage, aUserList);


        //메일 발송 폼 데이터
        ProjectCreateDto mailData = new ProjectCreateDto()
        {
          ToArr = ToArr.ToArray()
            ,
          From = new System.Net.Mail.MailAddress("noreply@unicosearch.com", "noreply")
            ,
          type = type
            ,
          title = project.title
            ,
          companyName = project.company_name
            ,
          pjtType = Utils.ReturnProjectTypeTxt(project.pjt_type)
            ,
          amNames = String.Join(",", amList.Select(x => x.name).ToList())
            ,
          searcherNames = String.Join(",", searcherList.Select(x => x.name).ToList())
            ,
          url = "http://univision.unicosearch.com" + url
            ,
          createDt = Utils.ConvertDateTimeHourToString2(project.create_dt)
        };

        MailService mService = new MailService();

        var result = mService.SendProjectCreateMail(mailData, new ProjectCreateTemplete());
        if (!result.isSend)
          return Json(new
          {
            ok = true
              ,
            p_seq = p_seq
              ,
            message = string.Format("프로젝트 {0}을 완료 했지만, AM과 Searcher에게 알림 및 메일 발송 중 오류가 발생 했습니다.", type)
          });


        return Json(new
        {
          ok = true
            ,
          p_seq = p_seq
            ,
          message = string.Format("프로젝트를 {0} 했습니다.<br/>AM과 Searcher에게 알림 및 메일을 발송 했습니다.", type)

        });

      }
      catch (Exception e)
      {
        return Json(new
        {
          ok = true
            ,
          p_seq = p_seq
            ,
          message = string.Format("프로젝트 {0}을 완료 했지만, AM과 Searcher에게 알림 및 메일 발송 중 오류가 발생 했습니다.", type)
        });
      }
    }

    #endregion

    #region 프로젝트 상세보기

    /// <summary>
    /// 프로젝트 상세보기 - 채용
    /// </summary>
    /// <param name="p_seq"></param>
    /// <returns></returns>
    public async Task<ActionResult> ProjectDetail(int p_seq = 0)
    {
      ProjectRepository pr = new ProjectRepository();
      ProjectEntityRepository per = new ProjectEntityRepository();

      var data = await pr.SelectProjectOneAsync(p_seq);
      if (data == null)
        data = new project();

      int is_my_key = 0;
      var mykey = await per.SelectProjectMyKeyOneAsync(p_seq, AppIdentity.user_seq);
      if (mykey != null)
      {
        is_my_key = 1;
      }

      ProjectCreateModel model = new ProjectCreateModel()
      {
        p_seq = data.p_seq,
        log_cnt = data.log_cnt,
        c_seq = data.c_seq,
        cc_seq = data.cc_seq,
        ctc_seq = data.ctc_seq,
        pjt_status = data.pjt_status,
        status_comment = data.status_comment,
        company_name = data.company_name,
        pjt_type = data.pjt_type,
        recruit_number = data.recruit_number,
        is_posting = data.is_posting,
        title = data.title,
        position_seq = data.position_seq,
        position_str = data.position_str,
        experience_type = data.experience_type,
        expreience_number = data.expreience_number,
        edu_code = data.edu_code,
        edu_name = data.edu_name,
        foreign_lang = data.foreign_lang,
        foreign_lang_name = data.foreign_lang_name,
        foreign_level = data.foreign_level,
        assign_task = data.assign_task,
        requirement = data.requirement,
        internal_contents = data.internal_contents,
        is_kor_resume = data.is_kor_resume,
        is_eng_resume = data.is_eng_resume,
        is_portfolio = data.is_portfolio,
        is_self_introduction = data.is_self_introduction,
        is_etc = data.is_etc,
        etc_comment = data.etc_comment,
        is_pre_interview = data.is_pre_interview,
        is_number = data.is_number,
        interview_number = data.interview_number,
        is_personality_test = data.is_personality_test,
        gender_type = data.gender_type,
        start_age = data.start_age,
        end_age = data.end_age,
        target_school_nm = data.target_school_nm,
        //target_school_name = data.target_school_name,
        //target_school_campus = data.target_school_campus,
        target_major_nm = data.target_major_nm,
        //target_major_name = data.target_major_name,
        target_company_nm = data.target_company_nm,
        //target_company_name = data.target_company_name,
        confidentiallity = data.confidentiallity,
        exp_salary = data.exp_salary,
        exp_salary_won = data.exp_salary_won,
        currency_cd = data.currency_cd,
        fee_rate = data.fee_rate,
        tax_division = data.tax_division,
        tax_name = data.tax_name,
        tax_email = data.tax_email,
        tax_phone = data.tax_phone,
        tax_cell_phone = data.tax_cell_phone,
        tax_deposit_manager = data.tax_deposit_manager,
        tax_deposit_email = data.tax_deposit_email,
        contact_name = data.contact_name,
        contact_gender = data.contact_gender,
        contact_email = data.contact_email,
        contact_phone = data.contact_phone,
        contact_cell_phone = data.contact_cell_phone,
        contact_division = data.contact_division,
        contact_position = data.contact_position,
        business_code1 = data.business_code1,
        business_code2 = data.business_code2,
        business_name1 = data.business_names1,
        business_name2 = data.business_names2,
        job_code1 = data.job_code1,
        job_code2 = data.job_code2,
        job_name1 = data.job_names1,
        job_name2 = data.job_names2,
        sub_business_code1 = data.sub_business_code1,
        sub_business_code2 = data.sub_business_code2,
        sub_business_name1 = data.sub_business_names1,
        sub_business_name2 = data.sub_business_names2,
        sub_job_code1 = data.sub_job_code1,
        sub_job_code2 = data.sub_job_code2,
        sub_job_name1 = data.sub_job_names1,
        sub_job_name2 = data.sub_job_names2,
        no_business = data.no_business == null ? 0 : data.no_business,
        no_job = data.no_job == null ? 0 : data.no_job,
        is_cowork = data.is_cowork == null ? 0 : data.is_cowork,
        is_share_pjt = data.is_share_pjt == null ? 0 : data.is_share_pjt,
        is_share_3m = data.is_share_3m,
        share_comments = data.share_comments,
        share_fee_rate = data.share_fee_rate,
        is_pe = data.is_pe == null ? 0 : data.is_pe,
        open_dt = data.open_dt,
        close_dt = data.close_dt,
        is_my_key = is_my_key
      };

      model.amList = await pr.SelectProjectAmListAsync(p_seq);
      model.searcherList = await pr.SelectProjectSearcherListAsync(p_seq);
      //model.busiList = await pr.SelectProjectBusiListAsync(p_seq);
      //model.jobList = await pr.SelectProjectJobListAsync(p_seq);
      model.placeList = await pr.SelectProjectPlaceListAsync(p_seq);
      model.keywordList = await pr.SelectProjectKeywordListAsync(p_seq);

      var read_data = await per.SelectProjectReadOneAsync(p_seq, AppIdentity.user_seq);
      string state = String.Empty;
      if (read_data == null)
      {
        read_data = new project_read_history()
        {
          p_seq = p_seq,
          read_user = AppIdentity.user_seq,
          read_dt = Utils.NowKorea()
        };
        state = CommonCodes.Create;
      }
      else
      {
        read_data.read_dt = Utils.NowKorea();
        state = CommonCodes.Update;
      }
      await per.CreateOrUpdateProjectRead(read_data, state);


      return View(model);
    }

    /// <summary>
    /// 프로젝트 마감
    /// </summary>
    /// <param name="p_seq"></param>
    /// <returns></returns>
    public PartialViewResult ProjectStateChange(int p_seq)
    {
      ViewBag.p_seq = p_seq;
      return PartialView();
    }

    /// <summary>
    /// 프로젝트 마감.
    /// </summary>
    /// <param name="p_seq"></param>
    /// <param name="pjt_status"></param>
    /// <param name="status_comment"></param>
    /// <returns></returns>
    [HttpPost]
    public async Task<JsonResult> ProjectStateChange(int p_seq, int pjt_status, string status_comment)
    {
      try
      {
        ProjectEntityRepository per = new ProjectEntityRepository();
        var project = await per.SelectProjectOneAsync(p_seq);

        if (project == null)
        {
          return Json(new
          {
            ok = false,
            message = "마감하려는 프로젝트를 찾을 수 없습니다."
          });
        }

        if (pjt_status == 1)
        {
          project.open_dt = Utils.NowKorea();
          project.close_dt = null;
        }
        else if (pjt_status == 2 || pjt_status == 3 || pjt_status == 4 || pjt_status == 5)
        {
          if (project.pjt_status == 1 && !project.close_dt.HasValue)
            project.close_dt = Utils.NowKorea();
        }

        project.pjt_status = pjt_status;
        project.status_comment = status_comment;
        project.modify_dt = Utils.NowKorea();
        project.modify_user = AppIdentity.user_seq;


        await per.UpdateProjectAsync(project, "U", AppIdentity.user_seq, 1);

        try
        {
          await HomepageUpdate(p_seq);
        }
        catch
        {

        }

        return Json(new
        {
          ok = true
            ,
          message = "변경 했습니다."
        });
      }
      catch (Exception e)
      {
        return Json(new
        {
          ok = false,
          message = "알 수 없는 오류가 발생 했습니다. - " + e.Message
        });
      }
    }

    public async Task<ActionResult> ProjectProgressExcelList(int excel_p_seq, int excel_state, string excel_orderOption, string excel_orderTxt)
    {
      try
      {
        ProjectRepository pr = new ProjectRepository();

        var excel = new Excel<ProjectProgressListExcelModel>();
        var excelRawData = await pr.SelectProjectProgressExcelListAsync(excel_p_seq, excel_state, excel_orderTxt, excel_orderOption);

        var excelData = new List<ProjectProgressListExcelModel>();
        foreach (var row in excelRawData)
        {
          excelData.Add(new ProjectProgressListExcelModel()
          {
            이름 = row.kor_name,
            성별 = row.gender,
            생년 = row.birth_date,
            학력 = row.edu,
            경력 = row.exp,
            참고사항 = row.comment,
            후보자메모 = row.can_memo,
            진행상황 = Utils.ReturnProjectHistoryStateTxt(row.state)

          });


        }
        int[] widths = { 13, 8, 15, 11, 33, 50, 65, 65 };
        var data = excel.WriteExcel(excelData, widths, "프로젝트 후보자 목록");

        return File(data.FileContents, data.ContentType, data.FileDownloadName);


      }
      catch (Exception e)
      {
        throw e;
      }
    }

    /// <summary>
    /// 프로젝트 진행사항 리스트
    /// </summary>
    /// <param name="p_seq"></param>
    /// <param name="state"></param>
    /// <param name="page"></param>
    /// <returns></returns>
    public async Task<PartialViewResult> ProjectProgressList(int p_seq, int state, string orderTxt, string orderOption, int page = 0)
    {
      int cookie_pjt_view_grid = 0;
      if (Request.Cookies.AllKeys.Contains("PJT_VIEW_GRID"))
      {
        cookie_pjt_view_grid = Convert.ToInt32(Request.Cookies["PJT_VIEW_GRID"].Value);
      }

      ProjectRepository pr = new ProjectRepository();

      int totalCount = 0;

      //한 페이지당 몇개의 행을 보여줄 것인지
      int pageSize = 96;

      var cntData = await pr.SelectProjectRecommandStateCountAsync(p_seq, AppIdentity.user_seq);

      if (String.IsNullOrEmpty(orderTxt))
        orderTxt = "A.state";
      if (String.IsNullOrEmpty(orderOption))
        orderOption = "DESC";

      var list = pr.SelectProjectRecommandHistoryListAsync(p_seq, state, orderTxt, orderOption, (page - 1) * pageSize, pageSize, out totalCount);

      SearchProjectProgressListModel model = new SearchProjectProgressListModel()
      {
        p_seq = p_seq
          ,
        state = state
          ,
        cntData = cntData
          ,
        orderTxt = orderTxt
          ,
        orderOption = orderOption
          ,
        list = list
          ,
        PagingInfo = new PagingInfo()
        {
          CurrentPage = page
              ,
          ItemsPerPage = pageSize
              ,
          TotalItems = totalCount
        }
      };

      if (cookie_pjt_view_grid == 1)
      {
        return PartialView(model);
      }
      else
      {
        return PartialView("ProjectProgressList2", model);
      }

    }

    public async Task<PartialViewResult> ProjectMoveCandidate(int p_seq, List<int> pic_seqs)
    {
      ProjectRepository pr = new ProjectRepository();

      var data = await pr.SelectProjectRecandidateListAsync(p_seq, pic_seqs);

      ProjectRecandidateMultiMoveModel model = new ProjectRecandidateMultiMoveModel();

      model.p_seq = p_seq;
      foreach (var pic in data)
      {
        model.list.Add(new pjt_recandidate_history
        {
          pic_seq = pic.pic_seq
        ,
          p_seq = p_seq
        ,
          c_seq = pic.c_seq
        ,
          prc_seq = 0
        ,
          kor_name = pic.kor_name
        });
      }




      return PartialView(model);
    }

    public async Task<PartialViewResult> SearchMoveProject(int p_seq, MyProjectSearchModel search, int page = 1)
    {
      ProjectRepository pr = new ProjectRepository();

      int totalCount = 0;

      //한 페이지당 몇개의 행을 보여줄 것인지
      int pageSize = AppPaging.PageLength10;

      search.orderTxt = "P.modify_dt";

      //var cntData = await pr.SelectProjectStatusCountAsync(search, AppIdentity.user_seq);
      //var list = pr.SelectMyProjectList(search, AppIdentity.user_seq, (page - 1) * pageSize, pageSize, out totalCount);
      var list = pr.SelectPopAllProjectList2(p_seq, search, AppIdentity.user_seq, (page - 1) * pageSize, pageSize, out totalCount);
      MyProjectListModel model = new MyProjectListModel()
      {
        search = search,
        //cntData = cntData,
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
      return PartialView(model);
    }

    [HttpPost]
    public async Task<JsonResult> ProjectMoveCandidateProc(int is_new, int p_seq, List<int> pic_seqs, int target_p_seq)
    {
      try
      {
        ProjectCreateUpdateModel model = new ProjectCreateUpdateModel();
        ProjectEntityRepository per = new ProjectEntityRepository();
        if (is_new == 1)
        {
          if (p_seq > 0 && pic_seqs.Count > 0)
          {
            //프로젝트 복사
            var project = await per.SelectProjectOneAsync(p_seq);
            var amList = await per.SelectPjtDirectorListAsync(p_seq);
            var searcherList = await per.SelectPjtContactListAsync(p_seq);
            var placeList = await per.SelectPjtPlaceListAsync(p_seq);
            var keywordList = await per.SelectPjtKeywordListAsync(p_seq);

            model.data = new project()
            {
              p_seq = 0
,
              pjt_type = 1
,
              c_seq = project.c_seq
,
              cc_seq = project.cc_seq
,
              ctc_seq = project.ctc_seq
,
              recruit_number = project.recruit_number
,
              is_posting = project.is_posting
,
              title = project.title + "(Copied)"
,
              position_seq = project.position_seq
,
              position_str = project.position_str
,
              experience_type = project.experience_type
,
              expreience_number = project.expreience_number
,
              edu_code = project.edu_code
,
              edu_name = project.edu_name
,
              foreign_lang = project.foreign_lang
,
              foreign_lang_name = project.foreign_lang_name
,
              foreign_level = project.foreign_level
,
              addr1 = project.addr1
,
              addr2 = project.addr2
,
              assign_task = project.assign_task
,
              requirement = project.requirement
,
              internal_contents = project.internal_contents
,
              client_require = project.client_require
,
              internal_note = project.internal_note
,
              is_kor_resume = project.is_kor_resume
,
              is_eng_resume = project.is_eng_resume
,
              is_portfolio = project.is_portfolio
,
              is_self_introduction = project.is_self_introduction
,
              is_etc = project.is_etc
,
              etc_comment = project.etc_comment
,
              is_pre_interview = project.is_pre_interview
,
              is_number = project.is_number
,
              interview_number = project.interview_number
,
              is_personality_test = project.is_personality_test
,
              gender_type = project.gender_type
,
              start_age = project.start_age
,
              end_age = project.end_age
,
              target_school_nm = project.target_school_nm
,
              target_major_nm = project.target_major_nm
,
              target_company_nm = project.target_company_nm
,
              confidentiallity = project.confidentiallity
,
              expected_salary = project.expected_salary
,
              exp_salary = project.exp_salary
,
              exp_salary_won = project.exp_salary_won
,
              currency_cd = project.currency_cd
,
              fee_rate = project.fee_rate
,
              contents = project.contents
,
              pjt_status = 1
,
              status_comment = "진행"
,
              create_dt = Utils.NowKorea()
,
              create_user = AppIdentity.user_seq
,
              modify_dt = Utils.NowKorea()
,
              open_dt = Utils.NowKorea()
,
              modify_user = AppIdentity.user_seq
,
              business_code1 = project.business_code1
,
              business_code2 = project.business_code2
,
              sub_business_code1 = project.sub_business_code1
,
              sub_business_code2 = project.sub_business_code2
,
              job_code1 = project.job_code1
,
              job_code2 = project.job_code2
,
              sub_job_code1 = project.sub_job_code1
,
              sub_job_code2 = project.sub_job_code2
,
              no_business = project.no_business
,
              no_job = project.no_job
,
              is_pe = project.is_pe
            };

            if (amList.Count > 0)
            {
              foreach (var am in amList)
              {
                pjt_director pjt_d = new pjt_director()
                {
                  p_seq = 0,
                  uv_seq = am.uv_seq,
                  entry_date = Utils.NowKorea()
                };
                model.amList.Add(pjt_d);
              }
            }

            if (searcherList.Count > 0)
            {
              foreach (var sm in searcherList)
              {
                pjt_manager pjt_m = new pjt_manager()
                {
                  p_seq = 0,
                  uv_seq = sm.uv_seq,
                  entry_date = Utils.NowKorea()
                };
                model.searcherList.Add(pjt_m);
              }
            }

            if (placeList.Count > 0)
            {
              foreach (var place in placeList)
              {
                pjt_place pjt_pl = new pjt_place()
                {
                  p_seq = 0,
                  area1 = place.area1,
                  area2 = place.area2,
                  code1 = place.code1,
                  code2 = place.code2,
                  pp_seq = place.pp_seq
                };
                model.placeList.Add(pjt_pl);
              }
            }

            if (keywordList.Count > 0)
            {
              foreach (var keyword in keywordList)
              {
                pjt_keyword pjt_kw = new pjt_keyword()
                {
                  p_seq = 0,
                  pk_name = keyword.pk_name,
                  pk_seq = keyword.pk_seq

                };
                model.keywordList.Add(pjt_kw);
              }
            }

            await per.CreateOrUpdateProject(model, CommonCodes.Create, AppIdentity.user_seq, 1);
            target_p_seq = model.data.p_seq;
          }
          else
          {
            throw new Exception("원본 프로젝트 및 이관 후보자 정보가 없습니다.");
          }
        }

        else if (is_new == 0 && target_p_seq == 0)
        {
          throw new Exception("이관할 프로젝트 정보가 올바르지 않습니다.");
        }

        CandidateEntityRepository cer = new CandidateEntityRepository();
        string memo_org = "후보자가 아래 프로젝트로 이관되었습니다.<br/>- 후보자 : ";
        string memo_target = "후보자가 아래 프로젝트로부터 이관되었습니다.<br/>- 후보자 : ";
        string candidate_names = "";
        string no_move_result = "";
        int move_cnt = 0;
        foreach (var pic_seq in pic_seqs)
        {
          var pjt_recandidate = await per.SelectPjtRecandidateOneAsync(pic_seq);
          var candidate = await cer.SelectCandidateOneAsync(pjt_recandidate.c_seq);
          var pjt_recandidate_target = await per.SelectPjtRecandidateOneAsync(target_p_seq, pjt_recandidate.c_seq);
          if (pjt_recandidate_target != null)
          {
            if (!String.IsNullOrEmpty(no_move_result))
            {
              no_move_result += ",";
            }
            no_move_result += candidate.kor_name;
            continue;
          }

          pjt_recandidate.p_seq = target_p_seq;

          if (!String.IsNullOrEmpty(candidate_names))
          {
            candidate_names += ",";
          }
          candidate_names += @"<a href=""/Candidate/CandidateDetail?c_seq=" + candidate.c_seq.ToString() + @"&is_pop=1"" target=""_blank"">" + candidate.kor_name + @"</a>";

          var pjt_recandidate_history = await per.SelectPjtRecHisListAsync(pic_seq);
          if (pjt_recandidate_history.Count > 0)
          {
            foreach (var prh in pjt_recandidate_history)
            {
              prh.p_seq = target_p_seq;
            }
          }
          await per.UpdatePjtRecandidate(pjt_recandidate, pjt_recandidate_history);
          move_cnt++;
        }
        
        if (!String.IsNullOrEmpty(no_move_result))
        {
          no_move_result = "<br/><span class='text-primary'>[※ 해당 후보자는 이미 등록되어 있어 이관되지 못했습니다.(" + no_move_result + ")]</span>";
        }

        if (move_cnt > 0)
        {
          memo_org += candidate_names + @"<br/> - 대상 프로젝트 : <a href=""/Project/ProjectDetail?p_seq=" + target_p_seq.ToString() + @""" target=""_blank"">[링크]</a>";
          memo_target += candidate_names + @"<br/> - 원본 프로젝트 : <a href=""/Project/ProjectDetail?p_seq=" + p_seq.ToString() + @""" target=""_blank"">[링크]</a>";

          pjt_memo pm = new pjt_memo()
          {
            p_seq = p_seq
            ,
            memo = memo_org
            ,
            create_user = AppIdentity.user_seq
            ,
            create_dt = Utils.NowKorea()
          };

          await per.CreateOrDeletePjtMemo(pm, CommonCodes.Create);

          pjt_memo pm2 = new pjt_memo()
          {
            p_seq = target_p_seq
            ,
            memo = memo_target
            ,
            create_user = AppIdentity.user_seq
            ,
            create_dt = Utils.NowKorea()
          };

          await per.CreateOrDeletePjtMemo(pm2, CommonCodes.Create);

          

          return Json(new
          {
            ok = true,
            new_p_seq = target_p_seq,
            message = "이관완료" + no_move_result
          });
        } 
        else
        {
          return Json(new
          {
            ok = false,
            new_p_seq = target_p_seq,
            message = no_move_result
          });
        }
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

    public async Task<PartialViewResult> ProjectProgressCreateMulti(int p_seq, List<int> pic_seqs)
    {
      ProjectRepository pr = new ProjectRepository();

      var data = await pr.SelectProjectRecandidateListAsync(p_seq, pic_seqs);

      ProjectRecandidateMultiCreateModel model = new ProjectRecandidateMultiCreateModel();

      model.p_seq = p_seq;
      foreach (var pic in data)
      {
        model.list.Add(new pjt_recandidate_history
        {
          pic_seq = pic.pic_seq
        ,
          p_seq = p_seq
        ,
          c_seq = pic.c_seq
        ,
          prc_seq = 0
        ,
          kor_name = pic.kor_name
        });
      }




      return PartialView(model);
    }


    [HttpPost]
    public async Task<JsonResult> ProjectProgressMultiCreateProc(List<pjt_recandidate_history> data)
    {
      try
      {
        if (data.Count > 0)
        {
          ProjectEntityRepository per = new ProjectEntityRepository();
          foreach (var pjt_recan_his in data)
          {
            pjt_recan_his.create_dt = Utils.NowKorea();
            pjt_recan_his.create_user = AppIdentity.user_seq;
            pjt_recan_his.modify_dt = Utils.NowKorea();
            pjt_recan_his.modify_user = AppIdentity.user_seq;

            await per.CreateOrUpdatePjtRecHistory(pjt_recan_his, null, null, CommonCodes.Create);

            if (pjt_recan_his.state == ProjectHistoryStateCode.interview || pjt_recan_his.is_schedule == 1)
            {
              //스케줄 등록
              ScheduleEntityRepository ser = new ScheduleEntityRepository();
              var sch = await ser.FindScheduleWithProjectOneAsync(pjt_recan_his.prc_seq, pjt_recan_his.pic_seq, pjt_recan_his.p_seq, pjt_recan_his.c_seq);

              ProjectRepository pr = new ProjectRepository();
              var pjt = await pr.SelectProjectOneAsync(pjt_recan_his.p_seq);
              var recCandi = await pr.SelectProjectRecommandCandiOneAsync(pjt_recan_his.prc_seq);
              var amList = await pr.SelectProjectAmListAsync(pjt_recan_his.p_seq);
              var searcherList = await pr.SelectProjectSearcherListAsync(pjt_recan_his.p_seq);

              //List 합쳐줌
              amList.AddRange(searcherList.Select(x => new pjt_director()
              {
                uv_seq = x.uv_seq
                  ,
                p_seq = x.p_seq
              }).ToList());

              var attendList = new List<schedule_attend>();

              if (sch == null)
              {
                sch = new schedule()
                {
                  type = ScheduleType.share,
                  sub_type = ScheduleSubType.candidate,
                  title = "[" + Utils.ReturnProjectHistoryStateTxt(pjt_recan_his.state) + "]" + recCandi.kor_name + " - " + pjt.title,
                  start_date = pjt_recan_his.schedule_date.Value,
                  end_date = pjt_recan_his.schedule_date.Value.AddHours(23).AddMinutes(59).AddSeconds(59),
                  contents = pjt_recan_his.contents,
                  prc_seq = pjt_recan_his.prc_seq,
                  pic_seq = pjt_recan_his.pic_seq,
                  p_seq = pjt_recan_his.p_seq,
                  c_seq = pjt_recan_his.c_seq,
                  create_dt = Utils.NowKorea(),
                  create_user = AppIdentity.user_seq,
                  modify_dt = Utils.NowKorea(),
                  modify_user = AppIdentity.user_seq,
                  bg_color = ScheduleType.shareColor
                };

                foreach (var am in amList)
                {
                  schedule_attend at = new schedule_attend()
                  {
                    uv_seq = am.uv_seq
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

                await ser.CreateOrUpdateSchedule(sch, attendList, null, CommonCodes.Create);
              }
              else
              {
                sch.title = "[" + Utils.ReturnProjectHistoryStateTxt(pjt_recan_his.state) + "]" + recCandi.kor_name + " - " + pjt.title;
                sch.start_date = pjt_recan_his.schedule_date.Value;
                sch.end_date = pjt_recan_his.schedule_date.Value.AddHours(23).AddMinutes(59).AddSeconds(59);
                sch.contents = pjt_recan_his.contents;
                sch.prc_seq = pjt_recan_his.prc_seq;
                sch.pic_seq = pjt_recan_his.pic_seq;
                sch.p_seq = pjt_recan_his.p_seq;
                sch.c_seq = pjt_recan_his.c_seq;
                sch.modify_dt = Utils.NowKorea();
                sch.modify_user = AppIdentity.user_seq;

                var beforeAttendList = await ser.FindScheduleAttendListAsync(sch.s_seq);

                foreach (var am in amList)
                {
                  schedule_attend at = new schedule_attend()
                  {
                    uv_seq = am.uv_seq
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

                await ser.CreateOrUpdateSchedule(sch, attendList, beforeAttendList, CommonCodes.Update);
              }
            }
          }
        }

        return Json(new
        {
          ok = true,
          message = "등록완료"
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



    /// <summary>
    /// 프로젝트 진행사항 생성
    /// </summary>
    /// <param name="p_seq"></param>
    /// <param name="c_seq"></param>
    /// <param name="state"></param>
    /// <returns></returns>
    public async Task<PartialViewResult> ProjectProgressCreate(int pic_seq, int p_seq, int c_seq, int prc_seq, string kor_name)
    {
      ProjectRepository pr = new ProjectRepository();
      var data = await pr.SelectProjectRecommandCandiOneAsync(prc_seq);

      if (data == null)
      {
        data = await pr.SelectProjectRecommandCreateCandiOneAsync(pic_seq);
        data.prc_seq = 0;
        data.contents = "";

      }

      if (data == null)
        data = new pjt_recandidate_history()
        {
          pic_seq = pic_seq
            ,
          p_seq = p_seq
            ,
          c_seq = c_seq
            ,
          prc_seq = prc_seq
            ,
          kor_name = kor_name
        };


      return PartialView(data);
    }

    public async Task<PartialViewResult> ProjectProgressCreate_CAND(int p_seq, int c_seq)
    {
      ProjectRepository pr = new ProjectRepository();

      var data = await pr.SelectProjectRecommandCreateCandiOneByPCAsync(p_seq, c_seq);
      data.prc_seq = 0;
      data.contents = "";

      return PartialView(data);
    }

    /// <summary>
    /// 프로젝트 진행사항 생성
    /// </summary>
    /// <param name="p_seq"></param>
    /// <param name="c_seq"></param>
    /// <param name="pic_seq"></param>
    /// <param name="prc_seq"></param>
    /// <param name="state"></param>
    /// <param name="schedule_date"></param>
    /// <param name="annual_income"></param>
    /// <param name="guarantee"></param>
    /// <param name="contents"></param>
    /// <param name="candiFile"></param>
    /// <returns></returns>
    [HttpPost]
    public async Task<JsonResult> ProjectProgressCreate(int p_seq, int c_seq, int pic_seq, int prc_seq, int state, string schedule_date, double? income_num, double? income_won, string currency_cd, int is_no_invoice, DateTime? guarantee_dt, string contents, int? is_schedule, int? is_complete, HttpPostedFileBase candiFile)
    {
      try
      {
        bool is_complete_ok = false;
        FileUpload fi = new FileUpload();
        UploadFileResult fiResult = new UploadFileResult();
        fiResult.status = true;
        fiResult.dbPath = "";

        if (candiFile != null)
          fiResult = fi.UploadCommon(Server.MapPath("~/UploadedFiles"), "PjtRecHistory\\" + c_seq, candiFile);

        //파일 업로드에서 오류 발생 시,
        if (!fiResult.status)
        {
          return Json(new
          {
            ok = false,
            message = "프로젝트 후보자 진행사항 파일 업로드에서 오류가 발생 했습니다. - " + fiResult.statusMessage
          });
        }
        else
        {
          ProjectEntityRepository per = new ProjectEntityRepository();

          var history = await per.SelectPjtRecHistory(prc_seq);

          DateTime scheduleDate;

          if (!DateTime.TryParse(schedule_date, out scheduleDate))
            return Json(new
            {
              ok = false,
              message = "올바른 일자를 입력하세요."
            });

          //신규
          if (history == null)
          {
            history = new pjt_recandidate_history()
            {
              pic_seq = pic_seq,
              c_seq = c_seq,
              p_seq = p_seq,
              state = state,
              schedule_date = scheduleDate,
              is_no_invoice = is_no_invoice,
              ann_income = income_num,
              ann_income_won = income_won,
              currency_cd = currency_cd,
              guarantee_dt = guarantee_dt,
              contents = contents,
              is_schedule = is_schedule.HasValue ? (int)is_schedule : 0,
              create_dt = Utils.NowKorea(),
              create_user = AppIdentity.user_seq,
              modify_dt = Utils.NowKorea(),
              modify_user = AppIdentity.user_seq
            };

            //파일 업로드 했을 시,
            if (!string.IsNullOrWhiteSpace(fiResult.dbPath))
            {
              history.file_directory = Path.Combine(Utils.GetRootUrl(Request), fiResult.dbPath);
              history.file_origin_path = fiResult.filePath;
              history.file_path = fiResult.originName;
            }

            await per.CreateOrUpdatePjtRecHistory(history, null, null, CommonCodes.Create);
          }
          //수정
          else
          {

            if (history == null)
              return Json(new
              {
                ok = false,
                message = "해당 후보자의 이력을 찾을 수 없습니다."
              });

            history.state = state;
            history.schedule_date = scheduleDate;
            history.is_no_invoice = is_no_invoice;
            history.ann_income = income_num;
            history.ann_income_won = income_won;
            history.currency_cd = currency_cd;
            history.guarantee_dt = guarantee_dt;
            history.contents = contents;
            history.is_schedule = is_schedule.HasValue ? (int)is_schedule : 0;

            if (!string.IsNullOrWhiteSpace(fiResult.dbPath))
            {
              history.file_directory = Path.Combine(Utils.GetRootUrl(Request), fiResult.dbPath);
              history.file_origin_path = fiResult.filePath;
              history.file_path = fiResult.originName;
            }

            history.modify_dt = Utils.NowKorea();
            history.modify_user = AppIdentity.user_seq;

            await per.CreateOrUpdatePjtRecHistory(history, null, null, CommonCodes.Update, AppIdentity.user_seq, 1);
          }

          //스케줄 등록
          //상태값 면접준비, 채용은 자동등록
          //그 외엔 화면에서 스케줄 등록 체크해야 등록.
          if (state == ProjectHistoryStateCode.interview || state == ProjectHistoryStateCode.hireReady || state == ProjectHistoryStateCode.hireOk || history.is_schedule == 1)
          {
            //스케줄 등록
            ScheduleEntityRepository ser = new ScheduleEntityRepository();
            var sch = await ser.FindScheduleWithProjectOneAsync(history.prc_seq, history.pic_seq, history.p_seq, history.c_seq);

            ProjectRepository pr = new ProjectRepository();
            var pjt = await pr.SelectProjectOneAsync(history.p_seq);
            var recCandi = await pr.SelectProjectRecommandCandiOneAsync(history.prc_seq);
            var amList = await pr.SelectProjectAmListAsync(p_seq);
            var searcherList = await pr.SelectProjectSearcherListAsync(p_seq);

            //List 합쳐줌
            amList.AddRange(searcherList.Select(x => new pjt_director()
            {
              uv_seq = x.uv_seq
                ,
              p_seq = x.p_seq
            }).ToList());

            var attendList = new List<schedule_attend>();

            if (sch == null)
            {
              sch = new schedule()
              {
                type = ScheduleType.share,
                sub_type = ScheduleSubType.candidate,
                title = "[" + Utils.ReturnProjectHistoryStateTxt(history.state) + "]" + recCandi.kor_name + " - " + pjt.title,
                start_date = scheduleDate,
                end_date = scheduleDate.AddHours(23).AddMinutes(59).AddSeconds(59),
                contents = history.contents,
                prc_seq = history.prc_seq,
                pic_seq = history.pic_seq,
                p_seq = history.p_seq,
                c_seq = history.c_seq,
                create_dt = Utils.NowKorea(),
                create_user = AppIdentity.user_seq,
                modify_dt = Utils.NowKorea(),
                modify_user = AppIdentity.user_seq,
                bg_color = ScheduleType.shareColor
              };

              foreach (var am in amList)
              {
                schedule_attend at = new schedule_attend()
                {
                  uv_seq = am.uv_seq
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

              await ser.CreateOrUpdateSchedule(sch, attendList, null, CommonCodes.Create);
            }
            else
            {
              sch.title = "[" + Utils.ReturnProjectHistoryStateTxt(history.state) + "]" + pjt.title + " - " + recCandi.kor_name;
              sch.start_date = scheduleDate;
              sch.end_date = scheduleDate.AddHours(23).AddMinutes(59).AddSeconds(59);
              sch.contents = history.contents;
              sch.prc_seq = history.prc_seq;
              sch.pic_seq = history.pic_seq;
              sch.p_seq = history.p_seq;
              sch.c_seq = history.c_seq;
              sch.modify_dt = Utils.NowKorea();
              sch.modify_user = AppIdentity.user_seq;

              var beforeAttendList = await ser.FindScheduleAttendListAsync(sch.s_seq);

              foreach (var am in amList)
              {
                schedule_attend at = new schedule_attend()
                {
                  uv_seq = am.uv_seq
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

              await ser.CreateOrUpdateSchedule(sch, attendList, beforeAttendList, CommonCodes.Update);
            }
          }

          if (is_complete == 1)
          {
            var project = await per.SelectProjectOneAsync(p_seq);

            if (project != null && project.pjt_status != 4 && project.pjt_status != 5)
            {
              if (project.exp_salary != income_num && income_num > 0)
              {
                project.exp_salary = income_num;
                project.exp_salary_won = income_won;
              }

              project.pjt_status = 4;
              project.close_dt = Utils.NowKorea();
              project.status_comment = "후보자 입사 확정으로 완료 처리";
              project.modify_dt = Utils.NowKorea();
              project.modify_user = AppIdentity.user_seq;

              await per.UpdateProjectAsync(project, "U", AppIdentity.user_seq, 1);

              //홈페이지 프로젝트 업데이트.
              try
              {
                await HomepageUpdate(p_seq);
              }
              catch
              {

              }


              is_complete_ok = true;
            }

          }
          else
          {
            await per.ProjectModifyUpdate(p_seq, AppIdentity.user_seq);
          }
        }

        return Json(new
        {
          ok = true,
          is_complete_ok = is_complete_ok,
          message = "등록완료"
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

    public async Task<PartialViewResult> MakeupRequestList(MakeupRequestSearchModel search)
    {


      ProjectRepository pr = new ProjectRepository();

      if (String.IsNullOrEmpty(search.start_date))
        search.start_date = Utils.ConvertDateTimeToString(Utils.NowKorea().AddDays(-4));
      if (String.IsNullOrEmpty(search.end_date))
        search.end_date = Utils.ConvertDateTimeToString(Utils.NowKorea());

      var list = await pr.SelectMakeupList(search);

      MakeupRequestListModel model = new MakeupRequestListModel()
      {
        search = search,
        list = list
      };

      return PartialView(model);
    }
    public async Task<JsonResult> RequestMakeup(int p_seq, List<string> resume_type, int? with_recommand, List<int> pic_seq_list)
    {
      try
      {
        ProjectRepository pr = new ProjectRepository();
        ProjectEntityRepository per = new ProjectEntityRepository();
        List<makeup_request> mrl = new List<makeup_request>();
        List<pjt_recandidate_history> prhl = new List<pjt_recandidate_history>();
        int waiting_cnt = await pr.SelectMakeupWaitingCnt();
        int total_cnt = pic_seq_list.Count;
        int suc_cnt = 0;
        int fail_cnt = 0;
        int no_cnt = 0;
        List<string> rtn_msg = new List<string>();

        if (p_seq != 0 && resume_type.Count > 0 && pic_seq_list.Count > 0)
        {

          //메이크업 메일 발송
          AccountRepository ar = new AccountRepository();
          ApiEntityRepository apier = new ApiEntityRepository();

          var project = await per.SelectProjectOneAsync(p_seq);
          var myuserinfo = await ar.FindUserBySeqAsync(AppIdentity.user_seq);
          MailService mService = new MailService(myuserinfo.email_id, myuserinfo.email_pwd);
          foreach (var r_type in resume_type)
          {
            string resume_type_str = r_type == "K" ? "국문" : "영문";
            foreach (var pic_seq in pic_seq_list)
            {
              pjt_recandidate_history pjt_recan = await pr.SelectProjectRecommandHistoryOneAsync(p_seq, pic_seq);

              int file_seq = 0;
              string file_name = String.Empty;
              string file_dir = String.Empty;
              if (r_type == "K")
              {
                file_seq = pjt_recan.final_kor_cr_seq.HasValue ? pjt_recan.final_kor_cr_seq.Value : 0;
                file_dir = pjt_recan.final_kor_resume_dir;
                file_name = pjt_recan.final_kor_resume_name;
              }
              else if (r_type == "E")
              {
                file_seq = pjt_recan.final_eng_cr_seq.HasValue ? pjt_recan.final_eng_cr_seq.Value : 0;
                file_dir = pjt_recan.final_eng_resume_dir;
                file_name = pjt_recan.final_eng_resume_name;
              }

              if (!String.IsNullOrEmpty(file_dir))
              {
                makeup_request mr = new makeup_request()
                {
                  p_seq = p_seq,
                  req_user = AppIdentity.user_seq,
                  request_date = Utils.NowKorea(),
                  reg_date = Utils.NowKorea(),
                  reg_seq = AppIdentity.user_seq,
                  mod_date = Utils.NowKorea(),
                  mod_seq = AppIdentity.user_seq,
                  c_seq = pjt_recan.c_seq,
                  del_yn = 0,
                  cr_seq = file_seq,
                  cr_dir = file_dir,
                  resume_type = r_type,
                  status = "요청"
                };


                pjt_recandidate_history prh = new pjt_recandidate_history()
                {
                  pic_seq = pic_seq,
                  c_seq = pjt_recan.c_seq,
                  p_seq = p_seq,
                  state = (pjt_recan.state == 20 && with_recommand == 1 ? 30 : pjt_recan.state),
                  schedule_date = pjt_recan.schedule_date,
                  annual_income = pjt_recan.annual_income,
                  guarantee_dt = pjt_recan.guarantee_dt,
                  contents = resume_type_str + " Makeup 요청",
                  is_schedule = 0,
                  create_dt = Utils.NowKorea(),
                  create_user = AppIdentity.user_seq,
                  modify_dt = Utils.NowKorea(),
                  modify_user = AppIdentity.user_seq,
                };
                try
                {
                  await per.CreateMakeupRequest(mr, prh, CommonCodes.Create);
                  suc_cnt++;
                  rtn_msg.Add(string.Format("[{0}] {1} Makeup 요청이 완료 되었습니다.", pjt_recan.kor_name, resume_type_str));
                  //알림 메시지 설정
                  alarm_message aMessage = new alarm_message()
                  {
                    href_url = "/Project/ProjectDetail?p_seq=" + p_seq,
                    message = string.Format("[{0}] {1} Makeup 요청이 완료 되었습니다.", pjt_recan.kor_name, resume_type_str),
                    create_dt = Utils.NowKorea()
                  };
                  //나에게만 보내기
                  alarm_user alarm = new alarm_user()
                  {
                    uv_seq = AppIdentity.user_seq
                          ,
                    is_read = 0
                  };
                  List<alarm_user> aList = new List<alarm_user>();
                  aList.Add(alarm);

                  await apier.CreateAlarm(aMessage, aList);

                  List<string> ToArr = new List<string>();
                  ToArr.Add(AppIdentity.email);
                  ToArr.Add("makeup@unicosearch.com");
                  //메일 Dto
                  MakeupRequestDto mailData = new MakeupRequestDto()
                  {
                    ToArr = ToArr.ToArray()
                      ,
                    From = new System.Net.Mail.MailAddress(AppIdentity.email, AppIdentity.name)
                      ,
                    name = AppIdentity.name
                      ,
                    title = string.Format("[{0}] {1} Makeup 요청.", pjt_recan.kor_name, resume_type_str)
                      ,
                    candidateurl = "http://univision.unicosearch.com/Candidate/CandidateDetail/?c_seq=" + pjt_recan.c_seq
                      ,
                    candidatename = pjt_recan.kor_name
                      ,
                    filelang = resume_type_str
                      ,
                    fileurl = "http://univision.unicosearch.com/DownloadFile/Download?type=canResume&element_seq=" + file_seq.ToString()
                      ,
                    filename = file_name
                      ,
                    projecturl = "http://univision.unicosearch.com/Project/ProjectDetail?p_seq=" + pjt_recan.p_seq
                      ,
                    pjttitle = project.title
                  };
                  var result_my = mService.SendMakeupRequestMail(mailData, new MakeupRequesTemplete());




                }
                catch (Exception e)
                {
                  fail_cnt++;
                  rtn_msg.Add(pjt_recan.kor_name + " 후보자 메이크업 요청 중 오류 발생.[" + e.Message + "]");
                }

              }
              else
              {
                no_cnt++;
                rtn_msg.Add(pjt_recan.kor_name + " 후보자는" + resume_type_str + "이력서가 존재하지 않습니다.");
              }
            }
          }

        }


        return Json(new
        {
          ok = true,
          waiting_cnt = waiting_cnt,
          total_cnt = total_cnt,
          suc_cnt = suc_cnt,
          fail_cnt = fail_cnt,
          no_cnt = no_cnt,
          message = rtn_msg
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

    public async Task<JsonResult> CancelMakeup(int mr_idx)
    {
      try
      {
        ProjectRepository pr = new ProjectRepository();
        ProjectEntityRepository per = new ProjectEntityRepository();
        AccountRepository ar = new AccountRepository();

        makeup_request data = await per.SelectMakeupRequestOneAsync(mr_idx);
        var myuserinfo = await ar.FindUserBySeqAsync(AppIdentity.user_seq);
        MailService mService = new MailService(myuserinfo.email_id, myuserinfo.email_pwd);

        if (data == null)
          return Json(new
          {
            ok = false,
            message = "취소하려는 요청을 찾을 수 없습니다."
          });

        if (data.receipt_seq > 0)
          return Json(new
          {
            ok = false,
            message = "취소할 수 없는 요청입니다. 담당자에게 직접 요청 해주시기 바랍니다."
          });

        data.del_date = Utils.NowKorea();
        data.del_yn = 1;
        data.del_seq = AppIdentity.user_seq;

        await per.CreateMakeupRequest(data, null, CommonCodes.Update);

        var mk_data = await pr.SelectMakeupRequestOneAsync(data.mr_idx);

        List<string> ToArr = new List<string>();
        ToArr.Add(AppIdentity.email);
        ToArr.Add("makeup@unicosearch.com");
        //메일 Dto
        MakeupCancelDto mailData = new MakeupCancelDto()
        {
          ToArr = ToArr.ToArray()
            ,
          From = new System.Net.Mail.MailAddress(AppIdentity.email, AppIdentity.name)
            ,
          name = AppIdentity.name
            ,
          title = string.Format("[{0}] {1} Makeup 취소 요청.", mk_data.kor_name, (mk_data.resume_type == "K" ? "국문" : "영문"))
            ,
          candidateurl = "http://univision.unicosearch.com/Candidate/CandidateDetail/?c_seq=" + mk_data.c_seq
            ,
          candidatename = mk_data.kor_name
            ,
          filelang = (mk_data.resume_type == "K" ? "국문" : "영문")
        };
        var result_my = mService.SendMakeupCancelMail(mailData, new MakeupCancelTemplete());



        return Json(new
        {
          ok = true,
          message = "Makeup 취소 요청을 완료 했습니다."
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
    /// 프로젝트 진행사항 이력 삭제.
    /// </summary>
    /// <param name="prc_seq"></param>
    /// <returns></returns>
    [HttpPost]
    public async Task<JsonResult> ProjectProgressDelete(int prc_seq)
    {
      try
      {
        ProjectEntityRepository per = new ProjectEntityRepository();

        var recan_his = await per.SelectPjtRecHistory(prc_seq);

        if (recan_his == null)
          return Json(new
          {
            ok = false,
            message = "삭제하려는 후보자의 이력을 찾을 수 없습니다."
          });

        var recan = await per.SelectPjtRecandidateOneAsync(recan_his.pic_seq);

        //var 
        //                var recand = await per.SelectPjtRecandidateOneAsync

        ScheduleEntityRepository ser = new ScheduleEntityRepository();
        var sch = await ser.FindScheduleWithProjectOneAsync(recan_his.prc_seq, recan_his.pic_seq, recan_his.p_seq, recan_his.c_seq);
        var sch_detail = new List<schedule_attend>();
        if (sch != null)
          sch_detail = await ser.FindScheduleAttendListAsync(sch.s_seq);



        await per.CreateOrUpdatePjtRecHistory(recan_his, sch, sch_detail, CommonCodes.Delete, AppIdentity.user_seq, 1);


        var recan_list = await per.SelectPjtRecHisListAsync(recan.pic_seq);
        if (recan_list.Count == 0)
        {
          await per.CreateOrDeletePjtRecandidate(recan);
        }


        return Json(new
        {
          ok = true
            ,
          message = "삭제 했습니다."
        });
      }
      catch (Exception e)
      {
        return Json(new
        {
          ok = false,
          message = "알 수 없는 오류가 발생 했습니다."
        });
      }
    }

    /// <summary>
    /// 프로젝트 진행사항 이력 삭제.
    /// </summary>
    /// <param name="prc_seq"></param>
    /// <returns></returns>
    [HttpPost]
    public async Task<JsonResult> ProjectSetPin(int prc_seq, int pin_to_top = 1)
    {
      try
      {
        ProjectEntityRepository per = new ProjectEntityRepository();

        var recan_his = await per.SelectPjtRecHistory(prc_seq);

        if (recan_his == null)
          return Json(new
          {
            ok = false,
            message = "처리하려는 후보자의 이력을 찾을 수 없습니다."
          });

        var recan = await per.SelectPjtRecandidateOneAsync(recan_his.pic_seq);

        recan.pin_to_top = pin_to_top;
        await per.CreateOrDeletePjtRecandidate(recan, "U");


        return Json(new
        {
          ok = true
            ,
          message = "처리완료"
        });
      }
      catch (Exception e)
      {
        return Json(new
        {
          ok = false,
          message = "알 수 없는 오류가 발생 했습니다."
        });
      }
    }

    /// <summary>
    /// 프로젝트 관심후보자 진행상황 리스트
    /// </summary>
    /// <param name="pic_seq"></param>
    /// <returns></returns>
    public async Task<PartialViewResult> ProjectProgressHistoryList(int pic_seq)
    {
      ProjectRepository pr = new ProjectRepository();
      var list = await pr.SelectProjectRecommandHistoryListAsync(pic_seq);

      return PartialView(list);
    }



    /// <summary>
    /// 프로젝트 세금계산서 담당자 저장
    /// </summary>
    /// <param name="p_seq"></param>
    /// <param name="ctc_seq"></param>
    /// <returns></returns>
    [HttpPost]
    public async Task<JsonResult> ProjectTaxContactSave(int p_seq, int ctc_seq)
    {
      try
      {
        ProjectEntityRepository per = new ProjectEntityRepository();
        var project = await per.SelectProjectOneAsync(p_seq);

        if (project == null)
          return Json(new
          {
            ok = false,
            message = "수정하려는 프로젝트를 찾을 수 없습니다."
          });

        project.ctc_seq = ctc_seq;
        project.modify_dt = Utils.NowKorea();
        project.modify_user = AppIdentity.user_seq;

        await per.UpdateProjectAsync(project, "U", AppIdentity.user_seq, 1);

        return Json(new
        {
          ok = true
            ,
          message = "전자세금계산서 담당자를 저장 했습니다."
        });
      }
      catch (Exception e)
      {
        return Json(new
        {
          ok = false,
          message = "전자 세금계산서 담당자 등록 중, 오류가 발생 했습니다. - " + e.Message
        });
      }
    }


    /// <summary>
    /// 신규 프로젝트 세금계산서 담당자 등록
    /// </summary>
    /// <param name="data"></param>
    /// <returns></returns>
    [HttpPost] // 인보이스 안쓸꺼임 ProjectNewTaxContactSubmit 로 대체
    public async Task<JsonResult> ProjectNewTaxContactSave(client_tax_contact data)
    {
      try
      {
        ProjectEntityRepository per = new ProjectEntityRepository();
        var project = await per.SelectProjectOneAsync(data.p_seq);

        if (project == null)
          return Json(new
          {
            ok = false,
            message = "세금계산서 담당자를 등록하려는 프로젝트를 찾을 수 없습니다."
          });

        ClientEntityRepository cer = new ClientEntityRepository();
        var client = await cer.SelectClientOneAsync(data.c_seq);
        if (client == null)
          return Json(new
          {
            ok = false,
            message = "세금계산서 담당자를 등록하려는 고객사 정보를 찾을 수 없습니다."
          });

        data.create_dt = Utils.NowKorea();
        data.create_seq = AppIdentity.user_seq;
        data.modify_dt = Utils.NowKorea();
        data.modify_seq = AppIdentity.user_seq;

        await cer.CreateOrUpdateTaxContact(data, CommonCodes.Create);

        project.ctc_seq = data.ctc_seq;
        project.modify_dt = Utils.NowKorea();
        project.modify_user = AppIdentity.user_seq;

        await per.UpdateProjectAsync(project, "U", AppIdentity.user_seq, 1);


        return Json(new
        {
          ok = true
            ,
          message = "전자세금계산서 담당자를 저장 했습니다."
        });
      }
      catch (Exception e)
      {
        return Json(new
        {
          ok = false,
          message = "전자 세금계산서 담당자 등록 중, 오류가 발생 했습니다. - " + e.Message
        });
      }
    }



    [HttpPost]
    public async Task<JsonResult> ProjectSetSuggType(int p_seq, int? busi_type, int? job_type)
    {
      try
      {
        ProjectEntityRepository per = new ProjectEntityRepository();
        var project = await per.SelectProjectOneAsync(p_seq);

        if (project == null)
          return Json(new
          {
            ok = false,
            message = "수정하려는 프로젝트를 찾을 수 없습니다."
          });
        if (busi_type != null)
        {
          project.no_business = busi_type;
        }
        if (job_type != null)
        {
          project.no_job = job_type;
        }


        project.modify_dt = Utils.NowKorea();
        project.modify_user = AppIdentity.user_seq;

        await per.UpdateProjectAsync(project, "U", AppIdentity.user_seq, 1);

        return Json(new
        {
          ok = true,
          p_seq = p_seq
        });
      }
      catch (Exception e)
      {
        return Json(new
        {
          ok = false,
          message = "후보자 제안 설정 중, 오류가 발생 했습니다. - " + e.Message
        });
      }
    }

    /// <summary>
    /// 프로젝트 후보자 제안
    /// </summary>
    /// <param name="p_seq"></param>
    /// <returns></returns>
    public async Task<PartialViewResult> ProjectCandiSuggetionList(int p_seq, int page = 1)
    {
      int totalCount = 0;
      ProjectRepository pr = new ProjectRepository();
      ProjectEntityRepository per = new ProjectEntityRepository();


      var project = await pr.SelectProjectOneAsync(p_seq);
      var pjt_keywords = await pr.SelectProjectKeywordListAsync(p_seq);
      var pjt_recandidate = await per.SelectPjtRecandidateListAsync(p_seq);

      SearchEngineRepository ser = new SearchEngineRepository();
      CandidateSearchModel search = new CandidateSearchModel();

      search.is_sugg = 1;
      search.startBirth = project.start_age.ToString();
      search.endBirth = project.end_age.ToString();
      search.gender = Convert.ToInt32(project.gender_type);
      search.school = project.target_school_nm;
      search.major = project.target_major_nm;
      search.company = project.target_company_nm;

      if (!project.no_business.HasValue || project.no_business.Value == 0)
      {
        search.business.Add(new code_business_dtl()
        {
          code1 = Convert.ToSingle(project.business_code1.Value),
          code2 = Convert.ToSingle(project.business_code2.Value)
        });

        if (project.sub_business_code1.HasValue)
        {
          search.business.Add(new code_business_dtl()
          {
            code1 = Convert.ToSingle(project.sub_business_code1.Value),
            code2 = Convert.ToSingle(project.sub_business_code2.Value)
          });
        }

      }
      else if (project.no_business.Value == 2)
      {
        search.business.Add(new code_business_dtl()
        {
          code1 = Convert.ToSingle(project.business_code1.Value),
          code2 = Convert.ToSingle(project.business_code1.Value)
        });

        if (project.sub_business_code1.HasValue)
        {
          search.business.Add(new code_business_dtl()
          {
            code1 = Convert.ToSingle(project.sub_business_code1.Value),
            code2 = Convert.ToSingle(project.sub_business_code1.Value)
          });
        }
      }

      if (!project.no_job.HasValue || project.no_job.Value != 1)
      {
        search.job.Add(new code_job_dtl()
        {
          code1 = Convert.ToSingle(project.job_code1),
          code2 = Convert.ToSingle(project.job_code2)
        });

        if (project.sub_job_code1.HasValue)
        {
          search.job.Add(new code_job_dtl()
          {
            code1 = Convert.ToSingle(project.sub_job_code1.Value),
            code2 = Convert.ToSingle(project.sub_job_code2.Value)
          });
        }

      }
      else if (project.no_job.Value == 2)
      {
        search.job.Add(new code_job_dtl()
        {
          code1 = Convert.ToSingle(project.job_code1),
          code2 = Convert.ToSingle(project.job_code1)
        });

        if (project.sub_job_code1.HasValue)
        {
          search.job.Add(new code_job_dtl()
          {
            code1 = Convert.ToSingle(project.sub_job_code1.Value),
            code2 = Convert.ToSingle(project.sub_job_code1.Value)
          });
        }
      }

      if (pjt_keywords != null)
      {
        foreach (var kw in pjt_keywords)
        {
          search.keyword_list.Add(@"""" + kw.pk_name + @"""");
        }
      }

      if (pjt_recandidate != null)
      {
        foreach (var cand in pjt_recandidate)
        {
          search.except_c_seq.Add(cand.c_seq);
        }
      }
      search.confidential = 0;
      search.inactive = 0;
      search.recent_hire = 0;
      search.modify_dt_start = Utils.ConvertDateTime8DigitToString(Utils.NowKorea().AddYears(-5));
      search.orderOption = String.Empty;
      search.orderTxt = String.Empty;

      //var list = ser.SelectCandSuggetionList(project, pjt_keywords, 1, 4, out totalCount);
      var list = ser.SelectCandidateList(search, page, 8, "", out totalCount);
      //var list = await pr.SelectPjtCandiSuggetionListAsync(p_seq);
      ProjectCandiSuggetionListModel model = new ProjectCandiSuggetionListModel()
      {
        p_seq = p_seq,
        no_business = project.no_business != null ? project.no_business.Value : 0,
        no_job = project.no_job != null ? project.no_job.Value : 0,
        totalCnt = totalCount,
        currentPage = page,
        PagingInfo = new PagingInfo()
        {
          CurrentPage = page
              ,
          ItemsPerPage = 8
              ,
          TotalItems = totalCount
        },
        list = list
      };

      return PartialView(model);
    }

    /// <summary>
    /// 관심후보 등록 - 후보자 제안에서 
    /// </summary>
    /// <param name="p_seq"></param>
    /// <param name="c_seq"></param>
    /// <returns></returns>
    [HttpPost]
    public async Task<JsonResult> ProjectRecandidateCreate(int p_seq, int c_seq, string contents, List<string> resume_type, int state = ProjectHistoryStateCode.interest)
    {
      try
      {
        ProjectEntityRepository per = new ProjectEntityRepository();
        var rec = await per.SelectPjtRecandidateOneAsync(p_seq, c_seq);

        if (rec != null)
          return Json(new
          {
            ok = false,
            message = "이미 잠재후보로 등록된 후보자 입니다."
          });

        if (string.IsNullOrWhiteSpace(contents))
          contents = "AI 제안에서 잠재후보로 등록됨.";


        pjt_recandidate recandidate = new pjt_recandidate
        {
          p_seq = p_seq
            ,
          c_seq = c_seq
            ,
          create_dt = Utils.NowKorea()
            ,
          create_user = AppIdentity.user_seq
            ,
          modify_dt = Utils.NowKorea()
            ,
          modify_user = AppIdentity.user_seq
        };

        pjt_recandidate_history history = new pjt_recandidate_history()
        {
          p_seq = p_seq
            ,
          c_seq = c_seq
            ,
          state = ProjectHistoryStateCode.interest
            ,
          schedule_date = Utils.NowKorea()
            ,
          contents = contents
            ,
          create_dt = Utils.NowKorea()
            ,
          create_user = AppIdentity.user_seq
            ,
          modify_dt = Utils.NowKorea()
            ,
          modify_user = AppIdentity.user_seq
        };


        await per.CreateOrUpdatePjtRecandidate(recandidate, history);

        if (resume_type == null)
        {
          resume_type = new List<string>();
        }

        if (state == 35 && resume_type.Count > 0)
        {
          return await RequestMakeup(p_seq, resume_type, 1, new List<int> { recandidate.pic_seq });
        }
        else if (state == ProjectHistoryStateCode.recommand && resume_type.Count == 0)
        {
          return await ProjectProgressCreate(p_seq, c_seq, recandidate.pic_seq, 0, state, Utils.ConvertDateTimeHourToString(Utils.NowKorea()), 0, 0, String.Empty, 0, null, "추천후보 등록", 0, 0, null);
        }

        //RequestMakeup(int p_seq, string resume_type, int with_recommand, List<int> pic_seq_list)
        return Json(new
        {
          ok = true
            ,
          message = "잠재후보로 등록했습니다."
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

    /// <summary>
    /// 프로젝트 담당자 저장
    /// </summary>
    /// <param name="p_seq"></param>
    /// <param name="cc_seq"></param>
    /// <returns></returns>
    [HttpPost]
    public async Task<JsonResult> ProjectContactSave(int p_seq, int cc_seq)
    {
      try
      {
        ProjectEntityRepository per = new ProjectEntityRepository();
        var project = await per.SelectProjectOneAsync(p_seq);

        if (project == null)
          return Json(new
          {
            ok = false,
            message = "수정하려는 프로젝트를 찾을 수 없습니다."
          });

        project.cc_seq = cc_seq;
        project.modify_dt = Utils.NowKorea();
        project.modify_user = AppIdentity.user_seq;

        await per.UpdateProjectAsync(project, "U", AppIdentity.user_seq, 1);

        return Json(new
        {
          ok = true
            ,
          message = "프로젝트 담당자를 저장 했습니다."
        });
      }
      catch (Exception e)
      {
        return Json(new
        {
          ok = false,
          message = "프로젝트 담당자 등록 중, 오류가 발생 했습니다. - " + e.Message
        });
      }
    }

    [HttpPost] // 인보이스 방식 변경 후 안쓸거임 ProjectNewContactSubmit으로 씀
    public async Task<JsonResult> ProjectNewContactSave(client_contact data)
    {
      try
      {
        ProjectEntityRepository per = new ProjectEntityRepository();
        var project = await per.SelectProjectOneAsync(data.p_seq);

        if (project == null)
          return Json(new
          {
            ok = false,
            message = "담당자를 등록하려는 프로젝트를 찾을 수 없습니다."
          });

        ClientEntityRepository cer = new ClientEntityRepository();
        var client = await cer.SelectClientOneAsync(data.c_seq);
        if (client == null)
          return Json(new
          {
            ok = false,
            message = "담당자를 등록하려는 고객사 정보를 찾을 수 없습니다."
          });

        data.create_dt = Utils.NowKorea();
        data.create_user = AppIdentity.user_seq;
        data.modify_dt = Utils.NowKorea();
        data.modify_user = AppIdentity.user_seq;

        await cer.CreateOrDeleteCliContact(data, CommonCodes.Create);

        project.cc_seq = data.cc_seq;
        project.modify_dt = Utils.NowKorea();
        project.modify_user = AppIdentity.user_seq;

        await per.UpdateProjectAsync(project, "U", AppIdentity.user_seq, 1);

        return Json(new
        {
          ok = true
            ,
          message = "프로젝트 담당자를 저장 했습니다."
        });
      }
      catch (Exception e)
      {
        return Json(new
        {
          ok = false,
          message = "프로젝트 담당자 등록 중, 오류가 발생 했습니다. - " + e.Message
        });
      }
    }



    /// <summary>
    /// 프로젝트 간편 후보자 리스트
    /// </summary>
    /// <param name="p_seq"></param>
    /// <param name="page"></param>
    /// <returns></returns>
    public PartialViewResult ProjectSimpleCandidateList(int p_seq, int page = 1)
    {
      ProjectRepository pr = new ProjectRepository();

      int totalCount = 0;

      //한 페이지당 몇개의 행을 보여줄 것인지
      int pageSize = AppPaging.PageLength10;

      var list = pr.SelectProjectSimpleCandidateList(p_seq, (page - 1) * pageSize, pageSize, out totalCount);

      ProjectSimpleCandidateListModel model = new ProjectSimpleCandidateListModel()
      {
        p_seq = p_seq
          ,
        list = list
          ,
        PagingInfo = new PagingInfo()
        {
          CurrentPage = page
              ,
          ItemsPerPage = pageSize
              ,
          TotalItems = totalCount
        }
      };

      return PartialView(model);
    }

    [HttpPost]
    public async Task<JsonResult> CreateSimpleCandidate(string kor_name, string gender, string birthdate, string ex_birthdate, string cell_phone, string email, string school, string company)
    {
      try
      {






        return Json(new
        {
          ok = true,
          message = "간편 후보 등록 했습니다."
        });
      }
      catch (Exception e)
      {
        return Json(new
        {
          ok = false,
          message = "알 수 없는 오류가 발생 했습니다."
        });
      }
    }

    /// <summary>
    /// 프로젝트 인보이스 리스트
    /// </summary>
    /// <param name="p_seq"></param>
    /// <param name="page"></param>
    /// <returns></returns>
    public PartialViewResult ProjectInvoiceList(int p_seq, int page = 1)
    {
      ProjectRepository pr = new ProjectRepository();

      int totalCount = 0;

      //한 페이지당 몇개의 행을 보여줄 것인지
      int pageSize = AppPaging.PageLength5;

      var list = pr.SelectProjectInvoiceList(p_seq, (page - 1) * pageSize, pageSize, out totalCount);

      ProjectInvoiceListModel model = new ProjectInvoiceListModel()
      {
        p_seq = p_seq,
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

      return PartialView(model);
    }

    /// <summary>
    /// 인보이스 생성
    /// </summary>
    /// <param name="p_seq"></param>
    /// <param name="c_seq"></param>
    /// <param name="pii_seq"></param>
    /// <returns></returns>
    public async Task<PartialViewResult> ProjectInvoiceCreate(int p_seq, int c_seq, int pii_seq)
    {
      ProjectRepository pr = new ProjectRepository();

      var data = await pr.SelectProjectInvoiceOneAsync(p_seq, c_seq, pii_seq);
      var chargeList = await pr.SelectProjectInvoiceChargeOneAsync(p_seq);
      var retainer_money = await pr.SelectProjectRetainerMoneyAsync(p_seq, pii_seq);
      if (chargeList.Count == 1)
      {
        chargeList.FirstOrDefault().sales_rate = 100;
        chargeList.FirstOrDefault().sales_money = (double)data.ann_income * ((double)data.fee_rate * 0.01);
      }
      if (retainer_money > 0)
      {
        data.invoice_type = 2;
      }

      ProjectInvoiceCreateModel model = new ProjectInvoiceCreateModel()
      {
        p_seq = data.p_seq,
        c_seq = data.c_seq,
        cc_seq = data.cc_seq,
        ctc_seq = data.ctc_seq,
        pii_seq = data.pii_seq,
        prc_seq = data.prc_seq,
        candidate_name = data.candidate_name,
        candidate_eng_name = data.candidate_eng_name,
        join_dt = data.join_dt,
        billing_dt = data.billing_dt,
        annual_income = data.annual_income,
        ann_income = data.ann_income,
        income_currency_cd = data.income_currency_cd,
        fee_rate = data.fee_rate,
        billing_money = data.billing_money,
        billing_total = data.billing_total,
        billing_won = data.billing_won,
        billing_amt = data.billing_amt,
        billing_vat = data.billing_vat,
        retainer_amt = (retainer_money * -1),
        bill_currency_cd = data.bill_currency_cd,
        vat_type = data.vat_type,
        is_po_no = data.is_po_no,
        billing_type = data.billing_type,
        expire_guarantee = data.expire_guarantee,
        client_seq = data.client_seq,
        client_name = data.client_name,
        client_eng_name = data.client_eng_name,
        ceo = data.ceo,
        addr = data.addr,
        biz_code = data.biz_code,
        is_open_name = data.is_open_name,
        invoice_lang = data.invoice_lang,
        invoice_type = data.invoice_type,
        is_open_annual_income = data.is_open_annual_income,
        remarks = data.remarks,
        candidate_source = data.candidate_source,
        candidate_source_txt = data.candidate_source_txt,
        candidate_position = data.candidate_position,
        candidate_position_txt = data.candidate_position_txt,
        chargeList = chargeList
      };

      return PartialView(model);
    }

    [HttpPost]
    public async Task<JsonResult> ProjectTempInvoiceCreate(ProjectInvoiceCreateModel data)
    {
      try
      {
        using (HttpClient client = new HttpClient())
        {
          string url = "http://10.1.2.150:9999/api/invoice/temp_invoice_file_proc.asp";

          //string json = JsonConvert.SerializeObject(data);

          //StringContent content = new StringContent(json, Encoding.UTF8, "application/json");

          var values = new Dictionary<string, string>
          {
            { "p_seq", data.p_seq.ToString() },
            { "c_seq", data.c_seq.ToString() },
            { "billing_money", data.billing_money.ToString() },
            { "bill_currency_cd", data.bill_currency_cd.ToString() },
            { "invoice_type", data.invoice_type.ToString() },
            { "invoice_lang", data.invoice_lang.ToString() },
            { "vat_type", data.vat_type.ToString() },
            { "is_open_name", data.is_open_name.ToString() },
            { "is_open_annual_income", data.is_open_annual_income.ToString() },
            { "candidate_name", data.candidate_name.ToString() },
            { "join_dt", data.join_dt.ToString() }
          };
          string responseBody = "";
          var content = new FormUrlEncodedContent(values);
          try
          {
            // Send a POST request to the specified URL with the JSON content
            HttpResponseMessage response = await client.PostAsync(url, content);

            // Ensure the request was successful
            response.EnsureSuccessStatusCode();

            // Read the response content as a string
            responseBody = await response.Content.ReadAsStringAsync();

            JObject jsonResponse = JObject.Parse(responseBody);

            // Output the response body
            //Console.WriteLine(responseBody);
            return Json(new
            {
              ok = (bool)jsonResponse["rtn"],
              file_url = jsonResponse["file_url"].ToString()
            });
          }
          catch (HttpRequestException e)
          {
            // Handle any HTTP request exceptions
            throw new Exception($"Request error: {e.Message}");
          }



        }
      }
      catch (Exception e)
      {
        return Json(new
        {
          ok = false,
          message = "알 수 없는 오류가 발생 했습니다. " + e.Message
        });
      }
    }
    /// <summary>
    /// 인보이스 생성
    /// </summary>
    /// <param name="data"></param>
    /// <returns></returns>
    [HttpPost]
    public async Task<JsonResult> ProjectInvoiceCreate(ProjectInvoiceCreateModel data, int? is_success)
    {
      try
      {
        bool is_success_ok = false;
        string share_str = String.Empty;

        ProjectEntityRepository per = new ProjectEntityRepository();

        ClientRepository cr = new ClientRepository();
        ProjectRepository pr = new ProjectRepository();

        var project = await pr.SelectProjectOneAsync(data.p_seq);

        //프로젝트에 고객사 세금계산서 담당자 정보가 없을 경우.
        if (!project.ctc_seq.HasValue)
          return Json(new
          {
            ok = false,
            message = "해당 프로젝트에 고객사 세금계산서 담당자 정보가 없습니다."
          });

        //var tax_contact = await cr.SelectTexWithDetailOneAsync((int)project.ctc_seq);

        pjt_invoice_info info = new pjt_invoice_info()
        {
          p_seq = data.p_seq,
          c_seq = data.c_seq,
          prc_seq = data.prc_seq,
          join_dt = data.join_dt == null ? data.billing_dt : data.join_dt,
          billing_dt = data.billing_dt,
          annual_income = data.annual_income,
          ann_income = data.ann_income,
          income_currency_cd = data.income_currency_cd,
          fee_rate = data.fee_rate,
          billing_money = data.billing_money,
          bill_currency_cd = data.bill_currency_cd,
          billing_total = data.billing_total,
          billing_amt = data.billing_amt,
          billing_vat = data.billing_vat,
          billing_won = data.billing_won,
          vat_type = data.vat_type,
          is_po_no = data.is_po_no,
          billing_type = data.billing_type,
          expire_guarantee = data.expire_guarantee,
          is_open_name = data.is_open_name,
          is_open_annual_income = data.is_open_annual_income,
          invoice_lang = data.invoice_lang,
          invoice_type = data.invoice_type,
          remarks = data.remarks,
          create_dt = Utils.NowKorea(),
          create_user = AppIdentity.user_seq,
          modify_dt = Utils.NowKorea(),
          modify_user = AppIdentity.user_seq,
          candidate_name = data.candidate_name,
          client_name = data.client_name,
          client_ceo = data.client_ceo,
          client_addr1 = data.client_addr1,
          client_biz_code = data.client_biz_code,
          client_contact_name = data.client_contact_name,
          client_contact_email = data.client_contact_email,
          client_contact_phone = data.client_contact_tel,
          etax_name = data.client_etax_name,
          etax_email = data.client_etax_email,
          etax_phone = data.client_etax_tel,
          candidate_source = data.candidate_source,
          candidate_source_txt = data.candidate_source_txt,
          candidate_position = data.candidate_position,
          candidate_position_txt = data.candidate_position_txt,
          is_deleted = 0
        };

        List<pjt_invoice_sales> sales = new List<pjt_invoice_sales>();
        foreach (var charge in data.chargeList)
        {
          pjt_invoice_sales sale = new pjt_invoice_sales()
          {
            prc_seq = data.prc_seq,
            c_seq = data.c_seq,
            p_seq = data.p_seq,
            uv_seq = charge.uv_seq,
            ud_seq = charge.ud_seq,
            sales_rate = charge.sales_rate,
            sales_money = charge.sales_money,
            comments = charge.comments,
            create_dt = Utils.NowKorea(),
            create_user = AppIdentity.user_seq,
            modify_dt = Utils.NowKorea(),
            modify_user = AppIdentity.user_seq,
          };
          share_str += (!String.IsNullOrEmpty(share_str) ? "<br/><br/>" : "") + " - " + charge.name + " ( " + charge.sales_rate.ToString() + "%) : " + charge.comments;
          sales.Add(sale);
        }
        await per.CreatePjtInvoice(info, sales);

        //해당 후보자로 같은 회사에서 중복 발행된 인보이스가 있는지 카운트.
        int sameCnt = await pr.SelectCandidateSameInvoiceCountAsync((int)data.c_seq, project.c_seq);

        //후보자 연봉 및 최종 경력 정보 등록
        CandidateEntityRepository cer = new CandidateEntityRepository();
        ClientEntityRepository clientr = new ClientEntityRepository();

        candidate candidate = new candidate();
        client client = new client();

        //order no 가 0이고, 후보자가 같은 회사의 인보이스로 중복 발행된 건이 있으면 
        //후보자의 연봉 정보를 업데이트 하지 않는다.
        if (sameCnt == 1) // order no == 0 추가 해야 함.
        {
          candidate = await cer.SelectCandidateOneAsync((int)info.c_seq);

          client = await clientr.SelectClientOneAsync(project.c_seq);


          List<can_career> ccareer = await cer.SelectCanCareerListAsync(candidate.c_seq);

          if (ccareer != null)
          {
            if (ccareer.Count > 0)
            {
              foreach (var career in ccareer)
              {
                career.is_work = 0;
                await cer.UpdateCandidateCareerOneAsync(career);
              }
            }
          }

          //후보자 연봉 정보 바인딩
          candidate.hope_salary = data.annual_income;
          candidate.caution_type = 6;
          can_career cc = new can_career()
          {
            c_seq = candidate.c_seq,
            company_name = string.IsNullOrWhiteSpace(client.kor_name) ? client.eng_name : client.kor_name,
            join_dt = ((DateTime)info.join_dt).ToString("yyyy-MM"),
            annual_income = data.annual_income,
            division_name = (project.position_str + "/" + Utils.ReturnPositionCodeTxt(project.position_seq)),
            is_work = 1,
            p_code = project.position_seq
          };

          await cer.CreateCandidateCareerOneAsync(cc);

          CandidateRepository canrep = new CandidateRepository();
          var procMsg = await canrep.SaveCandidateTxtMsg(candidate.c_seq);
          var procMsg2 = await canrep.UpdateCandidateCareerRange(candidate.c_seq);
        }


        //후보자 영문명 저장 해야 할 경우,
        if (data.isEngCandiSave)
        {
          //후보자 정보가 바인딩 되지 않았다면 조회.
          if (candidate.c_seq == 0)
            candidate = await cer.SelectCandidateOneAsync((int)info.c_seq);

          candidate.eng_name = info.candidate_name;
        }

        //후보자 정보가 바인딩 되어 있다면, 업데이트(연봉 및 영문명 저장에만 바인딩 되므로)
        //밖으로 빼는 이유는 연봉 저장과 영문명 저장이 동시에 있을 경우, DB 접속을 최소화 하기 위해.
        if (candidate.c_seq != 0)
          await cer.UpdateCandidateOneAsync(candidate);


        //고객사 영문명 저장 해야 할 경우,
        if (data.isEngCompanySave)
        {
          //고객사 정보가 바인딩 되지 않았다면 조회.
          if (client.c_seq == 0)
            client = await clientr.SelectClientOneAsync(project.c_seq);

          client.eng_name = info.client_name;

          await clientr.UpdateClientOneAsync(client);
        }

        var project_stat = await per.SelectProjectOneAsync(data.p_seq);
        if (is_success == 1)
        {
          if (project_stat != null && project_stat.pjt_status != 5)
          {
            project_stat.pjt_status = 5;
            //if (!project_stat.close_dt.HasValue)
            project_stat.close_dt = Utils.NowKorea();
            project_stat.status_comment = "인보이스 발행으로 자동 성공 처리";
            project_stat.modify_dt = Utils.NowKorea();
            project_stat.modify_user = AppIdentity.user_seq;
            await per.UpdateProjectAsync(project_stat, "U", AppIdentity.user_seq, 1);
            is_success_ok = true;
          }

        }



        AccountRepository ar = new AccountRepository();
        ApiEntityRepository apier = new ApiEntityRepository();
        List<alarm_user> me = new List<alarm_user>();
        List<string> ToArr = new List<string>();
        List<alarm_user> aList = new List<alarm_user>();


        var myuserinfo = await ar.FindUserBySeqAsync(AppIdentity.user_seq);
        MailService mService = new MailService(myuserinfo.email_id, myuserinfo.email_pwd);

        alarm_message aMessage = new alarm_message()
        {
          href_url = "/Project/ProjectDetail?p_seq=" + data.p_seq,
          message = string.Format("[{0}] 인보이스 요청 완료 되었습니다.", project_stat.title),
          create_dt = Utils.NowKorea()
        };

        var userList = await pr.SelectProjectAmListAsync(data.p_seq);

        foreach (var user in userList)
        {
          //이미 리스트에 포함되어 있는 uv_seq라면 넘김.
          if (aList.Where(x => (x.uv_seq == user.uv_seq)).FirstOrDefault() != null)
            continue;

          alarm_user alarm = new alarm_user()
          {
            uv_seq = user.uv_seq
              ,
            is_read = 0
          };

          aList.Add(alarm);

          //담당자 이메일 추가. (최대 3건이라 건별로 조회)
          var confirmUser = await ar.FindUserBySeqAsync(user.uv_seq);
          ToArr.Add(confirmUser.email);
        }

        var searcherList = await pr.SelectProjectSearcherListAsync(data.p_seq);

        foreach (var user in searcherList)
        {
          //이미 리스트에 포함되어 있는 uv_seq라면 넘김.
          if (aList.Where(x => (x.uv_seq == user.uv_seq)).FirstOrDefault() != null)
            continue;

          alarm_user alarm = new alarm_user()
          {
            uv_seq = user.uv_seq
              ,
            is_read = 0
          };

          aList.Add(alarm);

          //담당자 이메일 추가. (최대 3건이라 건별로 조회)
          var confirmUser = await ar.FindUserBySeqAsync(user.uv_seq);
          ToArr.Add(confirmUser.email);
        }

        ToArr.Add("unico@unicosearch.com");
        //ToArr.Add("planning@unicosearch.com");

        await apier.CreateAlarm(aMessage, aList);

        //메일 Dto
        InvoiceCreateDto mailData = new InvoiceCreateDto()
        {
          ToArr = ToArr.ToArray()
            ,
          From = new System.Net.Mail.MailAddress(AppIdentity.email, AppIdentity.name)
            ,
          name = AppIdentity.name
            ,
          title = string.Format("[{0}] 인보이스 요청 건 {1}", project_stat.title, (data.is_po_no == 1 ? "(* PO NO 필요)" : ""))
            ,
          comment = data.remarks.Replace("\r\n", "\n").Replace("\n", "<br />")
            ,
          clientname = data.client_name
            ,
          joindate = (data.join_dt != null ? "(출근일 : " + Utils.ConvertDateTimeToString(data.join_dt) + ")" : "")
            ,
          candidatesourcetxt = data.candidate_source_txt
            ,
          candidatepositiontxt = data.candidate_position_txt
            ,
          ceo = data.client_ceo
            ,
          address = data.client_addr1
            ,
          candidatename = data.candidate_name
            ,
          vattype = "[" + Utils.ReturnVatTypeTxt(data.vat_type) + "]"
            ,
          income = Utils.ConvertMoneyToString(data.ann_income)
            ,
          feerate = data.fee_rate.ToString()
            ,
          fee = Utils.ConvertMoneyToString(data.billing_total)
            ,
          amt = Utils.ConvertMoneyToString(data.billing_amt)
            ,
          vat = Utils.ConvertMoneyToString(data.billing_vat)
            ,
          pono = (data.is_po_no == 1 ? "<span style='color:red;'>* PO NO : 필요</span><br/><br/>" : "")
          ,
          contactname = data.client_contact_name
            ,
          contactemail = data.client_contact_email
            ,
          contactphone = data.client_contact_tel
            ,
          etaxname = data.client_etax_name
            ,
          etaxmail = data.client_etax_email
            ,
          etaxphone = data.client_etax_tel
            ,
          feeshare = share_str
            ,
          url = "http://univision.unicosearch.com/Project/ProjectDetail?p_seq=" + data.p_seq
        };
        var result_my = mService.SendInvoiceCreateMail(mailData, new InvoiceCreateTemplete());

        /*
         * 담당자에게 별도로 전송
         */
        List<string> SupportToArr = new List<string>();

        SupportToArr.Add("narae@unicosearch.com");
        SupportToArr.Add("claire@unicosearch.com");

        mailData.ToArr = SupportToArr.ToArray();

        var result_support = mService.SendInvoiceCreateMail(mailData, new InvoiceCreateTemplete());

        return Json(new
        {
          ok = true,
          is_success_ok = is_success_ok
        });
      }
      catch (Exception e)
      {
        return Json(new
        {
          ok = false,
          message = "인보이스 발행 중, 오류가 발생 했습니다. - " + e.Message
        });
      }
    }

    [HttpPost]
    public async Task<JsonResult> ProjectInvoiceDelete(int pii_seq)
    {
      try
      {
        ProjectEntityRepository per = new ProjectEntityRepository();

        var invoice = await per.SelectPjtInvoiceInfoOneAsnyc(pii_seq);
        if (invoice == null)
          return Json(new
          {
            ok = false,
            message = "삭제하려는 인보이스를 찾을 수 없습니다."
          });

        if (invoice.confirm_dt != null)
          return Json(new
          {
            ok = false,
            message = "승인된 인보이스는 삭제 할 수 없습니다."
          });

        invoice.is_deleted = 1;
        await per.UpdatePjtInvoice(invoice, AppIdentity.user_seq);

        return Json(new
        {
          ok = true,
          message = "인보이스를 삭제 했습니다."
        });
      }
      catch (Exception e)
      {
        return Json(new
        {
          ok = false,
          message = "알 수 없는 오류가 발생 했습니다."
        });
      }
    }



    /// <summary>
    /// 인보이스 View
    /// </summary>
    /// <param name="p_seq"></param>
    /// <param name="c_seq"></param>
    /// <param name="pii_seq"></param>
    /// <returns></returns>
    public async Task<PartialViewResult> ProjectInvoiceView(int p_seq, int? c_seq, int pii_seq)
    {
      ProjectRepository pr = new ProjectRepository();
      int c_seq_n = 0;
      if (c_seq.HasValue)
      {
        c_seq_n = c_seq.Value;
      }

      //var data = await pr.SelectProjectInvoiceOneAsync(p_seq, c_seq_n, pii_seq);
      var data = await pr.SelectProjectInvoiceViewOneAsync(p_seq, pii_seq);
      var chargeList = await pr.SelectProjectInvoiceSalesListAsync(pii_seq);
      var retainer_money = await pr.SelectProjectRetainerMoneyAsync(p_seq, pii_seq);

      ProjectInvoiceCreateModel model = new ProjectInvoiceCreateModel()
      {
        p_seq = data.p_seq,
        c_seq = data.c_seq,
        cc_seq = data.cc_seq,
        ctc_seq = data.ctc_seq,
        pii_seq = data.pii_seq,
        prc_seq = data.prc_seq,
        candidate_name = data.candidate_name,
        candidate_eng_name = data.candidate_eng_name,
        join_dt = data.join_dt,
        billing_dt = data.billing_dt,
        annual_income = data.annual_income,
        ann_income = data.ann_income,
        income_currency_cd = data.income_currency_cd,
        fee_rate = data.fee_rate,
        billing_money = data.billing_money,
        billing_total = data.billing_total,
        billing_amt = data.billing_amt,
        billing_vat = data.billing_vat,
        billing_won = data.billing_won,
        bill_currency_cd = data.bill_currency_cd,
        retainer_amt = retainer_money,
        vat_type = data.vat_type,
        is_po_no = data.is_po_no,
        billing_type = data.billing_type,
        expire_guarantee = data.expire_guarantee,
        client_seq = data.client_seq,
        client_name = data.client_name,
        client_eng_name = data.client_eng_name,
        ceo = data.ceo,
        addr = data.addr,
        biz_code = data.biz_code,
        is_open_name = data.is_open_name,
        invoice_lang = data.invoice_lang,
        invoice_type = data.invoice_type,
        is_open_annual_income = data.is_open_annual_income,
        remarks = data.remarks,
        client_contact_name = data.client_contact_name,
        client_contact_tel = data.client_contact_phone,
        client_contact_email = data.client_contact_email,
        client_etax_name = data.etax_name,
        client_etax_tel = data.etax_phone,
        client_etax_email = data.etax_email,
        candidate_source = data.candidate_source,
        candidate_source_txt = data.candidate_source_txt,
        chargeList = chargeList
      };

      return PartialView(model);
    }

    /// <summary>
    /// 프로젝트 메모 리스트
    /// </summary>
    /// <param name="p_seq"></param>
    /// <param name="page"></param>
    /// <returns></returns>
    public PartialViewResult ProjectMemoList(int p_seq, int page = 1)
    {
      ProjectRepository pr = new ProjectRepository();

      int totalCount = 0;

      //한 페이지당 몇개의 행을 보여줄 것인지
      int pageSize = AppPaging.PageLength5;

      var list = pr.SelectProjectMemoList(p_seq, (page - 1) * pageSize, pageSize, out totalCount);

      ProjectMemoListModel model = new ProjectMemoListModel()
      {
        p_seq = p_seq,
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

      return PartialView(model);
    }

    /// <summary>
    /// 프로젝트 메모 등록
    /// </summary>
    /// <param name="p_seq"></param>
    /// <param name="memo"></param>
    /// <returns></returns>
    [HttpPost]
    public async Task<JsonResult> CreateProjectMemo(int p_seq, string memo)
    {
      try
      {
        ProjectEntityRepository per = new ProjectEntityRepository();


        pjt_memo pm = new pjt_memo()
        {
          p_seq = p_seq
            ,
          memo = memo
            ,
          create_user = AppIdentity.user_seq
            ,
          create_dt = Utils.NowKorea()
        };

        await per.CreateOrDeletePjtMemo(pm, CommonCodes.Create);

        return Json(new
        {
          ok = true,
          message = "등록 했습니다."
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

    /// <summary>
    /// 프로젝트 메모 삭제.
    /// </summary>
    /// <param name="pm_seq"></param>
    /// <returns></returns>
    [HttpPost]
    public async Task<JsonResult> DeleteProjectMemo(int pm_seq)
    {
      try
      {
        ProjectEntityRepository per = new ProjectEntityRepository();

        var pm = await per.SelectPjtMemoOneAsync(pm_seq);
        if (pm == null)
          return Json(new
          {
            ok = false
              ,
            message = "삭제하려는 메모를 찾을 수 없습니다."
          });


        await per.CreateOrDeletePjtMemo(pm, CommonCodes.Delete, AppIdentity.user_seq);

        return Json(new
        {
          ok = true,
          message = "삭제 했습니다."
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

    /// <summary>
    /// 프로젝트 상세 - 평판조회 -
    /// </summary>
    /// <param name="p_seq"></param>
    /// <returns></returns>
    public async Task<ActionResult> ProjectRepDetail(int p_seq)
    {
      ProjectRepository pr = new ProjectRepository();

      var data = await pr.SelectProjectOneAsync(p_seq);
      if (data == null)
        data = new project();

      ProjectCreateRepModel model = new ProjectCreateRepModel()
      {
        p_seq = data.p_seq,
        c_seq = data.c_seq,
        company_name = data.company_name,
        cc_seq = data.cc_seq,
        ctc_seq = data.ctc_seq,
        pjt_type = data.pjt_type,
        pjt_status = data.pjt_status,
        status_comment = data.status_comment,
        title = data.title,
        exp_salary = data.exp_salary,
        exp_salary_won = data.exp_salary_won,
        expected_salary = data.expected_salary,
        currency_cd = data.currency_cd,
        fee_rate = data.fee_rate,
        client_require = data.client_require,
        internal_note = data.internal_note,
        tax_division = data.tax_division,
        tax_name = data.tax_name,
        tax_email = data.tax_email,
        tax_phone = data.tax_phone,
        tax_cell_phone = data.tax_cell_phone,
        tax_deposit_manager = data.tax_deposit_manager,
        tax_deposit_email = data.tax_deposit_email,
        contact_name = data.contact_name,
        contact_gender = data.contact_gender,
        contact_email = data.contact_email,
        contact_phone = data.contact_phone,
        contact_cell_phone = data.contact_cell_phone,
        contact_division = data.contact_division,
        contact_position = data.contact_position,
        business_code1 = data.business_code1,
        business_code2 = data.business_code2,
        business_name1 = data.business_names1,
        business_name2 = data.business_names2,
        job_code1 = data.job_code1,
        job_code2 = data.job_code2,
        job_name1 = data.job_names1,
        job_name2 = data.job_names2,
        is_share_pjt = data.is_share_pjt,
        is_cowork = data.is_cowork
      };

      model.amList = await pr.SelectProjectAmListAsync(p_seq);
      model.searcherList = await pr.SelectProjectSearcherListAsync(p_seq);

      return View(model);
    }

    public PartialViewResult ProjectFileList(int p_seq)
    {
      ProjectRepository pr = new ProjectRepository();

      int totalCount = 0;

      //한 페이지당 몇개의 행을 보여줄 것인지

      var list = pr.SelectProjectFileList(p_seq, 0, 100, out totalCount);

      ProjectFileListModel model = new ProjectFileListModel()
      {
        p_seq = p_seq
          ,
        list = list
      };

      return PartialView(model);
    }

    /// <summary>
    /// 프로젝트 관련파일 리스트
    /// </summary>
    /// <param name="p_seq"></param>
    /// <param name="page"></param>
    /// <returns></returns>
    public PartialViewResult ProjectRepFileList(int p_seq, int page = 0)
    {
      ProjectRepository pr = new ProjectRepository();

      int totalCount = 0;

      //한 페이지당 몇개의 행을 보여줄 것인지
      int pageSize = AppPaging.PageLength10;

      var list = pr.SelectProjectFileList(p_seq, (page - 1) * pageSize, pageSize, out totalCount);

      ProjectFileListModel model = new ProjectFileListModel()
      {
        p_seq = p_seq
          ,
        list = list
          ,
        PagingInfo = new PagingInfo()
        {
          CurrentPage = page
              ,
          ItemsPerPage = pageSize
              ,
          TotalItems = totalCount
        }
      };

      return PartialView(model);
    }

    /// <summary>
    /// 관련파일 등록
    /// </summary>
    /// <param name="p_seq"></param>
    /// <param name="file_type_name"></param>
    /// <param name="files"></param>
    /// <returns></returns>
    [HttpPost]
    public async Task<JsonResult> CreateProjectFile(int p_seq, string file_type_name, HttpPostedFileBase files)
    {
      try
      {
        FileUpload fi = new FileUpload();
        UploadFileResult fiResult = new UploadFileResult();
        fiResult.status = true;

        if (files != null)
          fiResult = fi.UploadCommon(Server.MapPath("~/UploadedFiles"), "PjtRepFiles\\" + p_seq, files);

        //파일 업로드에서 오류 발생 시,
        if (!fiResult.status)
        {
          return Json(new
          {
            ok = false,
            message = "프로젝트 관련 파일 업로드 중에 오류가 발생 했습니다. - " + fiResult.statusMessage
          });
        }
        else
        {
          ProjectEntityRepository per = new ProjectEntityRepository();

          pjt_file file = new pjt_file()
          {
            p_seq = p_seq,
            file_dir = Path.Combine(Utils.GetRootUrl(Request), fiResult.dbPath),
            file_origin_path = fiResult.filePath,
            file_path = fiResult.originName,
            file_extension = fiResult.extension,
            file_type_name = file_type_name,
            create_dt = Utils.NowKorea(),
            create_user = AppIdentity.user_seq
          };

          await per.CreateOrUpdatePjtFile(file, CommonCodes.Create);

          return Json(new
          {
            ok = true,
            message = "관련파일을 등록 했습니다.",
            result = file
          });
        }
      }
      catch (Exception e)
      {
        return Json(new
        {
          ok = false,
          message = "알 수 없는 오류가 발생 했습니다."
        });
      }
    }


    [HttpPost]
    public async Task<JsonResult> DeletePjtFile(int pf_seq)
    {
      try
      {
        ProjectEntityRepository per = new ProjectEntityRepository();

        var pf = await per.SelectPjtFileOneAsync(pf_seq);

        if (pf == null)
          return Json(new
          {
            ok = false
              ,
            message = "삭제하려는 파일을 찾을 수 없습니다."
          });

        pf.remove_dt = Utils.NowKorea();
        pf.remove_user = AppIdentity.user_seq;
        await per.DeletePjtFile(pf);

        return Json(new
        {
          ok = true,
          message = "파일을 삭제 했습니다."
        });

      }
      catch (Exception e)
      {
        return Json(new
        {
          ok = false,
          message = "알 수 없는 오류가 발생 했습니다."
        });
      }
    }


    /// <summary>
    /// 프로젝트 - 평판조회 인보이스
    /// </summary>
    /// <param name="p_seq"></param>
    /// <param name="page"></param>
    /// <returns></returns>
    public PartialViewResult ProjectRepInvoiceList(int p_seq, int page = 1)
    {
      ProjectRepository pr = new ProjectRepository();

      int totalCount = 0;

      //한 페이지당 몇개의 행을 보여줄 것인지
      int pageSize = AppPaging.PageLength5;

      var list = pr.SelectProjectRepInvoiceList(p_seq, (page - 1) * pageSize, pageSize, out totalCount);

      ProjectInvoiceListModel model = new ProjectInvoiceListModel()
      {
        p_seq = p_seq,
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

      return PartialView(model);
    }

    /// <summary>
    /// 프로젝트 - 평판조회 인보이스 생성
    /// </summary>
    /// <param name="p_seq"></param>
    /// <param name="c_seq"></param>
    /// <param name="pii_seq"></param>
    /// <returns></returns>
    public async Task<PartialViewResult> ProjectRepInvoiceCreate(int p_seq)
    {
      ProjectRepository pr = new ProjectRepository();

      var data = await pr.SelectProjectRepInvoiceOneAsync(p_seq);
      var chargeList = await pr.SelectProjectInvoiceChargeOneAsync(p_seq);

      if (chargeList.Count == 1)
      {
        chargeList.FirstOrDefault().sales_rate = 100;
        chargeList.FirstOrDefault().sales_money = (double)data.ann_income * ((double)data.fee_rate * 0.01);
      }

      ProjectInvoiceCreateModel model = new ProjectInvoiceCreateModel()
      {
        p_seq = data.p_seq,
        pii_seq = data.pii_seq,
        candidate_name = data.candidate_name,
        billing_dt = data.billing_dt.HasValue ? data.billing_dt : Utils.NowKorea(),
        billing_money = data.billing_money,
        bill_currency_cd = data.bill_currency_cd,
        annual_income = data.annual_income,
        ann_income = data.ann_income,
        fee_rate = data.fee_rate,
        expire_guarantee = data.expire_guarantee,
        client_name = data.client_name,
        client_eng_name = data.client_eng_name,
        ceo = data.ceo,
        addr = data.addr,
        biz_code = data.biz_code,
        is_open_name = data.is_open_name,
        invoice_lang = data.invoice_lang,
        invoice_type = data.invoice_type,
        remarks = data.remarks,
        chargeList = chargeList
      };

      return PartialView(model);
    }

    public async Task<PartialViewResult> ProjectRefundInvoiceCreate(int p_seq)
    {
      ProjectRepository pr = new ProjectRepository();

      var data = await pr.SelectProjectRepInvoiceOneAsync(p_seq);
      var chargeList = await pr.SelectProjectInvoiceChargeOneAsync(p_seq);

      if (chargeList.Count == 1)
      {
        chargeList.FirstOrDefault().sales_rate = 100;
        chargeList.FirstOrDefault().sales_money = (double)data.ann_income * ((double)data.fee_rate * 0.01);
      }

      ProjectInvoiceCreateModel model = new ProjectInvoiceCreateModel()
      {
        p_seq = data.p_seq,
        pii_seq = data.pii_seq,
        candidate_name = data.candidate_name,
        vat_type = data.vat_type,
        billing_dt = Utils.NowKorea(),
        billing_money = data.billing_money * -1,
        billing_won = data.billing_won * -1,
        bill_currency_cd = data.bill_currency_cd,
        ann_income = data.ann_income,
        fee_rate = data.fee_rate,
        expire_guarantee = data.expire_guarantee,
        client_name = data.client_name,
        client_eng_name = data.client_eng_name,
        ceo = data.ceo,
        addr = data.addr,
        biz_code = data.biz_code,
        is_open_name = data.is_open_name,
        invoice_lang = data.invoice_lang,
        invoice_type = data.invoice_type,
        remarks = "",
        chargeList = chargeList
      };

      return PartialView(model);
    }

    public async Task<PartialViewResult> ProjectRetainerInvoiceCreate(int p_seq)
    {
      ProjectRepository pr = new ProjectRepository();

      var data = await pr.SelectProjectRepInvoiceOneAsync(p_seq);
      var chargeList = await pr.SelectProjectInvoiceChargeOneAsync(p_seq);

      if (chargeList.Count == 1)
      {
        chargeList.FirstOrDefault().sales_rate = 100;
        chargeList.FirstOrDefault().sales_money = 0;
      }

      ProjectInvoiceCreateModel model = new ProjectInvoiceCreateModel()
      {
        p_seq = data.p_seq,
        billing_dt = Utils.NowKorea(),
        billing_money = 0,
        bill_currency_cd = data.bill_currency_cd,
        fee_rate = 100,
        client_name = data.client_name,
        client_eng_name = data.client_eng_name,
        ceo = data.ceo,
        addr = data.addr,
        biz_code = data.biz_code,
        is_open_name = data.is_open_name,
        invoice_lang = data.invoice_lang,
        invoice_type = data.invoice_type,
        remarks = "",
        chargeList = chargeList
      };

      //return PartialView("ProjectRetInvoiceCreate_Right", model);
      return PartialView(model);
    }

    /// <summary>
    /// 프로젝트 - 평판조회 인보이스 생성.
    /// </summary>
    /// <param name="data"></param>
    /// <returns></returns>
    [HttpPost]
    public async Task<JsonResult> ProjectRefundInvoiceCreate(ProjectInvoiceCreateModel data, int? is_success)
    {
      try
      {
        bool is_success_ok = false;
        string share_str = String.Empty;

        ProjectEntityRepository per = new ProjectEntityRepository();

        ClientRepository cr = new ClientRepository();
        ProjectRepository pr = new ProjectRepository();

        var project = await pr.SelectProjectOneAsync(data.p_seq);

        //프로젝트에 고객사 세금계산서 담당자 정보가 없을 경우.
        if (!project.ctc_seq.HasValue)
          return Json(new
          {
            ok = false,
            message = "해당 프로젝트에 고객사 세금계산서 담당자 정보가 없습니다."
          });

        /*
        pjt_invoice_info info = new pjt_invoice_info()
        {
            p_seq = data.p_seq,
            billing_dt = data.billing_dt,
            billing_money = data.billing_money,
            billing_type = data.billing_type,
            bill_currency_cd = data.bill_currency_cd,
            fee_rate = data.fee_rate,
            expire_guarantee = data.expire_guarantee,
            invoice_lang = data.invoice_lang,
            invoice_type = data.invoice_type,
            remarks = data.remarks,
            create_dt = Utils.NowKorea(),
            create_user = AppIdentity.user_seq,
            modify_dt = Utils.NowKorea(),
            modify_user = AppIdentity.user_seq,
            is_deleted = 0
        };
        */
        pjt_invoice_info info = new pjt_invoice_info()
        {
          p_seq = data.p_seq,
          billing_dt = data.billing_dt,
          fee_rate = 0,
          billing_money = data.billing_money,
          billing_total = data.billing_total,
          billing_amt = data.billing_amt,
          billing_vat = data.billing_vat,
          billing_won = data.billing_won,
          bill_currency_cd = data.bill_currency_cd,

          vat_type = data.vat_type,
          is_po_no = data.is_po_no,
          billing_type = data.billing_type,
          expire_guarantee = data.expire_guarantee,
          is_open_name = data.is_open_name,
          is_open_annual_income = data.is_open_annual_income,
          invoice_lang = data.invoice_lang,
          invoice_type = data.invoice_type,
          remarks = data.remarks,
          create_dt = Utils.NowKorea(),
          create_user = AppIdentity.user_seq,
          modify_dt = Utils.NowKorea(),
          modify_user = AppIdentity.user_seq,
          candidate_name = data.candidate_name,
          client_name = data.client_name,
          client_ceo = data.client_ceo,
          client_addr1 = data.client_addr1,
          client_biz_code = data.client_biz_code,
          client_contact_name = data.client_contact_name,
          client_contact_email = data.client_contact_email,
          client_contact_phone = data.client_contact_tel,
          etax_name = data.client_etax_name,
          etax_email = data.client_etax_email,
          etax_phone = data.client_etax_tel,

          is_deleted = 0
        };

        List<pjt_invoice_sales> sales = new List<pjt_invoice_sales>();
        foreach (var charge in data.chargeList)
        {
          pjt_invoice_sales sale = new pjt_invoice_sales()
          {
            p_seq = data.p_seq,
            uv_seq = charge.uv_seq,
            ud_seq = charge.ud_seq,
            sales_rate = charge.sales_rate,
            sales_money = charge.sales_money,
            comments = charge.comments,
            create_dt = Utils.NowKorea(),
            create_user = AppIdentity.user_seq,
            modify_dt = Utils.NowKorea(),
            modify_user = AppIdentity.user_seq,
          };

          share_str += (!String.IsNullOrEmpty(share_str) ? "<br/><br/>" : "") + " - " + charge.name + " ( " + charge.sales_rate.ToString() + "%) : " + charge.comments;
          sales.Add(sale);
        }

        await per.CreatePjtInvoice(info, sales);

        ClientEntityRepository clientr = new ClientEntityRepository();
        client client = new client();
        //==============================================
        //고객사 영문명 저장 해야 할 경우,
        if (data.isEngCompanySave)
        {
          //고객사 정보가 바인딩 되지 않았다면 조회.
          if (client.c_seq == 0)
            client = await clientr.SelectClientOneAsync(project.c_seq);

          client.eng_name = info.client_name;

          await clientr.UpdateClientOneAsync(client);
        }

        var project_stat = await per.SelectProjectOneAsync(data.p_seq);
        if (is_success == 1)
        {
          if (project_stat != null && project_stat.pjt_status != 5)
          {
            project_stat.pjt_status = 5;
            project_stat.status_comment = "인보이스 발행으로 자동 성공 처리";
            project_stat.modify_dt = Utils.NowKorea();
            project_stat.modify_user = AppIdentity.user_seq;
            await per.UpdateProjectAsync(project_stat);
            is_success_ok = true;
          }

        }



        AccountRepository ar = new AccountRepository();
        ApiEntityRepository apier = new ApiEntityRepository();
        List<alarm_user> me = new List<alarm_user>();
        List<string> ToArr = new List<string>();
        List<alarm_user> aList = new List<alarm_user>();


        var myuserinfo = await ar.FindUserBySeqAsync(AppIdentity.user_seq);
        MailService mService = new MailService(myuserinfo.email_id, myuserinfo.email_pwd);

        alarm_message aMessage = new alarm_message()
        {
          href_url = "/Project/ProjectDetail?p_seq=" + data.p_seq,
          message = string.Format("[{0}] 인보이스 요청 완료 되었습니다.", project_stat.title),
          create_dt = Utils.NowKorea()
        };

        var userList = await pr.SelectProjectAmListAsync(data.p_seq);

        foreach (var user in userList)
        {
          //이미 리스트에 포함되어 있는 uv_seq라면 넘김.
          if (aList.Where(x => (x.uv_seq == user.uv_seq)).FirstOrDefault() != null)
            continue;

          alarm_user alarm = new alarm_user()
          {
            uv_seq = user.uv_seq
              ,
            is_read = 0
          };

          aList.Add(alarm);

          //담당자 이메일 추가. (최대 3건이라 건별로 조회)
          var confirmUser = await ar.FindUserBySeqAsync(user.uv_seq);
          ToArr.Add(confirmUser.email);
        }

        var searcherList = await pr.SelectProjectSearcherListAsync(data.p_seq);

        foreach (var user in searcherList)
        {
          //이미 리스트에 포함되어 있는 uv_seq라면 넘김.
          if (aList.Where(x => (x.uv_seq == user.uv_seq)).FirstOrDefault() != null)
            continue;

          alarm_user alarm = new alarm_user()
          {
            uv_seq = user.uv_seq
              ,
            is_read = 0
          };

          aList.Add(alarm);

          //담당자 이메일 추가. (최대 3건이라 건별로 조회)
          var confirmUser = await ar.FindUserBySeqAsync(user.uv_seq);
          ToArr.Add(confirmUser.email);
        }

        ToArr.Add("unico@unicosearch.com");
        ToArr.Add("planning@unicosearch.com");
        //ToArr.Add("narae@unicosearch.com");


        await apier.CreateAlarm(aMessage, aList);

        //메일 Dto
        InvoiceCreateDto mailData = new InvoiceCreateDto()
        {
          ToArr = ToArr.ToArray()
            ,
          From = new System.Net.Mail.MailAddress(AppIdentity.email, AppIdentity.name)
            ,
          name = AppIdentity.name
            ,
          title = string.Format("[{0}] 환불 인보이스 요청 건", project_stat.title)
            ,
          comment = data.remarks.Replace("\r\n", "\n").Replace("\n", "<br />")
            ,
          clientname = data.client_name
            ,
          ceo = data.client_ceo
            ,
          address = data.client_addr1
            ,
          candidatename = data.candidate_name
            ,
          vattype = "[" + Utils.ReturnVatTypeTxt(data.vat_type) + "]"
            ,
          income = Utils.ConvertMoneyToString(data.annual_income)
            ,
          feerate = data.fee_rate.ToString()
            ,
          fee = Utils.ConvertMoneyToString(data.billing_total)
            ,
          amt = Utils.ConvertMoneyToString(data.billing_amt)
            ,
          vat = Utils.ConvertMoneyToString(data.billing_vat)
            ,
          contactname = data.client_contact_name
            ,
          contactemail = data.client_contact_email
            ,
          contactphone = data.client_contact_tel
            ,
          etaxname = data.client_etax_name
            ,
          etaxmail = data.client_etax_email
            ,
          etaxphone = data.client_etax_tel
            ,
          feeshare = share_str
            ,
          url = "http://univision.unicosearch.com/Project/ProjectDetail?p_seq=" + data.p_seq
        };
        var result_my = mService.SendInvoiceCreateMail(mailData, new InvoiceCreateTemplete());

        /*
         * 담당자에게 별도로 전송
         */
        List<string> SupportToArr = new List<string>();

        SupportToArr.Add("narae@unicosearch.com");
        SupportToArr.Add("claire@unicosearch.com");

        mailData.ToArr = SupportToArr.ToArray();

        var result_support = mService.SendInvoiceCreateMail(mailData, new InvoiceCreateTemplete());


        return Json(new
        {
          ok = true,
          is_success_ok = is_success_ok
        });
      }
      catch (Exception e)
      {
        return Json(new
        {
          ok = false,
          message = "인보이스 발행 중, 오류가 발생 했습니다. - " + e.Message
        });
      }

    }

    [HttpPost]
    public async Task<JsonResult> ProjectRetainerInvoiceCreate(ProjectInvoiceCreateModel data, int? is_success)
    {
      try
      {
        bool is_success_ok = false;
        string share_str = String.Empty;

        ProjectEntityRepository per = new ProjectEntityRepository();

        ClientRepository cr = new ClientRepository();
        ProjectRepository pr = new ProjectRepository();

        var project = await pr.SelectProjectOneAsync(data.p_seq);

        //프로젝트에 고객사 세금계산서 담당자 정보가 없을 경우.
        if (!project.ctc_seq.HasValue)
          return Json(new
          {
            ok = false,
            message = "해당 프로젝트에 고객사 세금계산서 담당자 정보가 없습니다."
          });

        /*
        pjt_invoice_info info = new pjt_invoice_info()
        {
            p_seq = data.p_seq,
            billing_dt = data.billing_dt,
            billing_money = data.billing_money,
            billing_type = data.billing_type,
            bill_currency_cd = data.bill_currency_cd,
            fee_rate = data.fee_rate,
            expire_guarantee = data.expire_guarantee,
            invoice_lang = data.invoice_lang,
            invoice_type = data.invoice_type,
            remarks = data.remarks,
            create_dt = Utils.NowKorea(),
            create_user = AppIdentity.user_seq,
            modify_dt = Utils.NowKorea(),
            modify_user = AppIdentity.user_seq,
            is_deleted = 0
        };
        */
        pjt_invoice_info info = new pjt_invoice_info()
        {
          p_seq = data.p_seq,
          billing_dt = data.billing_dt,
          fee_rate = 0,
          billing_money = data.billing_money,
          billing_total = data.billing_total,
          billing_amt = data.billing_amt,
          billing_vat = data.billing_vat,
          billing_won = data.billing_won,
          bill_currency_cd = data.bill_currency_cd,

          vat_type = data.vat_type,
          is_po_no = data.is_po_no,
          billing_type = data.billing_type,
          expire_guarantee = data.expire_guarantee,
          is_open_name = data.is_open_name,
          is_open_annual_income = data.is_open_annual_income,
          invoice_lang = data.invoice_lang,
          invoice_type = data.invoice_type,
          remarks = data.remarks,
          create_dt = Utils.NowKorea(),
          create_user = AppIdentity.user_seq,
          modify_dt = Utils.NowKorea(),
          modify_user = AppIdentity.user_seq,
          candidate_name = data.candidate_name,
          client_name = data.client_name,
          client_ceo = data.client_ceo,
          client_addr1 = data.client_addr1,
          client_biz_code = data.client_biz_code,
          client_contact_name = data.client_contact_name,
          client_contact_email = data.client_contact_email,
          client_contact_phone = data.client_contact_tel,
          etax_name = data.client_etax_name,
          etax_email = data.client_etax_email,
          etax_phone = data.client_etax_tel,

          is_deleted = 0
        };

        List<pjt_invoice_sales> sales = new List<pjt_invoice_sales>();
        foreach (var charge in data.chargeList)
        {
          pjt_invoice_sales sale = new pjt_invoice_sales()
          {
            p_seq = data.p_seq,
            uv_seq = charge.uv_seq,
            ud_seq = charge.ud_seq,
            sales_rate = charge.sales_rate,
            sales_money = charge.sales_money,
            comments = charge.comments,
            create_dt = Utils.NowKorea(),
            create_user = AppIdentity.user_seq,
            modify_dt = Utils.NowKorea(),
            modify_user = AppIdentity.user_seq,
          };

          share_str += (!String.IsNullOrEmpty(share_str) ? "<br/><br/>" : "") + " - " + charge.name + " ( " + charge.sales_rate.ToString() + "% / " + Utils.ConvertMoneyToString(charge.sales_money) + ") : " + charge.comments;
          sales.Add(sale);
        }

        await per.CreatePjtInvoice(info, sales);

        ClientEntityRepository clientr = new ClientEntityRepository();
        client client = new client();
        //==============================================
        //고객사 영문명 저장 해야 할 경우,
        if (data.isEngCompanySave)
        {
          //고객사 정보가 바인딩 되지 않았다면 조회.
          if (client.c_seq == 0)
            client = await clientr.SelectClientOneAsync(project.c_seq);

          client.eng_name = info.client_name;

          await clientr.UpdateClientOneAsync(client);
        }

        var project_stat = await per.SelectProjectOneAsync(data.p_seq);
        if (is_success == 1)
        {
          if (project_stat != null && project_stat.pjt_status != 5)
          {
            project_stat.pjt_status = 5;
            project_stat.status_comment = "인보이스 발행으로 자동 성공 처리";
            project_stat.modify_dt = Utils.NowKorea();
            project_stat.modify_user = AppIdentity.user_seq;
            await per.UpdateProjectAsync(project_stat);
            is_success_ok = true;
          }

        }



        AccountRepository ar = new AccountRepository();
        ApiEntityRepository apier = new ApiEntityRepository();
        List<alarm_user> me = new List<alarm_user>();
        List<string> ToArr = new List<string>();
        List<alarm_user> aList = new List<alarm_user>();


        var myuserinfo = await ar.FindUserBySeqAsync(AppIdentity.user_seq);
        MailService mService = new MailService(myuserinfo.email_id, myuserinfo.email_pwd);

        alarm_message aMessage = new alarm_message()
        {
          href_url = "/Project/ProjectDetail?p_seq=" + data.p_seq,
          message = string.Format("[{0}] 인보이스 요청 완료 되었습니다.", project_stat.title),
          create_dt = Utils.NowKorea()
        };

        var userList = await pr.SelectProjectAmListAsync(data.p_seq);

        foreach (var user in userList)
        {
          //이미 리스트에 포함되어 있는 uv_seq라면 넘김.
          if (aList.Where(x => (x.uv_seq == user.uv_seq)).FirstOrDefault() != null)
            continue;

          alarm_user alarm = new alarm_user()
          {
            uv_seq = user.uv_seq
              ,
            is_read = 0
          };

          aList.Add(alarm);

          //담당자 이메일 추가. (최대 3건이라 건별로 조회)
          var confirmUser = await ar.FindUserBySeqAsync(user.uv_seq);
          ToArr.Add(confirmUser.email);
        }

        var searcherList = await pr.SelectProjectSearcherListAsync(data.p_seq);

        foreach (var user in searcherList)
        {
          //이미 리스트에 포함되어 있는 uv_seq라면 넘김.
          if (aList.Where(x => (x.uv_seq == user.uv_seq)).FirstOrDefault() != null)
            continue;

          alarm_user alarm = new alarm_user()
          {
            uv_seq = user.uv_seq
              ,
            is_read = 0
          };

          aList.Add(alarm);

          //담당자 이메일 추가. (최대 3건이라 건별로 조회)
          var confirmUser = await ar.FindUserBySeqAsync(user.uv_seq);
          ToArr.Add(confirmUser.email);
        }

        ToArr.Add("unico@unicosearch.com");
        ToArr.Add("planning@unicosearch.com");
        //ToArr.Add("narae@unicosearch.com");


        await apier.CreateAlarm(aMessage, aList);

        //메일 Dto
        InvoiceCreateDto mailData = new InvoiceCreateDto()
        {
          ToArr = ToArr.ToArray()
            ,
          From = new System.Net.Mail.MailAddress(AppIdentity.email, AppIdentity.name)
            ,
          name = AppIdentity.name
            ,
          title = string.Format("[{0}] 선수금 인보이스 요청 건", project_stat.title)
            ,
          comment = data.remarks.Replace("\r\n", "\n").Replace("\n", "<br />")
            ,
          clientname = data.client_name
            ,
          ceo = data.client_ceo
            ,
          address = data.client_addr1
            ,
          candidatename = data.candidate_name
            ,
          vattype = "[" + Utils.ReturnVatTypeTxt(data.vat_type) + "]"
            ,
          income = Utils.ConvertMoneyToString(data.annual_income)
            ,
          feerate = data.fee_rate.ToString()
            ,
          fee = Utils.ConvertMoneyToString(data.billing_total)
            ,
          amt = Utils.ConvertMoneyToString(data.billing_amt)
            ,
          vat = Utils.ConvertMoneyToString(data.billing_vat)
            ,
          contactname = data.client_contact_name
            ,
          contactemail = data.client_contact_email
            ,
          contactphone = data.client_contact_tel
            ,
          etaxname = data.client_etax_name
            ,
          etaxmail = data.client_etax_email
            ,
          etaxphone = data.client_etax_tel
            ,
          feeshare = share_str
            ,
          url = "http://univision.unicosearch.com/Project/ProjectDetail?p_seq=" + data.p_seq
        };
        var result_my = mService.SendInvoiceCreateMail(mailData, new InvoiceCreateTemplete());

        /*
         * 담당자에게 별도로 전송
         */
        List<string> SupportToArr = new List<string>();

        SupportToArr.Add("narae@unicosearch.com");
        SupportToArr.Add("claire@unicosearch.com");

        mailData.ToArr = SupportToArr.ToArray();

        var result_support = mService.SendInvoiceCreateMail(mailData, new InvoiceCreateTemplete());


        return Json(new
        {
          ok = true,
          is_success_ok = is_success_ok
        });
      }
      catch (Exception e)
      {
        return Json(new
        {
          ok = false,
          message = "인보이스 발행 중, 오류가 발생 했습니다. - " + e.Message
        });
      }

    }

    /// <summary>
    /// 프로젝트 - 평판조회 인보이스 생성.
    /// </summary>
    /// <param name="data"></param>
    /// <returns></returns>
    [HttpPost]
    public async Task<JsonResult> ProjectRepInvoiceCreate(ProjectInvoiceCreateModel data, int? is_success)
    {
      try
      {
        bool is_success_ok = false;
        string share_str = String.Empty;

        ProjectEntityRepository per = new ProjectEntityRepository();

        ClientRepository cr = new ClientRepository();
        ProjectRepository pr = new ProjectRepository();

        var project = await pr.SelectProjectRepOneAsync(data.p_seq);

        //프로젝트에 고객사 세금계산서 담당자 정보가 없을 경우.
        if (!project.ctc_seq.HasValue)
          return Json(new
          {
            ok = false,
            message = "해당 프로젝트에 고객사 세금계산서 담당자 정보가 없습니다."
          });

        /*
        pjt_invoice_info info = new pjt_invoice_info()
        {
            p_seq = data.p_seq,
            billing_dt = data.billing_dt,
            billing_money = data.billing_money,
            billing_type = data.billing_type,
            bill_currency_cd = data.bill_currency_cd,
            fee_rate = data.fee_rate,
            expire_guarantee = data.expire_guarantee,
            invoice_lang = data.invoice_lang,
            invoice_type = data.invoice_type,
            remarks = data.remarks,
            create_dt = Utils.NowKorea(),
            create_user = AppIdentity.user_seq,
            modify_dt = Utils.NowKorea(),
            modify_user = AppIdentity.user_seq,
            is_deleted = 0
        };
        */
        pjt_invoice_info info = new pjt_invoice_info()
        {
          p_seq = data.p_seq,
          billing_dt = data.billing_dt,
          fee_rate = 0,
          billing_money = data.billing_money,
          billing_total = data.billing_total,
          billing_amt = data.billing_amt,
          billing_vat = data.billing_vat,
          billing_won = data.billing_won,
          bill_currency_cd = data.bill_currency_cd,

          vat_type = data.vat_type,
          is_po_no = data.is_po_no,
          billing_type = data.billing_type,
          expire_guarantee = data.expire_guarantee,
          is_open_name = data.is_open_name,
          is_open_annual_income = data.is_open_annual_income,
          invoice_lang = data.invoice_lang,
          invoice_type = data.invoice_type,
          remarks = data.remarks,
          create_dt = Utils.NowKorea(),
          create_user = AppIdentity.user_seq,
          modify_dt = Utils.NowKorea(),
          modify_user = AppIdentity.user_seq,
          candidate_name = data.candidate_name,
          client_name = data.client_name,
          client_ceo = data.client_ceo,
          client_addr1 = data.client_addr1,
          client_biz_code = data.client_biz_code,
          client_contact_name = data.client_contact_name,
          client_contact_email = data.client_contact_email,
          client_contact_phone = data.client_contact_tel,
          etax_name = data.client_etax_name,
          etax_email = data.client_etax_email,
          etax_phone = data.client_etax_tel,

          is_deleted = 0
        };

        List<pjt_invoice_sales> sales = new List<pjt_invoice_sales>();
        foreach (var charge in data.chargeList)
        {
          pjt_invoice_sales sale = new pjt_invoice_sales()
          {
            p_seq = data.p_seq,
            uv_seq = charge.uv_seq,
            ud_seq = charge.ud_seq,
            sales_rate = charge.sales_rate,
            sales_money = charge.sales_money,
            comments = charge.comments,
            create_dt = Utils.NowKorea(),
            create_user = AppIdentity.user_seq,
            modify_dt = Utils.NowKorea(),
            modify_user = AppIdentity.user_seq,
          };

          share_str += (!String.IsNullOrEmpty(share_str) ? "<br/><br/>" : "") + " - " + charge.name + " ( " + charge.sales_rate.ToString("C0") + "% / " + Utils.ConvertMoneyToString(charge.sales_money) + ") : " + charge.comments;
          sales.Add(sale);
        }

        await per.CreatePjtInvoice(info, sales);

        ClientEntityRepository clientr = new ClientEntityRepository();
        client client = new client();
        //==============================================
        //고객사 영문명 저장 해야 할 경우,
        if (data.isEngCompanySave)
        {
          //고객사 정보가 바인딩 되지 않았다면 조회.
          if (client.c_seq == 0)
            client = await clientr.SelectClientOneAsync(project.c_seq);

          client.eng_name = info.client_name;

          await clientr.UpdateClientOneAsync(client);
        }

        var project_stat = await per.SelectProjectOneAsync(data.p_seq);
        if (is_success == 1)
        {
          if (project_stat != null && project_stat.pjt_status != 5)
          {
            project_stat.pjt_status = 5;
            project_stat.status_comment = "인보이스 발행으로 자동 성공 처리";
            project_stat.close_dt = Utils.NowKorea();
            project_stat.modify_dt = Utils.NowKorea();
            project_stat.modify_user = AppIdentity.user_seq;
            await per.UpdateProjectAsync(project_stat);
            is_success_ok = true;
          }

        }



        AccountRepository ar = new AccountRepository();
        ApiEntityRepository apier = new ApiEntityRepository();
        List<alarm_user> me = new List<alarm_user>();
        List<string> ToArr = new List<string>();
        List<alarm_user> aList = new List<alarm_user>();


        var myuserinfo = await ar.FindUserBySeqAsync(AppIdentity.user_seq);
        MailService mService = new MailService(myuserinfo.email_id, myuserinfo.email_pwd);

        alarm_message aMessage = new alarm_message()
        {
          href_url = "/Project/ProjectRepDetail?p_seq=" + data.p_seq,
          message = string.Format("[{0}] 인보이스 요청 완료 되었습니다.", project_stat.title),
          create_dt = Utils.NowKorea()
        };

        var userList = await pr.SelectProjectAmListAsync(data.p_seq);

        foreach (var user in userList)
        {
          //이미 리스트에 포함되어 있는 uv_seq라면 넘김.
          if (aList.Where(x => (x.uv_seq == user.uv_seq)).FirstOrDefault() != null)
            continue;

          alarm_user alarm = new alarm_user()
          {
            uv_seq = user.uv_seq
              ,
            is_read = 0
          };

          aList.Add(alarm);

          //담당자 이메일 추가. (최대 3건이라 건별로 조회)
          var confirmUser = await ar.FindUserBySeqAsync(user.uv_seq);
          ToArr.Add(confirmUser.email);
        }

        var searcherList = await pr.SelectProjectSearcherListAsync(data.p_seq);

        foreach (var user in searcherList)
        {
          //이미 리스트에 포함되어 있는 uv_seq라면 넘김.
          if (aList.Where(x => (x.uv_seq == user.uv_seq)).FirstOrDefault() != null)
            continue;

          alarm_user alarm = new alarm_user()
          {
            uv_seq = user.uv_seq
              ,
            is_read = 0
          };

          aList.Add(alarm);

          //담당자 이메일 추가. (최대 3건이라 건별로 조회)
          var confirmUser = await ar.FindUserBySeqAsync(user.uv_seq);
          ToArr.Add(confirmUser.email);
        }

        ToArr.Add("unico@unicosearch.com");
        ToArr.Add("planning@unicosearch.com");
        //ToArr.Add("narae@unicosearch.com");


        await apier.CreateAlarm(aMessage, aList);

        //메일 Dto
        InvoiceCreateDto mailData = new InvoiceCreateDto()
        {
          ToArr = ToArr.ToArray()
            ,
          From = new System.Net.Mail.MailAddress(AppIdentity.email, AppIdentity.name)
            ,
          name = AppIdentity.name
            ,
          title = string.Format("[{0}] 인보이스 요청 건", project_stat.title)
            ,
          comment = data.remarks.Replace("\r\n", "\n").Replace("\n", "<br />")
            ,
          clientname = data.client_name
            ,
          ceo = data.client_ceo
            ,
          address = data.client_addr1
            ,
          candidatename = data.candidate_name
            ,
          vattype = "[" + Utils.ReturnVatTypeTxt(data.vat_type) + "]"
            ,
          income = Utils.ConvertMoneyToString(data.annual_income)
            ,
          feerate = data.fee_rate.ToString()
            ,
          fee = Utils.ConvertMoneyToString(data.billing_total)
            ,
          amt = Utils.ConvertMoneyToString(data.billing_amt)
            ,
          vat = Utils.ConvertMoneyToString(data.billing_vat)
            ,
          contactname = data.client_contact_name
            ,
          contactemail = data.client_contact_email
            ,
          contactphone = data.client_contact_tel
            ,
          etaxname = data.client_etax_name
            ,
          etaxmail = data.client_etax_email
            ,
          etaxphone = data.client_etax_tel
            ,
          feeshare = share_str
            ,
          url = "http://univision.unicosearch.com/Project/ProjectRepDetail?p_seq=" + data.p_seq
        };
        var result_my = mService.SendInvoiceCreateMail(mailData, new InvoiceCreateTemplete());

        /*
         * 담당자에게 별도로 전송
         */
        List<string> SupportToArr = new List<string>();

        SupportToArr.Add("narae@unicosearch.com");
        SupportToArr.Add("claire@unicosearch.com");

        mailData.ToArr = SupportToArr.ToArray();

        var result_support = mService.SendInvoiceCreateMail(mailData, new InvoiceCreateTemplete());

        return Json(new
        {
          ok = true,
          is_success_ok = is_success_ok
        });
      }
      catch (Exception e)
      {
        return Json(new
        {
          ok = false,
          message = "인보이스 발행 중, 오류가 발생 했습니다. - " + e.Message
        });
      }

    }

    /// <summary>
    /// 프로젝트 -평판조회 인보이스 상세
    /// </summary>
    /// <param name="p_seq"></param>
    /// <param name="c_seq"></param>
    /// <param name="pii_seq"></param>
    /// <returns></returns>
    public async Task<PartialViewResult> ProjectRepInvoiceView(int p_seq, int pii_seq)
    {
      ProjectRepository pr = new ProjectRepository();

      var data = await pr.SelectProjectRepInvoiceOneAsync(p_seq);
      var chargeList = await pr.SelectProjectInvoiceSalesListAsync(pii_seq);

      ProjectInvoiceCreateModel model = new ProjectInvoiceCreateModel()
      {
        p_seq = data.p_seq,
        pii_seq = data.pii_seq,
        billing_dt = data.billing_dt,
        billing_money = data.billing_money,
        billing_type = data.billing_type,
        billing_won = data.billing_won,
        bill_currency_cd = data.bill_currency_cd,
        annual_income = data.annual_income,
        fee_rate = data.fee_rate,
        expire_guarantee = data.expire_guarantee,
        client_name = data.client_name,
        ceo = data.ceo,
        addr = data.addr,
        biz_code = data.biz_code,
        invoice_lang = data.invoice_lang,
        invoice_type = data.invoice_type,
        remarks = data.remarks,
        chargeList = chargeList
      };

      return PartialView(model);
    }

    /// <summary>
    /// 공유프로젝트 상세
    /// </summary>
    /// <param name="p_seq"></param>
    /// <returns></returns>
    public async Task<ActionResult> ShareProjectDetail(int p_seq = 0)
    {
      ProjectRepository pr = new ProjectRepository();

      var data = await pr.SelectProjectOneAsync(p_seq);
      if (data == null)
        data = new project();

      ProjectCreateModel model = new ProjectCreateModel()
      {
        p_seq = data.p_seq,
        c_seq = data.c_seq,
        pjt_status = data.pjt_status,
        status_comment = data.status_comment,
        company_name = data.company_name,
        pjt_type = data.pjt_type,
        recruit_number = data.recruit_number,
        is_posting = data.is_posting,
        title = data.title,
        position_seq = data.position_seq,
        experience_type = data.experience_type,
        expreience_number = data.expreience_number,
        edu_code = data.edu_code,
        edu_name = data.edu_name,
        foreign_lang = data.foreign_lang,
        foreign_lang_name = data.foreign_lang_name,
        foreign_level = data.foreign_level,
        assign_task = data.assign_task,
        requirement = data.requirement,
        is_kor_resume = data.is_kor_resume,
        is_eng_resume = data.is_eng_resume,
        is_portfolio = data.is_portfolio,
        is_self_introduction = data.is_self_introduction,
        is_etc = data.is_etc,
        etc_comment = data.etc_comment,
        is_pre_interview = data.is_pre_interview,
        is_number = data.is_number,
        interview_number = data.interview_number,
        is_personality_test = data.is_personality_test,
        gender_type = data.gender_type,
        start_age = data.start_age,
        end_age = data.end_age,
        target_school_nm = data.target_school_nm,
        //target_school_name = data.target_school_name,
        //target_school_campus = data.target_school_campus,
        target_major_nm = data.target_major_nm,
        //target_major_name = data.target_major_name,
        target_company_nm = data.target_company_nm,
        //target_company_name = data.target_company_name,
        confidentiallity = data.confidentiallity,
        expected_salary = data.expected_salary,
        currency_cd = data.currency_cd,
        fee_rate = data.fee_rate,
        tax_division = data.tax_division,
        tax_name = data.tax_name,
        tax_email = data.tax_email,
        tax_phone = data.tax_phone,
        tax_cell_phone = data.tax_cell_phone,
        tax_deposit_manager = data.tax_deposit_manager,
        tax_deposit_email = data.tax_deposit_email,
        contact_name = data.contact_name,
        contact_gender = data.contact_gender,
        contact_email = data.contact_email,
        contact_phone = data.contact_phone,
        contact_cell_phone = data.contact_cell_phone,
        contact_division = data.contact_division,
        contact_position = data.contact_position,
        share_comments = data.share_comments,
        secret_info = data.secret_info,
        share_fee_rate = data.share_fee_rate,
        create_user = data.create_user,
        business_code1 = data.business_code1,
        business_code2 = data.business_code2,
        business_name2 = data.business_name2,
        job_code1 = data.job_code1,
        job_code2 = data.job_code2,
        job_name2 = data.job_name2,
        is_share_pjt = data.is_share_pjt,
        is_cowork = data.is_cowork,
        open_dt = data.open_dt,
        close_dt = data.close_dt
      };

      model.amList = await pr.SelectProjectAmListAsync(p_seq);
      model.searcherList = await pr.SelectProjectSearcherListAsync(p_seq);
      //model.busiList = await pr.SelectProjectBusiListAsync(p_seq);
      //model.jobList = await pr.SelectProjectJobListAsync(p_seq);
      model.placeList = await pr.SelectProjectPlaceListAsync(p_seq);
      model.keywordList = await pr.SelectProjectKeywordListAsync(p_seq);


      ProjectEntityRepository per = new ProjectEntityRepository();
      var read_data = await per.SelectProjectReadOneAsync(p_seq, AppIdentity.user_seq);
      string state = String.Empty;
      if (read_data == null)
      {
        read_data = new project_read_history()
        {
          p_seq = p_seq,
          read_user = AppIdentity.user_seq,
          read_dt = Utils.NowKorea()
        };
        state = CommonCodes.Create;
      }
      else
      {
        read_data.read_dt = Utils.NowKorea();
        state = CommonCodes.Update;
      }
      await per.CreateOrUpdateProjectRead(read_data, state);

      return View(model);
    }

    /// <summary>
    /// 공유 프로젝트 코멘트 리스트
    /// </summary>
    /// <param name="p_seq"></param>
    /// <param name="page"></param>
    /// <returns></returns>
    public async Task<PartialViewResult> ShareProjectBoardList(int p_seq, int page = 1)
    {
      ProjectRepository pr = new ProjectRepository();

      int totalCount = 0;

      //한 페이지당 몇개의 행을 보여줄 것인지
      int pageSize = AppPaging.PageLength5;

      var list = pr.SelectShareProjectBoardList(p_seq, (page - 1) * pageSize, pageSize, out totalCount);
      var amList = await pr.SelectProjectAmListAsync(p_seq);

      ShareProjectBoardListModel model = new ShareProjectBoardListModel()
      {
        p_seq = p_seq
          ,
        list = list
          ,
        amList = amList
          ,
        PagingInfo = new PagingInfo()
        {
          CurrentPage = page
              ,
          ItemsPerPage = pageSize
              ,
          TotalItems = totalCount
        }
      };

      return PartialView(model);
    }

    /// <summary>
    /// 공유 프로젝트 코멘트 등록 및 수정.
    /// </summary>
    /// <param name="p_seq"></param>
    /// <param name="psb_seq"></param>
    /// <param name="contents"></param>
    /// <returns></returns>
    [HttpPost]
    public async Task<JsonResult> CreateProjectBoard(int p_seq, int? psb_seq, string contents)
    {
      try
      {

        ProjectEntityRepository per = new ProjectEntityRepository();
        var board = new pjt_share_board();

        //수정
        if (psb_seq.HasValue)
        {
          board = await per.SelectProjectBoardOneAsync(p_seq, (int)psb_seq);

          if (board == null)
            return Json(new
            {
              ok = false,
              message = "수정하려는 코멘트를 찾을 수 없습니다."
            });

          board.contents = contents;
          board.modify_dt = Utils.NowKorea();
          board.modify_user = AppIdentity.user_seq;

          await per.CreateOrUpdatePjtBoard(board, CommonCodes.Update);
        }
        else
        {
          board = new pjt_share_board()
          {
            p_seq = p_seq
              ,
            contents = contents
              ,
            create_dt = Utils.NowKorea()
              ,
            create_user = AppIdentity.user_seq
              ,
            modify_dt = Utils.NowKorea()
              ,
            modify_user = AppIdentity.user_seq
          };

          await per.CreateOrUpdatePjtBoard(board, CommonCodes.Create);
        }

        return Json(new
        {
          ok = true,
          message = "코멘트를 등록 했습니다."
        });
      }
      catch (Exception e)
      {
        return Json(new
        {
          ok = false,
          message = "알 수 없는 오류가 발생 했습니다."
        });
      }
    }

    /// <summary>
    /// 공유 프로젝트 코멘트 삭제.
    /// </summary>
    /// <param name="p_seq"></param>
    /// <param name="psb_seq"></param>
    /// <returns></returns>
    [HttpPost]
    public async Task<JsonResult> DeleteProjectBoard(int p_seq, int psb_seq)
    {
      try
      {

        ProjectEntityRepository per = new ProjectEntityRepository();

        var board = await per.SelectProjectBoardOneAsync(p_seq, (int)psb_seq);
        if (board == null)
          return Json(new
          {
            ok = false,
            message = "삭제하려는 코멘트를 찾을 수 없습니다."
          });

        var reply = await per.SelectProjectBoardReplyListAsync(p_seq, (int)psb_seq);

        await per.DeletePjtBoard(board, reply, AppIdentity.user_seq);

        return Json(new
        {
          ok = true,
          message = "코멘트를 삭제 했습니다."
        });
      }
      catch (Exception e)
      {
        return Json(new
        {
          ok = false,
          message = "알 수 없는 오류가 발생 했습니다."
        });
      }
    }

    /// <summary>
    /// 공유 프로젝트 코멘트 댓글 등록.
    /// </summary>
    /// <param name="p_seq"></param>
    /// <param name="psb_seq"></param>
    /// <param name="psr_seq"></param>
    /// <param name="contents"></param>
    /// <returns></returns>
    [HttpPost]
    public async Task<JsonResult> CreateProjectBoardReply(int p_seq, int psb_seq, int? psr_seq, string contents)
    {
      try
      {

        ProjectEntityRepository per = new ProjectEntityRepository();
        var reply = new pjt_share_reply();

        //수정
        if (psr_seq.HasValue)
        {
          reply = await per.SelectProjectBoardReplyOneAsync(p_seq, psb_seq, (int)psr_seq);

          if (reply == null)
            return Json(new
            {
              ok = false,
              message = "수정하려는 코멘트 댓글을 찾을 수 없습니다."
            });

          reply.contents = contents;
          reply.modify_dt = Utils.NowKorea();
          reply.modify_user = AppIdentity.user_seq;

          await per.CreateOrUpdatePjtBoardReply(reply, CommonCodes.Update, AppIdentity.user_seq);
        }
        else
        {
          reply = new pjt_share_reply()
          {
            p_seq = p_seq
              ,
            psb_seq = psb_seq
              ,
            contents = contents
              ,
            create_dt = Utils.NowKorea()
              ,
            create_user = AppIdentity.user_seq
              ,
            modify_dt = Utils.NowKorea()
              ,
            modify_user = AppIdentity.user_seq
          };

          await per.CreateOrUpdatePjtBoardReply(reply, CommonCodes.Create);
        }

        return Json(new
        {
          ok = true,
          message = "코멘트 댓글을 등록 했습니다."
        });
      }
      catch (Exception e)
      {
        return Json(new
        {
          ok = false,
          message = "알 수 없는 오류가 발생 했습니다."
        });
      }
    }

    /// <summary>
    /// 공유 프로젝트 코멘트 댓글 삭제
    /// </summary>
    /// <param name="p_seq"></param>
    /// <param name="psb_seq"></param>
    /// <param name="psr_seq"></param>
    /// <returns></returns>
    [HttpPost]
    public async Task<JsonResult> DeleteProjectBoardReply(int p_seq, int psb_seq, int psr_seq)
    {
      try
      {

        ProjectEntityRepository per = new ProjectEntityRepository();

        var reply = await per.SelectProjectBoardReplyOneAsync(p_seq, psb_seq, (int)psr_seq);
        if (reply == null)
          return Json(new
          {
            ok = false,
            message = "삭제하려는 코멘트 댓글을 찾을 수 없습니다."
          });

        await per.DeletePjtBoardReply(reply, AppIdentity.user_seq);

        return Json(new
        {
          ok = true,
          message = "코멘트를 삭제 했습니다."
        });
      }
      catch (Exception e)
      {
        return Json(new
        {
          ok = false,
          message = "알 수 없는 오류가 발생 했습니다."
        });
      }
    }

    #endregion

    #region 프로젝트 관련 팝업

    public PartialViewResult SearchProjectClient(string searchTxt, int page = 1)
    {
      ProjectRepository pr = new ProjectRepository();

      int totalCount = 0;

      //한 페이지당 몇개의 행을 보여줄 것인지
      int pageSize = AppPaging.PageLength5;

      var list = pr.SelectClientList(searchTxt, (page - 1) * pageSize, pageSize, out totalCount);

      var model = new SearchProjectClientModel()
      {
        searchTxt = searchTxt
          ,
        list = list
          ,
        PagingInfo = new PagingInfo()
        {
          CurrentPage = page
              ,
          ItemsPerPage = pageSize
              ,
          TotalItems = totalCount
        }
      };

      return PartialView(model);
    }

    public PartialViewResult SearchClientPopup(string searchTxt, string c_seq_id, string name_id, string industry_id, string url_id, int page = 1) //Inorder
    {
      ProjectRepository pr = new ProjectRepository();

      int totalCount = 0;

      //한 페이지당 몇개의 행을 보여줄 것인지
      int pageSize = AppPaging.PageLength5;

      var list = pr.SelectClientList(searchTxt, (page - 1) * pageSize, pageSize, out totalCount);

      var model = new SearchProjectClientModel()
      {
        searchTxt = searchTxt
          ,
        list = list
          ,
        PagingInfo = new PagingInfo()
        {
          CurrentPage = page
              ,
          ItemsPerPage = pageSize
              ,
          TotalItems = totalCount
        }
      };

      ViewBag.name_id = name_id;
      ViewBag.c_seq_id = c_seq_id;
      ViewBag.industry_id = industry_id;
      ViewBag.url_id = url_id;

      return PartialView(model);
    }

    /// <summary>
    /// 프로젝트 AM 목록 조회
    /// </summary>
    /// <param name="main_contract"></param>
    /// <param name="ud_seq"></param>
    /// <param name="searchTxt"></param>
    /// <returns></returns>
    public async Task<PartialViewResult> SearchProjectAM(int ud_seq = 0, string searchTxt = "")
    {
      ProjectRepository pr = new ProjectRepository();
      AccountRepository ar = new AccountRepository();

      var list = await pr.SelectAMList(ud_seq, searchTxt);
      var deptList = await ar.SelectDivisionList();

      SearchProjectAMModel model = new SearchProjectAMModel()
      {
        ud_seq = ud_seq
          ,
        searchTxt = searchTxt
          ,
        list = list
          ,
        deptList = deptList
      };

      return PartialView(model);
    }

    /// <summary>
    /// 프로젝트 Searcher 목록 조회
    /// </summary>
    /// <param name="ud_seq"></param>
    /// <param name="searchTxt"></param>
    /// <returns></returns>
    public async Task<PartialViewResult> SearchProjectSearcher(int ud_seq = 0, string searchTxt = "")
    {
      ProjectRepository pr = new ProjectRepository();
      AccountRepository ar = new AccountRepository();

      var list = await pr.SelectSearcherList(ud_seq, searchTxt);
      var deptList = await ar.SelectDivisionList();

      SearchProjectSearcherModel model = new SearchProjectSearcherModel()
      {
        ud_seq = ud_seq
          ,
        searchTxt = searchTxt
          ,
        list = list
          ,
        deptList = deptList
      };

      return PartialView(model);
    }

    /// <summary>
    /// 프로젝트 클라이언트 담당자
    /// </summary>
    /// <param name="c_seq"></param>
    /// <returns></returns>
    public async Task<PartialViewResult> SearchProjectContact(int p_seq, int c_seq)
    {
      ClientRepository cr = new ClientRepository();

      var list = await cr.SelectClientContactListAsync(c_seq);

      ProjectContactListModel model = new ProjectContactListModel()
      {
        p_seq = p_seq,
        list = list
      };

      return PartialView(model);
    }



    /// <summary>
    /// 프로젝트 클라이언트 세금계산서 담당자
    /// </summary>
    /// <param name="c_seq"></param>
    /// <returns></returns>
    public async Task<PartialViewResult> SearchProjectTaxContact(int p_seq, int c_seq)
    {
      ClientRepository cr = new ClientRepository();

      var list = await cr.SelectClientTaxContactListAsync(c_seq);

      ProjectTaxContactListModel model = new ProjectTaxContactListModel()
      {
        p_seq = p_seq,
        list = list
      };

      return PartialView(model);
    }



    /// <summary>
    /// 프로젝트 인보이스 매출분배 담당자 추가.
    /// </summary>
    /// <param name="ud_seq"></param>
    /// <param name="searchTxt"></param>
    /// <returns></returns>

    public async Task<PartialViewResult> AddProjectInvoiceCharge(int ud_seq = 0, string searchTxt = "")
    {
      ProjectRepository pr = new ProjectRepository();
      AccountRepository ar = new AccountRepository();

      var list = await pr.SelectSearcherList(ud_seq, searchTxt);
      var deptList = await ar.SelectDivisionList();

      SearchProjectSearcherModel model = new SearchProjectSearcherModel()
      {
        ud_seq = ud_seq
          ,
        searchTxt = searchTxt
          ,
        list = list
          ,
        deptList = deptList
      };

      return PartialView(model);
    }

    /// <summary>
    /// 타겟 학교 - 2019-07-31 기준 사용 안함으로 변경.
    /// </summary>
    /// <param name="target_school"></param>
    /// <param name="school_name"></param>
    /// <param name="gubun"></param>
    /// <param name="page"></param>
    /// <returns></returns>
    public PartialViewResult SearchProjectTargetSchool(int target_school = 0, string gubun = "", string school_name = "", int page = 1)
    {
      ProjectRepository pr = new ProjectRepository();

      int totalCount = 0;

      //한 페이지당 몇개의 행을 보여줄 것인지
      int pageSize = AppPaging.PageLength10;

      var list = pr.SelectCodeSchoolList(school_name, gubun, (page - 1) * pageSize, pageSize, out totalCount);

      SearchProjectTargetSchoolModel model = new SearchProjectTargetSchoolModel()
      {
        target_school = target_school
          ,
        school_name = school_name
          ,
        gubun = gubun
          ,
        list = list
          ,
        PagingInfo = new PagingInfo()
        {
          CurrentPage = page
              ,
          ItemsPerPage = pageSize
              ,
          TotalItems = totalCount
        }
      };

      return PartialView(model);
    }


    #endregion

    #region 프로젝트 관련 AutoComplete

    /// <summary>
    /// 타겟학교 검색 AutoComplete
    /// </summary>
    /// <param name="gubun"></param>
    /// <param name="schoolName"></param>
    /// <returns></returns>
    [HttpPost]
    public async Task<JsonResult> FindCodeSchool(string school_name)
    {
      try
      {
        ProjectRepository pr = new ProjectRepository();

        var list = await pr.SelectCodeSchoolList(school_name);

        return Json(new
        {
          list = list
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

    /// <summary>
    /// 타겟학과 검색 AutoComplete
    /// </summary>
    /// <param name="major_name"></param>
    /// <returns></returns>
    [HttpPost]
    public async Task<JsonResult> FindCodeMajor(string major_name)
    {
      try
      {
        ProjectRepository pr = new ProjectRepository();

        var list = await pr.SelectCodeMajorList(major_name);

        return Json(new
        {
          list = list
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

    /// <summary>
    /// 타겟회사 검색 AutoComplete
    /// </summary>
    /// <param name="companyName"></param>
    /// <returns></returns>
    [HttpPost]
    public async Task<JsonResult> FindApiCompany(string companyName)
    {
      try
      {
        ProjectRepository pr = new ProjectRepository();

        var list = await pr.SelectApiCompanyList(companyName);

        return Json(new
        {
          list = list
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

    #endregion

    #region Excel 및 invoice Down 관련

    public async Task<ActionResult> MyProjectListExcelDown(string searchOption, string searchTxt, string startDt, string endDt, string orderOption, string orderTxt)
    {
      try
      {
        ProjectRepository pr = new ProjectRepository();

        MyProjectSearchModel search = new MyProjectSearchModel()
        {
          searchOption = searchOption,
          searchTxt = searchTxt,
          startDt = startDt,
          endDt = endDt,
          orderOption = orderOption,
          orderTxt = orderTxt
        };

        var excel = new Excel<ProjectListExcelModel>();
        var excelData = await pr.SelectMyProjectListWithoutCountAsync(search, AppIdentity.user_seq);

        var data = excel.WriteExcel(excelData, "나의 프로젝트-채용");

        return File(data.FileContents, data.ContentType, data.FileDownloadName);


      }
      catch (Exception e)
      {
        throw e;
      }
    }

    public async Task<ActionResult> MyProjectRepListExcelDown(string searchOption, string searchTxt, string startDt, string endDt, string orderOption, string orderTxt)
    {
      try
      {
        ProjectRepository pr = new ProjectRepository();

        MyProjectSearchModel search = new MyProjectSearchModel()
        {
          searchOption = searchOption,
          searchTxt = searchTxt,
          startDt = startDt,
          endDt = endDt,
          orderOption = orderOption,
          orderTxt = orderTxt
        };

        var excel = new Excel<ProjectRepListExcelModel>();
        var excelData = await pr.SelectMyProjectRepListWithoutAsync(search, AppIdentity.user_seq);

        var data = excel.WriteExcel(excelData, "나의 프로젝트-평판조회");

        return File(data.FileContents, data.ContentType, data.FileDownloadName);

      }
      catch (Exception e)
      {
        throw e;
      }
    }

    //public void ProjectInvoiceWordSave(ProjectInvoiceCreateModel data, string fileName)
    public async Task ProjectInvoiceWordSave()
    {
      try
      {

        ProjectRepository pr = new ProjectRepository();
        var data = await pr.SelectProjectInvoiceOneAsync(84, 47, 0);
        var chargeList = await pr.SelectProjectInvoiceSalesListAsync(1);

        ProjectInvoiceCreateModel model = new ProjectInvoiceCreateModel()
        {
          p_seq = data.p_seq,
          c_seq = data.c_seq,
          pii_seq = data.pii_seq,
          prc_seq = data.prc_seq,
          candidate_name = data.candidate_name,
          join_dt = data.join_dt,
          billing_dt = data.billing_dt,
          annual_income = data.annual_income,
          fee_rate = data.fee_rate,
          billing_money = data.billing_money,
          billing_type = data.billing_type,
          expire_guarantee = data.expire_guarantee,
          client_name = data.client_name,
          ceo = data.ceo,
          addr = data.addr,
          biz_code = data.biz_code,
          is_open_name = data.is_open_name,
          invoice_lang = data.invoice_lang,
          invoice_type = data.invoice_type,
          is_open_annual_income = data.is_open_annual_income,
          remarks = data.remarks,
          chargeList = chargeList
        };

        string fileName = "인보이스-채용.docx";

        string path = Path.Combine(Server.MapPath("~/UploadedFiles"), "Invoice\\");

        Directory.CreateDirectory(path);

        using (var document = DocX.Create(Path.Combine(path, fileName)))
        {
          document.InsertParagraph("Invoice Information").FontSize(15d).Bold().SpacingAfter(50d).Alignment = Alignment.center;

          //기본정보 영역
          document.InsertParagraph("기본정보").FontSize(15d).Bold().Alignment = Alignment.left;
          document.InsertParagraph("");

          //기본정보 테이블 생성.
          var normalTable = document.AddTable(6, 3);
          normalTable.Alignment = Alignment.center;
          normalTable.Design = TableDesign.None;
          normalTable.SetWidthsPercentage(new[] { 33f, 33f, 33f }, 500);
          normalTable.SetTableCellMargin(TableCellMarginType.bottom, 5f);


          //첫째줄 (셀 병합)
          normalTable.Rows[0].MergeCells(0, 2);
          normalTable.Rows[0].Cells[0].Paragraphs.First().Append("후보자명").Bold();
          normalTable.Rows[1].MergeCells(0, 2);
          normalTable.Rows[1].Cells[0].Paragraphs.First().Append(model.candidate_name);

          //둘째줄
          normalTable.Rows[2].Cells[0].Paragraphs.First().Append("입사일자").Bold();
          normalTable.Rows[2].Cells[1].Paragraphs.First().Append("빌링일자").Bold();
          normalTable.Rows[2].Cells[2].Paragraphs.First().Append("개런티 기간 만료일").Bold();

          normalTable.Rows[3].Cells[0].Paragraphs.First().Append(Utils.ConvertDateTimeToString(model.join_dt));
          normalTable.Rows[3].Cells[1].Paragraphs.First().Append(Utils.ConvertDateTimeToString(model.billing_dt));
          normalTable.Rows[3].Cells[2].Paragraphs.First().Append(Utils.ConvertDateTimeToString(model.expire_guarantee));

          //셋째줄
          //빌링타입 정액 일 때
          if (data.billing_type == 1)
          {
            normalTable.Rows[4].MergeCells(0, 1);
            normalTable.Rows[4].Cells[0].Paragraphs.First().Append("연봉").Bold();
            normalTable.Rows[4].Cells[2].Paragraphs.First().Append("빌링액").Bold();

            normalTable.Rows[5].MergeCells(0, 1);
            normalTable.Rows[5].Cells[0].Paragraphs.First().Append(Utils.ConvertMoneyToString(model.annual_income) + " Won");
            normalTable.Rows[5].Cells[2].Paragraphs.First().Append(Utils.ConvertMoneyToString(model.billing_money) + " Won");
          }
          //빌링타입 비율 일 때
          else
          {
            normalTable.Rows[4].Cells[0].Paragraphs.First().Append("연봉").Bold();
            normalTable.Rows[4].Cells[1].Paragraphs.First().Append("수수료율").Bold();
            normalTable.Rows[4].Cells[2].Paragraphs.First().Append("빌링액").Bold();

            normalTable.Rows[5].Cells[0].Paragraphs.First().Append(Utils.ConvertMoneyToString(model.annual_income) + " Won");
            normalTable.Rows[5].Cells[1].Paragraphs.First().Append(Utils.ConvertMoneyToString(model.fee_rate) + " %");
            normalTable.Rows[5].Cells[2].Paragraphs.First().Append(Utils.ConvertMoneyToString(model.billing_money) + " Won");
          }

          //문서에 테이블 추가.
          document.InsertTable(normalTable);

          document.InsertParagraph("");
          document.InsertParagraph("");

          //매출배분 영역
          document.InsertParagraph("매출배분").FontSize(15d).Bold().Alignment = Alignment.left;
          document.InsertParagraph("");

          //매출배분 테이블.
          var salesTable = document.AddTable(model.chargeList.Count(), 3);
          salesTable.Alignment = Alignment.center;
          salesTable.Design = TableDesign.None;
          salesTable.SetWidthsPercentage(new[] { 33f, 33f, 33f }, 500);
          salesTable.SetTableCellMargin(TableCellMarginType.bottom, 7f);
          salesTable.SetBorder(TableBorderType.InsideH, new Border() { Tcbs = BorderStyle.Tcbs_single, Color = Color.Black });
          salesTable.SetBorder(TableBorderType.Bottom, new Border() { Tcbs = BorderStyle.Tcbs_single, Color = Color.Black });

          //매출배분 담당자 리스트
          for (int i = 0; i < model.chargeList.Count(); i++)
          {
            salesTable.Rows[i].Cells[0].Paragraphs.First().Append(model.chargeList[i].name);
            salesTable.Rows[i].Cells[1].Paragraphs.First().Append(model.chargeList[i].sales_rate + " %");
            salesTable.Rows[i].Cells[2].Paragraphs.First().Append(model.chargeList[i].sales_money + " Won");
          }

          //문서에 테이블 추가.
          document.InsertTable(salesTable);

          document.InsertParagraph("");
          document.InsertParagraph("");

          //수신정보 영역
          document.InsertParagraph("수신정보").FontSize(15d).Bold().Alignment = Alignment.left;
          document.InsertParagraph("");

          //수신정보 테이블.
          var receiveTable = document.AddTable(4, 2);
          receiveTable.Alignment = Alignment.center;
          receiveTable.Design = TableDesign.None;
          receiveTable.SetWidthsPercentage(new[] { 66f, 33f }, 500);
          receiveTable.SetTableCellMargin(TableCellMarginType.bottom, 10f);

          //첫째줄
          receiveTable.Rows[0].Cells[0].Paragraphs.First().Append("고객사명").Bold();
          receiveTable.Rows[0].Cells[1].Paragraphs.First().Append("대표자명").Bold();

          receiveTable.Rows[1].Cells[0].Paragraphs.First().Append(model.client_name);
          receiveTable.Rows[1].Cells[1].Paragraphs.First().Append(model.ceo);

          //둘째줄
          receiveTable.Rows[2].Cells[0].Paragraphs.First().Append("주소").Bold();
          receiveTable.Rows[2].Cells[1].Paragraphs.First().Append("사업자번호").Bold();

          receiveTable.Rows[3].Cells[0].Paragraphs.First().Append(model.addr);
          receiveTable.Rows[3].Cells[1].Paragraphs.First().Append(model.biz_code);

          //문서에 테이블 추가.
          document.InsertTable(receiveTable);


          document.InsertParagraph("비고").FontSize(12d).Bold().Alignment = Alignment.left;
          var remarkBox = document.AddTable(1, 1);
          remarkBox.Alignment = Alignment.left;
          remarkBox.Design = TableDesign.None;
          remarkBox.Rows[0].Height = 100;
          remarkBox.SetWidthsPercentage(new[] { 100f }, 500);

          remarkBox.SetBorder(TableBorderType.Top, new Border() { Tcbs = BorderStyle.Tcbs_outset, Color = Color.Black });
          remarkBox.SetBorder(TableBorderType.Bottom, new Border() { Tcbs = BorderStyle.Tcbs_outset, Color = Color.Black });
          remarkBox.SetBorder(TableBorderType.Left, new Border() { Tcbs = BorderStyle.Tcbs_outset, Color = Color.Black });
          remarkBox.SetBorder(TableBorderType.Right, new Border() { Tcbs = BorderStyle.Tcbs_outset, Color = Color.Black });
          remarkBox.Rows[0].Cells[0].Paragraphs.First().Append(model.remarks);
          document.InsertTable(remarkBox);

          //저장.
          document.Save();

        }

      }
      catch (Exception e)
      {
        throw e;
      }


    }

    #endregion

    #region Worksheet

    /// <summary>
    /// WorkSheet Main
    /// </summary>
    /// <param name="search"></param>
    /// <returns></returns>
    public async Task<ActionResult> WorkSheet(WorkSheetSearchModel search, int page = 1)
    {
      AccountRepository ar = new AccountRepository();

      if (search.ud_seq == 0)
        search.ud_seq = AppIdentity.ud_seq;

      if (search.uv_seq == 0)
        search.uv_seq = AppIdentity.user_seq;

      if (string.IsNullOrWhiteSpace(search.startDt))
        search.startDt = Utils.NowKorea().AddDays(-7).ToString("yyyy-MM-dd");

      if (string.IsNullOrWhiteSpace(search.endDt))
        search.endDt = Utils.NowKorea().ToString("yyyy-MM-dd");

      var deptList = await ar.SelectDivisionList();
      var userList = await ar.SelectUserListByDivision(search.ud_seq);
      /*
      ProjectRepository pr = new ProjectRepository();

      int invoice_totalCount = 0;

      //한 페이지당 몇개의 행을 보여줄 것인지
      int pageSize = AppPaging.PageLength15;

      if (search.ud_seq == 0)
          search.ud_seq = AppIdentity.ud_seq;

      if (search.uv_seq == 0)
          search.uv_seq = AppIdentity.user_seq;

      if (string.IsNullOrWhiteSpace(search.startDt))
          search.startDt = Utils.NowKorea().AddDays(-7).ToString("yyyy-MM-dd");

      if (string.IsNullOrWhiteSpace(search.endDt))
          search.endDt = Utils.NowKorea().ToString("yyyy-MM-dd");

      var list = pr.SelectWorkSheetInvoiceStatisticsList(search, (page - 1) * pageSize, pageSize, out totalCount);

      WorkSheetInvoiceListModel invoice_model = new WorkSheetInvoiceListModel()
      {
          invoice_orderOption = search.invoice_orderOption
          ,
          invoice_orderTxt = search.invoice_orderTxt
          ,
          list = list
          ,
          PagingInfo = new PagingInfo()
          {
              CurrentPage = page,
              ItemsPerPage = pageSize,
              TotalItems = totalCount
          }
      };
      */
      WorkSheetMainModel model = new WorkSheetMainModel()
      {
        search = search
          ,
        deptList = deptList
          ,
        userList = userList
      };

      return View(model);
    }

    /// <summary>
    /// WorkSheet List
    /// </summary>
    /// <param name="search"></param>
    /// <param name="page"></param>
    /// <returns></returns>
    public PartialViewResult WorkSheetList(WorkSheetSearchModel search, int page = 1)
    {
      ProjectRepository pr = new ProjectRepository();

      int totalCount = 0;

      //한 페이지당 몇개의 행을 보여줄 것인지
      int pageSize = AppPaging.PageLength20;

      if (search.ud_seq == 0)
        search.ud_seq = AppIdentity.ud_seq;

      if (search.uv_seq == 0)
        search.uv_seq = AppIdentity.user_seq;

      if (string.IsNullOrWhiteSpace(search.startDt))
        search.startDt = Utils.NowKorea().AddDays(-7).ToString("yyyy-MM-dd");

      if (string.IsNullOrWhiteSpace(search.endDt))
        search.endDt = Utils.NowKorea().ToString("yyyy-MM-dd");

      var list = pr.SelectWorkSheetList(search, (page - 1) * pageSize, pageSize, out totalCount);

      WorkSheetListModel model = new WorkSheetListModel()
      {
        worksheet_orderOption = search.worksheet_orderOption
          ,
        worksheet_orderTxt = search.worksheet_orderTxt
          ,
        list = list
          ,
        PagingInfo = new PagingInfo()
        {
          CurrentPage = page,
          ItemsPerPage = pageSize,
          TotalItems = totalCount
        }
      };

      return PartialView(model);
    }

    /// <summary>
    /// WorkSheet Invoice
    /// </summary>
    /// <param name="search"></param>
    /// <param name="page"></param>
    /// <returns></returns>
    public PartialViewResult WorkSheetInvoiceList(WorkSheetSearchModel search, int page = 1)
    {
      ProjectRepository pr = new ProjectRepository();

      int totalCount = 0;

      //한 페이지당 몇개의 행을 보여줄 것인지
      int pageSize = AppPaging.PageLength15;

      if (search.ud_seq == 0)
        search.ud_seq = AppIdentity.ud_seq;

      if (search.uv_seq == 0)
        search.uv_seq = AppIdentity.user_seq;

      if (string.IsNullOrWhiteSpace(search.startDt))
        search.startDt = Utils.NowKorea().AddDays(-7).ToString("yyyy-MM-dd");

      if (string.IsNullOrWhiteSpace(search.endDt))
        search.endDt = Utils.NowKorea().ToString("yyyy-MM-dd");

      var list = pr.SelectWorkSheetInvoiceStatisticsList(search, (page - 1) * pageSize, pageSize, out totalCount);

      WorkSheetInvoiceListModel model = new WorkSheetInvoiceListModel()
      {
        invoice_orderOption = search.invoice_orderOption
          ,
        invoice_orderTxt = search.invoice_orderTxt
          ,
        list = list
          ,
        PagingInfo = new PagingInfo()
        {
          CurrentPage = page,
          ItemsPerPage = pageSize,
          TotalItems = totalCount
        }
      };

      return PartialView(model);
    }

    #endregion

    #region Weakly Report

    public async Task<ActionResult> WeeklyReport(WeeklyReportSearchModel search)
    {
      //if (search.ud_seq == 0)
      //    search.ud_seq = AppIdentity.ud_seq;

      if (string.IsNullOrWhiteSpace(search.startDt))
        search.startDt = Utils.NowKorea().AddDays(-7).ToString("yyyy-MM-dd");

      if (string.IsNullOrWhiteSpace(search.endDt))
        search.endDt = Utils.NowKorea().ToString("yyyy-MM-dd");

      AccountRepository ar = new AccountRepository();
      var deptList = await ar.SelectDivisionList();
      deptList.Insert(0, new uv_division()
      {
        ud_seq = 0,
        ud_name = "==Order Completion=="
      });

      ProjectRepository pr = new ProjectRepository();
      var totalList = await pr.SelectWeeklyReportList(search);
      var successList = await pr.SelectWeeklySuccessReport2List(search);
      var completeList = await pr.SelectWeeklyCompleteReportList(search);

      //var successList = totalList.Where(x => x.pjt_status == ProjectStatusCode.success).GroupBy(x => new { x.p_seq, x.hire_candidate_seq }).Select(x => x.First()).ToList();
      //var completeList = totalList.Where(x => x.pjt_status == ProjectStatusCode.complete).GroupBy(x => new { x.p_seq, x.hire_candidate_seq }).Select(x => x.First()).ToList();
      var list = totalList.Where(x => x.pjt_status != ProjectStatusCode.success && x.pjt_status != ProjectStatusCode.complete).OrderBy(x => (x.retire_date)).ThenBy(x => (x.ud_seq == search.ud_seq ? 1 : 2)).ThenBy(x => x.ua_seq).ThenBy(x => x.name).ThenByDescending(x => x.create_dt).ToList();

      WeeklyReportListModel model = new WeeklyReportListModel()
      {
        search = search
          ,
        deptList = deptList
          ,
        successList = successList
          ,
        completeList = completeList
          ,
        sum_point = (successList.Sum(x => x.point) + completeList.Sum(x => x.point) + list.Sum(x => x.point))
          ,
        list = list
      };

      return View(model);
    }

    #endregion


    public async Task HomepageUpdate(int p_seq)
    {
      ProjectRepository pr = new ProjectRepository();
      HomepageEntityRepository her = new HomepageEntityRepository();

      var uv_homepage_pjt = await pr.SelectHomeProjectOneAsync(p_seq);
      var home_pjt = await her.SelectHomeProjectOneAsync(p_seq);

      if (uv_homepage_pjt != null)
      {
        var state = CommonCodes.Update;
        if (home_pjt == null)
        {
          state = CommonCodes.Create;
          home_pjt = new home_project();
        }
        home_pjt.p_seq = uv_homepage_pjt.p_seq;
        home_pjt.open_dt = uv_homepage_pjt.open_dt;
        home_pjt.close_dt = uv_homepage_pjt.close_dt;
        home_pjt.title = uv_homepage_pjt.title;
        home_pjt.business_code1 = uv_homepage_pjt.business_code1;
        home_pjt.business_code2 = uv_homepage_pjt.business_code2;
        home_pjt.is_foreign_invest = uv_homepage_pjt.is_foreign_invest;
        home_pjt.job_code1 = uv_homepage_pjt.job_code1;
        home_pjt.job_code2 = uv_homepage_pjt.job_code2;
        home_pjt.edu_name = uv_homepage_pjt.edu_name;
        home_pjt.position_name = uv_homepage_pjt.position_name;
        home_pjt.assign_task = uv_homepage_pjt.assign_task;
        home_pjt.requirement = uv_homepage_pjt.requirement;
        home_pjt.experience_type = uv_homepage_pjt.experience_type;
        home_pjt.expreience_number = uv_homepage_pjt.expreience_number;
        home_pjt.working_area_1 = uv_homepage_pjt.working_area_1;
        home_pjt.eu_name = uv_homepage_pjt.eu_name;
        home_pjt.eu_email = uv_homepage_pjt.eu_email;
        home_pjt.eu_company_phone = uv_homepage_pjt.eu_company_phone;
        home_pjt.eu_position = uv_homepage_pjt.eu_position;
        home_pjt.interview_process_1 = uv_homepage_pjt.interview_process_1;
        home_pjt.create_dt = uv_homepage_pjt.create_dt;
        home_pjt.create_user = uv_homepage_pjt.create_user;
        home_pjt.modify_dt = uv_homepage_pjt.modify_dt;
        home_pjt.modify_user = uv_homepage_pjt.modify_user;


        await her.CreateOrUpdateHomeProject(home_pjt, state);
      }
      else
      {
        if (home_pjt != null)
        {
          await her.DeleteHomeProject(home_pjt);
        }
      }

    }
  }
}