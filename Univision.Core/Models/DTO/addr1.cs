using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Univision.Core.Models.DTO
{
    //상위 주소 테이블
    [Table("addr1")]
    public partial class addr1
    {
       /// <summary>
       /// 코드 pk
       /// <summary>
       [Key]
       public string code1 { get; set; }

       /// <summary>
       /// 시, 도 정보
       /// <summary>
       public string area1 { get; set; }

    }
}


