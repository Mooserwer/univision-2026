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
    public class CandidateLogRepository :  BaseRepository
    {

		public async Task can_interest_log(can_interest data, string event_type = "", int event_uv_seq = 0, int event_idx = 0)
		{
			try
			{
				SqlParameter[] parameters = new SqlParameter[] {
					new SqlParameter("@event_idx", event_idx),
					new SqlParameter("@event_type", event_type),
					new SqlParameter("@event_uv_seq", event_uv_seq),
					new SqlParameter("@cf_seq", data.cf_seq),
					new SqlParameter("@c_seq", data.c_seq),
					new SqlParameter("@uv_seq", data.uv_seq),
					new SqlParameter("@create_dt", data.create_dt),

				};
				await db.Database.ExecuteSqlCommandAsync("exec sp_can_interest_log  @event_idx, @event_type, @event_uv_seq,	  @cf_seq , @c_seq , @uv_seq , @create_dt  ", parameters);

			}
			catch (Exception)
			{


			}
		}

		public async Task can_activity_log(can_activity data, string event_type = "", int event_uv_seq = 0, int event_idx = 0)
		{
			try
			{
				SqlParameter[] parameters = new SqlParameter[] {
				new SqlParameter("@event_idx", event_idx),
				new SqlParameter("@event_type", event_type),
				new SqlParameter("@event_uv_seq", event_uv_seq),
	 			new SqlParameter("@ca_seq", data.ca_seq),
				new SqlParameter("@c_seq", data.c_seq),
				new SqlParameter("@ca_status", data.ca_status),
				new SqlParameter("@ca_date", data.ca_date),
				new SqlParameter("@memo", data.memo),
				new SqlParameter("@create_dt", data.create_dt),
				new SqlParameter("@create_seq", data.create_seq),
				new SqlParameter("@modify_dt", data.modify_dt),
				new SqlParameter("@modify_seq", data.modify_seq),

			};
				await db.Database.ExecuteSqlCommandAsync("exec sp_can_activity_log  @event_idx, @event_type, @event_uv_seq,	  @ca_seq , @c_seq ,  @ca_status , @ca_date , @memo , @create_dt , @create_seq , @modify_dt , @modify_seq  ", parameters);
			}
			catch //(Exception e)
			{
			}
		}

		public async Task can_interview_sheet_log(can_interview_sheet data, string event_type = "", int event_uv_seq = 0, int event_idx = 0)
		{
			try
			{
				SqlParameter[] parameters = new SqlParameter[] {
					 new SqlParameter("@event_idx", event_idx),
					 new SqlParameter("@event_type", event_type),
					 new SqlParameter("@event_uv_seq", event_uv_seq),
					 new SqlParameter("@cis_seq", data.cis_seq),
					new SqlParameter("@c_seq", data.c_seq),
					new SqlParameter("@memo", data.memo),
					new SqlParameter("@interview_dt", data.interview_dt),
					new SqlParameter("@create_dt", data.create_dt),
					new SqlParameter("@create_seq", data.create_seq),

				};

				await db.Database.ExecuteSqlCommandAsync("exec sp_can_interview_sheet_log  @event_idx, @event_type, @event_uv_seq,	  @cis_seq , @c_seq , @memo , @interview_dt , @create_dt , @create_seq  ", parameters);


			}
			catch (Exception)
			{


			}
		}

		public async Task CreateOrDeleteCanResume(can_resume data, string event_type, int event_uv_seq = 0, int event_idx = 0)
		{
			try
			{
				SqlParameter[] parameters = new SqlParameter[] {
					 new SqlParameter("@event_idx", event_idx),
					 new SqlParameter("@event_type", event_type),
					 new SqlParameter("@event_uv_seq", event_uv_seq),
					 new SqlParameter("@cr_seq", data.cr_seq),
					new SqlParameter("@c_seq", data.c_seq),
					new SqlParameter("@file_type", data.file_type),
					new SqlParameter("@file_dir", data.file_dir),
					new SqlParameter("@file_origin_path", data.file_origin_path),
					new SqlParameter("@file_path", data.file_path),
					new SqlParameter("@file_extension", data.file_extension),
					new SqlParameter("@remove_dt", data.remove_dt),
					new SqlParameter("@remove_user", data.remove_user),
					new SqlParameter("@create_dt", data.create_dt),
					new SqlParameter("@create_user", data.create_user),

				};

				await db.Database.ExecuteSqlCommandAsync("exec sp_can_resume_log  @event_idx, @event_type, @event_uv_seq,	  @cr_seq , @c_seq , @file_type , @file_dir , @file_origin_path , @file_path , @file_extension , @remove_dt , @remove_user , @create_dt , @create_user  ", parameters);


			}
			catch (Exception)
			{


			}
		}

		public async Task candidate_log(candidate data, string event_type = "", int event_uv_seq = 0, int event_idx = 0)
		{
			try
			{
				SqlParameter p_event_idx = new SqlParameter("@event_idx", event_idx);
				SqlParameter p_event_type = new SqlParameter("@event_type", event_type);
				SqlParameter p_event_uv_seq = new SqlParameter("@event_uv_seq", event_uv_seq);
				SqlParameter p_c_seq = new SqlParameter("@c_seq", SqlDbType.Int);
				p_c_seq.Value = data.c_seq;

				SqlParameter p_manager_seq = new SqlParameter("@manager_seq", SqlDbType.Int);
				p_manager_seq.Value = data.manager_seq ?? 0;

				SqlParameter p_kor_name = new SqlParameter("@kor_name", SqlDbType.VarChar, 50);
				p_kor_name.Value = data.kor_name as object ?? DBNull.Value;

				SqlParameter p_eng_name = new SqlParameter("@eng_name", SqlDbType.VarChar, 50);
				p_eng_name.Value = data.eng_name as object ?? DBNull.Value;

				SqlParameter p_is_foreign = new SqlParameter("@is_foreign", SqlDbType.Float);
				p_is_foreign.Value = data.is_foreign ?? 0;

				SqlParameter p_birth_date = new SqlParameter("@birth_date", SqlDbType.Date);
				p_birth_date.Value = data.birth_date as object ?? DBNull.Value;

				SqlParameter p_gender = new SqlParameter("@gender", SqlDbType.Float);
				p_gender.Value = data.gender ?? 0;

				SqlParameter p_ex_birthdate = new SqlParameter("@ex_birthdate", SqlDbType.Float);
				p_ex_birthdate.Value = data.ex_birthdate ?? 0;

				SqlParameter p_country_code = new SqlParameter("@country_code", SqlDbType.VarChar, 10);
				p_country_code.Value = data.country_code as object ?? DBNull.Value;

				SqlParameter p_addr1 = new SqlParameter("@addr1", SqlDbType.VarChar, 100);
				p_addr1.Value = data.addr1 as object ?? DBNull.Value;

				SqlParameter p_addr2 = new SqlParameter("@addr2", SqlDbType.VarChar, 100);
				p_addr2.Value = data.addr2 as object ?? DBNull.Value;

				SqlParameter p_phone = new SqlParameter("@phone", SqlDbType.VarChar, 30);
				p_phone.Value = data.phone as object ?? DBNull.Value;

				SqlParameter p_cell_phone = new SqlParameter("@cell_phone", SqlDbType.VarChar, 30);
				p_cell_phone.Value = data.cell_phone as object ?? DBNull.Value;

				SqlParameter p_email1 = new SqlParameter("@email1", SqlDbType.VarChar, 100);
				p_email1.Value = data.email1 as object ?? DBNull.Value;

				SqlParameter p_email2 = new SqlParameter("@email2", SqlDbType.VarChar, 100);
				p_email2.Value = data.email2 as object ?? DBNull.Value;

				SqlParameter p_hope_salary = new SqlParameter("@hope_salary", SqlDbType.Int);
				p_hope_salary.Value = data.hope_salary ?? 0;

                SqlParameter p_after_interview = new SqlParameter("@after_interview", SqlDbType.Int);
				p_after_interview.Value = data.after_interview ?? 0; 

				SqlParameter p_keyword = new SqlParameter("@keyword", SqlDbType.VarChar, 500);
				p_keyword.Value = data.keyword as object ?? DBNull.Value;

				SqlParameter p_ex_addr = new SqlParameter("@ex_addr", SqlDbType.Int);
				p_ex_addr.Value = data.ex_addr ?? 0;

				SqlParameter p_wrong_phone = new SqlParameter("@wrong_phone", SqlDbType.Int);
				p_wrong_phone.Value = data.wrong_phone ?? 0;

				SqlParameter p_wrong_phone2 = new SqlParameter("@wrong_phone2", SqlDbType.Int);
				p_wrong_phone2.Value = data.wrong_phone2 ?? 0;

				SqlParameter p_is_confidential = new SqlParameter("@is_confidential", SqlDbType.Int);
				p_is_confidential.Value = data.is_confidential ?? 0;

				SqlParameter p_is_inactive = new SqlParameter("@is_inactive", SqlDbType.Int);
				p_is_inactive.Value = data.is_inactive ?? 0;

				SqlParameter p_reg_type = new SqlParameter("@reg_type", SqlDbType.Int);
				p_reg_type.Value = data.reg_type ?? 0;

				SqlParameter p_sns_link1 = new SqlParameter("@sns_link1", SqlDbType.VarChar, 500);
				p_sns_link1.Value = data.sns_link1 as object ?? DBNull.Value;

				SqlParameter p_sns_link2 = new SqlParameter("@sns_link2", SqlDbType.VarChar, 500);
				p_sns_link2.Value = data.sns_link2 as object ?? DBNull.Value;

				SqlParameter p_sns_link3 = new SqlParameter("@sns_link3", SqlDbType.VarChar, 500);
				p_sns_link3.Value = data.sns_link3 as object ?? DBNull.Value;

				SqlParameter p_caution_type = new SqlParameter("@caution_type", SqlDbType.Int);
				p_caution_type.Value = data.caution_type ?? 0;

				SqlParameter p_confi_remark = new SqlParameter("@confi_remark", SqlDbType.VarChar, 500);
				p_confi_remark.Value = data.confi_remark as object ?? DBNull.Value;

				SqlParameter p_inactive_remark = new SqlParameter("@inactive_remark", SqlDbType.VarChar, 500);
				p_inactive_remark.Value = data.inactive_remark as object ?? DBNull.Value;



				await db.Database.ExecuteSqlCommandAsync(@"exec sp_candidate_log  @event_idx, @event_type, @event_uv_seq,	  @c_seq , @manager_seq , @kor_name , @eng_name , @is_foreign , @birth_date , @gender , @ex_birthdate , @country_code , @addr1 , @addr2 , @phone , @cell_phone , @email1 , @email2 , @hope_salary , @after_interview , @keyword , @ex_addr , @wrong_phone , @wrong_phone2 , @is_confidential , @is_inactive , @reg_type , @sns_link1 , @sns_link2 , @sns_link3 , @caution_type , @confi_remark , @inactive_remark  "
                , p_event_idx, p_event_type, p_event_uv_seq
				 , p_c_seq
				, p_manager_seq
				, p_kor_name
				, p_eng_name
				, p_is_foreign
				, p_birth_date
				, p_gender
				, p_ex_birthdate
				, p_country_code
				, p_addr1
				, p_addr2
				, p_phone
				, p_cell_phone
				, p_email1
				, p_email2
				, p_hope_salary
                , p_after_interview			
				, p_keyword
				, p_ex_addr
				, p_wrong_phone
				, p_wrong_phone2
				, p_is_confidential
				, p_is_inactive
				, p_reg_type
				, p_sns_link1
				, p_sns_link2
				, p_sns_link3
				, p_caution_type
				, p_confi_remark
				, p_inactive_remark
				);
			}
			catch //(Exception e)
			{
			}
		}

		public async Task tempsaved_can_resume_log(tempsaved_can_resume data, string event_type = "", int event_uv_seq = 0, int event_idx = 0)
		{
			try
			{
				SqlParameter[] parameters = new SqlParameter[] {
				new SqlParameter("@event_idx", event_idx),
				new SqlParameter("@event_type", event_type),
				new SqlParameter("@event_uv_seq", event_uv_seq),
	 			new SqlParameter("@cr_seq", data.cr_seq),
				new SqlParameter("@c_seq", data.c_seq),
				new SqlParameter("@file_type", data.file_type),
				new SqlParameter("@file_dir", data.file_dir),
				new SqlParameter("@file_origin_path", data.file_origin_path),
				new SqlParameter("@file_path", data.file_path),
				new SqlParameter("@file_extension", data.file_extension),
				new SqlParameter("@remove_dt", data.remove_dt),
				new SqlParameter("@remove_user", data.remove_user),
				new SqlParameter("@create_dt", data.create_dt),
				new SqlParameter("@create_user", data.create_user),

			};
				await db.Database.ExecuteSqlCommandAsync("exec sp_tempsaved_can_resume_log  @event_idx, @event_type, @event_uv_seq,	  @cr_seq , @c_seq , @file_type , @file_dir , @file_origin_path , @file_path , @file_extension , @remove_dt , @remove_user , @create_dt , @create_user  ", parameters);
			}
			catch //(Exception e)
			{
			}
		}

		public async Task tempsaved_candidate_log(tempsaved_candidate data, string event_type = "", int event_uv_seq = 0, int event_idx = 0)
		{
			try
			{
				SqlParameter[] parameters = new SqlParameter[] {
				new SqlParameter("@event_idx", event_idx),
				new SqlParameter("@event_type", event_type),
				new SqlParameter("@event_uv_seq", event_uv_seq),
	 			new SqlParameter("@c_seq", data.c_seq),
				new SqlParameter("@manager_seq", data.manager_seq),
				new SqlParameter("@kor_name", data.kor_name),
				new SqlParameter("@eng_name", data.eng_name),
				new SqlParameter("@is_foreign", data.is_foreign),
				new SqlParameter("@birth_date", data.birth_date),
				new SqlParameter("@gender", data.gender),
				new SqlParameter("@ex_birthdate", data.ex_birthdate),
				new SqlParameter("@country_code", data.country_code),
				new SqlParameter("@addr1", data.addr1),
				new SqlParameter("@phone", data.phone),
				new SqlParameter("@cell_phone", data.cell_phone),
				new SqlParameter("@email1", data.email1),
				new SqlParameter("@email2", data.email2),
				new SqlParameter("@hope_salary", data.hope_salary),
                new SqlParameter("@create_dt", data.create_dt),
				new SqlParameter("@create_seq", data.create_seq),
				new SqlParameter("@modify_dt", data.modify_dt),
				new SqlParameter("@modify_seq", data.modify_seq),
				new SqlParameter("@keyword", data.keyword),
				new SqlParameter("@ex_addr", data.ex_addr),
				new SqlParameter("@wrong_phone", data.wrong_phone),
				new SqlParameter("@wrong_phone2", data.wrong_phone2),
				new SqlParameter("@is_confidential", data.is_confidential),
				new SqlParameter("@is_inactive", data.is_inactive),
				new SqlParameter("@reg_type", data.reg_type),
				new SqlParameter("@sns_link1", data.sns_link1),
				new SqlParameter("@sns_link2", data.sns_link2),
				new SqlParameter("@sns_link3", data.sns_link3),

			};
				await db.Database.ExecuteSqlCommandAsync("exec sp_tempsaved_candidate_log  @event_idx, @event_type, @event_uv_seq,	  @c_seq , @manager_seq , @kor_name , @eng_name , @is_foreign , @birth_date , @gender , @ex_birthdate , @country_code , @addr1 ,@phone , @cell_phone , @email1 , @email2 , @hope_salary ,@create_dt , @create_seq , @modify_dt , @modify_seq , @keyword , @ex_addr , @wrong_phone , @wrong_phone2 , @is_confidential , @is_inactive , @reg_type , @sns_link1 , @sns_link2 , @sns_link3   ", parameters);
			}
			catch //(Exception e)
			{
			}
		}

		public async Task can_school_log(can_school data, string event_type = "", int event_uv_seq = 0, int event_idx = 0)
		{
			try
			{
				SqlParameter[] parameters = new SqlParameter[] {
					new SqlParameter("@event_idx", event_idx),
					new SqlParameter("@event_type", event_type),
					new SqlParameter("@event_uv_seq", event_uv_seq),
	 				new SqlParameter("@cs_seq", data.cs_seq),
					new SqlParameter("@c_seq", data.c_seq),
					new SqlParameter("@sc_seq", data.sc_seq),
					new SqlParameter("@gubun", data.gubun),
					new SqlParameter("@sch1", data.sch1),
					new SqlParameter("@graduate_year", data.graduate_year),
					new SqlParameter("@admission_year", data.admission_year),
					new SqlParameter("@graduate_status", data.graduate_status),
					new SqlParameter("@is_transfer", data.is_transfer),
					new SqlParameter("@credit", data.credit),
					new SqlParameter("@total_credit", data.total_credit),
					new SqlParameter("@category1_name", data.category1_name),
					new SqlParameter("@major_name", data.major_name),
					new SqlParameter("@sub_category1_name", data.sub_category1_name),
					new SqlParameter("@sub_major_name", data.sub_major_name),
					new SqlParameter("@is_foreign_school", data.is_foreign_school),

				};
				await db.Database.ExecuteSqlCommandAsync("exec sp_can_school_log  @event_idx, @event_type, @event_uv_seq,	  @cs_seq , @c_seq , @sc_seq , @gubun , @sch1 , @graduate_year , @admission_year , @graduate_status , @is_transfer , @credit , @total_credit , @category1_name , @major_name , @sub_category1_name , @sub_major_name , @is_foreign_school  ", parameters);
			}
			catch //(Exception e)
			{
			}
		}

		public async Task can_school_log(List<can_school> list, string event_type = "", int event_uv_seq = 0, int event_idx = 0)
		{
			try
			{
				
				using (var tran = db.Database.BeginTransaction())
				{
					SqlParameter[] parameters = null;
					int i = 1;
					foreach (can_school data in list)
					{
						parameters = new SqlParameter[] {
							new SqlParameter("@event_idx", i),
							new SqlParameter("@event_type", event_type),
							new SqlParameter("@event_uv_seq", event_uv_seq),
	 						new SqlParameter("@cs_seq", data.cs_seq),
							new SqlParameter("@c_seq", data.c_seq),
							new SqlParameter("@sc_seq", data.sc_seq),
							new SqlParameter("@gubun", (object)data.gubun ?? DBNull.Value),
							new SqlParameter("@sch1", (object)data.sch1 ?? DBNull.Value),
							new SqlParameter("@graduate_year", (object)data.graduate_year ?? DBNull.Value),
							new SqlParameter("@admission_year", (object)data.admission_year ?? DBNull.Value),
							new SqlParameter("@graduate_status", data.graduate_status.HasValue  == false ? DBNull.Value : (object)data.graduate_status.Value),
							new SqlParameter("@is_transfer", data.is_transfer.HasValue  == false ? DBNull.Value : (object)data.is_transfer.Value),
							new SqlParameter("@credit", data.credit.HasValue == false ? DBNull.Value : (object)data.credit.Value),
							new SqlParameter("@total_credit", data.total_credit.HasValue == false ? DBNull.Value : (object)data.total_credit.Value),
							new SqlParameter("@category1_name", (object)data.category1_name ?? DBNull.Value),
							new SqlParameter("@major_name", (object)data.major_name ?? DBNull.Value),
							new SqlParameter("@sub_category1_name", (object)data.sub_category1_name ?? DBNull.Value),
							new SqlParameter("@sub_major_name", (object)data.sub_major_name ?? DBNull.Value),
							new SqlParameter("@is_foreign_school", data.is_foreign_school.HasValue  == false ? DBNull.Value : (object)data.is_foreign_school.Value),
						};
						await db.Database.ExecuteSqlCommandAsync("exec sp_can_school_log  " +
							"  @event_idx, @event_type, @event_uv_seq, @cs_seq , @c_seq , @sc_seq , @gubun , @sch1 , @graduate_year , @admission_year" +
							", @graduate_status , @is_transfer , @credit , @total_credit , @category1_name , @major_name , @sub_category1_name " +
							", @sub_major_name , @is_foreign_school  "
							, parameters[0]
							, parameters[1]
							, parameters[2]
							, parameters[3]
							, parameters[4]
							, parameters[5]
							, parameters[6]
							, parameters[7]
							, parameters[8]
							, parameters[9]
							, parameters[10]
							, parameters[11]
							, parameters[12]
							, parameters[13]
							, parameters[14]
							, parameters[15]
							, parameters[16]
							, parameters[17]
							, parameters[18]
							);
					}
					i++;
					await db.SaveChangesAsync();
					tran.Commit();

				}
			}
			catch //(Exception e)
			{
			}
		}

		public async Task can_career_log(can_career data, string event_type = "", int event_uv_seq = 0, int event_idx = 0)
		{
			try
			{
				SqlParameter[] parameters = new SqlParameter[] {
					new SqlParameter("@event_idx", event_idx),
					new SqlParameter("@event_type", event_type),
					new SqlParameter("@event_uv_seq", event_uv_seq),
	 				new SqlParameter("@cc_seq", data.cc_seq),
					new SqlParameter("@c_seq", data.c_seq),
					new SqlParameter("@g_seq", data.g_seq),
					new SqlParameter("@company_name", data.company_name),
					new SqlParameter("@division_name", data.division_name),
					new SqlParameter("@join_dt", data.join_dt),
					new SqlParameter("@leave_dt", data.leave_dt),
					new SqlParameter("@annual_income", data.annual_income),
					new SqlParameter("@is_work", data.is_work),
					new SqlParameter("@r_code", data.r_code),
					new SqlParameter("@p_code", data.p_code),

				};
				await db.Database.ExecuteSqlCommandAsync("exec sp_can_career_log  @event_idx, @event_type, @event_uv_seq,	  @cc_seq , @c_seq , @g_seq , @company_name , @division_name , @join_dt , @leave_dt , @annual_income , @is_work , @r_code , @p_code  ", parameters);
			}
			catch //(Exception e)
			{
			}
		}

		public async Task can_career_log(List<can_career> list, string event_type = "", int event_uv_seq = 0, int event_idx = 0)
		{
			try
			{

				using (var tran = db.Database.BeginTransaction())
				{
					SqlParameter[] parameters = null;
					int i = 1;
					foreach (can_career data in list)
					{
						parameters = new SqlParameter[] {
							new SqlParameter("@event_idx", i),
							new SqlParameter("@event_type", event_type),
							new SqlParameter("@event_uv_seq", event_uv_seq),
	 						new SqlParameter("@cc_seq", data.cc_seq),
							new SqlParameter("@c_seq", data.c_seq),
							new SqlParameter("@g_seq", data.g_seq),
							new SqlParameter("@company_name", data.company_name),
							new SqlParameter("@division_name", data.division_name),
							new SqlParameter("@join_dt", data.join_dt),
							new SqlParameter("@leave_dt", data.leave_dt),
							new SqlParameter("@annual_income", data.annual_income),
							new SqlParameter("@is_work", data.is_work),
							new SqlParameter("@r_code", data.r_code),
							new SqlParameter("@p_code", data.p_code),

						};
						await db.Database.ExecuteSqlCommandAsync("exec sp_can_career_log  @event_idx, @event_type, @event_uv_seq,	  @cc_seq , @c_seq , @g_seq , @company_name , @division_name , @join_dt , @leave_dt , @annual_income , @is_work , @r_code , @p_code  ", parameters);
					}
					i++;
					await db.SaveChangesAsync();
					tran.Commit();

				}
			}
			catch //(Exception e)
			{
			}
		}

		public async Task can_career_job_log(can_career_job data, string event_type = "", int event_uv_seq = 0, int event_idx = 0)
		{
			try
			{
				SqlParameter[] parameters = new SqlParameter[] {
				new SqlParameter("@event_idx", event_idx),
				new SqlParameter("@event_type", event_type),
				new SqlParameter("@event_uv_seq", event_uv_seq),
	 			new SqlParameter("@cc_seq", data.cc_seq),
				new SqlParameter("@job_code3", data.job_code3),
				new SqlParameter("@job_name3", data.job_name3),

			};
				await db.Database.ExecuteSqlCommandAsync("exec sp_can_career_job_log  @event_idx, @event_type, @event_uv_seq,	  @cc_seq , @job_code3 , @job_name3  ", parameters);
			}
			catch //(Exception e)
			{
			}
		}

		public async Task can_career_job_log(List<can_career_job> list, string event_type = "", int event_uv_seq = 0, int event_idx = 0)
		{
			try
			{
				
				using (var tran = db.Database.BeginTransaction())
				{
					SqlParameter[] parameters = null;
					int i = 1;
					foreach (can_career_job data in list)
					{
						parameters = new SqlParameter[] {
							new SqlParameter("@event_idx", i),
							new SqlParameter("@event_type", event_type),
							new SqlParameter("@event_uv_seq", event_uv_seq),
	 						new SqlParameter("@cc_seq", data.cc_seq),
							new SqlParameter("@job_code3", (object)data.job_code3 ?? DBNull.Value),
							new SqlParameter("@job_name3", (object)data.job_name3 ?? DBNull.Value),

						};
						await db.Database.ExecuteSqlCommandAsync("exec sp_can_career_job_log  @event_idx, @event_type, @event_uv_seq,	  @cc_seq , @job_code3 , @job_name3  ", parameters);
					}
					i++;
					await db.SaveChangesAsync();
					tran.Commit();

				}
			}
			catch //(Exception e)
			{
			}
		}

		public async Task can_job_code_log(List<can_job_code> list, string event_type = "", int event_uv_seq = 0, int event_idx = 0)
		{
			try
			{
				using (var tran = db.Database.BeginTransaction())
				{
					SqlParameter[] parameters = null;
					int i = 1;
					foreach (can_job_code data in list)
					{
						parameters = new SqlParameter[] {
							new SqlParameter("@event_idx", i),
							new SqlParameter("@event_type", event_type),
							new SqlParameter("@event_uv_seq", event_uv_seq),
	 						new SqlParameter("@c_seq", data.c_seq),
							new SqlParameter("@code1", data.code1),
							new SqlParameter("@code2", data.code2),
							new SqlParameter("@code3", data.code3),

						};
						await db.Database.ExecuteSqlCommandAsync("exec sp_can_job_code_log  @event_idx, @event_type, @event_uv_seq,	  @c_seq , @code1 , @code2 , @code3  ", parameters);
					}
					i++;
					await db.SaveChangesAsync();
					tran.Commit();

				}
			}
			catch //(Exception e)
			{
			}
		}

		public async Task can_job_code_log(can_job_code data, string event_type = "", int event_uv_seq = 0, int event_idx = 0)
		{
			try
			{
				SqlParameter[] parameters = new SqlParameter[] {
				new SqlParameter("@event_idx", event_idx),
				new SqlParameter("@event_type", event_type),
				new SqlParameter("@event_uv_seq", event_uv_seq),
	 			new SqlParameter("@c_seq", data.c_seq),
				new SqlParameter("@code1", data.code1),
				new SqlParameter("@code2", data.code2),
				new SqlParameter("@code3", data.code3),

			};
				await db.Database.ExecuteSqlCommandAsync("exec sp_can_job_code_log  @event_idx, @event_type, @event_uv_seq,	  @c_seq , @code1 , @code2 , @code3  ", parameters);
			}
			catch //(Exception e)
			{
			}
		}


		public async Task can_place_log(can_place data, string event_type = "", int event_uv_seq = 0, int event_idx = 0)
		{
			try
			{
				SqlParameter[] parameters = new SqlParameter[] {
					new SqlParameter("@event_idx", event_idx),
					new SqlParameter("@event_type", event_type),
					new SqlParameter("@event_uv_seq", event_uv_seq),
	 				new SqlParameter("@c_seq", data.c_seq),
					new SqlParameter("@cp_seq", data.cp_seq),
					new SqlParameter("@code1", data.code1),
					new SqlParameter("@area1", data.area1),
					new SqlParameter("@code2", data.code2),
					new SqlParameter("@area2", data.area2),

				};
				await db.Database.ExecuteSqlCommandAsync("exec sp_can_place_log  @event_idx, @event_type, @event_uv_seq,	  @c_seq , @cp_seq , @code1 , @area1 , @code2 , @area2  ", parameters);
			}
			catch //(Exception e)
			{
			}
		}

		public async Task can_place_log(List<can_place> list, string event_type = "",  int event_uv_seq = 0)
		{
			try
			{
				using (var tran = db.Database.BeginTransaction())
				{
					SqlParameter[] parameters = null;
					int i = 1;
					foreach (can_place data in list)
					{
						parameters = new SqlParameter[] {
							new SqlParameter("@event_idx", i),
							new SqlParameter("@event_type", event_type),
							new SqlParameter("@event_uv_seq", event_uv_seq),
	 						new SqlParameter("@c_seq", data.c_seq),
							new SqlParameter("@cp_seq", data.cp_seq),
							new SqlParameter("@code1", data.code1),
							new SqlParameter("@area1", data.area1),
							new SqlParameter("@code2", data.code2),
							new SqlParameter("@area2", data.area2),

						};
						await db.Database.ExecuteSqlCommandAsync("exec sp_can_place_log  @event_idx, @event_type, @event_uv_seq,	  @c_seq , @cp_seq , @code1 , @area1 , @code2 , @area2  ", parameters);
					}
					i++;
					await db.SaveChangesAsync();
					tran.Commit();

				}
			}
			catch //(Exception e)
			{
			}
		}

		public async Task can_business_code_log(can_business_code data, string event_type = "", int event_uv_seq = 0, int event_idx = 0)
		{
			try
			{
				SqlParameter[] parameters = new SqlParameter[] {
				new SqlParameter("@event_idx", event_idx),
				new SqlParameter("@event_type", event_type),
				new SqlParameter("@event_uv_seq", event_uv_seq),
	 			new SqlParameter("@c_seq", data.c_seq),
				new SqlParameter("@code1", data.code1),
				new SqlParameter("@code2", data.code2),
				new SqlParameter("@code3", data.code3),

			};
				await db.Database.ExecuteSqlCommandAsync("exec sp_can_business_code_log  @event_idx, @event_type, @event_uv_seq,	  @c_seq , @code1 , @code2 , @code3  ", parameters);
			}
			catch //(Exception e)
			{
			}
		}
		public async Task can_business_code_log(List<can_business_code> list, string event_type = "", int event_uv_seq = 0, int event_idx = 0)
		{
			try
			{

				using (var tran = db.Database.BeginTransaction())
				{
					SqlParameter[] parameters = null;
					int i = 1;
					foreach (can_business_code data in list)
					{
						parameters = new SqlParameter[] {
							new SqlParameter("@event_idx", i),
							new SqlParameter("@event_type", event_type),
							new SqlParameter("@event_uv_seq", event_uv_seq),
	 						new SqlParameter("@c_seq", data.c_seq),
							new SqlParameter("@code1", data.code1),
							new SqlParameter("@code2", data.code2),
							new SqlParameter("@code3", data.code3),

						};
						await db.Database.ExecuteSqlCommandAsync("exec sp_can_business_code_log  @event_idx, @event_type, @event_uv_seq,	  @c_seq , @code1 , @code2 , @code3  ", parameters);
					}
					i++;
					await db.SaveChangesAsync();
					tran.Commit();

				}
			}
			catch //(Exception e)
			{
			}
		}

		public async Task can_foreign_lan_log(can_foreign_lan data, string event_type = "", int event_uv_seq = 0, int event_idx = 0)
		{
			try
			{
				SqlParameter[] parameters = new SqlParameter[] {
				new SqlParameter("@event_idx", event_idx),
				new SqlParameter("@event_type", event_type),
				new SqlParameter("@event_uv_seq", event_uv_seq),
	 			new SqlParameter("@c_seq", data.c_seq),
				new SqlParameter("@code", data.code),
				new SqlParameter("@ability", data.ability),

			};
				await db.Database.ExecuteSqlCommandAsync("exec sp_can_foreign_lan_log  @event_idx, @event_type, @event_uv_seq,	  @c_seq , @code , @ability  ", parameters);
			}
			catch //(Exception e)
			{
			}
		}


		public async Task can_foreign_lan_log(List<can_foreign_lan> list, string event_type = "", int event_uv_seq = 0, int event_idx = 0)
		{
			try
			{
				using (var tran = db.Database.BeginTransaction())
				{
					SqlParameter[] parameters = null;
					int i = 1;
					foreach (can_foreign_lan data in list)
					{
						parameters = new SqlParameter[] {
							new SqlParameter("@event_idx", i),
							new SqlParameter("@event_type", event_type),
							new SqlParameter("@event_uv_seq", event_uv_seq),
	 						new SqlParameter("@c_seq", data.c_seq),
							new SqlParameter("@code", data.code),
							new SqlParameter("@ability", data.ability),

						};
						await db.Database.ExecuteSqlCommandAsync("exec sp_can_foreign_lan_log  @event_idx, @event_type, @event_uv_seq,	  @c_seq , @code , @ability  ", parameters);
					}
					i++;
					await db.SaveChangesAsync();
					tran.Commit();

				}
					
			}
			catch //(Exception e)
			{
			}
		}

		public async Task simple_candidate_log(simple_candidate data, string event_type = "", int event_uv_seq = 0, int event_idx = 0)
		{
			try
			{
				SqlParameter[] parameters = new SqlParameter[] {
				new SqlParameter("@event_idx", event_idx),
				new SqlParameter("@event_type", event_type),
				new SqlParameter("@event_uv_seq", event_uv_seq),
	 			new SqlParameter("@sc_seq", data.sc_seq),
				new SqlParameter("@ac_seq", data.ac_seq),
				new SqlParameter("@c_seq", data.c_seq),
				new SqlParameter("@p_seq", data.p_seq),
				new SqlParameter("@kor_name", data.kor_name),
				new SqlParameter("@gender", data.gender),
				new SqlParameter("@birthdate", data.birthdate),
				new SqlParameter("@cell_phone", data.cell_phone),
				new SqlParameter("@email", data.email),
				new SqlParameter("@sns_url", data.sns_url),
				new SqlParameter("@company", data.company),
				new SqlParameter("@comments", data.comments),
				new SqlParameter("@create_dt", data.create_dt),
				new SqlParameter("@create_user", data.create_user),
				new SqlParameter("@modify_dt", data.modify_dt),
				new SqlParameter("@modify_user", data.modify_user),

			};
				await db.Database.ExecuteSqlCommandAsync("exec sp_simple_candidate_log  @event_idx, @event_type, @event_uv_seq,	  @sc_seq , @ac_seq , @c_seq , @p_seq , @kor_name , @gender , @birthdate , @cell_phone , @email , @sns_url , @company , @comments , @create_dt , @create_user , @modify_dt , @modify_user  ", parameters);
			}
			catch //(Exception e)
			{
			}
		}

		/// <summary>
		/// 후보자 상세보기
		/// </summary>
		/// <param name="c_seq"></param>
		/// <returns></returns>
		public async Task<candidate_log> ClientCandidateLog(int c_seq, int log_seq)
		{
			try
			{
				using (IDbConnection con = new SqlConnection(base.BaseConnectionString))
				{
					con.Open();

					string selectQuery = @" SELECT [log_seq]
      ,[event_idx]
      ,[event_type]
      ,[event_uv_seq]
      ,[event_dt]
      ,c.[c_seq]
      ,[manager_seq]
      ,[kor_name]
      ,[eng_name]
      ,[is_foreign]
      ,[birth_date]
      ,[gender]
      ,[ex_birthdate]
      ,[country_code]
      ,[addr1]
      ,[addr2]
      ,[phone]
      ,[cell_phone]
      ,[email1]
      ,[email2]
      ,[hope_salary]
      ,[after_interview]
      ,c.[create_dt]
      ,[create_seq]
      ,c.[modify_dt]
      ,[modify_seq]
      ,[keyword]
      ,[ex_addr]
      ,[wrong_phone]
      ,[wrong_phone2]
      ,[is_confidential]
      ,[is_inactive]
      ,[reg_type]
      ,[sns_link1]
      ,[sns_link2]
      ,[sns_link3]
      ,[caution_type]
      ,[confi_remark]
      ,[inactive_remark]
      ,CS.school_name
      ,CS.category1_name
      ,CC.company_name
      ,CC.division_name
      ,CC.r_name
      ,CC.p_name
      ,CC.is_work
      ,CJ.code_name2 AS jobName
      ,CB.code_name2 AS busiName
      ,CF.kor_foreign_name
      ,CF.abilityDesc
      ,mnu.name as manager_name
      ,crt.name as create_name
      ,mdf.name as modify_name
      ,(select count(0) from candidate_log where c_seq = c.c_seq) log_cnt
  FROM candidate_log AS C LEFT OUTER JOIN (SELECT c_seq
                                             ,school_name
                                             ,category1_name
                                         FROM (SELECT c_seq
                                                     ,DENSE_RANK() OVER(PARTITION BY c_seq ORDER BY c_seq, graduate_year DESC) AS rank
                                                     ,CASE WHEN ISNULL(SC.campus_name, '') <> '' THEN SC.school_name + '(' + SC.campus_name + ')' ELSE SC.school_name END AS school_name
                                                     ,CS.category1_name
                                                     ,CS.graduate_year
                                                 FROM can_school AS CS INNER JOIN code_school AS SC
                                                                               ON CS.sc_seq = SC.sc_seq
                                                GROUP BY CS.c_seq, SC.school_name, SC.campus_name, CS.category1_name,graduate_year) AS A
                                        WHERE rank = 1) AS CS 
                                   ON C.c_seq = CS.c_seq
                       LEFT OUTER JOIN (SELECT c_seq
                                              ,company_name
                                              ,division_name
                                              ,is_work
                                              ,r_name
                                              ,p_name
                                          FROM (SELECT c_seq
                                                      ,DENSE_RANK() OVER(PARTITION BY c_seq ORDER BY c_seq, join_dt DESC) AS rank
                                                      ,company_name
                                                      ,division_name
                                                      ,is_work
                                                      ,B.r_name
                                                      ,C.p_name
                                                  FROM can_career AS A LEFT OUTER JOIN code_rank AS B
                                                                                    ON A.r_code = B.r_code
                                                                       LEFT OUTER JOIN code_position AS C
                                                                                    ON A.p_code = C.p_code
                                                 GROUP BY c_seq, company_name, division_name, is_work, join_dt, B.r_name, C.p_name) AS A
                                         WHERE rank = 1) AS CC
                                   ON C.c_seq = CC.c_seq
                      LEFT OUTER JOIN (SELECT DISTINCT 
                                              CJ.c_seq
                                             ,REPLACE(STUFF((SELECT DISTINCT
                                                                    '/' + B.code_name2
                                                               FROM   can_job_code AS A INNER JOIN code_job2 AS B
                                                                                                ON A.code2 = B.code2
                                                               WHERE  A.c_seq = CJ.c_seq
                                                               FOR XML PATH('')),1,1,''),  '&amp;', '&') AS code_name2
                                         FROM can_business_code AS CJ) AS CJ
                                   ON C.c_seq = CJ.c_seq
                      LEFT OUTER JOIN (SELECT DISTINCT 
                                              CB.c_seq
                                             ,REPLACE(STUFF((SELECT DISTINCT
                                                                    '/' + B.code_name2
                                                               FROM   can_business_code AS A INNER JOIN code_business2 AS B
                                                                                                     ON A.code2 = B.code2
                                                               WHERE  A.c_seq = CB.c_seq
                                                               FOR XML PATH('')),1,1,''),  '&amp;', '&') AS code_name2
                                         FROM can_business_code AS CB) AS CB
                                   ON C.c_seq = CB.c_seq
                      LEFT OUTER JOIN (SELECT c_seq
                                             ,kor_foreign_name
                                             ,CASE WHEN ability = 1 THEN '상'
                                                   WHEN ability = 2 THEN '중'
                                                   WHEN ability = 3 THEN '하'
                                                   ELSE ''
                                               END AS abilityDesc
                                         FROM (SELECT c_seq
                                                     ,ROW_NUMBER() OVER(PARTITION BY c_seq ORDER BY c_seq, ability ASC) AS rank
                                                     ,ability
                                                     ,B.kor_foreign_name
                                                FROM can_foreign_lan AS A INNER JOIN code_foreign_lan AS B
                                                                                  ON A.code = B.code
                                               GROUP BY c_seq,ability, kor_foreign_name) AS A
                                        WHERE A.rank = 1) AS CF
                                   ON C.c_seq = CF.c_seq
                
                      LEFT OUTER JOIN uv_user AS mnu
                                   ON C.manager_seq = mnu.uv_seq
                      LEFT OUTER JOIN uv_user AS crt
                                   ON C.create_seq = crt.uv_seq
                      LEFT OUTER JOIN uv_user AS mdf
                                   ON C.modify_seq = mdf.uv_seq
 WHERE C.c_seq = @c_seq and log_seq = @log_seq ";

					DynamicParameters param = new DynamicParameters();
					param.Add("@c_seq", c_seq, DbType.Int32);
					param.Add("@log_seq", log_seq, DbType.Int32);


					var ret = await con.QueryAsync<candidate_log>(selectQuery, param);

					con.Close();

					return ret.FirstOrDefault();
				}
			}
			catch (Exception e)
			{
				throw e;
			}
		}

		/// <summary>
		/// 후보자 상세보기
		/// </summary>
		/// <param name="c_seq"></param>
		/// <returns></returns>
		public async Task<List<candidate_log>> ListCandidateSummary(int c_seq)
		{
			try
			{
				using (IDbConnection con = new SqlConnection(base.BaseConnectionString))
				{
					con.Open();

					string selectQuery = @" SELECT [log_seq]
      ,[event_idx]
      ,[event_type]
      ,[event_uv_seq]
      ,[event_dt]
     ,(select name from uv_user where uv_seq = event_uv_seq) event_name
  FROM candidate_log AS C 
 WHERE C.c_seq = @c_seq  
 order by log_seq desc
";

					DynamicParameters param = new DynamicParameters();
					param.Add("@c_seq", c_seq, DbType.Int32);
					

					var ret = await con.QueryAsync<candidate_log>(selectQuery, param);

					con.Close();

					return ret.ToList();
				}
			}
			catch (Exception e)
			{
				throw e;
			}
		}

		/// <summary>
		/// 후보자 학력 정보 리스트
		/// </summary>
		/// <param name="c_seq"></param>
		/// <returns></returns>
		public async Task<List<can_school_log>> ListCanSchoolLog( int log_seq)
		{
			try
			{
				List<can_school_log> list = new List<can_school_log>();

				using (IDbConnection con = new SqlConnection(base.BaseConnectionString))
				{
					con.Open();

					string selectQuery = @"
SELECT [log_seq]
      ,[log_can_seq]
      ,[event_idx]
      ,[event_uv_seq]
      ,[event_dt]
      ,[cs_seq]
      ,[c_seq]
      ,a.[sc_seq]
      ,a.[gubun]
      ,[sch1]
      ,[graduate_year]
      ,[admission_year]
      ,[graduate_status]
      ,[is_transfer]
      ,[credit]
      ,[total_credit]
      ,[category1_name]
      ,[major_name]
      ,[sub_category1_name]
      ,[sub_major_name]
      ,[is_foreign_school]
      ,a.[order_no]
      ,B.school_name AS schoolName
      ,B.campus_name AS campusName
FROM can_school_log AS A LEFT JOIN code_school AS B
                               ON A.sc_seq = B.sc_seq 
WHERE log_can_seq = @log_can_seq
ORDER BY order_no";

					DynamicParameters param = new DynamicParameters();
					
					param.Add("@log_can_seq", log_seq, DbType.Int32);

					var ret = await con.QueryAsync<can_school_log>(selectQuery, param);

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
		/// 후보자 경력 정보 리스트
		/// </summary>
		/// <param name="c_seq"></param>
		/// <returns></returns>
		public async Task<List<can_career_log>> ListCanCareerLog(int log_seq)
		{
			try
			{
				List<can_career_log> list = new List<can_career_log>();

				using (IDbConnection con = new SqlConnection(base.BaseConnectionString))
				{
					con.Open();

					string selectQuery = @" SELECT [log_seq]
      ,a.[log_can_seq]
      ,[event_idx]
      ,[event_uv_seq]
      ,[event_dt]
      ,[cc_seq]
      ,[c_seq]
      ,[g_seq]
      ,[company_name]
      ,[division_name]
      ,[join_dt]
      ,[leave_dt]
      ,[annual_income]
      ,[is_work]
      ,a.[r_code]
      ,a.[p_code]
       ,B.r_name
       ,C.p_name
   FROM can_career_log AS A LEFT JOIN code_rank AS B
                                ON A.r_code = B.r_code
                        LEFT JOIN code_position AS C
                                ON A.p_code = C.p_code 
  WHERE log_can_seq = @log_seq
order by order_no ASC";

					DynamicParameters param = new DynamicParameters();
					param.Add("@log_seq", log_seq, DbType.Int32);

					var ret = await con.QueryAsync<can_career_log>(selectQuery, param);

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
		/// 후보자 희망근무지 리스트
		/// </summary>
		/// <param name="c_seq"></param>
		/// <returns></returns>
		public async Task<List<can_place_log>> ListCanPlaceLog(int log_seq)
		{
			try
			{
				List<can_place_log> list = new List<can_place_log>();

				using (IDbConnection con = new SqlConnection(base.BaseConnectionString))
				{
					con.Open();

					string selectQuery = @" SELECT [log_seq]
      ,[log_can_seq]
      ,[event_idx]
      ,[event_uv_seq]
      ,[event_dt]
      ,[c_seq]
      ,[cp_seq]
      ,[code1]
      ,[area1]
      ,[code2]
      ,[area2]
  FROM [dbo].[can_place_log] WHERE log_can_seq = @log_seq ";

					DynamicParameters param = new DynamicParameters();
					param.Add("@log_seq", log_seq, DbType.Int32);

					var ret = await con.QueryAsync<can_place_log>(selectQuery, param);

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
		/// 후보자 직무정보 리스트
		/// </summary>
		/// <param name="c_seq"></param>
		/// <returns></returns>
		public async Task<List<can_job_code_log>> ListCanJobCodeLog(int log_seq)
		{
			try
			{
				List<can_job_code_log> list = new List<can_job_code_log>();

				using (IDbConnection con = new SqlConnection(base.BaseConnectionString))
				{
					con.Open();

					string selectQuery = @" SELECT [log_seq]
      ,[log_can_Seq]
      ,[event_idx]
      ,[event_uv_seq]
      ,[event_dt]
      ,[c_seq]
      ,a.[code1]
      ,a.[code2]
      ,a.[code3]
      ,B.code_name2
      ,C.code_name3
  FROM can_job_code_log AS A INNER JOIN code_job2 AS B
                                 ON A.code2 = B.code2
                    LEFT OUTER JOIN code_job3 AS C
                                 ON A.code2 = C.code2
                                AND A.code3 = C.code3
 WHERE log_can_Seq = @log_seq  ";

					DynamicParameters param = new DynamicParameters();
					param.Add("@log_seq", log_seq, DbType.Int32);

					var ret = await con.QueryAsync<can_job_code_log>(selectQuery, param);

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
		/// 후보자 산업정보 리스트
		/// </summary>
		/// <param name="c_seq"></param>
		/// <returns></returns>
		public async Task<List<can_business_code_log>> ListCanBusinessCodeLog(int log_seq)
		{
			try
			{
				List<can_business_code_log> list = new List<can_business_code_log>();

				using (IDbConnection con = new SqlConnection(base.BaseConnectionString))
				{
					con.Open();

					string selectQuery = @" SELECT [log_seq]
      ,[log_can_seq]
      ,[event_idx]
      ,[event_uv_seq]
      ,[event_dt]
      ,[c_seq]
      ,a.[code1]
      ,a.[code2]
      ,a.[code3]
      ,B.code_name2
      ,C.code_name3
  FROM can_business_code_log AS A INNER JOIN code_business2 AS B
                                 ON A.code2 = B.code2
                    LEFT OUTER JOIN code_business3 AS C
                                 ON A.code2 = C.code2
                                AND A.code3 = C.code3

 WHERE log_can_seq = @log_seq";

					DynamicParameters param = new DynamicParameters();
					param.Add("@log_seq", log_seq, DbType.Int32);

					var ret = await con.QueryAsync<can_business_code_log>(selectQuery, param);

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
		/// 후보자 외국어 능력 리스트
		/// </summary>
		/// <param name="c_seq"></param>
		/// <returns></returns>
		public async Task<List<can_foreign_lan_log>> ListCanForeignLanLog(int log_seq)
		{
			try
			{
				List<can_foreign_lan_log> list = new List<can_foreign_lan_log>();

				using (IDbConnection con = new SqlConnection(base.BaseConnectionString))
				{
					con.Open();

					string selectQuery = @" SELECT * FROM can_foreign_lan_log WHERE log_can_seq = @log_seq ";

					DynamicParameters param = new DynamicParameters();
					param.Add("@log_seq", log_seq, DbType.Int32);

					var ret = await con.QueryAsync<can_foreign_lan_log>(selectQuery, param);

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

	}
}
