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
    public class MeetingRoomEntityRepository : BaseRepository
    {
        public MeetingRoomEntityRepository(UnivisionContext db) : base(db)
        {

        }
        public MeetingRoomEntityRepository()
        {

        }

        public async Task<meeting_room_schedule> FindMeetingRoomScheduleOneAsync(int id)
        {
            try
            {
                return await db.meeting_room_schedules
                               .Where(x => x.id == id)
                               .FirstOrDefaultAsync();
            }
            catch(Exception e)
            {
                throw e;
            }
        }

        /// <summary>
        /// 미팅룸 예약 등록 or 삭제
        /// </summary>
        /// <param name="cm"></param>
        /// <param name="flag"></param>
        /// <returns></returns>
        public async Task CreateOrUpdateMeetingRoom(meeting_room_schedule schedule, string state)
        {

            try
            {
                if (state == "U")
                    db.Entry(schedule).State = EntityState.Modified;
                else if (state == "D")
                    db.Entry(schedule).State = EntityState.Deleted;
                else
                    db.Entry(schedule).State = EntityState.Added;

                await db.SaveChangesAsync();
            }
            catch (Exception e)
            {
                throw e;
            }
        }

    }
}
