using Dapper;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Univision.Core.Models;
using Univision.Core.Models.DTO;
using Univision.Core.Models.DTO.Remember;
using Univision.Core.Models.DTO.Request.Candidate;
using Univision.Core.Models.DTO.Response.Candidate;
using Univision.Core.Models.DTO.Response.Project;

namespace Univision.Core.Repositories
{
  public class SearchEngineRepository : BaseRepository
  {
    static string serverAddr = "220.117.204.171";
    static int serverPort = 7577;
    public SearchEngineRepository()
    {

    }
    public SearchEngineRepository(string strCon) : base(strCon)
    {

    }

    [DllImport("kmpdl.dll")]
    private static extern int CreateHandle();
    [DllImport("kmpdl.dll")]
    private static extern int DestroyHandle(int handle_id);
    [DllImport("kmpdl.dll")]
    private static extern IntPtr CallModuleAPI(int handle_id, String ip_addr, int port, IntPtr inJsonString);
    //private static extern IntPtr CallModuleAPI(int handle_id, String ip_addr, int port, String inJsonString);
    [DllImport("kmpdl.dll")]
    private static extern IntPtr GetErrorMessage(int handle_id);
    /// <summary>
    /// 문서필터를 통하여 파일 내용 추출
    /// </summary>
    /// <param name="file_path"></param>
    /// <returns></returns>
    public string TempResumeCheck(string file_path)
    {
      try
      {
        string result_str = String.Empty;
        int handle_id = CreateHandle();

        String inJson = "{'function':'filter', 'type':'auto', 'mode':'F2B', 'source':'" + file_path + "'}";

        IntPtr strptr = CallModuleAPI(handle_id, serverAddr, 7574, NativeUtf8FromString(inJson));

        String outJson = String.Empty;

        outJson = StringFromNativeUtf8(strptr);
        //System.Console.WriteLine("result: " + outJson);
        //outJson = Encoding.UTF8.GetString(rb);
        DestroyHandle(handle_id);


        JObject jObject = JObject.Parse(outJson);

        if (jObject["rc"].ToString() != "0")
        {
          throw new Exception("[필터 오류 : (rc : " + jObject["rc"].ToString() + ")");
        }

        if (!String.IsNullOrEmpty(jObject["content"].ToString()))
        {
          result_str = jObject["content"].ToString();
        }

        return result_str;
      }
      catch (Exception e)
      {
        throw e;
      }
    }

    static string getBooleanSearchParse(string key, string value, bool syno = false)
    {
      //string text = "안녕 안녕&& (사람 || 두두)";
      string pattern = @"(\&\&)|(\|\|)|(\!\!)|(\()|(\))";
      string prev = String.Empty;
      List<string> tokens = new List<string>();
      StringBuilder sb = new StringBuilder();

      foreach (string result in Regex.Split(value, pattern))
      {
        string rst = result.Trim();
        switch (rst)
        {
          case "(":
          case ")":
            sb.Append(rst);
            break;
          case "&&":
          case "!!":
            if (sb.Length > 0)
              sb.Append(" AND ");
            break;
          case "||":
            if (sb.Length > 0)
              sb.Append(" OR ");
            break;

          default:
            if (!String.IsNullOrWhiteSpace(rst))
            {
              string query = key + (prev == "!!" ? " != " : " = ") + "'" + rst + "' " + (syno ? " allorderadjacent synonym " : "");
              sb.Append(query);
            }
            break;
        }

        prev = rst;
      }

      return sb.ToString();
    }



    public IntPtr NativeUtf8FromString(string managedString)
    {
      byte[] buffer = Encoding.UTF8.GetBytes(managedString); // not null terminated
      Array.Resize(ref buffer, buffer.Length + 1);
      buffer[buffer.Length - 1] = 0; // terminating 0
      IntPtr nativeUtf8 = Marshal.AllocHGlobal(buffer.Length);
      Marshal.Copy(buffer, 0, nativeUtf8, buffer.Length);
      return nativeUtf8;
    }

    string StringFromNativeUtf8(IntPtr nativeUtf8)
    {
      int size = 0;
      byte[] buffer = { };
      do
      {
        ++size;
        Array.Resize(ref buffer, size);
        Marshal.Copy(nativeUtf8, buffer, 0, size);
      } while (buffer[size - 1] != 0); // till 0 termination found

      if (1 == size)
      {
        return ""; // empty string
      }

      Array.Resize(ref buffer, size - 1); // remove terminating 0
      return Encoding.UTF8.GetString(buffer);
    }

    /// <summary>
    /// 추출한 파일 내용 검색엔진에 삽입
    /// </summary>
    /// <param name="id"></param>
    /// <param name="file_path"></param>
    /// <returns></returns>
    public int GetResumeInfo(int id, string file_cont)
    {

      ATLKSearch.RCW.Client ksearch_ins = new ATLKSearch.RCW.Client();
      try
      {
        //ksearch_ins.SetOption(ksearch_ins.OPTION_REQUEST_CHARSET_UTF8, 1);
        int ret = 0, nflag = 1;

        List<string> fieldName = new List<string>(), fieldData = new List<string>();

        fieldName.Add("id");
        fieldName.Add("resume");

        fieldData.Add(id.ToString());
        fieldData.Add(file_cont);


        if (ksearch_ins.BeginSession() < 0)
          throw new Exception(ksearch_ins.GetErrorMessage());
        ksearch_ins.SetOption(ksearch_ins.OPTION_REQUEST_CHARSET_UTF8, 1);

        ret = ksearch_ins.Insert(serverAddr + ":" + serverPort.ToString(), "h1.temp_resume.temp_resume", fieldName.ToArray<object>(), fieldData.ToArray<object>(), 2, ksearch_ins.LC_KOREAN, ksearch_ins.CS_UTF8, nflag);

        ksearch_ins.EndSession();
        return ret;
      }
      catch (Exception e)
      {

        ksearch_ins.EndSession();
        throw e;
      }
    }

    /// <summary>
    /// 추출한 파일 내용 검색엔진에 삽입
    /// </summary>
    /// <param name="id"></param>
    /// <param name="file_path"></param>
    /// <returns></returns>
    public int DelResumeInfo(int id)
    {

      ATLKSearch.RCW.Client ksearch_del = new ATLKSearch.RCW.Client();
      try
      {
        //ksearch_ins.SetOption(ksearch_ins.OPTION_REQUEST_CHARSET_UTF8, 1);
        int ret = 0, nflag = 1;
        object record_cnt = null;
        string query = " id_idx = '" + id.ToString() + "' ";

        if (ksearch_del.BeginSession() < 0)
          throw new Exception(ksearch_del.GetErrorMessage());
        ksearch_del.SetOption(ksearch_del.OPTION_REQUEST_CHARSET_UTF8, 1);

        ret = ksearch_del.Delete(serverAddr + ":" + serverPort.ToString(), out record_cnt, "h1.temp_resume.temp_resume", query, ksearch_del.LC_KOREAN, ksearch_del.CS_UTF8, nflag);

        ksearch_del.EndSession();
        return ret;
      }
      catch (Exception e)
      {

        ksearch_del.EndSession();
        throw e;
      }
    }


    /// <summary>
    /// 회사 검색
    /// </summary>
    /// <param name="company_name"></param>
    /// <returns></returns>
    public int CheckDuplicateResume(List<string> phone, List<string> email)
    {
      ATLKSearch.RCW.Client ksearch_chk = new ATLKSearch.RCW.Client();
      try
      {
        int c_seq = 0;
        string query = String.Empty;// " company_idx = '" + company_name + "' ";
        if (phone.Count() > 0)
        {
          foreach (var p in phone)
          {
            if (!String.IsNullOrEmpty(new String(p.Where(Char.IsDigit).ToArray())))
            {
              query += (!String.IsNullOrWhiteSpace(query) ? " OR " : "") + " phone_idx like '*" + new String(p.Where(Char.IsDigit).ToArray()) + "*'";
            }
          }
        }

        if (email.Count() > 0)
        {
          foreach (var e in email)
          {
            query += (!String.IsNullOrWhiteSpace(query) ? " OR " : "") + " email_idx = '" + e + "' ";
          }
        }

        int ret = 0;
        object rows = null, total = null, arrdata = null, arrsize = null;


        if (ksearch_chk.BeginSession() < 0)
          throw new Exception(ksearch_chk.GetErrorMessage());
        ksearch_chk.SetOption(ksearch_chk.OPTION_REQUEST_CHARSET_UTF8, 1);

        ret = ksearch_chk.SubmitQuery_UTF8(serverAddr, serverPort, "", "compare", "scn_comparison_resume", query, "", "", 0, 1, ksearch_chk.LC_KOREAN, ksearch_chk.CS_UTF8);

        if (ret < 0)
          throw new Exception(ksearch_chk.GetErrorMessage());
        if (ksearch_chk.GetResult_RowSize(out rows) < 0)
          throw new Exception(ksearch_chk.GetErrorMessage());
        if (ksearch_chk.GetResult_TotalCount(out total) < 0)
          throw new Exception(ksearch_chk.GetErrorMessage());

        if ((int)total > 0)
        {
          ret = ksearch_chk.GetResult_Row_UTF8(out arrdata, out arrsize, 0);
          if (ret < 0)
            throw new Exception(ksearch_chk.GetErrorMessage());

          object[] arrdata_tmp = (object[])arrdata;
          c_seq = Convert.ToInt32(arrdata_tmp[0]);
        }

        ksearch_chk.EndSession();
        return c_seq;
      }
      catch (Exception e)
      {
        ksearch_chk.EndSession();
        throw e;
      }
    }


    /// <summary>
    /// 회사 검색
    /// </summary>
    /// <param name="company_name"></param>
    /// <returns></returns>
    public async Task<CandidateResumeFilterResultModel> GetResumeDuplicate(int id, string file_path)
    {
      string file_cont = TempResumeCheck(file_path);
      int ri_ret = GetResumeInfo(id, file_cont.Replace("filter", ""));
      if (ri_ret < 0)
      {
        throw new Exception("INSERT ERROR");
      }

      ATLKSearch.RCW.Client ksearch = new ATLKSearch.RCW.Client();
      try
      {
        string query = " id_idx = '" + id.ToString() + "' ";
        string order = "";
        int ret = 0;
        object rows = null, total = null, arrdata = null, arrsize = null;

        CandidateResumeFilterResultModel result = new CandidateResumeFilterResultModel();

        if (ksearch.BeginSession() < 0)
          throw new Exception(ksearch.GetErrorMessage());

        ksearch.SetOption(ksearch.OPTION_REQUEST_CHARSET_UTF8, 1);

        ret = ksearch.SubmitQuery_UTF8(serverAddr, serverPort, "", "duplicate", "scn_temp_resume", query, order, "", 0, 10, ksearch.LC_KOREAN, ksearch.CS_UTF8);
        if (ret < 0)
          throw new Exception(ksearch.GetErrorMessage());
        if (ksearch.GetResult_RowSize(out rows) < 0)
          throw new Exception(ksearch.GetErrorMessage());
        if (ksearch.GetResult_TotalCount(out total) < 0)
          throw new Exception(ksearch.GetErrorMessage());

        if ((int)total > 0)
        {
          ret = ksearch.GetResult_Row_UTF8(out arrdata, out arrsize, 0);
          if (ret < 0)
            throw new Exception(ksearch.GetErrorMessage());

          object[] arrdata_tmp = (object[])arrdata;

          result.info.names = Convert.ToString(arrdata_tmp[2]).Split('#').ToList();
          result.info.birth = Convert.ToString(arrdata_tmp[3]).Split('#').ToList();
          result.info.phone = Convert.ToString(arrdata_tmp[4]).Replace("02-556-4202", "").Split('#').ToList();
          result.info.email = Convert.ToString(arrdata_tmp[5]).Split('#').ToList();
          result.info.company = Convert.ToString(arrdata_tmp[6]).Split('#').ToList();
          result.info.keywords = Convert.ToString(arrdata_tmp[7]).Split('#').ToList();
          //result.info.school = Convert.ToString(arrdata_tmp[0]).Split('#').ToList();
        }

        if (result.info.email.Count() > 0 || result.info.phone.Count() > 0)
        {
          result.dup_c_seq = CheckDuplicateResume(result.info.phone, result.info.email);
          if (result.dup_c_seq > 0)
          {
            result.is_dup = 1;
          }
        }

        ri_ret = DelResumeInfo(id);

        ksearch.EndSession();
        result.is_work = 1;
        return result;
      }
      catch (Exception e)
      {
        DelResumeInfo(id);
        ksearch.EndSession();
        throw e;
      }
    }

