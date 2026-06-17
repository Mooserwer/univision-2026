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
    public class AdminRepository : BaseRepository
    {
        //부서 팝업
        public async Task<List<uv_division>> SelectDivisionListAsync(int ud_seq)
        {
            try
            {
                List<uv_division> list = new List<uv_division>();

                using (IDbConnection con = new SqlConnection(base.BaseConnectionString))
                {
                    con.Open();

                    string selectQuery = @" 
SELECT *
FROM UV_DIVISION
WHERE 1 = 1";


                    if (ud_seq != 0)
                    {
                        selectQuery += @" AND ud_seq = @ud_seq ";
                    }
                    selectQuery += @" ORDER BY ud_seq ";
                    DynamicParameters param = new DynamicParameters();
                    param.Add("@ud_seq", ud_seq, DbType.Int32);

                    var ret = await con.QueryAsync<uv_division>(selectQuery, param);

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
        //권한 팝업
        public async Task<List<uv_auth>> SelectAuthListAsync(int ua_seq)
        {
            try
            {
                List<uv_auth> list = new List<uv_auth>();

                using (IDbConnection con = new SqlConnection(base.BaseConnectionString))
                {
                    con.Open();

                    string selectQuery = @" 
SELECT *
FROM UV_AUTH
WHERE 1 = 1";


                    if (ua_seq != 0)
                    {
                        selectQuery += @" AND ua_seq = @ua_seq ";
                    }
                    selectQuery += @" ORDER BY ua_seq ";
                    DynamicParameters param = new DynamicParameters();
                    param.Add("@ua_seq", ua_seq, DbType.Int32);

                    var ret = await con.QueryAsync<uv_auth>(selectQuery, param);

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

        //이메일 리스트 팝업
        public async Task<List<uv_user>> SelectUserMailListAsync(string search_str, string id_list, string seq_list)
        {
            try
            {
                List<uv_user> list = new List<uv_user>();

                using (IDbConnection con = new SqlConnection(base.BaseConnectionString))
                {
                    con.Open();

                    string selectQuery = @" 
SELECT uv_seq, user_id, name, email
FROM uv_user
WHERE retire_date is null 
AND ud_seq NOT IN (69)
AND isnull(email, '') <> '' ";


                    if (!String.IsNullOrWhiteSpace(search_str))
                    {
                        selectQuery += @" AND ( name like '%'+@search_str+'%' or email like '%'+@search_str+'%')";
                    }

                    string ids_str = "";
                    if (!String.IsNullOrWhiteSpace(id_list))
                    {
                        string[] ids = id_list.Split(',');

                        foreach(var id in ids)
                        {
                            if (!string.IsNullOrWhiteSpace(ids_str))
                                ids_str += ",";

                            ids_str += "'" + id + "'";
                        } 
                        selectQuery += string.Format(" AND (user_id in ({0}))", ids_str);
                    }

                    //string seqs_str = "";
                    if (!string.IsNullOrWhiteSpace(seq_list))
                    {
                        //string[] seqs = seq_list.Split(',');

                        //foreach (var seq in seqs)
                        //{
                        //    if (!String.IsNullOrWhiteSpace(seqs_str))
                        //        seqs_str += ",";
                        //    seqs_str += seq;
                        //}
                        selectQuery += string.Format(" AND (uv_seq in ({0}))", seq_list);
                    }

                    selectQuery += @" ORDER BY name ";
                    DynamicParameters param = new DynamicParameters();
                    param.Add("@search_str", search_str, DbType.String);
                    //param.Add("@ids_str", ids_str, DbType.String);
                    //param.Add("@seqs_str", seqs_str, DbType.String);

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
        /// 사용자 리스트
        /// </summary>
        /// <returns></returns>
        public List<uv_user> SelectUserList(int currentPage, int pageSize, int IsRetire, string SearchString, out int totalCount)
        {
            try
            {
                using (IDbConnection con = new SqlConnection(base.BaseConnectionString))
                {
                    uv_user a = new uv_user();
                    

                    con.Open();
                    DynamicParameters parameters = new DynamicParameters();
                    parameters.Add("@CurrentPage", (currentPage - 1) * pageSize, DbType.Int16, null);
                    parameters.Add("@PageSize", pageSize, DbType.Int16, null);
                    parameters.Add("@SearchString", SearchString, DbType.String, null);
                    parameters.Add("@IsRetire", IsRetire, DbType.String, null);

                    

                    //카운트와 리스트에 공용사용 될 테이블 쿼리 부분 정의
                    string base_query = @"
                        FROM
	                        UV_USER as uv
                        WHERE 1 = 1
                    ";

                    if (SearchString != "")
                    {
                        base_query += " and (uv.user_id like '%' + @SearchString + '%'";
                        base_query += " or uv.name like '%' + @SearchString + '%'";
                        base_query += " or uv.email like '%' + @SearchString + '%' ) ";
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
