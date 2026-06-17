using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Univision.Core.Models.DTO
{
    //클라이언트 활동로그
    [Table("client_activity_log")]
    public partial class client_activity_log
    {
        /// <summary>
        /// pk
        /// <summary>
        [Key]
        public int cal_seq { get; set; }

        public int c_seq { get; set; }

        /// <summary>
        /// 제목
        /// <summary>
        public string title { get; set; }

       /// <summary>
       /// 내용
       /// <summary>
       public string log_comment { get; set; }

       /// <summary>
       /// 일자
       /// <summary>
       public DateTime log_date { get; set; }

        /// <summary>
        /// 스케줄 등록 여부(0: x, 1 : 등록)
        /// </summary>
        public int? is_schedule { get; set; }

        /// <summary>
        /// 등록일
        /// <summary>
        public DateTime? create_dt { get; set; }

       /// <summary>
       /// 등록자
       /// <summary>
       public int? create_user { get; set; }

       /// <summary>
       /// 수정일
       /// <summary>
       public DateTime? modify_dt { get; set; }

       /// <summary>
       /// 수정자
       /// <summary>
       public int? modify_user { get; set; }
       public int act_type { get; set; }
  }

    public partial class client_activity_log
    {
        /// <summary>
        /// 업체명
        /// <summary>
        [NotMapped]
        public string kor_name { get; set; }
        [NotMapped]
        public string eng_name { get; set; }
        /// <summary>
        /// 주담당자명
        /// <summary>
        [NotMapped]
        public string name { get; set; }

        [NotMapped]
        public string create_name { get; set; }

        [NotMapped]
        public string am_name { get; set; }

        [NotMapped]
        public string timelineDate { get; set; }

        [NotMapped]
        public string create_dt_str { get; set; }

        [NotMapped]
        public int log_cnt { get; set; }

    }
}


