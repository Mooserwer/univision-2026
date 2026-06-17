using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Univision.Core.Models.DTO.Logs
{
    [Table("project_log")]
    public partial class project_log
    {
        /// <summary>
        /// 
        /// <summary> 
        [Key]
        public int log_seq { get; set; }

        /// <summary>
        /// 
        /// <summary> 
        public int? event_idx { get; set; }

        /// <summary>
        /// 
        /// <summary> 
        public string event_type { get; set; }

        /// <summary>
        /// 
        /// <summary> 
        public int? event_uv_seq { get; set; }

        /// <summary>
        /// 
        /// <summary> 
        public DateTime? event_dt { get; set; }

        /// <summary>
        /// 
        /// <summary> 
        public int p_seq { get; set; }

        /// <summary>
        /// 
        /// <summary> 
        public int? c_seq { get; set; }

        /// <summary>
        /// 
        /// <summary> 
        public int? cc_seq { get; set; }

        /// <summary>
        /// 
        /// <summary> 
        public int? ctc_seq { get; set; }

        /// <summary>
        /// 
        /// <summary> 
        public int pjt_type { get; set; }

        /// <summary>
        /// 
        /// <summary> 
        public float? recruit_number { get; set; }

        /// <summary>
        /// 
        /// <summary> 
        public float? is_posting { get; set; }

        /// <summary>
        /// 
        /// <summary> 
        public string title { get; set; }

        /// <summary>
        /// 
        /// <summary> 
        public int? position_seq { get; set; }

        /// <summary>
        /// 
        /// <summary> 
        public float? experience_type { get; set; }

        /// <summary>
        /// 
        /// <summary> 
        public float? expreience_number { get; set; }

        /// <summary>
        /// 
        /// <summary> 
        public float? edu_code { get; set; }

        /// <summary>
        /// 
        /// <summary> 
        public string edu_name { get; set; }

        /// <summary>
        /// 
        /// <summary> 
        public string foreign_lang { get; set; }

        /// <summary>
        /// 
        /// <summary> 
        public string foreign_lang_name { get; set; }

        /// <summary>
        /// 
        /// <summary> 
        public float? foreign_level { get; set; }

        /// <summary>
        /// 
        /// <summary> 
        public string addr1 { get; set; }

        /// <summary>
        /// 
        /// <summary> 
        public string addr2 { get; set; }

        /// <summary>
        /// 
        /// <summary> 
        public string assign_task { get; set; }

        /// <summary>
        /// 
        /// <summary> 
        public string requirement { get; set; }

        /// <summary>
        /// 
        /// <summary> 
        public string client_require { get; set; }

        /// <summary>
        /// 
        /// <summary> 
        public string internal_note { get; set; }

        /// <summary>
        /// 
        /// <summary> 
        public float? is_kor_resume { get; set; }

        /// <summary>
        /// 
        /// <summary> 
        public float? is_eng_resume { get; set; }

        /// <summary>
        /// 
        /// <summary> 
        public int? is_portfolio { get; set; }

        /// <summary>
        /// 
        /// <summary> 
        public float? is_self_introduction { get; set; }

        /// <summary>
        /// 
        /// <summary> 
        public float? is_etc { get; set; }

        /// <summary>
        /// 
        /// <summary> 
        public string etc_comment { get; set; }

        /// <summary>
        /// 
        /// <summary> 
        public float? is_pre_interview { get; set; }

        /// <summary>
        /// 
        /// <summary> 
        public float? is_number { get; set; }

        /// <summary>
        /// 
        /// <summary> 
        public float? interview_number { get; set; }

        /// <summary>
        /// 
        /// <summary> 
        public float? is_personality_test { get; set; }

        /// <summary>
        /// 
        /// <summary> 
        public float? gender_type { get; set; }

        /// <summary>
        /// 
        /// <summary> 
        public float? start_age { get; set; }

        /// <summary>
        /// 
        /// <summary> 
        public float? end_age { get; set; }

        /// <summary>
        /// 
        /// <summary> 
        public int? target_school { get; set; }

        /// <summary>
        /// 
        /// <summary> 
        public string target_major { get; set; }

        /// <summary>
        /// 
        /// <summary> 
        public int? target_company { get; set; }

        /// <summary>
        /// 
        /// <summary> 
        public int? confidentiallity { get; set; }

        /// <summary>
        /// 
        /// <summary> 
        public int? expected_salary { get; set; }

        /// <summary>
        /// 
        /// <summary> 
        public decimal? fee_rate { get; set; }

        /// <summary>
        /// 
        /// <summary> 
        public string contents { get; set; }

        /// <summary>
        /// 
        /// <summary> 
        public int? pjt_status { get; set; }

        /// <summary>
        /// 
        /// <summary> 
        public string status_comment { get; set; }

        /// <summary>
        /// 
        /// <summary> 
        public int? is_cowork { get; set; }

        /// <summary>
        /// 
        /// <summary> 
        public int? is_share_pjt { get; set; }

        /// <summary>
        /// 
        /// <summary> 
        public string share_comments { get; set; }

        /// <summary>
        /// 
        /// <summary> 
        public string secret_info { get; set; }

        /// <summary>
        /// 
        /// <summary> 
        public float? share_fee_rate { get; set; }

        /// <summary>
        /// 
        /// <summary> 
        public DateTime? create_dt { get; set; }

        /// <summary>
        /// 
        /// <summary> 
        public int? create_user { get; set; }

        /// <summary>
        /// 
        /// <summary> 
        public DateTime? modify_dt { get; set; }

        /// <summary>
        /// 
        /// <summary> 
        public int? modify_user { get; set; }

        /// <summary>
        /// 
        /// <summary> 
        public float? business_code1 { get; set; }

        /// <summary>
        /// 
        /// <summary> 
        public float? business_code2 { get; set; }

        /// <summary>
        /// 
        /// <summary> 
        public int? job_code1 { get; set; }

        /// <summary>
        /// 
        /// <summary> 
        public int? job_code2 { get; set; }

        /// <summary>
        /// 
        /// <summary> 
        public string currency_cd { get; set; }

        /// <summary>
        /// 
        /// <summary> 
        public DateTime? share_dt { get; set; }

        /// <summary>
        /// 
        /// <summary> 
        public DateTime? cowork_dt { get; set; }

        /// <summary>
        /// 
        /// <summary> 
        public string position_str { get; set; }

    }

    public partial class project_log
    {
        [NotMapped]
        public string event_name { get; set; }
        [NotMapped]
        public string company_name { get; set; }
        [NotMapped]
        public string target_school_campus { get; set; }
        [NotMapped]
        public string target_school_name { get; set; }
        [NotMapped]
        public string target_major_name { get; set; }
        [NotMapped]
        public string target_company_name { get; set; }
        [NotMapped]
        public string tax_division { get; set; }
        [NotMapped]
        public string tax_name { get; set; }
        [NotMapped]
        public string tax_email { get; set; }
        [NotMapped]
        public string tax_phone { get; set; }
        [NotMapped]
        public string tax_cell_phone { get; set; }
        [NotMapped]
        public string tax_deposit_manager { get; set; }
        [NotMapped]
        public string tax_deposit_email { get; set; }
        [NotMapped]
        public string contact_name { get; set; }
        [NotMapped]
        public string contact_gender { get; set; }
        [NotMapped]
        public string contact_email { get; set; }
        [NotMapped]
        public string contact_phone { get; set; }
        [NotMapped]
        public string contact_cell_phone { get; set; }
        [NotMapped]
        public string contact_division { get; set; }
        [NotMapped]
        public string contact_position { get; set; }
        [NotMapped]
        public string business_name2 { get; set; }
        [NotMapped]
        public string job_name2 { get; set; }

    }
}
