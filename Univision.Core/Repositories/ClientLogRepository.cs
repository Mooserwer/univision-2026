using Dapper;
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
using Univision.Core.Models.DTO.Logs;
using Univision.Core.Models.DTO.Request;
using Univision.Core.Models.DTO.Request.Client;
namespace Univision.Core.Repositories
{
  public class ClientLogRepository : BaseRepository
  {
    public ClientLogRepository(UnivisionContext db) : base(db)
    {

    }
    public ClientLogRepository()
    {

    }
    public async Task client_contract_log_fee_type(int cc_seq, int idx = 1, string event_type = "", int event_user = 0)
    {
      try
      {
        SqlParameter event_idx = new SqlParameter("@event_idx", idx);
        SqlParameter param_event_type = new SqlParameter("@event_type", event_type);
        SqlParameter event_uv_seq = new SqlParameter("@event_uv_seq", event_user);
        SqlParameter db_cc_seq = new SqlParameter("@cc_seq", cc_seq);


        await db.Database.ExecuteSqlCommandAsync("exec sp_client_contract_log_fee_type", event_idx, param_event_type, event_uv_seq
            , db_cc_seq);

      }
      catch //(Exception e)
      {

      }
    }
    public async Task client_contract_log(client_contract data, int idx = 1, string event_type = "", int event_user = 0)
    {
      try
      {
        SqlParameter event_idx = new SqlParameter("@event_idx", idx);
        SqlParameter param_event_type = new SqlParameter("@event_type", event_type);
        SqlParameter event_uv_seq = new SqlParameter("@event_uv_seq", event_user);
        SqlParameter cc_seq = new SqlParameter("@cc_seq", data.cc_seq);
        SqlParameter c_seq = new SqlParameter("@c_seq", data.c_seq);
        SqlParameter fee_type = new SqlParameter("@fee_type", (object)data.fee_type ?? DBNull.Value);
        SqlParameter deposit_limit = new SqlParameter("@deposit_limit", (object)data.deposit_limit ?? DBNull.Value);
        SqlParameter guarntee_type = new SqlParameter("@guarntee_type", (object)data.guarntee_type ?? DBNull.Value);
        SqlParameter is_construct_debut = new SqlParameter("@is_construct_debut", (object)data.is_construct_debut ?? DBNull.Value);
        SqlParameter fix_fee_rate = new SqlParameter("@fix_fee_rate", data.fix_fee_rate);
        SqlParameter contract_comment = new SqlParameter("@contract_comment", (object)data.contract_comment ?? DBNull.Value);

        await db.Database.ExecuteSqlCommandAsync(@"exec sp_client_contract_log
                      @event_idx, @event_type, @event_uv_seq
                    , @cc_seq, @c_seq, @fee_type, @deposit_limit, @guarntee_type, @is_construct_debut, @fix_fee_rate, @contract_comment",
             event_idx, param_event_type, event_uv_seq
            , cc_seq, c_seq, fee_type, deposit_limit, guarntee_type, is_construct_debut, fix_fee_rate, contract_comment);

      }
      catch //(Exception e)
      {

      }
    }

    public async Task client_tax_contact_log(client_tax_contact data, int idx = 1, string event_type = "", int event_user = 0)
    {
      try
      {
        SqlParameter event_idx = new SqlParameter("@event_idx", idx);
        SqlParameter param_event_type = new SqlParameter("@event_type", event_type);
        SqlParameter event_uv_seq = new SqlParameter("@event_uv_seq", event_user);
        SqlParameter ctc_seq = new SqlParameter("@ctc_seq", data.ctc_seq);
        SqlParameter c_seq = new SqlParameter("@c_seq", data.c_seq);
        SqlParameter division = new SqlParameter("@division", (object)data.division ?? DBNull.Value);
        SqlParameter name = new SqlParameter("@name", (object)data.name ?? DBNull.Value);
        SqlParameter email = new SqlParameter("@email", (object)data.email ?? DBNull.Value);
        SqlParameter phone = new SqlParameter("@phone", (object)data.phone ?? DBNull.Value);
        SqlParameter cell_phone = new SqlParameter("@cell_phone", (object)data.cell_phone ?? DBNull.Value);
        SqlParameter deposit_manager = new SqlParameter("@deposit_manager", (object)data.deposit_manager ?? DBNull.Value);
        SqlParameter deposit_email = new SqlParameter("@deposit_email", (object)data.deposit_email ?? DBNull.Value);


        await db.Database.ExecuteSqlCommandAsync(@"exec sp_client_tax_contact_log
                      @event_idx, @event_type, @event_uv_seq
                    , @ctc_seq, @c_seq, @division,name, @email,@phone,@cell_phone, @deposit_manager, @deposit_email
"
            , event_idx, param_event_type, event_uv_seq
            , ctc_seq, c_seq, division, name, email, phone, cell_phone, deposit_manager, deposit_email);

      }
      catch //(Exception e)
      {

      }
    }