    /// <summary>
    /// 회사 검색
    /// </summary>
    /// <param name="company_name"></param>
    /// <returns></returns>
    public async Task<List<gov_api_company>> SelectCompanyListAsync(string company_name)
    {
      ATLKSearch.RCW.Client ksearch = new ATLKSearch.RCW.Client();
      try
      {
        string query = " company_idx like '*" + company_name + "*' ";
        string order = " order by $SIZE(wkpl_nm) asc, $RELEVANCE desc";
        int ret = 0;
        object rows = null, total = null, arrdata = null, arrsize = null;

        List<gov_api_company> list = new List<gov_api_company>();

        if (ksearch.BeginSession() < 0)
          throw new Exception(ksearch.GetErrorMessage());
        ksearch.SetOption(ksearch.OPTION_REQUEST_CHARSET_UTF8, 1);

        ret = ksearch.SubmitQuery_UTF8(serverAddr, serverPort, "", "company", "scn_company_api", query, order, "", 0, 10, ksearch.LC_KOREAN, ksearch.CS_UTF8);

        if (ret < 0)
          throw new Exception(ksearch.GetErrorMessage());
        if (ksearch.GetResult_RowSize(out rows) < 0)
          throw new Exception(ksearch.GetErrorMessage());
        if (ksearch.GetResult_TotalCount(out total) < 0)
          throw new Exception(ksearch.GetErrorMessage());

        for (int i = 0; i < (int)rows; i++)
        {
          ret = ksearch.GetResult_Row_UTF8(out arrdata, out arrsize, i);
          if (ret < 0)
            throw new Exception(ksearch.GetErrorMessage());

          object[] arrdata_tmp = (object[])arrdata;

          list.Add(new gov_api_company()
          {
            G_SEQ = Convert.ToInt32(arrdata_tmp[0])
              ,
            WKPL_NM = Convert.ToString(arrdata_tmp[1])
              ,
            BZOWR_RGST_NO = Convert.ToString(arrdata_tmp[2])
              ,
            VLDT_VL_KRN_NM = Convert.ToString(arrdata_tmp[3])
          });
        }

        ksearch.EndSession();
        return list;
      }
      catch (Exception e)
      {
        ksearch.EndSession();
        throw e;
      }
    }

    public async Task<List<code_keyword>> SelectKeywordListAsync(string keyword, List<string> busi, List<String> job)
    {
      ATLKSearch.RCW.Client ksearch = new ATLKSearch.RCW.Client();
      try
      {
        string query = " keyword_idx = '" + keyword + "' allwordthruindex synonym ";
        if (busi != null)
        {
          if (busi.Count > 0)
          {
            string busi_str = String.Empty;
            foreach (string v in busi)
            {
              busi_str += (!String.IsNullOrEmpty(busi_str) ? "," : "") + "'B" + v + "'";
            }
            query += " or code_total_idx in {" + busi_str + "} allwordthruindex ";
          }
        }

        if (job != null)
        {
          if (job.Count > 0)
          {
            string job_str = String.Empty;
            foreach (string v in job)
            {
              job_str += (!String.IsNullOrEmpty(job_str) ? "," : "") + "'J" + v + "'";
            }
            query += " or code_total_idx in {" + job_str + "} allwordthruindex ";
          }
        }

        string order = " order by $relevance(tf, idf, prox, ('" + keyword.ToLower() + "', 5)) desc";
        int ret = 0;
        object rows = null, total = null, arrdata = null, arrsize = null;

        List<code_keyword> list = new List<code_keyword>();

        if (ksearch.BeginSession() < 0)
          throw new Exception(ksearch.GetErrorMessage());
        ksearch.SetOption(ksearch.OPTION_REQUEST_CHARSET_UTF8, 1);

        ret = ksearch.SubmitQuery_UTF8(serverAddr, serverPort, "", "keyword", "scn_keyword", query, order, "", 0, 255, ksearch.LC_KOREAN, ksearch.CS_UTF8);

        if (ret < 0)
          throw new Exception(ksearch.GetErrorMessage());
        if (ksearch.GetResult_RowSize(out rows) < 0)
          throw new Exception(ksearch.GetErrorMessage());
        if (ksearch.GetResult_TotalCount(out total) < 0)
          throw new Exception(ksearch.GetErrorMessage());

        for (int i = 0; i < (int)rows; i++)
        {
          ret = ksearch.GetResult_Row_UTF8(out arrdata, out arrsize, i);
          if (ret < 0)
            throw new Exception(ksearch.GetErrorMessage());

          object[] arrdata_tmp = (object[])arrdata;

          list.Add(new code_keyword()
          {
            ck_seq = Convert.ToInt32(arrdata_tmp[0])
              ,
            name = Convert.ToString(arrdata_tmp[1])
              ,
            code_total = Convert.ToString(arrdata_tmp[2])
          });
        }

        ksearch.EndSession();
        return list;
      }
      catch (Exception e)
      {
        ksearch.EndSession();
        throw e;
      }
    }

