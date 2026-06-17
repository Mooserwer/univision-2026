using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Univision.Core.Models.DTO
{
    [Table("mail_report_last_list")]
    //주간 이메일 리포팅 지난주 요약
    public partial class mail_report_last_list
    {
        /// <summary>
        /// 
        /// <summary>
        [Key]
        public int mrll_idx { get; set; }
        
        /// <summary>
        /// 
        /// <summary>
        public int? mr_idx { get; set; }

        /// <summary>
        /// 구분
        /// <summary>
        public string gubun { get; set; }

        /// <summary>
        /// 구분 갯수
        /// <summary>
        public int? gubun_cnt { get; set; }

        /// <summary>
        /// 
        /// <summary>
        public DateTime? create_dt { get; set; }

    }
}


