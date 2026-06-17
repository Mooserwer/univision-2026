using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Univision.Core.Models.DTO
{
    //컨설턴트 관심후보
    [Table("can_interest")]
    public partial class can_interest
    {
        /// <summary>
        /// pk
        /// <summary>
        [Key]
        public int cf_seq { get; set; }

       /// <summary>
       /// candidate pk
       /// <summary>
       public int c_seq { get; set; }

       /// <summary>
       /// uv_user pk
       /// <summary>
       public int uv_seq { get; set; }

       /// <summary>
       /// 등록일
       /// <summary>
       public DateTime? create_dt { get; set; }

        public string memo { get; set; }
        public string company { get; set; }
        public string position { get; set; }
    }

    public partial class can_interest
    {
        [NotMapped]
        public string kor_name { get; set; }

        [NotMapped]
        public DateTime birth_date { get; set; }

        [NotMapped]
        public string phone { get; set; }

        [NotMapped]
        public string email1 { get; set; }

    }
}