    /// <summary>
    /// 회사 검색
    /// </summary>
    /// <param name="company_name"></param>
    /// <returns></returns>
    public List<candidate> SelectCandidateList(CandidateSearchModel search, int skip, int count, string user_name, out int totalCount)
    {
      ATLKSearch.RCW.Client ksearch = new ATLKSearch.RCW.Client();
      try
      {
        string selectQuery = String.Empty;
        string highlight_text = String.Empty;
        string order = String.Empty;
        if (!String.IsNullOrEmpty(search.orderOption) && !String.IsNullOrEmpty(search.orderTxt))
        {
          order = " order by " + search.orderTxt + " " + search.orderOption;
          if (search.orderTxt != "create_dt_idx")
          {
            order += ", c_seq_idx DESC ";
          }
        }
        else if (search.is_sugg == 1)
        {
          //order = " order by $RELEVANCE desc";
        }
        else
        {
          order = " order by create_dt_idx DESC, c_seq_idx DESC";
        }

        if (!string.IsNullOrWhiteSpace(search.total))
        {
          if (new string[] { "&&", "||", "!!" }.Any(c => search.total.Contains(c)))
          {
            //selectQuery += (!string.IsNullOrWhiteSpace(selectQuery) ? " AND " : "") + " primary_idx = '" + search.total.Replace("&&", "&").Replace("||", "|").Replace("!!", "!") + "' boolean";

            selectQuery += (!string.IsNullOrWhiteSpace(selectQuery) ? " AND " : "") + "(" + getBooleanSearchParse("primary_idx", search.total) + ")";
          }
          else
          {
            selectQuery += (!string.IsNullOrWhiteSpace(selectQuery) ? " AND " : "") + " primary_idx = '" + search.total + "' allwordthruindex synonym ";
          }
          highlight_text += (string.IsNullOrEmpty(highlight_text) ? "" : " ") + search.total;
          //order = " order by $RELEVANCE desc, $ROWID DESC";
        }


        if (!string.IsNullOrWhiteSpace(search.name))
        {
          if (new string[] { "\"" }.Any(c => search.name.Contains(c)))
          {
            selectQuery += (!string.IsNullOrWhiteSpace(selectQuery) ? " AND " : "") + " name_idx = '" + search.name.Replace("\"", "") + "' ";
          }
          else
          {
            selectQuery += (!string.IsNullOrWhiteSpace(selectQuery) ? " AND " : "") + " name_idx like '*" + search.name + "*' ";
          }

          highlight_text += (string.IsNullOrEmpty(highlight_text) ? "" : " ") + search.name;
        }

        if (search.foreign)
          selectQuery += (!string.IsNullOrWhiteSpace(selectQuery) ? " AND " : "") + " is_foreign_idx = 1 ";

        if (!string.IsNullOrWhiteSpace(search.startBirth))
          selectQuery += (!string.IsNullOrWhiteSpace(selectQuery) ? " AND " : "") + " birth_yyyy >= " + (search.startBirth.Length == 2 ? "19" + search.startBirth : search.startBirth);
        if (!string.IsNullOrWhiteSpace(search.endBirth))
          selectQuery += (!string.IsNullOrWhiteSpace(selectQuery) ? " AND " : "") + " birth_yyyy <= " + (search.endBirth.Length == 2 ? "19" + search.endBirth : search.endBirth);

        if (search.gender != 0)
          selectQuery += (!string.IsNullOrWhiteSpace(selectQuery) ? " AND " : "") + " gender_idx = " + search.gender;

        if (!string.IsNullOrWhiteSpace(search.phone))
          if (!string.IsNullOrWhiteSpace(new String(search.phone.Where(Char.IsDigit).ToArray())))
            selectQuery += (!string.IsNullOrWhiteSpace(selectQuery) ? " AND " : "") + " phone_idx like '*" + new String(search.phone.Where(Char.IsDigit).ToArray()) + "*'";

        if (!string.IsNullOrWhiteSpace(search.email))
          selectQuery += (!string.IsNullOrWhiteSpace(selectQuery) ? " AND " : "") + " email_idx like '*" + search.email + "*'";

        if (!string.IsNullOrWhiteSpace(search.sns))
          selectQuery += (!string.IsNullOrWhiteSpace(selectQuery) ? " AND " : "") + " sns_link_idx like '*" + search.sns + "*' ";


        if (!string.IsNullOrWhiteSpace(search.school))
        {
          if (new string[] { "&&", "||", "!!" }.Any(c => search.school.Contains(c)))
          {
            //selectQuery += (!string.IsNullOrWhiteSpace(selectQuery) ? " AND " : "") + " school_idx = '" + search.school.Replace("&&", "&").Replace("||", "|").Replace("!!", "!") + "' boolean";
            selectQuery += (!string.IsNullOrWhiteSpace(selectQuery) ? " AND " : "") + "(" + getBooleanSearchParse("school_idx", search.school) + ")";
          }
          else if (new string[] { "," }.Any(c => search.school.Contains(c)))
          {
            List<string> school_list = search.school.Split(',').ToList();

            if (school_list.Count > 0)
            {
              string school_sql = String.Empty;

              foreach (var scl in school_list.Where(x => !String.IsNullOrWhiteSpace(x)))
              {
                school_sql += (!string.IsNullOrWhiteSpace(school_sql) ? " OR " : "") + " (school_idx = '" + scl.Trim() + "' allword synonym OR school_idx like '*" + scl.Trim() + "*') ";
              }
              if (!string.IsNullOrEmpty(school_sql))
              {
                selectQuery += (!string.IsNullOrWhiteSpace(selectQuery) ? " AND " : "") + "(" + school_sql + ")";
              }
            }
          }
          else
          {
            selectQuery += (!string.IsNullOrWhiteSpace(selectQuery) ? " AND " : "") + " school_idx = '" + search.school + "' allwordthruindex synonym  ";
          }

          highlight_text += (string.IsNullOrEmpty(highlight_text) ? "" : " ") + search.school.Replace("|", " ").Replace("!", " ").Replace("&", " ").Replace(",", " ");
        }


        if (search.foreign_school)
          selectQuery += (!string.IsNullOrWhiteSpace(selectQuery) ? " AND " : "") + " is_foreign_school_idx = 1  ";

        if (!string.IsNullOrWhiteSpace(search.category1_name))
          selectQuery += (!string.IsNullOrWhiteSpace(selectQuery) ? " AND " : "") + " major_category_idx = '" + search.category1_name + "'";

        if (!string.IsNullOrWhiteSpace(search.major))
        {
          if (new string[] { "&&", "||", "!!" }.Any(c => search.major.Contains(c)))
          {
            //selectQuery += (!string.IsNullOrWhiteSpace(selectQuery) ? " AND " : "") + " major_idx = '" + search.major.Replace("&&", "&").Replace("||", "|").Replace("!!", "!") + "' boolean";
            selectQuery += (!string.IsNullOrWhiteSpace(selectQuery) ? " AND " : "") + "(" + getBooleanSearchParse("major_idx", search.major) + ")";
          }
          else if (new string[] { "," }.Any(c => search.major.Contains(c)))
          {
            List<string> major_list = search.major.Split(',').ToList();

            if (major_list.Count > 0)
            {
              string major_sql = String.Empty;

              foreach (var mjr in major_list.Where(x => !String.IsNullOrWhiteSpace(x)))
              {
                major_sql += (!string.IsNullOrWhiteSpace(major_sql) ? " OR " : "") + " major_idx = '" + mjr.Trim() + "' allword synonym ";
              }
              if (!string.IsNullOrEmpty(major_sql))
              {
                selectQuery += (!string.IsNullOrWhiteSpace(selectQuery) ? " AND " : "") + "(" + major_sql + ")";
              }
            }
          }
          else
          {
            selectQuery += (!string.IsNullOrWhiteSpace(selectQuery) ? " AND " : "") + " ( major_idx = '" + search.major + "' allword synonym  OR major_idx like '*" + search.major + "*')";
          }

          highlight_text += (string.IsNullOrEmpty(highlight_text) ? "" : " ") + search.major.Replace("|", " ").Replace("!", " ").Replace("&", " ").Replace(",", " ");
        }


        if (search.final_edu.Count > 0 && search.final_edu.Count < 6)
        {
          string in_str = String.Empty;
          foreach (var code in search.final_edu)
          {
            in_str += (!String.IsNullOrEmpty(in_str) ? ", " : "") + "'" + code.ToString() + "'";
          }
          selectQuery += (!string.IsNullOrWhiteSpace(selectQuery) ? " AND " : "") + " final_edu_code_idx in {" + in_str + "} ";
        }

        if (!string.IsNullOrWhiteSpace(search.company) && search.final_company)
        {
          if (new string[] { "&&", "||", "!!" }.Any(c => search.company.Contains(c)))
          {
            //selectQuery += (!string.IsNullOrWhiteSpace(selectQuery) ? " AND " : "") + " final_company_idx = '" + search.company.Replace("&&", "&").Replace("||", "|").Replace("!!", "!") + "' boolean";
            selectQuery += (!string.IsNullOrWhiteSpace(selectQuery) ? " AND " : "") + "(" + getBooleanSearchParse("final_company_idx", search.company) + ")";
          }
          else if (new string[] { "," }.Any(c => search.company.Contains(c)))
          {
            List<string> company_list = search.company.Split(',').ToList();

            if (company_list.Count > 0)
            {
              string company_sql = String.Empty;

              foreach (var cpn in company_list.Where(x => !String.IsNullOrWhiteSpace(x)))
              {
                company_sql += (!string.IsNullOrWhiteSpace(company_sql) ? " OR " : "") + " (final_company_idx = '" + cpn + "' allword synonym OR final_company_idx like '*" + cpn + "*' ) ";
              }
              if (!string.IsNullOrEmpty(company_sql))
              {
                selectQuery += (!string.IsNullOrWhiteSpace(selectQuery) ? " AND " : "") + "(" + company_sql + ")";
              }
            }
          }
          else
          {
            selectQuery += (!string.IsNullOrWhiteSpace(selectQuery) ? " AND " : "") + " (final_company_idx = '" + search.company + "' allwordthruindex synonym  OR final_company_idx like '*" + search.company + "*')";
          }

          highlight_text += (string.IsNullOrEmpty(highlight_text) ? "" : " ") + search.company.Replace("|", " ").Replace("!", " ").Replace("&", " ").Replace(",", " ");
        }

        else if (!string.IsNullOrWhiteSpace(search.company) && !search.final_company)
        {
          if (new string[] { "&&", "||", "!!" }.Any(c => search.company.Contains(c)))
          {
            //selectQuery += (!string.IsNullOrWhiteSpace(selectQuery) ? " AND " : "") + " company_idx = '" + search.company.Replace("&&", "&").Replace("||", "|").Replace("!!", "!") + "' boolean";
            selectQuery += (!string.IsNullOrWhiteSpace(selectQuery) ? " AND " : "") + "(" + getBooleanSearchParse("company_idx", search.company) + ")";
          }
          else if (new string[] { "," }.Any(c => search.company.Contains(c)))
          {
            List<string> company_list = search.company.Split(',').ToList();

            if (company_list.Count > 0)
            {
              string company_sql = String.Empty;

              foreach (var cpn in company_list.Where(x => !String.IsNullOrWhiteSpace(x)))
              {
                company_sql += (!string.IsNullOrWhiteSpace(company_sql) ? " OR " : "") + " (company_idx = '" + cpn.Trim() + "' allword synonym OR company_idx like '*" + cpn.Trim() + "*')";
              }
              if (!string.IsNullOrEmpty(company_sql))
              {
                selectQuery += (!string.IsNullOrWhiteSpace(selectQuery) ? " AND " : "") + "(" + company_sql + ")";
              }
            }
          }
          else
          {
            selectQuery += (!string.IsNullOrWhiteSpace(selectQuery) ? " AND " : "") + " ( company_idx = '" + search.company + "' allwordthruindex synonym  OR company_idx like '*" + search.company + "*')";
          }

          highlight_text += (string.IsNullOrEmpty(highlight_text) ? "" : " ") + search.company.Replace("|", " ").Replace("!", " ").Replace("&", " ").Replace(",", " ");
        }
        if (!string.IsNullOrWhiteSpace(search.isWork))
          selectQuery += (!string.IsNullOrWhiteSpace(selectQuery) ? " AND " : "") + " is_work_idx = " + search.isWork;

        if (search.job.Count > 0)
        {/*
          if (search.job.First().code2.ToString().Length == 3)
          {
            string from_code1 = search.job.First().code2.ToString() + "000";
            string to_code1 = search.job.First().code2.ToString() + "999";
            selectQuery += (!string.IsNullOrWhiteSpace(selectQuery) ? " AND " : "") + "(jobCode_idx >= '" + from_code1 + "' and jobCode_idx <= '" + to_code1 + "') ";
          }
          else
          {*/
          string job_sql = String.Empty;

          foreach (var job in search.job)
          {
            if (job.code2.ToString().Length == 3)
            {
              job_sql += (!string.IsNullOrWhiteSpace(job_sql) ? " " + search.job_search_type + " " : "") + " (jobCode_idx >= '" + job.code2 + "000' and jobCode_idx <= '" + job.code2 + "999') ";
            }
            else
            {
              job_sql += (!string.IsNullOrWhiteSpace(job_sql) ? " " + search.job_search_type + " " : "") + " jobCode_idx = '" + job.code2 + "' ";
            }
          }
          if (!string.IsNullOrEmpty(job_sql))
          {
            selectQuery += (!string.IsNullOrWhiteSpace(selectQuery) ? " AND " : "") + "(" + job_sql + ")";
          }
          /*}*/

        }
        if (search.business.Count > 0)
        {/*
          if (search.business.First().code2.ToString().Length == 3)
          {
            string from_code1 = search.business.First().code2.ToString() + "000";
            string to_code1 = search.business.First().code2.ToString() + "999";
            selectQuery += (!string.IsNullOrWhiteSpace(selectQuery) ? " AND " : "") + "(busiCode_idx >= '" + from_code1 + "' and busiCode_idx <= '" + to_code1 + "') ";
          }
          else
          {*/
          string business_sql = String.Empty;

          foreach (var business in search.business)
          {
            if (business.code2.ToString().Length == 3)
            {
              business_sql += (!string.IsNullOrWhiteSpace(business_sql) ? " " + search.business_search_type + " " : "") + " (busiCode_idx >= '" + business.code2 + "000' and busiCode_idx <= '" + business.code2 + "999') ";
            }
            else
            {
              business_sql += (!string.IsNullOrWhiteSpace(business_sql) ? " " + search.business_search_type + " " : "") + " busiCode_idx = '" + business.code2 + "' ";
            }
          }
          if (!string.IsNullOrEmpty(business_sql))
          {
            selectQuery += (!string.IsNullOrWhiteSpace(selectQuery) ? " AND " : "") + "(" + business_sql + ")";
          }
          /*}*/

        }

        if (!string.IsNullOrWhiteSpace(search.keyword))
        {
          if (new string[] { "&&", "||", "!!" }.Any(c => search.keyword.Contains(c)))
          {
            //selectQuery += (!string.IsNullOrWhiteSpace(selectQuery) ? " AND " : "") + " keyword_idx = '" + search.keyword.Replace("&&", "&").Replace("||", "|").Replace("!!", "!") + "' boolean  ";
            selectQuery += (!string.IsNullOrWhiteSpace(selectQuery) ? " AND " : "") + "(" + getBooleanSearchParse("keyword_idx", search.keyword) + ")";
          }
          else
          {
            selectQuery += (!string.IsNullOrWhiteSpace(selectQuery) ? " AND " : "") + " keyword_idx = '" + search.keyword + "' allwordthruindex synonym ";
          }
          highlight_text += (string.IsNullOrEmpty(highlight_text) ? "" : " ") + search.keyword;

        }


        if (!string.IsNullOrWhiteSpace(search.language) && search.ability > 0)
          selectQuery += (!string.IsNullOrWhiteSpace(selectQuery) ? " AND " : "") + " language_idx like '*" + search.language + "/" + search.ability.ToString() + "*'  ";

        if (!string.IsNullOrWhiteSpace(search.create_dt_start))
          selectQuery += (!string.IsNullOrWhiteSpace(selectQuery) ? " AND " : "") + " create_dt_idx >= '" + search.create_dt_start.Replace("-", "") + "000000'";
        if (!string.IsNullOrWhiteSpace(search.create_dt_end))
          selectQuery += (!string.IsNullOrWhiteSpace(selectQuery) ? " AND " : "") + " create_dt_idx <= '" + search.create_dt_end.Replace("-", "") + "235959'";

        if (search.confidential >= 0)
          selectQuery += (!string.IsNullOrWhiteSpace(selectQuery) ? " AND " : "") + " is_confidential_idx = " + search.confidential.ToString();

        if (search.inactive >= 0)
          selectQuery += (!string.IsNullOrWhiteSpace(selectQuery) ? " AND " : "") + " is_inactive_idx = " + search.inactive.ToString();

        if (!string.IsNullOrWhiteSpace(search.memo))
        {
          selectQuery += (!string.IsNullOrWhiteSpace(selectQuery) ? " AND " : "") + " all_activity_idx = '" + search.memo + "' allwordthruindex synonym ";
          highlight_text += (string.IsNullOrEmpty(highlight_text) ? "" : " ") + search.memo;
        }

        if (!string.IsNullOrWhiteSpace(search.modify_dt_start))
          selectQuery += (!string.IsNullOrWhiteSpace(selectQuery) ? " AND " : "") + " modify_dt_idx >= '" + search.modify_dt_start.Replace("-", "") + "000000'";
        if (!string.IsNullOrWhiteSpace(search.modify_dt_end))
          selectQuery += (!string.IsNullOrWhiteSpace(selectQuery) ? " AND " : "") + " modify_dt_idx <= '" + search.modify_dt_end.Replace("-", "") + "235959'";

        if (search.keyword_list.Count > 0)
        {
          string keyword_sql = String.Empty;

          foreach (var kws in search.keyword_list)
          {
            keyword_sql += (!string.IsNullOrWhiteSpace(keyword_sql) ? " OR " : "") + " primary_idx = '" + kws + "' allwordthruindex synonym ";
            //" primary_idx = '" + search.total + "' allwordthruindex synonym "
          }
          if (!string.IsNullOrEmpty(keyword_sql))
          {
            selectQuery += (!string.IsNullOrWhiteSpace(selectQuery) ? " AND " : "") + "(" + keyword_sql + ")";
          }
        }

        if (search.except_c_seq.Count > 0)
        {
          string c_in_str = String.Empty;
          foreach (var code in search.except_c_seq)
          {
            c_in_str += (!String.IsNullOrEmpty(c_in_str) ? ", " : "") + "'" + code.ToString() + "'";
          }
          selectQuery += (!string.IsNullOrWhiteSpace(selectQuery) ? " AND " : "") + " c_seq_idx not in {" + c_in_str + "} ";
        }


        if (search.career_start > 0)
        {
          var career_start_dt = DateTime.Today.AddMonths((search.career_start * -1)).ToString("yyyyMMddHHmmss");
          selectQuery += (!string.IsNullOrWhiteSpace(selectQuery) ? " AND " : "") + " ((career_start_idx <= '" + career_start_dt + "' ";
          selectQuery += " AND career_end_idx = 'X' ) ";
          selectQuery += " OR career_range_idx >= " + search.career_start + " ) ";
        }

        if (search.career_end > 0)
        {
          var career_end_dt = DateTime.Today.AddMonths((search.career_end * -1)).ToString("yyyyMMddHHmmss");
          selectQuery += (!string.IsNullOrWhiteSpace(selectQuery) ? " AND " : "") + " ((career_start_idx >= '" + career_end_dt + "' ";
          selectQuery += " AND career_end_idx = 'X' ) ";
          selectQuery += " OR (career_range_idx <= " + search.career_end + " AND career_range_idx > 0 )) ";
        }




        //selectQuery += (!string.IsNullOrWhiteSpace(selectQuery) ? ";" : "") + search.total;
        highlight_text = highlight_text.Replace("&&", " ").Replace("||", " ").Replace("!!", " ").Replace("(", " ").Replace(")", " ").Replace("\"", "");
        int ret = 0;
        object rows = null, total = null, arrdata = null, arrsize = null;

        List<candidate> list = new List<candidate>();

        if (ksearch.BeginSession() < 0)
          throw new Exception(ksearch.GetErrorMessage());
        ksearch.SetOption(ksearch.OPTION_REQUEST_CHARSET_UTF8, 1);

        string search_log = "TOTAL@candidate+";
        if (search.is_sugg == 1)
        {
          search_log = "TOTAL@cand_sugg+";
        }

        ret = ksearch.SubmitQuery_UTF8(serverAddr, serverPort, "", search_log + user_name + "$|첫검색|" + skip.ToString() + "|" + highlight_text, "scn_candidate", selectQuery, order, highlight_text, (skip - 1) * count, count, ksearch.LC_KOREAN, ksearch.CS_UTF8);

        if (ret < 0)
          throw new Exception(ksearch.GetErrorMessage());
        if (ksearch.GetResult_RowSize(out rows) < 0)
          throw new Exception(ksearch.GetErrorMessage());
        if (ksearch.GetResult_TotalCount(out total) < 0)
          throw new Exception(ksearch.GetErrorMessage());

        totalCount = (int)total;

        for (int i = 0; i < (int)rows; i++)
        {
          ret = ksearch.GetResult_Row_UTF8(out arrdata, out arrsize, i);
          if (ret < 0)
            throw new Exception(ksearch.GetErrorMessage());

          object[] arrdata_tmp = (object[])arrdata;

          candidate cd = new candidate();


          cd.c_seq = Convert.ToInt32(arrdata_tmp[0]);
          cd.kor_name = Convert.ToString(arrdata_tmp[1]);
          cd.eng_name = Convert.ToString(arrdata_tmp[2]);
          cd.is_foreign = Convert.ToDouble(arrdata_tmp[3]);
          //cd.birth_date = Convert.ToDateTime(arrdata_tmp[4]);
          if (!String.IsNullOrEmpty(Convert.ToString(arrdata_tmp[4])))
          {
            cd.birth_date = Convert.ToDateTime(arrdata_tmp[4]);
          }
          cd.ex_birthdate = Convert.ToDouble(arrdata_tmp[5]);
          cd.gender = Convert.ToDouble(arrdata_tmp[6]);
          //cd.create_dt = Convert.ToDateTime(arrdata_tmp[7]);
          if (!String.IsNullOrEmpty(Convert.ToString(arrdata_tmp[7])))
          {
            cd.create_dt = Convert.ToDateTime(arrdata_tmp[7]);
          }
          cd.email1 = Convert.ToString(arrdata_tmp[8]);
          cd.email2 = Convert.ToString(arrdata_tmp[9]);
          cd.phone = Convert.ToString(arrdata_tmp[10]);
          cd.wrong_phone = Convert.ToInt32(arrdata_tmp[11]);
          cd.cell_phone = Convert.ToString(arrdata_tmp[12]);
          cd.wrong_phone2 = Convert.ToInt32(arrdata_tmp[13]);
          cd.is_work = Convert.ToInt32(arrdata_tmp[14]);
          cd.company_name = Convert.ToString(arrdata_tmp[15]);
          cd.school_name = Convert.ToString(arrdata_tmp[16]);
          cd.category1_name = Convert.ToString(arrdata_tmp[17]);
          cd.busiName = Convert.ToString(arrdata_tmp[18]);
          cd.jobName = Convert.ToString(arrdata_tmp[19]);
          cd.keyword = Convert.ToString(arrdata_tmp[20]);
          cd.kor_keyword = Convert.ToString(arrdata_tmp[21]);
          cd.eng_keyword = Convert.ToString(arrdata_tmp[22]);
          cd.kor_file_dir = Convert.ToString(arrdata_tmp[23]);
          cd.kor_file_ext = Convert.ToString(arrdata_tmp[24]);
          cd.eng_file_dir = Convert.ToString(arrdata_tmp[25]);
          cd.eng_file_ext = Convert.ToString(arrdata_tmp[26]);
          cd.sns_link1 = Convert.ToString(arrdata_tmp[27]);
          cd.sns_link2 = Convert.ToString(arrdata_tmp[28]);
          cd.sns_link3 = Convert.ToString(arrdata_tmp[29]);
          cd.is_confidential = Convert.ToInt32(arrdata_tmp[30]);
          cd.is_inactive = Convert.ToInt32(arrdata_tmp[31]);
          if (!String.IsNullOrEmpty(Convert.ToString(arrdata_tmp[32])))
          {
            cd.modify_dt = Convert.ToDateTime(arrdata_tmp[32]);
          }
          cd.all_company = Convert.ToString(arrdata_tmp[33]);

          if (Convert.ToString(arrdata_tmp[34]).Contains("<span class='bg-warning'>"))
          {
            cd.kor_resume_body = Convert.ToString(arrdata_tmp[34]);
          }

          if (Convert.ToString(arrdata_tmp[35]).Contains("<span class='bg-warning'>"))
          {
            cd.eng_resume_body = Convert.ToString(arrdata_tmp[35]);
          }
          cd.major_name = Convert.ToString(arrdata_tmp[36]);

          if (Convert.ToString(arrdata_tmp[37]).Contains("<span class='bg-warning'>"))
          {
            cd.memo_str = Convert.ToString(arrdata_tmp[37]);
          }
          if (!String.IsNullOrEmpty(Convert.ToString(arrdata_tmp[38])))
            cd.caution_type = Convert.ToInt32(arrdata_tmp[38]);

          cd.kor_cr_seq = Convert.ToInt32(arrdata_tmp[39]);
          cd.eng_cr_seq = Convert.ToInt32(arrdata_tmp[41]);

          cd.all_school = Convert.ToString(arrdata_tmp[43]);

          if (!String.IsNullOrEmpty(Convert.ToString(arrdata_tmp[44])))
          {
            cd.career_start_dt = Convert.ToDateTime(arrdata_tmp[44]);
          }
          if (!String.IsNullOrEmpty(Convert.ToString(arrdata_tmp[45])) && Convert.ToString(arrdata_tmp[45]) != "X")
          {
            cd.career_end_dt = Convert.ToDateTime(arrdata_tmp[45]);
          }
          if (!String.IsNullOrEmpty(Convert.ToString(arrdata_tmp[46])))
          {
            cd.career_range = Convert.ToInt32(arrdata_tmp[46]);
          }

          if (cd.career_range.HasValue && cd.career_range > 0)
          {
            cd.cur_career_month = cd.career_range.Value;
          }
          else if (cd.career_start_dt.HasValue)
          {
            DateTime career_end = cd.career_end_dt.HasValue ? cd.career_end_dt.Value : DateTime.Today;

            cd.cur_career_month = ((career_end.Year - cd.career_start_dt.Value.Year) * 12) + career_end.Month - cd.career_start_dt.Value.Month;

          }

          list.Add(cd);

        }

        ksearch.EndSession();
        return list;
      }
      catch (Exception e)
      {
        ksearch.EndSession();
        throw e;
      }
    }

