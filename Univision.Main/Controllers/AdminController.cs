using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Threading.Tasks;
using Univision.Core.Repositories;
using Univision.Core.Models;
using Univision.Core.Models.DTO;
using Univision.Core.Models.DTO.Response.Admin;
using Univision.Core.Models.DTO.Response.Board;
using Univision.Core.Models.DTO.Request.Board;
using Univision.Main.Infrastructure;
using Univision.Security;
using Univision.Main.Infrastructure.CustomFilters;

namespace Univision.Main.Controllers
{
  public class AdminController : BaseController
  {
    #region 99.공통

    [HttpPost]
    public async Task<PartialViewResult> AuthSelectApi()
    {
      AdminRepository adr = new AdminRepository();

      List<uv_auth> list = await adr.SelectAuthListAsync(0);

      return PartialView(list);
    }

    [HttpPost]
    public async Task<PartialViewResult> DivisionSelectApi()
    {
      AdminRepository adr = new AdminRepository();

      List<uv_division> list = await adr.SelectDivisionListAsync(0);

      return PartialView(list);
    }

    //아이디 중복체크
    [HttpPost]
    public async Task<JsonResult> UserDupCheck(string user_id)
    {
      bool validate = false;
      if (user_id != "")
      {
        AdminRepository adr = new AdminRepository();

        uv_user user = await adr.SelectUserOneAsync(user_id);

        if (user == null)
        {
          validate = true;
        }
      }


      return Json(new
      {
        valid = validate
      });
    }


    #endregion

    public async Task<ActionResult> Index()
    {
      return View();
    }

    #region 0.사용자 관리
    #region 0.1 리스트
    // 공지사항 리스트
    public async Task<ActionResult> UserList(int page = 1, int pageSize = 20, int isRetire = 0, string SearchString = "")
    {

      int totalCnt = 0;

      AdminRepository adr = new AdminRepository();
      UserListModel model = new UserListModel()
      {

        UserList = adr.SelectUserList(page, pageSize, isRetire, SearchString, out totalCnt),
        PagingInfo = new PagingInfo()
        {
          CurrentPage = page,
          ItemsPerPage = pageSize,
          TotalItems = totalCnt
        }
      };
      ViewBag.Title = "End User";
      ViewBag.searchString = SearchString;
      ViewBag.isRetire = isRetire;
      return View(model);
    }
    #endregion
    #region 0.2 신규/수정 관련
    // 공지사항 신규/수정
    public async Task<ActionResult> UserCreate(int uv_seq = 0)
    {
      AdminRepository adr = new AdminRepository();


      var data = await adr.SelectUserOneAsync(uv_seq);

      if (data == null)
        data = new uv_user();

      ViewBag.Title = "End User";

      //return View(model);
      return View(data);
    }

    #endregion
    #region 0.3 사용자 상세보기
    //공지사항 상세보기
    public async Task<ActionResult> UserDetail(int uv_seq = 0)
    {
      AdminRepository adr = new AdminRepository();

      var data = await adr.SelectUserOneAsync(uv_seq);

      ViewBag.Title = "End User";

      return View(data);

    }
    #endregion
    #region 0.4 사용자 저장
    // 사용자 신규/수정 (저장)
    [HttpPost]
    public async Task<JsonResult> UserCreate(uv_user model, HttpPostedFileBase file)
    {
      try
      {


        FileUpload fiUpload = new FileUpload();
        string path = Server.MapPath("~/UploadedFiles");
        string sub_path = "USER";
        UploadFileResult file_result = new UploadFileResult();
        if (file != null)
        {
          file_result = fiUpload.UploadCommon(path, sub_path, file);

          if (!file_result.status)
          {
            return Json(new
            {
              ok = false,
              message = "Upload Error - " + file_result.statusMessage
            });
          }
          else
          {
            model.img_dir = file_result.dbPath;
            model.img_origin_path = file_result.filePath;
            model.img_path = file_result.name;
          }
        }


        AdminEntityRepository ader = new AdminEntityRepository();
        AdminRepository ar = new AdminRepository();

        //신규, 수정 state 변수
        string state = "";

        model.modify_dt = Utils.NowKorea();
        //model.modify_user = AppIdentity.user_seq;

        //신규

        //TODO 패스워드 암호화 필요
        if (model.uv_seq == 0)
        {
          model.create_dt = Utils.NowKorea();

          state = CommonCodes.Create;
        }
        //수정
        else
        {
          uv_user t_model = await ar.SelectUserOneAsync(model.uv_seq);
          if (model.pwd == "" || model.pwd == null)
          {
            model.pwd = t_model.pwd;
          }

          if (model.img_path == "" || model.img_path == null)
          {
            model.img_path = t_model.img_path;
            model.img_origin_path = t_model.img_origin_path;
            model.img_dir = t_model.img_dir;
          }

          state = CommonCodes.Update;
        }

        await ader.CreateOrUpdateUser(model, state, state, AppIdentity.user_seq, 1);

        return Json(new
        {
          ok = true,
          message = "Success",
          uv_seq = model.uv_seq
        }); ;
      }
      catch (Exception e)
      {
        return Json(new
        {
          ok = false,
          message = "Failed" + e.Message
        });
      }
    }
    #endregion
    #region 0.5 사용자 삭제
    //사용자 삭제
    [HttpPost]
    public async Task<JsonResult> UserRemove(int uv_seq)
    {
      try
      {
        AdminEntityRepository ader = new AdminEntityRepository();


        uv_user model = await ader.SelectUserOneAsync(uv_seq);


        if (model == null)
          return Json(new
          {
            ok = false
              ,
            message = "Can not Find User."
          });

        //TODO 파일 정리 프로세스 추가 필요함
        await ader.DeleteUser(model, "D", AppIdentity.user_seq, 1);

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
          message = "알 수 없는 오류가 발생 했습니다." + e.Message
        });
      }
    }
    #endregion
    #endregion
    /*
    public async Task<JsonResult> LoginFromAdmin(int uv_seq = 0)
    {
        AdminRepository adr = new AdminRepository();

        var data = await adr.SelectUserOneAsync(uv_seq);

        return await new AccountController().Login(data.user_id, data.pwd);
    }
    */
  }
}