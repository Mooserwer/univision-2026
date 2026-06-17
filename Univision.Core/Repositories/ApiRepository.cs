using Dapper;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Univision.Core.Models.DTO;
using Univision.Core.Models.DTO.Remember;
using Univision.Core.Models.DTO.Response;
using Univision.Core.Models.DTO.Response.Api;


namespace Univision.Core.Repositories
{
  public class ApiRepository : BaseRepository
  {
    public ApiRepository()
    {

    }
    public ApiRepository(string strCon) : base(strCon)
    {

    }

    /// <summary>
    /// 회사 검색
    /// </summary>
    /// <param name="company_name"></param>
    /// <returns></returns>
    public async Task<List<gov_api_company>> SelectCompanyListAsync(string company_name)
    {
      try
      {
        List<gov_api_company> list = new List<gov_api_company>();

        using (IDbConnection con = new SqlConnection(base.BaseConnectionString))
        {
          con.Open();

          string selectQuery = @" SELECT g_seq
      ,WKPL_NM
      ,BZOWR_RGST_NO
      ,VLDT_VL_KRN_NM
  FROM gov_api_company
 WHERE WKPL_NM LIKE '%' +@company_name + '%'";

          DynamicParameters param = new DynamicParameters();
          param.Add("@company_name", company_name, DbType.String);

          var ret = await con.QueryAsync<gov_api_company>(selectQuery, param);

          list = ret.ToList();

          con.Close();
        }

        return list;
      }
      catch (Exception e)
      {
        throw e;
      }
    }



    public async Task<List<client>> CodeClientListAsync(string client_name, int c_seq)
    {
      try
      {
        List<client> list = new List<client>();

        //List<client> list = new List<client>();

        using (IDbConnection con = new SqlConnection(base.BaseConnectionString))
        {
          con.Open();

          string queryString = @"select c_seq, kor_name, eng_name from client ";
          queryString += " where(REPLACE(kor_name, ' ', '') like '%' + @client_name + '%' or REPLACE(eng_name, ' ', '') like '%' + @client_name + '%' or offlimit_keyword like '%' + @client_name + '%') ";


          if (c_seq > 0)
          {
            queryString += " and c_seq <> @c_seq ";
          }
          DynamicParameters parameters = new DynamicParameters();
          parameters.Add("@client_name", client_name, DbType.String);
          parameters.Add("@c_seq", c_seq, DbType.Int32);

          var ret = await con.QueryAsync<client>(queryString, parameters);

          list = ret.ToList();

          con.Close();
        }

        return list;
      }
      catch (Exception e)
      {
        throw e;
      }
    }

    public async Task<List<code_can_keyword>> CodeKeywordListAsync(string keyword, string old, int? c_seq)
    {
      try
      {
        List<code_can_keyword> list = new List<code_can_keyword>();

        //List<client> list = new List<client>();

        using (IDbConnection con = new SqlConnection(base.BaseConnectionString))
        {
          con.Open();

          string queryString = @"
SELECT distinct TOP 255 A.keyword_str as name
FROM code_can_keyword A 
WHERE 1 = 1 ";

          if (!String.IsNullOrEmpty(keyword))
            queryString += " AND A.keyword_str like '%'+@keyword+'%' ";
          else
            queryString += " AND A.keyword_str = '' ";

          if (!String.IsNullOrEmpty(old))
            queryString += " AND A.keyword_str not in (" + old + ") ";

          DynamicParameters parameters = new DynamicParameters();
          parameters.Add("@keyword", keyword, DbType.String);



          var ret = await con.QueryAsync<code_can_keyword>(queryString, parameters);

          list = ret.ToList();

          con.Close();
        }

        return list;
      }
      catch (Exception e)
      {
        throw e;
      }
    }

    public async Task<List<code_school>> CodeSchoolListAsync(string school_name)
    {
      try
      {
        List<code_school> list = new List<code_school>();

        using (IDbConnection con = new SqlConnection(base.BaseConnectionString))
        {
          con.Open();

          string selectQuery = @" SELECT * FROM code_school WHERE school_name LIKE '%' + @school_name + '%' ";

          DynamicParameters param = new DynamicParameters();
          param.Add("@school_name", school_name, DbType.String);

          var ret = await con.QueryAsync<code_school>(selectQuery, param);

          list = ret.ToList();

          con.Close();
        }

        return list;
      }
      catch (Exception e)
      {
        throw e;
      }
    }

