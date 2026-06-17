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
using Univision.Core.Models.DTO.Response.Board;
using Univision.Core.Models.DTO.Request.Board;
using Univision.Main.Infrastructure;
using Univision.Security;

namespace Univision.Main.Controllers
{
    public class BoardController : BaseController
    {
        #region 99.공통
        #region 99.1 첨부파일관련 부분 뷰
        //첨부파일 부분 뷰 처리
        public PartialViewResult BoardCreateFileList(BoardDetailViewModel model)
        {
            return PartialView(model);
        }

        //첨부파일 임시 업로드후 부분뷰 처리
        [HttpPost]
        public PartialViewResult BoardCreateFileList(BoardDetailViewModel model, board_file[] files)
        {
            if (model == null) {
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
        public PartialViewResult BoardRemoveFileList(BoardDetailViewModel model, int file_seq)
        {
            if (model == null)
            {
                model = new BoardDetailViewModel();
            }

            if (model.boardFileList == null)
                model.boardFileList = new List<board_file>();


            model.boardFileList.RemoveAt(file_seq);
            //model.boardFileList[file_seq].file_dir = "";            
            return PartialView("BoardCreateFileList", model);
            //return PartialView(model);
        }
    #endregion
    #region 99.2 첨부파일 임시 업로드 처리

    [HttpPost]
    public async Task<JsonResult> UploadSummerNoteAttach(HttpPostedFileBase file)
    {
      try
      {
        FileUpload fiUpload = new FileUpload();
        string path = Server.MapPath("~/UploadedFiles");
        var rst = fiUpload.UploadDirectorActivity(path, "Board", "summernote", file, true);
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
    /// <summary>
    /// 파일 임시업로드 처리
    /// </summary>
    /// <param name="uploadFolder"></param>
    /// <param name="file"></param>
    /// <returns></returns>
    [HttpPost]
        public async Task<JsonResult> CreateTempBoardFile(string uploadFolder, HttpPostedFileBase file)
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
                    message = "알 수 없는 오류가 발생 했습니다." + e.Message
                });
            }
        }
        #endregion
        #region 99.3 게시글 저장(공통)
        // 게시판 신규/수정 (저장)
        [ValidateInput(false)]
        [HttpPost]        
        public async Task<JsonResult> BoardCreate(BoardCreateUpdateModel model)
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