    public async Task client_contact_log(client_contact data, string event_type = "", int event_uv_seq = 0, int event_idx = 0)
    {
      try
      {
        SqlParameter p_event_idx = new SqlParameter("@event_idx", event_idx);
        SqlParameter p_event_type = new SqlParameter("@event_type", event_type);
        SqlParameter p_event_uv_seq = new SqlParameter("@event_uv_seq", event_uv_seq);
        SqlParameter p_cc_seq = new SqlParameter("@cc_seq", SqlDbType.Int);
        p_cc_seq.Value = data.cc_seq;

        SqlParameter p_c_seq = new SqlParameter("@c_seq", SqlDbType.Int);
        p_c_seq.Value = data.c_seq;

        SqlParameter p_name = new SqlParameter("@name", SqlDbType.VarChar, 50);
        p_name.Value = data.name as object ?? DBNull.Value;

        SqlParameter p_gender = new SqlParameter("@gender", SqlDbType.Int);
        p_gender.Value = data.gender;

        SqlParameter p_email = new SqlParameter("@email", SqlDbType.VarChar, 150);
        p_email.Value = data.email as object ?? DBNull.Value;

        SqlParameter p_phone = new SqlParameter("@phone", SqlDbType.VarChar, 30);
        p_phone.Value = data.phone as object ?? DBNull.Value;

        SqlParameter p_cell_phone = new SqlParameter("@cell_phone", SqlDbType.VarChar, 30);
        p_cell_phone.Value = data.cell_phone as object ?? DBNull.Value;

        SqlParameter p_division = new SqlParameter("@division", SqlDbType.VarChar, 50);
        p_division.Value = data.division as object ?? DBNull.Value;

        SqlParameter p_position = new SqlParameter("@position", SqlDbType.VarChar, 50);
        p_position.Value = data.position as object ?? DBNull.Value;

        SqlParameter p_memo = new SqlParameter("@memo", SqlDbType.VarChar, 500);
        p_memo.Value = data.memo as object ?? DBNull.Value;



        await db.Database.ExecuteSqlCommandAsync(@"exec sp_client_contact_log  @event_idx, @event_type, @event_uv_seq,	  @cc_seq , @c_seq , @name , @gender , @email , @phone , @cell_phone , @division , @position , @memo  "
        , p_event_idx, p_event_type, p_event_uv_seq
         , p_cc_seq
        , p_c_seq
        , p_name
        , p_gender
        , p_email
        , p_phone
        , p_cell_phone
        , p_division
        , p_position
        , p_memo

        );
      }
      catch // (Exception e)
      {
      }
    }


    public async Task client_favorite_log(client_favorite data, int idx = 1, string event_type = "", int event_user = 0)
    {
      try
      {
        SqlParameter event_idx = new SqlParameter("@event_idx", idx);
        SqlParameter param_event_type = new SqlParameter("@event_type", event_type);
        SqlParameter event_uv_seq = new SqlParameter("@event_uv_seq", event_user);
        SqlParameter cf_seq = new SqlParameter("@cf_seq", data.cf_seq);
        SqlParameter c_seq = new SqlParameter("@c_seq", data.c_seq);
        SqlParameter uv_seq = new SqlParameter("@uv_seq", data.uv_seq);


        await db.Database.ExecuteSqlCommandAsync("exec sp_client_favorite_log", event_idx, param_event_type, event_uv_seq, cf_seq, c_seq, uv_seq);

      }
      catch //(Exception e)
      {

      }
    }

