using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Univision.Core.Models.DTO
{
    [Table("mail_report_pjt_list")]
    //주간 이메일 리포팅 프로젝트 현황
    public partial class mail_report_pjt_list
    {
        /// <summary>
        /// 
        /// <summary>
        [Key]
        public int mrpl_idx { get; set; }

        /// <summary>
        /// 
        /// <summary>
        public int mr_idx { get; set; }

        /// <summary>
        /// 프로젝트 코드
        /// <summary>
        public int? p_seq { get; set; }

        /// <summary>
        /// 고객사명
        /// <summary>
        public string client_name { get; set; }

        /// <summary>
        /// 구인제목
        /// <summary>
        public string pjt_title { get; set; }

        /// <summary>
        /// 
        /// <summary>
        public string user_names { get; set; }

        /// <summary>
        /// 
        /// <summary>
        public DateTime? pjt_update { get; set; }

        public DateTime? invoice_date { get; set; }

        /// <summary>
        /// 관심등록 수
        /// <summary>
        public int? interest_cnt { get; set; }

        /// <summary>
        /// 추천 수
        /// <summary>
        public int? recommend_cnt { get; set; }

        /// <summary>
        /// 면접 수 
        /// <summary>
        public int? interview_cnt { get; set; }

        /// <summary>
        /// 채용 수 
        /// <summary>
        public int? hire_cnt { get; set; }

        /// <summary>
        /// 
        /// <summary>
        public int? hire_can_c_seq { get; set; }

        /// <summary>
        /// 
        /// <summary>
        public string hire_can_name { get; set; }

        /// <summary>
        /// 
        /// <summary>
        public DateTime? hire_can_date { get; set; }

        /// <summary>
        /// 
        /// <summary>
        public int? is_no_invoice { get; set; }

        /// <summary>
        /// 
        /// <summary>
        public string status_name { get; set; }
        public int? pjt_status { get; set; }

        /// <summary>
        /// 
        /// <summary>
        public float? point { get; set; }

        /// <summary>
        /// 
        /// <summary>
        public DateTime? create_dt { get; set; }

    }
}