    public async Task<int> SelectRememberCodeSchoolTotalCountAsync()
    {
      try
      {
        int total_count = 0;

        using (IDbConnection con = new SqlConnection(base.BaseConnectionString))
        {
          con.Open();

  

          string selectQuery = @" SELECT count(*) FROM code_school ";

          var ret = await con.QueryAsync<int>(selectQuery);

          total_count = ret.FirstOrDefault();

          con.Close();
        }

        return total_count;
      }
      catch (Exception e)
      {
        throw e;
      }
    }

    /// <summary>
    /// 리멤버 API 학교 코드 조회
    /// </summary>
    /// <param name="sc_seq">학교코드</param>
    /// <param name="from_row">첫행</param>
    /// <param name="page_size">페이지크기</param>
    /// <returns></returns>
    public async Task<List<r_code_school>> SelectRememberCodeSchoolListAsync(int sc_seq = 0, int from_row = 0, int page_size = 100)
    {
      try
      {
        List<r_code_school> list = new List<r_code_school>();

        using (IDbConnection con = new SqlConnection(base.BaseConnectionString))
        {
          con.Open();

          if (from_row < 0)
          {
            from_row = 0;
          }

            string selectQuery = @" SELECT sc_seq, school_name FROM code_school ";
          if (sc_seq != 0)
          {
            selectQuery = selectQuery + @" WHERE sc_seq = @sc_seq ";
          }
          selectQuery = selectQuery + @" ORDER BY sc_seq ";
          if (from_row >= 0)
          {
            selectQuery = selectQuery + @" OFFSET @currentPage ROWS FETCH NEXT @pageSize ROWS ONLY ";
          }
          DynamicParameters param = new DynamicParameters();
          param.Add("@sc_seq", sc_seq, DbType.Int64);
          param.Add("@currentPage", from_row, DbType.Int64);
          param.Add("@pageSize", page_size, DbType.Int64);

          var ret = await con.QueryAsync<r_code_school>(selectQuery, param);

          list = ret.ToList();

          con.Close();
        }

        return list;
      }
      catch (Exception e)
      {
        throw e;
      }
    }


    /// <summary>
    /// 학과 리스트 검색
    /// </summary>
    /// <param name="major_name"></param>
    /// <returns></returns>
    public async Task<List<ResponseMajorDto>> SelectMajorListAsync(string category1_name, string major_name)
    {
      try
      {
        List<ResponseMajorDto> list = new List<ResponseMajorDto>();

        using (IDbConnection con = new SqlConnection(base.BaseConnectionString))
        {
          con.Open();

          string selectQuery = @" SELECT major_code, major_name, category1_name FROM code_major_2018 
            WHERE 1 = 1 ";

          if (!string.IsNullOrWhiteSpace(category1_name))
            selectQuery += " AND category1_name = @category1_name ";

          selectQuery += " AND major_name LIKE '%' + @major_name + '%' ";
          selectQuery += " ORDER BY CASE WHEN charindex(@major_name, major_name) = 1 THEN 0 ELSE 1 END, major_name";

          DynamicParameters param = new DynamicParameters();
          param.Add("@category1_name", category1_name, DbType.String);
          param.Add("@major_name", major_name, DbType.String);

          var ret = await con.QueryAsync<ResponseMajorDto>(selectQuery, param);

          list = ret.ToList();

          con.Close();
        }

        return list;
      }
      catch (Exception e)
      {
        throw e;
      }
    }

    /// <summary>
    /// 주소 1
    /// </summary>
    /// <returns></returns>
    public async Task<List<addr1>> SelectAddr1ListAsync()
    {
      try
      {
        List<addr1> list = new List<addr1>();

        using (IDbConnection con = new SqlConnection(base.BaseConnectionString))
        {
          con.Open();

          string selectQuery = @" SELECT code1, area1 FROM addr1 ";

          var ret = await con.QueryAsync<addr1>(selectQuery);

          list = ret.ToList();

          con.Close();
        }

        return list;
      }
      catch (Exception e)
      {
        throw e;
      }
    }

    /// <summary>
    /// 주소2
    /// </summary>
    /// <returns></returns>
    public async Task<List<addr2>> SelectAddr2ListAsync(string code1)
    {
      try
      {
        List<addr2> list = new List<addr2>();

        using (IDbConnection con = new SqlConnection(base.BaseConnectionString))
        {
          con.Open();

          string selectQuery = @" SELECT code1, code2, area2 FROM addr2 Where code1 = @code1 ";

          DynamicParameters param = new DynamicParameters();
          param.Add("@code1", code1, DbType.String);

          var ret = await con.QueryAsync<addr2>(selectQuery, param);

          list = ret.ToList();

          con.Close();
        }

        return list;
      }
      catch (Exception e)
      {
        throw e;
      }
    }

