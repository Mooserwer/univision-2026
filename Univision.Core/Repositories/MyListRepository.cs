using Dapper;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Univision.Core.Models.DTO;
using Univision.Core.Models.DTO.Request;
using Univision.Core.Models.DTO.Request.MyList;
using Univision.Core.Models.DTO.Response;
using Univision.Security;

namespace Univision.Core.Repositories
{
  public class MyListRepository : BaseRepository
  {

    

    public async Task<List<ResponseCalendar>> FindUserAttendListAsync(int uv_seq, string start_date, string end_date)
    {
      try
      {
        List<ResponseCalendar> list = new List<ResponseCalendar>();

        using (IDbConnection con = new SqlConnection(base.BaseConnectionString))
        {
          con.Open();

          string selectQuery = @" SELECT 
      A.state AS title
      ,CONVERT(VARCHAR, MIN(A.attend_date), 20) AS start
      ,backgroundColor
      ,backgroundColor AS borderColor
      ,'true' AS allDay
  FROM (SELECT * FROM 
  (SELECT uv_seq
      ,attend_date
      ,CASE WHEN DATEPART(HH, attend_date) > 12 THEN ''
            ELSE ''
        END AS state
      ,CASE WHEN DATEPART(HH, attend_date) <= 12 THEN '#3e8ef7'
            WHEN DATEPART(HH, attend_date) > 12 THEN '#28c0de'
            ELSE '#3e8ef7'
        END AS backgroundColor
  FROM uv_user_attend 
  UNION ALL
  SELECT Y.request_user
        ,X.v_date
        ,CASE X.v_type WHEN 0 THEN '(전일)' WHEN 1 THEN '(오전)' WHEN 2 THEN '(오후)' END AS state
        ,CASE Y.v_type WHEN 2 THEN '#dc3545' ELSE 
            CASE Y.is_confirm WHEN 1 THEN '#11c26d' ELSE '#999' END       
         END  AS backgroundColor
  FROM UV_VACATION_HISTORY_DTL X LEFT JOIN UV_VACATION_HISTORY Y
                                 ON X.v_seq = Y.v_seq ) O
 WHERE uv_seq = @uv_seq
   AND attend_date BETWEEN CONVERT(DATETIME, @start_date + ' 00:00:00') AND CONVERT(DATETIME, @end_date + ' 23:59:59')) AS A
 GROUP BY uv_seq, state, backgroundColor, CONVERT(VARCHAR(10), attend_date, 121) ";

          DynamicParameters param = new DynamicParameters();
          param.Add("@uv_seq", uv_seq, DbType.Int32);
          param.Add("@start_date", start_date, DbType.String);
          param.Add("@end_date", end_date, DbType.String);

          var ret = await con.QueryAsync<ResponseCalendar>(selectQuery, param);

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
    /// SMS 발송내역 목록
    /// </summary>
    /// <param name="search"></param>
    /// <param name="uv_seq"></param>
    /// <param name="skip"></param>
    /// <param name="count"></param>
    /// <param name="totalCount"></param>
    /// <returns></returns>
    public List<sms_history> SelectSmsHistoryList(SmsHistorySearchModel search, int uv_seq, int skip, int count, out int totalCount)
    {
      try
      {
        List<sms_history> list = new List<sms_history>();

        using (IDbConnection con = new SqlConnection(base.BaseConnectionString))
        {
          con.Open();

          string selectQuery = @" SELECT SH.*
      ,U.user_id
      ,ISNULL(REQ.dv_status, '') AS send_name      
      ,ISNULL(REQ.dv_desc, '') AS send_desc      
      ,ISNULL(SR.report_code, '') AS report_code
      ,ISNULL(RST.dv_status, '') AS report_name
      ,ISNULL(RST.dv_desc, '') AS report_desc
  FROM sms_history AS SH INNER JOIN uv_user AS U
                                 ON SH.create_user = U.uv_seq
                    LEFT OUTER JOIN SMS_STATUS_CD AS REQ
                                 ON SH.response_code = REQ.dv_cd
                                AND REQ.dv_gubun = 'REQ'
                    LEFT OUTER JOIN sms_report AS SR
                                 ON SH.msg_id = SR.msg_id
                    LEFT OUTER JOIN SMS_STATUS_CD AS RST
                                 ON SR.report_code = RST.dv_cd
                                AND RST.dv_gubun = 'RST' 
 WHERE SH.create_user = @uv_seq ";

          if (!string.IsNullOrWhiteSpace(search.searchTxt))
            selectQuery += string.Format(" AND {0} LIKE '%' + @searchTxt + '%' ", search.searchOption);

          selectQuery += @" ORDER BY sms_seq DESC OFFSET @currentPage ROWS FETCH NEXT @pageSize ROWS ONLY ";

          string countQuery = @" SELECT SH.*, U.user_id
  FROM sms_history AS SH INNER JOIN uv_user AS U
                                 ON SH.create_user = U.uv_seq
 WHERE SH.create_user = @uv_seq ";

          if (!string.IsNullOrWhiteSpace(search.searchTxt))
            countQuery += string.Format(" AND {0} LIKE '%' + @searchTxt + '%' ", search.searchOption);


          DynamicParameters param = new DynamicParameters();
          param.Add("@uv_seq", uv_seq, DbType.Int32);
          param.Add("@searchTxt", search.searchTxt, DbType.String);
          param.Add("@currentPage", skip, DbType.Int32);
          param.Add("@pageSize", count, DbType.Int32);

          DynamicParameters param2 = new DynamicParameters();
          param2.Add("@uv_seq", uv_seq, DbType.Int32);
          param2.Add("@searchTxt", search.searchTxt, DbType.String);


          var ret = con.Query<sms_history>(selectQuery, param);
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


    public uv_vacation SelectMyVacationStatus(VacationSearchModel search)
    {
      try
      {
        uv_vacation status = new uv_vacation();

        using (IDbConnection con = new SqlConnection(base.BaseConnectionString))
        {
          con.Open();

          string selectQuery = @" 
  SELECT  A.*
        , (SELECT SUM(vacation_number) FROM uv_vacation_history WHERE request_user = A.uv_seq AND YEAR(start_date) = A.va_year AND is_confirm = 1 AND v_type IN (1, 2)) As use_total
        , (SELECT SUM(vacation_number) FROM uv_vacation_history WHERE request_user = A.uv_seq AND YEAR(start_date) = A.va_year AND is_confirm is null AND isnull(leader_confirm, 0) <> -1 AND v_type IN (1, 2)) As use_total_ready
        , (SELECT SUM(vacation_number) FROM uv_vacation_history WHERE request_user = A.uv_seq AND YEAR(start_date) = A.va_year AND is_confirm = 1 AND v_type = 1) As use_vacation
        , (SELECT SUM(vacation_number) FROM uv_vacation_history WHERE request_user = A.uv_seq AND YEAR(start_date) = A.va_year AND is_confirm is null AND isnull(leader_confirm, 0) <> -1 AND v_type = 1) As use_vacation_ready
        , (SELECT SUM(vacation_number) FROM uv_vacation_history WHERE request_user = A.uv_seq AND YEAR(start_date) = A.va_year AND is_confirm = 1 AND v_type = 2) As use_not_shown
        , (SELECT SUM(vacation_number) FROM uv_vacation_history WHERE request_user = A.uv_seq AND YEAR(start_date) = A.va_year AND is_confirm = 1 AND v_type NOT IN (1, 2, 7)) As use_etc
        , (SELECT SUM(vacation_number) FROM uv_vacation_history WHERE request_user = A.uv_seq AND YEAR(start_date) = A.va_year AND is_confirm = 1 AND v_type = 7) As use_work
        , (SELECT SUM(vacation_number) FROM uv_vacation_history WHERE request_user = A.uv_seq AND YEAR(start_date) = YEAR(GETDATE()) AND isnull(is_confirm, 0) <> -1 AND isnull(leader_confirm, 0) <> -1 AND v_type IN (1, 2)) As cur_use_all
        , (SELECT va_num FROM uv_vacation WHERE uv_seq = A.uv_seq AND va_year = YEAR(GETDATE())) As cur_va_num
  FROM uv_vacation As A
  WHERE A.uv_seq = @uv_seq 
  AND   A.va_year = @year ";

          DynamicParameters param = new DynamicParameters();
          param.Add("@year", search.searchYear, DbType.Int32);
          param.Add("@uv_seq", search.uv_seq, DbType.Int32);


          var ret = con.Query<uv_vacation>(selectQuery, param);


          status = ret.FirstOrDefault<uv_vacation>();

          con.Close();

          return status;
        }
      }
      catch (Exception e)
      {
        throw e;
      }
    }

    public List<uv_vacation_history> SelectMyVacationList(VacationSearchModel search)
    {
      try
      {
        List<uv_vacation_history> list = new List<uv_vacation_history>();

        using (IDbConnection con = new SqlConnection(base.BaseConnectionString))
        {
          con.Open();

          string selectQuery = @" 
  SELECT  A.*
        , CASE v_type WHEN 1 THEN '연차' WHEN 2 THEN '출근미등록' WHEN 3 THEN '유급병가' WHEN 4 THEN '경조사' WHEN 5 THEN '예비군' WHEN 6 THEN '기타' WHEN 7 THEN '외근' 
                      WHEN 8 THEN '출근미등록 증빙완료' WHEN 10 THEN '재택근무' WHEN 11 THEN 'SHIFT(집)' WHEN 12 THEN '무급휴가' WHEN 13 THEN '장기근속휴가' ELSE '' END as type_name
        , R.name as request_name    
        , L.name as leader_name
        , C.name as confirm_name
        , S.name as s_leader_name
        , V.vacation_detail
  FROM uv_vacation_history AS A LEFT JOIN uv_user AS R
                                ON A.request_user = R.uv_seq
                                LEFT JOIN uv_user AS L
                                ON A.leader_seq = L.uv_seq
                                LEFT JOIN uv_user AS C
                                ON A.confirm_user = C.uv_seq
                                LEFT JOIN uv_user AS S
                                ON A.s_leader_seq = S.uv_seq
                                LEFT JOIN (SELECT DISTINCT
                                                   V.v_seq
                                                  ,STUFF((SELECT DISTINCT
                                                                 ',' + CONVERT(VARCHAR(10), v_date, 121) + CASE v_type WHEN 1 THEN '(오전)' WHEN 2 THEN '(오후)' ELSE '(전일)' END
                                                            FROM UV_VACATION_HISTORY_DTL DTL
                                                            WHERE DTL.v_seq = V.v_seq
                                                            FOR XML PATH('')), 1, 1, '') AS vacation_detail
                                              FROM UV_VACATION_HISTORY_DTL AS V) as V
                                              ON A.v_seq = V.v_seq
 WHERE A.request_user = @uv_seq 
 AND YEAR(A.start_date) = @year ";
          if (search.vacationType != 0 && search.vacationType != 99 && search.vacationType != null)
          {
            selectQuery += " AND v_type = @v_type ";
          }
          else if (search.vacationType == 99)
          {
            selectQuery += " AND v_type NOT IN (1, 2, 7) ";
          }



          if (!string.IsNullOrWhiteSpace(search.searchTxt))
            selectQuery += string.Format(" AND {0} LIKE '%' + @searchTxt + '%' ", search.searchOption);

          selectQuery += " ORDER BY start_date DESC ";

          DynamicParameters param = new DynamicParameters();
          param.Add("@uv_seq", search.uv_seq, DbType.Int32);
          param.Add("@year", search.searchYear, DbType.Int32);
          param.Add("@v_type", search.vacationType, DbType.Int32);
          param.Add("@searchTxt", search.searchTxt, DbType.String);


          var ret = con.Query<uv_vacation_history>(selectQuery, param);


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

    public string SelectMobileReceiptFileInfo(int rad_seq)
    {
      try
      {
        using (IDbConnection con = new SqlConnection(base.BaseConnectionString))
        {
          con.Open();

          string selectQuery = @" SELECT REPLACE(REPLACE(file_path, 'UploadedFiles/', 'D:\UploadFolder\'), '/', '\') FROM receipt_user_dtl WHERE rad_seq = @rad_seq ";

          DynamicParameters param = new DynamicParameters();
          param.Add("@rad_seq", rad_seq, DbType.Int32);

          var ret = con.Query<string>(selectQuery, param);

          con.Close();

          return ret.FirstOrDefault();
        }
      }
      catch (Exception e)
      {
        throw e;
      }
    }

    public async Task<receipt_user> SelectUserReceipt(int r_seq, int uv_seq)
    {
      try
      {
        receipt_user ruser = new receipt_user();

        using (IDbConnection con = new SqlConnection(base.BaseConnectionString))
        {
          con.Open();

          string selectQuery = @" 
SELECT A.*
       , B.r_type
       , B.r_month
       , B.r_start
       , B.r_end
       , B.is_open
       , CASE B.r_type WHEN 1 THEN '통신비' WHEN 2 THEN '법인카드' WHEN 3 THEN 'LinkedIn' END As r_type_name
       , CASE B.is_open WHEN 1 THEN '제출가능' WHEN 2 THEN '마감' END As is_open_name
       , CASE WHEN A.submit_date IS NOT NULL THEN '제출완료' ELSE '미제출' END As submit_name
       , C.rad_seq
       , C.file_path
       , C.file_name
       , U.name as uv_name
       , UD.ud_name as ud_name
FROM receipt_user A INNER JOIN receipt B 
                    ON A.r_seq = B.r_seq 
                    LEFT JOIN receipt_user_dtl C
                    ON A.r_seq = C.r_seq
                    LEFT JOIN uv_user U 
                    ON A.uv_seq = U.uv_seq
                    LEFT JOIN uv_division UD
                    ON U.ud_seq = UD.ud_seq
WHERE A.r_seq = @r_seq 
AND   A.uv_seq = @uv_seq ";

          DynamicParameters param = new DynamicParameters();
          param.Add("@r_seq", r_seq, DbType.Int32);
          param.Add("@uv_seq", uv_seq, DbType.Int32);

          var ret = await con.QueryAsync<receipt_user>(selectQuery, param);
          con.Close();

          return ret.FirstOrDefault();
        }
      }
      catch (Exception e)
      {
        throw e;
      }
    }

    public async Task<List<receipt_user_dtl>> SelectUserDtlReceiptList(int r_seq, int ra_seq, int uv_seq)
    {
      try
      {
        using (IDbConnection con = new SqlConnection(base.BaseConnectionString))
        {
          con.Open();

          string selectQuery = @" 
SELECT A.*
FROM receipt_user_dtl A INNER JOIN receipt B 
                    ON A.r_seq = B.r_seq 
                    LEFT JOIN receipt_user C
                    ON A.r_seq = C.r_seq
                    AND A.ra_seq = C.ra_seq
WHERE A.r_seq = @r_seq 
AND   A.ra_seq = @ra_seq 
AND   C.uv_seq = @uv_seq 
ORDER BY A.conf_date";

          DynamicParameters param = new DynamicParameters();
          param.Add("@r_seq", r_seq, DbType.Int32);
          param.Add("@ra_seq", ra_seq, DbType.Int32);
          param.Add("@uv_seq", uv_seq, DbType.Int32);

          var ret = await con.QueryAsync<receipt_user_dtl>(selectQuery, param);
          con.Close();

          return ret.ToList();
        }
      }
      catch (Exception e)
      {
        throw e;
      }
    }

    public List<receipt> SelectReceiptList(ReceiptSearchModel search)
    {
      try
      {
        List<receipt> list = new List<receipt>();

        using (IDbConnection con = new SqlConnection(base.BaseConnectionString))
        {
          con.Open();

          string selectQuery = @" 
  SELECT  A.*
        , B.ra_seq
        , B.is_lock as is_submit
        , B.submit_date
        , B.sub_money1 as money1
        , B.comment as user_comment
        , CASE WHEN B.money_total IS NOT NULL THEN B.money_total ELSE B.sub_money2 END as money2
        , CASE A.r_type WHEN 1 THEN '통신비' WHEN 2 THEN '법인카드' WHEN 3 THEN 'LinkedIn' END As r_type_name
        , CASE A.is_open WHEN 1 THEN '제출가능' WHEN 2 THEN '마감' END As is_open_name
        , CASE WHEN B.submit_date IS NOT NULL THEN '제출완료' ELSE '미제출' END As submit_name
        , C.rad_seq
        , C.file_path
        , C.file_name
  FROM receipt AS A LEFT JOIN receipt_user AS B
                    ON A.r_seq = B.r_seq
                    LEFT JOIN (SELECT ROW_NUMBER() OVER(partition by ra_seq ORDER BY rad_seq) as l, * 
                               FROM receipt_user_dtl ) AS C
                    ON A.r_seq = C.r_seq
                    AND B.ra_seq = C.ra_seq
                    AND C.l = 1
 WHERE B.uv_seq = @uv_seq 
 AND (YEAR(A.r_start) = @year 
      OR YEAR(A.r_end) = @year
      OR LEFT(A.r_month, 4) = @year)
 ";


          if (!string.IsNullOrWhiteSpace(search.searchTxt))
            selectQuery += string.Format(" AND {0} LIKE '%' + @searchTxt + '%' ", search.searchOption);

          selectQuery += " ORDER BY A.is_open, B.is_lock, A.r_seq DESC";

          DynamicParameters param = new DynamicParameters();
          param.Add("@uv_seq", search.uv_seq, DbType.Int32);
          param.Add("@year", search.searchYear, DbType.Int32);
          param.Add("@searchTxt", search.searchTxt, DbType.String);


          var ret = con.Query<receipt>(selectQuery, param);


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
    public List<uv_vacation_history> SelectMyVacationApprovalList(int uv_seq)
    {
      try
      {
        List<uv_vacation_history> list = new List<uv_vacation_history>();

        using (IDbConnection con = new SqlConnection(base.BaseConnectionString))
        {
          con.Open();

          string selectQuery = @" 
  SELECT  A.*
        , CASE v_type WHEN 1 THEN '연차' WHEN 2 THEN '출근미등록' WHEN 3 THEN '유급병가' WHEN 4 THEN '경조사' WHEN 5 THEN '예비군' WHEN 6 THEN '기타' WHEN 7 THEN '외근' 
                      WHEN 8 THEN '출근미등록 증빙완료' WHEN 10 THEN '재택근무' WHEN 11 THEN 'SHIFT(집)' WHEN 12 THEN '무급휴가' WHEN 13 THEN '장기근속휴가' ELSE '' END as type_name
        , R.name as request_name    
        , L.name as leader_name
        , C.name as confirm_name
        , S.name as s_leader_name
        , V.vacation_detail
  FROM uv_vacation_history AS A LEFT JOIN uv_user AS R
                                ON A.request_user = R.uv_seq
                                LEFT JOIN uv_user AS L
                                ON A.leader_seq = L.uv_seq
                                LEFT JOIN uv_user AS C
                                ON A.confirm_user = C.uv_seq
                                LEFT JOIN uv_user AS S
                                ON A.s_leader_seq = S.uv_seq
                                LEFT JOIN (SELECT DISTINCT
                                                   V.v_seq
                                                  ,STUFF((SELECT DISTINCT
                                                                 ',' + CONVERT(VARCHAR(10), v_date, 121) + CASE v_type WHEN 1 THEN '(오전)' WHEN 2 THEN '(오후)' ELSE '(전일)' END
                                                            FROM UV_VACATION_HISTORY_DTL DTL
                                                            WHERE DTL.v_seq = V.v_seq
                                                            FOR XML PATH('')), 1, 1, '') AS vacation_detail
                                              FROM UV_VACATION_HISTORY_DTL AS V) as V
                                              ON A.v_seq = V.v_seq
WHERE ((A.confirm_user = @uv_seq AND ISNULL(is_confirm,0)  = 0 AND ISNULL(leader_confirm,0) = 1)
        OR (A.leader_seq   = @uv_seq AND ISNULL(leader_confirm,0) = 0)
        OR (A.s_leader_seq = @uv_seq AND ISNULL(s_leader_confirm,0) = 0 AND ISNULL(is_confirm,0)  = 1 AND ISNULL(leader_confirm,0) = 1)) 
AND ((A.v_type = 5 AND @uv_seq = 215 AND A.end_date < GETDATE()) 
     OR (A.v_type <> 5 OR @uv_seq <> 215)) ";


          selectQuery += " ORDER BY R.name, request_date DESC ";

          DynamicParameters param = new DynamicParameters();
          param.Add("@uv_seq", uv_seq, DbType.Int32);


          var ret = con.Query<uv_vacation_history>(selectQuery, param);


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
    /// 개인 알람목록 조회.
    /// </summary>
    /// <param name="search"></param>
    /// <param name="uv_seq"></param>
    /// <param name="skip"></param>
    /// <param name="count"></param>
    /// <param name="totalCount"></param>
    /// <returns></returns>
    public List<alarm_message> SelectMyAlarmListAsync(AlarmHistorySearchModel search, int uv_seq, int skip, int count, out int totalCount)
    {
      try
      {
        List<alarm_message> list = new List<alarm_message>();

        using (IDbConnection con = new SqlConnection(base.BaseConnectionString))
        {
          con.Open();

          string selectQuery = @" SELECT AM.am_seq
      ,AM.message
      ,AM.href_url
      ,AM.create_dt
      ,AU.is_read
      ,AU.read_date
  FROM alarm_user AS AU INNER JOIN alarm_message AS AM
                                   ON AM.am_seq = AU.am_seq
 WHERE AU.uv_seq = @uv_seq ";

          if (!string.IsNullOrWhiteSpace(search.searchTxt))
            selectQuery += " AND AM.message LIKE '%' + @searchTxt + '%' ";

          selectQuery += " ORDER BY AM.create_dt DESC OFFSET @currentPage ROWS FETCH NEXT @pageSize ROWS ONLY ";


          string countQuery = @" SELECT COUNT(0)
  FROM alarm_user AS AU INNER JOIN alarm_message AS AM
                                   ON AM.am_seq = AU.am_seq
 WHERE AU.uv_seq = @uv_seq ";

          if (!string.IsNullOrWhiteSpace(search.searchTxt))
            countQuery += " AND AM.message LIKE '%' + @searchTxt + '%' ";

          DynamicParameters param = new DynamicParameters();
          param.Add("@uv_seq", uv_seq, DbType.Int32);
          param.Add("@searchTxt", search.searchTxt, DbType.String);
          param.Add("@currentPage", skip, DbType.Int32);
          param.Add("@pageSize", count, DbType.Int32);

          DynamicParameters param2 = new DynamicParameters();
          param2.Add("@uv_seq", uv_seq, DbType.Int32);
          param2.Add("@searchTxt", search.searchTxt, DbType.String);

          var ret = con.Query<alarm_message>(selectQuery, param);
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

    // ── Shift 자택근무(v_type=11) 주간 한도 ─────────────────────────
    // uv_user.weekly_shift_limit (NULL 이면 기본 1일)
    public int SelectWeeklyShiftLimit(int uv_seq)
    {
      using (IDbConnection con = new SqlConnection(base.BaseConnectionString))
      {
        return con.ExecuteScalar<int>(
          "SELECT ISNULL(weekly_shift_limit, 1) FROM uv_user WHERE uv_seq = @uv_seq", new { uv_seq });
      }
    }

    // 해당 주(weekStart 이상 ~ weekEndNext 미만)에 신청된 Shift 자택근무 일수 합계 (반려 제외)
    public decimal CountShiftHomeDaysInWeek(int uv_seq, string weekStart, string weekEndNext)
    {
      using (IDbConnection con = new SqlConnection(base.BaseConnectionString))
      {
        return con.ExecuteScalar<decimal?>(@"
SELECT ISNULL(SUM(CAST(vacation_number AS decimal(9,2))), 0)
FROM uv_vacation_history
WHERE request_user = @uv_seq
  AND v_type = 11
  AND ISNULL(leader_confirm, 0) <> -1
  AND start_date >= @weekStart
  AND start_date <  @weekEndNext", new { uv_seq, weekStart, weekEndNext }) ?? 0;
      }
    }
  }
}