    public async Task client_file_log(client_file data, int idx = 1, string event_type = "", int event_user = 0)
    {
      try
      {
        SqlParameter event_idx = new SqlParameter("@event_idx", idx);
        SqlParameter param_event_type = new SqlParameter("@event_type", event_type);
        SqlParameter event_uv_seq = new SqlParameter("@event_uv_seq", event_user);
        SqlParameter cf_seq = new SqlParameter("@cf_seq", data.cf_seq);
        SqlParameter c_seq = new SqlParameter("@c_seq", data.c_seq);
        SqlParameter cf_type = new SqlParameter("@cf_type", data.cf_type);
        SqlParameter directory = new SqlParameter("@directory", data.directory);
        SqlParameter origin_file_path = new SqlParameter("@origin_file_path", data.origin_file_path);
        SqlParameter file_path = new SqlParameter("@file_path", data.file_path);
        SqlParameter extension = new SqlParameter("@extension", data.extension);


        await db.Database.ExecuteSqlCommandAsync("exec sp_client_file_log", event_idx, param_event_type, event_uv_seq, cf_seq, c_seq, cf_type
            , directory, origin_file_path, origin_file_path, file_path, extension);

      }
      catch //(Exception e)
      {

      }
    }

    public async Task client_contract_file_log(client_contract_file data, int idx = 1, string event_type = "", int event_user = 0)
    {
      try
      {
        SqlParameter event_idx = new SqlParameter("@event_idx", idx);
        SqlParameter param_event_type = new SqlParameter("@event_type", event_type);
        SqlParameter event_uv_seq = new SqlParameter("@event_uv_seq", event_user);
        SqlParameter cf_seq = new SqlParameter("@cf_seq", data.cf_seq);
        SqlParameter c_seq = new SqlParameter("@c_seq", data.c_seq);
        SqlParameter file_type = new SqlParameter("@file_type", data.file_type);
        SqlParameter file_dir = new SqlParameter("@file_dir", data.file_dir);
        SqlParameter file_origin_path = new SqlParameter("@file_origin_path", data.file_origin_path);
        SqlParameter file_path = new SqlParameter("@file_path", data.file_path);
        SqlParameter file_extension = new SqlParameter("@file_extension", data.file_extension);
        SqlParameter remove_dt = new SqlParameter("@remove_dt", data.remove_dt);
        SqlParameter remove_user = new SqlParameter("@remove_user", data.remove_user);


        await db.Database.ExecuteSqlCommandAsync("exec sp_client_contract_file_log", event_idx, param_event_type, event_uv_seq, cf_seq, c_seq, file_type
            , file_dir, file_origin_path, file_path, file_extension, remove_dt, remove_user);

      }
      catch //(Exception e)
      {

      }
    }

    public async Task client_manager_log(client_manager data, int idx = 1, string event_type = "", int event_user = 0)
    {
      try
      {
        SqlParameter param_1 = new SqlParameter("@event_idx", idx);
        SqlParameter param_2 = new SqlParameter("@event_type", event_type);
        SqlParameter param_3 = new SqlParameter("@seq", data.seq);
        SqlParameter param_4 = new SqlParameter("@c_seq", data.c_seq);
        SqlParameter param_5 = new SqlParameter("@uv_seq", data.uv_seq);
        SqlParameter param_6 = new SqlParameter("@event_uv_seq", event_user);
        await db.Database.ExecuteSqlCommandAsync("exec sp_client_manager_log", param_1, param_2, param_3, param_4, param_5, param_6);

      }
      catch //(Exception e)
      {

      }
    }

