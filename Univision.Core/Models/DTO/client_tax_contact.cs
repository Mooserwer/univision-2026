using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Univision.Core.Models.DTO
{
    //전자세금 계산서 담당자
    [Table("client_tax_contact")]
    public partial class client_tax_contact
    {
        /// <summary>
        /// pk
        /// <summary>
        [Key]
        public int ctc_seq { get; set; }

       /// <summary>
       /// client pk
       /// <summary>
       public int c_seq { get; set; }

       /// <summary>
       /// 부서
       /// <summary>
       public string division { get; set; }

       /// <summary>
       /// 담당자명
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
       /// 입금담당자
       /// <summary>
       public string deposit_manager { get; set; }

       /// <summary>
       /// 입금이메일
       /// <summary>
       public string deposit_email { get; set; }

       /// <summary>
       /// 
       /// <summary>
       public DateTime? create_dt { get; set; }

       /// <summary>
       /// 
       /// <summary>
       public int? create_seq { get; set; }

       /// <summary>
       /// 
       /// <summary>
       public DateTime? modify_dt { get; set; }

       /// <summary>
       /// 
       /// <summary>
       public int? modify_seq { get; set; }

        [NotMapped]
        public string uv_name { get; set; }

    }

    public partial class client_tax_contact
    {
        [NotMapped]
        public int p_seq { get; set; }

        [NotMapped]
        public int log_cnt { get; set; }
    }

}


