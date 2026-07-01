using Dapper;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using Univision.Core.Models;
using Univision.Core.Models.DTO;
using Univision.Core.Models.DTO.Excel;
using Univision.Core.Models.DTO.Request.Project;
using Univision.Core.Models.DTO.Response.Project;
using Univision.Core.Models.HomePage;

namespace Univision.Core.Repositories
{
  public class ProjectRepository : BaseRepository
  {
    public List<inorder> SelectInorderList(InorderSearchModel search, int skip, int count, out int totalCount)
    {
      try
      {
        List<inorder> list = new List<inorder>();

        using (IDbConnection con = new SqlConnection(base.BaseConnectionString))
        {
          con.Open();

          string selectQuery = @" SELECT I.*
      ,C.kor_name as client_name
      ,CC.name as contact_name
      ,P.title as project_title
      ,P.pjt_status
      ,P.create_dt AS project_dt
      ,PD.am_names
      ,PM.searcher_names
      ,U.name AS director_name
      ,U.hp AS director_cp
      ,U.tel AS director_tel
      ,U.email AS director_email
    FROM Inorder AS I LEFT OUTER JOIN client AS C
                                   ON I.c_seq = C.c_seq
                      LEFT OUTER JOIN project AS P
                                   ON I.p_seq = P.p_seq
                      LEFT OUTER JOIN (SELECT distinct PD.p_seq
                                             ,STUFF((SELECT ',' + B.name
                                                       FROM pjt_director AS A INNER JOIN uv_user AS B
                                                                                      ON A.uv_seq = B.uv_seq
                                                      WHERE A.p_seq = PD.p_seq
                                                        FOR XML PATH('')), 1, 1, '') AS am_names
                                         FROM pjt_director AS PD) AS PD
                                   ON I.p_seq = PD.p_seq
                      LEFT OUTER JOIN (SELECT distinct PM.p_seq
                                             ,STUFF((SELECT ',' + B.name
                                                       FROM pjt_manager AS A INNER JOIN uv_user AS B
                                                                                     ON A.uv_seq = B.uv_seq
                                                      WHERE A.p_seq = PM.p_seq
                                                        FOR XML PATH('')), 1, 1, '') AS searcher_names
                                         FROM pjt_manager AS PM) AS PM
                                   ON I.p_seq = PM.p_seq
                      LEFT OUTER JOIN client_contact AS CC
                                   ON I.cc_seq = CC.cc_seq
                      LEFT OUTER JOIN uv_user AS U
                                   ON I.director_seq = U.uv_seq
 WHERE 1 = 1 ";

          if (!string.IsNullOrWhiteSpace(search.searchTxt))
            selectQuery += " AND (P.title LIKE '%' + @searchTxt + '%' OR  C.kor_name LIKE '%' + @searchTxt + '%') ";

          //AND P.create_user = @uv_seq 
          selectQuery += string.Format(" ORDER BY {0} {1} ", search.orderTxt, search.orderOption);
          selectQuery += " OFFSET @currentPage ROWS FETCH NEXT @pageSize ROWS ONLY ";

          string countQuery = @" SELECT COUNT(0)
    FROM Inorder AS I LEFT OUTER JOIN client AS C
                                   ON I.c_seq = C.c_seq
                      LEFT OUTER JOIN project AS P
                                   ON I.p_seq = P.p_seq
   WHERE 1 = 1 ";

          if (!string.IsNullOrWhiteSpace(search.searchTxt))
            countQuery += " AND (P.title LIKE '%' + @searchTxt + '%' OR C.kor_name LIKE '%' + @searchTxt + '%') ";

          DynamicParameters param = new DynamicParameters();
          param.Add("@searchTxt", search.searchTxt, DbType.String);
          param.Add("@currentPage", skip, DbType.Int32);
          param.Add("@pageSize", count, DbType.Int32);

          DynamicParameters param2 = new DynamicParameters();
          param2.Add("@searchTxt", search.searchTxt, DbType.String);

          var ret = con.Query<inorder>(selectQuery, param);
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



    public async Task<inorder> SelectInorderOneAsync(int i_seq)
    {
      try
      {
        using (IDbConnection con = new SqlConnection(base.BaseConnectionString))
        {
          con.Open();

          string selectQuery = @" SELECT I.*
      ,C.kor_name as client_name
      ,CC.name as contact_name
      ,P.title as project_title
      ,P.pjt_status
      ,P.create_dt AS project_dt
      ,PD.am_names
      ,PM.searcher_names
      ,U.name AS director_name
      ,U.hp AS director_cp
      ,U.tel AS director_tel
      ,U.email AS director_email
      , (select count(0) from inorder_log where i_seq = i.i_seq) log_cnt
    FROM Inorder AS I LEFT OUTER JOIN client AS C
                                   ON I.c_seq = C.c_seq
                      LEFT OUTER JOIN project AS P
                                   ON I.p_seq = P.p_seq
                      LEFT OUTER JOIN (SELECT distinct PD.p_seq
                                             ,STUFF((SELECT ',' + B.name
                                                       FROM pjt_director AS A INNER JOIN uv_user AS B
                                                                                      ON A.uv_seq = B.uv_seq
                                                      WHERE A.p_seq = PD.p_seq
                                                        FOR XML PATH('')), 1, 1, '') AS am_names
                                         FROM pjt_director AS PD) AS PD
                                   ON I.p_seq = PD.p_seq
                      LEFT OUTER JOIN (SELECT distinct PM.p_seq
                                             ,STUFF((SELECT ',' + B.name
                                                       FROM pjt_manager AS A INNER JOIN uv_user AS B
                                                                                     ON A.uv_seq = B.uv_seq
                                                      WHERE A.p_seq = PM.p_seq
                                                        FOR XML PATH('')), 1, 1, '') AS searcher_names
                                         FROM pjt_manager AS PM) AS PM
                                   ON I.p_seq = PM.p_seq
                      LEFT OUTER JOIN client_contact AS CC
                                   ON I.cc_seq = CC.cc_seq
                      LEFT OUTER JOIN uv_user AS U
                                   ON I.director_seq = U.uv_seq
    WHERE I.i_seq = @i_seq ";

          DynamicParameters param = new DynamicParameters();
          param.Add("@i_seq", i_seq, DbType.Int32);

          var ret = await con.QueryAsync<inorder>(selectQuery, param);

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
    /// 프로젝트 책임자(AM) 리스트 조회
    /// </summary>
    /// <param name="p_seq"></param>
    /// <returns></returns>
    public async Task<List<inorder_director>> SelectInorderDirectorListAsync(int i_seq)
    {
      try
      {
        List<inorder_director> list = new List<inorder_director>();

        using (IDbConnection con = new SqlConnection(base.BaseConnectionString))
        {
          con.Open();

          string selectQuery = @" SELECT ID.*
      ,U.name as user_name
      ,UD.ud_name as division_name
  FROM inorder_director AS ID INNER JOIN uv_user AS U
                                  ON ID.uv_seq = U.uv_seq
                          INNER JOIN uv_division AS UD
                                  ON U.ud_seq = UD.ud_seq
 WHERE ID.i_seq = @i_seq ";

          DynamicParameters param = new DynamicParameters();
          param.Add("@i_seq", i_seq, DbType.Int32);

          var ret = await con.QueryAsync<inorder_director>(selectQuery, param);

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

    public async Task<List<inorder_file>> SelectInorderFileListAsync(int i_seq)
    {
      try
      {
        List<inorder_file> list = new List<inorder_file>();

        using (IDbConnection con = new SqlConnection(base.BaseConnectionString))
        {
          con.Open();

          string selectQuery = @" SELECT [IF].*
  FROM inorder AS I INNER JOIN inorder_file AS [IF]
                            ON I.i_seq = [IF].i_seq
 WHERE I.i_seq = @i_seq 
   AND [IF].remove_dt IS NULL ";

          DynamicParameters param = new DynamicParameters();
          param.Add("@i_seq", i_seq, DbType.Int32);

          var ret = await con.QueryAsync<inorder_file>(selectQuery, param);

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

    public List<inorder_memo> SelectInOrderMemoList(int i_seq, int skip, int count, out int totalCount)
    {
      try
      {
        List<inorder_memo> list = new List<inorder_memo>();

        using (IDbConnection con = new SqlConnection(base.BaseConnectionString))
        {
          con.Open();

          string selectQuery = @" SELECT IM.*
      ,U.name AS create_name
  FROM inorder_memo AS IM INNER JOIN uv_user AS U
                                  ON IM.create_user = U.uv_seq
 WHERE IM.i_seq = @i_seq
 ORDER BY IM.im_seq DESC
OFFSET @currentPage ROWS FETCH NEXT @pageSize ROWS ONLY ";

          string countQuery = @" SELECT IM.*
      ,U.name AS create_name
  FROM inorder_memo AS IM INNER JOIN uv_user AS U
                                  ON IM.create_user = U.uv_seq 
 WHERE IM.i_seq = @i_seq ";

          DynamicParameters param = new DynamicParameters();
          param.Add("@i_seq", i_seq, DbType.Int32);
          param.Add("@currentPage", skip, DbType.Int32);
          param.Add("@pageSize", count, DbType.Int32);

          DynamicParameters param2 = new DynamicParameters();
          param2.Add("@i_seq", i_seq, DbType.Int32);

          var ret = con.Query<inorder_memo>(selectQuery, param);
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


    public async Task<home_project> SelectHomeProjectOneAsync(int p_seq)
    {
      try
      {
        using (IDbConnection con = new SqlConnection(base.BaseConnectionString))
        {
          con.Open();

          string selectQuery = @" SELECT P.[p_seq], [title], [close_dt], [open_dt], [business_code1], [business_code2], CL.[is_foreign_invest], 
[job_code1], [job_code2], 
[edu_name], 
POS.p_name as position_name, 
[assign_task], [requirement], [experience_type], [expreience_number], 
ISNULL(STUFF((SELECT DISTINCT
                                ',' + area1 + '-' + area2
                          FROM   univision.dbo.pjt_place
                        WHERE  p_seq = P.p_seq
                          FOR XML PATH('')),1,1,''), '') AS working_area_1, 
uv.name as eu_name, uv.email as eu_email, uv.tel as EU_Company_Phone, uv.rank_name as EU_Position
,CASE WHEN P.is_kor_resume = 1 THEN '[국문이력서]' ELSE '' END
            + CASE WHEN P.is_eng_resume = 1 THEN '[영문이력서]' ELSE '' END
            + CASE WHEN P.is_etc = 1 THEN '['+P.etc_comment +']' ELSE '' END as interview_process_1

		  , P.[create_dt], P.[create_user], P.[modify_dt], P.[modify_user]
FROM univision.dbo.project P left join univision.dbo.code_business_dtl IND
                   ON  P.business_code1 = IND.code1
                   AND P.business_code2 = IND.code2
                   left join univision.dbo.code_job_dtl JFC
                   ON  P.job_code1 = JFC.code1
                   AND P.job_code2 = JFC.code2  
                   LEFT JOIN univision.dbo.client as CL
                   ON  P.c_seq     = CL.c_seq
                   LEFT JOIN (select row_number() over(partition by p_seq order by uv_seq) as l, * from univision.dbo.pjt_manager) SC
                   ON  P.p_seq = SC.p_seq
				   and sc.l = 1
                   LEFT JOIN univision.dbo.uv_user uv
                   ON  SC.uv_seq = uv.uv_seq
                   LEFT JOIN univision.dbo.code_position POS
                   ON P.position_seq = POS.p_code
where P.pjt_type = 1 
AND P.pjt_status = 1 
AND P.is_posting = 1 
AND DATEDIFF(d, GETDATE(), DATEADD(d, 60, P.open_dt)) > 0 
AND P.p_seq = @p_seq
";

          DynamicParameters param = new DynamicParameters();
          param.Add("@p_seq", p_seq, DbType.Int32);

          var ret = await con.QueryAsync<home_project>(selectQuery, param);

          con.Close();

          return ret.FirstOrDefault();
        }
      }
      catch (Exception e)
      {
        throw e;
      }
    }

    public async Task<PgList<project>> SelectProjectList(ProjectSearchModel search, int skip, int count, int uv_seq)
    {
      try
      {
        List<project> list = new List<project>();
        int totalCount = 0;
        using (IDbConnection con = new SqlConnection(base.BaseConnectionString))
        {
          con.Open();

          string order = String.Empty;
          string order_key_first = String.Empty;
          string selectQuery = String.Empty;
          string selectQuery_state = String.Empty;
          string selectQuery_where = @"
WHERE (A.p_seq IN (select p_seq FROM pjt_director WHERE uv_seq = @uv_seq) OR A.p_seq IN (select p_seq FROM pjt_manager WHERE uv_seq = @uv_seq))";


          if (search.is_key_first.HasValue && search.is_key_first == 1)
          {
            order_key_first = " CASE WHEN ISNULL(B.p_seq, 0) > 0 THEN 1 ELSE 2 END, B.create_dt DESC, ";
          }

          if (!String.IsNullOrEmpty(search.orderOption1) && !String.IsNullOrEmpty(search.orderTxt1))
          {
            order = @" ORDER BY " + order_key_first + search.orderTxt1.Replace("_idx", "") + " " + search.orderOption1 + ", A.p_seq DESC ";
          }
          else
          {
            order = @" ORDER BY " + order_key_first + " A.p_seq DESC ";
          }

          order += " OFFSET @currentPage ROWS FETCH NEXT @pageSize ROWS ONLY ";

          if (search.pjt_type != 0)
            selectQuery_where += (!string.IsNullOrWhiteSpace(selectQuery_where) ? " AND " : "") + " A.pjt_type = @pjt_type ";

          

          if (!string.IsNullOrWhiteSpace(search.client))
          {

            selectQuery_where += (!string.IsNullOrWhiteSpace(selectQuery_where) ? " AND " : "") + " ( A.client_kor_name like '%' + @client + '%' or A.client_eng_name like '%' + @client + '%' ) ";

          }

          if (!string.IsNullOrWhiteSpace(search.title))
          {

            selectQuery_where += (!string.IsNullOrWhiteSpace(selectQuery_where) ? " AND " : "") + " A.title like '%' + @title + '%' ";


          }

          if (!string.IsNullOrWhiteSpace(search.user_name))
          {
            selectQuery_where += (!string.IsNullOrWhiteSpace(selectQuery_where) ? " AND " : "") + " (A.am_names like '%' + @user_name + '%' or A.searcher_names like '%' + @user_name + '%')";
          }


          if (search.job.Count > 0)
          {
            string job_sql = String.Empty;

            foreach (var job in search.job)
            {
              if (job.code2 < 1000)
              {
                job_sql += (!string.IsNullOrWhiteSpace(job_sql) ? " OR " : "") + " A.job_code1 = '" + job.code2 + "' ";
                job_sql += (!string.IsNullOrWhiteSpace(job_sql) ? " OR " : "") + " A.sub_job_code1 = '" + job.code2 + "' ";
              }
              else
              {
                job_sql += (!string.IsNullOrWhiteSpace(job_sql) ? " OR " : "") + " A.job_code2 = '" + job.code2 + "' ";
                job_sql += (!string.IsNullOrWhiteSpace(job_sql) ? " OR " : "") + " A.sub_job_code2 = '" + job.code2 + "' ";
              }

            }

            if (!string.IsNullOrEmpty(job_sql))
            {
              selectQuery_where += (!string.IsNullOrWhiteSpace(selectQuery_where) ? " AND " : "") + "(" + job_sql + ")";
            }
          }
          if (search.business.Count > 0)
          {

            string business_sql = String.Empty;

            foreach (var business in search.business)
            {
              if (business.code2 < 1000)
              {
                business_sql += (!string.IsNullOrWhiteSpace(business_sql) ? " OR " : "") + " A.business_code1 = '" + business.code2 + "' ";
                business_sql += (!string.IsNullOrWhiteSpace(business_sql) ? " OR " : "") + " A.sub_business_code1 = '" + business.code2 + "' ";
              }
              else
              {
                business_sql += (!string.IsNullOrWhiteSpace(business_sql) ? " OR " : "") + " A.business_code2 = '" + business.code2 + "' ";
                business_sql += (!string.IsNullOrWhiteSpace(business_sql) ? " OR " : "") + " A.sub_business_code2 = '" + business.code2 + "' ";
              }
            }
            if (!string.IsNullOrEmpty(business_sql))
            {
              selectQuery_where += (!string.IsNullOrWhiteSpace(selectQuery_where) ? " AND " : "") + "(" + business_sql + ")";
            }

            

          }


          if (!string.IsNullOrWhiteSpace(search.dt_start))
            selectQuery_where += (!string.IsNullOrWhiteSpace(selectQuery_where) ? " AND " : "") + " " + search.range_field.Replace("_idx", "") + " >= @dt_start ";
          if (!string.IsNullOrWhiteSpace(search.dt_end))
            selectQuery_where += (!string.IsNullOrWhiteSpace(selectQuery_where) ? " AND " : "") + " " + search.range_field.Replace("_idx", "") + " <= @dt_end ";




          if (search.position_start > 0)
          {
            selectQuery_where += (!string.IsNullOrWhiteSpace(selectQuery_where) ? " AND " : "") + " A.position_seq <= @position_start ";
          }

          if (search.position_end > 0)
          {

            selectQuery_where += (!string.IsNullOrWhiteSpace(selectQuery_where) ? " AND " : "") + " A.position_seq >= @position_end ";
          }

          if (search.is_pe == 0 || search.is_pe == 1)
          {
            selectQuery_where += (!string.IsNullOrWhiteSpace(selectQuery_where) ? " AND " : "") + " A.is_pe = @is_pe ";
          }

          if (search.state > 0)
          {
            selectQuery_state += (!string.IsNullOrWhiteSpace(selectQuery_where) ? " AND " : "") + " A.pjt_status = @state ";
          }

          

          DynamicParameters param = new DynamicParameters();
          param.Add("@uv_seq", uv_seq, DbType.Int32);
          param.Add("@pjt_type", search.pjt_type, DbType.Int32);
          param.Add("@client", search.client, DbType.String);
          param.Add("@title", search.title, DbType.String);
          param.Add("@user_name", search.user_name, DbType.String);
          param.Add("@dt_start", search.dt_start, DbType.String);
          param.Add("@dt_end", search.dt_end, DbType.String);
          param.Add("@position_start", search.position_start, DbType.Int32);
          param.Add("@position_end", search.position_end, DbType.Int32);
          param.Add("@state", search.state, DbType.Int32);
          param.Add("@is_pe", search.is_pe, DbType.Int32);
          param.Add("@currentPage", skip, DbType.Int32);
          param.Add("@pageSize", count, DbType.Int32);

          selectQuery = @" SELECT 
A.p_seq, 
A.c_seq, 
A.client_kor_name as company_name,
A.client_eng_name, 
A.title, 
A.pjt_type, 
A.pjt_type_str, 
A.pjt_status, 
A.pjt_status_str, 
A.am_names, 
A.searcher_names, 
A.hire_info, 
A.all_candidate, 
A.position_seq, 
A.position_name, 
A.position_str, 
A.exp_salary_won, 
A.business_code1, business_code2, sub_business_code1, sub_business_code2, 
A.business_name1 as business_names1, 
A.business_name2 as business_names2, 
A.sub_business_name1 as sub_business_names1, 
A.sub_business_name2 as sub_business_names2, 
A.job_code1, job_code2, sub_job_code1, sub_job_code2, 
A.job_name1 as job_names1, 
A.job_name2 as job_names2, 
A.sub_job_name1 as sub_job_names1, 
A.sub_job_name2 as sub_job_names2, 
A.create_dt_str as create_dt, 
A.modify_dt_str as modify_dt, 
A.open_dt_str as open_dt, 
A.close_dt_str as  close_dt,
A.is_pe as is_pe,
case when b.p_seq is not null then 1 else 0 end as is_my_key
FROM view_project_all A left join pjt_mykey B
on a.p_seq = b.p_seq
and b.uv_seq = @uv_seq " + selectQuery_where + selectQuery_state + order;

          var ret = await con.QueryAsync<project>(selectQuery, param);
          list = ret.ToList();

          selectQuery = @" SELECT pjt_status as code, count(*) as count FROM view_project_all A " + selectQuery_where + " GROUP BY pjt_status ORDER BY pjt_status ";

          var retCount = await con.QueryAsync<PagingGroup>(selectQuery, param);

          selectQuery = @" SELECT COUNT(*) FROM view_project_all A " + selectQuery_where + selectQuery_state;
          totalCount = await con.QueryFirstOrDefaultAsync<int>(selectQuery, param);

          //List<PagingGroup> pgg_list = new List<PagingGroup>();

          return new PgList<project>()
          {
            totalCount = totalCount,
            group_count = retCount.ToList(),
            Items = list
          };
        }
      }
      catch (Exception e)
      {
        throw e;
      }
    }


    public async Task<List<ProjectListNewExcelModel>> SelectProjectExcelList(ProjectSearchModel search, int uv_seq)
    {
      try
      {
        List<ProjectListNewExcelModel> list = new List<ProjectListNewExcelModel>();

        using (IDbConnection con = new SqlConnection(base.BaseConnectionString))
        {
          con.Open();
          string order = String.Empty;
          string selectQuery_where = @"
SELECT 
p_seq as [프로젝트_코드], 
c_seq as [고객사_코드], 
client_kor_name as [고객사_국문명],
client_eng_name as [고객사_영문명], 
title as [구인제목], 
pjt_type_str [프로젝트_구분], 
pjt_status_str as [프로젝트_진행상태], 
am_names as [AM], 
searcher_names as [SM], 
hire_info as [채용후보자정보],  
position_name as [직급분류], 
position_str as [포지션명], 
exp_salary_won as [예상_연봉], 
fee_rate as [수수료율],
business_name1 as [주_산업_대분류], 
business_name2 as [주_산업_소분류], 
sub_business_name1 as [보조_산업_대분류], 
sub_business_name2 as [보조_산업_소분류], 
job_name1 as [주_직무_대분류], 
job_name2 as [주_직무_소분류], 
sub_job_name1 as [보조_직무_대분류], 
sub_job_name2 as [보조_직무_소분류], 
create_dt_str as [최초등록일], 
modify_dt_str as [최종수정일],
CASE is_pe WHEN 1 THEN 'PE' ELSE '' END as [PE_구분]
FROM view_project_all
WHERE (p_seq IN (select p_seq FROM pjt_director WHERE uv_seq = @uv_seq) OR p_seq IN (select p_seq FROM pjt_manager WHERE uv_seq = @uv_seq))";

          if (!String.IsNullOrEmpty(search.orderOption1) && !String.IsNullOrEmpty(search.orderTxt1))
          {
            order = @" ORDER BY " + search.orderTxt1.Replace("_idx", "") + " " + search.orderOption1 + " ";
          }
          else
          {
            order = @" ORDER BY create_dt DESC, p_seq DESC ";
          }

          order += " OFFSET @currentPage ROWS FETCH NEXT @pageSize ROWS ONLY ";

          if (search.pjt_type != 0)
            selectQuery_where += (!string.IsNullOrWhiteSpace(selectQuery_where) ? " AND " : "") + " pjt_type = @pjt_type ";

          if (!string.IsNullOrWhiteSpace(search.client))
          {

            selectQuery_where += (!string.IsNullOrWhiteSpace(selectQuery_where) ? " AND " : "") + " ( client_name_kor like '%' + @client + '%' or client_name_eng like '%' + @client + '%' ) ";

          }

          if (!string.IsNullOrWhiteSpace(search.title))
          {

            selectQuery_where += (!string.IsNullOrWhiteSpace(selectQuery_where) ? " AND " : "") + " title like '%' + @title + '%' ";


          }

          if (!string.IsNullOrWhiteSpace(search.user_name))
          {
            selectQuery_where += (!string.IsNullOrWhiteSpace(selectQuery_where) ? " AND " : "") + " (am_names like '%' + @user_name + '%' or searcher_names like '%' + @user_name + '%')";
          }


          if (search.job.Count > 0)
          {
            string job_sql = String.Empty;

            foreach (var job in search.job)
            {
              if (job.code2 < 1000)
              {
                job_sql += (!string.IsNullOrWhiteSpace(job_sql) ? " OR " : "") + " job_code1 = '" + job.code2 + "' ";
                job_sql += (!string.IsNullOrWhiteSpace(job_sql) ? " OR " : "") + " sub_job_code1 = '" + job.code2 + "' ";
              }
              else
              {
                job_sql += (!string.IsNullOrWhiteSpace(job_sql) ? " OR " : "") + " job_code2 = '" + job.code2 + "' ";
                job_sql += (!string.IsNullOrWhiteSpace(job_sql) ? " OR " : "") + " sub_job_code2 = '" + job.code2 + "' ";
              }

            }

            if (!string.IsNullOrEmpty(job_sql))
            {
              selectQuery_where += (!string.IsNullOrWhiteSpace(selectQuery_where) ? " AND " : "") + "(" + job_sql + ")";
            }
          }
          if (search.business.Count > 0)
          {

            string business_sql = String.Empty;

            foreach (var business in search.business)
            {
              if (business.code2 < 1000)
              {
                business_sql += (!string.IsNullOrWhiteSpace(business_sql) ? " OR " : "") + " business_code1 = '" + business.code2 + "' ";
                business_sql += (!string.IsNullOrWhiteSpace(business_sql) ? " OR " : "") + " sub_business_code1 = '" + business.code2 + "' ";
              }
              else
              {
                business_sql += (!string.IsNullOrWhiteSpace(business_sql) ? " OR " : "") + " business_code2 = '" + business.code2 + "' ";
                business_sql += (!string.IsNullOrWhiteSpace(business_sql) ? " OR " : "") + " sub_business_code2 = '" + business.code2 + "' ";
              }
            }
            if (!string.IsNullOrEmpty(business_sql))
            {
              selectQuery_where += (!string.IsNullOrWhiteSpace(selectQuery_where) ? " AND " : "") + "(" + business_sql + ")";
            }


          }


          if (!string.IsNullOrWhiteSpace(search.dt_start))
            selectQuery_where += (!string.IsNullOrWhiteSpace(selectQuery_where) ? " AND " : "") + " " + search.range_field + " >= @dt_start ";
          if (!string.IsNullOrWhiteSpace(search.dt_end))
            selectQuery_where += (!string.IsNullOrWhiteSpace(selectQuery_where) ? " AND " : "") + " " + search.range_field + " <= @dt_end ";




          if (search.position_start > 0)
          {
            selectQuery_where += (!string.IsNullOrWhiteSpace(selectQuery_where) ? " AND " : "") + " position_seq <= @position_start ";
          }

          if (search.position_end > 0)
          {

            selectQuery_where += (!string.IsNullOrWhiteSpace(selectQuery_where) ? " AND " : "") + " position_seq >= @position_end ";
          }

          if (search.is_pe == 0 || search.is_pe == 1)
          {

            selectQuery_where += (!string.IsNullOrWhiteSpace(selectQuery_where) ? " AND " : "") + " is_pe = @is_pe ";
          }


          if (search.state > 0)
          {
            selectQuery_where += (!string.IsNullOrWhiteSpace(selectQuery_where) ? " AND " : "") + " pjt_status = @state ";
          }

          DynamicParameters param = new DynamicParameters();
          param.Add("@uv_seq", uv_seq, DbType.Int32);
          param.Add("@pjt_type", search.pjt_type, DbType.Int32);
          param.Add("@client", search.client, DbType.String);
          param.Add("@title", search.title, DbType.String);
          param.Add("@user_name", search.user_name, DbType.String);
          param.Add("@dt_start", search.dt_start, DbType.String);
          param.Add("@dt_end", search.dt_end, DbType.String);
          param.Add("@position_start", search.position_start, DbType.Int32);
          param.Add("@position_end", search.position_end, DbType.Int32);
          param.Add("@state", search.state, DbType.Int32);
          param.Add("@is_pe", search.is_pe, DbType.Int32);



          var ret = await con.QueryAsync<ProjectListNewExcelModel>(selectQuery_where, param);
          list = ret.ToList();
          return list;
        }
      }
      catch (Exception e)
      {
        throw e;
      }
    }

    public List<project> SelectPopAllProjectList2(int except_p_seq, MyProjectSearchModel search, int uv_seq, int skip, int count, out int totalCount)
    {
      try
      {
        List<project> list = new List<project>();

        using (IDbConnection con = new SqlConnection(base.BaseConnectionString))
        {
          con.Open();

          string selectQuery = @" 
  SELECT P.p_seq
        ,P.pjt_type
        ,P.pjt_status
        ,C.kor_name AS company_name
        ,P.title
        ,ISNULL(P.is_share_pjt, 0) AS is_share_pjt
        --,ISNULL(PR.state, 0) AS recState
        ,PD.am_names
        ,PM.searcher_names
        ,PB.code_name2 as business_names2
        ,PJ.code_name2 as job_names2
        ,P.create_dt
        ,CASE WHEN EXISTS (select p_seq FROM pjt_recandidate WHERE c_seq = @except_c_seq AND p_seq = P.p_seq ) THEN 1 ELSE 0 END as exist_candidate
        ,case when KY.p_seq is not null then 1 else 0 end as is_my_key
  FROM project AS P INNER JOIN client AS C
                            ON P.c_seq = C.c_seq
                   
                    INNER JOIN (SELECT distinct PD.p_seq
                                       ,STUFF((SELECT ',' + B.name
                                               FROM pjt_director AS A INNER JOIN uv_user AS B
                                                                      ON A.uv_seq = B.uv_seq
                                                    WHERE A.p_seq = PD.p_seq
                                                      FOR XML PATH('')), 1, 1, '') AS am_names
                                       FROM pjt_director AS PD) AS PD
                                 ON P.p_seq = PD.p_seq
                    INNER JOIN (SELECT distinct PM.p_seq
                                       ,STUFF((SELECT ',' + B.name
                                               FROM pjt_manager AS A INNER JOIN uv_user AS B
                                                                     ON A.uv_seq = B.uv_seq
                                               WHERE A.p_seq = PM.p_seq
                                               FOR XML PATH('')), 1, 1, '') AS searcher_names
                                       FROM pjt_manager AS PM) AS PM
                                 ON P.p_seq = PM.p_seq
                    LEFT OUTER JOIN code_business_dtl PB 
                    ON P.business_code2 = PB.code2
                    LEFT OUTER JOIN code_job_dtl PJ 
                    ON P.job_code2 = PJ.code2
                    LEFT JOIN pjt_mykey KY
                    ON P.p_seq = KY.p_seq
                    AND KY.uv_seq = @uv_seq
 WHERE P.p_seq <> @except_p_seq
 AND   P.pjt_type = 1
 AND   P.pjt_status = CASE WHEN @state <> 0 THEN @state ELSE P.pjt_status END ";
          

          if (search.pjt_type > 0)
            selectQuery += " AND P.pjt_type = @pjt_type ";
          if (!string.IsNullOrWhiteSpace(search.kor_name))
            selectQuery += " AND (C.kor_name LIKE '%' + @kor_name + '%') ";

          if (!string.IsNullOrWhiteSpace(search.searchTxt))
            selectQuery += " AND (P.title LIKE '%' + @searchTxt + '%') ";

          if (!string.IsNullOrWhiteSpace(search.searchTxt2))
            selectQuery += " AND (PD.am_names LIKE '%' + @searchTxt2 + '%' OR  PM.searcher_names LIKE '%' + @searchTxt2 + '%') ";
          else
            selectQuery += " AND (P.p_seq IN(select p_seq FROM pjt_director WHERE uv_seq = @uv_seq) OR P.p_seq IN(select p_seq FROM pjt_manager WHERE uv_seq = @uv_seq)) ";


          if (!string.IsNullOrWhiteSpace(search.startDt) && !string.IsNullOrWhiteSpace(search.endDt))
            selectQuery += " AND P.create_dt BETWEEN CONVERT(DATETIME, @startDt + ' 00:00:00') AND CONVERT(DATETIME, @endDt + ' 23:59:59') ";
          if (search.position_start > 0)
            selectQuery += " AND P.position_seq >= @position_start ";
          if (search.position_end > 0)
            selectQuery += " AND P.position_seq <= @position_end ";

          if (search.business.Count > 0)
          {
            string in_str = String.Empty;
            foreach (var code in search.business)
            {
              in_str += (!String.IsNullOrEmpty(in_str) ? ", " : "") + "'" + code.code2 + "'";
            }
            selectQuery += " AND P.business_code2 in (" + in_str + ") ";
          }

          if (search.job.Count > 0)
          {
            string in_str = String.Empty;
            foreach (var code in search.job)
            {
              in_str += (!String.IsNullOrEmpty(in_str) ? ", " : "") + "'" + code.code2 + "'";
            }
            selectQuery += " AND P.job_code2 in (" + in_str + ") ";
          }

          //if (search.except_c_seq != 0)
          //    selectQuery += " AND P.p_seq NOT IN (select p_seq FROM pjt_recandidate WHERE c_seq = @except_c_seq) ";

          //AND P.create_user = @uv_seq 
          selectQuery += string.Format(" ORDER BY CASE WHEN ISNULL(KY.p_seq, 0) > 0 THEN 1 ELSE 2 END, KY.create_dt DESC, {0} {1} ", search.orderTxt, search.orderOption);
          selectQuery += " OFFSET @currentPage ROWS FETCH NEXT @pageSize ROWS ONLY ";

          string countQuery = @" 
  SELECT COUNT(0)
  FROM project AS P INNER JOIN client AS C
                            ON P.c_seq = C.c_seq
                    INNER JOIN (SELECT PD.p_seq
                                       ,STUFF((SELECT ',' + B.name
                                                     FROM pjt_director AS A INNER JOIN uv_user AS B
                                                                                   ON A.uv_seq = B.uv_seq
                                                    WHERE A.p_seq = PD.p_seq
                                                      FOR XML PATH('')), 1, 1, '') AS am_names
                                       FROM pjt_director AS PD) AS PD
                                 ON P.p_seq = PD.p_seq
                    INNER JOIN (SELECT PM.p_seq
                                           ,STUFF((SELECT ',' + B.name
                                                     FROM pjt_manager AS A INNER JOIN uv_user AS B
                                                                                   ON A.uv_seq = B.uv_seq
                                                    WHERE A.p_seq = PM.p_seq
                                                      FOR XML PATH('')), 1, 1, '') AS searcher_names
                                       FROM pjt_manager AS PM) AS PM
                                 ON P.p_seq = PM.p_seq
 WHERE P.p_seq <> @except_p_seq
 AND   P.pjt_type = 1
 AND   P.pjt_status = CASE WHEN @state <> 0 THEN @state ELSE P.pjt_status END ";

          if (search.pjt_type > 0)
            countQuery += " AND P.pjt_type = @pjt_type ";
          if (!string.IsNullOrWhiteSpace(search.kor_name))
            countQuery += " AND (C.kor_name LIKE '%' + @kor_name + '%') ";
          if (!string.IsNullOrWhiteSpace(search.searchTxt))
            countQuery += " AND (P.title LIKE '%' + @searchTxt + '%') ";


          if (!string.IsNullOrWhiteSpace(search.searchTxt2))
            countQuery += " AND (PD.am_names LIKE '%' + @searchTxt2 + '%' OR  PM.searcher_names LIKE '%' + @searchTxt2 + '%') ";
          else
            countQuery += " AND (P.p_seq IN(select p_seq FROM pjt_director WHERE uv_seq = @uv_seq) OR P.p_seq IN(select p_seq FROM pjt_manager WHERE uv_seq = @uv_seq)) ";



          if (!string.IsNullOrWhiteSpace(search.startDt) && !string.IsNullOrWhiteSpace(search.endDt))
            countQuery += " AND P.create_dt BETWEEN CONVERT(DATETIME, @startDt + ' 00:00:00') AND CONVERT(DATETIME, @endDt + ' 23:59:59') ";
          if (search.position_start > 0)
            countQuery += " AND P.position_seq >= @position_start ";
          if (search.position_end > 0)
            countQuery += " AND P.position_seq <= @position_end ";

          if (search.business.Count > 0)
          {
            string in_str = String.Empty;
            foreach (var code in search.business)
            {
              in_str += (!String.IsNullOrEmpty(in_str) ? ", " : "") + "'" + code.code2 + "'";
            }
            countQuery += " AND P.business_code2 in (" + in_str + ") ";
          }

          if (search.job.Count > 0)
          {
            string in_str = String.Empty;
            foreach (var code in search.job)
            {
              in_str += (!String.IsNullOrEmpty(in_str) ? ", " : "") + "'" + code.code2 + "'";
            }
            countQuery += " AND P.job_code2 in (" + in_str + ") ";
          }

          //if (search.except_c_seq != 0)
          //    countQuery += " AND P.p_seq NOT IN (select p_seq FROM pjt_recandidate WHERE c_seq = @except_c_seq) ";

          DynamicParameters param = new DynamicParameters();
          param.Add("@except_p_seq", except_p_seq, DbType.Int64);
          param.Add("@state", search.state, DbType.Int64);
          param.Add("@uv_seq", uv_seq, DbType.Int64);
          param.Add("@except_c_seq", search.except_c_seq, DbType.Int64);
          param.Add("@position_start", search.position_start, DbType.Int64);
          param.Add("@position_end", search.position_end, DbType.Int64);
          param.Add("@pjt_type", search.pjt_type, DbType.String);
          param.Add("@kor_name", search.kor_name, DbType.String);
          param.Add("@searchTxt", search.searchTxt, DbType.String);
          param.Add("@searchTxt2", search.searchTxt2, DbType.String);
          param.Add("@startDt", search.startDt, DbType.String);
          param.Add("@endDt", search.endDt, DbType.String);
          param.Add("@currentPage", skip, DbType.Int32);
          param.Add("@pageSize", count, DbType.Int32);

          DynamicParameters param2 = new DynamicParameters();
          param2.Add("@except_p_seq", except_p_seq, DbType.Int64);
          param2.Add("@state", search.state, DbType.Int64);
          param2.Add("@uv_seq", uv_seq, DbType.Int64);
          param2.Add("@except_c_seq", search.except_c_seq, DbType.Int64);
          param2.Add("@position_start", search.position_start, DbType.Int64);
          param2.Add("@position_end", search.position_end, DbType.Int64);
          param2.Add("@pjt_type", search.pjt_type, DbType.String);
          param2.Add("@kor_name", search.kor_name, DbType.String);
          param2.Add("@searchTxt", search.searchTxt, DbType.String);
          param2.Add("@searchTxt2", search.searchTxt2, DbType.String);
          param2.Add("@startDt", search.startDt, DbType.String);
          param2.Add("@endDt", search.endDt, DbType.String);

          var ret = con.Query<project>(selectQuery, param);
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

    public List<project> SelectPopAllProjectList(MyProjectSearchModel search, int uv_seq, int skip, int count, out int totalCount)
    {
      try
      {
        List<project> list = new List<project>();

        using (IDbConnection con = new SqlConnection(base.BaseConnectionString))
        {
          con.Open();

          string selectQuery = @" 
  SELECT P.p_seq
        ,P.pjt_type
        ,P.pjt_status
        ,C.kor_name AS company_name
        ,P.title
        ,ISNULL(P.is_share_pjt, 0) AS is_share_pjt
        --,ISNULL(PR.state, 0) AS recState
        ,PD.am_names
        ,PM.searcher_names
        ,PB.code_name2 as business_names2
        ,PJ.code_name2 as job_names2
        ,P.create_dt
        ,CASE WHEN EXISTS (select p_seq FROM pjt_recandidate WHERE c_seq = @except_c_seq AND p_seq = P.p_seq ) THEN 1 ELSE 0 END as exist_candidate
        ,case when KY.p_seq is not null then 1 else 0 end as is_my_key
  FROM project AS P INNER JOIN client AS C
                            ON P.c_seq = C.c_seq
                   
                    INNER JOIN (SELECT distinct PD.p_seq
                                       ,STUFF((SELECT ',' + B.name
                                               FROM pjt_director AS A INNER JOIN uv_user AS B
                                                                      ON A.uv_seq = B.uv_seq
                                                    WHERE A.p_seq = PD.p_seq
                                                      FOR XML PATH('')), 1, 1, '') AS am_names
                                       FROM pjt_director AS PD) AS PD
                                 ON P.p_seq = PD.p_seq
                    INNER JOIN (SELECT distinct PM.p_seq
                                       ,STUFF((SELECT ',' + B.name
                                               FROM pjt_manager AS A INNER JOIN uv_user AS B
                                                                     ON A.uv_seq = B.uv_seq
                                               WHERE A.p_seq = PM.p_seq
                                               FOR XML PATH('')), 1, 1, '') AS searcher_names
                                       FROM pjt_manager AS PM) AS PM
                                 ON P.p_seq = PM.p_seq
                    LEFT OUTER JOIN code_business_dtl PB 
                    ON P.business_code2 = PB.code2
                    LEFT OUTER JOIN code_job_dtl PJ 
                    ON P.job_code2 = PJ.code2
                    LEFT JOIN pjt_mykey KY
                    ON P.p_seq = KY.p_seq
                    AND KY.uv_seq = @uv_seq
 WHERE P.pjt_type = 1
 AND   P.pjt_status = CASE WHEN @state <> 0 THEN @state ELSE P.pjt_status END ";
          /* LEFT OUTER JOIN (SELECT A.p_seq
                                           ,A.state
                                       FROM (SELECT PR.p_seq
                                                   ,PH.state
                                                   ,DENSE_RANK() OVER(PARTITION BY PR.p_seq ORDER BY PH.state DESC) AS rank
                                               FROM pjt_recandidate AS PR INNER JOIN pjt_recandidate_history AS PH
                                                                                  ON PR.p_seq = PH.p_seq
                                                                                 AND PR.pic_seq = PH.pic_seq
                                              GROUP BY PR.p_seq, PH.state) AS A
                                      WHERE A.rank = 1) AS PR
                                 ON P.p_seq = PR.p_seq

          */
          if (search.pjt_type > 0)
            selectQuery += " AND P.pjt_type = @pjt_type ";
          if (!string.IsNullOrWhiteSpace(search.searchTxt))
            selectQuery += " AND (P.title LIKE '%' + @searchTxt + '%' OR  C.kor_name LIKE '%' + @searchTxt + '%') ";

          if (!string.IsNullOrWhiteSpace(search.searchTxt2))
            selectQuery += " AND (PD.am_names LIKE '%' + @searchTxt2 + '%' OR  PM.searcher_names LIKE '%' + @searchTxt2 + '%') ";
          else
            selectQuery += " AND (P.p_seq IN(select p_seq FROM pjt_director WHERE uv_seq = @uv_seq) OR P.p_seq IN(select p_seq FROM pjt_manager WHERE uv_seq = @uv_seq)) ";


          if (!string.IsNullOrWhiteSpace(search.startDt) && !string.IsNullOrWhiteSpace(search.endDt))
            selectQuery += " AND P.create_dt BETWEEN CONVERT(DATETIME, @startDt + ' 00:00:00') AND CONVERT(DATETIME, @endDt + ' 23:59:59') ";
          if (search.position_start > 0)
            selectQuery += " AND P.position_seq >= @position_start ";
          if (search.position_end > 0)
            selectQuery += " AND P.position_seq <= @position_end ";

          if (search.business.Count > 0)
          {
            string in_str = String.Empty;
            foreach (var code in search.business)
            {
              in_str += (!String.IsNullOrEmpty(in_str) ? ", " : "") + "'" + code.code2 + "'";
            }
            selectQuery += " AND P.business_code2 in (" + in_str + ") ";
          }

          if (search.job.Count > 0)
          {
            string in_str = String.Empty;
            foreach (var code in search.job)
            {
              in_str += (!String.IsNullOrEmpty(in_str) ? ", " : "") + "'" + code.code2 + "'";
            }
            selectQuery += " AND P.job_code2 in (" + in_str + ") ";
          }

          //if (search.except_c_seq != 0)
          //    selectQuery += " AND P.p_seq NOT IN (select p_seq FROM pjt_recandidate WHERE c_seq = @except_c_seq) ";

          //AND P.create_user = @uv_seq 
          selectQuery += string.Format(" ORDER BY CASE WHEN ISNULL(KY.p_seq, 0) > 0 THEN 1 ELSE 2 END, KY.create_dt DESC, {0} {1} ", search.orderTxt, search.orderOption);
          selectQuery += " OFFSET @currentPage ROWS FETCH NEXT @pageSize ROWS ONLY ";

          string countQuery = @" 
  SELECT COUNT(0)
  FROM project AS P INNER JOIN client AS C
                            ON P.c_seq = C.c_seq
                    INNER JOIN (SELECT PD.p_seq
                                       ,STUFF((SELECT ',' + B.name
                                                     FROM pjt_director AS A INNER JOIN uv_user AS B
                                                                                   ON A.uv_seq = B.uv_seq
                                                    WHERE A.p_seq = PD.p_seq
                                                      FOR XML PATH('')), 1, 1, '') AS am_names
                                       FROM pjt_director AS PD) AS PD
                                 ON P.p_seq = PD.p_seq
                    INNER JOIN (SELECT PM.p_seq
                                           ,STUFF((SELECT ',' + B.name
                                                     FROM pjt_manager AS A INNER JOIN uv_user AS B
                                                                                   ON A.uv_seq = B.uv_seq
                                                    WHERE A.p_seq = PM.p_seq
                                                      FOR XML PATH('')), 1, 1, '') AS searcher_names
                                       FROM pjt_manager AS PM) AS PM
                                 ON P.p_seq = PM.p_seq
 WHERE P.pjt_status = CASE WHEN @state <> 0 THEN @state ELSE P.pjt_status END ";

          if (search.pjt_type > 0)
            countQuery += " AND P.pjt_type = @pjt_type ";
          if (!string.IsNullOrWhiteSpace(search.searchTxt))
            countQuery += " AND (P.title LIKE '%' + @searchTxt + '%' OR  C.kor_name LIKE '%' + @searchTxt + '%') ";

          if (!string.IsNullOrWhiteSpace(search.searchTxt2))
            countQuery += " AND (PD.am_names LIKE '%' + @searchTxt2 + '%' OR  PM.searcher_names LIKE '%' + @searchTxt2 + '%') ";
          else
            countQuery += " AND (P.p_seq IN(select p_seq FROM pjt_director WHERE uv_seq = @uv_seq) OR P.p_seq IN(select p_seq FROM pjt_manager WHERE uv_seq = @uv_seq)) ";



          if (!string.IsNullOrWhiteSpace(search.startDt) && !string.IsNullOrWhiteSpace(search.endDt))
            countQuery += " AND P.create_dt BETWEEN CONVERT(DATETIME, @startDt + ' 00:00:00') AND CONVERT(DATETIME, @endDt + ' 23:59:59') ";
          if (search.position_start > 0)
            countQuery += " AND P.position_seq >= @position_start ";
          if (search.position_end > 0)
            countQuery += " AND P.position_seq <= @position_end ";

          if (search.business.Count > 0)
          {
            string in_str = String.Empty;
            foreach (var code in search.business)
            {
              in_str += (!String.IsNullOrEmpty(in_str) ? ", " : "") + "'" + code.code2 + "'";
            }
            countQuery += " AND P.business_code2 in (" + in_str + ") ";
          }

          if (search.job.Count > 0)
          {
            string in_str = String.Empty;
            foreach (var code in search.job)
            {
              in_str += (!String.IsNullOrEmpty(in_str) ? ", " : "") + "'" + code.code2 + "'";
            }
            countQuery += " AND P.job_code2 in (" + in_str + ") ";
          }

          //if (search.except_c_seq != 0)
          //    countQuery += " AND P.p_seq NOT IN (select p_seq FROM pjt_recandidate WHERE c_seq = @except_c_seq) ";

          DynamicParameters param = new DynamicParameters();
          param.Add("@state", search.state, DbType.Int64);
          param.Add("@uv_seq", uv_seq, DbType.Int64);
          param.Add("@except_c_seq", search.except_c_seq, DbType.Int64);
          param.Add("@position_start", search.position_start, DbType.Int64);
          param.Add("@position_end", search.position_end, DbType.Int64);
          param.Add("@pjt_type", search.pjt_type, DbType.String);
          param.Add("@searchTxt", search.searchTxt, DbType.String);
          param.Add("@searchTxt2", search.searchTxt2, DbType.String);
          param.Add("@startDt", search.startDt, DbType.String);
          param.Add("@endDt", search.endDt, DbType.String);
          param.Add("@currentPage", skip, DbType.Int32);
          param.Add("@pageSize", count, DbType.Int32);

          DynamicParameters param2 = new DynamicParameters();
          param2.Add("@state", search.state, DbType.Int64);
          param2.Add("@uv_seq", uv_seq, DbType.Int64);
          param2.Add("@except_c_seq", search.except_c_seq, DbType.Int64);
          param2.Add("@position_start", search.position_start, DbType.Int64);
          param2.Add("@position_end", search.position_end, DbType.Int64);
          param2.Add("@pjt_type", search.pjt_type, DbType.String);
          param2.Add("@searchTxt", search.searchTxt, DbType.String);
          param2.Add("@searchTxt2", search.searchTxt2, DbType.String);
          param2.Add("@startDt", search.startDt, DbType.String);
          param2.Add("@endDt", search.endDt, DbType.String);

          var ret = con.Query<project>(selectQuery, param);
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

    public List<project> SelectAllProjectList(MyProjectSearchModel search, int skip, int count, out int totalCount)
    {
      try
      {
        List<project> list = new List<project>();

        using (IDbConnection con = new SqlConnection(base.BaseConnectionString))
        {
          con.Open();

          string selectQuery = @" 
  SELECT P.p_seq
        ,P.pjt_type
        ,P.pjt_status
        ,C.kor_name AS company_name
        ,P.title
        ,ISNULL(P.is_share_pjt, 0) AS is_share_pjt
        ,PD.am_names
        ,PM.searcher_names
        ,PB.code_name2 as business_names2
        ,PJ.code_name2  as job_names2
        ,P.create_dt
        ,P.modify_dt
  FROM project AS P INNER JOIN client AS C
                            ON P.c_seq = C.c_seq
                    INNER JOIN (SELECT DISTINCT PD.p_seq
                                       ,STUFF((SELECT ',' + B.name
                                               FROM pjt_director AS A INNER JOIN uv_user AS B
                                                                      ON A.uv_seq = B.uv_seq
                                                    WHERE A.p_seq = PD.p_seq
                                                      FOR XML PATH('')), 1, 1, '') AS am_names
                                       FROM pjt_director AS PD) AS PD
                                 ON P.p_seq = PD.p_seq
                    INNER JOIN (SELECT DISTINCT PM.p_seq
                                       ,STUFF((SELECT ',' + B.name
                                               FROM pjt_manager AS A INNER JOIN uv_user AS B
                                                                     ON A.uv_seq = B.uv_seq
                                               WHERE A.p_seq = PM.p_seq
                                               FOR XML PATH('')), 1, 1, '') AS searcher_names
                                       FROM pjt_manager AS PM) AS PM
                                 ON P.p_seq = PM.p_seq
                    LEFT OUTER JOIN code_business_dtl PB 
                    ON P.business_code2 = PB.code2
                    LEFT OUTER JOIN code_job_dtl PJ 
                    ON P.job_code2 = PJ.code2
 WHERE P.pjt_status = CASE WHEN @state <> 0 THEN @state ELSE P.pjt_status END ";

          if (search.pjt_type > 0)
            selectQuery += " AND P.pjt_type = @pjt_type ";
          if (!string.IsNullOrWhiteSpace(search.searchTxt))
            selectQuery += " AND (P.title LIKE '%' + @searchTxt + '%' OR  C.kor_name LIKE '%' + @searchTxt + '%') ";

          if (!string.IsNullOrWhiteSpace(search.searchTxt2))
            selectQuery += " AND (PD.am_names LIKE '%' + @searchTxt2 + '%' OR  PM.searcher_names LIKE '%' + @searchTxt2 + '%') ";



          if (!string.IsNullOrWhiteSpace(search.startDt) && !string.IsNullOrWhiteSpace(search.endDt))
            selectQuery += " AND P.create_dt BETWEEN CONVERT(DATETIME, @startDt + ' 00:00:00') AND CONVERT(DATETIME, @endDt + ' 23:59:59') ";
          if (search.position_start > 0)
            selectQuery += " AND P.position_seq >= @position_start ";
          if (search.position_end > 0)
            selectQuery += " AND P.position_seq <= @position_end ";

          if (search.business.Count > 0)
          {
            string in_str = String.Empty;
            foreach (var code in search.business)
            {
              in_str += (!String.IsNullOrEmpty(in_str) ? ", " : "") + "'" + code.code2 + "'";
            }
            selectQuery += " AND P.business_code2 in (" + in_str + ") ";
          }

          if (search.job.Count > 0)
          {
            string in_str = String.Empty;
            foreach (var code in search.job)
            {
              in_str += (!String.IsNullOrEmpty(in_str) ? ", " : "") + "'" + code.code2 + "'";
            }
            selectQuery += " AND P.job_code2 in (" + in_str + ") ";
          }


          //AND P.create_user = @uv_seq 
          selectQuery += string.Format(" ORDER BY {0} {1} ", search.orderTxt, search.orderOption);
          selectQuery += " OFFSET @currentPage ROWS FETCH NEXT @pageSize ROWS ONLY ";

          string countQuery = @" 
  SELECT COUNT(0)
  FROM project AS P INNER JOIN client AS C
                            ON P.c_seq = C.c_seq
                    INNER JOIN (SELECT PD.p_seq
                                       ,STUFF((SELECT ',' + B.name
                                                     FROM pjt_director AS A INNER JOIN uv_user AS B
                                                                                   ON A.uv_seq = B.uv_seq
                                                    WHERE A.p_seq = PD.p_seq
                                                      FOR XML PATH('')), 1, 1, '') AS am_names
                                       FROM pjt_director AS PD) AS PD
                                 ON P.p_seq = PD.p_seq
                    INNER JOIN (SELECT PM.p_seq
                                           ,STUFF((SELECT ',' + B.name
                                                     FROM pjt_manager AS A INNER JOIN uv_user AS B
                                                                                   ON A.uv_seq = B.uv_seq
                                                    WHERE A.p_seq = PM.p_seq
                                                      FOR XML PATH('')), 1, 1, '') AS searcher_names
                                       FROM pjt_manager AS PM) AS PM
                                 ON P.p_seq = PM.p_seq
 WHERE P.pjt_status = CASE WHEN @state <> 0 THEN @state ELSE P.pjt_status END ";

          if (search.pjt_type > 0)
            countQuery += " AND P.pjt_type = @pjt_type ";
          if (!string.IsNullOrWhiteSpace(search.searchTxt))
            countQuery += " AND (P.title LIKE '%' + @searchTxt + '%' OR  C.kor_name LIKE '%' + @searchTxt + '%') ";

          if (!string.IsNullOrWhiteSpace(search.searchTxt2))
            countQuery += " AND (PD.am_names LIKE '%' + @searchTxt2 + '%' OR  PM.searcher_names LIKE '%' + @searchTxt2 + '%') ";



          if (!string.IsNullOrWhiteSpace(search.startDt) && !string.IsNullOrWhiteSpace(search.endDt))
            countQuery += " AND P.create_dt BETWEEN CONVERT(DATETIME, @startDt + ' 00:00:00') AND CONVERT(DATETIME, @endDt + ' 23:59:59') ";
          if (search.position_start > 0)
            countQuery += " AND P.position_seq >= @position_start ";
          if (search.position_end > 0)
            countQuery += " AND P.position_seq <= @position_end ";

          if (search.business.Count > 0)
          {
            string in_str = String.Empty;
            foreach (var code in search.business)
            {
              in_str += (!String.IsNullOrEmpty(in_str) ? ", " : "") + "'" + code.code2 + "'";
            }
            countQuery += " AND P.business_code2 in (" + in_str + ") ";
          }

          if (search.job.Count > 0)
          {
            string in_str = String.Empty;
            foreach (var code in search.job)
            {
              in_str += (!String.IsNullOrEmpty(in_str) ? ", " : "") + "'" + code.code2 + "'";
            }
            countQuery += " AND P.job_code2 in (" + in_str + ") ";
          }

          DynamicParameters param = new DynamicParameters();
          param.Add("@state", search.state, DbType.Int64);
          param.Add("@position_start", search.position_start, DbType.Int64);
          param.Add("@position_end", search.position_end, DbType.Int64);
          param.Add("@pjt_type", search.pjt_type, DbType.String);
          param.Add("@searchTxt", search.searchTxt, DbType.String);
          param.Add("@searchTxt2", search.searchTxt2, DbType.String);
          param.Add("@startDt", search.startDt, DbType.String);
          param.Add("@endDt", search.endDt, DbType.String);
          param.Add("@currentPage", skip, DbType.Int32);
          param.Add("@pageSize", count, DbType.Int32);

          DynamicParameters param2 = new DynamicParameters();
          param2.Add("@state", search.state, DbType.Int64);
          param2.Add("@position_start", search.position_start, DbType.Int64);
          param2.Add("@position_end", search.position_end, DbType.Int64);
          param2.Add("@pjt_type", search.pjt_type, DbType.String);
          param2.Add("@searchTxt", search.searchTxt, DbType.String);
          param2.Add("@searchTxt2", search.searchTxt2, DbType.String);
          param2.Add("@startDt", search.startDt, DbType.String);
          param2.Add("@endDt", search.endDt, DbType.String);

          var ret = con.Query<project>(selectQuery, param);
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

    public List<project> AllopenprjListTop5(int page = 0, int page_size = 4)
    {
      try
      {
        List<project> list = new List<project>();

        using (IDbConnection con = new SqlConnection(base.BaseConnectionString))
        {
          con.Open();

          string qureyString = @"SELECT P.p_seq
        ,P.pjt_type
        ,P.pjt_status
        ,C.kor_name AS company_name
        ,P.title
        ,ISNULL(P.is_share_pjt, 0) AS is_share_pjt
        ,ISNULL(PR.state, 0) AS recState
        ,PD.am_names
        ,PM.searcher_names
        ,PB.code_name2  as business_names2
        ,PJ.code_name2  as job_names2
        ,P.create_dt
        ,P.share_fee_rate
        ,ISNULL(P.share_dt, P.cowork_dt) as share_dt
        ,P.share_comments
        ,P.is_share_pjt
        ,P.is_cowork
        ,(SELECT COUNT(*) FROM project_read_history WHERE p_seq = P.p_seq) as read_cnt
  FROM project AS P INNER JOIN client AS C
                            ON P.c_seq = C.c_seq
                    LEFT OUTER JOIN (SELECT A.p_seq
                                           ,A.state
                                       FROM (SELECT PR.p_seq
                                                   ,PH.state
                                                   ,DENSE_RANK() OVER(PARTITION BY PR.p_seq ORDER BY PH.state DESC) AS rank
                                               FROM pjt_recandidate AS PR INNER JOIN pjt_recandidate_history AS PH
                                                                                  ON PR.p_seq = PH.p_seq
                                                                                 AND PR.pic_seq = PH.pic_seq
                                              GROUP BY PR.p_seq, PH.state) AS A
                                      WHERE A.rank = 1) AS PR
                                 ON P.p_seq = PR.p_seq
                    LEFT OUTER JOIN (SELECT DISTINCT
                                            PD.p_seq
                                           ,STUFF((SELECT DISTINCT
                                                          ',' + B.name
                                                     FROM pjt_director AS A INNER JOIN uv_user AS B
                                                                                   ON A.uv_seq = B.uv_seq
                                                    WHERE A.p_seq = PD.p_seq
                                                      FOR XML PATH('')), 1, 1, '') AS am_names
                                       FROM pjt_director AS PD) AS PD
                                 ON P.p_seq = PD.p_seq
                    LEFT OUTER JOIN (SELECT DISTINCT
                                            PM.p_seq
                                           ,STUFF((SELECT DISTINCT
                                                          ',' + B.name
                                                     FROM pjt_manager AS A INNER JOIN uv_user AS B
                                                                                   ON A.uv_seq = B.uv_seq
                                                    WHERE A.p_seq = PM.p_seq
                                                      FOR XML PATH('')), 1, 1, '') AS searcher_names
                                       FROM pjt_manager AS PM) AS PM
                                 ON P.p_seq = PM.p_seq
                    LEFT OUTER JOIN code_business_dtl PB 
                    ON P.business_code2 = PB.code2
                    LEFT OUTER JOIN code_job_dtl PJ 
                    ON P.job_code2 = PJ.code2

 WHERE (is_share_pjt = 1 or is_cowork = 1)
 AND   P.pjt_type = 1
 AND   P.pjt_status = 1
 AND   (P.share_dt > DATEADD(d, -14, getdate()) or P.cowork_dt > DATEADD(d, -14, getdate()))
ORDER BY ISNULL(P.share_dt, P.cowork_dt) DESC
OFFSET @currentPage ROWS FETCH NEXT @pageSize ROWS ONLY ";

          DynamicParameters param = new DynamicParameters();
          param.Add("@currentPage", page, DbType.Int64);
          param.Add("@pageSize", page_size, DbType.Int64);
          var ret = con.Query<project>(qureyString, param);

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

    public List<project> openprjListTop5(int page = 0, int page_size = 4)
    {
      try
      {
        List<project> list = new List<project>();

        using (IDbConnection con = new SqlConnection(base.BaseConnectionString))
        {
          con.Open();

          string qureyString = @"SELECT P.p_seq
        ,P.pjt_type
        ,P.pjt_status
        ,C.kor_name AS company_name
        ,P.title
        ,ISNULL(P.is_share_pjt, 0) AS is_share_pjt
        ,ISNULL(PR.state, 0) AS recState
        ,PD.am_names
        ,PM.searcher_names
        ,PB.code_name2  as business_names2
        ,PJ.code_name2  as job_names2
        ,P.create_dt
        ,P.share_fee_rate
        ,ISNULL(P.share_dt, P.cowork_dt) as share_dt
        ,P.share_comments
        ,P.is_share_pjt
        ,P.is_cowork
        ,(SELECT COUNT(*) FROM project_read_history WHERE p_seq = P.p_seq) as read_cnt
  FROM project AS P INNER JOIN client AS C
                            ON P.c_seq = C.c_seq
                    LEFT OUTER JOIN (SELECT A.p_seq
                                           ,A.state
                                       FROM (SELECT PR.p_seq
                                                   ,PH.state
                                                   ,DENSE_RANK() OVER(PARTITION BY PR.p_seq ORDER BY PH.state DESC) AS rank
                                               FROM pjt_recandidate AS PR INNER JOIN pjt_recandidate_history AS PH
                                                                                  ON PR.p_seq = PH.p_seq
                                                                                 AND PR.pic_seq = PH.pic_seq
                                              GROUP BY PR.p_seq, PH.state) AS A
                                      WHERE A.rank = 1) AS PR
                                 ON P.p_seq = PR.p_seq
                    LEFT OUTER JOIN (SELECT DISTINCT
                                            PD.p_seq
                                           ,STUFF((SELECT DISTINCT
                                                          ',' + B.name
                                                     FROM pjt_director AS A INNER JOIN uv_user AS B
                                                                                   ON A.uv_seq = B.uv_seq
                                                    WHERE A.p_seq = PD.p_seq
                                                      FOR XML PATH('')), 1, 1, '') AS am_names
                                       FROM pjt_director AS PD) AS PD
                                 ON P.p_seq = PD.p_seq
                    LEFT OUTER JOIN (SELECT DISTINCT
                                            PM.p_seq
                                           ,STUFF((SELECT DISTINCT
                                                          ',' + B.name
                                                     FROM pjt_manager AS A INNER JOIN uv_user AS B
                                                                                   ON A.uv_seq = B.uv_seq
                                                    WHERE A.p_seq = PM.p_seq
                                                      FOR XML PATH('')), 1, 1, '') AS searcher_names
                                       FROM pjt_manager AS PM) AS PM
                                 ON P.p_seq = PM.p_seq
                    LEFT OUTER JOIN code_business_dtl PB 
                    ON P.business_code2 = PB.code2
                    LEFT OUTER JOIN code_job_dtl PJ 
                    ON P.job_code2 = PJ.code2

 WHERE (is_share_pjt = 1)
 AND   P.pjt_type = 1
 AND   P.pjt_status = 1
 AND   (P.share_dt > DATEADD(d, -14, getdate()))
ORDER BY ISNULL(P.share_dt, P.cowork_dt) DESC
OFFSET @currentPage ROWS FETCH NEXT @pageSize ROWS ONLY ";

          DynamicParameters param = new DynamicParameters();
          param.Add("@currentPage", page, DbType.Int64);
          param.Add("@pageSize", page_size, DbType.Int64);
          var ret = con.Query<project>(qureyString, param);

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


    public List<project> CoworkPrjListTop5()
    {
      try
      {
        List<project> list = new List<project>();

        using (IDbConnection con = new SqlConnection(base.BaseConnectionString))
        {
          con.Open();

          string qureyString = @"SELECT P.p_seq
        ,P.pjt_type
        ,P.pjt_status
        ,C.kor_name AS company_name
        ,P.title
        ,ISNULL(P.is_share_pjt, 0) AS is_share_pjt
        ,ISNULL(PR.state, 0) AS recState
        ,PD.am_names
        ,PM.searcher_names
        ,PB.code_name2  as business_names2
        ,PJ.code_name2  as job_names2
        ,P.create_dt
        ,P.cowork_dt
        ,(SELECT COUNT(*) FROM project_read_history WHERE p_seq = P.p_seq) as read_cnt
  FROM project AS P INNER JOIN client AS C
                            ON P.c_seq = C.c_seq
                    LEFT OUTER JOIN (SELECT A.p_seq
                                           ,A.state
                                       FROM (SELECT PR.p_seq
                                                   ,PH.state
                                                   ,DENSE_RANK() OVER(PARTITION BY PR.p_seq ORDER BY PH.state DESC) AS rank
                                               FROM pjt_recandidate AS PR INNER JOIN pjt_recandidate_history AS PH
                                                                                  ON PR.p_seq = PH.p_seq
                                                                                 AND PR.pic_seq = PH.pic_seq
                                              GROUP BY PR.p_seq, PH.state) AS A
                                      WHERE A.rank = 1) AS PR
                                 ON P.p_seq = PR.p_seq
                    LEFT OUTER JOIN (SELECT DISTINCT
                                            PD.p_seq
                                           ,STUFF((SELECT DISTINCT
                                                          ',' + B.name
                                                     FROM pjt_director AS A INNER JOIN uv_user AS B
                                                                                   ON A.uv_seq = B.uv_seq
                                                    WHERE A.p_seq = PD.p_seq
                                                      FOR XML PATH('')), 1, 1, '') AS am_names
                                       FROM pjt_director AS PD) AS PD
                                 ON P.p_seq = PD.p_seq
                    LEFT OUTER JOIN (SELECT DISTINCT
                                            PM.p_seq
                                           ,STUFF((SELECT DISTINCT
                                                          ',' + B.name
                                                     FROM pjt_manager AS A INNER JOIN uv_user AS B
                                                                                   ON A.uv_seq = B.uv_seq
                                                    WHERE A.p_seq = PM.p_seq
                                                      FOR XML PATH('')), 1, 1, '') AS searcher_names
                                       FROM pjt_manager AS PM) AS PM
                                 ON P.p_seq = PM.p_seq
                    LEFT OUTER JOIN code_business_dtl PB 
                    ON P.business_code2 = PB.code2
                    LEFT OUTER JOIN code_job_dtl PJ 
                    ON P.job_code2 = PJ.code2

 WHERE is_cowork = 1
 AND   P.pjt_type = 1
 AND   P.pjt_status = 1
ORDER BY P.cowork_dt DESC
";
          DynamicParameters param = new DynamicParameters();
          var ret = con.Query<project>(qureyString, param);

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



    public async Task<ProjectStatusCntModel> SelectAllProjectStatusCountAsync(MyProjectSearchModel search)
    {
      try
      {
        List<uv_user> list = new List<uv_user>();

        using (IDbConnection con = new SqlConnection(base.BaseConnectionString))
        {
          con.Open();

          string selectQuery = @" SELECT SUM(allCnt) AS allCnt
      ,SUM(progressCnt) AS progressCnt
      ,SUM(waitCnt) AS waitCnt
      ,SUM(failCnt) AS failCnt
      ,SUM(completeCnt) AS completeCnt
      ,SUM(successCnt) AS successCnt
  FROM (SELECT COUNT(0) AS allCnt
              ,CASE WHEN P.pjt_status = 1 THEN COUNT(P.pjt_status) ELSE 0 END AS progressCnt
              ,CASE WHEN P.pjt_status = 2 THEN COUNT(P.pjt_status) ELSE 0 END AS waitCnt
              ,CASE WHEN P.pjt_status = 3 THEN COUNT(P.pjt_status) ELSE 0 END AS failCnt
              ,CASE WHEN P.pjt_status = 4 THEN COUNT(P.pjt_status) ELSE 0 END AS completeCnt
              ,CASE WHEN P.pjt_status = 5 THEN COUNT(P.pjt_status) ELSE 0 END AS successCnt
         FROM project AS P INNER JOIN Client AS C
                                   ON P.c_seq = C.c_seq
                           LEFT OUTER JOIN (SELECT DISTINCT
                                            PD.p_seq
                                           ,STUFF((SELECT DISTINCT
                                                          ',' + B.name
                                                     FROM pjt_director AS A INNER JOIN uv_user AS B
                                                                                   ON A.uv_seq = B.uv_seq
                                                    WHERE A.p_seq = PD.p_seq
                                                      FOR XML PATH('')), 1, 1, '') AS am_names
                                       FROM pjt_director AS PD) AS PD
                                 ON P.p_seq = PD.p_seq
                    LEFT OUTER JOIN (SELECT DISTINCT
                                            PM.p_seq
                                           ,STUFF((SELECT DISTINCT
                                                          ',' + B.name
                                                     FROM pjt_manager AS A INNER JOIN uv_user AS B
                                                                                   ON A.uv_seq = B.uv_seq
                                                    WHERE A.p_seq = PM.p_seq
                                                      FOR XML PATH('')), 1, 1, '') AS searcher_names
                                       FROM pjt_manager AS PM) AS PM
                                 ON P.p_seq = PM.p_seq
        WHERE P.pjt_type = CASE WHEN @pjt_type = 0 THEN P.pjt_type ELSE @pjt_type END ";

          if (search.pjt_type > 0)
            selectQuery += " AND P.pjt_type = @pjt_type ";
          if (!string.IsNullOrWhiteSpace(search.searchTxt))
            selectQuery += " AND (P.title LIKE '%' + @searchTxt + '%' OR  C.kor_name LIKE '%' + @searchTxt + '%') ";

          if (!string.IsNullOrWhiteSpace(search.searchTxt2))
            selectQuery += " AND (PD.am_names LIKE '%' + @searchTxt2 + '%' OR  PM.searcher_names LIKE '%' + @searchTxt2 + '%') ";



          if (!string.IsNullOrWhiteSpace(search.startDt) && !string.IsNullOrWhiteSpace(search.endDt))
            selectQuery += " AND P.create_dt BETWEEN CONVERT(DATETIME, @startDt + ' 00:00:00') AND CONVERT(DATETIME, @endDt + ' 23:59:59') ";
          if (search.position_start > 0)
            selectQuery += " AND P.position_seq >= @position_start ";
          if (search.position_end > 0)
            selectQuery += " AND P.position_seq <= @position_end ";

          if (search.business.Count > 0)
          {
            string in_str = String.Empty;
            foreach (var code in search.business)
            {
              in_str += (!String.IsNullOrEmpty(in_str) ? ", " : "") + "'" + code.code2 + "'";
            }
            selectQuery += " AND P.business_code2 in (" + in_str + ") ";
          }

          if (search.job.Count > 0)
          {
            string in_str = String.Empty;
            foreach (var code in search.job)
            {
              in_str += (!String.IsNullOrEmpty(in_str) ? ", " : "") + "'" + code.code2 + "'";
            }
            selectQuery += " AND P.job_code2 in (" + in_str + ") ";
          }


          selectQuery += " GROUP BY P.pjt_status) AS A ";

          DynamicParameters param = new DynamicParameters();
          param.Add("@position_start", search.position_start, DbType.Int64);
          param.Add("@position_end", search.position_end, DbType.Int64);
          param.Add("@pjt_type", search.pjt_type, DbType.String);
          param.Add("@searchTxt", search.searchTxt, DbType.String);
          param.Add("@searchTxt2", search.searchTxt2, DbType.String);
          param.Add("@startDt", search.startDt, DbType.String);
          param.Add("@endDt", search.endDt, DbType.String);


          var ret = await con.QueryAsync<ProjectStatusCntModel>(selectQuery, param);

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
    /// 전체 프로젝트
    /// </summary>
    /// <param name="search"></param>
    /// <returns></returns>
    public async Task<List<AllProjectListExcelModel>> SelectAllProjectListWithoutCountAsnyc(MyProjectSearchModel search)
    {
      try
      {
        List<AllProjectListExcelModel> list = new List<AllProjectListExcelModel>();

        using (IDbConnection con = new SqlConnection(base.BaseConnectionString))
        {
          con.Open();

          string selectQuery = @" SELECT CASE WHEN P.pjt_type = 1 THEN '채용'
            WHEN P.pjt_type = 2 THEN '평판조회'
            WHEN P.pjt_type = 3 THEN '재취업'
            WHEN P.pjt_type = 4 THEN '기타'
            ELSE ''
       END AS '타입'
      ,CASE WHEN P.pjt_status = 1 THEN '진행'
            WHEN P.pjt_status = 2 THEN '보류'
            WHEN P.pjt_status = 3 THEN '종료'
            WHEN P.pjt_status = 4 THEN '완료'
            WHEN P.pjt_status = 5 THEN '성공'
            ELSE ''
       END AS '상태'
      ,C.kor_name AS '업체명'
      ,P.title AS '제목'
      ,CASE WHEN ISNULL(PR.state, 0) = 10 OR ISNULL(PR.state, 0) = 20 THEN '0%' 
            WHEN ISNULL(PR.state, 0) = 30 OR ISNULL(PR.state, 0) = 40 THEN '25%' 
            WHEN ISNULL(PR.state, 0) = 50 OR ISNULL(PR.state, 0) = 60 THEN '50%' 
            WHEN ISNULL(PR.state, 0) = 70 THEN '75%' 
            WHEN ISNULL(PR.state, 0) = 80 THEN '100%' 
            ELSE '0%'
        END AS '진행상태'
      ,PD.am_names AS AM
      ,PM.searcher_names AS Searcher
      ,CB2.code_name2 AS '산업'
      ,P.create_dt AS '생성일'
  FROM project AS P INNER JOIN client AS C
                            ON P.c_seq = C.c_seq
                   LEFT OUTER JOIN (SELECT A.p_seq
                                           ,A.state
                                       FROM (SELECT PR.p_seq
                                                   ,PH.state
                                                   ,DENSE_RANK() OVER(PARTITION BY PR.p_seq ORDER BY PH.state DESC) AS rank
                                               FROM pjt_recandidate AS PR INNER JOIN pjt_recandidate_history AS PH
                                                                                  ON PR.p_seq = PH.p_seq
                                                                                 AND PR.pic_seq = PH.pic_seq
                                              GROUP BY PR.p_seq, PH.state) AS A
                                      WHERE A.rank = 1) AS PR
                                 ON P.p_seq = PR.p_seq
                    LEFT OUTER JOIN (SELECT DISTINCT
                                            PD.p_seq
                                           ,STUFF((SELECT DISTINCT
                                                          ',' + B.name
                                                     FROM pjt_director AS A INNER JOIN uv_user AS B
                                                                                   ON A.uv_seq = B.uv_seq
                                                    WHERE A.p_seq = PD.p_seq
                                                      FOR XML PATH('')), 1, 1, '') AS am_names
                                       FROM pjt_director AS PD) AS PD
                                 ON P.p_seq = PD.p_seq
                    LEFT OUTER JOIN (SELECT DISTINCT
                                            PM.p_seq
                                           ,STUFF((SELECT DISTINCT
                                                          ',' + B.name
                                                     FROM pjt_manager AS A INNER JOIN uv_user AS B
                                                                                   ON A.uv_seq = B.uv_seq
                                                    WHERE A.p_seq = PM.p_seq
                                                      FOR XML PATH('')), 1, 1, '') AS searcher_names
                                       FROM pjt_manager AS PM) AS PM
                                 ON P.p_seq = PM.p_seq
                    LEFT OUTER JOIN code_job_mst AS CJ1
                            ON P.job_code1 = CJ1.code1
                   LEFT OUTER JOIN code_job_dtl AS CJ2
                                ON P.job_code1 = CJ2.code1
                               AND P.job_code2 = CJ2.code2
                   LEFT OUTER JOIN code_business_mst AS CB1
                                ON P.business_code1 = CB1.code1
                   LEFT OUTER JOIN code_business_dtl AS CB2
                                ON P.business_code1 = CB2.code1
                               AND P.business_code2 = CB2.code2
 WHERE 1 = 1 
   AND P.pjt_type = CASE WHEN @pjt_type = 0 THEN P.pjt_type ELSE @pjt_type END ";

          if (!string.IsNullOrWhiteSpace(search.searchTxt))
            selectQuery += " AND (P.title LIKE '%' + @searchTxt + '%' OR  C.kor_name LIKE '%' + @searchTxt + '%') ";

          if (search.except_c_seq != 0)
            selectQuery += " AND P.p_seq NOT IN (select p_seq FROM pjt_recandidate WHERE c_seq = @except_c_seq) ";

          if (!string.IsNullOrWhiteSpace(search.startDt) && !string.IsNullOrWhiteSpace(search.endDt))
            selectQuery += " AND P.create_dt BETWEEN CONVERT(DATETIME, @startDt + ' 00:00:00') AND CONVERT(DATETIME, @endDt + ' 23:59:59') ";

          selectQuery += string.Format(" ORDER BY {0} {1} ", search.orderTxt, search.orderOption);

          DynamicParameters param = new DynamicParameters();
          param.Add("@state", search.state, DbType.String);
          param.Add("@searchTxt", search.searchTxt, DbType.String);
          param.Add("@startDt", search.startDt, DbType.String);
          param.Add("@endDt", search.endDt, DbType.String);
          param.Add("@pjt_type", search.pjt_type, DbType.Int32);
          param.Add("@except_c_seq", search.except_c_seq, DbType.String);

          var ret = await con.QueryAsync<AllProjectListExcelModel>(selectQuery, param);

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

    public async Task<List<MyProjectKPIModel>> SelectMyProjectKPIList(int uv_seq)
    {
      try
      {
        List<MyProjectKPIModel> list = new List<MyProjectKPIModel>();

        using (IDbConnection con = new SqlConnection(base.BaseConnectionString))
        {
          con.Open();

          string selectQuery = @" select
	convert(char(7), getdate(), 120) month_name,
	sum(data1.pjt_cnt) pjt_cnt, 
	sum(data1.pjt_ing) pjt_ing,
	sum(data1.pjt_hold) pjt_hold,
	sum(data1.pjt_fail) pjt_fail,
    sum(data1.pjt_comp) pjt_comp,
	sum(data1.pjt_succ) pjt_succ,
	sum(data1.client_cnt) client_cnt, 
	sum(data1.activity_cnt) activity_cnt, 
	sum(data1.mem_cnt) mem_cnt, 
	sum(data1.inter_cnt) inter_cnt, 
	sum(data1.push_cnt) push_cnt, 
	sum(data1.interview_cnt) interview_cnt, 
	sum(data1.hire_cnt) hire_cnt
from
(
	select
		isnull(sum(case when a.pjt_status in (1,2,3,4,5) then 1 end), 0) pjt_cnt,
		isnull(sum(case when a.pjt_status = 1 then 1 end), 0) pjt_ing,
		isnull(sum(case when a.pjt_status = 2 then 1 end), 0) pjt_hold,
		isnull(sum(case when a.pjt_status = 3 then 1 end), 0) pjt_fail,
		isnull(sum(case when a.pjt_status = 4 then 1 end), 0) pjt_comp, 
        isnull(sum(case when a.pjt_status = 5 then 1 end), 0) pjt_succ, 
		'' client_cnt, '' activity_cnt, '' mem_cnt, '' inter_cnt, '' push_cnt, '' interview_cnt, '' hire_cnt
	from 
		(	select pjt_status, create_user 
			from project 
			where create_user = @uv_seq and convert(char(7), create_dt, 120) = convert(char(7), getdate(), 120)
		) a

	union all

	select
		'' pjt_cnt, '' pjt_ing, '' pjt_hold, '' pjt_fail, '' pjt_comp, '' pjt_succ, count(0) client_cnt, '' activity_cnt, '' mem_cnt, '' inter_cnt, '' push_cnt, '' interview_cnt, '' hire_cnt
	from client
	where create_user = @uv_seq and convert(char(7), create_dt, 120) = convert(char(7), getdate(), 120)

	union all

	select
		'' pjt_cnt,'' pjt_ing, '' pjt_hold, '' pjt_fail, '' pjt_comp, '' pjt_succ, '' client_cnt, count(0) activity_cnt, '' mem_cnt, '' inter_cnt, '' push_cnt, '' interview_cnt, '' hire_cnt
	from client_activity_log
	where create_user = @uv_seq and convert(char(7), create_dt, 120) = convert(char(7), getdate(), 120)

	union all

	select
		'' pjt_cnt, '' pjt_ing, '' pjt_hold, '' pjt_fail, '' pjt_comp, '' pjt_succ, '' client_cnt, '' activity_cnt, count(0) mem_cnt, '' inter_cnt, '' push_cnt, '' interview_cnt, '' hire_cnt
	from candidate
	where manager_seq = @uv_seq and convert(char(7), create_dt, 120) = convert(char(7), getdate(), 120)

	union all

	select
		'' pjt_cnt, '' pjt_ing, '' pjt_hold, '' pjt_fail, '' pjt_comp, '' pjt_succ, '' client_cnt, '' activity_cnt, '' mem_cnt,
		isnull(sum(case when a.state = 20 then 1 end), 0) inter_cnt,
		isnull(sum(case when a.state = 30 then 1 end), 0) push_cnt,
		isnull(sum(case when a.state in (50, 60, 70) then 1 end), 0) interview_cnt,
		isnull(sum(case when a.state = 80 then 1 end), 0) hire_cnt
	from 
		(	select state
			from pjt_recandidate_history 
			where create_user = @uv_seq and convert(char(7), create_dt, 120) = convert(char(7), getdate(), 120)
		) a
) data1

union all

select
	convert(char(7), dateadd(month, -1,getdate()), 120) month_name,
	sum(data2.pjt_cnt) pjt_cnt, 
	sum(data2.pjt_ing) pjt_ing,
	sum(data2.pjt_hold) pjt_hold,
	sum(data2.pjt_fail) pjt_fail,
    sum(data2.pjt_comp) pjt_comp,
	sum(data2.pjt_succ) pjt_succ,
	sum(data2.client_cnt) client_cnt, 
	sum(data2.activity_cnt) activity_cnt, 
	sum(data2.mem_cnt) mem_cnt, 
	sum(data2.inter_cnt) inter_cnt, 
	sum(data2.push_cnt) push_cnt, 
	sum(data2.interview_cnt) interview_cnt, 
	sum(data2.hire_cnt) hire_cnt
from
(
	select
		isnull(sum(case when a.pjt_status in (1,2,3,4,5) then 1 end), 0) pjt_cnt,
		isnull(sum(case when a.pjt_status = 1 then 1 end), 0) pjt_ing,
		isnull(sum(case when a.pjt_status = 2 then 1 end), 0) pjt_hold,
		isnull(sum(case when a.pjt_status = 3 then 1 end), 0) pjt_fail,
        isnull(sum(case when a.pjt_status = 4 then 1 end), 0) pjt_comp, 
		isnull(sum(case when a.pjt_status = 5 then 1 end), 0) pjt_succ, 
		'' client_cnt, '' activity_cnt, '' mem_cnt, '' inter_cnt, '' push_cnt, '' interview_cnt, '' hire_cnt
	from 
		(	select pjt_status, create_user 
			from project 
			where create_user = @uv_seq and convert(char(7), create_dt, 120) = convert(char(7), dateadd(month,-1,getdate()), 120)
		) a

	union all

	select
		'' pjt_cnt, '' pjt_ing, '' pjt_hold, '' pjt_fail, '' pjt_comp, '' pjt_succ, count(0) client_cnt, '' activity_cnt, '' mem_cnt, '' inter_cnt, '' push_cnt, '' interview_cnt, '' hire_cnt
	from client
	where create_user = @uv_seq and convert(char(7), create_dt, 120) = convert(char(7), dateadd(month,-1,getdate()), 120)

	union all

	select
		'' pjt_cnt,'' pjt_ing, '' pjt_hold, '' pjt_fail, '' pjt_comp, '' pjt_succ, '' client_cnt, count(0) activity_cnt, '' mem_cnt, '' inter_cnt, '' push_cnt, '' interview_cnt, '' hire_cnt
	from client_activity_log
	where create_user = @uv_seq and convert(char(7), create_dt, 120) = convert(char(7), dateadd(month,-1,getdate()), 120)

	union all

	select
		'' pjt_cnt, '' pjt_ing, '' pjt_hold, '' pjt_fail, '' pjt_comp, '' pjt_succ, '' client_cnt, '' activity_cnt, count(0) mem_cnt, '' inter_cnt, '' push_cnt, '' interview_cnt, '' hire_cnt
	from candidate
	where manager_seq = @uv_seq and convert(char(7), create_dt, 120) = convert(char(7), dateadd(month,-1,getdate()), 120)

	union all

	select
		'' pjt_cnt, '' pjt_ing, '' pjt_hold, '' pjt_fail, '' pjt_comp, '' pjt_succ, '' client_cnt, '' activity_cnt, '' mem_cnt,
		isnull(sum(case when a.state = 20 then 1 end), 0) inter_cnt,
		isnull(sum(case when a.state = 30 then 1 end), 0) push_cnt,
		isnull(sum(case when a.state in (50, 60, 70) then 1 end), 0) interview_cnt,
		isnull(sum(case when a.state = 80 then 1 end), 0) hire_cnt
	from 
		(	select state
			from pjt_recandidate_history 
			where create_user = @uv_seq and convert(char(7), create_dt, 120) = convert(char(7), dateadd(month, -1,getdate()), 120)
		) a
) data2

union all

select
	convert(char(7), dateadd(month, -2,getdate()), 120) month_name,
	sum(data3.pjt_cnt) pjt_cnt, 
	sum(data3.pjt_ing) pjt_ing,
	sum(data3.pjt_hold) pjt_hold,
	sum(data3.pjt_fail) pjt_fail,
    sum(data3.pjt_comp) pjt_comp,
	sum(data3.pjt_succ) pjt_succ,
	sum(data3.client_cnt) client_cnt, 
	sum(data3.activity_cnt) activity_cnt, 
	sum(data3.mem_cnt) mem_cnt, 
	sum(data3.inter_cnt) inter_cnt, 
	sum(data3.push_cnt) push_cnt, 
	sum(data3.interview_cnt) interview_cnt, 
	sum(data3.hire_cnt) hire_cnt
from
(
	select
		isnull(sum(case when a.pjt_status in (1,2,3,4,5) then 1 end), 0) pjt_cnt,
		isnull(sum(case when a.pjt_status = 1 then 1 end), 0) pjt_ing,
		isnull(sum(case when a.pjt_status = 2 then 1 end), 0) pjt_hold,
		isnull(sum(case when a.pjt_status = 3 then 1 end), 0) pjt_fail,
		isnull(sum(case when a.pjt_status = 4 then 1 end), 0) pjt_comp, 
        isnull(sum(case when a.pjt_status = 5 then 1 end), 0) pjt_succ, 
		'' client_cnt, '' activity_cnt, '' mem_cnt, '' inter_cnt, '' push_cnt, '' interview_cnt, '' hire_cnt
	from 
		(	select pjt_status, create_user 
			from project 
			where create_user = @uv_seq and convert(char(7), create_dt, 120) = convert(char(7), dateadd(month,-2,getdate()), 120)
		) a

	union all

	select
		'' pjt_cnt, '' pjt_ing, '' pjt_hold, '' pjt_fail, '' pjt_comp, '' pjt_succ, count(0) client_cnt, '' activity_cnt, '' mem_cnt, '' inter_cnt, '' push_cnt, '' interview_cnt, '' hire_cnt
	from client
	where create_user = @uv_seq and convert(char(7), create_dt, 120) = convert(char(7), dateadd(month,-2,getdate()), 120)

	union all

	select
		'' pjt_cnt,'' pjt_ing, '' pjt_hold, '' pjt_fail, '' pjt_comp, '' pjt_succ, '' client_cnt, count(0) activity_cnt, '' mem_cnt, '' inter_cnt, '' push_cnt, '' interview_cnt, '' hire_cnt
	from client_activity_log
	where create_user = @uv_seq and convert(char(7), create_dt, 120) = convert(char(7), dateadd(month,-2,getdate()), 120)

	union all

	select
		'' pjt_cnt, '' pjt_ing, '' pjt_hold, '' pjt_fail, '' pjt_comp, '' pjt_succ, '' client_cnt, '' activity_cnt, count(0) mem_cnt, '' inter_cnt, '' push_cnt, '' interview_cnt, '' hire_cnt
	from candidate
	where manager_seq = @uv_seq and convert(char(7), create_dt, 120) = convert(char(7), dateadd(month,-2,getdate()), 120)

	union all

	select
		'' pjt_cnt, '' pjt_ing, '' pjt_hold, '' pjt_fail, '' pjt_comp, '' pjt_succ, '' client_cnt, '' activity_cnt, '' mem_cnt,
		isnull(sum(case when a.state = 20 then 1 end), 0) inter_cnt,
		isnull(sum(case when a.state = 30 then 1 end), 0) push_cnt,
		isnull(sum(case when a.state in (50, 60, 70) then 1 end), 0) interview_cnt,
		isnull(sum(case when a.state = 80 then 1 end), 0) hire_cnt
	from 
		(	select state
			from pjt_recandidate_history 
			where create_user = @uv_seq and convert(char(7), create_dt, 120) = convert(char(7), dateadd(month, -2,getdate()), 120)
		) a
) data3

union all

select
	'합계' month_name,
	sum(data4.pjt_cnt) pjt_cnt, 
	sum(data4.pjt_ing) pjt_ing,
	sum(data4.pjt_hold) pjt_hold,
	sum(data4.pjt_fail) pjt_fail,
    sum(data4.pjt_succ) pjt_comp,
	sum(data4.pjt_succ) pjt_succ,
	sum(data4.client_cnt) client_cnt, 
	sum(data4.activity_cnt) activity_cnt, 
	sum(data4.mem_cnt) mem_cnt, 
	sum(data4.inter_cnt) inter_cnt, 
	sum(data4.push_cnt) push_cnt, 
	sum(data4.interview_cnt) interview_cnt, 
	sum(data4.hire_cnt) hire_cnt
from
(
	select
		isnull(sum(case when a.pjt_status in (1,2,3,4,5) then 1 end), 0) pjt_cnt,
		isnull(sum(case when a.pjt_status = 1 then 1 end), 0) pjt_ing,
		isnull(sum(case when a.pjt_status = 2 then 1 end), 0) pjt_hold,
		isnull(sum(case when a.pjt_status = 3 then 1 end), 0) pjt_fail,
		isnull(sum(case when a.pjt_status = 4 then 1 end), 0) pjt_comp, 
        isnull(sum(case when a.pjt_status = 5 then 1 end), 0) pjt_succ, 
		'' client_cnt, '' activity_cnt, '' mem_cnt, '' inter_cnt, '' push_cnt, '' interview_cnt, '' hire_cnt
	from 
		(	select pjt_status, create_user 
			from project 
			where create_user = @uv_seq and (convert(char(7), create_dt, 120) between convert(char(7), dateadd(month,-2,getdate()), 120) and convert(char(7), getdate(), 120))
		) a

	union all

	select
		'' pjt_cnt, '' pjt_ing, '' pjt_hold, '' pjt_fail, '' pjt_comp, '' pjt_succ, count(0) client_cnt, '' activity_cnt, '' mem_cnt, '' inter_cnt, '' push_cnt, '' interview_cnt, '' hire_cnt
	from client
	where create_user = @uv_seq and (convert(char(7), create_dt, 120) between convert(char(7), dateadd(month,-2,getdate()), 120) and convert(char(7), getdate(), 120))

	union all

	select
		'' pjt_cnt,'' pjt_ing, '' pjt_hold, '' pjt_fail, '' pjt_comp, '' pjt_succ, '' client_cnt, count(0) activity_cnt, '' mem_cnt, '' inter_cnt, '' push_cnt, '' interview_cnt, '' hire_cnt
	from client_activity_log
	where create_user = @uv_seq and (convert(char(7), create_dt, 120) between convert(char(7), dateadd(month,-2,getdate()), 120) and convert(char(7), getdate(), 120))

	union all

	select
		'' pjt_cnt, '' pjt_ing, '' pjt_hold, '' pjt_fail, '' pjt_comp, '' pjt_succ, '' client_cnt, '' activity_cnt, count(0) mem_cnt, '' inter_cnt, '' push_cnt, '' interview_cnt, '' hire_cnt
	from candidate
	where manager_seq = @uv_seq and (convert(char(7), create_dt, 120) between convert(char(7), dateadd(month,-2,getdate()), 120) and convert(char(7), getdate(), 120))

	union all

	select
		'' pjt_cnt, '' pjt_ing, '' pjt_hold, '' pjt_fail, '' pjt_comp, '' pjt_succ, '' client_cnt, '' activity_cnt, '' mem_cnt,
		isnull(sum(case when a.state = 20 then 1 end), 0) inter_cnt,
		isnull(sum(case when a.state = 30 then 1 end), 0) push_cnt,
		isnull(sum(case when a.state in (50, 60, 70) then 1 end), 0) interview_cnt,
		isnull(sum(case when a.state = 80 then 1 end), 0) hire_cnt
	from 
		(	select state
			from pjt_recandidate_history 
			where create_user = @uv_seq and (convert(char(7), create_dt, 120) between convert(char(7), dateadd(month,-2,getdate()), 120) and convert(char(7), getdate(), 120))
		) a
) data4 ";

          DynamicParameters param = new DynamicParameters();
          param.Add("@uv_seq", uv_seq, DbType.Int32);

          var ret = await con.QueryAsync<MyProjectKPIModel>(selectQuery, param);

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
    /// 마이프로젝트 - 채용 (페이징, 카운트 없이)
    /// </summary>
    /// <param name="search"></param>
    /// <param name="uv_seq"></param>
    /// <returns></returns>
    public async Task<List<ProjectListExcelModel>> SelectMyProjectListWithoutCountAsync(MyProjectSearchModel search, int uv_seq)
    {
      try
      {
        List<ProjectListExcelModel> list = new List<ProjectListExcelModel>();

        using (IDbConnection con = new SqlConnection(base.BaseConnectionString))
        {
          con.Open();

          string selectQuery = @" SELECT CASE WHEN P.pjt_status = 1 THEN '진행'
            WHEN P.pjt_status = 2 THEN '보류'
            WHEN P.pjt_status = 3 THEN '종료'
            WHEN P.pjt_status = 4 THEN '완료'
            WHEN P.pjt_status = 5 THEN '성공'
            ELSE ''
       END AS '상태'
      ,C.kor_name AS '업체명'
      ,P.title AS '제목'
      ,CASE WHEN ISNULL(PR.state, 0) = 10 OR ISNULL(PR.state, 0) = 20 THEN '0%' 
            WHEN ISNULL(PR.state, 0) = 30 OR ISNULL(PR.state, 0) = 40 THEN '25%' 
            WHEN ISNULL(PR.state, 0) = 50 OR ISNULL(PR.state, 0) = 60 THEN '50%' 
            WHEN ISNULL(PR.state, 0) = 70 THEN '75%' 
            WHEN ISNULL(PR.state, 0) = 80 THEN '100%' 
            ELSE '0%'
        END AS '진행상태'
      ,PD.am_names AS AM
      ,PM.searcher_names AS Searcher
      ,CB2.code_name2 AS '산업'
      ,P.create_dt AS '생성일'
  FROM project AS P INNER JOIN client AS C
                            ON P.c_seq = C.c_seq
                    LEFT OUTER JOIN (SELECT A.p_seq
                                           ,A.state
                                       FROM (SELECT PR.p_seq
                                                   ,PH.state
                                                   ,DENSE_RANK() OVER(PARTITION BY PR.p_seq ORDER BY PH.state DESC) AS rank
                                               FROM pjt_recandidate AS PR INNER JOIN pjt_recandidate_history AS PH
                                                                                  ON PR.p_seq = PH.p_seq
                                                                                 AND PR.pic_seq = PH.pic_seq
                                              GROUP BY PR.p_seq, PH.state) AS A
                                      WHERE A.rank = 1) AS PR
                                 ON P.p_seq = PR.p_seq
                    LEFT OUTER JOIN (SELECT DISTINCT
                                            PD.p_seq
                                           ,STUFF((SELECT DISTINCT
                                                          ',' + B.name
                                                     FROM pjt_director AS A INNER JOIN uv_user AS B
                                                                                   ON A.uv_seq = B.uv_seq
                                                    WHERE A.p_seq = PD.p_seq
                                                      FOR XML PATH('')), 1, 1, '') AS am_names
                                       FROM pjt_director AS PD) AS PD
                                 ON P.p_seq = PD.p_seq
                    LEFT OUTER JOIN (SELECT DISTINCT
                                            PM.p_seq
                                           ,STUFF((SELECT DISTINCT
                                                          ',' + B.name
                                                     FROM pjt_manager AS A INNER JOIN uv_user AS B
                                                                                   ON A.uv_seq = B.uv_seq
                                                    WHERE A.p_seq = PM.p_seq
                                                      FOR XML PATH('')), 1, 1, '') AS searcher_names
                                       FROM pjt_manager AS PM) AS PM
                                 ON P.p_seq = PM.p_seq
                    LEFT OUTER JOIN code_job_mst AS CJ1
                            ON P.job_code1 = CJ1.code1
               LEFT OUTER JOIN code_job_dtl AS CJ2
                            ON P.job_code1 = CJ2.code1
                           AND P.job_code2 = CJ2.code2
               LEFT OUTER JOIN code_business_mst AS CB1
                            ON P.business_code1 = CB1.code1
               LEFT OUTER JOIN code_business_dtl AS CB2
                            ON P.business_code1 = CB2.code1
                           AND P.business_code2 = CB2.code2
 WHERE P.pjt_type = 1
   AND (P.p_seq IN (select p_seq FROM pjt_director WHERE uv_seq = @uv_seq) OR P.p_seq IN (select p_seq FROM pjt_manager WHERE uv_seq = @uv_seq))
   AND P.pjt_status = CASE WHEN @state <> 0 THEN @state ELSE P.pjt_status END ";

          if (!string.IsNullOrWhiteSpace(search.searchTxt))
            selectQuery += " AND (P.title LIKE '%' + @searchTxt + '%' OR  C.kor_name LIKE '%' + @searchTxt + '%') ";

          if (search.except_c_seq != 0)
            selectQuery += " AND P.p_seq NOT IN (select p_seq FROM pjt_recandidate WHERE c_seq = @except_c_seq) ";

          if (!string.IsNullOrWhiteSpace(search.startDt) && !string.IsNullOrWhiteSpace(search.endDt))
            selectQuery += " AND P.create_dt BETWEEN CONVERT(DATETIME, @startDt + ' 00:00:00') AND CONVERT(DATETIME, @endDt + ' 23:59:59') ";

          //AND P.create_user = @uv_seq 
          selectQuery += string.Format(" ORDER BY {0} {1} ", search.orderTxt, search.orderOption);

          DynamicParameters param = new DynamicParameters();
          param.Add("@uv_seq", uv_seq, DbType.String);
          param.Add("@state", search.state, DbType.String);
          param.Add("@startDt", search.startDt, DbType.String);
          param.Add("@endDt", search.endDt, DbType.String);
          param.Add("@searchTxt", search.searchTxt, DbType.String);
          param.Add("@except_c_seq", search.except_c_seq, DbType.String);

          var ret = await con.QueryAsync<ProjectListExcelModel>(selectQuery, param);

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
    /// 마이프로젝트 리스트 채용
    /// </summary>
    /// <param name="searchTxt"></param>
    /// <param name="uv_seq"></param>
    /// <param name="skip"></param>
    /// <param name="count"></param>
    /// <param name="totalCount"></param>
    /// <returns></returns>
    public List<project> SelectMyProjectList(MyProjectSearchModel search, int uv_seq, int skip, int count, out int totalCount)
    {
      try
      {
        List<project> list = new List<project>();

        using (IDbConnection con = new SqlConnection(base.BaseConnectionString))
        {
          con.Open();

          string selectQuery = @" 
  SELECT P.p_seq
        ,P.pjt_type
        ,P.pjt_status
        ,C.c_seq
        ,C.kor_name AS company_name
        ,P.title
        ,ISNULL(P.is_share_pjt, 0) AS is_share_pjt
        ,ISNULL(P.is_cowork, 0) AS is_cowork
        ,PD.am_names
        ,PM.searcher_names
        ,PB.code_name2  as business_names2
        ,PJ.code_name2  as job_names2
        ,SPB.code_name2 as sub_business_names2
        ,SPJ.code_name2 as sub_job_names2
        ,P.create_dt
        ,P.modify_dt
  FROM project AS P INNER JOIN client AS C
                            ON P.c_seq = C.c_seq
                    LEFT OUTER JOIN (SELECT DISTINCT
                                            PD.p_seq
                                           ,STUFF((SELECT DISTINCT
                                                          ',' + B.name
                                                     FROM pjt_director AS A INNER JOIN uv_user AS B
                                                                                   ON A.uv_seq = B.uv_seq
                                                    WHERE A.p_seq = PD.p_seq
                                                      FOR XML PATH('')), 1, 1, '') AS am_names
                                       FROM pjt_director AS PD) AS PD
                                 ON P.p_seq = PD.p_seq
                    LEFT OUTER JOIN (SELECT DISTINCT
                                            PM.p_seq
                                           ,STUFF((SELECT DISTINCT
                                                          ',' + B.name
                                                     FROM pjt_manager AS A INNER JOIN uv_user AS B
                                                                                   ON A.uv_seq = B.uv_seq
                                                    WHERE A.p_seq = PM.p_seq
                                                      FOR XML PATH('')), 1, 1, '') AS searcher_names
                                       FROM pjt_manager AS PM) AS PM
                                 ON P.p_seq = PM.p_seq
                    LEFT OUTER JOIN code_business_dtl PB 
                    ON P.business_code2 = PB.code2
                    LEFT OUTER JOIN code_job_dtl PJ 
                    ON P.job_code2 = PJ.code2
                    LEFT OUTER JOIN code_business_dtl SPB 
                    ON P.sub_business_code2 = SPB.code2
                    LEFT OUTER JOIN code_job_dtl SPJ 
                    ON P.sub_job_code2 = SPJ.code2
                    
 WHERE (P.p_seq IN (select p_seq FROM pjt_director WHERE uv_seq = @uv_seq) OR P.p_seq IN (select p_seq FROM pjt_manager WHERE uv_seq = @uv_seq))
   AND P.pjt_status = CASE WHEN @state <> 0 THEN @state ELSE P.pjt_status END ";

          if (search.pjt_type > 0)
            selectQuery += " AND P.pjt_type = @pjt_type ";
          if (!string.IsNullOrWhiteSpace(search.searchTxt))
            selectQuery += " AND (P.title LIKE '%' + @searchTxt + '%' OR  C.kor_name LIKE '%' + @searchTxt + '%') ";
          if (!string.IsNullOrWhiteSpace(search.startDt) && !string.IsNullOrWhiteSpace(search.endDt))
            selectQuery += " AND P.create_dt BETWEEN CONVERT(DATETIME, @startDt + ' 00:00:00') AND CONVERT(DATETIME, @endDt + ' 23:59:59') ";
          if (search.position_start > 0)
            selectQuery += " AND P.position_seq >= @position_start ";
          if (search.position_end > 0)
            selectQuery += " AND P.position_seq <= @position_end ";

          if (search.business.Count > 0)
          {
            string in_str = String.Empty;
            foreach (var code in search.business)
            {
              in_str += (!String.IsNullOrEmpty(in_str) ? ", " : "") + "'" + code.code2 + "'";
            }
            selectQuery += " AND (P.business_code2 in (" + in_str + ") or P.sub_business_code2 in (" + in_str + ")) ";
          }

          if (search.job.Count > 0)
          {
            string in_str = String.Empty;
            foreach (var code in search.job)
            {
              in_str += (!String.IsNullOrEmpty(in_str) ? ", " : "") + "'" + code.code2 + "'";
            }
            selectQuery += " AND (P.job_code2 in (" + in_str + ") or P.sub_job_code2 in (" + in_str + ")) ";
          }

          //AND P.create_user = @uv_seq 
          selectQuery += string.Format(" ORDER BY {0} {1} ", search.orderTxt, search.orderOption);
          selectQuery += " OFFSET @currentPage ROWS FETCH NEXT @pageSize ROWS ONLY ";

          string countQuery = @" 
  SELECT COUNT(0)
  FROM project AS P INNER JOIN client AS C
                            ON P.c_seq = C.c_seq
                    LEFT OUTER JOIN (SELECT DISTINCT
                                            PD.p_seq
                                           ,STUFF((SELECT DISTINCT
                                                          ',' + B.name
                                                     FROM pjt_director AS A INNER JOIN uv_user AS B
                                                                                   ON A.uv_seq = B.uv_seq
                                                    WHERE A.p_seq = PD.p_seq
                                                      FOR XML PATH('')), 1, 1, '') AS am_names
                                       FROM pjt_director AS PD) AS PD
                                 ON P.p_seq = PD.p_seq
                    LEFT OUTER JOIN (SELECT DISTINCT
                                            PM.p_seq
                                           ,STUFF((SELECT DISTINCT
                                                          ',' + B.name
                                                     FROM pjt_manager AS A INNER JOIN uv_user AS B
                                                                                   ON A.uv_seq = B.uv_seq
                                                    WHERE A.p_seq = PM.p_seq
                                                      FOR XML PATH('')), 1, 1, '') AS searcher_names
                                       FROM pjt_manager AS PM) AS PM
                                 ON P.p_seq = PM.p_seq
                    LEFT OUTER JOIN code_business_dtl PB 
                    ON P.business_code2 = PB.code2
                    LEFT OUTER JOIN code_job_dtl PJ 
                    ON P.job_code2 = PJ.code2
 WHERE P.pjt_status = CASE WHEN @state <> 0 THEN @state ELSE P.pjt_status END 
   AND (P.p_seq IN (select p_seq FROM pjt_director WHERE uv_seq = @uv_seq) OR P.p_seq IN (select p_seq FROM pjt_manager WHERE uv_seq = @uv_seq)) ";

          if (search.pjt_type > 0)
            countQuery += " AND P.pjt_type = @pjt_type ";
          if (!string.IsNullOrWhiteSpace(search.searchTxt))
            countQuery += " AND (P.title LIKE '%' + @searchTxt + '%' OR  C.kor_name LIKE '%' + @searchTxt + '%') ";
          if (!string.IsNullOrWhiteSpace(search.startDt) && !string.IsNullOrWhiteSpace(search.endDt))
            countQuery += " AND P.create_dt BETWEEN CONVERT(DATETIME, @startDt + ' 00:00:00') AND CONVERT(DATETIME, @endDt + ' 23:59:59') ";
          if (search.position_start > 0)
            countQuery += " AND P.position_seq >= @position_start ";
          if (search.position_end > 0)
            countQuery += " AND P.position_seq <= @position_end ";

          if (search.business.Count > 0)
          {
            string in_str = String.Empty;
            foreach (var code in search.business)
            {
              in_str += (!String.IsNullOrEmpty(in_str) ? ", " : "") + "'" + code.code2 + "'";
            }
            countQuery += " AND (P.business_code2 in (" + in_str + ") or P.sub_business_code2 in (" + in_str + ")) ";
          }

          if (search.job.Count > 0)
          {
            string in_str = String.Empty;
            foreach (var code in search.job)
            {
              in_str += (!String.IsNullOrEmpty(in_str) ? ", " : "") + "'" + code.code2 + "'";
            }
            countQuery += " AND (P.job_code2 in (" + in_str + ") or P.sub_job_code2 in (" + in_str + ")) ";
          }

          DynamicParameters param = new DynamicParameters();
          param.Add("@uv_seq", uv_seq, DbType.String);
          param.Add("@state", search.state, DbType.String);
          param.Add("@position_start", search.position_start, DbType.Int64);
          param.Add("@position_end", search.position_end, DbType.Int64);
          param.Add("@pjt_type", search.pjt_type, DbType.String);
          param.Add("@searchTxt", search.searchTxt, DbType.String);
          param.Add("@startDt", search.startDt, DbType.String);
          param.Add("@endDt", search.endDt, DbType.String);
          param.Add("@except_c_seq", search.except_c_seq, DbType.String);
          param.Add("@currentPage", skip, DbType.Int32);
          param.Add("@pageSize", count, DbType.Int32);





          DynamicParameters param2 = new DynamicParameters();
          param2.Add("@uv_seq", uv_seq, DbType.String);
          param2.Add("@state", search.state, DbType.String);
          param2.Add("@position_start", search.position_start, DbType.Int64);
          param2.Add("@position_end", search.position_end, DbType.Int64);
          param2.Add("@pjt_type", search.pjt_type, DbType.String);
          param2.Add("@searchTxt", search.searchTxt, DbType.String);
          param2.Add("@startDt", search.startDt, DbType.String);
          param2.Add("@endDt", search.endDt, DbType.String);
          param2.Add("@except_c_seq", search.except_c_seq, DbType.String);

          var ret = con.Query<project>(selectQuery, param);
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
    /// 
    /// </summary>
    /// <param name="search"></param>
    /// <param name="uv_seq"></param>
    /// <returns></returns>
    public async Task<List<ProjectRepListExcelModel>> SelectMyProjectRepListWithoutAsync(MyProjectSearchModel search, int uv_seq)
    {
      try
      {
        List<ProjectRepListExcelModel> list = new List<ProjectRepListExcelModel>();

        using (IDbConnection con = new SqlConnection(base.BaseConnectionString))
        {
          con.Open();

          string selectQuery = @" SELECT CASE WHEN P.pjt_status = 1 THEN '진행'
            WHEN P.pjt_status = 2 THEN '보류'
            WHEN P.pjt_status = 3 THEN '종료'
            WHEN P.pjt_status = 4 THEN '완료'
            WHEN P.pjt_status = 5 THEN '성공'
            ELSE ''
       END AS '상태'
      ,C.kor_name AS '업체명'
      ,P.title AS '제목'
      ,PD.am_names AS AM
      ,PM.searcher_names AS Searcher
      ,P.create_dt AS '생성일'
  FROM project AS P INNER JOIN client AS C
                            ON P.c_seq = C.c_seq
                    LEFT OUTER JOIN (SELECT DISTINCT
                                            PD.p_seq
                                           ,STUFF((SELECT DISTINCT
                                                          ',' + B.name
                                                     FROM pjt_director AS A INNER JOIN uv_user AS B
                                                                                   ON A.uv_seq = B.uv_seq
                                                    WHERE A.p_seq = PD.p_seq
                                                      FOR XML PATH('')), 1, 1, '') AS am_names
                                       FROM pjt_director AS PD) AS PD
                                 ON P.p_seq = PD.p_seq
                    LEFT OUTER JOIN (SELECT DISTINCT
                                            PM.p_seq
                                           ,STUFF((SELECT DISTINCT
                                                          ',' + B.name
                                                     FROM pjt_manager AS A INNER JOIN uv_user AS B
                                                                                   ON A.uv_seq = B.uv_seq
                                                    WHERE A.p_seq = PM.p_seq
                                                      FOR XML PATH('')), 1, 1, '') AS searcher_names
                                       FROM pjt_manager AS PM) AS PM
                                 ON P.p_seq = PM.p_seq
  WHERE P.pjt_type = 2
    AND (P.p_seq IN (select p_seq FROM pjt_director WHERE uv_seq = @uv_seq) OR P.p_seq IN (select p_seq FROM pjt_manager WHERE uv_seq = @uv_seq))
    AND P.pjt_status = CASE WHEN @state <> 0 THEN @state ELSE P.pjt_status END ";

          if (!string.IsNullOrWhiteSpace(search.searchTxt))
            selectQuery += " AND (P.title LIKE '%' + @searchTxt + '%' OR  C.kor_name LIKE '%' + @searchTxt + '%') ";

          if (!string.IsNullOrWhiteSpace(search.startDt) && !string.IsNullOrWhiteSpace(search.endDt))
            selectQuery += " AND P.create_dt BETWEEN CONVERT(DATETIME, @startDt + ' 00:00:00') AND CONVERT(DATETIME, @endDt + ' 23:59:59') ";

          selectQuery += string.Format(" ORDER BY {0} {1} ", search.orderTxt, search.orderOption);

          DynamicParameters param = new DynamicParameters();
          param.Add("@uv_seq", uv_seq, DbType.String);
          param.Add("@state", search.state, DbType.String);
          param.Add("@startDt", search.startDt, DbType.String);
          param.Add("@endDt", search.endDt, DbType.String);
          param.Add("@searchTxt", search.searchTxt, DbType.String);

          var ret = await con.QueryAsync<ProjectRepListExcelModel>(selectQuery, param);

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
    /// 마이프로젝트 리스트 평판조회
    /// </summary>
    /// <param name="search"></param>
    /// <param name="uv_seq"></param>
    /// <param name="skip"></param>
    /// <param name="count"></param>
    /// <param name="totalCount"></param>
    /// <returns></returns>
    public List<project> SelectMyProjectRepList(MyProjectSearchModel search, int uv_seq, int skip, int count, out int totalCount)
    {
      try
      {
        List<project> list = new List<project>();

        using (IDbConnection con = new SqlConnection(base.BaseConnectionString))
        {
          con.Open();

          string selectQuery = @" SELECT P.p_seq
      ,P.pjt_status
      ,C.kor_name AS company_name
      ,P.title
      ,PD.am_names
      ,PM.searcher_names
      ,P.create_dt
  FROM project AS P INNER JOIN client AS C
                            ON P.c_seq = C.c_seq
                    LEFT OUTER JOIN (SELECT DISTINCT
                                            PD.p_seq
                                           ,STUFF((SELECT DISTINCT
                                                          ',' + B.name
                                                     FROM pjt_director AS A INNER JOIN uv_user AS B
                                                                                   ON A.uv_seq = B.uv_seq
                                                    WHERE A.p_seq = PD.p_seq
                                                      FOR XML PATH('')), 1, 1, '') AS am_names
                                       FROM pjt_director AS PD) AS PD
                                 ON P.p_seq = PD.p_seq
                    LEFT OUTER JOIN (SELECT DISTINCT
                                            PM.p_seq
                                           ,STUFF((SELECT DISTINCT
                                                          ',' + B.name
                                                     FROM pjt_manager AS A INNER JOIN uv_user AS B
                                                                                   ON A.uv_seq = B.uv_seq
                                                    WHERE A.p_seq = PM.p_seq
                                                      FOR XML PATH('')), 1, 1, '') AS searcher_names
                                       FROM pjt_manager AS PM) AS PM
                                 ON P.p_seq = PM.p_seq
  WHERE P.pjt_type > 1
    AND P.create_user = @uv_seq 
    AND P.pjt_status = CASE WHEN @state <> 0 THEN @state ELSE P.pjt_status END ";

          if (!string.IsNullOrWhiteSpace(search.searchTxt))
            selectQuery += " AND (P.title LIKE '%' + @searchTxt + '%' OR  C.kor_name LIKE '%' + @searchTxt + '%') ";

          if (!string.IsNullOrWhiteSpace(search.startDt) && !string.IsNullOrWhiteSpace(search.endDt))
            selectQuery += " AND P.create_dt BETWEEN CONVERT(DATETIME, @startDt + ' 00:00:00') AND CONVERT(DATETIME, @endDt + ' 23:59:59') ";

          selectQuery += string.Format(" ORDER BY {0} {1} ", search.orderTxt, search.orderOption);
          selectQuery += " OFFSET @currentPage ROWS FETCH NEXT @pageSize ROWS ONLY ";

          string countQuery = @" SELECT COUNT(0)
  FROM project AS P INNER JOIN client AS C
                            ON P.c_seq = C.c_seq
  WHERE P.pjt_type > 1
    AND P.pjt_status = CASE WHEN @state <> 0 THEN @state ELSE P.pjt_status END 
    AND P.create_user = @uv_seq ";

          if (!string.IsNullOrWhiteSpace(search.searchTxt))
            countQuery += " AND (P.title LIKE '%' + @searchTxt + '%' OR  C.kor_name LIKE '%' + @searchTxt + '%') ";

          if (!string.IsNullOrWhiteSpace(search.startDt) && !string.IsNullOrWhiteSpace(search.endDt))
            countQuery += " AND P.create_dt BETWEEN CONVERT(DATETIME, @startDt + ' 00:00:00') AND CONVERT(DATETIME, @endDt + ' 23:59:59') ";

          DynamicParameters param = new DynamicParameters();
          param.Add("@uv_seq", uv_seq, DbType.String);
          param.Add("@state", search.state, DbType.String);
          param.Add("@searchTxt", search.searchTxt, DbType.String);
          param.Add("@startDt", search.startDt, DbType.String);
          param.Add("@endDt", search.endDt, DbType.String);
          param.Add("@currentPage", skip, DbType.Int32);
          param.Add("@pageSize", count, DbType.Int32);

          DynamicParameters param2 = new DynamicParameters();
          param2.Add("@uv_seq", uv_seq, DbType.String);
          param2.Add("@state", search.state, DbType.String);
          param2.Add("@searchTxt", search.searchTxt, DbType.String);
          param2.Add("@startDt", search.startDt, DbType.String);
          param2.Add("@endDt", search.endDt, DbType.String);

          var ret = con.Query<project>(selectQuery, param);
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
    /// 마이프로젝트 리스트 상태별 카운트 - 채용
    /// </summary>
    /// <param name="search"></param>
    /// <param name="uv_seq"></param>
    /// <returns></returns>
    public async Task<ProjectStatusCntModel> SelectProjectStatusCountAsync(MyProjectSearchModel search, int uv_seq)
    {
      try
      {
        List<uv_user> list = new List<uv_user>();

        using (IDbConnection con = new SqlConnection(base.BaseConnectionString))
        {
          con.Open();

          string selectQuery = @" SELECT SUM(allCnt) AS allCnt
      ,SUM(progressCnt) AS progressCnt
      ,SUM(waitCnt) AS waitCnt
      ,SUM(failCnt) AS failCnt
      ,SUM(completeCnt) AS completeCnt
      ,SUM(successCnt) AS successCnt
  FROM (SELECT COUNT(0) AS allCnt
              ,CASE WHEN P.pjt_status = 1 THEN COUNT(P.pjt_status) ELSE 0 END AS progressCnt
              ,CASE WHEN P.pjt_status = 2 THEN COUNT(P.pjt_status) ELSE 0 END AS waitCnt
              ,CASE WHEN P.pjt_status = 3 THEN COUNT(P.pjt_status) ELSE 0 END AS failCnt
              ,CASE WHEN P.pjt_status = 4 THEN COUNT(P.pjt_status) ELSE 0 END AS completeCnt
              ,CASE WHEN P.pjt_status = 5 THEN COUNT(P.pjt_status) ELSE 0 END AS successCnt
         FROM project AS P INNER JOIN Client AS C
                                   ON P.c_seq = C.c_seq
        WHERE (P.p_seq IN (select p_seq FROM pjt_director WHERE uv_seq = @uv_seq) OR P.p_seq IN (select p_seq FROM pjt_manager WHERE uv_seq = @uv_seq)) ";

          if (search.pjt_type > 0)
            selectQuery += " AND P.pjt_type = @pjt_type ";
          if (!string.IsNullOrWhiteSpace(search.searchTxt))
            selectQuery += " AND (P.title LIKE '%' + @searchTxt + '%' OR  C.kor_name LIKE '%' + @searchTxt + '%') ";
          if (!string.IsNullOrWhiteSpace(search.startDt) && !string.IsNullOrWhiteSpace(search.endDt))
            selectQuery += " AND P.create_dt BETWEEN CONVERT(DATETIME, @startDt + ' 00:00:00') AND CONVERT(DATETIME, @endDt + ' 23:59:59') ";
          if (search.position_start > 0)
            selectQuery += " AND P.position_seq >= @position_start ";
          if (search.position_end > 0)
            selectQuery += " AND P.position_seq <= @position_end ";

          if (search.business.Count > 0)
          {
            string in_str = String.Empty;
            foreach (var code in search.business)
            {
              in_str += (!String.IsNullOrEmpty(in_str) ? ", " : "") + "'" + code.code2 + "'";
            }
            selectQuery += " AND P.business_code2 in (" + in_str + ") ";
          }

          if (search.job.Count > 0)
          {
            string in_str = String.Empty;
            foreach (var code in search.job)
            {
              in_str += (!String.IsNullOrEmpty(in_str) ? ", " : "") + "'" + code.code2 + "'";
            }
            selectQuery += " AND P.job_code2 in (" + in_str + ") ";
          }


          selectQuery += " GROUP BY P.pjt_status) AS A ";

          DynamicParameters param = new DynamicParameters();
          param.Add("@uv_seq", uv_seq, DbType.Int64);
          param.Add("@position_start", search.position_start, DbType.Int64);
          param.Add("@position_end", search.position_end, DbType.Int64);
          param.Add("@pjt_type", search.pjt_type, DbType.String);
          param.Add("@searchTxt", search.searchTxt, DbType.String);
          param.Add("@startDt", search.startDt, DbType.String);
          param.Add("@endDt", search.endDt, DbType.String);

          var ret = await con.QueryAsync<ProjectStatusCntModel>(selectQuery, param);

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
    /// 마이프로젝트 리스트 상태별 카운트 - 평판조회
    /// </summary>
    /// <param name="search"></param>
    /// <param name="uv_seq"></param>
    /// <returns></returns>
    public async Task<ProjectStatusCntModel> SelectProjectRepStatusCountAsync(MyProjectSearchModel search, int uv_seq)
    {
      try
      {
        List<uv_user> list = new List<uv_user>();

        using (IDbConnection con = new SqlConnection(base.BaseConnectionString))
        {
          con.Open();

          string selectQuery = @" SELECT SUM(allCnt) AS allCnt
      ,SUM(progressCnt) AS progressCnt
      ,SUM(waitCnt) AS waitCnt
      ,SUM(failCnt) AS failCnt
      ,SUM(completeCnt) AS completeCnt
      ,SUM(successCnt) AS successCnt
  FROM (SELECT COUNT(0) AS allCnt
              ,CASE WHEN P.pjt_status = 1 THEN COUNT(P.pjt_status) ELSE 0 END AS progressCnt
              ,CASE WHEN P.pjt_status = 2 THEN COUNT(P.pjt_status) ELSE 0 END AS waitCnt
              ,CASE WHEN P.pjt_status = 3 THEN COUNT(P.pjt_status) ELSE 0 END AS failCnt
              ,CASE WHEN P.pjt_status = 4 THEN COUNT(P.pjt_status) ELSE 0 END AS completeCnt
              ,CASE WHEN P.pjt_status = 5 THEN COUNT(P.pjt_status) ELSE 0 END AS successCnt
         FROM project AS P INNER JOIN Client AS C
                                   ON P.c_seq = C.c_seq
        WHERE P.pjt_type > 1
          AND (P.p_seq IN (select p_seq FROM pjt_director WHERE uv_seq = @uv_seq) OR P.p_seq IN (select p_seq FROM pjt_manager WHERE uv_seq = @uv_seq)) ";

          if (!string.IsNullOrWhiteSpace(search.searchTxt))
            selectQuery += " AND (P.title LIKE '%' + @searchTxt + '%' OR  C.kor_name LIKE '%' + @searchTxt + '%') ";

          if (!string.IsNullOrWhiteSpace(search.startDt) && !string.IsNullOrWhiteSpace(search.endDt))
            selectQuery += " AND P.create_dt BETWEEN CONVERT(DATETIME, @startDt + ' 00:00:00') AND CONVERT(DATETIME, @endDt + ' 23:59:59') ";

          selectQuery += " GROUP BY P.pjt_status) AS A ";

          DynamicParameters param = new DynamicParameters();
          param.Add("@uv_seq", uv_seq, DbType.String);
          param.Add("@searchTxt", search.searchTxt, DbType.String);
          param.Add("@startDt", search.startDt, DbType.String);
          param.Add("@endDt", search.endDt, DbType.String);

          var ret = await con.QueryAsync<ProjectStatusCntModel>(selectQuery, param);

          con.Close();

          return ret.FirstOrDefault();
        }
      }
      catch (Exception e)
      {
        throw e;
      }
    }

    public async Task<List<ProjectListExcelModel>> SelectShareProjectListWithoutCountAsync(MyProjectSearchModel search)
    {
      try
      {
        List<ProjectListExcelModel> list = new List<ProjectListExcelModel>();

        using (IDbConnection con = new SqlConnection(base.BaseConnectionString))
        {
          con.Open();

          string selectQuery = @" SELECT CASE WHEN P.pjt_status = 1 THEN '진행'
            WHEN P.pjt_status = 2 THEN '보류'
            WHEN P.pjt_status = 3 THEN '종료'
            WHEN P.pjt_status = 4 THEN '완료'
            WHEN P.pjt_status = 5 THEN '성공'
            ELSE ''
       END AS '상태'
      ,C.kor_name AS '업체명'
      ,P.title AS '제목'
      ,CASE WHEN ISNULL(PR.state, 0) = 10 OR ISNULL(PR.state, 0) = 20 THEN '0%' 
            WHEN ISNULL(PR.state, 0) = 30 OR ISNULL(PR.state, 0) = 40 THEN '25%' 
            WHEN ISNULL(PR.state, 0) = 50 OR ISNULL(PR.state, 0) = 60 THEN '50%' 
            WHEN ISNULL(PR.state, 0) = 70 THEN '75%' 
            WHEN ISNULL(PR.state, 0) = 80 THEN '100%' 
            ELSE '0%'
        END AS '진행상태'
      ,PD.am_names AS AM
      ,PM.searcher_names AS Searcher
      ,PB.code_name2 AS '산업'
      ,P.create_dt AS '생성일'
  FROM project AS P INNER JOIN client AS C
                            ON P.c_seq = C.c_seq
                   LEFT OUTER JOIN (SELECT A.p_seq
                                           ,A.state
                                       FROM (SELECT PR.p_seq
                                                   ,PH.state
                                                   ,DENSE_RANK() OVER(PARTITION BY PR.p_seq ORDER BY PH.state DESC) AS rank
                                               FROM pjt_recandidate AS PR INNER JOIN pjt_recandidate_history AS PH
                                                                                  ON PR.p_seq = PH.p_seq
                                                                                 AND PR.pic_seq = PH.pic_seq
                                              GROUP BY PR.p_seq, PH.state) AS A
                                      WHERE A.rank = 1) AS PR
                                 ON P.p_seq = PR.p_seq
                    LEFT OUTER JOIN (SELECT DISTINCT
                                            PD.p_seq
                                           ,STUFF((SELECT DISTINCT
                                                          ',' + B.name
                                                     FROM pjt_director AS A INNER JOIN uv_user AS B
                                                                                   ON A.uv_seq = B.uv_seq
                                                    WHERE A.p_seq = PD.p_seq
                                                      FOR XML PATH('')), 1, 1, '') AS am_names
                                       FROM pjt_director AS PD) AS PD
                                 ON P.p_seq = PD.p_seq
                    LEFT OUTER JOIN (SELECT DISTINCT
                                            PM.p_seq
                                           ,STUFF((SELECT DISTINCT
                                                          ',' + B.name
                                                     FROM pjt_manager AS A INNER JOIN uv_user AS B
                                                                                   ON A.uv_seq = B.uv_seq
                                                    WHERE A.p_seq = PM.p_seq
                                                      FOR XML PATH('')), 1, 1, '') AS searcher_names
                                       FROM pjt_manager AS PM) AS PM
                                 ON P.p_seq = PM.p_seq
                    LEFT OUTER JOIN code_job_mst AS CJ1
                            ON P.job_code1 = CJ1.code1
               LEFT OUTER JOIN code_job_dtl AS CJ2
                            ON P.job_code1 = CJ2.code1
                           AND P.job_code2 = CJ2.code2
               LEFT OUTER JOIN code_business_mst AS CB1
                            ON P.business_code1 = CB1.code1
               LEFT OUTER JOIN code_business_dtl AS CB2
                            ON P.business_code1 = CB2.code1
                           AND P.business_code2 = CB2.code2
 WHERE P.pjt_type = 1
   AND P.is_share_pjt = 1
   AND P.pjt_status = CASE WHEN @state <> 0 THEN @state ELSE P.pjt_status END ";

          if (!string.IsNullOrWhiteSpace(search.searchTxt))
            selectQuery += " AND (P.title LIKE '%' + @searchTxt + '%' OR  C.kor_name LIKE '%' + @searchTxt + '%') ";

          if (!string.IsNullOrWhiteSpace(search.startDt) && !string.IsNullOrWhiteSpace(search.endDt))
            selectQuery += " AND P.create_dt BETWEEN CONVERT(DATETIME, @startDt + ' 00:00:00') AND CONVERT(DATETIME, @endDt + ' 23:59:59') ";

          selectQuery += string.Format(" ORDER BY {0} {1} ", search.orderTxt, search.orderOption);

          DynamicParameters param = new DynamicParameters();
          param.Add("@state", search.state, DbType.String);
          param.Add("@searchTxt", search.searchTxt, DbType.String);
          param.Add("@startDt", search.startDt, DbType.String);
          param.Add("@endDt", search.endDt, DbType.String);

          var ret = await con.QueryAsync<ProjectListExcelModel>(selectQuery, param);

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
    /// 공유프로젝트 리스트
    /// </summary>
    /// <param name="search"></param>
    /// <param name="uv_seq"></param>
    /// <param name="skip"></param>
    /// <param name="count"></param>
    /// <param name="totalCount"></param>
    /// <returns></returns>
    public List<project> SelectShareProjectList(MyProjectSearchModel search, int skip, int count, out int totalCount)
    {
      try
      {
        List<project> list = new List<project>();

        using (IDbConnection con = new SqlConnection(base.BaseConnectionString))
        {
          con.Open();

          string selectQuery = @" SELECT P.p_seq
      ,C.c_seq
      ,P.pjt_status
      ,C.kor_name AS company_name
      ,P.title
      ,ISNULL(PR.state, 0) AS recState
      ,PD.am_names
      ,PM.searcher_names
      ,CB2.code_name2 as business_names2
      ,P.create_dt
      ,(SELECT COUNT(*) FROM project_read_history WHERE p_seq = P.p_seq) as read_cnt
  FROM project AS P INNER JOIN client AS C
                            ON P.c_seq = C.c_seq
                   LEFT OUTER JOIN (SELECT A.p_seq
                                           ,A.state
                                       FROM (SELECT PR.p_seq
                                                   ,PH.state
                                                   ,DENSE_RANK() OVER(PARTITION BY PR.p_seq ORDER BY PH.state DESC) AS rank
                                               FROM pjt_recandidate AS PR INNER JOIN pjt_recandidate_history AS PH
                                                                                  ON PR.p_seq = PH.p_seq
                                                                                 AND PR.pic_seq = PH.pic_seq
                                              GROUP BY PR.p_seq, PH.state) AS A
                                      WHERE A.rank = 1) AS PR
                                 ON P.p_seq = PR.p_seq
                    LEFT OUTER JOIN (SELECT DISTINCT
                                            PD.p_seq
                                           ,STUFF((SELECT DISTINCT
                                                          ',' + B.name
                                                     FROM pjt_director AS A INNER JOIN uv_user AS B
                                                                                   ON A.uv_seq = B.uv_seq
                                                    WHERE A.p_seq = PD.p_seq
                                                      FOR XML PATH('')), 1, 1, '') AS am_names
                                       FROM pjt_director AS PD) AS PD
                                 ON P.p_seq = PD.p_seq
                    LEFT OUTER JOIN (SELECT DISTINCT
                                            PM.p_seq
                                           ,STUFF((SELECT DISTINCT
                                                          ',' + B.name
                                                     FROM pjt_manager AS A INNER JOIN uv_user AS B
                                                                                   ON A.uv_seq = B.uv_seq
                                                    WHERE A.p_seq = PM.p_seq
                                                      FOR XML PATH('')), 1, 1, '') AS searcher_names
                                       FROM pjt_manager AS PM) AS PM
                                 ON P.p_seq = PM.p_seq
                    LEFT OUTER JOIN code_job_mst AS CJ1
                            ON P.job_code1 = CJ1.code1
               LEFT OUTER JOIN code_job_dtl AS CJ2
                            ON P.job_code1 = CJ2.code1
                           AND P.job_code2 = CJ2.code2
               LEFT OUTER JOIN code_business_mst AS CB1
                            ON P.business_code1 = CB1.code1
               LEFT OUTER JOIN code_business_dtl AS CB2
                            ON P.business_code1 = CB2.code1
                           AND P.business_code2 = CB2.code2
 WHERE P.pjt_type = 1
   AND P.is_share_pjt = 1
   AND P.pjt_status = CASE WHEN @state <> 0 THEN @state ELSE P.pjt_status END ";

          if (!string.IsNullOrWhiteSpace(search.searchTxt))
            selectQuery += " AND (P.title LIKE '%' + @searchTxt + '%' OR  C.kor_name LIKE '%' + @searchTxt + '%') ";

          if (!string.IsNullOrWhiteSpace(search.startDt) && !string.IsNullOrWhiteSpace(search.endDt))
            selectQuery += " AND P.create_dt BETWEEN CONVERT(DATETIME, @startDt + ' 00:00:00') AND CONVERT(DATETIME, @endDt + ' 23:59:59') ";

          selectQuery += string.Format(" ORDER BY {0} {1} ", search.orderTxt, search.orderOption);
          selectQuery += " OFFSET @currentPage ROWS FETCH NEXT @pageSize ROWS ONLY ";

          string countQuery = @" SELECT COUNT(0)
  FROM project AS P INNER JOIN client AS C
                            ON P.c_seq = C.c_seq
 WHERE P.pjt_type = 1
   AND P.pjt_status = CASE WHEN @state <> 0 THEN @state ELSE P.pjt_status END 
   AND P.is_share_pjt = 1 ";

          if (!string.IsNullOrWhiteSpace(search.searchTxt))
            countQuery += " AND (P.title LIKE '%' + @searchTxt + '%' OR  C.kor_name LIKE '%' + @searchTxt + '%') ";

          if (!string.IsNullOrWhiteSpace(search.startDt) && !string.IsNullOrWhiteSpace(search.endDt))
            countQuery += " AND P.create_dt BETWEEN CONVERT(DATETIME, @startDt + ' 00:00:00') AND CONVERT(DATETIME, @endDt + ' 23:59:59') ";

          DynamicParameters param = new DynamicParameters();
          param.Add("@state", search.state, DbType.String);
          param.Add("@searchTxt", search.searchTxt, DbType.String);
          param.Add("@startDt", search.startDt, DbType.String);
          param.Add("@endDt", search.endDt, DbType.String);
          param.Add("@currentPage", skip, DbType.Int32);
          param.Add("@pageSize", count, DbType.Int32);

          DynamicParameters param2 = new DynamicParameters();
          param2.Add("@state", search.state, DbType.String);
          param2.Add("@searchTxt", search.searchTxt, DbType.String);
          param2.Add("@startDt", search.startDt, DbType.String);
          param2.Add("@endDt", search.endDt, DbType.String);

          var ret = con.Query<project>(selectQuery, param);
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

    public async Task<ProjectStatusCntModel> SelectShareProjectStatusCountAsync(MyProjectSearchModel search)
    {
      try
      {
        List<uv_user> list = new List<uv_user>();

        using (IDbConnection con = new SqlConnection(base.BaseConnectionString))
        {
          con.Open();

          string selectQuery = @" SELECT SUM(allCnt) AS allCnt
      ,SUM(progressCnt) AS progressCnt
      ,SUM(waitCnt) AS waitCnt
      ,SUM(failCnt) AS failCnt
      ,SUM(completeCnt) AS completeCnt
      ,SUM(successCnt) AS successCnt
  FROM (SELECT COUNT(0) AS allCnt
              ,CASE WHEN P.pjt_status = 1 THEN COUNT(P.pjt_status) ELSE 0 END AS progressCnt
              ,CASE WHEN P.pjt_status = 2 THEN COUNT(P.pjt_status) ELSE 0 END AS waitCnt
              ,CASE WHEN P.pjt_status = 3 THEN COUNT(P.pjt_status) ELSE 0 END AS failCnt
              ,CASE WHEN P.pjt_status = 4 THEN COUNT(P.pjt_status) ELSE 0 END AS completeCnt
              ,CASE WHEN P.pjt_status = 5 THEN COUNT(P.pjt_status) ELSE 0 END AS successCnt
         FROM project AS P INNER JOIN Client AS C
                                   ON P.c_seq = C.c_seq
        WHERE P.pjt_type = 1
          AND P.is_share_pjt = 1 ";

          if (!string.IsNullOrWhiteSpace(search.searchTxt))
            selectQuery += " AND (P.title LIKE '%' + @searchTxt + '%' OR  C.kor_name LIKE '%' + @searchTxt + '%') ";

          if (!string.IsNullOrWhiteSpace(search.startDt) && !string.IsNullOrWhiteSpace(search.endDt))
            selectQuery += " AND P.create_dt BETWEEN CONVERT(DATETIME, @startDt + ' 00:00:00') AND CONVERT(DATETIME, @endDt + ' 23:59:59') ";

          selectQuery += " GROUP BY P.pjt_status) AS A ";

          DynamicParameters param = new DynamicParameters();
          param.Add("@searchTxt", search.searchTxt, DbType.String);
          param.Add("@startDt", search.startDt, DbType.String);
          param.Add("@endDt", search.endDt, DbType.String);

          var ret = await con.QueryAsync<ProjectStatusCntModel>(selectQuery, param);

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
    /// 전담프로젝트 리스트 조회 (카운트, 페이징 없음)
    /// </summary>
    /// <param name="search"></param>
    /// <param name="skip"></param>
    /// <param name="count"></param>
    /// <param name="totalCount"></param>
    /// <returns></returns>
    public async Task<List<ProjectListExcelModel>> SelectCoWorkProjectListWithoutCountAsnyc(MyProjectSearchModel search)
    {
      try
      {
        List<ProjectListExcelModel> list = new List<ProjectListExcelModel>();

        using (IDbConnection con = new SqlConnection(base.BaseConnectionString))
        {
          con.Open();

          string selectQuery = @" SELECT CASE WHEN P.pjt_status = 1 THEN '진행'
            WHEN P.pjt_status = 2 THEN '보류'
            WHEN P.pjt_status = 3 THEN '종료'
            WHEN P.pjt_status = 4 THEN '완료'
            WHEN P.pjt_status = 5 THEN '성공'
            ELSE ''
       END AS '상태'
      ,C.kor_name AS '업체명'
      ,P.title AS '제목'
      ,CASE WHEN ISNULL(PR.state, 0) = 10 OR ISNULL(PR.state, 0) = 20 THEN '0%' 
            WHEN ISNULL(PR.state, 0) = 30 OR ISNULL(PR.state, 0) = 40 THEN '25%' 
            WHEN ISNULL(PR.state, 0) = 50 OR ISNULL(PR.state, 0) = 60 THEN '50%' 
            WHEN ISNULL(PR.state, 0) = 70 THEN '75%' 
            WHEN ISNULL(PR.state, 0) = 80 THEN '100%' 
            ELSE '0%'
        END AS '진행상태'
      ,PD.am_names AS AM
      ,PM.searcher_names AS Searcher
      ,CB2.code_name2 AS '산업'
      ,P.create_dt AS '생성일'
  FROM project AS P INNER JOIN client AS C
                            ON P.c_seq = C.c_seq
                   LEFT OUTER JOIN (SELECT A.p_seq
                                           ,A.state
                                       FROM (SELECT PR.p_seq
                                                   ,PH.state
                                                   ,DENSE_RANK() OVER(PARTITION BY PR.p_seq ORDER BY PH.state DESC) AS rank
                                               FROM pjt_recandidate AS PR INNER JOIN pjt_recandidate_history AS PH
                                                                                  ON PR.p_seq = PH.p_seq
                                                                                 AND PR.pic_seq = PH.pic_seq
                                              GROUP BY PR.p_seq, PH.state) AS A
                                      WHERE A.rank = 1) AS PR
                                 ON P.p_seq = PR.p_seq
                    LEFT OUTER JOIN (SELECT DISTINCT
                                            PD.p_seq
                                           ,STUFF((SELECT DISTINCT
                                                          ',' + B.name
                                                     FROM pjt_director AS A INNER JOIN uv_user AS B
                                                                                   ON A.uv_seq = B.uv_seq
                                                    WHERE A.p_seq = PD.p_seq
                                                      FOR XML PATH('')), 1, 1, '') AS am_names
                                       FROM pjt_director AS PD) AS PD
                                 ON P.p_seq = PD.p_seq
                    LEFT OUTER JOIN (SELECT DISTINCT
                                            PM.p_seq
                                           ,STUFF((SELECT DISTINCT
                                                          ',' + B.name
                                                     FROM pjt_manager AS A INNER JOIN uv_user AS B
                                                                                   ON A.uv_seq = B.uv_seq
                                                    WHERE A.p_seq = PM.p_seq
                                                      FOR XML PATH('')), 1, 1, '') AS searcher_names
                                       FROM pjt_manager AS PM) AS PM
                                 ON P.p_seq = PM.p_seq
                    LEFT OUTER JOIN code_job_mst AS CJ1
                            ON P.job_code1 = CJ1.code1
               LEFT OUTER JOIN code_job_dtl AS CJ2
                            ON P.job_code1 = CJ2.code1
                           AND P.job_code2 = CJ2.code2
               LEFT OUTER JOIN code_business_mst AS CB1
                            ON P.business_code1 = CB1.code1
               LEFT OUTER JOIN code_business_dtl AS CB2
                            ON P.business_code1 = CB2.code1
                           AND P.business_code2 = CB2.code2
 WHERE P.pjt_type = 1
   AND P.is_cowork = 1
   AND P.pjt_status = CASE WHEN @state <> 0 THEN @state ELSE P.pjt_status END ";

          if (!string.IsNullOrWhiteSpace(search.searchTxt))
            selectQuery += " AND (P.title LIKE '%' + @searchTxt + '%' OR  C.kor_name LIKE '%' + @searchTxt + '%') ";

          if (!string.IsNullOrWhiteSpace(search.startDt) && !string.IsNullOrWhiteSpace(search.endDt))
            selectQuery += " AND P.create_dt BETWEEN CONVERT(DATETIME, @startDt + ' 00:00:00') AND CONVERT(DATETIME, @endDt + ' 23:59:59') ";

          selectQuery += string.Format(" ORDER BY {0} {1} ", search.orderTxt, search.orderOption);

          DynamicParameters param = new DynamicParameters();
          param.Add("@state", search.state, DbType.String);
          param.Add("@searchTxt", search.searchTxt, DbType.String);
          param.Add("@startDt", search.startDt, DbType.String);
          param.Add("@endDt", search.endDt, DbType.String);

          var ret = await con.QueryAsync<ProjectListExcelModel>(selectQuery, param);

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
    /// 전담코웤 프로젝트 리스트 조회.
    /// </summary>
    /// <param name="search"></param>
    /// <param name="skip"></param>
    /// <param name="count"></param>
    /// <param name="totalCount"></param>
    /// <returns></returns>
    public List<project> SelectCoWorkProjectList(MyProjectSearchModel search, int skip, int count, out int totalCount)
    {
      try
      {
        List<project> list = new List<project>();

        using (IDbConnection con = new SqlConnection(base.BaseConnectionString))
        {
          con.Open();

          string selectQuery = @" SELECT P.p_seq
      ,C.c_seq
      ,P.pjt_status
      ,C.kor_name AS company_name
      ,P.title
      ,ISNULL(PR.state, 0) AS recState
      ,PD.am_names
      ,PM.searcher_names
      ,CB2.code_name2 as business_names2
      ,P.create_dt
  FROM project AS P INNER JOIN client AS C
                            ON P.c_seq = C.c_seq
                   LEFT OUTER JOIN (SELECT A.p_seq
                                           ,A.state
                                       FROM (SELECT PR.p_seq
                                                   ,PH.state
                                                   ,DENSE_RANK() OVER(PARTITION BY PR.p_seq ORDER BY PH.state DESC) AS rank
                                               FROM pjt_recandidate AS PR INNER JOIN pjt_recandidate_history AS PH
                                                                                  ON PR.p_seq = PH.p_seq
                                                                                 AND PR.pic_seq = PH.pic_seq
                                              GROUP BY PR.p_seq, PH.state) AS A
                                      WHERE A.rank = 1) AS PR
                                 ON P.p_seq = PR.p_seq
                    LEFT OUTER JOIN (SELECT DISTINCT
                                            PD.p_seq
                                           ,STUFF((SELECT DISTINCT
                                                          ',' + B.name
                                                     FROM pjt_director AS A INNER JOIN uv_user AS B
                                                                                   ON A.uv_seq = B.uv_seq
                                                    WHERE A.p_seq = PD.p_seq
                                                      FOR XML PATH('')), 1, 1, '') AS am_names
                                       FROM pjt_director AS PD) AS PD
                                 ON P.p_seq = PD.p_seq
                    LEFT OUTER JOIN (SELECT DISTINCT
                                            PM.p_seq
                                           ,STUFF((SELECT DISTINCT
                                                          ',' + B.name
                                                     FROM pjt_manager AS A INNER JOIN uv_user AS B
                                                                                   ON A.uv_seq = B.uv_seq
                                                    WHERE A.p_seq = PM.p_seq
                                                      FOR XML PATH('')), 1, 1, '') AS searcher_names
                                       FROM pjt_manager AS PM) AS PM
                                 ON P.p_seq = PM.p_seq
                    LEFT OUTER JOIN code_job_mst AS CJ1
                            ON P.job_code1 = CJ1.code1
               LEFT OUTER JOIN code_job_dtl AS CJ2
                            ON P.job_code1 = CJ2.code1
                           AND P.job_code2 = CJ2.code2
               LEFT OUTER JOIN code_business_mst AS CB1
                            ON P.business_code1 = CB1.code1
               LEFT OUTER JOIN code_business_dtl AS CB2
                            ON P.business_code1 = CB2.code1
                           AND P.business_code2 = CB2.code2
 WHERE P.pjt_type = 1
   AND P.is_cowork = 1
   AND P.pjt_status = CASE WHEN @state <> 0 THEN @state ELSE P.pjt_status END ";

          if (!string.IsNullOrWhiteSpace(search.searchTxt))
            selectQuery += " AND (P.title LIKE '%' + @searchTxt + '%' OR  C.kor_name LIKE '%' + @searchTxt + '%') ";

          if (!string.IsNullOrWhiteSpace(search.startDt) && !string.IsNullOrWhiteSpace(search.endDt))
            selectQuery += " AND P.create_dt BETWEEN CONVERT(DATETIME, @startDt + ' 00:00:00') AND CONVERT(DATETIME, @endDt + ' 23:59:59') ";

          selectQuery += string.Format(" ORDER BY {0} {1} ", search.orderTxt, search.orderOption);
          selectQuery += " OFFSET @currentPage ROWS FETCH NEXT @pageSize ROWS ONLY ";

          string countQuery = @" SELECT COUNT(0)
  FROM project AS P INNER JOIN client AS C
                            ON P.c_seq = C.c_seq
 WHERE P.pjt_type = 1
   AND P.pjt_status = CASE WHEN @state <> 0 THEN @state ELSE P.pjt_status END 
   AND P.is_cowork = 1 ";

          if (!string.IsNullOrWhiteSpace(search.searchTxt))
            countQuery += " AND (P.title LIKE '%' + @searchTxt + '%' OR  C.kor_name LIKE '%' + @searchTxt + '%') ";

          if (!string.IsNullOrWhiteSpace(search.startDt) && !string.IsNullOrWhiteSpace(search.endDt))
            countQuery += " AND P.create_dt BETWEEN CONVERT(DATETIME, @startDt + ' 00:00:00') AND CONVERT(DATETIME, @endDt + ' 23:59:59') ";

          DynamicParameters param = new DynamicParameters();
          param.Add("@state", search.state, DbType.String);
          param.Add("@searchTxt", search.searchTxt, DbType.String);
          param.Add("@startDt", search.startDt, DbType.String);
          param.Add("@endDt", search.endDt, DbType.String);
          param.Add("@currentPage", skip, DbType.Int32);
          param.Add("@pageSize", count, DbType.Int32);

          DynamicParameters param2 = new DynamicParameters();
          param2.Add("@state", search.state, DbType.String);
          param2.Add("@searchTxt", search.searchTxt, DbType.String);
          param2.Add("@startDt", search.startDt, DbType.String);
          param2.Add("@endDt", search.endDt, DbType.String);

          var ret = con.Query<project>(selectQuery, param);
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

    public async Task<ProjectStatusCntModel> SelectCoWorkProjectStatusCountAsync(MyProjectSearchModel search)
    {
      try
      {
        List<uv_user> list = new List<uv_user>();

        using (IDbConnection con = new SqlConnection(base.BaseConnectionString))
        {
          con.Open();

          string selectQuery = @" SELECT SUM(allCnt) AS allCnt
      ,SUM(progressCnt) AS progressCnt
      ,SUM(waitCnt) AS waitCnt
      ,SUM(failCnt) AS failCnt
      ,SUM(completeCnt) AS completeCnt
      ,SUM(successCnt) AS successCnt
  FROM (SELECT COUNT(0) AS allCnt
              ,CASE WHEN P.pjt_status = 1 THEN COUNT(P.pjt_status) ELSE 0 END AS progressCnt
              ,CASE WHEN P.pjt_status = 2 THEN COUNT(P.pjt_status) ELSE 0 END AS waitCnt
              ,CASE WHEN P.pjt_status = 3 THEN COUNT(P.pjt_status) ELSE 0 END AS failCnt
              ,CASE WHEN P.pjt_status = 4 THEN COUNT(P.pjt_status) ELSE 0 END AS completeCnt
              ,CASE WHEN P.pjt_status = 5 THEN COUNT(P.pjt_status) ELSE 0 END AS successCnt
         FROM project AS P INNER JOIN Client AS C
                                   ON P.c_seq = C.c_seq
        WHERE P.pjt_type = 1
          AND P.is_cowork = 1 ";

          if (!string.IsNullOrWhiteSpace(search.searchTxt))
            selectQuery += " AND (P.title LIKE '%' + @searchTxt + '%' OR  C.kor_name LIKE '%' + @searchTxt + '%') ";

          if (!string.IsNullOrWhiteSpace(search.startDt) && !string.IsNullOrWhiteSpace(search.endDt))
            selectQuery += " AND P.create_dt BETWEEN CONVERT(DATETIME, @startDt + ' 00:00:00') AND CONVERT(DATETIME, @endDt + ' 23:59:59') ";

          selectQuery += " GROUP BY P.pjt_status) AS A ";

          DynamicParameters param = new DynamicParameters();
          param.Add("@searchTxt", search.searchTxt, DbType.String);
          param.Add("@startDt", search.startDt, DbType.String);
          param.Add("@endDt", search.endDt, DbType.String);

          var ret = await con.QueryAsync<ProjectStatusCntModel>(selectQuery, param);

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
    /// 프로젝트 단건 조회 - 채용
    /// </summary>
    /// <returns></returns>
    public async Task<project> SelectProjectOneAsync(int p_seq)
    {
      try
      {
        using (IDbConnection con = new SqlConnection(base.BaseConnectionString))
        {
          con.Open();

          string selectQuery = @" SELECT P.*
      ,C.kor_name AS company_name
      ,CS.campus_name AS target_school_campus
      ,CS.school_name AS target_school_name
      ,CM.major_name AS target_major_name
      ,GC.WKPL_NM AS target_company_name
      ,CT.division AS tax_division
      ,CT.name AS  tax_name
      ,CT.email AS tax_email
      ,CT.phone AS tax_phone
      ,CT.cell_phone AS tax_cell_phone
      ,CT.deposit_manager AS tax_deposit_manager
      ,CT.deposit_email AS tax_deposit_email
      ,CC.name AS contact_name
      ,CC.gender AS contact_gender
      ,CC.email AS contact_email
      ,CC.phone AS contact_phone
      ,CC.cell_phone AS contact_cell_phone
      ,CC.division AS contact_division
      ,CC.position AS contact_position
      ,ISNULL(P.business_code1, 0) AS business_code1
      ,ISNULL(P.business_code2, 0) AS business_code2
      ,ISNULL(P.job_code1, 0) AS job_code1
      ,ISNULL(P.job_code2, 0) AS job_code2
      ,CB1.code_name1 AS business_names1
      ,CB2.code_name2 AS business_names2
      ,CJ1.code_name1 AS job_names1
      ,CJ2.code_name2 AS job_names2
      ,SCB1.code_name1 AS sub_business_names1
      ,SCB2.code_name2 AS sub_business_names2
      ,SCJ1.code_name1 AS sub_job_names1
      ,SCJ2.code_name2 AS sub_job_names2
      , (select count(0) from project_log where p_seq = p.p_seq) log_cnt
      , CASE WHEN P.is_share_pjt = 1 AND P.share_dt > DATEADD(m, -3, getdate()) THEN 1 ELSE 0 END as is_share_3m
  FROM project AS P INNER JOIN client AS C
                            ON P.c_seq = C.c_seq
               LEFT OUTER JOIN code_school AS CS
                            ON P.target_school = CS.sc_seq
               LEFT OUTER JOIN code_major_2018 AS CM
                            ON P.target_major = CM.major_code
               LEFT OUTER JOIN gov_api_company AS GC
                            ON P.target_company = GC.G_SEQ
               LEFT OUTER JOIN client_contact AS CC
                            ON P.c_seq = CC.c_seq
                           AND P.cc_seq = CC.cc_seq
               LEFT OUTER JOIN client_tax_contact AS CT
                            ON P.c_seq = CT.c_seq
                           AND P.ctc_seq = CT.ctc_seq
               LEFT OUTER JOIN code_job_mst AS CJ1
                            ON P.job_code1 = CJ1.code1
               LEFT OUTER JOIN code_job_dtl AS CJ2
                            ON P.job_code1 = CJ2.code1
                           AND P.job_code2 = CJ2.code2
               LEFT OUTER JOIN code_business_mst AS CB1
                            ON P.business_code1 = CB1.code1
               LEFT OUTER JOIN code_business_dtl AS CB2
                            ON P.business_code1 = CB2.code1
                           AND P.business_code2 = CB2.code2
               LEFT OUTER JOIN code_job_mst AS SCJ1
                            ON P.sub_job_code1 = SCJ1.code1
               LEFT OUTER JOIN code_job_dtl AS SCJ2
                            ON P.sub_job_code1 = SCJ2.code1
                           AND P.sub_job_code2 = SCJ2.code2
               LEFT OUTER JOIN code_business_mst AS SCB1
                            ON P.sub_business_code1 = SCB1.code1
               LEFT OUTER JOIN code_business_dtl AS SCB2
                            ON P.sub_business_code1 = SCB2.code1
                           AND P.sub_business_code2 = SCB2.code2
 WHERE P.p_seq = @p_seq ";

          DynamicParameters param = new DynamicParameters();
          param.Add("@p_seq", p_seq, DbType.Int32);

          var ret = await con.QueryAsync<project>(selectQuery, param);

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
    /// 프로젝트 조회 - 평판조회
    /// </summary>
    /// <param name="p_seq"></param>
    /// <returns></returns>
    public async Task<project> SelectProjectRepOneAsync(int p_seq)
    {
      try
      {
        using (IDbConnection con = new SqlConnection(base.BaseConnectionString))
        {
          con.Open();

          string selectQuery = @" SELECT P.*
      ,C.kor_name AS company_name
      ,CT.division AS tax_division
      ,CT.name AS  tax_name
      ,CT.email AS tax_email
      ,CT.phone AS tax_phone
      ,CT.cell_phone AS tax_cell_phone
      ,CT.deposit_manager AS tax_deposit_manager
      ,CT.deposit_email AS tax_deposit_email
      ,CC.name AS contact_name
      ,CC.gender AS contact_gender
      ,CC.email AS contact_email
      ,CC.phone AS contact_phone
      ,CC.cell_phone AS contact_cell_phone
      ,CC.division AS contact_division
      ,CC.position AS contact_position
      ,CB1.code_name1 AS business_names1
      ,CB2.code_name2 AS business_names2
      ,CJ1.code_name1 AS job_names1
      ,CJ2.code_name2 AS job_names2
  FROM project AS P INNER JOIN client AS C
                            ON P.c_seq = C.c_seq
               LEFT OUTER JOIN client_contact AS CC
                            ON P.c_seq = CC.c_seq
                           AND P.cc_seq = CC.cc_seq
               LEFT OUTER JOIN client_tax_contact AS CT
                            ON P.c_seq = CT.c_seq
                           AND P.ctc_seq = CT.ctc_seq
               LEFT OUTER JOIN code_job_mst AS CJ1
                            ON P.job_code1 = CJ1.code1
               LEFT OUTER JOIN code_job_dtl AS CJ2
                            ON P.job_code1 = CJ2.code1
                           AND P.job_code2 = CJ2.code2
               LEFT OUTER JOIN code_business_mst AS CB1
                            ON P.business_code1 = CB1.code1
               LEFT OUTER JOIN code_business_dtl AS CB2
                            ON P.business_code1 = CB2.code1
                           AND P.business_code2 = CB2.code2
  WHERE P.p_seq = @p_seq 
    AND P.pjt_type <> 1 ";

          DynamicParameters param = new DynamicParameters();
          param.Add("@p_seq", p_seq, DbType.Int32);

          var ret = await con.QueryAsync<project>(selectQuery, param);

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
    /// 프로젝트 책임자(AM) 리스트 조회
    /// </summary>
    /// <param name="p_seq"></param>
    /// <returns></returns>
    public async Task<List<pjt_director>> SelectProjectAmListAsync(int p_seq)
    {
      try
      {
        List<pjt_director> list = new List<pjt_director>();

        using (IDbConnection con = new SqlConnection(base.BaseConnectionString))
        {
          con.Open();

          string selectQuery = @" SELECT PD.*
      ,U.name
      ,UD.ud_name
  FROM pjt_director AS PD INNER JOIN uv_user AS U
                                  ON PD.uv_seq = U.uv_seq
                          INNER JOIN uv_division AS UD
                                  ON U.ud_seq = UD.ud_seq
 WHERE PD.p_seq = @p_seq ";

          DynamicParameters param = new DynamicParameters();
          param.Add("@p_seq", p_seq, DbType.Int32);

          var ret = await con.QueryAsync<pjt_director>(selectQuery, param);

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
    /// 프로젝트 담당자(Searcher) 조회
    /// </summary>
    /// <param name="p_seq"></param>
    /// <returns></returns>
    public async Task<List<pjt_manager>> SelectProjectSearcherListAsync(int p_seq)
    {
      try
      {
        List<pjt_manager> list = new List<pjt_manager>();

        using (IDbConnection con = new SqlConnection(base.BaseConnectionString))
        {
          con.Open();

          string selectQuery = @" SELECT PM.*
      ,U.name
      ,UD.ud_name
  FROM pjt_manager AS PM INNER JOIN uv_user AS U
                                 ON PM.uv_seq = U.uv_seq
                         INNER JOIN uv_division AS UD
                                 ON U.ud_seq = UD.ud_seq
 WHERE PM.p_seq = @p_seq ";

          DynamicParameters param = new DynamicParameters();
          param.Add("@p_seq", p_seq, DbType.Int32);

          var ret = await con.QueryAsync<pjt_manager>(selectQuery, param);

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
    /// 프로젝트 산업 리스트 조회
    /// </summary>
    /// <param name="p_seq"></param>
    /// <returns></returns>
    public async Task<List<pjt_business_code>> SelectProjectBusiListAsync(int p_seq)
    {
      try
      {
        List<pjt_business_code> list = new List<pjt_business_code>();

        using (IDbConnection con = new SqlConnection(base.BaseConnectionString))
        {
          con.Open();

          string selectQuery = @" SELECT PB.*
      ,CB.code_name3
  FROM pjt_business_code AS PB INNER JOIN code_business3 AS CB
                                       ON PB.code1 = CB.code1
                                      AND PB.code2 = CB.code2
                                      AND PB.code3 = CB.code3
 WHERE PB.p_seq = @p_seq ";

          DynamicParameters param = new DynamicParameters();
          param.Add("@p_seq", p_seq, DbType.Int32);

          var ret = await con.QueryAsync<pjt_business_code>(selectQuery, param);

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
    /// 프로젝트 직무 리스트 조회.
    /// </summary>
    /// <param name="p_seq"></param>
    /// <returns></returns>
    public async Task<List<pjt_job_code>> SelectProjectJobListAsync(int p_seq)
    {
      try
      {
        List<pjt_job_code> list = new List<pjt_job_code>();

        using (IDbConnection con = new SqlConnection(base.BaseConnectionString))
        {
          con.Open();

          string selectQuery = @" SELECT PJ.*
      ,CJ.code_name3
  FROM pjt_job_code AS PJ INNER JOIN code_job3 AS CJ
                                  ON PJ.code1 = CJ.code1
                                 AND PJ.code2 = CJ.code2
                                 AND PJ.code3 = CJ.code3
 WHERE PJ.p_seq = @p_seq ";

          DynamicParameters param = new DynamicParameters();
          param.Add("@p_seq", p_seq, DbType.Int32);

          var ret = await con.QueryAsync<pjt_job_code>(selectQuery, param);

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
    /// 프로젝트 근무지 리스트 조회.
    /// </summary>
    /// <param name="p_seq"></param>
    /// <returns></returns>
    public async Task<List<pjt_place>> SelectProjectPlaceListAsync(int p_seq)
    {
      try
      {
        List<pjt_place> list = new List<pjt_place>();

        using (IDbConnection con = new SqlConnection(base.BaseConnectionString))
        {
          con.Open();

          string selectQuery = @" SELECT * FROM pjt_place
 WHERE p_seq = @p_seq ";

          DynamicParameters param = new DynamicParameters();
          param.Add("@p_seq", p_seq, DbType.Int32);

          var ret = await con.QueryAsync<pjt_place>(selectQuery, param);

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
    /// 프로젝트 키워드 리스트 조회.
    /// </summary>
    /// <param name="p_seq"></param>
    /// <returns></returns>
    public async Task<List<pjt_keyword>> SelectProjectKeywordListAsync(int p_seq)
    {
      try
      {
        List<pjt_keyword> list = new List<pjt_keyword>();

        using (IDbConnection con = new SqlConnection(base.BaseConnectionString))
        {
          con.Open();

          string selectQuery = @" SELECT * FROM pjt_keyword
 WHERE p_seq = @p_seq ";

          DynamicParameters param = new DynamicParameters();
          param.Add("@p_seq", p_seq, DbType.Int32);

          var ret = await con.QueryAsync<pjt_keyword>(selectQuery, param);

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
    /// 업체 목록 조회
    /// </summary>
    /// <returns></returns>
    public List<client> SelectClientList(string searchTxt, int skip, int count, out int totalCount)
    {
      try
      {
        List<client> list = new List<client>();

        using (IDbConnection con = new SqlConnection(base.BaseConnectionString))
        {
          con.Open();

          string selectQuery = @" SELECT C.c_seq
      ,C.kor_name
      ,C.eng_name
      ,C.biz_code
      ,C.biz_type
      ,C.ceo
      ,C.biz_category
      ,C.biz_industry
      ,C.addr1
      ,CM.uv_seq as am_seq
      ,U.name AS am_name
      ,C.fix_title
      ,C.biz_industry_code1
      ,C.biz_industry_code2
      ,CB2.code_name2 AS business_name2
      ,ISNULL(CC.cc_seq, 0) AS cc_seq
      ,ISNULL(CC.fee_type, '') AS fee_type
      ,ISNULL(CC.fix_fee_rate, 0) AS fix_fee_rate
  FROM client AS C LEFT OUTER JOIN code_business_dtl AS CB2
                                ON C.biz_industry_code1 = CB2.code1
                               AND C.biz_industry_code2 = CB2.code2
                   LEFT OUTER JOIN client_manager AS CM
                           ON C.c_seq = CM.c_seq
                   LEFT OUTER JOIN uv_user AS U
                       	ON CM.uv_seq = U.uv_seq
              LEFT OUTER JOIN (select row_number() over(partition by c_seq order by cc_seq desc) as l, * from client_contract) AS CC
                           ON C.c_seq = CC.c_seq
                           AND CC.l = 1
 WHERE (C.kor_name LIKE '%' + @searchTxt + '%') OR (C.eng_name LIKE '%' + @searchTxt + '%') OR (C.offlimit_keyword LIKE '%' + @searchTxt + '%') 
ORDER BY C.kor_name ASC, C.eng_name ASC 
OFFSET @currentPage ROWS FETCH NEXT @pageSize ROWS ONLY ";

          string countQuery = @" SELECT COUNT(0)
  FROM client AS C LEFT OUTER JOIN code_business_dtl AS CB2
                                ON C.biz_industry_code1 = CB2.code1
                               AND C.biz_industry_code2 = CB2.code2
                   LEFT OUTER JOIN client_manager AS CM
                                ON C.c_seq = CM.c_seq
                   LEFT OUTER JOIN uv_user AS U
                            	ON CM.uv_seq = U.uv_seq
 WHERE (C.kor_name LIKE '%' + @searchTxt + '%') OR (C.eng_name LIKE '%' + @searchTxt + '%') OR (C.offlimit_keyword LIKE '%' + @searchTxt + '%') ";

          DynamicParameters param = new DynamicParameters();
          param.Add("@searchTxt", searchTxt, DbType.String);
          param.Add("@currentPage", skip, DbType.Int32);
          param.Add("@pageSize", count, DbType.Int32);

          DynamicParameters param2 = new DynamicParameters();
          param2.Add("@searchTxt", searchTxt, DbType.String);

          var ret = con.Query<client>(selectQuery, param);
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
    /*
            /// <summary>
            /// 프로젝트 후보자 제안 리스트
            /// </summary>
            /// <param name="p_seq"></param>
            /// <returns></returns>
            public async Task<List<candidate>> SelectPjtCandiSuggetionListAsync(int p_seq)
            {
                try
                {
                    List<candidate> list = new List<candidate>();

                    using (IDbConnection con = new SqlConnection(base.BaseConnectionString))
                    {
                        con.Open();

                        string selectQuery = @" SELECT TOP 4 
           p_seq
          ,c_seq
          ,kor_name
          ,hope_salary
          ,ISNULL(school_name2, school_name) school_name
          ,ISNULL(graduate_status2, graduate_status) graduate_status
          ,ISNULL(company_name2, company_name) company_name
          ,r_name
          ,ISNULL(is_work2, is_work) is_work
      FROM (SELECT P.p_seq
                  ,C1.c_seq
                  ,C1.kor_name
                  ,C1.hope_salary
                  ,CASE WHEN ISNULL(SC.campus_name, '') <> '' THEN SC.school_name + '(' + SC.campus_name + ')' ELSE SC.school_name END AS school_name
                  ,CASE WHEN ISNULL(SC2.campus_name, '') <> '' THEN SC2.school_name + '(' + SC2.campus_name + ')' ELSE SC2.school_name END AS school_name2
                  ,RANK() OVER(PARTITION BY CS.c_seq ORDER BY CS.c_seq, CS.order_no ASC) AS school_rank
                  ,RANK() OVER(PARTITION BY CS2.c_seq ORDER BY CS2.c_seq, CS2.order_no ASC) AS school_rank2
                  ,RANK() OVER(PARTITION BY CC.c_seq ORDER BY CC.c_seq, CC.order_no ASC) AS career_rank
                  ,RANK() OVER(PARTITION BY CC2.c_seq ORDER BY CC2.c_seq, CC2.order_no ASC) AS career_rank2
                  ,CS.graduate_status
                  ,CS2.graduate_status graduate_status2
                  ,CC.company_name
                  ,CC2.company_name company_name2
                  ,CR.r_name
                  ,CC.is_work
                  ,CC2.is_work is_work2
              FROM project AS P INNER JOIN can_job AS CJ 
                                        ON P.job_code1 = CJ.code1
                                       AND P.job_code2 = CJ.code2
                                INNER JOIN can_business AS CB
                                        ON CB.code1 = P.business_code1
                                       AND CB.code2 = P.business_code2
                                INNER JOIN candidate AS C1
                                        ON CJ.c_seq = C1.c_seq
                                       AND CB.c_seq = C1.c_seq
                                INNER JOIN can_school AS CS 
                                        ON c1.c_seq = cs.c_seq
                                INNER JOIN code_school AS SC 
                                        ON CS.sc_seq = SC.sc_seq
                                INNER JOIN can_career AS CC
                                        ON C1.c_seq = CC.c_seq
                           LEFT OUTER JOIN can_career AS CC2
                                        ON P.target_company = CC2.g_seq
                                       AND C1.c_seq = CC2.c_seq
                           LEFT OUTER JOIN can_school AS CS2
                                        ON P.target_school = CS2.sc_seq
                                       AND C1.c_seq = CS2.c_seq
                           LEFT OUTER JOIN code_school AS SC2
                                        ON CS2.sc_seq = SC2.sc_seq
                           LEFT OUTER JOIN code_rank AS CR
                                        ON CC.r_code = CR.r_code
                           LEFT OUTER JOIN code_position AS CP
                                        ON CC.p_code = CP.p_code
             WHERE P.p_seq = @p_seq
               AND C1.c_seq NOT IN (SELECT c_seq
                                      FROM pjt_recandidate
                                     WHERE p_seq = @p_seq)
               AND C1.c_seq NOT IN (SELECT ISNULL(c_seq, 0) AS c_seq
                                      FROM pjt_invoice_info 
                                     WHERE confirm_dt BETWEEN DATEADD(YEAR, -1, GETDATE()) AND GETDATE()
                                       AND is_deleted <> 1
                                     GROUP BY c_seq)
               AND C1.is_confidential = 0
               AND C1.is_inactive = 0
               AND (C1.caution_type IS NULL OR C1.caution_type = 0)
               AND C1.gender = CASE WHEN ISNULL(P.gender_type, 3) = 3 THEN C1.gender ELSE P.gender_type END
               AND C1.birth_date BETWEEN CONVERT(DATETIME, CONVERT(VARCHAR(4), P.start_age) + '-01-01') AND CONVERT(DATETIME, CONVERT(VARCHAR(4), P.end_age) + '-01-01')
             GROUP BY P.p_seq, C1.c_seq, C1.kor_name, C1.hope_salary, CS.c_seq, CS2.c_seq, CC.c_seq, CC2.c_seq, SC.campus_name, SC2.school_name, SC2.campus_name, SC.school_name
                     ,CS.order_no, CC.order_no, CS2.order_no, CC2.order_no, CS.graduate_status, CS2.graduate_status, CC.company_name, CC2.company_name, CR.r_name, CC.is_work, CC2.is_work) AS A
      WHERE school_rank = 1
        AND school_rank2 = 1
        AND career_rank = 1
        AND career_rank2 = 1
      ORDER BY kor_name ASC, school_name2 DESC, company_name2 DESC, is_work2 DESC ";

                        DynamicParameters param = new DynamicParameters();
                        param.Add("@p_seq", p_seq, DbType.Int32);

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
    */
          /// <summary>
          /// 프로젝트 AM 조회.
          /// </summary>
          /// <param name="p_seq"></param>
          /// <returns></returns>
          public async Task<List<uv_user>> SelectProjectAM(int p_seq)
    {
      try
      {
        List<uv_user> list = new List<uv_user>();

        using (IDbConnection con = new SqlConnection(base.BaseConnectionString))
        {
          con.Open();

          string selectQuery = @" SELECT U.*
  FROM pjt_director AS PD INNER JOIN uv_user AS U
                                  ON PD.uv_seq =  U.uv_seq
 WHERE PD.p_seq = @p_seq
   AND U.retire_date is null ";

          DynamicParameters param = new DynamicParameters();
          param.Add("@p_seq", p_seq, DbType.Int32);

          var ret = await con.QueryAsync<uv_user>(selectQuery, param);

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
    /// 프로젝트 Searcher 조회.
    /// </summary>
    /// <param name="p_seq"></param>
    /// <returns></returns>
    public async Task<List<uv_user>> SelectProjectSearcher(int p_seq)
    {
      try
      {
        List<uv_user> list = new List<uv_user>();

        using (IDbConnection con = new SqlConnection(base.BaseConnectionString))
        {
          con.Open();

          string selectQuery = @" SELECT U.*
  FROM pjt_manager AS PM INNER JOIN uv_user AS U
                                  ON PM.uv_seq =  U.uv_seq
 WHERE PM.p_seq = @p_seq
   AND U.retire_date is null ";

          DynamicParameters param = new DynamicParameters();
          param.Add("@p_seq", p_seq, DbType.Int32);

          var ret = await con.QueryAsync<uv_user>(selectQuery, param);

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
    /// 프로젝트 AM 검색
    /// </summary>
    /// <param name="main_contract"></param>
    /// <param name="ud_seq"></param>
    /// <param name="searchTxt"></param>
    /// <returns></returns>
    public async Task<List<uv_user>> SelectAMList(int ud_seq, string searchTxt)
    {
      try
      {
        List<uv_user> list = new List<uv_user>();

        using (IDbConnection con = new SqlConnection(base.BaseConnectionString))
        {
          con.Open();

          string selectQuery = @" SELECT UD.ud_seq
      ,UD.ud_name
      ,U.uv_seq
      ,U.name
      ,U.rank_name
      ,U.hp
      ,CASE WHEN level = 2 THEN 'AM'
            WHEN level = 3 THEN 'SM'
            ELSE ''
        END AS gubun
  FROM uv_user AS U INNER JOIN uv_division AS UD
                            ON U.ud_seq = UD.ud_seq
                    INNER JOIN uv_auth AS UA
                            ON U.ua_seq = UA.ua_seq
 WHERE U.retire_date is null
   AND U.ud_seq NOT IN (69)
   AND UD.ud_seq = CASE WHEN @ud_seq > 0 THEN @ud_seq ELSE UD.ud_seq END ";

          if (!string.IsNullOrWhiteSpace(searchTxt))
            selectQuery += " AND U.name LIKE '%' + @searchTxt + '%' ";

          selectQuery += " ORDER BY U.name ASC ";

          DynamicParameters param = new DynamicParameters();
          param.Add("@ud_seq", ud_seq, DbType.Int32);
          param.Add("@searchTxt", searchTxt, DbType.String);

          var ret = await con.QueryAsync<uv_user>(selectQuery, param);

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
    /// Searcher 리스트
    /// </summary>
    /// <param name="searchTxt"></param>
    /// <param name="skip"></param>
    /// <param name="count"></param>
    /// <param name="totalCount"></param>
    /// <returns></returns>
    public async Task<List<uv_user>> SelectSearcherList(int ud_seq, string searchTxt)
    {
      try
      {
        List<uv_user> list = new List<uv_user>();

        using (IDbConnection con = new SqlConnection(base.BaseConnectionString))
        {
          con.Open();

          string selectQuery = @" SELECT UD.ud_seq
      ,UD.ud_name
      ,U.uv_seq
      ,U.name
      ,U.rank_name
      ,U.hp
      ,CASE WHEN level = 2 THEN 'AM'
            WHEN level = 3 THEN 'SM'
            ELSE ''
        END AS gubun
  FROM uv_user AS U INNER JOIN uv_division AS UD
                            ON U.ud_seq = UD.ud_seq
                    INNER JOIN uv_auth AS UA
                            ON U.ua_seq = UA.ua_seq
 WHERE U.retire_date is null
   AND U.ud_seq NOT IN (69)
   AND UD.ud_seq = CASE WHEN @ud_seq > 0 THEN @ud_seq ELSE UD.ud_seq END "; //UA.level IN(2, 3)

          if (!string.IsNullOrWhiteSpace(searchTxt))
            selectQuery += " AND U.name LIKE '%' + @searchTxt + '%' ";

          selectQuery += " ORDER BY U.name ASC ";

          DynamicParameters param = new DynamicParameters();
          param.Add("@ud_seq", ud_seq, DbType.Int32);
          param.Add("@searchTxt", searchTxt, DbType.String);

          var ret = await con.QueryAsync<uv_user>(selectQuery, param);

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
    /// 프로젝트 진행사항 상태 카운트
    /// </summary>
    /// <param name="p_seq"></param>
    /// <param name="uv_seq"></param>
    /// <returns></returns>
    public async Task<PjtProgressCntModel> SelectProjectRecommandStateCountAsync(int p_seq, int uv_seq)
    {
      try
      {
        List<uv_user> list = new List<uv_user>();

        using (IDbConnection con = new SqlConnection(base.BaseConnectionString))
        {
          con.Open();

          string selectQuery = @" SELECT SUM(allCnt) AS allCnt
      ,SUM(hireOkCnt) AS hireOkCnt
      ,SUM(hireCnt) AS hireCnt
      ,SUM(interviewOkCnt) AS interviewOkCnt
      ,SUM(interviewCnt) AS interviewCnt
      ,SUM(paperOkCnt) AS paperOkCnt
      ,SUM(recommandCnt) AS recommandCnt
      ,SUM(interestCnt) AS interestCnt
      ,SUM(failCnt) AS failCnt
  FROM (SELECT COUNT(0) AS allCnt
              ,CASE WHEN A.state = 80 THEN COUNT(A.state) ELSE 0 END AS hireOkCnt
              ,CASE WHEN A.state = 70 THEN COUNT(A.state) ELSE 0 END AS hireCnt
              ,CASE WHEN A.state = 60 THEN COUNT(A.state) ELSE 0 END AS interviewOkCnt
              ,CASE WHEN A.state = 50 THEN COUNT(A.state) ELSE 0 END AS interviewCnt
              ,CASE WHEN A.state = 40 THEN COUNT(A.state) ELSE 0 END AS paperOkCnt
              ,CASE WHEN A.state = 30 THEN COUNT(A.state) ELSE 0 END AS recommandCnt
              ,CASE WHEN A.state = 20 THEN COUNT(A.state) ELSE 0 END AS interestCnt
              ,CASE WHEN A.state = 10 THEN COUNT(A.state) ELSE 0 END AS failCnt
              ,CASE WHEN A.state = 13 THEN COUNT(A.state) ELSE 0 END AS selfdropCnt
              ,CASE WHEN A.state = 15 THEN COUNT(A.state) ELSE 0 END AS no_intrestCnt
          FROM (SELECT ROW_NUMBER() OVER(PARTITION BY PR.c_seq ORDER BY MAX(RH.create_dt) DESC, MAX(RH.state) DESC) AS row
                      ,PR.p_seq
                      ,PR.pic_seq
                      ,RH.prc_seq
                      ,PR.c_seq
                      ,C.kor_name
                      ,RH.state
                      ,RH.schedule_date
                      ,RH.contents
                      ,RH.file_origin_path
                  FROM pjt_recandidate AS PR INNER JOIN pjt_recandidate_history AS RH
                                                     ON PR.p_seq = RH.p_seq
                                                    AND PR.pic_seq = RH.pic_seq
                                             INNER JOIN candidate AS C
                                                     ON PR.c_seq = C.c_seq
                 WHERE PR.p_seq = @p_seq
                   --AND PR.create_user = @uv_seq
                 GROUP BY PR.p_seq, PR.pic_seq, RH.prc_seq, PR.c_seq, C.kor_name, RH.schedule_date, RH.state, RH.contents, RH.file_origin_path) AS A
         WHERE A.row = 1
         GROUP BY  A.state) AS A ";

          DynamicParameters param = new DynamicParameters();
          param.Add("@p_seq", p_seq, DbType.Int32);
          param.Add("@uv_seq", uv_seq, DbType.Int32);

          var ret = await con.QueryAsync<PjtProgressCntModel>(selectQuery, param);

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
    /// 프로젝트 진행사항 리스트 조회.
    /// </summary>
    /// <param name="ud_seq"></param>
    /// <param name="searchTxt"></param>
    /// <returns></returns>
    public List<pjt_recandidate_history> SelectProjectReCanMakeupList(int p_seq, List<int> pic_seq)
    {
      try
      {
        List<pjt_recandidate_history> list = new List<pjt_recandidate_history>();

        using (IDbConnection con = new SqlConnection(base.BaseConnectionString))
        {
          con.Open();
          string selectQuery = @" 
  SELECT A.*,
       KR.cr_seq as final_kor_cr_seq,
       KR.file_dir as final_kor_resume_dir,
       KR.create_dt as final_kor_resume_date,
       KR.file_path as final_kor_resume_name,
       MKR.cr_seq as final_mkor_cr_seq,
       MKR.file_dir as final_mkor_resume_dir,
       MKR.create_dt as final_mkor_resume_date,
       MKR.file_path as final_mkor_resume_name,
       EN.cr_seq as final_eng_cr_seq,
       EN.file_dir as final_eng_resume_dir,
       EN.create_dt as final_eng_resume_date,
       EN.file_path as final_eng_resume_name,
       MEN.cr_seq as final_meng_cr_seq,
       MEN.file_dir as final_meng_resume_dir,
       MEN.create_dt as final_meng_resume_date,
       MEN.file_path as final_meng_resume_name,
       C.kor_name,
       C.birth_date,
       C.gender
  FROM pjt_recandidate AS A 
         LEFT JOIN (SELECT RANK() OVER (partition by c_seq ORDER BY cr_seq desc) as r, * FROM can_Resume WHERE file_type = 'K' AND remove_dt is null) As KR
         ON A.c_seq = KR.c_seq
         AND KR.r = 1
         LEFT JOIN (SELECT RANK() OVER (partition by c_seq ORDER BY cr_seq desc) as r, * FROM can_Resume WHERE file_type = 'E' AND remove_dt is null) As EN
         ON A.c_seq = EN.c_seq
         AND EN.r = 1
         LEFT JOIN (SELECT RANK() OVER (partition by c_seq ORDER BY cr_seq desc) as r, * FROM can_Resume WHERE file_type = 'M' AND remove_dt is null) As MKR
         ON A.c_seq = MKR.c_seq
         AND MKR.r = 1
         LEFT JOIN (SELECT RANK() OVER (partition by c_seq ORDER BY cr_seq desc) as r, * FROM can_Resume WHERE file_type = 'N' AND remove_dt is null) As MEN
         ON A.c_seq = MEN.c_seq
         AND MEN.r = 1         
         INNER JOIN candidate AS C
         ON A.c_seq = C.c_seq 
  WHERE A.p_seq = @p_seq
";
          string where_str = "";
          if (pic_seq != null)
          {
            foreach (var pic in pic_seq){
              if (!String.IsNullOrEmpty(where_str))
              {
                where_str += ",";
              }
              where_str += pic.ToString();
            }
          }
          if (!String.IsNullOrEmpty(where_str))
          {
            selectQuery += " AND A.pic_seq in (" + where_str + ")";
          }

          selectQuery += " ORDER BY C.kor_name ";


            DynamicParameters param = new DynamicParameters();
          param.Add("@p_seq", p_seq, DbType.Int32);

          var ret = con.Query<pjt_recandidate_history>(selectQuery, param);


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

    public List<pjt_recandidate_history> SelectProjectReCanMakeupList(int p_seq, int c_seq)
    {
      try
      {
        List<pjt_recandidate_history> list = new List<pjt_recandidate_history>();

        using (IDbConnection con = new SqlConnection(base.BaseConnectionString))
        {
          con.Open();
          string selectQuery = @" 
  SELECT A.*,
       KR.cr_seq as final_kor_cr_seq,
       KR.file_dir as final_kor_resume_dir,
       KR.create_dt as final_kor_resume_date,
       KR.file_path as final_kor_resume_name,
       MKR.cr_seq as final_mkor_cr_seq,
       MKR.file_dir as final_mkor_resume_dir,
       MKR.create_dt as final_mkor_resume_date,
       MKR.file_path as final_mkor_resume_name,
       EN.cr_seq as final_eng_cr_seq,
       EN.file_dir as final_eng_resume_dir,
       EN.create_dt as final_eng_resume_date,
       EN.file_path as final_eng_resume_name,
       MEN.cr_seq as final_meng_cr_seq,
       MEN.file_dir as final_meng_resume_dir,
       MEN.create_dt as final_meng_resume_date,
       MEN.file_path as final_meng_resume_name,
       C.kor_name,
       C.birth_date,
       C.gender
  FROM pjt_recandidate AS A 
         LEFT JOIN (SELECT RANK() OVER (partition by c_seq ORDER BY cr_seq desc) as r, * FROM can_Resume WHERE file_type = 'K' AND remove_dt is null) As KR
         ON A.c_seq = KR.c_seq
         AND KR.r = 1
         LEFT JOIN (SELECT RANK() OVER (partition by c_seq ORDER BY cr_seq desc) as r, * FROM can_Resume WHERE file_type = 'E' AND remove_dt is null) As EN
         ON A.c_seq = EN.c_seq
         AND EN.r = 1
         LEFT JOIN (SELECT RANK() OVER (partition by c_seq ORDER BY cr_seq desc) as r, * FROM can_Resume WHERE file_type = 'M' AND remove_dt is null) As MKR
         ON A.c_seq = MKR.c_seq
         AND MKR.r = 1
         LEFT JOIN (SELECT RANK() OVER (partition by c_seq ORDER BY cr_seq desc) as r, * FROM can_Resume WHERE file_type = 'N' AND remove_dt is null) As MEN
         ON A.c_seq = MEN.c_seq
         AND MEN.r = 1         
         INNER JOIN candidate AS C
         ON A.c_seq = C.c_seq 
  WHERE A.p_seq = @p_seq
  AND   A.c_seq = @c_seq
";
          

          selectQuery += " ORDER BY C.kor_name ";


          DynamicParameters param = new DynamicParameters();
          param.Add("@p_seq", p_seq, DbType.Int32);
          param.Add("@c_seq", c_seq, DbType.Int32);

          var ret = con.Query<pjt_recandidate_history>(selectQuery, param);


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
    public async Task<List<pjt_recandidate_history>> SelectHireNoInvCandidateListAsync(int p_seq)
    {
      try
      {
        List<pjt_recandidate_history> list = new List<pjt_recandidate_history>();

        using (IDbConnection con = new SqlConnection(base.BaseConnectionString))
        {
          con.Open();
          string selectQuery = @" 
SELECT B.*, 
       C.kor_name,
       C.eng_name,
       C.birth_date,
       C.gender
FROM pjt_recandidate A INNER JOIN(select ROW_NUMBER() OVER(partition by pic_seq ORDER BY prc_seq DESC) as L, *
                                 FROM pjt_recandidate_history) B
                       ON A.p_seq = B.p_seq
                       AND A.pic_seq = B.pic_seq
                       AND B.L = 1
                       AND B.state = 80
                       INNER JOIN candidate C
                       ON B.c_seq = C.c_seq
                       LEFT JOIN pjt_invoice_info INV
                       ON B.p_seq = INV.p_seq
                       AND B.prc_seq = INV.prc_seq
                       AND INV.is_deleted = 0
                       AND INV.invoice_type NOT IN(4, 5)
                       AND NOT EXISTS(select 1
                                       FROM pjt_invoice_info CAN_INV
                                       WHERE CAN_INV.p_seq = INV.p_seq
                                       AND   CAN_INV.pre_pii = INV.pii_seq
                                       AND   CAN_INV.is_deleted = 0
                                       AND   CAN_INV.invoice_type IN(4, 5))



WHERE A.p_seq = @p_seq
AND INV.pii_seq IS NULL
ORDER BY B.schedule_date ";

          DynamicParameters param = new DynamicParameters();
          param.Add("@p_seq", p_seq, DbType.Int32);

          var ret = await con.QueryAsync<pjt_recandidate_history>(selectQuery, param);
          
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
    /// 프로젝트 진행사항 리스트 조회.
    /// </summary>
    /// <param name="ud_seq"></param>
    /// <param name="searchTxt"></param>
    /// <returns></returns>
    public List<pjt_recandidate_history> SelectProjectRecommandHistoryListAsync(int p_seq, int state, string orderTxt, string orderOption, int skip, int count, out int totalCount)
    {
      try
      {
        List<pjt_recandidate_history> list = new List<pjt_recandidate_history>();

        using (IDbConnection con = new SqlConnection(base.BaseConnectionString))
        {
          con.Open();
          string order_str = "";
          string selectQuery = @" 
  SELECT A.*,
       KR.cr_seq as final_kor_cr_seq,
       KR.file_dir as final_kor_resume_dir,
       KR.create_dt as final_kor_resume_date,
       KR.file_path as final_kor_resume_name,
       MKR.cr_seq as final_mkor_cr_seq,
       MKR.file_dir as final_mkor_resume_dir,
       MKR.create_dt as final_mkor_resume_date,
       MKR.file_path as final_mkor_resume_name,
       EN.cr_seq as final_eng_cr_seq,
       EN.file_dir as final_eng_resume_dir,
       EN.create_dt as final_eng_resume_date,
       EN.file_path as final_eng_resume_name,
       MEN.cr_seq as final_meng_cr_seq,
       MEN.file_dir as final_meng_resume_dir,
       MEN.create_dt as final_meng_resume_date,
       MEN.file_path as final_meng_resume_name,
       C.kor_name,
       CASE WHEN isnull(c.email1, '') = '' then C.email2 else c.email1 end as email,
       CASE WHEN isnull(c.phone, '') = '' then C.cell_phone else c.phone end as phone,
       CST.final_school,
       CST.final_major,
       CST.company_name,
       C.birth_date,
       C.gender,
       PRIV.agree_dt,
       PRIV.send_dt,
       PRIV.pa_seq
  FROM (SELECT ROW_NUMBER() OVER(PARTITION BY PR.c_seq ORDER BY MAX(RH.create_dt) DESC, MAX(RH.state) DESC) AS row
              ,PR.p_seq
              ,PR.pic_seq
              ,RH.prc_seq
              ,PR.c_seq
              ,RH.is_no_invoice
              ,RH.state
              ,RH.schedule_date
              ,PR.create_dt
              ,RH.modify_dt
              ,RH.contents
              ,RH.file_directory
              ,COUNT(PI.pii_seq) AS invoice_cnt
              ,U.name AS create_name
              ,U2.name AS modify_name
              ,PR.pin_to_top
          FROM pjt_recandidate AS PR INNER JOIN pjt_recandidate_history AS RH
                                             ON PR.p_seq = RH.p_seq
                                            AND PR.pic_seq = RH.pic_seq
                                LEFT OUTER JOIN pjt_invoice_info AS PI
                                             ON PR.p_seq = PI.p_seq
                                            AND PR.c_seq = PI.c_seq
                                            AND PI.is_deleted <> 1
                                     INNER JOIN uv_user AS U
                                             ON PR.create_user = U.uv_seq
                                     INNER JOIN uv_user AS U2
                                             ON RH.modify_user = U2.uv_seq
         WHERE PR.p_seq = @p_seq
         GROUP BY PR.p_seq, PR.pic_seq, RH.prc_seq, PR.c_seq, RH.schedule_date, RH.is_no_invoice, PR.create_dt, RH.state, RH.contents, RH.file_directory, U.name, RH.modify_dt, U2.name, PR.pin_to_top) AS A
         LEFT JOIN project AS p
         ON A.p_seq = p.p_seq
         LEFT JOIN (SELECT RANK() OVER (partition by c_seq ORDER BY cr_seq desc) as r, * FROM can_Resume WHERE file_type = 'K' AND remove_dt is null) As KR
         ON A.c_seq = KR.c_seq
         AND KR.r = 1
         LEFT JOIN (SELECT RANK() OVER (partition by c_seq ORDER BY cr_seq desc) as r, * FROM can_Resume WHERE file_type = 'E' AND remove_dt is null) As EN
         ON A.c_seq = EN.c_seq
         AND EN.r = 1
         LEFT JOIN (SELECT RANK() OVER (partition by c_seq ORDER BY cr_seq desc) as r, * FROM can_Resume WHERE file_type = 'M' AND remove_dt is null) As MKR
         ON A.c_seq = MKR.c_seq
         AND MKR.r = 1
         LEFT JOIN (SELECT RANK() OVER (partition by c_seq ORDER BY cr_seq desc) as r, * FROM can_Resume WHERE file_type = 'N' AND remove_dt is null) As MEN
         ON A.c_seq = MEN.c_seq
         AND MEN.r = 1
         LEFT JOIN (select RANK() OVER (partition by X.c_seq, Y.client_seq ORDER BY X.agree_dt desc, X.send_dt DESC) as r, X.*, Y.client_seq
from privacy_agree X inner join privacy_agree_dtl Y 
                     on x.pa_seq = y.pa_seq
                     and y.agree_type = 2
WHERE send_dt >= DATEADD(YEAR, -1, GETDATE())) As PRIV
         ON PRIV.c_seq = A.c_seq
         AND (PRIV.client_seq = p.c_seq or PRIV.client_seq = -1)
         AND PRIV.r = 1
         INNER JOIN candidate AS C
         ON A.c_seq = C.c_seq
         LEFT JOIN candidate_search_txt AS CST
         ON A.c_seq = CST.c_seq
         
 WHERE A.row = 1 
   AND A.state = CASE WHEN @state <> 0 THEN @state ELSE A.state END ";

          if (String.IsNullOrEmpty(orderOption))
            orderOption = "DESC";

          if (String.IsNullOrEmpty(orderTxt) || orderTxt == "A.state")
          {

            order_str = " A.state " + orderOption + ", A.pin_to_top DESC, A.modify_dt DESC ";

          }
          else
          {

            order_str = orderTxt + " " + orderOption;

            if (orderTxt != "A.modify_dt")
            {
              order_str = order_str + ", A.modify_dt DESC ";
            }
          }



          selectQuery += @"
ORDER BY " + order_str + " OFFSET @currentPage ROWS FETCH NEXT @pageSize ROWS ONLY ";

          string countQuery = @" SELECT COUNT(0)
  FROM (SELECT ROW_NUMBER() OVER(PARTITION BY PR.c_seq ORDER BY MAX(RH.create_dt) DESC, MAX(RH.state) DESC) AS row
              ,PR.p_seq
              ,PR.pic_seq
              ,RH.prc_seq
              ,PR.c_seq
              ,RH.is_no_invoice
              ,RH.state
              ,RH.schedule_date
              ,RH.contents
              ,RH.file_directory
              ,COUNT(PI.pii_seq) AS invoice_cnt
              ,U.name
          FROM pjt_recandidate AS PR INNER JOIN pjt_recandidate_history AS RH
                                             ON PR.p_seq = RH.p_seq
                                            AND PR.pic_seq = RH.pic_seq
                                LEFT OUTER JOIN pjt_invoice_info AS PI
                                             ON PR.p_seq = PI.p_seq
                                            AND PR.c_seq = PI.c_seq
                                            AND PI.is_deleted <> 1
                                     INNER JOIN uv_user AS U
                                             ON PR.create_user = U.uv_seq
         WHERE PR.p_seq = @p_seq
         GROUP BY PR.p_seq, PR.pic_seq, RH.prc_seq, PR.c_seq, RH.schedule_date, RH.is_no_invoice, RH.state, RH.contents, RH.file_directory, U.name) AS A
         INNER JOIN candidate AS C
         ON A.c_seq = C.c_seq
         LEFT JOIN candidate_search_txt AS CST
         ON A.c_seq = CST.c_seq
 WHERE A.row = 1 
   AND A.state = CASE WHEN @state <> 0 THEN @state ELSE A.state END ";

          DynamicParameters param = new DynamicParameters();
          param.Add("@p_seq", p_seq, DbType.Int32);
          param.Add("@state", state, DbType.Int32);
          param.Add("@currentPage", skip, DbType.Int32);
          param.Add("@pageSize", count, DbType.Int32);

          DynamicParameters param2 = new DynamicParameters();
          param2.Add("@p_seq", p_seq, DbType.Int32);
          param2.Add("@state", state, DbType.Int32);

          var ret = con.Query<pjt_recandidate_history>(selectQuery, param);
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


    public async Task<List<ProjectProgressListRawExcelModel>> SelectProjectProgressExcelListAsync(int p_seq, int state, string orderTxt, string orderOption)
    {
      try
      {
        List<ProjectProgressListRawExcelModel> list = new List<ProjectProgressListRawExcelModel>();

        using (IDbConnection con = new SqlConnection(base.BaseConnectionString))
        {
          con.Open();
          //프로젝트> 후보자목록> 엑셀로 다운로드할 수 있는 기능  
          //▶ 회사 / 이름 / 성별 / 생년 / 학력 / 경력 / 참고사항 / 후보자메모

          string selectQuery = @" 
  
select C.kor_name as kor_name,
       A.state,
       CASE C.gender WHEN 1 then '남' WHEN 2 THEN '여' ELSE '모름' END As gender,
       CONVERT(VARCHAR(10), C.birth_date, 121) as birth_date,
       CASE WHEN isnull(c.email1, '') = '' then C.email2 else c.email1 end as email,
       CASE WHEN isnull(c.phone, '') = '' then C.cell_phone else c.phone end as phone, 
       STUFF((SELECT CHAR(10) + '['+CONVERT(VARCHAR(10), RH1.schedule_date, 121) +']'+ ISNULL(RH1.contents, '')
                                                  FROM   pjt_recandidate_history AS RH1 
                                                  WHERE  RH1.pic_seq = A.pic_seq
                                                  ORDER BY prc_seq DESC
                                                  FOR XML PATH('')),1,1,'') AS comment,
       STUFF((SELECT CHAR(10) + ISNULL(B1.school_name, '')
                             + CASE WHEN ISNULL(B1.campus_name, '') = '' THEN '' ELSE ' ' + ISNULL(B1.campus_name, '') END
                                    + ' ' + ISNULL(A1.major_name, '')
                                    + ' ' + ISNULL(C1.name, '') + ' ' + ISNULL(D1.name, '')
                                FROM   can_school AS A1 LEFT JOIN code_school AS B1
                                                      ON A1.sc_seq = B1.sc_seq
                                                      LEFT JOIN code_can_education AS C1
                                                      ON A1.gubun = C1.code
                                                      LEFT JOIN code_graduate_status AS D1
                                                      ON A1.graduate_status = D1.code
                                                      AND D1.code > 0
                                WHERE  A1.c_seq = C.c_seq
                                order by order_no 
                                FOR XML PATH('')),1,1,'') AS edu,
      STUFF((SELECT CHAR(10) + ISNULL(X.company_name, '')
                              FROM   can_career AS X 
                              WHERE  X.c_seq = C.c_seq
                              order by order_no 
                              FOR XML PATH('')),1,1,'') AS exp,
      STUFF((SELECT CHAR(10) +ISNULL(CONVERT(VARCHAR(MAX), X.memo), '')
                              FROM   can_activity AS X 
                              WHERE  X.c_seq = C.c_seq
                              AND    X.cl_seq is null
                              order by ca_seq DESC 
                              FOR XML PATH('')),1,1,'') AS can_memo 
from pjt_recandidate X inner join candidate C
                       on X.c_seq = C.c_seq
                       inner join (select ROW_NUMBER() over(partition by pic_seq order by prc_seq desc) as r1, pic_seq, state, modify_dt
                                   from pjt_recandidate_history RHS ) A
                       ON X.pic_seq = A.pic_seq
                       AND A.r1 = 1
                       AND A.state = CASE WHEN @state <> 0 THEN @state ELSE A.state END
WHERE X.p_seq = @p_seq ";

          if (String.IsNullOrEmpty(orderTxt))
            orderTxt = "A.state";
          if (String.IsNullOrEmpty(orderOption))
            orderOption = "DESC";

          selectQuery += @"
ORDER BY " + orderTxt + " " + orderOption;



          DynamicParameters param = new DynamicParameters();
          param.Add("@p_seq", p_seq, DbType.Int32);
          param.Add("@state", state, DbType.Int32);



          var ret = await con.QueryAsync<ProjectProgressListRawExcelModel>(selectQuery, param);


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

    public async Task<pjt_recandidate_history> SelectProjectRecommandHistoryOneAsync(int p_seq, int pic_seq)
    {
      try
      {
        pjt_recandidate_history list = new pjt_recandidate_history();

        using (IDbConnection con = new SqlConnection(base.BaseConnectionString))
        {
          con.Open();

          string selectQuery = @" 
  SELECT A.*,
       KR.cr_seq as final_kor_cr_seq,
       KR.file_dir as final_kor_resume_dir,
       KR.create_dt as final_kor_resume_date,
       KR.file_path as final_kor_resume_name,
       MKR.cr_seq as final_mkor_cr_seq,
       MKR.file_dir as final_mkor_resume_dir,
       MKR.create_dt as final_mkor_resume_date,
       MKR.file_path as final_mkor_resume_name,
       EN.cr_seq as final_eng_cr_seq,
       EN.file_dir as final_eng_resume_dir,
       EN.create_dt as final_eng_resume_date,
       EN.file_path as final_eng_resume_name,
       MEN.cr_seq as final_meng_cr_seq,
       MEN.file_dir as final_meng_resume_dir,
       MEN.create_dt as final_meng_resume_date,
       MEN.file_path as final_meng_resume_name
  FROM (SELECT ROW_NUMBER() OVER(PARTITION BY PR.c_seq ORDER BY MAX(RH.create_dt) DESC, MAX(RH.state) DESC) AS row
              ,PR.p_seq
              ,PR.pic_seq
              ,RH.prc_seq
              ,PR.c_seq
              ,C.kor_name
              ,RH.state
              ,RH.schedule_date
              ,RH.contents
              ,RH.file_directory
              ,COUNT(PI.pii_seq) AS invoice_cnt
              ,U.name AS create_name
          FROM pjt_recandidate AS PR INNER JOIN pjt_recandidate_history AS RH
                                             ON PR.p_seq = RH.p_seq
                                            AND PR.pic_seq = RH.pic_seq
                                     INNER JOIN candidate AS C
                                             ON PR.c_seq = C.c_seq
                                LEFT OUTER JOIN pjt_invoice_info AS PI
                                             ON PR.p_seq = PI.p_seq
                                            AND PR.c_seq = PI.c_seq
                                            AND PI.is_deleted <> 1
                                     INNER JOIN uv_user AS U
                                             ON PR.create_user = U.uv_seq
         WHERE PR.p_seq = @p_seq
         AND   PR.pic_seq = @pic_seq
         GROUP BY PR.p_seq, PR.pic_seq, RH.prc_seq, PR.c_seq, C.kor_name, RH.schedule_date, RH.state, RH.contents, RH.file_directory, U.name) AS A
         LEFT JOIN (SELECT RANK() OVER (partition by c_seq ORDER BY cr_seq desc) as r, * FROM can_Resume WHERE file_type = 'K' AND remove_dt is null) As KR
         ON A.c_seq = KR.c_seq
         AND KR.r = 1
         LEFT JOIN (SELECT RANK() OVER (partition by c_seq ORDER BY cr_seq desc) as r, * FROM can_Resume WHERE file_type = 'E' AND remove_dt is null) As EN
         ON A.c_seq = EN.c_seq
         AND EN.r = 1
         LEFT JOIN (SELECT RANK() OVER (partition by c_seq ORDER BY cr_seq desc) as r, * FROM can_Resume WHERE file_type = 'M' AND remove_dt is null) As MKR
         ON A.c_seq = MKR.c_seq
         AND MKR.r = 1
         LEFT JOIN (SELECT RANK() OVER (partition by c_seq ORDER BY cr_seq desc) as r, * FROM can_Resume WHERE file_type = 'N' AND remove_dt is null) As MEN
         ON A.c_seq = MEN.c_seq
         AND MEN.r = 1
 WHERE A.row = 1 
 ORDER BY A.state DESC, A.kor_name ASC";



          DynamicParameters param = new DynamicParameters();
          param.Add("@p_seq", p_seq, DbType.Int32);
          param.Add("@pic_seq", pic_seq, DbType.Int32);

          var ret = await con.QueryFirstOrDefaultAsync<pjt_recandidate_history>(selectQuery, param);

          //list = ret.FirstOrDefault();

          con.Close();

          return ret;
        }
      }
      catch (Exception e)
      {
        throw e;
      }
    }

    /// <summary>
    /// 프로젝트 진행사항 리스트 조회.
    /// </summary>
    /// <param name="pic_seq"></param>
    /// <returns></returns>
    public async Task<List<pjt_recandidate_history>> SelectProjectRecommandHistoryListAsync(int pic_seq)
    {
      try
      {
        List<pjt_recandidate_history> list = new List<pjt_recandidate_history>();

        using (IDbConnection con = new SqlConnection(base.BaseConnectionString))
        {
          con.Open();

          string selectQuery = @" SELECT RH.*
      ,C.kor_name
  FROM pjt_recandidate AS PR INNER JOIN pjt_recandidate_history AS RH
                                     ON PR.p_seq = RH.p_seq
                                    AND PR.pic_seq = RH.pic_seq
                             INNER JOIN candidate AS C
                                     ON PR.c_seq = C.c_seq
 WHERE RH.pic_seq = @pic_seq 
 ORDER BY RH.create_dt DESC ";


          DynamicParameters param = new DynamicParameters();
          param.Add("@pic_seq", pic_seq, DbType.Int32);

          var ret = await con.QueryAsync<pjt_recandidate_history>(selectQuery, param);

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

    public async Task<List<pjt_recandidate_history>> SelectProjectRecandidateListAsync(int p_seq, List<int> pic_seqs)
    {
      try
      {
        List<pjt_recandidate_history> list = new List<pjt_recandidate_history>();

        using (IDbConnection con = new SqlConnection(base.BaseConnectionString))
        {
          con.Open();

          string selectQuery = @" SELECT PR.*
      ,C.kor_name
  FROM pjt_recandidate AS PR INNER JOIN candidate AS C
                                        ON PR.c_seq = C.c_seq
 WHERE PR.p_seq = @p_seq 
 AND   PR.pic_seq IN (" + String.Join(",", pic_seqs) + @") 
 ORDER BY PR.pic_seq DESC ";


          DynamicParameters param = new DynamicParameters();
          param.Add("@p_seq", p_seq, DbType.Int32);

          var ret = await con.QueryAsync<pjt_recandidate_history>(selectQuery, param);

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

    public async Task<List<pjt_recandidate_history>> SelectProjectRecommandHistoryListAsync(int p_seq, int c_seq)
    {
      try
      {
        List<pjt_recandidate_history> list = new List<pjt_recandidate_history>();

        using (IDbConnection con = new SqlConnection(base.BaseConnectionString))
        {
          con.Open();

          string selectQuery = @" SELECT RH.*
      ,C.kor_name
      ,U.name As create_name
  FROM pjt_recandidate AS PR INNER JOIN pjt_recandidate_history AS RH
                                     ON PR.p_seq = RH.p_seq
                                    AND PR.pic_seq = RH.pic_seq
                             INNER JOIN candidate AS C
                                     ON PR.c_seq = C.c_seq
                             INNER JOIN UV_USER AS U
                                     ON RH.create_user = U.uv_seq
 WHERE RH.p_seq = @p_seq 
 AND   RH.c_seq = @c_seq 
 ORDER BY RH.create_dt DESC ";


          DynamicParameters param = new DynamicParameters();
          param.Add("@p_seq", p_seq, DbType.Int32);
          param.Add("@c_seq", c_seq, DbType.Int32);

          var ret = await con.QueryAsync<pjt_recandidate_history>(selectQuery, param);

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

    public async Task<List<makeup_request>> SelectMakeupList(MakeupRequestSearchModel search)
    {
      //int skip, int count, out int totalCount
      try
      {
        List<makeup_request> list = new List<makeup_request>();

        using (IDbConnection con = new SqlConnection(base.BaseConnectionString))
        {
          con.Open();

          string selectQuery = @" 
SELECT A.mr_idx
      ,A.p_seq
      ,A.c_seq
      ,A.request_date
      ,A.complete_date
      ,A.status
      ,A.resume_type
      ,A.req_user
      ,C.name
      ,B.kor_name
      ,P.title
FROM MAKEUP_REQUEST A LEFT JOIN CANDIDATE B
                      ON A.c_seq = B.c_seq
                      LEFT JOIN uv_user C
                      ON A.req_user = C.uv_seq
                      LEFT JOIN project P
                      ON A.p_seq = P.p_seq
WHERE A.del_yn = 0 
      
";

          if (search.search_status == 1)
            selectQuery += " AND   A.complete_date is null ";
          else if (search.search_status == 2)
            selectQuery += " AND   A.complete_date is not null ";

          if (!String.IsNullOrEmpty(search.start_date) && !String.IsNullOrEmpty(search.end_date))
            selectQuery += " AND   CONVERT(VARCHAR(10), A.request_date, 121) between @start_date and @end_date  ";

          if (!String.IsNullOrEmpty(search.search_field) && !String.IsNullOrEmpty(search.search_value))
            selectQuery += "  AND @search_field LIKE '%' + @search_value + '%' ";

          //AND P.create_user = @uv_seq 
          selectQuery += "ORDER BY A.request_date ASC";
          //selectQuery += " OFFSET @currentPage ROWS FETCH NEXT @pageSize ROWS ONLY ";
          /*
                              string countQuery = @" 
          SELECT COUNT(0)
          FROM MAKEUP_REQUEST A LEFT JOIN CANDIDATE B
                                ON A.c_seq = B.c_seq
                                LEFT JOIN uv_user C
                                ON A.req_user = B.uv_seq
          WHERE A.del_yn = 0  ";


                              if (search.search_status == 1)
                                  countQuery += " AND   A.complete_date is null ";
                              else if (search.search_status == 2)
                                  countQuery += " AND   A.complete_date is not null ";

                              if (!String.IsNullOrEmpty(search.start_date) && !String.IsNullOrEmpty(search.end_date))
                                  countQuery += " AND   CONVERT(VARCHAR(10), A.request_date, 121) between @start_date and @end_date  ";

                              if (!String.IsNullOrEmpty(search.search_field) && !String.IsNullOrEmpty(search.search_value))
                                  countQuery += "  AND @search_field LIKE '%' + @search_value + '%' ";

          */
          DynamicParameters param = new DynamicParameters();
          param.Add("@search_field", search.search_field, DbType.String);
          param.Add("@search_value", search.search_value, DbType.String);
          param.Add("@start_date", search.start_date, DbType.String);
          param.Add("@end_date", search.end_date, DbType.String);

          //param.Add("@currentPage", skip, DbType.Int32);
          //param.Add("@pageSize", count, DbType.Int32);
          /*
                              DynamicParameters param2 = new DynamicParameters();
                              param2.Add("@search_field", search.search_field, DbType.String);
                              param2.Add("@search_value", search.search_value, DbType.String);
                              param2.Add("@start_date", search.start_date, DbType.String);
                              param2.Add("@end_date", search.end_date, DbType.String);
          */
          var ret = await con.QueryAsync<makeup_request>(selectQuery, param);
          //                    totalCount = con.QueryFirstOrDefault<int>(countQuery, param2);

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

    public async Task<makeup_request> SelectMakeupRequestOneAsync(int mr_idx)
    {
      try
      {
        using (IDbConnection con = new SqlConnection(base.BaseConnectionString))
        {
          con.Open();

          string selectQuery = @" SELECT A.*
      ,C.name
      ,B.kor_name
      ,P.title
  FROM MAKEUP_REQUEST A LEFT JOIN CANDIDATE B
                      ON A.c_seq = B.c_seq
                      LEFT JOIN uv_user C
                      ON A.req_user = C.uv_seq
                      LEFT JOIN project P
                      ON A.p_seq = P.p_seq
 WHERE A.mr_idx = @mr_idx ";

          DynamicParameters param = new DynamicParameters();
          param.Add("@mr_idx", mr_idx, DbType.Int32);

          var ret = await con.QueryAsync<makeup_request>(selectQuery, param);

          con.Close();

          return ret.FirstOrDefault();
        }
      }
      catch (Exception e)
      {
        throw e;
      }
    }

    public async Task<int> SelectMakeupWaitingCnt()
    {
      try
      {
        int waiting_cnt = 0;

        using (IDbConnection con = new SqlConnection(base.BaseConnectionString))
        {
          con.Open();

          string selectQuery = @" 
SELECT Count(*) as Cnt
FROM makeup_request
WHERE del_yn = 0
AND   complete_date is null
";

          waiting_cnt = await con.QueryFirstOrDefaultAsync<int>(selectQuery, null);

          con.Close();

          return waiting_cnt;
        }
      }
      catch (Exception e)
      {
        throw e;
      }
    }

    /// <summary>
    /// 프로젝트 진행사항 관심후보 단건 조회.
    /// </summary>
    /// <param name="p_seq"></param>
    /// <param name="uv_seq"></param>
    /// <param name="c_seq"></param>
    /// <param name="state"></param>
    /// <returns></returns>
    public async Task<pjt_recandidate_history> SelectProjectRecommandCandiOneAsync(int prc_seq)
    {
      try
      {
        using (IDbConnection con = new SqlConnection(base.BaseConnectionString))
        {
          con.Open();

          string selectQuery = @" SELECT RH.*
      ,C.kor_name
  FROM pjt_recandidate AS PR INNER JOIN pjt_recandidate_history AS RH
                                     ON PR.p_seq = RH.p_seq
                                    AND PR.pic_seq = RH.pic_seq
                             INNER JOIN candidate AS C
                                     ON PR.c_seq = C.c_seq
 WHERE RH.prc_seq = @prc_seq ";

          DynamicParameters param = new DynamicParameters();
          param.Add("@prc_seq", prc_seq, DbType.Int32);

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

    public async Task<pjt_recandidate_history> SelectProjectRecommandCreateCandiOneAsync(int pic_seq)
    {
      try
      {
        using (IDbConnection con = new SqlConnection(base.BaseConnectionString))
        {
          con.Open();

          string selectQuery = @" SELECT TOP 1 RH.*
      ,C.kor_name
  FROM pjt_recandidate AS PR INNER JOIN pjt_recandidate_history AS RH
                                     ON PR.p_seq = RH.p_seq
                                    AND PR.pic_seq = RH.pic_seq
                             INNER JOIN candidate AS C
                                     ON PR.c_seq = C.c_seq
 WHERE PR.pic_seq = @pic_seq 
 ORDER BY RH.prc_seq DESC ";

          DynamicParameters param = new DynamicParameters();
          param.Add("@pic_seq", pic_seq, DbType.Int32);

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


    public async Task<pjt_recandidate_history> SelectProjectRecommandCreateCandiOneByPCAsync(int p_seq, int c_seq)
    {
      try
      {
        using (IDbConnection con = new SqlConnection(base.BaseConnectionString))
        {
          con.Open();

          string selectQuery = @" SELECT TOP 1 RH.*
      ,C.kor_name
  FROM pjt_recandidate AS PR INNER JOIN pjt_recandidate_history AS RH
                                     ON PR.p_seq = RH.p_seq
                                    AND PR.pic_seq = RH.pic_seq
                             INNER JOIN candidate AS C
                                     ON PR.c_seq = C.c_seq
 WHERE PR.p_seq = @p_seq 
   AND PR.c_seq = @c_seq 
 ORDER BY RH.prc_seq DESC ";

          DynamicParameters param = new DynamicParameters();
          param.Add("@p_seq", p_seq, DbType.Int32);
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
    /// 프로젝트 간편후보자 리스트 조회
    /// </summary>
    /// <param name="p_seq"></param>
    /// <param name="skip"></param>
    /// <param name="count"></param>
    /// <param name="totalCount"></param>
    /// <returns></returns>
    public List<ProjectSimpleCandidateModel> SelectProjectSimpleCandidateList(int p_seq, int skip, int count, out int totalCount)
    {
      try
      {
        List<ProjectSimpleCandidateModel> list = new List<ProjectSimpleCandidateModel>();

        using (IDbConnection con = new SqlConnection(base.BaseConnectionString))
        {
          con.Open();

          string selectQuery = @" SELECT P.p_seq
      ,PT.pti_seq
      ,SC.sc_seq
      ,SC.kor_name
      ,SC.gender
      ,SC.birthdate
      ,SC.cell_phone
      ,SC.email
      ,SC.sns_url
      ,SC.company
      ,SC.comments
      ,PT.memo
  FROM project AS P INNER JOIN pjt_tmp_icandidate AS PT 
                            ON P.p_seq = PT.p_seq
                    INNER JOIN simple_candidate AS SC
                            ON PT.sc_seq = SC.sc_seq
 WHERE P.p_seq = @p_seq 
 ORDER BY SC.kor_name DESC 
OFFSET @currentPage ROWS FETCH NEXT @pageSize ROWS ONLY ";

          string countQuery = @" SELECT COUNT(0)
  FROM project AS P INNER JOIN pjt_tmp_icandidate AS PT 
                            ON P.p_seq = PT.p_seq
                    INNER JOIN simple_candidate AS SC
                            ON PT.sc_seq = SC.sc_seq
 WHERE P.p_seq = @p_seq  ";

          DynamicParameters param = new DynamicParameters();
          param.Add("@p_seq", p_seq, DbType.Int32);
          param.Add("@currentPage", skip, DbType.Int32);
          param.Add("@pageSize", count, DbType.Int32);

          DynamicParameters param2 = new DynamicParameters();
          param2.Add("@p_seq", p_seq, DbType.Int32);

          var ret = con.Query<ProjectSimpleCandidateModel>(selectQuery, param);
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
    /// 프로젝트 인보이스 리스트 조회.
    /// </summary>
    /// <param name="p_seq"></param>
    /// <param name="skip"></param>
    /// <param name="count"></param>
    /// <param name="totalCount"></param>
    /// <returns></returns>
    public List<pjt_invoice_info> SelectProjectInvoiceList(int p_seq, int skip, int count, out int totalCount)
    {
      try
      {
        List<pjt_invoice_info> list = new List<pjt_invoice_info>();

        using (IDbConnection con = new SqlConnection(base.BaseConnectionString))
        {
          con.Open();

          string selectQuery = @" SELECT U1.name AS create_user_name
      ,U2.name AS confirm_user_name
      ,C.kor_name AS candidate_name
      ,PI.*
  FROM pjt_invoice_info AS PI INNER JOIN uv_user AS U1
                                      ON PI.create_user = U1.uv_seq
                         LEFT OUTER JOIN uv_user AS U2
                                      ON PI.confirm_user = U2.uv_seq
                              LEFT JOIN candidate AS C
                                      ON PI.c_seq = C.c_seq
 WHERE PI.p_seq = @p_seq
   AND PI.is_deleted <> 1
 ORDER BY PI.pii_seq DESC 
OFFSET @currentPage ROWS FETCH NEXT @pageSize ROWS ONLY ";

          string countQuery = @" SELECT COUNT(0)
  FROM pjt_invoice_info AS PI INNER JOIN uv_user AS U1
                                      ON PI.create_user = U1.uv_seq
                         LEFT OUTER JOIN uv_user AS U2
                                      ON PI.confirm_user = U2.uv_seq
                              INNER JOIN candidate AS C
                                      ON PI.c_seq = C.c_seq
 WHERE PI.p_seq = @p_seq 
   AND PI.is_deleted <> 1";

          DynamicParameters param = new DynamicParameters();
          param.Add("@p_seq", p_seq, DbType.Int32);
          param.Add("@currentPage", skip, DbType.Int32);
          param.Add("@pageSize", count, DbType.Int32);

          DynamicParameters param2 = new DynamicParameters();
          param2.Add("@p_seq", p_seq, DbType.Int32);

          var ret = con.Query<pjt_invoice_info>(selectQuery, param);
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
    /// 프로젝트 평판조회 인보이스
    /// </summary>
    /// <param name="p_seq"></param>
    /// <param name="skip"></param>
    /// <param name="count"></param>
    /// <param name="totalCount"></param>
    /// <returns></returns>
    public List<pjt_invoice_info> SelectProjectRepInvoiceList(int p_seq, int skip, int count, out int totalCount)
    {
      try
      {
        List<pjt_invoice_info> list = new List<pjt_invoice_info>();

        using (IDbConnection con = new SqlConnection(base.BaseConnectionString))
        {
          con.Open();

          string selectQuery = @" SELECT U1.name AS create_user_name
      ,U2.name AS confirm_user_name
      ,PI.*
  FROM pjt_invoice_info AS PI INNER JOIN uv_user AS U1
                                      ON PI.create_user = U1.uv_seq
                         LEFT OUTER JOIN uv_user AS U2
                                      ON PI.confirm_user = U2.uv_seq
 WHERE PI.p_seq = @p_seq
   AND PI.is_deleted <> 1
 ORDER BY PI.pii_seq DESC 
OFFSET @currentPage ROWS FETCH NEXT @pageSize ROWS ONLY ";

          string countQuery = @" SELECT COUNT(0)
  FROM pjt_invoice_info AS PI INNER JOIN uv_user AS U1
                                      ON PI.create_user = U1.uv_seq
                         LEFT OUTER JOIN uv_user AS U2
                                      ON PI.confirm_user = U2.uv_seq
 WHERE PI.p_seq = @p_seq 
   AND PI.is_deleted <> 1 ";

          DynamicParameters param = new DynamicParameters();
          param.Add("@p_seq", p_seq, DbType.Int32);
          param.Add("@currentPage", skip, DbType.Int32);
          param.Add("@pageSize", count, DbType.Int32);

          DynamicParameters param2 = new DynamicParameters();
          param2.Add("@p_seq", p_seq, DbType.Int32);

          var ret = con.Query<pjt_invoice_info>(selectQuery, param);
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
    /// 프로젝트 인보이스 조회
    /// </summary>
    /// <param name="p_seq"></param>
    /// <param name="c_seq"></param>
    /// <returns></returns>
    public async Task<pjt_invoice_info> SelectProjectInvoiceOneAsync(int p_seq, int c_seq, int pii_seq)
    {
      try
      {
        using (IDbConnection con = new SqlConnection(base.BaseConnectionString))
        {
          con.Open();

          string selectQuery = @" SELECT P.p_seq
      ,ISNULL(P.cc_seq, 0) AS cc_seq
      ,ISNULL(P.ctc_seq, 0) AS ctc_seq
      ,PR.c_seq
      ,PI.pii_seq
      ,ISNULL(PI.candidate_name, C.kor_name) AS candidate_name
      ,C.eng_name AS candidate_eng_name
      ,ISNULL(PI.join_dt, RH.schedule_date) AS join_dt
      ,PI.billing_dt
      ,ISNULL(PI.annual_income, ISNULL(RH.annual_income, 0)) AS annual_income
      ,ISNULL(PI.ann_income, ISNULL(RH.ann_income, 0)) AS ann_income
      ,ISNULL(PI.income_currency_cd, ISNULL(RH.currency_cd, P.currency_cd)) AS income_currency_cd
      ,ISNULL(PI.fee_rate, P.fee_rate) as fee_rate
      ,ISNULL(PI.billing_money, 0) AS billing_money
      ,ISNULL(PI.billing_total, 0) AS billing_total
      ,ISNULL(PI.billing_amt, 0) AS billing_amt
      ,ISNULL(PI.billing_vat, 0) AS billing_vat
      ,ISNULL(PI.bill_currency_cd, ISNULL(RH.currency_cd, P.currency_cd)) AS bill_currency_cd
      ,PI.billing_type
      ,PI.expire_guarantee
      ,ISNULL(PI.join_dt, RH.guarantee_dt) as expire_guarantee
      ,ISNULL(PI.vat_type, 1) as vat_type
      ,RH.prc_seq
      ,CI.c_seq AS client_seq
      ,ISNULL(PI.client_name, CI.kor_name) AS client_name
      ,ISNULL(CI.eng_name, '') AS client_eng_name
      ,CI.ceo
      ,CI.addr1 + ISNULL(CI.addr2, '') AS addr
      ,CI.biz_code
      ,ISNULL(PI.is_po_no, 0) AS is_po_no
      ,ISNULL(PI.is_open_name, 1) AS is_open_name
      ,ISNULL(PI.invoice_lang, 0) AS invoice_lang
      ,ISNULL(PI.invoice_type, 0) AS invoice_type
      ,ISNULL(PI.is_open_annual_income, 1) AS is_open_annual_income
      ,PI.remarks
      ,PI.client_contact_name 
      ,PI.client_contact_email 
      ,PI.client_contact_phone 
      ,PI.client_contact_division
      ,PI.etax_name
      ,PI.etax_email
      ,PI.etax_phone
      ,PI.candidate_source
      ,PI.candidate_source_txt
      ,PI.candidate_position
      ,PI.candidate_position_txt
  FROM project AS P INNER JOIN client AS CI
                            ON P.c_seq = CI.c_seq
                    INNER JOIN pjt_recandidate AS PR
                            ON P.p_seq = PR.p_seq
                    INNER JOIN (SELECT TOP 1 p_seq
                                      ,pic_seq
                                      ,prc_seq
                                      ,c_seq
                                      ,schedule_date
                                      ,annual_income
                                      ,ann_income
                                      ,currency_cd
                                      ,guarantee_dt
                                 FROM pjt_recandidate_history
                                WHERE p_seq = @p_seq
                                  AND c_seq = @c_seq
                                  AND state = 80 ORDER BY prc_seq DESC) AS RH
                            ON PR.pic_seq = RH.pic_seq
                           AND PR.p_seq = RH.p_seq
                           AND PR.c_seq = RH.c_seq
                    INNER JOIN candidate AS C
                            ON PR.c_seq = C.c_seq
               LEFT OUTER JOIN pjt_invoice_info AS PI
                            ON P.p_seq = PI.p_seq
                            AND RH.c_seq = PI.c_seq
                            AND PI.is_deleted <> 1 
WHERE P.p_seq = @p_seq 
";

          if (c_seq != 0)
          {
            selectQuery += @"
AND C.c_seq = @c_seq ";
          }

          if (pii_seq != 0)
          {
            selectQuery += @"
AND PI.pii_seq = @pii_seq ";
          }


          DynamicParameters param = new DynamicParameters();
          param.Add("@p_seq", p_seq, DbType.Int32);
          param.Add("@c_seq", c_seq, DbType.Int32);
          param.Add("@pii_seq", pii_seq, DbType.Int32);

          var ret = await con.QueryAsync<pjt_invoice_info>(selectQuery, param);

          con.Close();

          return ret.FirstOrDefault();
        }
      }
      catch (Exception e)
      {
        throw e;
      }
    }

    public async Task<double> SelectProjectRetainerMoneyAsync(int p_seq, int pii_seq)
    {
      try
      {
        using (IDbConnection con = new SqlConnection(base.BaseConnectionString))
        {
          con.Open();

          string selectQuery = @" 
SELECT ISNULL(SUM(PI.billing_money), 0) as retainer_money
FROM pjt_invoice_info AS PI
WHERE PI.p_seq = @p_seq 
AND PI.is_deleted <> 1 
AND PI.invoice_type = 1
";

          if (pii_seq != 0)
          {
            selectQuery += @"
AND PI.pii_seq <> @pii_seq ";
          }


          DynamicParameters param = new DynamicParameters();
          param.Add("@p_seq", p_seq, DbType.Int32);
          param.Add("@pii_seq", pii_seq, DbType.Int32);

          var ret = await con.QueryAsync<double>(selectQuery, param);

          con.Close();

          return ret.FirstOrDefault();
        }
      }
      catch (Exception e)
      {
        return (double)0;
      }
    }

    public async Task<pjt_invoice_info> SelectProjectInvoiceViewOneAsync(int p_seq, int pii_seq)
    {
      try
      {
        using (IDbConnection con = new SqlConnection(base.BaseConnectionString))
        {
          con.Open();

          string selectQuery = @" SELECT P.p_seq
      ,ISNULL(P.cc_seq, 0) AS cc_seq
      ,ISNULL(P.ctc_seq, 0) AS ctc_seq
      ,PI.*
  FROM pjt_invoice_info AS PI LEFT JOIN PROJECT P
                              ON PI.p_seq = P.p_seq
WHERE PI.p_seq = @p_seq 
AND   PI.pii_seq = @pii_seq 
";



          DynamicParameters param = new DynamicParameters();
          param.Add("@p_seq", p_seq, DbType.Int32);
          param.Add("@pii_seq", pii_seq, DbType.Int32);

          var ret = await con.QueryAsync<pjt_invoice_info>(selectQuery, param);

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
    /// 프로젝트 평판조회 인보이스 조회.
    /// </summary>
    /// <param name="p_seq"></param>
    /// <param name="c_seq"></param>
    /// <returns></returns>
    public async Task<pjt_invoice_info> SelectProjectRepInvoiceOneAsync(int p_seq)
    {
      try
      {
        using (IDbConnection con = new SqlConnection(base.BaseConnectionString))
        {
          con.Open();

          string selectQuery = @" SELECT P.p_seq
      ,PI.pii_seq
      ,PI.billing_dt
      ,PI.billing_money
      ,PI.billing_won
      ,PI.vat_type 
      ,P.exp_salary as ann_income
      ,PI.billing_type
      ,ISNULL(PI.fee_rate, P.fee_rate) as fee_rate
      ,ISNULL(PI.bill_currency_cd, P.currency_cd) AS bill_currency_cd
      ,PI.expire_guarantee
      ,ISNULL(PI.client_name, CI.kor_name) AS client_name
      ,ISNULL(CI.eng_name, '') AS client_eng_name
      ,CI.ceo
      ,CI.addr1 + CI.addr2 AS addr
      ,CI.biz_code
      ,ISNULL(PI.is_open_name, 0) AS is_open_name
      ,ISNULL(PI.invoice_lang, 0) AS invoice_lang
      ,ISNULL(PI.invoice_type, 0) AS invoice_type
      ,ISNULL(PI.is_open_annual_income, 0) AS is_open_annual_income
      ,PI.remarks
  FROM project AS P INNER JOIN client AS CI
                            ON P.c_seq = CI.c_seq
               LEFT OUTER JOIN pjt_invoice_info AS PI
                            ON P.p_seq = PI.p_seq
                           AND PI.is_deleted <> 1
 WHERE P.p_seq = @p_seq ";

          DynamicParameters param = new DynamicParameters();
          param.Add("@p_seq", p_seq, DbType.Int32);

          var ret = await con.QueryAsync<pjt_invoice_info>(selectQuery, param);

          con.Close();

          return ret.FirstOrDefault();
        }
      }
      catch (Exception e)
      {
        throw e;
      }
    }

    public async Task<pjt_invoice_info> SelectProjectInvoiceOneByClientAsync(int c_seq, int type=-1)
    {
      try
      {
        using (IDbConnection con = new SqlConnection(base.BaseConnectionString))
        {
          con.Open();

          string selectQuery = @" SELECT 
      PI.*
      
  FROM project AS P INNER JOIN client AS CI
                            ON P.c_seq = CI.c_seq
               LEFT OUTER JOIN pjt_invoice_info AS PI
                            ON P.p_seq = PI.p_seq
                           AND PI.is_deleted <> 1
 WHERE CI.c_seq = @c_seq ";

          if (type >= 0)
          {
            selectQuery += @" AND PI.invoice_type = @type ";
          }
          selectQuery += @" ORDER BY PI.pii_seq DESC ";

          DynamicParameters param = new DynamicParameters();
          param.Add("@c_seq", c_seq, DbType.Int32);
          param.Add("@type", type, DbType.Int32);

          var ret = await con.QueryAsync<pjt_invoice_info>(selectQuery, param);

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
    /// 프로젝트에서 같은 후보자로 중복 인보이스가 발행된 건이 있는지 카운트
    /// </summary>
    /// <param name="candidate_seq"></param>
    /// <param name="client_seq"></param>
    /// <returns></returns>
    public async Task<int> SelectCandidateSameInvoiceCountAsync(int candidate_seq, int client_seq)
    {
      try
      {
        using (IDbConnection con = new SqlConnection(base.BaseConnectionString))
        {
          con.Open();

          string selectQuery = @" SELECT COUNT(0)
  FROM pjt_invoice_info AS PI INNER JOIN project AS P
                                      ON PI.p_seq = P.p_seq
 WHERE PI.c_seq = @candidate_seq
   AND P.c_seq = @client_seq";

          DynamicParameters param = new DynamicParameters();
          param.Add("@candidate_seq", candidate_seq, DbType.Int32);
          param.Add("@client_seq", client_seq, DbType.Int32);

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
    /// 프로젝트 인보이스 담당자 매출배분 리스트 조회.
    /// </summary>
    /// <param name="pii_seq"></param>
    /// <returns></returns>
    public async Task<List<ProjectInvoiceChargeModel>> SelectProjectInvoiceSalesListAsync(int pii_seq)
    {
      try
      {
        List<ProjectInvoiceChargeModel> list = new List<ProjectInvoiceChargeModel>();

        using (IDbConnection con = new SqlConnection(base.BaseConnectionString))
        {
          con.Open();

          string selectQuery = @" SELECT '' AS gubun
       ,U.name AS name
       ,U.uv_seq
       ,U.ud_seq
       ,PS.sales_rate
       ,PS.sales_money
       ,PS.comments
   FROM pjt_invoice_sales AS PS INNER JOIN uv_user AS U
                                        ON PS.uv_seq = U.uv_seq
  WHERE PS.pii_seq = @pii_seq ";

          DynamicParameters param = new DynamicParameters();
          param.Add("@pii_seq", pii_seq, DbType.Int32);

          var ret = await con.QueryAsync<ProjectInvoiceChargeModel>(selectQuery, param);

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
    /// 프로젝트 인보이스 담당자 조회
    /// </summary>
    /// <param name="p_seq"></param>
    /// <returns></returns>
    public async Task<List<ProjectInvoiceChargeModel>> SelectProjectInvoiceChargeOneAsync(int p_seq)
    {
      try
      {
        List<ProjectInvoiceChargeModel> list = new List<ProjectInvoiceChargeModel>();

        using (IDbConnection con = new SqlConnection(base.BaseConnectionString))
        {
          con.Open();

          string selectQuery = @" SELECT 'AM' AS comments
      ,U.uv_seq
      ,U.name
      ,U.ud_seq
      ,0 AS sales_rate
      ,0 AS sales_money
  FROM project AS P INNER JOIN pjt_director AS PD
                            ON P.p_seq = PD.p_seq
                    INNER JOIN uv_user AS U
                            ON PD.uv_seq = U.uv_seq
 WHERE P.p_seq = @p_seq
UNION ALL
SELECT 'SM' AS comments
      ,U.uv_seq
      ,U.name
      ,U.ud_seq
      ,0 AS sales_rate
      ,0 AS sales_money
  FROM project AS P 
              INNER JOIN pjt_manager AS PM
                            ON P.p_seq = PM.p_seq
                           AND PM.uv_seq NOT IN (SELECT uv_seq FROM pjt_director WHERE p_seq = P.p_seq)
                    INNER JOIN uv_user AS U
                            ON PM.uv_seq = U.uv_seq
WHERE P.p_seq = @p_seq ";

          DynamicParameters param = new DynamicParameters();
          param.Add("@p_seq", p_seq, DbType.Int32);

          var ret = await con.QueryAsync<ProjectInvoiceChargeModel>(selectQuery, param);

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
    /// 프로젝트 메모 목록
    /// </summary>
    /// <param name="p_seq"></param>
    /// <param name="skip"></param>
    /// <param name="count"></param>
    /// <param name="totalCount"></param>
    /// <returns></returns>
    public List<pjt_memo> SelectProjectMemoList(int p_seq, int skip, int count, out int totalCount)
    {
      try
      {
        List<pjt_memo> list = new List<pjt_memo>();

        using (IDbConnection con = new SqlConnection(base.BaseConnectionString))
        {
          con.Open();

          string selectQuery = @" SELECT U.name AS create_user_name
        ,M.*
   FROM pjt_memo AS M INNER JOIN uv_user AS U
                              ON M.create_user = U.uv_seq
 WHERE M.p_seq = @p_seq
 ORDER BY M.pm_seq DESC 
OFFSET @currentPage ROWS FETCH NEXT @pageSize ROWS ONLY ";

          string countQuery = @" SELECT COUNT(0)
   FROM pjt_memo AS M INNER JOIN uv_user AS U
                              ON M.create_user = U.uv_seq
 WHERE M.p_seq = @p_seq ";

          DynamicParameters param = new DynamicParameters();
          param.Add("@p_seq", p_seq, DbType.Int32);
          param.Add("@currentPage", skip, DbType.Int32);
          param.Add("@pageSize", count, DbType.Int32);

          DynamicParameters param2 = new DynamicParameters();
          param2.Add("@p_seq", p_seq, DbType.Int32);

          var ret = con.Query<pjt_memo>(selectQuery, param);
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
    /// 프로젝트 첨부파일 리스트
    /// </summary>
    /// <param name="p_seq"></param>
    /// <param name="skip"></param>
    /// <param name="count"></param>
    /// <param name="totalCount"></param>
    /// <returns></returns>
    public List<pjt_file> SelectProjectFileList(int p_seq, int skip, int count, out int totalCount)
    {
      try
      {
        List<pjt_file> list = new List<pjt_file>();

        using (IDbConnection con = new SqlConnection(base.BaseConnectionString))
        {
          con.Open();

          string selectQuery = @" SELECT U.name AS create_user_name
        ,F.*
   FROM pjt_file AS F INNER JOIN uv_user AS U
                              ON F.create_user = U.uv_seq
 WHERE F.p_seq = @p_seq
 AND F.remove_dt is null
 ORDER BY F.pf_seq DESC 
OFFSET @currentPage ROWS FETCH NEXT @pageSize ROWS ONLY ";

          string countQuery = @" SELECT COUNT(0)
   FROM pjt_file AS F INNER JOIN uv_user AS U
                              ON F.create_user = U.uv_seq

 WHERE F.p_seq = @p_seq 
AND F.remove_dt is null ";

          DynamicParameters param = new DynamicParameters();
          param.Add("@p_seq", p_seq, DbType.Int32);
          param.Add("@currentPage", skip, DbType.Int32);
          param.Add("@pageSize", count, DbType.Int32);

          DynamicParameters param2 = new DynamicParameters();
          param2.Add("@p_seq", p_seq, DbType.Int32);

          var ret = con.Query<pjt_file>(selectQuery, param);
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

    public string SelectProjectFileInfo(int pf_seq)
    {
      try
      {
        using (IDbConnection con = new SqlConnection(base.BaseConnectionString))
        {
          con.Open();

          string selectQuery = @" SELECT file_origin_path FROM pjt_file WHERE pf_seq = @pf_seq ";

          DynamicParameters param = new DynamicParameters();
          param.Add("@pf_seq", pf_seq, DbType.Int32);

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

    public string SelectProjectRecHisFileInfo(int prc_seq)
    {
      try
      {
        using (IDbConnection con = new SqlConnection(base.BaseConnectionString))
        {
          con.Open();

          string selectQuery = @" SELECT file_origin_path FROM pjt_recandidate_history WHERE prc_seq = @prc_seq ";

          DynamicParameters param = new DynamicParameters();
          param.Add("@prc_seq", prc_seq, DbType.Int32);

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

    public string SelectInOrderFileInfo(int if_seq)
    {
      try
      {
        using (IDbConnection con = new SqlConnection(base.BaseConnectionString))
        {
          con.Open();

          string selectQuery = @" SELECT file_origin_path FROM inorder_file WHERE if_seq = @if_seq ";

          DynamicParameters param = new DynamicParameters();
          param.Add("@if_seq", if_seq, DbType.Int32);

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

    /// <summary>
    /// 
    /// </summary>
    /// <param name="search"></param>
    /// <param name="uv_seq"></param>
    /// <param name="skip"></param>
    /// <param name="count"></param>
    /// <param name="totalCount"></param>
    /// <returns></returns>
    public List<pjt_share_board> SelectShareProjectBoardList(int p_seq, int skip, int count, out int totalCount)
    {
      try
      {
        List<pjt_share_board> list = new List<pjt_share_board>();

        using (IDbConnection con = new SqlConnection(base.BaseConnectionString))
        {
          con.Open();

          string selectQuery = @" SELECT PB.*
      ,U.name AS create_user_name
  FROM pjt_share_board AS PB INNER JOIN uv_user AS U
                                     ON PB.create_user = U.uv_seq 
 WHERE PB.p_seq = @p_seq 
 ORDER BY PB.psb_seq DESC 
OFFSET @currentPage ROWS FETCH NEXT @pageSize ROWS ONLY ";

          string countQuery = @" SELECT COUNT(0)
  FROM pjt_share_board AS PB INNER JOIN uv_user AS U
                                     ON PB.create_user = U.uv_seq 
 WHERE PB.p_seq = @p_seq ";

          DynamicParameters param = new DynamicParameters();
          param.Add("@p_seq", p_seq, DbType.String);
          param.Add("@currentPage", skip, DbType.Int32);
          param.Add("@pageSize", count, DbType.Int32);

          DynamicParameters param2 = new DynamicParameters();
          param2.Add("@p_seq", p_seq, DbType.String);

          var ret = con.Query<pjt_share_board>(selectQuery, param);
          totalCount = con.QueryFirstOrDefault<int>(countQuery, param2);

          list = ret.ToList();

          //댓글 리스트
          foreach (var data in list)
          {
            string selectQuery2 = @" SELECT PR.*
      ,U.name AS create_user_name
  FROM pjt_share_reply AS PR INNER JOIN uv_user AS U
                                     ON PR.create_user = U.uv_seq
 WHERE PR.p_seq = @p_seq
   AND PR.psb_seq = @psb_seq ";

            DynamicParameters param3 = new DynamicParameters();
            param3.Add("@p_seq", p_seq, DbType.String);
            param3.Add("@psb_seq", data.psb_seq, DbType.String);

            var result = con.Query<pjt_share_reply>(selectQuery2, param3);

            data.replyList = result.ToList();

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
    /// 학교 코드 조회.
    /// </summary>
    /// <param name="school_name"></param>
    /// <param name="gubun"></param>
    /// <param name="skip"></param>
    /// <param name="count"></param>
    /// <param name="totalCount"></param>
    /// <returns></returns>
    public List<code_school> SelectCodeSchoolList(string school_name, string gubun, int skip, int count, out int totalCount)
    {
      try
      {
        List<code_school> list = new List<code_school>();

        using (IDbConnection con = new SqlConnection(base.BaseConnectionString))
        {
          con.Open();

          string selectQuery = @" SELECT * 
                                              FROM code_school
                                             WHERE 1 = 1 ";
          if (!string.IsNullOrWhiteSpace(school_name))
            selectQuery += " AND school_name LIKE '%' + @school_name + '%' ";

          if (!string.IsNullOrWhiteSpace(gubun))
            selectQuery += " AND gubun LIKE '%' + @gubun + '%' ";

          selectQuery += @" ORDER BY school_name ASC OFFSET @currentPage ROWS FETCH NEXT @pageSize ROWS ONLY ";

          string countQuery = @" SELECT COUNT(0)
                                             FROM code_school
                                            WHERE 1 = 1 ";
          if (!string.IsNullOrWhiteSpace(school_name))
            countQuery += " AND school_name LIKE '%' + @school_name + '%' ";

          if (!string.IsNullOrWhiteSpace(gubun))
            countQuery += " AND gubun LIKE '%' + @gubun + '%' ";

          DynamicParameters param = new DynamicParameters();
          param.Add("@school_name", school_name, DbType.String);
          param.Add("@gubun", gubun, DbType.String);
          param.Add("@currentPage", skip, DbType.Int32);
          param.Add("@pageSize", count, DbType.Int32);

          DynamicParameters param2 = new DynamicParameters();
          param2.Add("@school_name", school_name, DbType.String);
          param2.Add("@gubun", gubun, DbType.String);

          var ret = con.Query<code_school>(selectQuery, param);
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
    /// 학교 검색 AutoComplete
    /// </summary>
    /// <param name="school_name"></param>
    /// <returns></returns>
    public async Task<List<code_school>> SelectCodeSchoolList(string school_name)
    {
      try
      {
        List<code_school> list = new List<code_school>();

        using (IDbConnection con = new SqlConnection(base.BaseConnectionString))
        {
          con.Open();

          string selectQuery = @" SELECT sc_seq, school_name, campus_name
                                              FROM code_school
                                             WHERE school_name LIKE '%' + @school_name + '%' 
                                             ORDER BY school_name ASC ";

          DynamicParameters param = new DynamicParameters();
          param.Add("@school_name", school_name, DbType.String);

          var ret = await con.QueryAsync<code_school>(selectQuery, param);

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
    /// 전공 검색 AutoComplete
    /// </summary>
    /// <param name="major_name"></param>
    /// <returns></returns>
    public async Task<List<code_major_2018>> SelectCodeMajorList(string major_name)
    {
      try
      {
        List<code_major_2018> list = new List<code_major_2018>();

        using (IDbConnection con = new SqlConnection(base.BaseConnectionString))
        {
          con.Open();

          string selectQuery = @" SELECT major_code, major_name
                                              FROM code_major_2018
                                             WHERE major_name LIKE '%' + @major_name + '%' 
                                             ORDER BY major_name ASC ";

          DynamicParameters param = new DynamicParameters();
          param.Add("@major_name", major_name, DbType.String);

          var ret = await con.QueryAsync<code_major_2018>(selectQuery, param);

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
    /// 업체 검색 AutoComplete
    /// </summary>
    /// <param name="companyName"></param>
    /// <returns></returns>
    public async Task<List<gov_api_company>> SelectApiCompanyList(string companyName)
    {
      try
      {
        List<gov_api_company> list = new List<gov_api_company>();

        using (IDbConnection con = new SqlConnection(base.BaseConnectionString))
        {
          con.Open();

          string selectQuery = @" SELECT G_SEQ, WKPL_NM
                                              FROM gov_api_company
                                             WHERE WKPL_NM_trim LIKE '%' + @companyName + '%' 
                                             ORDER BY WKPL_NM_trim ASC ";

          DynamicParameters param = new DynamicParameters();
          param.Add("@companyName", companyName, DbType.String);

          var ret = await con.QueryAsync<gov_api_company>(selectQuery, param);

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
    /// 워크시트 리스트
    /// </summary>
    /// <param name="search"></param>
    /// <param name="skip"></param>
    /// <param name="count"></param>
    /// <param name="totalCount"></param>
    /// <returns></returns>
    public List<WorkSheetModel> SelectWorkSheetList(WorkSheetSearchModel search, int skip, int count, out int totalCount)
    {
      try
      {
        List<WorkSheetModel> list = new List<WorkSheetModel>();

        using (IDbConnection con = new SqlConnection(base.BaseConnectionString))
        {
          con.Open();

          string selectQuery = @" SELECT P.p_seq
      ,C.c_seq AS client_seq
      ,C.kor_name AS client_name
      ,P.title
      ,P.pjt_status
      ,P.pjt_type
      ,P.create_dt
      ,P.modify_dt
      ,(SELECT DISTINCT 
               STUFF((SELECT DISTINCT
                             ',' + B.name
                        FROM pjt_director AS A INNER JOIN uv_user AS B
                                                       ON A.uv_seq = B.uv_seq
                       WHERE A.p_seq = PD.p_seq
                         FOR XML PATH('')), 1, 1, '') AS am_name
          FROM pjt_director AS PD
         WHERE PD.p_seq = P.p_seq) AS am_names
      ,(SELECT DISTINCT
               STUFF((SELECT DISTINCT
                             ',' + B.name
                        FROM pjt_manager AS A INNER JOIN uv_user AS B
                                                      ON A.uv_seq = B.uv_seq
                       WHERE A.p_seq = PM.p_seq
                         FOR XML PATH('')), 1, 1, '') AS searcher_name
          FROM pjt_manager AS PM
         WHERE PM.p_seq = P.p_seq) AS searcher_names
      ,(SELECT COUNT(distinct A.c_seq)
          FROM pjt_recandidate AS A INNER JOIN pjt_recandidate_history AS B 
                                            ON A.p_seq = B.p_seq
                                           AND A.pic_seq = B.pic_seq
                                    INNER JOIN candidate AS C
                                            ON B.c_seq = C.c_seq
         WHERE A.p_seq = P.p_seq
           AND B.state = 20) AS interestCnt
      ,(SELECT DISTINCT STUFF((SELECT ',' + C.kor_name 
                                 FROM pjt_recandidate AS A INNER JOIN pjt_recandidate_history AS B 
                                                                   ON A.p_seq = B.p_seq
                                                                  AND A.pic_seq = B.pic_seq
                                                           INNER JOIN candidate AS C
                                                                   ON B.c_seq = C.c_seq
                                WHERE A.p_seq = PR.p_seq
                                  AND B.state = 30
                                ORDER BY B.schedule_date DESC, A.c_seq
                        FOR XML PATH('')), 1, 1, '') AS recommandNames
          FROM pjt_recandidate AS PR
         WHERE PR.p_seq = P.p_seq) AS recommandNames
      ,(SELECT DISTINCT STUFF((SELECT ',' + CONVERT(VARCHAR(10), B.schedule_date, 121)
                                 FROM pjt_recandidate AS A INNER JOIN pjt_recandidate_history AS B 
                                                                   ON A.p_seq = B.p_seq
                                                                  AND A.pic_seq = B.pic_seq
                                                           INNER JOIN candidate AS C
                                                                   ON B.c_seq = C.c_seq
                                WHERE A.p_seq = PR.p_seq
                                  AND B.state = 30
                                ORDER BY B.schedule_date DESC, A.c_seq
                        FOR XML PATH('')), 1, 1, '') AS recommandDates
          FROM pjt_recandidate AS PR
         WHERE PR.p_seq = P.p_seq) AS recommandDates
      ,(SELECT COUNT(distinct A.c_seq)
          FROM pjt_recandidate AS A INNER JOIN pjt_recandidate_history AS B 
                                            ON A.p_seq = B.p_seq
                                           AND A.pic_seq = B.pic_seq
                                    INNER JOIN candidate AS C
                                            ON B.c_seq = C.c_seq
         WHERE A.p_seq = P.p_seq
           AND B.state = 30) AS recommandCnt
      ,(SELECT DISTINCT STUFF((SELECT ',' + C.kor_name 
                                 FROM pjt_recandidate AS A INNER JOIN pjt_recandidate_history AS B 
                                                                   ON A.p_seq = B.p_seq
                                                                  AND A.pic_seq = B.pic_seq
                                                           INNER JOIN candidate AS C
                                                                   ON B.c_seq = C.c_seq
                                WHERE A.p_seq = PR.p_seq
                                  AND B.state = 50
                                ORDER BY B.schedule_date DESC, A.c_seq
                        FOR XML PATH('')), 1, 1, '') AS interviewNames
          FROM pjt_recandidate AS PR
         WHERE PR.p_seq = P.p_seq) AS interviewNames
      ,(SELECT DISTINCT STUFF((SELECT ',' + CONVERT(VARCHAR(10), B.schedule_date, 121)
                                 FROM pjt_recandidate AS A INNER JOIN pjt_recandidate_history AS B 
                                                                   ON A.p_seq = B.p_seq
                                                                  AND A.pic_seq = B.pic_seq
                                                           INNER JOIN candidate AS C
                                                                   ON B.c_seq = C.c_seq
                                WHERE A.p_seq = PR.p_seq
                                  AND B.state = 50
                                ORDER BY B.schedule_date DESC, A.c_seq
                        FOR XML PATH('')), 1, 1, '') AS interviewDates
          FROM pjt_recandidate AS PR
         WHERE PR.p_seq = P.p_seq) AS interviewDates
      ,(SELECT COUNT(distinct A.c_seq)
          FROM pjt_recandidate AS A INNER JOIN pjt_recandidate_history AS B 
                                            ON A.p_seq = B.p_seq
                                           AND A.pic_seq = B.pic_seq
                                    INNER JOIN candidate AS C
                                            ON B.c_seq = C.c_seq
         WHERE A.p_seq = P.p_seq
           AND B.state = 50) AS interviewCnt
  FROM project AS P INNER JOIN client AS C
                            ON P.c_seq = C.c_seq
 WHERE P.pjt_status NOT IN (5) ";
          if (search.ud_seq > 0)
            selectQuery += " AND (P.p_seq IN (SELECT p_seq FROM pjt_director WHERE uv_seq IN (SELECT uv_seq FROM uv_user WHERE ud_seq = @ud_seq)) OR P.p_seq IN (SELECT p_seq FROM pjt_manager WHERE uv_seq IN (SELECT uv_seq FROM uv_user WHERE ud_seq = @ud_seq))) ";

          if (search.uv_seq > 0)
            selectQuery += " AND (P.p_seq IN(select p_seq FROM pjt_director WHERE uv_seq = @uv_seq) OR P.p_seq IN(select p_seq FROM pjt_manager WHERE uv_seq = @uv_seq)) ";

          if (!string.IsNullOrWhiteSpace(search.startDt) && !string.IsNullOrWhiteSpace(search.endDt))
            selectQuery += @" AND( P.create_dt BETWEEN CONVERT(DATETIME, @startDt + ' 00:00:00') AND CONVERT(DATETIME, @endDt + ' 23:59:59') 
                                                OR  P.modify_dt BETWEEN CONVERT(DATETIME, @startDt + ' 00:00:00') AND CONVERT(DATETIME, @endDt + ' 23:59:59')
                                                OR  P.pjt_status = 1) ";

          selectQuery += string.Format(" ORDER BY {0} {1} ", search.worksheet_orderTxt, search.worksheet_orderOption);
          selectQuery += "  OFFSET @currentPage ROWS FETCH NEXT @pageSize ROWS ONLY ";


          string countQuery = @" SELECT COUNT(0)
  FROM project AS P INNER JOIN client AS C
                            ON P.c_seq = C.c_seq
 WHERE P.pjt_status NOT IN (5) ";

          if (search.ud_seq > 0)
            countQuery += " AND (P.p_seq IN (SELECT p_seq FROM pjt_director WHERE uv_seq IN (SELECT uv_seq FROM uv_user WHERE ud_seq = @ud_seq)) OR P.p_seq IN (SELECT p_seq FROM pjt_manager WHERE uv_seq IN (SELECT uv_seq FROM uv_user WHERE ud_seq = @ud_seq))) ";

          if (search.uv_seq > 0)
            countQuery += " AND (P.p_seq IN(select p_seq FROM pjt_director WHERE uv_seq = @uv_seq) OR P.p_seq IN(select p_seq FROM pjt_manager WHERE uv_seq = @uv_seq)) ";

          if (!string.IsNullOrWhiteSpace(search.startDt) && !string.IsNullOrWhiteSpace(search.endDt))
            countQuery += @" AND( P.create_dt BETWEEN CONVERT(DATETIME, @startDt + ' 00:00:00') AND CONVERT(DATETIME, @endDt + ' 23:59:59') 
                                                OR  P.modify_dt BETWEEN CONVERT(DATETIME, @startDt + ' 00:00:00') AND CONVERT(DATETIME, @endDt + ' 23:59:59')
                                                OR  P.pjt_status = 1) ";


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

          var ret = con.Query<WorkSheetModel>(selectQuery, param);
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
    /// 
    /// </summary>
    /// <param name="search"></param>
    /// <param name="skip"></param>
    /// <param name="count"></param>
    /// <param name="totalCount"></param>
    /// <returns></returns>
    public List<WorkSheetInvoiceModel> SelectWorkSheetInvoiceStatisticsList(WorkSheetSearchModel search, int skip, int count, out int totalCount)
    {
      try
      {
        List<WorkSheetInvoiceModel> list = new List<WorkSheetInvoiceModel>();

        using (IDbConnection con = new SqlConnection(base.BaseConnectionString))
        {
          con.Open();

          string selectQuery = @" SELECT P.p_seq
      ,P.c_seq AS client_seq
      ,C.kor_name AS client_name
      ,P.title 
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
      ,PR.billing_dt
      ,PR.candidate_seq
      ,PR.candidate_name
      ,CP.p_name AS position_name
      ,PR.schedule_date
      ,PR.billing_money
  FROM project AS P INNER JOIN client AS C
                            ON P.c_seq = C.c_seq
                    INNER JOIN (SELECT PR.p_seq
                                      ,PR.c_seq AS candidate_seq
                                      ,C.kor_name AS candidate_name
                                      ,RH.state
                                      ,RH.schedule_date
                                      ,PI.billing_dt
                                      ,PI.billing_money
                                      ,DENSE_RANK() OVER(PARTITION BY PR.p_seq, PR.pic_seq ORDER BY RH.prc_seq DESC) AS d_rank
                                  FROM pjt_recandidate AS PR INNER JOIN pjt_recandidate_history AS RH
                                                                     ON PR.p_seq = RH.p_seq
                                                                    AND PR.pic_seq = RH.pic_seq
                                                             INNER JOIN candidate AS C
                                                                     ON PR.c_seq = C.c_seq
                                                             INNER JOIN pjt_invoice_info AS PI
                                                                     ON PR.p_seq = PI.p_seq
                                                                    AND PR.c_seq = PI.c_seq
                                 WHERE RH.state = 80
                                   AND PI.is_deleted <> 1) AS PR
                    ON P.p_seq = PR.p_seq
                    AND PR.d_rank = 1
               LEFT OUTER JOIN code_position AS CP
                            ON P.position_seq = CP.p_code
 WHERE P.pjt_status = 5 ";
          if (search.ud_seq > 0)
            selectQuery += " AND (P.p_seq IN (SELECT p_seq FROM pjt_director WHERE uv_seq IN (SELECT uv_seq FROM uv_user WHERE ud_seq = @ud_seq)) OR P.p_seq IN (SELECT p_seq FROM pjt_manager WHERE uv_seq IN (SELECT uv_seq FROM uv_user WHERE ud_seq = @ud_seq))) ";

          if (search.uv_seq > 0)
            selectQuery += " AND (P.p_seq IN(select p_seq FROM pjt_director WHERE uv_seq = @uv_seq) OR P.p_seq IN(select p_seq FROM pjt_manager WHERE uv_seq = @uv_seq)) ";

          if (!string.IsNullOrWhiteSpace(search.startDt) && !string.IsNullOrWhiteSpace(search.endDt))
            selectQuery += @" AND PR.billing_dt BETWEEN CONVERT(DATETIME, @startDt + ' 00:00:00') AND CONVERT(DATETIME, @endDt + ' 23:59:59') ";

          selectQuery += string.Format(" ORDER BY {0} {1} ", search.invoice_orderTxt, search.invoice_orderOption);
          selectQuery += " OFFSET @currentPage ROWS FETCH NEXT @pageSize ROWS ONLY ";


          string countQuery = @" SELECT COUNT(0)
  FROM project AS P INNER JOIN client AS C
                            ON P.c_seq = C.c_seq
                    INNER JOIN (SELECT PR.p_seq
                                      ,PR.c_seq AS candidate_seq
                                      ,C.kor_name AS candidate_name
                                      ,RH.state
                                      ,RH.schedule_date
                                      ,PI.billing_dt
                                      ,PI.billing_money
                                      ,DENSE_RANK() OVER(PARTITION BY PR.p_seq, PR.pic_seq ORDER BY RH.prc_seq DESC) AS d_rank
                                  FROM pjt_recandidate AS PR INNER JOIN pjt_recandidate_history AS RH
                                                                     ON PR.p_seq = RH.p_seq
                                                                    AND PR.pic_seq = RH.pic_seq
                                                             INNER JOIN candidate AS C
                                                                     ON PR.c_seq = C.c_seq
                                                             INNER JOIN pjt_invoice_info AS PI
                                                                     ON PR.p_seq = PI.p_seq
                                                                    AND PR.c_seq = PI.c_seq
                                 WHERE RH.state = 80
                                   AND PI.is_deleted <> 1) AS PR
                     ON P.p_seq = PR.p_seq
                     AND PR.d_rank = 1
               LEFT OUTER JOIN code_position AS CP
                            ON P.position_seq = CP.p_code
 WHERE P.pjt_status = 5 ";

          if (search.ud_seq > 0)
            countQuery += " AND (P.p_seq IN (SELECT p_seq FROM pjt_director WHERE uv_seq IN (SELECT uv_seq FROM uv_user WHERE ud_seq = @ud_seq)) OR P.p_seq IN (SELECT p_seq FROM pjt_manager WHERE uv_seq IN (SELECT uv_seq FROM uv_user WHERE ud_seq = @ud_seq))) ";

          if (search.uv_seq > 0)
            countQuery += " AND (P.p_seq IN(select p_seq FROM pjt_director WHERE uv_seq = @uv_seq) OR P.p_seq IN(select p_seq FROM pjt_manager WHERE uv_seq = @uv_seq)) ";

          if (!string.IsNullOrWhiteSpace(search.startDt) && !string.IsNullOrWhiteSpace(search.endDt))
            countQuery += @" AND PR.billing_dt BETWEEN CONVERT(DATETIME, @startDt + ' 00:00:00') AND CONVERT(DATETIME, @endDt + ' 23:59:59')  ";


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

          var ret = con.Query<WorkSheetInvoiceModel>(selectQuery, param);
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
    /// 프로젝트 단건 조회 - 메일에서 사용.
    /// </summary>
    /// <param name="p_seq"></param>
    /// <returns></returns>
    public async Task<project> SelectProjectWithMailOneAsync(int p_seq)
    {
      try
      {
        using (IDbConnection con = new SqlConnection(base.BaseConnectionString))
        {
          con.Open();

          string selectQuery = @" SELECT P.*
      ,C.kor_name AS company_name
  FROM project AS P INNER JOIN client AS C
                            ON P.c_seq = C.c_seq
  WHERE P.p_seq = @p_seq ";

          DynamicParameters param = new DynamicParameters();
          param.Add("@p_seq", p_seq, DbType.Int32);

          var ret = await con.QueryAsync<project>(selectQuery, param);

          con.Close();

          return ret.FirstOrDefault();
        }
      }
      catch (Exception e)
      {
        throw e;
      }
    }


    public async Task<List<WeeklyReportModel>> SelectWeeklySuccessReportList(WeeklyReportSearchModel search)
    {
      try
      {
        List<WeeklyReportModel> list = new List<WeeklyReportModel>();

        using (IDbConnection con = new SqlConnection(base.BaseConnectionString))
        {
          con.Open();


          string selectQuery = @" 
SELECT P.p_seq
      ,C.c_seq AS client_seq
      ,C.kor_name AS client_name
      ,P.title
      ,P.is_posting
      ,PD_N.ud_seq
      ,PD_N.ua_seq
      ,PD_N.name
      ,PD_N.retire_date as retire_date
      ,ISNULL(PR1.c_seq, 0) AS hire_candidate_seq
      ,ISNULL(PR1.kor_name, '') AS hire_candidate_name
      ,PR1.schedule_date AS hire_candidate_date
      ,ISNULL(PR1.pii_seq, 0) As is_invoice
      ,ISNULL((SELECT COUNT(*) FROM pjt_recandidate WHERE p_seq = P.p_seq), 0) as interestCnt
      ,ISNULL(PR3.recommandCnt, 0) AS recommandCnt
      ,ISNULL(PR4.afterInterviewCnt, 0) AS afterInterviewCnt
      ,ISNULL(PR5.negoCnt, 0) AS negoCnt
      ,P.pjt_status
      ,P.pjt_type
      ,P.open_dt as create_dt
      ,ISNULL(PR1.is_no_invoice, 0) as is_no_invoice
      ,CASE WHEN ISNULL(PR1.is_no_invoice, 0) > 0 THEN 0 
       ELSE 
           CASE fee_rate WHEN 0 THEN 
                           ISNULL(PR1.billing_won, 0) / 1000000
                         ELSE
                           (ISNULL(PR1.ann_income_won, P.exp_salary_won) * (fee_rate / 100)) / 1000000
                         END                  
       END as point
      ,STUFF((SELECT ', ' + B.NAME
                FROM pjt_director A INNER JOIN uv_user B
                                            ON A.uv_seq = B.uv_seq
			   WHERE A.p_seq = P.p_seq
                 FOR XML PATH('')),1,1,'') AS am_name
      ,STUFF((SELECT ', ' + B.NAME
                FROM pjt_manager A INNER JOIN uv_user B
                                           ON A.uv_seq = B.uv_seq
			   WHERE A.p_seq = P.p_seq
               AND   A.uv_seq NOT IN (SELECT uv_seq FROM pjt_director WHERE p_seq = A.p_seq)
                 FOR XML PATH('')),1,1,'') AS searcher_name
  FROM PROJECT As P INNER JOIN CLIENT As C
                  ON P.c_seq = C.c_seq
               LEFT JOIN PJT_DIRECTOR As PD
               ON P.p_seq = PD.p_seq
                  LEFT JOIN UV_USER PD_N
                  ON PD.uv_seq = PD_N.uv_seq
                  LEFT OUTER JOIN (SELECT PR.p_seq
                                              ,PH.state
                                              ,C.c_seq
                                              ,C.kor_name
                                              ,PH.annual_income
                                              ,PH.ann_income
                                              ,PH.ann_income_won
                                              ,PH.schedule_date
                                              ,ISNULL(inv.billing_dt, PH.schedule_date) as bill_dt
                                              ,PH.is_no_invoice
                                              ,inv.billing_money
                                              ,inv.billing_won
                                              ,inv.pii_seq
                                              ,inv.invoice_type
                                              ,inv.billing_dt
                                              ,DENSE_RANK() OVER(PARTITION BY PR.p_seq, PR.pic_seq ORDER BY PH.prc_seq DESC) AS rank
                                          FROM pjt_recandidate AS PR INNER JOIN pjt_recandidate_history AS PH
                                                                             ON PR.p_seq = PH.p_seq
                                                                            AND PR.pic_seq = PH.pic_seq
                                                                     LEFT JOIN pjt_invoice_info inv
                                                                             ON PH.prc_seq = inv.prc_Seq
                                                                            AND PH.p_seq   = inv.p_seq
                                                                            AND inv.invoice_type <> 1
                                                                            AND inv.is_deleted <> 1
                                                                     INNER JOIN candidate AS C
                                                                             ON PR.c_seq = C.c_seq) As PR1
                ON P.p_seq = PR1.p_seq            
                AND PR1.state = 80
                AND PR1.rank = 1
                LEFT OUTER JOIN (SELECT p_seq,
                                        COUNT(DISTINCT pic_seq) as recommandCnt
                                 FROM pjt_recandidate_history A
                                 WHERE A.state >= 30 
                                 GROUP BY p_seq) AS PR3
                            ON P.p_seq = PR3.p_seq
              LEFT OUTER JOIN (SELECT p_seq,
                                      COUNT(DISTINCT pic_seq) as afterInterviewCnt
                                 FROM pjt_recandidate_history A
                                 WHERE A.state >= 50 
                                 GROUP BY p_seq) AS PR4
                            ON P.p_seq = PR4.p_seq
               LEFT OUTER JOIN (SELECT p_seq,
                                      COUNT(DISTINCT pic_seq) as negoCnt
                                 FROM pjt_recandidate_history A
                                 WHERE A.state >= 70 
                                 GROUP BY p_seq) AS PR5
                            ON P.p_seq = PR5.p_seq


WHERE (
    P.close_dt BETWEEN CONVERT(DATETIME, @startDt + ' 00:00:00') AND CONVERT(DATETIME, @endDt + ' 23:59:59')
    OR P.open_dt BETWEEN CONVERT(DATETIME, @startDt + ' 00:00:00') AND CONVERT(DATETIME, @endDt + ' 23:59:59')
    OR PR1.schedule_date BETWEEN CONVERT(DATETIME, @startDt + ' 00:00:00') AND CONVERT(DATETIME, @endDt + ' 23:59:59')
    OR PR1.billing_dt BETWEEN CONVERT(DATETIME, @startDt + ' 00:00:00') AND CONVERT(DATETIME, @endDt + ' 23:59:59')
    OR P.pjt_status = 1
)
AND ((P.pjt_type > 1 AND P.pjt_status = 5)
     OR (PR1.is_no_invoice > 0)  
     OR (P.pjt_type = 1 AND  (PR1.billing_dt BETWEEN CONVERT(DATETIME, @startDt + ' 00:00:00') AND CONVERT(DATETIME, @endDt + ' 23:59:59')))
) ";
          if (search.ud_seq != 0)
          {
            selectQuery += @" 
AND (P.p_seq IN (SELECT p_seq FROM pjt_manager AS PD INNER JOIN uv_user AS U 
                                                      ON PD.uv_seq = U.uv_seq 
                                                      WHERE U.ud_seq = @ud_seq))
";
          }
          selectQuery += " ORDER BY P.pjt_status DESC, C.c_seq, PR1.billing_dt ASC ";



          DynamicParameters param = new DynamicParameters();
          param.Add("@ud_seq", search.ud_seq, DbType.Int32);
          param.Add("@startDt", search.startDt, DbType.String);
          param.Add("@endDt", search.endDt, DbType.String);

          var ret = await con.QueryAsync<WeeklyReportModel>(selectQuery, param);

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


    public async Task<List<WeeklyReportModel>> SelectWeeklySuccessReport2List(WeeklyReportSearchModel search)
    {
      try
      {
        List<WeeklyReportModel> list = new List<WeeklyReportModel>();

        using (IDbConnection con = new SqlConnection(base.BaseConnectionString))
        {
          con.Open();


          string selectQuery = @" 
SELECT 
	A.p_seq
	,A.client_seq
	,C.kor_name AS client_name
	,A.title
	,A.is_posting
	,PD_N.ud_seq
  ,PD_N.ua_seq
  ,PD_N.name
  ,PD_N.retire_date as retire_date
	,ISNULL(A.cand_seq, 0) AS hire_candidate_seq
  ,ISNULL(CN.kor_name, '') AS hire_candidate_name
  ,A.schedule_date AS hire_candidate_date
	,ISNULL((SELECT COUNT(*) FROM pjt_recandidate WHERE p_seq = A.p_seq), 0) as interestCnt
  ,ISNULL(PR3.recommandCnt, 0) AS recommandCnt
  ,ISNULL(PR4.afterInterviewCnt, 0) AS afterInterviewCnt
  ,ISNULL(PR5.negoCnt, 0) AS negoCnt
	,A.pjt_status
	,A.pjt_type
	,A.open_dt as create_dt
  ,A.billing_dt
	,ISNULL(A.is_no_invoice, 0) as is_no_invoice
	,ISNULL(A.billing_won, 0) / 1000000 as point
	,STUFF((SELECT ', ' + B1.NAME
          FROM pjt_director A1 INNER JOIN uv_user B1
                               ON A1.uv_seq = B1.uv_seq
          WHERE A1.p_seq = A.p_seq
   FOR XML PATH('')),1,1,'') AS am_name
  ,STUFF((SELECT ', ' + B1.NAME
          FROM pjt_manager A1 INNER JOIN uv_user B1
                              ON A1.uv_seq = B1.uv_seq
          WHERE A1.p_seq = A.p_seq
          AND   A1.uv_seq NOT IN (SELECT uv_seq FROM pjt_director WHERE p_seq = A1.p_seq)
   FOR XML PATH('')),1,1,'') AS searcher_name
	,A.invoice_type
FROM (
SELECT A.p_seq, P.c_seq as client_seq, PH.c_seq as cand_seq, P.title, ISNULL(PH.schedule_date, A.billing_dt) As schedule_date, A.billing_dt, A.invoice_type, A.billing_won, A.billing_money, A.bill_currency_cd,
       P.is_posting, P.pjt_status, P.pjt_type, P.open_dt, P.close_dt,
	     PH.is_no_invoice
FROM PJT_INVOICE_INFO A LEFT JOIN PROJECT P
                        ON A.p_seq= P.p_seq
						LEFT JOIN PJT_RECANDIDATE_HISTORY PH
						ON P.p_seq= PH.p_seq
						AND A.prc_seq= PH.prc_seq
WHERE A.billing_dt BETWEEN CONVERT(DATETIME, @startDt + ' 00:00:00') AND CONVERT(DATETIME, @endDt + ' 23:59:59')
AND A.is_deleted = 0
AND P.pjt_status IN (1, 4, 5)
UNION ALL 
SELECT P.p_seq, P.c_seq as client_seq, PR.c_seq as cand_seq, P.title,PH.schedule_date, null, null, 0, 0, '',
	     P.is_posting, P.pjt_status, P.pjt_type, P.open_dt, P.close_dt,
	     PH.is_no_invoice
FROM PROJECT P LEFT JOIN PJT_RECANDIDATE PR
			   ON P.p_seq = PR.p_seq
			   left join (select row_number() over(partition by pic_seq order by prc_seq desc) as l, * 
			              from pjt_recandidate_history) PH
		       on PR.pic_seq= PH.pic_seq
			   and PH.l = 1

WHERE PH.state IN (80)
AND PH.is_no_invoice = 1
AND P.pjt_status IN (1, 4, 5)
AND PH.schedule_date BETWEEN CONVERT(DATETIME, @startDt + ' 00:00:00') AND CONVERT(DATETIME, @endDt + ' 23:59:59')
) A LEFT JOIN client C
    ON A.client_seq= C.c_seq
	LEFT JOIN candidate cn
    ON A.cand_seq = CN.c_seq
	LEFT JOIN PJT_DIRECTOR As PD
    ON A.p_seq = PD.p_seq
    LEFT JOIN UV_USER PD_N
    ON PD.uv_seq = PD_N.uv_seq
	LEFT OUTER JOIN (SELECT p_seq,
							COUNT(DISTINCT pic_seq) as recommandCnt
						FROM pjt_recandidate_history A
						WHERE A.state >= 30 
						GROUP BY p_seq) AS PR3
				ON A.p_seq = PR3.p_seq
	LEFT OUTER JOIN (SELECT p_seq,
							COUNT(DISTINCT pic_seq) as afterInterviewCnt
						FROM pjt_recandidate_history A
						WHERE A.state >= 50 
						GROUP BY p_seq) AS PR4
				ON A.p_seq = PR4.p_seq
	LEFT OUTER JOIN (SELECT p_seq,
							COUNT(DISTINCT pic_seq) as negoCnt
						FROM pjt_recandidate_history A
						WHERE A.state >= 70 
						GROUP BY p_seq) AS PR5
				ON A.p_seq = PR5.p_seq ";
          if (search.ud_seq != 0)
          {
            selectQuery += @" 
WHERE (A.p_seq IN (SELECT p_seq FROM pjt_manager AS PD INNER JOIN uv_user AS U 
                                                      ON PD.uv_seq = U.uv_seq 
                                                      WHERE U.ud_seq = @ud_seq))
";
          }
          selectQuery += " ORDER BY A.client_seq, A.billing_dt ASC ";



          DynamicParameters param = new DynamicParameters();
          param.Add("@ud_seq", search.ud_seq, DbType.Int32);
          param.Add("@startDt", search.startDt, DbType.String);
          param.Add("@endDt", search.endDt, DbType.String);

          var ret = await con.QueryAsync<WeeklyReportModel>(selectQuery, param);

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

    public async Task<List<WeeklyReportModel>> SelectWeeklyCompleteReportList(WeeklyReportSearchModel search)
    {
      try
      {
        List<WeeklyReportModel> list = new List<WeeklyReportModel>();

        using (IDbConnection con = new SqlConnection(base.BaseConnectionString))
        {
          con.Open();


          string selectQuery = @" 
SELECT P.p_seq
      ,C.c_seq AS client_seq
      ,C.kor_name AS client_name
      ,P.title
      ,P.is_posting
      ,PD_N.ud_seq
      ,PD_N.ua_seq
      ,PD_N.name
      ,PD_N.retire_date as retire_date
      ,ISNULL(PR1.c_seq, 0) AS hire_candidate_seq
      ,ISNULL(PR1.kor_name, '') AS hire_candidate_name
      ,PR1.schedule_date AS hire_candidate_date
      ,ISNULL(PR1.pii_seq, 0) As is_invoice
      ,ISNULL((SELECT COUNT(*) FROM pjt_recandidate WHERE p_seq = P.p_seq), 0) as interestCnt
      ,ISNULL(PR3.recommandCnt, 0) AS recommandCnt
      ,ISNULL(PR4.afterInterviewCnt, 0) AS afterInterviewCnt
      ,ISNULL(PR5.negoCnt, 0) AS negoCnt
      ,P.pjt_status
      ,P.pjt_type
      ,P.open_dt as create_dt
      ,P.close_dt
      ,ISNULL(PR1.is_no_invoice, 0) as is_no_invoice
      ,CASE WHEN ISNULL(PR1.is_no_invoice, 0) > 0 THEN 0 
       ELSE 
           CASE fee_rate WHEN 0 THEN 
                           ISNULL(PR1.billing_won, 0) / 1000000
                         ELSE
                           (ISNULL(PR1.ann_income_won, P.exp_salary_won) * (fee_rate / 100)) / 1000000
                         END                  
       END as point
      ,STUFF((SELECT ', ' + B.NAME
                FROM pjt_director A INNER JOIN uv_user B
                                            ON A.uv_seq = B.uv_seq
			   WHERE A.p_seq = P.p_seq
                 FOR XML PATH('')),1,1,'') AS am_name
      ,STUFF((SELECT ', ' + B.NAME
                FROM pjt_manager A INNER JOIN uv_user B
                                           ON A.uv_seq = B.uv_seq
			   WHERE A.p_seq = P.p_seq
               AND   A.uv_seq NOT IN (SELECT uv_seq FROM pjt_director WHERE p_seq = A.p_seq)
                 FOR XML PATH('')),1,1,'') AS searcher_name
  FROM PROJECT As P INNER JOIN CLIENT As C
                  ON P.c_seq = C.c_seq
               LEFT JOIN PJT_DIRECTOR As PD
               ON P.p_seq = PD.p_seq
                  LEFT JOIN UV_USER PD_N
                  ON PD.uv_seq = PD_N.uv_seq
                  LEFT OUTER JOIN (SELECT PR.p_seq
                                              ,PH.state
                                              ,C.c_seq
                                              ,C.kor_name
                                              ,PH.annual_income
                                              ,PH.ann_income
                                              ,PH.ann_income_won
                                              ,PH.schedule_date
                                              ,ISNULL(inv.billing_dt, PH.schedule_date) as bill_dt
                                              ,PH.is_no_invoice
                                              ,inv.billing_money
                                              ,inv.billing_won
                                              ,inv.pii_seq
                                              ,inv.invoice_type
                                              ,inv.billing_dt
                                              ,PH.modify_dt
                                              ,DENSE_RANK() OVER(PARTITION BY PR.p_seq, PR.pic_seq ORDER BY PH.prc_seq DESC) AS rank
                                          FROM pjt_recandidate AS PR INNER JOIN pjt_recandidate_history AS PH
                                                                             ON PR.p_seq = PH.p_seq
                                                                            AND PR.pic_seq = PH.pic_seq
                                                                     LEFT JOIN pjt_invoice_info inv
                                                                             ON PH.prc_seq = inv.prc_Seq
                                                                            AND PH.p_seq   = inv.p_seq
                                                                            AND inv.invoice_type = 0
                                                                            AND inv.is_deleted <> 1
                                                                     INNER JOIN candidate AS C
                                                                             ON PR.c_seq = C.c_seq) As PR1
                ON P.p_seq = PR1.p_seq            
                AND PR1.state = 80
                AND PR1.rank = 1
                LEFT OUTER JOIN (SELECT p_seq,
                                        COUNT(DISTINCT pic_seq) as recommandCnt
                                 FROM pjt_recandidate_history A
                                 WHERE A.state >= 30 
                                 GROUP BY p_seq) AS PR3
                            ON P.p_seq = PR3.p_seq
              LEFT OUTER JOIN (SELECT p_seq,
                                      COUNT(DISTINCT pic_seq) as afterInterviewCnt
                                 FROM pjt_recandidate_history A
                                 WHERE A.state >= 50 
                                 GROUP BY p_seq) AS PR4
                            ON P.p_seq = PR4.p_seq
               LEFT OUTER JOIN (SELECT p_seq,
                                      COUNT(DISTINCT pic_seq) as negoCnt
                                 FROM pjt_recandidate_history A
                                 WHERE A.state >= 70 
                                 GROUP BY p_seq) AS PR5
                            ON P.p_seq = PR5.p_seq


WHERE (
    P.close_dt BETWEEN CONVERT(DATETIME, @startDt + ' 00:00:00') AND CONVERT(DATETIME, @endDt + ' 23:59:59')
    OR P.open_dt BETWEEN CONVERT(DATETIME, @startDt + ' 00:00:00') AND CONVERT(DATETIME, @endDt + ' 23:59:59')
    OR PR1.schedule_date BETWEEN CONVERT(DATETIME, @startDt + ' 00:00:00') AND CONVERT(DATETIME, @endDt + ' 23:59:59')
    OR PR1.billing_dt BETWEEN CONVERT(DATETIME, @startDt + ' 00:00:00') AND CONVERT(DATETIME, @endDt + ' 23:59:59')
    OR P.pjt_status = 1
)
AND ((P.pjt_type > 1 AND P.pjt_status = 4)
     OR (P.pjt_type = 1 AND PR1.pii_seq is null AND (PR1.modify_dt BETWEEN CONVERT(DATETIME, @startDt + ' 00:00:00') AND CONVERT(DATETIME, @endDt + ' 23:59:59')))
) ";
          if (search.ud_seq != 0)
          {
            selectQuery += @" 
AND (P.p_seq IN (SELECT p_seq FROM pjt_manager AS PD INNER JOIN uv_user AS U 
                                                      ON PD.uv_seq = U.uv_seq 
                                                      WHERE U.ud_seq = @ud_seq))
";
          }
          selectQuery += " ORDER BY P.pjt_status DESC, C.c_seq, PR1.modify_dt ASC ";



          DynamicParameters param = new DynamicParameters();
          param.Add("@ud_seq", search.ud_seq, DbType.Int32);
          param.Add("@startDt", search.startDt, DbType.String);
          param.Add("@endDt", search.endDt, DbType.String);

          var ret = await con.QueryAsync<WeeklyReportModel>(selectQuery, param);

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

    public async Task<List<WeeklyReportModel>> SelectWeeklyReportList(WeeklyReportSearchModel search)
    {
      try
      {
        List<WeeklyReportModel> list = new List<WeeklyReportModel>();

        using (IDbConnection con = new SqlConnection(base.BaseConnectionString))
        {
          con.Open();

          string selectQuery = @" SELECT P.p_seq
      ,C.c_seq AS client_seq
      ,C.kor_name AS client_name
      ,P.title
      ,P.is_posting
      ,PM.ud_seq
      ,PM.ua_seq
      ,PM.name as name
      ,PM.expertise as expertise
      ,PM.retire_date as retire_date
      ,ISNULL(PR1.hire_candidate_seq, 0) AS hire_candidate_seq
      ,ISNULL(PR1.hire_candidate_name, '') AS hire_candidate_name
      ,ISNULL(PR1.schedule_date, '') AS hire_candidate_date
      ,ISNULL((SELECT COUNT(*) FROM pjt_recandidate WHERE p_seq = P.p_seq), 0) as interestCnt
      ,ISNULL(PR3.recommandCnt, 0) AS recommandCnt
      ,ISNULL(PR4.afterInterviewCnt, 0) AS afterInterviewCnt
      ,ISNULL(PR5.negoCnt, 0) AS negoCnt
      ,P.pjt_status
      ,P.pjt_type
      ,P.create_dt
      ,P.modify_dt
      ,ISNULL(PR1.is_no_invoice, 0) as is_no_invoice
      ,CASE WHEN ISNULL(PR1.is_no_invoice, 0) > 0 THEN 0 
       ELSE 
           CASE fee_rate WHEN 0 THEN 
                           ISNULL(PR1.billing_won, 0) / 1000000
                         ELSE
                           (ISNULL(PR1.ann_income_won, P.exp_salary_won) * (fee_rate / 100)) / 1000000
                         END                  
       END as point
      ,STUFF((SELECT ', ' + B.NAME
                FROM pjt_director A INNER JOIN uv_user B
                                            ON A.uv_seq = B.uv_seq
			   WHERE A.p_seq = P.p_seq
                 FOR XML PATH('')),1,1,'') AS am_name
      ,STUFF((SELECT ', ' + B.NAME
                FROM pjt_manager A INNER JOIN uv_user B
                                           ON A.uv_seq = B.uv_seq
			   WHERE A.p_seq = P.p_seq
               AND   A.uv_seq NOT IN (SELECT uv_seq FROM pjt_director WHERE p_seq = A.p_seq)
                 FOR XML PATH('')),1,1,'') AS searcher_name
  FROM project AS P INNER JOIN client AS C
                            ON P.c_seq = C.c_seq
               LEFT OUTER JOIN (";
          if (search.ud_seq != 0)
          {

            selectQuery += @"
                                SELECT P.p_seq
                                      ,U.ud_seq
                                      ,U.name
                                      ,U.ua_seq
                                      ,U.retire_date
                                      ,U.expertise
                                FROM project AS P INNER JOIN pjt_manager AS PD
                                                  ON P.p_seq = PD.p_seq
                                                  INNER JOIN uv_user AS U
                                                  ON PD.uv_seq = U.uv_seq";
          }
          else
          {
            selectQuery += @"
                                SELECT P.p_seq
                                      ,U.ud_seq
                                      ,U.name
                                      ,U.ua_seq
                                      ,U.retire_date
                                      ,U.expertise
                                FROM project AS P INNER JOIN pjt_director AS PD
                                                  ON P.p_seq = PD.p_seq
                                                  INNER JOIN uv_user AS U
                                                  ON PD.uv_seq = U.uv_seq";


          }

          selectQuery += @") AS PM
               ON P.p_seq = PM.p_seq
               LEFT OUTER JOIN (SELECT A.p_seq
                                      ,A.state
                                      ,A.c_seq AS hire_candidate_seq
                                      ,A.kor_name AS hire_candidate_name
                                      ,A.schedule_date
                                      ,A.annual_income
                                      ,A.ann_income
                                      ,A.ann_income_won
                                      ,A.is_no_invoice
                                      ,A.billing_money
                                      ,A.billing_won
                                  FROM (SELECT PR.p_seq
                                              ,PH.state
                                              ,C.c_seq
                                              ,C.kor_name
                                              ,PH.annual_income
                                              ,PH.ann_income
                                              ,PH.ann_income_won
                                              ,PH.schedule_date
                                              ,PH.is_no_invoice
                                              ,inv.billing_money
                                              ,inv.billing_won
                                              ,DENSE_RANK() OVER(PARTITION BY PR.p_seq, PR.pic_seq ORDER BY PH.prc_seq DESC) AS rank
                                          FROM pjt_recandidate AS PR INNER JOIN pjt_recandidate_history AS PH
                                                                             ON PR.p_seq = PH.p_seq
                                                                            AND PR.pic_seq = PH.pic_seq
                                                                     LEFT JOIN pjt_invoice_info inv
                                                                             ON PH.prc_seq = inv.prc_Seq
                                                                            AND PH.p_seq   = inv.p_seq
                                                                            AND inv.is_deleted <> 1
                                                                     INNER JOIN candidate AS C
                                                                             ON PR.c_seq = C.c_seq) AS A
                                 WHERE A.rank = 1
                                   AND A.state = 80
                                   ) AS PR1
                            ON P.p_seq = PR1.p_seq
                LEFT OUTER JOIN (SELECT p_seq,
                                        COUNT(DISTINCT pic_seq) as recommandCnt
                                 FROM pjt_recandidate_history A
                                 WHERE A.state >= 30 
                                 GROUP BY p_seq) AS PR3
                            ON P.p_seq = PR3.p_seq
              LEFT OUTER JOIN (SELECT p_seq,
                                      COUNT(DISTINCT pic_seq) as afterInterviewCnt
                                 FROM pjt_recandidate_history A
                                 WHERE A.state >= 50 
                                 GROUP BY p_seq) AS PR4
                            ON P.p_seq = PR4.p_seq
               LEFT OUTER JOIN (SELECT p_seq,
                                      COUNT(DISTINCT pic_seq) as negoCnt
                                 FROM pjt_recandidate_history A
                                 WHERE A.state >= 70 
                                 GROUP BY p_seq) AS PR5
                            ON P.p_seq = PR5.p_seq
WHERE (
    P.modify_dt BETWEEN CONVERT(DATETIME, @startDt + ' 00:00:00') AND CONVERT(DATETIME, @endDt + ' 23:59:59')
    OR P.create_dt BETWEEN CONVERT(DATETIME, @startDt + ' 00:00:00') AND CONVERT(DATETIME, @endDt + ' 23:59:59') ";
          if (search.include_ended != 1)
          {
            selectQuery += "OR P.pjt_status = 1";
          }

          selectQuery += ")";
          if (search.ud_seq != 0)
          {
            if (search.include_ended != 1)
            {
              selectQuery += @" AND P.pjt_status IN (1, 4, 5) ";
            }
            selectQuery += @"   AND 
                                          (P.p_seq IN (SELECT p_seq FROM pjt_manager AS PD INNER JOIN uv_user AS U 
                                                                                          ON PD.uv_seq = U.uv_seq 
                                                      WHERE U.ud_seq = @ud_seq)
                                           OR
                                          P.p_seq IN (SELECT p_seq FROM pjt_director AS PD INNER JOIN uv_user AS U 
                                                                                          ON PD.uv_seq = U.uv_seq 
                                                      WHERE U.ud_seq = @ud_seq))
                                          ORDER BY PM.ua_seq ASC, PM.name ASC ";
          }
          else
          {
            selectQuery += @" AND P.pjt_status IN (4, 5)
                                          ORDER BY P.pjt_status DESC ";
          }


          DynamicParameters param = new DynamicParameters();
          param.Add("@ud_seq", search.ud_seq, DbType.Int32);
          param.Add("@startDt", search.startDt, DbType.String);
          param.Add("@endDt", search.endDt, DbType.String);

          var ret = await con.QueryAsync<WeeklyReportModel>(selectQuery, param);

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