    public async Task client_log(client data, string event_type = "", int event_uv_seq = 0, int event_idx = 0)
    {
      try
      {
        SqlParameter p_event_idx = new SqlParameter("@event_idx", event_idx);
        SqlParameter p_event_type = new SqlParameter("@event_type", event_type);
        SqlParameter p_event_uv_seq = new SqlParameter("@event_uv_seq", event_uv_seq);
        SqlParameter p_c_seq = new SqlParameter("@c_seq", SqlDbType.Int);
        p_c_seq.Value = data.c_seq;

        SqlParameter p_kor_name = new SqlParameter("@kor_name", SqlDbType.VarChar, 100);
        p_kor_name.Value = data.kor_name as object ?? DBNull.Value;

        SqlParameter p_eng_name = new SqlParameter("@eng_name", SqlDbType.VarChar, 100);
        p_eng_name.Value = data.eng_name as object ?? DBNull.Value;

        SqlParameter p_ceo = new SqlParameter("@ceo", SqlDbType.VarChar, 50);
        p_ceo.Value = data.ceo as object ?? DBNull.Value;

        SqlParameter p_addr1 = new SqlParameter("@addr1", SqlDbType.VarChar, 255);
        p_addr1.Value = data.addr1 as object ?? DBNull.Value;

        SqlParameter p_addr2 = new SqlParameter("@addr2", SqlDbType.VarChar, 100);
        p_addr2.Value = data.addr2 as object ?? DBNull.Value;

        SqlParameter p_is_foreign_invest = new SqlParameter("@is_foreign_invest", SqlDbType.Int);
        p_is_foreign_invest.Value = data.is_foreign_invest;

        SqlParameter p_is_contract = new SqlParameter("@is_contract", SqlDbType.Int);
        p_is_contract.Value = data.is_contract;

        SqlParameter p_is_foreign = new SqlParameter("@is_foreign", SqlDbType.Int);
        p_is_foreign.Value = data.is_foreign;

        SqlParameter p_foreign_code = new SqlParameter("@foreign_code", SqlDbType.Char, 10);
        p_foreign_code.Value = data.foreign_code as object ?? DBNull.Value;

        SqlParameter p_country_name = new SqlParameter("@country_name", SqlDbType.VarChar, 50);
        p_country_name.Value = data.country_name as object ?? DBNull.Value;

        SqlParameter p_biz_code = new SqlParameter("@biz_code", SqlDbType.Char, 10);
        p_biz_code.Value = data.biz_code as object ?? DBNull.Value;

        SqlParameter p_biz_type = new SqlParameter("@biz_type", SqlDbType.VarChar, 100);
        p_biz_type.Value = data.biz_type as object ?? DBNull.Value;

        SqlParameter p_biz_type_code = new SqlParameter("@biz_type_code", SqlDbType.Char, 10);
        p_biz_type_code.Value = data.biz_type_code as object ?? DBNull.Value;

        SqlParameter p_biz_category = new SqlParameter("@biz_category", SqlDbType.VarChar, 100);
        p_biz_category.Value = data.biz_category as object ?? DBNull.Value;

        SqlParameter p_biz_category_code = new SqlParameter("@biz_category_code", SqlDbType.Char, 10);
        p_biz_category_code.Value = data.biz_category_code as object ?? DBNull.Value;

        SqlParameter p_biz_industry = new SqlParameter("@biz_industry", SqlDbType.VarChar, 100);
        p_biz_industry.Value = data.biz_industry as object ?? DBNull.Value;

        SqlParameter p_biz_industry_code1 = new SqlParameter("@biz_industry_code1", SqlDbType.Float);
        p_biz_industry_code1.Value = data.biz_industry_code1 ?? 0;

        SqlParameter p_biz_industry_code2 = new SqlParameter("@biz_industry_code2", SqlDbType.Float);
        p_biz_industry_code2.Value = data.biz_industry_code2 ?? 0;

        SqlParameter p_fix_title = new SqlParameter("@fix_title", SqlDbType.VarChar, 150);
        p_fix_title.Value = data.fix_title as object ?? DBNull.Value;

        SqlParameter p_homepage = new SqlParameter("@homepage", SqlDbType.VarChar, 150);
        p_homepage.Value = data.homepage as object ?? DBNull.Value;

        SqlParameter p_employee_number = new SqlParameter("@employee_number", SqlDbType.Int);
        p_employee_number.Value = data.employee_number ?? 0;

        SqlParameter p_sales_amount = new SqlParameter("@sales_amount", SqlDbType.Int);
        p_sales_amount.Value = data.sales_amount ?? 0;

        SqlParameter p_main_contract = new SqlParameter("@main_contract", SqlDbType.Int);
        p_main_contract.Value = data.main_contract ?? 0;

        SqlParameter p_offlimit = new SqlParameter("@offlimit", SqlDbType.Int);
        p_offlimit.Value = data.offlimit ?? 0;

        SqlParameter p_create_dt = new SqlParameter("@create_dt", SqlDbType.DateTime);
        p_create_dt.Value = data.create_dt as object ?? DBNull.Value;

        SqlParameter p_create_user = new SqlParameter("@create_user", SqlDbType.Int);
        p_create_user.Value = data.create_user ?? 0;

        SqlParameter p_modify_dt = new SqlParameter("@modify_dt", SqlDbType.DateTime);
        p_modify_dt.Value = data.modify_dt as object ?? DBNull.Value;

        SqlParameter p_modify_user = new SqlParameter("@modify_user", SqlDbType.Int);
        p_modify_user.Value = data.modify_user ?? 0;

        SqlParameter p_is_inorder = new SqlParameter("@is_inorder", SqlDbType.Int);
        p_is_inorder.Value = data.is_inorder;

        SqlParameter p_offlimit_keyword = new SqlParameter("@offlimit_keyword", SqlDbType.VarChar, 205);
        p_offlimit_keyword.Value = data.offlimit_keyword as object ?? DBNull.Value;

        SqlParameter p_is_portfolio = new SqlParameter("@is_portfolio", SqlDbType.Int);
        p_is_portfolio.Value = data.is_portfolio;



        await db.Database.ExecuteSqlCommandAsync(@"exec sp_client_log  @event_idx, @event_type, @event_uv_seq,	  @c_seq , @kor_name , @eng_name , @ceo , @addr1 , @addr2 , @is_foreign_invest , @is_contract , @is_foreign , @foreign_code , @country_name , @biz_code , @biz_type , @biz_type_code , @biz_category , @biz_category_code , @biz_industry , @biz_industry_code1 , @biz_industry_code2 , @fix_title , @homepage , @employee_number , @sales_amount , @main_contract , @offlimit , @create_dt , @create_user , @modify_dt , @modify_user, @is_inorder, @offlimit_keyword, @is_portfolio  "
        , p_event_idx, p_event_type, p_event_uv_seq
         , p_c_seq
        , p_kor_name
        , p_eng_name
        , p_ceo
        , p_addr1
        , p_addr2
        , p_is_foreign_invest
        , p_is_contract
        , p_is_foreign
        , p_foreign_code
        , p_country_name
        , p_biz_code
        , p_biz_type
        , p_biz_type_code
        , p_biz_category
        , p_biz_category_code
        , p_biz_industry
        , p_biz_industry_code1
        , p_biz_industry_code2
        , p_fix_title
        , p_homepage
        , p_employee_number
        , p_sales_amount
        , p_main_contract
        , p_offlimit
        , p_create_dt
        , p_create_user
        , p_modify_dt
        , p_modify_user
        , p_is_inorder
        , p_offlimit_keyword
        , p_is_portfolio
        );
      }
      catch //(Exception e)
      {
      }
    }

