using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Validation;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Univision.Core.Models.DTO;
using Univision.Core.Models.DTO.Request;
using Univision.Core.Models.DTO.Request.Candidate;


namespace Univision.Core.Repositories
{
  public class RememberTaskEntityRepository : BaseRepository
  {
    public RememberTaskEntityRepository(UnivisionContext db) : base(db)
    {

    }
    public RememberTaskEntityRepository()
    {

    }

    //task_type (0:요청, 1:전송, 2: 완료, 9:실패)
    public async Task UpdateRequest(Decimal task_id, int task_type, string addr = "", string comment = "")
    {
      try
      {
        ///테스트 후 할것
        using (var tran = db.Database.BeginTransaction())
        {
          try
          {
            remember_task_his his = new remember_task_his
            {
              task_id = task_id,
              task_type = task_type,
              task_dt = DateTime.UtcNow.AddHours(9),
              task_addr = addr,
              comment = comment
            };

            db.Entry(his).State = EntityState.Added;

            await db.SaveChangesAsync();

            tran.Commit();
          }
          catch (DbEntityValidationException dbEx)
          {
            foreach (var validationErrors in dbEx.EntityValidationErrors)
            {
              foreach (var validationError in validationErrors.ValidationErrors)
              {
                System.Console.WriteLine("Property: {0} Error: {1}", validationError.PropertyName, validationError.ErrorMessage);
              }
            }
            tran.Rollback();
            throw dbEx;
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

    public async Task UpdateRequest(List<remember_task> task, int task_type, string addr = "", string comment = "")
    {
      try
      {
        ///테스트 후 할것
        using (var tran = db.Database.BeginTransaction())
        {
          try
          {
            foreach (var data in task)
            {
              remember_task_his his = new remember_task_his
              {
                task_id = data.task_id,
                task_type = task_type,
                task_dt = DateTime.UtcNow.AddHours(9),
                task_addr = addr,
                comment = comment
              };

              db.Entry(his).State = EntityState.Added;
            }

            await db.SaveChangesAsync();

            tran.Commit();
          }
          catch (DbEntityValidationException dbEx)
          {
            foreach (var validationErrors in dbEx.EntityValidationErrors)
            {
              foreach (var validationError in validationErrors.ValidationErrors)
              {
                System.Console.WriteLine("Property: {0} Error: {1}", validationError.PropertyName, validationError.ErrorMessage);
              }
            }
            tran.Rollback();
            throw dbEx;
          }
        }
      }
      catch (Exception e)
      {
        throw e;
      }
    }

    public async Task BackupRequest(List<remember_task> task)
    {
      try
      {
        if (task.Count > 0)
        {
          ///테스트 후 할것
          using (var tran = db.Database.BeginTransaction())
          {
            try
            {

              var tmp_list = await db.remember_task_tmps.ToListAsync();
              foreach (var tmp_data in tmp_list)
              {
                db.Entry(tmp_data).State = EntityState.Deleted;
                await db.SaveChangesAsync();
              }
              
              foreach (var data in task)
              {
                remember_task_tmp tmp = new remember_task_tmp
                {
                  task_id = data.task_id,
                  iud_type = data.iud_type,
                  c_seq = data.c_seq,
                  create_dt = data.create_dt
                };

                db.Entry(tmp).State = EntityState.Added;
              }

              await db.SaveChangesAsync();

              tran.Commit();
            }
            catch (DbEntityValidationException dbEx)
            {
              string errmsg = "";
              foreach (var validationErrors in dbEx.EntityValidationErrors)
              {
                foreach (var validationError in validationErrors.ValidationErrors)
                {
                  errmsg = errmsg + @"
" + String.Format("Property: {0} Error: {1}", validationError.PropertyName, validationError.ErrorMessage);
                }
              }
              tran.Rollback();
              throw new Exception(dbEx.Message +" -> "+ errmsg);
            }
            catch (Exception e)
            {
              tran.Rollback();
              throw e;
            }
          }
        }
      }
      catch (Exception e)
      {
        throw e;
      }
    }

    public async Task<remember_task> SelectTaskOneAsync(int task_id)
    {
      try
      {
        return await db.remember_tasks
                       .Where(x => x.task_id == task_id)
                       .FirstOrDefaultAsync();
      }
      catch (Exception e)
      {
        throw e;
      }
    }

    public async Task<List<remember_task>> SelectTaskListAsync()
    {
      try
      {
        return await db.remember_tasks
          .Where(x => true).ToListAsync();
      }
      catch (Exception e)
      {
        throw e;
      }
    }


  }
}
