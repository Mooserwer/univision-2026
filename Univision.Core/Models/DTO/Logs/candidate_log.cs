using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace Univision.Core.Models.DTO.Logs
{
    [Table("candidate_log")]
    public partial class candidate_log
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
        public int c_seq { get; set; }

        /// <summary>
        /// 
        /// <summary> 
        public int? manager_seq { get; set; }

        /// <summary>
        /// 
        /// <summary> 
        public string kor_name { get; set; }

        /// <summary>
        /// 
        /// <summary> 
        public string eng_name { get; set; }

        /// <summary>
        /// 
        /// <summary> 
        public float? is_foreign { get; set; }

        /// <summary>
        /// 
        /// <summary> 
        public DateTime? birth_date { get; set; }

        /// <summary>
        /// 
        /// <summary> 
        public float? gender { get; set; }

        /// <summary>
        /// 
        /// <summary> 
        public float? ex_birthdate { get; set; }

        /// <summary>
        /// 
        /// <summary> 
        public string country_code { get; set; }

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
        public string phone { get; set; }

        /// <summary>
        /// 
        /// <summary> 
        public string cell_phone { get; set; }

        /// <summary>
        /// 
        /// <summary> 
        public string email1 { get; set; }

        /// <summary>
        /// 
        /// <summary> 
        public string email2 { get; set; }

        /// <summary>
        /// 
        /// <summary> 
        public int? hope_salary { get; set; }

        public int? hope_salary2 { get; set; }

        /// <summary>
        /// 
        /// <summary> 
        public int? after_interview { get; set; }

        /// <summary>
        /// 
        /// <summary> 
        public DateTime? create_dt { get; set; }

        /// <summary>
        /// 
        /// <summary> 
        public int? create_seq { get; set; }

        /// <summary>
        /// 
        /// <summary> 
        public DateTime? modify_dt { get; set; }

        /// <summary>
        /// 
        /// <summary> 
        public int? modify_seq { get; set; }

        /// <summary>
        /// 
        /// <summary> 
        public string keyword { get; set; }

        /// <summary>
        /// 
        /// <summary> 
        public int? ex_addr { get; set; }

        /// <summary>
        /// 
        /// <summary> 
        public int? wrong_phone { get; set; }

        /// <summary>
        /// 
        /// <summary> 
        public int? wrong_phone2 { get; set; }

        /// <summary>
        /// 
        /// <summary> 
        public int? is_confidential { get; set; }

        /// <summary>
        /// 
        /// <summary> 
        public int? is_inactive { get; set; }

        /// <summary>
        /// 
        /// <summary> 
        public int? reg_type { get; set; }

        /// <summary>
        /// 
        /// <summary> 
        public string sns_link1 { get; set; }

        /// <summary>
        /// 
        /// <summary> 
        public string sns_link2 { get; set; }

        /// <summary>
        /// 
        /// <summary> 
        public string sns_link3 { get; set; }

        /// <summary>
        /// 
        /// <summary> 
        public int? caution_type { get; set; }

        /// <summary>
        /// 
        /// <summary> 
        public string confi_remark { get; set; }

        /// <summary>
        /// 
        /// <summary> 
        public string inactive_remark { get; set; }

    }

    public partial class candidate_log
    {
        [NotMapped]
        public string event_name { get; set; }
        [NotMapped]
        public string school_name { get; set; }
        [NotMapped]
        public string category1_name { get; set; }
        [NotMapped]
        public string company_name { get; set; }
        [NotMapped]
        public int? division_name { get; set; }
       
        [NotMapped]
        public string r_name { get; set; }
        [NotMapped]
        public int? is_work { get; set; }
        [NotMapped]
        public string jobName { get; set; }
        [NotMapped]
        public string busiName { get; set; }
        [NotMapped]
        public string abilityDesc { get; set; }
        [NotMapped]
        public string manager_name { get; set; }
        [NotMapped]
        public string create_name { get; set; }
        [NotMapped]
        public string modify_name { get; set; }

    }
    /// <summary>
    /// 리스트에서 쓰임.
    /// </summary>
    public partial class candidate_log
    {
        
        /// <summary>
        /// 후보자 최종 학교 전공
        /// </summary>
        [NotMapped]
        public string major_name { get; set; }


        [NotMapped]
        public int graduate_status { get; set; }


        [NotMapped]
        public string all_company { get; set; }
       

        [NotMapped]
        public string memo_str { get; set; }

      
        /// <summary>
        /// 직책 명
        /// </summary>
        [NotMapped]
        public string p_name { get; set; }

      

       

        [NotMapped]
        public int cr_seq { get; set; } = 0;

        /// <summary>
        /// 국문이력서 파일 위치
        /// </summary>
        [NotMapped]
        public string kor_file_dir { get; set; }

        /// <summary>
        /// 국문이력서 파일 확장자
        /// </summary>
        [NotMapped]
        public string kor_file_ext { get; set; }

        /// <summary>
        /// 영문이력서 파일 위치
        /// </summary>
        [NotMapped]
        public string eng_file_dir { get; set; }

        /// <summary>
        /// 영문이력서 파일 확장자
        /// </summary>
        [NotMapped]
        public string eng_file_ext { get; set; }

        [NotMapped]
        public int cf_seq { get; set; }

        [NotMapped]
        public string kor_foreign_name { get; set; }

        [NotMapped]
        public string kor_resume_body { get; set; }

        [NotMapped]
        public string eng_resume_body { get; set; }

       

        [NotMapped]
        public string kor_keyword { get; set; }

        [NotMapped]
        public string eng_keyword { get; set; }
        
        
        [NotMapped]
        public int? tempsaved_seq { get; set; }

        [NotMapped]
        public string country_name { get; set; }

        [NotMapped]
        public int is_external_lock { get; set; }

     
    }

}
