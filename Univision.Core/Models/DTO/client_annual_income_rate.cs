using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Univision.Core.Models.DTO
{
    //클라이언트 연봉 수수료 비율
    [Table("client_annual_income_rate")]
    public partial class client_annual_income_rate
    {
        /// <summary>
        /// pk
        /// <summary>
        [Key]
        public int cfi_seq { get; set; }

       /// <summary>
       /// Code_Money_Unit_Table(통화 코드 테이블) pk
       /// <summary>
       public string Currency_Name { get; set; }

       /// <summary>
       /// client pk
       /// <summary>
       public int c_seq { get; set; }

       /// <summary>
       /// client_contract(클라이언트 계약정보)pk
       /// <summary>
       public int cc_seq { get; set; }

       /// <summary>
       /// 시작 금액
       /// <summary>
       public int? start_income { get; set; }

       /// <summary>
       /// 종료 금액
       /// <summary>
       public int? end_income { get; set; }

       /// <summary>
       /// 퍼센트지
       /// <summary>
       public decimal? percentage { get; set; }

    }

    public partial class client_annual_income_rate
    {
        [NotMapped]
        public string start_income_name { get; set; }

        [NotMapped]
        public string end_income_name { get; set; }
    }
}


