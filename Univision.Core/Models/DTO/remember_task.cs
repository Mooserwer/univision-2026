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
  [Table("remember_task")]
  //
  public partial class remember_task
  {
    /// <summary>
    /// IDX
    /// <summary>
    [Key]
    public Decimal task_id { get; set; }

    /// <summary>
    /// 발생일시
    /// <summary>
    public DateTime create_dt { get; set; }

    /// <summary>
    /// 후보자 고유코드
    /// <summary>
    public int c_seq { get; set; }

    /// <summary>
    /// 작업구분(I:신규, U:업데이트, D:삭제)
    /// <summary>
    public string iud_type { get; set; }

  }

  public partial class remember_task
  {
    
    /// <summary>
    /// 자료요청시간
    /// <summary>
    [NotMapped]
    public DateTime? request_dt { get; set; }

    /// <summary>
    /// 자료요청횟수
    /// <summary>
    [NotMapped]
    public int request_cnt { get; set; }

    /// <summary>
    /// 자료전송시간
    /// <summary>
    [NotMapped]
    public DateTime? send_dt { get; set; }

    
    /// <summary>
    /// 자료전송횟수
    /// <summary>
    [NotMapped]
    public int send_cnt { get; set; }

    /// <summary>
    /// 수신완료시간
    /// <summary>
    [NotMapped]
    public DateTime? complete_dt { get; set; }

  }

}


