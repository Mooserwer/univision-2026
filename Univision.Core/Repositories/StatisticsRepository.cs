using Dapper;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Univision.Core.Models.DTO;
using Univision.Core.Models.DTO.Response.Statistics;
using Univision.Security;

namespace Univision.Core.Repositories
{
  public class StatisticsRepository : BaseRepository
  {

    #region 기간별 매출통계 - 전체

    /// <summary>
    /// 매출내역이 있는 년도 조회.
    /// </summary>
    /// <returns></returns>
    public async Task<List<SalesYearMonth>> SumBillingYearMonthListAsync()
    {
      try
      {
        List<SalesYearMonth> list = new List<SalesYearMonth>();

        using (IDbConnection con = new SqlConnection(base.BaseConnectionString))
        {
          con.Open();

          string selectQuery = @" SELECT DISTINCT YEAR(billing_dt) as billing_year
                                            FROM pjt_invoice_info
                                            WHERE is_deleted <> 1
                                            ORDER BY billing_year DESC ";

          var ret = await con.QueryAsync<SalesYearMonth>(selectQuery);

          con.Close();

          list = ret.ToList();

          return list;
        }
      }
      catch (Exception e)
      {
        throw e;
      }
    }

    /// <summary>
    /// 기간별 매출통계 - 전체 (월별 매출내역 합산, 월별 매출내역 그래프)
    /// </summary>
    /// <param name="startDt"></param>
    /// <param name="endDt"></param>
    /// <returns></returns>
    public async Task<List<SalesMonthlySum>> SelectSalesMonthlySumList(string startDt, string endDt, int ud_seq, int uv_seq)
    {
      try
      {
        List<SalesMonthlySum> list = new List<SalesMonthlySum>();

        using (IDbConnection con = new SqlConnection(base.BaseConnectionString))
        {
          con.Open();

          string selectQuery = @" 
select *
      ,ISNULL(SUM(sales_money) over(order by billing_dt), 0) AS sales_money_acc
      ,ISNULL(SUM(total_cnt) over(order by billing_dt), 0) AS total_cnt_acc
      ,ISNULL(SUM(deposit_amt) over(order by billing_dt), 0) AS deposit_amt_acc
      ,ISNULL(SUM(deposit_cnt) over(order by billing_dt), 0) AS deposit_cnt_acc
from (
SELECT A.billing_dt
      ,ISNULL(SUM(A.totalCnt), 0) AS total_cnt
      ,ISNULL(SUM(A.sales_money), 0) AS sales_money
      ,ISNULL(SUM(A.deposit_cnt), 0) AS deposit_cnt
      ,ISNULL(SUM(A.deposit_amt), 0) AS deposit_amt
  FROM (SELECT CONVERT(VARCHAR(7), PV.billing_dt, 121) AS billing_dt
              ,COUNT(0) AS totalCnt
              ,ISNULL(SUM(CAST(PS.sales_money AS BIGINT)), 0) AS sales_money
              ,SUM(CASE WHEN PV.etax_date is not null OR PV.deposit_dt is not null then 1 else 0 END) as deposit_cnt
              ,SUM(CASE WHEN PV.deposit_dt is not null then PS.sales_money WHEN PV.etax_date is not null THEN PS.sales_money else 0 END) as deposit_amt
        FROM pjt_invoice_sales AS PS INNER JOIN pjt_invoice_info AS PV
                                             ON PS.pii_seq = PV.pii_seq
        WHERE PV.is_deleted <> 1
          AND PV.billing_dt BETWEEN CONVERT(DATETIME, @startDt + ' 00:00:00') AND CONVERT(DATETIME, @endDt + ' 23:59:59') 
          AND PS.ud_seq = CASE WHEN @ud_seq <> 0 THEN @ud_seq ELSE PS.ud_seq END
          AND PS.uv_seq = CASE WHEN @uv_seq <> 0 THEN @uv_seq ELSE PS.uv_seq END
        GROUP BY PV.billing_dt) AS A
GROUP BY A.billing_dt
) X
ORDER BY billing_dt DESC ";
          //SUM(CASE WHEN PV.deposit_dt is not null then PS.sales_money WHEN PV.etax_date is not null THEN PS.sales_money else 0 END) as deposit_amt
          //나중에 수정해야함
          DynamicParameters param = new DynamicParameters();
          param.Add("@startDt", startDt, DbType.String);
          param.Add("@endDt", endDt, DbType.String);
          param.Add("@ud_seq", ud_seq, DbType.Int32);
          param.Add("@uv_seq", uv_seq, DbType.Int32);

          var ret = await con.QueryAsync<SalesMonthlySum>(selectQuery, param);

          con.Close();

          list = ret.ToList();

          return list;
        }
      }
      catch (Exception e)
      {
        throw e;
      }
    }

    /// <summary>
    /// 기간별 매출통계 - 전체 (팀별 매출내역 그래프)
    /// </summary>
    /// <param name="startDt"></param>
    /// <param name="endDt"></param>
    /// <returns></returns>
    public async Task<List<SalesDivision>> SelectSalesDivisionList(string startDt, string endDt, int ud_seq)
    {
      try
      {
        List<SalesDivision> list = new List<SalesDivision>();

        using (IDbConnection con = new SqlConnection(base.BaseConnectionString))
        {
          con.Open();

          string selectQuery = @" SELECT TOP 5
       UD.ud_name
      ,ISNULL(SUM(CAST(PS.sales_money AS BIGINT)), 0) AS sales_money
 FROM pjt_invoice_sales AS PS INNER JOIN pjt_invoice_info AS PV
                                      ON PS.pii_seq = PV.pii_seq
                              INNER JOIN uv_user AS U
                                      ON PS.uv_seq = U.uv_seq
                              INNER JOIN uv_division AS UD
                                      ON PS.ud_seq = UD.ud_seq
 WHERE PV.billing_dt BETWEEN CONVERT(DATETIME, @startDt + ' 00:00:00') AND CONVERT(DATETIME, @endDt + ' 23:59:59')
   AND PV.is_deleted <> 1
   AND PS.ud_seq = CASE WHEN @ud_seq <> 0 THEN @ud_seq ELSE PS.ud_seq END
 GROUP BY UD.ud_name
 ORDER BY sales_money DESC ";

          DynamicParameters param = new DynamicParameters();
          param.Add("@startDt", startDt, DbType.String);
          param.Add("@endDt", endDt, DbType.String);
          param.Add("@ud_seq", ud_seq, DbType.Int32);

          var ret = await con.QueryAsync<SalesDivision>(selectQuery, param);

          con.Close();

          list = ret.ToList();

          return list;
        }
      }
      catch (Exception e)
      {
        throw e;
      }
    }

    /// <summary>
    /// 기간별 매출통계 리스트 - 전체 합계 (하단 리스트)
    /// </summary>
    /// <param name="startDt"></param>
    /// <param name="endDt"></param>
    /// <returns></returns>
    public async Task<SalesDetailSum> SumPeriodSalesAllAsync(string startDt, string endDt, int ud_seq, int uv_seq)
    {
      try
      {
        using (SqlConnection con = new SqlConnection(base.BaseConnectionString))
        {
          con.Open();

          string selectQuery = @" SELECT CAST(SUM(CAST(PV.annual_income AS BIGINT)) AS VARCHAR(20)) annual_income
      ,CAST(SUM(CAST(PV.billing_money AS BIGINT)) AS VARCHAR(20))AS billing_money
      ,CAST(SUM(CAST(PS.sales_money AS BIGINT)) AS VARCHAR(20)) AS sales_money
FROM pjt_invoice_sales AS PS INNER JOIN pjt_invoice_info AS PV
                                     ON PS.pii_seq = PV.pii_seq
WHERE PV.billing_dt BETWEEN CONVERT(DATETIME, @startDt + ' 00:00:00') AND CONVERT(DATETIME, @endDt + ' 23:59:59') 
  AND PV.is_deleted <> 1
  AND PS.ud_seq = CASE WHEN @ud_seq <> 0 THEN @ud_seq ELSE PS.ud_seq END 
  AND PS.uv_seq = CASE WHEN @uv_seq <> 0 THEN @uv_seq ELSE PS.uv_seq END ";

          DynamicParameters param = new DynamicParameters();
          param.Add("@startDt", startDt, DbType.String);
          param.Add("@endDt", endDt, DbType.String);
          param.Add("@ud_seq", ud_seq, DbType.Int32);
          param.Add("@uv_seq", uv_seq, DbType.Int32);

          //var ret = await con.QueryAsync<SalesDetailSum>(selectQuery, param);

          IDataReader reader = await con.ExecuteReaderAsync(selectQuery, param);
          SalesDetailSum data = new SalesDetailSum();
          while (reader.Read())
          {
            long sum_annual_income = 0;
            long sum_billing_money = 0;
            long sum_sales_money = 0;

            if (long.TryParse(reader["annual_income"].ToString(), out sum_annual_income))
              data.sum_annual_income = sum_annual_income;
            else
              data.sum_annual_income = 0;

            if (long.TryParse(reader["billing_money"].ToString(), out sum_billing_money))
              data.sum_billing_money = sum_billing_money;
            else
              data.sum_billing_money = 0;

            if (long.TryParse(reader["sales_money"].ToString(), out sum_sales_money))
              data.sum_sales_money = sum_sales_money;
            else
              data.sum_sales_money = 0;
          }

          con.Close();

          return data;
        }
      }
      catch (Exception e)
      {
        throw e;
      }
    }

    /// <summary>
    /// 기간별 매출통계 리스트 - 전체,팀,개인 (하단 리스트)
    /// </summary>
    /// <param name="startDt"></param>
    /// <param name="endDt"></param>
    /// <param name="skip"></param>
    /// <param name="count"></param>
    /// <param name="totalCount"></param>
    /// <returns></returns>
    public List<SalesDetailModel> SelectPeriodSalesAllList(string startDt, string endDt, int ud_seq, int uv_seq, int skip, int count, out int totalCount)
    {
      try
      {
        List<SalesDetailModel> list = new List<SalesDetailModel>();

        using (IDbConnection con = new SqlConnection(base.BaseConnectionString))
        {
          con.Open();

          string selectQuery = @" SELECT C.c_seq AS client_seq
      ,C.kor_name AS client_name
      ,STUFF((SELECT ',' + B.name
                FROM pjt_director AS A INNER JOIN uv_user AS B
      			                              ON A.uv_seq = B.uv_seq
               WHERE A.p_seq = PS.p_seq
                 FOR XML PATH('')), 1, 1, '') AS am_name
      ,STUFF((SELECT ',' + B.name
                FROM pjt_manager AS A INNER JOIN uv_user AS B
                                              ON A.uv_seq = B.uv_seq
               WHERE A.p_seq = PS.p_seq
                 FOR XML PATH('')), 1, 1, '') AS searcher_name
      ,CC.name AS contact_name
      ,CD.c_seq AS candidate_seq
      ,CD.kor_name AS candidate_name
      ,PV.join_dt
      ,PV.billing_dt
      ,PV.deposit_dt
      ,ISNULL(PV.annual_income, 0) AS annual_income
      ,ISNULL(PV.fee_rate, 0) AS fee_rate
      ,ISNULL(PV.billing_money, 0) AS billing_money
      ,ISNULL(SUM(PS.sales_rate), 0) AS sales_rate
      ,ISNULL(SUM(PS.sales_money), 0) AS sales_money
FROM pjt_invoice_sales AS PS INNER JOIN pjt_invoice_info AS PV
                                     ON PS.pii_seq = PV.pii_seq
                        LEFT OUTER JOIN candidate AS CD
                                     ON PV.c_seq = CD.c_seq 
                        LEFT OUTER JOIN project AS P
                                     ON PS.p_seq = P.p_seq
                        LEFT OUTER JOIN client AS C
                                     ON P.c_seq = C.c_seq
                        LEFT OUTER JOIN client_contact AS CC
                                     ON P.cc_seq = CC.cc_seq
WHERE PV.billing_dt BETWEEN CONVERT(DATETIME, @startDt + ' 00:00:00') AND CONVERT(DATETIME, @endDt + ' 23:59:59')
  AND PV.is_deleted <> 1
  AND PS.ud_seq = CASE WHEN @ud_seq <> 0 THEN @ud_seq ELSE PS.ud_seq END
  AND PS.uv_seq = CASE WHEN @uv_seq <> 0 THEN @uv_seq ELSE PS.uv_seq END
GROUP BY C.c_seq, PS.p_seq, C.kor_name, CC.name, CD.c_seq, CD.kor_name, PV.join_dt, PV.billing_dt, PV.deposit_dt, PV.annual_income, PV.fee_rate, PV.billing_money
ORDER BY C.kor_name ASC ";
          selectQuery += " OFFSET @currentPage ROWS FETCH NEXT @pageSize ROWS ONLY ";

          string countQuery = @" SELECT COUNT(0)
  FROM (SELECT C.c_seq AS client_seq
        FROM pjt_invoice_sales AS PS INNER JOIN pjt_invoice_info AS PV
                                             ON PS.pii_seq = PV.pii_seq
                                LEFT OUTER JOIN candidate AS CD
                                             ON PV.c_seq = CD.c_seq 
                                LEFT OUTER JOIN project AS P
                                             ON PS.p_seq = P.p_seq
                                LEFT OUTER JOIN client AS C
                                             ON P.c_seq = C.c_seq
                                LEFT OUTER JOIN client_contact AS CC
                                             ON P.cc_seq = CC.cc_seq
        WHERE PV.billing_dt BETWEEN CONVERT(DATETIME, @startDt + ' 00:00:00') AND CONVERT(DATETIME, @endDt + ' 23:59:59')
          AND PV.is_deleted <> 1
          AND PS.ud_seq = CASE WHEN @ud_seq <> 0 THEN @ud_seq ELSE PS.ud_seq END
          AND PS.uv_seq = CASE WHEN @uv_seq <> 0 THEN @uv_seq ELSE PS.uv_seq END
        GROUP BY C.c_seq, PS.p_seq, C.kor_name, CC.name, CD.c_seq, CD.kor_name, PV.join_dt, PV.billing_dt, PV.deposit_dt, PV.annual_income, PV.fee_rate, PV.billing_money) AS A ";

          DynamicParameters param = new DynamicParameters();
          param.Add("@startDt", startDt, DbType.String);
          param.Add("@endDt", endDt, DbType.String);
          param.Add("@ud_seq", ud_seq, DbType.Int32);
          param.Add("@uv_seq", uv_seq, DbType.Int32);
          param.Add("@currentPage", skip, DbType.Int32);
          param.Add("@pageSize", count, DbType.Int32);

          DynamicParameters param2 = new DynamicParameters();
          param2.Add("@startDt", startDt, DbType.String);
          param2.Add("@endDt", endDt, DbType.String);
          param2.Add("@ud_seq", ud_seq, DbType.Int32);
          param2.Add("@uv_seq", uv_seq, DbType.Int32);

          var ret = con.Query<SalesDetailModel>(selectQuery, param);
          totalCount = con.QueryFirstOrDefault<int>(countQuery, param2);

          list = ret.ToList();

          con.Close();

          return list;
        }
      }
      catch (Exception e)
      {
        throw e;
      }
    }

    #endregion

    #region 기간별 매출 통계 - 팀별

    public async Task<List<SalesPersonal>> SelectSalesDivisionUserList(string startDt, string endDt, int ud_seq)
    {
      try
      {
        List<SalesPersonal> list = new List<SalesPersonal>();

        using (IDbConnection con = new SqlConnection(base.BaseConnectionString))
        {
          con.Open();

          string selectQuery = @" SELECT U.name
      ,ISNULL(SUM(CAST(PS.sales_money AS BIGINT)), 0) AS sales_money
 FROM pjt_invoice_sales AS PS INNER JOIN pjt_invoice_info AS PV
                                      ON PS.pii_seq = PV.pii_seq
                              INNER JOIN uv_user AS U
                                      ON PS.uv_seq = U.uv_seq
                              INNER JOIN uv_division AS UD
                                      ON PS.ud_seq = UD.ud_seq
 WHERE PV.billing_dt BETWEEN CONVERT(DATETIME, @startDt + ' 00:00:00') AND CONVERT(DATETIME, @endDt + ' 23:59:59')
   AND PV.is_deleted <> 1
   AND PS.ud_seq = CASE WHEN @ud_seq <> 0 THEN @ud_seq ELSE PS.ud_seq END
 GROUP BY U.name
 ORDER BY sales_money DESC ";

          DynamicParameters param = new DynamicParameters();
          param.Add("@startDt", startDt, DbType.String);
          param.Add("@endDt", endDt, DbType.String);
          param.Add("@ud_seq", ud_seq, DbType.Int32);

          var ret = await con.QueryAsync<SalesPersonal>(selectQuery, param);

          con.Close();

          list = ret.ToList();

          return list;
        }
      }
      catch (Exception e)
      {
        throw e;
      }
    }

    #endregion

    #region 기간별 매출통계 - 개인

