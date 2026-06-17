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
using Univision.Core.Models.DTO.Request.Candidate;

namespace Univision.Core.Repositories
{
    public class ProjectLogRepository : BaseRepository
    {
		public async Task Inorder_log(inorder data, string event_type = "", int event_uv_seq = 0, int event_idx = 0)
		{
			try
			{ 
				SqlParameter p_event_idx = new SqlParameter("@event_idx", event_idx);
				SqlParameter p_event_type = new SqlParameter("@event_type", event_type);
				SqlParameter p_event_uv_seq = new SqlParameter("@event_uv_seq", event_uv_seq);
				SqlParameter p_i_seq = new SqlParameter("@i_seq", SqlDbType.Int);
				p_i_seq.Value = data.i_seq;

				SqlParameter p_p_seq = new SqlParameter("@p_seq", SqlDbType.Int);
				p_p_seq.Value = data.p_seq ?? 0;

				SqlParameter p_c_seq = new SqlParameter("@c_seq", SqlDbType.Int);
				p_c_seq.Value = data.c_seq ?? 0;

				SqlParameter p_cc_seq = new SqlParameter("@cc_seq", SqlDbType.Int);
				p_cc_seq.Value = data.cc_seq ?? 0;

				SqlParameter p_inorder_type = new SqlParameter("@inorder_type", SqlDbType.Int);
				p_inorder_type.Value = data.inorder_type ?? 0;

				SqlParameter p_inorder_status = new SqlParameter("@inorder_status", SqlDbType.Int);
				p_inorder_status.Value = data.inorder_status ?? 0;

				SqlParameter p_clt_name = new SqlParameter("@clt_name", SqlDbType.VarChar, 255);
				p_clt_name.Value = data.clt_name as object ?? DBNull.Value;

				SqlParameter p_clt_busi_type = new SqlParameter("@clt_busi_type", SqlDbType.VarChar, 255);
				p_clt_busi_type.Value = data.clt_busi_type as object ?? DBNull.Value;

				SqlParameter p_clt_url = new SqlParameter("@clt_url", SqlDbType.VarChar, 255);
				p_clt_url.Value = data.clt_url as object ?? DBNull.Value;

				SqlParameter p_cc_name = new SqlParameter("@cc_name", SqlDbType.VarChar, 255);
				p_cc_name.Value = data.cc_name as object ?? DBNull.Value;

				SqlParameter p_cc_gender = new SqlParameter("@cc_gender", SqlDbType.Int);
				p_cc_gender.Value = data.cc_gender;

				SqlParameter p_cc_division = new SqlParameter("@cc_division", SqlDbType.VarChar, 255);
				p_cc_division.Value = data.cc_division as object ?? DBNull.Value;

				SqlParameter p_cc_position = new SqlParameter("@cc_position", SqlDbType.VarChar, 255);
				p_cc_position.Value = data.cc_position as object ?? DBNull.Value;

				SqlParameter p_cc_phone = new SqlParameter("@cc_phone", SqlDbType.VarChar, 20);
				p_cc_phone.Value = data.cc_phone as object ?? DBNull.Value;

				SqlParameter p_cc_cell_phone = new SqlParameter("@cc_cell_phone", SqlDbType.VarChar, 20);
				p_cc_cell_phone.Value = data.cc_cell_phone as object ?? DBNull.Value;

				SqlParameter p_cc_email = new SqlParameter("@cc_email", SqlDbType.VarChar, 255);
				p_cc_email.Value = data.cc_email as object ?? DBNull.Value;

				SqlParameter p_cc_reason = new SqlParameter("@cc_reason", SqlDbType.VarChar, 255);
				p_cc_reason.Value = data.cc_reason as object ?? DBNull.Value;

				SqlParameter p_can_dept = new SqlParameter("@can_dept", SqlDbType.VarChar, 255);
				p_can_dept.Value = data.can_dept as object ?? DBNull.Value;

				SqlParameter p_can_pos = new SqlParameter("@can_pos", SqlDbType.VarChar, 255);
				p_can_pos.Value = data.can_pos as object ?? DBNull.Value;

				SqlParameter p_can_location = new SqlParameter("@can_location", SqlDbType.VarChar, 255);
				p_can_location.Value = data.can_location as object ?? DBNull.Value;

				SqlParameter p_can_jobdesc = new SqlParameter("@can_jobdesc", SqlDbType.Text);
				p_can_jobdesc.Value = data.can_jobdesc as object ?? DBNull.Value;

				SqlParameter p_can_contents = new SqlParameter("@can_contents", SqlDbType.Text);
				p_can_contents.Value = data.can_contents as object ?? DBNull.Value;

				SqlParameter p_inorder_dt = new SqlParameter("@inorder_dt", SqlDbType.DateTime);
				p_inorder_dt.Value = data.inorder_dt as object ?? DBNull.Value;

				SqlParameter p_director_seq = new SqlParameter("@director_seq", SqlDbType.Int);
				p_director_seq.Value = data.director_seq ?? 0;

				SqlParameter p_director_dt = new SqlParameter("@director_dt", SqlDbType.DateTime);
				p_director_dt.Value = data.director_dt as object ?? DBNull.Value;



				await db.Database.ExecuteSqlCommandAsync(@"exec sp_Inorder_log  @event_idx, @event_type, @event_uv_seq,	  @i_seq , @p_seq , @c_seq , @cc_seq , @inorder_type , @inorder_status , @clt_name , @clt_busi_type , @clt_url , @cc_name , @cc_gender , @cc_division , @cc_position , @cc_phone , @cc_cell_phone , @cc_email , @cc_reason , @can_dept , @can_pos , @can_location , @can_jobdesc , @can_contents , @inorder_dt , @director_seq , @director_dt  "
				, p_event_idx, p_event_type, p_event_uv_seq
				 , p_i_seq
				, p_p_seq
				, p_c_seq
				, p_cc_seq
				, p_inorder_type
				, p_inorder_status
				, p_clt_name
				, p_clt_busi_type
				, p_clt_url
				, p_cc_name
				, p_cc_gender
				, p_cc_division
				, p_cc_position
				, p_cc_phone
				, p_cc_cell_phone
				, p_cc_email
				, p_cc_reason
				, p_can_dept
				, p_can_pos
				, p_can_location
				, p_can_jobdesc
				, p_can_contents
				, p_inorder_dt
				, p_director_seq
				, p_director_dt
				);
			}
			catch //(Exception e)
			{
			}
		}


		public async Task pjt_director_log(pjt_director data, string event_type = "", int event_uv_seq = 0, int event_idx = 0)
		{
			try
			{
				SqlParameter[] parameters = new SqlParameter[] {
				new SqlParameter("@event_idx", event_idx),
				new SqlParameter("@event_type", event_type),
				new SqlParameter("@event_uv_seq", event_uv_seq),
	 			new SqlParameter("@p_seq", data.p_seq),
				new SqlParameter("@uv_seq", data.uv_seq),

			};
				await db.Database.ExecuteSqlCommandAsync("exec sp_pjt_director_log  @event_idx, @event_type, @event_uv_seq,	  @p_seq , @uv_seq  ", parameters);
			}
			catch //(Exception e)
			{
			}
		}

