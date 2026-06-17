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
using Univision.Core.Models.DTO.Response;
using Univision.Core.Models.DTO.Response.Api;
using Univision.Core.Models.DTO.Response.Candidate;
using Univision.Security;

namespace Univision.Core.Repositories
{
  public class CandidateRepository : BaseRepository
  {
    /// <summary>
    /// 후보자 기본정보
    /// </summary>
    /// <param name="c_seq"></param>
    /// <returns></returns>
    public async Task<candidate> SelectCandidateOneAsync(int c_seq)
    {
      try
      {
        using (IDbConnection con = new SqlConnection(base.BaseConnectionString))
        {
          con.Open();

          string selectQuery = @" 
SELECT A.*, B.name as manager_name, CN.Kr_Country_Name AS country_name
       ,CU.name as confidential_name
       ,IU.name as inactive_name
       ,AU.name as attention_name
FROM candidate A LEFT JOIN UV_USER B 
                 ON A.manager_seq = B.uv_seq
                 LEFT JOIN UV_USER CU
                 ON A.confidential_user = CU.uv_seq
                 LEFT JOIN UV_USER IU 
                 ON A.inactive_user = IU.uv_seq
                 LEFT JOIN UV_USER AU 
                 ON A.attention_user = AU.uv_seq
                 LEFT OUTER JOIN code_nationality AS CN
                              ON A.country_code = CN.Nationality_Code
WHERE c_seq = @c_seq ";

          DynamicParameters param = new DynamicParameters();
          param.Add("@c_seq", c_seq, DbType.Int32);

          var ret = await con.QueryAsync<candidate>(selectQuery, param);

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
    /// 후보자 기본정보
    /// </summary>
    /// <param name="c_seq"></param>
    /// <returns></returns>
    public async Task<director> SelectDirectorOneAsync(int d_seq)
    {
      try
      {
        using (IDbConnection con = new SqlConnection(base.BaseConnectionString))
        {
          con.Open();

          string selectQuery = @" 
SELECT A.*, B.name as manager_name, CN.Kr_Country_Name AS country_name
       ,CU.name as confidential_name
       ,IU.name as inactive_name
       ,AU.name as attention_name
FROM director A LEFT JOIN UV_USER B 
                 ON A.manager_seq = B.uv_seq
                 LEFT JOIN UV_USER CU
                 ON A.confidential_user = CU.uv_seq
                 LEFT JOIN UV_USER IU 
                 ON A.inactive_user = IU.uv_seq
                 LEFT JOIN UV_USER AU 
                 ON A.attention_user = AU.uv_seq
                 LEFT OUTER JOIN code_nationality AS CN
                              ON A.country_code = CN.Nationality_Code
WHERE d_seq = @d_seq ";

          DynamicParameters param = new DynamicParameters();
          param.Add("@d_seq", d_seq, DbType.Int32);

          var ret = await con.QueryAsync<director>(selectQuery, param);

          con.Close();

          return ret.FirstOrDefault();
        }
      }
      catch (Exception e)
      {
        throw e;
      }
    }


    public async Task<mail_resume> SelectMailResumeOneASync(string timestamp)
    {
      try
      {
        using (IDbConnection con = new SqlConnection(base.BaseConnectionString))
        {
          con.Open();

          string selectQuery = @" 
SELECT *
       , ISNULL(B.uv_seq, 0) as manager_seq
       , B.name as manager_name
FROM mail_resume A LEFT JOIN uv_user B
                   ON A.dv_rcv_id = B.user_id
WHERE dv_timestamp = @timestamp ";

          DynamicParameters param = new DynamicParameters();
          param.Add("@timestamp", timestamp, DbType.String);

          var ret = await con.QueryAsync<mail_resume>(selectQuery, param);

          con.Close();

          return ret.FirstOrDefault();
        }
      }
      catch (Exception e)
      {
        throw e;
      }
    }
    public async Task<mail_resume_file> SelectMailResumeFileOneASync(string timestamp, int dn_seq)
    {
      try
      {
        using (IDbConnection con = new SqlConnection(base.BaseConnectionString))
        {
          con.Open();

          string selectQuery = @" 
SELECT *, replace(dv_file_path, 'Y:\', 'D:\UploadFolder\OLD\resume_from_mail\') as file_path
FROM mail_resume_file A 
WHERE dv_timestamp = @timestamp 
AND   dn_seq = @dn_seq ";

          DynamicParameters param = new DynamicParameters();
          param.Add("@timestamp", timestamp, DbType.String);
          param.Add("@dn_seq", dn_seq, DbType.Int32);

          var ret = await con.QueryAsync<mail_resume_file>(selectQuery, param);

          con.Close();

          return ret.FirstOrDefault();
        }
      }
      catch (Exception e)
      {
        throw e;
      }
    }

    public async Task<mail_resume_gpt> SelectMailGPTOneASync(string timestamp)
    {
      try
      {
        using (IDbConnection con = new SqlConnection(base.BaseConnectionString))
        {
          con.Open();

          string selectQuery = @" 
SELECT *
FROM mail_resume_gpt
WHERE dv_timestamp = @timestamp ";

          DynamicParameters param = new DynamicParameters();
          param.Add("@timestamp", timestamp, DbType.String);

          var ret = await con.QueryAsync<mail_resume_gpt>(selectQuery, param);

          con.Close();

          return ret.FirstOrDefault();
        }
      }
      catch (Exception e)
      {
        throw e;
      }
    }

    public async Task<int> SelectPhoneDuplicateCheck(string phone, int except_c_seq)
    {
      try
      {
        using (IDbConnection con = new SqlConnection(base.BaseConnectionString))
        {
          con.Open();

          string selectQuery = String.Empty;

          if (!String.IsNullOrEmpty(phone))
            selectQuery += @" 
SELECT COUNT(*)
FROM CANDIDATE_SEARCH_TXT A INNER JOIN CANDIDATE B
ON A.c_seq= B.c_seq
WHERE (A.phone_search = @phone or A.cell_phone_search = @phone ) 
AND  (isnull(A.phone_search, '') <> '' 
OR  ISNULL(A.cell_phone_search, '') <> '') ";

          if (except_c_seq > 0)
          {
            selectQuery += " AND A.c_seq <>  @c_seq ";
          }

          DynamicParameters param = new DynamicParameters();
          param.Add("@phone", phone, DbType.String);
          param.Add("@c_seq", except_c_seq, DbType.Int32);

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

    public async Task<candidate> SelectFindDuplicateCandidate(string phone, string email, int except_c_seq)
    {
      try
      {
        using (IDbConnection con = new SqlConnection(base.BaseConnectionString))
        {
          con.Open();

          string selectQuery = String.Empty;
          string selectWhere = String.Empty;
          if (!String.IsNullOrEmpty(phone) || !String.IsNullOrEmpty(email))
          {
            selectQuery += @" 
SELECT A.c_seq, B.kor_name, B.birth_date, B.cell_phone, B.email1, B.gender
FROM CANDIDATE_SEARCH_TXT A INNER JOIN CANDIDATE B
ON A.c_seq= B.c_seq ";

            if (!String.IsNullOrEmpty(phone))
            {

              selectWhere += @"
( A.phone_search = @phone or A.cell_phone_search = @phone)
  AND(isnull(A.phone_search, '') <> '' OR  ISNULL(A.cell_phone_search, '') <> '') ";
            }
            if (!String.IsNullOrEmpty(email))
            {
              if (!String.IsNullOrEmpty(selectWhere))
                selectWhere += @" OR ";

              selectWhere += @"
(B.email1 like @email + '%' or B.email2 like @email + '%') ";
            }

            if (!String.IsNullOrEmpty(selectWhere))
            {
              selectWhere = @" WHERE ( " + selectWhere + ") ";
            }

            if (except_c_seq > 0)
            {
              if (!String.IsNullOrEmpty(selectWhere))
                selectWhere += @" AND ";
              selectWhere += " AND A.c_seq <>  @c_seq ";
            }
          }
          DynamicParameters param = new DynamicParameters();
          param.Add("@phone", phone, DbType.String);
          param.Add("@email", email, DbType.String);
          param.Add("@c_seq", except_c_seq, DbType.Int32);

          var ret = await con.QueryAsync<candidate>(selectQuery + selectWhere, param);

          con.Close();

          return ret.FirstOrDefault();
        }
      }
      catch (Exception e)
      {
        throw e;
      }
    }



    public async Task<int> SelectEmailDuplicateCheck(string email, int except_c_seq)
    {
      try
      {
        using (IDbConnection con = new SqlConnection(base.BaseConnectionString))
        {
          con.Open();

          string selectQuery = String.Empty;

          if (!String.IsNullOrEmpty(email))
            selectQuery += @" 
SELECT COUNT(*)
FROM candidate
WHERE (email1 like @email+'%' or email2 like @email+'%') ";

          if (except_c_seq > 0)
          {
            selectQuery += " AND c_seq <>  @c_seq ";
          }

          DynamicParameters param = new DynamicParameters();
          param.Add("@email", email, DbType.String);
          param.Add("@c_seq", except_c_seq, DbType.Int32);

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

    public async Task<int> SelectCandidateDuplicateCheck(string phone, string email)
    {
      try
      {
        using (IDbConnection con = new SqlConnection(base.BaseConnectionString))
        {
          con.Open();

          string selectQuery = String.Empty;

          if (!String.IsNullOrEmpty(phone))
            selectQuery += @" 
SELECT COUNT(*)
FROM CANDIDATE_SEARCH_TXT
WHERE (phone_search = @phone or cell_phone_search = @phone ) 
AND  (isnull(phone_search, '') <> '' 
OR  ISNULL(cell_phone_search, '') <> '') ";

          if (!String.IsNullOrEmpty(email))
            selectQuery += @" 
SELECT COUNT(*)
FROM candidate
WHERE (email1 like @email+'%' or email2 like @email+'%') ";

          DynamicParameters param = new DynamicParameters();
          param.Add("@phone", phone, DbType.String);
          param.Add("@email", email, DbType.String);

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


    public async Task<view_can_activity> SelectCanProjectProgressOneAsync(int c_seq)
    {
      try
      {
        using (IDbConnection con = new SqlConnection(base.BaseConnectionString))
        {
          con.Open();

          string selectQuery = @" 
SELECT TOP 1 
       C.kor_name as client_name,
       P.title as pjt_title,
       A.schedule_date as ca_date,
       A.state,
       A.contents as memo,
       P.pjt_status
FROM pjt_recandidate_history A LEFT JOIN project P 
                 ON A.p_seq = P.p_seq
                 LEFT OUTER JOIN client AS C
                 ON P.c_seq = C.c_seq
WHERE A.c_seq = @c_seq 
AND   A.modify_dt > dateadd(m, -12, getdate())
ORDER BY A.modify_dt DESC, A.state DESC";

          DynamicParameters param = new DynamicParameters();
          param.Add("@c_seq", c_seq, DbType.Int32);

          var ret = await con.QueryAsync<view_can_activity>(selectQuery, param);

          con.Close();

          return ret.FirstOrDefault();
        }
      }
      catch (Exception e)
      {
        throw e;
      }
    }

    public async Task<candidate> TempSelectCandidateOneAsync(int t_seq)
    {
      try
      {
        using (IDbConnection con = new SqlConnection(base.BaseConnectionString))
        {
          con.Open();

          string selectQuery = @" 
SELECT A.*, B.name as manager_name, CN.Kr_Country_Name AS country_name
FROM tempsaved_candidate A LEFT JOIN UV_USER B 
                 ON A.manager_seq = B.uv_seq
                 LEFT OUTER JOIN code_nationality AS CN
                              ON A.country_code = CN.Nationality_Code
WHERE c_seq = @c_seq ";

          DynamicParameters param = new DynamicParameters();
          param.Add("@c_seq", t_seq, DbType.Int32);

          var ret = await con.QueryAsync<candidate>(selectQuery, param);

          con.Close();

          return ret.FirstOrDefault();
        }
      }
      catch (Exception e)
      {
        throw e;
      }
    }

    public async Task<simple_candidate> SelectSimpleCandidateOneAsync(int sc_seq)
    {
      try
      {
        using (IDbConnection con = new SqlConnection(base.BaseConnectionString))
        {
          con.Open();

          string selectQuery = @" 
SELECT A.*
FROM simple_candidate A 
WHERE sc_seq = @sc_seq ";

          DynamicParameters param = new DynamicParameters();
          param.Add("@sc_seq", sc_seq, DbType.Int32);

          var ret = await con.QueryAsync<simple_candidate>(selectQuery, param);

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
    /// 후보자 상세보기
    /// </summary>
    /// <param name="c_seq"></param>
    /// <returns></returns>
    public async Task<candidate> SelectCandidateWithDetailOneAsync(int c_seq)
    {
      try
      {
        using (IDbConnection con = new SqlConnection(base.BaseConnectionString))
        {
          con.Open();

          string selectQuery = @" SELECT C.*
      ,DATEDIFF(m, ISNULL(C.career_start_dt, GETDATE()), ISNULL(C.career_end_dt, GETDATE())) as cur_career_month
      ,CS.school_name
      ,CS.category1_name
      ,CC.company_name
      ,CC.division_name
      ,CC.r_name
      ,CC.p_name
      ,CC.is_work
      ,CJ.code_name2 AS jobName
      ,CB.code_name2 AS busiName
      ,CF.kor_foreign_name
      ,CF.abilityDesc
      ,CI.cf_seq
      ,mnu.name as manager_name
      ,crt.name as create_name
      ,mdf.name as modify_name
      ,CU.name as confidential_name
      ,IU.name as inactive_name
      ,AU.name as attention_name
      ,(select count(0) from candidate_log where c_seq = c.c_seq) log_cnt
  FROM candidate AS C LEFT OUTER JOIN (SELECT c_seq
                                             ,school_name
                                             ,category1_name
                                         FROM (SELECT c_seq
                                                     ,DENSE_RANK() OVER(PARTITION BY c_seq ORDER BY c_seq, graduate_year DESC) AS rank
                                                     ,CASE WHEN ISNULL(SC.campus_name, '') <> '' THEN SC.school_name + '(' + SC.campus_name + ')' ELSE SC.school_name END AS school_name
                                                     ,CS.category1_name
                                                     ,CS.graduate_year
                                                 FROM can_school AS CS INNER JOIN code_school AS SC
                                                                               ON CS.sc_seq = SC.sc_seq
                                                GROUP BY CS.c_seq, SC.school_name, SC.campus_name, CS.category1_name,graduate_year) AS A
                                        WHERE rank = 1) AS CS 
                                   ON C.c_seq = CS.c_seq
                       LEFT OUTER JOIN (SELECT c_seq
                                              ,company_name
                                              ,division_name
                                              ,is_work
                                              ,r_name
                                              ,p_name
                                          FROM (SELECT c_seq
                                                      ,DENSE_RANK() OVER(PARTITION BY c_seq ORDER BY c_seq, join_dt DESC) AS rank
                                                      ,company_name
                                                      ,division_name
                                                      ,is_work
                                                      ,B.r_name
                                                      ,C.p_name
                                                  FROM can_career AS A LEFT OUTER JOIN code_rank AS B
                                                                                    ON A.r_code = B.r_code
                                                                       LEFT OUTER JOIN code_position AS C
                                                                                    ON A.p_code = C.p_code
                                                 GROUP BY c_seq, company_name, division_name, is_work, join_dt, B.r_name, C.p_name) AS A
                                         WHERE rank = 1) AS CC
                                   ON C.c_seq = CC.c_seq
                      LEFT OUTER JOIN (SELECT DISTINCT 
                                              CJ.c_seq
                                             ,REPLACE(STUFF((SELECT DISTINCT
                                                                    '/' + B.code_name2
                                                               FROM   can_job AS A INNER JOIN code_job_dtl AS B
                                                                                                ON A.code2 = B.code2
                                                               WHERE  A.c_seq = CJ.c_seq
                                                               FOR XML PATH('')),1,1,''),  '&amp;', '&') AS code_name2
                                         FROM can_job AS CJ) AS CJ
                                   ON C.c_seq = CJ.c_seq
                      LEFT OUTER JOIN (SELECT DISTINCT 
                                              CB.c_seq
                                             ,REPLACE(STUFF((SELECT DISTINCT
                                                                    '/' + B.code_name2
                                                               FROM   can_business AS A INNER JOIN code_business_dtl AS B
                                                                                                     ON A.code2 = B.code2
                                                               WHERE  A.c_seq = CB.c_seq
                                                               FOR XML PATH('')),1,1,''),  '&amp;', '&') AS code_name2
                                         FROM can_business AS CB) AS CB
                                   ON C.c_seq = CB.c_seq
                      LEFT OUTER JOIN (SELECT c_seq
                                             ,kor_foreign_name
                                             ,CASE WHEN ability = 1 THEN '상'
                                                   WHEN ability = 2 THEN '중'
                                                   WHEN ability = 3 THEN '하'
                                                   ELSE ''
                                               END AS abilityDesc
                                         FROM (SELECT c_seq
                                                     ,ROW_NUMBER() OVER(PARTITION BY c_seq ORDER BY c_seq, ability ASC) AS rank
                                                     ,ability
                                                     ,B.kor_foreign_name
                                                FROM can_foreign_lan AS A INNER JOIN code_foreign_lan AS B
                                                                                  ON A.code = B.code
                                               GROUP BY c_seq,ability, kor_foreign_name) AS A
                                        WHERE A.rank = 1) AS CF
                                   ON C.c_seq = CF.c_seq
                      LEFT OUTER JOIN can_interest AS CI
                                   ON C.c_seq = CI.c_seq
                                  AND CI.uv_seq = @user_seq
                      LEFT OUTER JOIN uv_user AS mnu
                                   ON C.manager_seq = mnu.uv_seq
                      LEFT OUTER JOIN uv_user AS crt
                                   ON C.create_seq = crt.uv_seq
                      LEFT OUTER JOIN uv_user AS mdf
                                   ON C.modify_seq = mdf.uv_seq
                      LEFT JOIN UV_USER CU
                 ON C.confidential_user = CU.uv_seq
                 LEFT JOIN UV_USER IU 
                 ON C.inactive_user = IU.uv_seq
                 LEFT JOIN UV_USER AU 
                 ON C.attention_user = AU.uv_seq
 WHERE C.c_seq = @c_seq ";

          DynamicParameters param = new DynamicParameters();
          param.Add("@c_seq", c_seq, DbType.Int32);
          param.Add("@user_seq", AppIdentity.user_seq, DbType.Int32);

          var ret = await con.QueryAsync<candidate>(selectQuery, param);

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
    /// 후보자 상세보기
    /// </summary>
    /// <param name="c_seq"></param>
    /// <returns></returns>
    public async Task<candidate> SelectCandidateWithDetailOneAsync_2024(int c_seq)
    {
      try
      {
        using (IDbConnection con = new SqlConnection(base.BaseConnectionString))
        {
          con.Open();

          string selectQuery = @" SELECT C.*
      ,DATEDIFF(m, ISNULL(C.career_start_dt, GETDATE()), ISNULL(C.career_end_dt, GETDATE())) as cur_career_month
      
      ,CF.kor_foreign_name
      ,CF.abilityDesc
      ,CI.cf_seq
      ,mnu.name as manager_name
      ,crt.name as create_name
      ,mdf.name as modify_name
      ,CU.name as confidential_name
      ,IU.name as inactive_name
      ,AU.name as attention_name
      ,(select count(0) from candidate_log where c_seq = c.c_seq) log_cnt
  FROM candidate AS C LEFT OUTER JOIN (SELECT c_seq
                                             ,kor_foreign_name
                                             ,CASE WHEN ability = 1 THEN '상'
                                                   WHEN ability = 2 THEN '중'
                                                   WHEN ability = 3 THEN '하'
                                                   ELSE ''
                                               END AS abilityDesc
                                         FROM (SELECT c_seq
                                                     ,ROW_NUMBER() OVER(PARTITION BY c_seq ORDER BY c_seq, ability ASC) AS rank
                                                     ,ability
                                                     ,B.kor_foreign_name
                                                FROM can_foreign_lan AS A INNER JOIN code_foreign_lan AS B
                                                                                  ON A.code = B.code
                                               GROUP BY c_seq,ability, kor_foreign_name) AS A
                                        WHERE A.rank = 1) AS CF
                                   ON C.c_seq = CF.c_seq
                      LEFT OUTER JOIN can_interest AS CI
                                   ON C.c_seq = CI.c_seq
                                  AND CI.uv_seq = @user_seq
                      LEFT OUTER JOIN uv_user AS mnu
                                   ON C.manager_seq = mnu.uv_seq
                      LEFT OUTER JOIN uv_user AS crt
                                   ON C.create_seq = crt.uv_seq
                      LEFT OUTER JOIN uv_user AS mdf
                                   ON C.modify_seq = mdf.uv_seq
                      LEFT JOIN UV_USER CU
                 ON C.confidential_user = CU.uv_seq
                 LEFT JOIN UV_USER IU 
                 ON C.inactive_user = IU.uv_seq
                 LEFT JOIN UV_USER AU 
                 ON C.attention_user = AU.uv_seq
 WHERE C.c_seq = @c_seq ";

          DynamicParameters param = new DynamicParameters();
          param.Add("@c_seq", c_seq, DbType.Int32);
          param.Add("@user_seq", AppIdentity.user_seq, DbType.Int32);

          var ret = await con.QueryAsync<candidate>(selectQuery, param);

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
    /// 후보자 상세보기
    /// </summary>
    /// <param name="d_seq"></param>
    /// <returns></returns>
    public async Task<director> SelectDirectorDetailOneAsync(int d_seq)
    {
      try
      {
        using (IDbConnection con = new SqlConnection(base.BaseConnectionString))
        {
          con.Open();

          string selectQuery = @" SELECT C.*
      ,YEAR(C.birth_date) as birth_year
      ,CS.school_name
      ,CS.category1_name
      ,CC.company_name
      ,CC.division_name
      ,CC.r_name
      ,CC.p_name
      ,CC.is_work
      ,CJ.code_name2 AS jobName
      ,CB.code_name2 AS busiName      
      ,mnu.name as manager_name
      ,crt.name as create_name
      ,mdf.name as modify_name
      ,CU.name as confidential_name
      ,IU.name as inactive_name
      ,AU.name as attention_name
      ,0 log_cnt
  FROM director AS C LEFT OUTER JOIN (SELECT d_seq
                                             ,school_name
                                             ,category1_name
                                         FROM (SELECT d_seq                                                
                                                     ,CS.school_name AS school_name
                                                     ,CS.category1_name
                                                     ,CS.graduate_year
                                                 FROM director_school AS CS 
                                                ) AS A
                                        ) AS CS 
                                   ON C.d_seq = CS.d_seq
                       LEFT OUTER JOIN (SELECT d_seq
                                              ,company_name
                                              ,division_name
                                              ,is_work
                                              ,r_name
                                              ,p_name
                                          FROM (SELECT d_seq
                                                      ,DENSE_RANK() OVER(PARTITION BY d_seq ORDER BY d_seq, join_dt DESC) AS rank
                                                      ,company_name
                                                      ,division_name
                                                      ,is_work
                                                      ,B.r_name
                                                      ,C.p_name
                                                  FROM director_career AS A LEFT OUTER JOIN code_rank AS B
                                                                                    ON A.r_code = B.r_code
                                                                       LEFT OUTER JOIN code_position AS C
                                                                                    ON A.p_code = C.p_code
                                                 GROUP BY d_seq, company_name, division_name, is_work, join_dt, B.r_name, C.p_name) AS A
                                         WHERE rank = 1) AS CC
                                   ON C.d_seq = CC.d_seq
                      LEFT OUTER JOIN (SELECT DISTINCT 
                                              CJ.d_seq
                                             ,REPLACE(STUFF((SELECT DISTINCT
                                                                    '/' + B.code_name2
                                                               FROM   director_job AS A INNER JOIN code_job_dtl AS B
                                                                                                ON A.code2 = B.code2
                                                               WHERE  A.d_seq = CJ.d_seq
                                                               FOR XML PATH('')),1,1,''),  '&amp;', '&') AS code_name2
                                         FROM director_job AS CJ) AS CJ
                                   ON C.d_seq = CJ.d_seq
                      LEFT OUTER JOIN (SELECT DISTINCT 
                                              CB.d_seq
                                             ,REPLACE(STUFF((SELECT DISTINCT
                                                                    '/' + B.code_name2
                                                               FROM   director_business AS A INNER JOIN code_business_dtl AS B
                                                                                                     ON A.code2 = B.code2
                                                               WHERE  A.d_seq = CB.d_seq
                                                               FOR XML PATH('')),1,1,''),  '&amp;', '&') AS code_name2
                                         FROM director_business AS CB) AS CB
                                   ON C.d_seq = CB.d_seq
                      LEFT OUTER JOIN uv_user AS mnu
                                   ON C.manager_seq = mnu.uv_seq
                      LEFT OUTER JOIN uv_user AS crt
                                   ON C.create_seq = crt.uv_seq
                      LEFT OUTER JOIN uv_user AS mdf
                                   ON C.modify_seq = mdf.uv_seq
                      LEFT JOIN UV_USER CU
                 ON C.confidential_user = CU.uv_seq
                 LEFT JOIN UV_USER IU 
                 ON C.inactive_user = IU.uv_seq
                 LEFT JOIN UV_USER AU 
                 ON C.attention_user = AU.uv_seq
 WHERE C.d_seq = @d_seq ";

          DynamicParameters param = new DynamicParameters();
          param.Add("@d_seq", d_seq, DbType.Int32);

          var ret = await con.QueryAsync<director>(selectQuery, param);

          con.Close();

          return ret.FirstOrDefault();
        }
      }
      catch (Exception e)
      {
        throw e;
      }
    }

    public async Task<pjt_recandidate_history> SelectCandidateLastestHire(int c_seq)
    {
      try
      {
        using (IDbConnection con = new SqlConnection(base.BaseConnectionString))
        {
          con.Open();

          string selectQuery = @" 
SELECT TOP 1 *
FROM PJT_RECANDIDATE_HISTORY 
WHERE prc_seq in (
  SELECT MAX(prc_seq) 
  FROM PJT_RECANDIDATE_HISTORY 
  WHERE c_seq = @c_seq
  GROUP BY p_seq, c_seq
)
AND state = 80
ORDER BY schedule_date DESC ";

          DynamicParameters param = new DynamicParameters();
          param.Add("@c_seq", c_seq, DbType.Int32);

          var ret = await con.QueryAsync<pjt_recandidate_history>(selectQuery, param);

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
    /// 후보자 목록 조회
    /// </summary>
    /// <returns></returns>
    public List<candidate> SelectCandidateList(CandidateSearchModel search, int skip, int count, out int totalCount)
    {
      try
      {
        List<candidate> list = new List<candidate>();

        using (IDbConnection con = new SqlConnection(base.BaseConnectionString))
        {
          con.Open();

          string selectQuery = @" SELECT C.*
      ,CASE WHEN C.is_foreign = 1 THEN C.eng_name ELSE C.kor_name END AS kor_name
      ,CS.school_name
      ,CS.category1_name
      ,CC.company_name
      ,CC.is_work
      ,CJ.code_name2 AS jobName
      ,CB.code_name2 AS busiName
      ,CK.file_dir AS kor_file_dir
      ,CK.file_extension AS kor_file_ext
      ,CE.file_dir AS eng_file_dir
      ,CE.file_extension AS eng_file_ext
      ,C.create_dt
      ,C.modify_dt
      ,CI.cf_seq
  FROM candidate AS C LEFT OUTER JOIN (SELECT c_seq
                                             ,final_edu
                                             ,school_name
                                             ,category1_name
                                             ,major_name
                                         FROM (SELECT c_seq
                                                     ,CS.gubun as final_edu
                                                     ,major_name
                                                     ,DENSE_RANK() OVER(PARTITION BY c_seq ORDER BY c_seq, graduate_year DESC) AS rank
                                                     ,CASE WHEN ISNULL(SC.campus_name, '') <> '' THEN SC.school_name + '(' + SC.campus_name + ')' ELSE SC.school_name END AS school_name
                                                     ,CS.category1_name
                                                     ,CS.graduate_year
                                                 FROM can_school AS CS INNER JOIN code_school AS SC
                                                                               ON CS.sc_seq = SC.sc_seq
                                                GROUP BY CS.c_seq, CS.gubun, SC.school_name, SC.campus_name, CS.category1_name, CS.major_name, graduate_year) AS A
                                        WHERE rank = 1) AS CS 
                                   ON C.c_seq = CS.c_seq
                       LEFT OUTER JOIN (SELECT c_seq
                                              ,company_name
                                              ,is_work
                                          FROM (SELECT c_seq
                                                      ,DENSE_RANK() OVER(PARTITION BY c_seq ORDER BY c_seq, join_dt DESC) AS rank
                                                      ,company_name
                                                      ,is_work
                                                  FROM can_career
                                                 GROUP BY c_seq, company_name, is_work, join_dt) AS A
                                         WHERE rank = 1) AS CC
                                   ON C.c_seq = CC.c_seq
                      LEFT OUTER JOIN (SELECT DISTINCT 
                                              CJ.c_seq
                                             ,REPLACE(STUFF((SELECT DISTINCT
                                                                    '/' + B.code_name2
                                                               FROM   can_job AS A INNER JOIN code_job_dtl AS B
                                                                                                ON A.code2 = B.code2
                                                               WHERE  A.c_seq = CJ.c_seq
                                                               FOR XML PATH('')),1,1,''),  '&amp;', '&') AS code_name2
                                         FROM can_job AS CJ) AS CJ
                                   ON C.c_seq = CJ.c_seq
                      LEFT OUTER JOIN (SELECT DISTINCT 
                                              CB.c_seq
                                             ,REPLACE(STUFF((SELECT DISTINCT
                                                                    '/' + B.code_name2
                                                               FROM   can_business AS A INNER JOIN code_business_dtl AS B
                                                                                                     ON A.code2 = B.code2
                                                               WHERE  A.c_seq = CB.c_seq
                                                               FOR XML PATH('')),1,1,''),  '&amp;', '&') AS code_name2
                                         FROM can_business AS CB) AS CB
                                   ON C.c_seq = CB.c_seq
                      LEFT OUTER JOIN (SELECT *
                                         FROM (SELECT c_seq
                                                     ,DENSE_RANK() OVER(PARTITION BY c_seq ORDER BY c_seq, cr_seq DESC) AS rank
                                                     ,file_dir
                                                     ,file_extension
                                                 FROM can_resume
                                                WHERE file_type = 'K') AS A
                                        WHERE A.rank = 1) AS CK
                                   ON C.c_seq = CK.c_seq
                      LEFT OUTER JOIN (SELECT *
                                         FROM (SELECT c_seq
                                                     ,DENSE_RANK() OVER(PARTITION BY c_seq ORDER BY c_seq, cr_seq DESC) AS rank
                                                     ,file_dir
                                                     ,file_extension
                                                 FROM can_resume
                                                WHERE file_type = 'E') AS A
                                        WHERE A.rank = 1) AS CE
                                   ON C.c_seq = CE.c_seq
                      LEFT OUTER JOIN can_interest AS CI
                                   ON C.c_seq = CI.c_seq
                                  AND CI.uv_seq = @user_seq
 WHERE 1 = 1 ";
          if (!string.IsNullOrWhiteSpace(search.name))
            selectQuery += " AND (C.kor_name LIKE '%' + @name + '%' OR C.eng_name LIKE '%' + @name + '%') ";

          if (!string.IsNullOrWhiteSpace(search.startBirth) && !string.IsNullOrWhiteSpace(search.endBirth))
            selectQuery += " AND C.birth_date BETWEEN CONVERT(DATETIME, @startDt + ' 00:00:00') AND CONVERT(DATETIME, @endDt + ' 23:59:59') ";

          if (!string.IsNullOrWhiteSpace(search.phone))
            selectQuery += " AND (C.phone LIKE '%' + @phone + '%' OR C.cell_phone LIKE '%' + @phone + '%') ";

          if (!string.IsNullOrWhiteSpace(search.email))
            selectQuery += " AND (C.email1 LIKE '%' + @email + '%' OR C.email2 LIKE '%' + @email + '%') ";

          if (search.gender != 0)
            selectQuery += " AND (C.gender = @gender) ";

          if (!string.IsNullOrWhiteSpace(search.school))
            selectQuery += " AND CS.school_name LIKE '%' + @school + '%' ";

          if (search.foreign_school)
            selectQuery += " AND C.c_seq IN (SELECT c_seq FROM can_school WHERE is_foreign_school = 1) ";

          if (search.final_edu.Count > 0 && search.final_edu.Count < 6)
          {
            selectQuery += " AND  CS.final_edu IN @list_final_edu ";
          }

          if (!string.IsNullOrWhiteSpace(search.category1_name))
            selectQuery += " AND CS.category1_name LIKE '%' + @category1_name + '%' ";

          if (!string.IsNullOrWhiteSpace(search.major))
            selectQuery += " AND CS.major_name LIKE '%' + @major + '%' ";

          if (!string.IsNullOrWhiteSpace(search.company) && search.final_company)
            selectQuery += " AND CC.company_name LIKE '%' + @company + '%' ";
          else if (!string.IsNullOrWhiteSpace(search.company) && !search.final_company)
            selectQuery += " AND C.c_seq IN (SELECT c_seq FROM can_career WHERE company_name  LIKE '%' + @company + '%') ";
          /*
          if (!string.IsNullOrWhiteSpace(search.job))
              selectQuery += " AND CJ.code_name2 LIKE '%' + @job + '%' ";

          if (!string.IsNullOrWhiteSpace(search.business))
              selectQuery += " AND CB.code_name2 LIKE '%' + @business + '%' ";
          */
          if (!string.IsNullOrWhiteSpace(search.isWork))
            selectQuery += " AND CC.is_work = CASE WHEN @isWork <> '' THEN @isWork ELSE CC.is_work END ";

          selectQuery += @" ORDER BY C.c_seq DESC 
OFFSET @currentPage ROWS FETCH NEXT @pageSize ROWS ONLY ";

          string countQuery = @" 
SELECT COUNT(0)
FROM candidate AS C LEFT OUTER JOIN (SELECT c_seq
                                             ,final_edu
                                             ,school_name
                                             ,category1_name
                                             ,major_name
                                         FROM (SELECT c_seq
                                                     ,CS.gubun as final_edu
                                                     ,major_name
                                                     ,DENSE_RANK() OVER(PARTITION BY c_seq ORDER BY c_seq, graduate_year DESC) AS rank
                                                     ,CASE WHEN ISNULL(SC.campus_name, '') <> '' THEN SC.school_name + '(' + SC.campus_name + ')' ELSE SC.school_name END AS school_name
                                                     ,CS.category1_name
                                                     ,CS.graduate_year
                                                 FROM can_school AS CS INNER JOIN code_school AS SC
                                                                               ON CS.sc_seq = SC.sc_seq
                                                GROUP BY CS.c_seq, CS.gubun, SC.school_name, SC.campus_name, CS.category1_name, CS.major_name, graduate_year) AS A
                                        WHERE rank = 1) AS CS 
                                   ON C.c_seq = CS.c_seq
                       LEFT OUTER JOIN (SELECT c_seq
                                              ,company_name
                                              ,is_work
                                          FROM (SELECT c_seq
                                                      ,DENSE_RANK() OVER(PARTITION BY c_seq ORDER BY c_seq, join_dt DESC) AS rank
                                                      ,company_name
                                                      ,is_work
                                                  FROM can_career
                                                 GROUP BY c_seq, company_name, is_work, join_dt) AS A
                                         WHERE rank = 1) AS CC
                                   ON C.c_seq = CC.c_seq
                      LEFT OUTER JOIN (SELECT DISTINCT 
                                              CJ.c_seq
                                             ,REPLACE(STUFF((SELECT DISTINCT
                                                                    '/' + B.code_name2
                                                               FROM   can_job AS A INNER JOIN code_job_dtl AS B
                                                                                                ON A.code2 = B.code2
                                                               WHERE  A.c_seq = CJ.c_seq
                                                               FOR XML PATH('')),1,1,''),  '&amp;', '&') AS code_name2
                                         FROM can_job AS CJ) AS CJ
                                   ON C.c_seq = CJ.c_seq
                      LEFT OUTER JOIN (SELECT DISTINCT 
                                              CB.c_seq
                                             ,REPLACE(STUFF((SELECT DISTINCT
                                                                    '/' + B.code_name2
                                                               FROM   can_business AS A INNER JOIN code_business_dtl AS B
                                                                                                     ON A.code2 = B.code2
                                                               WHERE  A.c_seq = CB.c_seq
                                                               FOR XML PATH('')),1,1,''),  '&amp;', '&') AS code_name2
                                         FROM can_business AS CB) AS CB
                                   ON C.c_seq = CB.c_seq
                      LEFT OUTER JOIN (SELECT *
                                         FROM (SELECT c_seq
                                                     ,DENSE_RANK() OVER(PARTITION BY c_seq ORDER BY c_seq, cr_seq DESC) AS rank
                                                     ,file_dir
                                                     ,file_extension
                                                 FROM can_resume
                                                WHERE file_type = 'K') AS A
                                        WHERE A.rank = 1) AS CK
                                   ON C.c_seq = CK.c_seq
                      LEFT OUTER JOIN (SELECT *
                                         FROM (SELECT c_seq
                                                     ,DENSE_RANK() OVER(PARTITION BY c_seq ORDER BY c_seq, cr_seq DESC) AS rank
                                                     ,file_dir
                                                     ,file_extension
                                                 FROM can_resume
                                                WHERE file_type = 'E') AS A
                                        WHERE A.rank = 1) AS CE
                                   ON C.c_seq = CE.c_seq
                      LEFT OUTER JOIN can_interest AS CI
                                   ON C.c_seq = CI.c_seq
                                  AND CI.uv_seq = @user_seq
 WHERE 1 = 1 ";

          if (!string.IsNullOrWhiteSpace(search.name))
            countQuery += " AND (C.kor_name LIKE '%' + @name + '%' OR C.eng_name LIKE '%' + @name + '%') ";

          if (!string.IsNullOrWhiteSpace(search.startBirth) && !string.IsNullOrWhiteSpace(search.endBirth))
            countQuery += " AND C.birth_date BETWEEN CONVERT(DATETIME, @startDt + ' 00:00:00') AND CONVERT(DATETIME, @endDt + ' 23:59:59') ";

          if (!string.IsNullOrWhiteSpace(search.phone))
            countQuery += " AND (C.phone LIKE '%' + @phone + '%' OR C.cell_phone LIKE '%' + @phone + '%') ";

          if (!string.IsNullOrWhiteSpace(search.email))
            countQuery += " AND (C.email1 LIKE '%' + @email + '%' OR C.email2 LIKE '%' + @email + '%') ";

          if (search.gender != 0)
            countQuery += " AND (C.gender = @gender) ";

          if (!string.IsNullOrWhiteSpace(search.school))
            countQuery += " AND CS.school_name LIKE '%' + @school + '%' ";

          if (search.foreign_school)
            countQuery += " AND C.c_seq IN (SELECT c_seq FROM can_school WHERE is_foreign_school = 1) ";

          if (search.final_edu.Count > 0 && search.final_edu.Count < 6)
          {
            countQuery += " AND  CS.final_edu IN @list_final_edu ";
          }

          if (!string.IsNullOrWhiteSpace(search.category1_name))
            countQuery += " AND CS.category1_name LIKE '%' + @category1_name + '%' ";

          if (!string.IsNullOrWhiteSpace(search.major))
            countQuery += " AND CS.major_name LIKE '%' + @major + '%' ";

          if (!string.IsNullOrWhiteSpace(search.company) && search.final_company)
            countQuery += " AND CC.company_name LIKE '%' + @company + '%' ";
          else if (!string.IsNullOrWhiteSpace(search.company) && !search.final_company)
            countQuery += " AND C.c_seq IN (SELECT c_seq FROM can_career WHERE company_name  LIKE '%' + @company + '%') ";
          /*
          if (!string.IsNullOrWhiteSpace(search.job))
              countQuery += " AND CJ.code_name2 LIKE '%' + @job + '%' ";

          if (!string.IsNullOrWhiteSpace(search.business))
              countQuery += " AND CB.code_name2 LIKE '%' + @business + '%' ";
          */
          if (!string.IsNullOrWhiteSpace(search.isWork))
            countQuery += " AND CC.is_work = CASE WHEN @isWork <> '' THEN @isWork ELSE CC.is_work END ";

          DynamicParameters param = new DynamicParameters();
          param.Add("@name", search.name, DbType.String);
          param.Add("@startDt", search.startBirth, DbType.String);
          param.Add("@endDt", search.endBirth, DbType.String);
          param.Add("@phone", search.phone, DbType.String);
          param.Add("@email", search.email, DbType.String);
          param.Add("@gender", search.gender, DbType.Int32);
          param.Add("@school", search.school, DbType.String);
          param.Add("@category1_name", search.category1_name, DbType.String);
          param.Add("@list_final_edu", search.final_edu);
          param.Add("@major", search.major, DbType.String);
          param.Add("@company", search.company, DbType.String);
          param.Add("@job", search.job, DbType.String);
          param.Add("@business", search.business, DbType.String);
          param.Add("@isWork", search.isWork, DbType.String);
          param.Add("@user_seq", AppIdentity.user_seq, DbType.Int32);
          param.Add("@currentPage", skip, DbType.Int32);
          param.Add("@pageSize", count, DbType.Int32);


          DynamicParameters param2 = new DynamicParameters();
          param.Add("@name", search.name, DbType.String);
          param2.Add("@startDt", search.startBirth, DbType.String);
          param2.Add("@endDt", search.endBirth, DbType.String);
          param2.Add("@phone", search.phone, DbType.String);
          param2.Add("@email", search.email, DbType.String);
          param2.Add("@gender", search.gender, DbType.Int32);
          param2.Add("@school", search.school, DbType.String);
          param2.Add("@category1_name", search.category1_name, DbType.String);
          param2.Add("@list_final_edu", search.final_edu);
          param2.Add("@major", search.major, DbType.String);
          param2.Add("@company", search.company, DbType.String);
          param2.Add("@job", search.job, DbType.String);
          param2.Add("@business", search.business, DbType.String);
          param2.Add("@isWork", search.isWork, DbType.String);
          param2.Add("@user_seq", AppIdentity.user_seq, DbType.Int32);
          param2.Add("@currentPage", skip, DbType.Int32);
          param2.Add("@pageSize", count, DbType.Int32);

          var ret = con.Query<candidate>(selectQuery, param);
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

    public List<view_can_activity> SelectViewCanActivityList(CandidateHistorySearchModel search, int skip, int count, out int totalCount)
    {
      try
      {
        List<view_can_activity> list = new List<view_can_activity>();

        using (IDbConnection con = new SqlConnection(base.BaseConnectionString))
        {
          con.Open();
          string selectQuery = @" SELECT VCA.*
                                                  , CONVERT(VARCHAR(10), VCA.create_dt, 121) as create_dt_str
                                                  , CL.kor_name AS client_name
                                                  , C.kor_name AS candidate_name
                                                  , UV.name AS create_name
                                                  , P.title As pjt_title
                                            FROM view_can_activity AS VCA LEFT JOIN project AS P
                                                                          ON VCA.p_seq = P.p_seq
                                                                          LEFT JOIN client AS CL
                                                                          ON P.c_seq = CL.c_seq
                                                                          LEFT JOIN candidate AS C
                                                                          ON VCA.c_seq = C.c_seq
                                                                          INNER JOIN uv_user AS UV
                                                                          ON VCA.create_seq = UV.uv_seq
                                            WHERE 1 = 1 ";

          if (!string.IsNullOrWhiteSpace(search.search_txt))
          {
            selectQuery += @" AND (CL.kor_name LIKE '%' + @search_txt + '%'  
                              OR CL.kor_name LIKE '%' + @search_txt + '%' 
                              OR C.kor_name LIKE '%' + @search_txt + '%' 
                              OR UV.name LIKE '%' + @search_txt + '%' 
                              OR P.title LIKE '%' + @search_txt + '%' 
                              OR VCA.memo LIKE '%' + @search_txt + '%') ";
          }


          if (search.is_my_history)
            selectQuery += " AND VCA.create_seq = @user_seq ";

          if (!string.IsNullOrWhiteSpace(search.start_dt) && !string.IsNullOrWhiteSpace(search.end_dt))
            selectQuery += " AND VCA.create_dt BETWEEN CONVERT(DATETIME, @start_dt + ' 00:00:00') AND CONVERT(DATETIME, @end_dt + ' 23:59:59') ";

          selectQuery += @" ORDER BY VCA.create_dt DESC, VCA.table_type, VCA.p_seq, VCA.c_seq
OFFSET @currentPage ROWS FETCH NEXT @pageSize ROWS ONLY ";

          string countQuery = @" SELECT COUNT(VCA.c_seq)
                                           FROM view_can_activity AS VCA LEFT JOIN project AS P
                                                                          ON VCA.p_seq = P.p_seq
                                                                          LEFT JOIN client AS CL
                                                                          ON P.c_seq = CL.c_seq
                                                                          LEFT JOIN candidate AS C
                                                                          ON VCA.c_seq = C.c_seq
                                                                          INNER JOIN uv_user AS UV
                                                                          ON VCA.create_seq = UV.uv_seq
                                           WHERE 1 = 1 ";

          if (!string.IsNullOrWhiteSpace(search.search_txt))
          {
            countQuery += @" AND (CL.kor_name LIKE '%' + @search_txt + '%'  
                              OR CL.kor_name LIKE '%' + @search_txt + '%' 
                              OR C.kor_name LIKE '%' + @search_txt + '%' 
                              OR UV.name LIKE '%' + @search_txt + '%' 
                              OR P.title LIKE '%' + @search_txt + '%' 
                              OR VCA.memo LIKE '%' + @search_txt + '%') ";
          }

          if (search.is_my_history)
            countQuery += " AND VCA.create_seq = @user_seq ";

          if (!string.IsNullOrWhiteSpace(search.start_dt) && !string.IsNullOrWhiteSpace(search.end_dt))
            countQuery += " AND VCA.create_dt BETWEEN CONVERT(DATETIME, @start_dt + ' 00:00:00') AND CONVERT(DATETIME, @end_dt + ' 23:59:59') ";

          DynamicParameters param = new DynamicParameters();
          param.Add("@search_txt", search.search_txt, DbType.String);
          param.Add("@start_dt", search.start_dt, DbType.String);
          param.Add("@end_dt", search.end_dt, DbType.String);
          param.Add("@user_seq", AppIdentity.user_seq, DbType.Int32);
          param.Add("@currentPage", skip, DbType.Int32);
          param.Add("@pageSize", count, DbType.Int32);


          DynamicParameters param2 = new DynamicParameters();
          param2.Add("@search_txt", search.search_txt, DbType.String);
          param2.Add("@startDt", search.start_dt, DbType.String);
          param2.Add("@endDt", search.end_dt, DbType.String);
          param2.Add("@user_seq", AppIdentity.user_seq, DbType.Int32);

          var ret = con.Query<view_can_activity>(selectQuery, param);
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

    public List<tempsaved_candidate> SelectTempCandidateList(TempCandidateSearchModel search, int skip, int count, out int totalCount)
    {
      try
      {
        List<tempsaved_candidate> list = new List<tempsaved_candidate>();

        using (IDbConnection con = new SqlConnection(base.BaseConnectionString))
        {
          con.Open();
          string selectQuery = @" SELECT A.*
                                            FROM tempsaved_candidate  AS A LEFT JOIN uv_user AS UV
                                                                          ON A.create_seq = UV.uv_seq
                                            WHERE 1 = 1 ";

          if (!string.IsNullOrWhiteSpace(search.search_txt))
          {
            selectQuery += @" AND (A.kor_name LIKE '%' + @search_txt + '%'  ) ";
          }


          if (search.uv_seq != 0 && search.uv_seq != null)
            selectQuery += " AND A.create_seq = @user_seq ";

          selectQuery += @" ORDER BY A.c_seq DESC
OFFSET @currentPage ROWS FETCH NEXT @pageSize ROWS ONLY ";

          string countQuery = @" SELECT COUNT(A.c_seq)
                                           FROM tempsaved_candidate  AS A LEFT JOIN uv_user AS UV
                                                                          ON A.create_seq = UV.uv_seq
                                            WHERE 1 = 1 ";


          if (!string.IsNullOrWhiteSpace(search.search_txt))
          {
            countQuery += @" AND (A.kor_name LIKE '%' + @search_txt + '%'  ) ";
          }

          if (search.uv_seq != 0 && search.uv_seq != null)
            countQuery += " AND A.create_seq = @user_seq ";


          DynamicParameters param = new DynamicParameters();
          param.Add("@search_txt", search.search_txt, DbType.String);
          param.Add("@user_seq", AppIdentity.user_seq, DbType.Int32);
          param.Add("@currentPage", skip, DbType.Int32);
          param.Add("@pageSize", count, DbType.Int32);


          DynamicParameters param2 = new DynamicParameters();
          param2.Add("@search_txt", search.search_txt, DbType.String);
          param2.Add("@user_seq", AppIdentity.user_seq, DbType.Int32);

          var ret = con.Query<tempsaved_candidate>(selectQuery, param);
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
    /// 후보자 학력 정보 리스트
    /// </summary>
    /// <param name="c_seq"></param>
    /// <returns></returns>
    public async Task<List<can_school>> SelectCanSchoolListAsync(int c_seq)
    {
      try
      {
        List<can_school> list = new List<can_school>();

        using (IDbConnection con = new SqlConnection(base.BaseConnectionString))
        {
          con.Open();

          string selectQuery = @"
SELECT A.*
      ,B.school_name AS schoolName
      ,B.campus_name AS campusName
FROM can_school AS A LEFT JOIN code_school AS B
                               ON A.sc_seq = B.sc_seq 
WHERE c_seq = @c_seq 
ORDER BY A.order_no ASC";

          DynamicParameters param = new DynamicParameters();
          param.Add("@c_seq", c_seq, DbType.Int32);

          var ret = await con.QueryAsync<can_school>(selectQuery, param);

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
    /// 후보자 학력 정보 리스트
    /// </summary>
    /// <param name="c_seq"></param>
    /// <returns></returns>
    public async Task<List<director_school>> SelectDirectorSchoolListAsync(int d_seq)
    {
      try
      {
        List<director_school> list = new List<director_school>();

        using (IDbConnection con = new SqlConnection(base.BaseConnectionString))
        {
          con.Open();

          string selectQuery = @"
SELECT A.*
      ,A.school_name AS schoolName      
      ,CASE A.gubun WHEN 3 THEN '학사' WHEN 4 THEN '석사' WHEN 5 THEN '박사' ELSE '' END as gubun_str
FROM director_school AS A 
WHERE d_seq = @d_seq 
ORDER BY A.order_no ASC";

          DynamicParameters param = new DynamicParameters();
          param.Add("@d_seq", d_seq, DbType.Int32);

          var ret = await con.QueryAsync<director_school>(selectQuery, param);

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
    /// 후보자 학력 정보 리스트
    /// </summary>
    /// <param name="c_seq"></param>
    /// <returns></returns>
    public async Task<List<can_school>> TempSelectCanSchoolListAsync(int t_seq)
    {
      try
      {
        List<can_school> list = new List<can_school>();

        using (IDbConnection con = new SqlConnection(base.BaseConnectionString))
        {
          con.Open();

          string selectQuery = @"
SELECT A.*
      ,A.school_name AS schoolName      
FROM tempsaved_can_school AS A 
WHERE c_seq = @c_seq 
ORDER BY gubun desc, sch1 desc, graduate_year desc";

          DynamicParameters param = new DynamicParameters();
          param.Add("@c_seq", t_seq, DbType.Int32);

          var ret = await con.QueryAsync<can_school>(selectQuery, param);

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
    /// 후보자 경력 정보 리스트
    /// </summary>
    /// <param name="c_seq"></param>
    /// <returns></returns>
    public async Task<List<can_career>> SelectCanCareerListAsync(int c_seq, string order_option = "")
    {
      try
      {
        List<can_career> list = new List<can_career>();

        using (IDbConnection con = new SqlConnection(base.BaseConnectionString))
        {
          con.Open();

          string selectQuery = @" SELECT A.*
       ,B.r_name
       ,C.p_name
   FROM can_career AS A LEFT JOIN code_rank AS B
                                ON A.r_code = B.r_code
                        LEFT JOIN code_position AS C
                                ON A.p_code = C.p_code 
  WHERE A.c_seq = @c_seq 
order by A.order_no, A.cc_seq DESC " + order_option;

          DynamicParameters param = new DynamicParameters();
          param.Add("@c_seq", c_seq, DbType.Int32);

          var ret = await con.QueryAsync<can_career>(selectQuery, param);

          list = ret.ToList();
          /*
          foreach (var data in list)
          {
            var list2 = new List<can_career_job>();
            string selectQuery2 = @" SELECT A.*
      ,B.code1
      ,B.code2
    FROM can_career_job AS A LEFT JOIN code_job3 AS B
                                   ON A.job_code3 = B.code3
    WHERE A.cc_seq = @cc_seq ";
            DynamicParameters param2 = new DynamicParameters();
            param2.Add("@cc_seq", data.cc_seq, DbType.Int32);

            var ret2 = await con.QueryAsync<can_career_job>(selectQuery2, param2);

            list2 = ret2.ToList();

            data.jobList = list2;
          }
          */
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
    /// 후보자 경력 정보 리스트
    /// </summary>
    /// <param name="c_seq"></param>
    /// <returns></returns>
    public async Task<List<director_career>> SelectDirectorCareerListAsync(int d_seq, int pos_type, string order_option = "")
    {
      try
      {
        List<director_career> list = new List<director_career>();

        using (IDbConnection con = new SqlConnection(base.BaseConnectionString))
        {
          con.Open();

          string selectQuery = @" SELECT A.*
   FROM director_career AS A 
  WHERE A.d_seq = @d_seq  ";
          if (pos_type < 0)
          {
            selectQuery += @" AND A.pos_type not in (2) ";
          }
          else if (pos_type > 0)
          {
            selectQuery += @" AND A.pos_type = @pos_type ";
          }
          selectQuery += @" order by A.order_no" + order_option;

          DynamicParameters param = new DynamicParameters();
          param.Add("@d_seq", d_seq, DbType.Int32);
          param.Add("@pos_type", pos_type, DbType.Int32);

          var ret = await con.QueryAsync<director_career>(selectQuery, param);

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
    /// 후보자 경력 정보 리스트
    /// </summary>
    /// <param name="c_seq"></param>
    /// <returns></returns>
    public async Task<List<can_career>> TempSelectCanCareerListAsync(int t_seq)
    {
      try
      {
        List<can_career> list = new List<can_career>();

        using (IDbConnection con = new SqlConnection(base.BaseConnectionString))
        {
          con.Open();

          string selectQuery = @" SELECT A.*
       ,B.r_name
       ,C.p_name
   FROM tempsaved_can_career AS A LEFT JOIN code_rank AS B
                                ON A.r_code = B.r_code
                        LEFT JOIN code_position AS C
                                ON A.p_code = C.p_code 
  WHERE A.c_seq = @c_seq 
order by is_work desc, join_dt desc, leave_dt desc";

          DynamicParameters param = new DynamicParameters();
          param.Add("@c_seq", t_seq, DbType.Int32);

          var ret = await con.QueryAsync<can_career>(selectQuery, param);

          list = ret.ToList();

          foreach (var data in list)
          {
            var list2 = new List<can_career_job>();
            string selectQuery2 = @" SELECT A.*
      ,B.code1
      ,B.code2
  FROM tempsaved_can_career_job AS A LEFT JOIN code_job3 AS B
                                   ON A.job_code3 = B.code3
 WHERE A.cc_seq = @cc_seq ";
            DynamicParameters param2 = new DynamicParameters();
            param2.Add("@cc_seq", data.cc_seq, DbType.Int32);

            var ret2 = await con.QueryAsync<can_career_job>(selectQuery2, param2);

            list2 = ret2.ToList();

            data.jobList = list2;
          }

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
    /// 후보자 희망근무지 리스트
    /// </summary>
    /// <param name="c_seq"></param>
    /// <returns></returns>
    public async Task<List<can_place>> SelectCanPlaceListAsync(int c_seq)
    {
      try
      {
        List<can_place> list = new List<can_place>();

        using (IDbConnection con = new SqlConnection(base.BaseConnectionString))
        {
          con.Open();

          string selectQuery = @" SELECT * FROM can_place WHERE c_seq = @c_seq ";

          DynamicParameters param = new DynamicParameters();
          param.Add("@c_seq", c_seq, DbType.Int32);

          var ret = await con.QueryAsync<can_place>(selectQuery, param);

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

    public async Task<List<can_place>> TempSelectCanPlaceListAsync(int t_seq)
    {
      try
      {
        List<can_place> list = new List<can_place>();

        using (IDbConnection con = new SqlConnection(base.BaseConnectionString))
        {
          con.Open();

          string selectQuery = @" SELECT * FROM tempsaved_can_place WHERE c_seq = @c_seq ";

          DynamicParameters param = new DynamicParameters();
          param.Add("@c_seq", t_seq, DbType.Int32);

          var ret = await con.QueryAsync<can_place>(selectQuery, param);

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
    /// 후보자 직무정보 리스트
    /// </summary>
    /// <param name="c_seq"></param>
    /// <returns></returns>
    public async Task<List<can_job>> SelectCanJobCodeListAsync(int c_seq)
    {
      try
      {
        List<can_job> list = new List<can_job>();

        using (IDbConnection con = new SqlConnection(base.BaseConnectionString))
        {
          con.Open();

          string selectQuery = @" 
SELECT A.*
      ,B.code_name1
      ,C.code_name2
FROM can_job AS A INNER JOIN code_job_mst AS B
                                 ON A.code1 = B.code1
                    LEFT OUTER JOIN code_job_dtl AS C
                                 ON A.code1 = C.code1
                                AND A.code2 = C.code2
WHERE c_seq = @c_seq 
ORDER BY A.code1, A.code2
";

          DynamicParameters param = new DynamicParameters();
          param.Add("@c_seq", c_seq, DbType.Int32);

          var ret = await con.QueryAsync<can_job>(selectQuery, param);

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
    /// 후보자 직무정보 리스트
    /// </summary>
    /// <param name="c_seq"></param>
    /// <returns></returns>
    public async Task<List<director_job>> SelectDirectorJobCodeListAsync(int d_seq)
    {
      try
      {
        List<director_job> list = new List<director_job>();

        using (IDbConnection con = new SqlConnection(base.BaseConnectionString))
        {
          con.Open();

          string selectQuery = @" 
SELECT A.*
      ,B.code_name1
      ,C.code_name2
FROM director_job AS A INNER JOIN code_job_mst AS B
                                 ON A.code1 = B.code1
                    LEFT OUTER JOIN code_job_dtl AS C
                                 ON A.code1 = C.code1
                                AND A.code2 = C.code2
WHERE d_seq = @d_seq 
ORDER BY A.code1, A.code2
";

          DynamicParameters param = new DynamicParameters();
          param.Add("@d_seq", d_seq, DbType.Int32);

          var ret = await con.QueryAsync<director_job>(selectQuery, param);

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

    public async Task<List<can_job>> TempSelectCanJobCodeListAsync(int t_seq)
    {
      try
      {
        List<can_job> list = new List<can_job>();

        using (IDbConnection con = new SqlConnection(base.BaseConnectionString))
        {
          con.Open();

          string selectQuery = @"
SELECT A.*
      ,B.code_name1
      ,C.code_name2
FROM tempsaved_can_job AS A INNER JOIN code_job_mst AS B
                                 ON A.code1 = B.code1
                    LEFT OUTER JOIN code_job_dtl AS C
                                 ON A.code1 = C.code1
                                AND A.code2 = C.code2
WHERE c_seq = @c_seq 
ORDER BY A.code1, A.code2
";

          DynamicParameters param = new DynamicParameters();
          param.Add("@c_seq", t_seq, DbType.Int32);

          var ret = await con.QueryAsync<can_job>(selectQuery, param);

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
    /// 후보자 산업정보 리스트
    /// </summary>
    /// <param name="c_seq"></param>
    /// <returns></returns>
    public async Task<List<can_business>> SelectCanBusiCodeListAsync(int c_seq)
    {
      try
      {
        List<can_business> list = new List<can_business>();

        using (IDbConnection con = new SqlConnection(base.BaseConnectionString))
        {
          con.Open();

          string selectQuery = @" SELECT A.*
      ,B.code_name1
      ,C.code_name2
  FROM can_business AS A INNER JOIN code_business_mst AS B
                                 ON A.code1 = B.code1
                    LEFT OUTER JOIN code_business_dtl AS C
                                 ON A.code1 = C.code1
                                AND A.code2 = C.code2
 WHERE c_seq = @c_seq 
 ORDER BY A.code1, A.code2
";

          DynamicParameters param = new DynamicParameters();
          param.Add("@c_seq", c_seq, DbType.Int32);

          var ret = await con.QueryAsync<can_business>(selectQuery, param);

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
    /// 후보자 산업정보 리스트
    /// </summary>
    /// <param name="d_seq"></param>
    /// <returns></returns>
    public async Task<List<director_business>> SelectDirectorBusiCodeListAsync(int d_seq)
    {
      try
      {
        List<director_business> list = new List<director_business>();

        using (IDbConnection con = new SqlConnection(base.BaseConnectionString))
        {
          con.Open();

          string selectQuery = @" SELECT A.*
      ,B.code_name1
      ,C.code_name2
  FROM director_business AS A INNER JOIN code_business_mst AS B
                                 ON A.code1 = B.code1
                    LEFT OUTER JOIN code_business_dtl AS C
                                 ON A.code1 = C.code1
                                AND A.code2 = C.code2
 WHERE d_seq = @d_seq 
 ORDER BY A.code1, A.code2
";

          DynamicParameters param = new DynamicParameters();
          param.Add("@d_seq", d_seq, DbType.Int32);

          var ret = await con.QueryAsync<director_business>(selectQuery, param);

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

    public async Task<List<can_business>> TempSelectCanBusiCodeListAsync(int t_seq)
    {
      try
      {
        List<can_business> list = new List<can_business>();

        using (IDbConnection con = new SqlConnection(base.BaseConnectionString))
        {
          con.Open();

          string selectQuery = @" SELECT A.*
      ,B.code_name1
      ,C.code_name2
 FROM tempsaved_can_business AS A INNER JOIN code_business_mst AS B
                                 ON A.code1 = B.code1
                    LEFT OUTER JOIN code_business_dtl AS C
                                 ON A.code1 = C.code1
                                AND A.code2 = C.code2
 WHERE c_seq = @c_seq 
ORDER BY A.code1, A.code2
 ";

          DynamicParameters param = new DynamicParameters();
          param.Add("@c_seq", t_seq, DbType.Int32);

          var ret = await con.QueryAsync<can_business>(selectQuery, param);

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
    /// 후보자 외국어 능력 리스트
    /// </summary>
    /// <param name="c_seq"></param>
    /// <returns></returns>
    public async Task<List<can_foreign_lan>> SelectCanForeignLanListAsync(int c_seq)
    {
      try
      {
        List<can_foreign_lan> list = new List<can_foreign_lan>();

        using (IDbConnection con = new SqlConnection(base.BaseConnectionString))
        {
          con.Open();

          string selectQuery = @" SELECT * FROM can_foreign_lan WHERE c_seq = @c_seq ";

          DynamicParameters param = new DynamicParameters();
          param.Add("@c_seq", c_seq, DbType.Int32);

          var ret = await con.QueryAsync<can_foreign_lan>(selectQuery, param);

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

    public async Task<List<can_foreign_lan>> TempSelectCanForeignLanListAsync(int t_seq)
    {
      try
      {
        List<can_foreign_lan> list = new List<can_foreign_lan>();

        using (IDbConnection con = new SqlConnection(base.BaseConnectionString))
        {
          con.Open();

          string selectQuery = @" SELECT * FROM tempsaved_can_foreign_lan WHERE c_seq = @c_seq ";

          DynamicParameters param = new DynamicParameters();
          param.Add("@c_seq", t_seq, DbType.Int32);

          var ret = await con.QueryAsync<can_foreign_lan>(selectQuery, param);

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
    /// 후보자 활동내역 리스트
    /// </summary>
    /// <param name="c_seq"></param>
    /// <returns></returns>
    public List<view_can_activity> SelectCanActivityList(int c_seq, int skip, int count, out int totalCount)
    {
      try
      {
        List<view_can_activity> list = new List<view_can_activity>();

        using (IDbConnection con = new SqlConnection(base.BaseConnectionString))
        {
          con.Open();

          string selectQuery = @" SELECT VCA.*
                                                  , C.kor_name AS client_name
                                                  , UV.name AS create_name
                                                  , P.title As pjt_title
                                                  , P.position_str
                                                  , CP.p_name as position_nm
                                                  , (select count(*) from pjt_recandidate_history where p_seq = VCA.p_seq AND c_seq = VCA.c_seq) as pjt_recan_his_cnt
                                                  , P.pjt_status
                                            FROM view_can_activity AS VCA LEFT JOIN project AS P
                                                                          ON VCA.p_seq = P.p_seq
                                                                          LEFT JOIN client AS C
                                                                          ON P.c_seq = C.c_seq
                                                                          INNER JOIN uv_user AS UV
                                                                          ON VCA.create_seq = UV.uv_seq
                                                                          LEFT JOIN code_position AS CP
                                                                          ON P.position_seq = CP.p_code
                                            WHERE VCA.c_seq = @c_seq 
                                            ORDER BY VCA.create_dt DESC, VCA.seq DESC
                                            OFFSET @currentPage ROWS FETCH NEXT @pageSize ROWS ONLY ";

          string countQuery = @" SELECT COUNT(VCA.c_seq)
                                           FROM view_can_activity AS VCA LEFT JOIN project AS P
                                                                          ON VCA.p_seq = P.p_seq
                                                                          LEFT JOIN client AS C
                                                                          ON P.c_seq = C.c_seq
                                                                          INNER JOIN uv_user AS UV
                                                                          ON VCA.create_seq = UV.uv_seq
                                             WHERE VCA.c_seq = @c_seq  ";

          DynamicParameters param = new DynamicParameters();
          param.Add("@c_seq", c_seq, DbType.Int32);
          param.Add("@currentPage", skip, DbType.Int32);
          param.Add("@pageSize", count, DbType.Int32);

          DynamicParameters param2 = new DynamicParameters();
          param2.Add("@c_seq", c_seq, DbType.Int32);

          var ret = con.Query<view_can_activity>(selectQuery, param);
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

    public async Task<List<director_activity>> SelectDirectorActivityList(int d_seq, int type = 0)
    {
      try
      {
        List<director_activity> list = new List<director_activity>();

        using (IDbConnection con = new SqlConnection(base.BaseConnectionString))
        {
          con.Open();

          string selectQuery = @" SELECT VCA.*,
                                         UV.name as create_name,
                                         CASE memo_type WHEN 1 THEN '뉴스' WHEN 2 THEN '메모' WHEN 3 THEN '자료' ELSE '기타' END as type_str
                                  FROM director_activity AS VCA LEFT JOIN uv_user AS UV
                                                                ON VCA.create_seq = UV.uv_seq
                                  WHERE VCA.d_seq = @d_seq ";
          if (type > 0)
          {
            selectQuery += @" AND memo_type = @type ";
          }
          selectQuery += @" ORDER BY VCA.da_seq DESC ";

          DynamicParameters param = new DynamicParameters();
          param.Add("@d_seq", d_seq, DbType.Int32);
          param.Add("@type", type, DbType.String);

          var ret = con.Query<director_activity>(selectQuery, param);

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
    /// 후보자 메모 리스트
    /// </summary>
    /// <param name="c_seq"></param>
    /// <returns></returns>
    public List<can_memo> SelectCanMemoList(int c_seq, int skip, int count, out int totalCount)
    {
      try
      {
        List<can_memo> list = new List<can_memo>();

        using (IDbConnection con = new SqlConnection(base.BaseConnectionString))
        {
          con.Open();

          string selectQuery = @" SELECT CM.*
                                                  ,UV.name AS uv_name
                                              FROM can_memo AS CM INNER JOIN uv_user AS UV
                                                                          ON CM.create_seq = UV.uv_seq
                                             WHERE CM.c_seq = @c_seq 
                                             ORDER BY CM.memo_dt DESC 
                                            OFFSET @currentPage ROWS FETCH NEXT @pageSize ROWS ONLY ";

          string countQuery = @" SELECT COUNT(CM.cm_seq) 
                                             FROM can_memo AS CM INNER JOIN uv_user AS UV
                                                                         ON CM.create_seq = UV.uv_seq
                                            WHERE CM.c_seq = @c_seq ";

          DynamicParameters param = new DynamicParameters();
          param.Add("@c_seq", c_seq, DbType.Int32);
          param.Add("@currentPage", skip, DbType.Int32);
          param.Add("@pageSize", count, DbType.Int32);

          DynamicParameters param2 = new DynamicParameters();
          param2.Add("@c_seq", c_seq, DbType.Int32);

          var ret = con.Query<can_memo>(selectQuery, param);
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
    /// 후보자 인터뷰 리스트
    /// </summary>
    /// <param name="c_seq"></param>
    /// <returns></returns>
    public List<can_interview_sheet> SelectCanInterviewList(int c_seq, int skip, int count, out int totalCount)
    {
      try
      {
        List<can_interview_sheet> list = new List<can_interview_sheet>();

        using (IDbConnection con = new SqlConnection(base.BaseConnectionString))
        {
          con.Open();

          string selectQuery = @" SELECT CI.*
                                                  ,UV.name AS uv_name
                                              FROM can_interview_sheet AS CI INNER JOIN uv_user AS UV
                                                                                     ON CI.create_seq = UV.uv_seq
                                             WHERE CI.c_seq = @c_seq 
                                             ORDER BY CI.interview_dt DESC 
                                            OFFSET @currentPage ROWS FETCH NEXT @pageSize ROWS ONLY ";

          string countQuery = @" SELECT COUNT(CI.cis_seq) 
                                             FROM can_interview_sheet AS CI INNER JOIN uv_user AS UV
                                                                                    ON CI.create_seq = UV.uv_seq
                                            WHERE CI.c_seq = @c_seq ";

          DynamicParameters param = new DynamicParameters();
          param.Add("@c_seq", c_seq, DbType.Int32);
          param.Add("@currentPage", skip, DbType.Int32);
          param.Add("@pageSize", count, DbType.Int32);

          DynamicParameters param2 = new DynamicParameters();
          param2.Add("@c_seq", c_seq, DbType.Int32);

          var ret = con.Query<can_interview_sheet>(selectQuery, param);
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
    /// 후보자 최종 업로드 이력서
    /// </summary>
    /// <param name="c_seq"></param>
    /// <param name="file_type"></param>
    /// <returns></returns>
    public async Task<can_resume> SelectLaseCanResume(int c_seq, string file_type)
    {
      try
      {
        using (IDbConnection con = new SqlConnection(base.BaseConnectionString))
        {
          con.Open();

          string selectQuery = @" SELECT TOP 1 * 
     FROM can_resume 
    WHERE file_type = @file_type
      AND c_seq = @c_seq
    ORDER BY cr_seq DESC ";

          DynamicParameters param = new DynamicParameters();
          param.Add("@c_seq", c_seq, DbType.Int32);
          param.Add("@file_type", file_type, DbType.String);

          var ret = await con.QueryAsync<can_resume>(selectQuery, param);

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
    /// 후보자 이력서 리스트
    /// </summary>
    /// <param name="c_seq"></param>
    /// <returns></returns>
    public async Task<List<can_resume>> SelectCanResumeListAsync(int c_seq, int show_remove = 0)
    {
      try
      {
        List<can_resume> list = new List<can_resume>();

        using (IDbConnection con = new SqlConnection(base.BaseConnectionString))
        {
          con.Open();

          string selectQuery = @" 
SELECT A.*
, C.name as create_name
, R.name as remove_name
FROM can_resume A LEFT JOIN uv_user C 
                  ON A.create_user = C.uv_seq
                  LEFT JOIN uv_user R 
                  ON A.remove_user = R.uv_seq
WHERE A.c_seq = @c_seq ";
          if (show_remove == 0)
          {
            selectQuery += @" AND A.remove_dt is null ";
          }
          selectQuery += @" ORDER BY A.cr_seq DESC ";

          DynamicParameters param = new DynamicParameters();
          param.Add("@c_seq", c_seq, DbType.Int32);

          var ret = await con.QueryAsync<can_resume>(selectQuery, param);

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
    /// 후보자 이력서 리스트
    /// </summary>
    /// <param name="c_seq"></param>
    /// <returns></returns>       

    public DownloadFileModel SelectCanResumeFileInfo(int cr_seq)
    {
      try
      {
        using (IDbConnection con = new SqlConnection(base.BaseConnectionString))
        {
          con.Open();

          string selectQuery = @" SELECT file_origin_path, file_path as file_name FROM can_resume WHERE cr_seq = @cr_seq  ";

          DynamicParameters param = new DynamicParameters();
          param.Add("@cr_seq", cr_seq, DbType.Int32);

          var ret = con.Query<DownloadFileModel>(selectQuery, param);
          con.Close();

          return ret.FirstOrDefault();

        }
      }
      catch (Exception e)
      {
        throw e;
      }
    }

    public async Task<List<can_resume>> TempSelectCanResumeListAsync(int t_seq)
    {
      try
      {
        List<can_resume> list = new List<can_resume>();

        using (IDbConnection con = new SqlConnection(base.BaseConnectionString))
        {
          con.Open();

          string selectQuery = @" SELECT * FROM tempsaved_can_resume WHERE remove_dt is null AND c_seq = @c_seq ORDER BY cr_seq DESC ";

          DynamicParameters param = new DynamicParameters();
          param.Add("@c_seq", t_seq, DbType.Int32);

          var ret = await con.QueryAsync<can_resume>(selectQuery, param);

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

    public async Task<int> CountSimpleCandidateAsync(int uv_seq, int? p_seq = 0)
    {
      try
      {
        using (IDbConnection con = new SqlConnection(base.BaseConnectionString))
        {
          con.Open();

          string selectQuery = @" SELECT COUNT(0)
  FROM simple_candidate 
 WHERE is_del <>  1
   AND create_user = @uv_seq 
   AND p_seq = CASE WHEN @p_seq <> 0 THEN @p_seq ELSE p_seq END ";

          DynamicParameters param = new DynamicParameters();
          param.Add("@uv_seq", uv_seq, DbType.Int32);
          param.Add("@p_seq", p_seq, DbType.Int32);

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

    public async Task<List<simple_candidate>> SelectSimpleCandidateListAsync(simple_candidate search, int uv_seq)
    {
      try
      {
        List<simple_candidate> list = new List<simple_candidate>();

        using (IDbConnection con = new SqlConnection(base.BaseConnectionString))
        {
          con.Open();

          string selectQuery = @" 
SELECT A.*,
       B.name as create_name,
       C.name as modify_name,
       CONVERT(VARCHAR(MAX), A.create_dt, 121) as create_dt_str,
       CONVERT(VARCHAR(MAX), A.modify_dt, 121) as modify_dt_str,
       CONVERT(VARCHAR(10), birthdate, 121) AS birthDateStr
FROM simple_candidate A LEFT JOIN uv_user B
                        on A.create_user = B.uv_seq
                        LEFT JOIN uv_user C 
                        on A.modify_user = C.uv_seq
WHERE A.is_del <> 1
";

          if (uv_seq > 0)
          {
            selectQuery += "AND A.create_user = @uv_seq ";
          }

          DynamicParameters param = new DynamicParameters();
          param.Add("@uv_seq", uv_seq, DbType.Int32);

          var ret = await con.QueryAsync<simple_candidate>(selectQuery, param);

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
    /// 후보자 관련 테이블 저장?
    /// </summary>
    /// <param name="c_seq"></param>
    /// <returns></returns>
    public async Task<ProcedureReturnModel> UpdateCandidateCareerRange(int c_seq)
    {
      try
      {
        using (IDbConnection con = new SqlConnection(base.BaseConnectionString))
        {
          con.Open();

          ProcedureReturnModel rtn = new ProcedureReturnModel();

          try
          {
            DynamicParameters param = new DynamicParameters();
            param.Add("@c_seq", c_seq, DbType.Int32);
            param.Add("@RTN_CD", "", DbType.String, direction: ParameterDirection.Output);
            param.Add("@RTN_MSG", "", DbType.String, direction: ParameterDirection.Output);

            var ret = await SqlMapper.ExecuteAsync(con, "nSP_CAND_CAREER_UPDATE", param, commandType: CommandType.StoredProcedure);

            rtn.RTN_CD = param.Get<string>("@RTN_CD");
            rtn.RTN_MSG = param.Get<string>("@RTN_CD");
          }
          catch (Exception ex)
          {
            con.Close();
            throw ex;
          }

          con.Close();

          return rtn;
        }
      }
      catch (Exception e)
      {
        throw e;
      }
    }

    /// <summary>
    /// 후보자 관련 테이블 저장?
    /// </summary>
    /// <param name="c_seq"></param>
    /// <returns></returns>
    public async Task<ProcedureReturnModel> SaveCandidateTxtMsg(int c_seq)
    {
      try
      {
        using (IDbConnection con = new SqlConnection(base.BaseConnectionString))
        {
          con.Open();

          ProcedureReturnModel rtn = new ProcedureReturnModel();

          try
          {
            DynamicParameters param = new DynamicParameters();
            param.Add("@c_seq", c_seq, DbType.Int32);
            param.Add("@RTN_CD", "", DbType.String, direction: ParameterDirection.Output);
            param.Add("@RTN_MSG", "", DbType.String, direction: ParameterDirection.Output);

            var ret = await SqlMapper.ExecuteAsync(con, "nSP_CANDIDATE_TEXT_BUILD", param, commandType: CommandType.StoredProcedure);

            rtn.RTN_CD = param.Get<string>("@RTN_CD");
            rtn.RTN_MSG = param.Get<string>("@RTN_CD");
          }
          catch (Exception ex)
          {
            con.Close();
            throw ex;
          }

          con.Close();

          return rtn;
        }
      }
      catch (Exception e)
      {
        throw e;
      }
    }

    public async Task<ProcedureReturnModel> SaveDirectorTxtMsg(int d_seq)
    {
      try
      {
        using (IDbConnection con = new SqlConnection(base.BaseConnectionString))
        {
          con.Open();

          ProcedureReturnModel rtn = new ProcedureReturnModel();

          try
          {
            DynamicParameters param = new DynamicParameters();
            param.Add("@D_SEQ", d_seq, DbType.Int32);
            param.Add("@RTN_CD", "", DbType.String, direction: ParameterDirection.Output);
            param.Add("@RTN_MSG", "", DbType.String, direction: ParameterDirection.Output);

            var ret = await SqlMapper.ExecuteAsync(con, "nSP_DIRECTOR_TEXT_BUILD", param, commandType: CommandType.StoredProcedure);

            rtn.RTN_CD = param.Get<string>("@RTN_CD");
            rtn.RTN_MSG = param.Get<string>("@RTN_CD");
          }
          catch (Exception ex)
          {
            con.Close();
            throw ex;
          }

          con.Close();

          return rtn;
        }
      }
      catch (Exception e)
      {
        throw e;
      }
    }

    public async Task<bool> UpdateMailWork(int c_seq, string timestamp, string status_str = "신규등록")
    {
      try
      {
        using (IDbConnection con = new SqlConnection(base.BaseConnectionString))
        {
          con.Open();
          try
          {
            string sql = @"
UPDATE mail_resume
SET dv_update_state = 'C',
    dv_del_reason = @status_str,
    dv_cd_no  = @c_seq,
    dd_update = GETDATE(),
    dv_mod_id = @user_seq,
    dd_mod_date = GETDATE()
WHERE dv_timestamp = @timestamp
";
            DynamicParameters param = new DynamicParameters();
            param.Add("@user_seq", AppIdentity.user_seq, DbType.Int32);
            param.Add("@c_seq", c_seq, DbType.Int32);
            param.Add("@status_str", status_str, DbType.String);
            param.Add("@timestamp", timestamp, DbType.String);

            var ret = await SqlMapper.ExecuteAsync(con, sql, param, commandType: CommandType.Text);

            con.Close();

            return true;
          }
          catch  //(Exception ex)
          {
            con.Close();
            return false;
            //throw ex;
          }


        }
      }
      catch (Exception e)
      {
        throw e;
      }
    }

    public async Task<bool> UpdateMailRemove(string timestamp, string remove_yn, string desc = "")
    {
      try
      {
        using (IDbConnection con = new SqlConnection(base.BaseConnectionString))
        {
          con.Open();
          try
          {
            string sql = @"
UPDATE mail_resume
SET dv_update_state = case @remove_yn when 'Y' then 'C' else dv_update_state end,
    dd_update = case @remove_yn when 'Y' then GETDATE() else dd_update end,
    dv_del_yn = case @remove_yn when 'Y' then 'Y' else dv_del_yn end,
    dv_del_reason = @desc,
    dv_mod_id = @user_seq,
    dd_mod_date = GETDATE()
WHERE dv_timestamp = @timestamp
";
            DynamicParameters param = new DynamicParameters();
            param.Add("@user_seq", AppIdentity.user_seq, DbType.Int32);
            param.Add("@remove_yn", remove_yn, DbType.String);
            param.Add("@desc", desc, DbType.String);
            param.Add("@timestamp", timestamp, DbType.String);

            var ret = await SqlMapper.ExecuteAsync(con, sql, param, commandType: CommandType.Text);

            con.Close();

            return true;
          }
          catch  //(Exception ex)
          {
            con.Close();
            return false;
            //throw ex;
          }


        }
      }
      catch (Exception e)
      {
        throw e;
      }
    }

    /// <summary>
    /// 내 관심후보 목록
    /// </summary>
    /// <param name="search"></param>
    /// <param name="skip"></param>
    /// <param name="count"></param>
    /// <param name="totalCount"></param>
    /// <returns></returns>
    public List<privacy_agree> SelectPrivacyAgreeList(PrivacyAgreeSearchModel search, int skip, int count, out int totalCount)
    {
      try
      {
        List<privacy_agree> list = new List<privacy_agree>();

        using (IDbConnection con = new SqlConnection(base.BaseConnectionString))
        {
          con.Open();
          string selectQuery = @" SELECT PA.*
      , CASE WHEN PD1.pad_seq is not null AND  PD2.pad_seq is not null THEN 3 
             WHEN PD1.pad_seq is not null AND  PD2.pad_seq is null THEN 1
             WHEN PD1.pad_seq is null AND  PD2.pad_seq is not null THEN 2
             ELSE 0 END as agree_gubun
      , PD2.client_name
      , PD2.client_seq      
      , U.name as create_name
  FROM privacy_agree AS PA LEFT JOIN privacy_agree_dtl AS PD1
                           ON PA.pa_seq = PD1.pa_seq
                           AND PD1.agree_type = 1
                           LEFT JOIN privacy_agree_dtl AS PD2
                           ON PA.pa_seq = PD2.pa_seq
                           AND PD2.agree_type = 2
                           LEFT JOIN UV_USER U
                           ON PA.create_user = U.uv_seq
 WHERE 1 = 1";
          if (search.is_agree_only == 1)
          {
            selectQuery += " AND PA.agree_dt is not null ";
          }
          if (search.uv_seq > 0)
          {
            selectQuery += " AND PA.create_user = @uv_seq ";
          }

          if (search.c_seq > 0)
          {
            selectQuery += " AND PA.c_seq = @c_seq ";
          }

          if (search.client_seq > 0)
          {
            selectQuery += " AND PD2.client_seq = @client_seq ";
          }

          if (!string.IsNullOrWhiteSpace(search.searchTxt))
            selectQuery += string.Format(" AND {0} LIKE '%' + @search_txt + '%' ", search.searchOption);

          selectQuery += string.Format(" ORDER BY {0} {1} ", search.orderTxt, search.orderOption);
          selectQuery += @" OFFSET @currentPage ROWS FETCH NEXT @pageSize ROWS ONLY ";

          string countQuery = @" SELECT COUNT(0)
  FROM privacy_agree AS PA LEFT JOIN privacy_agree_dtl AS PD1
                           ON PA.pa_seq = PD1.pa_seq
                           AND PD1.agree_type = 1
                           LEFT JOIN privacy_agree_dtl AS PD2
                           ON PA.pa_seq = PD2.pa_seq
                           AND PD2.agree_type = 2
 WHERE 1 = 1 ";

          if (search.is_agree_only == 1)
          {
            countQuery += " AND PA.agree_dt is not null ";
          }

          if (search.uv_seq > 0)
          {
            countQuery += " AND PA.create_user = @uv_seq ";
          }

          if (search.c_seq > 0)
          {
            countQuery += " AND PA.c_seq = @c_seq ";
          }

          if (search.client_seq > 0)
          {
            countQuery += " AND PD2.client_seq = @client_seq ";
          }

          if (!string.IsNullOrWhiteSpace(search.searchTxt))
            countQuery += string.Format(" AND {0} LIKE '%' + @search_txt + '%' ", search.searchOption);

          DynamicParameters param = new DynamicParameters();
          param.Add("@search_txt", search.searchTxt, DbType.String);
          param.Add("@uv_seq", search.uv_seq, DbType.Int32);
          param.Add("@client_seq", search.client_seq, DbType.Int32);
          param.Add("@c_seq", search.c_seq, DbType.Int32);
          param.Add("@currentPage", skip, DbType.Int32);
          param.Add("@pageSize", count, DbType.Int32);

          DynamicParameters param2 = new DynamicParameters();
          param2.Add("@search_txt", search.searchTxt, DbType.String);
          param2.Add("@uv_seq", search.uv_seq, DbType.Int32);
          param2.Add("@client_seq", search.client_seq, DbType.Int32);
          param2.Add("@c_seq", search.c_seq, DbType.Int32);

          var ret = con.Query<privacy_agree>(selectQuery, param);
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
    /// 내 관심후보 목록
    /// </summary>
    /// <param name="search"></param>
    /// <param name="skip"></param>
    /// <param name="count"></param>
    /// <param name="totalCount"></param>
    /// <returns></returns>
    public List<can_interest> SelectMyCandidateList(MyCandidateSearchModel search, int uv_seq, int skip, int count, out int totalCount)
    {
      try
      {
        List<can_interest> list = new List<can_interest>();

        using (IDbConnection con = new SqlConnection(base.BaseConnectionString))
        {
          con.Open();
          string selectQuery = @" SELECT CI.*
      ,C.kor_name
      ,C.birth_date
      ,ISNULL(C.phone, C.cell_phone) AS phone
      ,C.email1
  FROM can_interest AS CI INNER JOIN candidate AS C
                                  ON CI.c_seq = C.c_seq
 WHERE CI.uv_seq = @uv_seq ";

          if (!string.IsNullOrWhiteSpace(search.searchTxt))
            selectQuery += string.Format(" AND {0} LIKE '%' + @search_txt + '%' ", search.searchOption);

          selectQuery += string.Format(" ORDER BY {0} {1} ", search.orderTxt, search.orderOption);
          selectQuery += @" OFFSET @currentPage ROWS FETCH NEXT @pageSize ROWS ONLY ";

          string countQuery = @" SELECT COUNT(0)
  FROM can_interest AS CI INNER JOIN candidate AS C
                                  ON CI.c_seq = C.c_seq
 WHERE CI.uv_seq = @uv_seq ";

          if (!string.IsNullOrWhiteSpace(search.searchTxt))
            countQuery += string.Format(" AND {0} LIKE '%' + @search_txt + '%' ", search.searchOption);

          DynamicParameters param = new DynamicParameters();
          param.Add("@search_txt", search.searchTxt, DbType.String);
          param.Add("@uv_seq", uv_seq, DbType.Int32);
          param.Add("@currentPage", skip, DbType.Int32);
          param.Add("@pageSize", count, DbType.Int32);

          DynamicParameters param2 = new DynamicParameters();
          param2.Add("@search_txt", search.searchTxt, DbType.String);
          param2.Add("@uv_seq", uv_seq, DbType.Int32);

          var ret = con.Query<can_interest>(selectQuery, param);
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
    /// 내 관심후보 단건 조회.
    /// </summary>
    /// <param name="cf_seq"></param>
    /// <param name="skip"></param>
    /// <param name="count"></param>
    /// <param name="totalCount"></param>
    /// <returns></returns>
    public async Task<can_interest> SelectMyCandidateOneAsync(int cf_seq)
    {
      try
      {
        using (IDbConnection con = new SqlConnection(base.BaseConnectionString))
        {
          con.Open();
          string selectQuery = @" SELECT CI.*
      ,C.kor_name
      ,C.birth_date
      ,ISNULL(C.cell_phone, C.phone) AS phone
      ,C.email1
  FROM can_interest AS CI INNER JOIN candidate AS C
                                  ON CI.c_seq = C.c_seq
 WHERE CI.cf_seq = @cf_seq ";

          DynamicParameters param = new DynamicParameters();
          param.Add("@cf_seq", cf_seq, DbType.Int32);

          var ret = await con.QueryAsync<can_interest>(selectQuery, param);

          con.Close();

          return ret.FirstOrDefault();
        }
      }
      catch (Exception e)
      {
        throw e;
      }
    }

    public async Task<can_interest> SelectMyCandidateNewOneAsync(int c_seq, int uv_seq)
    {
      try
      {
        using (IDbConnection con = new SqlConnection(base.BaseConnectionString))
        {
          con.Open();
          string selectQuery = @" 
SELECT CI.*
      ,C.kor_name
      ,C.birth_date
      ,ISNULL(C.cell_phone, C.phone) AS phone
      ,C.email1
  FROM candidate AS C LEFT JOIN can_interest CI
                      ON C.c_seq = CI.c_seq
                      AND CI.uv_seq = @uv_seq
 WHERE C.c_seq = @c_seq ";

          DynamicParameters param = new DynamicParameters();
          param.Add("@c_seq", c_seq, DbType.Int32);
          param.Add("@uv_seq", uv_seq, DbType.Int32);

          var ret = await con.QueryAsync<can_interest>(selectQuery, param);

          con.Close();

          return ret.FirstOrDefault();
        }
      }
      catch (Exception e)
      {
        throw e;
      }
    }


    public async Task<PgList<director>> SelectDirectorListAsync(DirectorSearchModel search, int skip, int count)
    {
      try
      {
        int totalCount = 0;
        var list = new List<director>();
        using (IDbConnection con = new SqlConnection(base.BaseConnectionString))
        {
          con.Open();

          string selectQuery = @" 
SELECT A.*
     ,  YEAR(A.birth_date) as birth_year
     , CU.name as create_name
     , MU.name as modify_name
FROM director A LEFT JOIN UV_USER CU
                          ON A.create_seq = CU.uv_seq
                          LEFT JOIN UV_USER MU
                          ON A.modify_seq = MU.uv_seq
";

          string countQuery = @" 
SELECT COUNT(*)
FROM director A LEFT JOIN UV_USER CU
                          ON A.create_seq = CU.uv_seq
                          LEFT JOIN UV_USER MU
                          ON A.modify_seq = MU.uv_seq
";
          string selectOrder = String.Empty;
          string selectWhere = String.Empty;

          if (!string.IsNullOrWhiteSpace(search.total))
          {
            if (!string.IsNullOrWhiteSpace(selectWhere))
            {
              selectWhere += " AND ";
            }

            selectWhere += @"
(A.kor_name like '%'+@total+'%'
OR A.edu_history like '%'+@total+'%'
OR A.exp_history like '%'+@total+'%'
OR A.keyword like '%'+@total+'%'
OR A.group_name like '%'+@total+'%'
OR A.comp_name like '%'+@total+'%')
";
          }

          if (!string.IsNullOrWhiteSpace(search.name))
          {
            if (!string.IsNullOrWhiteSpace(selectWhere))
            {
              selectWhere += " AND ";
            }

            if (new string[] { "\"" }.Any(c => search.name.Contains(c)))
            {
              selectWhere += @"A.kor_name = @name ";
            }
            else
            {
              selectWhere += @"A.kor_name like '%'+@name+'%'";
            }
          }


          if (!string.IsNullOrWhiteSpace(search.startBirth))
          {
            if (!string.IsNullOrWhiteSpace(selectWhere))
            {
              selectWhere += " AND ";
            }
            selectWhere += @"YEAR(A.birth_date) >= @startBirth";
          }

          if (!string.IsNullOrWhiteSpace(search.endBirth))
          {
            if (!string.IsNullOrWhiteSpace(selectWhere))
            {
              selectWhere += " AND ";
            }
            selectWhere += @"YEAR(A.birth_date) <= @endBirth";
          }

          if (search.gender != 0)
          {
            if (!string.IsNullOrWhiteSpace(selectWhere))
            {
              selectWhere += " AND ";
            }
            selectWhere += @"A.gender = @gender ";
          }

          if (search.expr_type != 0)
          {
            if (!string.IsNullOrWhiteSpace(selectWhere))
            {
              selectWhere += " AND ";
            }
            selectWhere += @"A.exec_type = @expr_type ";
          }

          if (search.od_type != 0)
          {
            if (!string.IsNullOrWhiteSpace(selectWhere))
            {
              selectWhere += " AND ";
            }
            selectWhere += @"A.od_type = @od_type ";
          }

          if (!string.IsNullOrWhiteSpace(search.school))
          {
            if (!string.IsNullOrWhiteSpace(selectWhere))
            {
              selectWhere += " AND ";
            }
            selectWhere += @"A.edu_history like '%'+@school+'%'";
          }

          if (!string.IsNullOrWhiteSpace(search.expr))
          {
            if (!string.IsNullOrWhiteSpace(selectWhere))
            {
              selectWhere += " AND ";
            }
            selectWhere += @"A.exp_history like '%'+@expr+'%'";
          }

          if (!string.IsNullOrWhiteSpace(search.keyword))
          {
            if (!string.IsNullOrWhiteSpace(selectWhere))
            {
              selectWhere += " AND ";
            }
            selectWhere += @"A.keyword like '%'+@keyword+'%'";
          }

          if (!string.IsNullOrWhiteSpace(search.group_name))
          {
            if (!string.IsNullOrWhiteSpace(selectWhere))
            {
              selectWhere += " AND ";
            }
            selectWhere += @"A.group_name like '%'+@group_name+'%'";
          }

          if (!string.IsNullOrWhiteSpace(search.company))
          {
            if (!string.IsNullOrWhiteSpace(selectWhere))
            {
              selectWhere += " AND ";
            }
            selectWhere += @"A.comp_name like '%'+@company+'%'";
          }

          if (!string.IsNullOrWhiteSpace(search.create_dt_start))
          {
            if (!string.IsNullOrWhiteSpace(selectWhere))
            {
              selectWhere += " AND ";
            }
            selectWhere += @"A.create_dt >= '@create_dt_start'";
          }

          if (!string.IsNullOrWhiteSpace(search.create_dt_end))
          {
            if (!string.IsNullOrWhiteSpace(selectWhere))
            {
              selectWhere += " AND ";
            }
            selectWhere += @"DATEADD(d, -1, A.create_dt) <= '@create_dt_end'";
          }

          if (!string.IsNullOrWhiteSpace(search.modify_dt_start))
          {
            if (!string.IsNullOrWhiteSpace(selectWhere))
            {
              selectWhere += " AND ";
            }
            selectWhere += @"A.modify_dt >= '@modify_dt_start'";
          }

          if (!string.IsNullOrWhiteSpace(search.modify_dt_end))
          {
            if (!string.IsNullOrWhiteSpace(selectWhere))
            {
              selectWhere += " AND ";
            }
            selectWhere += @"DATEADD(d, -1, A.modify_dt) <= '@modify_dt_end'";
          }


          if (search.job.Count > 0)
          {

            string job_sql = String.Empty;

            foreach (var job in search.job)
            {
              job_sql += (!string.IsNullOrWhiteSpace(job_sql) ? ", " : "") + job.code2;
            }
            if (!string.IsNullOrEmpty(job_sql))
            {
              selectWhere += (!string.IsNullOrWhiteSpace(selectWhere) ? " AND " : "") + " A.d_seq IN (SELECT d_seq FROM director_job WHERE code2 in (" + job_sql + "))";
            }
          }
          if (search.business.Count > 0)
          {

            string business_sql = String.Empty;

            foreach (var business in search.business)
            {
              business_sql += (!string.IsNullOrWhiteSpace(business_sql) ? ", " : "") + business.code2;
            }
            if (!string.IsNullOrEmpty(business_sql))
            {
              selectWhere += (!string.IsNullOrWhiteSpace(selectWhere) ? " AND " : "") + " A.d_seq IN (SELECT d_seq FROM director_business WHERE code2 in (" + business_sql + "))";
            }
          }


          selectOrder = string.Format(" ORDER BY {0} {1} , A.c_seq DESC ", search.orderTxt, search.orderOption);
          selectOrder += @" OFFSET @skip ROWS FETCH NEXT @count ROWS ONLY ";


          DynamicParameters param = new DynamicParameters();
          param.Add("@total", search.total, DbType.String);

          param.Add("@name", search.name.Replace("\"", ""), DbType.String);

          param.Add("@startBirth", search.startBirth, DbType.String);
          param.Add("@endBirth", search.endBirth, DbType.String);
          param.Add("@gender", search.gender, DbType.Int32);

          param.Add("@expr_type", search.expr_type, DbType.Int32);
          param.Add("@od_type", search.od_type, DbType.Int32);

          param.Add("@school", search.school, DbType.String);
          param.Add("@expr", search.expr, DbType.String);
          param.Add("@group_name", search.group_name, DbType.String);
          param.Add("@company", search.company, DbType.String);
          param.Add("@keyword", search.keyword, DbType.String);

          param.Add("@create_dt_start", search.create_dt_start, DbType.String);
          param.Add("@create_dt_end", search.create_dt_end, DbType.String);
          param.Add("@modify_dt_start", search.modify_dt_start, DbType.String);
          param.Add("@modify_dt_end", search.modify_dt_end, DbType.String);

          param.Add("@skip", skip, DbType.Int32);
          param.Add("@count", count, DbType.Int32);

          if (!string.IsNullOrWhiteSpace(selectWhere))
          {
            selectQuery += " WHERE " + selectWhere + selectOrder;
            countQuery += " WHERE " + selectWhere;
          }
          else
          {
            selectQuery += selectOrder;
          }

          var ret = con.Query<director>(selectQuery, param);
          totalCount = con.QueryFirstOrDefault<int>(countQuery, param);

          list = ret.ToList();

          con.Close();

          return new PgList<director>()
          {
            totalCount = totalCount,
            Items = list
          };
        }

      }
      catch (Exception e)
      {
        throw e;
      }
    }
  }
}