    public List<candidate> SelectTokenCandidateList(CandidateSearchModel search, CandidateSearchOptionModel opt, int skip, int count, string user_name, out int totalCount)
    {
      ATLKSearch.RCW.Client ksearch = new ATLKSearch.RCW.Client();
      try
      {
        string selectQuery = String.Empty;
        string highlight_text = String.Empty;
        List<string> syn_highlight_text = new List<string>();
        string order = String.Empty;
        if (!String.IsNullOrEmpty(search.orderOption) && !String.IsNullOrEmpty(search.orderTxt))
        {
          order = " order by " + search.orderTxt + " " + search.orderOption;
        }
        else if (search.is_sugg == 1)
        {
          //order = " order by $RELEVANCE desc";
        }
        else
        {
          order = " order by create_dt_idx DESC, c_seq_idx DESC";
        }

        if (!string.IsNullOrWhiteSpace(search.total))
        {
          string search_word = String.Empty;

          foreach (var word in search.total.Split(','))
          {
            search_word += (!string.IsNullOrWhiteSpace(search_word) ? opt.total_and_or : "");

            if (new string[] { "&&", "||", "!!" }.Any(c => word.Contains(c)))
            {
              search_word += " (" + getBooleanSearchParse("primary_idx", word.Trim().ToLower(), opt.total) + ") ";
              highlight_text += (string.IsNullOrEmpty(highlight_text) ? "" : " ") + word.Replace("&&", " ").Replace("||", " ").Replace("!!", " ").Replace(")", "").Replace("(", "");
            }
            else
            {
              if (opt.total)
              {
                search_word += "  primary_idx = '" + word.Trim().ToLower() + "' allorderadjacent synonym ";
                syn_highlight_text.Add(word.Trim());
              }
              else
              {

                search_word += "  primary_idx = '" + word.Trim().ToLower() + "' allorderadjacent ";
                highlight_text += (string.IsNullOrEmpty(highlight_text) ? "" : " ") + word.Trim();

              }
            }
          }

          selectQuery += (!string.IsNullOrWhiteSpace(selectQuery) ? " AND " : "") + "(" + search_word + ")";
        }


        if (!string.IsNullOrWhiteSpace(search.name))
        {
          if (search.name.Contains("*"))
          {
            selectQuery += (!string.IsNullOrWhiteSpace(selectQuery) ? " AND " : "") + " name_idx like '" + search.name + "' ";
            highlight_text += (string.IsNullOrEmpty(highlight_text) ? "" : " ") + search.name.Replace("*", "");
          }
          else
          {
            if (opt.name)
            {
              selectQuery += (!string.IsNullOrWhiteSpace(selectQuery) ? " AND " : "") + " name_idx = '" + search.name + "' ";
            }
            else
            {
              selectQuery += (!string.IsNullOrWhiteSpace(selectQuery) ? " AND " : "") + " name_idx like '*" + search.name + "*' ";
            }
            highlight_text += (string.IsNullOrEmpty(highlight_text) ? "" : " ") + search.name;
          }



        }

        if (search.foreign)
          selectQuery += (!string.IsNullOrWhiteSpace(selectQuery) ? " AND " : "") + " is_foreign_idx = 1 ";

        if (!string.IsNullOrWhiteSpace(search.startBirth))
          selectQuery += (!string.IsNullOrWhiteSpace(selectQuery) ? " AND " : "") + " birth_yyyy >= " + (search.startBirth.Length == 2 ? "19" + search.startBirth : search.startBirth);
        if (!string.IsNullOrWhiteSpace(search.endBirth))
          selectQuery += (!string.IsNullOrWhiteSpace(selectQuery) ? " AND " : "") + " birth_yyyy <= " + (search.endBirth.Length == 2 ? "19" + search.endBirth : search.endBirth);

        if (search.gender != 0)
          selectQuery += (!string.IsNullOrWhiteSpace(selectQuery) ? " AND " : "") + " gender_idx = " + search.gender;

        if (!string.IsNullOrWhiteSpace(search.phone))
          if (!string.IsNullOrWhiteSpace(new String(search.phone.Where(Char.IsDigit).ToArray())))
            selectQuery += (!string.IsNullOrWhiteSpace(selectQuery) ? " AND " : "") + " phone_idx like '*" + new String(search.phone.Where(Char.IsDigit).ToArray()) + "*'";

        if (!string.IsNullOrWhiteSpace(search.email))
          selectQuery += (!string.IsNullOrWhiteSpace(selectQuery) ? " AND " : "") + " email_idx like '*" + search.email + "*'";

        if (!string.IsNullOrWhiteSpace(search.sns))
          selectQuery += (!string.IsNullOrWhiteSpace(selectQuery) ? " AND " : "") + " sns_link_idx like '*" + search.sns + "*' ";


        if (!string.IsNullOrWhiteSpace(search.school))
        {
          string search_word = String.Empty;

          foreach (var word in search.school.Split(','))
          {
            search_word += (!string.IsNullOrWhiteSpace(search_word) ? opt.school_and_or : "");
            if (word.Contains("#"))
            {
              search_word += " school_idx = '" + word.Trim().ToLower() + "' allorderadjacent synonym ";
              highlight_text += (string.IsNullOrEmpty(highlight_text) ? "" : " ") + word.Replace("#", "").Trim();
            }
            else if (new string[] { "&&", "||", "!!" }.Any(c => word.Contains(c)))
            {
              search_word += " (" + getBooleanSearchParse("school_idx", word.Trim().ToLower(), opt.school) + ") ";
              highlight_text += (string.IsNullOrEmpty(highlight_text) ? "" : " ") + word.Replace("&&", " ").Replace("||", " ").Replace("!!", " ").Replace(")", "").Replace("(", "");
            }
            else
            {
              if (opt.school)
              {
                search_word += " ( school_idx = '" + word.Trim().ToLower() + "' allorderadjacent synonym OR school_idx like '*" + word.Trim().ToLower() + "*') ";
                syn_highlight_text.Add(word.Trim());
              }
              else
              {
                search_word += " ( school_idx = '\"" + word.Trim().ToLower() + "\"' allorder OR school_idx = '" + word.Trim().ToLower() + "') ";
                highlight_text += (string.IsNullOrEmpty(highlight_text) ? "" : " ") + word.Trim();
              }

            }


          }

          selectQuery += (!string.IsNullOrWhiteSpace(selectQuery) ? " AND " : "") + "(" + search_word + ")";
        }

        if (search.foreign_school)
          selectQuery += (!string.IsNullOrWhiteSpace(selectQuery) ? " AND " : "") + " is_foreign_school_idx = 1  ";

        if (!string.IsNullOrWhiteSpace(search.category1_name))
          selectQuery += (!string.IsNullOrWhiteSpace(selectQuery) ? " AND " : "") + " major_category_idx = '" + search.category1_name + "'";

        if (!string.IsNullOrWhiteSpace(search.major))
        {
          string search_word = String.Empty;

          foreach (var word in search.major.Split(','))
          {
            search_word += (!string.IsNullOrWhiteSpace(search_word) ? opt.major_and_or : "");

            if (new string[] { "&&", "||", "!!" }.Any(c => word.Contains(c)))
            {
              search_word += " (" + getBooleanSearchParse("major_idx", word.Trim().ToLower(), opt.major) + ") ";

              highlight_text += (string.IsNullOrEmpty(highlight_text) ? "" : " ") + word.Replace("&&", " ").Replace("||", " ").Replace("!!", " ").Replace(")", "").Replace("(", "");
            }
            else
            {
              if (opt.major)
              {
                search_word += " (major_idx = '" + word.Trim().ToLower() + "' allorderadjacent synonym OR  major_idx like '*" + word.Trim().ToLower() + "*')";
                syn_highlight_text.Add(word.Trim());
              }
              else
              {
                search_word += "(major_idx = '\"" + word.Trim().ToLower() + "\"' allorder OR  major_idx like '*" + word.Trim().ToLower() + "*') ";
                highlight_text += (string.IsNullOrEmpty(highlight_text) ? "" : " ") + word.Trim();
              }
            }

          }

          selectQuery += (!string.IsNullOrWhiteSpace(selectQuery) ? " AND " : "") + "(" + search_word + ")";
        }

        if (search.final_edu.Count > 0 && search.final_edu.Count < 6)
        {
          string in_str = String.Empty;
          foreach (var code in search.final_edu)
          {
            in_str += (!String.IsNullOrEmpty(in_str) ? ", " : "") + "'" + code.ToString() + "'";
          }
          selectQuery += (!string.IsNullOrWhiteSpace(selectQuery) ? " AND " : "") + " final_edu_code_idx in {" + in_str + "} ";
        }

        if (!string.IsNullOrWhiteSpace(search.company))
        {
          string search_word = String.Empty;

          foreach (var word in search.company.Split(','))
          {
            search_word += (!string.IsNullOrWhiteSpace(search_word) ? opt.expr_and_or : "");
            if (new string[] { "&&", "||", "!!" }.Any(c => word.Contains(c)))
            {
              if (search.final_company)
                search_word += " (" + getBooleanSearchParse("final_company_idx", word.Trim().ToLower(), opt.expr) + ") ";
              else
                search_word += " (" + getBooleanSearchParse("company_idx", word.Trim().ToLower(), opt.expr) + ") ";

              highlight_text += (string.IsNullOrEmpty(highlight_text) ? "" : " ") + word.Replace("&&", " ").Replace("||", " ").Replace("!!", " ").Replace(")", "").Replace("(", "");
            }
            else
            {
              if (opt.expr)
              {
                if (search.final_company)
                  search_word += " (final_company_idx = '" + word.Trim().ToLower() + "' allorderadjacent synonym OR final_company_idx like '*" + word.Trim().ToLower() + "*')";
                else
                  search_word += " (company_idx = '" + word.Trim().ToLower() + "' allorderadjacent synonym OR company_idx like '*" + word.Trim().ToLower() + "*')";

                syn_highlight_text.Add(word.Trim());
              }
              else
              {
                if (search.final_company)
                  search_word += " (final_company_idx = '\"" + word.Trim().ToLower() + "\"' allorder OR final_company_idx like '*" + word.Trim().ToLower() + "*')";
                else
                  search_word += " (company_idx = '\"" + word.Trim().ToLower() + "\"' allorder OR company_idx like '*" + word.Trim().ToLower() + "*')";

                highlight_text += (string.IsNullOrEmpty(highlight_text) ? "" : " ") + word.Trim();
              }
            }


          }

          selectQuery += (!string.IsNullOrWhiteSpace(selectQuery) ? " AND " : "") + "(" + search_word + ")";
        }

        if (!string.IsNullOrWhiteSpace(search.isWork))
          selectQuery += (!string.IsNullOrWhiteSpace(selectQuery) ? " AND " : "") + " is_work_idx = " + search.isWork;

        if (search.job.Count > 0)
        { /*
          if (search.job.First().code2.ToString().Length == 3)
          {
            string from_code1 = search.job.First().code2.ToString() + "000";
            string to_code1 = search.job.First().code2.ToString() + "999";
            selectQuery += (!string.IsNullOrWhiteSpace(selectQuery) ? " AND " : "") + "(jobCode_idx >= '" + from_code1 + "' and jobCode_idx <= '" + to_code1 + "') ";
          }
          else
          {*/
          string job_sql = String.Empty;


          foreach (var job in search.job)
          {
            if (job.code2.ToString().Length == 3)
            {
              job_sql += (!string.IsNullOrWhiteSpace(job_sql) ? " " + search.job_search_type + " " : "") + " (jobCode_idx >= '" + job.code2 + "000' and jobCode_idx <= '" + job.code2 + "999') ";
            }
            else
            {
              job_sql += (!string.IsNullOrWhiteSpace(job_sql) ? " " + search.job_search_type + " " : "") + " jobCode_idx = '" + job.code2 + "' ";
            }
          }
          if (!string.IsNullOrEmpty(job_sql))
          {
            selectQuery += (!string.IsNullOrWhiteSpace(selectQuery) ? " AND " : "") + "(" + job_sql + ")";
          }
          /*
        }*/

        }
        if (search.business.Count > 0)
        { /*
          if (search.business.First().code2.ToString().Length == 3)
          {
            string from_code1 = search.business.First().code2.ToString() + "000";
            string to_code1 = search.business.First().code2.ToString() + "999";
            selectQuery += (!string.IsNullOrWhiteSpace(selectQuery) ? " AND " : "") + "(busiCode_idx >= '" + from_code1 + "' and busiCode_idx <= '" + to_code1 + "') ";
          }
          else
          {*/
          string business_sql = String.Empty;

          foreach (var business in search.business)
          {
            if (business.code2.ToString().Length == 3)
            {
              business_sql += (!string.IsNullOrWhiteSpace(business_sql) ? " " + search.business_search_type + " " : "") + " (busiCode_idx >= '" + business.code2 + "000' and busiCode_idx <= '" + business.code2 + "999') ";
            }
            else
            {
              business_sql += (!string.IsNullOrWhiteSpace(business_sql) ? " " + search.business_search_type + " " : "") + " busiCode_idx = '" + business.code2 + "' ";
            }
          }
          if (!string.IsNullOrEmpty(business_sql))
          {
            selectQuery += (!string.IsNullOrWhiteSpace(selectQuery) ? " AND " : "") + "(" + business_sql + ")";
          }
          /*}*/

        }

        if (!string.IsNullOrWhiteSpace(search.keyword))
        {
          string search_word = String.Empty;

          foreach (var word in search.keyword.Split(','))
          {
            search_word += (!string.IsNullOrWhiteSpace(search_word) ? opt.keyword_and_or : "");
            if (new string[] { "&&", "||", "!!" }.Any(c => word.Contains(c)))
            {
              search_word += " (" + getBooleanSearchParse("keyword_idx", word.Trim().ToLower(), opt.keyword) + ") ";

              highlight_text += (string.IsNullOrEmpty(highlight_text) ? "" : " ") + word.Replace("&&", " ").Replace("||", " ").Replace("!!", " ").Replace(")", "").Replace("(", "");
            }
            else
            {
              if (opt.keyword)
              {
                search_word += " (keyword_idx = '" + word.Trim().ToLower() + "' allorderadjacent synonym OR keyword_idx like '*" + word.Trim().ToLower() + "*') ";
                syn_highlight_text.Add(word.Trim());
              }
              else
              {
                search_word += " (keyword_idx = '\"" + word.Trim().ToLower() + "\"' allorder OR keyword_idx like '*" + word.Trim().ToLower() + "*') ";
                highlight_text += (string.IsNullOrEmpty(highlight_text) ? "" : " ") + word.Trim();
              }
            }



          }

          selectQuery += (!string.IsNullOrWhiteSpace(selectQuery) ? " AND " : "") + "(" + search_word + ")";
        }


        if (!string.IsNullOrWhiteSpace(search.language) && search.ability > 0)
          selectQuery += (!string.IsNullOrWhiteSpace(selectQuery) ? " AND " : "") + " language_idx like '*" + search.language + "/" + search.ability.ToString() + "*'  ";

        if (!string.IsNullOrWhiteSpace(search.create_dt_start))
          selectQuery += (!string.IsNullOrWhiteSpace(selectQuery) ? " AND " : "") + " create_dt_idx >= '" + search.create_dt_start.Replace("-", "") + "000000'";
        if (!string.IsNullOrWhiteSpace(search.create_dt_end))
          selectQuery += (!string.IsNullOrWhiteSpace(selectQuery) ? " AND " : "") + " create_dt_idx <= '" + search.create_dt_end.Replace("-", "") + "235959'";

        if (search.confidential >= 0)
          selectQuery += (!string.IsNullOrWhiteSpace(selectQuery) ? " AND " : "") + " is_confidential_idx = " + search.confidential.ToString();

        if (search.inactive >= 0)
          selectQuery += (!string.IsNullOrWhiteSpace(selectQuery) ? " AND " : "") + " is_inactive_idx = " + search.inactive.ToString();


        if (!string.IsNullOrWhiteSpace(search.memo))
        {
          string search_word = String.Empty;

          foreach (var word in search.memo.Split(','))
          {
            search_word += (!string.IsNullOrWhiteSpace(search_word) ? opt.memo_and_or : "");
            if (new string[] { "&&", "||", "!!" }.Any(c => word.Contains(c)))
            {
              search_word += " (" + getBooleanSearchParse("all_activity_idx", word.Trim().ToLower(), opt.memo) + ") ";

              highlight_text += (string.IsNullOrEmpty(highlight_text) ? "" : " ") + word.Replace("&&", " ").Replace("||", " ").Replace("!!", " ").Replace(")", "").Replace("(", "");
            }
            else
            {
              if (opt.memo)
              {
                search_word += " (all_activity_idx = '" + word.Trim().ToLower() + "' allorderadjacent synonym OR all_activity_idx = '*" + word.Trim().ToLower() + "*')";
                syn_highlight_text.Add(word.Trim());
              }
              else
              {
                search_word += " (all_activity_idx = '\"" + word.Trim().ToLower() + "\"' allorder OR all_activity_idx = '*" + word.Trim().ToLower() + "*')";
                highlight_text += (string.IsNullOrEmpty(highlight_text) ? "" : " ") + word.Trim();
              }
            }


          }

          selectQuery += (!string.IsNullOrWhiteSpace(selectQuery) ? " AND " : "") + "(" + search_word + ")";
        }

        if (!string.IsNullOrWhiteSpace(search.modify_dt_start))
          selectQuery += (!string.IsNullOrWhiteSpace(selectQuery) ? " AND " : "") + " modify_dt_idx >= '" + search.modify_dt_start.Replace("-", "") + "000000'";
        if (!string.IsNullOrWhiteSpace(search.modify_dt_end))
          selectQuery += (!string.IsNullOrWhiteSpace(selectQuery) ? " AND " : "") + " modify_dt_idx <= '" + search.modify_dt_end.Replace("-", "") + "235959'";



        if (search.except_c_seq.Count > 0)
        {
          string c_in_str = String.Empty;
          foreach (var code in search.except_c_seq)
          {
            c_in_str += (!String.IsNullOrEmpty(c_in_str) ? ", " : "") + "'" + code.ToString() + "'";
          }
          selectQuery += (!string.IsNullOrWhiteSpace(selectQuery) ? " AND " : "") + " c_seq_idx not in {" + c_in_str + "} ";
        }


        if (search.career_start > 0)
        {
          var career_start_dt = DateTime.Today.AddMonths((search.career_start * -1)).ToString("yyyyMMddHHmmss");
          selectQuery += (!string.IsNullOrWhiteSpace(selectQuery) ? " AND " : "") + " ((career_start_idx <= '" + career_start_dt + "' ";
          selectQuery += " AND career_end_idx = 'X' ) ";
          selectQuery += " OR career_range_idx >= " + search.career_start + " ) ";
        }

        if (search.career_end > 0)
        {
          var career_end_dt = DateTime.Today.AddMonths((search.career_end * -1)).ToString("yyyyMMddHHmmss");
          selectQuery += (!string.IsNullOrWhiteSpace(selectQuery) ? " AND " : "") + " ((career_start_idx >= '" + career_end_dt + "' ";
          selectQuery += " AND career_end_idx = 'X' ) ";
          selectQuery += " OR (career_range_idx <= " + search.career_end + " AND career_range_idx > 0 )) ";
        }

        if (!string.IsNullOrWhiteSpace(search.reg_type))
        {
          selectQuery += (!string.IsNullOrWhiteSpace(selectQuery) ? " AND " : "") + " reg_type_idx = " + search.reg_type;
        }





        //selectQuery += (!string.IsNullOrWhiteSpace(selectQuery) ? ";" : "") + search.total;


        //ksearch.GetSynonymList

        int ret = 0;
        object rows = null, total = null, arrdata = null, arrsize = null;

        List<candidate> list = new List<candidate>();

        if (ksearch.BeginSession() < 0)
          throw new Exception(ksearch.GetErrorMessage());
        ksearch.SetOption(ksearch.OPTION_REQUEST_CHARSET_UTF8, 1);
        /*
        if (syn_highlight_text.Count > 0)
        {
          try
          {
            foreach(var highlist_word in syn_highlight_text) {
              int syn_ret = 0;
              string syn_res_text = String.Empty;
              object TermCnt = null, SynCnt = null, arrSynList = null;
              syn_ret = ksearch.GetSynonymList(serverAddr + ":" + serverPort.ToString(), out TermCnt, out SynCnt, out arrSynList, 5, highlist_word, 1, 1, 1, 5, 5, 0);
              if (syn_ret >= 0)
              {
                object[,] s = (object[,])arrSynList;
                object[] arrsynCnt = (object[])SynCnt;

                for (int i = 0; i < (int)TermCnt; i++)
                {
                  for (int j = 0; j < (int)arrsynCnt[i]; j++)
                  {
                    if (!String.IsNullOrEmpty(Convert.ToString(s[i, j])))
                      highlight_text += (string.IsNullOrEmpty(highlight_text) ? "" : " ") + Convert.ToString(s[i, j]);
                  }

                }

              }
            }
          }
          catch(Exception e)
          {
            throw e;
          }

        }

        */
        if (syn_highlight_text.Count > 0)
        {
          foreach (var highlist_word in syn_highlight_text)
          {
            highlight_text += (string.IsNullOrEmpty(highlight_text) ? "" : " ") + "\"" + highlist_word + "\"";
          }
        }

        highlight_text = highlight_text.Replace("(", " ").Replace(")", " ").Replace("\"", "");

        string search_log = "TOTAL@candidate+";
        if (search.is_sugg == 1)
        {
          search_log = "TOTAL@cand_sugg+";
        }
        ret = ksearch.SubmitQuery_UTF8(serverAddr, serverPort, "", search_log + user_name + "$|첫검색|" + skip.ToString() + "|" + highlight_text, "scn_candidate", selectQuery, order, highlight_text, (skip - 1) * count, count, ksearch.LC_KOREAN, ksearch.CS_UTF8);

        if (ret < 0)
          throw new Exception(ksearch.GetErrorMessage());
        if (ksearch.GetResult_RowSize(out rows) < 0)
          throw new Exception(ksearch.GetErrorMessage());
        if (ksearch.GetResult_TotalCount(out total) < 0)
          throw new Exception(ksearch.GetErrorMessage());

        totalCount = (int)total;

        for (int i = 0; i < (int)rows; i++)
        {
          ret = ksearch.GetResult_Row_UTF8(out arrdata, out arrsize, i);
          if (ret < 0)
            throw new Exception(ksearch.GetErrorMessage());

          object[] arrdata_tmp = (object[])arrdata;

          candidate cd = new candidate();


          cd.c_seq = Convert.ToInt32(arrdata_tmp[0]);
          cd.kor_name = Convert.ToString(arrdata_tmp[1]);
          cd.eng_name = Convert.ToString(arrdata_tmp[2]);
          cd.is_foreign = Convert.ToDouble(arrdata_tmp[3]);
          //cd.birth_date = Convert.ToDateTime(arrdata_tmp[4]);
          if (!String.IsNullOrEmpty(Convert.ToString(arrdata_tmp[4])))
          {
            cd.birth_date = Convert.ToDateTime(arrdata_tmp[4]);
          }
          cd.ex_birthdate = Convert.ToDouble(arrdata_tmp[5]);
          cd.gender = Convert.ToDouble(arrdata_tmp[6]);
          //cd.create_dt = Convert.ToDateTime(arrdata_tmp[7]);
          if (!String.IsNullOrEmpty(Convert.ToString(arrdata_tmp[7])))
          {
            cd.create_dt = Convert.ToDateTime(arrdata_tmp[7]);
          }
          cd.email1 = Convert.ToString(arrdata_tmp[8]);
          cd.email2 = Convert.ToString(arrdata_tmp[9]);
          cd.phone = Convert.ToString(arrdata_tmp[10]);
          cd.wrong_phone = Convert.ToInt32(arrdata_tmp[11]);
          cd.cell_phone = Convert.ToString(arrdata_tmp[12]);
          cd.wrong_phone2 = Convert.ToInt32(arrdata_tmp[13]);
          cd.is_work = Convert.ToInt32(arrdata_tmp[14]);
          cd.company_name = Convert.ToString(arrdata_tmp[15]);
          cd.school_name = Convert.ToString(arrdata_tmp[16]);
          cd.category1_name = Convert.ToString(arrdata_tmp[17]);
          cd.busiName = Convert.ToString(arrdata_tmp[18]);
          cd.jobName = Convert.ToString(arrdata_tmp[19]);
          cd.keyword = Convert.ToString(arrdata_tmp[20]);
          cd.kor_keyword = Convert.ToString(arrdata_tmp[21]);
          cd.eng_keyword = Convert.ToString(arrdata_tmp[22]);
          cd.kor_file_dir = Convert.ToString(arrdata_tmp[23]);
          cd.kor_file_ext = Convert.ToString(arrdata_tmp[24]);
          cd.eng_file_dir = Convert.ToString(arrdata_tmp[25]);
          cd.eng_file_ext = Convert.ToString(arrdata_tmp[26]);
          cd.sns_link1 = Convert.ToString(arrdata_tmp[27]);
          cd.sns_link2 = Convert.ToString(arrdata_tmp[28]);
          cd.sns_link3 = Convert.ToString(arrdata_tmp[29]);
          cd.is_confidential = Convert.ToInt32(arrdata_tmp[30]);
          cd.is_inactive = Convert.ToInt32(arrdata_tmp[31]);
          if (!String.IsNullOrEmpty(Convert.ToString(arrdata_tmp[32])))
          {
            cd.modify_dt = Convert.ToDateTime(arrdata_tmp[32]);
          }
          cd.all_company = Convert.ToString(arrdata_tmp[33]);

          if (Convert.ToString(arrdata_tmp[34]).Contains("<span class='bg-warning'>"))
          {
            cd.kor_resume_body = Convert.ToString(arrdata_tmp[34]);
          }

          if (Convert.ToString(arrdata_tmp[35]).Contains("<span class='bg-warning'>"))
          {
            cd.eng_resume_body = Convert.ToString(arrdata_tmp[35]);
          }
          cd.major_name = Convert.ToString(arrdata_tmp[36]);

          if (Convert.ToString(arrdata_tmp[37]).Contains("<span class='bg-warning'>"))
          {
            cd.memo_str = Convert.ToString(arrdata_tmp[37]);
          }
          if (!String.IsNullOrEmpty(Convert.ToString(arrdata_tmp[38])))
            cd.caution_type = Convert.ToInt32(arrdata_tmp[38]);

          cd.kor_cr_seq = Convert.ToInt32(arrdata_tmp[39]);
          cd.eng_cr_seq = Convert.ToInt32(arrdata_tmp[41]);

          cd.all_school = Convert.ToString(arrdata_tmp[43]);

          if (!String.IsNullOrEmpty(Convert.ToString(arrdata_tmp[44])))
          {
            cd.career_start_dt = Convert.ToDateTime(arrdata_tmp[44]);
          }
          if (!String.IsNullOrEmpty(Convert.ToString(arrdata_tmp[45])) && Convert.ToString(arrdata_tmp[45]) != "X")
          {
            cd.career_end_dt = Convert.ToDateTime(arrdata_tmp[45]);
          }
          if (!String.IsNullOrEmpty(Convert.ToString(arrdata_tmp[46])))
          {
            cd.career_range = Convert.ToInt32(arrdata_tmp[46]);
          }

          if (cd.career_range.HasValue && cd.career_range > 0)
          {
            cd.cur_career_month = cd.career_range.Value;
          }
          else if (cd.career_start_dt.HasValue)
          {
            DateTime career_end = cd.career_end_dt.HasValue ? cd.career_end_dt.Value : DateTime.Today;

            cd.cur_career_month = ((career_end.Year - cd.career_start_dt.Value.Year) * 12) + career_end.Month - cd.career_start_dt.Value.Month;

          }

          list.Add(cd);

        }

        ksearch.EndSession();
        return list;
      }
      catch (Exception e)
      {
        ksearch.EndSession();
        throw e;
      }
    }

