using Dapper;
using Univision.Core.Models.DTO;
using Univision.Core.Repositories;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Univision.Core.Models.DTO.Remember;

namespace Univision.Core.Repositories
{
  public class AccountRepository : BaseRepository
  {
    public async Task<uv_user> SelectUser(string id, string pwd, int isAdmin = 0)
    {
      try
      {
        using (IDbConnection con = new SqlConnection(base.BaseConnectionString))
        {
          con.Open();
          DynamicParameters parameters = new DynamicParameters();
          parameters.Add("@id", id, DbType.String, null, 50);
          parameters.Add("@pwd", pwd, DbType.String, null, 80);
          parameters.Add("@isAdmin", isAdmin, DbType.Int16, null);
          string query = @"
select 
	uv.uv_seq uv_seq,
	uv.user_id user_id,
	uv.name name,
    uv.rank_name as rank_name,
	ud.ud_name ud_name,
	ua.level level,
	uv.email,
    isnull(uv.tel, '') AS tel,
    isnull(uv.hp, '') AS hp,
	uv.img_dir,
	uv.img_origin_path,
	uv.img_path,
	uv.entry_date,
	uv.ua_seq,
	uv.ud_seq,
    ua.level,
    s_team1.uv_seq as s_confirm_seq,
    s_team2.uv_seq as s_leader_seq,
    ISNULL((SELECT TOP 1 uv_seq FROM UV_USER WHERE ud_seq = uv.ud_seq AND ua_seq in (2, 4) AND user_id NOT IN ('sshan', 'ivykim', 'canon92')), uv.uv_seq) as leader_seq,
    uv.is_out_login
from
	uv_user uv inner join uv_division ud
					on uv.ud_seq = ud.ud_seq
				inner join uv_auth ua
					on uv.ua_seq = ua.ua_seq
                inner join uv_user s_team1
					on s_team1.user_id = 'euna'
                inner join uv_user s_team2
					on s_team2.user_id = 'euna'
where   (uv.retire_date is null OR uv.retire_date < GETDATE())
AND 	uv.user_id = @id and uv.pwd = @pwd
";
          var ret = await SqlMapper.QueryAsync<uv_user>(con, query, parameters, commandType: CommandType.Text);
          con.Close();
          return ret.FirstOrDefault();
        }
      }
      catch (Exception)
      {

        throw;
      }

    }

    public async Task<List<uv_user>> SelectAllUser()
    {
      try
      {
        List<uv_user> list = new List<uv_user>();
        using (IDbConnection con = new SqlConnection(base.BaseConnectionString))
        {
          con.Open();


          string query = @" SELECT uv_seq, user_id, name FROM uv_user WHERE retire_date IS NULL ";

          var ret = await SqlMapper.QueryAsync<uv_user>(con, query, commandType: CommandType.Text);
          con.Close();

          list = ret.ToList();

          return list;
        }
      }
      catch (Exception)
      {

        throw;
      }

    }



    public async Task<List<r_uv_user>> SelectRememberUserListAsync(int uv_seq = 0)
    {
      try
      {
        List<r_uv_user> list = new List<r_uv_user>();
        using (IDbConnection con = new SqlConnection(base.BaseConnectionString))
        {
          con.Open();

          string query = @" 
SELECT A.uv_seq, 
       CASE WHEN retire_date IS NOT NULL THEN '[퇴사] ' + REPLACE(A.name, 'R_', '') ELSE A.name END as name, 
       A.rank_name, 
       B.ud_name as div_name,
       CASE WHEN retire_date IS NOT NULL THEN '' ELSE A.email END as email 
       
FROM uv_user A left join UV_DIVISION B
     ON A.ud_seq = B.ud_seq
WHERE A.ud_seq <> 69 ";
          if (uv_seq != 0)
          {
            query = query + @" AND uv_seq = @uv_seq ";
          }
          query = query + @" ORDER BY uv_seq ";
          DynamicParameters parameters = new DynamicParameters();
          parameters.Add("@uv_seq", uv_seq, DbType.Int16, null);


          var ret = await SqlMapper.QueryAsync<r_uv_user>(con, query, parameters, commandType: CommandType.Text);
          con.Close();

          list = ret.ToList();

          return list;
        }
      }
      catch (Exception)
      {

        throw;
      }

    }

    public async Task<int> FindUvSeq(string user_id)
    {
      try
      {
        using (IDbConnection con = new SqlConnection(base.BaseConnectionString))
        {
          con.Open();

          string query = @" SELECT uv_seq FROM uv_user WHERE user_id = @user_id ";

          DynamicParameters parameters = new DynamicParameters();
          parameters.Add("@user_id", user_id, DbType.String, null, 50);

          var ret = await SqlMapper.QueryAsync<int>(con, query, parameters, commandType: CommandType.Text);
          con.Close();

          return ret.FirstOrDefault();
        }
      }
      catch (Exception e)
      {
        throw e;
      }
    }

