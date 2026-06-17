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
  [Table("mail_resume")]
  //
  public partial class mail_resume
  {
    /// <summary>
    /// 
    /// <summary>
    [Key]
    [MaxLength(255)]
    public string dv_timestamp { get; set; }

    /// <summary>
    /// 
    /// <summary>
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public decimal dn_idx { get; set; }

    /// <summary>
    /// 
    /// <summary>
    public DateTime? dd_rcv { get; set; }

    /// <summary>
    /// 
    /// <summary>
    public string dv_snd_name { get; set; }

    /// <summary>
    /// 
    /// <summary>
    public string dv_snd_addr { get; set; }

    /// <summary>
    /// 
    /// <summary>
    public string dv_rcv_name { get; set; }

    /// <summary>
    /// 
    /// <summary>
    public string dv_rcv_addr { get; set; }

    /// <summary>
    /// 
    /// <summary>
    public string dv_cc_addr { get; set; }

    /// <summary>
    /// 
    /// <summary>
    public string dv_subject { get; set; }

    /// <summary>
    /// 
    /// <summary>
    public string dt_body { get; set; }

    /// <summary>
    /// 
    /// <summary>
    public int? dv_cd_no { get; set; }

    /// <summary>
    /// 
    /// <summary>
    public DateTime? dd_email_chk { get; set; }

    /// <summary>
    /// 
    /// <summary>
    public string dv_rcv_id { get; set; }

    /// <summary>
    /// 
    /// <summary>
    public DateTime? dd_rcv_id_chk { get; set; }

    /// <summary>
    /// 
    /// <summary>
    public DateTime? dd_update { get; set; }

    /// <summary>
    /// 
    /// <summary>
    public string dv_worker_id { get; set; }

    /// <summary>
    /// 
    /// <summary>
    public string dv_mail_file_path { get; set; }

    /// <summary>
    /// 
    /// <summary>
    public string dv_del_yn { get; set; }

    /// <summary>
    /// 
    /// <summary>
    public DateTime? dd_reg_date { get; set; }

    /// <summary>
    /// 
    /// <summary>
    public string dv_reg_id { get; set; }

    /// <summary>
    /// 
    /// <summary>
    public DateTime? dd_mod_date { get; set; }

    /// <summary>
    /// 
    /// <summary>
    public string dv_mod_id { get; set; }

    /// <summary>
    /// 
    /// <summary>
    public string dv_update_state { get; set; }

    /// <summary>
    /// 
    /// <summary>
    public string dv_del_reson { get; set; }

    /// <summary>
    /// 
    /// <summary>
    public string dv_del_reason { get; set; }

    public string no_resume { get; set; }
  }

  public partial class mail_resume
  {
    [NotMapped]
    public int manager_seq { get; set; }

    [NotMapped]
    public string manager_name { get; set; }
  }

}


