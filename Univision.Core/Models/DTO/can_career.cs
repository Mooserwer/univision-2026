using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Univision.Core.Models.DTO
{
    //후보자 경력 정보
    [Table("can_career")]
    public partial class can_career
    {
        /// <summary>
        /// pk
        /// <summary>
        [Key]
        public int cc_seq { get; set; }

        /// <summary>
        /// 후보자 seq
        /// <summary>
        public int c_seq { get; set; }

        /// <summary>
        /// 순서(낮은 수가 최신)
        /// <summary>
        public int order_no { get; set; }

        /// <summary>
        /// gov_api_company pk
        /// <summary>
        public int? g_seq { get; set; }

        /// <summary>
        /// 회사명
        /// <summary>
        public string company_name { get; set; }

        /// <summary>
        /// 부서명
        /// <summary>
        public string division_name { get; set; }

        /// <summary>
        /// 입사일
        /// <summary>
        public string join_dt { get; set; }

        /// <summary>
        /// 퇴사일
        /// <summary>
        public string leave_dt { get; set; }

        /// <summary>
        /// 연봉
        /// <summary>
        public int? annual_income { get; set; }

        /// <summary>
        /// 재직여부
        /// <summary>
        public int? is_work { get; set; }

        /// <summary>
        /// code_rank - 직급코드
        /// <summary>
        public int? r_code { get; set; }

        /// <summary>
        /// code_position - 직책코드
        /// <summary>
        public int? p_code { get; set; }

        public string position_str { get; set; }


    }

    public partial class can_career
    {
        [NotMapped]
        public string r_name { get; set; }

        [NotMapped]
        public string p_name { get; set; }

        [NotMapped]
        public List<can_career_job> jobList { get; set; }
    }
}