                return Json(new
                {
                    ok = true,
                    message = "Success",
                    b_seq = model.data.b_seq
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
        #region 99.4 게시글 삭제(공통)
        //게시글 삭제(공통)
        [HttpPost]
        public async Task<JsonResult> BoardRemove(int b_seq)
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
                    message = "알 수 없는 오류가 발생 했습니다." + e.Message
                });
            }
        }
    #endregion

    public async Task<ActionResult> AllBoardList(BoardListSearch search, int page = 1)
    {

      //한 페이지당 몇개의 행을 보여줄 것인지
      int pageSize = AppPaging.PageLength20;
      int totalCnt = 0;
      int TopBoardCnt = 0;
      int topTotalCnt = 0;

      ViewBag.Title = "전체 자료 검색";

      BoardRepository br = new BoardRepository();

      BoardListModel model = new BoardListModel()
      {
        search = search,
        BoardList = br.AllBoardList(page, pageSize, search.search_option, search.search_txt, out totalCnt),
        PagingInfo = new PagingInfo()
        {
          CurrentPage = page,
          ItemsPerPage = pageSize,
          TotalItems = totalCnt
        }
      };

      //ViewBag.nodeId = nodeId;

      return View(model);
      //return View(model);
    }

    #region 99.5 더미 컨트롤러(BoardList/BoardCreate)
    public ActionResult BoardList()
        {
            //log out the user
            return RedirectToAction("NoticeList");
        }
        public ActionResult BoardCreate()
        {
            //log out the user
            return RedirectToAction("NoticeCreate");
        }
        #endregion
        #endregion
        #region 00.대내업무
        #region 00.1 리스트
        //자료실 리스트
        public async Task<ActionResult> InternalList(BoardListSearch search, int page = 1)
        {

            //한 페이지당 몇개의 행을 보여줄 것인지
            int pageSize = AppPaging.PageLength20;
            search.b_type = 0; //대내:0, 대외:1, 자료실:2
            search.b_type_sub1 = (search.b_type_sub1 > 0 ? search.b_type_sub1 : 10);
            search.b_type_sub2 = (search.b_type_sub2 > 0 ? search.b_type_sub2 : 0);
            int totalCnt = 0;
            int TopBoardCnt = 0;
            int topTotalCnt = 0;

            ViewBag.Title = "유니코 내부";
            ViewBag.pageTag = "Internal";

            BoardRepository br = new BoardRepository();

            BoardListModel model = new BoardListModel()
            {
                search = search,
                DocumentTypeList = await GetDocumentTypeModel(ViewBag.pageTag, search.b_type, search.b_type_sub1, search.b_type_sub2),
                TopBoardList = br.BoardList(search.b_type, search.b_type_sub1, search.b_type_sub2, 1, 3, "b_top", "1", out topTotalCnt),
                BoardList = br.BoardList(search.b_type, search.b_type_sub1, search.b_type_sub2, page, pageSize, search.search_option, search.search_txt, out totalCnt),
                PagingInfo = new PagingInfo()
                {
                    CurrentPage = page,
                    ItemsPerPage = pageSize,
                    TotalItems = totalCnt
                }
            };
            
            //ViewBag.nodeId = nodeId;

            return View("BoardNewList", model);
            //return View(model);
        }
        #endregion
        #region 00.2 신규/수정 관련
        // 문서게시판 신규/수정
        public async Task<ActionResult> InternalCreate(int b_seq = 0, int b_type_sub1 = 10, int b_type_sub2 = 10)
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
                    b_type = 0,
                    b_type_sub1 = b_type_sub1,
                    b_type_sub2 = b_type_sub2,
                    b_type_sub_name = await bd.SelectDocCategoryAsOne(0, b_type_sub1, b_type_sub2)
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
            ViewBag.Title = "유니코 내부";
            ViewBag.pageTag = "Internal";

            //return View(model);
            return View("BoardNewCreate", model);
        }
        #endregion
        #region 00.3 상세보기
        //문서게시판 상세보기
        public async Task<ActionResult> InternalDetail(int b_seq = 0)
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

            ViewBag.Title = "유니코 내부";
            ViewBag.pageTag = "Internal";

            return View("BoardNewDetail", model);

        }
        #endregion
        #endregion


        #region 01.대외업무
        #region 01.1 리스트
        //자료실 리스트
        public async Task<ActionResult> ExternalList(BoardListSearch search, int page = 1)
        {

            //한 페이지당 몇개의 행을 보여줄 것인지
            int pageSize = AppPaging.PageLength20;
            search.b_type = 1; //대내:0, 대외:1, 자료실:2
            search.b_type_sub1 = (search.b_type_sub1 > 0 ? search.b_type_sub1 : 41);
            search.b_type_sub2 = (search.b_type_sub2 > 0 ? search.b_type_sub2 : 0);
            int totalCnt = 0;
            int TopBoardCnt = 0;
            int topTotalCnt = 0;

            ViewBag.Title = "고객사 및 후보자";
            ViewBag.pageTag = "External";

            BoardRepository br = new BoardRepository();

            BoardListModel model = new BoardListModel()
            {
                search = search,
                DocumentTypeList = await GetDocumentTypeModel(ViewBag.pageTag, search.b_type, search.b_type_sub1, search.b_type_sub2),
                TopBoardList = br.BoardList(search.b_type, search.b_type_sub1, search.b_type_sub2, 1, 3, "b_top", "1", out topTotalCnt),
                BoardList = br.BoardList(search.b_type, search.b_type_sub1, search.b_type_sub2, page, pageSize, search.search_option, search.search_txt, out totalCnt),
                PagingInfo = new PagingInfo()
                {
                    CurrentPage = page,
                    ItemsPerPage = pageSize,
                    TotalItems = totalCnt
                }
            };

            //ViewBag.nodeId = nodeId;

            return View("BoardNewList", model);
            //return View(model);
        }
        #endregion
        #region 01.2 신규/수정 관련
        // 문서게시판 신규/수정
        public async Task<ActionResult> ExternalCreate(int b_seq = 0, int b_type_sub1 = 40, int b_type_sub2 = 10)
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
                    b_type = 1,
                    b_type_sub1 = b_type_sub1,
                    b_type_sub2 = b_type_sub2,
                    b_type_sub_name = await bd.SelectDocCategoryAsOne(1, b_type_sub1, b_type_sub2)
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
            ViewBag.Title = "고객사 및 후보자";
            ViewBag.pageTag = "External";

            //return View(model);
            return View("BoardNewCreate", model);
        }
        #endregion
        #region 00.3 상세보기
        //문서게시판 상세보기
        public async Task<ActionResult> ExternalDetail(int b_seq = 0)
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

            ViewBag.Title = "고객사 및 후보자";
            ViewBag.pageTag = "External";

            return View("BoardNewDetail", model);

        }
        #endregion
        #endregion

        #region 02.자료실
        #region 02.1 리스트
        //자료실 리스트
        public async Task<ActionResult> LibraryList(BoardListSearch search, int page = 1)
        {

            //한 페이지당 몇개의 행을 보여줄 것인지
            int pageSize = AppPaging.PageLength20;
            search.b_type = 2; //대내:0, 대외:1, 자료실:2
            search.b_type_sub1 = (search.b_type_sub1 > 0 ? search.b_type_sub1 : 60);
            search.b_type_sub2 = (search.b_type_sub2 > 0 ? search.b_type_sub2 : 0);
            int totalCnt = 0;
            int TopBoardCnt = 0;
            int topTotalCnt = 0;

            ViewBag.Title = "자료실";
            ViewBag.pageTag = "Library";

            BoardRepository br = new BoardRepository();

            BoardListModel model = new BoardListModel()
            {
                search = search,
                DocumentTypeList = await GetDocumentTypeModel(ViewBag.pageTag, search.b_type, search.b_type_sub1, search.b_type_sub2),
                TopBoardList = br.BoardList(search.b_type, search.b_type_sub1, search.b_type_sub2, 1, 3, "b_top", "1", out topTotalCnt),
                BoardList = br.BoardList(search.b_type, search.b_type_sub1, search.b_type_sub2, page, pageSize, search.search_option, search.search_txt, out totalCnt),
                PagingInfo = new PagingInfo()
                {
                    CurrentPage = page,
                    ItemsPerPage = pageSize,
                    TotalItems = totalCnt
                }
            };

            //ViewBag.nodeId = nodeId;

            return View("BoardNewList", model);
            //return View(model);
        }
        #endregion
        #region 01.2 신규/수정 관련
        // 문서게시판 신규/수정
        public async Task<ActionResult> LibraryCreate(int b_seq = 0, int b_type_sub1 = 70, int b_type_sub2 = 10)
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
                    b_type = 2,
                    b_type_sub1 = b_type_sub1,
                    b_type_sub2 = b_type_sub2,
                    b_type_sub_name = await bd.SelectDocCategoryAsOne(2, b_type_sub1, b_type_sub2)
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
            ViewBag.Title = "자료실";
            ViewBag.pageTag = "Library";

            //return View(model);
            return View("BoardNewCreate", model);
        }
        #endregion
        #region 00.3 상세보기
        //문서게시판 상세보기
        public async Task<ActionResult> LibraryDetail(int b_seq = 0)
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

            ViewBag.Title = "자료실";
            ViewBag.pageTag = "Library";

            return View("BoardNewDetail", model);

        }
        #endregion
        #endregion






        #region 0.공지사항
        #region 0.1 리스트
        // 공지사항 리스트
        public async Task<ActionResult> NoticeList(int page = 1, int pageSize = 20, string SelectOption = "", string SearchString = "")
        {
            
            int bType = 0; //공지사항:0, 교육게:1, 지식게:2, 자유게:3, 파일게:4
            int bTypeSub1 = 0; //자료실용 대분류(자료실 외에는 0)
            int bTypeSub2 = 0; //자료실용 소분류(자료실 외에는 0)
            int totalCnt = 0;
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
            ViewBag.Title   = "Notice";
            ViewBag.pageTag = "Notice";
            ViewBag.searchOption = SelectOption;
            ViewBag.searchString = SearchString;

            return View("BoardList", model);
        }
        #endregion
        #region 0.2 신규/수정 관련
        // 공지사항 신규/수정
        public async Task<ActionResult> NoticeCreate(int b_seq = 0)
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
                    b_type = 0,
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
            ViewBag.Title = "Notice";
            ViewBag.pageTag = "Notice";

            //return View(model);
            return View("BoardCreate", model);
        }

        #endregion
        #region 0.3 상세보기
        //공지사항 상세보기
        public async Task<ActionResult> NoticeDetail(int b_seq = 0)
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
            } else
            {
                read_data.read_dt = Utils.NowKorea();
                state = CommonCodes.Update;
            }
            await ber.CreateOrUpdateBoardRead(read_data, state);



            ViewBag.Title = "Notice";
            ViewBag.pageTag = "Notice";

            return View("BoardDetail", model);

        }
        #endregion
        #endregion
        
        #region 1.교육 게시판
        #region 1.1 리스트
        //교육게시판 리스트
        public async Task<ActionResult> EducationList(int page = 1, int pageSize = 20, string SelectOption = "", string SearchString = "")
        {
            int bType = 1; //공지사항:0, 교육게:1, 지식게:2, 자유게:3, 파일게:4
            int bTypeSub1 = 0;
            int bTypeSub2 = 0;
            int totalCnt = 0;
            int TopBoardCnt = 0;

            BoardRepository bd = new BoardRepository();
            BoardListModel model = new BoardListModel()
            {
                TopBoardList = new List<board>(),
                BoardList = bd.BoardList(bType, bTypeSub1, bTypeSub2, page, pageSize, SelectOption, SearchString, out totalCnt),
                PagingInfo = new PagingInfo()
                {
                    CurrentPage = page,
                    ItemsPerPage = pageSize,
                    TotalItems = totalCnt
                }
            };
            ViewBag.Title = "Education";
            ViewBag.pageTag = "Education";
            ViewBag.searchOption = SelectOption;
            ViewBag.searchString = SearchString;

            return View("BoardList", model);
        }
        #endregion
        #region 1.2 신규/수정 관련
        // 교육게시판 신규/수정
        public async Task<ActionResult> EducationCreate(int b_seq = 0)
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
                    b_type = 1,
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
            ViewBag.Title = "Education";
            ViewBag.pageTag = "Education";

            //return View(model);
            return View("BoardCreate", model);
        }
        #endregion
        #region 1.3 상세보기
        //교육게시판 상세보기
        public async Task<ActionResult> EducationDetail(int b_seq = 0)
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

            ViewBag.Title = "Education";
            ViewBag.pageTag = "Education";

            return View("BoardDetail", model);

        }
        #endregion
        #endregion

        #region 2.지식검색 게시판
        #region 2.1 리스트
        //자료실 리스트
        public async Task<ActionResult> KnowledgeList(int page = 1, int pageSize = 20, string SelectOption = "", string SearchString = "")
        {
            int bType = 2; //공지사항:0, 교육게:1, 지식게:2, 자유게:3, 파일게:4
            int bTypeSub1 = 0;
            int bTypeSub2 = 0;
            int totalCnt = 0;
            int TopBoardCnt = 0;

            BoardRepository bd = new BoardRepository();
            BoardListModel model = new BoardListModel()
            {
                TopBoardList = new List<board>(),
                BoardList = bd.BoardList(bType, bTypeSub1, bTypeSub2, page, pageSize, SelectOption, SearchString, out totalCnt),
                PagingInfo = new PagingInfo()
                {
                    CurrentPage = page,
                    ItemsPerPage = pageSize,
                    TotalItems = totalCnt
                }
            };
            ViewBag.Title = "Market Insight";
            ViewBag.pageTag = "Knowledge";
            ViewBag.searchOption = SelectOption;
            ViewBag.searchString = SearchString;

            return View("BoardList", model);
        }
        #endregion
        #region 1.2 신규/수정 관련
        // 지식검색 게시판 신규/수정
        public async Task<ActionResult> KnowledgeCreate(int b_seq = 0)
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
                    b_type = 2,
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
            ViewBag.Title = "Market Insight";
            ViewBag.pageTag = "Knowledge";

            //return View(model);
            return View("BoardCreate", model);
        }
        #endregion
        #region 2.3 상세보기
        //지식검색게시판 상세보기
        public async Task<ActionResult> KnowledgeDetail(int b_seq = 0)
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

            ViewBag.Title = "Market Insight";
            ViewBag.pageTag = "Knowledge";

            return View("BoardDetail", model);

        }
        #endregion
        #endregion

        #region 3.자유 게시판
        #region 3.1 리스트
        //자료실 리스트
        public async Task<ActionResult> FreeBoardList(int page = 1, int pageSize = 20, string SelectOption = "", string SearchString = "")
        {
            int bType = 3; //공지사항:0, 교육게:1, 지식게:2, 자유게:3, 파일게:4
            int bTypeSub1 = 0;
            int bTypeSub2 = 0;
            int totalCnt = 0;
            int TopBoardCnt = 0;

            BoardRepository bd = new BoardRepository();
            BoardListModel model = new BoardListModel()
            {
                TopBoardList = new List<board>(),
                BoardList = bd.BoardList(bType, bTypeSub1, bTypeSub2, page, pageSize, SelectOption, SearchString, out totalCnt),
                PagingInfo = new PagingInfo()
                {
                    CurrentPage = page,
                    ItemsPerPage = pageSize,
                    TotalItems = totalCnt
                }
            };
            ViewBag.Title = "Free board";
            ViewBag.pageTag = "FreeBoard";
            ViewBag.searchOption = SelectOption;
            ViewBag.searchString = SearchString;

            return View("BoardList", model);
        }
        #endregion
        #region 3.2 신규/수정 관련
        // 자유게시판 신규/수정
        public async Task<ActionResult> FreeBoardCreate(int b_seq = 0)
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
                    b_type = 3,
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
            ViewBag.Title = "Free Board";
            ViewBag.pageTag = "FreeBoard";

            //return View(model);
            return View("BoardCreate", model);
        }
        #endregion
        #region 3.3 상세보기
        //자유게시판 상세보기
        public async Task<ActionResult> FreeBoardDetail(int b_seq = 0)
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

            ViewBag.Title = "Free Board";
            ViewBag.pageTag = "FreeBoard";

            return View("BoardDetail", model);

        }
        #endregion
        #endregion

        #region 4.문서게시판
        #region 4.1 리스트
        //자료실 리스트
        public async Task<ActionResult> DocumentList(int page = 1, int pageSize = 20, int bTypeSub1 = 0, int bTypeSub2 = 0, int nodeId = 0, string SelectOption = "", string SearchString = "")
        {
            int bType = 4; //공지사항:0, 교육게:1, 지식게:2, 자유게:3, 파일게:4
            int totalCnt = 0;
            int TopBoardCnt = 0;

            BoardRepository br = new BoardRepository();

            BoardListModel model = new BoardListModel()
            {
                DocumentTypeList = await GetDocumentTypeModel("", bTypeSub1, bTypeSub2),
                TopBoardList = new List<board>(),
                BoardList = br.BoardList(bType, bTypeSub1, bTypeSub2, page, pageSize, SelectOption, SearchString, out totalCnt),
                PagingInfo = new PagingInfo()
                {
                    CurrentPage = page,
                    ItemsPerPage = pageSize,
                    TotalItems = totalCnt
                }
            };
            ViewBag.Title = "Document";
            ViewBag.pageTag = "Document";
            ViewBag.searchOption = SelectOption;
            ViewBag.searchString = SearchString;
            ViewBag.nodeId = nodeId;

            //return View("BoardList", model);
            return View(model);
        }
        #endregion
        #region 4.2 신규/수정 관련
        // 문서게시판 신규/수정
        public async Task<ActionResult> DocumentCreate(int b_seq = 0)
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
                    b_type = 4,
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
            ViewBag.Title = "Document";
            ViewBag.pageTag = "Document";

            //return View(model);
            return View("BoardCreate", model);
        }
        #endregion
        #region 4.3 상세보기
        //문서게시판 상세보기
        public async Task<ActionResult> DocumentDetail(int b_seq = 0)
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

            ViewBag.Title = "Document";
            ViewBag.pageTag = "Document";

            return View("BoardDetail", model);

        }
        #endregion
        #region 4.4 문서게시판 게시판 구분 트리뷰용(미사용)
        [HttpPost]
        public async Task<JsonResult> DocumentCategory(int code1 = 0, int code2 = 0)
        {
            BoardRepository br = new BoardRepository();
            List<DocumentTypeModel> result_model = new List<DocumentTypeModel>();
            try
            {
                result_model = await GetDocumentTypeModel("", code1, code2);
            }
            catch(Exception e)
            {
                
            }

            return Json(result_model);

        }
        #endregion
        #region 4.5 문서게시판 게시판 구분 선택 부분뷰
        [HttpPost]
        public async Task<PartialViewResult> BoardTypeSelectApi(int type = 0)
        {
            BoardRepository br = new BoardRepository();

            List<code_board_sub1> list = await br.SelectDocumentType1ListAsync(type, 0);

            return PartialView(list);
        }

        [HttpPost]
        public async Task<JsonResult> BoardType2SelectApi(int type = 0, int code1 = 0)
        {
            try
            {
                BoardRepository br = new BoardRepository();

                List<code_board_sub2> list = await br.SelectDocumentType2ListAsync(type, code1, 0);

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
        #endregion
        #region 4.99 게시판 구분 모델 조합용
        public async Task<List<DocumentTypeModel>> GetDocumentTypeModel(string page_tag = "Internal", int type = 0, int code1 = 0, int code2 = 0)
        {
            List<DocumentTypeModel> result_model = new List<DocumentTypeModel>();

            BoardRepository br = new BoardRepository();
            List<code_board_sub1> code1_list;

            code1_list = await br.SelectDocumentType1ListAsync(type, 0);

            if (code1_list != null && code1_list.Count > 0)
            {
                int i = 0;
                /*
                result_model.Add(new DocumentTypeModel()
                {
                    text = "전체",
                    href = Url.Action(page_tag + "List", "Board"),
                    tags = new List<string>() { "0" },
                    is_select = (code1 == 0 ? true : false)
                });
                */
                foreach (code_board_sub1 sub_code1 in code1_list)
                {

                    List<code_board_sub2> code2_list = new List<code_board_sub2>();
                    code2_list = await br.SelectDocumentType2ListAsync(type, sub_code1.code1, 0);

                    List<DocumentTypeModel> sub_nodes = null;
                    if (code2_list != null && code2_list.Count > 0)
                    {
                        sub_nodes = new List<DocumentTypeModel>();

                        sub_nodes.Add(new DocumentTypeModel()
                        {
                            text = "전체" + " [" + sub_code1.sub1_doc_count + "]",
                            href = Url.Action(page_tag + "List", "Board", new { b_type_sub1 = sub_code1.code1, b_type_sub2 = 0 }),
                            tags = new List<string>() { "0" },
                            is_select = (code1 == sub_code1.code1 && code2 == 0 ? true : false)
                        });
                        foreach (code_board_sub2 sub_code2 in code2_list)
                        {
                            sub_nodes.Add(
                                   new DocumentTypeModel()
                                   {

                                       text = sub_code2.code_name + " [" + sub_code2.sub2_doc_count + "]",
                                       href = Url.Action(page_tag + "List", "Board", new { b_type_sub1 = sub_code2.code1, b_type_sub2 = sub_code2.code2 }),
                                       tags = new List<string>() { "0" },
                                       is_select = (code1 == sub_code2.code1 && code2 == sub_code2.code2 ? true : false)
                                   });
                        }
                    }

                    result_model.Add(new DocumentTypeModel()
                    {
                        text = sub_code1.code_name + " [" + sub_code1.sub1_doc_count + "]",
                        href = Url.Action(page_tag + "List", "Board", new { b_type_sub1 = sub_code1.code1 }),
                        tags = new List<string>() { sub_code1.code1.ToString() },
                        is_select = (code1 == sub_code1.code1 ? true : false),
                        nodes = sub_nodes
                    });



                }
            }
            return result_model;
        }
        #endregion
        #endregion
    }
}