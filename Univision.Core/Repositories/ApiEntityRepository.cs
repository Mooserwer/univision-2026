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


namespace Univision.Core.Repositories
{
    public class ApiEntityRepository : BaseRepository
    {
        public ApiEntityRepository(UnivisionContext db) : base(db)
        {

        }
        public ApiEntityRepository()
        {

        }


        public async Task<alarm_message> SelectAlarmMessageOneAsync(int am_seq)
        {
            try
            {
                return await db.alarm_messages
                               .Where(x => x.am_seq == am_seq)
                               .FirstOrDefaultAsync();
            }
            catch (Exception e)
            {
                throw e;
            }
        }



        public async Task<alarm_user> SelectAlarmUserOneAsync(int am_seq, int uv_seq)
        {
            try
            {
                return await db.alarm_users
                               .Where(x => x.am_seq == am_seq && x.uv_seq == uv_seq)
                               .FirstOrDefaultAsync();
            }
            catch (Exception e)
            {
                throw e;
            }
        }
    public async Task<List<code_school_search>> SelectCodeSchoolSearchListAsync()
    {
      try
      {
        return await db.code_school_searchs
                       .ToListAsync();
      }
      catch (Exception e)
      {
        throw e;
      }
    }
    public async Task<List<alarm_user>> SelectAlarmUserListAsync(int[] am_seqs, int uv_seq)
        {
            try
            {
                return await db.alarm_users
                               .Where(x => am_seqs.Contains(x.am_seq) && x.uv_seq == uv_seq)
                               .ToListAsync();
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public async Task CreateOrUpdateAlarmUserOneAsync(alarm_user alarm, string state)
        {
            try
            {
                if (state == "U")
                {
                    db.Entry(alarm).State = EntityState.Modified;
                }
                else
                {
                    db.Entry(alarm).State = EntityState.Added;
                }

                //저장
                await db.SaveChangesAsync();
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public async Task CreateOrUpdateAlarmUserListAsync(List<alarm_user> alarmList, string state)
        {
            try
            {
                using (var tran = db.Database.BeginTransaction())
                {
                    try
                    {
                        foreach (var alarm in alarmList)
                        {
                            if (state == "U")
                            {
                                db.Entry(alarm).State = EntityState.Modified;
                            }
                            else
                            {
                                db.Entry(alarm).State = EntityState.Added;
                            }
                        }

                        //저장
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

        /// <summary>
        /// 알림 생성.
        /// </summary>
        /// <param name="aMessage"></param>
        /// <param name="aUserList"></param>
        /// <returns></returns>
        public async Task CreateAlarm(alarm_message aMessage, List<alarm_user> aUserList)
        {
            try
            {
                using (var tran = db.Database.BeginTransaction())
                {
                    try
                    {
                        db.Entry(aMessage).State = EntityState.Added;

                        await db.SaveChangesAsync();

                        foreach (var aUser in aUserList)
                        {
                            aUser.am_seq = aMessage.am_seq;
                            db.Entry(aUser).State = EntityState.Added;
                        }

                        //저장
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

        /// <summary>
        /// 모바일 오픈 로그 저장.
        /// </summary>
        /// <param name="log"></param>
        /// <returns></returns>
        public async Task CreateMobileOpenLog(element_open_log log)
        {
            try
            {
                db.Entry(log).State = EntityState.Added;

                //저장
                await db.SaveChangesAsync();
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        /// <summary>
        /// 로그인 히스토리 저장
        /// </summary>
        /// <param name="history"></param>
        /// <returns></returns>
        public async Task CreateLoginHistory(uv_login_history history)
        {
            try
            {
                db.Entry(history).State = EntityState.Added;

                //저장
                await db.SaveChangesAsync();
            }
            catch (Exception e)
            {
                throw e;
            }
        }

    }
}
