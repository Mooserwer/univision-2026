using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Univision.Core.Models.DTO
{
  //프로젝트 관련 파일
  [Table("pjt_file")]
  public partial class pjt_file
  {
    /// <summary>
    /// pk
    /// <summary>
    [Key]
    public int pf_seq { get; set; }

    /// <summary>
    /// project pk
    /// <summary>
    public int p_seq { get; set; }

    /// <summary>
    /// 파일 디렉토리
    /// <summary>
    public string file_dir { get; set; }

    /// <summary>
    /// 파일 원본 이름
    /// <summary>
    public string file_origin_path { get; set; }

    /// <summary>
    /// 파일이름
    /// <summary>
    public string file_path { get; set; }

    /// <summary>
    /// 파일 확장자
    /// <summary>
    public string file_extension { get; set; }

    /// <summary>
    /// 파일 구분 명
    /// <summary>
    public string file_type_name { get; set; }

    /// <summary>
    /// 등록일
    /// <summary>
    public DateTime? create_dt { get; set; }

    /// <summary>
    /// 등록자
    /// <summary>
    public int? create_user { get; set; }

    public DateTime? remove_dt{ get; set; }

    /// <summary>
    /// 등록자
    /// <summary>
    public int? remove_user { get; set; }
  }

  public partial class pjt_file
  {
    [NotMapped]
    public string create_user_name { get; set; }
  }
}