    public async Task client_activity_log_log(client_activity_log data, string event_type = "", int event_uv_seq = 0, int event_idx = 0)
    {
      try
      {
        SqlParameter p_event_idx = new SqlParameter("@event_idx", event_idx);
        SqlParameter p_event_type = new SqlParameter("@event_type", event_type);
        SqlParameter p_event_uv_seq = new SqlParameter("@event_uv_seq", event_uv_seq);
        SqlParameter p_cal_seq = new SqlParameter("@cal_seq", SqlDbType.Int);
        p_cal_seq.Value = data.cal_seq;

        SqlParameter p_c_seq = new SqlParameter("@c_seq", SqlDbType.Int);
        p_c_seq.Value = data.c_seq;

        SqlParameter p_title = new SqlParameter("@title", SqlDbType.VarChar, 100);
        p_title.Value = data.title as object ?? DBNull.Value;

        SqlParameter p_log_comment = new SqlParameter("@log_comment", SqlDbType.Text);
        p_log_comment.Value = data.log_comment as object ?? DBNull.Value;

        SqlParameter p_log_date = new SqlParameter("@log_date", SqlDbType.DateTime);
        p_log_date.Value = data.log_date as object ?? DBNull.Value;

        SqlParameter p_is_schedule = new SqlParameter("@is_schedule", SqlDbType.Int);
        p_is_schedule.Value = data.is_schedule;

        await db.Database.ExecuteSqlCommandAsync(@"exec sp_client_activity_log_log  @event_idx, @event_type, @event_uv_seq,	  @cal_seq , @c_seq , @title , @log_comment , @log_date , @is_schedule "
        , p_event_idx, p_event_type, p_event_uv_seq
         , p_cal_seq
        , p_c_seq
        , p_title
        , p_log_comment
        , p_log_date
        , p_is_schedule
        );
      }
      catch //(Exception e)
      {
      }
    }

    public async Task<List<client_log>> ListSummaryClientLog(int c_seq)
    {
      try
      {
        List<client_log> list = new List<client_log>();

        using (IDbConnection con = new SqlConnection(base.BaseConnectionString))
        {
          con.Open();

          string queryString = @"SELECT log_seq
      ,[event_idx]
      ,[event_type]
      ,(select name from uv_user where uv_seq = [event_uv_seq]) name
	  ,CONVERT(CHAR(16), event_dt, 120) event_dt_str
      , event_dt
      ,[c_seq]
  FROM [dbo].[client_log]
  WHERE c_seq = @c_seq 
  ORDER BY log_seq DESC ";

          DynamicParameters parameters = new DynamicParameters();
          parameters.Add("@c_seq", c_seq, DbType.Int32);

          var ret = await con.QueryAsync<client_log>(queryString, parameters);

          list = ret.ToList();

          con.Close();

          return list;
        }
      }
      catch (Exception e)
      {
        throw e;
      }
    }

