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
using Univision.Security;


namespace Univision.Core.Repositories
{
    public class ConsoleRepository : BaseRepository
    {
        public void IndexRebuild(string databaseNae)
        {
            try
            {
                using (IDbConnection con = new SqlConnection(base.BaseConnectionString))
                {
                    con.Open();
                    DynamicParameters parameters = new DynamicParameters();
                    parameters.Add("@database_name", databaseNae, DbType.String, null);

                    string query = @"sp_index_rebuild ";
                    SqlMapper.Execute(con, query, parameters, commandType: CommandType.StoredProcedure);

                    con.Close();
                }
            }
            catch (Exception e)
            {
                throw e;
            }

        }
    }
}
