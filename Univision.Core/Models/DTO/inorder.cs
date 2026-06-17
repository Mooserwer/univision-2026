using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Univision.Core.Models.DTO
{
    //인바운드 오더
    [Table("inorder")]
    public partial class inorder
    {
        /// <summary>
        /// 
        /// <summary>
        [Key]
        public int i_seq { get; set; }

        /// <summary>
        /// 연결 프로젝트
        /// <summary>
        public int? p_seq { get; set; }

        /// <summary>
        /// 연결 고객사
        /// <summary>
        public int? c_seq { get; set; }

        /// <summary>
        /// 연결 담당자
        /// <summary>
        public int? cc_seq { get; set; }

        /// <summary>
        /// 요청일자
        /// <summary>
        public DateTime? inorder_dt { get; set; }

        /// <summary>
        /// 인오더 구분
        /// <summary>
        public int? inorder_type { get; set; }

        /// <summary>
        /// 인오더 진행상태
        /// <summary>
        public int? inorder_status { get; set; }

        /// <summary>
        /// 고객사명
        /// <summary>
        public string clt_name { get; set; }

        /// <summary>
        /// 고객사업종
        /// <summary>
        public string clt_busi_type { get; set; }

        /// <summary>
        /// 고객사 홈페이지
        /// <summary>
        public string clt_url { get; set; }

        /// <summary>
        /// 담당자명
        /// <summary>
        public string cc_name { get; set; }
        
        /// <summary>
        /// 담당자 성별
        /// </summary>
        public int cc_gender { get; set; } = 0;

        /// <summary>
        /// 담당자 직급
        /// <summary>
        public string cc_position { get; set; }

        /// <summary>
        /// 담당자 부서
        /// </summary>
        public string cc_division { get; set; }

        /// <summary>
        /// 담당자 전화
        /// <summary>
        public string cc_phone { get; set; }

        /// <summary>
        /// 담당자 휴대전화
        /// <summary>
        public string cc_cell_phone { get; set; }

        /// <summary>
        /// 담당자 이메일
        /// <summary>
        public string cc_email { get; set; }

        /// <summary>
        /// 알게된 경로
        /// <summary>
        public string cc_reason { get; set; }

        /// <summary>
        /// 채용부서
        /// <summary>
        public string can_dept { get; set; }

        /// <summary>
        /// 채용직급
        /// <summary>
        public string can_pos { get; set; }

        /// <summary>
        /// 채용위치
        /// <summary>
        public string can_location { get; set; }

        /// <summary>
        /// 요구사항
        /// <summary>
        public string can_jobdesc { get; set; }

        /// <summary>
        /// 업무요약
        /// <summary>
        public string can_contents { get; set; }

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
        /// 담당 컨설턴트 uv_seq
        /// </summary>
        public int? director_seq { get; set; }

        /// <summary>
        /// 담당컨설턴트 등록일.
        /// </summary>
        public DateTime? director_dt { get; set; }
    }

    public partial class inorder
    {
        [NotMapped]
        public string client_name { get; set; }
        [NotMapped]
        public string contact_name { get; set; }
        [NotMapped]
        public string project_title { get; set; }
        [NotMapped]
        public string director_name { get; set; }
        [NotMapped]
        public string director_cp { get; set; }
        [NotMapped]
        public string director_tel { get; set; }
        [NotMapped]
        public string director_email { get; set; }

        [NotMapped]
        public int pjt_status { get; set; } = 0;

        [NotMapped]
        public DateTime? project_dt { get; set; }

        [NotMapped]
        public string am_names { get; set; }

        [NotMapped]
        public string searcher_names { get; set; }

        [NotMapped]
        public int log_cnt { get; set; }
    }
}