    public async Task<List<client_manager_log>> ListClientManagerLog(int c_seq, int log_seq)
    {
      try
      {
        List<client_manager_log> list = new List<client_manager_log>();

        using (IDbConnection con = new SqlConnection(base.BaseConnectionString))
        {
          con.Open();

          string queryString = @"select
	cm.c_seq c_seq
	, cm.uv_seq uv_seq
	, uu.name am_name
	, ud.ud_name ud_name
from client_manager_log cm inner join uv_user uu
							on cm.uv_seq = uu.uv_seq
						inner join uv_division ud
							on uu.ud_seq = ud.ud_seq
where cm.c_seq = @c_seq and client_log_seq = @log_seq
  and uu.retire_date is null ";

          DynamicParameters parameters = new DynamicParameters();
          parameters.Add("@c_seq", c_seq, DbType.Int32);
          parameters.Add("@log_seq", log_seq, DbType.Int32);

          var ret = await con.QueryAsync<client_manager_log>(queryString, parameters);

          list = ret.ToList();

          con.Close();

          return list;
        }
      }
      catch (Exception e)
      {
        throw e;
      }
    }

    /// <summary>
    /// 업체 기본정보
    /// </summary>
    /// <param name="c_seq"></param>
    /// <returns></returns>
    public async Task<client_log> SelectClientLog(int c_seq, int log_seq)
    {
      try
      {
        using (IDbConnection con = new SqlConnection(base.BaseConnectionString))
        {
          con.Open();


          string selectQuery = @" select
    a.c_seq
	, a.kor_name kor_name
	, a.eng_name eng_name
	, a.ceo ceo
	, a.is_foreign_invest is_foreign_invest
	, a.addr1 addr1
	, a.addr2 addr2
	, a.is_contract is_contract
	, a.is_foreign is_foreign
	, a.foreign_code foreign_code
	, a.country_name country_name
	, case when len(a.biz_code) = 12 then a.biz_code
            when len(a.biz_code) = 10 then left(a.biz_code, 3) + '-' + SUBSTRING(a.biz_code, 4, 2) + '-' + SUBSTRING(a.biz_code, 6, 5) 
            else a.biz_code
        end biz_code
	, a.biz_type biz_type
	, a.biz_category biz_category
	, a.biz_industry biz_industry
	, a.biz_industry_code1 AS biz_industry_code1
    , cb1.code_name1 AS biz_industry_name1
    , a.biz_industry_code2 AS biz_industry_code2
    , cb2.code_name2 AS biz_industry_name2
	, a.fix_title fix_title
	, a.homepage homepage
	, a.employee_number employee_number
	, a.sales_amount sales_amount
	, a.create_user create_user
	, b.name contact_name
	, b.gender contact_gender
	, b.email contact_email
	, b.phone contact_phone
	, b.cell_phone contact_cell_phone
	, b.division contact_division
	, b.position contact_position
	, c.name tax_name
	, c.division tax_division
	, c.email tax_email
	, c.phone tax_phone
	, c.cell_phone tax_cell_phone
	, c.deposit_email tax_deposit_email
	, c.deposit_manager tax_deposit_manager
    , d.name create_name
    , cb.code_name2 biz_industry
	, a.biz_industry_code1 biz_industry_code1
	, a.biz_industry_code2 biz_industry_code2
	, c.ctc_seq tax_seq
	, b.cc_seq contact_seq
from
	client_log a left outer join client_contact b
					on a.c_seq = b.c_seq
			left outer join client_tax_contact c
					on a.c_seq = c.c_seq
            left outer join code_business_mst cb1     
                         on a.biz_industry_code1 = cb1.code1
            left outer join code_business_dtl cb2
					on a.biz_industry_code1 = cb2.code1
					and a.biz_industry_code2 = cb2.code2
            left outer join uv_user d
					on a.create_user = d.uv_seq
            left outer join code_business2 cb
						    on a.biz_industry_code1 = cb.code1
						    and a.biz_industry_code2 = cb.code2
where 
    a.c_seq = @c_seq
    and  a.log_seq = @log_seq
";

          DynamicParameters param = new DynamicParameters();
          param.Add("@c_seq", c_seq, DbType.Int32);
          param.Add("@log_seq", log_seq, DbType.Int32);

          var ret = await con.QueryAsync<client_log>(selectQuery, param);

          con.Close();

          return ret.FirstOrDefault();
        }
      }
      catch (Exception e)
      {
        throw e;
      }
    }
    public List<client_activity_log_log> ListClientActivity(int c_seq, int cal_seq, int skip, int count, out int totalCount)
    {
      try
      {
        List<client_activity_log_log> list = new List<client_activity_log_log>();

        using (IDbConnection con = new SqlConnection(base.BaseConnectionString))
        {
          con.Open();

          string selectQuery = @"select
    a.cal_seq cal_seq,
	a.title title, 
	a.log_comment log_comment,
	a.log_date log_date,
    b.name name,
	a.create_dt create_dt,
	a.log_seq,
	a.event_idx,
	a.event_dt,
	a.event_type,
	c.name event_name
	
from client_activity_log_log a inner join uv_user b
		on a.create_user = b.uv_seq
	inner join uv_user c
		on a.event_uv_seq = c.uv_seq
where c_seq = @c_seq
and cal_seq = case when @cal_seq = 0 then cal_seq else @cal_seq end
order by log_seq desc OFFSET @currentPage ROWS FETCH NEXT @pageSize ROWS ONLY
";
          string countQuery = @"select count(*) 
    from client_activity_log_log 
    where c_seq = @c_seq
    and cal_seq = case when @cal_seq = 0 then cal_seq else @cal_seq end

";

          DynamicParameters param = new DynamicParameters();
          param.Add("@c_seq", c_seq, DbType.Int32);
          param.Add("@cal_seq", cal_seq, DbType.Int32);
          param.Add("@currentPage", skip, DbType.Int32);
          param.Add("@pageSize", count, DbType.Int32);

          DynamicParameters param2 = new DynamicParameters();
          param2.Add("@c_seq", c_seq, DbType.Int32);
          param2.Add("@cal_seq", cal_seq, DbType.Int32);
          var ret = con.Query<client_activity_log_log>(selectQuery, param);
          totalCount = con.QueryFirstOrDefault<int>(countQuery, param2);

          list = ret.ToList();

          con.Close();

          return list;
        }
      }
      catch (Exception e)
      {
        throw e;
      }
    }

