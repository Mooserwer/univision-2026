using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using System.Data.SqlClient;
using System.Configuration;
using Univision.Core.Models.DTO;
using Univision.Core.Models.HomePage;

namespace Univision.Core.Repositories.Homepage
{
    public class HomepageBaseRepository : IDisposable
    {
        protected HomepageContext db = null;

        protected readonly IDbConnection BaseDb = null;
        protected readonly string BaseConnectionString = null;

        public HomepageBaseRepository()
        {
#if DEBUG
            BaseDb = new SqlConnection(ConfigurationManager.ConnectionStrings["Homepage"].ConnectionString);
            BaseConnectionString = ConfigurationManager.ConnectionStrings["Homepage"].ConnectionString;
#else
            BaseDb = new SqlConnection(ConfigurationManager.ConnectionStrings["Homepage"].ConnectionString);
            BaseConnectionString = ConfigurationManager.ConnectionStrings["Homepage"].ConnectionString;
#endif

            db = new HomepageContext();
        }
        public HomepageBaseRepository(string connStr)
        {
            BaseDb = new SqlConnection(ConfigurationManager.ConnectionStrings[connStr].ConnectionString);
            BaseConnectionString = ConfigurationManager.ConnectionStrings[connStr].ConnectionString;
        }

        public HomepageBaseRepository(HomepageContext db)
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