    /// <summary>
    /// 산업파이
    /// </summary>
    /// <param name="startDt"></param>
    /// <param name="endDt"></param>
    /// <param name="uv_seq"></param>
    /// <returns></returns>
    public async Task<List<PieChartListModel>> SelectBusinessPiePersonalAsync(string startDt, string endDt, int uv_seq)
    {
      try
      {
        List<PieChartListModel> list = new List<PieChartListModel>();

        using (IDbConnection con = new SqlConnection(base.BaseConnectionString))
        {
          con.Open();

          string selectQuery = @" SELECT TOP 3
       A.code_name1 AS title
      ,A.allCnt
      ,A.billing_money
  FROM (SELECT  P.business_code1
               ,CB1.code_name1
               ,COUNT(0) AS allCnt
               ,ISNULL(SUM(PI.billing_money), 0) AS billing_money
           FROM project AS P INNER JOIN pjt_invoice_info AS PI
                                     ON P.p_seq = PI.p_seq
                             INNER JOIN code_business_mst AS CB1
                                     ON P.business_code1 = CB1.code1
          WHERE 1 = 1 
            AND PI.is_deleted <> 1
            AND P.create_dt BETWEEN CONVERT(DATETIME, @startDt + ' 00:00:00') AND CONVERT(DATETIME, @endDt + ' 23:59:59')
            AND (P.p_seq IN(select p_seq FROM pjt_director WHERE uv_seq = @uv_seq) OR P.p_seq IN(select p_seq FROM pjt_manager WHERE uv_seq = @uv_seq))
GROUP BY P.business_code1, CB1.code_name1, P.pjt_status ) AS A 
ORDER BY billing_money DESC ";

          DynamicParameters param = new DynamicParameters();
          param.Add("@startDt", startDt, DbType.String);
          param.Add("@endDt", endDt, DbType.String);
          param.Add("@uv_seq", uv_seq, DbType.Int32);

          var ret = await con.QueryAsync<PieChartListModel>(selectQuery, param);

          con.Close();

          list = ret.ToList();

          return list;
        }
      }
      catch (Exception e)
      {
        throw e;
      }
    }

    /// <summary>
    /// 개인 프로젝트 성공율
    /// </summary>
    /// <param name="startDt"></param>
    /// <param name="endDt"></param>
    /// <param name="uv_seq"></param>
    /// <returns></returns>
    public async Task<int> SelectPjtSuccessRateOneAsync(string startDt, string endDt, int uv_seq)
    {
      try
      {
        using (IDbConnection con = new SqlConnection(base.BaseConnectionString))
        {
          con.Open();

          string selectQuery = @" SELECT ISNULL(ROUND(SUM(CAST(cnt AS float)) / SUM(CAST(totalCnt AS float)) * 100, 0), 0)
  FROM ( 
SELECT COUNT(0) AS totalCnt
      ,CASE WHEN pjt_status = 5 THEN COUNT(0) ELSE 0 END AS cnt
  FROM project AS P
 WHERE P.create_dt BETWEEN CONVERT(DATETIME, @startDt + ' 00:00:00') AND CONVERT(DATETIME, @endDt + ' 23:59:59')
   AND (P.p_seq IN (SELECT p_seq FROM pjt_director WHERE uv_seq = @uv_seq) OR P.p_seq IN (SELECT p_seq FROM pjt_manager WHERE uv_seq = @uv_seq))
 GROUP BY pjt_status)AS A ";

          DynamicParameters param = new DynamicParameters();
          param.Add("@startDt", startDt, DbType.String);
          param.Add("@endDt", endDt, DbType.String);
          param.Add("@uv_seq", uv_seq, DbType.Int32);

          var ret = await con.QueryAsync<int>(selectQuery, param);

          con.Close();

          return ret.FirstOrDefault();
        }
      }
      catch (Exception e)
      {
        throw e;
      }
    }

    /// <summary>
    /// 수수료율
    /// </summary>
    /// <param name="startDt"></param>
    /// <param name="endDt"></param>
    /// <param name="uv_seq"></param>
    /// <returns></returns>
    public async Task<List<PieChartListModel>> SelectFeeRatePiePersonalAsync(string startDt, string endDt, int uv_seq)
    {
      try
      {
        List<PieChartListModel> list = new List<PieChartListModel>();

        using (IDbConnection con = new SqlConnection(base.BaseConnectionString))
        {
          con.Open();

          string selectQuery = @" SELECT TOP 3
       fee_rate AS title
      ,COUNT(0) AS allCnt
      ,ISNULL(SUM(billing_money), 0) AS billing_money
  FROM (SELECT CASE WHEN PI.fee_rate BETWEEN 0 AND 9 THEN '0% ~ 9%'
                    WHEN PI.fee_rate BETWEEN 10 AND 19 THEN '10% ~ 19%'
                    ELSE '20% 이상'
                END AS fee_rate
              ,ISNULL(SUM(PI.billing_money), 0) AS billing_money
          FROM project AS P INNER JOIN pjt_invoice_info AS PI
                                    ON P.p_seq = PI.p_seq 
  WHERE P.create_dt BETWEEN CONVERT(DATETIME, @startDt + ' 00:00:00') AND CONVERT(DATETIME, @endDt + ' 23:59:59')
    AND PI.is_deleted <> 1
    AND (P.p_seq IN (SELECT p_seq FROM pjt_director WHERE uv_seq = @uv_seq) OR P.p_seq IN (SELECT p_seq FROM pjt_manager WHERE uv_seq = @uv_seq))
  GROUP BY PI.fee_rate) AS A 
GROUP BY A.fee_rate 
ORDER BY billing_money ";

          DynamicParameters param = new DynamicParameters();
          param.Add("@startDt", startDt, DbType.String);
          param.Add("@endDt", endDt, DbType.String);
          param.Add("@uv_seq", uv_seq, DbType.Int32);

          var ret = await con.QueryAsync<PieChartListModel>(selectQuery, param);

          con.Close();

          list = ret.ToList();

          return list;
        }
      }
      catch (Exception e)
      {
        throw e;
      }
    }

    #endregion

    #region 년도별 매출통계 - 전체

    public async Task<List<SalesYearlySum>> SelectSalesYearlySumList(int ud_seq, int uv_seq)
    {
      try
      {
        List<SalesYearlySum> list = new List<SalesYearlySum>();

        using (IDbConnection con = new SqlConnection(base.BaseConnectionString))
        {
          con.Open();

          string selectQuery = @" SELECT A.billing_dt
      ,ISNULL(SUM(A.totalCnt), 0) AS total_cnt
      ,ISNULL(SUM(CAST(A.sales_money AS BIGINT)), 0) AS sales_money
  FROM (SELECT CONVERT(VARCHAR(4), PV.billing_dt, 121) AS billing_dt
              ,COUNT(0) AS totalCnt
              ,ISNULL(SUM(CAST(PS.sales_money AS BIGINT)), 0) AS sales_money
        FROM pjt_invoice_sales AS PS INNER JOIN pjt_invoice_info AS PV
                                             ON PS.pii_seq = PV.pii_seq
        WHERE PS.ud_seq = CASE WHEN @ud_seq <> 0 THEN @ud_seq ELSE PS.ud_seq END
          AND PS.uv_seq = CASE WHEN @uv_seq <> 0 THEN @uv_seq ELSE PS.uv_seq END
          AND PV.is_deleted <> 1
        GROUP BY PV.billing_dt) AS A
GROUP BY A.billing_dt
ORDER BY A.billing_dt ASC ";

          DynamicParameters param = new DynamicParameters();
          param.Add("@ud_seq", ud_seq, DbType.Int32);
          param.Add("@uv_seq", uv_seq, DbType.Int32);

          var ret = await con.QueryAsync<SalesYearlySum>(selectQuery, param);

          con.Close();

          list = ret.ToList();

          return list;
        }
      }
      catch (Exception e)
      {
        throw e;
      }
    }

    public async Task<SalesDetailSum> SumYearlySalesAllAsync(int ud_seq, int uv_seq)
    {
      try
      {
        using (SqlConnection con = new SqlConnection(base.BaseConnectionString))
        {
          con.Open();

          string selectQuery = @" SELECT CAST(SUM(CAST(PV.annual_income AS BIGINT)) AS VARCHAR(20)) annual_income
      ,CAST(SUM(CAST(PV.billing_money AS BIGINT)) AS VARCHAR(20))AS billing_money
      ,CAST(SUM(CAST(PS.sales_money AS BIGINT)) AS VARCHAR(20)) AS sales_money
FROM pjt_invoice_sales AS PS INNER JOIN pjt_invoice_info AS PV
                                     ON PS.pii_seq = PV.pii_seq
WHERE PS.ud_seq = CASE WHEN @ud_seq <> 0 THEN @ud_seq ELSE PS.ud_seq END 
  AND PS.uv_seq = CASE WHEN @uv_seq <> 0 THEN @uv_seq ELSE PS.uv_seq END 
  AND PV.is_deleted <> 1 ";

          DynamicParameters param = new DynamicParameters();
          param.Add("@ud_seq", ud_seq, DbType.Int32);
          param.Add("@uv_seq", uv_seq, DbType.Int32);

          //var ret = await con.QueryAsync<SalesDetailSum>(selectQuery, param);

          IDataReader reader = await con.ExecuteReaderAsync(selectQuery, param);
          SalesDetailSum data = new SalesDetailSum();
          while (reader.Read())
          {
            long sum_annual_income = 0;
            long sum_billing_money = 0;
            long sum_sales_money = 0;

            if (long.TryParse(reader["annual_income"].ToString(), out sum_annual_income))
              data.sum_annual_income = sum_annual_income;
            else
              data.sum_annual_income = 0;

            if (long.TryParse(reader["billing_money"].ToString(), out sum_billing_money))
              data.sum_billing_money = sum_billing_money;
            else
              data.sum_billing_money = 0;

            if (long.TryParse(reader["sales_money"].ToString(), out sum_sales_money))
              data.sum_sales_money = sum_sales_money;
            else
              data.sum_sales_money = 0;
          }

          con.Close();

          return data;
        }
      }
      catch (Exception e)
      {
        throw e;
      }
    }

    /// <summary>
    /// 년도별 매출통계 리스트 - 전체,팀,개인 (하단 리스트)
    /// </summary>
    /// <param name="skip"></param>
    /// <param name="count"></param>
    /// <param name="totalCount"></param>
    /// <returns></returns>
    public List<SalesDetailModel> SelectYearlySalesAllList(int ud_seq, int uv_seq, int skip, int count, out int totalCount)
    {
      try
      {
        List<SalesDetailModel> list = new List<SalesDetailModel>();

        using (IDbConnection con = new SqlConnection(base.BaseConnectionString))
        {
          con.Open();

          string selectQuery = @" SELECT C.c_seq AS client_seq
      ,C.kor_name AS client_name
      ,STUFF((SELECT ',' + B.name
                FROM pjt_director AS A INNER JOIN uv_user AS B
      			                              ON A.uv_seq = B.uv_seq
               WHERE A.p_seq = PS.p_seq
                 FOR XML PATH('')), 1, 1, '') AS am_name
      ,STUFF((SELECT ',' + B.name
                FROM pjt_manager AS A INNER JOIN uv_user AS B
                                              ON A.uv_seq = B.uv_seq
               WHERE A.p_seq = PS.p_seq
                 FOR XML PATH('')), 1, 1, '') AS searcher_name
      ,CC.name AS contact_name
      ,CD.c_seq AS candidate_seq
      ,CD.kor_name AS candidate_name
      ,PV.join_dt
      ,PV.billing_dt
      ,PV.deposit_dt
      ,ISNULL(PV.annual_income, 0) AS annual_income
      ,ISNULL(PV.fee_rate, 0) AS fee_rate
      ,ISNULL(PV.billing_money, 0) AS billing_money
      ,ISNULL(SUM(PS.sales_rate), 0) AS sales_rate
      ,ISNULL(SUM(PS.sales_money), 0) AS sales_money
FROM pjt_invoice_sales AS PS INNER JOIN pjt_invoice_info AS PV
                                     ON PS.pii_seq = PV.pii_seq
                        LEFT OUTER JOIN candidate AS CD
                                     ON PV.c_seq = CD.c_seq 
                        LEFT OUTER JOIN project AS P
                                     ON PS.p_seq = P.p_seq
                        LEFT OUTER JOIN client AS C
                                     ON P.c_seq = C.c_seq
                        LEFT OUTER JOIN client_contact AS CC
                                     ON P.cc_seq = CC.cc_seq
WHERE PS.ud_seq = CASE WHEN @ud_seq <> 0 THEN @ud_seq ELSE PS.ud_seq END
  AND PS.uv_seq = CASE WHEN @uv_seq <> 0 THEN @uv_seq ELSE PS.uv_seq END
  AND PV.is_deleted <> 1
GROUP BY C.c_seq, PS.p_seq, C.kor_name, CC.name, CD.c_seq, CD.kor_name, PV.join_dt, PV.billing_dt, PV.deposit_dt, PV.annual_income, PV.fee_rate, PV.billing_money
ORDER BY C.kor_name ASC ";
          selectQuery += " OFFSET @currentPage ROWS FETCH NEXT @pageSize ROWS ONLY ";

          string countQuery = @" SELECT COUNT(0)
  FROM (SELECT C.c_seq AS client_seq
        FROM pjt_invoice_sales AS PS INNER JOIN pjt_invoice_info AS PV
                                             ON PS.pii_seq = PV.pii_seq
                                LEFT OUTER JOIN candidate AS CD
                                             ON PV.c_seq = CD.c_seq 
                                LEFT OUTER JOIN project AS P
                                             ON PS.p_seq = P.p_seq
                                LEFT OUTER JOIN client AS C
                                             ON P.c_seq = C.c_seq
                                LEFT OUTER JOIN client_contact AS CC
                                             ON P.cc_seq = CC.cc_seq
        WHERE PS.ud_seq = CASE WHEN @ud_seq <> 0 THEN @ud_seq ELSE PS.ud_seq END
          AND PS.uv_seq = CASE WHEN @uv_seq <> 0 THEN @uv_seq ELSE PS.uv_seq END
          AND PV.is_deleted <> 1
        GROUP BY C.c_seq, PS.p_seq, C.kor_name, CC.name, CD.c_seq, CD.kor_name, PV.join_dt, PV.billing_dt, PV.deposit_dt, PV.annual_income, PV.fee_rate, PV.billing_money) AS A ";

          DynamicParameters param = new DynamicParameters();
          param.Add("@ud_seq", ud_seq, DbType.Int32);
          param.Add("@uv_seq", uv_seq, DbType.Int32);
          param.Add("@currentPage", skip, DbType.Int32);
          param.Add("@pageSize", count, DbType.Int32);

          DynamicParameters param2 = new DynamicParameters();
          param2.Add("@ud_seq", ud_seq, DbType.Int32);
          param2.Add("@uv_seq", uv_seq, DbType.Int32);

          var ret = con.Query<SalesDetailModel>(selectQuery, param);
          totalCount = con.QueryFirstOrDefault<int>(countQuery, param2);

          list = ret.ToList();

          con.Close();

          return list;
        }
      }
      catch (Exception e)
      {
        throw e;
      }
    }
    #endregion

    #region 년도별 통계 - 개인

    /// <summary>
    /// 산업파이
    /// </summary>
    /// <param name="startDt"></param>
    /// <param name="endDt"></param>
    /// <param name="uv_seq"></param>
    /// <returns></returns>
    public async Task<List<PieChartListModel>> SelectYearlyBusinessPiePersonalAsync(int uv_seq)
    {
      try
      {
        List<PieChartListModel> list = new List<PieChartListModel>();

        using (IDbConnection con = new SqlConnection(base.BaseConnectionString))
        {
          con.Open();

          string selectQuery = @" SELECT TOP 3
       A.code_name1 AS title
      ,A.allCnt
      ,A.billing_money
  FROM (SELECT  P.business_code1
               ,CB1.code_name1
               ,COUNT(0) AS allCnt
               ,ISNULL(SUM(PI.billing_money), 0) AS billing_money
           FROM project AS P INNER JOIN pjt_invoice_info AS PI
                                     ON P.p_seq = PI.p_seq
                             INNER JOIN code_business_mst AS CB1
                                     ON P.business_code1 = CB1.code1
          WHERE 1 = 1 
            AND PI.is_deleted <> 1
            AND (P.p_seq IN(select p_seq FROM pjt_director WHERE uv_seq = @uv_seq) OR P.p_seq IN(select p_seq FROM pjt_manager WHERE uv_seq = @uv_seq))
GROUP BY P.business_code1, CB1.code_name1, P.pjt_status ) AS A 
ORDER BY billing_money DESC ";

          DynamicParameters param = new DynamicParameters();
          param.Add("@uv_seq", uv_seq, DbType.Int32);

          var ret = await con.QueryAsync<PieChartListModel>(selectQuery, param);

          con.Close();

          list = ret.ToList();

          return list;
        }
      }
      catch (Exception e)
      {
        throw e;
      }
    }

    /// <summary>
    /// 개인 프로젝트 성공율
    /// </summary>
    /// <param name="startDt"></param>
    /// <param name="endDt"></param>
    /// <param name="uv_seq"></param>
    /// <returns></returns>
    public async Task<int> SelectYearlyPjtSuccessRateOneAsync(int uv_seq)
    {
      try
      {
        using (IDbConnection con = new SqlConnection(base.BaseConnectionString))
        {
          con.Open();

          string selectQuery = @" SELECT ISNULL(ROUND(SUM(CAST(cnt AS float)) / SUM(CAST(totalCnt AS float)) * 100, 0), 0)
  FROM ( 
SELECT COUNT(0) AS totalCnt
      ,CASE WHEN pjt_status = 5 THEN COUNT(0) ELSE 0 END AS cnt
  FROM project AS P
 WHERE (P.p_seq IN (SELECT p_seq FROM pjt_director WHERE uv_seq = @uv_seq) OR P.p_seq IN (SELECT p_seq FROM pjt_manager WHERE uv_seq = @uv_seq))
 GROUP BY pjt_status)AS A ";

          DynamicParameters param = new DynamicParameters();
          param.Add("@uv_seq", uv_seq, DbType.Int32);

          var ret = await con.QueryAsync<int>(selectQuery, param);

          con.Close();

          return ret.FirstOrDefault();
        }
      }
      catch (Exception e)
      {
        throw e;
      }
    }

