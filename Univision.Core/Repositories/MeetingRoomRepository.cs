using Dapper;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Univision.Core.Models.DTO;
using Univision.Security;

namespace Univision.Core.Repositories
{
    public class MeetingRoomRepository : BaseRepository
    {
        //회의실 리스트
        public async Task<List<meeting_room>> SelectMeetingRoomListAsync()
        {
            try
            {
                List<meeting_room> list = new List<meeting_room>();

                using (IDbConnection con = new SqlConnection(base.BaseConnectionString))
                {
                    con.Open();

                    string selectQuery = @" SELECT id
      ,title
      ,short_title
      ,floor
      ,color
      ,tv
      ,network
      ,phone
      ,wifi
      ,is_close
      ,ISNULL(room_notice, '') AS room_notice
FROM meeting_room
WHERE is_close = 0
ORDER BY id ";

                    //DynamicParameters param = new DynamicParameters();
                    //param.Add("@ud_seq", ud_seq, DbType.Int32);

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

        public async Task<meeting_room> SelectMeetingRoomOneAsync(string resource_id)
        {
            try
            {
                List<meeting_room> list = new List<meeting_room>();

                using (IDbConnection con = new SqlConnection(base.BaseConnectionString))
                {
                    con.Open();

                    string selectQuery = @" 
SELECT *
FROM meeting_room
WHERE id = @resource_id";

                    DynamicParameters param = new DynamicParameters();
                    param.Add("@resource_id", resource_id, DbType.String);

                    var ret = await con.QueryAsync<meeting_room>(selectQuery, param);
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
        /// 미팅룸 사용가능여부
        /// </summary>
        /// <param name="resource_id"></param>
        /// <param name="start_dt"></param>
        /// <param name="end_dt"></param>
        /// <returns></returns>
        public async Task<bool> MeetingRoomAvailable(string resource_id, int? event_id, string start_dt, string end_dt)
        {
            try
            {
                using (IDbConnection con = new SqlConnection(base.BaseConnectionString))
                {
                    con.Open();

                    string selectQuery = @" 
SELECT count(*) as cnt
FROM meeting_room_schedule
WHERE resourceId = @resource_id
AND dd_start < @end_dt 
AND dd_end > @start_dt ";
                    if (event_id != 0 && event_id != null)
                    {
                        selectQuery += @" AND id != @event_id";
                    }

                    DynamicParameters param = new DynamicParameters();
                    param.Add("@event_id", event_id, DbType.Int32);
                    param.Add("@resource_id", resource_id, DbType.String);
                    param.Add("@start_dt", start_dt, DbType.String);
                    param.Add("@end_dt", end_dt, DbType.String);


                    var ret = await con.QueryAsync<int>(selectQuery, param);
                    con.Close();
                    if (ret.FirstOrDefault() > 0)
                        return false;
                    else
                        return true;
                }
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        //일정
        public async Task<List<meeting_room_schedule>> SelectEventListAsync(string start_dt, string end_dt)
        {
            try
            {
                List<meeting_room_schedule> list = new List<meeting_room_schedule>();

                using (IDbConnection con = new SqlConnection(base.BaseConnectionString))
                {
                    con.Open();

                    string selectQuery = @" SELECT A.id
      ,A.req_user_seq
      ,ISNULL(A.attend_user, '') AS attned_user
      ,A.resourceId
      ,A.usage_cd
      ,A.date_str
      ,A.dd_start
      ,A.dd_end
      ,ISNULL(A.laptop, 0) AS laptop
      ,A.create_dt
      ,A.create_seq
      ,A.modify_dt
      ,A.modify_seq
      ,ISNULL(A.comment, '') AS comment
      ,ISNULL(A.is_del, 0) AS is_del
      ,ISNULL(A.is_schedule, 0) AS is_schedule
      ,C.name as req_user_name 
      ,B.title as resourceName
      ,B.short_title as resourceSortName
      ,CASE usage_cd WHEN 'client' THEN '고객사미팅'  
                     WHEN 'cand' THEN '후보자미팅'  
                     WHEN 'team' THEN '팀미팅'  
                     WHEN 'edu' THEN '교육'  
                     ELSE '기타' END As title
      ,CONVERT(VARCHAR(19), A.dd_start, 120) as start_time
      ,CONVERT(VARCHAR(19), A.dd_start, 120) as start
      ,CONVERT(VARCHAR(19), A.dd_end, 120) as end_time
      ,CONVERT(VARCHAR(19), A.dd_end, 120) as [end]
      ,b.color as event_color
FROM meeting_room_schedule A LEFT JOIN meeting_room B 
                             ON A.resourceId = B.id
                             LEFT JOIN UV_USER C 
                             ON A.req_user_seq = C.uv_seq
WHERE date_str between @start and  @end 
AND ISNULL(is_del, 0) = 0 ";


                    DynamicParameters param = new DynamicParameters();
                    param.Add("@start", start_dt, DbType.String);
                    param.Add("@end", end_dt, DbType.String);

                    var ret = await con.QueryAsync<meeting_room_schedule>(selectQuery, param);

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

        //일정
        public async Task<meeting_room_schedule> SelectEventOneAsync(int? evt_id)
        {
            try
            {
                List<meeting_room_schedule> list = new List<meeting_room_schedule>();

                using (IDbConnection con = new SqlConnection(base.BaseConnectionString))
                {
                    con.Open();

                    string selectQuery = @"
SELECT A.*, 
       C.name as req_user_name,  
       B.title as resourceName,  
       B.short_title as resourceSortName,
       CASE usage_cd WHEN 'client' THEN '고객사미팅'  
                     WHEN 'cand' THEN '후보자미팅'  
                     WHEN 'team' THEN '팀미팅'  
                     WHEN 'edu' THEN '교육'  
                     ELSE '기타' END As title,  
       CONVERT(VARCHAR(19), dd_start, 120) as start_time, 
       CONVERT(VARCHAR(19), dd_start, 120) as start, 
       CONVERT(VARCHAR(19), dd_end, 120) as end_time, 
       CONVERT(VARCHAR(19), dd_end, 120) as [end], 
       b.room_notice,
       b.color as event_color
FROM meeting_room_schedule A LEFT JOIN meeting_room B 
                             ON A.resourceId = B.id
                             LEFT JOIN UV_USER C 
                             ON A.req_user_seq = C.uv_seq
WHERE A.id = @evt_id
AND ISNULL(is_del, 0) = 0 
";


                    DynamicParameters param = new DynamicParameters();
                    param.Add("@evt_id", evt_id, DbType.Int32);

                    var ret = await con.QueryAsync<meeting_room_schedule>(selectQuery, param);
                    
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
        /// 사용자 리스트
        /// </summary>
        /// <returns></returns>
        public List<uv_user> SelectUserList(int currentPage, int pageSize, int IsRetire, string SelectOption, string SearchString, out int totalCount)
        {
            try
            {
                using (IDbConnection con = new SqlConnection(base.BaseConnectionString))
                {
                    uv_user a = new uv_user();
                    

                    con.Open();
                    DynamicParameters parameters = new DynamicParameters();
                    parameters.Add("@CurrentPage", currentPage - 1, DbType.Int16, null);
                    parameters.Add("@PageSize", pageSize, DbType.Int16, null);
                    parameters.Add("@SelectOption", SelectOption, DbType.String, null);
                    parameters.Add("@SearchString", SearchString, DbType.String, null);
                    parameters.Add("@IsRetire", IsRetire, DbType.String, null);

                    

                    //카운트와 리스트에 공용사용 될 테이블 쿼리 부분 정의
                    string base_query = @"
                        FROM
	                        UV_USER as uv
                        WHERE 1 = 1
                    ";

                    if (SelectOption == "id" && SearchString != "")
                    {
                        base_query += " and uv.user_id like '%' + @SearchString + '%'";
                    }
                    else if (SelectOption == "name" && SearchString != "")
                    {
                        base_query += " and uv.name like '%' + @SearchString + '%'";
                    }

                    if (IsRetire != 1)
                    {
                        base_query += " and uv.retire_date is null";
                    } 

                    // 리스트 부분에만 사용될 ORDER BY 절
                    string order_query = @" 
ORDER BY name, user_id ASC 
OFFSET @currentPage ROWS FETCH NEXT @pageSize ROWS ONLY
";


                    //리스트 부분에는 셀렉트 필드 + 공용쿼리(테이블&WHERE) + ORDER BY
                    string query = @"
SELECT
	uv.*,
    (select name from uv_auth where ua_seq = uv.ua_seq) as ua_name,
    (select ud_name from uv_division where ud_seq = uv.ud_seq) as ud_name
" + base_query + order_query;

                    var ret = SqlMapper.Query<uv_user>(con, query, parameters, commandType: CommandType.Text);
                    List<uv_user> list = ret.ToList();

                    //카운트 부분에는 셀렉트 필드(COUNT) + 공용쿼리(테이블&WHERE)
                    query = @"
SELECT COUNT(0) 
" + base_query;

                    totalCount = SqlMapper.ExecuteScalar<int>(con, query, parameters, commandType: CommandType.Text);

                    con.Close();

                    return list;
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        /// <summary>
        /// 사용자 단건 셀렉트
        /// </summary>
        /// <param name="uv_seq"></param>
        /// <returns></returns>
        public async Task<uv_user> SelectUserOneAsync(int uv_seq)
        {
            try
            {
                using (IDbConnection con = new SqlConnection(base.BaseConnectionString))
                {
                    con.Open();

                    string selectQuery = @" 
SELECT
    uv.*,
    (select name from uv_auth where ua_seq = uv.ua_seq) as ua_name,
    (select ud_name from uv_division where ud_seq = uv.ud_seq) as ud_name
FROM UV_USER as uv
WHERE uv.uv_seq = @uv_seq ";

                    DynamicParameters param = new DynamicParameters();
                    param.Add("@uv_seq", uv_seq, DbType.Int32);

                    var ret = await con.QueryAsync<uv_user>(selectQuery, param);

                    con.Close();

                    return ret.FirstOrDefault();
                }
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public async Task<uv_user> SelectUserOneAsync(string user_id)
        {
            try
            {
                using (IDbConnection con = new SqlConnection(base.BaseConnectionString))
                {
                    con.Open();

                    string selectQuery = @" 
SELECT
    uv.*
FROM UV_USER as uv
WHERE uv.user_id = @user_id ";

                    DynamicParameters param = new DynamicParameters();
                    param.Add("@user_id", user_id, DbType.String);

                    var ret = await con.QueryAsync<uv_user>(selectQuery, param);

                    con.Close();

                    return ret.FirstOrDefault();
                }
            }
            catch (Exception e)
            {
                throw e;
            }
        }


    }
}
