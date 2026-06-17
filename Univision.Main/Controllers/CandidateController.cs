using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Serialization;
using OpenAI_API;
using OpenAI_API.Chat;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using Univision.Core.Models;
using Univision.Core.Models.DTO;
using Univision.Core.Models.DTO.Request;
using Univision.Core.Models.DTO.Request.Board;
using Univision.Core.Models.DTO.Request.Candidate;
using Univision.Core.Models.DTO.Response.Board;
using Univision.Core.Models.DTO.Response.Candidate;
using Univision.Core.Models.DTO.Response.Project;
using Univision.Core.Repositories;
using Univision.Core.Repositories.MailResume;
using Univision.Main.Infrastructure;
using Univision.Main.Infrastructure.Mailing;
using Univision.Main.Models.Api;
using Univision.Main.Models.Candidate;
using Univision.Main.Models.Project;
using Univision.Security;

namespace Univision.Main.Controllers
{
  public class CandidateController : BaseController
  {
    #region 공통 사용

    #region [리스트/디테일 공통]관심후보 등록을 위한 프로젝트 목록

    /// <summary>
    /// 관심후보 등록을 위한 프로젝트 목록
    /// </summary>
    /// <param name="c_seq"></param>
    /// <param name="search"></param>
    /// <param name="page"></param>
    /// <returns></returns>
    public async Task<PartialViewResult> SearchMyProject(int? c_seq, MyProjectSearchModel search, int page = 1)
    {
      ProjectRepository pr = new ProjectRepository();

      int totalCount = 0;

      //한 페이지당 몇개의 행을 보여줄 것인지
      int pageSize = AppPaging.PageLength10;

      search.orderTxt = "P.modify_dt";

      //var cntData = await pr.SelectProjectStatusCountAsync(search, AppIdentity.user_seq);
      //var list = pr.SelectMyProjectList(search, AppIdentity.user_seq, (page - 1) * pageSize, pageSize, out totalCount);
      var list = pr.SelectPopAllProjectList(search, AppIdentity.user_seq, (page - 1) * pageSize, pageSize, out totalCount);
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
      return PartialView("SearchMyProjectModal", model);
    }

    #endregion

    #region [리스트/디테일 공통]마이리스트 추가하기.

    /// <summary>
    /// 마이리스트 추가하기.
    /// </summary>
    /// <param name="c_seq"></param>
    /// <returns></returns>
    public async Task<JsonResult> CreateMyList(int c_seq)
    {
      try
      {
        CandidateEntityRepository cer = new CandidateEntityRepository();

        var cf = await cer.SelectCanMyListOneAsync(c_seq, AppIdentity.user_seq);

        if (cf != null)
        {
          return Json(new
          {
            ok = false
              ,
            message = "이미 마이리스트 등록된 후보자 입니다."
          });
        }

        cf = new can_interest()
        {
          c_seq = c_seq
            ,
          uv_seq = AppIdentity.user_seq
            ,
          create_dt = Utils.NowKorea()
        };

        await cer.CreateOrDeleteCanMyList(cf, CommonCodes.Create);

        return Json(new
        {
          ok = true
            ,
          cf_seq = cf.cf_seq
            ,
          message = "마이리스트 등록 완료."
        });
      }
      catch (Exception e)
      {
        return Json(new
        {
          ok = false
            ,
          message = "알 수 없는 오류가 발생 했습니다." + e.Message
        });
      }
    }

    #endregion

    #region [리스트/디테일 공통]마이리스트 삭제하기.

    /// <summary>
    /// 마이리스트 삭제하기
    /// </summary>
    /// <param name="cf_seq"></param>
    /// <returns></returns>
    public async Task<JsonResult> DeleteMyList(int cf_seq)
    {
      try
      {
        CandidateEntityRepository cer = new CandidateEntityRepository();

        var cf = await cer.SelectCanMyListOneAsync(cf_seq);

        if (cf == null)
        {
          return Json(new
          {
            ok = false
              ,
            message = "마이리스트에서 삭제하려는 후보를 찾을 수 없습니다."
          });
        }

        await cer.CreateOrDeleteCanMyList(cf, CommonCodes.Delete);

        return Json(new
        {
          ok = true
            ,
          message = "마이리스트에서 후보자를 삭제 했습니다."
        });
      }
      catch (Exception e)
      {
        return Json(new
        {
          ok = false
            ,
          message = "알 수 없는 오류가 발생 했습니다." + e.Message
        });
      }
    }

    #endregion

    #endregion

    #region 후보자 리스트