    /// <summary>
    /// 수수료율
    /// </summary>
    /// <param name="startDt"></param>
    /// <param name="endDt"></param>
    /// <param name="uv_seq"></param>
    /// <returns></returns>
    public async Task<List<PieChartListModel>> SelectYearlyFeeRatePiePersonalAsync(int uv_seq)
    {
      try
      {
        List<PieChartListModel> list = new List<PieChartListModel>();

        using (IDbConnection con = new SqlConnection(base.BaseConnectionString))
        {
          con.Open();

          string selectQuery = @" SELECT TOP 3
       fee_rate AS title
      ,COUNT(0) AS allCnt
      ,ISNULL(SUM(billing_money), 0) AS billing_money
  FROM (SELECT CASE WHEN PI.fee_rate BETWEEN 0 AND 9 THEN '0% ~ 9%'
                    WHEN PI.fee_rate BETWEEN 10 AND 19 THEN '10% ~ 19%'
                    ELSE '20% 이상'
                END AS fee_rate
              ,ISNULL(SUM(PI.billing_money), 0) AS billing_money
          FROM project AS P INNER JOIN pjt_invoice_info AS PI
                                    ON P.p_seq = PI.p_seq 
  WHERE (P.p_seq IN (SELECT p_seq FROM pjt_director WHERE uv_seq = @uv_seq) OR P.p_seq IN (SELECT p_seq FROM pjt_manager WHERE uv_seq = @uv_seq))
    AND PI.is_deleted <> 1
  GROUP BY PI.fee_rate) AS A 
GROUP BY A.fee_rate 
ORDER BY billing_money ";

          DynamicParameters param = new DynamicParameters();
          param.Add("@uv_seq", uv_seq, DbType.Int32);

          var ret = await con.QueryAsync<PieChartListModel>(selectQuery, param);

          con.Close();

          list = ret.ToList();

          return list;
        }
      }
      catch (Exception e)
      {
        throw e;
      }
    }

    #endregion

