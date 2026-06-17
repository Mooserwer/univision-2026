using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Univision.Core.Models.DTO.Logs
{

    [Table("client_log")]
    //
    public partial class client_log
    {
        /// <summary>
        /// 
        /// <summary> 
        [Key]
        public int log_seq { get; set; }

        /// <summary>
        /// 
        /// <summary> 
        public int? event_idx { get; set; }

        /// <summary>
        /// 
        /// <summary> 
        public string event_type { get; set; }

        /// <summary>
        /// 
        /// <summary> 
        public int? event_uv_seq { get; set; }

        /// <summary>
        /// 
        /// <summary> 
        public DateTime? event_dt { get; set; }

        /// <summary>
        /// 
        /// <summary> 
        public int c_seq { get; set; }

        /// <summary>
        /// 
        /// <summary> 
        public string kor_name { get; set; }

        /// <summary>
        /// 
        /// <summary> 
        public string eng_name { get; set; }

        /// <summary>
        /// 
        /// <summary> 
        public string ceo { get; set; }

        /// <summary>
        /// 
        /// <summary> 
        public string addr1 { get; set; }

        /// <summary>
        /// 
        /// <summary> 
        public string addr2 { get; set; }

        /// <summary>
        /// 
        /// <summary> 
        public int? is_foreign_invest { get; set; }

        /// <summary>
        /// 
        /// <summary> 
        public int? is_contract { get; set; }

        /// <summary>
        /// 
        /// <summary> 
        public int? is_foreign { get; set; }

        /// <summary>
        /// 
        /// <summary> 
        public string foreign_code { get; set; }

        /// <summary>
        /// 
        /// <summary> 
        public string country_name { get; set; }

        /// <summary>
        /// 
        /// <summary> 
        public string biz_code { get; set; }

        /// <summary>
        /// 
        /// <summary> 
        public string biz_type { get; set; }

        /// <summary>
        /// 
        /// <summary> 
        public string biz_type_code { get; set; }

        /// <summary>
        /// 
        /// <summary> 
        public string biz_category { get; set; }

        /// <summary>
        /// 
        /// <summary> 
        public string biz_category_code { get; set; }

        /// <summary>
        /// 
        /// <summary> 
        public string biz_industry { get; set; }

        /// <summary>
        /// 
        /// <summary> 
        public float? biz_industry_code1 { get; set; }

        /// <summary>
        /// 
        /// <summary> 
        public float? biz_industry_code2 { get; set; }

        /// <summary>
        /// 
        /// <summary> 
        public string fix_title { get; set; }

        /// <summary>
        /// 
        /// <summary> 
        public string homepage { get; set; }

        /// <summary>
        /// 
        /// <summary> 
        public int? employee_number { get; set; }

        /// <summary>
        /// 
        /// <summary> 
        public int? sales_amount { get; set; }

        /// <summary>
        /// 
        /// <summary> 
        public int? main_contract { get; set; }

        /// <summary>
        /// 
        /// <summary> 
        public int? offlimit { get; set; }

        /// <summary>
        /// 
        /// <summary> 
        public DateTime? create_dt { get; set; }

        /// <summary>
        /// 
        /// <summary> 
        public int? create_user { get; set; }

        /// <summary>
        /// 
        /// <summary> 
        public DateTime? modify_dt { get; set; }

        /// <summary>
        /// 
        /// <summary> 
        public int? modify_user { get; set; }

    }

    public partial class client_log
    {
        /// <summary>
        /// 수정자
        /// </summary>
        [NotMapped]
        public string name { get; set; } = "";
        /// <summary>
        /// 수정날짜
        /// </summary>
        [NotMapped]
        public string event_dt_str { get; set; } = "";
    }
}
