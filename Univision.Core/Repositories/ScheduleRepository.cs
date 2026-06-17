using Dapper;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Univision.Core.Models.DTO;
using Univision.Core.Models.DTO.Response.Schedult;
using Univision.Security;

namespace Univision.Core.Repositories
{
    public class ScheduleRepository : BaseRepository
    {
        /// <summary>
        /// 스케줄 단건 조회
        /// </summary>
        /// <param name="s_seq"></param>
        /// <returns></returns>
        public async Task<schedule> FindScheduleOneAsync(int s_seq)
        {
            try
            {
                using (IDbConnection con = new SqlConnection(base.BaseConnectionString))
                {
                    con.Open();

                    string selectQuery = @" SELECT S.*
      ,SA.attend_user
FROM schedule AS S LEFT OUTER JOIN (SELECT DISTINCT S.s_seq
                                          ,ISNULL(STUFF((SELECT DISTINCT
                                                                ',' + CONVERT(VARCHAR(5), B.uv_seq)
                                                           FROM   schedule AS A INNER JOIN schedule_attend AS B
                                                                                        ON A.s_seq = B.s_seq
                                                          WHERE  A.s_seq = S.s_seq
                                                            FOR XML PATH('')),1,1,''), 0) AS attend_user
                                      FROM schedule AS S)AS SA
                                ON S.s_seq = SA.s_seq
WHERE S.s_seq = @s_seq ";

                    DynamicParameters param = new DynamicParameters();
                    param.Add("@s_seq", s_seq, DbType.Int32);

                    var ret = await con.QueryAsync<schedule>(selectQuery, param);

                    con.Close();

                    return ret.FirstOrDefault();
                }
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public async Task<ScheduleCount> FindAllScheduleCountAsync(int uv_seq, string start_date, string end_date, int ud_seq)
        {
            try
            {
                List<schedule> list = new List<schedule>();

                using (IDbConnection con = new SqlConnection(base.BaseConnectionString))
                {
                    con.Open();

                    string selectQuery = @" SELECT DISTINCT *
  FROM (SELECT S.s_seq
              ,S.type
          FROM schedule AS S LEFT OUTER JOIN (SELECT s_seq, COUNT(0) AS cnt
                                                FROM schedule_attend
                                               GROUP BY s_seq) AS A
                                          ON S.s_seq = A.s_seq
         WHERE (S.create_user = @uv_seq OR S.type = 1 OR S.ud_seq = @ud_seq)
           AND S.start_date >= CONVERT(DATETIME, @start_date + ' 00:00:01')
           AND S.end_date <= CONVERT(DATETIME, @end_date + ' 23:59:59')
         UNION ALL
        SELECT S.s_seq
              ,S.type
          FROM schedule AS S INNER JOIN schedule_attend AS SA
                                     ON S.s_seq = SA.s_seq
                        LEFT OUTER JOIN (SELECT s_seq, COUNT(0) AS cnt
                                           FROM schedule_attend
                                          GROUP BY s_seq) AS A
                                     ON S.s_seq = A.s_seq
         WHERE SA.uv_seq = @uv_seq
           AND S.start_date >= CONVERT(DATETIME, @start_date + ' 00:00:01')
           AND S.end_date <= CONVERT(DATETIME, @end_date + ' 23:59:59')) AS A ";

                    DynamicParameters param = new DynamicParameters();
                    param.Add("@uv_seq", uv_seq, DbType.Int32);
                    param.Add("@start_date", start_date, DbType.String);
                    param.Add("@end_date", end_date, DbType.String);
                    param.Add("@ud_seq", ud_seq, DbType.Int32);

                    var ret = await con.QueryAsync<schedule>(selectQuery, param);

                    list = ret.ToList();

                    ScheduleCount CntModel = new ScheduleCount()
                    {
                        allCnt = list.Count(),
                        personalCnt = list.Where(x => x.type == ScheduleType.personal).Count(),
                        companyCnt = list.Where(x => x.type == ScheduleType.company).Count(),
                        teamCnt = list.Where(x => x.type == ScheduleType.team).Count(),
                        shareCnt = list.Where(x => x.type == ScheduleType.share).Count(),
                    };

                    con.Close();

                    return CntModel;
                }
            }
            catch (Exception e)
            {
                throw e;
            }
        }


        public async Task<List<schedule>> FindScheduleList(int uv_seq, string type, int sub_type, string start_date, string end_date, int ud_seq)
        {
            try
            {
                List<schedule> list = new List<schedule>();

                using (IDbConnection con = new SqlConnection(base.BaseConnectionString))
                {
                    con.Open();

                    string selectQuery = @" SELECT DISTINCT *
  FROM (SELECT S.s_seq AS id
              ,ISNULL(S.title, '') + 
               CASE WHEN S.type = 0 THEN '[개인]'
                    WHEN S.type = 1 THEN '[회사]'
                    WHEN S.type = 2 THEN '[팀]'
                    WHEN S.type = 3 THEN '[공유]'
                    ELSE '' 
               END + 
               CASE WHEN S.sub_type = 1 THEN '[일반]'
                    WHEN S.sub_type = 2 THEN '[고객사]'
                    WHEN S.sub_type = 3 THEN '[후보자]'
                    ELSE ''
                END AS title
              ,ISNULL(S.contents, '') AS contents
              ,S.type
              ,S.start_date
              ,S.end_date
              ,CONVERT(VARCHAR(19), S.start_date, 121) AS [start]
              ,CONVERT(VARCHAR(19), S.end_date, 121) AS [end]
              ,S.bg_color AS backgroundColor
              ,S.bg_color AS borderColor
              ,ISNULL(A.cnt, 0) AS attend_cnt
          FROM schedule AS S LEFT OUTER JOIN (SELECT s_seq, COUNT(0) AS cnt
                                                FROM schedule_attend
                                               GROUP BY s_seq) AS A
                                          ON S.s_seq = A.s_seq
         WHERE (S.create_user = @uv_seq OR S.type = 1 OR S.ud_seq = @ud_seq OR S.s_seq in (SELECT s_seq FROM schedule_attend WHERE uv_seq = @uv_seq))
           AND S.start_date >= CONVERT(DATETIME, @start_date + ' 00:00:01')
           AND S.end_date <= CONVERT(DATETIME, @end_date + ' 23:59:59')
           AND S.type = CASE WHEN ISNULL(@type, '') <> '' THEN @type ELSE S.type END
           AND S.sub_type = CASE WHEN ISNULL(@sub_type, 0) <> 0 THEN @sub_type ELSE S.sub_type END
        union all
        select ((vd_seq) * -1) as id,
		         CASE WHEN a.v_type = 1 THEN '[연차]'
                    WHEN a.v_type = 3 THEN '[병가]'
                    WHEN a.v_type = 4 THEN '[경조사]'
                    WHEN a.v_type = 5 THEN '[예비군]'
					WHEN a.v_type = 7 THEN '[외근]'
                    ELSE '' 
				   END + ' ' +
				   ISNULL(c.name, '') +
				   CASE WHEN b.v_type = 1 THEN '(오전)'
						WHEN b.v_type = 2 THEN '(오후)'
						ELSE ''
				   END AS title,
				   CASE WHEN a.v_type = 1 THEN '[연차]'
                    WHEN a.v_type = 3 THEN '[병가]'
                    WHEN a.v_type = 4 THEN '[경조사]'
                    WHEN a.v_type = 5 THEN '[예비군]'
					WHEN a.v_type = 7 THEN '[외근]'
                    ELSE '' 
				   END + ' ' +
				   ISNULL(c.name, '') +
				   CASE WHEN b.v_type = 1 THEN '(오전)'
						WHEN b.v_type = 2 THEN '(오후)'
						ELSE ''
				   END AS contents
		          ,4 as [type]
				  ,b.v_date as [start_date]
				  ,b.v_date as [end_date]
				  ,CONVERT(VARCHAR(19), b.v_date, 121) AS [start]
				  ,CONVERT(VARCHAR(19), b.v_date, 121) AS [end]
				  ,'#ddd' AS backgroundColor
				  ,'#ddd' AS borderColor
				  ,0 AS attend_cnt
		  from uv_vacation_history a left join uv_vacation_history_dtl b
		                             on a.v_seq = b.v_seq
									 inner join uv_user c
									 on a.request_user = c.uv_seq
									 and c.retire_date is null
          where a.v_type in (1,3,4,5,7)
		  and a.is_confirm = 1 and a.s_leader_confirm = 1 and a.leader_confirm = 1
      and 1 = case @sub_type when '' then 1 when 4 then 1 else 0 end
      and a.request_user = case @type when '0' then @uv_seq else a.request_user end
      and c.ud_seq = case @type when '' then @ud_seq when 2 then @ud_seq else c.ud_seq end
        
      AND b.v_date >= CONVERT(DATETIME, @start_date + ' 00:00:01')
      AND b.v_date <= CONVERT(DATETIME, @end_date + ' 23:59:59')



         ) AS A ";

                    DynamicParameters param = new DynamicParameters();
                    param.Add("@uv_seq", uv_seq, DbType.Int32);
                    param.Add("@type", type, DbType.String);
                    param.Add("@sub_type", sub_type, DbType.Int32);
                    param.Add("@start_date", start_date, DbType.String);
                    param.Add("@end_date", end_date, DbType.String);
                    param.Add("@ud_seq", ud_seq, DbType.Int32);

                    var ret = await con.QueryAsync<schedule>(selectQuery, param);

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


    public async Task<List<VacationList>> FindAllVacationToday()
    {
      try
      {
        List<VacationList> list = new List<VacationList>();
        string today = DateTime.Now.ToString("yyyy-MM-dd");
        using (IDbConnection con = new SqlConnection(base.BaseConnectionString))
        {
          con.Open();

          string selectQuery = @" 
SELECT A.user_id, A.ua_seq, A.name, A.rank_name, A.email, A.tel, A.hp, B.ud_name, V.v_type_str, V.v_type
FROM UV_USER A LEFT JOIN UV_DIVISION B
               ON A.ud_seq = B.ud_seq
               LEFT JOIN (SELECT request_user,
                                 MIN(X.v_type) as v_type,
                                 STUFF((SELECT '/'+CASE Y1.v_type WHEN 0 THEN '[전일]' WHEN 1 THEN '[오전]' WHEN 2 THEN '[오후]' ELSE '' END+CASE X1.v_type WHEN 1 THEN '연차' WHEN 10 THEN '재택근무' WHEN 11 THEN 'Shift' WHEN 12 THEN '무급휴가' WHEN 3 THEN '유급병가' WHEN 4 THEN '경조사' WHEN 5 THEN '예비군' WHEN 6 THEN '외근' WHEN 7 THEN '외근' ELSE '' END 
                                        FROM UV_VACATION_HISTORY X1 LEFT JOIN UV_VACATION_HISTORY_DTL Y1
                                        ON X1.v_seq = Y1.v_seq
                                        WHERE Y1.v_date = Y.v_date
                                        AND X1.request_user = X.request_user
                                        AND X1.s_leader_confirm =1 
                                 FOR XML PATH ('')), 1, 1, '') AS v_type_str
                          FROM UV_VACATION_HISTORY X LEFT JOIN UV_VACATION_HISTORY_DTL Y
                                                     ON X.v_seq = Y.v_seq
                          WHERE Y.v_date = @today
                          AND X.s_leader_confirm = 1 
                          GROUP BY X.request_user, Y.v_date) V
               ON a.uv_seq = V.request_user

WHERE retire_date IS NULL
AND A.ud_seq NOT IN (69, 52)
AND A.user_id NOT IN ('woochaelee73', 'dbmanager', 'resume', 'hykim', 'ejpark')
ORDER BY CASE A.ud_seq WHEN 4 THEN 1 ELSE 0 END, 
         B.ud_name, 
         CASE A.ua_seq WHEN 4 THEN 0 ELSE 1 END, 
         CASE A.rank_name WHEN '전무' THEN 1 WHEN '상무' THEN 2 WHEN '이사' THEN 3 WHEN '부장' THEN 4 ELSE 5 END, 
         A.name ";

          DynamicParameters param = new DynamicParameters();
          param.Add("@today", today, DbType.String);
          
          var ret = await con.QueryAsync<VacationList>(selectQuery, param);

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
