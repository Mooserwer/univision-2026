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
    public class ScheduleEntityRepository : BaseRepository
    {
        public ScheduleEntityRepository(UnivisionContext db) : base(db)
        {

        }
        public ScheduleEntityRepository()
        {

        }

        public async Task<schedule> FindScheduleOneAsync(int s_seq)
        {
            try
            {
                return await db.schedules
                               .Where(x => x.s_seq == s_seq)
                               .FirstOrDefaultAsync();
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public async Task<schedule> FindScheduleWithProjectOneAsync(int prc_seq, int pic_seq, int p_seq, int c_seq)
        {
            try
            {
                return await db.schedules
                               .Where(x => x.prc_seq == prc_seq)
                               .Where(x => x.pic_seq == pic_seq)
                               .Where(x => x.p_seq == p_seq)
                               .Where(x => x.c_seq == c_seq)
                               .FirstOrDefaultAsync();
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public async Task<schedule> FindScheduleWithClientOneAsync(int cl_seq, int cal_seq)
        {
            try
            {
                return await db.schedules
                               .Where(x => x.cl_seq == cl_seq)
                               .Where(x => x.cal_seq == cal_seq)
                               .FirstOrDefaultAsync();
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public async Task<schedule> FindScheduleWithMeetingRoomOneAsync(int meeting_id)
        {
            try
            {
                return await db.schedules
                               .Where(x => x.meeting_id == meeting_id)
                               .FirstOrDefaultAsync();
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public async Task<List<schedule_attend>> FindScheduleAttendListAsync(int s_seq)
        {
            try
            {
                return await db.schedule_attends
                               .Where(x => x.s_seq == s_seq)
                               .ToListAsync();
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        /// <summary>
        /// 스케줄 등록, 수정
        /// </summary>
        /// <param name="schedule">스케줄</param>
        /// <param name="attendList">공유자 리스트</param>
        /// <param name="deleteAttends">삭제할 공유자 리스트(기존 공유자)</param>
        /// <param name="state">상태</param>
        /// <returns></returns>
        public async Task CreateOrUpdateSchedule(schedule schedule, List<schedule_attend> attendList, List<schedule_attend> deleteAttends, string state, int event_uv_seq = 0)
        {

            try
            {
                ProjectLogRepository log = new ProjectLogRepository();
                if (deleteAttends != null)
                {
                    foreach (var del in deleteAttends)
                    {
                        db.Entry(del).State = EntityState.Deleted;
                    }
                    await log.schedule_attend_log(deleteAttends, "D", event_uv_seq, 1);
                }

                if (state == "U")
                {
                    db.Entry(schedule).State = EntityState.Modified;
                    await log.schedule_log(schedule, state, event_uv_seq, 1);
                }
                else
                    db.Entry(schedule).State = EntityState.Added;


               
                //s_seq 를 얻어야 함.
                await db.SaveChangesAsync();

                foreach (var attend in attendList)
                {
                    attend.s_seq = schedule.s_seq;
                    db.Entry(attend).State = EntityState.Added;
                }

                await db.SaveChangesAsync();
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        /// <summary>
        /// 스케줄 삭제
        /// </summary>
        /// <param name="schedule"></param>
        /// <param name="attendList"></param>
        /// <returns></returns>
        public async Task DeleteSchedule(schedule schedule, List<schedule_attend> attendList, int event_uv_seq = 0)
        {

            try
            {
                ProjectLogRepository log = new ProjectLogRepository();
                db.Entry(schedule).State = EntityState.Deleted;
                await log.schedule_log(schedule, "D", event_uv_seq, 1);

                foreach (var attend in attendList)
                {
                    db.Entry(attend).State = EntityState.Deleted;
                }
                await log.schedule_attend_log(attendList, "D", event_uv_seq, 1);
                await db.SaveChangesAsync();
            }
            catch (Exception e)
            {
                throw e;
            }
        }

    }
}
