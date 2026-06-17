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
  public class BoardRepository : BaseRepository
  {

        public async Task<List<code_board_sub1>> SelectDocumentType1ListAsync(int type, int code1)
        {
            try
            {
                List<code_board_sub1> list = new List<code_board_sub1>();

                using (IDbConnection con = new SqlConnection(base.BaseConnectionString))
                {
                    con.Open();

                    string selectQuery = @" 
SELECT *,
       (SELECT COUNT(*) FROM BOARD WHERE b_type = cbt.type AND b_type_sub1 = cbt.code1) As sub1_doc_count
FROM CODE_BOARD_SUB1 As cbt
WHERE type = @type";


                    if (code1 != 0)
                    {
                        selectQuery += @" AND code1 = @code1 ";
                    }

                    selectQuery += @" ORDER BY order_no ";
                    DynamicParameters param = new DynamicParameters();
                    param.Add("@code1", code1, DbType.Int32);
                    param.Add("@type", type, DbType.Int32);

                    var ret = await con.QueryAsync<code_board_sub1>(selectQuery, param);

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
        public async Task<List<code_board_sub2>> SelectDocumentType2ListAsync(int type, int code1, int code2)
        {
            try
            {
                List<code_board_sub2> list = new List<code_board_sub2>();

                using (IDbConnection con = new SqlConnection(base.BaseConnectionString))
                {
                    con.Open();

                    string selectQuery = @" 
SELECT *,
       (SELECT COUNT(*) FROM BOARD WHERE b_type = cbt.type AND b_type_sub1 = cbt.code1 AND b_type_sub2 = cbt.code2) As sub2_doc_count
FROM CODE_BOARD_SUB2 As cbt
WHERE type  = @type 
AND   code1 = @code1 ";


                    if (code2 != 0)
                    {
                        selectQuery += @" AND code2 = @code2 ";
                    }
                    selectQuery += @" ORDER BY order_no ";
                    DynamicParameters param = new DynamicParameters();
                    param.Add("@type", type, DbType.Int32);
                    param.Add("@code1", code1, DbType.Int32);
                    param.Add("@code2", code2, DbType.Int32);

                    var ret = await con.QueryAsync<code_board_sub2>(selectQuery, param);

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
        /// 게시판 답글 리스트
        /// </summary>
        /// <param name="b_seq"></param>
        /// <returns></returns>
        public List<board_reply> SelectBoardReplyList(int b_seq, int skip, int count, out int totalCount)
        {
            try
            {
                List<board_reply> list = new List<board_reply>();

                using (IDbConnection con = new SqlConnection(base.BaseConnectionString))
                {
                    con.Open();

                    string selectQuery = @" SELECT br.
                                            FROM board_reply AS br LEFT JOIN uv_user AS uv
                                                                   ON br.create_user = uv.uv_seq
                                            WHERE br.b_seq = @b_seq 
                                            ORDER BY CA.ca_seq DESC  
                                            OFFSET @currentPage ROWS FETCH NEXT @pageSize ROWS ONLY ";

                    string countQuery = @" SELECT COUNT(br.br_seq)
                                           FROM board_reply AS br LEFT JOIN uv_user AS uv
                                                                   ON br.create_user = uv.uv_seq
                                           WHERE br.b_seq = @b_seq ";

                    DynamicParameters param = new DynamicParameters();
                    param.Add("@b_seq", b_seq, DbType.Int32);
                    param.Add("@currentPage", skip, DbType.Int32);
                    param.Add("@pageSize", count, DbType.Int32);

                    DynamicParameters param2 = new DynamicParameters();
                    param2.Add("@c_seq", b_seq, DbType.Int32);

                    var ret = con.Query<board_reply>(selectQuery, param);
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
        /// 게시판 파일 리스트
        /// </summary>
        /// <param name="b_seq"></param>
        /// <returns></returns>
        public async Task<List<board_file>> SelectBoardFileListAsync(int b_seq)
        {
            try
            {
                List<board_file> list = new List<board_file>();

                using (IDbConnection con = new SqlConnection(base.BaseConnectionString))
                {
                    con.Open();

                    string selectQuery = @" SELECT * FROM board_file WHERE b_seq = @b_seq ";

                    DynamicParameters param = new DynamicParameters();
                    param.Add("@b_seq", b_seq, DbType.Int32);

                    var ret = await con.QueryAsync<board_file>(selectQuery, param);

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

        public List<board_file> SelectBoardFileListAsync_Map(int b_seq)
        {
            try
            {
                List<board_file> list = new List<board_file>();

                using (IDbConnection con = new SqlConnection(base.BaseConnectionString))
                {
                    con.Open();

                    string selectQuery = @" SELECT * FROM board_file WHERE b_seq = @b_seq ";

                    DynamicParameters param = new DynamicParameters();
                    param.Add("@b_seq", b_seq, DbType.Int32);

                    //var ret = await con.QueryAsync<board_file>(selectQuery, param);
                    var ret = SqlMapper.Query<board_file>(con, selectQuery, param, commandType: CommandType.Text);

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

        public string SelectBoardFileInfo(int bf_seq)
        {
            try
            {
                using (IDbConnection con = new SqlConnection(base.BaseConnectionString))
                {
                    con.Open();

                    string selectQuery = @" SELECT file_origin_path FROM board_file WHERE bf_seq = @bf_seq ";

                    DynamicParameters param = new DynamicParameters();
                    param.Add("@bf_seq", bf_seq, DbType.Int32);

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
        /// 게시글 단건 셀렉트
        /// </summary>
        /// <param name="b_seq"></param>
        /// <returns></returns>
        public async Task<board> SelectBoardOneAsync(int b_seq, int uv_seq)
    {
      try
      {
        using (IDbConnection con = new SqlConnection(base.BaseConnectionString))
        {
          con.Open();

          string selectQuery = @" 
SELECT
    bd.*,
	isnull(us.name, '') As user_name,    
	(select count(*) from board_read_history where c_seq = bd.b_seq) As read_cnt,
	(select count(*) from board_reply where b_seq = bd.b_seq) As relpy_cnt,
    CASE WHEN bd.b_type_sub1 is not null and bd.b_type_sub1 <> 0 then
	    '[' + convert(varchar(3), bd.b_type_sub1) +'] ' + (select code_name from code_board_sub1 where type = bd.b_type AND code1 = bd.b_type_sub1) 
        +' > [' + convert(varchar(3), bd.b_type_sub1) + '-' + convert(varchar(3), bd.b_type_sub2) +'] ' + (select code_name from code_board_sub2 where type = bd.b_type AND code1 = bd.b_type_sub1 and code2 = bd.b_type_sub2) 
    ELSE '' END As b_type_sub_name,
    rh.read_dt as last_read_date
FROM
	BOARD As bd LEFT JOIN UV_USER As us
			    on bd.modify_user = us.uv_seq
                LEFT JOIN board_read_history as rh
                on bd.b_seq = rh.c_seq
                and rh.read_user = @read_user
WHERE bd.b_seq = @b_seq ";

          DynamicParameters param = new DynamicParameters();
          param.Add("@b_seq", b_seq, DbType.Int32);
          param.Add("@read_user", uv_seq, DbType.Int32);

          var ret = await con.QueryAsync<board>(selectQuery, param);

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
        /// 카테고리
        /// </summary>
        /// <param name="b_seq"></param>
        /// <returns></returns>
        public async Task<String> SelectDocCategoryAsOne(int b_type, int b_type_sub1, int b_type_sub2)
        {
            try
            {
                using (IDbConnection con = new SqlConnection(base.BaseConnectionString))
                {
                    con.Open();

                    string selectQuery = @" 
SELECT TOP 1 '[' + convert(varchar(3), A.code1) +'] ' + A.code_name  
        +' > [' + convert(varchar(3), B.code1) + '-' + convert(varchar(3), B.code2) +'] ' + B.code_name b_type_sub_name   
FROM code_board_sub1 As a LEFT JOIN code_board_sub2 As B
	                 	  on  A.type = B.type
                          AND A.code1 = B.code1
WHERE A.type = @type 
AND   A.code1 = @code1
AND   B.code2 = @code2 ";

                    DynamicParameters param = new DynamicParameters();
                    param.Add("@type", b_type, DbType.Int32);
                    param.Add("@code1", b_type_sub1, DbType.Int32);
                    param.Add("@code2", b_type_sub2, DbType.Int32);

                    var ret = await con.QueryAsync<String>(selectQuery, param);

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
        /// 게시판 리스트
        /// </summary>
        /// <param name="b_seq"></param>
        /// <returns></returns>
        public List<board> BoardList(int bType, int bTypeSub1, int bTypeSub2, int currentPage, int pageSize, string SelectOption, string SearchString, out int totalCount)
        {
            try
            {
                using (IDbConnection con = new SqlConnection(base.BaseConnectionString))
                {
                    con.Open();
                    DynamicParameters parameters = new DynamicParameters();
                    parameters.Add("@CurrentPage" , (currentPage - 1) * pageSize, DbType.Int16, null);
                    parameters.Add("@PageSize", pageSize, DbType.Int16, null);
                    parameters.Add("@SelectOption", SelectOption, DbType.String, null);
                    parameters.Add("@SearchString", SearchString, DbType.String, null);
                    parameters.Add("@bType", bType, DbType.Int16, null);
                    parameters.Add("@bTypeSub1", bTypeSub1, DbType.Int16, null);
                    parameters.Add("@bTypeSub2", bTypeSub2, DbType.Int16, null);

                    //카운트와 리스트에 공용사용 될 테이블 쿼리 부분 정의
                    string base_query = @"
                        FROM
	                        BOARD As bd LEFT JOIN UV_USER As us
					                        on bd.modify_user = us.uv_seq
                        WHERE bd.b_type = @bType
                        AND   bd.b_type_sub1 <> 999 
                    ";

                    if (bTypeSub1 != 0)
                    {
                        base_query += " AND   bd.b_type_sub1 = @bTypeSub1 ";
                    }
                    if (bTypeSub2 != 0)
                    {
                        base_query += " AND   bd.b_type_sub2 = @bTypeSub2 ";
                    }

                    if (SelectOption == "title" && !String.IsNullOrEmpty(SearchString))
                    {
                        base_query += " and bd.title like '%' + @SearchString + '%'";
                    }
                    else if (SelectOption == "contents" && !String.IsNullOrEmpty(SearchString))
                    {
                        base_query += " and bd.contents like '%' + @SearchString + '%'";
                    }
                    else if (SelectOption == "writer" && !String.IsNullOrEmpty(SearchString))
                    {
                        base_query += " and us.name like '%' + @SearchString + '%'";
                    }
                    else if (SelectOption == "b_top" && !String.IsNullOrEmpty(SearchString))
                    {
                        base_query += " and bd.b_top = @SearchString ";
                    }

                    // 리스트 부분에만 사용될 ORDER BY 절
                    string order_query = @" 
ORDER BY bd.modify_dt DESC 
OFFSET @currentPage ROWS FETCH NEXT @pageSize ROWS ONLY
";
                    

                    //리스트 부분에는 셀렉트 필드 + 공용쿼리(테이블&WHERE) + ORDER BY
                    string query = @"
SELECT
    bd.*,
	isnull(us.name, '') As user_name,    
	(select count(*) from board_read_history where c_seq = bd.b_seq) As read_cnt,
	(select count(*) from board_reply where b_seq = bd.b_seq) As relpy_cnt,
    CASE WHEN bd.b_type_sub1 is not null and bd.b_type_sub1 <> 0 then
	    (select code_name from code_board_sub1 where type = bd.b_type AND code1 = bd.b_type_sub1) 
        +' > ' + (select code_name from code_board_sub2 where type = bd.b_type AND code1 = bd.b_type_sub1 and code2 = bd.b_type_sub2) 
    ELSE '' END As b_type_sub_name
" + base_query + order_query;
                    
                    var ret = SqlMapper.Query<board>(con, query, parameters, commandType: CommandType.Text);
                    List<board> list = ret.ToList();

                    //카운트 부분에는 셀렉트 필드(COUNT) + 공용쿼리(테이블&WHERE)
                    query = @"
SELECT COUNT(0) 
" + base_query;

                    totalCount = SqlMapper.ExecuteScalar<int>(con, query, parameters, commandType: CommandType.Text);



                    con.Close();

                    if (list.Count() > 0)
                    {
                        foreach (var board in list)
                        {
                            board.file_list = SelectBoardFileListAsync_Map(board.b_seq);
                        }
                    }


                    return list;
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

    public List<board> AllBoardList(int currentPage, int pageSize, string SelectOption, string SearchString, out int totalCount)
    {
      try
      {
        using (IDbConnection con = new SqlConnection(base.BaseConnectionString))
        {
          con.Open();
          DynamicParameters parameters = new DynamicParameters();
          parameters.Add("@CurrentPage", (currentPage - 1) * pageSize, DbType.Int16, null);
          parameters.Add("@PageSize", pageSize, DbType.Int16, null);
          parameters.Add("@SelectOption", SelectOption, DbType.String, null);
          parameters.Add("@SearchString", SearchString, DbType.String, null);

          //카운트와 리스트에 공용사용 될 테이블 쿼리 부분 정의
          string base_query = @"
                        FROM
	                        BOARD As bd LEFT JOIN UV_USER As us
					                        on bd.modify_user = us.uv_seq
                        WHERE bd.b_type_sub1 <> 999 and bd.b_type <> 5
                    ";

          if (SelectOption == "tcontents" && !String.IsNullOrEmpty(SearchString))
          {
            base_query += " and ( bd.title like '%' + @SearchString + '%' or bd.contents like '%' + @SearchString + '%')";
          }
          else if (SelectOption == "title" && !String.IsNullOrEmpty(SearchString))
          {
            base_query += " and bd.title like '%' + @SearchString + '%'";
          }
          else if (SelectOption == "contents" && !String.IsNullOrEmpty(SearchString))
          {
            base_query += " and bd.contents like '%' + @SearchString + '%'";
          }
          else if (SelectOption == "writer" && !String.IsNullOrEmpty(SearchString))
          {
            base_query += " and us.name like '%' + @SearchString + '%'";
          }
          else if (SelectOption == "b_top" && !String.IsNullOrEmpty(SearchString))
          {
            base_query += " and bd.b_top = @SearchString ";
          }

          // 리스트 부분에만 사용될 ORDER BY 절
          string order_query = @" 
ORDER BY bd.modify_dt DESC 
OFFSET @currentPage ROWS FETCH NEXT @pageSize ROWS ONLY
";


          //리스트 부분에는 셀렉트 필드 + 공용쿼리(테이블&WHERE) + ORDER BY
          string query = @"
SELECT
    bd.*,
	isnull(us.name, '') As user_name,    
	(select count(*) from board_read_history where c_seq = bd.b_seq) As read_cnt,
	(select count(*) from board_reply where b_seq = bd.b_seq) As relpy_cnt,
    CASE WHEN bd.b_type_sub1 is not null and bd.b_type_sub1 <> 0 then
	    (select code_name from code_board_sub1 where type = bd.b_type AND code1 = bd.b_type_sub1) 
        +' > ' + (select code_name from code_board_sub2 where type = bd.b_type AND code1 = bd.b_type_sub1 and code2 = bd.b_type_sub2) 
    ELSE '' END As b_type_sub_name
" + base_query + order_query;

          var ret = SqlMapper.Query<board>(con, query, parameters, commandType: CommandType.Text);
          List<board> list = ret.ToList();

          //카운트 부분에는 셀렉트 필드(COUNT) + 공용쿼리(테이블&WHERE)
          query = @"
SELECT COUNT(0) 
" + base_query;

          totalCount = SqlMapper.ExecuteScalar<int>(con, query, parameters, commandType: CommandType.Text);



          con.Close();

          if (list.Count() > 0)
          {
            foreach (var board in list)
            {
              board.file_list = SelectBoardFileListAsync_Map(board.b_seq);
            }
          }


          return list;
        }
      }
      catch (Exception)
      {
        throw;
      }
    }
  }
}
