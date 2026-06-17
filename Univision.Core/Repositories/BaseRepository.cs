using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using System.Data.SqlClient;
using System.Configuration;
using Univision.Core.Models.DTO;

namespace Univision.Core.Repositories
{
    public class BaseRepository : IDisposable
    {
        protected UnivisionContext db = null;

        protected readonly IDbConnection BaseDb = null;
        protected readonly string BaseConnectionString = null;

        public BaseRepository()
        {
#if DEBUG
            BaseDb = new SqlConnection(ConfigurationManager.ConnectionStrings["UnivisionDebug"].ConnectionString);
            BaseConnectionString = ConfigurationManager.ConnectionStrings["UnivisionDebug"].ConnectionString;
#else
            BaseDb = new SqlConnection(ConfigurationManager.ConnectionStrings["Univision"].ConnectionString);
            BaseConnectionString = ConfigurationManager.ConnectionStrings["Univision"].ConnectionString;
#endif

            db = new UnivisionContext();
        }
        public BaseRepository(string connStr)
        {
            BaseDb = new SqlConnection(ConfigurationManager.ConnectionStrings[connStr].ConnectionString);
            BaseConnectionString = ConfigurationManager.ConnectionStrings[connStr].ConnectionString;
        }

        public BaseRepository(UnivisionContext db)
        {
            this.db = db;
        }


        public void Dispose()
        {
            if (BaseDb != null)
            {
                BaseDb.Dispose();
            }
        }
    }
}