    /// <summary>
    /// 산업별 프로젝트 통계
    /// </summary>
    /// <param name="start_date"></param>
    /// <param name="end_date"></param>
    /// <returns></returns>
    public async Task<List<BusiJobStatisticsModel>> SelectBusinessStatisticsList(BusiJobSearchModel search)
    {
      try
      {
        List<BusiJobStatisticsModel> list = new List<BusiJobStatisticsModel>();

        using (IDbConnection con = new SqlConnection(base.BaseConnectionString))
        {
          con.Open();

          string selectQuery = @" SELECT A.code_name1
      ,A.code_name2
      ,A.allCnt
      ,successCnt
      ,failCnt
      ,A.billing_money
  FROM (SELECT  P.business_code1
               ,P.business_code2
               ,CB1.code_name1
               ,CB2.code_name2
               ,CASE WHEN P.pjt_status = 5 THEN COUNT(P.p_seq) ELSE 0 END AS successCnt
               ,CASE WHEN P.pjt_status = 3 THEN COUNT(P.p_seq) ELSE 0 END AS failCnt
               ,COUNT(0) AS allCnt
               ,ISNULL(SUM(PI.billing_money), 0) AS billing_money
           FROM project AS P INNER JOIN pjt_invoice_info AS PI
                                     ON P.p_seq = PI.p_seq
                             INNER JOIN code_business_mst AS CB1
                                     ON P.business_code1 = CB1.code1
                             INNER JOIN code_business_dtl AS CB2
                                     ON P.business_code1 = CB2.code1
                                    AND P.business_code2 = CB2.code2
          WHERE 1 = 1 
            AND PI.is_deleted <> 1 ";

          if (!string.IsNullOrWhiteSpace(search.startDt) && !string.IsNullOrWhiteSpace(search.endDt))
            selectQuery += " AND P.create_dt BETWEEN CONVERT(DATETIME, @startDt + ' 00:00:00') AND CONVERT(DATETIME, @endDt + ' 23:59:59') ";

          if (!string.IsNullOrWhiteSpace(search.searchTxt))
            selectQuery += string.Format(" AND {0} LIKE '%' + @searchTxt + '%' ", search.searchOption);

          selectQuery += @" GROUP BY P.business_code1, P.business_code2, CB1.code_name1, CB2.code_name2, P.pjt_status ) AS A ";
          selectQuery += string.Format(" ORDER BY A.business_code1, A.business_code2, {0} {1} ", search.orderTxt, search.orderOption);

          DynamicParameters param = new DynamicParameters();
          param.Add("@startDt", search.startDt, DbType.String);
          param.Add("@endDt", search.endDt, DbType.String);
          param.Add("@searchTxt", search.searchTxt, DbType.String);

          var ret = await con.QueryAsync<BusiJobStatisticsModel>(selectQuery, param);

          list = ret.ToList();

          con.Close();

          return list;
        }
      }
      catch (Exception e)
      {
        throw e;
      }
    }

    /// <summary>
    /// 직무별 프로젝트 통계
    /// </summary>
    /// <param name="search"></param>
    /// <returns></returns>
    public async Task<List<BusiJobStatisticsModel>> SelectJobStatisticsList(BusiJobSearchModel search)
    {
      try
      {
        List<BusiJobStatisticsModel> list = new List<BusiJobStatisticsModel>();

        using (IDbConnection con = new SqlConnection(base.BaseConnectionString))
        {
          con.Open();

          string selectQuery = @" SELECT A.code_name1
      ,A.code_name2
      ,A.allCnt
      ,successCnt
      ,failCnt
      ,A.billing_money
  FROM (SELECT  P.job_code1
               ,P.job_code2
               ,CJ1.code_name1
               ,CJ2.code_name2
               ,CASE WHEN P.pjt_status = 5 THEN COUNT(P.p_seq) ELSE 0 END AS successCnt
               ,CASE WHEN P.pjt_status = 3 THEN COUNT(P.p_seq) ELSE 0 END AS failCnt
               ,COUNT(0) AS allCnt
               ,ISNULL(SUM(PI.billing_money), 0) AS billing_money
           FROM project AS P INNER JOIN pjt_invoice_info AS PI
                                     ON P.p_seq = PI.p_seq
                             INNER JOIN code_job1 AS CJ1
                                     ON P.job_code1 = CJ1.code1
                             INNER JOIN code_job2 AS CJ2
                                     ON P.job_code1 = CJ2.code1
                                    AND P.job_code2 = CJ2.code2
          WHERE 1 = 1 
            AND PI.is_deleted <> 1 ";

          if (!string.IsNullOrWhiteSpace(search.startDt) && !string.IsNullOrWhiteSpace(search.endDt))
            selectQuery += " AND P.create_dt BETWEEN CONVERT(DATETIME, @startDt + ' 00:00:00') AND CONVERT(DATETIME, @endDt + ' 23:59:59') ";

          if (!string.IsNullOrWhiteSpace(search.searchTxt))
          {
            if (search.searchOption == "c1")
              selectQuery += " AND CB1.code_name1 LIKE '%' + @searchTxt + '%' ";
            else if (search.searchOption == "c2")
              selectQuery += " AND CB2.code_name2 LIKE '%' + @searchTxt + '%' ";
          }

          selectQuery += @" GROUP BY P.job_code1, P.job_code2, CJ1.code_name1, CJ2.code_name2, P.pjt_status) AS A ";
          selectQuery += string.Format(" ORDER BY A.code_name1 ASC, {0} {1} ", search.orderTxt, search.orderOption);

          DynamicParameters param = new DynamicParameters();
          param.Add("@startDt", search.startDt, DbType.String);
          param.Add("@endDt", search.endDt, DbType.String);
          param.Add("@searchTxt", search.searchTxt, DbType.String);

          var ret = await con.QueryAsync<BusiJobStatisticsModel>(selectQuery, param);

          list = ret.ToList();

          con.Close();

          return list;
        }
      }
      catch (Exception e)
      {
        throw e;
      }
    }

    /// <summary>
    /// 고객사별 프로젝트 현황 합계
    /// </summary>
    /// <param name="search"></param>
    /// <returns></returns>
    public async Task<ClientProjectStatisticsModel> SumClientProjectStatisticsAsync(ClientProjectStatisticsSearchModel search)
    {
      try
      {
        using (IDbConnection con = new SqlConnection(base.BaseConnectionString))
        {
          con.Open();

          string selectQuery = @" SELECT ISNULL(SUM(P.progressCnt) + SUM(P.waitCnt) + SUM(P.failCnt) + SUM(P.successCnt), 0) AS allCnt
      ,ISNULL(SUM(P.progressCnt), 0) AS progressCnt
      ,ISNULL(SUM(P.waitCnt), 0) AS waitCnt
      ,ISNULL(SUM(P.failCnt), 0) AS failCnt
      ,ISNULL(SUM(P.completeCnt), 0) AS completeCnt
      ,ISNULL(SUM(P.successCnt), 0) AS successCnt
      ,ISNULL(SUM(P.billing_money), 0) AS billing_money
 FROM client AS C INNER JOIN (SELECT P.p_seq
                                    ,P.c_seq
                                    ,CASE WHEN P.pjt_status = 1 THEN COUNT(P.p_seq) ELSE 0 END AS progressCnt
                                    ,CASE WHEN P.pjt_status = 2 THEN COUNT(P.p_seq) ELSE 0 END AS waitCnt
                                    ,CASE WHEN P.pjt_status = 3 THEN COUNT(P.p_seq) ELSE 0 END AS failCnt
                                    ,CASE WHEN P.pjt_status = 4 THEN COUNT(P.p_seq) ELSE 0 END AS completeCnt
                                    ,CASE WHEN P.pjt_status = 5 THEN COUNT(P.p_seq) ELSE 0 END AS successCnt
                                    ,ISNULL(SUM(billing_money), 0) AS billing_money
                                FROM project AS P LEFT OUTER JOIN pjt_invoice_info AS PI
                                                               ON P.p_seq = PI.p_seq
                                                              AND PI.is_deleted <> 1 
                               WHERE 1 = 1 ";

          if (!string.IsNullOrWhiteSpace(search.startDt) && !string.IsNullOrWhiteSpace(search.endDt))
            selectQuery += " AND P.create_dt BETWEEN CONVERT(DATETIME, @startDt + ' 00:00:00') AND CONVERT(DATETIME, @endDt + ' 23:59:59') ";

          selectQuery += @" GROUP BY P.p_seq, P.c_seq, P.pjt_status) AS P
                          ON C.c_seq = P.c_seq ";

          if (!string.IsNullOrWhiteSpace(search.searchTxt))
            selectQuery += " WHERE C.kor_name LIKE '%' + @searchTxt + '%' ";


          DynamicParameters param = new DynamicParameters();
          param.Add("@startDt", search.startDt, DbType.String);
          param.Add("@endDt", search.endDt, DbType.String);
          param.Add("@searchTxt", search.searchTxt, DbType.String);

          var ret = await con.QueryAsync<ClientProjectStatisticsModel>(selectQuery, param);

          con.Close();

          return ret.FirstOrDefault();
        }
      }
      catch (Exception e)
      {
        throw e;
      }
    }

    /// <summary>
    /// 고객사별 프로젝트 현황
    /// </summary>
    /// <param name="search"></param>
    /// <param name="skip"></param>
    /// <param name="count"></param>
    /// <param name="totalCount"></param>
    /// <returns></returns>
    public List<ClientProjectStatisticsModel> SelectClientProjectStatisticsList(ClientProjectStatisticsSearchModel search, int skip, int count, out int totalCount)
    {
      try
      {
        List<ClientProjectStatisticsModel> list = new List<ClientProjectStatisticsModel>();

        using (IDbConnection con = new SqlConnection(base.BaseConnectionString))
        {
          con.Open();

          string selectQuery = @" SELECT C.c_seq
      ,C.kor_name
      ,ISNULL(SUM(P.progressCnt) + SUM(P.waitCnt) + SUM(P.failCnt) + SUM(P.successCnt), 0) AS allCnt
      ,ISNULL(SUM(P.progressCnt), 0) AS progressCnt
      ,ISNULL(SUM(P.waitCnt), 0) AS waitCnt
      ,ISNULL(SUM(P.failCnt), 0) AS failCnt
      ,ISNULL(SUM(P.completeCnt), 0) AS completeCnt
      ,ISNULL(SUM(P.successCnt), 0) AS successCnt
      ,ISNULL(SUM(P.billing_money), 0) AS billing_money
  FROM client AS C INNER JOIN (SELECT P.p_seq
                                     ,P.c_seq
                                     ,CASE WHEN P.pjt_status = 1 THEN COUNT(P.p_seq) ELSE 0 END AS progressCnt
                                     ,CASE WHEN P.pjt_status = 2 THEN COUNT(P.p_seq) ELSE 0 END AS waitCnt
                                     ,CASE WHEN P.pjt_status = 3 THEN COUNT(P.p_seq) ELSE 0 END AS failCnt
                                     ,CASE WHEN P.pjt_status = 4 THEN COUNT(P.p_seq) ELSE 0 END AS completeCnt
                                     ,CASE WHEN P.pjt_status = 5 THEN COUNT(P.p_seq) ELSE 0 END AS successCnt
                                     ,ISNULL(SUM(billing_money), 0) AS billing_money
                                 FROM project AS P LEFT OUTER JOIN pjt_invoice_info AS PI
                                                                ON P.p_seq = PI.p_seq
                                                               AND PI.is_deleted <> 1 
                                WHERE 1 = 1 ";

          if (!string.IsNullOrWhiteSpace(search.startDt) && !string.IsNullOrWhiteSpace(search.endDt))
            selectQuery += " AND P.open_dt BETWEEN CONVERT(DATETIME, @startDt + ' 00:00:00') AND CONVERT(DATETIME, @endDt + ' 23:59:59') ";

          selectQuery += @" GROUP BY P.p_seq, P.c_seq, P.pjt_status) AS P
                          ON C.c_seq = P.c_seq ";

          if (!string.IsNullOrWhiteSpace(search.searchTxt))
            selectQuery += " WHERE C.kor_name LIKE '%' + @searchTxt + '%' ";

          selectQuery += @" GROUP BY C.c_seq, C.kor_name ";
          selectQuery += string.Format(" ORDER BY {0} {1} ", search.orderTxt, search.orderOption);
          selectQuery += " OFFSET @currentPage ROWS FETCH NEXT @pageSize ROWS ONLY ";

          string countQuery = @" SELECT COUNT(0)
  FROM (SELECT C.c_seq
         FROM client AS C INNER JOIN (SELECT P.p_seq
                                            ,P.c_seq
                                            ,CASE WHEN P.pjt_status = 1 THEN COUNT(P.p_seq) ELSE 0 END AS progressCnt
                                            ,CASE WHEN P.pjt_status = 2 THEN COUNT(P.p_seq) ELSE 0 END AS waitCnt
                                            ,CASE WHEN P.pjt_status = 3 THEN COUNT(P.p_seq) ELSE 0 END AS failCnt
                                            ,CASE WHEN P.pjt_status = 4 THEN COUNT(P.p_seq) ELSE 0 END AS completeCnt
                                            ,CASE WHEN P.pjt_status = 5 THEN COUNT(P.p_seq) ELSE 0 END AS successCnt
                                            ,ISNULL(SUM(billing_money), 0) AS billing_money
                                        FROM project AS P LEFT OUTER JOIN pjt_invoice_info AS PI
                                                                       ON P.p_seq = PI.p_seq
                                                                      AND PI.is_deleted <> 1 
                                       WHERE 1 = 1 ";

          if (!string.IsNullOrWhiteSpace(search.startDt) && !string.IsNullOrWhiteSpace(search.endDt))
            countQuery += " AND P.open_dt BETWEEN CONVERT(DATETIME, @startDt + ' 00:00:00') AND CONVERT(DATETIME, @endDt + ' 23:59:59') ";

          countQuery += @" GROUP BY P.p_seq, P.c_seq, P.pjt_status) AS P
                          ON C.c_seq = P.c_seq ";


          if (!string.IsNullOrWhiteSpace(search.searchTxt))
            countQuery += " WHERE C.kor_name LIKE '%' + @searchTxt + '%' ";

          countQuery += " GROUP BY C.c_seq) AS A ";


          DynamicParameters param = new DynamicParameters();
          param.Add("@searchTxt", search.searchTxt, DbType.String);
          param.Add("@startDt", search.startDt, DbType.String);
          param.Add("@endDt", search.endDt, DbType.String);
          param.Add("@currentPage", skip, DbType.Int32);
          param.Add("@pageSize", count, DbType.Int32);

          DynamicParameters param2 = new DynamicParameters();
          param2.Add("@searchTxt", search.searchTxt, DbType.String);
          param2.Add("@startDt", search.startDt, DbType.String);
          param2.Add("@endDt", search.endDt, DbType.String);

          var ret = con.Query<ClientProjectStatisticsModel>(selectQuery, param);
          totalCount = con.QueryFirstOrDefault<int>(countQuery, param2);

          list = ret.ToList();

          con.Close();

          return list;
        }
      }
      catch (Exception e)
      {
        throw e;
      }
    }

    /// <summary>
    /// 매출내역 표 금액 합계
    /// </summary>
    /// <param name="search"></param>
    /// <returns></returns>
    public async Task<SalesHistorySumAmtModel> SumSalesHistoryStatisticsAsync(SaleHistorySearchModel search)
    {
      try
      {
        using (IDbConnection con = new SqlConnection(base.BaseConnectionString))
        {
          con.Open();

          string selectQuery = @" SELECT ISNULL(SUM(CAST(PI.annual_income AS BIGINT)), 0) AS sum_annual_income
      ,ISNULL(SUM(CAST(PI.billing_money AS BIGINT)), 0) AS sum_billing_money
      ,ISNULL(SUM(100000), 0) AS sum_auth_amt
  FROM project AS P INNER JOIN client AS C
                            ON P.c_seq = C.c_seq
                    INNER JOIN client_contact AS CC
                            ON P.c_seq = CC.c_seq
                           AND P.cc_seq = CC.cc_seq
                    INNER JOIN (SELECT PR.p_seq
                                      ,PR.c_seq
                                      ,RH.schedule_date
                                  FROM pjt_recandidate AS PR INNER JOIN pjt_recandidate_history AS RH
                                                                     ON PR.p_seq = RH.p_seq
                                                                    AND PR.pic_seq = RH.pic_seq
                                 WHERE RH.state = 80
                                   AND PR.c_seq NOT IN (SELECT c_seq FROM pjt_recandidate_history WHERE state = 10)) AS PR
                            ON P.p_seq = PR.p_seq
                    INNER JOIN candidate AS CA
                            ON PR.c_seq = CA.c_seq
                    INNER JOIN pjt_invoice_info AS PI
                            ON P.p_seq = PI.p_seq
                           AND PR.c_seq = PI.c_seq
 WHERE 1 = 1 
   AND PI.is_deleted <> 1 ";

          if (search.ud_seq > 0 && search.uv_seq == 0)
            selectQuery += " AND (P.p_seq IN (SELECT p_seq FROM pjt_director WHERE uv_seq IN (SELECT uv_seq FROM uv_user WHERE ud_seq = @ud_seq)) OR P.p_seq IN (SELECT p_seq FROM pjt_manager WHERE uv_seq IN (SELECT uv_seq FROM uv_user WHERE ud_seq = @ud_seq))) ";

          if (search.uv_seq > 0)
            selectQuery += " AND (P.p_seq IN(select p_seq FROM pjt_director WHERE uv_seq = @uv_seq) OR P.p_seq IN(select p_seq FROM pjt_manager WHERE uv_seq = @uv_seq)) ";

          if (!string.IsNullOrWhiteSpace(search.startDt) && !string.IsNullOrWhiteSpace(search.endDt))
            selectQuery += " AND P.create_dt BETWEEN CONVERT(DATETIME, @startDt + ' 00:00:00') AND CONVERT(DATETIME, @endDt + ' 23:59:59') ";

          DynamicParameters param = new DynamicParameters();
          param.Add("@ud_seq", search.ud_seq, DbType.Int32);
          param.Add("@uv_seq", search.uv_seq, DbType.Int32);
          param.Add("@startDt", search.startDt, DbType.String);
          param.Add("@endDt", search.endDt, DbType.String);

          var ret = await con.QueryAsync<SalesHistorySumAmtModel>(selectQuery, param);

          con.Close();

          return ret.FirstOrDefault();
        }
      }
      catch (Exception e)
      {
        throw e;
      }
    }

    /// <summary>
    /// 매출내역 표 셀렉트
    /// </summary>
    /// <param name="search"></param>
    /// <param name="skip"></param>
    /// <param name="count"></param>
    /// <param name="totalCount"></param>
    /// <returns></returns>
    public List<SalesHistoryModel> SelectSalesHistoryStatisticsList(SaleHistorySearchModel search, int skip, int count, out int totalCount)
    {
      try
      {
        List<SalesHistoryModel> list = new List<SalesHistoryModel>();

        using (IDbConnection con = new SqlConnection(base.BaseConnectionString))
        {
          con.Open();

          string selectQuery = @" SELECT P.p_seq
      ,C.c_seq AS client_seq
      ,C.kor_name AS client_name
      ,CC.name AS contact_name
      ,STUFF((SELECT ', ' + B.NAME
                FROM pjt_director A INNER JOIN uv_user B
                                            ON A.uv_seq = B.uv_seq
			   WHERE A.p_seq = P.p_seq
                 FOR XML PATH('')),1,1,'') AS am_name
      ,STUFF((SELECT ', ' + B.NAME
                FROM pjt_manager A INNER JOIN uv_user B
                                           ON A.uv_seq = B.uv_seq
			   WHERE A.p_seq = P.p_seq
                 FOR XML PATH('')),1,1,'') AS searcher_name
      ,PR.c_seq AS candidate_seq
      ,CA.kor_name AS candidate_name
      ,PR.schedule_date
      ,PI.billing_dt
      ,PI.deposit_dt
      ,ISNULL(PI.annual_income, 0) AS annual_income
      ,ISNULL(PI.fee_rate, 0) AS fee_rate
      ,ISNULL(PI.billing_money, 0) AS billing_money
      ,100 AS auth_rate
      ,100000 AS auth_amt
  FROM project AS P INNER JOIN client AS C
                            ON P.c_seq = C.c_seq
                    INNER JOIN client_contact AS CC
                            ON P.c_seq = CC.c_seq
                           AND P.cc_seq = CC.cc_seq
                    INNER JOIN (SELECT PR.p_seq
                                      ,PR.c_seq
                                      ,RH.schedule_date
                                  FROM pjt_recandidate AS PR INNER JOIN pjt_recandidate_history AS RH
                                                                     ON PR.p_seq = RH.p_seq
                                                                    AND PR.pic_seq = RH.pic_seq
                                 WHERE RH.state = 80
                                   AND PR.c_seq NOT IN (SELECT c_seq FROM pjt_recandidate_history WHERE state = 10)) AS PR
                            ON P.p_seq = PR.p_seq
                    INNER JOIN candidate AS CA
                            ON PR.c_seq = CA.c_seq
                    INNER JOIN pjt_invoice_info AS PI
                            ON P.p_seq = PI.p_seq
                           AND PR.c_seq = PI.c_seq
 WHERE 1 = 1 
   AND PI.is_deleted <> 1 ";

          if (search.ud_seq > 0 && search.uv_seq == 0)
            selectQuery += " AND (P.p_seq IN (SELECT p_seq FROM pjt_director WHERE uv_seq IN (SELECT uv_seq FROM uv_user WHERE ud_seq = @ud_seq)) OR P.p_seq IN (SELECT p_seq FROM pjt_manager WHERE uv_seq IN (SELECT uv_seq FROM uv_user WHERE ud_seq = @ud_seq))) ";

          if (search.uv_seq > 0)
            selectQuery += " AND (P.p_seq IN(select p_seq FROM pjt_director WHERE uv_seq = @uv_seq) OR P.p_seq IN(select p_seq FROM pjt_manager WHERE uv_seq = @uv_seq)) ";

          if (!string.IsNullOrWhiteSpace(search.startDt) && !string.IsNullOrWhiteSpace(search.endDt))
            selectQuery += " AND P.create_dt BETWEEN CONVERT(DATETIME, @startDt + ' 00:00:00') AND CONVERT(DATETIME, @endDt + ' 23:59:59') ";

          selectQuery += @" ORDER BY C.kor_name ASC OFFSET @currentPage ROWS FETCH NEXT @pageSize ROWS ONLY ";

          string countQuery = @" SELECT COUNT(0)
  FROM project AS P INNER JOIN client AS C
                            ON P.c_seq = C.c_seq
                    INNER JOIN client_contact AS CC
                            ON P.c_seq = CC.c_seq
                           AND P.cc_seq = CC.cc_seq
                    INNER JOIN (SELECT PR.p_seq
                                      ,PR.c_seq
                                      ,RH.schedule_date
                                  FROM pjt_recandidate AS PR INNER JOIN pjt_recandidate_history AS RH
                                                                     ON PR.p_seq = RH.p_seq
                                                                    AND PR.pic_seq = RH.pic_seq
                                 WHERE RH.state = 80
                                   AND PR.c_seq NOT IN (SELECT c_seq FROM pjt_recandidate_history WHERE state = 10)) AS PR
                            ON P.p_seq = PR.p_seq
                    INNER JOIN candidate AS CA
                            ON PR.c_seq = CA.c_seq
                    INNER JOIN pjt_invoice_info AS PI
                            ON P.p_seq = PI.p_seq
                           AND PR.c_seq = PI.c_seq
 WHERE 1 = 1 
   AND PI.is_deleted <> 1 ";

          if (search.ud_seq > 0 && search.uv_seq == 0)
            countQuery += " AND (P.p_seq IN (SELECT p_seq FROM pjt_director WHERE uv_seq IN (SELECT uv_seq FROM uv_user WHERE ud_seq = @ud_seq)) OR P.p_seq IN (SELECT p_seq FROM pjt_manager WHERE uv_seq IN (SELECT uv_seq FROM uv_user WHERE ud_seq = @ud_seq))) ";

          if (search.uv_seq > 0)
            countQuery += " AND (P.p_seq IN(select p_seq FROM pjt_director WHERE uv_seq = @uv_seq) OR P.p_seq IN(select p_seq FROM pjt_manager WHERE uv_seq = @uv_seq)) ";

          if (!string.IsNullOrWhiteSpace(search.startDt) && !string.IsNullOrWhiteSpace(search.endDt))
            countQuery += " AND P.create_dt BETWEEN CONVERT(DATETIME, @startDt + ' 00:00:00') AND CONVERT(DATETIME, @endDt + ' 23:59:59') ";


          DynamicParameters param = new DynamicParameters();
          param.Add("@ud_seq", search.ud_seq, DbType.Int32);
          param.Add("@uv_seq", search.uv_seq, DbType.Int32);
          param.Add("@startDt", search.startDt, DbType.String);
          param.Add("@endDt", search.endDt, DbType.String);
          param.Add("@currentPage", skip, DbType.Int32);
          param.Add("@pageSize", count, DbType.Int32);

          DynamicParameters param2 = new DynamicParameters();
          param2.Add("@ud_seq", search.ud_seq, DbType.Int32);
          param2.Add("@uv_seq", search.uv_seq, DbType.Int32);
          param2.Add("@startDt", search.startDt, DbType.String);
          param2.Add("@endDt", search.endDt, DbType.String);

          var ret = con.Query<SalesHistoryModel>(selectQuery, param);
          totalCount = con.QueryFirstOrDefault<int>(countQuery, param2);

          list = ret.ToList();

          con.Close();

          return list;
        }
      }
      catch (Exception e)
      {
        throw e;
      }
    }

    /// <summary>
    /// 직급별 매출내역 파이 차트
    /// </summary>
    /// <param name="search"></param>
    /// <returns></returns>
    public async Task<List<PieChartListModel>> SelectPositionPieStatisticsAsync(SaleHistorySearchModel search)
    {
      try
      {
        List<PieChartListModel> list = new List<PieChartListModel>();

        using (IDbConnection con = new SqlConnection(base.BaseConnectionString))
        {
          con.Open();

          string selectQuery = @" SELECT TOP 3
       CP.p_name AS title
      ,COUNT(0) AS allCnt
      ,ISNULL(SUM(PI.billing_money), 0) AS billing_money
  FROM project AS P INNER JOIN pjt_invoice_info AS PI
                            ON P.p_seq = PI.p_seq
                    INNER JOIN code_position AS CP
                            ON P.position_seq = CP.p_code 
 WHERE 1 = 1 
   AND PI.is_deleted <> 1 ";

          if (search.ud_seq > 0 && search.uv_seq == 0)
            selectQuery += " AND (P.p_seq IN (SELECT p_seq FROM pjt_director WHERE uv_seq IN (SELECT uv_seq FROM uv_user WHERE ud_seq = @ud_seq)) OR P.p_seq IN (SELECT p_seq FROM pjt_manager WHERE uv_seq IN (SELECT uv_seq FROM uv_user WHERE ud_seq = @ud_seq))) ";

          if (search.uv_seq > 0)
            selectQuery += " AND (P.p_seq IN(select p_seq FROM pjt_director WHERE uv_seq = @uv_seq) OR P.p_seq IN(select p_seq FROM pjt_manager WHERE uv_seq = @uv_seq)) ";

          if (!string.IsNullOrWhiteSpace(search.startDt) && !string.IsNullOrWhiteSpace(search.endDt))
            selectQuery += " AND P.create_dt BETWEEN CONVERT(DATETIME, @startDt + ' 00:00:00') AND CONVERT(DATETIME, @endDt + ' 23:59:59') ";

          selectQuery += @" GROUP BY P.position_seq, CP.p_name
 ORDER BY billing_money ";

          DynamicParameters param = new DynamicParameters();
          param.Add("@ud_seq", search.ud_seq, DbType.Int32);
          param.Add("@uv_seq", search.uv_seq, DbType.Int32);
          param.Add("@startDt", search.startDt, DbType.String);
          param.Add("@endDt", search.endDt, DbType.String);

          var ret = await con.QueryAsync<PieChartListModel>(selectQuery, param);

          con.Close();

          list = ret.ToList();

          return list;
        }
      }
      catch (Exception e)
      {
        throw e;
      }
    }

    /// <summary>
    /// 수수료율별 매출내역 파이 차트
    /// </summary>
    /// <param name="search"></param>
    /// <returns></returns>
    public async Task<List<PieChartListModel>> SelectFeeRatePieStatisticsAsync(SaleHistorySearchModel search)
    {
      try
      {
        List<PieChartListModel> list = new List<PieChartListModel>();

        using (IDbConnection con = new SqlConnection(base.BaseConnectionString))
        {
          con.Open();

          string selectQuery = @" SELECT TOP 3
       fee_rate AS title
      ,COUNT(0) AS allCnt
      ,ISNULL(SUM(billing_money), 0) AS billing_money
  FROM (SELECT CASE WHEN PI.fee_rate BETWEEN 0 AND 9 THEN '0% ~ 9%'
                    WHEN PI.fee_rate BETWEEN 10 AND 19 THEN '10% ~ 19%'
                    ELSE '20% 이상'
                END AS fee_rate
              ,ISNULL(SUM(PI.billing_money), 0) AS billing_money
          FROM project AS P INNER JOIN pjt_invoice_info AS PI
                                    ON P.p_seq = PI.p_seq 
 WHERE 1 = 1 
   AND PI.is_deleted <> 1 ";

          if (search.ud_seq > 0 && search.uv_seq == 0)
            selectQuery += " AND (P.p_seq IN (SELECT p_seq FROM pjt_director WHERE uv_seq IN (SELECT uv_seq FROM uv_user WHERE ud_seq = @ud_seq)) OR P.p_seq IN (SELECT p_seq FROM pjt_manager WHERE uv_seq IN (SELECT uv_seq FROM uv_user WHERE ud_seq = @ud_seq))) ";

          if (search.uv_seq > 0)
            selectQuery += " AND (P.p_seq IN(select p_seq FROM pjt_director WHERE uv_seq = @uv_seq) OR P.p_seq IN(select p_seq FROM pjt_manager WHERE uv_seq = @uv_seq)) ";

          if (!string.IsNullOrWhiteSpace(search.startDt) && !string.IsNullOrWhiteSpace(search.endDt))
            selectQuery += " AND P.create_dt BETWEEN CONVERT(DATETIME, @startDt + ' 00:00:00') AND CONVERT(DATETIME, @endDt + ' 23:59:59') ";

          selectQuery += @" GROUP BY PI.fee_rate) AS A 
GROUP BY A.fee_rate 
ORDER BY billing_money ";

          DynamicParameters param = new DynamicParameters();
          param.Add("@ud_seq", search.ud_seq, DbType.Int32);
          param.Add("@uv_seq", search.uv_seq, DbType.Int32);
          param.Add("@startDt", search.startDt, DbType.String);
          param.Add("@endDt", search.endDt, DbType.String);

          var ret = await con.QueryAsync<PieChartListModel>(selectQuery, param);

          con.Close();

          list = ret.ToList();

          return list;
        }
      }
      catch (Exception e)
      {
        throw e;
      }
    }

    /// <summary>
    /// 산업별 매출내역 파이 차트
    /// </summary>
    /// <param name="search"></param>
    /// <returns></returns>
    public async Task<List<PieChartListModel>> SelectBusinessPieStatisticsAsync(SaleHistorySearchModel search)
    {
      try
      {
        List<PieChartListModel> list = new List<PieChartListModel>();

        using (IDbConnection con = new SqlConnection(base.BaseConnectionString))
        {
          con.Open();

          string selectQuery = @" SELECT TOP 3
       A.code_name1 AS title
      ,A.allCnt
      ,A.billing_money
  FROM (SELECT  P.business_code1
               ,CB1.code_name1
               ,COUNT(0) AS allCnt
               ,ISNULL(SUM(PI.billing_money), 0) AS billing_money
           FROM project AS P INNER JOIN pjt_invoice_info AS PI
                                     ON P.p_seq = PI.p_seq
                             INNER JOIN code_business_mst AS CB1
                                     ON P.business_code1 = CB1.code1
          WHERE 1 = 1 
            AND PI.is_deleted <> 1 ";

          if (search.ud_seq > 0 && search.uv_seq == 0)
            selectQuery += " AND (P.p_seq IN (SELECT p_seq FROM pjt_director WHERE uv_seq IN (SELECT uv_seq FROM uv_user WHERE ud_seq = @ud_seq)) OR P.p_seq IN (SELECT p_seq FROM pjt_manager WHERE uv_seq IN (SELECT uv_seq FROM uv_user WHERE ud_seq = @ud_seq))) ";

          if (search.uv_seq > 0)
            selectQuery += " AND (P.p_seq IN(select p_seq FROM pjt_director WHERE uv_seq = @uv_seq) OR P.p_seq IN(select p_seq FROM pjt_manager WHERE uv_seq = @uv_seq)) ";

          if (!string.IsNullOrWhiteSpace(search.startDt) && !string.IsNullOrWhiteSpace(search.endDt))
            selectQuery += " AND P.create_dt BETWEEN CONVERT(DATETIME, @startDt + ' 00:00:00') AND CONVERT(DATETIME, @endDt + ' 23:59:59') ";

          selectQuery += @" GROUP BY P.business_code1, CB1.code_name1, P.pjt_status ) AS A 
          ORDER BY billing_money DESC ";

          DynamicParameters param = new DynamicParameters();
          param.Add("@ud_seq", search.ud_seq, DbType.Int32);
          param.Add("@uv_seq", search.uv_seq, DbType.Int32);
          param.Add("@startDt", search.startDt, DbType.String);
          param.Add("@endDt", search.endDt, DbType.String);

          var ret = await con.QueryAsync<PieChartListModel>(selectQuery, param);

          con.Close();

          list = ret.ToList();

          return list;
        }
      }
      catch (Exception e)
      {
        throw e;
      }
    }

    /// <summary>
    /// 직무별 매출내역 파이 차트
    /// </summary>
    /// <param name="search"></param>
    /// <returns></returns>
    public async Task<List<PieChartListModel>> SelectJobPieStatisticsAsync(SaleHistorySearchModel search)
    {
      try
      {
        List<PieChartListModel> list = new List<PieChartListModel>();

        using (IDbConnection con = new SqlConnection(base.BaseConnectionString))
        {
          con.Open();

          string selectQuery = @" SELECT TOP 3
       A.code_name1 AS title
      ,A.allCnt
      ,A.billing_money
  FROM (SELECT  P.job_code1
               ,CJ1.code_name1
               ,COUNT(0) AS allCnt
               ,ISNULL(SUM(PI.billing_money), 0) AS billing_money
           FROM project AS P INNER JOIN pjt_invoice_info AS PI
                                     ON P.p_seq = PI.p_seq
                             INNER JOIN code_job1 AS CJ1
                                     ON P.job_code1 = CJ1.code1
          WHERE 1 = 1 
            AND PI.is_deleted <> 1 ";

          if (search.ud_seq > 0 && search.uv_seq == 0)
            selectQuery += " AND (P.p_seq IN (SELECT p_seq FROM pjt_director WHERE uv_seq IN (SELECT uv_seq FROM uv_user WHERE ud_seq = @ud_seq)) OR P.p_seq IN (SELECT p_seq FROM pjt_manager WHERE uv_seq IN (SELECT uv_seq FROM uv_user WHERE ud_seq = @ud_seq))) ";

          if (search.uv_seq > 0)
            selectQuery += " AND (P.p_seq IN(select p_seq FROM pjt_director WHERE uv_seq = @uv_seq) OR P.p_seq IN(select p_seq FROM pjt_manager WHERE uv_seq = @uv_seq)) ";

          if (!string.IsNullOrWhiteSpace(search.startDt) && !string.IsNullOrWhiteSpace(search.endDt))
            selectQuery += " AND P.create_dt BETWEEN CONVERT(DATETIME, @startDt + ' 00:00:00') AND CONVERT(DATETIME, @endDt + ' 23:59:59') ";

          selectQuery += @" GROUP BY P.job_code1, P.job_code2, CJ1.code_name1, P.pjt_status) AS A 
          ORDER BY billing_money DESC ";

          DynamicParameters param = new DynamicParameters();
          param.Add("@ud_seq", search.ud_seq, DbType.Int32);
          param.Add("@uv_seq", search.uv_seq, DbType.Int32);
          param.Add("@startDt", search.startDt, DbType.String);
          param.Add("@endDt", search.endDt, DbType.String);

          var ret = await con.QueryAsync<PieChartListModel>(selectQuery, param);

          con.Close();

          list = ret.ToList();

          return list;
        }
      }
      catch (Exception e)
      {
        throw e;
      }
    }

    public async Task<List<can_activity>> SelectCandidateMemoList(UnivisionMemoSearchModel search)
    {
      try
      {
        List<can_activity> list = new List<can_activity>();

        using (IDbConnection con = new SqlConnection(base.BaseConnectionString))
        {
          con.Open();

          string selectQuery = @" 
select A.*, C.kor_name
FROM can_activity A LEFT JOIN candidate C
                    ON A.c_seq = C.c_seq
WHERE isnull(A.cl_seq, 0) <> 1
AND   CONVERT(VARCHAR(7), A.create_dt, 121) >= @startDt
AND   CONVERT(VARCHAR(7), A.create_dt, 121) <= @endDt";

          if (search.uv_seq > 0)
          {
            selectQuery = selectQuery + @" AND A.create_seq = @uv_seq ";
          }

          if (!String.IsNullOrEmpty(search.search_txt))
          {
            selectQuery = selectQuery + @" AND A.memo like '%'+@search_txt+'%' ";
          }

          selectQuery = selectQuery + @" ORDER BY A.create_dt DESC ";

          DynamicParameters param = new DynamicParameters();
          param.Add("@uv_seq", search.uv_seq, DbType.Int32);
          param.Add("@startDt", search.from_month_str, DbType.String);
          param.Add("@endDt", search.to_month_str, DbType.String);
          param.Add("@search_txt", search.search_txt, DbType.String);


          var ret = await con.QueryAsync<can_activity>(selectQuery, param);

          list = ret.ToList();

          con.Close();

          return list;
        }
      }
      catch (Exception e)
      {
        throw e;
      }
    }

    public async Task<List<PjtRecandidateStaticstic>> SelectPjtCandidateList(UnivisionPjtCandidateSearchModel search)
    {
      try
      {
        List<PjtRecandidateStaticstic> list = new List<PjtRecandidateStaticstic>();

        using (IDbConnection con = new SqlConnection(base.BaseConnectionString))
        {
          con.Open();

          string selectQuery = @" 
select 
  P.p_seq,
  P.title, 
  CL.kor_name as pjt_client,
  C.c_seq,
  C.kor_name, 
  C.birth_date, 
  C.gender,
  MIN(A.schedule_date) as schedule_date
from pjt_recandidate_history A inner join project P
                               on A.p_seq = P.p_Seq
                               inner join client CL
                               on P.c_Seq= CL.c_seq
                               inner JOIN candidate C
                               on A.c_seq = C.c_seq
                               inner join pjt_recandidate PR
                               on A.pic_Seq = PR.pic_seq

WHERE A.state = @status
";
          if (search.uv_seq > 0)
          {
            selectQuery = selectQuery + @" AND PR.create_user = @uv_seq ";
          }

          if (!String.IsNullOrEmpty(search.search_txt))
          {
            selectQuery = selectQuery + @" AND C.kor_name like '%'+@search_txt+'%' ";
          }

          selectQuery = selectQuery + @"
group by P.p_seq,P.title, CL.kor_name,C.c_seq, C.kor_name, birth_date, gender
HAVING CONVERT(VARCHAR(7), MIN(A.schedule_date), 121) >= @startDt
AND   CONVERT(VARCHAR(7), MIN(A.schedule_date), 121) <= @endDt
ORDER BY MIN(A.schedule_date) DESC ";

          DynamicParameters param = new DynamicParameters();
          param.Add("@uv_seq", search.uv_seq, DbType.Int32);
          param.Add("@status", search.status, DbType.Int32);
          param.Add("@startDt", search.from_month_str, DbType.String);
          param.Add("@endDt", search.to_month_str, DbType.String);
          param.Add("@search_txt", search.search_txt, DbType.String);

          var ret = await con.QueryAsync<PjtRecandidateStaticstic>(selectQuery, param);

          list = ret.ToList();

          con.Close();

          return list;
        }
      }
      catch (Exception e)
      {
        throw e;
      }
    }

    public async Task<List<PjtRecandidateStaticstic>> SelectPjtHireCandidateList(UnivisionPjtCandidateSearchModel search)
    {
      try
      {
        List<PjtRecandidateStaticstic> list = new List<PjtRecandidateStaticstic>();

        using (IDbConnection con = new SqlConnection(base.BaseConnectionString))
        {
          con.Open();

          string selectQuery = @" 
select 
  P.p_seq,
  P.title, 
  CL.kor_name as pjt_client,
  C.c_seq,
  C.kor_name, 
  C.birth_date, 
  C.gender,
  MIN(A.schedule_date) as schedule_date
from pjt_recandidate_history A inner join project P
                               on A.p_seq = P.p_Seq
                               inner join client CL
                               on P.c_Seq= CL.c_seq
                               inner JOIN candidate C
                               on A.c_seq = C.c_seq
                               inner join pjt_recandidate PR
                               on A.pic_Seq = PR.pic_seq
                               inner join pjt_invoice_info PI
                               on A.prc_seq = PI.prc_seq
                               AND PI.is_deleted = 0
WHERE A.state = @status
";
          if (search.uv_seq > 0)
          {
            selectQuery = selectQuery + @" AND PR.create_user = @uv_seq ";
          }

          if (!String.IsNullOrEmpty(search.search_txt))
          {
            selectQuery = selectQuery + @" AND C.kor_name like '%'+@search_txt+'%' ";
          }

          selectQuery = selectQuery + @"
group by P.p_seq,P.title, CL.kor_name,C.c_seq, C.kor_name, birth_date, gender
HAVING CONVERT(VARCHAR(7), MIN(A.schedule_date), 121) >= @startDt
AND   CONVERT(VARCHAR(7), MIN(A.schedule_date), 121) <= @endDt
ORDER BY MIN(A.schedule_date) DESC ";

          DynamicParameters param = new DynamicParameters();
          param.Add("@uv_seq", search.uv_seq, DbType.Int32);
          param.Add("@status", search.status, DbType.Int32);
          param.Add("@startDt", search.from_month_str, DbType.String);
          param.Add("@endDt", search.to_month_str, DbType.String);
          param.Add("@search_txt", search.search_txt, DbType.String);

          var ret = await con.QueryAsync<PjtRecandidateStaticstic>(selectQuery, param);

          list = ret.ToList();

          con.Close();

          return list;
        }
      }
      catch (Exception e)
      {
        throw e;
      }
    }

    public async Task<List<candidate>> SelectCandidateUpdateList(UnivisionCandidateSearchModel search)
    {
      try
      {
        List<candidate> list = new List<candidate>();

        using (IDbConnection con = new SqlConnection(base.BaseConnectionString))
        {
          con.Open();

          string selectQuery = @" 
select A.c_seq, C.kor_name, C.gender, C.birth_date, MAX(A.create_dt) as create_dt
FROM can_resume A INNER JOIN candidate C
                  ON A.c_seq = C.c_seq
WHERE CONVERT(VARCHAR(7), A.create_dt, 121) >= @startDt
AND   CONVERT(VARCHAR(7), A.create_dt, 121) <= @endDt
AND   A.create_user = @uv_seq
AND   A.remove_dt is null
AND A.c_seq NOT IN (SELECT c_seq FROM candidate WHERE create_seq = @uv_seq)
AND A.c_seq NOT IN (SELECT c_seq FROM candidate WHERE manager_seq = @uv_seq)";


          if (!String.IsNullOrEmpty(search.search_txt))
          {
            selectQuery = selectQuery + @" AND C.kor_name like '%'+@search_txt+'%' ";
          }

          selectQuery = selectQuery + @"
GROUP BY A.c_seq, C.kor_name, C.gender, C.birth_date
ORDER BY MAX(A.create_dt)  DESC ";

          DynamicParameters param = new DynamicParameters();
          param.Add("@uv_seq", search.uv_seq, DbType.Int32);
          param.Add("@startDt", search.from_month_str, DbType.String);
          param.Add("@endDt", search.to_month_str, DbType.String);
          param.Add("@search_txt", search.search_txt, DbType.String);

          var ret = await con.QueryAsync<candidate>(selectQuery, param);

          list = ret.ToList();

          con.Close();

          return list;
        }
      }
      catch (Exception e)
      {
        throw e;
      }
    }
    public async Task<List<candidate>> SelectCandidateRegistrationList(UnivisionCandidateSearchModel search)
    {
      try
      {
        List<candidate> list = new List<candidate>();

        using (IDbConnection con = new SqlConnection(base.BaseConnectionString))
        {
          con.Open();

          string selectQuery = @" 
select *
FROM candidate C
WHERE CONVERT(VARCHAR(7), C.create_dt, 121) >= @startDt
AND   CONVERT(VARCHAR(7), C.create_dt, 121) <= @endDt";

          if (search.is_direct == 1 && search.uv_seq > 0)
          {
            selectQuery = selectQuery + @" AND C.create_seq = @uv_seq ";
          }
          else
          {
            selectQuery = selectQuery + @" AND C.manager_seq = @uv_seq AND C.create_seq <> @uv_seq";
          }

          if (!String.IsNullOrEmpty(search.search_txt))
          {
            selectQuery = selectQuery + @" AND C.kor_name like '%'+@search_txt+'%' ";
          }

          selectQuery = selectQuery + @" ORDER BY C.create_dt DESC ";

          DynamicParameters param = new DynamicParameters();
          param.Add("@uv_seq", search.uv_seq, DbType.Int32);
          param.Add("@startDt", search.from_month_str, DbType.String);
          param.Add("@endDt", search.to_month_str, DbType.String);
          param.Add("@search_txt", search.search_txt, DbType.String);

          var ret = await con.QueryAsync<candidate>(selectQuery, param);

          list = ret.ToList();

          con.Close();

          return list;
        }
      }
      catch (Exception e)
      {
        throw e;
      }
    }

    public async Task<List<UnivisionActivityModel>> SelectUnivisionActivity(UnivisionActivitySearchModel search)
    {
      try
      {
        List<UnivisionActivityModel> list = new List<UnivisionActivityModel>();

        using (IDbConnection con = new SqlConnection(base.BaseConnectionString))
        {
          con.Open();
          string selectQuery = String.Empty;
          if (search.uv_seq > 0)
          {
            selectQuery = @" 
select LEFT(dv_date, 7) as mon
       , isnull(can_create_cnt, 0) as can_create_cnt  
       , isnull(can_create_cnt_agt, 0) as can_create_cnt_agt
       , isnull(can_update_cnt, 0) as can_update_cnt  
       , isnull(can_create_cnt, 0) + isnull(can_update_cnt, 0) + isnull(can_create_cnt_agt, 0) as can_create_update_cnt  
       , isnull(can_memo_cnt, 0) as can_memo_cnt  
       , isnull(pjt_rec_cnt, 0) as pjt_rec_cnt  
       , isnull(pjt_interview_cnt, 0) as pjt_interview_cnt  
       , isnull(pjt_nego_cnt, 0) as pjt_nego_cnt  
       , isnull(pjt_memo_cnt, 0) as pjt_memo_cnt  
       , isnull(can_memo_cnt, 0) + isnull(pjt_memo_cnt, 0) as can_pjt_memo_cnt
       , isnull(pjt_total_cnt, 0) as pjt_total_cnt  
       , isnull(pjt_cnt, 0) as pjt_cnt
       , isnull(pjt_invoice_cnt, 0) as pjt_invoice_cnt
       , isnull(pjt_hire_cnt, 0) as pjt_hire_cnt
from [dbo].DateRange('m', @startDt, @endDt) A LEFT JOIN (
      SELECT create_seq, dt, count(*) as can_create_cnt 
      FROM (SELECT create_seq, c_seq, CONVERT(VARCHAR(07), create_dt, 121) as dt
            FROM candidate WHERE create_seq = @uv_seq AND CONVERT(VARCHAR(10), create_dt, 121) BETWEEN @startDt AND  @endDt) x
      GROUP BY create_seq, dt) B
      ON LEFT(A.dv_date, 7) = B.dt
      LEFT JOIN (
      SELECT create_seq, dt, count(*) as can_create_cnt_agt
      FROM (
            SELECT manager_seq AS create_seq, c_seq, CONVERT(VARCHAR(07), create_dt, 121) as dt
            FROM candidate WHERE manager_seq = @uv_seq AND create_seq <> @uv_seq AND CONVERT(VARCHAR(10), create_dt, 121) BETWEEN @startDt AND  @endDt) X
      GROUP BY create_seq, dt) B1
      ON LEFT(A.dv_date, 7) = B1.dt
      LEFT JOIN (
      SELECT create_user, dt, count(*) as can_update_cnt 
      FROM (SELECT X.create_user, X.c_seq, CONVERT(VARCHAR(07), MAX(X.create_dt), 121) as dt
            FROM can_resume X INNER JOIN candidate Y
                              ON X.c_seq = Y.c_seq
            WHERE X.create_user = @uv_seq
            AND X.remove_dt is null
            AND X.c_seq NOT IN (SELECT c_seq FROM candidate WHERE create_seq = @uv_seq)
            AND X.c_seq NOT IN (SELECT c_seq FROM candidate WHERE manager_seq = @uv_seq)
            AND CONVERT(VARCHAR(10), X.create_dt, 121) BETWEEN @startDt AND  @endDt
            GROUP BY X.create_user, X.c_seq
            ) X
      GROUP BY create_user, dt) C
      ON LEFT(A.dv_date, 7) = C.dt
      LEFT JOIN (
      SELECT create_seq, dt, count(*) as can_memo_cnt 
      FROM (SELECT distinct X.create_seq, X.c_seq, CONVERT(VARCHAR(07), X.create_dt, 121) as dt
            FROM can_activity X inner join candidate Y on X.c_seq = Y.c_seq WHERE X.create_seq = @uv_seq AND isnull(X.cl_seq, 0) <> 1
            AND CONVERT(VARCHAR(10), X.create_dt, 121) BETWEEN @startDt AND  @endDt
            ) X
      GROUP BY create_seq, dt) D
      ON LEFT(A.dv_date, 7) = D.dt
      LEFT JOIN (
      SELECT dt, count(distinct pic_seq) as pjt_rec_cnt 
      FROM (
            SELECT X1.create_user, X1.pic_seq, CONVERT(VARCHAR(07), MAX(Y1.schedule_date), 121) as dt
            FROM pjt_recandidate X1 INNER JOIN pjt_recandidate_history Y1
                                    ON X1.pic_seq = Y1.pic_seq
                                    INNER JOIN project PP
                                    ON X1.p_seq = PP.p_seq
            WHERE X1.create_user = @uv_seq
            AND state = 30
            AND CONVERT(VARCHAR(10), Y1.schedule_date, 121) BETWEEN @startDt AND  @endDt
            GROUP BY X1.create_user, X1.pic_seq
            ) X
      GROUP BY dt) E
      ON LEFT(A.dv_date, 7) = E.dt
      LEFT JOIN (
      SELECT dt, count(distinct pic_seq) as pjt_interview_cnt 
      FROM (
            SELECT X1.create_user, X1.pic_seq, CONVERT(VARCHAR(07), MIN(Y1.schedule_date), 121) as dt
            FROM pjt_recandidate X1 INNER JOIN pjt_recandidate_history Y1
                                    ON X1.pic_seq = Y1.pic_seq
                                    INNER JOIN project PP
                                    ON X1.p_seq = PP.p_seq
            WHERE X1.create_user = @uv_seq
            AND state = 50
            AND CONVERT(VARCHAR(10), Y1.schedule_date, 121) BETWEEN @startDt AND  @endDt
            GROUP BY X1.create_user, X1.pic_seq
            ) X
      GROUP BY dt) F
      ON LEFT(A.dv_date, 7) = F.dt
      LEFT JOIN (
      SELECT dt, count(distinct pic_seq) as pjt_hire_cnt,
                 SUM(CASE WHEN bill_dt IS NULL THEN 0 ELSE 1 END) as pjt_invoice_cnt
      FROM (
            SELECT X1.create_user, X1.pic_seq, CONVERT(VARCHAR(07), MIN(Y1.schedule_date), 121) as dt, MIN(PI.billing_dt) as bill_dt
            FROM pjt_recandidate X1 INNER JOIN pjt_recandidate_history Y1
                                    ON X1.pic_seq = Y1.pic_seq
                                    INNER JOIN project PP
                                    ON X1.p_seq = PP.p_seq
                                    LEFT JOIN pjt_invoice_info PI
									                  ON Y1.prc_seq = PI.prc_seq
									                  AND PI.is_deleted = 0
            WHERE X1.create_user = @uv_seq
            AND state = 80
            AND CONVERT(VARCHAR(10), Y1.schedule_date, 121) BETWEEN @startDt AND  @endDt
            GROUP BY X1.create_user, X1.pic_seq
            ) X
      GROUP BY dt) H
      ON LEFT(A.dv_date, 7) = H.dt
      LEFT JOIN (
      SELECT dt, count(distinct pic_seq) as pjt_memo_cnt 
      FROM (
             SELECT X1.create_user, X1.pic_seq, CONVERT(VARCHAR(07), MIN(Y1.schedule_date), 121) as dt
            FROM pjt_recandidate X1 INNER JOIN pjt_recandidate_history Y1
                                    ON X1.pic_seq = Y1.pic_seq
                                    INNER JOIN project PP
                                    ON X1.p_seq = PP.p_seq
            WHERE X1.create_user = @uv_seq
            AND LEN(Y1.contents) > 30
            AND CONVERT(VARCHAR(10), Y1.schedule_date, 121) BETWEEN @startDt AND  @endDt
            GROUP BY X1.create_user, X1.pic_seq
            ) X
      GROUP BY dt) G
      ON LEFT(A.dv_date, 7) = G.dt
      LEFT JOIN (
      SELECT dt, count(distinct pic_seq) as pjt_nego_cnt 
      FROM (
            SELECT X1.create_user, X1.pic_seq, CONVERT(VARCHAR(07), MIN(Y1.schedule_date), 121) as dt
            FROM pjt_recandidate X1 INNER JOIN pjt_recandidate_history Y1
                                    ON X1.pic_seq = Y1.pic_seq
                                    INNER JOIN project PP
                                    ON X1.p_seq = PP.p_seq
            WHERE X1.create_user = @uv_seq
            AND state = 70
            AND CONVERT(VARCHAR(10), Y1.schedule_date, 121) BETWEEN @startDt AND  @endDt
            GROUP BY X1.create_user, X1.pic_seq
            ) X
      GROUP BY dt) K
      ON LEFT(A.dv_date, 7) = K.dt
      LEFT JOIN (SELECT CONVERT(VARCHAR(07), P.create_dt, 121) as dt, count(*) as pjt_cnt 
      FROM PROJECT P
      WHERE p_seq IN (
        SELECT p_seq FROM pjt_director WHERE uv_seq = @uv_seq
        union all
        SELECT p_seq FROM pjt_manager WHERE uv_seq = @uv_seq)
      AND CONVERT(VARCHAR(10), P.create_dt, 121) BETWEEN @startDt AND  @endDt
      GROUP BY CONVERT(VARCHAR(07), P.create_dt, 121)) J
      ON LEFT(A.dv_date, 7) = J.dt
      LEFT JOIN (SELECT count(*) as pjt_total_cnt 
      FROM PROJECT P
      WHERE p_seq IN (
        SELECT p_seq FROM pjt_director WHERE uv_seq = @uv_seq
        union all
        SELECT p_seq FROM pjt_manager WHERE uv_seq = @uv_seq )
      AND pjt_status = 1) I
      ON 1 = 1

ORDER BY mon DESC ";
          }
          else
          {
            selectQuery = @" 

select LEFT(dv_date, 7) as mon
       , isnull(can_create_cnt, 0) as can_create_cnt  
       , 0 as can_create_cnt_agt
       , isnull(can_update_cnt, 0) as can_update_cnt  
       , isnull(can_create_cnt, 0) + isnull(can_update_cnt, 0) as can_create_update_cnt  
       , isnull(can_memo_cnt, 0) as can_memo_cnt  
       , isnull(pjt_rec_cnt, 0) as pjt_rec_cnt  
       , isnull(pjt_interview_cnt, 0) as pjt_interview_cnt  
       , isnull(pjt_nego_cnt, 0) as pjt_nego_cnt  
       , isnull(pjt_memo_cnt, 0) as pjt_memo_cnt
       , isnull(can_memo_cnt, 0) + isnull(pjt_memo_cnt, 0) as can_pjt_memo_cnt
       , isnull(pjt_total_cnt, 0) as pjt_total_cnt  
       , isnull(pjt_cnt, 0) as pjt_cnt
       , isnull(pjt_invoice_cnt, 0) as pjt_invoice_cnt
       , isnull(pjt_hire_cnt, 0) as pjt_hire_cnt
from [dbo].DateRange('m', @startDt, @endDt) A LEFT JOIN (
      SELECT dt, count(*) as can_create_cnt 
      FROM (SELECT c_seq, CONVERT(VARCHAR(07), create_dt, 121) as dt
            FROM candidate 
            WHERE CONVERT(VARCHAR(10), create_dt, 121) BETWEEN @startDt AND  @endDt) x
      GROUP BY dt) B
      ON LEFT(A.dv_date, 7) = B.dt
      LEFT JOIN (
      SELECT 0 as create_seq, dt, count(*) as can_update_cnt 
      FROM (SELECT DISTINCT c_seq, CONVERT(VARCHAR(07), create_dt, 121) as dt
            FROM can_resume 
            WHERE c_seq NOT IN (SELECT c_seq FROM candidate ccc1 WHERE ccc1.create_seq = create_user)
            AND c_seq NOT IN (SELECT c_seq FROM candidate ccc2 WHERE ccc2.manager_seq = create_user)
            AND CONVERT(VARCHAR(10), create_dt, 121) BETWEEN @startDt AND  @endDt
            ) X
      GROUP BY dt) C
      ON LEFT(A.dv_date, 7) = C.dt
      LEFT JOIN (
      SELECT dt, count(*) as can_memo_cnt 
      FROM (SELECT DISTINCT create_seq, c_seq, CONVERT(VARCHAR(07), create_dt, 121) as dt
            FROM can_activity 
            WHERE CONVERT(VARCHAR(10), create_dt, 121) BETWEEN @startDt AND  @endDt
            ) X
      GROUP BY dt) D
      ON LEFT(A.dv_date, 7) = D.dt
      LEFT JOIN (
      SELECT dt, count(*) as pjt_rec_cnt 
      FROM (SELECT X1.pic_seq, CONVERT(VARCHAR(07), MIN(Y1.schedule_date), 121) as dt
            FROM pjt_recandidate X1 INNER JOIN pjt_recandidate_history Y1
                                    ON X1.pic_seq = Y1.pic_seq
                                    INNER JOIN project PP
                                    ON X1.p_seq = PP.p_seq
            WHERE state = 30
            AND CONVERT(VARCHAR(10), Y1.schedule_date, 121) BETWEEN @startDt AND  @endDt
            GROUP BY X1.pic_seq
            ) X
      GROUP BY dt) E
      ON LEFT(A.dv_date, 7) = E.dt
      LEFT JOIN (
	  SELECT dt, count(distinct pic_seq) as pjt_interview_cnt 
      FROM (
            SELECT X1.pic_seq, CONVERT(VARCHAR(07), MIN(Y1.schedule_date), 121) as dt
            FROM pjt_recandidate X1 INNER JOIN pjt_recandidate_history Y1
                                    ON X1.pic_seq = Y1.pic_seq
                                    INNER JOIN project PP
                                    ON X1.p_seq = PP.p_seq
            WHERE state = 50
            AND CONVERT(VARCHAR(10), Y1.schedule_date, 121) BETWEEN @startDt AND  @endDt
            GROUP BY X1.pic_seq
            ) X
      GROUP BY dt) F
      ON LEFT(A.dv_date, 7) = F.dt
      LEFT JOIN (
      SELECT dt, count(distinct pic_seq) as pjt_hire_cnt,
                 SUM(CASE WHEN bill_dt IS NULL THEN 0 ELSE 1 END) as pjt_invoice_cnt
      FROM (
            SELECT X1.create_user, X1.pic_seq, CONVERT(VARCHAR(07), MIN(Y1.schedule_date), 121) as dt, MIN(PI.billing_dt) as bill_dt
            FROM pjt_recandidate X1 INNER JOIN pjt_recandidate_history Y1
                                    ON X1.pic_seq = Y1.pic_seq
                                    INNER JOIN project PP
                                    ON X1.p_seq = PP.p_seq
                                    LEFT JOIN pjt_invoice_info PI
									                  ON Y1.prc_seq = PI.prc_seq
									                  AND PI.is_deleted = 0
            WHERE state >= 80
            AND CONVERT(VARCHAR(10), Y1.schedule_date, 121) BETWEEN @startDt AND  @endDt
            GROUP BY X1.create_user, X1.pic_seq
            ) X
      GROUP BY dt) H
      ON LEFT(A.dv_date, 7) = H.dt
      LEFT JOIN (
      SELECT dt, count(*) as pjt_memo_cnt 
      FROM (SELECT create_user, c_seq, CONVERT(VARCHAR(07), create_dt, 121) as dt
            FROM pjt_recandidate X1 WHERE pic_seq IN (SELECT pic_seq FROM pjt_recandidate_history WHERE LEN(contents) > 30 AND pic_seq = X1.pic_seq)
            AND CONVERT(VARCHAR(10), X1.create_dt, 121) BETWEEN @startDt AND  @endDt
            ) X
      GROUP BY dt) G
      ON LEFT(A.dv_date, 7) = G.dt
      LEFT JOIN (
      SELECT dt, count(*) as pjt_nego_cnt 
      FROM (SELECT X1.pic_seq, CONVERT(VARCHAR(07), MIN(Y1.schedule_date), 121) as dt
            FROM pjt_recandidate X1 INNER JOIN pjt_recandidate_history Y1
                                    ON X1.pic_seq = Y1.pic_seq
                                    INNER JOIN project PP
                                    ON X1.p_seq = PP.p_seq
            WHERE state = 70
            AND CONVERT(VARCHAR(10), Y1.schedule_date, 121) BETWEEN @startDt AND  @endDt
            GROUP BY X1.pic_seq
            ) X
      GROUP BY dt) K
      ON LEFT(A.dv_date, 7) = K.dt
      LEFT JOIN (
        SELECT CONVERT(VARCHAR(07), P.create_dt, 121) as dt, count(*) as pjt_cnt 
      FROM PROJECT P
      WHERE CONVERT(VARCHAR(10), P.create_dt, 121) BETWEEN @startDt AND  @endDt
      GROUP BY CONVERT(VARCHAR(07), P.create_dt, 121)) J
      ON LEFT(A.dv_date, 7) = J.dt
      LEFT JOIN (SELECT count(*) as pjt_total_cnt 
      FROM PROJECT P
      WHERE pjt_status = 1) I
      ON 1 = 1
ORDER BY mon DESC  ";
          }


          DynamicParameters param = new DynamicParameters();
          param.Add("@uv_seq", search.uv_seq, DbType.Int32);
          param.Add("@startDt", search.startDt, DbType.String);
          param.Add("@endDt", search.endDt, DbType.String);


          var ret = await con.QueryAsync<UnivisionActivityModel>(selectQuery, param, commandTimeout: 60);

          list = ret.ToList();

          con.Close();

          return list;
        }
      }
      catch (Exception e)
      {
        throw e;
      }
    }

    public async Task<List<ProjectActionModel>> SelectProjectActivity(ProjectActionSearchModel search)
    {
      try
      {
        List<ProjectActionModel> list = new List<ProjectActionModel>();

        using (IDbConnection con = new SqlConnection(base.BaseConnectionString))
        {
          con.Open();
          string selectQuery = String.Empty;

          selectQuery = @" 
select p.p_seq as [p_seq],
convert(varchar(30), p.create_dt, 121) as [create_dt],
isnull(convert(varchar(30), p.close_dt, 121), '') as [close_dt],
isnull(p.status_comment, '') as [status_comment],
cl.kor_name as [client_kor_name],
p.title as [pjt_title],
p.pjt_status as [pjt_status],
case p.pjt_status when 1 then '진행' when 2 then '보류' when 3 then '종료/취소' when 4 then '완료' when 5 then '성공' end as [pjt_status_name],
STUFF((SELECT ', ' + B.NAME
                FROM pjt_director A INNER JOIN uv_user B
                                            ON A.uv_seq = B.uv_seq
			   WHERE A.p_seq = P.p_seq
                 FOR XML PATH('')),1,1,'') AS [am]
      ,isnull(STUFF((SELECT ', ' + B.NAME
                FROM pjt_manager A INNER JOIN uv_user B
                                           ON A.uv_seq = B.uv_seq
			   WHERE A.p_seq = P.p_seq
               AND   A.uv_seq NOT IN (SELECT uv_seq FROM pjt_director WHERE p_seq = A.p_seq)
                 FOR XML PATH('')),1,1,''), '') AS [sm]
, isnull(convert(Varchar(10), b.c_seq), '') as [c_seq]
, isnull(c.kor_name, '') as [recan_name]
, isnull(case b.state when 20 then '잠재후보등록' 
               when 30 then '추천'
			   when 40 then '서류통과'  
			   when 50 then '면접' 
			   when 60 then '면접통과' 
			   when 70 then '협상검증' 
			   when 80 then '입사확정' 
			   when 10 then '탈락' 
			   when 13 then 'Self Drop' 
			   when 15 then '관심없음' end, '') as [recan_status]
, isnull(convert(varchar(10), b.schedule_date, 121),'') as [recan_schedule_dt]
, isnull(b.contents,'') as [recan_memo]
, isnull(u.name,'') as [recan_create_user_name]
, isnull(convert(varchar(30),b.create_dt, 121), '') as [recan_create_dt]
from project p left join pjt_recandidate_history b
on p.p_seq= b.p_seq
left join candidate c
on b.c_seq= c.c_Seq
left join client cl
on p.c_seq= cl.c_seq
left join uv_user u
on b.create_user = u.uv_seq

where ((b.create_dt > @startDt
and b.create_dt < @endDt  + ' 23:29:29.999') or (p.create_dt > @startDt
and p.create_dt < @endDt  + ' 23:29:29.999'))
order by case when prc_seq is null then 0 else 1 end, p.p_seq, prc_seq ";

          DynamicParameters param = new DynamicParameters();
          param.Add("@uv_seq", search.uv_seq, DbType.Int32);
          param.Add("@startDt", search.startDt, DbType.String);
          param.Add("@endDt", search.endDt, DbType.String);


          var ret = await con.QueryAsync<ProjectActionModel>(selectQuery, param, commandTimeout: 60);

          list = ret.ToList();

          con.Close();

          return list;
        }
      }
      catch (Exception e)
      {
        throw e;
      }
    }

    public async Task<List<CandidateRegModel>> SelectCandidateRegUser(CandidateRegSearchModel search)
    {
      try
      {
        List<CandidateRegModel> list = new List<CandidateRegModel>();

        using (IDbConnection con = new SqlConnection(base.BaseConnectionString))
        {
          con.Open();
          string selectQuery = String.Empty;

          selectQuery = @" 
select 
case when reg_type = 1 and grouping(reg_name) = 1 and grouping(reg_type) = 0 then '직접등록'
     when grouping(reg_name) = 1 and grouping(reg_type) = 1 then '합계' 
	 else reg_name end as reg_name,

count(*) as reg_count
from (
select case a.reg_type when 91 then 2 else 1 end as reg_type , 
       case a.reg_type when 91 then '홈페이지' else d.name end as reg_name
from candidate a left join uv_user d
                 on a.create_seq = d.uv_seq
where a.create_dt between @startDt and @endDt  + ' 23:29:29.999') X
group by reg_type,  reg_name
WITH ROLLUP
having NOT (grouping(reg_type) = 0 and reg_type = 2 and grouping(reg_name) = 1)
order by grouping(reg_type),  reg_type, grouping(reg_name), count(*) desc ";

          DynamicParameters param = new DynamicParameters();
          param.Add("@uv_seq", search.uv_seq, DbType.Int32);
          param.Add("@startDt", search.startDt, DbType.String);
          param.Add("@endDt", search.endDt, DbType.String);


          var ret = await con.QueryAsync<CandidateRegModel>(selectQuery, param, commandTimeout: 60);

          list = ret.ToList();

          con.Close();

          return list;
        }
      }
      catch (Exception e)
      {
        throw e;
      }
    }

    public async Task<List<CandidateRegModel>> SelectCandidateRegBusi(CandidateRegSearchModel search)
    {
      try
      {
        List<CandidateRegModel> list = new List<CandidateRegModel>();

        using (IDbConnection con = new SqlConnection(base.BaseConnectionString))
        {
          con.Open();
          string selectQuery = String.Empty;

          selectQuery = @" 
select 
 a.code1, isnull(a.code_name1, '소계') as code1_name, isnull(b.code_name2, '') as code2_name, 
 isnull(sum(c.cnt), 0) as reg_count
from code_business_mst a left join code_business_Dtl b
                      on a.code1 = b.code1
                      left join (select code1, code2, count(*) as cnt
                                from (
                                select row_number() over (partition by a.c_seq order by b.code1, b.code2) as rank, code1, code2
                                from candidate a left join can_business b
                                                 on a.c_seq = b.c_seq
                                where a.create_dt between @startDt and @endDt  + ' 23:29:29.999'
                                and isnull(code2, 0) <> 0) x 
                                where x.rank = 1
                                group by code1, code2) c
                      on a.code1 = c.code1
                      and b.code2 = c.code2
GROUP BY a.code1, a.code_name1, b.code_name2
WITH ROLLUP
HAVING NOT(grouping(a.code1) = 0 AND grouping(a.code_name1) = 0 AND grouping(b.code_name2) = 1)
ORDER BY grouping(a.code1), a.code1, 
grouping(b.code_name2), reg_count desc ";

          DynamicParameters param = new DynamicParameters();
          param.Add("@uv_seq", search.uv_seq, DbType.Int32);
          param.Add("@startDt", search.startDt, DbType.String);
          param.Add("@endDt", search.endDt, DbType.String);


          var ret = await con.QueryAsync<CandidateRegModel>(selectQuery, param, commandTimeout: 60);

          list = ret.ToList();

          con.Close();

          return list;
        }
      }
      catch (Exception e)
      {
        throw e;
      }
    }

    public async Task<List<CandidateRegModel>> SelectCandidateRegJob(CandidateRegSearchModel search)
    {
      try
      {
        List<CandidateRegModel> list = new List<CandidateRegModel>();

        using (IDbConnection con = new SqlConnection(base.BaseConnectionString))
        {
          con.Open();
          string selectQuery = String.Empty;

          selectQuery = @" 
select 
 a.code1, isnull(a.code_name1, '소계') as code1_name, isnull(b.code_name2, '') as code2_name, 
 isnull(sum(c.cnt), 0) as reg_count
from code_job_mst a left join code_job_dtl b
                      on a.code1 = b.code1
                      left join (select code1, code2, count(*) as cnt
                                from (
                                select row_number() over (partition by a.c_seq order by b.code1, b.code2) as rank, code1, code2
                                from candidate a left join can_job b
                                                 on a.c_seq = b.c_seq
                                where a.create_dt between @startDt and @endDt  + ' 23:29:29.999'
                                and isnull(code2, 0) <> 0) x 
                                where x.rank = 1
                                group by code1, code2) c
                      on a.code1 = c.code1
                      and b.code2 = c.code2
GROUP BY a.code1, a.code_name1, b.code_name2
WITH ROLLUP
HAVING NOT(grouping(a.code1) = 0 AND grouping(a.code_name1) = 0 AND grouping(b.code_name2) = 1)
ORDER BY grouping(a.code1), a.code1, 
grouping(b.code_name2), reg_count desc ";

          DynamicParameters param = new DynamicParameters();
          param.Add("@uv_seq", search.uv_seq, DbType.Int32);
          param.Add("@startDt", search.startDt, DbType.String);
          param.Add("@endDt", search.endDt, DbType.String);


          var ret = await con.QueryAsync<CandidateRegModel>(selectQuery, param, commandTimeout: 60);

          list = ret.ToList();

          con.Close();

          return list;
        }
      }
      catch (Exception e)
      {
        throw e;
      }
    }

    public async Task<List<MonthlyKPI>> SelectMonthlyKPI(MonthlyKPISearchModel search)
    {
      try
      {
        List<MonthlyKPI> list = new List<MonthlyKPI>();

        using (IDbConnection con = new SqlConnection(base.BaseConnectionString))
        {
          con.Open();
          string selectQuery = String.Empty;

          selectQuery = @" 
select 
         U.name as uv_name
       , dv.ud_name
       , LEFT(dv_date, 7) as mon
       , isnull(can_create_cnt, 0) as can_reg_dr
       , isnull(can_create_cnt_agt, 0) as can_reg_ag
       , isnull(can_update_cnt, 0) as can_udt
       , isnull(can_create_cnt, 0) + isnull(can_create_cnt_agt, 0) as can_reg_all  
       , isnull(can_memo_cnt, 0) as can_memo
       , isnull(pjt_rec_cnt, 0) as rec_cnt
       , isnull(pjt_recpjt_cnt, 0) as rec_pjt_cnt
       , isnull(pjt_interview_cnt, 0) as int_cnt
       , isnull(pjt_cnt_am, 0) as pjt_cnt_am
       , isnull(pjt_cnt_sm, 0) as pjt_cnt_sm
from UV_USER U LEFT JOIN [dbo].DateRange('m', @startDt, @endDt) A 
      ON 1 = 1
	  left join uv_division Dv
	  on U.ud_seq = Dv.ud_seq
      LEFT JOIN (
      SELECT create_seq, dt, count(*) as can_create_cnt 
      FROM (SELECT create_seq, c_seq, CONVERT(VARCHAR(07), create_dt, 121) as dt
            FROM candidate ) x
      GROUP BY create_seq, dt) B
      ON LEFT(A.dv_date, 7) = B.dt
      AND U.uv_seq = B.create_seq
      LEFT JOIN (
      SELECT create_seq, dt, count(*) as can_create_cnt_agt
      FROM (
            SELECT manager_seq AS create_seq, c_seq, CONVERT(VARCHAR(07), create_dt, 121) as dt
            FROM candidate WHERE manager_seq <> create_seq) X
      GROUP BY create_seq, dt) B1
      ON LEFT(A.dv_date, 7) = B1.dt
      AND U.uv_seq = B1.create_seq
      LEFT JOIN (
      SELECT create_user, dt, count(*) as can_update_cnt 
      FROM (SELECT A.create_user, A.c_seq, CONVERT(VARCHAR(07), MAX(A.create_dt), 121) as dt
                          from can_resume A inner join candidate B
                                            ON A.c_Seq = B.c_seq
                          where A.create_user <> B.create_seq 
                          and A.create_user <> ISNULL(B.manager_seq, 0)
						  AND A.remove_dt is null
                          GROUP BY A.create_user, A.c_seq
            ) X
      GROUP BY create_user, dt) C
      ON LEFT(A.dv_date, 7) = C.dt
      AND U.uv_seq = C.create_user
      LEFT JOIN (
      SELECT create_seq, dt, count(*) as can_memo_cnt 
      FROM (SELECT distinct X.create_seq, X.c_seq, CONVERT(VARCHAR(07), X.create_dt, 121) as dt
            FROM can_activity X inner join candidate Y on X.c_seq = Y.c_seq  WHERE isnull(X.cl_seq, 0) <> 1
            ) X
      GROUP BY create_seq, dt) D
      ON LEFT(A.dv_date, 7) = D.dt
      AND U.uv_seq = D.create_seq
      LEFT JOIN (
      SELECT create_user, dt, count(distinct pic_seq) as pjt_rec_cnt 
      FROM (
            SELECT X1.create_user, X1.pic_seq, CONVERT(VARCHAR(07), MAX(Y1.schedule_date), 121) as dt
            FROM pjt_recandidate X1 INNER JOIN pjt_recandidate_history Y1
                                    ON X1.pic_seq = Y1.pic_seq
                                    INNER JOIN project PP
                                    ON X1.p_seq = PP.p_seq
            WHERE state = 30
            GROUP BY X1.create_user, X1.pic_seq
            ) X
      GROUP BY create_user, dt) E
      ON LEFT(A.dv_date, 7) = E.dt
      AND U.uv_seq = E.create_user
      
      LEFT JOIN (
      SELECT create_user, dt, count(distinct p_seq) as pjt_recpjt_cnt 
      FROM (
            SELECT X1.create_user, X1.p_seq, CONVERT(VARCHAR(07), MAX(Y1.schedule_date), 121) as dt
            FROM pjt_recandidate X1 INNER JOIN pjt_recandidate_history Y1
                                    ON X1.pic_seq = Y1.pic_seq
                                    INNER JOIN project PP
                                    ON X1.p_seq = PP.p_seq
            WHERE state = 30
            GROUP BY X1.create_user, X1.p_seq
            ) X
      GROUP BY create_user, dt) E2
      ON LEFT(A.dv_date, 7) = E2.dt
      AND U.uv_seq = E2.create_user


      LEFT JOIN (
      SELECT create_user, dt, count(distinct pic_seq) as pjt_interview_cnt 
      FROM (
            SELECT X1.create_user, X1.pic_seq, CONVERT(VARCHAR(07), MAX(Y1.schedule_date), 121) as dt
            FROM pjt_recandidate X1 INNER JOIN pjt_recandidate_history Y1
                                    ON X1.pic_seq = Y1.pic_seq
                                    INNER JOIN project PP
                                    ON X1.p_seq = PP.p_seq
            WHERE state = 50
            GROUP BY X1.create_user, X1.pic_seq
            ) X
      GROUP BY create_user, dt) F
      ON LEFT(A.dv_date, 7) = F.dt
      AND U.uv_seq = F.create_user

      LEFT JOIN (SELECT PDD.uv_seq as uv_seq, CONVERT(VARCHAR(07), P.create_dt, 121) as dt, count(*) as pjt_cnt_am
      FROM PROJECT P LEFT JOIN pjt_director PDD
                     ON P.p_seq = PDD.p_seq      
      WHERE CONVERT(VARCHAR(10), P.create_dt, 121) BETWEEN @startDt AND  @endDt
      GROUP BY PDD.uv_seq, CONVERT(VARCHAR(07), P.create_dt, 121)) J
      ON LEFT(A.dv_date, 7) = J.dt
      AND U.uv_seq = J.uv_seq

      LEFT JOIN (SELECT PMM.uv_seq as uv_seq, CONVERT(VARCHAR(07), P.create_dt, 121) as dt, count(*) as pjt_cnt_sm
      FROM PROJECT P LEFT JOIN pjt_manager PMM
                     ON P.p_seq = PMM.p_seq      
      WHERE CONVERT(VARCHAR(10), P.create_dt, 121) BETWEEN @startDt AND  @endDt
      GROUP BY PMM.uv_seq, CONVERT(VARCHAR(07), P.create_dt, 121)) K
      ON LEFT(A.dv_date, 7) = K.dt
      AND U.uv_seq = K.uv_seq
where U.retire_date is null
and U.uv_seq not in (
16, 14, 22
, 138, 328, 354
)
and u.ud_seq not in (69, 11, 4, 70)
and U.user_id not like '%intern%' 
order by u.name ";

          DynamicParameters param = new DynamicParameters();
          param.Add("@startDt", search.startDt, DbType.String);
          param.Add("@endDt", search.endDt, DbType.String);


          var ret = await con.QueryAsync<MonthlyKPI>(selectQuery, param, commandTimeout: 60);

          list = ret.ToList();

          con.Close();

          return list;
        }
      }
      catch (Exception e)
      {
        throw e;
      }
    }

    public async Task<List<KeyProjectStatistic>> SelectKeyProject(KeyProjectSearchModel search)
    {
      try
      {
        List<KeyProjectStatistic> list = new List<KeyProjectStatistic>();

        using (IDbConnection con = new SqlConnection(base.BaseConnectionString))
        {
          con.Open();
          string selectQuery = String.Empty;

          selectQuery = @" 
select 
     a.p_seq,
     a.uv_seq,
     u.name as key_user_name,
     CONVERT(VARCHAR(10), A.create_dt, 121) as key_create_dt,
     B.client_kor_name as client_name,
	   B.title,
	   B.pjt_status_str,
	   B.am_names,
	   B.searcher_names,
	   B.open_dt_str as open_dt,
	   B.close_dt_str as close_dt,
     ISNULL((SELECT COUNT(*) FROM pjt_recandidate WHERE p_seq = B.p_seq), 0) as interest_cnt,
     ISNULL(recommandCnt, 0) as recommend_cnt,
     ISNULL(afterInterviewCnt, 0) as interview_cnt,  
     ISNULL(negoCnt, 0) as nego_cnt  
from pjt_mykey A inner join view_project_all B
                 on A.p_seq = B.p_seq
                 left join uv_user U
                 on A.uv_seq = U.uv_seq
                 LEFT OUTER JOIN (SELECT p_seq,
                                        COUNT(DISTINCT pic_seq) as recommandCnt
                                 FROM pjt_recandidate_history A
                                 WHERE A.state >= 30 
                                 GROUP BY p_seq) AS PR3
                            ON B.p_seq = PR3.p_seq
                LEFT OUTER JOIN (SELECT p_seq,
                                      COUNT(DISTINCT pic_seq) as afterInterviewCnt
                                 FROM pjt_recandidate_history A
                                 WHERE A.state >= 50 
                                 GROUP BY p_seq) AS PR4
                            ON B.p_seq = PR4.p_seq
                LEFT OUTER JOIN (SELECT p_seq,
                                      COUNT(DISTINCT pic_seq) as negoCnt
                                 FROM pjt_recandidate_history A
                                 WHERE A.state >= 70 
                                 GROUP BY p_seq) AS PR5
                            ON B.p_seq = PR5.p_seq 
WHERE 1 = 1";
          if (!String.IsNullOrEmpty(search.startDt) && !String.IsNullOrEmpty(search.endDt))
          {
            selectQuery = selectQuery + @" 
AND U.retire_date is null
and A.create_dt >= @startDt
AND CONVERT(VARCHAR(10), A.create_dt, 121) <= @endDt
";
          }

          if (search.uv_seq > 0)
          {
            selectQuery = selectQuery + @" AND A.uv_seq = @uv_seq ";
          }

          selectQuery = selectQuery + @"order by A.create_dt desc ";
/*
 and U.uv_seq not in (
16, 14, 22
, 138, 328, 354
)
and u.ud_seq not in (69, 11, 4, 70)
*/
          DynamicParameters param = new DynamicParameters();
          param.Add("@startDt", search.startDt, DbType.String);
          param.Add("@endDt", search.endDt, DbType.String);
          param.Add("@uv_seq", search.uv_seq, DbType.Int32);


          var ret = await con.QueryAsync<KeyProjectStatistic>(selectQuery, param, commandTimeout: 60);

          list = ret.ToList();

          con.Close();

          return list;
        }
      }
      catch (Exception e)
      {
        throw e;
      }
    }

    public async Task<List<KeyProjectStatistic>> SelectProjectStatus(KeyProjectSearchModel search)
    {
      try
      {
        List<KeyProjectStatistic> list = new List<KeyProjectStatistic>();

        using (IDbConnection con = new SqlConnection(base.BaseConnectionString))
        {
          con.Open();
          string selectQuery = String.Empty;

          selectQuery = @" 
select 
     b.p_seq,
     B.client_kor_name as client_name,
	   B.title,
	   B.pjt_status_str,
	   B.am_names,
	   B.searcher_names,
	   B.open_dt_str as open_dt,
	   B.close_dt_str as close_dt,
     ISNULL((SELECT COUNT(*) FROM pjt_recandidate WHERE p_seq = B.p_seq), 0) as interest_cnt,
     ISNULL(recommandCnt, 0) as recommend_cnt,
     ISNULL(afterInterviewCnt, 0) as interview_cnt,  
     ISNULL(negoCnt, 0) as nego_cnt  
from view_project_all B LEFT OUTER JOIN (SELECT A.p_seq,
                                        COUNT(DISTINCT A.pic_seq) as recommandCnt
                                 FROM pjt_recandidate_history A left join pjt_recandidate B
                                                                ON A.pic_seq = B.pic_seq
                                 WHERE A.state >= 30 
                                 AND B.create_user = @uv_seq
                                 GROUP BY A.p_seq) AS PR3
                            ON B.p_seq = PR3.p_seq
                LEFT OUTER JOIN (SELECT A.p_seq,
                                      COUNT(DISTINCT A.pic_seq) as afterInterviewCnt
                                 FROM pjt_recandidate_history A left join pjt_recandidate B
                                                                ON A.pic_seq = B.pic_seq
                                 WHERE A.state >= 50 
                                 AND B.create_user = @uv_seq
                                 GROUP BY A.p_seq) AS PR4
                            ON B.p_seq = PR4.p_seq
                LEFT OUTER JOIN (SELECT A.p_seq,
                                      COUNT(DISTINCT A.pic_seq) as negoCnt
                                 FROM pjt_recandidate_history A left join pjt_recandidate B
                                                                ON A.pic_seq = B.pic_seq
                                 WHERE A.state >= 70 
                                 AND B.create_user = @uv_seq
                                 GROUP BY A.p_seq) AS PR5
                            ON B.p_seq = PR5.p_seq 
WHERE 1 = 1";
          if (!String.IsNullOrEmpty(search.startDt) && !String.IsNullOrEmpty(search.endDt))
          {
            selectQuery = selectQuery + @" 
and B.create_dt_str >= @startDt
AND B.create_dt_str <= @endDt
AND (b.p_seq in (select p_seq FROM pjt_manager where uv_seq = @uv_seq) 
OR b.p_seq in (select p_seq FROM pjt_director where uv_seq = @uv_seq))
";
          }

        

          if (search.is_progress_only == 1)
          {
            selectQuery = selectQuery + @" AND b.pjt_status = 1 ";
          }

          selectQuery = selectQuery + @"order by b.p_seq desc ";
          /*
           and U.uv_seq not in (
          16, 14, 22
          , 138, 328, 354
          )
          and u.ud_seq not in (69, 11, 4, 70)
          */
          DynamicParameters param = new DynamicParameters();
          param.Add("@startDt", search.startDt, DbType.String);
          param.Add("@endDt", search.endDt, DbType.String);
          param.Add("@uv_seq", search.uv_seq, DbType.Int32);


          var ret = await con.QueryAsync<KeyProjectStatistic>(selectQuery, param, commandTimeout: 60);

          list = ret.ToList();

          con.Close();

          return list;
        }
      }
      catch (Exception e)
      {
        throw e;
      }
    }

    public async Task<List<NegoProjectStatistic>> SelectNegoProject(NegoProjectSearchModel search)
    {
      try
      {
        List<NegoProjectStatistic> list = new List<NegoProjectStatistic>();

        using (IDbConnection con = new SqlConnection(base.BaseConnectionString))
        {
          con.Open();
          string selectQuery = String.Empty;

          selectQuery = @" 
select 
a.p_seq,
cl.kor_name as [client_name],
p.title, 
p.pjt_status,
ISNULL(p.exp_salary_won, 0) as exp_salary_won,
ISNULL(p.exp_salary, 0) as exp_salary,
p.currency_cd,
p.fee_rate,
CONVERT(INT, (ISNULL(p.exp_salary_won, 0) * ISNULL(fee_rate, 0) / 100)) as exp_sales,
case p.pjt_status when 1 then '진행' when 2 then '보류' when 3 then '종료/취소' when 4 then '완료' when 5 then '성공' end as [pjt_status_str],
STUFF((SELECT ', ' + B.NAME
                FROM pjt_director A INNER JOIN uv_user B
                                            ON A.uv_seq = B.uv_seq
			   WHERE A.p_seq = P.p_seq
                 FOR XML PATH('')),1,1,'') AS [am_names]
      ,isnull(STUFF((SELECT ', ' + B.NAME
                FROM pjt_manager A INNER JOIN uv_user B
                                           ON A.uv_seq = B.uv_seq
			   WHERE A.p_seq = P.p_seq
               AND   A.uv_seq NOT IN (SELECT uv_seq FROM pjt_director WHERE p_seq = A.p_seq)
                 FOR XML PATH('')),1,1,''), '') AS [searcher_names]
, isnull(c.kor_name, '') as [recan_name]
, isnull(case a.state when 20 then '잠재후보등록' 
               when 30 then '추천'
			   when 40 then '서류통과'  
			   when 50 then '면접' 
			   when 60 then '면접통과' 
			   when 70 then '협상검증' 
			   when 80 then '입사확정' 
			   when 10 then '탈락' 
			   when 13 then 'Self Drop' 
			   when 15 then '관심없음' end, '') as [recan_status]
, isnull(convert(varchar(10), a.schedule_date, 121),'') as [recan_schedule_dt]
, isnull(a.contents,'') as [recan_memo]
, isnull(u.name,'') as [recan_create_user_name]
, isnull(convert(varchar(30),a.create_dt, 121), '') as [recan_create_dt]
, isnull(case b.state when 20 then '잠재후보등록' 
               when 30 then '추천'
			   when 40 then '서류통과'  
			   when 50 then '면접' 
			   when 60 then '면접통과' 
			   when 70 then '협상검증' 
			   when 80 then '입사확정' 
			   when 10 then '탈락' 
			   when 13 then 'Self Drop' 
			   when 15 then '관심없음' end, '') as [recan_status2]
, isnull(convert(varchar(10), b.schedule_date, 121),'') as [recan_schedule_dt2]
, isnull(b.contents,'') as [recan_memo2]
, isnull(convert(varchar(30),b.create_dt, 121), '') as [recan_create_dt2]
--,a.*, b.state, b.schedule_date, b.contents --a.pic_seq, count(*)
from (select ROW_NUMBER() over (partition by p_seq, pic_seq order by prc_seq desc) as r, * from pjt_recandidate_history where state = 70) a left join 
                               (select ROW_NUMBER() over (partition by p_seq, pic_seq order by prc_seq desc) as r, * from pjt_recandidate_history) b
                               on a.p_seq= b.p_seq
                               and a.pic_seq = b.pic_seq
                               and a.prc_seq < b.prc_seq
                               and b.r = 1
                               LEFT JOIN project p
                               on a.p_seq = p.p_seq
                               left join candidate c
                               on a.c_seq= c.c_Seq
                               left join client cl
                               on p.c_seq= cl.c_seq
                               left join uv_user u
                               on a.create_user = u.uv_seq
where a.r = 1
";
          if (!String.IsNullOrEmpty(search.startDt) && !String.IsNullOrEmpty(search.endDt))
          {
            selectQuery = selectQuery + @" 
and (a.create_dt between @startDt + ' 00:00:00' and @endDt + ' 23:59:59'
     or b.create_dt between @startDt + ' 00:00:00' and @endDt + ' 23:59:59')

";
          }

          if (search.is_no_after_only == 1)
          {
            selectQuery = selectQuery + @" AND B.pic_seq is null ";
          }

          if (search.is_progress_only == 1)
          {
            selectQuery = selectQuery + @" AND P.pjt_status = 1 ";
          }

          if (search.uv_seq > 0)
          {
            selectQuery = selectQuery + @" AND A.create_seq = @uv_seq ";
          }

          selectQuery = selectQuery + @" order by b.schedule_date, a.schedule_date  ";
          /*
           and U.uv_seq not in (
          16, 14, 22
          , 138, 328, 354
          )
          and u.ud_seq not in (69, 11, 4, 70)
          */
          DynamicParameters param = new DynamicParameters();
          param.Add("@startDt", search.startDt, DbType.String);
          param.Add("@endDt", search.endDt, DbType.String);
          param.Add("@uv_seq", search.uv_seq, DbType.Int32);


          var ret = await con.QueryAsync<NegoProjectStatistic>(selectQuery, param, commandTimeout: 60);

          list = ret.ToList();

          con.Close();

          return list;
        }
      }
      catch (Exception e)
      {
        throw e;
      }
    }

    public async Task<List<MyInvoiceStatistic>> SelectMyInvoice(MyInvoiceSearchModel search)
    {
      try
      {
        List<MyInvoiceStatistic> list = new List<MyInvoiceStatistic>();

        using (IDbConnection con = new SqlConnection(base.BaseConnectionString))
        {
          con.Open();
          string selectQuery = String.Empty;

          selectQuery = @" 
SELECT *
FROM (
SELECT 1 as ord
      ,0 as pic_seq
      ,INV.pii_seq 
      ,INV.p_seq
      ,CASE INV.invoice_type WHEN 0 THEN '채용(성공)' WHEN 1 THEN '채용(선수금)' WHEN 2 THEN '채용(잔금)' WHEN 3 THEN '컨설팅' WHEN 4 THEN '환불' WHEN 5 THEN '취소' 
            ELSE '' END as invoice_type_name
      ,CASE WHEN INV.send_dt is not null THEN '발송완료'
            WHEN INV.is_file = 1 THEN '발행'
            WHEN INV.confirm_dt is not null THEN '승인'
            ELSE '승인대기' END as invoice_status
      ,CASE WHEN INV.send_dt is not null THEN 'SENT'
            WHEN INV.is_file = 1 THEN 'PUBL'
            WHEN INV.confirm_dt is not null THEN 'APPR'
            ELSE 'REQ' END as invoice_status_cd
      ,CONVERT(VARCHAR(10), INV.billing_dt, 121) as billing_dt_str
      ,CONVERT(VARCHAR(10), INV.expire_guarantee, 121) as expire_guarantee_str
      ,INV.client_name
      ,CASE WHEN P.pjt_type = 1 THEN '채용'
            WHEN P.pjt_type = 2 THEN '평판조회'
            WHEN P.pjt_type = 3 THEN '재취업'
            WHEN P.pjt_type = 4 THEN '채용대행'
            WHEN P.pjt_type = 5 THEN '마켓매핑'
            WHEN P.pjt_type = 6 THEN '사외이사'
            ELSE '기타' END as pjt_type_str
      ,P.title
      ,PD.director_name
      ,PM.manager_name
      ,PIS.share_name
      ,INV.billing_money
      ,INV.bill_currency_cd
      ,CASE INV.vat_type WHEN 0 THEN 'VAT포함' WHEN 1 THEN 'VAT별도'  WHEN 2 THEN '영세율' WHEN 3 THEN '면세' ELSE '' END as vat_type_str
      ,CASE WHEN INV.billing_type = 1 OR INV.fee_rate IS NULL OR INV.fee_rate = 0 OR INV.fee_rate = 100  THEN '정액'
            ELSE CONVERT(VARCHAR(10), INV.fee_rate) + '%' 
       END as fee_rate_str
      ,ISNULL(INV.candidate_name, '') as candidate_name
      ,CONVERT(VARCHAR(10), INV.join_dt, 121) as join_dt_str
      ,INV.ann_income
      ,INV.income_currency_cd
      ,P.c_seq as client_seq
      ,INV.c_seq AS candidate_seq
      ,U.name as request_user_name
      ,P.position_str
      ,P.pjt_type
      ,INV.invoice_no
      ,INV.invoice_type
      ,(SELECT count(*) FROM pjt_invoice_info WHERE pre_pii = INV.pii_seq AND invoice_type = 4 AND is_deleted = 0) as refund_cnt
      ,(SELECT count(*) FROM pjt_invoice_info WHERE pre_pii = INV.pii_seq AND invoice_type = 5 AND is_deleted = 0) as cancel_cnt
from pjt_invoice_info AS INV LEFT JOIN project AS P
                            ON INV.p_seq = P.p_seq
                            LEFT JOIN client AS CL
                            ON P.c_seq = CL.c_seq
                            LEFT JOIN candidate AS C
                            ON INV.c_seq = C.c_seq 
                            LEFT JOIN (SELECT DISTINCT
                                PD.p_seq
                                ,STUFF((SELECT DISTINCT
                                            ',' + B.name
                                        FROM pjt_director AS A INNER JOIN uv_user AS B
                                                                        ON A.uv_seq = B.uv_seq
                                        WHERE A.p_seq = PD.p_seq
                                        FOR XML PATH('')), 1, 1, '') AS director_name
                            FROM pjt_director AS PD) as PD
                            ON INV.p_seq = PD.p_seq
                            LEFT JOIN (SELECT DISTINCT
                                PM.p_seq
                                ,STUFF((SELECT DISTINCT
                                            ',' + B.name 
                                        FROM pjt_manager  AS A INNER JOIN uv_user AS B
                                                                        ON A.uv_seq = B.uv_seq
									                            LEFT JOIN PJT_DIRECTOR PDD
									                            ON A.p_seq = PDD.p_seq
									                            AND A.uv_seq = PDD.uv_seq
                                        WHERE A.p_seq = PM.p_seq
			                            AND PDD.uv_seq is null
									  
                                        FOR XML PATH('')), 1, 1, '') AS manager_name
                            FROM pjt_manager AS PM) as PM
                            ON INV.p_seq = PM.p_seq
                            LEFT JOIN (SELECT DISTINCT
                                PIS.pii_seq
                                ,STUFF((SELECT DISTINCT
                                            ',' + B.name + ' ('+convert(varchar(10), sales_rate) + ' %)' 
                                        FROM pjt_invoice_sales AS A INNER JOIN uv_user AS B
                                                                        ON A.uv_seq = B.uv_seq
                                        WHERE A.pii_seq = PIS.pii_seq
                                        AND A.sales_rate > 0   
                                        FOR XML PATH('')), 1, 1, '') AS share_name
                            FROM pjt_invoice_sales AS PIS) as PIS
                            ON INV.pii_seq = PIS.pii_seq
                            LEFT JOIN UV_USER U
                            ON INV.create_user = U.uv_seq
WHERE INV.is_deleted <> 1 ";

          if (search.uv_seq > 0)
          {
            selectQuery = selectQuery + @" AND (INV.create_user = @uv_seq OR INV.pii_seq in (SELECT pii_seq FROM pjt_invoice_sales WHERE uv_seq = @uv_seq))";
          }
          selectQuery = selectQuery + @"
UNION ALL
SELECT 0 as ord
      ,PR.pic_seq
      ,0 as pii_seq
      ,P.p_seq
      ,'채용' as invoice_type_name
      ,'발행 전' as invoice_status
      , '' as invoice_status_cd
      , CONVERT(VARCHAR(10), PR.schedule_date, 121) as billing_dt_str
      , '' as expire_guarantee_str
      ,C.kor_name AS client_name
      , '채용' as pjt_type_str
      ,P.title
      ,PD.director_name
      ,PM.manager_name
      ,'' as share_name
      ,0 as billing_money
      ,'발행예정' as bill_currency_cd
      ,'발행예정' as vat_type_str
      ,'(예상)' + Convert(varchar(10), P.fee_rate) + '%' as fee_rate_str
      ,PR.kor_name as candidate_name
      ,CONVERT(VARCHAR(10), PR.schedule_date, 121) as join_dt_str
      ,PR.ann_income
      ,P.currency_cd as income_currency_cd
      ,C.c_seq AS client_seq
      ,PR.c_seq as candidate_seq
      ,'' as request_user_name
      ,P.position_str
      ,P.pjt_type
      ,'' as invoice_no
      ,-1 as invoice_type
      ,0 as refund_cnt
      ,0 as cancel_cnt
  FROM PROJECT AS P INNER JOIN(SELECT *
                                  FROM (SELECT ROW_NUMBER() OVER(PARTITION BY PR.p_seq, PR.c_seq ORDER BY RH.create_dt DESC) AS row
                                              , PR.p_seq
                                              ,PR.c_seq
                                              ,C.kor_name
                                              ,ISNULL(RH.is_no_invoice, 0) as is_no_invoice
                                              ,RH.pic_seq
                                              ,RH.state
                                              ,RH.schedule_date
                                              ,RH.ann_income
                                              ,(SELECT count(*) FROM pjt_invoice_info WHERE p_seq = PR.p_seq AND c_seq = PR.c_seq AND is_deleted = 0)  AS invoice_cnt
                                          FROM pjt_recandidate AS PR INNER JOIN pjt_recandidate_history AS RH
                                                                             ON PR.p_seq = RH.p_seq
                                                                            AND PR.pic_seq = RH.pic_seq
                                                                     INNER JOIN candidate AS C
                                                                             ON PR.c_seq = C.c_seq
                                         ) AS A
                                 WHERE A.row = 1
                                   AND A.state = 80
                                   AND A.invoice_cnt = 0
                                   AND A.is_no_invoice = 0) AS PR
                            ON P.p_seq = PR.p_seq
                            AND PR.schedule_date > '2024-01-01'
                    INNER JOIN client AS C
                            ON P.c_seq = C.c_seq
                    LEFT JOIN(SELECT DISTINCT
                                PD.p_seq
                                , STUFF((SELECT DISTINCT
                                            ',' + B.name
                                        FROM pjt_director AS A INNER JOIN uv_user AS B
                                                                        ON A.uv_seq = B.uv_seq
                                        WHERE A.p_seq = PD.p_seq
                                        FOR XML PATH('')), 1, 1, '') AS director_name
                            FROM pjt_director AS PD) as PD
                            ON P.p_seq = PD.p_seq
                            LEFT JOIN(SELECT DISTINCT
                                PM.p_seq
                                , STUFF((SELECT DISTINCT
                                            ',' + B.name
                                        FROM pjt_manager  AS A INNER JOIN uv_user AS B
                                                                        ON A.uv_seq = B.uv_seq

                                              LEFT JOIN PJT_DIRECTOR PDD

                                              ON A.p_seq = PDD.p_seq

                                              AND A.uv_seq = PDD.uv_seq
                                        WHERE A.p_seq = PM.p_seq

                                  AND PDD.uv_seq is null


                                        FOR XML PATH('')), 1, 1, '') AS manager_name
                            FROM pjt_manager AS PM) as PM
                            ON P.p_seq = PM.p_seq
 WHERE P.pjt_status in (1, 4, 5)";
          if (search.uv_seq > 0)
          {
            selectQuery = selectQuery + @" AND(P.p_seq IN (SELECT p_seq FROM pjt_director WHERE uv_seq = @uv_seq) OR P.p_seq IN (SELECT p_seq FROM pjt_manager WHERE uv_seq = @uv_seq))";
          }
          selectQuery = selectQuery + @" ) A ";
          selectQuery = selectQuery + @"order by ord asc, billing_dt_str desc ";
          /*
           and U.uv_seq not in (
          16, 14, 22
          , 138, 328, 354
          )
          and u.ud_seq not in (69, 11, 4, 70)
          */
          DynamicParameters param = new DynamicParameters();
          param.Add("@startDt", search.startDt, DbType.String);
          param.Add("@endDt", search.endDt, DbType.String);
          param.Add("@uv_seq", search.uv_seq, DbType.Int32);


          var ret = await con.QueryAsync<MyInvoiceStatistic>(selectQuery, param, commandTimeout: 60);

          list = ret.ToList();

          con.Close();

          return list;
        }
      }
      catch (Exception e)
      {
        throw e;
      }
    }
  }

}
