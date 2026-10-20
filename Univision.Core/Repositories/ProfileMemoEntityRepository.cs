using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using Univision.Core.Models.DTO;

namespace Univision.Core.Repositories
{
  public class ProfileMemoEntityRepository : BaseRepository
  {
    public ProfileMemoEntityRepository() { }
    public ProfileMemoEntityRepository(UnivisionContext db) : base(db) { }

    /// <summary>
    /// 커서(after = 마지막으로 받은 memo_id) 이후 메모를 memo_id 오름차순으로 limit 건 조회.
    /// </summary>
    public async Task<List<profile_memo>> SelectMemosAfterAsync(int after, int limit)
    {
      return await db.profile_memos
          .Where(x => x.memo_id > after)
          .OrderBy(x => x.memo_id)
          .Take(limit)
          .ToListAsync();
    }
  }
}
