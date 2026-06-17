using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Threading.Tasks;


namespace Univision.Exchange
{
    public class dbConnection
    {
        static string _connectionString;
        static public SqlConnection _con;
        static public SqlCommand _com;

        public dbConnection()
        {
            //_connectionString = ConfigurationSettings.AppSettings["connectionString"].ToString();
            _connectionString = ConfigurationManager.ConnectionStrings["Univision"].ConnectionString;
        }

        //DB Open
        public static bool dbOpen()
        {
            try
            {
                _con = new SqlConnection(_connectionString);
                _con.Open();

                return true;

            }
            catch (Exception e)
            {
                return false;
            }
        }

        //DB Close
        public static void dbClose()
        {
            try
            {
                _con.Close();
                _con.Dispose();

            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public async Task<bool> SaveData(string ex_date, List<exchange> data)
        {
            try
            {
                if (dbOpen())
                {
                    _com = new SqlCommand();
                    _com.CommandText = "DELETE FROM EXCHANGE WHERE ex_date = @ex_date";
                    SqlParameter[] del_param = new SqlParameter[]
                            {
                                    new SqlParameter("@ex_date", ex_date)
                            };
                    _com.Parameters.AddRange(del_param);
                    _com.Connection = _con;
                    await _com.ExecuteNonQueryAsync();

                    string sql = @" INSERT INTO exchange (ex_date,  ex_code,  per_won,  ex_rate,  read_date)
                                        SELECT                @ex_date, @ex_code, @per_won, @ex_rate, @read_date ";

                    foreach (var row in data)
                    {
                        SqlParameter[] param = new SqlParameter[]
                            {
                                    new SqlParameter("@ex_date", row.ex_date)
                                    ,new SqlParameter("@ex_code", row.ex_code)
                                    ,new SqlParameter("@per_won", row.per_won)
                                    ,new SqlParameter("@ex_rate", row.ex_rate)
                                    ,new SqlParameter("@read_date", row.read_date)
                            };

                        _com = new SqlCommand();
                        _com.CommandText = sql;
                        _com.Parameters.AddRange(param);
                        _com.Connection = _con;

                        await _com.ExecuteNonQueryAsync();

                    }


                }
                else
                {
                    return false;
                }

                dbClose();

                return true;

            }
            catch (Exception e)
            {

                return false;
            }
        }
    }
}
