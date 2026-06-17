using Dapper;
using Univision.Core.Models.DTO;
using Univision.Core.Repositories;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Univision.Core.Models.DTO.Response.Dashboard;
using System.Data;
using System.Threading.Tasks;
using System.Data.SqlClient;

namespace Univision.Core.Repositories
{
    public class DashboardRepository : BaseRepository
    {
        /// <summary>
        /// 이번달 추천후보자 건수
        /// </summary>
        /// <returns></returns>
        public async Task<int> CountRegRecandidateMonth(int uv_seq, string startDt, string endDt)
        {
            try
            {
                using (IDbConnection con = new SqlConnection(base.BaseConnectionString))
                {
                    con.Open();

                    string selectQuery = @" SELECT ISNULL(COUNT(distinct X1.pic_seq), 0) AS rCandidateCnt
            FROM pjt_recandidate X1 INNER JOIN pjt_recandidate_history Y1
                                    ON X1.pic_seq = Y1.pic_seq
                                    INNER JOIN project PP
                                    ON X1.p_seq = PP.p_seq
            WHERE X1.create_user = @uv_seq
            AND state = 30
            AND Y1.schedule_date BETWEEN CONVERT(DATETIME, @startDt + ' 00:00:00') AND CONVERT(DATETIME, @endDt + ' 23:59:59') ";
                    
                    DynamicParameters param = new DynamicParameters();
                    param.Add("@uv_seq", uv_seq, DbType.Int32);
                    param.Add("@startDt", startDt, DbType.String);
                    param.Add("@endDt", endDt, DbType.String);

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

        public async Task<int> CountInterviewRecandidateMonth(int uv_seq, string startDt, string endDt)
        {
            try
            {
                using (IDbConnection con = new SqlConnection(base.BaseConnectionString))
                {
                    con.Open();

                    string selectQuery = @" SELECT ISNULL(COUNT(distinct X1.pic_seq), 0) AS rCandidateCnt
            FROM pjt_recandidate X1 INNER JOIN pjt_recandidate_history Y1
                                    ON X1.pic_seq = Y1.pic_seq
                                    INNER JOIN project PP
                                    ON X1.p_seq = PP.p_seq
            WHERE X1.create_user = @uv_seq
            AND state = 50
            AND Y1.schedule_date BETWEEN CONVERT(DATETIME, @startDt + ' 00:00:00') AND CONVERT(DATETIME, @endDt + ' 23:59:59') ";

                    DynamicParameters param = new DynamicParameters();
                    param.Add("@uv_seq", uv_seq, DbType.Int32);
                    param.Add("@startDt", startDt, DbType.String);
                    param.Add("@endDt", endDt, DbType.String);

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

        public async Task<int> CountMemoMonth(int uv_seq, string startDt, string endDt)
        {
            try
            {
                using (IDbConnection con = new SqlConnection(base.BaseConnectionString))
                {
                    con.Open();

                    string selectQuery = @" SELECT ISNULL(COUNT(distinct X1.c_seq), 0) AS memoCandidateCnt
FROM can_activity X1 INNER JOIN candidate Y1
                     ON X1.c_seq = Y1.c_seq
WHERE X1.create_seq = @uv_seq 
AND isnull(X1.cl_seq, 0) <> 1
AND X1.create_dt BETWEEN CONVERT(DATETIME, @startDt + ' 00:00:00') AND CONVERT(DATETIME, @endDt + ' 23:59:59') ";

                    DynamicParameters param = new DynamicParameters();
                    param.Add("@uv_seq", uv_seq, DbType.Int32);
                    param.Add("@startDt", startDt, DbType.String);
                    param.Add("@endDt", endDt, DbType.String);

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
        /// 최근 1주 후보자 간편등록 건수
        /// </summary>
        /// <param name="uv_seq"></param>
        /// <param name="startDt"></param>
        /// <param name="endDt"></param>
        /// <returns></returns>
        public async Task<int> CountRegSimpleCandidateWeek(int uv_seq, string startDt, string endDt)
        {
            try
            {
                using (IDbConnection con = new SqlConnection(base.BaseConnectionString))
                {
                    con.Open();

                    string selectQuery = @" SELECT ISNULL(COUNT(0), 0) AS sCandidateCnt
  FROM simple_candidate
 WHERE create_user = @uv_seq
   AND is_del <> 1
   AND create_dt BETWEEN CONVERT(DATETIME, @startDt + ' 00:00:00') AND CONVERT(DATETIME, @endDt + ' 23:59:59') ";

                    DynamicParameters param = new DynamicParameters();
                    param.Add("@uv_seq", uv_seq, DbType.Int32);
                    param.Add("@startDt", startDt, DbType.String);
                    param.Add("@endDt", endDt, DbType.String);

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

    public async Task<string> SelectBoardUpdateDate(int b_seq)
    {
      try
      {
        using (IDbConnection con = new SqlConnection(base.BaseConnectionString))
        {
          con.Open();

          string selectQuery = @" SELECT CONVERT(VARCHAR(10), modify_dt, 121) as updated
  FROM board
 WHERE b_seq = @b_seq ";

          DynamicParameters param = new DynamicParameters();
          param.Add("@b_seq", b_seq, DbType.Int32);

          var ret = await con.QueryAsync<string>(selectQuery, param);

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
    /// 이번달 내 후보자 등록 건수
    /// </summary>
    /// <param name="uv_seq"></param>
    /// <param name="startDt"></param>
    /// <param name="endDt"></param>
    /// <returns></returns>
    public async Task<int> CountRegCandidateMonth(int uv_seq, string startDt, string endDt)
        {
            try
            {
                using (IDbConnection con = new SqlConnection(base.BaseConnectionString))
                {
                    con.Open();

                    string selectQuery = @" SELECT ISNULL(COUNT(0), 0) AS candidateCnt
  FROM candidate
 WHERE (create_seq = @uv_seq
       OR manager_seq = @uv_seq)
   AND create_dt BETWEEN CONVERT(DATETIME, @startDt + ' 00:00:00') AND CONVERT(DATETIME, @endDt + ' 23:59:59') ";

                    DynamicParameters param = new DynamicParameters();
                    param.Add("@uv_seq", uv_seq, DbType.Int32);
                    param.Add("@startDt", startDt, DbType.String);
                    param.Add("@endDt", endDt, DbType.String);

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

    public async Task<int> CountUpdateCandidateMonth(int uv_seq, string startDt, string endDt)
    {
      try
      {
        using (IDbConnection con = new SqlConnection(base.BaseConnectionString))
        {
          con.Open();

          string selectQuery = @"
SELECT ISNULL(COUNT(distinct X.c_seq), 0) AS candidateCnt
FROM can_resume X inner join candidate Y
                  on X.c_seq = Y.c_seq      
WHERE X.create_user = @uv_seq            
AND X.remove_dt is null
AND X.create_dt BETWEEN CONVERT(DATETIME, @startDt + ' 00:00:00') AND CONVERT(DATETIME, @endDt + ' 23:59:59')
AND X.c_seq NOT IN(SELECT c_seq FROM candidate WHERE create_seq = @uv_seq)
AND X.c_seq NOT IN(SELECT c_seq FROM candidate WHERE manager_seq = @uv_seq)
            ";

          DynamicParameters param = new DynamicParameters();
          param.Add("@uv_seq", uv_seq, DbType.Int32);
          param.Add("@startDt", startDt, DbType.String);
          param.Add("@endDt", endDt, DbType.String);

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
    /// 현재 진행중인 프로젝트 건수
    /// </summary>
    /// <param name="uv_seq"></param>
    /// <param name="status"></param>
    /// <returns></returns>
    public async Task<int> CountNowProgressPjt(int uv_seq, int status)
        {
            try
            {
                using (IDbConnection con = new SqlConnection(base.BaseConnectionString))
                {
                    con.Open();

                    string selectQuery = @" SELECT ISNULL(COUNT(0), 0) AS pjtCnt
  FROM project
 WHERE pjt_status = @status
   AND (p_seq in (SELECT p_seq from pjt_manager WHERE uv_seq = @uv_seq) 
    OR p_seq in (SELECT p_seq from pjt_director WHERE uv_seq = @uv_seq))";


                    DynamicParameters param = new DynamicParameters();
                    param.Add("@uv_seq", uv_seq, DbType.Int32);
                    param.Add("@status", status, DbType.Int32);

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
        /// 이번달 출근일
        /// </summary>
        /// <param name="uv_seq"></param>
        /// <param name="startDt"></param>
        /// <param name="endDt"></param>
        /// <returns></returns>
        public async Task<double> CountAttendMonth(int uv_seq, string startDt, string endDt)
        {
            try
            {
                using (IDbConnection con = new SqlConnection(base.BaseConnectionString))
                {
                    con.Open();

                    string selectQuery = @" SELECT ISNULL(COUNT(0), 0) AS attendCnt
  FROM uv_user_attend
 WHERE uv_seq = @uv_seq
   AND attend_date BETWEEN CONVERT(DATETIME, @startDt + ' 00:00:00') AND CONVERT(DATETIME, @endDt + ' 23:59:59') ";

                    DynamicParameters param = new DynamicParameters();
                    param.Add("@uv_seq", uv_seq, DbType.Int32);
                    param.Add("@startDt", startDt, DbType.String);
                    param.Add("@endDt", endDt, DbType.String);

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

        public async Task<double> CountUsedVacation(int uv_seq, string year)
        {
            try
            {
                using (IDbConnection con = new SqlConnection(base.BaseConnectionString))
                {
                    con.Open();

                    string selectQuery = @" 
SELECT ISNULL((SELECT SUM(vacation_number) FROM uv_vacation_history WHERE request_user = A.uv_seq AND YEAR(start_date) = A.va_year AND is_confirm = 1 AND v_type IN (1, 2)), 0) As use_total
FROM uv_vacation As A
WHERE A.uv_seq  = @uv_seq
AND   A.va_year = @year "; 

                    DynamicParameters param = new DynamicParameters();
                    param.Add("@uv_seq", uv_seq, DbType.Int32);
                    param.Add("@year", year, DbType.String);
                    

                    var ret = await con.QueryAsync<double>(selectQuery, param);

                    con.Close();

                    return ret.FirstOrDefault();
                }
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public async Task<double> CountTotalVacation(int uv_seq, string year)
        {
            try
            {
                using (IDbConnection con = new SqlConnection(base.BaseConnectionString))
                {
                    con.Open();

                    string selectQuery = @" 
SELECT va_num
FROM uv_vacation As A
WHERE A.uv_seq  = @uv_seq
AND   A.va_year = @year ";

                    DynamicParameters param = new DynamicParameters();
                    param.Add("@uv_seq", uv_seq, DbType.Int32);
                    param.Add("@year", year, DbType.String);


                    var ret = await con.QueryAsync<double>(selectQuery, param);

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
        /// 미팅룸 현황 조회.
        /// </summary>
        /// <returns></returns>
        public async Task<List<meeting_room>> SelectMeetingRoomStatusListAsync()
        {
            try
            {
                List<meeting_room> list = new List<meeting_room>();

                using (IDbConnection con = new SqlConnection(base.BaseConnectionString))
                {
                    con.Open();

                    string selectQuery = @" SELECT M.title
       ,M.floor
       ,CASE WHEN ISNULL(MS.cnt, 0) > 0 
             THEN '사용불가 [Unavailable]' 
             ELSE '사용가능 [Available]' 
         END AS status
 FROM MEETING_ROOM AS M LEFT OUTER JOIN (SELECT resourceId, COUNT(*) as cnt
                                           FROM MEETING_ROOM_SCHEDULE 
                                          WHERE (dd_start BETWEEN GETDATE() AND  DATEADD(N, 15, GETDATE())  
                                                 OR  dd_end BETWEEN GETDATE() AND  DATEADD(N, 15, GETDATE()) 
                                                 OR  GETDATE() BETWEEN dd_start AND dd_end  
                                                 OR  DATEADD(N, 15, GETDATE()) BETWEEN dd_start AND  dd_end ) 
                                            AND is_del = 0
                                          GROUP BY resourceId) MS
                                     ON M.id = MS.resourceId 
WHERE M.is_close = 0 
ORDER BY M.id ";
                    var ret = await con.QueryAsync<meeting_room>(selectQuery);

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


        public async Task<List<pjt_invoice_info>> SelectBilledInvoiceListAsync(int uv_seq)
        {
            try
            {
                List<pjt_invoice_info> list = new List<pjt_invoice_info>();

                using (IDbConnection con = new SqlConnection(base.BaseConnectionString))
                {
                    con.Open();

                    string selectQuery = @" SELECT TOP 5
      B.c_seq AS client_seq
	 ,C.kor_name AS client_name
	 ,B.title AS pjt_title
     ,D.c_seq AS candidate_seq
	 ,ISNULL(D.kor_name, B.title) AS candidate_name
	 ,A.billing_money AS billing_money
	 ,A.billing_dt AS billing_dt
	 ,B.pjt_type
	 ,B.p_seq
   ,A.expire_guarantee
FROM pjt_invoice_info A INNER JOIN project B
								ON A.p_seq = B.p_seq
						INNER JOIN client C
								ON B.c_seq = C.c_seq
						LEFT  JOIN candidate D
								ON A.c_seq = D.c_seq
						INNER JOIN pjt_invoice_sales E
								ON A.pii_seq = E.pii_seq
WHERE E.uv_seq = @uv_seq 
AND A.is_deleted <> 1 
AND B.pjt_status in (1, 4, 5)
ORDER BY A.pii_seq DESC ";

                    DynamicParameters param = new DynamicParameters();
                    param.Add("@uv_seq", uv_seq, DbType.Int32);

                    var ret = await con.QueryAsync<pjt_invoice_info>(selectQuery, param);

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
        /// 인보이스 미발행 리스트 조회.
        /// </summary>
        /// <param name="uv_seq"></param>
        /// <param name="state"></param>
        /// <returns></returns>
        public async Task<List<UnBilledInvoiceModel>> SelectUnBilledInvoiceListAsync(int uv_seq, int state)
        {
            try
            {
                List<UnBilledInvoiceModel> list = new List<UnBilledInvoiceModel>();

                using (IDbConnection con = new SqlConnection(base.BaseConnectionString))
                {
                    con.Open();

                    string selectQuery = @" SELECT TOP 5
       C.c_seq AS client_seq
      ,C.kor_name AS client_name
      ,P.p_seq
      ,P.title
      ,PR.candidate_seq
      ,PR.kor_name AS candidate_name
      ,PR.schedule_date
  FROM project AS P INNER JOIN (SELECT A.p_seq
                                      ,A.c_seq AS candidate_seq
                                      ,A.kor_name
                                      ,A.state
                                      ,A.schedule_date
                                      ,A.invoice_cnt
                                  FROM (SELECT ROW_NUMBER() OVER(PARTITION BY PR.p_seq, PR.c_seq ORDER BY MAX(RH.create_dt) DESC, MAX(RH.state) DESC) AS row
                                              ,PR.p_seq
                                              ,PR.c_seq
                                              ,C.kor_name
                                              ,ISNULL(RH.is_no_invoice, 0) as is_no_invoice
                                              ,RH.state
                                              ,RH.schedule_date
                                              ,COUNT(PI.pii_seq) AS invoice_cnt
                                          FROM pjt_recandidate AS PR INNER JOIN pjt_recandidate_history AS RH
                                                                             ON PR.p_seq = RH.p_seq
                                                                            AND PR.pic_seq = RH.pic_seq
                                                                     INNER JOIN candidate AS C
                                                                             ON PR.c_seq = C.c_seq
                                                                LEFT OUTER JOIN pjt_invoice_info AS PI
                                                                             ON PR.p_seq = PI.p_seq
                                                                            AND PR.c_seq = PI.c_seq
                                                                            AND PI.is_deleted <> 1
                                         
                                         GROUP BY PR.p_seq, PR.pic_seq, RH.prc_seq, PR.c_seq, C.kor_name, RH.is_no_invoice, RH.schedule_date, RH.state) AS A
                                 WHERE A.row = 1 
                                   AND A.state = @state
                                   AND A.invoice_cnt = 0
                                   AND A.is_no_invoice = 0) AS PR
                            ON P.p_seq = PR.p_seq
                    INNER JOIN client AS C
                            ON P.c_seq = C.c_seq
 WHERE P.pjt_status in (1, 4, 5)
   AND(P.p_seq IN (SELECT p_seq FROM pjt_director WHERE uv_seq = @uv_seq) OR P.p_seq IN (SELECT p_seq FROM pjt_manager WHERE uv_seq = @uv_seq))
 ORDER BY schedule_date DESC ";

                    DynamicParameters param = new DynamicParameters();
                    param.Add("@uv_seq", uv_seq, DbType.Int32);
                    param.Add("@state", state, DbType.Int32);

                    var ret = await con.QueryAsync<UnBilledInvoiceModel>(selectQuery, param);
                    
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


        public async Task<List<KPIModel>> SelectKpiDataListAsync(int uv_seq)
        {
            try
            {
                List<KPIModel> list = new List<KPIModel>();

                using (IDbConnection con = new SqlConnection(base.BaseConnectionString))
                {
                    con.Open();

                    string selectQuery = @" select
	'MY' user_name,
	isnull((select count(0) from candidate where (create_seq = @uv_seq OR manager_seq = @uv_seq) and (convert(int, datepart(year, create_dt), 120) = convert(int, datepart(year, getdate()), 120)) 
	and (DATEPART(week, create_dt) between (DATEPART(week, getdate()) - 4) and (DATEPART(week, getdate()) - 1))), 0) mem_cnt0,
	isnull(sum(case when  state>=20 and (DATEPART(week, pr.create_dt) between (DATEPART(week, getdate()) - 4) and (DATEPART(week, getdate()) - 1)) then 1 end), 0) as inter0,
	isnull(sum(case when  state>=30 and (DATEPART(week, pr.create_dt) between (DATEPART(week, getdate()) - 4) and (DATEPART(week, getdate()) - 1)) then 1 end), 0) as push0,
	isnull(sum(case when  state>=50 and (DATEPART(week, pr.create_dt) between (DATEPART(week, getdate()) - 4) and (DATEPART(week, getdate()) - 1)) then 1 end), 0) as interview0,
	isnull(sum(case when  state>=80 and (DATEPART(week, pr.create_dt) between (DATEPART(week, getdate()) - 4) and (DATEPART(week, getdate()) - 1)) then 1 end), 0) as hire0,
	isnull((select count(0) from candidate where (create_seq = @uv_seq OR manager_seq = @uv_seq) and  (convert(int, datepart(year, create_dt), 120) = convert(int, datepart(year, getdate()), 120)) and (DATEPART(week, create_dt) = (DATEPART(week, getdate()) - 1))), 0) as mem_cnt1,
	isnull(sum(case when  state>=20 and DATEPART(week, pr.create_dt) = (DATEPART(week, getdate()) - 1) then 1 end), 0) as inter1,
	isnull(sum(case when  state>=30 and DATEPART(week, pr.create_dt) = (DATEPART(week, getdate()) - 1) then 1 end), 0) as push1,
	isnull(sum(case when  state>=50 and DATEPART(week, pr.create_dt) = (DATEPART(week, getdate()) - 1) then 1 end), 0) as interview1,
	isnull(sum(case when  state>=80 and DATEPART(week, pr.create_dt) = (DATEPART(week, getdate()) - 1) then 1 end), 0) as hire1,
	isnull((select count(0) from candidate where (create_seq = @uv_seq OR manager_seq = @uv_seq) and  (convert(int, datepart(year, create_dt), 120) = convert(int, datepart(year, getdate()), 120)) and (DATEPART(week, create_dt) = (DATEPART(week, getdate()) - 2))), 0) as mem_cnt2,
	isnull(sum(case when  state>=20 and DATEPART(week, pr.create_dt) = (DATEPART(week, getdate()) - 2) then 1 end), 0) as inter2,
	isnull(sum(case when  state>=30 and DATEPART(week, pr.create_dt) = (DATEPART(week, getdate()) - 2) then 1 end), 0) as push2,
	isnull(sum(case when  state>=50 and DATEPART(week, pr.create_dt) = (DATEPART(week, getdate()) - 2) then 1 end), 0) as interview2,
	isnull(sum(case when  state>=80 and DATEPART(week, pr.create_dt) = (DATEPART(week, getdate()) - 2) then 1 end), 0) as hire2,
	isnull((select count(0) from candidate where (create_seq = @uv_seq OR manager_seq = @uv_seq) and  (convert(int, datepart(year, create_dt), 120) = convert(int, datepart(year, getdate()), 120)) and (DATEPART(week, create_dt) = (DATEPART(week, getdate()) - 3))), 0) as mem_cnt3,
	isnull(sum(case when  state>=20 and DATEPART(week, pr.create_dt) = (DATEPART(week, getdate()) - 3) then 1 end), 0) as inter3,
	isnull(sum(case when  state>=30 and DATEPART(week, pr.create_dt) = (DATEPART(week, getdate()) - 3) then 1 end), 0) as push3,
	isnull(sum(case when  state>=50 and DATEPART(week, pr.create_dt) = (DATEPART(week, getdate()) - 3) then 1 end), 0) as interview3,
	isnull(sum(case when  state>=80 and DATEPART(week, pr.create_dt) = (DATEPART(week, getdate()) - 3) then 1 end), 0) as hire3,
	isnull((select count(0) from candidate where (create_seq = @uv_seq OR manager_seq = @uv_seq) and  (convert(int, datepart(year, create_dt), 120) = convert(int, datepart(year, getdate()), 120)) and (DATEPART(week, create_dt) = (DATEPART(week, getdate()) - 4))), 0) as mem_cnt4,
	isnull(sum(case when  state>=20 and DATEPART(week, pr.create_dt) = (DATEPART(week, getdate()) - 4) then 1 end), 0) as inter4,
	isnull(sum(case when  state>=30 and DATEPART(week, pr.create_dt) = (DATEPART(week, getdate()) - 4) then 1 end), 0) as push4,
	isnull(sum(case when  state>=50 and DATEPART(week, pr.create_dt) = (DATEPART(week, getdate()) - 4) then 1 end), 0) as interview4,
	isnull(sum(case when  state>=80 and DATEPART(week, pr.create_dt) = (DATEPART(week, getdate()) - 4) then 1 end), 0) as hire4,
    1 as div_user
from pjt_recandidate_history pr inner join uv_user uv
									on pr.create_user = uv.uv_seq
where pr.create_user = @uv_seq 
  and state in (20,30,50,80) 
  and (convert(int, datepart(year, pr.create_dt), 120) = convert(int, datepart(year, getdate()), 120))
  and (DATEPART(week, pr.create_dt) between (DATEPART(week, getdate()) - 4) and (DATEPART(week, getdate()) - 1))
group by uv.name
union all
select
	'평균' user_name,
	isnull((select count(0) from candidate where (convert(int, datepart(year, create_dt), 120) = convert(int, datepart(year, getdate()), 120)) 
	and (DATEPART(week, create_dt) between (DATEPART(week, getdate()) - 4) and (DATEPART(week, getdate()) - 1))), 0) mem_cnt0,
	isnull(sum(case when  state>=20 and (DATEPART(week, pr.create_dt) between (DATEPART(week, getdate()) - 4) and (DATEPART(week, getdate()) - 1)) then 1 end), 0) as inter0,
	isnull(sum(case when  state>=30 and (DATEPART(week, pr.create_dt) between (DATEPART(week, getdate()) - 4) and (DATEPART(week, getdate()) - 1)) then 1 end), 0) as push0,
	isnull(sum(case when  state>=50 and (DATEPART(week, pr.create_dt) between (DATEPART(week, getdate()) - 4) and (DATEPART(week, getdate()) - 1)) then 1 end), 0) as interview0,
	isnull(sum(case when  state>=80 and (DATEPART(week, pr.create_dt) between (DATEPART(week, getdate()) - 4) and (DATEPART(week, getdate()) - 1)) then 1 end), 0) as hire0,
	isnull((select count(0) from candidate where (convert(int, datepart(year, create_dt), 120) = convert(int, datepart(year, getdate()), 120)) and (DATEPART(week, create_dt) = (DATEPART(week, getdate()) - 1))), 0) as mem_cnt1,
	isnull(sum(case when  state>=20 and DATEPART(week, pr.create_dt) = (DATEPART(week, getdate()) - 1) then 1 end), 0) as inter1,
	isnull(sum(case when  state>=30 and DATEPART(week, pr.create_dt) = (DATEPART(week, getdate()) - 1) then 1 end), 0) as push1,
	isnull(sum(case when  state>=50 and DATEPART(week, pr.create_dt) = (DATEPART(week, getdate()) - 1) then 1 end), 0) as interview1,
	isnull(sum(case when  state>=80 and DATEPART(week, pr.create_dt) = (DATEPART(week, getdate()) - 1) then 1 end), 0) as hire1,
	isnull((select count(0) from candidate where (convert(int, datepart(year, create_dt), 120) = convert(int, datepart(year, getdate()), 120)) and (DATEPART(week, create_dt) = (DATEPART(week, getdate()) - 2))), 0) as mem_cnt2,
	isnull(sum(case when  state>=20 and DATEPART(week, pr.create_dt) = (DATEPART(week, getdate()) - 2) then 1 end), 0) as inter2,
	isnull(sum(case when  state>=30 and DATEPART(week, pr.create_dt) = (DATEPART(week, getdate()) - 2) then 1 end), 0) as push2,
	isnull(sum(case when  state>=50 and DATEPART(week, pr.create_dt) = (DATEPART(week, getdate()) - 2) then 1 end), 0) as interview2,
	isnull(sum(case when  state>=80 and DATEPART(week, pr.create_dt) = (DATEPART(week, getdate()) - 2) then 1 end), 0) as hire2,
	isnull((select count(0) from candidate where (convert(int, datepart(year, create_dt), 120) = convert(int, datepart(year, getdate()), 120)) and (DATEPART(week, create_dt) = (DATEPART(week, getdate()) - 3))), 0) as mem_cnt3,
	isnull(sum(case when  state>=20 and DATEPART(week, pr.create_dt) = (DATEPART(week, getdate()) - 3) then 1 end), 0) as inter3,
	isnull(sum(case when  state>=30 and DATEPART(week, pr.create_dt) = (DATEPART(week, getdate()) - 3) then 1 end), 0) as push3,
	isnull(sum(case when  state>=50 and DATEPART(week, pr.create_dt) = (DATEPART(week, getdate()) - 3) then 1 end), 0) as interview3,
	isnull(sum(case when  state>=80 and DATEPART(week, pr.create_dt) = (DATEPART(week, getdate()) - 3) then 1 end), 0) as hire3,
	isnull((select count(0) from candidate where (convert(int, datepart(year, create_dt), 120) = convert(int, datepart(year, getdate()), 120)) and (DATEPART(week, create_dt) = (DATEPART(week, getdate()) - 4))), 0) as mem_cnt4,
	isnull(sum(case when  state>=20 and DATEPART(week, pr.create_dt) = (DATEPART(week, getdate()) - 4) then 1 end), 0) as inter4,
	isnull(sum(case when  state>=30 and DATEPART(week, pr.create_dt) = (DATEPART(week, getdate()) - 4) then 1 end), 0) as push4,
	isnull(sum(case when  state>=50 and DATEPART(week, pr.create_dt) = (DATEPART(week, getdate()) - 4) then 1 end), 0) as interview4,
	isnull(sum(case when  state>=80 and DATEPART(week, pr.create_dt) = (DATEPART(week, getdate()) - 4) then 1 end), 0) as hire4,
    (SELECT COUNT(*) FROM uv_user WHERE retire_date IS NULL AND ud_seq NOT IN (4, 17)) as div_user
from pjt_recandidate_history pr 
where state in (20,30,50,80) 
  and (convert(int, datepart(year, pr.create_dt), 120) = convert(int, datepart(year, getdate()), 120))
  and (DATEPART(week, pr.create_dt) between (DATEPART(week, getdate()) - 4) and (DATEPART(week, getdate()) - 1)) ";

                    DynamicParameters param = new DynamicParameters();
                    param.Add("@uv_seq", uv_seq, DbType.Int32);

                    var ret = await con.QueryAsync<KPIModel>(selectQuery, param);

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