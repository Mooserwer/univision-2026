using Dapper;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Univision.Core.Models;
using Univision.Core.Models.DTO;
using Univision.Core.Models.DTO.Remember;
using Univision.Core.Models.DTO.Response.Schedult;
using Univision.Security;

namespace Univision.Core.Repositories
{
  public class RememberTaskRepository : BaseRepository
  {
    /// <summary>
    /// 스케줄 단건 조회
    /// </summary>
    /// <param name="s_seq"></param>
    /// <returns></returns>
    public async Task<remember_task> FindTaskOneAsync(int task_id)
    {
      try
      {
        using (IDbConnection con = new SqlConnection(base.BaseConnectionString))
        {
          con.Open();

          string selectQuery = @" 
SELECT A.*
       , R.dt as [request_dt]
       , R.cnt as [request_cnt]
       , S.dt as [send_dt]
       , S.cnt as [send_cnt]
       , C.dt as [complete_dt]
       , C.cnt as [complete_cnt]
FROM remember_task AS A LEFT OUTER JOIN (SELECT task_id, MAX(task_dt) as dt, COUNT(*) as cnt
                                         FROM remember_task_his 
                                         WHERE task_type = 0
                                         GROUP BY task_id) R
                        ON A.task_id = R.task_id
                        LEFT OUTER JOIN (SELECT task_id, MAX(task_dt) as dt, COUNT(*) as cnt
                                         FROM remember_task_his 
                                         WHERE task_type = 1
                                         GROUP BY task_id) S
                        ON A.task_id = S.task_id
                        LEFT OUTER JOIN (SELECT task_id, MAX(task_dt) as dt, COUNT(*) as cnt
                                         FROM remember_task_his 
                                         WHERE task_type = 2
                                         GROUP BY task_id) C
                        ON A.task_id = C.task_id
WHERE A.task_id = @task_id ";

          DynamicParameters param = new DynamicParameters();
          param.Add("@task_id", task_id, DbType.Int32);

          var ret = await con.QueryAsync<remember_task>(selectQuery, param);

          con.Close();

          return ret.FirstOrDefault();
        }
      }
      catch (Exception e)
      {
        throw e;
      }
    }


