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


namespace Univision.Core.Repositories
{
    public class AdminEntityRepository : BaseRepository
    {
        public AdminEntityRepository(UnivisionContext db) : base(db)
        {

        }
        public AdminEntityRepository()
        {

        }
        
        public async Task<uv_user> SelectUserOneAsync(int uv_seq)
        {
            try
            {
                return await db.uv_users
                                .Where(x => x.uv_seq == uv_seq)
                                .FirstOrDefaultAsync();
            }
            catch (Exception e)
            {
                throw e;
            }
        }
        
        public async Task DeleteUser(uv_user model, string event_type = "", int event_uv_seq = 0, int event_idx = 0)
        {
            try
            {

                db.Entry(model).State = EntityState.Deleted;
                AccountLogRepository log = new AccountLogRepository();
                await log.uv_user_log(model, event_type, event_uv_seq, event_idx);

                await db.SaveChangesAsync();
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public async Task CreateOrUpdateUser(uv_user model, string state, string event_type = "", int event_uv_seq = 0, int event_idx = 0)
        {
            try
            {
                using (var tran = db.Database.BeginTransaction())
                {
                    try
                    {
                        if (state == "U")
                        {
                            db.Entry(model).State = EntityState.Modified;
                            AccountLogRepository log = new AccountLogRepository();
                            await log.uv_user_log(model, state, event_uv_seq, 1);
                        }
                        else
                        {
                            db.Entry(model).State = EntityState.Added;
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
        }/*

        public async Task CreateOrUpdateBoardFile(BoardCreateUpdateModel model, string state)
        {
            try
            {
                using (var tran = db.Database.BeginTransaction())
                {
                    try
                    {
                        if (state == "U")
                        {

                            if (model.deleteFileList.Count > 0)
                                foreach (var file in model.deleteFileList)
                                {
                                    db.Entry(file).State = EntityState.Deleted;
                                }
                        }

                        //파일
                        if (model.boardFileList != null)
                            foreach (var file in model.boardFileList)
                            {
                                if (file.file_dir != "")
                                {
                                    file.b_seq = model.data.b_seq;

                                    db.Entry(file).State = EntityState.Added;
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
        }*/

    }
}
