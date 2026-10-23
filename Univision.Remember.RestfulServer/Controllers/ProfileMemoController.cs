using System;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Http;
using Univision.Core.Models.DTO;
using Univision.Core.Repositories;

namespace Univision.Remember.RestfulServer.Controllers
{
  [RoutePrefix("profile-memos")]
  public class ProfileMemoController : ApiController
  {
    /// <summary>
    /// 프로필 메모 리스트 조회 (커서 페이징).
    /// GET /profile-memos?after={cursor}&amp;limit={n}
    ///   - after : 마지막으로 받은 memo_id (커서). 미지정 시 처음부터.
    ///   - limit : 한 번에 불러올 건수 (기본 50, 최대 200).
    /// 결과: [{ memo_id, profile_id, content, created_at }, ...]
    /// </summary>
    [HttpGet]
    [Route("")]
    public async Task<IHttpActionResult> Get(int after = 0, int limit = 50)
    {
      return await GetMemos(new UnivisionContext(), after, limit);
    }

    /// <summary>
    /// [테스트] 테스트 메모(R_PROFILE_MEMO_TEST) 조회. 파라미터/결과 형식은 /profile-memos 와 동일.
    /// GET /test/profile-memos?after={cursor}&amp;limit={n}
    /// </summary>
    [HttpGet]
    [Route("~/test/profile-memos")]
    public async Task<IHttpActionResult> GetTest(int after = 0, int limit = 50)
    {
      return await GetMemos(new UnivisionTestContext(), after, limit);
    }

    private async Task<IHttpActionResult> GetMemos(UnivisionContext context, int after, int limit)
    {
      try
      {
        if (after < 0) after = 0;
        if (limit <= 0) limit = 50;
        if (limit > 200) limit = 200;   // 과도한 요청 방지 상한

        using (context)
        using (ProfileMemoEntityRepository repo = new ProfileMemoEntityRepository(context))
        {
          var list = await repo.SelectMemosAfterAsync(after, limit);

          var result = list.Select(m => new
          {
            memo_id = m.memo_id,
            profile_id = m.profile_id,
            content = m.content,
            created_at = m.create_dt
          });

          return Json(result);
        }
      }
      catch (Exception e)
      {
        return Json(new { result = -1, message = e.Message });
      }
    }
  }
}
