using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Univision.Core.Models.DTO
{
    [Table("mail_report_send_his")]
    //주간 이메일 리포팅 발송 내역
    public partial class mail_report_send_his
    {
        /// <summary>
        /// 
        /// <summary>
        [Key]
        public int mrsh_idx { get; set; }

        /// <summary>
        /// 
        /// <summary>
        public int? mr_idx { get; set; }

        /// <summary>
        /// 
        /// <summary>
        public string to_addr { get; set; }

        /// <summary>
        /// 
        /// <summary>
        public string cc_addr { get; set; }

        /// <summary>
        /// 
        /// <summary>
        public DateTime? create_dt { get; set; }

        /// <summary>
        /// 
        /// <summary>
        public DateTime? send_dt { get; set; }

    }
}


