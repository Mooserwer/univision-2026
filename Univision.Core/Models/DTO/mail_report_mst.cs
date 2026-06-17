using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Univision.Core.Models.DTO
{
    [Table("mail_report_mst")]
    //주간 이메일 리포팅 마스터
    public partial class mail_report_mst
    {
        /// <summary>
        /// 
        /// <summary>
        [Key]
        public int mr_idx { get; set; }

        /// <summary>
        /// 사용자
        /// <summary>
        public int? uv_seq { get; set; }

        /// <summary>
        /// 기준일
        /// <summary>
        public DateTime? base_dt { get; set; }

        /// <summary>
        /// 이름
        /// <summary>
        public string uv_name { get; set; }



        /// <summary>
        /// 부서명
        /// <summary>
        public string ud_name { get; set; }

        public int? is_leader { get; set; }

        /// <summary>
        /// 지난주 인보이스 생성 갯수
        /// <summary>
        public int? last_inv_cnt { get; set; }

        /// <summary>
        /// 지난주 프로젝트 생성 갯수
        /// <summary>
        public int? last_pjt_cnt { get; set; }

        /// <summary>
        /// 이번주 입사 예정
        /// <summary>
        public int? new_join_cnt { get; set; }

        /// <summary>
        /// 
        /// <summary>
        public int? new_grt_cnt { get; set; }

        /// <summary>
        /// 이번주 면접 예정
        /// <summary>
        public int? new_intv_cnt { get; set; }

        /// <summary>
        /// 전체 프로젝트(액티브) 건수
        /// <summary>
        public int? all_pjt_cnt { get; set; }

        /// <summary>
        /// 지난주 관심등록 후보 수
        /// <summary>
        public int? all_interest_cnt { get; set; }

        /// <summary>
        /// 지난주 추천 후보수
        /// <summary>
        public int? all_recommend_cnt { get; set; }

        /// <summary>
        /// 지난주 면접 후보 수 
        /// <summary>
        public int? all_interview_cnt { get; set; }

        /// <summary>
        /// 지난주 채용 후보 수
        /// <summary>
        public int? all_hire_cnt { get; set; }

        /// <summary>
        /// 지난주 후보자 등록 수 
        /// <summary>
        public int? week_new_can_cnt { get; set; }

        /// <summary>
        /// 월 누적 후보자 등록 수 
        /// <summary>
        public int? month_new_can_cnt { get; set; }

        /// <summary>
        /// 지난주 후보자 업데이트 수 
        /// <summary>
        public int? week_update_can_cnt { get; set; }

        /// <summary>
        /// 월 누적 후보자 업데이트 수 
        /// <summary>
        public int? month_update_can_cnt { get; set; }

        /// <summary>
        /// 지난주 메모장 등록 수 
        /// <summary>
        public int? week_memo_cnt { get; set; }

        /// <summary>
        /// 월 누적 메모장 등록 수
        /// <summary>
        public int? month_memo_cnt { get; set; }

        /// <summary>
        /// 생성일
        /// <summary>
        public DateTime? create_dt { get; set; }

        /// <summary>
        /// 메일 제목
        /// <summary>
        public string mail_title { get; set; }

        /// <summary>
        /// 메일 내용
        /// <summary>
        public string mail_contents { get; set; }

    }

    public partial class mail_report_mst
    {
        [NotMapped]
        public string to_addr { get; set; }

        [NotMapped]
        public string cc_addr { get; set; }

        [NotMapped]
        public string bcc_addr { get; set; }
    }
}


