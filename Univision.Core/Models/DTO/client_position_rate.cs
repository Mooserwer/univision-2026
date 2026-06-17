using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Univision.Core.Models.DTO
{
    //클라이언트 포지션 별 수수료 율
    [Table("client_position_rate")]
    public partial class client_position_rate
    {
        /// <summary>
        /// pk
        /// <summary>
        [Key]
        public int cpr_seq { get; set; }

       /// <summary>
       /// client pk
       /// <summary>
       public int c_seq { get; set; }

       /// <summary>
       /// client_contract pk
       /// <summary>
       public int cc_seq { get; set; }

       /// <summary>
       /// code_position 테이블 참조
       /// <summary>
       public int start_code_seq { get; set; }

       /// <summary>
       /// code_position 테이블 참조
       /// <summary>
       public int end_code_seq { get; set; }

       /// <summary>
       /// 수수료율
       /// <summary>
       public decimal? rate { get; set; }

    }
}