    #region 산업API

    public async Task<List<JobBusiSuggModel>> SelectCanBusiCodeListAsync(List<string> curr_job, List<string> curr_busi, List<string> keywords) //, List<string> sugg_busi)
    {
      try
      {
        List<JobBusiSuggModel> list = new List<JobBusiSuggModel>();

        using (IDbConnection con = new SqlConnection(base.BaseConnectionString))
        {
          con.Open();
          string cjob = "''";
          string cbusi = "''";
          //string sjob = "''";
          //string sbusi = "''";
          string keyword = "''";

          if (curr_job.Count > 0)
            cjob = string.Join(",", curr_job.Select(x => "'" + x + "'"));
          if (curr_busi.Count > 0)
            cbusi = string.Join(",", curr_busi.Select(x => "'" + x + "'"));
          if (keywords.Count > 0)
            keyword = string.Join(",", keywords.Select(x => "'" + x + "'"));
          /*
           if (sugg_job.Count > 0)
            sjob = string.Join(",", sugg_job.Select(x => "'" + x + "'"));
          if (sugg_busi.Count > 0)
            sbusi = string.Join(",", sugg_busi.Select(x => "'" + x + "'"));
          */

          string selectQuery = @" 
SELECT 'B' AS type
      ,A.code1
      ,A.code_name1 as name1
      ,B.code2
      ,B.code_name2 as name2
  FROM code_business_mst AS A INNER JOIN code_business_dtl AS B
                                   ON A.code1 = B.code1
 WHERE B.code2 NOT IN(" + cbusi + @")
   AND B.code2 IN (SELECT code 
                    FROM code_keyword_mst X left join code_keyword_dtl Y
                                            on X.ck_seq = Y.ck_seq
                    WHERE Y.key_type = 1 
                    AND X.keyword_str IN(" + keyword + @"))

 UNION ALL
SELECT 'J' AS type
      ,A.code1
      ,A.code_name1 as name1
      ,B.code2
      ,B.code_name2 as name2
  FROM code_job_mst AS A INNER JOIN code_job_dtl AS B
                              ON A.code1 = B.code1
 WHERE B.code2 NOT IN(" + cjob + @")
   AND B.code2 IN (SELECT code 
                    FROM code_keyword_mst X left join code_keyword_dtl Y
                                            on X.ck_seq = Y.ck_seq
                    WHERE Y.key_type = 2 
                    AND X.keyword_str IN(" + keyword + @"))";


          DynamicParameters param = new DynamicParameters();

          var ret = await con.QueryAsync<JobBusiSuggModel>(selectQuery, param);

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
    /// 산업 코드 1단계 조회
    /// </summary>
    /// <returns></returns>
    public async Task<List<code_business_mst>> SelectBusinessCode1ListAsync()
    {
      try
      {
        List<code_business_mst> list = new List<code_business_mst>();

        using (IDbConnection con = new SqlConnection(base.BaseConnectionString))
        {
          con.Open();

          string selectQuery = @" SELECT * FROM code_business_mst ";

          var ret = await con.QueryAsync<code_business_mst>(selectQuery);

          list = ret.ToList();

          con.Close();
        }

        return list;
      }
      catch (Exception e)
      {
        throw e;
      }
    }

    /// <summary>
    /// 산업 코드 2단계 조회
    /// </summary>
    /// <param name="code1">1단계 코드</param>
    /// <returns></returns>
    public async Task<List<code_business_dtl>> SelectBusinessCode2ListAsync(float code1)
    {
      try
      {
        List<code_business_dtl> list = new List<code_business_dtl>();

        using (IDbConnection con = new SqlConnection(base.BaseConnectionString))
        {
          con.Open();

          string selectQuery = @" 
SELECT A.*, B.code_name1
FROM code_business_dtl A LEFT JOIN code_business_mst B
                         ON A.code1 = B.code1 
WHERE A.code1 = @code1 
ORDER BY A.order_no ASC";

          DynamicParameters param = new DynamicParameters();
          param.Add("@code1", code1, DbType.Double);

          var ret = await con.QueryAsync<code_business_dtl>(selectQuery, param);

          list = ret.ToList();

          con.Close();
        }

        return list;
      }
      catch (Exception e)
      {
        throw e;
      }
    }


    public async Task<List<code_business_dtl>> SelectBusinessCode2ListAsyncAll()
    {
      try
      {
        List<code_business_dtl> list = new List<code_business_dtl>();

        using (IDbConnection con = new SqlConnection(base.BaseConnectionString))
        {
          con.Open();

          string selectQuery = @" 
SELECT A.*, B.code_name1
FROM code_business_dtl A inner JOIN code_business_mst B
                         ON A.code1 = B.code1 
WHERE A.is_hide = 0
AND   B.is_hide = 0
ORDER BY B.order_no, A.order_no";

          var ret = await con.QueryAsync<code_business_dtl>(selectQuery);

          list = ret.ToList();

          con.Close();
        }

        return list;
      }
      catch (Exception e)
      {
        throw e;
      }
    }

    public async Task<List<code_job_dtl>> SelectJobCode2ListAsyncAll()
    {
      try
      {
        List<code_job_dtl> list = new List<code_job_dtl>();

        using (IDbConnection con = new SqlConnection(base.BaseConnectionString))
        {
          con.Open();

          string selectQuery = @" 
SELECT A.*, B.code_name1
FROM code_job_dtl A inner JOIN code_job_mst B
                         ON A.code1 = B.code1 
WHERE A.is_hide = 0
AND   B.is_hide = 0
ORDER BY B.order_no, A.order_no";

          var ret = await con.QueryAsync<code_job_dtl>(selectQuery);

          list = ret.ToList();

          con.Close();
        }

        return list;
      }
      catch (Exception e)
      {
        throw e;
      }
    }

    /// <summary>
    /// 리멤버 API용 산업 코드 조회
    /// </summary>
    /// <param name="code1">대분류 코드</param>
    /// <param name="code2">소분류 코드</param>
    /// <returns></returns>
    public async Task<List<r_code_business>> SelectRememberCodeBusinessListAsync(float code1 = 0, float code2 = 0)
    {
      try
      {
        List<r_code_business> list = new List<r_code_business>();

        using (IDbConnection con = new SqlConnection(base.BaseConnectionString))
        {
          con.Open();

          string selectQuery = @" 
SELECT A.*, B.code_name1
FROM code_business_dtl A LEFT JOIN code_business_mst B
                         ON A.code1 = B.code1 
WHERE A.code1 <> '107' ";
          if (code1 != 0)
          {
            selectQuery = selectQuery + @" AND A.code1 = @code1 ";
          }

          if (code1 != 0 && code2 != 0)
          {
            selectQuery = selectQuery + @" AND A.code2 = @code2 ";
          }

          selectQuery = selectQuery + @" ORDER BY A.order_no ASC";

          DynamicParameters param = new DynamicParameters();
          param.Add("@code1", code1, DbType.Double);
          param.Add("@code2", code2, DbType.Double);

          var ret = await con.QueryAsync<r_code_business>(selectQuery, param);

          list = ret.ToList();

          con.Close();
        }

        return list;
      }
      catch (Exception e)
      {
        throw e;
      }
    }

    /// <summary>
    /// 산업 코드 3단계 조회
    /// </summary>
    /// <param name="code1">1단계 코드</param>
    /// <param name="code2">2단계 코드</param>
    /// <returns></returns>
    public async Task<List<code_business3>> SelectBusinessCode3ListAsync(float code1, float code2)
    {
      try
      {
        List<code_business3> list = new List<code_business3>();

        using (IDbConnection con = new SqlConnection(base.BaseConnectionString))
        {
          con.Open();

          string selectQuery = @" SELECT * FROM code_business3 WHERE code1 = @code1 AND code2 = @code2 ";

          DynamicParameters param = new DynamicParameters();
          param.Add("@code1", code1, DbType.Double);
          param.Add("@code2", code2, DbType.Double);

          var ret = await con.QueryAsync<code_business3>(selectQuery, param);

          list = ret.ToList();

          con.Close();
        }

        return list;
      }
      catch (Exception e)
      {
        throw e;
      }
    }

    /// <summary>
    /// 산업 키워드 검색
    /// </summary>
    /// <param name="searchValue"></param>
    /// <returns></returns>
    public async Task<List<ResponseBusinessCodeDto>> SelectBusinessKeywordListAsync(string searchValue)
    {
      try
      {
        List<ResponseBusinessCodeDto> list = new List<ResponseBusinessCodeDto>();

        using (IDbConnection con = new SqlConnection(base.BaseConnectionString))
        {
          con.Open();

          string selectQuery = @" SELECT A.code1
      ,A.code_name1
      ,B.code2
      ,B.code_name2
  FROM code_business_mst AS A INNER JOIN code_business_dtl AS B
                                   ON A.code1 = B.code1
 WHERE A.code_name1 LIKE '%' +@value + '%'
    OR B.code_name2 LIKE '%' +@value + '%'
    OR (B.code2 IN (SELECT code 
                    FROM code_keyword_mst X left join code_keyword_dtl Y
                                            on X.ck_seq = Y.ck_seq
                    WHERE Y.key_type = 1 
                    AND X.keyword_str like '%' +@value + '%'))
order by case when charindex(@value, B.code_name2, 1) >= 1 then 0 else 1 end, 
case when charindex(@value, A.code_name1, 1) >= 1 then 0 else 1 end, B.code2
";

          DynamicParameters param = new DynamicParameters();
          param.Add("@value", searchValue, DbType.String);

          var ret = await con.QueryAsync<ResponseBusinessCodeDto>(selectQuery, param);

          list = ret.ToList();

          con.Close();
        }

        return list;
      }
      catch (Exception e)
      {
        throw e;
      }
    }

    /// <summary>
    /// 산업 키워드 검색 2단계까지만 검색
    /// </summary>
    /// <param name="searchValue"></param>
    /// <returns></returns>
    public async Task<List<ResponseBusinessCodeDto>> SelectBusinessKeywordListAsync2(string searchValue)
    {
      try
      {
        List<ResponseBusinessCodeDto> list = new List<ResponseBusinessCodeDto>();

        using (IDbConnection con = new SqlConnection(base.BaseConnectionString))
        {
          con.Open();

          string selectQuery = @" SELECT A.code1
      ,A.code_name1
      ,B.code2
      ,B.code_name2
  FROM code_business_mst AS A INNER JOIN code_business_dtl AS B
                                   ON A.code1 = B.code1
 WHERE A.code_name1 LIKE '%' +@value + '%'
    OR B.code_name2 LIKE '%' +@value + '%' 
    OR (B.code2 IN (SELECT code 
                    FROM code_keyword_mst X left join code_keyword_dtl Y
                                            on X.ck_seq = Y.ck_seq
                    WHERE Y.key_type = 1 
                    AND X.keyword_str like '%' +@value + '%'))
order by case when charindex(@value, B.code_name2, 1) >= 1 then 0 else 1 end, 
case when charindex(@value, A.code_name1, 1) >= 1 then 0 else 1 end, B.code2
";

          DynamicParameters param = new DynamicParameters();
          param.Add("@value", searchValue, DbType.String);

          var ret = await con.QueryAsync<ResponseBusinessCodeDto>(selectQuery, param);

          list = ret.ToList();

          con.Close();
        }

        return list;
      }
      catch (Exception e)
      {
        throw e;
      }
    }

    #endregion

    #region 직무 API

    /// <summary>
    /// 직무 코드 1단계 조회
    /// </summary>
    /// <returns></returns>
    public async Task<List<code_job_mst>> SelectJobCode1ListAsync()
    {
      try
      {
        List<code_job_mst> list = new List<code_job_mst>();

        using (IDbConnection con = new SqlConnection(base.BaseConnectionString))
        {
          con.Open();

          string selectQuery = @" SELECT * FROM code_job_mst ";

          var ret = await con.QueryAsync<code_job_mst>(selectQuery);

          list = ret.ToList();

          con.Close();
        }

        return list;
      }
      catch (Exception e)
      {
        throw e;
      }
    }

    /// <summary>
    /// 직무 코드 2단계 조회
    /// </summary>
    /// <param name="code1">1단계 코드</param>
    /// <returns></returns>
    public async Task<List<code_job_dtl>> SelectJobCode2ListAsync(float code1)
    {
      try
      {
        List<code_job_dtl> list = new List<code_job_dtl>();

        using (IDbConnection con = new SqlConnection(base.BaseConnectionString))
        {
          con.Open();

          string selectQuery = @" 
SELECT A.*, B.code_name1
FROM code_job_dtl A LEFT JOIN code_job_mst B
                         ON A.code1 = B.code1 
WHERE A.code1 = @code1 
ORDER BY A.order_no ASC ";

          DynamicParameters param = new DynamicParameters();
          param.Add("@code1", code1, DbType.Double);

          var ret = await con.QueryAsync<code_job_dtl>(selectQuery, param);

          list = ret.ToList();

          con.Close();
        }

        return list;
      }
      catch (Exception e)
      {
        throw e;
      }
    }

    /// <summary>
    /// 직무 코드 3단계 조회
    /// </summary>
    /// <param name="code1">1단계 코드</param>
    /// <param name="code2">2단계 코드</param>
    /// <returns></returns>
    public async Task<List<code_job3>> SelectJobCode3ListAsync(float code1, float code2)
    {
      try
      {
        List<code_job3> list = new List<code_job3>();

        using (IDbConnection con = new SqlConnection(base.BaseConnectionString))
        {
          con.Open();

          string selectQuery = @" SELECT * FROM code_job3 WHERE code1 = @code1 AND code2 = @code2 ";

          DynamicParameters param = new DynamicParameters();
          param.Add("@code1", code1, DbType.Double);
          param.Add("@code2", code2, DbType.Double);

          var ret = await con.QueryAsync<code_job3>(selectQuery, param);

          list = ret.ToList();

          con.Close();
        }

        return list;
      }
      catch (Exception e)
      {
        throw e;
      }
    }

    /// <summary>
    /// 리멤버 API 직무 코드 조회
    /// </summary>
    /// <param name="code1">대분류 코드</param>
    /// <param name="code2">소분류 코드</param>
    /// <returns></returns>
    public async Task<List<r_code_job>> SelectRememberCodeJobListAsync(float code1 = 0, float code2 = 0)
    {
      try
      {
        List<r_code_job> list = new List<r_code_job>();

        using (IDbConnection con = new SqlConnection(base.BaseConnectionString))
        {
          con.Open();

          string selectQuery = @" 
SELECT A.*, B.code_name1
FROM code_job_dtl A LEFT JOIN code_job_mst B
                         ON A.code1 = B.code1 
WHERE 1 = 1";
          if (code1 != 0)
          {
            selectQuery = selectQuery + @" AND A.code1 = @code1 ";
          }

          if (code1 != 0 && code2 != 0)
          {
            selectQuery = selectQuery + @" AND A.code2 = @code2 ";
          }

          selectQuery = selectQuery + @" ORDER BY A.order_no ASC ";

          DynamicParameters param = new DynamicParameters();
          param.Add("@code1", code1, DbType.Double);
          param.Add("@code2", code2, DbType.Double);

          var ret = await con.QueryAsync<r_code_job>(selectQuery, param);

          list = ret.ToList();

          con.Close();
        }

        return list;
      }
      catch (Exception e)
      {
        throw e;
      }
    }

    /// <summary>
    /// 직무 키워드 검색
    /// </summary>
    /// <param name="searchValue"></param>
    /// <returns></returns>
    public async Task<List<ResponseJobCodeDto>> SelectJobKeywordListAsync(string searchValue)
    {
      try
      {
        List<ResponseJobCodeDto> list = new List<ResponseJobCodeDto>();

        using (IDbConnection con = new SqlConnection(base.BaseConnectionString))
        {
          con.Open();

          string selectQuery = @" SELECT A.code1
      ,A.code_name1
      ,B.code2
      ,B.code_name2
  FROM code_job_mst AS A INNER JOIN code_job_dtl AS B
                              ON A.code1 = B.code1
                      
 WHERE A.code_name1 LIKE '%' +@value + '%'
    OR B.code_name2 LIKE '%' +@value + '%'
    OR (B.code2 IN (SELECT code 
                    FROM code_keyword_mst X left join code_keyword_dtl Y
                                            on X.ck_seq = Y.ck_seq
                    WHERE Y.key_type = 2 
                    AND X.keyword_str like '%' +@value + '%'))
order by case when charindex(@value, B.code_name2, 1) >= 1 then 0 else 1 end, 
case when charindex(@value, A.code_name1, 1) >= 1 then 0 else 1 end, B.code2
";

          DynamicParameters param = new DynamicParameters();
          param.Add("@value", searchValue, DbType.String);

          var ret = await con.QueryAsync<ResponseJobCodeDto>(selectQuery, param);

          list = ret.ToList();

          con.Close();
        }

        return list;
      }
      catch (Exception e)
      {
        throw e;
      }
    }

    /// <summary>
    /// 직무 2단계 키워드 검색
    /// </summary>
    /// <param name="searchValue"></param>
    /// <returns></returns>
    public async Task<List<ResponseJobCodeDto>> SelectJobKeywordListAsync2(string searchValue)
    {
      try
      {
        List<ResponseJobCodeDto> list = new List<ResponseJobCodeDto>();

        using (IDbConnection con = new SqlConnection(base.BaseConnectionString))
        {
          con.Open();

          string selectQuery = @" SELECT A.code1
      ,A.code_name1
      ,B.code2
      ,B.code_name2
  FROM code_job_mst AS A INNER JOIN code_job_dtl AS B
                              ON A.code1 = B.code1
 WHERE A.code_name1 LIKE '%' +@value + '%'
    OR B.code_name2 LIKE '%' +@value + '%' 
    OR (B.code2 IN (SELECT code 
                    FROM code_keyword_mst X left join code_keyword_dtl Y
                                            on X.ck_seq = Y.ck_seq
                    WHERE Y.key_type = 2 
                    AND X.keyword_str like '%' +@value + '%'))
order by case when charindex(@value, B.code_name2, 1) >= 1 then 0 else 1 end, 
case when charindex(@value, A.code_name1, 1) >= 1 then 0 else 1 end, B.code2
";

          DynamicParameters param = new DynamicParameters();
          param.Add("@value", searchValue, DbType.String);

          var ret = await con.QueryAsync<ResponseJobCodeDto>(selectQuery, param);

          list = ret.ToList();

          con.Close();
        }

        return list;
      }
      catch (Exception e)
      {
        throw e;
      }
    }

    #endregion

    /// <summary>
    /// 직무, 산업 키워드 검색
    /// </summary>
    /// <param name="searchValue"></param>
    /// <returns></returns>
    public async Task<List<ResponseJobBusinessCodeDto>> SelectJobBusinessKeywordListAsync(string searchValue)
    {
      try
      {
        List<ResponseJobBusinessCodeDto> list = new List<ResponseJobBusinessCodeDto>();

        using (IDbConnection con = new SqlConnection(base.BaseConnectionString))
        {
          con.Open();

          string selectQuery = @" SELECT 'job' AS type
      ,A.code1
      ,A.code_name1
      ,B.code2
      ,B.code_name2
  FROM code_job_mst AS A INNER JOIN code_job_dtl AS B
                              ON A.code1 = B.code1
 WHERE A.code_name1 LIKE '%' +@value + '%'
    OR B.code_name2 LIKE '%' +@value + '%'
    OR (B.code2 IN (SELECT code 
                    FROM code_keyword_mst X left join code_keyword_dtl Y
                                            on X.ck_seq = Y.ck_seq
                    WHERE Y.key_type = 2 
                    AND X.keyword_str like '%' +@value + '%'))
 UNION ALL
SELECT 'busi' AS type
      ,A.code1
      ,A.code_name1
      ,B.code2
      ,B.code_name2
  FROM code_business_mst AS A INNER JOIN code_business_dtl AS B
                                   ON A.code1 = B.code1
 WHERE A.code_name1 LIKE '%' +@value + '%'
    OR B.code_name2 LIKE '%' +@value + '%'
    OR (B.code2 IN (SELECT code 
                    FROM code_keyword_mst X left join code_keyword_dtl Y
                                            on X.ck_seq = Y.ck_seq
                    WHERE Y.key_type = 1 
                    AND X.keyword_str like '%' +@value + '%'))
";

          DynamicParameters param = new DynamicParameters();
          param.Add("@value", searchValue, DbType.String);

          var ret = await con.QueryAsync<ResponseJobBusinessCodeDto>(selectQuery, param);

          list = ret.ToList();

          con.Close();
        }

        return list;
      }
      catch (Exception e)
      {
        throw e;
      }
    }

    #region 직급/직책

    /// <summary>
    /// 직급 목록 조회
    /// </summary>
    /// <returns></returns>
    public async Task<List<code_rank>> SelectRankCodeListAsync()
    {
      try
      {
        List<code_rank> list = new List<code_rank>();

        using (IDbConnection con = new SqlConnection(base.BaseConnectionString))
        {
          con.Open();

          string selectQuery = @" SELECT * FROM code_rank ";

          var ret = await con.QueryAsync<code_rank>(selectQuery);

          list = ret.ToList();

          con.Close();
        }

        return list;
      }
      catch (Exception e)
      {
        throw e;
      }
    }

    /// <summary>
    /// 직책 목록 조회
    /// </summary>
    /// <returns></returns>
    public async Task<List<code_position>> SelectPositionCodeListAsync()
    {
      try
      {
        List<code_position> list = new List<code_position>();

        using (IDbConnection con = new SqlConnection(base.BaseConnectionString))
        {
          con.Open();

          string selectQuery = @" SELECT * FROM code_position ";

          var ret = await con.QueryAsync<code_position>(selectQuery);

          list = ret.ToList();

          con.Close();
        }

        return list;
      }
      catch (Exception e)
      {
        throw e;
      }
    }

    #endregion

    #region 알림 뱃지

    /// <summary>
    /// 알림 뱃지
    /// </summary>
    /// <param name="uv_seq">회원 seq</param>
    /// <returns></returns>
    public async Task<List<ResponseAlramModel>> SelectAlramBadgeListAsync(int uv_seq)
    {
      try
      {
        List<ResponseAlramModel> list = new List<ResponseAlramModel>();

        using (IDbConnection con = new SqlConnection(base.BaseConnectionString))
        {
          con.Open();

          string selectQuery = @" SELECT AM.am_seq
      ,AM.message
      ,AM.href_url
      ,AM.create_dt
      ,AU.uv_seq
      ,AU.is_read
      ,AU.read_date
  FROM alarm_message AS AM INNER JOIN alarm_user AS AU
                                   ON AM.am_seq = AU.am_seq
 WHERE AU.uv_seq = @uv_seq
   AND AU.is_read = 0 
 ORDER BY AM.create_dt DESC ";

          DynamicParameters param = new DynamicParameters();
          param.Add("@uv_seq", uv_seq, DbType.Int32);

          var ret = await con.QueryAsync<ResponseAlramModel>(selectQuery, param);

          list = ret.ToList();

          con.Close();
        }

        return list;
      }
      catch (Exception e)
      {
        throw e;
      }
    }

    public async Task<List<receipt>> SelectReceiptCountAsync(int uv_seq)
    {
      try
      {
        //int count = 0;
        List<receipt> list = new List<receipt>();
        using (IDbConnection con = new SqlConnection(base.BaseConnectionString))
        {
          con.Open();

          string selectQuery = @" 
 SELECT DISTINCT A.*
        , CASE A.r_type WHEN 1 THEN '통신비' WHEN 2 THEN '법인카드' WHEN 3 THEN 'LinkedIn' END As r_type_name
 FROM receipt AS A LEFT JOIN receipt_user AS B
                    ON A.r_seq = B.r_seq
                    LEFT JOIN receipt_user_dtl AS C
                    ON A.r_seq = C.r_seq
                    AND B.ra_seq = C.ra_seq
 WHERE A.is_open = 1
 AND   B.submit_date is null
 AND   GETDATE() BETWEEN A.r_start AND A.r_end
 AND   B.uv_seq =  @uv_seq ";

          DynamicParameters param = new DynamicParameters();
          param.Add("@uv_seq", uv_seq, DbType.Int32);

          var ret = await con.QueryAsync<receipt>(selectQuery, param);

          //count = ret.Count();
          list = ret.ToList();
          con.Close();
        }

        return list;
      }
      catch (Exception e)
      {
        throw e;
      }
    }

    public async Task<int> SelectAttendCountAsync(int uv_seq, string dt)
    {
      try
      {
        int count = 0;
        using (IDbConnection con = new SqlConnection(base.BaseConnectionString))
        {
          con.Open();

          string selectQuery = @" 
 SELECT COUNT(*) as Cnt
 FROM uv_user_attend
 WHERE uv_seq = @uv_seq
 AND   attend_date > @dt ";

          DynamicParameters param = new DynamicParameters();
          param.Add("@uv_seq", uv_seq, DbType.Int32);
          param.Add("@dt", dt, DbType.String);

          var ret = await con.QueryAsync<int>(selectQuery, param);

          //count = ret.Count();
          count = ret.FirstOrDefault();
          con.Close();
        }

        return count;
      }
      catch (Exception e)
      {
        throw e;
      }
    }

    #endregion

    /// <summary>
    /// 국가 검색
    /// </summary>
    /// <param name="country_name"></param>
    /// <returns></returns>
    public async Task<List<code_nationality>> SelectCountryNameListAsync(string country_name)
    {
      try
      {
        List<code_nationality> list = new List<code_nationality>();

        using (IDbConnection con = new SqlConnection(base.BaseConnectionString))
        {
          con.Open();

          string selectQuery = @" SELECT Nationality_Code
      ,En_Country_Name
      ,Kr_Country_Name
  FROM code_nationality
 WHERE (En_Country_Name LIKE '%' +@country_name + '%') or (Kr_Country_Name LIKE '%' +@country_name + '%')";

          DynamicParameters param = new DynamicParameters();
          param.Add("@country_name", country_name, DbType.String);

          var ret = await con.QueryAsync<code_nationality>(selectQuery, param);

          list = ret.ToList();

          con.Close();
        }

        return list;
      }
      catch (Exception e)
      {
        throw e;
      }
    }
  }
}
