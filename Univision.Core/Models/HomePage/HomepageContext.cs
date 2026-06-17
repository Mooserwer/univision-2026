using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace Univision.Core.Models.HomePage
{

  public class HomepageContext : DbContext
  {
    private static string GetConnectionString()
    {
#if DEBUG
            return ConfigurationManager.ConnectionStrings["HomepageEntities"].ConnectionString;
#else
      return ConfigurationManager.ConnectionStrings["HomepageEntities"].ConnectionString;
#endif
    }

    /// <summary>Constructor</summary>
    public HomepageContext() : base(GetConnectionString()) { }

    #region dbSet

    public DbSet<home_project> home_projects { get; set; }
    public DbSet<home_privacy_agree> home_privacy_agrees { get; set; }
    public DbSet<home_privacy_agree_dtl> home_privacy_agree_dtls { get; set; }
    public DbSet<home_sms_history> home_sms_historys { get; set; }
    public DbSet<home_sms_report> home_sms_reports { get; set; }


    #endregion
  }
}
