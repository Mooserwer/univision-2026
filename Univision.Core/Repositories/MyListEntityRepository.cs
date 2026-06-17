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
using Univision.Core.Models.DTO.Request.MyList;

namespace Univision.Core.Repositories
{
    public class MyListEntityRepository : BaseRepository
    {
        public MyListEntityRepository(UnivisionContext db) : base(db)
        {

        }
        public MyListEntityRepository()
        {

        }

        /// <summary>
        /// 후보자 메모 등록 or 삭제
        /// </summary>
        /// <param name="cm"></param>
        /// <param name="flag"></param>
        /// <returns></returns>
        public async Task CreateOrDeleteUserAttend(List<uv_user_attend> attendList)
        {
            try
            {
                foreach(var attend in attendList)
                {
                        db.Entry(attend).State = EntityState.Added;
                }
                
                await db.SaveChangesAsync();
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public async Task CreateMyVacation(VacationCreateModel model, alarm_message alarm, List<alarm_user> userList)
        {
            try
            {
                using (var tran = db.Database.BeginTransaction())
                {
                    try
                    {
                        //휴가 마스터 저장
                        db.Entry(model.data).State = EntityState.Added;
                        await db.SaveChangesAsync();
                    
                        //휴가 디테일
                        if (model.detail_list != null)
                            foreach (var vacation_detail in model.detail_list)
                            {
                                vacation_detail.v_seq = model.data.v_seq;
                                db.Entry(vacation_detail).State = EntityState.Added;
                            }

                        if(alarm != null)
                            db.Entry(alarm).State = EntityState.Added;

                        //저장
                        await db.SaveChangesAsync();

                        if (userList != null)
                            foreach(var user in userList)
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

    public async Task<receipt_user> SelectReceiptUserOneAsync(int ra_seq)
    {
      try
      {
        return await db.receipt_users
                       .Where(x => x.ra_seq == ra_seq)
                       .FirstOrDefaultAsync();
      }
      catch (Exception e)
      {
        throw e;
      }
    }

    public async Task<receipt_user_dtl> SelectReceiptUserDtlOneAsync(int ra_seq)
    {
      try
      {
        return await db.receipt_user_dtls
                       .Where(x => x.ra_seq == ra_seq)
                       .FirstOrDefaultAsync();
      }
      catch (Exception e)
      {
        throw e;
      }
    }

    public async Task<receipt_user_dtl> SelectReceiptUserDtlOneAsync(int ra_seq, int rad_seq)
    {
      try
      {
        return await db.receipt_user_dtls
                       .Where(x => x.ra_seq == ra_seq && x.rad_seq == rad_seq)
                       .FirstOrDefaultAsync();
      }
      catch (Exception e)
      {
        throw e;
      }
    }

    public async Task CreateMobileReceipt(receipt_user ru, receipt_user_dtl rud)
    {
      try
      {
        using (var tran = db.Database.BeginTransaction())
        {
          try
          {
            //휴가 마스터 저장
            db.Entry(ru).State = EntityState.Modified;
            await db.SaveChangesAsync();

            //휴가 디테일
            if (rud != null)
              if(rud.rad_seq > 0)
                db.Entry(rud).State = EntityState.Modified;
              else
                db.Entry(rud).State = EntityState.Added;

            //저장
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

    public async Task CreateCoCardReceipt(receipt_user ru, List<receipt_user_dtl> rud)
    {
      try
      {
        using (var tran = db.Database.BeginTransaction())
        {
          try
          {
            //휴가 마스터 저장
            db.Entry(ru).State = EntityState.Modified;
            await db.SaveChangesAsync();

            //휴가 디테일
            if (rud.Count > 0)
              foreach(var data in rud)
              {
                db.Entry(data).State = EntityState.Modified;
              }              
            //저장
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


    public async Task<uv_vacation_history> SelectVacationOneAsync(int v_seq)
        {
            try
            {
                return await db.uv_vacation_historys
                               .Where(x => x.v_seq == v_seq)
                               .FirstOrDefaultAsync();
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public async Task CreateVacationApprAlarm(uv_vacation_history v_history, alarm_message aMessage, List<alarm_user> aList)
        {
            try
            {
                using (var tran = db.Database.BeginTransaction())
                {
                    try
                    {
                        db.Entry(v_history).State = EntityState.Modified;

                        db.Entry(aMessage).State = EntityState.Added;

                        //저장
                        await db.SaveChangesAsync();


                        foreach (var aUser in aList)
                        {
                            aUser.am_seq = aMessage.am_seq;
                            db.Entry(aUser).State = EntityState.Added;
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
            catch (Exception e)
            {
                throw e;
            }
        }

    }
    
}