    public async Task<client_activity_log_log> ActivityDetailLog(int log_seq)
    {
      try
      {
        using (IDbConnection con = new SqlConnection(base.BaseConnectionString))
        {
          con.Open();

          string queryString = @"select
    cal_seq
	, c_seq
	, title
	, log_comment
	, log_date
    , ISNULL(is_schedule, 0) AS is_schedule
from client_activity_log_log
where log_seq = @log_seq
";
          DynamicParameters parameters = new DynamicParameters();
          parameters.Add("@log_seq", log_seq, DbType.Int32);

          var ret = await con.QueryAsync<client_activity_log_log>(queryString, parameters);

          con.Close();

          return ret.FirstOrDefault();
        }
      }

      catch (Exception e)
      {
        throw e;
      }
    }

    public List<client_contact_log> ListClientContactLog(int c_seq, int cc_seq, int skip, int count, out int totalCount)
    {
      try
      {
        List<client_contact_log> list = new List<client_contact_log>();

        using (IDbConnection con = new SqlConnection(base.BaseConnectionString))
        {
          con.Open();

          string selectQuery = @"select
	a.cc_seq cc_seq
	, a.name name
	, case when a.gender=1 then '남' else '여' end sex
	, a.division division
	, a.position position
    , a.phone phone
    , a.cell_phone cell_phone
	, a.email email
	, a.memo memo
	, b.name uv_name
	, a.create_dt
    , a.log_seq
	, a.event_idx
	, a.event_dt
	, a.event_type
	, c.name event_name
from
	client_contact_log a inner join uv_user b
			on a.create_user = b.uv_seq
    inner join uv_user c
		on a.event_uv_seq = c.uv_seq
where a.c_seq = @c_seq
and a.cc_seq = case when @cc_seq = 0 then a.cc_seq else @cc_seq end
order by a.log_seq desc OFFSET @currentPage ROWS FETCH NEXT @pageSize ROWS ONLY
";
          string countQuery = @"select count(*) from client_contact_log a inner join uv_user b
                                                                                        on a.create_user = b.uv_seq
where c_seq = @c_seq
and a.cc_seq = case  when @cc_seq = 0 then a.cc_seq else @cc_seq end
";

          DynamicParameters param = new DynamicParameters();
          param.Add("@c_seq", c_seq, DbType.Int32);
          param.Add("@cc_seq", cc_seq, DbType.Int32);
          param.Add("@currentPage", skip, DbType.Int32);
          param.Add("@pageSize", count, DbType.Int32);

          DynamicParameters param2 = new DynamicParameters();
          param2.Add("@c_seq", c_seq, DbType.Int32);
          param2.Add("@cc_seq", cc_seq, DbType.Int32);
          var ret = con.Query<client_contact_log>(selectQuery, param);
          totalCount = con.QueryFirstOrDefault<int>(countQuery, param2);

          list = ret.ToList();

          con.Close();

          return list;
        }
      }
      catch (Exception e)
      {
        throw e;
      }
    }
    public async Task<client_contact_log> ClientContactDetailLog(int log_seq)
    {
      try
      {
        using (IDbConnection con = new SqlConnection(base.BaseConnectionString))
        {
          con.Open();

          string queryString = @"select
    cc_seq
	, c_seq
	, name
	, gender
	, email
	, phone
	, cell_phone
	, division
	, position
	, memo
from client_contact_log
where log_seq = @log_seq
";
          DynamicParameters parameters = new DynamicParameters();
          parameters.Add("@log_seq", log_seq, DbType.Int32);

          var ret = await con.QueryAsync<client_contact_log>(queryString, parameters);

          con.Close();

          return ret.FirstOrDefault();
        }
      }

      catch (Exception e)
      {
        throw e;
      }
    }

