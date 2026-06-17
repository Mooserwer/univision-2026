using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Validation;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Univision.Core.Models.HomePage;


namespace Univision.Core.Repositories.Homepage
{
  public class HomepageEntityRepository : HomepageBaseRepository
  {
    public HomepageEntityRepository(HomepageContext db) : base(db)
    {

    }
    public HomepageEntityRepository()
    {

    }

    public async Task<home_project> SelectHomeProjectOneAsync(int p_seq)
    {
      try
      {
        return await db.home_projects
                       .Where(x => x.p_seq == p_seq)
                       .FirstOrDefaultAsync();
      }
      catch (Exception e)
      {
        throw e;
      }
    }



    public async Task DeleteHomeProject(home_project model)
    {
      try
      {
        db.Entry(model).State = EntityState.Deleted;


        await db.SaveChangesAsync();
      }
      catch (Exception e)
      {
        throw e;
      }
    }

    public async Task CreateOrUpdateHomeProject(home_project homepage_project, string state)
    {
      try
      {
        using (var tran = db.Database.BeginTransaction())
        {
          try
          {
            if (state == "U")
            {
              db.Entry(homepage_project).State = EntityState.Modified;


            }
            else
            {
              db.Entry(homepage_project).State = EntityState.Added;
            }


            //프로젝트 seq를 얻어야 하므로 먼저 한번 저장.
            await db.SaveChangesAsync();




            tran.Commit();
          }
          catch (Exception e)
          {
            tran.Rollback();
            throw e;

          }
        }

      }
      catch (Exception e)
      {
        throw e;
      }
    }


  }
}
