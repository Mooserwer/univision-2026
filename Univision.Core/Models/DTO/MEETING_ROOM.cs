using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Univision.Core.Models.DTO
{
    //회의실 예약 - 회의실 CODE
    [Table("meeting_room")]
    public partial class meeting_room
    {
        /// <summary>
        /// 회의실 코드
        /// <summary>
        [Key]
        public string id { get; set; }

        /// <summary>
        /// 회의실명
        /// <summary>
        public string title { get; set; }

        /// <summary>
        /// 회의실명 줄임용
        /// <summary>
        public string short_title { get; set; }

        /// <summary>
        /// 
        /// <summary>
        public string floor { get; set; }

        /// <summary>
        /// 회의실 표시색
        /// <summary>
        public string color { get; set; }

        /// <summary>
        /// 티비 유무
        /// <summary>
        public int? tv { get; set; }

        /// <summary>
        /// 인터넷 유무
        /// <summary>
        public int? network { get; set; }

        /// <summary>
        /// 전화유무
        /// <summary>
        public int? phone { get; set; }

        /// <summary>
        /// 와이파이 유무
        /// <summary>
        public int? wifi { get; set; }

        /// <summary>
        /// 폐쇠 여부
        /// <summary>
        public int? is_close { get; set; }

        /// <summary>
        /// 회의실별 공지
        /// <summary>
        public string room_notice { get; set; }

    }

    public partial class meeting_room
    {
        [NotMapped]
        public string status { get; set; }
    }
}


