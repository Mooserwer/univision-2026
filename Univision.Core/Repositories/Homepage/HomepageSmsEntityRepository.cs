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
  public class HomepageSmsEntityRepository : HomepageBaseRepository
  {
    public HomepageSmsEntityRepository(HomepageContext db) : base(db)
    {

    }
    public HomepageSmsEntityRepository()
    {

    }

    /// <summary>
    /// sms 발송내역 등록
    /// </summary>
    /// <param name="smsList"></param>
    /// <returns></returns>
    public async Task CreateSms(List<home_sms_history> smsList)
    {
      try
      {
        foreach (var sms in smsList)
        {
          db.Entry(sms).State = EntityState.Added;
        }

        await db.SaveChangesAsync();
      }
      catch (Exception e)
      {
        throw e;
      }
    }

    public async Task CreatePrivacyData(home_privacy_agree mst, List<home_privacy_agree_dtl> dtl)
    {
      try
      {
        using (var tran = db.Database.BeginTransaction())
        {
          try
          {
            db.Entry(mst).State = EntityState.Added;

            //저장
            await db.SaveChangesAsync();

            foreach (var p_dtl in dtl)
            {
              p_dtl.pa_seq = mst.pa_seq;
              db.Entry(p_dtl).State = EntityState.Added;
            }
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

    public async Task UpdatePrivacyMst(home_privacy_agree mst)
    {
        try
        {
          db.Entry(mst).State = EntityState.Modified;

          //저장
          await db.SaveChangesAsync();

         
        }
        catch (Exception e)
        {
          throw e;

        }
        
     
    }

    public async Task<home_privacy_agree> SelectPrivacyAgreeOneAsnyc(int pa_seq)
    {
      try
      {
        return await db.home_privacy_agrees
                       .Where(x => x.pa_seq == pa_seq)
                       .FirstOrDefaultAsync();
      }
      catch (Exception e)
      {
        throw e;
      }
    }

    public async Task<List<home_privacy_agree>> SelectPrivacyAgreeListAsync(int pa_seq)
    {
      try
      {
        return await db.home_privacy_agrees
                       .Where(x => x.pa_seq == pa_seq)
                       .ToListAsync();
      }
      catch (Exception e)
      {
        throw e;
      }
    }

    public async Task<List<home_privacy_agree>> SelectPrivacyAgreeListDateAsync(DateTime latest)
    {
      try
      {
        return await db.home_privacy_agrees
                       .Where(x => x.agree_dt.HasValue && x.agree_dt >= latest )
                       .ToListAsync();
      }
      catch (Exception e)
      {
        throw e;
      }
    }

    public async Task<List<home_sms_report>> SelectSmsReportListDateAsync(DateTime latest)
    {
      try
      {
        return await db.home_sms_reports
                       .Where(x => x.create_dt >= latest)
                       .ToListAsync();
      }
      catch (Exception e)
      {
        throw e;
      }
    }

    public async Task<home_privacy_agree_dtl> SelectPrivacyAgreeDtlOneAsnyc(int pa_seq, int pad_seq)
    {
      try
      {
        return await db.home_privacy_agree_dtls
                       .Where(x => x.pa_seq == pa_seq && x.pad_seq == pad_seq)
                       .FirstOrDefaultAsync();
      }
      catch (Exception e)
      {
        throw e;
      }
    }

    public async Task<List<home_privacy_agree_dtl>> SelectPrivacyAgreeDtlListAsync(int pa_seq)
    {
      try
      {
        return await db.home_privacy_agree_dtls
                       .Where(x => x.pa_seq == pa_seq)
                       .ToListAsync();
      }
      catch (Exception e)
      {
        throw e;
      }
    }

  }
}
