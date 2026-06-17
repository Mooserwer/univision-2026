using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Univision.Core.Models;
using Univision.Core.Models.DTO;
using Univision.Core.Models.DTO.Response;
using Univision.Core.Models.DTO.Response.Project;
using Univision.Core.Models.DTO.Response.Statistics;
using Univision.Core.Repositories;
using Univision.Security;

namespace Univision.Main.Controllers
{
  public class StatisticsController : BaseController
  {
    #region 기간별 통계 - 팀

    /// <summary>
    /// 기간별 통계 - 팀별
    /// </summary>
    /// <param name="year"></param>
    /// <param name="month"></param>
    /// <param name="ud_seq"></param>
    /// <returns></returns>
    public async Task<ActionResult> PeriodSalesTeam(int year = 0, int month = 0, int ud_seq = 0)
    {
      StatisticsRepository sr = new StatisticsRepository();

      var yearMonthData = await sr.SumBillingYearMonthListAsync();
      var yearList = yearMonthData.GroupBy(x => x.billing_year).OrderByDescending(y => y.Max(x => x.billing_year)).ToList().Select(y => new YearMonthModel() { key = y.Key, value = y.Key + "년" }).ToList();
      //var monthList = yearMonthData.Select(x => new YearMonthModel() { key = x.billing_month, value = x.billing_month + "월" }).ToList();

      if (year == 0)
        year = yearList.FirstOrDefault().key;

      if (month == 0)
        month = Utils.NowKorea().Month;
      //month = monthList.LastOrDefault().key;

      AccountRepository ar = new AccountRepository();
      var divisionList = await ar.SelectDivisionList();

      if (ud_seq == 0)
        ud_seq = AppIdentity.ud_seq;

      string startDt = year + "-01-01";
      string endDt = year + "-12-31";

      string startDt2 = year.ToString() + "-" + month.ToString() + "-01";
      string endDt2 = new DateTime(year, month, 1).AddMonths(1).AddDays(-1).ToString("yyyy-MM-dd");

      var monthlySalesList = await sr.SelectSalesMonthlySumList(startDt, endDt, ud_seq, 0);
      var divisonUserSalesList = await sr.SelectSalesDivisionUserList(startDt2, endDt2, ud_seq);

      PeriodSalesModel model = new PeriodSalesModel()
      {
        year = year,
        month = month,
        ud_seq = ud_seq,
        yearList = yearList,
        //monthList = monthList,
        divisionList = divisionList,
        salesMonthlySumList = monthlySalesList,
        salesDivisionUserList = divisonUserSalesList
      };

      return View(model);
    }

