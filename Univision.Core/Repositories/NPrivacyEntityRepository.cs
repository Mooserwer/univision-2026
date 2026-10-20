using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using Univision.Core.Models.DTO;

namespace Univision.Core.Repositories
{
  // 신규 개인정보 동의(n_privacy_agree) - 내부 DB 저장/조회
  public class NPrivacyEntityRepository : BaseRepository
  {
    public NPrivacyEntityRepository() { }
    public NPrivacyEntityRepository(UnivisionContext db) : base(db) { }

    public async Task CreatePrivacyData(n_privacy_agree mst, List<n_privacy_agree_dtl> dtl)
    {
      using (var tran = db.Database.BeginTransaction())
      {
        try
        {
          db.Entry(mst).State = EntityState.Added;
          await db.SaveChangesAsync();   // pa_seq 채번

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

    public async Task UpdatePrivacyMst(n_privacy_agree mst)
    {
      db.Entry(mst).State = EntityState.Modified;
      await db.SaveChangesAsync();
    }

    public async Task<n_privacy_agree> SelectOneAsync(int pa_seq)
    {
      return await db.n_privacy_agrees.FirstOrDefaultAsync(x => x.pa_seq == pa_seq);
    }

    public async Task<List<n_privacy_agree_dtl>> SelectDtlListAsync(int pa_seq)
    {
      return await db.n_privacy_agree_dtls.Where(x => x.pa_seq == pa_seq).ToListAsync();
    }
  }
}
