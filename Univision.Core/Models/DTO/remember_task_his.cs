using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Univision.Core.Models.DTO
{
  //리멤버 api 작업 테이블
  [Table("remember_task_his")]
  //
  public partial class remember_task_his
  {
    /// <summary>
    /// IDX
    /// <summary>
    [Key]
    [Column(Order = 1)]
    public Decimal task_id { get; set; }

    /// <summary>
    /// 작업구분 (0:요청, 1:전송, 2: 완료, 9:실패)
    /// <summary>
    [Key]
    [Column(Order = 2)]
    public int  task_type { get; set; }

    /// <summary>
    /// 작업일
    /// <summary>
    [Key]
    [Column(Order = 3)]
    public DateTime task_dt { get; set; }

    /// <summary>
    /// 요청주소
    /// <summary>
    public string task_addr { get; set; }

    /// <summary>
    /// 요청코멘트
    /// <summary>
    public string comment { get; set; }

  }

}