    /// <summary>
    /// 후보자 리스트
    /// </summary>
    /// <param name="search"></param>
    /// <param name="page"></param>
    /// <returns></returns>
    public async Task<ActionResult> CandidateList(CandidateSearchModel search, CandidateSearchOptionModel search_opt, int page = 1)
    {
      int totalCount = 0;

      search.replace_search_text();

      //한 페이지당 몇개의 행을 보여줄 것인지
      int pageSize = AppPaging.PageLength20;

      //CandidateRepository cr = new CandidateRepository();
      SearchEngineRepository ser = new SearchEngineRepository();

      //var list = cr.SelectCandidateList(search, (page - 1) * pageSize, pageSize, out totalCount);
      //var list = ser.SelectCandidateList(search, page, pageSize, AppIdentity.name, out totalCount);
      var list = ser.SelectTokenCandidateList(search, search_opt, page, pageSize, AppIdentity.name, out totalCount);
      //var list = ser.SelectCandidateList(search, page, pageSize, out totalCount);

      CandidateRepository cr = new CandidateRepository();
      CandidateEntityRepository cer = new CandidateEntityRepository();

      for (int i = 0; i < list.Count; i++)
      {
        can_interest cf = await cer.SelectCanMyListOneAsync(list[i].c_seq, AppIdentity.user_seq);

        if (cf != null)
          list[i].cf_seq = cf.cf_seq;

        view_can_activity vca = await cr.SelectCanProjectProgressOneAsync(list[i].c_seq);
        if (vca != null)
          list[i].project_history = vca;



        if (AppIdentity.isExternal == 1)
        {
          element_open_log eol = await cer.SelectCanOpenLogOneAsync(list[i].c_seq, AppIdentity.user_seq);
          if (eol == null)
          {
            list[i].is_external_lock = 1;
            if (!String.IsNullOrEmpty(list[i].email1))
              list[i].email1 = Regex.Replace(list[i].email1, "[a-zA-Z0-9]", "*");
            if (!String.IsNullOrEmpty(list[i].email2))
              list[i].email2 = Regex.Replace(list[i].email2, "[a-zA-Z0-9]", "*");

            if (!String.IsNullOrEmpty(list[i].phone))
              list[i].phone = Regex.Replace(list[i].phone, "[0-9]", "0");

            if (!String.IsNullOrEmpty(list[i].cell_phone))
              list[i].cell_phone = Regex.Replace(list[i].cell_phone, "[0-9]", "0");

          }
        }
      }

      CandidateListViewModel model = new CandidateListViewModel
      {
        search = search
        ,
        search_opt = search_opt
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

      return View(model);
    }


    public async Task<ActionResult> CandidateOldList(CandidateSearchModel search, int page = 1)
    {
      int totalCount = 0;

      search.replace_search_text();

      //한 페이지당 몇개의 행을 보여줄 것인지
      int pageSize = AppPaging.PageLength20;

      //CandidateRepository cr = new CandidateRepository();
      SearchEngineRepository ser = new SearchEngineRepository();

      //var list = cr.SelectCandidateList(search, (page - 1) * pageSize, pageSize, out totalCount);
      var list = ser.SelectCandidateList(search, page, pageSize, AppIdentity.name, out totalCount);
      //var list = ser.SelectCandidateList(search, page, pageSize, out totalCount);

      CandidateRepository cr = new CandidateRepository();
      CandidateEntityRepository cer = new CandidateEntityRepository();

      for (int i = 0; i < list.Count; i++)
      {
        can_interest cf = await cer.SelectCanMyListOneAsync(list[i].c_seq, AppIdentity.user_seq);

        if (cf != null)
          list[i].cf_seq = cf.cf_seq;

        view_can_activity vca = await cr.SelectCanProjectProgressOneAsync(list[i].c_seq);
        if (vca != null)
          list[i].project_history = vca;



        if (AppIdentity.isExternal == 1)
        {
          element_open_log eol = await cer.SelectCanOpenLogOneAsync(list[i].c_seq, AppIdentity.user_seq);
          if (eol == null)
          {
            list[i].is_external_lock = 1;
            if (!String.IsNullOrEmpty(list[i].email1))
              list[i].email1 = Regex.Replace(list[i].email1, "[a-zA-Z0-9]", "*");
            if (!String.IsNullOrEmpty(list[i].email2))
              list[i].email2 = Regex.Replace(list[i].email2, "[a-zA-Z0-9]", "*");

            if (!String.IsNullOrEmpty(list[i].phone))
              list[i].phone = Regex.Replace(list[i].phone, "[0-9]", "0");

            if (!String.IsNullOrEmpty(list[i].cell_phone))
              list[i].cell_phone = Regex.Replace(list[i].cell_phone, "[0-9]", "0");

          }
        }
      }

      CandidateListViewModel model = new CandidateListViewModel
      {
        search = search
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

      return View(model);
    }


    public async Task<ActionResult> DirectorList(DirectorSearchModel search, int CurrentPage = 1, int ItemsPerPage = 20)
    {

      DirectorListViewModel model = new DirectorListViewModel
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

    public async Task<ActionResult> DirectorAjxList(DirectorSearchModel search, int CurrentPage = 1, int ItemsPerPage = 20)
    {
      search.replace_search_text();

      //CandidateRepository cr = new CandidateRepository();
      CandidateEntityRepository ser = new CandidateEntityRepository();
      CandidateRepository cr = new CandidateRepository();
      //var list = cr.SelectCandidateList(search, (page - 1) * pageSize, pageSize, out totalCount);

      var data = await cr.SelectDirectorListAsync(search, CurrentPage - 1, ItemsPerPage);

      DirectorListViewModel model = new DirectorListViewModel
      {
        search = search,
        list = data.Items,
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


    public async Task<PartialViewResult> CandidateSingleList(int c_seq)
    {

      //CandidateRepository cr = new CandidateRepository();
      SearchEngineRepository cr = new SearchEngineRepository();


      //var list = cr.SelectCandidateList(search, (page - 1) * pageSize, pageSize, out totalCount);
      var model = cr.SelectCandidateOne(c_seq);

      CandidateEntityRepository cer = new CandidateEntityRepository();

      if (model != null)
      {
        can_interest cf = await cer.SelectCanMyListOneAsync(model.c_seq, AppIdentity.user_seq);

        if (cf != null)
          model.cf_seq = cf.cf_seq;


        if (AppIdentity.isExternal == 1)
        {
          element_open_log eol = await cer.SelectCanOpenLogOneAsync(model.c_seq, AppIdentity.user_seq);
          if (eol == null)
          {
            model.is_external_lock = 1;
            if (!String.IsNullOrEmpty(model.email1))
              model.email1 = Regex.Replace(model.email1, "[a-zA-Z0-9]", "*");
            if (!String.IsNullOrEmpty(model.email2))
              model.email2 = Regex.Replace(model.email2, "[a-zA-Z0-9]", "*");

            if (!String.IsNullOrEmpty(model.phone))
              model.phone = Regex.Replace(model.phone, "[0-9]", "0");

            if (!String.IsNullOrEmpty(model.cell_phone))
              model.cell_phone = Regex.Replace(model.cell_phone, "[0-9]", "0");

          }
        }
      }

      return PartialView(model);
    }

    public async Task<PartialViewResult> PartialSearchBusiList(CandidateSearchModel search)
    {
      return PartialView(search);
    }

    public async Task<PartialViewResult> PartialDirectorSearchBusiList(DirectorSearchModel search)
    {

      return PartialView(search);
    }

    public async Task<PartialViewResult> PartialSearchJobList(CandidateSearchModel search)
    {

      return PartialView(search);
    }

    public async Task<PartialViewResult> PartialDirectorSearchJobList(DirectorSearchModel search)
    {
      return PartialView(search);
    }
    /// <summary>
    /// 후보자 리스트
    /// </summary>
    /// <param name="search"></param>
    /// <param name="page"></param>
    /// <returns></returns>
    public async Task<ActionResult> TemporaryCandidateList(TempCandidateSearchModel search, int page = 1)
    {
      //한 페이지당 몇개의 행을 보여줄 것인지
      int pageSize = AppPaging.PageLength20;
      int totalCount = 0;
      CandidateRepository cr = new CandidateRepository();
      search.uv_seq = AppIdentity.user_seq;
      var list = cr.SelectTempCandidateList(search, (page - 1) * pageSize, pageSize, out totalCount);

      TempCandidateListViewModel model = new TempCandidateListViewModel()
      {
        search = search
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

      return View(model);

    }

    public async Task<ActionResult> ShareCandidateList(int page = 1, int pageSize = 20, string SelectOption = "", string SearchString = "")
    {
      int bType = 5; //공지사항:0, 교육게:1, 지식게:2, 자유게:3, 파일게:4
      int bTypeSub1 = 0;
      int bTypeSub2 = 0;
      int totalCnt = 0;
      int TopBoardCnt = 0;
      int topTotalCnt = 0;

      BoardRepository bd = new BoardRepository();
      BoardListModel model = new BoardListModel()
      {
        TopBoardList = bd.BoardList(bType, bTypeSub1, bTypeSub2, 1, 5, "b_top", "1", out topTotalCnt),
        BoardList = bd.BoardList(bType, bTypeSub1, bTypeSub2, page, pageSize, SelectOption, SearchString, out totalCnt),
        PagingInfo = new PagingInfo()
        {
          CurrentPage = page,
          ItemsPerPage = pageSize,
          TotalItems = totalCnt
        }
      };
      ViewBag.searchOption = SelectOption;
      ViewBag.searchString = SearchString;

      return View(model);

    }

    public async Task<ActionResult> ShareCandidateCreate(int b_seq = 0)
    {
      BoardRepository bd = new BoardRepository();


      var data = await bd.SelectBoardOneAsync(b_seq, AppIdentity.user_seq);
      var fileList = await bd.SelectBoardFileListAsync(b_seq);
      //string uploadFolder = b_seq.ToString();
      string uploadTmpFolder = Utils.ReturnUniqueValue(AppIdentity.user_seq);
      if (data == null)
      {
        data = new board()
        {
          b_type = 5,
          b_type_sub1 = 0,
          b_type_sub2 = 0
        };

      }

      if (fileList == null)
      {
        fileList = new List<board_file>();
      }

      BoardDetailViewModel model = new BoardDetailViewModel()
      {
        data = data,
        boardFileList = fileList
      };

      ViewBag.uploadFolder = uploadTmpFolder;

      //return View(model);
      return View(model);
    }

    public async Task<ActionResult> ShareCandidateDetail(int b_seq = 0)
    {
      BoardRepository bd = new BoardRepository();

      var data = await bd.SelectBoardOneAsync(b_seq, AppIdentity.user_seq);
      var fileList = await bd.SelectBoardFileListAsync(b_seq);

      BoardDetailViewModel model = new BoardDetailViewModel()
      {
        data = data,
        boardFileList = fileList
      };

      BoardEntityRepository ber = new BoardEntityRepository();
      var read_data = await ber.SelectBoardReadOneAsync(b_seq, AppIdentity.user_seq);
      string state = String.Empty;
      if (read_data == null)
      {
        read_data = new board_read_history()
        {
          c_seq = b_seq,
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
      await ber.CreateOrUpdateBoardRead(read_data, state);

      return View(model);

    }

    public PartialViewResult ShareCandidateCreateFileList(BoardDetailViewModel model)
    {
      return PartialView(model);
    }

    //첨부파일 임시 업로드후 부분뷰 처리
    [HttpPost]
    public PartialViewResult ShareCandidateCreateFileList(BoardDetailViewModel model, board_file[] files)
    {
      if (model == null)
      {
        model = new BoardDetailViewModel();
      }

      if (model.boardFileList == null)
        model.boardFileList = new List<board_file>();

      if (files != null)
      {
        foreach (board_file file in files)
          model.boardFileList.Add(file);
      }

      return PartialView(model);
    }
    //첨부파일 임시 제거 후 부분뷰 처리
    [HttpPost]
    public PartialViewResult ShareCandidateRemoveFileList(BoardDetailViewModel model, int file_seq)
    {
      if (model == null)
      {
        model = new BoardDetailViewModel();
      }

      if (model.boardFileList == null)
        model.boardFileList = new List<board_file>();


      model.boardFileList.RemoveAt(file_seq);
      //model.boardFileList[file_seq].file_dir = "";

      return PartialView("ShareCandidateCreateFileList", model);
      //return PartialView(model);
    }

    [HttpPost]
    public async Task<JsonResult> CreateTempShareCandidateFile(string uploadFolder, HttpPostedFileBase file)
    {
      try
      {
        FileUpload fiUpload = new FileUpload();
        string path = Server.MapPath("~/UploadedFiles");
        var rst = fiUpload.UploadTemp(path, "Temp", uploadFolder, file);


        board_file bf = new board_file()
        {
          file_dir = rst.dbPath
            ,
          file_origin_path = rst.filePath
            ,
          file_path = rst.name
            ,
          file_extension = rst.extension
        };

        if (!rst.status)
          return Json(new
          {
            ok = false,
            message = rst.statusMessage
          });

        return Json(new
        {
          ok = true,
          message = "이력서를 등록 했습니다.",
          result = bf
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

    // 게시판 신규/수정 (저장)
    [HttpPost]
    public async Task<JsonResult> ShareCandidateCreate(BoardCreateUpdateModel model, int do_alert = 0)
    {
      try
      {

        BoardEntityRepository ber = new BoardEntityRepository();
        BoardRepository br = new BoardRepository();
        //신규, 수정 state 변수
        string state = "";

        model.data.b_top = model.data.b_top != null ? model.data.b_top : 0;
        model.data.modify_dt = Utils.NowKorea();
        model.data.modify_user = AppIdentity.user_seq;
        //신규
        if (model.data.b_seq == 0)
        {
          model.data.b_seq = 0;
          model.data.create_dt = Utils.NowKorea();
          model.data.create_user = AppIdentity.user_seq;

          state = CommonCodes.Create;
        }
        //신규
        else
        {
          model.deleteFileList = await br.SelectBoardFileListAsync(model.data.b_seq);
          state = CommonCodes.Update;
        }


        //게시글 업데이트
        await ber.CreateOrUpdateOnlyBoard(model, state);

        //파일(임시파일 정식 폴더로 이동)
        if (model.boardFileList != null)
        {
          string originpath = "";
          foreach (var file in model.boardFileList)
          {
            if (file.create_dt == null && file.file_dir != "")
            {
              FileUpload fiUpload = new FileUpload();

              string path = Server.MapPath("~/UploadedFiles");
              string source_file = file.file_origin_path;
              string target_dir = Path.Combine(path, "Board", "" + model.data.b_type, "" + model.data.b_seq);
              string target_file = fiUpload.UploadTempToFolder(file.file_origin_path, target_dir);
              if (target_file != "")
              {
                if (originpath == "")
                  originpath = file.file_origin_path;

                file.file_path = target_file;
                file.file_dir = "/UploadedFiles/Board/" + model.data.b_type + "/" + model.data.b_seq + "/" + file.file_path;
                file.file_origin_path = Path.Combine(target_dir, file.file_path);
              }
              file.create_dt = Utils.NowKorea();
              file.create_user = AppIdentity.user_seq;
            }
          }
          //임시폴더 작업이 있으면 최종 파일 이동 후 임시폴더 삭제.
          if (originpath != "")
          {
            originpath = Path.GetDirectoryName(originpath);
            Directory.Delete(originpath, true);
          }
        }
        //파일 업데이트    
        await ber.CreateOrUpdateBoardFile(model, state);

        if (do_alert == 1)
        {
          alarm_message aMessage = new alarm_message()
          {
            href_url = "/Candidate/ShareCandidateDetail?b_seq=" + model.data.b_seq,
            message = "[후보추천드려요]" + model.data.title,
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

          await ber.CreateAlarm(aMessage, aList);

          var myuserinfo = await ar.FindUserBySeqAsync(AppIdentity.user_seq);
          MailService mService = new MailService();//(myuserinfo.email_id, myuserinfo.email_pwd);
          //메일 발송 폼 데이터
          ShareCandidateDto mailData = new ShareCandidateDto()
          {
            ToArr = new string[] { "unicousers@unicosearch.com" }
              ,
            From = new System.Net.Mail.MailAddress("noreply@unicosearch.com", "noreply")
              ,
            name = AppIdentity.name
              ,
            title = model.data.title
              ,
            createDate = Utils.ConvertDateTimeHourToString2(model.data.create_dt)
              ,
            Comment = model.data.contents.Replace("&lt;", "<").Replace("&gt;", ">")
              ,
            url = "http://univision.unicosearch.com" + "/Candidate/ShareCandidateDetail?b_seq=" + model.data.b_seq
          };

          var result = mService.SendShareCandidateMail(mailData, new ShareCandidateTemplete());
          if (!result.isSend)
            return Json(new
            {
              ok = true,
              message = "Success",
              b_seq = model.data.b_seq
            });


          return Json(new
          {
            ok = true,
            message = "게시글을 저장했지만 메일 발송에 실패했습니다.",
            b_seq = model.data.b_seq
          });


        }
        else
        {
          return Json(new
          {
            ok = true,
            message = "Success",
            b_seq = model.data.b_seq
          });
        }


      }
      catch (Exception e)
      {
        return Json(new
        {
          ok = false,
          message = "Failed"
        });
      }
    }

    //게시글 삭제(공통)
    [HttpPost]
    public async Task<JsonResult> ShareCandidateRemove(int b_seq)
    {
      try
      {
        BoardEntityRepository ber = new BoardEntityRepository();
        BoardRepository br = new BoardRepository();


        BoardCreateUpdateModel model = new BoardCreateUpdateModel()
        {
          data = await ber.SelectBoardOneAsync(b_seq),
          deleteFileList = await br.SelectBoardFileListAsync(b_seq)
        };

        if (model.data == null)
          return Json(new
          {
            ok = false
              ,
            message = "Can not Find Post."
          });

        //TODO 파일 정리 프로세스 추가 필요함
        await ber.DeleteBoard(model);

        return Json(new
        {
          ok = true,
          message = "Delete Complete."
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

    #region 후보자 상세보기
    public async Task<candidate> FindDup(string phone, string email, int except_c = 0)
    {
      string number_only_phone = new String(phone.Where(Char.IsDigit).ToArray());

      if (!string.IsNullOrEmpty(number_only_phone) && phone != "000-0000-0000" && phone != "0000" && number_only_phone.Length > 4)
      {
        if (number_only_phone.Substring(0, 4) == "8210")
          number_only_phone = "010" + number_only_phone.Substring(4);
      }

      if (email != "noemail@email" && email != "no@email.address" && email != "no@no" && email != "no@email")
      {
        if (!string.IsNullOrEmpty(email))
        {
          email = email.Replace("_", "[_]");
        }
      }

      CandidateRepository cr = new CandidateRepository();
      var candidate = await cr.SelectFindDuplicateCandidate(number_only_phone, email, except_c);

      return candidate;
    }
    [HttpPost]
    public async Task<JsonResult> FindDuplicateCandidate(string phone, string email, int except_c = 0)
    {
      try
      {

        var candidate = await FindDup(phone, email, except_c);

        return Json(new
        {
          ok = true,
          candidate = candidate
        });


      }
      catch (Exception e)
      {
        return Json(new
        {
          ok = false
          ,
          message = e.Message
        });
      }
    }

    [HttpPost]
    public async Task<string> PhoneDuplicateCheck2(string phone, int except_c = 0)
    {
      try
      {
        string number_only_phone = new String(phone.Where(Char.IsDigit).ToArray());
        int dup_check = 0;
        if (!string.IsNullOrEmpty(number_only_phone) && phone != "000-0000-0000" && phone != "0000" && number_only_phone.Length > 4)
        {
          if (number_only_phone.Substring(0, 4) == "8210")
            number_only_phone = "010" + number_only_phone.Substring(4);

          CandidateRepository cr = new CandidateRepository();
          dup_check = await cr.SelectPhoneDuplicateCheck(number_only_phone, except_c);
        }

        if (dup_check > 0)
          return "false";

        return "true";
      }
      catch (Exception e)
      {
        return "false";
      }
    }


    [HttpPost]
    public async Task<String> EmailDuplicateCheck2(string email, int except_c = 0)
    {
      try
      {
        CandidateRepository cr = new CandidateRepository();

        int dup_check = 0;

        if (email != "noemail@email" && email != "no@email.address" && email != "no@no" && email != "no@email")
        {
          if (!string.IsNullOrEmpty(email))
          {
            email = email.Replace("_", "[_]");
          }
          dup_check = await cr.SelectEmailDuplicateCheck(email, except_c);
        }

        if (dup_check > 0)
          return "false";

        return "true";
      }
      catch (Exception e)
      {
        return "false";
      }
    }

    [HttpPost]
    public async Task<String> EmailDuplicateCheck(string email)
    {
      try
      {
        CandidateRepository cr = new CandidateRepository();

        int dup_check = 0;

        if (email != "noemail@email" && email != "no@email.address" && email != "no@no" && email != "no@email")
        {
          if (!string.IsNullOrEmpty(email))
          {
            email = email.Replace("_", "[_]");
          }
          dup_check = await cr.SelectCandidateDuplicateCheck(null, email);
        }

        if (dup_check > 0)
          return "false";

        return "true";
      }
      catch (Exception e)
      {
        return "false";
      }
    }

    /// <summary>
    /// 후보자 상세보기
    /// </summary>
    /// <param name="c_seq"></param>
    /// <returns></returns>
    public async Task<ActionResult> CandidateDetail(int c_seq = 0, int is_pop = 0)
    {
      CandidateRepository cr = new CandidateRepository();

      PrivacyAgreeSearchModel pa_search = new PrivacyAgreeSearchModel();
      pa_search.c_seq = c_seq;
      pa_search.is_agree_only = 1;
      int totalCount = 0;

      var data = await cr.SelectCandidateWithDetailOneAsync_2024(c_seq);
      var resumeList = await cr.SelectCanResumeListAsync(c_seq, 0);
      var careerList = await cr.SelectCanCareerListAsync(c_seq);
      var schoolList = await cr.SelectCanSchoolListAsync(c_seq);
      var businessList = await cr.SelectCanBusiCodeListAsync(c_seq);
      var jobList = await cr.SelectCanJobCodeListAsync(c_seq);
      var agreeList = cr.SelectPrivacyAgreeList(pa_search, 0, 10, out totalCount);

      /*
      DateTime tot_exp = new DateTime();
      int tot_years = 0;
      int tot_months = 0;
      int tot_days = 0;
      string bef_from = "";

      if (careerList.Count > 0)
      {
        foreach (can_career career in careerList)
        {
          DateTime to_date = new DateTime();
          DateTime from_date = new DateTime();
          //시작일자가 있어야지만 날짜 계산
          if (!string.IsNullOrWhiteSpace(career.join_dt) && DateTime.TryParse(career.join_dt.Replace("-00", "-01") + "-01", out from_date))
          {

            //시작일월 날짜 데이터로 변환
            //DateTime.TryParse(career.join_dt.Replace("-00", "-01") + "-01");
            from_date = DateTime.Parse(career.join_dt.Replace("-00", "-01") + "-01");

            //종료일자가 있을 경우 날짜 데이터로 변환
            if (!string.IsNullOrWhiteSpace(career.leave_dt) && DateTime.TryParse(career.leave_dt.Replace("-00", "-01") + "-01", out to_date))
            {
              //DateTime.TryParse(career.leave_dt.Replace("-00", "-01") + "-01", out to_date);

              to_date = DateTime.Parse(career.leave_dt.Replace("-00", "-01") + "-01");
            }
            else
            {
              //종료 일자가 없고 재직 중일경우 종료일자는 오늘로 계산
              if (career.is_work == 1 || tot_exp == new DateTime())
              {
                to_date = DateTime.Now;
              }
              //종료 일자가 없고 재직 중이 아니고 다음경력에 시작 일자가 있을 경우 종료일자는 다음경력의 시작일자를 종료 일로 계산
              else if (!string.IsNullOrWhiteSpace(bef_from))
              {
                DateTime.TryParse(bef_from.Replace("-00", "-01") + "-01", out to_date);
              }

            }

            //to_date 값이 있고 from_date < to_date 일 경우 계산 시작
            if (to_date.Year > 1)
            {
              to_date = to_date.AddMonths(1).AddDays(-1);
            }

            if (from_date.Year > 1 && to_date.Year > 1 && from_date < to_date)
            {
              tot_exp += (to_date - from_date);
            }
          }

          bef_from = career.join_dt;

        }

      }

      if (tot_exp.Year > 1 || tot_exp.Month > 1)
      {
        tot_years = tot_exp.Year - 1;//tot_days / 365;
        tot_months = tot_exp.Month - 1;//(tot_days % 365) / 30;
      }
      */
      if (AppIdentity.isExternal == 1)
      {
        CandidateEntityRepository cer = new CandidateEntityRepository();
        element_open_log eol = await cer.SelectCanOpenLogOneAsync(data.c_seq, AppIdentity.user_seq);
        if (eol == null)
        {
          data.is_external_lock = 1;
          if (!String.IsNullOrEmpty(data.email1))
            data.email1 = Regex.Replace(data.email1, "[a-zA-Z0-9]", "*");
          if (!String.IsNullOrEmpty(data.email2))
            data.email2 = Regex.Replace(data.email2, "[a-zA-Z0-9]", "*");

          if (!String.IsNullOrEmpty(data.phone))
            data.phone = Regex.Replace(data.phone, "[0-9]", "0");

          if (!String.IsNullOrEmpty(data.cell_phone))
            data.cell_phone = Regex.Replace(data.cell_phone, "[0-9]", "0");

        }
      }

      //유니코를 통한 입사일 경우 최근 1년이내 입사인지 확인 
      ViewBag.hire_in_year = false;
      if (data.caution_type == 6)
      {
        var hire_data = await cr.SelectCandidateLastestHire(c_seq);
        if (hire_data != null)
        {
          ViewBag.latest_hire_dt = "(" + Utils.ConvertDateTimeToString(hire_data.schedule_date) + ")";

          ViewBag.hire_in_year = Utils.GetAge(hire_data.schedule_date) < 1;
        }
      }

      CandidateDetailViewModel model = new CandidateDetailViewModel()
      {
        data = data,
        resumeList = resumeList,
        careerList = careerList,
        schoolList = schoolList,
        businessList = businessList,
        jobList = jobList,
        agreeList = agreeList
      };

      //ViewBag.tot_years = tot_years;
      //ViewBag.tot_months = tot_months;
      ViewBag.is_pop = is_pop;
      ViewBag.NoMenu = (is_pop == 1 ? true : false);
      ViewBag.Title = "Candidate : " + model.data.kor_name + "/" + "만 " + Utils.GetAge(model.data.birth_date) + "세(" + Utils.ReturnGenderTxt(model.data.gender) + ")";
      return View(model);
    }


    public async Task<ActionResult> DirectorDetail(int d_seq = 0, int c_seq = 0, int is_pop = 0)
    {
      CandidateRepository cr = new CandidateRepository();
      var data = await cr.SelectDirectorDetailOneAsync(d_seq);

      var careerList = await cr.SelectDirectorCareerListAsync(d_seq, -1);
      var outdirectorList = await cr.SelectDirectorCareerListAsync(d_seq, 2);
      var schoolList = await cr.SelectDirectorSchoolListAsync(d_seq);
      var businessList = await cr.SelectDirectorBusiCodeListAsync(d_seq);
      var jobList = await cr.SelectDirectorJobCodeListAsync(d_seq);

      if (AppIdentity.isExternal == 1)
      {
        CandidateEntityRepository cer = new CandidateEntityRepository();
        element_open_log eol = await cer.SelectDirectorOpenLogOneAsync(data.d_seq, AppIdentity.user_seq);
        if (eol == null)
        {
          data.is_external_lock = 1;
          if (!String.IsNullOrEmpty(data.email1))
            data.email1 = Regex.Replace(data.email1, "[a-zA-Z0-9]", "*");
          if (!String.IsNullOrEmpty(data.email2))
            data.email2 = Regex.Replace(data.email2, "[a-zA-Z0-9]", "*");

          if (!String.IsNullOrEmpty(data.phone))
            data.phone = Regex.Replace(data.phone, "[0-9]", "0");

          if (!String.IsNullOrEmpty(data.cell_phone))
            data.cell_phone = Regex.Replace(data.cell_phone, "[0-9]", "0");

        }
      }

      DirectorDetailViewModel model = new DirectorDetailViewModel()
      {
        data = data,
        schoolList = schoolList,
        careerList = careerList,
        DirectorList = outdirectorList,
        businessList = businessList,
        jobList = jobList
      };

      ViewBag.is_pop = is_pop;
      ViewBag.NoMenu = (is_pop == 1 ? true : false);
      ViewBag.Title = "C-Level : " + model.data.kor_name + "/" + "만 " + Utils.GetAge(model.data.birth_date.Value) + "세(" + Utils.ReturnGenderTxt(model.data.gender) + ")";

      return View(model);
    }

    /// <summary>
    /// 후보자 주의요망사항 업데이트
    /// </summary>
    /// <param name="c_seq"></param>
    /// <param name="caution_type"></param>
    /// <returns></returns>
    [HttpPost]
    public async Task<JsonResult> UpdateCandidateCautionType(int c_seq, int caution_type)
    {
      try
      {
        CandidateEntityRepository cer = new CandidateEntityRepository();
        var candidate = await cer.SelectCandidateOneAsync(c_seq);

        candidate.caution_type = caution_type;
        candidate.modify_dt = Utils.NowKorea();
        candidate.modify_seq = AppIdentity.user_seq;

        await cer.UpdateCandidateOneAsync(candidate, AppIdentity.user_seq, 1);

        return Json(new
        {
          ok = true,
          message = "후보자 주의 요망사항을 업데이트 했습니다."
        });
      }
      catch (Exception e)
      {
        return Json(new
        {
          ok = false,
          message = "후보자 주의 요망사항 업데이트 중 오류가 발생 했습니다."
        });
      }
    }

    /// <summary>
    /// 후보자 주의요망사항 업데이트 new
    /// </summary>
    /// <param name="c_seq"></param>
    /// <param name="caution_type"></param>
    /// <returns></returns>
    [HttpPost]
    public async Task<JsonResult> UpdateCandidateContact(int c_seq, string cell_phone, string phone, string email1, string email2, int wrong_phone2 = 0, int wrong_phone = 0)
    {
      try
      {
        CandidateEntityRepository cer = new CandidateEntityRepository();
        var candidate = await cer.SelectCandidateOneAsync(c_seq);

        candidate.cell_phone = cell_phone;
        candidate.phone = phone;
        candidate.email1 = email1;
        candidate.email2 = email2;
        candidate.wrong_phone = wrong_phone;
        candidate.wrong_phone2 = wrong_phone2;

        candidate.modify_dt = Utils.NowKorea();
        candidate.modify_seq = AppIdentity.user_seq;

        await cer.UpdateCandidateOneAsync(candidate, AppIdentity.user_seq, 1);

        //후보자 관련정보 저장 프로시저 실행.
        CandidateRepository cr = new CandidateRepository();
        var procMsg = await cr.SaveCandidateTxtMsg(candidate.c_seq);

        return Json(new
        {
          ok = true,
          message = "후보자 연락처를 업데이트 했습니다."
        });
      }
      catch (Exception e)
      {
        return Json(new
        {
          ok = false,
          message = "후보자 연락처 업데이트 중 오류가 발생 했습니다."
        });
      }
    }

    /// <summary>
    /// 후보자 주의요망사항 업데이트 new
    /// </summary>
    /// <param name="c_seq"></param>
    /// <param name="caution_type"></param>
    /// <returns></returns>
    [HttpPost]
    public async Task<JsonResult> UpdateCandidateAttentionType(int c_seq, int type, string remark)
    {
      try
      {
        CandidateEntityRepository cer = new CandidateEntityRepository();
        var candidate = await cer.SelectCandidateOneAsync(c_seq);

        can_activity cActivity = new can_activity();
        bool is_update = false;

        if (((candidate.attention_type ?? 0) == 0 && type == 0) || candidate.attention_type == type)
        {
          is_update = false;
        }
        else if (candidate.attention_type > 0 && type == 0)
        {
          cActivity.c_seq = candidate.c_seq;
          cActivity.cl_seq = 91;
          cActivity.memo = "{{{[ATTENTION]}}}<br/>주의요망사항 해당없음으로 변경 됨";
          cActivity.create_seq = AppIdentity.user_seq;
          cActivity.create_dt = Utils.NowKorea();

          is_update = true;
        }
        else
        {
          cActivity.c_seq = candidate.c_seq;
          cActivity.cl_seq = 91;
          cActivity.memo = "{{{[ATTENTION] " + Utils.ReturnAttentionTypeTxt(type) + "}}}<br/>" + remark;
          cActivity.create_seq = AppIdentity.user_seq;
          cActivity.create_dt = Utils.NowKorea();

          is_update = true;
        }


        if (type == 0)
        {
          remark = String.Empty;
        }

        candidate.attention_type = type;
        candidate.attention_remark = remark;
        candidate.attention_user = AppIdentity.user_seq;
        candidate.attention_date = Utils.NowKorea();
        candidate.modify_dt = Utils.NowKorea();
        candidate.modify_seq = AppIdentity.user_seq;

        await cer.UpdateCandidateOneAsync(candidate, AppIdentity.user_seq, 1);
        if (is_update)
        {
          await cer.CreateOrUpdateCanActivity(cActivity, CommonCodes.Create);
        }

        //후보자 관련정보 저장 프로시저 실행.
        //CandidateRepository cr = new CandidateRepository();
        //var procMsg = await cr.SaveCandidateTxtMsg(candidate.c_seq);

        /*
         
        can_activity cActivity = new can_activity();
          cActivity.c_seq = model.data.c_seq;
          cActivity.cl_seq = 1;
          cActivity.memo = "[Confidential]<br/>" + data.confi_remark;
          cActivity.create_seq = AppIdentity.user_seq;
          cActivity.create_dt = Utils.NowKorea();
          //await cer.CreateOrDeleteCanActivity(ca, CommonCodes.Create);
          activity_list.Add(cActivity);

         * */


        return Json(new
        {
          ok = true,
          message = "후보자 주의 요망사항을 업데이트 했습니다."
        });
      }
      catch (Exception e)
      {
        return Json(new
        {
          ok = false,
          message = "후보자 주의 요망사항 업데이트 중 오류가 발생 했습니다."
        });
      }
    }

    /// <summary>
    /// 후보자 컨피덴셜 사항 업데이트 new
    /// </summary>
    /// <param name="c_seq"></param>
    /// <param name="caution_type"></param>
    /// <returns></returns>
    [HttpPost]
    public async Task<JsonResult> UpdateCandidateConfi(int c_seq, int is_confidential, string confi_remark)
    {
      try
      {
        CandidateEntityRepository cer = new CandidateEntityRepository();
        var candidate = await cer.SelectCandidateOneAsync(c_seq);

        can_activity cActivity = new can_activity();
        bool is_update = false;

        if (((candidate.is_confidential ?? 0) == 0 && is_confidential == 0) || candidate.is_confidential == is_confidential)
        {
          is_update = false;
        }
        else if (candidate.is_confidential == 1 && is_confidential == 0)
        {
          cActivity.c_seq = candidate.c_seq;
          cActivity.cl_seq = 92;
          cActivity.memo = "{{{[CONFIDENTIAL]}}}<br/>Confidential 해제 됨";
          cActivity.create_seq = AppIdentity.user_seq;
          cActivity.create_dt = Utils.NowKorea();

          is_update = true;
        }
        else
        {
          cActivity.c_seq = candidate.c_seq;
          cActivity.cl_seq = 92;
          cActivity.memo = "{{{[CONFIDENTIAL]}}}<br/>" + confi_remark;
          cActivity.create_seq = AppIdentity.user_seq;
          cActivity.create_dt = Utils.NowKorea();

          is_update = true;
        }

        if (is_confidential == 0)
        {
          confi_remark = String.Empty;
        }

        candidate.is_confidential = is_confidential;
        candidate.confi_remark = confi_remark;
        candidate.confidential_user = AppIdentity.user_seq;
        candidate.confidential_date = Utils.NowKorea();
        candidate.modify_dt = Utils.NowKorea();
        candidate.modify_seq = AppIdentity.user_seq;

        await cer.UpdateCandidateOneAsync(candidate, AppIdentity.user_seq, 1);

        if (is_update)
        {
          await cer.CreateOrUpdateCanActivity(cActivity, CommonCodes.Create);
        }

        //후보자 관련정보 저장 프로시저 실행.
        //CandidateRepository cr = new CandidateRepository();
        //var procMsg = await cr.SaveCandidateTxtMsg(candidate.c_seq);


        return Json(new
        {
          ok = true,
          message = "후보자 Confidential 사항을 업데이트 했습니다."
        });
      }
      catch (Exception e)
      {
        return Json(new
        {
          ok = false,
          message = "후보자 Confidential 사항 업데이트 중 오류가 발생 했습니다."
        });
      }
    }

    /// <summary>
    /// 후보자 Inactive 사항 업데이트 new
    /// </summary>
    /// <param name="c_seq"></param>
    /// <param name="caution_type"></param>
    /// <returns></returns>
    [HttpPost]
    public async Task<JsonResult> UpdateCandidateInactive(int c_seq, int is_inactive, string inactive_remark)
    {
      try
      {
        CandidateEntityRepository cer = new CandidateEntityRepository();
        var candidate = await cer.SelectCandidateOneAsync(c_seq);

        can_activity cActivity = new can_activity();
        bool is_update = false;

        if (((candidate.is_inactive ?? 0) == 0 && is_inactive == 0) || candidate.is_inactive == is_inactive)
        {
          is_update = false;
        }
        else if (candidate.is_inactive == 1 && is_inactive == 0)
        {
          cActivity.c_seq = candidate.c_seq;
          cActivity.cl_seq = 93;
          cActivity.memo = "{{{[INACTIVE]}}}<br/>Inactive 해제 됨";
          cActivity.create_seq = AppIdentity.user_seq;
          cActivity.create_dt = Utils.NowKorea();

          is_update = true;
        }
        else
        {
          cActivity.c_seq = candidate.c_seq;
          cActivity.cl_seq = 93;
          cActivity.memo = "{{{[INACTIVE]}}}<br/>" + inactive_remark;
          cActivity.create_seq = AppIdentity.user_seq;
          cActivity.create_dt = Utils.NowKorea();

          is_update = true;
        }


        if (is_inactive == 0)
        {
          inactive_remark = String.Empty;
        }

        candidate.is_inactive = is_inactive;
        candidate.inactive_remark = inactive_remark;
        candidate.inactive_user = AppIdentity.user_seq;
        candidate.inactive_date = Utils.NowKorea();
        candidate.modify_dt = Utils.NowKorea();
        candidate.modify_seq = AppIdentity.user_seq;

        await cer.UpdateCandidateOneAsync(candidate, AppIdentity.user_seq, 1);

        if (is_update)
        {
          await cer.CreateOrUpdateCanActivity(cActivity, CommonCodes.Create);
        }

        //후보자 관련정보 저장 프로시저 실행.
        //CandidateRepository cr = new CandidateRepository();
        //var procMsg = await cr.SaveCandidateTxtMsg(candidate.c_seq);


        return Json(new
        {
          ok = true,
          message = "후보자 Confidential 사항을 업데이트 했습니다."
        });
      }
      catch (Exception e)
      {
        return Json(new
        {
          ok = false,
          message = "후보자 Confidential 사항 업데이트 중 오류가 발생 했습니다."
        });
      }
    }

    #endregion

    #region 후보자 활동내역 리스트 파샬.

    /// <summary>
    /// 후보자 활동내역 리스트 파샬
    /// </summary>
    /// <param name="c_seq"></param>
    /// <param name="page"></param>
    /// <returns></returns>
    public PartialViewResult CandidateActivityList(int c_seq, int page = 1)
    {
      int totalCount = 0;

      //한 페이지당 몇개의 행을 보여줄 것인지
      int pageSize = 1000;// AppPaging.PageLength20;


      CandidateRepository cr = new CandidateRepository();
      PrivacyAgreeSearchModel pa_search = new PrivacyAgreeSearchModel();
      pa_search.c_seq = c_seq;

      int totalCount2 = 0;

      var agreeList = cr.SelectPrivacyAgreeList(pa_search, 0, 10, out totalCount2);

      var list = cr.SelectCanActivityList(c_seq, (page - 1) * pageSize, pageSize, out totalCount);

      if (agreeList.Count() > 0)
      {
        foreach (var agrees in agreeList)
        {
          view_can_activity v = new view_can_activity();

          v.table_type = "AG";
          v.seq = agrees.pa_seq;
          v.c_seq = (agrees.c_seq.HasValue ? agrees.c_seq.Value : 0);
          v.ca_date = (agrees.agree_dt != null ? agrees.agree_dt : agrees.send_dt);
          v.create_seq = agrees.create_user;
          v.create_name = agrees.create_name;
          v.client_seq = agrees.client_seq;
          v.client_name = agrees.client_name;
          if (agrees.agree_gubun == 3)
          {
            v.memo = @"개인정보 수집 및 제3자 제공 동의서";
          }
          else if (agrees.agree_gubun == 1)
          {
            v.memo = @"개인정보 수집 동의서";
          }
          else if (agrees.agree_gubun == 1)
          {
            v.memo = @"개인정보 제 3자 제공 동의서";
          }
          v.send_dt = agrees.send_dt;
          v.agree_dt = agrees.agree_dt;
          v.is_agree = (agrees.agree_dt != null ? 1 : 0);
          v.create_dt = (agrees.agree_dt != null ? agrees.agree_dt : agrees.send_dt);

          list.Add(v);
        }
      }

      list = list.OrderByDescending(x => x.create_dt).ToList();


      CandidateActivityList model = new CandidateActivityList()
      {
        c_seq = c_seq
          ,
        activityList = list
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
    /// 후보자 활동내역 리스트 파샬
    /// </summary>
    /// <param name="c_seq"></param>
    /// <param name="page"></param>
    /// <returns></returns>
    public async Task<PartialViewResult> DirectorActivityList(int d_seq, int type = 0)
    {
      CandidateRepository cr = new CandidateRepository();

      var list = await cr.SelectDirectorActivityList(d_seq, type);

      return PartialView(list);
    }

    public async Task<PartialViewResult> CanProjectProgressList(int p_seq, int c_seq)
    {
      ProjectRepository pr = new ProjectRepository();
      int totalCount;
      var list = pr.SelectProjectRecommandHistoryListAsync(p_seq, 0, "A.state", "DESC", 0, 1000, out totalCount);
      ViewBag.this_c_seq = c_seq;
      return PartialView(list);
    }
    public async Task<PartialViewResult> CanProjectProgressHistoryList(int p_seq, int c_seq)
    {
      ProjectRepository pr = new ProjectRepository();
      var list = await pr.SelectProjectRecommandHistoryListAsync(p_seq, c_seq);

      return PartialView(list);
    }

    #endregion

    #region 후보자 메모(미사용 일듯..)

    /// <summary>
    /// 후보자 메모 리스트 파샬
    /// </summary>
    /// <param name="c_seq"></param>
    /// <param name="page"></param>
    /// <returns></returns>
    public PartialViewResult CandidateMemoList(int c_seq, int page = 1)
    {
      int totalCount = 0;

      //한 페이지당 몇개의 행을 보여줄 것인지
      int pageSize = AppPaging.PageLength5;


      CandidateRepository cr = new CandidateRepository();

      var list = cr.SelectCanMemoList(c_seq, (page - 1) * pageSize, pageSize, out totalCount);

      CandidateMemoList model = new CandidateMemoList()
      {
        c_seq = c_seq
          ,
        memoList = list
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

    #region 후보자 메모 등록
    /// <summary>
    /// 후보자 메모 등록
    /// </summary>
    /// <param name="c_seq"></param>
    /// <param name="memo"></param>
    /// <returns></returns>
    [HttpPost]
    public async Task<JsonResult> CreateCandidateActivity(can_activity data)
    {
      try
      {
        CandidateEntityRepository cer = new CandidateEntityRepository();

        can_activity ca = new can_activity()
        {
          c_seq = data.c_seq
            ,
          memo = data.memo
            ,
          ca_date = data.ca_date
            ,
          create_seq = AppIdentity.user_seq
            ,
          create_dt = Utils.NowKorea()
        };

        await cer.CreateOrUpdateCanActivity(ca, CommonCodes.Create);

        //후보자 관련정보 저장 프로시저 실행.
        CandidateRepository cr = new CandidateRepository();
        var procMsg = await cr.SaveCandidateTxtMsg(data.c_seq);

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
    /// 후보자 메모 등록
    /// </summary>
    /// <param name="c_seq"></param>
    /// <param name="memo"></param>
    /// <returns></returns>
    [HttpPost]
    [ValidateInput(false)]
    public async Task<JsonResult> CreateDirectorActivity(director_activity data)
    {
      try
      {
        CandidateEntityRepository cer = new CandidateEntityRepository();

        director_activity da;
        if (data.da_seq > 0)
        {
          da = await cer.SelectDirectorActivityOneAsync(data.da_seq);
          if (da != null)
          {
            da.memo_type = data.memo_type;
            da.memo = HttpUtility.UrlDecode(data.memo);
            da.modify_dt = Utils.NowKorea();
            da.modify_seq = AppIdentity.user_seq;

            await cer.CreateOrUpdateDirectorActivity(da, CommonCodes.Update);
          }

        }
        else
        {
          da = new director_activity()
          {
            d_seq = data.d_seq
            ,
            memo = HttpUtility.UrlDecode(data.memo)
            ,
            memo_type = data.memo_type
            ,
            ca_date = data.ca_date
            ,
            create_seq = AppIdentity.user_seq
            ,
            create_dt = Utils.NowKorea()
          ,
            modify_dt = Utils.NowKorea()
          ,
            modify_seq = AppIdentity.user_seq
          ,
          };
          await cer.CreateOrUpdateDirectorActivity(da, CommonCodes.Create);
        }

        //후보자 관련정보 저장 프로시저 실행.
        //CandidateRepository cr = new CandidateRepository();
        //var procMsg = await cr.SaveDirectorTxtMsg(data.d_seq);

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


    [HttpPost]
    public async Task<JsonResult> ModifyCandidateActivity(int ca_seq, string memo)
    {
      try
      {

        CandidateEntityRepository cer = new CandidateEntityRepository();

        var ca = await cer.SelectCanActivityOneAsync(ca_seq);
        if (ca == null)
          return Json(new
          {
            ok = false,
            message = "수정하려는 후보자 활동내역을 찾을 수 없습니다."
          });

        ca.memo = HttpUtility.UrlDecode(memo);
        ca.modify_dt = Utils.NowKorea();
        ca.modify_seq = AppIdentity.user_seq;

        await cer.CreateOrUpdateCanActivity(ca, CommonCodes.Update, AppIdentity.user_seq, 1);

        return Json(new
        {
          ok = true
            ,
          message = "후보자 활동내역을 수정 했습니다."
        });
      }
      catch (Exception e)
      {
        return Json(new
        {
          ok = false,
          message = "후보자 활동내역 수정 에러 : " + e.Message
        });
      }
    }

    [HttpPost]
    [ValidateInput(false)]
    public async Task<JsonResult> ModifyDirectorActivity(int da_seq, int memo_type, string memo)
    {
      try
      {

        CandidateEntityRepository cer = new CandidateEntityRepository();

        var da = await cer.SelectDirectorActivityOneAsync(da_seq);
        if (da == null)
          return Json(new
          {
            ok = false,
            message = "수정하려는 활동내역을 찾을 수 없습니다."
          });

        da.memo = HttpUtility.UrlDecode(memo);
        da.memo_type = memo_type;
        da.modify_dt = Utils.NowKorea();
        da.modify_seq = AppIdentity.user_seq;

        await cer.CreateOrUpdateDirectorActivity(da, CommonCodes.Update);

        return Json(new
        {
          ok = true
            ,
          message = "활동내역을 수정 했습니다."
        });
      }
      catch (Exception e)
      {
        return Json(new
        {
          ok = false,
          message = "활동내역 수정 에러 : " + e.Message
        });
      }
    }

    #endregion

    #region 후보자 메모 삭제
    /// <summary>
    /// 후보자 메모 삭제
    /// </summary>
    /// <param name="cm_seq"></param>
    /// <returns></returns>
    [HttpPost]
    public async Task<JsonResult> DeleteCandidateMemo(int ca_seq)
    {
      try
      {
        CandidateEntityRepository cer = new CandidateEntityRepository();

        var ca = await cer.SelectCanActivityOneAsync(ca_seq);

        if (ca == null)
          return Json(new
          {
            ok = false
              ,
            message = "삭제하려는 활동내역을 찾을 수 없습니다."
          });

        await cer.CreateOrUpdateCanActivity(ca, CommonCodes.Delete, AppIdentity.user_seq, 1);

        return Json(new
        {
          ok = true,
          message = "활동내역을 삭제 했습니다."
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
    public async Task<JsonResult> DeleteDirectorMemo(int da_seq)
    {
      try
      {
        CandidateEntityRepository cer = new CandidateEntityRepository();

        var da = await cer.SelectDirectorActivityOneAsync(da_seq);

        if (da == null)
          return Json(new
          {
            ok = false
              ,
            message = "삭제하려는 활동내역을 찾을 수 없습니다."
          });

        await cer.CreateOrUpdateDirectorActivity(da, CommonCodes.Delete);

        return Json(new
        {
          ok = true,
          message = "활동내역을 삭제 했습니다."
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

    #region 후보자 인터뷰(미사용일듯..)

    /// <summary>
    /// 후보자 인터뷰 리스트 파샬
    /// </summary>
    /// <param name="c_seq"></param>
    /// <param name="page"></param>
    /// <returns></returns>
    public PartialViewResult CandidateInterviewList(int c_seq, int page = 1)
    {
      int totalCount = 0;

      //한 페이지당 몇개의 행을 보여줄 것인지
      //int pageSize = 2;
      int pageSize = AppPaging.PageLength5;


      CandidateRepository cr = new CandidateRepository();

      var list = cr.SelectCanInterviewList(c_seq, (page - 1) * pageSize, pageSize, out totalCount);

      CandidateInterviewList model = new CandidateInterviewList()
      {
        c_seq = c_seq
          ,
        interviewList = list
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
    /// 후보자 인터뷰 등록
    /// </summary>
    /// <param name="c_seq"></param>
    /// <param name="memo"></param>
    /// <returns></returns>
    [HttpPost]
    public async Task<JsonResult> CreateCandidateInterview(int c_seq, string memo, string interview_dt)
    {
      try
      {
        CandidateEntityRepository cer = new CandidateEntityRepository();

        DateTime interviewDt = new DateTime();

        if (!DateTime.TryParse(interview_dt, out interviewDt))
          return Json(new
          {
            ok = false
              ,
            message = "인터뷰 일자가 올바르지 않습니다."
          });

        can_interview_sheet cis = new can_interview_sheet()
        {
          c_seq = c_seq
            ,
          memo = memo
            ,
          interview_dt = interviewDt
            ,
          create_seq = AppIdentity.user_seq
            ,
          create_dt = Utils.NowKorea()
        };

        await cer.CreateOrDeleteCanInterview(cis, CommonCodes.Create);

        //후보자 관련정보 저장 프로시저 실행.
        CandidateRepository cr = new CandidateRepository();
        var procMsg = await cr.SaveCandidateTxtMsg(c_seq);

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
    /// 후보자 인터뷰 삭제
    /// </summary>
    /// <param name="cis_seq"></param>
    /// <returns></returns>
    [HttpPost]
    public async Task<JsonResult> DeleteCandidateInterview(int cis_seq)
    {
      try
      {
        CandidateEntityRepository cer = new CandidateEntityRepository();

        var cis = await cer.SelectCanInterviewOneAsync(cis_seq);

        if (cis == null)
          return Json(new
          {
            ok = false
              ,
            message = "삭제하려는 인터뷰를 찾을 수 없습니다."
          });

        await cer.CreateOrDeleteCanInterview(cis, CommonCodes.Delete, AppIdentity.user_seq, 1);

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

    #endregion

    #region 후보자 이력서

    public async Task<PartialViewResult> CandidateResumeList(int c_seq, int show_remove = 0)
    {

      CandidateRepository cr = new CandidateRepository();

      var list = await cr.SelectCanResumeListAsync(c_seq, show_remove);

      CandidateResumeList model = new CandidateResumeList()
      {
        c_seq = c_seq
          ,
        resumeList = list

      };

      return PartialView(model);
    }

    /// <summary>
    /// 후보자 이력서 임시 업로드(검색엔진을 위해)
    /// </summary>
    /// <param name="c_seq"></param>
    /// <param name="file_type"></param>
    /// <param name="files"></param>
    /// <returns></returns>
    /*
    [HttpPost]
    public async Task<JsonResult> TempCandidateResumeUpload(HttpPostedFileBase korResume, HttpPostedFileBase engResume, HttpPostedFileBase etcResume)
    {
        try
        {
            FileUpload fiUpload = new FileUpload();
            string path = Server.MapPath("~/UploadedFiles");

            UploadFileResult kor_result = new UploadFileResult();
            if (korResume != null)
            {
                kor_result = fiUpload.UploadResumeTemp(path, korResume);

                if (!kor_result.status)
                    return Json(new
                    {
                        ok = false,
                        message = "국문 이력서 업로드 오류 - " + kor_result.statusMessage
                    });
            }


            UploadFileResult eng_result = new UploadFileResult();
            if (engResume != null)
            {
                eng_result = fiUpload.UploadResumeTemp(path, engResume);

                if (!eng_result.status)
                    return Json(new
                    {
                        ok = false,
                        message = "영문 이력서 업로드 오류 - " + kor_result.statusMessage
                    });
            }


            UploadFileResult etc_result = new UploadFileResult();
            if (etcResume != null)
            {
                etc_result = fiUpload.UploadResumeTemp(path, etcResume);

                if (!etc_result.status)
                    return Json(new
                    {
                        ok = false,
                        message = "기타 이력서 업로드 오류 - " + kor_result.statusMessage
                    });
            }

            return Json(new
            {
                ok = true
                ,
                kor_result = kor_result
                ,
                eng_result = eng_result
                ,
                etc_result = etc_result
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
    */

    [HttpPost]
    public async Task<JsonResult> UploadDirectorActivityFile(int d_seq, HttpPostedFileBase file)
    {
      try
      {
        FileUpload fiUpload = new FileUpload();
        string path = Server.MapPath("~/UploadedFiles");
        var rst = fiUpload.UploadDirectorActivity(path, "Director", d_seq.ToString(), file, true);
        //TODO : 검색엔진에서 파일을 읽어서 주요 정보 추출 프로세스 추가 예정

        if (!rst.status)
        {
          return Json(new
          {
            ok = false,
            message = rst.statusMessage
            //TODO : 추출한 주요 정보 JSON으로 전달 예정
          });
        }

        return Json(new
        {
          ok = true,
          message = "Upload Complete.",
          file_name = rst.name,
          file_ext = rst.extension,
          file_path = rst.dbPath
        });

      }
      catch (Exception e)
      {
        return Json(new
        {
          ok = false,
          message = "[Error]알 수 없는 오류가 발생 했습니다." + e.Message
        });
      }
    }
    [HttpPost]
    public async Task<JsonResult> CreateDirectorPictureFile(string uploadFolder, HttpPostedFileBase file)
    {
      try
      {
        FileUpload fiUpload = new FileUpload();
        string path = Server.MapPath("~/UploadedFiles");
        var rst = fiUpload.UploadImageTemp(path, "Director", "picture", file, true);
        //TODO : 검색엔진에서 파일을 읽어서 주요 정보 추출 프로세스 추가 예정

        CandidateResumeFilterResultModel file_rst = new CandidateResumeFilterResultModel();
        if (!rst.status)
        {
          return Json(new
          {
            ok = false,
            message = rst.statusMessage
            //TODO : 추출한 주요 정보 JSON으로 전달 예정
          });
        }

        return Json(new
        {
          ok = true,
          message = "Upload Complete.",
          img_path = rst.dbPath,
          file_rst = file_rst
        });

      }
      catch (Exception e)
      {
        return Json(new
        {
          ok = false,
          message = "[Error]알 수 없는 오류가 발생 했습니다." + e.Message
        });
      }
    }

    /// <summary>
    /// 후보자 이력서 등록(이현창)
    /// </summary>
    /// <param name="uploadFolder"></param>
    /// <param name="file"></param>
    /// <returns></returns>
    [HttpPost]
    public async Task<JsonResult> CreateTempResumeFile(string file_type, string uploadFolder, HttpPostedFileBase file)
    {
      try
      {
        FileUpload fiUpload = new FileUpload();
        string path = Server.MapPath("~/UploadedFiles");
        var rst = fiUpload.UploadTemp(path, "Temp", uploadFolder, file);

        string file_content = String.Empty;
        string openai_result = String.Empty;
        //TODO : 검색엔진에서 파일을 읽어서 주요 정보 추출 프로세스 추가 예정

        can_resume cr = new can_resume()
        {
          file_type = file_type,

          file_dir = rst.dbPath
            ,
          file_origin_path = rst.filePath
            ,
          file_path = rst.name
            ,
          file_extension = rst.extension
        };



        CandidateResumeFilterResultModel file_rst = new CandidateResumeFilterResultModel();
        if (!rst.status)
        {
          return Json(new
          {
            ok = false,
            message = rst.statusMessage
            //TODO : 추출한 주요 정보 JSON으로 전달 예정
          });
        }
        else
        {
          try
          {
            SearchEngineRepository ser = new SearchEngineRepository();
            string new_file_name = fiUpload.CopyTempFileForKMPDL(rst.filePath, Utils.ReturnUniqueValue(AppIdentity.user_seq, "F"));
            string source = "/media/uploadfiles/kmpdl/" + new_file_name;
            file_rst = await ser.GetResumeDuplicate(AppIdentity.user_seq, source);
            fiUpload.DeleteTempFileForKMPDL(new_file_name);
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


        return Json(new
        {
          ok = true,
          message = "Upload Complete.",
          result = cr,
          file_rst = file_rst
          //,          content = openai_result
        });

      }
      catch (Exception e)
      {
        return Json(new
        {
          ok = false,
          message = "[Error]알 수 없는 오류가 발생 했습니다." + e.Message
        });
      }
    }



    /// <summary>
    /// 후보자 이력서 등록
    /// </summary>
    /// <param name="c_seq"></param>
    /// <param name="file_type"></param>
    /// <param name="files"></param>
    /// <returns></returns>
    [HttpPost]
    public async Task<JsonResult> CreateCandidateResume(int c_seq, string file_type, HttpPostedFileBase file)
    {
      try
      {
        FileUpload fiUpload = new FileUpload();
        string path = Server.MapPath("~/UploadedFiles");
        var result = fiUpload.UploadResume(c_seq, path, file);

        if (!result.status)
          return Json(new
          {
            ok = false,
            message = result.statusMessage
          });

        CandidateEntityRepository cer = new CandidateEntityRepository();

        can_resume cr = new can_resume()
        {
          c_seq = c_seq
            ,
          file_type = file_type
            ,
          file_dir = result.dbPath
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

        await cer.CreateOrDeleteCanResume(cr, CommonCodes.Create);

        var can_date = await cer.SelectCandidateOneAsync(c_seq);
        if (can_date != null)
        {

          can_date.modify_dt = Utils.NowKorea();
          can_date.modify_seq = AppIdentity.user_seq;

          await cer.UpdateCandidateOneAsync(can_date, AppIdentity.user_seq, 1);
        }
        //후보자 관련정보 저장 프로시저 실행.
        CandidateRepository canRepo = new CandidateRepository();
        var procMsg = await canRepo.SaveCandidateTxtMsg(c_seq);

        return Json(new
        {
          ok = true,
          message = "이력서를 등록 했습니다.",
          result = cr
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
    /// 후보자 이력서 삭제
    /// </summary>
    /// <param name="cr_seq"></param>
    /// <param name="c_seq"></param>
    /// <param name="fileName"></param>
    /// <returns></returns>
    [HttpPost]
    public async Task<JsonResult> DeleteCandidateResume(int cr_seq)
    {
      try
      {
        CandidateEntityRepository cer = new CandidateEntityRepository();

        var cr = await cer.SelectCanResumeOneAsync(cr_seq);

        if (cr == null)
          return Json(new
          {
            ok = false
              ,
            message = "삭제하려는 이력서를 찾을 수 없습니다."
          });
        /*
                        FileUpload fiUpload = new FileUpload();

                        string path = Server.MapPath("~/UploadedFiles");

                        //실 파일 삭제
                        if (!fiUpload.DeleteFile(cr.file_origin_path))
                            return Json(new
                            {
                                ok = false
                                ,
                                message = "경로에서 삭제하려는 이력서를 찾을 수 없습니다."
                            });
        */
        cr.remove_dt = Utils.NowKorea();
        cr.remove_user = AppIdentity.user_seq;
        await cer.CreateOrDeleteCanResume(cr, CommonCodes.Delete, AppIdentity.user_seq, 1);

        return Json(new
        {
          ok = true,
          message = "이력서를 삭제 했습니다."
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
    /// 후보자 임시 이력서 삭제
    /// </summary>
    /// <param name="file_origin_path"></param>
    /// <returns></returns>
    [HttpPost]
    public async Task<JsonResult> DeleteCandidateTempResume(string file_origin_path)
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
            message = "경로에서 삭제하려는 이력서를 찾을 수 없습니다."
          });

        return Json(new
        {
          ok = true,
          message = "이력서를 삭제 했습니다."
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

    #region 후보자 생성/수정
    /// <summary>
    /// 후보자 생성화면
    /// </summary>
    /// <param name="c_seq">후보자 seq (candidate)</param>
    /// <param name="sc_seq">간편후보자 seq (simple candidate)</param>
    /// <returns></returns>
    public async Task<ActionResult> CandidateCreateFromMail(CandidateMailResumeListModel data)
    {

      string uploadTmpFolder = Utils.ReturnUniqueValue(AppIdentity.user_seq, "M");

      CandidateRepository cr = new CandidateRepository();

      var m_resume = await cr.SelectMailResumeOneASync(data.new_mail_key);

      if (m_resume == null)
      {
        m_resume = new mail_resume();
      }
      else
      {
        m_resume.dv_snd_name = (m_resume.dv_snd_name == "사람인" ? "" : m_resume.dv_snd_name);
        m_resume.dv_snd_addr = (m_resume.dv_snd_addr == "webmaster@saramin.co.kr" ? "" : m_resume.dv_snd_addr);
      }


      //var korResume = await cr.SelectLaseCanResume(c_seq, "K");
      //var engResume = await cr.SelectLaseCanResume(c_seq, "E");
      //var etcResume = await cr.SelectLaseCanResume(c_seq, "O");

      var schoolList = new List<can_school>();
      var careerList = new List<can_career>();
      var placeList = new List<can_place>();
      var jobList = new List<can_job>();
      var busiList = new List<can_business>();
      var foreignList = new List<can_foreign_lan>();
      var resumeList = new List<can_resume>();

      //간편후보자 등록에서 넘어온 경우
      FileUpload fi = new FileUpload();
      //이력서 임시파일 정식으로 이동
      foreach (var resume in data.resumes)
      {
        if (!String.IsNullOrEmpty(resume.resume_type) && resume.resume_type != "NONE")
        {
          var mr_file = await cr.SelectMailResumeFileOneASync(data.new_mail_key, resume.dn_seq);
          if (mr_file != null)
          {
            var rst = fi.CopyFile(mr_file.file_path, Path.Combine(Server.MapPath("~/UploadedFiles"), "Temp/" + uploadTmpFolder + "/"));
            if (rst.status)
            {
              resumeList.Add(new can_resume()
              {
                file_type = resume.resume_type
              ,
                file_dir = "/UploadedFiles/Temp/" + uploadTmpFolder + "/" + rst.name
              ,
                file_origin_path = rst.filePath
              ,
                file_path = rst.name
              ,
                file_extension = rst.extension
              });
            }

          }
        }
      }



      CandidateCreateModel model = new CandidateCreateModel()
      {
        mail_key = data.new_mail_key
          ,
        kor_name = (!String.IsNullOrEmpty(m_resume.dv_snd_name) && Utils.Chk_Han_Eng(m_resume.dv_snd_name)) ? m_resume.dv_snd_name : ""
          ,
        eng_name = (!String.IsNullOrEmpty(m_resume.dv_snd_name) && !Utils.Chk_Han_Eng(m_resume.dv_snd_name)) ? m_resume.dv_snd_name : ""
          ,
        manager_seq = m_resume.manager_seq
          ,
        manager_name = m_resume.manager_name
          ,
        country_code = "KR"
          ,
        country_name = "한국"
          ,
        email1 = m_resume.dv_snd_addr
          ,
        schoolList = schoolList
          ,
        companyList = careerList
          ,
        placeList = placeList
          ,
        jobList = jobList
          ,
        busiList = busiList
          ,
        foreignList = foreignList
          ,
        resumeList = resumeList

      };
      ViewBag.uploadFolder = uploadTmpFolder;

      ViewBag.Title = "Candidate Registration";


      return View("CandidateCreate", model);
      //return View();
    }

    [HttpPost]
    public async Task<ContentResult> ResumeGptCall(List<GptMessage> messages)
    {
      try
      {
        string contentType = Request.Headers["Content-Type"]; // Content-Type 헤더 값 가져오기
        string authorization = Request.Headers["Authorization"]; // Authorization 헤더 값 가져오기
        string apiKey = "";
        if (!string.IsNullOrEmpty(authorization) && authorization.StartsWith("Bearer "))
        {
          // "Bearer " 이후의 부분을 추출
          apiKey = authorization.Substring("Bearer ".Length);
        }
        else
        {
          // Authorization 헤더가 없거나 Bearer로 시작하지 않으면 처리할 로직
          apiKey = "YOUR_OPENAI_API_KEY";
        }
        //const makeupApiKey = "";
        GPTServer gpt = new GPTServer(apiKey);
        var result = await gpt.GetCompletionAsyncRaw(messages, "gpt-4.1-mini");
        var settings = new JsonSerializerSettings
        {
          ContractResolver = new CamelCasePropertyNamesContractResolver(),
          Formatting = Newtonsoft.Json.Formatting.Indented // JSON 예쁘게 출력
        };

        string jsonResponse = JsonConvert.SerializeObject(result);

        //return Json(jsonResponse, JsonRequestBehavior.AllowGet); ;
        return Content(jsonResponse, "application/json");
      }
      catch (Exception e)
      {
        // 오류 처리 후 ContentResult로 반환
        var errorResponse = new
        {
          ok = false,
          message = e.Message
        };

        // JSON 문자열로 직렬화하여 반환
        return Content(JsonConvert.SerializeObject(errorResponse), "application/json");
      }
    }


    #region AI프론트엔드용
    public async Task<ActionResult> CandidateAi()
    {
      return View();
    }
    public async Task<ActionResult> CandidateAi2()
    {
      return View();
    }

    public async Task<ActionResult> CandidateAi2025()
    {
      return View();
    }

    public async Task<ActionResult> CandidateAi_v2()
    {
      return View();
    }

    static bool IsHangul(char c)
    {
      return c >= '\uAC00' && c <= '\uD7A3';
    }


    static bool IsMostlyHangul(string text)
    {
      string input = Regex.Replace(text, @"[^가-힣A-z\s]", "");

      int hangulCount = input.Count(c => IsHangul(c));
      int totalCount = input.Length;

      return (double)hangulCount / totalCount > 0.3;
    }

    //AI분석을 위한 파일 업로드
    [HttpPost]
    public async Task<ActionResult> ResumeUploadGetContents(HttpPostedFileBase file)
    {
      try
      {
        FileUpload fiUpload = new FileUpload();
        SearchEngineRepository ser = new SearchEngineRepository();
        string path = Server.MapPath("~/UploadedFiles");
        string uploadTmpFolder = Utils.ReturnUniqueValue(AppIdentity.user_seq);
        var rst = fiUpload.UploadTemp(path, "Temp", uploadTmpFolder, file);

        can_resume cr = new can_resume()
        {
          //file_type = file_type,

          file_dir = rst.dbPath
            ,
          file_origin_path = rst.filePath
            ,
          file_path = rst.name
            ,
          file_extension = rst.extension
        };

        if (!rst.status)
        {
          throw (new Exception(rst.statusMessage));
        }

        string file_content = String.Empty;
        //FileUpload fiUpload = new FileUpload();

        var creator = new GPTController().GetCreatorFromPdf(cr.file_origin_path);

        if (cr.file_extension == ".pdf" && creator != "react-pdf")
        {
          file_content = new GPTController().ExtractTextFromPdf(cr.file_origin_path);
        }
        else
        {
#if DEBUG
          string source = "/media/uploadfiles/kmpdl/20250807.docx";
#else
          string new_file_name = fiUpload.CopyTempFileForKMPDL(rst.filePath, Utils.ReturnUniqueValue(AppIdentity.user_seq, "F"));
          string source = "/media/uploadfiles/kmpdl/" + new_file_name;
#endif


          //string source = "/media/uploadfiles/kmpdl/20231115.docx";
          file_content = ser.TempResumeCheck(source);
#if DEBUG

#else
          fiUpload.DeleteTempFileForKMPDL(new_file_name);
#endif
        }
        if (IsMostlyHangul(file_content))
        {
          cr.file_type = "K";
        }
        else
        {
          cr.file_type = "E";
        }


        return Json(new
        {
          ok = true,
          message = "Upload Complete.",
          result = cr,
          file_content = file_content,
          temp_folder = uploadTmpFolder
        });

      }
      catch (Exception e)
      {
        return Json(new
        {
          ok = false,
          message = "[Error]알 수 없는 오류가 발생 했습니다." + e.Message
        });
      }

    }

    #endregion
    /*
    public async Task<ActionResult> CandidateAiUpload()
    {
      return View();
    }
    */


    /*
    //AI분석을 위한 파일 업로드
    [HttpPost]
    public async Task<ActionResult> CreateAiResumeUpload(HttpPostedFileBase file)
    {
      try
      {
        FileUpload fiUpload = new FileUpload();
        string path = Server.MapPath("~/UploadedFiles");
        string uploadTmpFolder = Utils.ReturnUniqueValue(AppIdentity.user_seq);
        var rst = fiUpload.UploadTemp(path, "Temp", uploadTmpFolder, file);

        can_resume cr = new can_resume()
        {
          //file_type = file_type,

          file_dir = rst.dbPath
            ,
          file_origin_path = rst.filePath
            ,
          file_path = rst.name
            ,
          file_extension = rst.extension
        };

        if (!rst.status)
        {
          throw (new Exception(rst.statusMessage));
        }

        return Json(new
        {
          ok = true,
          message = "Upload Complete.",
          result = cr,
          temp_folder = uploadTmpFolder
        });

      }
      catch (Exception e)
      {
        return Json(new
        {
          ok = false,
          message = "[Error]알 수 없는 오류가 발생 했습니다." + e.Message
        });
      }

    }
    */
    /*
    public async Task<JsonResult> CandidateGetFileContents(can_resume can_file_info, string uploadFolder)
    {
      SearchEngineRepository ser = new SearchEngineRepository();

      string file_content = String.Empty;
      string ai_result = String.Empty;
      try
      {
        if (String.IsNullOrEmpty(uploadFolder))
        {
          throw new Exception("업로드 폴더 정보가 없습니다.");
        }

        if (can_file_info == null)
        {
          throw new Exception("파일 정보가 없습니다.");
        }
        FileUpload fiUpload = new FileUpload();
        string new_file_name = fiUpload.CopyTempFileForKMPDL(can_file_info.file_origin_path, Utils.ReturnUniqueValue(AppIdentity.user_seq, "F"));
        string source = "/media/uploadfiles/kmpdl/" + new_file_name;
        //string source = "/media/uploadfiles/kmpdl/20231115.docx";
        file_content = ser.TempResumeCheck(source);

        fiUpload.DeleteTempFileForKMPDL(new_file_name);

        return Json(new
        {
          ok = true,
          contents = file_content,
        });
      }
      catch (Exception e)
      {
        return Json(new
        {
          ok = false,
          message = "[Error]알 수 없는 오류가 발생 했습니다." + e.Message
        });
      }
    }
    */
    /*
    public async Task<JsonResult> CandidateAiRecognizing(can_resume can_file_info, string uploadFolder)
    {
      SearchEngineRepository ser = new SearchEngineRepository();

      string file_content = String.Empty;
      string ai_result = String.Empty;
      try
      {
        if (String.IsNullOrEmpty(uploadFolder))
        {
          throw new Exception("업로드 폴더 정보가 없습니다.");
        }

        if (can_file_info == null)
        {
          throw new Exception("파일 정보가 없습니다.");
        }
        FileUpload fiUpload = new FileUpload();
        string new_file_name = fiUpload.CopyTempFileForKMPDL(can_file_info.file_origin_path, Utils.ReturnUniqueValue(AppIdentity.user_seq, "F"));
        string source = "/media/uploadfiles/kmpdl/" + new_file_name;
        //string source = "/media/uploadfiles/kmpdl/20231115.docx";
        file_content = ser.TempResumeCheck(source);
        if (!String.IsNullOrWhiteSpace(file_content))
        {
          //if (file_content.Length > 1500)
          //  file_content = file_content.Substring(0, 1500);

          var openai_result = await GetResumeSummarFromOpenAi(file_content);
          ai_result = openai_result.ToString();
          //JObject jObject = JObject.Parse(openai_result.ToString());
        }

        return Json(new
        {
          ok = true,
          contents = file_content,
          summary  = ai_result
        });
      } 
      catch (Exception e)
      {
        return Json(new
        {
          ok = false,
          message = "[Error]알 수 없는 오류가 발생 했습니다." + e.Message
        });
      }
    }
    */
    public ActionResult CandidateAiCreate(string candidate_model, string uploadFolder)
    {
      if (String.IsNullOrEmpty(candidate_model))
      {
        throw new Exception("이력서 분석 정보가 없습니다.");
      }

      if (String.IsNullOrEmpty(uploadFolder))
      {
        throw new Exception("업로드 폴더 정보가 없습니다.");
      }
      //JObject jobject = JObject.Parse(candidate_model);
      var model = JsonConvert.DeserializeObject<CandidateCreateModel>(candidate_model);

      model.manager_seq = AppIdentity.ud_seq != 4 ? AppIdentity.user_seq : 0;
      model.manager_name = AppIdentity.ud_seq != 4 ? AppIdentity.name : "";
      model.is_ai_reg = 1;

      ViewBag.uploadFolder = uploadFolder;
      ViewBag.Title = "Candidate Registration";


      return View("CandidateCreate", model);
    }
    public async Task<ActionResult> CandidateCreateFromMailAI(CandidateMailResumeListModel data, int c_seq = 0)
    {

      string uploadTmpFolder = Utils.ReturnUniqueValue(AppIdentity.user_seq, "M");

      CandidateRepository cr = new CandidateRepository();

      var m_resume = await cr.SelectMailResumeOneASync(data.new_mail_key);
      var gpt_data = await cr.SelectMailGPTOneASync(data.new_mail_key);

      if (m_resume == null)
      {
        m_resume = new mail_resume();
      }
      else
      {
        m_resume.dv_snd_name = (m_resume.dv_snd_name == "사람인" ? "" : m_resume.dv_snd_name);
        m_resume.dv_snd_addr = (m_resume.dv_snd_addr == "webmaster@saramin.co.kr" ? "" : m_resume.dv_snd_addr);
      }

      string candidate_model = gpt_data.gpt_result;



      //신규 후보자이면 산업직무 제안 가져오기
      //if (c_seq == 0)
      var resumeList = new List<can_resume>();

      FileUpload fi = new FileUpload();
      //이력서 임시파일 정식으로 이동
      foreach (var resume in data.resumes)
      {
        if (!String.IsNullOrEmpty(resume.resume_type) && resume.resume_type != "NONE")
        {
          var mr_file = await cr.SelectMailResumeFileOneASync(data.new_mail_key, resume.dn_seq);
          if (mr_file != null)
          {
            var rst = fi.CopyFile(mr_file.file_path, Path.Combine(Server.MapPath("~/UploadedFiles"), "Temp/" + uploadTmpFolder + "/"));
            if (rst.status)
            {
              resumeList.Add(new can_resume()
              {
                c_seq = c_seq
                ,
                file_type = resume.resume_type
              ,
                file_dir = "/UploadedFiles/Temp/" + uploadTmpFolder + "/" + rst.name
              ,
                file_origin_path = rst.filePath
              ,
                file_path = rst.name
              ,
                file_extension = rst.extension
              });
            }

          }
        }
      }

      if (c_seq == 0)
      {
        CandidateCreateModel model = new CandidateCreateModel();
        if (!String.IsNullOrEmpty(candidate_model))
        {
          model = JsonConvert.DeserializeObject<CandidateCreateModel>(candidate_model);
        }

        model.resumeList = resumeList;
        model.mail_key = data.new_mail_key;
        if (String.IsNullOrEmpty(model.kor_name))
        {
          model.kor_name = (!String.IsNullOrEmpty(m_resume.dv_snd_name) && Utils.Chk_Han_Eng(m_resume.dv_snd_name)) ? m_resume.dv_snd_name : "";
        }
        if (String.IsNullOrEmpty(model.eng_name))
        {
          model.eng_name = (!String.IsNullOrEmpty(m_resume.dv_snd_name) && !Utils.Chk_Han_Eng(m_resume.dv_snd_name)) ? m_resume.dv_snd_name : "";
        }

        model.manager_seq = m_resume.manager_seq;
        model.manager_name = m_resume.manager_name;
        if (String.IsNullOrEmpty(model.email1))
        {
          model.email1 = m_resume.dv_snd_addr;
        }
        model.is_ai_reg = 1;

        if (model.country_name == "한국" && String.IsNullOrEmpty(model.country_code))
        {
          model.country_code = "KR";
        }

        if (model.schoolList.Count > 1)
        {
          model.schoolList.RemoveAll(x => x.gubun == "1");
        }
        model.foreignList.Clear();
        try
        {
          //제안받은 산업 직무코드가 오류나는경우가 있어서 실제 코드와 매칭하여 매칭된것만 표기하기
          
          ApiRepository ar = new ApiRepository();
          string busi_model = gpt_data.gpt_result_busi;
          string job_model = gpt_data.gpt_result_job;
          if (!String.IsNullOrEmpty(busi_model))
          {
            var busi = JsonConvert.DeserializeObject<GptBusiness>(busi_model);
            var all_busi_list = await ar.SelectBusinessCode2ListAsyncAll();
            var sugg_busi = new List<can_business>();
            foreach (var business in busi.business)
            {
              double code1_left3;
              if (!double.TryParse(business.code1.ToString().Substring(0, 3), out code1_left3))
              {
                code1_left3 = business.code1;
              }
              code_business_dtl new_business = all_busi_list.Where(x => x.code1 == code1_left3 && x.code2 == business.code2).FirstOrDefault();
              if (new_business == null)
              {
                continue;
              }
              can_business cb = new can_business()
              {
                c_seq = 0,
                code1 = new_business.code1,
                code2 = new_business.code2,
                code_name1 = new_business.code_name1,
                code_name2 = new_business.code_name2,
                reason = business.reason
              };
              sugg_busi.Add(cb);
            }
            model.gpt_busiList = sugg_busi;
          }

          if (!String.IsNullOrEmpty(job_model))
          {
            var jobs = JsonConvert.DeserializeObject<GptJob>(job_model);
            var all_job_list = await ar.SelectJobCode2ListAsyncAll();
            var sugg_job = new List<can_job>();
            foreach (var job in jobs.job)
            {
              double code1_left3;
              if(!double.TryParse(job.code1.ToString().Substring(0, 3), out code1_left3))
              {
                code1_left3 = job.code1;
              }
              code_job_dtl new_job = all_job_list.Where(x => x.code1 == code1_left3 && x.code2 == job.code2).FirstOrDefault();
              if (new_job == null)
              {
                continue;
              }
              can_job cj = new can_job()
              {
                c_seq = 0,
                code1 = new_job.code1,
                code2 = new_job.code2,
                code_name1 = new_job.code_name1,
                code_name2 = new_job.code_name2,
                reason = job.reason
              };
              sugg_job.Add(cj);
            }

            model.gpt_jobList = sugg_job;
          }

        }
        catch
        {

        }

        ViewBag.uploadFolder = uploadTmpFolder;
        ViewBag.Title = "Candidate Registration";
        return View("CandidateCreate", model);

      }
      else
      {
        CandidateAiUpdateModel new_cand = new CandidateAiUpdateModel();
        if (!String.IsNullOrEmpty(candidate_model))
        {
          new_cand = JsonConvert.DeserializeObject<CandidateAiUpdateModel>(candidate_model);
        }
        if (new_cand.schoolList.Count > 1)
        {
          new_cand.schoolList.RemoveAll(x => x.gubun == "1");
        }

        var candidate = await cr.SelectCandidateOneAsync(c_seq);
        if (candidate == null)
        {
          throw new Exception("업데이트 할 후보자 정보가 없습니다.");
        }

        var schoolList = await cr.SelectCanSchoolListAsync(c_seq);
        var careerList = await cr.SelectCanCareerListAsync(c_seq);
        var placeList = await cr.SelectCanPlaceListAsync(c_seq);
        var jobList = await cr.SelectCanJobCodeListAsync(c_seq);
        var busiList = await cr.SelectCanBusiCodeListAsync(c_seq);
        var foreignList = await cr.SelectCanForeignLanListAsync(c_seq);

        CandidateAiUpdateModel model = new CandidateAiUpdateModel()
        {
          c_seq = candidate.c_seq
          ,
          mail_key = data.new_mail_key
          ,
          kor_name = candidate.kor_name
          ,
          manager_seq = candidate.manager_seq
          ,
          manager_name = candidate.manager_name
          ,
          eng_name = !String.IsNullOrWhiteSpace(candidate.eng_name) ? candidate.eng_name : new_cand.eng_name
          ,
          is_foreign = candidate.is_foreign
          ,
          birth_date = candidate.birth_date
          ,
          gender = candidate.gender
          ,
          ex_birthdate = candidate.ex_birthdate
          ,
          country_code = candidate.country_code
          ,
          country_name = candidate.country_name
          ,
          addr1 = !String.IsNullOrWhiteSpace(candidate.addr1) ? candidate.addr1 : new_cand.addr1
          ,
          ex_addr = candidate.ex_addr
          ,
          phone = !String.IsNullOrWhiteSpace(candidate.phone) && candidate.phone != "0000" ? candidate.phone : new_cand.phone
          ,
          wrong_phone = candidate.wrong_phone
          ,
          cell_phone = !String.IsNullOrWhiteSpace(candidate.cell_phone) && candidate.cell_phone != "0000" ? candidate.cell_phone : new_cand.cell_phone
          ,
          wrong_phone2 = candidate.wrong_phone2
          ,
          email1 = !String.IsNullOrWhiteSpace(candidate.email1) && candidate.email1 != "no@email" ? candidate.email1 : new_cand.email1
          ,
          email2 = !String.IsNullOrWhiteSpace(candidate.email2) && candidate.email2 != "no@email" ? candidate.email2 : new_cand.email2
          ,
          sns_link1 = !String.IsNullOrWhiteSpace(candidate.sns_link1) ? candidate.sns_link1 : new_cand.sns_link1
          ,
          sns_link2 = !String.IsNullOrWhiteSpace(candidate.sns_link2) ? candidate.sns_link2 : new_cand.sns_link2
          ,
          sns_link3 = !String.IsNullOrWhiteSpace(candidate.sns_link3) ? candidate.sns_link3 : new_cand.sns_link3
          //,
          //hope_salary = candidate.hope_salary
          //,
          //hope_salary2 = candidate.hope_salary2
          ,
          reg_type = candidate.reg_type
          ,
          keyword = candidate.keyword
          ,
          is_confidential = candidate.is_confidential
          ,
          is_inactive = candidate.is_inactive
          ,
          confi_remark = candidate.confi_remark
          ,
          inactive_remark = candidate.inactive_remark
          ,
          schoolList = schoolList
          ,
          companyList = careerList
          ,
          placeList = placeList
          ,
          jobList = jobList
          ,
          busiList = busiList
          ,
          foreignList = foreignList
          ,
          resumeList = resumeList
        };

        model.new_cand = new_cand;
        model.is_ai_reg = 1;



        ViewBag.uploadFolder = uploadTmpFolder;

        ViewBag.Title = "Candidate Update(" + candidate.kor_name + ")";


        return View("CandidateCreateAIUpdate", model);
      }




    }

    public async Task<ActionResult> CandidateCreateAiUpdate(int c_seq, string candidate_model, string uploadFolder)
    {
      if (c_seq == 0)
      {
        throw new Exception("업데이트 할 후보자 정보가 없습니다.");
      }

      if (String.IsNullOrEmpty(candidate_model))
      {
        throw new Exception("이력서 분석 정보가 없습니다.");
      }

      if (String.IsNullOrEmpty(uploadFolder))
      {
        throw new Exception("업로드 폴더 정보가 없습니다.");
      }

      CandidateAiUpdateModel new_cand = JsonConvert.DeserializeObject<CandidateAiUpdateModel>(candidate_model);


      //JObject jobject = JObject.Parse(candidate_model);
      CandidateRepository cr = new CandidateRepository();


      var data = await cr.SelectCandidateOneAsync(c_seq);
      if (data == null)
      {
        throw new Exception("업데이트 할 후보자 정보가 없습니다.");
      }

      var schoolList = await cr.SelectCanSchoolListAsync(c_seq);
      var careerList = await cr.SelectCanCareerListAsync(c_seq);
      var placeList = await cr.SelectCanPlaceListAsync(c_seq);
      var jobList = await cr.SelectCanJobCodeListAsync(c_seq);
      var busiList = await cr.SelectCanBusiCodeListAsync(c_seq);
      var foreignList = await cr.SelectCanForeignLanListAsync(c_seq);

      CandidateAiUpdateModel model = new CandidateAiUpdateModel()
      {
        c_seq = data.c_seq
          ,
        // mail_key = new_mail_key
        //   ,
        kor_name = data.kor_name
          ,
        manager_seq = data.manager_seq
          ,
        manager_name = data.manager_name
          ,
        eng_name = !String.IsNullOrWhiteSpace(data.eng_name) ? data.eng_name : new_cand.eng_name
          ,
        is_foreign = data.is_foreign
          ,
        birth_date = data.birth_date
          ,
        gender = data.gender
          ,
        ex_birthdate = data.ex_birthdate
          ,
        country_code = data.country_code
          ,
        country_name = data.country_name
          ,
        addr1 = !String.IsNullOrWhiteSpace(data.addr1) ? data.addr1 : new_cand.addr1
          ,
        ex_addr = data.ex_addr
          ,
        phone = !String.IsNullOrWhiteSpace(data.phone) && data.phone != "0000" ? data.phone : new_cand.phone
          ,
        wrong_phone = data.wrong_phone
          ,
        cell_phone = !String.IsNullOrWhiteSpace(data.cell_phone) && data.cell_phone != "0000" ? data.cell_phone : new_cand.cell_phone
          ,
        wrong_phone2 = data.wrong_phone2
          ,
        email1 = !String.IsNullOrWhiteSpace(data.email1) && data.email1 != "no@email" ? data.email1 : new_cand.email1
          ,
        email2 = !String.IsNullOrWhiteSpace(data.email2) && data.email2 != "no@email" ? data.email2 : new_cand.email2
          ,
        sns_link1 = !String.IsNullOrWhiteSpace(data.sns_link1) ? data.sns_link1 : new_cand.sns_link1
          ,
        sns_link2 = !String.IsNullOrWhiteSpace(data.sns_link2) ? data.sns_link2 : new_cand.sns_link2
          ,
        sns_link3 = !String.IsNullOrWhiteSpace(data.sns_link3) ? data.sns_link3 : new_cand.sns_link3
          ,
        hope_salary = data.hope_salary
          ,
        hope_salary2 = data.hope_salary2
          ,
        reg_type = data.reg_type
          ,
        keyword = data.keyword
          ,
        is_confidential = data.is_confidential
          ,
        is_inactive = data.is_inactive
          ,
        confi_remark = data.confi_remark
          ,
        inactive_remark = data.inactive_remark
          ,
        schoolList = schoolList
          ,
        companyList = careerList
          ,
        placeList = placeList
          ,
        jobList = jobList
          ,
        busiList = busiList
          ,
        foreignList = foreignList
          ,
        resumeList = new_cand.resumeList
      };

      model.new_cand = new_cand;
      model.is_ai_reg = 1;

      ViewBag.uploadFolder = uploadFolder;
      ViewBag.Title = "Candidate Registration";


      return View(model);
    }



    /// <summary>
    /// 후보자 생성화면
    /// </summary>
    /// <param name="c_seq">후보자 seq (candidate)</param>
    /// <param name="sc_seq">간편후보자 seq (simple candidate)</param>
    /// <returns></returns>
    public async Task<ActionResult> CandidateCreate(int c_seq = 0, int sc_seq = 0, int t_seq = 0, int is_pop = 0)
    {
      if (AppIdentity.isExternal == 1 && c_seq != 0)
      {
        CandidateEntityRepository cer = new CandidateEntityRepository();
        element_open_log eol = await cer.SelectCanOpenLogOneAsync(c_seq, AppIdentity.user_seq);
        if (eol == null)
        {
          return RedirectToAction("CandidateDetail", "Candidate", new { c_seq = c_seq });
        }
      }

      string uploadTmpFolder = Utils.ReturnUniqueValue(AppIdentity.user_seq);

      CandidateRepository cr = new CandidateRepository();

      candidate data;
      if (t_seq != 0)
      {
        data = await cr.TempSelectCandidateOneAsync(t_seq);

        if (data != null)
        {
          data.tempsaved_seq = data.c_seq;
          data.c_seq = 0;
        }

      }
      else
      {
        data = await cr.SelectCandidateOneAsync(c_seq);
        if (data != null)
        {
          if (data.is_confidential == 1 && String.IsNullOrEmpty(data.confi_remark))
          {
            data.confi_remark = " ";
          }

          if (data.is_inactive == 1 && String.IsNullOrEmpty(data.inactive_remark))
          {
            data.confi_remark = " ";
          }
        }

      }

      if (data == null)
        data = new candidate()
        {
          country_code = "KR",
          country_name = "한국",
          manager_seq = AppIdentity.ud_seq != 4 ? AppIdentity.user_seq : 0,
          manager_name = AppIdentity.ud_seq != 4 ? AppIdentity.name : ""
        };

      var korResume = await cr.SelectLaseCanResume(c_seq, "K");
      var engResume = await cr.SelectLaseCanResume(c_seq, "E");
      var etcResume = await cr.SelectLaseCanResume(c_seq, "O");

      var schoolList = new List<can_school>();
      var careerList = new List<can_career>();
      var placeList = new List<can_place>();
      var jobList = new List<can_job>();
      var busiList = new List<can_business>();
      var foreignList = new List<can_foreign_lan>();
      var resumeList = new List<can_resume>();

      //간편후보자 등록에서 넘어온 경우
      if (sc_seq != 0)
      {
        var simpleCandi = await cr.SelectSimpleCandidateOneAsync(sc_seq);
        data.kor_name = simpleCandi.kor_name;
        data.gender = simpleCandi.gender;
        data.birth_date = simpleCandi.birthdate;
        data.cell_phone = simpleCandi.cell_phone;
        data.email1 = simpleCandi.email;
        data.sns_link1 = simpleCandi.sns_url;

        can_career canCareer = new can_career()
        {
          company_name = simpleCandi.company
            ,
          jobList = new List<can_career_job>()
        };
        careerList.Add(canCareer);
      }
      else if (t_seq != 0)
      {
        schoolList = await cr.TempSelectCanSchoolListAsync(t_seq);
        careerList = await cr.TempSelectCanCareerListAsync(t_seq);
        placeList = await cr.TempSelectCanPlaceListAsync(t_seq);
        jobList = await cr.TempSelectCanJobCodeListAsync(t_seq);
        busiList = await cr.TempSelectCanBusiCodeListAsync(t_seq);
        foreignList = await cr.TempSelectCanForeignLanListAsync(t_seq);
        resumeList = await cr.TempSelectCanResumeListAsync(t_seq);
      }
      else
      {
        schoolList = await cr.SelectCanSchoolListAsync(c_seq);
        careerList = await cr.SelectCanCareerListAsync(c_seq);
        placeList = await cr.SelectCanPlaceListAsync(c_seq);
        jobList = await cr.SelectCanJobCodeListAsync(c_seq);
        busiList = await cr.SelectCanBusiCodeListAsync(c_seq);
        foreignList = await cr.SelectCanForeignLanListAsync(c_seq);
      }


      CandidateCreateModel model = new CandidateCreateModel()
      {
        c_seq = data.c_seq
          ,
        tempsaved_seq = data.tempsaved_seq
          ,
        kor_name = data.kor_name
          ,
        manager_seq = data.manager_seq
          ,
        manager_name = data.manager_name
          ,
        eng_name = data.eng_name
          ,
        is_foreign = data.is_foreign
          ,
        birth_date = data.birth_date
          ,
        gender = data.gender
          ,
        ex_birthdate = data.ex_birthdate
          ,
        country_code = data.country_code
          ,
        country_name = data.country_name
          ,
        addr1 = data.addr1
          ,
        ex_addr = data.ex_addr
          ,
        phone = data.phone
          ,
        wrong_phone = data.wrong_phone
          ,
        cell_phone = data.cell_phone
          ,
        wrong_phone2 = data.wrong_phone2
          ,
        email1 = data.email1
          ,
        email2 = data.email2
          ,
        sns_link1 = data.sns_link1
          ,
        sns_link2 = data.sns_link2
          ,
        sns_link3 = data.sns_link3
          ,
        hope_salary = data.hope_salary
          ,
        hope_salary2 = data.hope_salary2
          ,
        reg_type = data.reg_type
          ,
        keyword = data.keyword
          ,
        is_confidential = data.is_confidential
          ,
        is_inactive = data.is_inactive
          ,
        confi_remark = data.confi_remark
          ,
        inactive_remark = data.inactive_remark
          ,
        schoolList = schoolList
          ,
        companyList = careerList
          ,
        placeList = placeList
          ,
        jobList = jobList
          ,
        busiList = busiList
          ,
        foreignList = foreignList
          ,
        resumeList = resumeList

      };
      ViewBag.uploadFolder = uploadTmpFolder;
      ViewBag.is_pop = is_pop;
      ViewBag.NoMenu = (is_pop == 1 ? true : false);
      if (model.c_seq > 0)
      {
        ViewBag.Title = "Candidate Update: " + model.kor_name + "/" + "만 " + Utils.GetAge(model.birth_date) + "세(" + Utils.ReturnGenderTxt(model.gender) + ")";
      }
      else
      {
        ViewBag.Title = "Candidate Registration";
      }

      return View(model);
      //return View();
    }


    /// <summary>
    /// 후보자 생성화면
    /// </summary>
    /// <param name="c_seq">후보자 seq (candidate)</param>
    /// <param name="sc_seq">간편후보자 seq (simple candidate)</param>
    /// <returns></returns>
    public async Task<ActionResult> DirectorCreate(int d_seq = 0, int nc_seq = 0, int is_pop = 0)
    {
      if (AppIdentity.isExternal == 1 && d_seq != 0)
      {
        CandidateEntityRepository cer = new CandidateEntityRepository();
        element_open_log eol = await cer.SelectDirectorOpenLogOneAsync(d_seq, AppIdentity.user_seq);
        if (eol == null)
        {
          return RedirectToAction("DirectorDetail", "Candidate", new { d_seq = d_seq });
        }
      }

      string uploadTmpFolder = Utils.ReturnUniqueValue(AppIdentity.user_seq);

      CandidateRepository cr = new CandidateRepository();

      director data;

      data = await cr.SelectDirectorOneAsync(d_seq);
      if (data != null)
      {
        if (data.is_confidential == 1 && String.IsNullOrEmpty(data.confi_remark))
        {
          data.confi_remark = " ";
        }

        if (data.is_inactive == 1 && String.IsNullOrEmpty(data.inactive_remark))
        {
          data.confi_remark = " ";
        }
      }



      if (data == null)
        data = new director()
        {
          country_code = "KR",
          country_name = "한국",
        };

      var schoolList = new List<director_school>();
      var careerList = new List<director_career>();
      var jobList = new List<director_job>();
      var busiList = new List<director_business>();

      //간편후보자 등록에서 넘어온 경우
      /*
      if (nc_seq != 0)
      {
        schoolList = await cr.SelectCanSchoolListAsync(nc_seq);
        careerList = await cr.SelectCanCareerListAsync(nc_seq);        
        jobList = await cr.SelectCanJobCodeListAsync(nc_seq);
        busiList = await cr.SelectCanBusiCodeListAsync(nc_seq);        
      }
      else
      {
      */
      schoolList = await cr.SelectDirectorSchoolListAsync(d_seq);
      careerList = await cr.SelectDirectorCareerListAsync(d_seq, 0);
      jobList = await cr.SelectDirectorJobCodeListAsync(d_seq);
      busiList = await cr.SelectDirectorBusiCodeListAsync(d_seq);
      /*
      }
      */

      DirectorCreateModel model = new DirectorCreateModel()
      {
        d_seq = data.d_seq
          ,
        c_seq = data.c_seq
          ,
        kor_name = data.kor_name
          ,
        manager_seq = data.manager_seq
          ,
        eng_name = data.eng_name
          ,
        is_foreign = data.is_foreign
          ,
        birth_date = data.birth_date
          ,
        gender = data.gender
          ,
        ex_birthdate = data.ex_birthdate
          ,
        country_code = data.country_code
          ,
        country_name = data.country_name
          ,
        addr1 = data.addr1
          ,
        phone = data.phone
          ,
        cell_phone = data.cell_phone
          ,
        email1 = data.email1
          ,
        email2 = data.email2
          ,
        hope_salary = data.hope_salary
          ,
        hope_salary2 = data.hope_salary2
          ,
        reg_type = data.reg_type
          ,
        keyword = data.keyword
          ,
        is_confidential = data.is_confidential
          ,
        is_inactive = data.is_inactive
          ,
        confi_remark = data.confi_remark
          ,
        inactive_remark = data.inactive_remark
          ,
        picture_path = data.picture_path
          ,
        schoolList = schoolList
          ,
        companyList = careerList
          ,
        jobList = jobList
          ,
        busiList = busiList
      };
      ViewBag.uploadFolder = uploadTmpFolder;
      ViewBag.is_pop = is_pop;
      ViewBag.NoMenu = (is_pop == 1 ? true : false);
      if (model.c_seq > 0)
      {
        ViewBag.Title = "C-Level Update: " + model.kor_name + "/" + "만 " + Utils.GetAge(model.birth_date) + "세(" + Utils.ReturnGenderTxt(model.gender) + ")";
      }
      else
      {
        ViewBag.Title = "C-Level Registration";
      }

      return View(model);
      //return View();
    }

    [HttpPost]
    public async Task<JsonResult> CandidateCreate(CandidateCreateModel data)
    {
      try
      {

        CandidateEntityRepository cer = new CandidateEntityRepository();

        CandidateCreateUpdateModel model = new CandidateCreateUpdateModel();

        //신규, 수정 state 변수
        string state = "";
        bool is_confi_update = false;
        bool is_inactive_update = false;
        //신규
        if (data.c_seq == 0)
        {
          //기본 정보
          model.data = new candidate()
          {
            kor_name = data.kor_name
              ,
            eng_name = data.eng_name
              ,
            is_foreign = data.is_foreign != null ? data.is_foreign : 0
              ,
            birth_date = data.birth_date
              ,
            gender = data.gender
              ,
            ex_birthdate = data.ex_birthdate
              ,
            country_code = data.country_code
              ,
            addr1 = data.addr1
              ,
            ex_addr = data.ex_addr
              ,
            phone = !string.IsNullOrEmpty(data.phone) ? Regex.Replace(data.phone.Replace(' ', '-').Replace('_', '-').Replace("+82-", "0"), "^82", "0") : ""
              ,
            wrong_phone = data.wrong_phone != null ? data.wrong_phone : 0
              ,
            cell_phone = !string.IsNullOrEmpty(data.cell_phone) ? Regex.Replace(data.cell_phone.Replace(' ', '-').Replace('_', '-').Replace("+82-", "0"), "^82", "0") : ""
              ,
            wrong_phone2 = data.wrong_phone2 != null ? data.wrong_phone2 : 0
              ,
            email1 = data.email1
              ,
            email2 = data.email2
              ,
            sns_link1 = data.sns_link1
              ,
            sns_link2 = data.sns_link2
              ,
            sns_link3 = data.sns_link3
              ,
            hope_salary = data.hope_salary
              ,
            hope_salary2 = data.hope_salary2
              ,
            keyword = data.keyword
              ,
            manager_seq = data.manager_seq
              ,
            reg_type = data.reg_type
              ,
            is_confidential = data.is_confidential != null ? data.is_confidential : 0
              ,
            is_inactive = data.is_inactive != null ? data.is_inactive : 0
              ,
            confi_remark = data.confi_remark
              ,
            inactive_remark = data.inactive_remark
              ,
            create_dt = Utils.NowKorea()
              ,
            create_seq = AppIdentity.user_seq
              ,
            modify_dt = Utils.NowKorea()
              ,
            modify_seq = AppIdentity.user_seq
          };

          if ((data.is_confidential ?? 0) == 1)
          {
            model.data.confidential_user = AppIdentity.user_seq;
            model.data.confidential_date = Utils.NowKorea();

          }

          if (data.is_confidential == 1)
          {
            is_confi_update = true;
          }

          state = CommonCodes.Create;
        }
        //수정
        else
        {
          model.data = await cer.SelectCandidateOneAsync(data.c_seq);

          model.data.kor_name = data.kor_name;
          model.data.eng_name = data.eng_name;
          model.data.is_foreign = data.is_foreign != null ? data.is_foreign : 0;
          model.data.birth_date = data.birth_date;
          model.data.gender = data.gender;
          model.data.ex_birthdate = data.ex_birthdate;
          model.data.country_code = data.country_code;
          model.data.addr1 = data.addr1;
          model.data.ex_addr = data.ex_addr != null ? data.ex_addr : 0;
          //model.data.phone = data.phone;

          model.data.phone = !string.IsNullOrEmpty(data.phone) ? Regex.Replace(data.phone.Replace(' ', '-').Replace('_', '-').Replace("+82-", "0"), "^82", "0") : "";
          model.data.wrong_phone = data.wrong_phone != null ? data.wrong_phone : 0;
          //model.data.cell_phone = data.cell_phone;
          model.data.cell_phone = !string.IsNullOrEmpty(data.cell_phone) ? Regex.Replace(data.cell_phone.Replace(' ', '-').Replace('_', '-').Replace("+82-", "0"), "^82", "0") : "";
          model.data.wrong_phone2 = data.wrong_phone2 != null ? data.wrong_phone2 : 0;
          model.data.email1 = data.email1;
          model.data.email2 = data.email2;
          model.data.sns_link1 = data.sns_link1;
          model.data.sns_link2 = data.sns_link2;
          model.data.sns_link3 = data.sns_link3;
          model.data.manager_seq = data.manager_seq;
          model.data.hope_salary = data.hope_salary;
          model.data.hope_salary2 = data.hope_salary2;
          model.data.keyword = data.keyword;
          model.data.reg_type = data.reg_type;
          //model.data.is_confidential = data.is_confidential != null ? data.is_confidential : 0;
          //model.data.is_inactive = data.is_inactive != null ? data.is_inactive : 0;
          //model.data.confi_remark = data.confi_remark;
          //model.data.inactive_remark = data.inactive_remark;

          model.data.modify_dt = Utils.NowKorea();
          model.data.modify_seq = AppIdentity.user_seq;

          model.deleteSchoolList = await cer.SelectCanSchoolListAsync(data.c_seq);
          model.deleteCareerList = await cer.SelectCanCareerListAsync(data.c_seq);
          model.deletePlaceList = await cer.SelectCanPlaceListAsync(data.c_seq);
          model.deleteJobList = await cer.SelectCanJobCodeListAsync(data.c_seq);
          model.deleteBusiList = await cer.SelectCanBusiCodeListAsync(data.c_seq);
          model.deleteForeignList = await cer.SelectCanForeignLanListAsync(data.c_seq);

          state = CommonCodes.Update;
        }

        //학교정보
        foreach (var sch in data.schoolList)
        {
          var schoolName = sch.schoolName;
          var campusName = "";
          if (!string.IsNullOrWhiteSpace(sch.schoolName))
          {
            //if (string.IsNullOrWhiteSpace(sch.campusName))
            //{
            //  //캠퍼스명이 들어가있다면, 학교명에 학교명(캠퍼스명) 형식으로 들어가 있으므로 제거함.
            //  if (sch.schoolName.IndexOf("(") > 0)
            //    schoolName = sch.schoolName.Substring(0, sch.schoolName.IndexOf("("));
            //
            //  campusName = sch.campusName;
            //}
            var school = await cer.SelectSchoolOneAsync(schoolName, campusName);

            SchoolModel schoolModel = new SchoolModel();

            //학교정보가 db에 없을시, 신규 학교 생성.
            if (school == null)
            {
              school = new code_school();
              school.school_name = schoolName;
              school.campus_name = campusName;
              /*
              CallApi api = new CallApi();
              RequestSchool reqSchool = new RequestSchool()
              {
                  gubun = sch.gubun_str
                  ,
                  searchSchulNm = schoolName
                  ,
                  perPage = "20"
                  ,
                  thisPage = "1"
              };

              //API 에서 학교정보 찾아옴.
              var repSchool = await api.SchoolSearchApi(reqSchool);

              school = new code_school()
              {
                  school_name = schoolName
              };

              //API에서 찾을 수 있는 학교정보라면 항목 추가 입력.
              //해외학교나 이미 폐교된 학교, 통합된 학교는 API 찾을 수 없기 때문.
              if (repSchool.Count > 0)
              {
                  var apiSchool = repSchool.Where(x => x.schoolName == schoolName && (String.IsNullOrEmpty(campusName) || x.campusName == campusName)).FirstOrDefault();

                  school.gubun = sch.gubun;
                  school.school_type = apiSchool.schoolType;
                  school.est_type = apiSchool.estType;
                  school.region = apiSchool.region;
                  school.adres = apiSchool.adres;
                  school.college_info = apiSchool.collegeinfourl;
                  school.link = apiSchool.link;
              }
              */

              schoolModel.newSchool = new code_school();
              schoolModel.newSchool = school;
            }

            var sc = sch;
            sc.sc_seq = school.sc_seq;
            sc.is_transfer = sch.is_transfer != null ? sch.is_transfer : 0;
            sc.is_foreign_school = sch.is_foreign_school != null ? sch.is_foreign_school : 0;

            schoolModel.school = sch;

            model.schoolList.Add(schoolModel);
          }
        }

        //경력 정보
        foreach (var comp in data.companyList)
        {
          if (!string.IsNullOrWhiteSpace(comp.company_name))
          {
            var career = new CareerModel();

            var ca = comp;
            ca.is_work = comp.is_work != null ? comp.is_work : 0;

            career.career = ca;

            ////경력 직무 정보
            career.careerJobList = comp.jobList;

            model.careerList.Add(career);
          }
        }

        //희망 근무지역
        model.placeList = data.placeList;

        //직무 정보
        model.jobList = data.jobList;

        //산업 정보
        model.busiList = data.busiList;

        //외국어 정보
        model.foreignList = data.foreignList;


        List<can_activity> activity_list = new List<can_activity>();

        if (data.is_ai_reg == 1)
        {
          can_activity cActivity = new can_activity();
          cActivity.c_seq = model.data.c_seq;
          cActivity.cl_seq = 1;
          if (data.c_seq == 0)
          {
            cActivity.memo = "AI등록을 통해 등록됨";
          }
          else
          {
            cActivity.memo = "AI등록을 통해 업데이트됨";
          }
          cActivity.create_seq = 1;//AppIdentity.user_seq;
          cActivity.create_dt = Utils.NowKorea();
          //await cer.CreateOrDeleteCanActivity(ca, CommonCodes.Create);
          activity_list.Add(cActivity);
        }
        if (!String.IsNullOrWhiteSpace(data.memo))
        {
          can_activity cActivity = new can_activity();
          cActivity.c_seq = model.data.c_seq;
          cActivity.memo = data.memo;
          cActivity.create_seq = AppIdentity.user_seq;
          cActivity.create_dt = Utils.NowKorea();
          //await cer.CreateOrDeleteCanActivity(ca, CommonCodes.Create);
          activity_list.Add(cActivity);
        }

        if (is_confi_update)
        {
          can_activity cActivity = new can_activity();
          cActivity.c_seq = model.data.c_seq;
          cActivity.cl_seq = 1;
          cActivity.memo = "{{{[Confidential]}}}<br/>" + data.confi_remark;
          cActivity.create_seq = AppIdentity.user_seq;
          cActivity.create_dt = Utils.NowKorea();
          //await cer.CreateOrDeleteCanActivity(ca, CommonCodes.Create);
          activity_list.Add(cActivity);
        }
        /*
        if (is_inactive_update)
        {
          can_activity cActivity = new can_activity();
          cActivity.c_seq = model.data.c_seq;
          cActivity.cl_seq = 1;
          cActivity.memo = "[Inactive]<br/>" + data.inactive_remark;
          cActivity.create_seq = AppIdentity.user_seq;
          cActivity.create_dt = Utils.NowKorea();
          //await cer.CreateOrDeleteCanActivity(ca, CommonCodes.Create);
          activity_list.Add(cActivity);
        }
        */

        await cer.CreateOrUpdateCandidate(model, state, activity_list, CommonCodes.Create, AppIdentity.user_seq, 1);

        if (data.resumeList != null && data.resumeList.Count > 0)
        {
          FileUpload fi = new FileUpload();
          //이력서 임시파일 정식으로 이동
          foreach (var resume in data.resumeList)
          {
            var result = fi.MoveFile(resume.file_origin_path, Path.Combine(Server.MapPath("~/UploadedFiles"), "candidate/" + model.data.c_seq));
            if (!result.status)
              return Json(new
              {
                ok = false
                  ,
                message = result.statusMessage
              });

            can_resume cr = new can_resume()
            {
              c_seq = model.data.c_seq
                ,
              file_type = resume.file_type
                ,
              file_dir = "/UploadedFiles/candidate/" + model.data.c_seq + "/" + result.name
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

            await cer.CreateOrDeleteCanResume(cr, CommonCodes.Create);
          }
        }

        CandidateRepository canrepository = new CandidateRepository();

        //메일에서 등록된 이력서의 경우 메일 정보에 신규등록 완료 정보 업데이트
        if (!String.IsNullOrEmpty(data.mail_key))
        {
          string update_str = (data.c_seq > 0 ? "업데이트" : "신규등록");
          if (data.is_ai_reg == 1)
          {
            update_str += "(GPT)";
          }
          if (AppIdentity.ua_seq > 3)
          {
            update_str = "직접[" + AppIdentity.name + "]" + update_str;
          }

          await canrepository.UpdateMailWork(model.data.c_seq, data.mail_key, update_str);
        }

        //후보자 관련정보 저장 프로시저 실행.
        var procMsg = await canrepository.SaveCandidateTxtMsg(model.data.c_seq);
        var procMsg2 = await canrepository.UpdateCandidateCareerRange(model.data.c_seq);


        return Json(new
        {
          ok = true,
          c_seq = model.data.c_seq
        });
      }
      catch (Exception e)
      {
        return Json(new
        {
          ok = false,
          message = "후보자 저장 중 오류 발생 : " + e.Message +"<br/>" + e.StackTrace
        });
      }
    }

    public async Task<PartialViewResult> CandidateMailRemoveModal(string dv_timestamp)
    {
      MailResumeRepository mrr = new MailResumeRepository();
      var data = await mrr.SelectReadyMailOneAsync(dv_timestamp);
      //MailResumeData data = new MailResumeData();
      if (data == null)
      {
        data = new MailResumeData();
      }

      return PartialView(data);
    }
    public async Task<PartialViewResult> CandidateMailGptModal(string dv_timestamp)
    {
      MailResumeRepository mrr = new MailResumeRepository();
      var data = await mrr.SelectReadyMailOneAsync(dv_timestamp);
      //MailResumeData data = new MailResumeData();
      if (data == null)
      {
        data = new MailResumeData();
      }

      if (String.IsNullOrEmpty(data.candidate_seq) && !String.IsNullOrEmpty(data.gpt_result))
      {
        try
        {

          var model = JsonConvert.DeserializeObject<CandidateCreateModel>(data.gpt_result);
          if (String.IsNullOrEmpty(model.email1))
          {
            model.email1 = data.dv_snd_addr;
          }
          if (!String.IsNullOrEmpty(model.cell_phone) || !String.IsNullOrEmpty(model.email1))
          {
            var dup_result = await FindDup(model.cell_phone, model.email1);

            if (dup_result != null)
            {
              data.candidate_seq = dup_result.c_seq.ToString();
              data.candidate_name = dup_result.kor_name;
              data.birth_year = dup_result.birth_date.Value.Year.ToString();
              data.gender = (dup_result.gender == 1 ? "남" : (dup_result.gender == 2 ? "여" : (dup_result.gender == 0 ? "모름" : "")));
            }
          }
        }
        catch
        {

        }
      }

      try
      {
        var files = await mrr.SelectReadyMailResumeFileListAsync(data.dv_timestamp);
        if (files != null)
        {
          data.files = files;
        }
      }
      catch
      {

      }

      return PartialView(data);
    }

    public async Task<JsonResult> CandidateMailAiAjx(MailResumeSearchModel search)
    {
      try
      {
        search.uv_seq = AppIdentity.user_seq;

        MailResumeRepository mrr = new MailResumeRepository();
        var list = await mrr.SelectUserMailResumeListAsync(search);

        foreach (var mail in list)
        {
          try
          {
            var files = await mrr.SelectReadyMailResumeFileListAsync(mail.dv_timestamp);
            if (files != null)
            {
              mail.files = files;
            }
          }
          catch
          {

          }
        }

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

    public async Task<ActionResult> CandidateMailAi(MailResumeSearchModel search)
    {
      MailResumeListModel model = new MailResumeListModel()
      {
        search = search
        //  ,
        //list = list
      };

      return View(model);
    }


    [HttpPost]
    public async Task<JsonResult> RemoveMailProc(string dv_timestamp, string remove_yn ="N" , string desc = "")
    {
      try
      {
        CandidateRepository canrepository = new CandidateRepository();

        //메일에서 등록된 이력서의 경우 메일 정보에 신규등록 완료 정보 업데이트
        if (!String.IsNullOrEmpty(dv_timestamp))
        {
          
          
          if (AppIdentity.ua_seq > 3)
          {
            desc = "[" + AppIdentity.name + "]" + desc;
          }

          await canrepository.UpdateMailRemove(dv_timestamp, remove_yn, desc);
        }

        return Json(new
        {
          ok = true,
          message = "처리완료"
        });
      }
      catch (Exception e)
      {
        return Json(new
        {
          ok = false,
          message = "메일 삭제 중 오류 발생 : " + e.Message
        });
      }
    }
    [HttpPost]
    public async Task<JsonResult> DirectorCreate(DirectorCreateModel data)
    {
      try
      {

        CandidateEntityRepository cer = new CandidateEntityRepository();

        DirectorCreateUpdateModel model = new DirectorCreateUpdateModel();

        //신규, 수정 state 변수
        string state = "";
        bool is_confi_update = false;
        bool is_inactive_update = false;
        //신규
        if (data.d_seq == 0)
        {
          //기본 정보
          model.data = new director()
          {
            kor_name = data.kor_name
              ,
            eng_name = data.eng_name
              ,
            picture_path = data.picture_path
              ,
            is_foreign = data.is_foreign != null ? data.is_foreign : 0
              ,
            birth_date = data.birth_date
              ,
            gender = data.gender
              ,
            ex_birthdate = data.ex_birthdate
              ,
            country_code = data.country_code
              ,
            addr1 = data.addr1
              ,
            ex_addr = data.ex_addr
              ,
            phone = !string.IsNullOrEmpty(data.phone) ? Regex.Replace(data.phone.Replace(' ', '-').Replace('_', '-').Replace("+82-", "0"), "^82", "0") : ""
              ,
            cell_phone = !string.IsNullOrEmpty(data.cell_phone) ? Regex.Replace(data.cell_phone.Replace(' ', '-').Replace('_', '-').Replace("+82-", "0"), "^82", "0") : ""
              ,
            email1 = data.email1
              ,
            email2 = data.email2
              ,
            hope_salary = data.hope_salary
              ,
            hope_salary2 = data.hope_salary2
              ,
            keyword = data.keyword
              ,
            manager_seq = data.manager_seq
              ,
            reg_type = data.reg_type
              ,
            is_confidential = data.is_confidential != null ? data.is_confidential : 0
              ,
            is_inactive = data.is_inactive != null ? data.is_inactive : 0
              ,
            confi_remark = data.confi_remark
              ,
            inactive_remark = data.inactive_remark
              ,
            create_dt = Utils.NowKorea()
              ,
            create_seq = AppIdentity.user_seq
              ,
            modify_dt = Utils.NowKorea()
              ,
            modify_seq = AppIdentity.user_seq
          };

          if ((data.is_confidential ?? 0) == 1)
          {
            model.data.confidential_user = AppIdentity.user_seq;
            model.data.confidential_date = Utils.NowKorea();

          }

          if (data.is_confidential == 1)
          {
            is_confi_update = true;
          }

          state = CommonCodes.Create;
        }
        //수정
        else
        {
          model.data = await cer.SelectDirectorOneAsync(data.d_seq);

          model.data.kor_name = data.kor_name;
          model.data.eng_name = data.eng_name;
          model.data.picture_path = data.picture_path;
          model.data.is_foreign = data.is_foreign != null ? data.is_foreign : 0;
          model.data.birth_date = data.birth_date;
          model.data.gender = data.gender;
          model.data.ex_birthdate = data.ex_birthdate;
          model.data.country_code = data.country_code;
          model.data.addr1 = data.addr1;
          model.data.ex_addr = data.ex_addr != null ? data.ex_addr : 0;
          //model.data.phone = data.phone;

          model.data.phone = !string.IsNullOrEmpty(data.phone) ? Regex.Replace(data.phone.Replace(' ', '-').Replace('_', '-').Replace("+82-", "0"), "^82", "0") : "";
          model.data.cell_phone = !string.IsNullOrEmpty(data.cell_phone) ? Regex.Replace(data.cell_phone.Replace(' ', '-').Replace('_', '-').Replace("+82-", "0"), "^82", "0") : "";
          model.data.email1 = data.email1;
          model.data.email2 = data.email2;
          model.data.manager_seq = data.manager_seq;
          model.data.hope_salary = data.hope_salary;
          model.data.hope_salary2 = data.hope_salary2;
          model.data.keyword = data.keyword;
          model.data.reg_type = data.reg_type;
          //model.data.is_confidential = data.is_confidential != null ? data.is_confidential : 0;
          //model.data.is_inactive = data.is_inactive != null ? data.is_inactive : 0;
          //model.data.confi_remark = data.confi_remark;
          //model.data.inactive_remark = data.inactive_remark;

          model.data.modify_dt = Utils.NowKorea();
          model.data.modify_seq = AppIdentity.user_seq;

          model.deleteSchoolList = await cer.SelectDirectorSchoolListAsync(data.d_seq);
          model.deleteCareerList = await cer.SelectDirectorCareerListAsync(data.d_seq);
          model.deleteJobList = await cer.SelectDirectorJobCodeListAsync(data.d_seq);
          model.deleteBusiList = await cer.SelectDirectorBusiCodeListAsync(data.d_seq);

          state = CommonCodes.Update;
        }

        //학교정보
        foreach (var sch in data.schoolList)
        {
          model.schoolList.Add(sch);
        }

        //경력 정보
        foreach (var comp in data.companyList)
        {
          if (!string.IsNullOrWhiteSpace(comp.company_name))
          {
            comp.is_work = comp.is_work != null ? comp.is_work : 0;

            model.careerList.Add(comp);
          }
        }

        //직무 정보
        model.jobList = data.jobList;

        //산업 정보
        model.busiList = data.busiList;
        /*
        List<can_activity> activity_list = new List<can_activity>();

        if (!String.IsNullOrWhiteSpace(data.memo))
        {
          can_activity cActivity = new can_activity();
          cActivity.c_seq = model.data.c_seq;
          cActivity.memo = data.memo;
          cActivity.create_seq = AppIdentity.user_seq;
          cActivity.create_dt = Utils.NowKorea();
          //await cer.CreateOrDeleteCanActivity(ca, CommonCodes.Create);
          activity_list.Add(cActivity);
        }
        */
        /*
        if (is_confi_update)
        {
          can_activity cActivity = new can_activity();
          cActivity.c_seq = model.data.c_seq;
          cActivity.cl_seq = 1;
          cActivity.memo = "{{{[Confidential]}}}<br/>" + data.confi_remark;
          cActivity.create_seq = AppIdentity.user_seq;
          cActivity.create_dt = Utils.NowKorea();
          //await cer.CreateOrDeleteCanActivity(ca, CommonCodes.Create);
          activity_list.Add(cActivity);
        }
        */
        /*
        if (is_inactive_update)
        {
          can_activity cActivity = new can_activity();
          cActivity.c_seq = model.data.c_seq;
          cActivity.cl_seq = 1;
          cActivity.memo = "[Inactive]<br/>" + data.inactive_remark;
          cActivity.create_seq = AppIdentity.user_seq;
          cActivity.create_dt = Utils.NowKorea();
          //await cer.CreateOrDeleteCanActivity(ca, CommonCodes.Create);
          activity_list.Add(cActivity);
        }
        */

        await cer.CreateOrUpdateDirector(model, state, CommonCodes.Create, AppIdentity.user_seq, 1);
        /*
        if (data.resumeList != null && data.resumeList.Count > 0)
        {
          FileUpload fi = new FileUpload();
          //이력서 임시파일 정식으로 이동
          foreach (var resume in data.resumeList)
          {
            var result = fi.MoveFile(resume.file_origin_path, Path.Combine(Server.MapPath("~/UploadedFiles"), "candidate/" + model.data.c_seq));
            if (!result.status)
              return Json(new
              {
                ok = false
                  ,
                message = result.statusMessage
              });

            can_resume cr = new can_resume()
            {
              c_seq = model.data.c_seq
                ,
              file_type = resume.file_type
                ,
              file_dir = "/UploadedFiles/candidate/" + model.data.c_seq + "/" + result.name
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

            await cer.CreateOrDeleteCanResume(cr, CommonCodes.Create);
          }
        }
        */
        CandidateRepository canrepository = new CandidateRepository();



        //후보자 관련정보 저장 프로시저 실행.
        var procMsg = await canrepository.SaveDirectorTxtMsg(model.data.d_seq);
        //var procMsg = await canrepository.SaveCandidateTxtMsg(model.data.c_seq);

        return Json(new
        {
          ok = true,
          d_seq = model.data.d_seq
        });
      }
      catch (Exception e)
      {
        return Json(new
        {
          ok = false,
          message = "C-Level 저장 중 오류 발생 : " + e.Message
        });
      }
    }


    [HttpPost]
    public async Task<JsonResult> TempSaveCandidate(TempCandidateCreateModel data)
    {
      try
      {

        CandidateEntityRepository cer = new CandidateEntityRepository();

        TempCandidateCreateUpdateModel model = new TempCandidateCreateUpdateModel();

        //신규, 수정 state 변수
        string state = "";


        model.temp_seq = data.tempsaved_seq;
        //기본 정보
        model.data = new tempsaved_candidate()
        {
          kor_name = data.kor_name
            ,
          eng_name = data.eng_name
            ,
          is_foreign = data.is_foreign != null ? data.is_foreign : 0
            ,
          birth_date = data.birth_date
            ,
          gender = data.gender
            ,
          ex_birthdate = data.ex_birthdate
            ,
          country_code = data.country_code
            ,
          addr1 = data.addr1
            ,
          manager_seq = data.manager_seq
            ,
          ex_addr = data.ex_addr
            ,
          phone = data.phone
            ,
          wrong_phone = data.wrong_phone != null ? data.wrong_phone : 0
            ,
          cell_phone = data.cell_phone
            ,
          wrong_phone2 = data.wrong_phone2 != null ? data.wrong_phone2 : 0
            ,
          email1 = data.email1
            ,
          email2 = data.email2
            ,
          sns_link1 = data.sns_link1
            ,
          sns_link2 = data.sns_link2
            ,
          sns_link3 = data.sns_link3
            ,
          keyword = data.keyword
            ,
          reg_type = data.reg_type
            ,
          is_confidential = data.is_confidential != null ? data.is_confidential : 0
            ,
          is_inactive = data.is_inactive != null ? data.is_inactive : 0
            ,
          create_dt = Utils.NowKorea()
            ,
          create_seq = AppIdentity.user_seq
        };

        state = CommonCodes.Create;


        //학교정보
        foreach (var sch in data.schoolList)
        {
          var schoolName = sch.schoolName;
          var campusName = "";
          if (!string.IsNullOrWhiteSpace(sch.schoolName))
          {

            tempsaved_SchoolModel schoolModel = new tempsaved_SchoolModel();

            //학교정보가 db에 없을시, 신규 학교 생성.

            sch.school_name = schoolName;
            var sc = sch;
            sc.is_transfer = sch.is_transfer != null ? sch.is_transfer : 0;
            sc.is_foreign_school = sch.is_foreign_school != null ? sch.is_foreign_school : 0;

            schoolModel.school = sch;

            model.schoolList.Add(schoolModel);
          }
        }

        //경력 정보
        foreach (var comp in data.companyList)
        {
          if (!String.IsNullOrWhiteSpace(comp.company_name))
          {
            var career = new tempsaved_CareerModel();

            var ca = comp;
            ca.is_work = comp.is_work != null ? comp.is_work : 0;

            career.career = ca;

            ////경력 직무 정보
            career.careerJobList = comp.jobList;

            model.careerList.Add(career);
          }
        }

        //희망 근무지역
        model.placeList = data.placeList;

        //직무 정보
        model.jobList = data.jobList;

        //산업 정보
        model.busiList = data.busiList;

        //외국어 정보
        model.foreignList = data.foreignList;

        await cer.TempCreateOrUpdateCandidate(model, state);

        if (data.resumeList != null && data.resumeList.Count > 0)
        {
          FileUpload fi = new FileUpload();
          //이력서 임시파일 정식으로 이동
          foreach (var resume in data.resumeList)
          {
            var result = fi.MoveFile(resume.file_origin_path, Path.Combine(Server.MapPath("~/UploadedFiles"), "t_candidate/" + model.data.c_seq));
            if (!result.status)
              return Json(new
              {
                ok = false
                  ,
                message = result.statusMessage
              });

            tempsaved_can_resume cr = new tempsaved_can_resume()
            {
              c_seq = model.data.c_seq
                ,
              file_type = resume.file_type
                ,
              file_dir = "/UploadedFiles/t_candidate/" + model.data.c_seq + "/" + result.filePath
                ,
              file_origin_path = result.filePath
                ,
              file_path = result.name
                ,
              file_extension = result.extension
            };

            await cer.CreateOrDeleteTempCanResume(cr, CommonCodes.Create);
          }
        }

        return Json(new
        {
          ok = true
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

    #region 간편 후보 등록



    public ActionResult CandidateContactList()
    {
      return View();
    }
    public ActionResult SimpleCandidateList()
    {
      return View();
    }

    [HttpGet]
    public async Task<JsonResult> LoadSimpleCandidateList(simple_candidate data)
    {
      try
      {
        CandidateRepository cr = new CandidateRepository();

        var totalCnt = await cr.CountSimpleCandidateAsync(AppIdentity.user_seq, data.p_seq);
        var list = await cr.SelectSimpleCandidateListAsync(data, AppIdentity.user_seq);

        return Json(new
        {
          data = list,
          totalCnt = totalCnt,
        }, JsonRequestBehavior.AllowGet);
      }
      catch (Exception e)
      {
        return Json(new
        {

        }, JsonRequestBehavior.AllowGet);
      }

    }

    [HttpPost]
    public async Task<JsonResult> SaveSimpleCandidate(List<simple_candidate> data_list, List<simple_candidate_history> log_list)
    {
      try
      {

        CandidateEntityRepository cer = new CandidateEntityRepository();
        var arr = new List<object>();

        foreach (var data in data_list)
        {
          try
          {
            var sc = await cer.SelectSimpleCandidateOneAsync(data.sc_seq);
            //신규
            if (sc == null)
            {
              sc = new simple_candidate()
              {
                kor_name = data.kor_name,
                birth_str = data.birth_str,
                cell_phone = data.cell_phone,
                email = data.email,
                sns_url = data.sns_url,
                school = data.school,
                company = data.company,
                company_desc = data.company_desc,
                comments = data.comments,
                create_dt = Utils.NowKorea(),
                create_user = AppIdentity.user_seq,
                modify_dt = Utils.NowKorea(),
                modify_user = AppIdentity.user_seq
              };

              await cer.CreateOrUpdateSimpleCandidate(sc, CommonCodes.Create);
            }
            else
            {
              sc.kor_name = data.kor_name;
              sc.birth_str = data.birth_str;
              sc.cell_phone = data.cell_phone;
              sc.email = data.email;
              sc.sns_url = data.sns_url;
              sc.school = data.school;
              sc.company = data.company;
              sc.company_desc = data.company_desc;
              sc.comments = data.comments;
              sc.modify_dt = Utils.NowKorea();
              sc.modify_user = AppIdentity.user_seq;

              await cer.CreateOrUpdateSimpleCandidate(sc, CommonCodes.Update);
            }

            arr.Add(new
            {
              ok = true,
              rowindex = data.rowindex,
              sc_seq = sc.sc_seq,
              create_name = (sc.create_user == AppIdentity.user_seq ? AppIdentity.name : null),
              create_dt = sc.create_dt.ToString(),
              modify_name = (sc.modify_user == AppIdentity.user_seq ? AppIdentity.name : null),
              modify_dt = sc.modify_dt.ToString(),
              message = "간편 후보를 등록 했습니다."
            });
          }
          catch (Exception e)
          {
            arr.Add(new
            {
              ok = false,
              rowindex = data.rowindex,
              message = "오류가 발생했습니다. - [" + e.Message + "][" + e.InnerException.InnerException.Message + "]"
            });
          }
        }




        return Json(new
        {
          ok = true,
          data = arr
        });
      }
      catch (Exception e)
      {
        return Json(new
        {
          ok = false,
          message = "알 수 없는 오류가 발생 했습니다. - [" + e.Message + "][" + e.InnerException.InnerException.Message + "]"
        });
      }
    }

    [HttpPost]
    public async Task<JsonResult> RemoveSimpleCandidate(List<simple_candidate> remove_list)
    {
      try
      {

        CandidateEntityRepository cer = new CandidateEntityRepository();
        var arr = new List<object>();

        foreach (var data in remove_list)
        {
          var sc = await cer.SelectSimpleCandidateOneAsync(data.sc_seq);
          //신규
          if (sc != null)
          {
            sc.is_del = 1;
            sc.modify_dt = Utils.NowKorea();
            sc.modify_user = AppIdentity.user_seq;

            await cer.CreateOrUpdateSimpleCandidate(sc, CommonCodes.Update);
          }

          arr.Add(new
          {
            ok = true,
            message = "간편 후보를 삭제 했습니다."
          });
        }




        return Json(new
        {
          ok = true,
          data = arr
        });
      }
      catch (Exception e)
      {
        return Json(new
        {
          ok = false,
          message = "알 수 없는 오류가 발생 했습니다. - [" + e.Message + "][" + e.InnerException.InnerException.Message + "]"
        });
      }
    }

    [HttpPost]
    public async Task<JsonResult> CreateSimpleCandidate(string kor_name, int? gender, string birthDateStr, string cell_phone, string email, string sns_url, string company, string comments, int sc_seq = 0, int p_seq = 0)
    {
      try
      {
        DateTime birthDt = new DateTime();
        DateTime.TryParse(birthDateStr, out birthDt);

        CandidateEntityRepository cer = new CandidateEntityRepository();
        var sc = await cer.SelectSimpleCandidateOneAsync(sc_seq);

        //신규
        if (sc == null)
        {
          sc = new simple_candidate()
          {
            ac_seq = 0,
            c_seq = 0,
            p_seq = p_seq,
            kor_name = kor_name,
            gender = gender,
            birthdate = birthDt,
            cell_phone = cell_phone,
            email = email,
            sns_url = sns_url,
            company = company,
            comments = comments,
            create_dt = Utils.NowKorea(),
            create_user = AppIdentity.user_seq,
            modify_dt = Utils.NowKorea(),
            modify_user = AppIdentity.user_seq
          };

          await cer.CreateOrUpdateSimpleCandidate(sc, CommonCodes.Create);
        }
        else
        {
          sc.ac_seq = 0;
          sc.c_seq = 0;
          sc.p_seq = p_seq;
          sc.kor_name = kor_name;
          sc.gender = gender;
          sc.birthdate = birthDt;
          sc.cell_phone = cell_phone;
          sc.email = email;
          sc.sns_url = sns_url;
          sc.company = company;
          sc.comments = comments;
          sc.modify_dt = Utils.NowKorea();
          sc.modify_user = AppIdentity.user_seq;

          await cer.CreateOrUpdateSimpleCandidate(sc, CommonCodes.Update);
        }

        return Json(new
        {
          ok = true,
          sc_seq = sc.sc_seq,
          message = "간편 후보를 등록 했습니다."
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
    public async Task<JsonResult> DeleteSimpleCandidate(int sc_seq)
    {
      try
      {
        CandidateEntityRepository cer = new CandidateEntityRepository();
        var sc = await cer.SelectSimpleCandidateOneAsync(sc_seq);

        if (sc == null)
          return Json(new
          {
            ok = false,
            message = "삭제하려는 간편 후보를 찾을 수 없습니다."
          });

        await cer.CreateOrUpdateSimpleCandidate(sc, CommonCodes.Delete);

        return Json(new
        {
          ok = true,
          message = "간편 후보를 삭제 했습니다."
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

    [HttpPost]
    public async Task<JsonResult> DeleteTempCandidate(int c_seq)
    {
      try
      {
        CandidateEntityRepository cer = new CandidateEntityRepository();
        var tc = await cer.SelectTempCandidateOneAsync(c_seq);

        if (tc == null)
          return Json(new
          {
            ok = false,
            message = "삭제하려는 임시저장 후보를 찾을 수 없습니다."
          });

        await cer.CreateOrUpdateTempCandidate(tc, CommonCodes.Delete);

        return Json(new
        {
          ok = true,
          message = "임시저장 후보를 삭제 했습니다."
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
    /// 동의서 관리
    /// </summary>
    /// <param name="search"></param>
    /// <param name="page"></param>
    /// <returns></returns>
    public ActionResult PrivacyAgreeList(PrivacyAgreeSearchModel search, int page = 1)
    {

      //한 페이지당 몇개의 행을 보여줄 것인지
      int pageSize = AppPaging.PageLength20;

      int totalCount = 0;

      CandidateRepository cr = new CandidateRepository();

      search.uv_seq = AppIdentity.user_seq;
      var list = cr.SelectPrivacyAgreeList(search, (page - 1) * pageSize, pageSize, out totalCount);

      PrivacyAgreeListViewModel model = new PrivacyAgreeListViewModel()
      {
        search = search
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


      return View(model);
    }

    #region 관심 후보자 (잠재후보)

    /// <summary>
    /// 나의 관심후보 리스트
    /// </summary>
    /// <param name="search"></param>
    /// <param name="page"></param>
    /// <returns></returns>
    public ActionResult MyCandidateList(MyCandidateSearchModel search, int page = 1)
    {

      //한 페이지당 몇개의 행을 보여줄 것인지
      int pageSize = AppPaging.PageLength20;

      int totalCount = 0;

      CandidateRepository cr = new CandidateRepository();

      var list = cr.SelectMyCandidateList(search, AppIdentity.user_seq, (page - 1) * pageSize, pageSize, out totalCount);

      MyCandidateListViewModel model = new MyCandidateListViewModel()
      {
        search = search
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


      return View(model);
    }

    /// <summary>
    /// 관심후보자 정보 수정 view
    /// </summary>
    /// <param name="cf_seq"></param>
    /// <returns></returns>
    public async Task<PartialViewResult> MyCandidateModify(int cf_seq)
    {
      CandidateRepository cr = new CandidateRepository();
      var data = await cr.SelectMyCandidateOneAsync(cf_seq);

      return PartialView(data);
    }

    public async Task<PartialViewResult> MyCandidateAdd(int c_seq)
    {
      CandidateRepository cr = new CandidateRepository();
      var data = await cr.SelectMyCandidateNewOneAsync(c_seq, AppIdentity.user_seq);

      return PartialView("MyCandidateModify", data);
    }

    /// <summary>
    /// 검색조건 저장 팝업
    /// </summary>
    /// <param name="url"></param>
    /// <returns></returns>
    public async Task<PartialViewResult> CandidateSearchCreate(string url)
    {
      //CandidateEntityRepository cer = new CandidateEntityRepository();
      candidate_search_save data = new candidate_search_save
      {
        url = url
      };

      return PartialView("CandidateSearchCreate", data);
    }

    /// <summary>
    /// 검색조건 삭제
    /// </summary>
    /// <param name="url"></param>
    /// <returns></returns>
    [HttpPost]
    public async Task<JsonResult> DeleteCandidateSearchSave(int css_seq)
    {
      try
      {
        if (css_seq == 0)
        {
          return Json(new
          {
            ok = false,
            message = "잘못된 접근입니다.."
          });
        }
        CandidateEntityRepository cer = new CandidateEntityRepository();
        string flag = "D";
        var css = await cer.SelectCanSearchSaveOneAsync(css_seq);
        await cer.CreateOrDeleteCanSearchSave(css, flag);

        return Json(new
        {
          ok = true,
          message = "검색조건 삭제"
        });
      }
      catch (Exception e)
      {
        return Json(new
        {
          ok = false,
          message = "검색조건 삭제 중 오류가 발생 했습니다."
        });
      }
    }


    /// <summary>
    /// 검색조건 저장 
    /// </summary>
    /// <param name="url"></param>
    /// <returns></returns>
    [HttpPost]
    public async Task<JsonResult> CandidateSearchCreate(int css_seq = 0, string url = "", string title = "")
    {
      try
      {
        if (String.IsNullOrWhiteSpace(url) || String.IsNullOrWhiteSpace(title))
        {
          return Json(new
          {
            ok = false,
            message = "잘못된 검색조건 또는 설명이 없습니다."
          });
        }
        CandidateEntityRepository cer = new CandidateEntityRepository();
        string flag = "C";
        var css = await cer.SelectCanSearchSaveOneAsync(css_seq);
        if (css == null)
        {
          css = new candidate_search_save
          {
            url = url,
            title = title,
            create_user = AppIdentity.user_seq,
            create_dt = Utils.NowKorea(),
            modify_user = AppIdentity.user_seq,
            modify_dt = Utils.NowKorea()
          };
        }
        else
        {
          css.modify_dt = Utils.NowKorea();
          css.url = url;
          css.title = title;
          flag = "U";
        }

        await cer.CreateOrDeleteCanSearchSave(css, flag);

        return Json(new
        {
          ok = true,
          message = "검색조건 등록완료"
        });
      }
      catch (Exception e)
      {
        return Json(new
        {
          ok = false,
          message = "검색조건 등록 중 오류가 발생 했습니다."
        });
      }
    }
    //검색조건 보기화면
    public async Task<PartialViewResult> CandidateSearchSaveList()
    {
      CandidateEntityRepository cer = new CandidateEntityRepository();

      var model = await cer.SelectCanSearchSaveListAsync(AppIdentity.user_seq);

      return PartialView(model);
    }



    public async Task<PartialViewResult> CandidateContactCreate(int c_seq)
    {
      CandidateRepository cr = new CandidateRepository();
      var data = await cr.SelectCandidateOneAsync(c_seq);

      return PartialView("CandidateContactCreate", data);
    }

    public async Task<PartialViewResult> CandidateAttentionCreate(int c_seq)
    {
      CandidateRepository cr = new CandidateRepository();
      var data = await cr.SelectCandidateOneAsync(c_seq);

      return PartialView("CandidateAttentionCreate", data);
    }

    public async Task<PartialViewResult> CandidateConfiCreate(int c_seq)
    {
      CandidateRepository cr = new CandidateRepository();
      var data = await cr.SelectCandidateOneAsync(c_seq);

      return PartialView("CandidateConfiCreate", data);
    }

    public async Task<PartialViewResult> CandidateInactiveCreate(int c_seq)
    {
      CandidateRepository cr = new CandidateRepository();
      var data = await cr.SelectCandidateOneAsync(c_seq);

      return PartialView("CandidateInactiveCreate", data);
    }


    /// <summary>
    /// 관심후보자 정보 수정.
    /// </summary>
    /// <param name="cf_seq"></param>
    /// <param name="company"></param>
    /// <param name="position"></param>
    /// <param name="memo"></param>
    /// <returns></returns>
    [HttpPost]
    public async Task<JsonResult> MyCandidateModify(int c_seq, int cf_seq, string company, string position, string memo)
    {
      try
      {
        CandidateEntityRepository cer = new CandidateEntityRepository();

        can_interest interest = null;
        string state = String.Empty;
        string msg_str = "수정";

        if (cf_seq > 0)
        {
          interest = await cer.SelectCanMyListOneAsync(cf_seq);
          state = CommonCodes.Update;
        }
        else if (c_seq > 0)
        {

          interest = await cer.SelectCanMyListOneAsync(c_seq, AppIdentity.user_seq);

          if (interest != null)
          {
            state = CommonCodes.Update;
          }
          else
          {
            interest = new can_interest()
            {
              c_seq = c_seq
            ,
              uv_seq = AppIdentity.user_seq
            ,
              create_dt = Utils.NowKorea()
            };

            state = CommonCodes.Create;
            msg_str = "등록";

          }

        }


        if (interest == null)
          return Json(new
          {
            ok = false,
            message = msg_str + "하려는 후보자를 찾을 수 없습니다."
          });

        interest.company = company;
        interest.position = position;
        interest.memo = memo;

        await cer.CreateOrDeleteCanMyList(interest, state);

        return Json(new
        {
          ok = true,
          message = "My List 정보를 " + msg_str + " 했습니다.",
          c_seq = interest.c_seq
        });
      }
      catch (Exception e)
      {
        return Json(new
        {
          ok = false,
          message = "[Server Error] : " + e.Message
        });
      }
    }

    #endregion

    public ActionResult CandidateHistoryList(CandidateHistorySearchModel search, int page = 1)
    {

      int totalCount = 0;
      string[] colorClasses = { "bg-cyan-600", "bg-orange-600", "bg-green-500", "bg-orange-500" };
      Random rand = new Random();

      //한 페이지당 몇개의 행을 보여줄 것인지
      int pageSize = AppPaging.PageLength30;

      CandidateRepository cr = new CandidateRepository();

      var list = cr.SelectViewCanActivityList(search, (page - 1) * pageSize, pageSize, out totalCount);
      var history_date_group_list = new List<CandidateHistoryDateGroupModel>();

      if (list.Count > 0)
      {
        CandidateHistoryDateGroupModel history_date_group = null;
        CandidateHistoryGroupModel history_group = null;
        foreach (var item in list)
        {
          if (history_date_group == null || history_date_group.group_date != item.create_dt_str)
          {

            if (history_date_group != null && history_group != null)
            {
              history_date_group.group_list.Add(history_group);
            }

            if (history_date_group != null)
            {
              history_date_group_list.Add(history_date_group);
            }

            history_date_group = new CandidateHistoryDateGroupModel()
            {
              group_date = item.create_dt_str
            };

            history_group = null;
          }
          //구간 변경될 경우 
          if (history_group == null || history_group.table_type != item.table_type || history_group.c_seq != item.c_seq || history_group.p_seq != item.p_seq)
          {
            if (history_group != null)
            {
              history_date_group.group_list.Add(history_group);
            }

            history_group = new CandidateHistoryGroupModel()
            {
              table_type = item.table_type
                ,
              p_seq = item.p_seq
                ,
              c_seq = item.c_seq
                ,
              start_dt = item.create_dt
                ,
              end_dt = item.create_dt
                ,
              dot_color = colorClasses[rand.Next(colorClasses.Length)]
            };
          }
          if (history_date_group != null && history_group != null)
          {
            history_group.activityList.Add(item);
            history_group.start_dt = item.create_dt;
          }
        }


        if (history_date_group != null && history_group != null)
        {
          history_date_group.group_list.Add(history_group);
        }

        if (history_date_group != null)
        {
          history_date_group_list.Add(history_date_group);
        }
      }

      CandidateHistoryListViewModel model = new CandidateHistoryListViewModel
      {
        search = search
          ,
        list = history_date_group_list
          ,
        PagingInfo = new PagingInfo()
        {
          CurrentPage = page,
          ItemsPerPage = pageSize,
          TotalItems = totalCount
        }
      };

      return View(model);
    }

    /*
    public ActionResult PartialCandidateHistoryList(CandidateHistorySearchModel search, int page = 1)
    {
        int totalCount = 0;
        string[] colorClasses = { "bg-cyan-600", "bg-orange-600", "bg-green-500", "bg-orange-500" };
        Random rand = new Random();

        //한 페이지당 몇개의 행을 보여줄 것인지
        int pageSize = AppPaging.PageLength10;

        CandidateRepository cr = new CandidateRepository();

        var list = cr.SelectViewCanActivityList(search, (page - 1) * pageSize, pageSize, out totalCount);
        var history_date_group_list = new List<CandidateHistoryDateGroupModel>();

        if (list.Count > 0)
        {
            CandidateHistoryDateGroupModel history_date_group = null;
            CandidateHistoryGroupModel history_group = null;
            foreach (var item in list)
            {
                if (history_date_group == null || history_date_group.group_date != item.create_dt_str)
                {

                    if (history_date_group != null && history_group != null)
                    {
                        history_date_group.group_list.Add(history_group);
                    }

                    if (history_date_group != null)
                    {
                        history_date_group_list.Add(history_date_group);
                    }

                    history_date_group = new CandidateHistoryDateGroupModel()
                    {
                        group_date = item.create_dt_str
                    };

                    history_group = null;
                }
                //구간 변경될 경우 
                if (history_group == null || history_group.table_type != item.table_type || history_group.c_seq != item.c_seq || history_group.p_seq != item.p_seq)
                {
                    if (history_group != null)
                    {
                        history_date_group.group_list.Add(history_group);
                    }

                    history_group = new CandidateHistoryGroupModel()
                    {
                        table_type = item.table_type
                        ,
                        p_seq = item.p_seq
                        ,
                        c_seq = item.c_seq
                        ,
                        start_dt = item.create_dt
                        ,
                        end_dt = item.create_dt
                        ,
                        dot_color = colorClasses[rand.Next(colorClasses.Length)]
                    };
                }
                if (history_date_group != null && history_group != null)
                {
                    history_group.activityList.Add(item);
                    history_group.start_dt = item.create_dt;
                }
            }


            if (history_date_group != null && history_group != null)
            {
                history_date_group.group_list.Add(history_group);
            }

            if (history_date_group != null)
            {
                history_date_group_list.Add(history_date_group);
            }
        }

        CandidateHistoryListViewModel model = new CandidateHistoryListViewModel
        {
            search = search
            ,
            list = history_date_group_list
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
    */


    public async Task<PartialViewResult> SearchCandidateManager(int ud_seq = 0, string searchTxt = "")
    {
      ProjectRepository pr = new ProjectRepository();
      AccountRepository ar = new AccountRepository();

      //Searcher이지만, 같이 써도 무관함.
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

    public async Task<PartialViewResult> SearchClientManager(int ud_seq = 0, string searchTxt = "")
    {
      ProjectRepository pr = new ProjectRepository();
      AccountRepository ar = new AccountRepository();

      //Searcher이지만, 같이 써도 무관함.
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
  }
}