using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Univision.Core.Models.DTO
{
    //회의실 예약 - 스케쥴
    [Table("MEETING_ROOM_SCHEDULE")]
    //회의실 예약 - 스케쥴
    public partial class meeting_room_schedule
    {
        /// <summary>
        /// 
        /// <summary>
        [Key]
        public int id { get; set; }

        /// <summary>
        /// 신청자 아이디
        /// <summary>
        public int req_user_seq { get; set; }

        /// <summary>
        /// 초대 아이디 묶음
        /// <summary>
        public string attend_user { get; set; }

        /// <summary>
        /// 회의실 코드
        /// <summary>
        public string resourceId { get; set; }

        /// <summary>
        /// 용도
        /// <summary>
        public string usage_cd { get; set; }

        /// <summary>
        /// 예약일자
        /// <summary>
        public string date_str { get; set; }

        /// <summary>
        /// 예약시작일시
        /// <summary>
        public DateTime? dd_start { get; set; }

        /// <summary>
        /// 예약종료일시
        /// <summary>
        public DateTime? dd_end { get; set; }

        /// <summary>
        /// 노트북사용여부
        /// <summary>
        public int? laptop { get; set; }

        /// <summary>
        /// 
        /// <summary>
        public DateTime? create_dt { get; set; }

        /// <summary>
        /// 
        /// <summary>
        public int create_seq { get; set; }

        /// <summary>
        /// 
        /// <summary>
        public DateTime? modify_dt { get; set; }

        /// <summary>
        /// 
        /// <summary>
        public int modify_seq { get; set; }

        /// <summary>
        /// 메모
        /// <summary>
        public string comment { get; set; }

        /// <summary>
        /// 취소여부
        /// <summary>
        public int? is_del { get; set; }

        /// <summary>
        /// 스케줄 등록 여부
        /// </summary>
        public int? is_schedule { get; set; }
    }

    public partial class meeting_room_schedule
    {   
        [NotMapped]
        public string req_user_name { get; set; }

        [NotMapped]
        public string room_notice { get; set; }
        [NotMapped]
        public string title { get; set; }
        [NotMapped]
        public string resourceName { get; set; }
        [NotMapped]
        public string resrouceShortName { get; set; }
        [NotMapped]
        public string start { get; set; }
        [NotMapped]
        public string end { get; set; }
        [NotMapped]
        public string start_time { get; set; }
        [NotMapped]
        public string end_time { get; set; }

        [NotMapped]
        public int is_mail_send { get; set; }
    }
}


