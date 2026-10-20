using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Univision.Core.Models.DTO
{
  // 프로필 메모 (헤드헌팅 Pro 내부인재풀)
  [Table("R_PROFILE_MEMO")]
  public partial class profile_memo
  {
    /// <summary>메모 고유 키 (PK)</summary>
    [Key]
    public int memo_id { get; set; }

    /// <summary>대상 프로필 키</summary>
    public int profile_id { get; set; }

    /// <summary>메모 내용</summary>
    public string content { get; set; }

    /// <summary>생성일</summary>
    public DateTime create_dt { get; set; }
  }
}
