using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Security.Claims;
using System.Threading.Tasks;
using Univision.Core.Models;
using Univision.Core.Models.DTO;
using Univision.Core.Repositories;
using Univision.Core.Models.DTO.Response.Board;
using Univision.Core.Models.DTO.Request.Board;
using Univision.Core.Models.DTO.Request.Client;
using Univision.Core.Models.DTO.Response.Client;
using Univision.Core.Models.DTO.Response.Project;
using Univision.Main.Models.Project;
using Univision.Main.Infrastructure;
using Univision.Security;
using Univision.Core.Models.DTO.Response.Dashboard;
using Univision.Main.Models.DashBoard;

namespace Univision.Main.Controllers
{
    public class DashboardController : BaseController
    {   

        /// <summary>
        /// 대쉬보드 메인
        /// </summary>
        /// <returns></returns>
        public async Task<ActionResult> Index()
        {
            DashboardRepository dr = new DashboardRepository();

            DateTime startDate = Utils.NowKorea().AddDays(1 - Utils.NowKorea().Day);
            DateTime endDate = startDate.AddMonths(1).AddDays(-1);

            string startDt = startDate.ToString("yyyy-MM-dd");
            string endDt = endDate.ToString("yyyy-MM-dd");

            var dayCnt = 0;

            for(int i = 0; i < DateTime.DaysInMonth(startDate.Year, startDate.Month); i++)
            {
                if (startDate.DayOfWeek != DayOfWeek.Saturday && startDate.DayOfWeek != DayOfWeek.Sunday)
                    dayCnt++;

                startDate = startDate.AddDays(1);
            }


            DashBoardTopSummaryModel model = new DashBoardTopSummaryModel()
            {
                rCandidateCnt = await dr.CountRegRecandidateMonth(AppIdentity.user_seq, startDt, endDt)
                ,
                intvCandidateCnt = await dr.CountInterviewRecandidateMonth(AppIdentity.user_seq, startDt, endDt)
                ,
                MemoCnt = await dr.CountMemoMonth(AppIdentity.user_seq, startDt, endDt)
                ,
                sCandidateCnt = await dr.CountRegSimpleCandidateWeek(AppIdentity.user_seq, startDt, endDt)
                ,
                candidateCnt = await dr.CountRegCandidateMonth(AppIdentity.user_seq, startDt, endDt)
                ,
                candidateUdCnt = await dr.CountUpdateCandidateMonth(AppIdentity.user_seq, startDt, endDt)
                ,
                pjtCnt = await dr.CountNowProgressPjt(AppIdentity.user_seq, ProjectStatusCode.progress)
                ,
                usedVacation = await dr.CountUsedVacation(AppIdentity.user_seq, Utils.NowKorea().Year.ToString())
                ,
                totalVacation = await dr.CountTotalVacation(AppIdentity.user_seq, Utils.NowKorea().Year.ToString())
                ,
                contract_update_date = await dr.SelectBoardUpdateDate(1441)
                ,
                kr_intro_update_date = await dr.SelectBoardUpdateDate(1350)
                ,
                en_intro_update_date = await dr.SelectBoardUpdateDate(1351)
            };

            return View(model);
        }

        /// <summary>
        /// 미팅룸 현황
        /// </summary>
        /// <returns></returns>
        public async Task<PartialViewResult> MeetingRoomStatusList()
        {
            DashboardRepository dr = new DashboardRepository();

            var list = await dr.SelectMeetingRoomStatusListAsync();

            ViewBag.is_meetingroom_admin = (AppIdentity.ua_seq == 1 || AppIdentity.ua_seq == 2 ? true : false);

            return PartialView(list);
        }

        /// <summary>
        /// 주간 캘린더
        /// </summary>
        /// <returns></returns>
        public PartialViewResult WeeklyCalendarView()
        {
            return PartialView();
        }

