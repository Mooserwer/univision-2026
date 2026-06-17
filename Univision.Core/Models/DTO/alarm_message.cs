using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Univision.Core.Models.DTO
{
    //알람 메세지 테이블
    [Table("alarm_message")]
    public partial class alarm_message
    {
        /// <summary>
        /// pk
        /// <summary> 
        [Key]
        public int am_seq { get; set; }

        /// <summary>
        /// 메세지
        /// <summary> 
        public string message { get; set; }

        /// <summary>
        /// 링크 url
        /// <summary> 
        public string href_url { get; set; }

        /// <summary>
        /// 등록일
        /// <summary> 
        public DateTime? create_dt { get; set; }

    }

    public partial class alarm_message
    {
        [NotMapped]
        public int is_read { get; set; }

        [NotMapped]
        public DateTime? read_date { get; set; }
    }
}


