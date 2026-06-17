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
    public class AccountEntityRepository : BaseRepository
    {
        public AccountEntityRepository(UnivisionContext db) : base(db)
        {

        }
        public AccountEntityRepository()
        {

        }

        /// <summary>
        /// 외부접속 로그인 인증코드 단건 조회.
        /// </summary>
        /// <param name="a_seq"></param>
        /// <param name="user_id"></param>
        /// <returns></returns>
        public async Task<auth_code_temp> SelectAuthCodeOneAsync(int a_seq, string user_id)
        {
            try
            {
                return await db.auth_code_temps
                               .Where(x => x.a_seq == a_seq && x.user_id == user_id)
                               .FirstOrDefaultAsync();
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        /// <summary>
        /// 외부접속 로그인 인증코드 저장.
        /// </summary>
        /// <param name="code"></param>
        /// <returns></returns>
        public async Task CreateAuthCodeOneAsync(auth_code_temp code)
        {
            try
            {
               db.Entry(code).State = EntityState.Added;

                //저장
                await db.SaveChangesAsync();
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        /// <summary>
        /// 외부접속 로그인 인증코드 저장.
        /// </summary>
        /// <param name="code"></param>
        /// <returns></returns>
        public List<uv_user> ListUvUser()
        {
            try
            {
                return db.uv_users.ToList();

                //저장
               
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public uv_user SelectUser(int user_seq)
        {
            try
            {
                return db.uv_users.Where(x => x.uv_seq == user_seq).FirstOrDefault();

                //저장

            }
            catch (Exception e)
            {
                throw e;
            }
        }


        public void UpadateCryptPasswordByUserAll(List<uv_user> listUser)
        {
            try
            {
                using (var tran = db.Database.BeginTransaction())
                {
                    foreach (uv_user user in listUser)
                    {
                        db.uv_users.Attach(user);
                        db.Entry(user).Property(x => x.pwd).IsModified = true;
                        db.SaveChanges();

                    }
                    tran.Commit();
                }
                
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public void UpdateCryptPassword(uv_user user)
        {
            try
            {
                db.uv_users.Attach(user);
                db.Entry(user).Property(x => x.pwd).IsModified = true;
                db.SaveChanges();
            }
            catch (Exception e)
            {
                throw e;
            }
        }


    }
}