    public candidate SelectCandidateOne(int c_seq)
    {
      ATLKSearch.RCW.Client ksearch = new ATLKSearch.RCW.Client();
      try
      {
        string selectQuery = String.Empty;
        string highlight_text = String.Empty;
        string order = String.Empty;

        if (c_seq > 0)
        {
          selectQuery += " c_seq_idx = '" + c_seq.ToString() + "' ";

        }

        int ret = 0;
        object rows = null, total = null, arrdata = null, arrsize = null;



        if (ksearch.BeginSession() < 0)
          throw new Exception(ksearch.GetErrorMessage());
        ksearch.SetOption(ksearch.OPTION_REQUEST_CHARSET_UTF8, 1);

        ret = ksearch.SubmitQuery_UTF8(serverAddr, serverPort, "", "candidate", "scn_candidate", selectQuery, order, highlight_text, 0, 1, ksearch.LC_KOREAN, ksearch.CS_UTF8);

        if (ret < 0)
          throw new Exception(ksearch.GetErrorMessage());
        if (ksearch.GetResult_RowSize(out rows) < 0)
          throw new Exception(ksearch.GetErrorMessage());
        if (ksearch.GetResult_TotalCount(out total) < 0)
          throw new Exception(ksearch.GetErrorMessage());


        candidate cd = new candidate();

        if (0 < (int)rows)
        {
          ret = ksearch.GetResult_Row_UTF8(out arrdata, out arrsize, 0);
          if (ret < 0)
            throw new Exception(ksearch.GetErrorMessage());

          object[] arrdata_tmp = (object[])arrdata;




          cd.c_seq = Convert.ToInt32(arrdata_tmp[0]);
          cd.kor_name = Convert.ToString(arrdata_tmp[1]);
          cd.eng_name = Convert.ToString(arrdata_tmp[2]);
          cd.is_foreign = Convert.ToDouble(arrdata_tmp[3]);
          //cd.birth_date = Convert.ToDateTime(arrdata_tmp[4]);
          if (!String.IsNullOrEmpty(Convert.ToString(arrdata_tmp[4])))
          {
            cd.birth_date = Convert.ToDateTime(arrdata_tmp[4]);
          }
          cd.ex_birthdate = Convert.ToDouble(arrdata_tmp[5]);
          cd.gender = Convert.ToDouble(arrdata_tmp[6]);
          //cd.create_dt = Convert.ToDateTime(arrdata_tmp[7]);
          if (!String.IsNullOrEmpty(Convert.ToString(arrdata_tmp[7])))
          {
            cd.create_dt = Convert.ToDateTime(arrdata_tmp[7]);
          }
          cd.email1 = Convert.ToString(arrdata_tmp[8]);
          cd.email2 = Convert.ToString(arrdata_tmp[9]);
          cd.phone = Convert.ToString(arrdata_tmp[10]);
          cd.wrong_phone = Convert.ToInt32(arrdata_tmp[11]);
          cd.cell_phone = Convert.ToString(arrdata_tmp[12]);
          cd.wrong_phone2 = Convert.ToInt32(arrdata_tmp[13]);
          cd.is_work = Convert.ToInt32(arrdata_tmp[14]);
          cd.company_name = Convert.ToString(arrdata_tmp[15]);
          cd.school_name = Convert.ToString(arrdata_tmp[16]);
          cd.category1_name = Convert.ToString(arrdata_tmp[17]);
          cd.busiName = Convert.ToString(arrdata_tmp[18]);
          cd.jobName = Convert.ToString(arrdata_tmp[19]);
          cd.keyword = Convert.ToString(arrdata_tmp[20]);
          cd.kor_keyword = Convert.ToString(arrdata_tmp[21]);
          cd.eng_keyword = Convert.ToString(arrdata_tmp[22]);
          cd.kor_file_dir = Convert.ToString(arrdata_tmp[23]);
          cd.kor_file_ext = Convert.ToString(arrdata_tmp[24]);
          cd.eng_file_dir = Convert.ToString(arrdata_tmp[25]);
          cd.eng_file_ext = Convert.ToString(arrdata_tmp[26]);
          cd.sns_link1 = Convert.ToString(arrdata_tmp[27]);
          cd.sns_link2 = Convert.ToString(arrdata_tmp[28]);
          cd.sns_link3 = Convert.ToString(arrdata_tmp[29]);
          cd.is_confidential = Convert.ToInt32(arrdata_tmp[30]);
          cd.is_inactive = Convert.ToInt32(arrdata_tmp[31]);
          if (!String.IsNullOrEmpty(Convert.ToString(arrdata_tmp[32])))
          {
            cd.modify_dt = Convert.ToDateTime(arrdata_tmp[32]);
          }
          cd.all_company = Convert.ToString(arrdata_tmp[33]);

          if (Convert.ToString(arrdata_tmp[34]).Contains("<span class='bg-warning'>"))
          {
            cd.kor_resume_body = Convert.ToString(arrdata_tmp[34]);
          }

          if (Convert.ToString(arrdata_tmp[35]).Contains("<span class='bg-warning'>"))
          {
            cd.eng_resume_body = Convert.ToString(arrdata_tmp[35]);
          }
          cd.major_name = Convert.ToString(arrdata_tmp[36]);

          if (Convert.ToString(arrdata_tmp[37]).Contains("<span class='bg-warning'>"))
          {
            cd.memo_str = Convert.ToString(arrdata_tmp[37]);
          }
          if (!String.IsNullOrEmpty(Convert.ToString(arrdata_tmp[38])))
            cd.caution_type = Convert.ToInt32(arrdata_tmp[38]);

          cd.kor_cr_seq = Convert.ToInt32(arrdata_tmp[39]);
          //40 : 국문 파일명
          cd.eng_cr_seq = Convert.ToInt32(arrdata_tmp[41]);
          //42 : 영문 파일명
          cd.all_school = Convert.ToString(arrdata_tmp[43]);

          if (!String.IsNullOrEmpty(Convert.ToString(arrdata_tmp[44])))
          {
            cd.career_start_dt = Convert.ToDateTime(arrdata_tmp[44]);
          }
          if (!String.IsNullOrEmpty(Convert.ToString(arrdata_tmp[45])) && Convert.ToString(arrdata_tmp[45]) != "X")
          {
            cd.career_end_dt = Convert.ToDateTime(arrdata_tmp[45]);
          }
          if (!String.IsNullOrEmpty(Convert.ToString(arrdata_tmp[46])))
          {
            cd.career_range = Convert.ToInt32(arrdata_tmp[46]);
          }

          if (cd.career_range.HasValue && cd.career_range > 0)
          {
            cd.cur_career_month = cd.career_range.Value;
          }
          else if (cd.career_start_dt.HasValue)
          {
            DateTime career_end = cd.career_end_dt.HasValue ? cd.career_end_dt.Value : DateTime.Today;

            cd.cur_career_month = ((career_end.Year - cd.career_start_dt.Value.Year) * 12) + career_end.Month - cd.career_start_dt.Value.Month;

          }


        }
        else
        {
          cd = null;
        }

        ksearch.EndSession();
        return cd;
      }
      catch (Exception e)
      {
        ksearch.EndSession();
        throw e;
      }
    }


