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
    [Table("uv_vacation_history_dtl")]
    public partial class uv_vacation_history_dtl
    {
        /// <summary>
        /// 키
        /// <summary>
        [Key]
        public int vd_seq { get; set; }

        /// <summary>
        /// 마스터 키
        /// <summary>
        public int v_seq { get; set; }

        /// <summary>
        /// 반차일 경우 오전/오후 여부
        /// <summary>
        public int? v_type { get; set; }

        /// <summary>
        /// 사용일 단일
        /// <summary>
        public DateTime? v_date { get; set; }

        /// <summary>
        /// 사용기간(0.5~1)
        /// <summary>
        public float? v_period { get; set; }

    }
}