    public async Task<List<remember_task>> FindTaskList(string type = "")
    {
      try
      {
        List<remember_task> list = new List<remember_task>();

        using (IDbConnection con = new SqlConnection(base.BaseConnectionString))
        {
          con.Open();

          string selectQuery = @" 
SELECT A.*
       , R.dt as [request_dt]
       , R.cnt as [request_cnt]
       , S.dt as [send_dt]
       , S.cnt as [send_cnt]
       , C.dt as [complete_dt]
       , C.cnt as [complete_cnt]
FROM remember_task AS A LEFT OUTER JOIN (SELECT task_id, MAX(task_dt) as dt, COUNT(*) as cnt
                                         FROM remember_task_his 
                                         WHERE task_type = 0
                                         GROUP BY task_id) R
                        ON A.task_id = R.task_id
                        LEFT OUTER JOIN (SELECT task_id, MAX(task_dt) as dt, COUNT(*) as cnt
                                         FROM remember_task_his 
                                         WHERE task_type = 1
                                         GROUP BY task_id) S
                        ON A.task_id = S.task_id
                        LEFT OUTER JOIN (SELECT task_id, MAX(task_dt) as dt, COUNT(*) as cnt
                                         FROM remember_task_his 
                                         WHERE task_type = 2
                                         GROUP BY task_id) C
                        ON A.task_id = C.task_id 
WHERE C.task_id is null
ORDER BY task_id ";


          var ret = await con.QueryAsync<remember_task>(selectQuery);

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

    public async Task<List<remember_task>> FindTaskReList(string type = "")
    {
      try
      {
        List<remember_task> list = new List<remember_task>();

        using (IDbConnection con = new SqlConnection(base.BaseConnectionString))
        {
          con.Open();

          string selectQuery = @" 
SELECT A.*
       , R.dt as [request_dt]
       , R.cnt as [request_cnt]
       , S.dt as [send_dt]
       , S.cnt as [send_cnt]
       , C.dt as [complete_dt]
FROM remember_task_tmp AS A LEFT OUTER JOIN (SELECT task_id, MAX(task_dt) as dt, COUNT(*) as cnt
                                         FROM remember_task_his 
                                         WHERE task_type = 0
                                         GROUP BY task_id) R
                        ON A.task_id = R.task_id
                        LEFT OUTER JOIN (SELECT task_id, MAX(task_dt) as dt, COUNT(*) as cnt
                                         FROM remember_task_his 
                                         WHERE task_type = 1
                                         GROUP BY task_id) S
                        ON A.task_id = S.task_id
                        LEFT OUTER JOIN (SELECT task_id, MAX(task_dt) as dt, COUNT(*) as cnt
                                         FROM remember_task_his 
                                         WHERE task_type = 2
                                         GROUP BY task_id) C
                        ON A.task_id = C.task_id 
ORDER BY task_id ";


          var ret = await con.QueryAsync<remember_task>(selectQuery);

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

    public async Task<r_common_paging<T>> SelectRemeberCandidateListAsync<T>(int c_seq = 0, int from_row = 0, int page_size = 100)
    {
      try
      {
        r_common_paging<T> list = new r_common_paging<T>();

        using (IDbConnection con = new SqlConnection(base.BaseConnectionString))
        {
          con.Open();

          DynamicParameters param = new DynamicParameters();
          param.Add("@c_seq", c_seq, DbType.Int64);
          param.Add("@currentPage", from_row, DbType.Int64);
          param.Add("@pageSize", page_size, DbType.Int64);

          string selectCountQuery = @"
SELECT  count(*)
FROM candidate
";
          if (c_seq != 0)
          {
            selectCountQuery = selectCountQuery + @" WHERE c_seq = @c_seq ";
          }

          var count = await con.QueryFirstOrDefaultAsync<int>(selectCountQuery, param);
          list.total_cnt = count;

          string selectQuery = @" 
SELECT  c_seq
        , kor_name
        , eng_name
        , career_start_dt
        , create_dt
        , modify_dt
        , career_end_dt
        , career_range
        , manager_seq
        , create_seq
        , modify_seq
        , keyword
        , sns_link1
        , sns_link2
        , sns_link3
        , birth_date
        , gender
        , ex_birthdate
        , country_code
        , is_foreign
        , addr1
        , addr2
        , phone
        , wrong_phone
        , cell_phone
        , wrong_phone2
        , email1
        , email2
        , ex_addr
        , hope_salary
        , hope_salary2
        , is_confidential
        , confi_remark
        , confidential_date
        , confidential_user
        , is_inactive
        , inactive_remark
        , inactive_date
        , inactive_user
        , attention_type
        , attention_remark
        , attention_date
        , attention_user
        , reg_type
        , caution_type
FROM candidate 
";
          if (c_seq != 0)
          {
            selectQuery = selectQuery + @" WHERE c_seq = @c_seq ";
          }
          selectQuery = selectQuery + @"
ORDER BY c_seq 
OFFSET @currentPage ROWS FETCH NEXT @pageSize ROWS ONLY ";



          var ret = await con.QueryAsync<T>(selectQuery, param);

          list.item = ret.ToList();

          con.Close();


          return list;
        }
      }
      catch (Exception e)
      {
        throw e;
      }
    }

    public async Task<List<r_can_school>> SelectRemeberCanSchoolListAsync(int c_seq)
    {
      try
      {
        List<r_can_school> list = new List<r_can_school>();

        using (IDbConnection con = new SqlConnection(base.BaseConnectionString))
        {
          con.Open();

          DynamicParameters param = new DynamicParameters();
          param.Add("@c_seq", c_seq, DbType.Int64);


          string selectQuery = @" 
SELECT  cs_seq
      , c_seq
      , sc_seq
      , gubun
      , graduate_year
      , admission_year
      , graduate_status
      , is_transfer
      , category1_name
      , major_name
      , is_foreign_school
      , sub_category1_name
      , sub_major_name
      , order_no
FROM can_school 
WHERE c_seq = @c_seq 
ORDER BY order_no, cs_seq ";



          var ret = await con.QueryAsync<r_can_school>(selectQuery, param);

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

    public async Task<List<r_can_career>> SelectRemeberCanCareerListAsync(int c_seq)
    {
      try
      {
        List<r_can_career> list = new List<r_can_career>();

        using (IDbConnection con = new SqlConnection(base.BaseConnectionString))
        {
          con.Open();

          DynamicParameters param = new DynamicParameters();
          param.Add("@c_seq", c_seq, DbType.Int64);


          string selectQuery = @" 
SELECT  cc_seq
      , c_seq
      , company_name
      , division_name
      , join_dt
      , leave_dt
      , is_work
      , annual_income
      , order_no
FROM can_career 
WHERE c_seq = @c_seq 
ORDER BY order_no, cc_seq ";



          var ret = await con.QueryAsync<r_can_career>(selectQuery, param);

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

    public async Task<List<r_can_business>> SelectRemeberCanBusinessListAsync(int c_seq)
    {
      try
      {
        List<r_can_business> list = new List<r_can_business>();

        using (IDbConnection con = new SqlConnection(base.BaseConnectionString))
        {
          con.Open();

          DynamicParameters param = new DynamicParameters();
          param.Add("@c_seq", c_seq, DbType.Int64);


          string selectQuery = @" 
SELECT  c_seq
      , code1
      , code2
      , order_no
FROM can_business 
WHERE c_seq = @c_seq 
ORDER BY order_no, code1, code2 ";

          var ret = await con.QueryAsync<r_can_business>(selectQuery, param);

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

    public async Task<List<r_can_job>> SelectRemeberCanJobListAsync(int c_seq)
    {
      try
      {
        List<r_can_job> list = new List<r_can_job>();

        using (IDbConnection con = new SqlConnection(base.BaseConnectionString))
        {
          con.Open();

          DynamicParameters param = new DynamicParameters();
          param.Add("@c_seq", c_seq, DbType.Int64);


          string selectQuery = @" 
SELECT  c_seq
      , code1
      , code2
      , order_no
FROM can_job
WHERE c_seq = @c_seq 
ORDER BY order_no, code1, code2 ";



          var ret = await con.QueryAsync<r_can_job>(selectQuery, param);

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

    public async Task<List<r_can_foreign_lan>> SelectRemeberCanLanguageListAsync(int c_seq)
    {
      try
      {
        List<r_can_foreign_lan> list = new List<r_can_foreign_lan>();

        using (IDbConnection con = new SqlConnection(base.BaseConnectionString))
        {
          con.Open();

          DynamicParameters param = new DynamicParameters();
          param.Add("@c_seq", c_seq, DbType.Int64);


          string selectQuery = @" 
SELECT  c_seq
      , code
      , ability
FROM can_foreign_lan
WHERE c_seq = @c_seq 
ORDER BY code";



          var ret = await con.QueryAsync<r_can_foreign_lan>(selectQuery, param);

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

    public async Task<List<r_can_resume_file_info>> SelectRemeberCanResumeListAsync(int c_seq)
    {
      try
      {
        List<r_can_resume_file_info> list = new List<r_can_resume_file_info>();

        using (IDbConnection con = new SqlConnection(base.BaseConnectionString))
        {
          con.Open();

          DynamicParameters param = new DynamicParameters();
          param.Add("@c_seq", c_seq, DbType.Int64);


          string selectQuery = @" 
SELECT  cr_seq
,c_seq
,file_type
,file_dir
,replace(file_origin_path, '/', '\') as file_origin_path
,file_path
,file_extension
,create_dt
,create_user
FROM can_resume
WHERE c_seq = @c_seq 
AND   remove_dt is null
ORDER BY cr_seq desc";



          var ret = await con.QueryAsync<r_can_resume_file_info>(selectQuery, param);

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


    public async Task<List<r_can_activity>> SelectRemeberCanActivityListAsync(int c_seq)
    {
      try
      {
        List<r_can_activity> list = new List<r_can_activity>();

        using (IDbConnection con = new SqlConnection(base.BaseConnectionString))
        {
          con.Open();

          DynamicParameters param = new DynamicParameters();
          param.Add("@c_seq", c_seq, DbType.Int64);


          string selectQuery = @" 
SELECT  ca_seq
      , c_seq
      , memo
      , create_dt
      , modify_dt
      , create_seq
      , modify_seq
FROM can_activity
WHERE c_seq = @c_seq 
ORDER BY ca_seq";



          var ret = await con.QueryAsync<r_can_activity>(selectQuery, param);

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


    public async Task<r_common_paging<T>> SelectRememberProjectListAsync<T>(string update_at = "", int p_seq = 0, int from_row = 0, int page_size = 100)
    {
      try
      {
        r_common_paging<T> list = new r_common_paging<T>();

        using (IDbConnection con = new SqlConnection(base.BaseConnectionString))
        {
          con.Open();

          DynamicParameters param = new DynamicParameters();
          param.Add("@p_seq", p_seq, DbType.Int64);
          param.Add("@update_at", update_at, DbType.String);
          param.Add("@currentPage", from_row, DbType.Int64);
          param.Add("@pageSize", page_size, DbType.Int64);

          string selectCountQuery = @"
SELECT  count(*)
FROM view_project_remember
WHERE create_dt > '2023-01-01'
";
          if (p_seq != 0)
          {
            selectCountQuery = selectCountQuery + @"  AND p_seq = @p_seq ";
          }
          else if (!String.IsNullOrEmpty(update_at))
          {
            selectCountQuery = selectCountQuery + @"  AND modify_dt >= @update_at ";
          }

          var count = await con.QueryFirstOrDefaultAsync<int>(selectCountQuery, param);
          list.total_cnt = count;

          string selectQuery = @" 
SELECT  p_seq
        , c_seq
        , client_kor_name as kor_name
        , client_eng_name as eng_name
        , pjt_type
        , pjt_type_str
        , pjt_status
        , pjt_status_str
        , status_comment
        , is_pe
        , title
        , exp_salary
        , exp_salary_won
        , currency_cd
        , fee_rate
        , position_str
        , position_seq
        , position_name
        , business_code1
        , business_name1
        , business_code2
        , business_name2
        , sub_business_code1
        , sub_business_name1
        , sub_business_code2
        , sub_business_name2
        , job_code1
        , job_name1
        , job_code2
        , job_name2
        , sub_job_code1
        , sub_job_name1
        , sub_job_code2
        , sub_job_name2
        , create_dt
        , modify_dt
        , close_dt        

        , edu_name
        , language_name
        , language_level
        , assign_task
        , requirement
        , is_posting

FROM view_project_remember
WHERE create_dt > '2023-01-01'
";
          if (p_seq != 0)
          {
            selectQuery = selectQuery + @" AND p_seq = @p_seq ";
          }
          else if (!String.IsNullOrEmpty(update_at))
          {
            selectQuery = selectQuery + @"  AND modify_dt >= @update_at ";
          }
          selectQuery = selectQuery + @"
ORDER BY p_seq 
OFFSET @currentPage ROWS FETCH NEXT @pageSize ROWS ONLY ";



          var ret = await con.QueryAsync<T>(selectQuery, param);

          list.item = ret.ToList();

          con.Close();


          return list;
        }
      }
      catch (Exception e)
      {
        throw e;
      }
    }

    public async Task<r_common_paging<T>> SelectRememberProjectListAsync_v2<T>(string update_at = "", int p_seq = 0, int from_row = 0, int page_size = 100)
    {
      try
      {
        r_common_paging<T> list = new r_common_paging<T>();

        using (IDbConnection con = new SqlConnection(base.BaseConnectionString))
        {
          con.Open();

          DynamicParameters param = new DynamicParameters();
          param.Add("@p_seq", p_seq, DbType.Int64);
          param.Add("@update_at", update_at, DbType.String);
          param.Add("@currentPage", from_row, DbType.Int64);
          param.Add("@pageSize", page_size, DbType.Int64);

          string selectCountQuery = @"
SELECT  count(*)
FROM view_project_remember
WHERE 1 = 1 ";
          if (p_seq != 0)
          {
            selectCountQuery = selectCountQuery + @"  AND p_seq = @p_seq ";
          }
          

          var count = await con.QueryFirstOrDefaultAsync<int>(selectCountQuery, param);
          list.total_cnt = count;

          string selectQuery = @" 
SELECT  p_seq
        , c_seq
        , client_kor_name as kor_name
        , client_eng_name as eng_name
        , pjt_type
        , pjt_type_str
        , pjt_status
        , pjt_status_str
        , status_comment
        , is_pe
        , title
        , exp_salary
        , exp_salary_won
        , currency_cd
        , fee_rate
        , position_str
        , position_seq
        , position_name
        , business_code1
        , business_name1
        , business_code2
        , business_name2
        , sub_business_code1
        , sub_business_name1
        , sub_business_code2
        , sub_business_name2
        , job_code1
        , job_name1
        , job_code2
        , job_name2
        , sub_job_code1
        , sub_job_name1
        , sub_job_code2
        , sub_job_name2
        , create_dt
        , modify_dt
        , close_dt        

        , edu_name
        , language_name
        , language_level
        , assign_task
        , requirement
        , is_posting
        , create_user
        , modify_user
        , internal_contents
        , client_require
        , internal_note
        , experience_type
        , experience_year
FROM view_project_remember
WHERE 1 = 1
";
          if (p_seq != 0)
          {
            selectQuery = selectQuery + @" AND p_seq = @p_seq ";
          }

          selectQuery = selectQuery + @"
ORDER BY p_seq 
OFFSET @currentPage ROWS FETCH NEXT @pageSize ROWS ONLY ";



          var ret = await con.QueryAsync<T>(selectQuery, param);

          list.item = ret.ToList();

          con.Close();


          return list;
        }
      }
      catch (Exception e)
      {
        throw e;
      }
    }


    public async Task<List<r_project_user>> SelectRememberPjtUserListAsync(int p_seq, bool director = true)
    {
      try
      {
        List<r_project_user> list = new List<r_project_user>();

        using (IDbConnection con = new SqlConnection(base.BaseConnectionString))
        {
          con.Open();

          DynamicParameters param = new DynamicParameters();
          param.Add("@p_seq", p_seq, DbType.Int64);
          string table_name = "pjt_director";
          if (!director)
          {
            table_name = "pjt_manager";
          }

          string selectQuery = @" 
SELECT  A.p_seq
       ,A.uv_seq
       ,B.name
FROM " + table_name + @" A left join uv_user B
                           ON A.uv_seq = B.uv_seq
WHERE A.p_seq = @p_seq ";



          var ret = await con.QueryAsync<r_project_user>(selectQuery, param);

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

    public async Task<List<r_pjt_recandidate_v2>> SelectRemmemberPjtRecandidateListAsync(int p_seq)
    {
      try
      {
        List<r_pjt_recandidate_v2> list = new List<r_pjt_recandidate_v2>();

        using (IDbConnection con = new SqlConnection(base.BaseConnectionString))
        {
          con.Open();

          string selectQuery = @"
SELECT a.*,
b.state as cur_state,
CASE b.state
          WHEN 10 THEN '탈락'
          WHEN 13 THEN '탈락(Self Drop)'
          WHEN 15 THEN '탈락(관심없음)'
          WHEN 20 THEN '잠재후보'
          WHEN 30 THEN '추천'
          WHEN 40 THEN '서류통과'
          WHEN 50 THEN '면접'
          WHEN 60 THEN '면접통과'
          WHEN 70 THEN '협상/검증'
          WHEN 80 THEN '입사확정'
          ELSE '' END as cur_state_str,
c.state as pre_state,
CASE c.state
          WHEN 10 THEN '탈락'
          WHEN 13 THEN '탈락(Self Drop)'
          WHEN 15 THEN '탈락(관심없음)'
          WHEN 20 THEN '잠재후보'
          WHEN 30 THEN '추천'
          WHEN 40 THEN '서류통과'
          WHEN 50 THEN '면접'
          WHEN 60 THEN '면접통과'
          WHEN 70 THEN '협상/검증'
          WHEN 80 THEN '입사확정'
          ELSE '' END as pre_state_str
FROM pjt_recandidate A LEFT JOIN (SELECT Row_number() over(partition by pic_seq order by prc_seq desc) as l, * from pjt_recandidate_history) B
on a.pic_seq = b.pic_seq
and b.l = 1
LEFT JOIN (SELECT Row_number() over(partition by pic_seq order by prc_seq desc) as l, * from pjt_recandidate_history) c
on a.pic_seq = c.pic_seq
and c.l = 2
WHERE A.p_seq = @p_seq ";

          DynamicParameters param = new DynamicParameters();
          param.Add("@p_seq", p_seq, DbType.Int32);

          var ret = await con.QueryAsync<r_pjt_recandidate_v2>(selectQuery, param);

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

    public async Task<List<r_pjt_recandidate_history_v2>> SelectRemmemberPjtRecandidateHisListAsync(int p_seq, int pic_seq)
    {
      try
      {
        List<r_pjt_recandidate_history_v2> list = new List<r_pjt_recandidate_history_v2>();

        using (IDbConnection con = new SqlConnection(base.BaseConnectionString))
        {
          con.Open();

          string selectQuery = @" 
SELECT  *
        , CASE state 
          WHEN 10 THEN '탈락'
          WHEN 13 THEN '탈락(Self Drop)'
          WHEN 15 THEN '탈락(관심없음)'
          WHEN 20 THEN '잠재후보'
          WHEN 30 THEN '추천'
          WHEN 40 THEN '서류통과'
          WHEN 50 THEN '면접'
          WHEN 60 THEN '면접통과'
          WHEN 70 THEN '협상/검증'
          WHEN 80 THEN '입사확정'
          ELSE '' END as state_str
        
FROM pjt_recandidate_history
 WHERE p_seq = @p_seq 
ORDER BY prc_seq";

          DynamicParameters param = new DynamicParameters();
          param.Add("@p_seq", p_seq, DbType.Int32);

          var ret = await con.QueryAsync<r_pjt_recandidate_history_v2>(selectQuery, param);

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

    public async Task<List<r_pjt_place>> SelectRememberPjtPlaceListAsync(int p_seq)
    {
      try
      {
        List<r_pjt_place> list = new List<r_pjt_place>();

        using (IDbConnection con = new SqlConnection(base.BaseConnectionString))
        {
          con.Open();

          string selectQuery = @" SELECT * FROM pjt_place
 WHERE p_seq = @p_seq ";

          DynamicParameters param = new DynamicParameters();
          param.Add("@p_seq", p_seq, DbType.Int32);

          var ret = await con.QueryAsync<r_pjt_place>(selectQuery, param);

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


    public async Task<r_common_paging<T>> SelectRememberClientListAsync<T>(string update_at = "", int c_seq = 0, int from_row = 0, int page_size = 100)
    {
      try
      {
        r_common_paging<T> list = new r_common_paging<T>();

        using (IDbConnection con = new SqlConnection(base.BaseConnectionString))
        {
          con.Open();

          DynamicParameters param = new DynamicParameters();
          param.Add("@c_seq", c_seq, DbType.Int64);
          param.Add("@update_at", update_at, DbType.String);
          param.Add("@currentPage", from_row, DbType.Int64);
          param.Add("@pageSize", page_size, DbType.Int64);

          string selectCountQuery = @"
SELECT  count(*)
FROM client a
";
          if (c_seq != 0)
          {
            selectCountQuery = selectCountQuery + @"  WHERE a.c_seq  = @c_seq  ";
          }
          else if (!String.IsNullOrEmpty(update_at))
          {
            selectCountQuery = selectCountQuery + @"  WHERE a.modify_dt >= @update_at ";
          }

          var count = await con.QueryFirstOrDefaultAsync<int>(selectCountQuery, param);
          list.total_cnt = count;

          string selectQuery = @" 
SELECT A.c_seq
      ,A.kor_name
      ,A.eng_name
      ,A.biz_code
      ,A.create_dt
      ,(select max(create_dt) from client_file where c_seq = A.c_seq AND cf_type = 4) as upload_dt
      , B.cc_seq as contract_seq
      , B.contract_date
      , b.fee_type
      , case b.fee_type when 'C' then b.fix_fee_rate else null end as fix_fee_rate      
FROM CLIENT A LEFT JOIN (SELECT ROW_NUMBER() OVER(PARTITION BY c_seq ORDER BY cc_seq DESC) as l, * 
                         FROM client_contract) B
              ON A.c_seq = B.c_seq
			  AND B.l = 1
			  AND B.fee_type IS NOT NULL
";
          if (c_seq != 0)
          {
            selectQuery = selectQuery + @" WHERE a.c_seq  = @c_seq  ";
          }
          else if (!String.IsNullOrEmpty(update_at))
          {
            selectQuery = selectQuery + @"  WHERE a.modify_dt >= @update_at ";
          }
          selectQuery = selectQuery + @"
ORDER BY a.c_seq  
OFFSET @currentPage ROWS FETCH NEXT @pageSize ROWS ONLY ";



          var ret = await con.QueryAsync<T>(selectQuery, param);

          list.item = ret.ToList();

          con.Close();


          return list;
        }
      }
      catch (Exception e)
      {
        throw e;
      }
    }

    public async Task<r_common_paging<T>> SelectRememberClientListAsync_v2<T>(string update_at = "", int c_seq = 0, int from_row = 0, int page_size = 100)
    {
      try
      {
        r_common_paging<T> list = new r_common_paging<T>();

        using (IDbConnection con = new SqlConnection(base.BaseConnectionString))
        {
          con.Open();

          DynamicParameters param = new DynamicParameters();
          param.Add("@c_seq", c_seq, DbType.Int64);
          param.Add("@update_at", update_at, DbType.String);
          param.Add("@currentPage", from_row, DbType.Int64);
          param.Add("@pageSize", page_size, DbType.Int64);

          string selectCountQuery = @"
SELECT  count(*)
FROM client a
";
          if (c_seq != 0)
          {
            selectCountQuery = selectCountQuery + @"  WHERE a.c_seq  = @c_seq  ";
          }
          else if (!String.IsNullOrEmpty(update_at))
          {
            selectCountQuery = selectCountQuery + @"  WHERE a.modify_dt >= @update_at ";
          }

          var count = await con.QueryFirstOrDefaultAsync<int>(selectCountQuery, param);
          list.total_cnt = count;

          string selectQuery = @" 
SELECT *      
FROM CLIENT A
";
          if (c_seq != 0)
          {
            selectQuery = selectQuery + @" WHERE a.c_seq  = @c_seq  ";
          }
          else if (!String.IsNullOrEmpty(update_at))
          {
            selectQuery = selectQuery + @"  WHERE a.modify_dt >= @update_at ";
          }
          selectQuery = selectQuery + @"
ORDER BY a.c_seq  
OFFSET @currentPage ROWS FETCH NEXT @pageSize ROWS ONLY ";



          var ret = await con.QueryAsync<T>(selectQuery, param);

          list.item = ret.ToList();

          con.Close();


          return list;
        }
      }
      catch (Exception e)
      {
        throw e;
      }
    }

    public async Task<List<r_client_manager_v2>> SelectRememberClientAmListAsync_v2(int c_seq)
    {
      try
      {
        List<r_client_manager_v2> list = new List<r_client_manager_v2>();

        using (IDbConnection con = new SqlConnection(base.BaseConnectionString))
        {
          con.Open();

          DynamicParameters param = new DynamicParameters();
          param.Add("@c_seq", c_seq, DbType.Int64);


          string selectQuery = @" 
SELECT A.*,
       B.name as uv_name
FROM client_manager A LEFT JOIN UV_USER B
                      ON A.uv_seq = B.uv_seq
WHERE A.c_seq = @c_seq 
order by A.uv_seq ";



          var ret = await con.QueryAsync<r_client_manager_v2>(selectQuery, param);

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

    public async Task<List<r_client_tax_contact_v2>> SelectRememberClientTaxContactListAsync_v2(int c_seq)
    {
      try
      {
        List<r_client_tax_contact_v2> list = new List<r_client_tax_contact_v2>();

        using (IDbConnection con = new SqlConnection(base.BaseConnectionString))
        {
          con.Open();

          DynamicParameters param = new DynamicParameters();
          param.Add("@c_seq", c_seq, DbType.Int64);


          string selectQuery = @" 
SELECT A.*
FROM client_tax_contact A 
WHERE A.c_seq = @c_seq 
order by A.ctc_seq ";



          var ret = await con.QueryAsync<r_client_tax_contact_v2>(selectQuery, param);

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

    public async Task<List<r_client_activity_log_v2>> SelectRememberClientMemoListAsync_v2(int c_seq)
    {
      try
      {
        List<r_client_activity_log_v2> list = new List<r_client_activity_log_v2>();

        using (IDbConnection con = new SqlConnection(base.BaseConnectionString))
        {
          con.Open();

          DynamicParameters param = new DynamicParameters();
          param.Add("@c_seq", c_seq, DbType.Int64);


          string selectQuery = @" 
SELECT A.*
FROM client_activity_log A 
WHERE A.c_seq = @c_seq 
order by A.cal_seq ";



          var ret = await con.QueryAsync<r_client_activity_log_v2>(selectQuery, param);

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

    public async Task<List<r_client_file_v2>> SelectRememberClientFileListAsync_v2(int c_seq)
    {
      try
      {
        List<r_client_file_v2> list = new List<r_client_file_v2>();

        using (IDbConnection con = new SqlConnection(base.BaseConnectionString))
        {
          con.Open();

          DynamicParameters param = new DynamicParameters();
          param.Add("@c_seq", c_seq, DbType.Int64);


          string selectQuery = @" 
SELECT A.cf_seq,
c_seq,
cf_type,
replace(origin_file_path, '/', '\') as origin_file_path,
file_path,
extension,
create_dt,
create_user
FROM client_file A 
WHERE A.c_seq = @c_seq 
order by A.cf_seq ";



          var ret = await con.QueryAsync<r_client_file_v2>(selectQuery, param);

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

    public async Task<List<r_client_contact_v2>> SelectRememberClientContactListAsync_v2(int c_seq)
    {
      try
      {
        List<r_client_contact_v2> list = new List<r_client_contact_v2>();

        using (IDbConnection con = new SqlConnection(base.BaseConnectionString))
        {
          con.Open();

          DynamicParameters param = new DynamicParameters();
          param.Add("@c_seq", c_seq, DbType.Int64);


          string selectQuery = @" 
SELECT A.*
FROM client_contact A 
WHERE A.c_seq = @c_seq 
order by A.cc_seq ";



          var ret = await con.QueryAsync<r_client_contact_v2>(selectQuery, param);

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

    public async Task<r_client_contract_v2> SelectRememberClientContractListAsync_v2(int c_seq)
    {
      try
      {
        //List<r_client_contract_v2> list = new List<r_client_contract_v2>();

        using (IDbConnection con = new SqlConnection(base.BaseConnectionString))
        {
          con.Open();

          DynamicParameters param = new DynamicParameters();
          param.Add("@c_seq", c_seq, DbType.Int64);


          string selectQuery = @" 
SELECT A.cc_seq,
A.c_seq,
A.fee_type,
CASE WHEN A.fee_type = 'C' THEN A.fix_fee_rate ELSE null END as fix_fee_rate,
CASE WHEN A.currency_cd = 'WON' THEN 'KRW' ELSE A.currency_cd END as currency_cd,
A.vat_type,
(select top 1 contract_date from client_contract where c_seq = a.c_seq order by cc_seq) as contract_created,
A.contract_date as contract_updated,
A.is_construct_debut,
A.deposit_gubun1,
A.deposit_period,
A.deposit_gubun2,
A.grt_type,
A.grt_period,
A.grt_gubun,
A.contract_comment
FROM (SELECT ROW_NUMBER() OVER(PARTITION BY c_seq ORDER BY cc_seq DESC) as l, * 
                         FROM client_contract) A
                      
WHERE A.c_seq = @c_seq
AND   A.l = 1
order by A.cc_seq ";



          var ret = await con.QueryAsync<r_client_contract_v2>(selectQuery, param);

          var data = ret.FirstOrDefault();

          con.Close();


          return data;
        }
      }
      catch (Exception e)
      {
        throw e;
      }
    }

    public async Task<List<r_client_contract>> SelectRememberContractIncomeListAsync(int c_seq, int cc_seq)
    {
      try
      {
        List<r_client_contract> list = new List<r_client_contract>();

        using (IDbConnection con = new SqlConnection(base.BaseConnectionString))
        {
          con.Open();

          DynamicParameters param = new DynamicParameters();
          param.Add("@c_seq", c_seq, DbType.Int64);
          param.Add("@cc_seq", cc_seq, DbType.Int64);


          string selectQuery = @" 
SELECT  A.c_seq
       , A.Currency_Name
       , A.start_income
       , A.end_income
       , percentage
FROM client_annual_income_rate A
WHERE A.c_seq = @c_seq 
AND   A.cc_seq = @cc_seq 
order by A.cfi_seq ";



          var ret = await con.QueryAsync<r_client_contract>(selectQuery, param);

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

    public async Task<List<r_contract_fee_rate_v2>> SelectRememberContractIncomeListAsync_v2(int c_seq, int cc_seq)
    {
      try
      {
        List<r_contract_fee_rate_v2> list = new List<r_contract_fee_rate_v2>();

        using (IDbConnection con = new SqlConnection(base.BaseConnectionString))
        {
          con.Open();

          DynamicParameters param = new DynamicParameters();
          param.Add("@c_seq", c_seq, DbType.Int64);
          param.Add("@cc_seq", cc_seq, DbType.Int64);


          string selectQuery = @" 
SELECT  A.c_seq
       , A.start_income
       , A.end_income
       , percentage
FROM client_annual_income_rate A
WHERE A.c_seq = @c_seq 
AND   A.cc_seq = @cc_seq 
order by A.cfi_seq ";



          var ret = await con.QueryAsync<r_contract_fee_rate_v2>(selectQuery, param);

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



    public async Task<List<r_client_contract>> SelectRememberContractPositionListAsync(int c_seq, int cc_seq)
    {
      try
      {
        List<r_client_contract> list = new List<r_client_contract>();

        using (IDbConnection con = new SqlConnection(base.BaseConnectionString))
        {
          con.Open();

          DynamicParameters param = new DynamicParameters();
          param.Add("@c_seq", c_seq, DbType.Int64);
          param.Add("@cc_seq", cc_seq, DbType.Int64);


          string selectQuery = @" 
SELECT c_seq
      ,CASE start_code_seq when 100 then '신입사원급' 
	      when 90 then '경력사원급' 
		    when 80 then '대리급' 
		    when 70 then '과장급' 
		    when 60 then '차장급' 
		    when 50 then '부장급' 
		    when 40 then '본부장/지사장' 
		    when 30 then '임원급' 
		    when 20 then '최고재무경영자' 
		    when 10 then '최고 경영자' else '' end as start_position
	    ,case end_code_seq when 100 then '신입사원급' 
	      when 90 then '경력사원급' 
		    when 80 then '대리급' 
		    when 70 then '과장급' 
		    when 60 then '차장급' 
		    when 50 then '부장급' 
		    when 40 then '본부장/지사장' 
		    when 30 then '임원급' 
		    when 20 then '최고재무경영자' 
		    when 10 then '최고 경영자' else '' end as end_position
	    ,rate as percentage
FROM client_position_rate a
WHERE A.c_seq = @c_seq 
AND   A.cc_seq = @cc_seq 
ORDER BY A.cpr_seq ";



          var ret = await con.QueryAsync<r_client_contract>(selectQuery, param);

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

    public async Task<List<r_contract_fee_rate_v2>> SelectRememberContractPositionListAsync_v2(int c_seq, int cc_seq)
    {
      try
      {
        List<r_contract_fee_rate_v2> list = new List<r_contract_fee_rate_v2>();

        using (IDbConnection con = new SqlConnection(base.BaseConnectionString))
        {
          con.Open();

          DynamicParameters param = new DynamicParameters();
          param.Add("@c_seq", c_seq, DbType.Int64);
          param.Add("@cc_seq", cc_seq, DbType.Int64);


          string selectQuery = @" 
SELECT c_seq
      ,CASE start_income when 100 then '신입사원급' 
	      when 90 then '경력사원급' 
		    when 80 then '대리급' 
		    when 70 then '과장급' 
		    when 60 then '차장급' 
		    when 50 then '부장급' 
		    when 40 then '본부장/지사장' 
		    when 30 then '임원급' 
		    when 20 then '최고재무경영자' 
		    when 10 then '최고 경영자' else '' end as start_position
	    ,case end_income when 100 then '신입사원급' 
	      when 90 then '경력사원급' 
		    when 80 then '대리급' 
		    when 70 then '과장급' 
		    when 60 then '차장급' 
		    when 50 then '부장급' 
		    when 40 then '본부장/지사장' 
		    when 30 then '임원급' 
		    when 20 then '최고재무경영자' 
		    when 10 then '최고 경영자' else '' end as end_position
	    ,percentage as percentage
FROM client_annual_income_rate a
WHERE A.c_seq = @c_seq 
AND   A.cc_seq = @cc_seq 
ORDER BY A.cfi_seq ";



          var ret = await con.QueryAsync<r_contract_fee_rate_v2>(selectQuery, param);

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

    public async Task<r_common_paging<T>> SelectRememberPjtActivityListAsync<T>(string update_at = "", int p_seq = 0, int c_seq = 0, int from_row = 0, int page_size = 100)
    {
      try
      {
        r_common_paging<T> list = new r_common_paging<T>();

        using (IDbConnection con = new SqlConnection(base.BaseConnectionString))
        {
          con.Open();

          DynamicParameters param = new DynamicParameters();
          param.Add("@p_seq", p_seq, DbType.Int64);
          param.Add("@c_seq", c_seq, DbType.Int64);
          param.Add("@update_at", update_at, DbType.String);
          param.Add("@currentPage", from_row, DbType.Int64);
          param.Add("@pageSize", page_size, DbType.Int64);

          string selectCountQuery = @"
SELECT  count(*)
FROM pjt_recandidate_history
WHERE p_seq in (select p_seq from project WHERE create_dt > '2023-01-01')
";
          if (p_seq != 0)
          {
            selectCountQuery = selectCountQuery + @"  AND p_seq = @p_seq ";
          }

          if (c_seq != 0)
          {
            selectCountQuery = selectCountQuery + @"  AND c_seq = @c_seq ";
          }

          if (!String.IsNullOrEmpty(update_at))
          {
            selectCountQuery = selectCountQuery + @"  AND modify_dt >= @update_at ";
          }

          var count = await con.QueryFirstOrDefaultAsync<int>(selectCountQuery, param);
          list.total_cnt = count;

          string selectQuery = @" 
SELECT  p_seq
        , c_seq
        , state
        , CASE state 
          WHEN 10 THEN '탈락'
          WHEN 13 THEN '탈락(Self Drop)'
          WHEN 15 THEN '탈락(관심없음)'
          WHEN 20 THEN '잠재후보'
          WHEN 30 THEN '추천'
          WHEN 40 THEN '서류통과'
          WHEN 50 THEN '면접'
          WHEN 60 THEN '면접통과'
          WHEN 70 THEN '협상/검증'
          WHEN 80 THEN '입사확정'
          ELSE '' END as state_str
        , schedule_date
        , create_dt
        , create_user
FROM pjt_recandidate_history
WHERE p_seq in (select p_seq from project WHERE  create_dt > '2023-01-01')
";
          if (p_seq != 0)
          {
            selectQuery = selectQuery + @"  AND p_seq = @p_seq ";
          }

          if (c_seq != 0)
          {
            selectQuery = selectQuery + @"  AND c_seq = @c_seq ";
          }

          if (!String.IsNullOrEmpty(update_at))
          {
            selectQuery = selectQuery + @"  AND modify_dt >= @update_at ";
          }
          selectQuery = selectQuery + @"
ORDER BY prc_seq 
OFFSET @currentPage ROWS FETCH NEXT @pageSize ROWS ONLY ";



          var ret = await con.QueryAsync<T>(selectQuery, param);

          list.item = ret.ToList();

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