    public r_can_resume SelectCandidateResumeOnly(int c_seq)
    {
      ATLKSearch.RCW.Client ksearch = new ATLKSearch.RCW.Client();
      try
      {
        string selectQuery = String.Empty;
        string highlight_text = String.Empty;
        string order = String.Empty;

        if (c_seq > 0)
        {
          selectQuery += " c_seq_idx = '" + c_seq.ToString() + "' ";

        }

        int ret = 0;
        object rows = null, total = null, arrdata = null, arrsize = null;



        if (ksearch.BeginSession() < 0)
          throw new Exception(ksearch.GetErrorMessage());
        ksearch.SetOption(ksearch.OPTION_REQUEST_CHARSET_UTF8, 1);

        ret = ksearch.SubmitQuery_UTF8(serverAddr, serverPort, "", "candidate", "scn_candidate_resume", selectQuery, order, highlight_text, 0, 1, ksearch.LC_KOREAN, ksearch.CS_UTF8);

        if (ret < 0)
          throw new Exception(ksearch.GetErrorMessage());
        if (ksearch.GetResult_RowSize(out rows) < 0)
          throw new Exception(ksearch.GetErrorMessage());
        if (ksearch.GetResult_TotalCount(out total) < 0)
          throw new Exception(ksearch.GetErrorMessage());


        r_can_resume cd = new r_can_resume();

        if (0 < (int)rows)
        {
          ret = ksearch.GetResult_Row_UTF8(out arrdata, out arrsize, 0);
          if (ret < 0)
            throw new Exception(ksearch.GetErrorMessage());

          object[] arrdata_tmp = (object[])arrdata;




          cd.c_seq = Convert.ToInt32(arrdata_tmp[0]);
          if (!String.IsNullOrEmpty(Convert.ToString(arrdata_tmp[1])))
            cd.kor_resume_body = Convert.ToString(arrdata_tmp[1]);
          if (!String.IsNullOrEmpty(Convert.ToString(arrdata_tmp[2])))
            cd.eng_resume_body = Convert.ToString(arrdata_tmp[2]);
        }
        else
        {
          cd = null;
        }

        ksearch.EndSession();
        return cd;
      }
      catch (Exception e)
      {
        ksearch.EndSession();
        throw e;
      }
    }


