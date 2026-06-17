using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Univision.Core.Models.DTO
{
    //프로젝트 세금 담당자
    [Table("pjt_tax_contract")]
    public partial class pjt_tax_contract
    {
        /// <summary>
        /// pk
        /// <summary>
        [Key]
        public int ptc_seq { get; set; }

       /// <summary>
       /// project pk
       /// <summary>
       public int p_seq { get; set; }

       /// <summary>
       /// 부서
       /// <summary>
       public string division { get; set; }

       /// <summary>
       /// 담당자
       /// <summary>
       public string name { get; set; }

       /// <summary>
       /// 이메일
       /// <summary>
       public string email { get; set; }

       /// <summary>
       /// 전화번호
       /// <summary>
       public string phone { get; set; }

       /// <summary>
       /// 핸드폰번호
       /// <summary>
       public string cell_phone { get; set; }

       /// <summary>
       /// 입금당담자 
       /// <summary>
       public string deposit_manager { get; set; }

       /// <summary>
       /// 입금이메일
       /// <summary>
       public string deposit_email { get; set; }

    }

    public partial class pjt_tax_contract
    {
        [NotMapped]
        public int c_seq { get; set; }
    }
}


