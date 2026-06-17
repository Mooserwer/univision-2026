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
    public class ExchangeEntityRepository : BaseRepository
    {
        public ExchangeEntityRepository(UnivisionContext db) : base(db)
        {

        }
        public ExchangeEntityRepository()
        {

        }


        public async Task<exchange> SelectExchangeOneAsync(int ex_seq)
        {
            try
            {
                return await db.exchanges
                               .Where(x => x.ex_seq == ex_seq)
                               .FirstOrDefaultAsync();
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public async Task<exchange> SelectExchangeOneAsync(string date, string currency)
        {
            try
            {
                return await db.exchanges
                               .Where(x => x.ex_date == date && x.ex_code == currency)
                               .FirstOrDefaultAsync();
            }
            catch (Exception e)
            {
                throw e;
            }
        }

    public async Task<exchange> SelectExchangeOneAsync(string currency)
    {
      try
      {
        return await db.exchanges
                       .Where(x => x.ex_code == currency)
                       .OrderByDescending(x => x.ex_date)
                       .FirstOrDefaultAsync();
      }
      catch (Exception e)
      {
        throw e;
      }
    }

    public async Task CreateExchange(List<exchange> exch_list)

        {
            try
            {
                using (var tran = db.Database.BeginTransaction())
                {
                    try
                    {
                        foreach (var exch in exch_list)
                        {
                            if (exch.ex_seq > 0)
                            {
                                db.Entry(exch).State = EntityState.Modified;
                            }
                            else
                            {
                                db.Entry(exch).State = EntityState.Added;
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
        public async Task CreateExchange1(List<exchange> exch_list)
        {
            try
            {
                //using (var tran = db.Database.BeginTransaction())
                //{
                    try
                    {
                        foreach (var exch in exch_list)
                        {
                            /*
                            if (exch.ex_seq > 0)
                            {
                                db.Entry(exch).State = EntityState.Modified;
                            } else
                            {
                                db.Entry(exch).State = EntityState.Added;
                            }
                            */
                            db.Entry(exch).State = EntityState.Added;

                        }

                        //저장

                        await db.SaveChangesAsync();
                 //       tran.Commit();
                    }
                    catch (Exception e)
                    {
                 //       tran.Rollback();
                        throw e;
                    }
                //}
            }
            catch (Exception e)
            {
                throw e;
            }
        }

    }
}