    public async Task<PgList<project>> SelectProjectList(ProjectSearchModel search, int skip, int count, string user_name)
    {
      ATLKSearch.RCW.Client ksearch = new ATLKSearch.RCW.Client();
      try
      {
        string selectQuery = String.Empty;
        string selectQuery_state = String.Empty;
        string highlight_text = String.Empty;
        string order = String.Empty;
        int totalCount = 0;

        if (!String.IsNullOrEmpty(search.orderOption1) && !String.IsNullOrEmpty(search.orderTxt1))
        {
          order = " order by " + search.orderTxt1 + " " + search.orderOption1;
        }
        else
        {
          order = " order by create_dt_idx DESC, p_seq_idx DESC";
        }

        if (!string.IsNullOrWhiteSpace(search.total))
        {
          if (new string[] { "&&", "||", "!!" }.Any(c => search.total.Contains(c)))
          {
            //selectQuery += (!string.IsNullOrWhiteSpace(selectQuery) ? " AND " : "") + " primary_idx = '" + search.total.Replace("&&", "&").Replace("||", "|").Replace("!!", "!") + "' boolean";

            selectQuery += (!string.IsNullOrWhiteSpace(selectQuery) ? " AND " : "") + "(" + getBooleanSearchParse("primary_idx", search.total) + " " + getBooleanSearchParse("primary_text_idx", search.total) + ")";
          }
          else
          {
            selectQuery += (!string.IsNullOrWhiteSpace(selectQuery) ? " AND " : "") + " ( primary_idx = '" + search.total + "' allwordthruindex synonym ";
            selectQuery += " OR primary_idx like '*" + search.total + "*')";
          }
          highlight_text += (string.IsNullOrEmpty(highlight_text) ? "" : " ") + search.total;
          //order = " order by $RELEVANCE desc, $ROWID DESC";
        }


        if (search.pjt_type != 0)
          selectQuery += (!string.IsNullOrWhiteSpace(selectQuery) ? " AND " : "") + " pjt_type_idx = " + search.pjt_type;

        if (!string.IsNullOrWhiteSpace(search.client))
        {

          selectQuery += (!string.IsNullOrWhiteSpace(selectQuery) ? " AND " : "") + "( client_idx = '" + search.client.Replace("\"", "") + "' allwordthruindex ";
          selectQuery += " OR client_idx like '*" + search.client + "*') ";


          highlight_text += (string.IsNullOrEmpty(highlight_text) ? "" : " ") + search.client;
        }

        if (!string.IsNullOrWhiteSpace(search.title))
        {
          //if (new string[] { "\"" }.Any(c => search.title.Contains(c)))
          //{
          selectQuery += (!string.IsNullOrWhiteSpace(selectQuery) ? " AND " : "") + "( title_idx = '" + search.title.Replace("\"", "") + "' allword synonym ";
          selectQuery += " OR title_idx like '*" + search.title + "*') ";
          //}
          //else
          //{
          //  selectQuery += (!string.IsNullOrWhiteSpace(selectQuery) ? " AND " : "") + " title_idx like '*" + search.title + "*' ";
          //}

          highlight_text += (string.IsNullOrEmpty(highlight_text) ? "" : " ") + search.title;
        }

        if (!string.IsNullOrWhiteSpace(search.user_name))
        {
          if (new string[] { "\"" }.Any(c => search.user_name.Contains(c)))
          {
            selectQuery += (!string.IsNullOrWhiteSpace(selectQuery) ? " AND " : "") + "all_user_idx = '" + search.user_name.Replace("\"", "") + "' ";
          }
          else
          {
            selectQuery += (!string.IsNullOrWhiteSpace(selectQuery) ? " AND " : "") + " all_user_idx like '*" + search.user_name + "*' ";
          }

          highlight_text += (string.IsNullOrEmpty(highlight_text) ? "" : " ") + search.user_name;
        }


        if (search.job.Count > 0)
        {
          /*if (search.job.First().code2.ToString().Length == 3)
          {
            string from_code1 = search.job.First().code2.ToString() + "000";
            string to_code1 = search.job.First().code2.ToString() + "999";
            selectQuery += (!string.IsNullOrWhiteSpace(selectQuery) ? " AND " : "") + "(job_idx >= '" + from_code1 + "' and job_idx <= '" + to_code1 + "') ";
          }
          else
          {*/
          string job_sql = String.Empty;

          foreach (var job in search.job)
          {
            if (job.code2.ToString().Length == 3)
            {
              job_sql += (!string.IsNullOrWhiteSpace(job_sql) ? " OR " : "") + " (job_idx >= '" + job.code2 + "000' and job_idx <= '" + job.code2 + "999') ";
            }
            else
            {
              job_sql += (!string.IsNullOrWhiteSpace(job_sql) ? " OR " : "") + " job_idx = '" + job.code2 + "' ";
            }

          }
          if (!string.IsNullOrEmpty(job_sql))
          {
            selectQuery += (!string.IsNullOrWhiteSpace(selectQuery) ? " AND " : "") + "(" + job_sql + ")";
          }
          /*}*/

        }
        if (search.business.Count > 0)
        { /*
          if (search.business.First().code2.ToString().Length == 3)
          {
            string from_code1 = search.business.First().code2.ToString() + "000";
            string to_code1 = search.business.First().code2.ToString() + "999";
            selectQuery += (!string.IsNullOrWhiteSpace(selectQuery) ? " AND " : "") + "(business_idx >= '" + from_code1 + "' and business_idx <= '" + to_code1 + "') ";
          }
          else
          {*/

          string business_sql = String.Empty;

          foreach (var business in search.business)
          {
            if (business.code2.ToString().Length == 3)
            {
              business_sql += (!string.IsNullOrWhiteSpace(business_sql) ? " OR " : "") + " (business_idx >= '" + business.code2 + "000' and business_idx <= '" + business.code2 + "999') ";
            }
            else
            {
              business_sql += (!string.IsNullOrWhiteSpace(business_sql) ? " OR " : "") + " business_idx = '" + business.code2 + "' ";
            }

          }
          if (!string.IsNullOrEmpty(business_sql))
          {
            selectQuery += (!string.IsNullOrWhiteSpace(selectQuery) ? " AND " : "") + "(" + business_sql + ")";
          }

          /*}*/

        }


        if (!string.IsNullOrWhiteSpace(search.dt_start))
          selectQuery += (!string.IsNullOrWhiteSpace(selectQuery) ? " AND " : "") + " " + search.range_field + " >= '" + search.dt_start.Replace("-", "") + "000000'";
        if (!string.IsNullOrWhiteSpace(search.dt_end))
          selectQuery += (!string.IsNullOrWhiteSpace(selectQuery) ? " AND " : "") + " " + search.range_field + " <= '" + search.dt_end.Replace("-", "") + "235959'";





        if (search.position_start > 0)
        {
          selectQuery += (!string.IsNullOrWhiteSpace(selectQuery) ? " AND " : "") + " position_idx <= '" + search.position_start + "' ";
        }

        if (search.position_end > 0)
        {

          selectQuery += (!string.IsNullOrWhiteSpace(selectQuery) ? " AND " : "") + " position_idx >= '" + search.position_end + "' ";
        }

        if (search.is_pe == 0 || search.is_pe == 1)
        {

          selectQuery += (!string.IsNullOrWhiteSpace(selectQuery) ? " AND " : "") + " pjt_pe_idx = '" + search.is_pe + "' ";
        }

        if (search.state > 0)
        {

          selectQuery_state += (!string.IsNullOrWhiteSpace(selectQuery) ? " AND " : "") + " pjt_status_idx = '" + search.state + "' ";
        }




        //selectQuery += (!string.IsNullOrWhiteSpace(selectQuery) ? ";" : "") + search.total;
        highlight_text = highlight_text.Replace("&&", " ").Replace("||", " ").Replace("!!", " ").Replace("(", " ").Replace(")", " ").Replace("\"", "");
        int ret = 0;
        object rows = null, total = null, arrdata = null, arrsize = null;

        List<project> list = new List<project>();

        if (ksearch.BeginSession() < 0)
          throw new Exception(ksearch.GetErrorMessage());
        ksearch.SetOption(ksearch.OPTION_REQUEST_CHARSET_UTF8, 1);

        string search_log = "TOTAL@project+";

        ret = ksearch.SubmitQuery_UTF8(serverAddr, serverPort, "", search_log + user_name + "$|첫검색|" + skip.ToString() + "|" + highlight_text, "scn_project", selectQuery + selectQuery_state, order, highlight_text, (skip - 1) * count, count, ksearch.LC_KOREAN, ksearch.CS_UTF8);

        if (ret < 0)
          throw new Exception(ksearch.GetErrorMessage());
        if (ksearch.GetResult_RowSize(out rows) < 0)
          throw new Exception(ksearch.GetErrorMessage());
        if (ksearch.GetResult_TotalCount(out total) < 0)
          throw new Exception(ksearch.GetErrorMessage());

        totalCount = (int)total;

        for (int i = 0; i < (int)rows; i++)
        {
          ret = ksearch.GetResult_Row_UTF8(out arrdata, out arrsize, i);
          if (ret < 0)
            throw new Exception(ksearch.GetErrorMessage());

          object[] arrdata_tmp = (object[])arrdata;

          project pjt = new project();


          pjt.p_seq = Convert.ToInt32(arrdata_tmp[0]);
          pjt.c_seq = Convert.ToInt32(arrdata_tmp[1]);
          pjt.pjt_type = Convert.ToInt32(arrdata_tmp[2]);
          pjt.pjt_type_str = Convert.ToString(arrdata_tmp[3]);
          pjt.pjt_status = Convert.ToInt32(arrdata_tmp[4]);
          pjt.pjt_status_str = Convert.ToString(arrdata_tmp[5]);

          pjt.company_name = Convert.ToString(arrdata_tmp[6]);
          pjt.title = Convert.ToString(arrdata_tmp[7]);
          pjt.am_names = Convert.ToString(arrdata_tmp[8]);
          pjt.searcher_names = Convert.ToString(arrdata_tmp[9]);

          pjt.business_names1 = Convert.ToString(arrdata_tmp[10]);
          pjt.business_names2 = Convert.ToString(arrdata_tmp[11]);
          pjt.sub_business_names1 = Convert.ToString(arrdata_tmp[12]);
          pjt.sub_business_names2 = Convert.ToString(arrdata_tmp[13]);

          pjt.job_names1 = Convert.ToString(arrdata_tmp[14]);
          pjt.job_names2 = Convert.ToString(arrdata_tmp[15]);
          pjt.sub_job_names1 = Convert.ToString(arrdata_tmp[16]);
          pjt.sub_job_names2 = Convert.ToString(arrdata_tmp[17]);

          if (!String.IsNullOrEmpty(Convert.ToString(arrdata_tmp[18])))
          {
            pjt.create_dt = Convert.ToDateTime(arrdata_tmp[18]);
          }

          if (!String.IsNullOrEmpty(Convert.ToString(arrdata_tmp[19])))
          {
            pjt.modify_dt = Convert.ToDateTime(arrdata_tmp[19]);
          }

          pjt.position_name = Convert.ToString(arrdata_tmp[20]);


          pjt.company_name_eng = Convert.ToString(arrdata_tmp[21]);
          pjt.exp_salary_won_str = Convert.ToString(arrdata_tmp[22]);
          pjt.hire_info = Convert.ToString(arrdata_tmp[23]);
          pjt.position_str = Convert.ToString(arrdata_tmp[24]);
          pjt.fee_rate_str = Convert.ToString(arrdata_tmp[25]);
          if (!String.IsNullOrEmpty(Convert.ToString(arrdata_tmp[26])))
          {
            pjt.open_dt = Convert.ToDateTime(arrdata_tmp[26]);
          }

          if (pjt.pjt_status > 1 && !String.IsNullOrEmpty(Convert.ToString(arrdata_tmp[27])))
          {
            pjt.close_dt = Convert.ToDateTime(arrdata_tmp[27]);
          }
          pjt.is_pe = Convert.ToInt32(arrdata_tmp[28]);

          list.Add(pjt);

        }

        //object group_count = null, group_key_count = null, group_key_val = null, group_size = null;
        List<PagingGroup> pgg_list = new List<PagingGroup>();
        object rowIds = null, scores = null;
        ret = ksearch.SubmitQuery_UTF8(serverAddr, serverPort, "", "", "scn_project", selectQuery, "GROUP BY pjt_status order by pjt_status ", highlight_text, 0, 10, ksearch.LC_KOREAN, ksearch.CS_UTF8);
        if (ret < 0)
          throw new Exception(ksearch.GetErrorMessage());
        if (ksearch.GetResult_RowSize(out rows) < 0)
          throw new Exception(ksearch.GetErrorMessage());
        if (ksearch.GetResult_TotalCount(out total) < 0)
          throw new Exception(ksearch.GetErrorMessage());
        if (ksearch.GetResult_ROWID(out rowIds, out scores) < 0)
          throw new Exception(ksearch.GetErrorMessage());

        object[] gc_tmp = (object[])scores;
        for (int i = 0; i < (int)rows; i++)
        {
          ret = ksearch.GetResult_Row_UTF8(out arrdata, out arrsize, i);
          if (ret < 0)
            throw new Exception(ksearch.GetErrorMessage());


          object[] arrdata_tmp = (object[])arrdata;
          PagingGroup pgg = new PagingGroup();


          pgg.code = Convert.ToInt32(arrdata_tmp[4]);
          //pgg.name = Convert.ToString(arrdata_tmp[5]);
          pgg.count = Convert.ToInt32(gc_tmp[i]);

          pgg_list.Add(pgg);
        }
        //int gc = Convert.ToInt32(gc_tmp[0]);// (int)group_count;

        ksearch.EndSession();

        return new PgList<project>()
        {
          totalCount = totalCount,
          group_count = pgg_list,
          Items = list
        };
      }
      catch (Exception e)
      {
        ksearch.EndSession();
        throw e;
      }
    }




  }
}