		public async Task pjt_director_log(List<pjt_director> list, string event_type = "", int event_uv_seq = 0, int event_idx = 0)
		{
			try
			{
				using (var tran = db.Database.BeginTransaction())
				{
					SqlParameter[] parameters = null;
					int i = 1;
					foreach (pjt_director data in list)
					{
						parameters = new SqlParameter[] {
							new SqlParameter("@event_idx", i),
							new SqlParameter("@event_type", event_type),
							new SqlParameter("@event_uv_seq", event_uv_seq),
	 						new SqlParameter("@p_seq", data.p_seq),
							new SqlParameter("@uv_seq", data.uv_seq),

						};
						await db.Database.ExecuteSqlCommandAsync("exec sp_pjt_director_log  @event_idx, @event_type, @event_uv_seq,	  @p_seq , @uv_seq  ", parameters);
					}
					i++;
					await db.SaveChangesAsync();
					tran.Commit();

				}
			}
			catch // (Exception e)
			{
			}
		}

		public async Task pjt_manager_log(pjt_manager data, string event_type = "", int event_uv_seq = 0, int event_idx = 0)
		{
			try
			{
				SqlParameter[] parameters = new SqlParameter[] {
				new SqlParameter("@event_idx", event_idx),
				new SqlParameter("@event_type", event_type),
				new SqlParameter("@event_uv_seq", event_uv_seq),
	 			new SqlParameter("@p_seq", data.p_seq),
				new SqlParameter("@uv_seq", data.uv_seq),

			};
				await db.Database.ExecuteSqlCommandAsync("exec sp_pjt_manager_log  @event_idx, @event_type, @event_uv_seq,	  @p_seq , @uv_seq  ", parameters);
			}
			catch // (Exception e)
			{
			}
		}

		public async Task pjt_manager_log(List<pjt_manager> list, string event_type = "", int event_uv_seq = 0, int event_idx = 0)
		{
			try
			{ 
				using (var tran = db.Database.BeginTransaction())
				{
					SqlParameter[] parameters = null;
					int i = 1;
					foreach (pjt_manager data in list)
					{
						parameters = new SqlParameter[] {
							new SqlParameter("@event_idx", i),
							new SqlParameter("@event_type", event_type),
							new SqlParameter("@event_uv_seq", event_uv_seq),
	 						new SqlParameter("@p_seq", data.p_seq),
							new SqlParameter("@uv_seq", data.uv_seq),

						};
						await db.Database.ExecuteSqlCommandAsync("exec sp_pjt_manager_log  @event_idx, @event_type, @event_uv_seq,	  @p_seq , @uv_seq  ", parameters);
					}
					i++;
					await db.SaveChangesAsync();
					tran.Commit();

				}
			}
			catch // (Exception e)
			{
			}
		}

		public async Task pjt_place_log(pjt_place data, string event_type = "", int event_uv_seq = 0, int event_idx = 0)
		{
			try
			{
				SqlParameter[] parameters = new SqlParameter[] {
				new SqlParameter("@event_idx", event_idx),
				new SqlParameter("@event_type", event_type),
				new SqlParameter("@event_uv_seq", event_uv_seq),
	 			new SqlParameter("@p_seq", data.p_seq),
				new SqlParameter("@pp_seq", data.pp_seq),
				new SqlParameter("@code1", data.code1),
				new SqlParameter("@area1", data.area1),
				new SqlParameter("@code2", data.code2),
				new SqlParameter("@area2", data.area2),

			};
				await db.Database.ExecuteSqlCommandAsync("exec sp_pjt_place_log  @event_idx, @event_type, @event_uv_seq,	  @p_seq , @pp_seq , @code1 , @area1 , @code2 , @area2  ", parameters);
			}
			catch // (Exception e)
			{
			}
		}

		public async Task pjt_place_log(List<pjt_place> list, string event_type = "", int event_uv_seq = 0, int event_idx = 0)
		{
			try
			{
				using (var tran = db.Database.BeginTransaction())
				{
					SqlParameter[] parameters = null;
					int i = 1;
					foreach (pjt_place data in list)
					{
						parameters = new SqlParameter[] {
							new SqlParameter("@event_idx", i),
							new SqlParameter("@event_type", event_type),
							new SqlParameter("@event_uv_seq", event_uv_seq),
	 						new SqlParameter("@p_seq", data.p_seq),
							new SqlParameter("@pp_seq", data.pp_seq),
							new SqlParameter("@code1", data.code1),
							new SqlParameter("@area1", data.area1),
							new SqlParameter("@code2", data.code2),
							new SqlParameter("@area2", data.area2),

						};
						await db.Database.ExecuteSqlCommandAsync("exec sp_pjt_place_log  @event_idx, @event_type, @event_uv_seq,	  @p_seq , @pp_seq , @code1 , @area1 , @code2 , @area2  ", parameters);
					}
					i++;
					await db.SaveChangesAsync();
					tran.Commit();

				}
			}
			catch // (Exception e)
			{
			}
		}

		public async Task pjt_keyword_log(pjt_keyword data, string event_type = "", int event_uv_seq = 0, int event_idx = 0)
		{
			try
			{
				SqlParameter[] parameters = new SqlParameter[] {
				new SqlParameter("@event_idx", event_idx),
				new SqlParameter("@event_type", event_type),
				new SqlParameter("@event_uv_seq", event_uv_seq),
	 			new SqlParameter("@p_seq", data.p_seq),
				new SqlParameter("@pk_seq", data.pk_seq),
				new SqlParameter("@pk_name", data.pk_name),

			};
				await db.Database.ExecuteSqlCommandAsync("exec sp_pjt_keyword_log  @event_idx, @event_type, @event_uv_seq,	  @p_seq , @pk_seq , @pk_name  ", parameters);
			}
			catch // (Exception e)
			{
			}
		}


