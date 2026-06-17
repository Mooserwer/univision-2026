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
using Univision.Core.Models.DTO.Response.Board;
using Univision.Core.Models.DTO.Request.Board;


namespace Univision.Core.Repositories.MailResume
{
  public class MailResumeEntityRepository : BaseRepository
  {
    public MailResumeEntityRepository(UnivisionContext db) : base(db)
    {

    }
    public MailResumeEntityRepository()
    {

    }


    public async Task<mail_resume_file> SelectMailResumeFileOneAsync(string timestamp, int dn_seq)
    {
      try
      {
        return await db.mail_resume_files
                       .Where(x => (x.dv_timestamp == timestamp && x.dn_seq == dn_seq))
                       .FirstOrDefaultAsync();
      }
      catch (Exception e)
      {
        throw e;
      }
    }

    public async Task<mail_resume> SelectMailResumeOneAsync(string timestamp)
    {
      try
      {
        return await db.mail_resumes
                       .Where(x => (x.dv_timestamp == timestamp))
                       .FirstOrDefaultAsync();
      }
      catch (Exception e)
      {
        throw e;
      }
    }

    public async Task<mail_resume_gpt> SelectMailResumeGptOneAsync(string timestamp)
    {
      try
      {
        return await db.mail_resume_gpts
                       .Where(x => (x.dv_timestamp == timestamp))
                       .FirstOrDefaultAsync();
      }
      catch (Exception e)
      {
        throw e;
      }
    }



    public async Task CreateOrUpdateMailResumeFileOneAsync(mail_resume_file resume, string state)
    {
      try
      {
        if (state == "U")
        {
          db.Entry(resume).State = EntityState.Modified;
        }
        else
        {
          db.Entry(resume).State = EntityState.Added;
        }

        //저장
        await db.SaveChangesAsync();
      }
      catch (Exception e)
      {
        throw e;
      }
    }

    public async Task CreateOrUpdateMailResumeOneAsync(mail_resume mail, string state)
    {
      try
      {
        if (state == "U")
        {
          db.Entry(mail).State = EntityState.Modified;
        }
        else
        {
          db.Entry(mail).State = EntityState.Added;
        }

        //저장
        await db.SaveChangesAsync();
      }
      catch (Exception e)
      {
        throw e;
      }
    }

    public async Task CreateOrUpdateMailResumeGPTOneAsync(mail_resume_gpt gpt, string state)
    {
      try
      {
        if (state == "U")
        {
          db.Entry(gpt).State = EntityState.Modified;
        }
        else
        {
          db.Entry(gpt).State = EntityState.Added;
        }

        //저장
        await db.SaveChangesAsync();
      }
      catch (Exception e)
      {
        throw e;
      }
    }

    public async Task DeleteGptResult(mail_resume_gpt gpt)
    {
      try
      {


        db.Entry(gpt).State = EntityState.Deleted;


        await db.SaveChangesAsync();
      }
      catch (Exception e)
      {
        throw e;
      }
    }
  }
}
