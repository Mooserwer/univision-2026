using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Univision.Core.Models.DTO
{
    //연차 사용 기록 테이블
    [Table("uv_vacation_history")]
    public partial class uv_vacation_history
    {
        /// <summary>
        /// pk
        /// <summary>
        [Key]
        public int v_seq { get; set; }

        /// <summary>
        /// 연차 구분(2:출근미등록, 1 :연차, 3:예비군(미차감), 4:병가(미차감), 5:경조사(미차감), 6:기타(미차감), 7:외근(미차감)
        /// <summary>
        public int v_type { get; set; }


        /// <summary>
        /// 사유
        /// <summary>
        public string comment { get; set; }

        /// <summary>
        /// 시작일
        /// <summary>
        public DateTime? start_date { get; set; }

        /// <summary>
        /// 종료일
        /// <summary>
        public DateTime? end_date { get; set; }

        /// <summary>
        /// 승인 여부
        /// <summary>
        public double? is_confirm { get; set; }

        /// <summary>
        /// 승인 자
        /// <summary>
        public int? confirm_user { get; set; }

        /// <summary>
        /// 승인 일자
        /// <summary>
        public DateTime? confirm_date { get; set; }

        /// <summary>
        /// 요청 일
        /// <summary>
        public DateTime? request_date { get; set; }

        /// <summary>
        /// 요청 자
        /// <summary>
        public int? request_user { get; set; }

        /// <summary>
        /// 연차 설정(연차 : 1, 반차 : 0.5)
        /// <summary>
        public double? vacation_number { get; set; } = 0;

        /// <summary>
        /// 부서장 seq
        /// </summary>
        public int? leader_seq { get; set; }
        /// <summary>
        /// 부서장 승인여부
        /// </summary>
        public int? leader_confirm { get; set; }
        /// <summary>
        /// 승인 일자
        /// <summary>
        public DateTime? leader_date { get; set; }
        /// <summary>
        /// 경영팀장 seq
        /// </summary>
        public int? s_leader_seq { get; set; }
        /// <summary>
        /// 경영팀장 승인여부
        /// </summary>
        public int? s_leader_confirm { get; set; }
        /// <summary>
        /// 경영팀장 승인 일자
        /// <summary>
        public DateTime? s_leader_date { get; set; }

        public double? cur_remain { get; set; } = 0;
    }

    public partial class uv_vacation_history
    {
        [NotMapped]
        public string type_name { get; set; }
        [NotMapped]
        public string request_name { get; set; }

        [NotMapped]
        public string leader_name { get; set; }

        [NotMapped]
        public string confirm_name { get; set; }

        [NotMapped]
        public string s_leader_name { get; set; }

        [NotMapped]
        public string vacation_detail { get; set; }
    }
}


