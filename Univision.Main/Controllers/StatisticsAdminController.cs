using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Univision.Core.Models;
using Univision.Core.Models.DTO;
using Univision.Core.Models.DTO.Response;
using Univision.Core.Models.DTO.Response.Statistics;
using Univision.Core.Repositories;
using Univision.Security;

namespace Univision.Main.Controllers
{
    public class StatisticsAdminController : BaseController
    {
        #region 기간별 통계 - 전체

        /// <summary>
        /// 기간별 통계 - 전체
        /// </summary>
        /// <param name="year"></param>
        /// <param name="month"></param>
        /// <returns></returns>
        public async Task<ActionResult> PeriodSalesAll(int year = 0, int month = 0)
        {

            StatisticsRepository sr = new StatisticsRepository();

            var yearMonthData = await sr.SumBillingYearMonthListAsync();
            var yearList = yearMonthData.GroupBy(x => x.billing_year).ToList().Select(y => new YearMonthModel(){ key = y.Key, value = y.Key + "년" }).ToList();
            var monthList = yearMonthData.Select(x => new YearMonthModel() { key = x.billing_month, value = x.billing_month + "월" }).ToList();

            if (year == 0)
                year = yearList.FirstOrDefault().key;

            if (month == 0)
                month = Utils.NowKorea().Month;

            string startDt = year + "-01-01";
            string endDt = year + "-12-31";

            string startDt2 = year.ToString() + "-" + month.ToString() + "-01";
            string endDt2 = new DateTime(year, month, 1).AddMonths(1).AddDays(-1).ToString("yyyy-MM-dd");

            var monthlySalesList = await sr.SelectSalesMonthlySumList(startDt, endDt, 0, 0);
            var divisionSalesList = await sr.SelectSalesDivisionList(startDt2, endDt2, 0);

            PeriodSalesModel model = new PeriodSalesModel()
            {
                year = year,
                month = month,
                yearList = yearList,
                monthList = monthList,
                salesMonthlySumList = monthlySalesList,
                salesDivisionList = divisionSalesList
            };

            return View(model);
        }

        /// <summary>
        /// 기간별 통계 - 전체 (하단 영역 리스트)
        /// </summary>
        /// <param name="year"></param>
        /// <param name="page"></param>
        /// <returns></returns>
        public async Task<PartialViewResult> PeriodSalesAllList(string year = "", int page = 1)
        {
            StatisticsRepository sr = new StatisticsRepository();

            int totalCount = 0;

            //한 페이지당 몇개의 행을 보여줄 것인지
            int pageSize = AppPaging.PageLength10;

            if (string.IsNullOrWhiteSpace(year))
                year = Utils.NowKorea().Year.ToString();

            string startDt = year + "-01-01";
            string endDt = year + "-12-31";

            var sumData = await sr.SumPeriodSalesAllAsync(startDt, endDt, 0, 0);
            var list = sr.SelectPeriodSalesAllList(startDt, endDt, 0, 0, (page - 1) * pageSize, pageSize, out totalCount);

            SalesDetailListModel model = new SalesDetailListModel(){
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

        /// <summary>
        /// 기간별 통계 - 전체 (가운데 영역 부서별 차트)
        /// </summary>
        /// <param name="year"></param>
        /// <param name="month"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<JsonResult> ChangeDivisionSalesChart(int year = 0, int month= 0)
        {
            try
            {
                if (year == 0)
                    year = Utils.NowKorea().Year;

                if (month == 0)
                    month = Utils.NowKorea().Month;
                 
                string startDt = year.ToString() + "-" + month.ToString() + "-01";
                string endDt = new DateTime(year, month, 1).AddMonths(1).AddDays(-1).ToString("yyyy-MM-dd");

                StatisticsRepository sr = new StatisticsRepository();

                var divisionSalesList = await sr.SelectSalesDivisionList(startDt, endDt, 0);
                
                return Json(new
                {
                    ok = true,
                    list = divisionSalesList
                });
            }
            catch(Exception e)
            {
                return Json(new
                {
                    ok = false,
                    message = e.Message
                });
            }
        }

        #endregion

        #region 년도별 통계 - 전체
        
        /// <summary>
        /// 년도별 통계 - 전체
        /// </summary>
        /// <param name="year"></param>
        /// <param name="month"></param>
        /// <returns></returns>
        public async Task<ActionResult> YearlySalesAll(int year = 0)
        {

            StatisticsRepository sr = new StatisticsRepository();

            var yearMonthData = await sr.SumBillingYearMonthListAsync();
            var yearList = yearMonthData.GroupBy(x => x.billing_year).ToList().Select(y => new YearMonthModel() { key = y.Key, value = y.Key + "년" }).ToList();
           
            if (year == 0)
                year = yearList.FirstOrDefault().key;
            
            string startDt = year + "-01-01";
            string endDt = year + "-12-31";

            var yearlySalesList = await sr.SelectSalesYearlySumList(0, 0);
            var divisionSalesList = await sr.SelectSalesDivisionList(startDt, endDt, 0);

            YearlySalesModel model = new YearlySalesModel()
            {
                year = year,
                yearList = yearList,
                salesYearlySumList = yearlySalesList,
                salesDivisionList = divisionSalesList
            };

            return View(model);
        }

        /// <summary>
        /// 년도별 통계 - 전체 (하단 영역 리스트)
        /// </summary>
        /// <param name="page"></param>
        /// <returns></returns>
        public async Task<PartialViewResult> YearlySalesAllList(int page = 1)
        {
            StatisticsRepository sr = new StatisticsRepository();

            int totalCount = 0;

            //한 페이지당 몇개의 행을 보여줄 것인지
            int pageSize = AppPaging.PageLength10;

            var sumData = await sr.SumYearlySalesAllAsync(0, 0);
            var list = sr.SelectYearlySalesAllList(0, 0, (page - 1) * pageSize, pageSize, out totalCount);

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

        [HttpPost]
        public async Task<JsonResult> ChangeYearlyDivisionSalesChart(int year = 0, int ud_seq = 0)
        {
            try
            {
                if (year == 0)
                    year = Utils.NowKorea().Year;

                string startDt = year + "-01-01";
                string endDt = year + "-12-31";

                StatisticsRepository sr = new StatisticsRepository();

                var divisionSalesList = await sr.SelectSalesDivisionList(startDt, endDt, ud_seq);

                return Json(new
                {
                    ok = true,
                    list = divisionSalesList
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

    }
}