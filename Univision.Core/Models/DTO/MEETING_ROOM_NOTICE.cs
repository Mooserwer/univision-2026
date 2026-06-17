using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Univision.Core.Models.DTO
{
    //회의실 예약 - 회의실 전체 안내사항
    [Table("MEETING_ROOM_NOTICE")]
    public partial class MEETING_ROOM_NOTICE
    {
        /// <summary>
        /// 회의실 예약 전체 공지
        /// <summary>
        [Key]
        public string dt_notice { get; set; }

       /// <summary>
       /// 최종 수정일
       /// <summary>
       public DateTime? dd_mod_date { get; set; }

    }
}