        /// <summary>
        /// 공지 목록
        /// </summary>
        /// <param name="page"></param>
        /// <param name="SelectOption"></param>
        /// <param name="SearchString"></param>
        /// <returns></returns>
        public PartialViewResult NoticeList(int page = 1, string SelectOption = "", string SearchString = "")
        {
            int totalCount = 0;
            int bType = 0; //내부:0, 외부:1, 참고자료:2 
            int bTypeSub1 = 10; //자료실용 대분류(자료실 외에는 0)
            int bTypeSub2 = 10; //자료실용 소분류(자료실 외에는 0)
            int totalCnt = 0;
            int pageSize = AppPaging.PageLength5;

            BoardRepository bd = new BoardRepository();
            BoardListModel model = new BoardListModel()
            {
                BoardList = bd.BoardList(bType, bTypeSub1, bTypeSub2, page, pageSize, SelectOption, SearchString, out totalCnt),
                PagingInfo = new PagingInfo()
                {
                    CurrentPage = page,
                    ItemsPerPage = pageSize,
                    TotalItems = totalCnt
                }
            };

            //var list = bd.BoardList(bType, bTypeSub1, bTypeSub2, page, pageSize, SelectOption, SearchString, out totalCnt);



            return PartialView(model);
        }

        /// <summary>
        /// 업체 목록
        /// </summary>
        /// <param name="page"></param>
        /// <returns></returns>
        public PartialViewResult ClientList(int page = 1)
        {
            int totalCount = 0;
            int pageSize = AppPaging.PageLength5;

            ClientRepository cr = new ClientRepository();

            var list = cr.clientListTop5();

            ClientListViewModel model = new ClientListViewModel
            {
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
        
        public PartialViewResult VacationApprList(int page = 1)
        {
            int totalCount = 0;

            MyListRepository mlr = new MyListRepository();

            List<uv_vacation_history> model = new List<uv_vacation_history>();
            model = mlr.SelectMyVacationApprovalList(AppIdentity.user_seq);

            

            return PartialView(model);
        }

        /// <summary>
        /// 오픈 프로젝트 목록
        /// </summary>
        /// <param name="page"></param>
        /// <returns></returns>
        public PartialViewResult OpenProjectList(int page = 1)
        {
            int totalCount = 0;
            int pageSize = 5;


            ProjectRepository pr = new ProjectRepository();

            var list = pr.openprjListTop5(page -1, pageSize);

            MyProjectListModel model = new MyProjectListModel()
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

        /// <summary>
        /// 오픈 프로젝트 목록
        /// </summary>
        /// <param name="page"></param>
        /// <returns></returns>
        public PartialViewResult CoworkProjectList(int page = 1)
        {
            int totalCount = 0;
            int pageSize = AppPaging.PageLength5;


            ProjectRepository pr = new ProjectRepository();

            var list = pr.CoworkPrjListTop5();

            MyProjectListModel model = new MyProjectListModel()
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

        /// <summary>
        /// 발행된 인보이스 목록
        /// </summary>
        /// <param name="page"></param>
        /// <returns></returns>
        public async Task<PartialViewResult> BilledInvoiceList(int width = 0)
        {
            DashboardRepository dr = new DashboardRepository();
            
            var list = await dr.SelectBilledInvoiceListAsync(AppIdentity.user_seq);

            ProjectInvoiceListModel model = new ProjectInvoiceListModel()
            {   
                list = list,
            };

            ViewBag.windowWidth = width;

            return PartialView(model);
        }

        /// <summary>
        /// 미발행 인보이스 목록
        /// </summary>
        /// <returns></returns>
        public async Task<PartialViewResult> UnBilledInvoiceList(int width = 0)
        {
            DashboardRepository dr = new DashboardRepository();

            var list = await dr.SelectUnBilledInvoiceListAsync(AppIdentity.user_seq, ProjectHistoryStateCode.hireOk);

            UnBilledInvoiceListModel model = new UnBilledInvoiceListModel()
            {
                list = list
            };

            ViewBag.windowWidth = width;

            return PartialView(model);
        }

        public async Task<PartialViewResult> KPIList()
        {
            DashboardRepository dr = new DashboardRepository();

            var list = await dr.SelectKpiDataListAsync(AppIdentity.user_seq);

            return PartialView(list);
        }
    }
}