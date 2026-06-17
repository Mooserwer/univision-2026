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
    public class BoardEntityRepository : BaseRepository
    {
        public BoardEntityRepository(UnivisionContext db) : base(db)
        {

        }
        public BoardEntityRepository()
        {

        }

        public async Task<board> SelectBoardOneAsync(int b_seq)
        {
            try
            {
                return await db.board
                               .Where(x => x.b_seq == b_seq)
                               .FirstOrDefaultAsync();
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public async Task<board_read_history> SelectBoardReadOneAsync(int b_seq, int uv_seq)
        {
            try
            {
                return await db.board_read_historys
                               .Where(x => (x.c_seq == b_seq && x.read_user == uv_seq))
                               .FirstOrDefaultAsync();
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public async Task CreateOrUpdateBoardRead(board_read_history model, string state)
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
        }

        public async Task DeleteBoard(BoardCreateUpdateModel model)
        {
            try
            {

                if (model.deleteFileList.Count > 0)
                    foreach (var file in model.deleteFileList)
                    {
                        db.Entry(file).State = EntityState.Deleted;
                    }

                db.Entry(model.data).State = EntityState.Deleted;
                

                await db.SaveChangesAsync();
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public async Task CreateOrUpdateOnlyBoard(BoardCreateUpdateModel model, string state)
        {
            try
            {
                using (var tran = db.Database.BeginTransaction())
                {
                    try
                    {
                        if (state == "U")
                        {
                            db.Entry(model.data).State = EntityState.Modified;
                        }
                        else
                        {
                            db.Entry(model.data).State = EntityState.Added;
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
        }


        public async Task CreateAlarm(alarm_message alarm, List<alarm_user> userList)
        {
            try
            {
                using (var tran = db.Database.BeginTransaction())
                {
                    try
                    {

                        if (alarm != null)
                            db.Entry(alarm).State = EntityState.Added;

                        //저장
                        await db.SaveChangesAsync();

                        if (userList != null)
                            foreach (var user in userList)
                            {
                                user.am_seq = alarm.am_seq;
                                db.Entry(user).State = EntityState.Added;
                            }

                        await db.SaveChangesAsync();

                        tran.Commit();
                    }
                    catch (DbEntityValidationException dbEx)
                    {
                        foreach (var validationErrors in dbEx.EntityValidationErrors)
                        {
                            foreach (var validationError in validationErrors.ValidationErrors)
                            {
                                System.Console.WriteLine("Property: {0} Error: {1}", validationError.PropertyName, validationError.ErrorMessage);
                            }
                        }
                        tran.Rollback();
                        throw dbEx;
                    }
                }
            }
            catch (Exception e)
            {
                throw e;
            }
        }
    }
}
