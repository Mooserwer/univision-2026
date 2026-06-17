using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Univision.Core.Models.DTO
{
    [Table("schedule")]
    public partial class schedule
    {
        /// <summary>
        /// schedule seq
        /// <summary>
        [Key]
        public int s_seq { get; set; }

        /// <summary>
        /// 스케줄 타입 (0: 개인, 1: 회사, 2: 부서, 3: 특정인)
        /// <summary>
        public int type { get; set; }

        /// <summary>
        /// 부 타입(1:일반, 2: 고객사, 3:후보자(프로젝트)
        /// </summary>
        public int sub_type { get; set; }

        /// <summary>
        /// 타이틀
        /// <summary>
        public string title { get; set; }

        /// <summary>
        /// 시작일자
        /// <summary>
        public DateTime start_date { get; set; }

        /// <summary>
        /// 종료일자
        /// <summary>
        public DateTime end_date { get; set; }

        /// <summary>
        /// 내용
        /// <summary>
        public string contents { get; set; }

        /// <summary>
        /// 부서seq (부서 캘린더의 경우)
        /// <summary>
        public int? ud_seq { get; set; }

        /// <summary>
        /// 캘린더 이벤트 색상(헥사코드 컬러)
        /// </summary>
        public string bg_color { get; set; }

        /// <summary>
        /// pjt_recandidate_history Seq
        /// <summary>
        public int? prc_seq { get; set; }

        /// <summary>
        /// project_recandidate pk
        /// <summary>
        public int? pic_seq { get; set; }

        /// <summary>
        /// project pk
        /// <summary>
        public int? p_seq { get; set; }

        /// <summary>
        /// candidate pk
        /// <summary>
        public int? c_seq { get; set; }

        /// <summary>
        /// can_activity seq
        /// <summary>
        public int? cal_seq { get; set; }

        /// <summary>
        /// client seq FK
        /// <summary>
        public int? cl_seq { get; set; }

        /// <summary>
        /// meeting_room_schedule id
        /// </summary>
        public int? meeting_id { get; set; }

        /// <summary>
        /// 생성일
        /// <summary>
        public DateTime create_dt { get; set; }

        /// <summary>
        /// 생성자 uv_seq
        /// <summary>
        public int create_user { get; set; }

        /// <summary>
        /// 수정일
        /// <summary>
        public DateTime? modify_dt { get; set; }

        /// <summary>
        /// 수정자 uv_seq
        /// <summary>
        public int? modify_user { get; set; }
    }

    public partial class schedule
    {
        [NotMapped]
        public int id { get; set; }

        [NotMapped]
        public string start { get; set; }

        [NotMapped]
        public string end { get; set; }

        [NotMapped]
        public string backgroundColor { get; set; }

        [NotMapped]
        public string borderColor { get; set; }

        [NotMapped]
        public string typeDesc { get; set; }

        [NotMapped]
        public string attend_user { get; set; }

        [NotMapped]
        public int attend_cnt { get; set; }

        [NotMapped]
        public string sub_title { get; set; }
    }
}
