using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Univision.Core.Models.DTO;

namespace Univision.Core.Repositories
{
    public class GptUseHistoryRepository : BaseRepository
    {
        public GptUseHistoryRepository() { }
        public GptUseHistoryRepository(UnivisionContext db) : base(db) { }

        // ─────────────────────────────────────────────────────
        //  신규 이력 생성 (처리 시작 시점, status=0)
        //  반환: 생성된 guh_seq
        // ─────────────────────────────────────────────────────
        public async Task<int> CreateAsync(gpt_use_history model)
        {
            model.status    = 0;
            model.create_dt = DateTime.Now;

            db.gpt_use_historys.Add(model);
            await db.SaveChangesAsync();
            return model.guh_seq;
        }

        // ─────────────────────────────────────────────────────
        //  완료 업데이트 (status=1)
        // ─────────────────────────────────────────────────────
        public async Task<bool> CompleteAsync(
            int     guh_seq,
            string  result_nc,
            string  result_oc,
            string  output_file,
            string  output_path,
            decimal proc_sec)
        {
            var row = await db.gpt_use_historys.FindAsync(guh_seq);
            if (row == null) return false;

            row.result_nc   = result_nc;
            row.result_oc   = result_oc;
            row.output_file = output_file;
            row.output_path = output_path;
            row.proc_sec    = proc_sec;
            row.status      = 1;

            await db.SaveChangesAsync();
            return true;
        }

        // ─────────────────────────────────────────────────────
        //  실패 업데이트 (status=2)
        // ─────────────────────────────────────────────────────
        public async Task<bool> FailAsync(int guh_seq, string error_msg)
        {
            var row = await db.gpt_use_historys.FindAsync(guh_seq);
            if (row == null) return false;

            row.error_msg = error_msg?.Length > 500
                            ? error_msg.Substring(0, 500)
                            : error_msg;
            row.status    = 2;

            await db.SaveChangesAsync();
            return true;
        }

        // ─────────────────────────────────────────────────────
        //  단건 조회
        // ─────────────────────────────────────────────────────
        public async Task<gpt_use_history> SelectOneAsync(int guh_seq)
        {
            return await db.gpt_use_historys.FindAsync(guh_seq);
        }

        // ─────────────────────────────────────────────────────
        //  사용자별 최근 이력 조회
        // ─────────────────────────────────────────────────────
        public List<gpt_use_history> SelectByUser(int create_seq, int top = 20)
        {
            return db.gpt_use_historys
                     .Where(x => x.create_seq == create_seq)
                     .OrderByDescending(x => x.guh_seq)
                     .Take(top)
                     .ToList();
        }
    }
}
