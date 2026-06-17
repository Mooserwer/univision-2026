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
using Univision.Core.Models.DTO.Request.Client;
using Univision.Core.Models.DTO.Response.Client;

namespace Univision.Core.Repositories
{
  public class ClientEntityRepository : BaseRepository
  {
    public ClientEntityRepository(UnivisionContext db) : base(db)
    {

    }

    public ClientEntityRepository()
    {

    }

    public async Task<List<OfflimitCheckModel>> SelectOfflimitCheckListAsync()
    {
      try
      {
        var offlimit_clients = from CL in db.clients
                               where CL.offlimit > 0
                               select new OfflimitCheckModel()
                               {
                                 kor_name = CL.kor_name,
                                 eng_name = CL.eng_name,
                                 kor_name_trim = CL.kor_name.Replace(" ", ""),
                                 eng_name_trim = CL.eng_name.Replace(" ", ""),
                                 name_all = CL.kor_name + "_" + CL.eng_name,
                                 name_all_trim = CL.kor_name.Replace(" ", "") + "_" + CL.eng_name.Replace(" ", ""),
                                 offlimit = CL.offlimit.HasValue ? CL.offlimit.Value : 0
                               };


        return await offlimit_clients.OrderBy(x => x.kor_name).ToListAsync();
      }
      catch (Exception e)
      {
        throw e;
      }
    }




    public async Task<client> SelectClientOneAsync(int c_seq)
    {
      try
      {
        return await db.clients
                       .Where(x => x.c_seq == c_seq)
                       .FirstOrDefaultAsync();
      }
      catch (Exception e)
      {
        throw e;
      }
    }



    /// <summary>
    /// 클라이언트 단건 업데이트
    /// </summary>
    /// <param name="client"></param>
    /// <returns></returns>
    public async Task UpdateClientOneAsync(client client)
    {
      try
      {
        db.Entry(client).State = EntityState.Modified;
        await db.SaveChangesAsync();
      }
      catch (Exception e)
      {
        throw e;
      }
    }

