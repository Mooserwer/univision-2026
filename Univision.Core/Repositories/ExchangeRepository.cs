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
    public class ExchangeRepository : BaseRepository
    {
        //회의실 리스트
        public async Task<exchange> SelectExchangeOneAsync(int ex_seq)
        {
            try
            {
                exchange list = new exchange();

                using (IDbConnection con = new SqlConnection(base.BaseConnectionString))
                {
                    con.Open();

                    string selectQuery = @" 
SELECT *
FROM exchange
WHERE ex_seq = @ex_seq ";

                    //DynamicParameters param = new DynamicParameters();
                    //param.Add("@ud_seq", ud_seq, DbType.Int32);

                    DynamicParameters param = new DynamicParameters();
                    param.Add("@ex_seq", ex_seq, DbType.Int32);

                    var ret = await con.QueryAsync<exchange>(selectQuery, param);
                    con.Close();

                    return ret.FirstOrDefault();
                }
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public async Task<exchange> SelectExchangeOneAsync(string ex_date, string ex_cur)
        {
            try
            {
                exchange list = new exchange();

                using (IDbConnection con = new SqlConnection(base.BaseConnectionString))
                {
                    con.Open();

                    string selectQuery = @" 
SELECT *
FROM exchange
WHERE ex_date = @ex_date
AND   ex_code = @ex_cur";

                    //DynamicParameters param = new DynamicParameters();
                    //param.Add("@ud_seq", ud_seq, DbType.Int32);

                    DynamicParameters param = new DynamicParameters();
                    param.Add("@ex_date", ex_date, DbType.String);
                    param.Add("@ex_cur", ex_cur, DbType.String);

                    var ret = await con.QueryAsync<exchange>(selectQuery, param);
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
