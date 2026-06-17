using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Univision.Core.Models.DTO
{
    //코드-화폐단위
    [Table("Code_Money_Unit_Table")]
    public partial class Code_Money_Unit_Table
    {
        /// <summary>
        /// 코드
        /// <summary>
        [Key]
        public string Currency_Name { get; set; }

       /// <summary>
       /// 영문국가명
       /// <summary>
       public string Er_Nation { get; set; }

       /// <summary>
       /// 단위명
       /// <summary>
       public string Kr_Currency { get; set; }

       /// <summary>
       /// 단위기호
       /// <summary>
       public string Money_Mark { get; set; }

    }
}