    public async Task CreateOrUpdateClient(ClientCreateUpdateModel model, string state, int event_user = 0)
    {
      try
      {
        ClientLogRepository log = new ClientLogRepository();
        using (var tran = db.Database.BeginTransaction())
        {
          try
          {
            if (state == "U")
            {
              if (model.deleteAmList.Count > 0)
              {
                int idx = 0;
                foreach (var dAm in model.deleteAmList)
                {
                  db.Entry(dAm).State = EntityState.Deleted;

                  idx++;
                }
              }
              db.Entry(model.data).State = EntityState.Modified;
              await log.client_log(model.data, state, event_user, 0);

              /*
              if (model.data2.cc_seq != 0)
              {
                  db.Entry(model.data2).State = EntityState.Modified;
              }


              if (model.data3.ctc_seq != 0)
              {
                  db.Entry(model.data3).State = EntityState.Modified;
              }
              */

            }
            else
            {
              db.Entry(model.data).State = EntityState.Added;
              await db.SaveChangesAsync();


              if (model.data2.name != null)
              {
                model.data2.c_seq = model.data.c_seq;
                db.Entry(model.data2).State = EntityState.Added;
              }


              if (model.data3.name != null)
              {
                model.data3.c_seq = model.data.c_seq;
                db.Entry(model.data3).State = EntityState.Added;
              }

            }
            await db.SaveChangesAsync();

            if (model.amList != null)
              foreach (var am in model.amList)
              {
                am.c_seq = model.data.c_seq;
                db.Entry(am).State = EntityState.Added;
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

    public async Task<client_contract> SelectClientLatestContractOneAsync(int c_seq)
    {
      try
      {
        return await db.client_contracts
                        .Where(x => x.c_seq == c_seq)
                        .OrderByDescending(x => x.cc_seq)
                        .FirstOrDefaultAsync();
      }
      catch (Exception e)
      {
        throw e;
      }
    }

    public async Task<client_contract> SelectClientContractOneAsync(int cc_seq)
    {
      try
      {
        return await db.client_contracts
                        .Where(x => x.cc_seq == cc_seq)
                        .FirstOrDefaultAsync();
      }
      catch (Exception e)
      {
        throw e;
      }
    }
    public async Task CreateContract(string edit_mode, client_contract contract, List<client_annual_income_rate> annualIncomeList, string state, int event_user = 0)
    {
      try
      {
        ClientLogRepository log = new ClientLogRepository();
        using (var tran = db.Database.BeginTransaction())
        {
          try
          {
            if (state == "U")
            {
              db.Entry(contract).State = EntityState.Modified;
              await log.client_contract_log(contract, 1, "U", event_user);
            }
            else
            {
              db.Entry(contract).State = EntityState.Added;
            }

            await db.SaveChangesAsync();

            if (contract.fee_type == "A" || contract.fee_type == "B")
            {
              if (annualIncomeList != null)
              {
                foreach (var annual in annualIncomeList)
                {
                  annual.c_seq = contract.c_seq;
                  annual.cc_seq = contract.cc_seq;
                  db.Entry(annual).State = EntityState.Added;
                }
              }
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


    public async Task CreateOrUpdateClientContract(client_contract contract, List<client_annual_income_rate> annualIncomeList, List<client_position_rate> positionRateList, string state, int event_user = 0)
    {
      try
      {
        ClientLogRepository log = new ClientLogRepository();
        using (var tran = db.Database.BeginTransaction())
        {
          try
          {
            if (state == "U")
            {
              db.Entry(contract).State = EntityState.Modified;
              await log.client_contract_log(contract, 1, "U", event_user);
            }
            else
            {
              db.Entry(contract).State = EntityState.Added;
            }

            await db.SaveChangesAsync();

            if (contract.fee_type == "A")
            {
              if (annualIncomeList != null)
              {
                var originList = await SelectClientAnnualIncomeRateList(contract.c_seq, contract.cc_seq);
                if (originList != null)
                {
                  //예전 수수료율 삭제
                  foreach (var origin in originList)
                  {
                    db.Entry(origin).State = EntityState.Deleted;
                  }
                }

                var positionList = await SelectClientPositionRateList(contract.c_seq, contract.cc_seq);
                if (positionList != null)
                {
                  //직급별 수수료율 삭제
                  foreach (var position in positionList)
                  {
                    db.Entry(position).State = EntityState.Deleted;
                  }
                }

                foreach (var annual in annualIncomeList)
                {
                  annual.c_seq = contract.c_seq;
                  annual.cc_seq = contract.cc_seq;
                  db.Entry(annual).State = EntityState.Added;
                }
              }
            }
            else if (contract.fee_type == "B")
            {
              if (positionRateList != null)
              {
                var originList = await SelectClientPositionRateList(contract.c_seq, contract.cc_seq);
                if (originList != null)
                {
                  //예전 수수료율 삭제
                  foreach (var origin in originList)
                  {
                    db.Entry(origin).State = EntityState.Deleted;
                  }
                }

                var annualList = await SelectClientAnnualIncomeRateList(contract.c_seq, contract.cc_seq);
                if (annualList != null)
                {
                  //연봉별 수수료율 삭제
                  foreach (var annual in annualList)
                  {
                    db.Entry(annual).State = EntityState.Deleted;
                  }
                }

                foreach (var position in positionRateList)
                {
                  position.c_seq = contract.c_seq;
                  position.cc_seq = contract.cc_seq;
                  db.Entry(position).State = EntityState.Added;
                }
              }
            }
            else
            {
              var annualList = await SelectClientAnnualIncomeRateList(contract.c_seq, contract.cc_seq);
              if (annualList != null)
              {
                //연봉별 수수료율 삭제
                foreach (var annual in annualList)
                {
                  db.Entry(annual).State = EntityState.Deleted;
                }
              }

              var positionList = await SelectClientPositionRateList(contract.c_seq, contract.cc_seq);
              if (positionList != null)
              {
                //직급별 수수료율 삭제
                foreach (var position in positionList)
                {
                  db.Entry(position).State = EntityState.Deleted;
                }
              }
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

    /// <summary>
    /// 연봉별 수수료율 select
    /// </summary>
    /// <param name="c_seq"></param>
    /// <param name="cc_seq"></param>
    /// <returns></returns>
    public async Task<List<client_annual_income_rate>> SelectClientAnnualIncomeRateList(int c_seq, int cc_seq)
    {
      try
      {
        return await db.client_annual_income_rates
                        .Where(x => x.c_seq == c_seq && x.cc_seq == cc_seq)
                        .ToListAsync();
      }
      catch (Exception e)
      {
        throw e;
      }
    }

    public async Task<List<client_position_rate>> SelectClientPositionRateList(int c_seq, int cc_seq)
    {
      try
      {
        return await db.client_position_rates
                        .Where(x => x.c_seq == c_seq && x.cc_seq == cc_seq)
                        .ToListAsync();
      }
      catch (Exception e)
      {
        throw e;
      }
    }

    public async Task<List<client_manager>> SelectClientAmListAsync(int c_seq)
    {
      try
      {
        return await db.client_manager
                        .Where(x => x.c_seq == c_seq)
                        .ToListAsync();
      }
      catch (Exception e)
      {
        throw e;
      }
    }

    public async Task CreateOrUpdateContractFile(client_contract_file cf, string flag, int event_uv_seq = 0)
    {
      try
      {
        if (flag == "D")
        {
          ClientLogRepository log = new ClientLogRepository();
          await log.client_contract_file_log(cf, 1, "D", event_uv_seq);
          db.Entry(cf).State = EntityState.Modified;
        }
        else if (flag == "U")
        {
          ClientLogRepository log = new ClientLogRepository();
          await log.client_contract_file_log(cf, 1, "U", event_uv_seq);
          db.Entry(cf).State = EntityState.Modified;
        }
        else
          db.Entry(cf).State = EntityState.Added;

        await db.SaveChangesAsync();
      }
      catch (Exception e)
      {
        throw e;
      }
    }


    //public async Task CreateOrDeleteClientFile(client_file cf, string flag)
    //{
    //    try
    //    {
    //        if (flag == "D")
    //            db.Entry(cf).State = EntityState.Modified;
    //        else
    //            db.Entry(cf).State = EntityState.Added;

    //        await db.SaveChangesAsync();
    //    }
    //    catch (Exception e)
    //    {
    //        throw;
    //    }
    //}

    public async Task<client_file> SelectClientFileOneAsync(int c_seq, int cf_seq)
    {
      try
      {
        return await db.client_files
                        .Where(x => x.c_seq == c_seq && x.cf_seq == cf_seq)
                        .FirstOrDefaultAsync();
      }
      catch (Exception e)
      {
        throw e;
      }
    }

    public async Task CreateOrDeleteClientFile(client_file cr, string flag, int event_user = 0)
    {
      try
      {
        if (flag == "D")
        {
          ClientLogRepository log = new ClientLogRepository();
          await log.client_file_log(cr, 1, "D", event_user);
          db.Entry(cr).State = EntityState.Deleted;
        }

        else
          db.Entry(cr).State = EntityState.Added;

        await db.SaveChangesAsync();
      }
      catch (Exception e)
      {
        throw e;
      }
    }


    public async Task<client_favorite> SelectCliAttentionOneAsync(int c_seq, int user_seq)
    {
      try
      {
        return await db.client_favorites
                       .Where(x => x.c_seq == c_seq && x.uv_seq == user_seq)
                       .FirstOrDefaultAsync();
      }
      catch (Exception e)
      {
        throw e;
      }
    }

    public async Task<client_favorite> SelectCliAttentionOneAsync(int cf_seq)
    {
      try
      {
        return await db.client_favorites
                       .Where(x => x.cf_seq == cf_seq)
                       .FirstOrDefaultAsync();
      }
      catch (Exception e)
      {
        throw e;
      }
    }

    public async Task CreateOrDeleteCliAttention(client_favorite cf, string flag, int event_user = 0)
    {
      try
      {
        if (flag == "D")
        {
          ClientLogRepository log = new ClientLogRepository();
          await log.client_favorite_log(cf, 1, "D", event_user);
          db.Entry(cf).State = EntityState.Deleted;
        }
        else
          db.Entry(cf).State = EntityState.Added;

        await db.SaveChangesAsync();
      }
      catch (Exception e)
      {
        throw e;
      }
    }


    public async Task<client_contact> SelectCliContactOneAsync(int cc_seq)
    {
      try
      {
        return await db.client_contacts
                       .Where(x => x.cc_seq == cc_seq)
                       .FirstOrDefaultAsync();
      }
      catch (Exception e)
      {
        throw e;
      }
    }

    public async Task<client_tax_contact> SelectCliTaxContactOneAsync(int ctc_seq)
    {
      try
      {
        return await db.client_tax_contacts
                       .Where(x => x.ctc_seq == ctc_seq)
                       .FirstOrDefaultAsync();
      }
      catch (Exception e)
      {
        throw e;
      }
    }

    public async Task<client_contract_file> SelectContractFileAsync(int cf_seq)
    {
      try
      {
        return await db.client_Contract_files
                        .Where(x => x.cf_seq == cf_seq)
                        .FirstOrDefaultAsync();
      }
      catch (Exception e)
      {
        throw e;
      }
    }

    public async Task<client_contract_file> SelectContractFileAsync(int c_seq, string fileName)
    {
      try
      {
        return await db.client_Contract_files
                        .Where(x => x.c_seq == c_seq && x.file_path == fileName)
                        .FirstOrDefaultAsync();
      }
      catch (Exception e)
      {
        throw e;
      }
    }


    public async Task CreateOrDeleteCliContact(client_contact cc, string flag, int event_user = 0)
    {
      try
      {
        if (flag == "D")
        {
          ClientLogRepository log = new ClientLogRepository();
          await log.client_contact_log(cc, flag, event_user, 1);
          db.Entry(cc).State = EntityState.Deleted;
        }

        else
          db.Entry(cc).State = EntityState.Added;

        await db.SaveChangesAsync();
      }
      catch (Exception e)
      {
        throw e;
      }
    }



    public async Task<client_activity_log> selectActivityOneAsync(int cal_seq)
    {
      try
      {
        return await db.client_activity_logs
                       .Where(x => x.cal_seq == cal_seq)
                       .FirstOrDefaultAsync();
      }
      catch (Exception e)
      {
        throw e;
      }
    }

    /// <summary>
    /// 활동로그 신규, 수정, 삭제
    /// </summary>
    /// <param name="cal"></param>
    /// <param name="flag">C:신규, U:수정, D:삭제</param>
    /// <returns></returns>
    public async Task CreateOrUpdateActivity(client_activity_log cal, string flag, int user_seq)
    {
      try
      {
        if (flag == "D")
          db.Entry(cal).State = EntityState.Deleted;
        else if (flag == "U")
          db.Entry(cal).State = EntityState.Modified;
        else
          db.Entry(cal).State = EntityState.Added;
        ClientLogRepository log = new ClientLogRepository();
        await log.client_activity_log_log(cal, flag, user_seq, 1);
        await db.SaveChangesAsync();
      }
      catch (Exception e)
      {
        throw e;
      }
    }

    /// <summary>
    /// 클라이언트 세금계산서 담당자 등록
    /// </summary>
    /// <param name="tax"></param>
    /// <param name="flag"></param>
    /// <returns></returns>
    public async Task CreateOrUpdateTaxContact(client_tax_contact tax, string flag)
    {
      try
      {
        if (flag == "U")
          db.Entry(tax).State = EntityState.Modified;
        else
          db.Entry(tax).State = EntityState.Added;

        await db.SaveChangesAsync();
      }
      catch (Exception e)
      {
        throw e;
      }
    }

    public async Task<element_open_log> SelectClientOpenLogOneAsync(int c_seq, int user_seq)
    {
      try
      {
        var date_time = DateTime.Now.AddDays(-1);
        return await db.element_open_logs
                       .Where(x => x.element_seq == c_seq && x.uv_seq == user_seq && x.page_name == "client" && x.table_name == "client" && x.log_date > date_time)
                       .FirstOrDefaultAsync();
      }
      catch (Exception e)
      {
        throw e;
      }
    }
  }
}
