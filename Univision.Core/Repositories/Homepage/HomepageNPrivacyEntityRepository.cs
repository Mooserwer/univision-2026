using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using Univision.Core.Models.HomePage;

namespace Univision.Core.Repositories.Homepage
{
  // 신규 개인정보 동의(n_privacy_agree) - 홈페이지 DB 저장/동기화 조회
  public class HomepageNPrivacyEntityRepository : HomepageBaseRepository
  {
    public HomepageNPrivacyEntityRepository() { }
    public HomepageNPrivacyEntityRepository(HomepageContext db) : base(db) { }

    // 발송 시 홈페이지 DB에도 마스터/상세 저장 (pa_seq 는 내부에서 생성된 값 복사)
    public async Task CreatePrivacyData(home_n_privacy_agree mst, List<home_n_privacy_agree_dtl> dtl)
    {
      using (var tran = db.Database.BeginTransaction())
      {
        try
        {
          db.Entry(mst).State = EntityState.Added;
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

    // 스케줄러 풀링용: 동의완료(agree_dt 있음) & 미동기화(is_sync=0) 건 조회
    public async Task<List<home_n_privacy_agree>> SelectAgreedNotSyncedAsync()
    {
      return await db.home_n_privacy_agrees
          .Where(x => x.is_sync == 0 && x.agree_dt != null)
          .OrderBy(x => x.pa_seq)
          .ToListAsync();
    }

    public async Task<List<home_n_privacy_agree_dtl>> SelectDtlListAsync(int pa_seq)
    {
      return await db.home_n_privacy_agree_dtls.Where(x => x.pa_seq == pa_seq).ToListAsync();
    }

    // 동기화 완료 처리
    public async Task MarkSyncedAsync(home_n_privacy_agree mst)
    {
      mst.is_sync = 1;
      mst.sync_dt = DateTime.UtcNow.AddHours(9);
      db.Entry(mst).State = EntityState.Modified;
      await db.SaveChangesAsync();
    }
  }
}