		public async Task pjt_keyword_log(List<pjt_keyword> list, string event_type = "", int event_uv_seq = 0, int event_idx = 0)
		{
			try
			{
				using (var tran = db.Database.BeginTransaction())
				{
					SqlParameter[] parameters = null;
					int i = 1;
					foreach (pjt_keyword data in list)
					{
						parameters = new SqlParameter[] {
							new SqlParameter("@event_idx", i),
							new SqlParameter("@event_type", event_type),
							new SqlParameter("@event_uv_seq", event_uv_seq),
	 						new SqlParameter("@p_seq", data.p_seq),
							new SqlParameter("@pk_seq", data.pk_seq),
							new SqlParameter("@pk_name", data.pk_name),

						};
						await db.Database.ExecuteSqlCommandAsync("exec sp_pjt_keyword_log  @event_idx, @event_type, @event_uv_seq,	  @p_seq , @pk_seq , @pk_name  ", parameters);
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

		public async Task project_log(project data, string event_type = "", int event_uv_seq = 0, int event_idx = 0)
		{
			try
			{
				SqlParameter p_event_idx = new SqlParameter("@event_idx", event_idx);
				SqlParameter p_event_type = new SqlParameter("@event_type", event_type);
				SqlParameter p_event_uv_seq = new SqlParameter("@event_uv_seq", event_uv_seq);
				SqlParameter p_p_seq = new SqlParameter("@p_seq", SqlDbType.Int);
				p_p_seq.Value = data.p_seq;

				SqlParameter p_c_seq = new SqlParameter("@c_seq", SqlDbType.Int);
				p_c_seq.Value = data.c_seq;

				SqlParameter p_cc_seq = new SqlParameter("@cc_seq", SqlDbType.Int);
				p_cc_seq.Value = data.cc_seq ?? 0;

				SqlParameter p_ctc_seq = new SqlParameter("@ctc_seq", SqlDbType.Int);
				p_ctc_seq.Value = data.ctc_seq ?? 0;

				SqlParameter p_pjt_type = new SqlParameter("@pjt_type", SqlDbType.Int);
				p_pjt_type.Value = data.pjt_type;

				SqlParameter p_recruit_number = new SqlParameter("@recruit_number", SqlDbType.Float);
				p_recruit_number.Value = data.recruit_number ?? 0;

				SqlParameter p_is_posting = new SqlParameter("@is_posting", SqlDbType.Float);
				p_is_posting.Value = data.is_posting ?? 0;

				SqlParameter p_title = new SqlParameter("@title", SqlDbType.VarChar, 500);
				p_title.Value = data.title as object ?? DBNull.Value;

				SqlParameter p_position_seq = new SqlParameter("@position_seq", SqlDbType.Int);
				p_position_seq.Value = data.position_seq ?? 0;

				SqlParameter p_experience_type = new SqlParameter("@experience_type", SqlDbType.Float);
				p_experience_type.Value = data.experience_type ?? 0;

				SqlParameter p_expreience_number = new SqlParameter("@expreience_number", SqlDbType.Float);
				p_expreience_number.Value = data.expreience_number ?? 0;

				SqlParameter p_edu_code = new SqlParameter("@edu_code", SqlDbType.Float);
				p_edu_code.Value = data.edu_code ?? 0;

				SqlParameter p_edu_name = new SqlParameter("@edu_name", SqlDbType.VarChar, 50);
				p_edu_name.Value = data.edu_name as object ?? DBNull.Value;

				SqlParameter p_foreign_lang = new SqlParameter("@foreign_lang", SqlDbType.Char, 2);
				p_foreign_lang.Value = data.foreign_lang as object ?? DBNull.Value;

				SqlParameter p_foreign_lang_name = new SqlParameter("@foreign_lang_name", SqlDbType.VarChar, 50);
				p_foreign_lang_name.Value = data.foreign_lang_name as object ?? DBNull.Value;

				SqlParameter p_foreign_level = new SqlParameter("@foreign_level", SqlDbType.Float);
				p_foreign_level.Value = data.foreign_level ?? 0;

				SqlParameter p_addr1 = new SqlParameter("@addr1", SqlDbType.VarChar, 300);
				p_addr1.Value = data.addr1 as object ?? DBNull.Value;

				SqlParameter p_addr2 = new SqlParameter("@addr2", SqlDbType.VarChar, 300);
				p_addr2.Value = data.addr2 as object ?? DBNull.Value;

				SqlParameter p_assign_task = new SqlParameter("@assign_task", SqlDbType.Text);
				p_assign_task.Value = data.assign_task as object ?? DBNull.Value;

				SqlParameter p_requirement = new SqlParameter("@requirement", SqlDbType.Text);
				p_requirement.Value = data.requirement as object ?? DBNull.Value;

				SqlParameter p_client_require = new SqlParameter("@client_require", SqlDbType.VarChar, 5000);
				p_client_require.Value = data.client_require as object ?? DBNull.Value;

				SqlParameter p_internal_note = new SqlParameter("@internal_note", SqlDbType.VarChar, 5000);
				p_internal_note.Value = data.internal_note as object ?? DBNull.Value;

				SqlParameter p_is_kor_resume = new SqlParameter("@is_kor_resume", SqlDbType.Float);
				p_is_kor_resume.Value = data.is_kor_resume ?? 0;

				SqlParameter p_is_eng_resume = new SqlParameter("@is_eng_resume", SqlDbType.Float);
				p_is_eng_resume.Value = data.is_eng_resume ?? 0;

				SqlParameter p_is_portfolio = new SqlParameter("@is_portfolio", SqlDbType.Int);
				p_is_portfolio.Value = data.is_portfolio ?? 0;

				SqlParameter p_is_self_introduction = new SqlParameter("@is_self_introduction", SqlDbType.Float);
				p_is_self_introduction.Value = data.is_self_introduction ?? 0;

				SqlParameter p_is_etc = new SqlParameter("@is_etc", SqlDbType.Float);
				p_is_etc.Value = data.is_etc ?? 0;

				SqlParameter p_etc_comment = new SqlParameter("@etc_comment", SqlDbType.VarChar, 800);
				p_etc_comment.Value = data.etc_comment as object ?? DBNull.Value;

				SqlParameter p_is_pre_interview = new SqlParameter("@is_pre_interview", SqlDbType.Float);
				p_is_pre_interview.Value = data.is_pre_interview ?? 0;

				SqlParameter p_is_number = new SqlParameter("@is_number", SqlDbType.Float);
				p_is_number.Value = data.is_number ?? 0;

				SqlParameter p_interview_number = new SqlParameter("@interview_number", SqlDbType.Float);
				p_interview_number.Value = data.interview_number ?? 0;

				SqlParameter p_is_personality_test = new SqlParameter("@is_personality_test", SqlDbType.Float);
				p_is_personality_test.Value = data.is_personality_test ?? 0;

				SqlParameter p_gender_type = new SqlParameter("@gender_type", SqlDbType.Float);
				p_gender_type.Value = data.gender_type ?? 0;

				SqlParameter p_start_age = new SqlParameter("@start_age", SqlDbType.Float);
				p_start_age.Value = data.start_age ?? 0;

				SqlParameter p_end_age = new SqlParameter("@end_age", SqlDbType.Float);
				p_end_age.Value = data.end_age ?? 0;

				//SqlParameter p_target_school = new SqlParameter("@target_school", SqlDbType.Int);
				//p_target_school.Value = data.target_school ?? 0;

				//SqlParameter p_target_major = new SqlParameter("@target_major", SqlDbType.Char, 12);
				//p_target_major.Value = data.target_major as object ?? DBNull.Value;

				//SqlParameter p_target_company = new SqlParameter("@target_company", SqlDbType.Int);
				//p_target_company.Value = data.target_company ?? 0;

				SqlParameter p_confidentiallity = new SqlParameter("@confidentiallity", SqlDbType.Int);
				p_confidentiallity.Value = data.confidentiallity ?? 0;

				SqlParameter p_expected_salary = new SqlParameter("@expected_salary", SqlDbType.Int);
				p_expected_salary.Value = data.expected_salary ?? 0;

				SqlParameter p_fee_rate = new SqlParameter("@fee_rate", SqlDbType.Decimal, 18);
				p_fee_rate.Value = data.fee_rate ?? 0;

				SqlParameter p_contents = new SqlParameter("@contents", SqlDbType.Text);
				p_contents.Value = data.contents as object ?? DBNull.Value;

				SqlParameter p_pjt_status = new SqlParameter("@pjt_status", SqlDbType.Int);
				p_pjt_status.Value = data.pjt_status ?? 0;

				SqlParameter p_status_comment = new SqlParameter("@status_comment", SqlDbType.VarChar, 1000);
				p_status_comment.Value = data.status_comment as object ?? DBNull.Value;

				SqlParameter p_is_cowork = new SqlParameter("@is_cowork", SqlDbType.Int);
				p_is_cowork.Value = data.is_cowork ?? 0;

				SqlParameter p_is_share_pjt = new SqlParameter("@is_share_pjt", SqlDbType.Int);
				p_is_share_pjt.Value = data.is_share_pjt ?? 0;

				SqlParameter p_share_comments = new SqlParameter("@share_comments", SqlDbType.VarChar, 500);
				p_share_comments.Value = data.share_comments as object ?? DBNull.Value;

				SqlParameter p_secret_info = new SqlParameter("@secret_info", SqlDbType.VarChar, 500);
				p_secret_info.Value = data.secret_info as object ?? DBNull.Value;

				SqlParameter p_share_fee_rate = new SqlParameter("@share_fee_rate", SqlDbType.Float);
				p_share_fee_rate.Value = data.share_fee_rate ?? 0;

				SqlParameter p_business_code1 = new SqlParameter("@business_code1", SqlDbType.Float);
				p_business_code1.Value = data.business_code1 ?? 0;

				SqlParameter p_business_code2 = new SqlParameter("@business_code2", SqlDbType.Float);
				p_business_code2.Value = data.business_code2 ?? 0;

				SqlParameter p_job_code1 = new SqlParameter("@job_code1", SqlDbType.Int);
				p_job_code1.Value = data.job_code1 ;

				SqlParameter p_job_code2 = new SqlParameter("@job_code2", SqlDbType.Int);
				p_job_code2.Value = data.job_code2;

				SqlParameter p_currency_cd = new SqlParameter("@currency_cd", SqlDbType.VarChar, 3);
				p_currency_cd.Value = data.currency_cd as object ?? DBNull.Value;

				SqlParameter p_share_dt = new SqlParameter("@share_dt", SqlDbType.DateTime);
				p_share_dt.Value = data.share_dt as object ?? DBNull.Value;

				SqlParameter p_cowork_dt = new SqlParameter("@cowork_dt", SqlDbType.DateTime);
				p_cowork_dt.Value = data.cowork_dt as object ?? DBNull.Value;

				SqlParameter p_position_str = new SqlParameter("@position_str", SqlDbType.VarChar, 50);
				p_position_str.Value = data.position_str as object ?? DBNull.Value;



				await db.Database.ExecuteSqlCommandAsync(@"exec sp_project_log  @event_idx, @event_type, @event_uv_seq,	  @p_seq , @c_seq , @cc_seq , @ctc_seq , @pjt_type , @recruit_number , @is_posting , @title , @position_seq , @experience_type , @expreience_number , @edu_code , @edu_name , @foreign_lang , @foreign_lang_name , @foreign_level , @addr1 , @addr2 , @assign_task , @requirement , @client_require , @internal_note , @is_kor_resume , @is_eng_resume , @is_portfolio , @is_self_introduction , @is_etc , @etc_comment , @is_pre_interview , @is_number , @interview_number , @is_personality_test , @gender_type , @start_age , @end_age , @target_school , @target_major , @target_company , @confidentiallity , @expected_salary , @fee_rate , @contents , @pjt_status , @status_comment , @is_cowork , @is_share_pjt , @share_comments , @secret_info , @share_fee_rate , @business_code1 , @business_code2 , @job_code1 , @job_code2 , @currency_cd , @share_dt , @cowork_dt , @position_str  "
				, p_event_idx, p_event_type, p_event_uv_seq
				 , p_p_seq
				, p_c_seq
				, p_cc_seq
				, p_ctc_seq
				, p_pjt_type
				, p_recruit_number
				, p_is_posting
				, p_title
				, p_position_seq
				, p_experience_type
				, p_expreience_number
				, p_edu_code
				, p_edu_name
				, p_foreign_lang
				, p_foreign_lang_name
				, p_foreign_level
				, p_addr1
				, p_addr2
				, p_assign_task
				, p_requirement
				, p_client_require
				, p_internal_note
				, p_is_kor_resume
				, p_is_eng_resume
				, p_is_portfolio
				, p_is_self_introduction
				, p_is_etc
				, p_etc_comment
				, p_is_pre_interview
				, p_is_number
				, p_interview_number
				, p_is_personality_test
				, p_gender_type
				, p_start_age
				, p_end_age
				//, p_target_school
				//, p_target_major
				//, p_target_company
				, p_confidentiallity
				, p_expected_salary
				, p_fee_rate
				, p_contents
				, p_pjt_status
				, p_status_comment
				, p_is_cowork
				, p_is_share_pjt
				, p_share_comments
				, p_secret_info
				, p_share_fee_rate				
				, p_business_code1
				, p_business_code2
				, p_job_code1
				, p_job_code2
				, p_currency_cd
				, p_share_dt
				, p_cowork_dt
				, p_position_str
				);
			}
			catch // (Exception e)
			{
			}
		}


		public async Task pjt_recandidate_history_log(pjt_recandidate_history data, string event_type = "", int event_uv_seq = 0, int event_idx = 0)
		{
			try
			{
				SqlParameter[] parameters = new SqlParameter[] {
				new SqlParameter("@event_idx", event_idx),
				new SqlParameter("@event_type", event_type),
				new SqlParameter("@event_uv_seq", event_uv_seq),
	 			new SqlParameter("@prc_seq", data.prc_seq),
				new SqlParameter("@pic_seq", data.pic_seq),
				new SqlParameter("@c_seq", data.c_seq),
				new SqlParameter("@p_seq", data.p_seq),
				new SqlParameter("@state", data.state),
				new SqlParameter("@schedule_date", data.schedule_date),
				new SqlParameter("@annual_income", data.annual_income),
				new SqlParameter("@guarantee", data.guarantee),
				new SqlParameter("@contents", data.contents),
				new SqlParameter("@file_directory", data.file_directory),
				new SqlParameter("@file_origin_path", data.file_origin_path),
				new SqlParameter("@file_path", data.file_path),
				new SqlParameter("@is_schedule", data.is_schedule),
				new SqlParameter("@create_dt", data.create_dt),
				new SqlParameter("@create_user", data.create_user),
				new SqlParameter("@modify_dt", data.modify_dt),
				new SqlParameter("@modify_user", data.modify_user),

			};
				await db.Database.ExecuteSqlCommandAsync("exec sp_pjt_recandidate_history_log  @event_idx, @event_type, @event_uv_seq,	  @prc_seq , @pic_seq , @c_seq , @p_seq , @state , @schedule_date , @annual_income , @guarantee , @contents , @file_directory , @file_origin_path , @file_path , @is_schedule , @create_dt , @create_user , @modify_dt , @modify_user  ", parameters);
			}
			catch // (Exception e)
			{
			}
		}

		public async Task schedule_log(schedule data, string event_type = "", int event_uv_seq = 0, int event_idx = 0)
		{
			try
			{
				SqlParameter[] parameters = new SqlParameter[] {
				new SqlParameter("@event_idx", event_idx),
				new SqlParameter("@event_type", event_type),
				new SqlParameter("@event_uv_seq", event_uv_seq),
	 			new SqlParameter("@s_seq", data.s_seq),
				new SqlParameter("@type", data.type),
				new SqlParameter("@sub_type", data.sub_type),
				new SqlParameter("@title", data.title),
				new SqlParameter("@start_date", data.start_date),
				new SqlParameter("@end_date", data.end_date),
				new SqlParameter("@contents", data.contents),
				new SqlParameter("@ud_seq", data.ud_seq),
				new SqlParameter("@prc_seq", data.prc_seq),
				new SqlParameter("@pic_seq", data.pic_seq),
				new SqlParameter("@p_seq", data.p_seq),
				new SqlParameter("@c_seq", data.c_seq),
				new SqlParameter("@cal_seq", data.cal_seq),
				new SqlParameter("@cl_seq", data.cl_seq),
				new SqlParameter("@meeting_id", data.meeting_id),
				new SqlParameter("@create_dt", data.create_dt),
				new SqlParameter("@create_user", data.create_user),
				new SqlParameter("@modify_dt", data.modify_dt),
				new SqlParameter("@modify_user", data.modify_user),
				new SqlParameter("@bg_color", data.bg_color),

			};
				await db.Database.ExecuteSqlCommandAsync("exec sp_schedule_log  @event_idx, @event_type, @event_uv_seq,	  @s_seq , @type , @sub_type , @title , @start_date , @end_date , @contents , @ud_seq , @prc_seq , @pic_seq , @p_seq , @c_seq , @cal_seq , @cl_seq , @meeting_id , @create_dt , @create_user , @modify_dt , @modify_user , @bg_color  ", parameters);
			}
			catch // (Exception e)
			{
			}
		}

		public async Task schedule_attend_log(schedule_attend data, string event_type = "", int event_uv_seq = 0, int event_idx = 0)
		{
			try
			{
				SqlParameter[] parameters = new SqlParameter[] {
				new SqlParameter("@event_idx", event_idx),
				new SqlParameter("@event_type", event_type),
				new SqlParameter("@event_uv_seq", event_uv_seq),
	 			new SqlParameter("@sa_seq", data.sa_seq),
				new SqlParameter("@s_seq", data.s_seq),
				new SqlParameter("@uv_seq", data.uv_seq),
				new SqlParameter("@create_dt", data.create_dt),
				new SqlParameter("@create_user", data.create_user),
				new SqlParameter("@modify_dt", data.modify_dt),
				new SqlParameter("@modify_user", data.modify_user),

			};
				await db.Database.ExecuteSqlCommandAsync("exec sp_schedule_attend_log  @event_idx, @event_type, @event_uv_seq,	  @sa_seq , @s_seq , @uv_seq , @create_dt , @create_user , @modify_dt , @modify_user  ", parameters);
			}
			catch // (Exception e)
			{
			}
		}

		public async Task schedule_attend_log(List<schedule_attend> list, string event_type = "", int event_uv_seq = 0, int event_idx = 0)
		{
			try
			{
				
				using (var tran = db.Database.BeginTransaction())
				{
					SqlParameter[] parameters = null;
					int i = 1;
					foreach (schedule_attend data in list)
					{
						parameters = new SqlParameter[] {
							new SqlParameter("@event_idx", i),
							new SqlParameter("@event_type", event_type),
							new SqlParameter("@event_uv_seq", event_uv_seq),
	 						new SqlParameter("@sa_seq", data.sa_seq),
							new SqlParameter("@s_seq", data.s_seq),
							new SqlParameter("@uv_seq", data.uv_seq),
							new SqlParameter("@create_dt", data.create_dt),
							new SqlParameter("@create_user", data.create_user),
							new SqlParameter("@modify_dt", data.modify_dt),
							new SqlParameter("@modify_user", data.modify_user),

						};
						await db.Database.ExecuteSqlCommandAsync("exec sp_schedule_attend_log  @event_idx, @event_type, @event_uv_seq,	  @sa_seq , @s_seq , @uv_seq , @create_dt , @create_user , @modify_dt , @modify_user  ", parameters);
					}
					i++;
					await db.SaveChangesAsync();
					tran.Commit();

				}
			}
			catch // (Exception e)
			{
			}
		}

		public async Task makeup_request_log(makeup_request data, string event_type = "", int event_uv_seq = 0, int event_idx = 0)
		{
			try
			{
				SqlParameter[] parameters = new SqlParameter[] {
				new SqlParameter("@event_idx", event_idx),
				new SqlParameter("@event_type", event_type),
				new SqlParameter("@event_uv_seq", event_uv_seq),
	 			new SqlParameter("@mr_idx", data.mr_idx),
				new SqlParameter("@req_user", data.req_user),
				new SqlParameter("@p_seq", data.p_seq),
				new SqlParameter("@c_seq", data.c_seq),
				new SqlParameter("@request_date", data.request_date),
				new SqlParameter("@resume_type", data.resume_type),
				new SqlParameter("@cr_seq", data.cr_seq),
				new SqlParameter("@cr_dir", data.cr_dir),
				new SqlParameter("@receipt_date", data.receipt_date),
				new SqlParameter("@receipt_seq", data.receipt_seq),
				new SqlParameter("@complete_date", data.complete_date),
				new SqlParameter("@complete_seq", data.complete_seq),
				new SqlParameter("@del_date", data.del_date),
				new SqlParameter("@del_yn", data.del_yn),
				new SqlParameter("@del_seq", data.del_seq),
				new SqlParameter("@reg_seq", data.reg_seq),
				new SqlParameter("@reg_date", data.reg_date),
				new SqlParameter("@mod_seq", data.mod_seq),
				new SqlParameter("@mod_date", data.mod_date),
				new SqlParameter("@status", data.status),

			};
				await db.Database.ExecuteSqlCommandAsync("exec sp_makeup_request_log  @event_idx, @event_type, @event_uv_seq,	  @mr_idx , @req_user , @p_seq , @c_seq , @request_date , @resume_type , @cr_seq , @cr_dir , @receipt_date , @receipt_seq , @complete_date , @complete_seq , @del_date , @del_yn , @del_seq , @reg_seq , @reg_date , @mod_seq , @mod_date , @status  ", parameters);
			}
			catch //(Exception e)
			{
			}
		}

		public async Task pjt_invoice_info_log(pjt_invoice_info data, string event_type = "", int event_uv_seq = 0, int event_idx = 0)
		{
			try
			{
				SqlParameter[] parameters = new SqlParameter[] {
				new SqlParameter("@event_idx", event_idx),
				new SqlParameter("@event_type", event_type),
				new SqlParameter("@event_uv_seq", event_uv_seq),
	 			new SqlParameter("@pii_seq", data.pii_seq),
				new SqlParameter("@p_seq", data.p_seq),
				new SqlParameter("@c_seq", data.c_seq),
				new SqlParameter("@prc_seq", data.prc_seq),
				new SqlParameter("@join_dt", data.join_dt),
				new SqlParameter("@billing_dt", data.billing_dt),
				new SqlParameter("@annual_income", data.annual_income),
				new SqlParameter("@fee_rate", data.fee_rate),
				new SqlParameter("@billing_money", data.billing_money),
				new SqlParameter("@billing_type", data.billing_type),
				new SqlParameter("@expire_guarantee", data.expire_guarantee),
				new SqlParameter("@is_open_name", data.is_open_name),
				new SqlParameter("@is_open_annual_income", data.is_open_annual_income),
				new SqlParameter("@invoice_lang", data.invoice_lang),
				new SqlParameter("@invoice_type", data.invoice_type),
				new SqlParameter("@remarks", data.remarks),
				new SqlParameter("@confirm_dt", data.confirm_dt),
				new SqlParameter("@confirm_user", data.confirm_user),
				new SqlParameter("@send_dt", data.send_dt),
				new SqlParameter("@send_user", data.send_user),
				new SqlParameter("@create_dt", data.create_dt),
				new SqlParameter("@create_user", data.create_user),
				new SqlParameter("@modify_dt", data.modify_dt),
				new SqlParameter("@modify_user", data.modify_user),
				new SqlParameter("@invoice_no", data.invoice_no),
				new SqlParameter("@is_file", data.is_file),
				new SqlParameter("@file_dir", data.file_dir),
				new SqlParameter("@file_origin_path", data.file_origin_path),
				new SqlParameter("@file_path", data.file_path),
				new SqlParameter("@file_extension", data.file_extension),
				new SqlParameter("@deposit_amt", data.deposit_amt),
				new SqlParameter("@deposit_dt", data.deposit_dt),
				new SqlParameter("@file_dt", data.file_dt),
				new SqlParameter("@file_user", data.file_user),
				new SqlParameter("@po_no", data.po_no),
				new SqlParameter("@pjt_title", data.pjt_title),
				new SqlParameter("@candidate_name", data.candidate_name),
				new SqlParameter("@client_name", data.client_name),
				new SqlParameter("@client_ceo", data.client_ceo),
				new SqlParameter("@client_addr1", data.client_addr1),
				new SqlParameter("@client_biz_code", data.client_biz_code),
				new SqlParameter("@client_fee_type", data.client_fee_type),
				new SqlParameter("@client_contact_name", data.client_contact_name),
				new SqlParameter("@client_contact_email", data.client_contact_email),
				new SqlParameter("@client_contact_phone", data.client_contact_phone),
				new SqlParameter("@client_contact_cell_phone", data.client_contact_cell_phone),
				new SqlParameter("@client_contact_division", data.client_contact_division),
				new SqlParameter("@etax_name", data.etax_name),
				new SqlParameter("@etax_email", data.etax_email),
				new SqlParameter("@etax_phone", data.etax_phone),
				new SqlParameter("@bill_currency_cd", data.bill_currency_cd),
				new SqlParameter("@income_currency_cd", data.income_currency_cd),
				new SqlParameter("@vat_type", data.vat_type),
				new SqlParameter("@is_po_no", data.is_po_no),
				new SqlParameter("@remark_admin", data.remark_admin),
				new SqlParameter("@is_deleted", data.is_deleted),

			};
				await db.Database.ExecuteSqlCommandAsync("exec sp_pjt_invoice_info_log  @event_idx, @event_type, @event_uv_seq,	  @pii_seq , @p_seq , @c_seq , @prc_seq , @join_dt , @billing_dt , @annual_income , @fee_rate , @billing_money , @billing_type , @expire_guarantee , @is_open_name , @is_open_annual_income , @invoice_lang , @invoice_type , @remarks , @confirm_dt , @confirm_user , @send_dt , @send_user , @create_dt , @create_user , @modify_dt , @modify_user , @invoice_no , @is_file , @file_dir , @file_origin_path , @file_path , @file_extension , @deposit_amt , @deposit_dt , @file_dt , @file_user , @po_no , @pjt_title , @candidate_name , @client_name , @client_ceo , @client_addr1 , @client_biz_code , @client_fee_type , @client_contact_name , @client_contact_email , @client_contact_phone , @client_contact_cell_phone , @client_contact_division   , @etax_name , @etax_email , @etax_phone ,  @bill_currency_cd , @income_currency_cd , @vat_type , @is_po_no , @remark_admin , @is_deleted  ", parameters);
			}
			catch // (Exception e)
			{
			}
		}

		public async Task pjt_memo_log(pjt_memo data, string event_type = "", int event_uv_seq = 0, int event_idx = 0)
		{
			try
			{
				SqlParameter[] parameters = new SqlParameter[] {
				new SqlParameter("@event_idx", event_idx),
				new SqlParameter("@event_type", event_type),
				new SqlParameter("@event_uv_seq", event_uv_seq),
	 			new SqlParameter("@pm_seq", data.pm_seq),
				new SqlParameter("@p_seq", data.p_seq),
				new SqlParameter("@memo", data.memo),
			};
				await db.Database.ExecuteSqlCommandAsync("exec sp_pjt_memo_log  @event_idx, @event_type, @event_uv_seq,	  @pm_seq , @p_seq , @memo  ", parameters);
			}
			catch // (Exception e)
			{
			}
		}

		public async Task pjt_share_board_log(pjt_share_board data, string event_type = "", int event_uv_seq = 0, int event_idx = 0)
		{
			try
			{
				SqlParameter[] parameters = new SqlParameter[] {
				new SqlParameter("@event_idx", event_idx),
				new SqlParameter("@event_type", event_type),
				new SqlParameter("@event_uv_seq", event_uv_seq),
	 			new SqlParameter("@p_seq", data.p_seq),
				new SqlParameter("@psb_seq", data.psb_seq),
				new SqlParameter("@contents", data.contents),
				new SqlParameter("@create_dt", data.create_dt),
				new SqlParameter("@create_user", data.create_user),
				new SqlParameter("@modify_dt", data.modify_dt),
				new SqlParameter("@modify_user", data.modify_user),

			};
				await db.Database.ExecuteSqlCommandAsync("exec sp_pjt_share_board_log  @event_idx, @event_type, @event_uv_seq,	  @p_seq , @psb_seq , @contents , @create_dt , @create_user , @modify_dt , @modify_user  ", parameters);
			}
			catch //(Exception e)
			{
			}
		}

		public async Task pjt_share_reply_log(pjt_share_reply data, string event_type = "", int event_uv_seq = 0, int event_idx = 0)
		{
			try
			{
				SqlParameter[] parameters = new SqlParameter[] {
				new SqlParameter("@event_idx", event_idx),
				new SqlParameter("@event_type", event_type),
				new SqlParameter("@event_uv_seq", event_uv_seq),
	 			new SqlParameter("@p_seq", data.p_seq),
				new SqlParameter("@psb_seq", data.psb_seq),
				new SqlParameter("@psr_seq", data.psr_seq),
				new SqlParameter("@contents", data.contents),
				new SqlParameter("@create_dt", data.create_dt),
				new SqlParameter("@create_user", data.create_user),
				new SqlParameter("@modify_dt", data.modify_dt),
				new SqlParameter("@modify_user", data.modify_user),

			};
				await db.Database.ExecuteSqlCommandAsync("exec sp_pjt_share_reply_log  @event_idx, @event_type, @event_uv_seq,	  @p_seq , @psb_seq , @psr_seq , @contents , @create_dt , @create_user , @modify_dt , @modify_user  ", parameters);
			}
			catch //(Exception e)
			{
			}
		}

		public async Task pjt_share_reply_log(List<pjt_share_reply> list, string event_type = "", int event_uv_seq = 0, int event_idx = 0)
		{
			try
			{ 
				using (var tran = db.Database.BeginTransaction())
				{
					SqlParameter[] parameters = null;
					int i = 1;
					foreach (pjt_share_reply data in list)
					{
						parameters = new SqlParameter[] {
							new SqlParameter("@event_idx", i),
							new SqlParameter("@event_type", event_type),
							new SqlParameter("@event_uv_seq", event_uv_seq),
	 						new SqlParameter("@p_seq", data.p_seq),
							new SqlParameter("@psb_seq", data.psb_seq),
							new SqlParameter("@psr_seq", data.psr_seq),
							new SqlParameter("@contents", data.contents),
							new SqlParameter("@create_dt", data.create_dt),
							new SqlParameter("@create_user", data.create_user),
							new SqlParameter("@modify_dt", data.modify_dt),
							new SqlParameter("@modify_user", data.modify_user),

						};
						await db.Database.ExecuteSqlCommandAsync("exec sp_pjt_share_reply_log  @event_idx, @event_type, @event_uv_seq,	  @p_seq , @psb_seq , @psr_seq , @contents , @create_dt , @create_user , @modify_dt , @modify_user  ", parameters);
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

		public async Task<inorder_log> SelectInorderLog(int log_seq = 0)
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
	  ,(select name from uv_user where uv_seq = i.event_uv_seq) event_name
      ,[event_dt]
      ,[i_seq]
      ,i.[p_seq]
      ,i.[c_seq]
      ,i.[cc_seq]
      ,[inorder_type]
      ,[inorder_status]
      ,[clt_name]
      ,[clt_busi_type]
      ,[clt_url]
      ,[cc_name]
      ,[cc_gender]
      ,[cc_division]
      ,[cc_position]
      ,[cc_phone]
      ,[cc_cell_phone]
      ,[cc_email]
      ,[cc_reason]
      ,[can_dept]
      ,[can_pos]
      ,[can_location]
      ,[can_jobdesc]
      ,[can_contents]
      ,i.[create_dt]
      ,i.[create_user]
      ,i.[modify_dt]
      ,i.[modify_user]
      ,[inorder_dt]
      ,[director_seq]
      ,[director_dt]
      ,C.kor_name as client_name
      ,CC.name as contact_name
      ,P.title as project_title
      ,P.pjt_status
      ,P.create_dt AS project_dt
      ,PD.am_names
      ,PM.searcher_names
      ,U.name AS director_name
      ,U.hp AS director_cp
      ,U.tel AS director_tel
      ,U.email AS director_email
    FROM Inorder_log AS I LEFT OUTER JOIN client AS C
                                   ON I.c_seq = C.c_seq
                      LEFT OUTER JOIN project AS P
                                   ON I.p_seq = P.p_seq
                      LEFT OUTER JOIN (SELECT distinct PD.p_seq
                                             ,STUFF((SELECT ',' + B.name
                                                       FROM pjt_director AS A INNER JOIN uv_user AS B
                                                                                      ON A.uv_seq = B.uv_seq
                                                      WHERE A.p_seq = PD.p_seq
                                                        FOR XML PATH('')), 1, 1, '') AS am_names
                                         FROM pjt_director AS PD) AS PD
                                   ON I.p_seq = PD.p_seq
                      LEFT OUTER JOIN (SELECT distinct PM.p_seq
                                             ,STUFF((SELECT ',' + B.name
                                                       FROM pjt_manager AS A INNER JOIN uv_user AS B
                                                                                     ON A.uv_seq = B.uv_seq
                                                      WHERE A.p_seq = PM.p_seq
                                                        FOR XML PATH('')), 1, 1, '') AS searcher_names
                                         FROM pjt_manager AS PM) AS PM
                                   ON I.p_seq = PM.p_seq
                      LEFT OUTER JOIN client_contact AS CC
                                   ON I.cc_seq = CC.cc_seq
                      LEFT OUTER JOIN uv_user AS U
                                   ON I.director_seq = U.uv_seq
    WHERE I.log_seq = @log_seq";

					DynamicParameters param = new DynamicParameters();
					param.Add("@log_seq", log_seq, DbType.Int32);

					var ret = await con.QueryAsync<inorder_log>(selectQuery, param);

					con.Close();

					return ret.FirstOrDefault();
				}
			}
			catch (Exception e)
			{
				throw e;
			}
		}

		public async Task<List<inorder_log>> ListInorderLogSummary(int i_seq)
		{
			try
			{
				using (IDbConnection con = new SqlConnection(base.BaseConnectionString))
				{
					con.Open();

					string selectQuery = @" 
select
	log_seq
	,event_idx
	,event_dt
	,event_type
	,event_uv_seq
	,(select name from uv_user where uv_seq = a.event_uv_seq) event_name
from inorder_log a
where i_seq = @i_seq
 order by log_seq desc
";

					DynamicParameters param = new DynamicParameters();
					param.Add("@i_seq", i_seq, DbType.Int32);

					var ret = await con.QueryAsync<inorder_log>(selectQuery, param);

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
		/// 프로젝트 단건 조회 - 채용
		/// </summary>
		/// <returns></returns>
		public async Task<project_log> SelectProjectLog(int log_seq)
		{
			try
			{
				using (IDbConnection con = new SqlConnection(base.BaseConnectionString))
				{
					con.Open();

					string selectQuery = @" SELECT  [log_seq]
      ,[event_idx]
      ,[event_type]
      ,[event_uv_seq]
      ,[event_dt]
      ,[p_seq]
      ,p.[c_seq]
      ,p.[cc_seq]
      ,p.[ctc_seq]
      ,[pjt_type]
      ,[recruit_number]
      ,[is_posting]
      ,[title]
      ,[position_seq]
      ,[experience_type]
      ,[expreience_number]
      ,[edu_code]
      ,[edu_name]
      ,[foreign_lang]
      ,[foreign_lang_name]
      ,[foreign_level]
      ,p.[addr1]
      ,p.[addr2]
      ,[assign_task]
      ,[requirement]
      ,[client_require]
      ,[internal_note]
      ,[is_kor_resume]
      ,[is_eng_resume]
      ,[is_portfolio]
      ,[is_self_introduction]
      ,[is_etc]
      ,[etc_comment]
      ,[is_pre_interview]
      ,[is_number]
      ,[interview_number]
      ,[is_personality_test]
      ,[gender_type]
      ,[start_age]
      ,[end_age]
      ,[target_school]
      ,[target_major]
      ,[target_company]
      ,[confidentiallity]
      ,[expected_salary]
      ,[fee_rate]
      ,[contents]
      ,[pjt_status]
      ,[status_comment]
      ,[is_cowork]
      ,[is_share_pjt]
      ,[share_comments]
      ,[secret_info]
      ,[share_fee_rate]
      ,p.[create_dt]
      ,p.[create_user]
      ,p.[modify_dt]
      ,p.[modify_user]
      ,[business_code1]
      ,[business_code2]
      ,[job_code1]
      ,[job_code2]
      ,[currency_cd]
      ,[share_dt]
      ,[cowork_dt]
      ,[position_str]
      ,C.kor_name AS company_name
      ,CS.campus_name AS target_school_campus
      ,CS.school_name AS target_school_name
      ,CM.major_name AS target_major_name
      ,GC.WKPL_NM AS target_company_name
      ,CT.division AS tax_division
      ,CT.name AS  tax_name
      ,CT.email AS tax_email
      ,CT.phone AS tax_phone
      ,CT.cell_phone AS tax_cell_phone
      ,CT.deposit_manager AS tax_deposit_manager
      ,CT.deposit_email AS tax_deposit_email
      ,CC.name AS contact_name
      ,CC.gender AS contact_gender
      ,CC.email AS contact_email
      ,CC.phone AS contact_phone
      ,CC.cell_phone AS contact_cell_phone
      ,CC.division AS contact_division
      ,CC.position AS contact_position
      ,ISNULL(P.business_code1, 0) AS business_code1
      ,ISNULL(P.business_code2, 0) AS business_code2
      ,ISNULL(P.job_code1, 0) AS job_code1
      ,ISNULL(P.job_code2, 0) AS job_code2
      ,CB.code_name2 AS business_name2
      ,CJ.code_name2 AS job_name2
  FROM project_log AS P INNER JOIN client AS C
                            ON P.c_seq = C.c_seq
               LEFT OUTER JOIN code_school AS CS
                            ON P.target_school = CS.sc_seq
               LEFT OUTER JOIN code_major_2018 AS CM
                            ON P.target_major = CM.major_code
               LEFT OUTER JOIN gov_api_company AS GC
                            ON P.target_company = GC.G_SEQ
               LEFT OUTER JOIN client_contact AS CC
                            ON P.c_seq = CC.c_seq
                           AND P.cc_seq = CC.cc_seq
               LEFT OUTER JOIN client_tax_contact AS CT
                            ON P.c_seq = CT.c_seq
                           AND P.ctc_seq = CT.ctc_seq
               LEFT OUTER JOIN code_job2 AS CJ
                            ON P.job_code1 = CJ.code1
                           AND P.job_code2 = CJ.code2
               LEFT OUTER JOIN code_business2 AS CB
                            ON P.business_code1 = CB.code1
                           AND P.business_code2 = CB.code2
 WHERE log_seq = @log_seq";

					DynamicParameters param = new DynamicParameters();
					param.Add("@log_seq", log_seq, DbType.Int32);

					var ret = await con.QueryAsync<project_log>(selectQuery, param);

					con.Close();

					return ret.FirstOrDefault();
				}
			}
			catch (Exception e)
			{
				throw e;
			}
		}

		/// 프로젝트 단건 조회 - 채용
		/// </summary>
		/// <returns></returns>
		public async Task<List<project_log>> ListProjectLogSummary(int p_seq)
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
      ,[p_seq]
     ,(select name from uv_user where uv_seq = event_uv_seq) event_name
  FROM [project_log] 
  where p_seq = @p_seq
  order by log_seq desc
";

					DynamicParameters param = new DynamicParameters();
					param.Add("@p_seq", p_seq, DbType.Int32);

					var ret = await con.QueryAsync<project_log>(selectQuery, param);

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
		/// 프로젝트 책임자(AM) 리스트 조회
		/// </summary>
		/// <param name="p_seq"></param>
		/// <returns></returns>
		public async Task<List<pjt_director_log>> ListPjtDirectorLog(int pjt_log_seq)
		{
			try
			{
				List<pjt_director_log> list = new List<pjt_director_log>();

				using (IDbConnection con = new SqlConnection(base.BaseConnectionString))
				{
					con.Open();

					string selectQuery = @" SELECT [log_seq]
      ,pd.[pjt_log_seq]
      ,[event_idx]
      ,[event_uv_seq]
      ,[event_dt]
      ,[p_seq]
      ,pd.[uv_seq]
      ,U.name
      ,UD.ud_name
  FROM pjt_director_log AS PD INNER JOIN uv_user AS U
                                  ON PD.uv_seq = U.uv_seq
                          INNER JOIN uv_division AS UD
                                  ON U.ud_seq = UD.ud_seq
 WHERE PD.pjt_log_seq = @pjt_log_seq ";

					DynamicParameters param = new DynamicParameters();
					param.Add("@pjt_log_seq", pjt_log_seq, DbType.Int32);

					var ret = await con.QueryAsync<pjt_director_log>(selectQuery, param);

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
		/// 프로젝트 담당자(Searcher) 조회
		/// </summary>
		/// <param name="p_seq"></param>
		/// <returns></returns>
		public async Task<List<pjt_manager_log>> ListPjtManagerLog(int pjt_log_seq)
		{
			try
			{
				List<pjt_manager_log> list = new List<pjt_manager_log>();

				using (IDbConnection con = new SqlConnection(base.BaseConnectionString))
				{
					con.Open();

					string selectQuery = @" SELECT 
	   pm.[log_seq]
      ,pm.[pjt_log_seq]
      ,pm.[event_idx]
      ,pm.[event_uv_seq]
      ,pm.[event_dt]
      ,pm.[p_seq]
      ,pm.[uv_seq]
      ,U.name
      ,UD.ud_name
  FROM pjt_manager_log AS PM INNER JOIN uv_user AS U
                                 ON PM.uv_seq = U.uv_seq
                         INNER JOIN uv_division AS UD
                                 ON U.ud_seq = UD.ud_seq
 WHERE PM.pjt_log_seq = @pjt_log_seq ";

					DynamicParameters param = new DynamicParameters();
					param.Add("@pjt_log_seq", pjt_log_seq, DbType.Int32);

					var ret = await con.QueryAsync<pjt_manager_log>(selectQuery, param);

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
		/// 프로젝트 근무지 리스트 조회.
		/// </summary>
		/// <param name="p_seq"></param>
		/// <returns></returns>
		public async Task<List<pjt_place_log>> ListPjtPlaceLog(int pjt_log_seq)
		{
			try
			{
				List<pjt_place_log> list = new List<pjt_place_log>();

				using (IDbConnection con = new SqlConnection(base.BaseConnectionString))
				{
					con.Open();

					string selectQuery = @" SELECT [log_seq]
      ,[pjt_log_seq]
      ,[event_idx]
      ,[event_uv_seq]
      ,[event_dt]
      ,[p_seq]
      ,[pp_seq]
      ,[code1]
      ,[area1]
      ,[code2]
      ,[area2]
  FROM [dbo].[pjt_place_log]
  where pjt_log_seq = @pjt_log_seq";

					DynamicParameters param = new DynamicParameters();
					param.Add("@pjt_log_seq", pjt_log_seq, DbType.Int32);

					var ret = await con.QueryAsync<pjt_place_log>(selectQuery, param);

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
		/// 프로젝트 키워드 리스트 조회.
		/// </summary>
		/// <param name="p_seq"></param>
		/// <returns></returns>
		public async Task<List<pjt_keyword_log>> ListPjtKeywordLog(int pjt_log_seq)
		{
			try
			{
				List<pjt_keyword_log> list = new List<pjt_keyword_log>();

				using (IDbConnection con = new SqlConnection(base.BaseConnectionString))
				{
					con.Open();

					string selectQuery = @" SELECT [log_seq]
      ,[pjt_log_seq]
      ,[event_idx]
      ,[event_uv_seq]
      ,[event_dt]
      ,[p_seq]
      ,[pk_seq]
      ,[pk_name]
  FROM [dbo].[pjt_keyword_log]
    where pjt_log_seq = @pjt_log_seq";

					DynamicParameters param = new DynamicParameters();
					param.Add("@pjt_log_seq", pjt_log_seq, DbType.Int32);

					var ret = await con.QueryAsync<pjt_keyword_log>(selectQuery, param);

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
