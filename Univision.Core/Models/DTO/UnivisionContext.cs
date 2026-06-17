using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace Univision.Core.Models.DTO
{

  public class UnivisionContext : DbContext
  {
    private static string GetConnectionString()
    {
#if DEBUG
            return ConfigurationManager.ConnectionStrings["UnivisionEntities"].ConnectionString;
#else
      return ConfigurationManager.ConnectionStrings["UnivisionEntities"].ConnectionString;
#endif
    }

    /// <summary>Constructor</summary>
    public UnivisionContext() : base(GetConnectionString()) { }

    #region dbSet

    public DbSet<addr1> addr1s { get; set; }
    public DbSet<addr2> addr2s { get; set; }
    public DbSet<addr3> addr3s { get; set; }
    public DbSet<alarm_message> alarm_messages { get; set; }
    public DbSet<alarm_user> alarm_users { get; set; }
    public DbSet<api_company> api_companys { get; set; }
    public DbSet<board> board { get; set; }
    public DbSet<board_read_history> board_read_historys { get; set; }
    public DbSet<board_file> board_files { get; set; }
    public DbSet<can_activity> can_activitys { get; set; }
    public DbSet<can_business_code> can_business_codes { get; set; }
    public DbSet<can_business> can_businesss { get; set; }
    public DbSet<can_career> can_careers { get; set; }
    public DbSet<can_career_job> can_career_jobs { get; set; }
    public DbSet<can_foreign_lan> can_foreign_lans { get; set; }
    public DbSet<can_interest> can_interests { get; set; }
    public DbSet<can_interview_sheet> can_interview_sheets { get; set; }
    public DbSet<can_job_code> can_job_codes { get; set; }
    public DbSet<can_job> can_jobs { get; set; }
    public DbSet<can_memo> can_memos { get; set; }
    public DbSet<can_place> can_places { get; set; }
    public DbSet<can_resume> can_resumes { get; set; }

    public DbSet<director> directors { get; set; }
    public DbSet<director_career> director_careers { get; set; }    
    public DbSet<director_school> director_schools { get; set; }
    public DbSet<director_activity> director_activitys { get; set; }

    public DbSet<director_business> director_businesss { get; set; }
    public DbSet<director_job> director_jobs { get; set; }

    public DbSet<privacy_agree> privacy_agrees { get; set; }

    public DbSet<privacy_agree_dtl> privacy_agree_dtls { get; set; }
    public DbSet<can_school> can_schools { get; set; }
    public DbSet<candidate_search_save> candidate_search_saves { get; set; }
    public DbSet<candidate> candidate { get; set; }
    public DbSet<client> clients { get; set; }
    public DbSet<client_activity_log> client_activity_logs { get; set; }
    public DbSet<client_annual_income_rate> client_annual_income_rates { get; set; }
    public DbSet<client_contact> client_contacts { get; set; }
    public DbSet<client_contract> client_contracts { get; set; }
    public DbSet<client_contract_file> client_Contract_files { get; set; }
    public DbSet<client_favorite> client_favorites { get; set; }
    public DbSet<client_file> client_files { get; set; }
    public DbSet<client_manager> client_manager { get; set; }
    public DbSet<client_position_rate> client_position_rates { get; set; }
    public DbSet<client_tax_contact> client_tax_contacts { get; set; }
    public DbSet<code_keyword> code_keywords { get; set; }
    public DbSet<code_can_keyword> code_can_keywords { get; set; }
    public DbSet<code_board_sub1> code_board_sub1s { get; set; }
    public DbSet<code_board_sub2> code_board_sub2s { get; set; }
    public DbSet<code_business1> code_business1s { get; set; }
    public DbSet<code_business2> code_business2s { get; set; }
    public DbSet<code_business_mst> code_business_msts { get; set; }
    public DbSet<code_business_dtl> code_business_dtls { get; set; }
    public DbSet<code_business3> code_business3s { get; set; }
    public DbSet<code_can_education> code_can_educations { get; set; }
    public DbSet<code_graduate_status> code_graduate_statuss { get; set; }
    public DbSet<code_education_level> code_education_levels { get; set; }
    public DbSet<code_foreign_lan> code_foreign_lans { get; set; }
    public DbSet<code_job1> code_job1s { get; set; }
    public DbSet<code_job2> code_job2s { get; set; }
    public DbSet<code_job_mst> code_job_msts { get; set; }
    public DbSet<code_job_dtl> code_job_dtls { get; set; }
    public DbSet<code_job3> code_job3s { get; set; }
    public DbSet<code_major_2018> code_major_2018s { get; set; }
    public DbSet<Code_Money_Unit_Table> Code_Money_Unit_Tables { get; set; }
    public DbSet<code_pjt_interview_status> code_pjt_interview_statuss { get; set; }
    public DbSet<code_position> code_positions { get; set; }
    public DbSet<code_rank> code_ranks { get; set; }
    public DbSet<code_salary_code> code_salary_codes { get; set; }
    public DbSet<code_school> code_schools { get; set; }
    public DbSet<code_school_search> code_school_searchs { get; set; }
    public DbSet<code_sectors1> code_sectors1s { get; set; }
    public DbSet<code_sectors2> code_sectors2s { get; set; }
    public DbSet<code_sectors3> code_sectors3s { get; set; }
    public DbSet<code_work_type> code_work_types { get; set; }

    public DbSet<exchange> exchanges { get; set; }
    public DbSet<gov_api_company> gov_api_companys { get; set; }

    public DbSet<inorder> inorders { get; set; }
    public DbSet<inorder_director> Inorder_directors { get; set; }
    public DbSet<inorder_file> inorder_files { get; set; }
    public DbSet<inorder_memo> inorder_memos { get; set; }
    public DbSet<makeup_request> makeup_requests { get; set; }
    public DbSet<gpt_use_history> gpt_use_historys { get; set; }
    public DbSet<meeting_room> meeting_rooms { get; set; }
    public DbSet<MEETING_ROOM_NOTICE> MEETING_ROOM_NOTICEs { get; set; }
    public DbSet<meeting_room_schedule> meeting_room_schedules { get; set; }
    public DbSet<pjt_business_code> pjt_business_codes { get; set; }
    public DbSet<pjt_contact> pjt_contacts { get; set; }
    public DbSet<pjt_director> pjt_directors { get; set; }
    public DbSet<pjt_file> pjt_files { get; set; }
    public DbSet<pjt_icandidate> pjt_icandidates { get; set; }
    public DbSet<pjt_invoice_info> pjt_invoice_infos { get; set; }
    public DbSet<pjt_invoice_sales> pjt_invoice_saless { get; set; }
    public DbSet<pjt_job_code> pjt_job_codes { get; set; }
    public DbSet<pjt_keyword> pjt_keywords { get; set; }
    public DbSet<pjt_manager> pjt_managers { get; set; }
    public DbSet<pjt_mykey> pjt_mykeys { get; set; }
    public DbSet<pjt_memo> pjt_memos { get; set; }
    public DbSet<pjt_place> pjt_places { get; set; }
    public DbSet<pjt_recandidate> pjt_recandidates { get; set; }
    public DbSet<pjt_recandidate_history> pjt_recandidate_historys { get; set; }
    public DbSet<pjt_tax_contract> pjt_tax_contracts { get; set; }
    public DbSet<pjt_tmp_icandidate> pjt_tmp_icandidates { get; set; }
    public DbSet<project> projects { get; set; }
    public DbSet<project_read_history> project_read_historys { get; set; }
    public DbSet<pjt_share_board> pjt_share_boards { get; set; }
    public DbSet<pjt_share_reply> pjt_share_replys { get; set; }
    
    public DbSet<remember_task> remember_tasks { get; set; }
        public DbSet<remember_task_tmp> remember_task_tmps { get; set; }
    public DbSet<remember_task_his> remember_task_hiss { get; set; }

    public DbSet<simple_candidate> simple_candidates { get; set; }

    public DbSet<uv_auth> uv_auths { get; set; }
    public DbSet<uv_division> uv_divisions { get; set; }
    public DbSet<uv_failed_login> uv_failed_logins { get; set; }
    public DbSet<uv_login_history> uv_login_historys { get; set; }
    public DbSet<uv_user> uv_users { get; set; }
    public DbSet<uv_vacation> uv_vacations { get; set; }
    public DbSet<uv_vacation_history> uv_vacation_historys { get; set; }
    public DbSet<uv_vacation_history_dtl> uv_vacation_history_dtls { get; set; }

    public DbSet<uv_user_attend> uv_user_attends { get; set; }

    public DbSet<receipt> receipts { get; set; }
    public DbSet<receipt_user> receipt_users { get; set; }
    public DbSet<receipt_user_dtl> receipt_user_dtls { get; set; }

    public DbSet<schedule> schedules { get; set; }
    public DbSet<schedule_attend> schedule_attends { get; set; }

    public DbSet<sms_history> sms_historys { get; set; }
    public DbSet<sms_report> sms_reports { get; set; }


    public DbSet<tempsaved_can_business_code> tempsaved_can_business_codes { get; set; }
    public DbSet<tempsaved_can_business> tempsaved_can_businesss { get; set; }
    public DbSet<tempsaved_can_career> tempsaved_can_careers { get; set; }
    public DbSet<tempsaved_can_career_job> tempsaved_can_career_jobs { get; set; }
    public DbSet<tempsaved_can_foreign_lan> tempsaved_can_foreign_lans { get; set; }
    public DbSet<tempsaved_can_job_code> tempsaved_can_job_codes { get; set; }
    public DbSet<tempsaved_can_job> tempsaved_can_s { get; set; }
    public DbSet<tempsaved_can_place> tempsaved_can_places { get; set; }
    public DbSet<tempsaved_can_resume> tempsaved_can_resumes { get; set; }
    public DbSet<tempsaved_can_school> tempsaved_can_schools { get; set; }
    public DbSet<tempsaved_candidate> tempsaved_candidates { get; set; }

    public DbSet<element_open_log> element_open_logs { get; set; }

    public DbSet<auth_code_temp> auth_code_temps { get; set; }


    public DbSet<mail_report_mst> mail_report_msts { get; set; }
    public DbSet<mail_report_last_list> mail_report_last_lists { get; set; }
    public DbSet<mail_report_pjt_list> mail_report_pjt_lists { get; set; }
    public DbSet<mail_report_send_his> mail_report_send_hiss { get; set; }
    public DbSet<mail_report_this_list> mail_report_this_lists { get; set; }


    public DbSet<mail_resume> mail_resumes { get; set; }
    public DbSet<mail_resume_file> mail_resume_files { get; set; }
    public DbSet<mail_resume_gpt> mail_resume_gpts { get; set; }

    public DbSet<invoice_new> invoice_news { get; set; }
    public DbSet<invoice_new_dtl> invoice_new_dtls { get; set; }

    public DbSet<test_invoice_new> test_invoice_news { get; set; }
    public DbSet<test_invoice_new_dtl> test_invoice_new_dtls { get; set; }
    #endregion
  }
}