    /// <summary>
    /// 기간별 통계 - 팀별 (하단 리스트)
    /// </summary>
    /// <param name="year"></param>
    /// <param name="page"></param>
    /// <returns></returns>
    public async Task<PartialViewResult> PeriodSalesTeamList(string year = "", int ud_seq = 0, int page = 1)
    {
      StatisticsRepository sr = new StatisticsRepository();

      int totalCount = 0;

      //한 페이지당 몇개의 행을 보여줄 것인지
      int pageSize = AppPaging.PageLength10;

      if (string.IsNullOrWhiteSpace(year))
        year = Utils.NowKorea().Year.ToString();

      string startDt = year + "-01-01";
      string endDt = year + "-12-31";

      var sumData = await sr.SumPeriodSalesAllAsync(startDt, endDt, ud_seq, 0);
      var list = sr.SelectPeriodSalesAllList(startDt, endDt, ud_seq, 0, (page - 1) * pageSize, pageSize, out totalCount);

      SalesDetailListModel model = new SalesDetailListModel()
      {
        year = year
          ,
        ud_seq = ud_seq
          ,
        sumData = sumData
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
    /// 기간별 통계 - 팀별 (가운데 영역 팀원별 차트)
    /// </summary>
    /// <param name="year"></param>
    /// <param name="month"></param>
    /// <returns></returns>
    [HttpPost]
    public async Task<JsonResult> ChangeDivisionUserSalesChart(int year = 0, int month = 0, int ud_seq = 0)
    {
      try
      {
        if (year == 0)
          year = Utils.NowKorea().Year;

        if (month == 0)
          month = Utils.NowKorea().Month;

        string startDt = year.ToString() + "-" + month.ToString() + "-01";
        string endDt = new DateTime(year, month, 1).AddMonths(1).AddDays(-1).ToString("yyyy-MM-dd");

        AccountRepository ar = new AccountRepository();
        var divisionList = await ar.SelectDivisionList();

        if (ud_seq == 0)
          ud_seq = divisionList.FirstOrDefault().ud_seq;

        StatisticsRepository sr = new StatisticsRepository();

        var personalSalesList = await sr.SelectSalesDivisionUserList(startDt, endDt, ud_seq);

        return Json(new
        {
          ok = true,
          list = personalSalesList
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

    #region 기간별 통계 - 개인

    /// <summary>
    /// 기간별 통계 - 개인
    /// </summary>
    /// <param name="year"></param>
    /// <returns></returns>
    public async Task<ActionResult> PeriodSalesPersonal(int year = 0)
    {
      StatisticsRepository sr = new StatisticsRepository();

      var yearMonthData = await sr.SumBillingYearMonthListAsync();
      var yearList = yearMonthData.GroupBy(x => x.billing_year).ToList().Select(y => new YearMonthModel() { key = y.Key, value = y.Key + "년" }).ToList();

      if (year == 0)
        year = yearList.FirstOrDefault().key;

      string startDt = year + "-01-01";
      string endDt = year + "-12-31";

      var monthlySalesList = await sr.SelectSalesMonthlySumList(startDt, endDt, 0, AppIdentity.user_seq);

      var businessList = await sr.SelectBusinessPiePersonalAsync(startDt, endDt, AppIdentity.user_seq);
      var pjtSuccessRate = await sr.SelectPjtSuccessRateOneAsync(startDt, endDt, AppIdentity.user_seq);
      var feeRateList = await sr.SelectFeeRatePiePersonalAsync(startDt, endDt, AppIdentity.user_seq);

      PeriodSalesModel model = new PeriodSalesModel()
      {
        year = year,
        yearList = yearList,
        salesMonthlySumList = monthlySalesList,
        businessList = businessList,
        pjtSuccessRate = pjtSuccessRate,
        feeRateList = feeRateList
      };

      return View(model);
    }

    /// <summary>
    /// 기간별 통계 - 개인(하단 리스트)
    /// </summary>
    /// <param name="year"></param>
    /// <param name="ud_seq"></param>
    /// <param name="page"></param>
    /// <returns></returns>
    public async Task<PartialViewResult> PeriodSalesPersonalList(string year = "", int page = 1)
    {
      StatisticsRepository sr = new StatisticsRepository();

      int totalCount = 0;

      //한 페이지당 몇개의 행을 보여줄 것인지
      int pageSize = AppPaging.PageLength10;

      if (string.IsNullOrWhiteSpace(year))
        year = Utils.NowKorea().Year.ToString();

      string startDt = year + "-01-01";
      string endDt = year + "-12-31";

      var sumData = await sr.SumPeriodSalesAllAsync(startDt, endDt, 0, AppIdentity.user_seq);
      var list = sr.SelectPeriodSalesAllList(startDt, endDt, 0, AppIdentity.user_seq, (page - 1) * pageSize, pageSize, out totalCount);

      SalesDetailListModel model = new SalesDetailListModel()
      {
        year = year
          ,
        sumData = sumData
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

    #region 년도별 통계 - 팀

    /// <summary>
    /// 년도별 통계 팀
    /// </summary>
    /// <param name="year"></param>
    /// <param name="ud_seq"></param>
    /// <returns></returns>
    public async Task<ActionResult> YearlySalesTeam(int year = 0, int ud_seq = 0)
    {
      StatisticsRepository sr = new StatisticsRepository();

      var yearMonthData = await sr.SumBillingYearMonthListAsync();
      var yearList = yearMonthData.GroupBy(x => x.billing_year).ToList().Select(y => new YearMonthModel() { key = y.Key, value = y.Key + "년" }).ToList();

      if (year == 0)
        year = yearList.FirstOrDefault().key;

      AccountRepository ar = new AccountRepository();
      var divisionList = await ar.SelectDivisionList();

      if (ud_seq == 0)
        ud_seq = AppIdentity.ud_seq;

      string startDt = year + "-01-01";
      string endDt = year + "-12-31";

      var yearlySalesList = await sr.SelectSalesYearlySumList(ud_seq, 0);
      var divisionSalesList = await sr.SelectSalesDivisionUserList(startDt, endDt, ud_seq);

      YearlySalesModel model = new YearlySalesModel()
      {
        year = year,
        ud_seq = ud_seq,
        yearList = yearList,
        divisionList = divisionList,
        salesYearlySumList = yearlySalesList,
        salesDivisionUserList = divisionSalesList
      };

      return View(model);
    }

    /// <summary>
    /// 년도별 통계 - 팀별 (하단 영역 리스트)
    /// </summary>
    /// <param name="ud_seq"></param>
    /// <param name="page"></param>
    /// <returns></returns>
    public async Task<PartialViewResult> YearlySalesTeamList(int ud_seq = 0, int page = 1)
    {
      StatisticsRepository sr = new StatisticsRepository();

      int totalCount = 0;

      //한 페이지당 몇개의 행을 보여줄 것인지
      int pageSize = AppPaging.PageLength10;

      var sumData = await sr.SumYearlySalesAllAsync(ud_seq, 0);
      var list = sr.SelectYearlySalesAllList(ud_seq, 0, (page - 1) * pageSize, pageSize, out totalCount);

      SalesDetailListModel model = new SalesDetailListModel()
      {
        sumData = sumData
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

    #region 년도별 통계 - 개인

    public async Task<ActionResult> YearlySalesPersonal()
    {
      StatisticsRepository sr = new StatisticsRepository();

      var yearlySalesList = await sr.SelectSalesYearlySumList(AppIdentity.ud_seq, AppIdentity.user_seq);

      var businessList = await sr.SelectYearlyBusinessPiePersonalAsync(AppIdentity.user_seq);
      var pjtSuccessRate = await sr.SelectYearlyPjtSuccessRateOneAsync(AppIdentity.user_seq);
      var feeRateList = await sr.SelectYearlyFeeRatePiePersonalAsync(AppIdentity.user_seq);

      YearlySalesModel model = new YearlySalesModel()
      {
        salesYearlySumList = yearlySalesList,
        businessList = businessList,
        pjtSuccessRate = pjtSuccessRate,
        feeRateList = feeRateList
      };

      return View(model);
    }

    public async Task<PartialViewResult> YearlySalesPersonalList(int ud_seq = 0, int page = 1)
    {
      StatisticsRepository sr = new StatisticsRepository();

      int totalCount = 0;

      //한 페이지당 몇개의 행을 보여줄 것인지
      int pageSize = AppPaging.PageLength10;

      var sumData = await sr.SumYearlySalesAllAsync(ud_seq, AppIdentity.user_seq);
      var list = sr.SelectYearlySalesAllList(ud_seq, AppIdentity.user_seq, (page - 1) * pageSize, pageSize, out totalCount);

      SalesDetailListModel model = new SalesDetailListModel()
      {
        sumData = sumData
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

    /// <summary>
    /// 매출내역 메인 (맨 하단 리스트 제외)
    /// </summary>
    /// <param name="search"></param>
    /// <param name="page"></param>
    /// <returns></returns>
    public async Task<ActionResult> SalesHistory(SaleHistorySearchModel search, int page = 1)
    {
      AccountRepository ar = new AccountRepository();

      var deptList = await ar.SelectDivisionList();

      var userList = new List<uv_user>();

      search.ud_seq = AppIdentity.ud_seq;
      search.uv_seq = AppIdentity.user_seq;

      if (string.IsNullOrWhiteSpace(search.startDt))
        search.startDt = new DateTime(Utils.NowKorea().Year, Utils.NowKorea().Month, 1).ToString("yyyy-MM-dd");

      if (string.IsNullOrWhiteSpace(search.endDt))
        search.endDt = Utils.NowKorea().ToString("yyyy-MM-dd");

      //부서 seq를 가지고 있다면 userList 바인딩.
      if (search.ud_seq > 0)
      {
        userList = await ar.SelectUserListByDivision(search.ud_seq);
      }

      StatisticsRepository sr = new StatisticsRepository();

      var positionList = await sr.SelectPositionPieStatisticsAsync(search);
      var feeRateList = await sr.SelectFeeRatePieStatisticsAsync(search);
      var businessList = await sr.SelectBusinessPieStatisticsAsync(search);
      var jobList = await sr.SelectJobPieStatisticsAsync(search);

      SalesHistoryViewModel model = new SalesHistoryViewModel()
      {
        search = search
          ,
        deptList = deptList
          ,
        userList = userList
          ,
        positionList = positionList
          ,
        feeRateList = feeRateList
          ,
        businessList = businessList
          ,
        jobList = jobList
      };

      return View(model);
    }

    /// <summary>
    /// 매출내역 (맨 하단 리스트)
    /// </summary>
    /// <param name="search"></param>
    /// <param name="page"></param>
    /// <returns></returns>
    public async Task<PartialViewResult> SalesHistoryList(SaleHistorySearchModel search, int page = 1)
    {
      StatisticsRepository sr = new StatisticsRepository();

      int totalCount = 0;

      //한 페이지당 몇개의 행을 보여줄 것인지
      int pageSize = AppPaging.PageLength10;

      var sumData = await sr.SumSalesHistoryStatisticsAsync(search);

      var list = sr.SelectSalesHistoryStatisticsList(search, (page - 1) * pageSize, pageSize, out totalCount);

      SalesHistoryListModel model = new SalesHistoryListModel()
      {
        sumData = sumData
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
    /// 고객사별 프로젝트 통계
    /// </summary>
    /// <param name="search"></param>
    /// <param name="page"></param>
    /// <returns></returns>
    public async Task<ActionResult> ClientProjectHistory(ClientProjectStatisticsSearchModel search, int page = 1)
    {
      StatisticsRepository sr = new StatisticsRepository();

      int totalCount = 0;

      //한 페이지당 몇개의 행을 보여줄 것인지
      int pageSize = AppPaging.PageLength20;

      if (string.IsNullOrWhiteSpace(search.startDt))
        search.startDt = new DateTime(Utils.NowKorea().Year, 1, 1).ToString("yyyy-MM-dd");

      if (string.IsNullOrWhiteSpace(search.endDt))
        search.endDt = Utils.NowKorea().ToString("yyyy-MM-dd");

      var sumData = await sr.SumClientProjectStatisticsAsync(search);
      var list = sr.SelectClientProjectStatisticsList(search, (page - 1) * pageSize, pageSize, out totalCount);

      ClientProjectStatisticsListModel model = new ClientProjectStatisticsListModel()
      {
        search = search
          ,
        list = list
          ,
        sumData = sumData
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
    /// 산업별 프로젝트 현황
    /// </summary>
    /// <param name="search"></param>
    /// <returns></returns>
    public async Task<ActionResult> BusinessProjectHistory(BusiJobSearchModel search)
    {
      StatisticsRepository sr = new StatisticsRepository();

      if (string.IsNullOrWhiteSpace(search.startDt))
        search.startDt = new DateTime(Utils.NowKorea().Year, 1, 1).ToString("yyyy-MM-dd");

      if (string.IsNullOrWhiteSpace(search.endDt))
        search.endDt = Utils.NowKorea().ToString("yyyy-MM-dd");

      var list = await sr.SelectBusinessStatisticsList(search);

      BusiJobStatisticsListModel model = new BusiJobStatisticsListModel()
      {
        search = search
          ,
        list = list
          ,
        SumAll = list.Count
          ,
        SumSuccess = list.Sum(x => x.successCnt)
          ,
        SumFail = list.Sum(x => x.failCnt)
          ,
        SumMoney = list.Sum(x => x.billing_money)
      };

      return View(model);
    }

    /// <summary>
    /// 직무별 프로젝트 현황
    /// </summary>
    /// <param name="search"></param>
    /// <returns></returns>
    public async Task<ActionResult> JobProjectHistory(BusiJobSearchModel search)
    {
      StatisticsRepository sr = new StatisticsRepository();

      if (string.IsNullOrWhiteSpace(search.startDt))
        search.startDt = new DateTime(Utils.NowKorea().Year, 1, 1).ToString("yyyy-MM-dd");

      if (string.IsNullOrWhiteSpace(search.endDt))
        search.endDt = Utils.NowKorea().ToString("yyyy-MM-dd");

      var list = await sr.SelectBusinessStatisticsList(search);

      BusiJobStatisticsListModel model = new BusiJobStatisticsListModel()
      {
        search = search
          ,
        list = list
          ,
        SumAll = list.Count
          ,
        SumSuccess = list.Sum(x => x.successCnt)
          ,
        SumFail = list.Sum(x => x.failCnt)
          ,
        SumMoney = list.Sum(x => x.billing_money)
      };

      return View(model);
    }
    public async Task<JsonResult> ProjectActivityAjx(ProjectActionSearchModel search)
    {
      try
      {
        DateTime cur_month_last_date = Utils.NowKorea();
        DateTime start_first_date = Utils.NowKorea().AddDays(-7);

        if (string.IsNullOrWhiteSpace(search.startDt))
          search.startDt = start_first_date.ToString("yyyy-MM-dd");

        if (string.IsNullOrWhiteSpace(search.endDt))
          search.endDt = cur_month_last_date.ToString("yyyy-MM-dd");

        StatisticsRepository sr = new StatisticsRepository();
        var list = await sr.SelectProjectActivity(search);

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
    public async Task<ActionResult> ProjectActivity(ProjectActionSearchModel search)
    {
      AccountRepository ar = new AccountRepository();
      DateTime cur_month_last_date = Utils.NowKorea();
      //cur_month_last_date = new DateTime(cur_month_last_date.Year, cur_month_last_date.Month, 1).AddDays(-1);
      DateTime start_first_date = Utils.NowKorea().AddDays(-7);
      //start_first_date = new DateTime(start_first_date.Year, start_first_date.Month, 1);
      int div_no = 0;
      /*
            if (AppIdentity.ua_seq > 3 && AppIdentity.user_seq != 123)
            {
                div_no = AppIdentity.ud_seq;
            }
      */
      if (search.uv_seq == 0)
        search.uv_seq = AppIdentity.user_seq;

      if (string.IsNullOrWhiteSpace(search.startDt))
        search.startDt = start_first_date.ToString("yyyy-MM-dd");

      if (string.IsNullOrWhiteSpace(search.endDt))
        search.endDt = cur_month_last_date.ToString("yyyy-MM-dd");
      var except_user = new List<int> { 197, 244, 317, 326, 354, 328, 138, 393, 394, 395, 396 };
      var userList = await ar.SelectAllUserList(except_user);

      //StatisticsRepository sr = new StatisticsRepository();
      //var list = await sr.SelectProjectActivity(search);

      ProjectActionListModel model = new ProjectActionListModel()
      {
        search = search
          ,
        userList = userList
        //  ,
        //list = list
      };

      return View(model);
    }

    public async Task<JsonResult> MonthlyKPIAjx(MonthlyKPISearchModel search)
    {
      try
      {
        DateTime cur_month_last_date = Utils.NowKorea();
        DateTime start_first_date = Utils.NowKorea().AddDays(-7);

        if (string.IsNullOrWhiteSpace(search.startDt))
          search.startDt = start_first_date.ToString("yyyy-MM-dd");

        if (string.IsNullOrWhiteSpace(search.endDt))
          search.endDt = cur_month_last_date.ToString("yyyy-MM-dd");

        StatisticsRepository sr = new StatisticsRepository();
        var list = await sr.SelectMonthlyKPI(search);

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
    public async Task<ActionResult> MonthlyKPI(MonthlyKPISearchModel search)
    {
      var now = DateTime.Now;

      AccountRepository ar = new AccountRepository();
      DateTime start_first_date = new DateTime(now.Year, now.Month, 01);
      DateTime cur_month_last_date= new DateTime(now.Year, now.Month, new DateTime(now.Year, now.AddMonths(1).Month, 01).AddDays(-1).Day);      
      int div_no = 0;
      /*
            if (AppIdentity.ua_seq > 3 && AppIdentity.user_seq != 123)
            {
                div_no = AppIdentity.ud_seq;
            }
      */
      if (search.uv_seq == 0)
        search.uv_seq = AppIdentity.user_seq;

      if (string.IsNullOrWhiteSpace(search.startDt))
        search.startDt = start_first_date.ToString("yyyy-MM-dd");

      if (string.IsNullOrWhiteSpace(search.endDt))
        search.endDt = cur_month_last_date.ToString("yyyy-MM-dd");
      var except_user = new List<int> { 197, 244, 317, 326, 328, 138, 393, 394, 395, 396 };
      var userList = await ar.SelectAllUserList(except_user);

      //StatisticsRepository sr = new StatisticsRepository();
      //var list = await sr.SelectProjectActivity(search);

      MonthlyKPIModel model = new MonthlyKPIModel()
      {
        search = search
          ,
        userList = userList
        //  ,
        //list = list
      };

      return View(model);
    }

    public async Task<JsonResult> KeyProjectListAjx(KeyProjectSearchModel search)
    {
      try
      {
        DateTime cur_month_last_date = Utils.NowKorea();
        DateTime start_first_date = Utils.NowKorea().AddYears(-1);


        if (string.IsNullOrWhiteSpace(search.startDt))
          search.startDt = start_first_date.ToString("yyyy-MM-dd");

        if (string.IsNullOrWhiteSpace(search.endDt))
          search.endDt = cur_month_last_date.ToString("yyyy-MM-dd");

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

    public async Task<ActionResult> KeyProjectList(KeyProjectSearchModel search)
    {
      var now = DateTime.Now;

      AccountRepository ar = new AccountRepository();
      DateTime cur_month_last_date = Utils.NowKorea();
      DateTime start_first_date = Utils.NowKorea().AddYears(-1);
      int div_no = 0;
      /*
            if (AppIdentity.ua_seq > 3 && AppIdentity.user_seq != 123)
            {
                div_no = AppIdentity.ud_seq;
            }
      */
      if (search.uv_seq == 0)
        search.uv_seq = AppIdentity.user_seq;

      if (string.IsNullOrWhiteSpace(search.startDt))
        search.startDt = start_first_date.ToString("yyyy-MM-dd");

      if (string.IsNullOrWhiteSpace(search.endDt))
        search.endDt = cur_month_last_date.ToString("yyyy-MM-dd");
      var except_user = new List<int> { 197, 244, 317, 326, 354, 328, 138, 393, 394, 395, 396 };
      var userList = await ar.SelectAllUserList(except_user);

      //StatisticsRepository sr = new StatisticsRepository();
      //var list = await sr.SelectProjectActivity(search);

      KeyProjectListModel model = new KeyProjectListModel()
      {
        search = search
          ,
        userList = userList
        //  ,
        //list = list
      };

      return View(model);
    }

    public async Task<JsonResult> ProjectStatusListAjx(KeyProjectSearchModel search)
    {
      try
      {
        DateTime cur_month_last_date = Utils.NowKorea();
        DateTime start_first_date = Utils.NowKorea().AddYears(-1);


        if (string.IsNullOrWhiteSpace(search.startDt))
          search.startDt = start_first_date.ToString("yyyy-MM-dd");

        if (string.IsNullOrWhiteSpace(search.endDt))
          search.endDt = cur_month_last_date.ToString("yyyy-MM-dd");

        StatisticsRepository sr = new StatisticsRepository();
        var list = await sr.SelectProjectStatus(search);

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

    public async Task<ActionResult> ProjectStatusList(KeyProjectSearchModel search)
    {
      var now = DateTime.Now;

      AccountRepository ar = new AccountRepository();
      DateTime cur_month_last_date = Utils.NowKorea();
      DateTime start_first_date = Utils.NowKorea().AddYears(-1);
      int div_no = 0;
      /*
            if (AppIdentity.ua_seq > 3 && AppIdentity.user_seq != 123)
            {
                div_no = AppIdentity.ud_seq;
            }
      */
      if (search.uv_seq == 0)
        search.uv_seq = AppIdentity.user_seq;

      if (string.IsNullOrWhiteSpace(search.startDt))
        search.startDt = start_first_date.ToString("yyyy-MM-dd");

      if (string.IsNullOrWhiteSpace(search.endDt))
        search.endDt = cur_month_last_date.ToString("yyyy-MM-dd");
      var except_user = new List<int> { 197, 244, 317, 326, 354, 328, 138, 393, 394, 395, 396 };
      var userList = await ar.SelectAllUserList(except_user);

      //StatisticsRepository sr = new StatisticsRepository();
      //var list = await sr.SelectProjectActivity(search);

      KeyProjectListModel model = new KeyProjectListModel()
      {
        search = search
          ,
        userList = userList
        //  ,
        //list = list
      };

      return View(model);
    }
    

    public async Task<JsonResult> NegoProjectListAjx(NegoProjectSearchModel search)
    {
      try
      {
        DateTime cur_month_last_date = Utils.NowKorea();
        DateTime start_first_date = Utils.NowKorea().AddYears(-1);


        if (string.IsNullOrWhiteSpace(search.startDt))
          search.startDt = start_first_date.ToString("yyyy-MM-dd");

        if (string.IsNullOrWhiteSpace(search.endDt))
          search.endDt = cur_month_last_date.ToString("yyyy-MM-dd");

        StatisticsRepository sr = new StatisticsRepository();
        var list = await sr.SelectNegoProject(search);

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

    public async Task<ActionResult> NegoProjectList(NegoProjectSearchModel search)
    {
      var now = DateTime.Now;

      AccountRepository ar = new AccountRepository();
      DateTime cur_month_last_date = Utils.NowKorea();
      DateTime start_first_date = Utils.NowKorea().AddMonths(-3);
      int div_no = 0;
      /*
            if (AppIdentity.ua_seq > 3 && AppIdentity.user_seq != 123)
            {
                div_no = AppIdentity.ud_seq;
            }
      */
      if (search.uv_seq == 0)
        search.uv_seq = AppIdentity.user_seq;

      if (string.IsNullOrWhiteSpace(search.startDt))
        search.startDt = start_first_date.ToString("yyyy-MM-dd");

      if (string.IsNullOrWhiteSpace(search.endDt))
        search.endDt = cur_month_last_date.ToString("yyyy-MM-dd");
      //var except_user = new List<int> { 197, 244, 317, 326, 354, 328, 138, 393, 394, 395, 396 };
      //var userList = await ar.SelectAllUserList(except_user);

      //StatisticsRepository sr = new StatisticsRepository();
      //var list = await sr.SelectProjectActivity(search);

      NegoProjectListModel model = new NegoProjectListModel()
      {
        search = search
          ,
        //userList = userList
        //  ,
        //list = list
      };

      return View(model);
    }

    public async Task<JsonResult> WeeklyProjectSuccListAjx(WeeklyReportSearchModel search)
    {
      try
      {
        if (string.IsNullOrWhiteSpace(search.startDt))
          search.startDt = Utils.NowKorea().AddDays(-7).ToString("yyyy-MM-dd");

        if (string.IsNullOrWhiteSpace(search.endDt))
          search.endDt = Utils.NowKorea().ToString("yyyy-MM-dd");

        ProjectRepository pr = new ProjectRepository();
        var successList = await pr.SelectWeeklySuccessReport2List(search);

        return Json(new
        {
          ok = true,
          list = successList
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
    public async Task<JsonResult> WeeklyProjectCompListAjx(WeeklyReportSearchModel search)
    {
      try
      {
        if (string.IsNullOrWhiteSpace(search.startDt))
          search.startDt = Utils.NowKorea().AddDays(-7).ToString("yyyy-MM-dd");

        if (string.IsNullOrWhiteSpace(search.endDt))
          search.endDt = Utils.NowKorea().ToString("yyyy-MM-dd");

        ProjectRepository pr = new ProjectRepository();
        var completeList = await pr.SelectWeeklyCompleteReportList(search);

        return Json(new
        {
          ok = true,
          list = completeList
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
    public async Task<ActionResult> WeeklyProjectList(WeeklyReportSearchModel search)
    {
      if (string.IsNullOrWhiteSpace(search.startDt))
        search.startDt = Utils.NowKorea().AddDays(-7).ToString("yyyy-MM-dd");

      if (string.IsNullOrWhiteSpace(search.endDt))
        search.endDt = Utils.NowKorea().ToString("yyyy-MM-dd");

      

      ProjectRepository pr = new ProjectRepository();
      //var totalList = await pr.SelectWeeklyReportList(search);
      var successList = await pr.SelectWeeklySuccessReport2List(search);
      var completeList = await pr.SelectWeeklyCompleteReportList(search);

      //var successList = totalList.Where(x => x.pjt_status == ProjectStatusCode.success).GroupBy(x => new { x.p_seq, x.hire_candidate_seq }).Select(x => x.First()).ToList();
      //var completeList = totalList.Where(x => x.pjt_status == ProjectStatusCode.complete).GroupBy(x => new { x.p_seq, x.hire_candidate_seq }).Select(x => x.First()).ToList();
      //var list = totalList.Where(x => x.pjt_status != ProjectStatusCode.success && x.pjt_status != ProjectStatusCode.complete).OrderBy(x => (x.retire_date)).ThenBy(x => (x.ud_seq == search.ud_seq ? 1 : 2)).ThenBy(x => x.ua_seq).ThenBy(x => x.name).ThenByDescending(x => x.create_dt).ToList();

      WeeklyReportListModel model = new WeeklyReportListModel()
      {
        search = search
      };

      return View(model);
    }
    public async Task<JsonResult> MonthlyCandidateRegUserAjx(CandidateRegSearchModel search)
    {
      try
      {
        var now = DateTime.Now;
        DateTime cur_month_last_date = new DateTime(now.Year, now.Month, 25);
        DateTime start_first_date = new DateTime(now.AddMonths(-1).Year, now.AddMonths(-1).Month, 26);

        if (string.IsNullOrWhiteSpace(search.startDt))
          search.startDt = start_first_date.ToString("yyyy-MM-dd");

        if (string.IsNullOrWhiteSpace(search.endDt))
          search.endDt = cur_month_last_date.ToString("yyyy-MM-dd");

        StatisticsRepository sr = new StatisticsRepository();
        var list = await sr.SelectCandidateRegUser(search);

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

    public async Task<ActionResult> MonthlyCandidateRegStatus(CandidateRegSearchModel search)
    {
      //AccountRepository ar = new AccountRepository();
      var now = DateTime.Now;
      DateTime cur_month_last_date = new DateTime(now.Year, now.Month, 25);
      //cur_month_last_date = new DateTime(cur_month_last_date.Year, cur_month_last_date.Month, 1).AddDays(-1);
      DateTime start_first_date = new DateTime(now.Year, now.AddMonths(-1).Month, 26);
      //start_first_date = new DateTime(start_first_date.Year, start_first_date.Month, 1);
    

      if (string.IsNullOrWhiteSpace(search.startDt))
        search.startDt = start_first_date.ToString("yyyy-MM-dd");

      if (string.IsNullOrWhiteSpace(search.endDt))
        search.endDt = cur_month_last_date.ToString("yyyy-MM-dd");
  
      CandidateRegStatusModel model = new CandidateRegStatusModel()
      {
        search = search
        //  ,
        //list = list
      };

      return View(model);
    }

    public async Task<JsonResult> MonthlyCandidateRegBusiAjx(CandidateRegSearchModel search)
    {
      try
      {
        var now = DateTime.Now;
        DateTime cur_month_last_date = new DateTime(now.Year, now.Month, 25);
        DateTime start_first_date = new DateTime(now.Year, now.AddMonths(-1).Month, 26);

        if (string.IsNullOrWhiteSpace(search.startDt))
          search.startDt = start_first_date.ToString("yyyy-MM-dd");

        if (string.IsNullOrWhiteSpace(search.endDt))
          search.endDt = cur_month_last_date.ToString("yyyy-MM-dd");

        StatisticsRepository sr = new StatisticsRepository();
        var list = await sr.SelectCandidateRegBusi(search);

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


    public async Task<JsonResult> MonthlyCandidateRegJobAjx(CandidateRegSearchModel search)
    {
      try
      {
        var now = DateTime.Now;
        DateTime cur_month_last_date = new DateTime(now.Year, now.Month, 25);
        DateTime start_first_date = new DateTime(now.Year, now.AddMonths(-1).Month, 26);

        if (string.IsNullOrWhiteSpace(search.startDt))
          search.startDt = start_first_date.ToString("yyyy-MM-dd");

        if (string.IsNullOrWhiteSpace(search.endDt))
          search.endDt = cur_month_last_date.ToString("yyyy-MM-dd");

        StatisticsRepository sr = new StatisticsRepository();
        var list = await sr.SelectCandidateRegJob(search);

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

    #region Univision Activiry Report

    public async Task<ActionResult> UnivisionActivity(UnivisionActivitySearchModel search)
    {
      AccountRepository ar = new AccountRepository();
      DateTime cur_month_last_date = Utils.NowKorea().AddMonths(1);
      cur_month_last_date = new DateTime(cur_month_last_date.Year, cur_month_last_date.Month, 1).AddDays(-1);
      DateTime start_first_date = cur_month_last_date.AddMonths(-2);
      start_first_date = new DateTime(start_first_date.Year, start_first_date.Month, 1);
      int div_no = 0;
      /*
            if (AppIdentity.ua_seq > 3 && AppIdentity.user_seq != 123)
            {
                div_no = AppIdentity.ud_seq;
            }
      */
      if (search.uv_seq == 0)
        search.uv_seq = AppIdentity.user_seq;

      if (string.IsNullOrWhiteSpace(search.startDt))
        search.startDt = start_first_date.ToString("yyyy-MM-dd");

      if (string.IsNullOrWhiteSpace(search.endDt))
        search.endDt = cur_month_last_date.ToString("yyyy-MM-dd");
      var except_user = new List<int> { 197, 244, 317, 326, 328, 138, 393, 394, 395, 396, 459 };
      var userList = await ar.SelectAllUserList(except_user);

      StatisticsRepository sr = new StatisticsRepository();
      var list = await sr.SelectUnivisionActivity(search);

      UnivisionActivityListModel model = new UnivisionActivityListModel()
      {
        search = search
          ,
        userList = userList
          ,
        list = list
      };

      return View(model);
    }


    public async Task<PartialViewResult> UnivisionCandidateList(UnivisionCandidateSearchModel search)
    {


      StatisticsRepository sr = new StatisticsRepository();



      var list = await sr.SelectCandidateRegistrationList(search);

      UnivisionCandidateListModel model = new UnivisionCandidateListModel()
      {
        search = search,
        list = list
      };

      return PartialView(model);
    }

    public async Task<PartialViewResult> UnivisionCandidateUpdateList(UnivisionCandidateSearchModel search)
    {


      StatisticsRepository sr = new StatisticsRepository();

      var list = await sr.SelectCandidateUpdateList(search);

      UnivisionCandidateListModel model = new UnivisionCandidateListModel()
      {
        search = search,
        list = list
      };

      return PartialView("UnivisionCandidateList", model);
    }

    public async Task<PartialViewResult> UnivisionPjtCandidateList(UnivisionPjtCandidateSearchModel search)
    {


      StatisticsRepository sr = new StatisticsRepository();

      var list = await sr.SelectPjtCandidateList(search);

      UnivisionPjtCandidateListModel model = new UnivisionPjtCandidateListModel()
      {
        search = search,
        list = list
      };

      return PartialView(model);
    }

    public async Task<PartialViewResult> UnivisionPjtHireCandidateList(UnivisionPjtCandidateSearchModel search)
    {


      StatisticsRepository sr = new StatisticsRepository();

      var list = await sr.SelectPjtHireCandidateList(search);

      UnivisionPjtCandidateListModel model = new UnivisionPjtCandidateListModel()
      {
        search = search,
        list = list
      };

      return PartialView(model);
    }
    public async Task<PartialViewResult> UnivisionMemoList(UnivisionMemoSearchModel search)
    {


      StatisticsRepository sr = new StatisticsRepository();



      var list = await sr.SelectCandidateMemoList(search);

      UnivisionMemoListModel model = new UnivisionMemoListModel()
      {
        search = search,
        list = list
      };

      return PartialView(model);
    }

    #endregion

  }
}