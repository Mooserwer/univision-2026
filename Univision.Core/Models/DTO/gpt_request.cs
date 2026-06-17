using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Univision.Core.Models.DTO
{
  //프로젝트 인보이스 담당자 금액선정
  [Table("gpt_request")]
  public partial class gpt_request
  {
    /// <summary>
    /// pk
    /// <summary>
    [Key]
    public int gr_seq { get; set; }

    /// <summary>
    /// project_invoice_info pk
    /// <summary>
    public int request_type { get; set; }

    /// <summary>
    /// project_recandidate pk
    /// <summary>
    public DateTime request_dt { get; set; }

    /// <summary>
    /// candidate pk
    /// <summary>
    public int request_user { get; set; }

    /// <summary>
    /// project pk
    /// <summary>
    public string gpt_model { get; set; }

    /// <summary>
    /// 담당자 uv_seq
    /// <summary>
    public int? c_seq { get; set; }

    /// <summary>
    /// 담당자 uv_seq
    /// <summary>
    public int? ud_seq { get; set; }
    /// <summary>
    /// 비율
    /// <summary>
    public string file_dir { get; set; }

    /// <summary>
    /// 금액
    /// <summary>
    public string file_origin_path { get; set; }
    /// <summary>
    /// 원화 환산 액
    /// <summary>
    public string file_path { get; set; }
    /// <summary>
    /// 실제 입금액
    /// <summary>
    public string file_extension { get; set; }

    /// <summary>
    /// 
    /// <summary>
    public string file_cont { get; set; }

    public string gr_status_cd { get; set; }
    public string gr_status_str { get; set; }
    public string result_cd { get; set; }
    public string result { get; set; }
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
  }
}