    public List<client_tax_contact_log> ListClientTaxLog(int c_seq, int ctc_seq, int skip, int count, out int totalCount)
    {
      try
      {
        List<client_tax_contact_log> list = new List<client_tax_contact_log>();

        using (IDbConnection con = new SqlConnection(base.BaseConnectionString))
        {
          con.Open();

          string selectQuery = @"select
	a.ctc_seq ctc_seq
    , a.name
	, a.deposit_manager deposit_manager
	, a.division division
	, a.phone phone
    , a.cell_phone cell_phone
	, a.deposit_email deposit_email
    , a.email email
	, a.create_seq create_seq
	, b.name uv_name
	, a.create_dt create_dt
    , a.log_seq
	, a.event_idx
	, a.event_dt
	, a.event_type
	, c.name event_name
from
	client_tax_contact_log a inner join uv_user b
	    on a.create_seq = b.uv_seq
    inner join uv_user c
		on a.event_uv_seq = c.uv_seq
where a.c_seq = @c_seq 
and a.ctc_seq = case when @ctc_seq = 0 then a.ctc_seq else @ctc_seq end
and deposit_manager is not null
order by a.log_seq desc 
OFFSET @currentPage ROWS FETCH NEXT @pageSize ROWS ONLY
";
          string countQuery = @"
select count(*) 
from client_tax_contact_log a inner join uv_user b
    on a.create_seq = b.uv_seq
where c_seq = @c_seq and deposit_manager is not null 
and a.ctc_seq = case when @ctc_seq = 0 then a.ctc_seq else @ctc_seq end";

          DynamicParameters param = new DynamicParameters();
          param.Add("@c_seq", c_seq, DbType.Int32);
          param.Add("@ctc_seq", ctc_seq, DbType.Int32);
          param.Add("@currentPage", skip, DbType.Int32);
          param.Add("@pageSize", count, DbType.Int32);

          DynamicParameters param2 = new DynamicParameters();
          param2.Add("@c_seq", c_seq, DbType.Int32);
          param2.Add("@ctc_seq", ctc_seq, DbType.Int32);
          var ret = con.Query<client_tax_contact_log>(selectQuery, param);
          totalCount = con.QueryFirstOrDefault<int>(countQuery, param2);

          list = ret.ToList();

          con.Close();

          return list;
        }
      }
      catch (Exception e)
      {
        throw e;
      }
    }

    public async Task<client_tax_contact_log> ClientTaxContactLog(int log_seq)
    {
      try
      {
        using (IDbConnection con = new SqlConnection(base.BaseConnectionString))
        {
          con.Open();

          string queryString = @"select
	ctc_seq
	, c_seq
	, division
	, name
	, email
	, phone
	, cell_phone
	, deposit_manager
	, deposit_email
from client_tax_contact_log
where log_seq = @log_seq
";
          DynamicParameters parameters = new DynamicParameters();
          parameters.Add("@log_seq", log_seq, DbType.Int32);

          var ret = await con.QueryAsync<client_tax_contact_log>(queryString, parameters);

          con.Close();

          return ret.FirstOrDefault();
        }
      }

      catch (Exception e)
      {
        throw e;
      }
    }
  }
}
