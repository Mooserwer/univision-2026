using Dapper;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Univision.Core.Models.DTO;
using Univision.Core.Models.DTO.Excel;
using Univision.Core.Models.DTO.Response.Api;
using Univision.Core.Models.DTO.Response.Client;





namespace Univision.Core.Repositories
{
    public class MailReportRepository : BaseRepository
    {
        public string CreateReportData(string base_dt_str, int div_no, string user_id, int is_rebuild)
        {
            try
            {
                using (IDbConnection con = new SqlConnection(base.BaseConnectionString))
                {
                    con.Open();
                    DynamicParameters param = new DynamicParameters();
                    param.Add("@BASE_DT_STR",  base_dt_str, DbType.String);
                    param.Add("@ONLY_DIV_NO", div_no, DbType.Int64);
                    param.Add("@ONLY_USER_ID", user_id, DbType.String);
                    param.Add("@IS_REBUILD", is_rebuild, DbType.Int64);
                    param.Add("@RTN_CD", "", DbType.String, direction: ParameterDirection.Output);
                    param.Add("@RTN_MSG", "", DbType.String, direction: ParameterDirection.Output);

                    string query = @"nSP_CREATE_WEEKLY_MAIL";
                    SqlMapper.Execute(con, query, param, commandTimeout: 120, commandType: CommandType.StoredProcedure);

                    con.Close();

                    return param.Get<string>("RTN_CD"); ;
                }
            }
            catch (Exception e)
            {
                throw e;
            }

        }

        public List<mail_report_mst> SelectMailReportMstList(string base_date)
        {
            try
            {
                List<mail_report_mst> list = new List<mail_report_mst>();

                using (IDbConnection con = new SqlConnection(base.BaseConnectionString))
                {
                    con.Open();

                    string selectQuery = @" 
SELECT A.*, 
       B.email as to_addr,
       (SELECT TOP 1 email FROM uv_user X WHERE X.ud_seq = B.ud_seq AND X.ua_seq <= 4 ORDER BY ua_seq ASC) as cc_addr
FROM mail_report_mst A LEFT JOIN uv_user B
                       on A.uv_seq = B.uv_seq

WHERE A.base_dt = @base_date 
ORDER BY A.mr_idx
";

                    DynamicParameters param = new DynamicParameters();
                    param.Add("@base_date", base_date, DbType.String);

                    var ret = con.Query<mail_report_mst>(selectQuery, param);

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

        public List<mail_report_last_list> SelectMailReportLastList(int mr_idx)
        {
            try
            {
                List<mail_report_last_list> list = new List<mail_report_last_list>();

                using (IDbConnection con = new SqlConnection(base.BaseConnectionString))
                {
                    con.Open();

                    string selectQuery = @" 
SELECT *
FROM mail_report_last_list
WHERE mr_idx = @mr_idx
ORDER BY mrll_idx
";

                    DynamicParameters param = new DynamicParameters();
                    param.Add("@mr_idx", mr_idx, DbType.Int32);

                    var ret = con.Query<mail_report_last_list>(selectQuery, param);

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

        public List<mail_report_this_list> SelectMailReportThisList(int mr_idx)
        {
            try
            {
                List<mail_report_this_list> list = new List<mail_report_this_list>();

                using (IDbConnection con = new SqlConnection(base.BaseConnectionString))
                {
                    con.Open();

                    string selectQuery = @" 
SELECT *
FROM mail_report_this_list
WHERE mr_idx = @mr_idx
ORDER BY mrtl_idx
";

                    DynamicParameters param = new DynamicParameters();
                    param.Add("@mr_idx", mr_idx, DbType.Int32);

                    var ret = con.Query<mail_report_this_list>(selectQuery, param);

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

        public List<mail_report_pjt_list> SelectMailReportProjectList(int mr_idx)
        {
            try
            {
                List<mail_report_pjt_list> list = new List<mail_report_pjt_list>();

                using (IDbConnection con = new SqlConnection(base.BaseConnectionString))
                {
                    con.Open();

                    string selectQuery = @" 
SELECT *
FROM mail_report_pjt_list
WHERE mr_idx = @mr_idx
ORDER BY case pjt_status when 5 then 1 when 4 then 2 when 1 then 3 when 2 then 4 else 5 end, pjt_update DESC, mrpl_idx
";

                    DynamicParameters param = new DynamicParameters();
                    param.Add("@mr_idx", mr_idx, DbType.Int32);

                    var ret = con.Query<mail_report_pjt_list>(selectQuery, param);

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
