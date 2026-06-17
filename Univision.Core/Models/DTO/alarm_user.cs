using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Univision.Core.Models.DTO
{
    //알람 수신자 테이블
    [Table("alarm_user")]
    public partial class alarm_user
    {
        /// <summary>
        /// alarm_message pk
        /// <summary>
        [Key]
        [Column(Order = 1)]
        public int am_seq { get; set; }

        /// <summary>
        /// uv_user pk
        /// <summary>
        [Key]
        [Column(Order = 2)]
        public int uv_seq { get; set; }

       /// <summary>
       /// 읽음 여부
       /// <summary>
       public double? is_read { get; set; }

       /// <summary>
       /// 읽은 날짜
       /// <summary>
       public DateTime? read_date { get; set; }

    }
}