    public async Task<uv_user> FindUserByIdAsync(string user_id)
    {
      try
      {
        using (IDbConnection con = new SqlConnection(base.BaseConnectionString))
        {
          con.Open();

          string query = @" SELECT * FROM uv_user WHERE user_id = @user_id ";

          DynamicParameters parameters = new DynamicParameters();
          parameters.Add("@user_id", user_id, DbType.String, null, 50);

          var ret = await SqlMapper.QueryAsync<uv_user>(con, query, parameters, commandType: CommandType.Text);
          con.Close();

          return ret.FirstOrDefault();
        }
      }
      catch (Exception e)
      {
        throw e;
      }
    }

    public async Task<uv_user> FindUserBySeqAsync(int uv_seq)
    {
      try
      {
        using (IDbConnection con = new SqlConnection(base.BaseConnectionString))
        {
          con.Open();

          string query = @" SELECT U.* 
      ,UD.ud_name
      ,UA.name AS ua_name
  FROM uv_user AS U LEFT OUTER JOIN uv_division AS UD 
                                 ON U.ud_seq = UD.ud_seq
                    LEFT OUTER JOIN uv_auth AS UA
                                 ON U.ua_seq = UA.ua_seq
 WHERE uv_seq = @uv_seq ";

          DynamicParameters parameters = new DynamicParameters();
          parameters.Add("@uv_seq", uv_seq, DbType.Int32, null);

          var ret = await SqlMapper.QueryAsync<uv_user>(con, query, parameters, commandType: CommandType.Text);
          con.Close();

          return ret.FirstOrDefault();
        }
      }
      catch (Exception e)
      {
        throw e;
      }
    }

    public async Task<string> FindUserEmailById(string user_id)
    {
      try
      {
        using (IDbConnection con = new SqlConnection(base.BaseConnectionString))
        {
          con.Open();

          string query = @" SELECT email FROM uv_user WHERE user_id = @user_id ";

          DynamicParameters parameters = new DynamicParameters();
          parameters.Add("@user_id", user_id, DbType.String, null, 50);

          var ret = await SqlMapper.QueryAsync<string>(con, query, parameters, commandType: CommandType.Text);
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
    /// 부서 리스트
    /// </summary>
    /// <returns></returns>
    public async Task<List<uv_division>> SelectDivisionList()
    {
      try
      {
        List<uv_division> list = new List<uv_division>();

        using (IDbConnection con = new SqlConnection(base.BaseConnectionString))
        {
          con.Open();

          string selectQuery = @" SELECT ud_seq, ud_name
  FROM uv_division 
 WHERE is_use = 1 
   AND ud_seq NOT IN (69, 70)
ORDER BY ud_name COLLATE Korean_Wansung_BIN ASC ";

          var ret = await con.QueryAsync<uv_division>(selectQuery);

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
    /// 부서에 속한 유저 리스트 검색
    /// </summary>
    /// <param name="ud_seq"></param>
    /// <returns></returns>
    public async Task<List<uv_user>> SelectUserListByDivision(int ud_seq)
    {
      try
      {
        List<uv_user> list = new List<uv_user>();

        using (IDbConnection con = new SqlConnection(base.BaseConnectionString))
        {
          con.Open();

          string selectQuery = @" SELECT uv_seq, name
  FROM uv_user
 WHERE retire_date IS NULL ";
          if (ud_seq > 0)
          {
            selectQuery += @" AND ud_seq = @ud_seq ";
          }


          selectQuery += @"ORDER BY name";

          DynamicParameters param = new DynamicParameters();
          param.Add("@ud_seq", ud_seq, DbType.Int32);

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

    public async Task<List<uv_user>> SelectAllUserList(List<int> except_user)
    {
      try
      {
        List<uv_user> list = new List<uv_user>();

        using (IDbConnection con = new SqlConnection(base.BaseConnectionString))
        {
          con.Open();

          string selectQuery = @" SELECT uv_seq, name
  FROM uv_user
 WHERE retire_date IS NULL 
 AND ud_seq NOT IN (69, 70)
 AND user_id not like '%intern%' ";
          if (except_user.Count > 0)
          {
            bool first = true;
            selectQuery += @" AND uv_seq not in (";
            foreach (var e_user in except_user)
            {
              if (first)
              {
                selectQuery += e_user.ToString();
                first = false;
              }
              else
              {
                selectQuery += ", " + e_user.ToString();
              }


            }
            selectQuery += @" ) ";
          }


          selectQuery += @"ORDER BY name";


          var ret = await con.QueryAsync<uv_user>(selectQuery);

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
