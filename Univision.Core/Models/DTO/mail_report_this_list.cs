using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Univision.Core.Models.DTO
{
    [Table("mail_report_this_list")]
    //주간 이메일 리포팅 예정 요약
    public partial class mail_report_this_list
    {
        /// <summary>
        /// 
        /// <summary>
        [Key]
        public int mrtl_idx { get; set; }

        /// <summary>
        /// 
        /// <summary>
        public int? mr_idx { get; set; }

        /// <summary>
        /// 
        /// <summary>
        public string gubun { get; set; }

        /// <summary>
        /// 
        /// <summary>
        public int? c_seq { get; set; }

        /// <summary>
        /// 
        /// <summary>
        public string can_name { get; set; }

        /// <summary>
        /// 
        /// <summary>
        public string client_name { get; set; }

        /// <summary>
        /// 
        /// <summary>
        public DateTime? schedule_date { get; set; }

        /// <summary>
        /// 
        /// <summary>
        public string memo { get; set; }

        /// <summary>
        /// 
        /// <summary>
        public DateTime? create_dt { get; set; }

    }
}


