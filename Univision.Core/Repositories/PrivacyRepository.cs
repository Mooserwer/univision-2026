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
    public class PrivacyRepository : BaseRepository
    {
        //회의실 리스트
        public async Task<List<privacy_agree>> SelectPrivacyListAsync()
        {
            try
            {
                List<privacy_agree> list = new List<privacy_agree>();

                using (IDbConnection con = new SqlConnection(base.BaseConnectionString))
                {
                    con.Open();

                    string selectQuery = @" 
SELECT *
FROM privacy_agree
";

                    //DynamicParameters param = new DynamicParameters();
                    //param.Add("@ud_seq", ud_seq, DbType.Int32);

                    var ret = await con.QueryAsync<privacy_agree>(selectQuery);

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

        public async Task<privacy_agree> SelectPrivacyOneAsync(int pa_seq)
        {
            try
            {
                List<meeting_room> list = new List<meeting_room>();

                using (IDbConnection con = new SqlConnection(base.BaseConnectionString))
                {
                    con.Open();

                    string selectQuery = @" 
SELECT *
FROM privacy_agree
WHERE pa_seq = @pa_seq";

                    DynamicParameters param = new DynamicParameters();
                    param.Add("@pa_seq", pa_seq, DbType.Int32);

                    var ret = await con.QueryAsync<privacy_agree>(selectQuery, param);
                    con.Close();

                    return ret.FirstOrDefault();
                }
            }
            catch (Exception e)
            {
                throw e;
            }
        }


        public async Task<privacy_agree> SelectPrivacyByCandOneAsync(int c_seq, int period_day = 365)
        {
            try
            {
                List<meeting_room> list = new List<meeting_room>();

                using (IDbConnection con = new SqlConnection(base.BaseConnectionString))
                {
                    con.Open();

                    string selectQuery = @" 
SELECT A.*
FROM privacy_agree A INNER JOIN privacy_agree_dtl B
                     ON A.pa_seq = B.pa_seq
                     AND B.agree_type = 1
                     AND B.is_agree = 1
WHERE A.c_seq = @c_seq 
AND A.agree_dt > dateadd(d, @period_day, getdate())
ORDER BY A.agree_dt DESC ";

                    DynamicParameters param = new DynamicParameters();
                    param.Add("@c_seq", c_seq, DbType.Int32);
                    param.Add("@period_day", (period_day * -1), DbType.Int32);
                    

                    var ret = await con.QueryAsync<privacy_agree>(selectQuery, param);
                    con.Close();

                    return ret.FirstOrDefault();
                }
            }
            catch (Exception e)
            {
                throw e;
            }
        }



        public async Task<privacy_agree> SelectPrivacyByCandAndClientOneAsync(int c_seq, int cl_seq, int period_day = 365)
        {
            try
            {
                List<meeting_room> list = new List<meeting_room>();

                using (IDbConnection con = new SqlConnection(base.BaseConnectionString))
                {
                    con.Open();

                    string selectQuery = @" 
SELECT A.*
FROM privacy_agree A INNER JOIN privacy_agree_dtl B
                     ON A.pa_seq = B.pa_seq
                     AND B.agree_type = 2
                     AND B.client_seq = @cl_seq
                     AND B.is_agree = 1
WHERE A.c_seq = @c_seq 
AND A.agree_dt > dateadd(d, @period_day, getdate())
ORDER BY A.agree_dt DESC ";

                    DynamicParameters param = new DynamicParameters();
                    param.Add("@c_seq", c_seq, DbType.Int32);
                    param.Add("@cl_seq", cl_seq, DbType.Int32);
                    param.Add("@period_day", (period_day * -1), DbType.Int32);


                    var ret = await con.QueryAsync<privacy_agree>(selectQuery, param);
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
