using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace Univision.Core.Models.DTO.Logs
{
    [Table("inorder_log")]
    public partial class inorder_log
    {
        /// <summary>
        /// 
        /// <summary> 
        [Key]
        public int log_seq { get; set; }

        /// <summary>
        /// 
        /// <summary> 
        public int? event_idx { get; set; }

        /// <summary>
        /// 
        /// <summary> 
        public string event_type { get; set; }

        /// <summary>
        /// 
        /// <summary> 
        public int? event_uv_seq { get; set; }

        /// <summary>
        /// 
        /// <summary> 
        public DateTime? event_dt { get; set; }

        /// <summary>
        /// 
        /// <summary> 
        public int i_seq { get; set; }

        /// <summary>
        /// 
        /// <summary> 
        public int? p_seq { get; set; }

        /// <summary>
        /// 
        /// <summary> 
        public int? c_seq { get; set; }

        /// <summary>
        /// 
        /// <summary> 
        public int? cc_seq { get; set; }

        /// <summary>
        /// 
        /// <summary> 
        public int? inorder_type { get; set; }

        /// <summary>
        /// 
        /// <summary> 
        public int? inorder_status { get; set; }

        /// <summary>
        /// 
        /// <summary> 
        public string clt_name { get; set; }

        /// <summary>
        /// 
        /// <summary> 
        public string clt_busi_type { get; set; }

        /// <summary>
        /// 
        /// <summary> 
        public string clt_url { get; set; }

        /// <summary>
        /// 
        /// <summary> 
        public string cc_name { get; set; }

        /// <summary>
        /// 
        /// <summary> 
        public int? cc_gender { get; set; }

        /// <summary>
        /// 
        /// <summary> 
        public string cc_division { get; set; }

        /// <summary>
        /// 
        /// <summary> 
        public string cc_position { get; set; }

        /// <summary>
        /// 
        /// <summary> 
        public string cc_phone { get; set; }

        /// <summary>
        /// 
        /// <summary> 
        public string cc_cell_phone { get; set; }

        /// <summary>
        /// 
        /// <summary> 
        public string cc_email { get; set; }

        /// <summary>
        /// 
        /// <summary> 
        public string cc_reason { get; set; }

        /// <summary>
        /// 
        /// <summary> 
        public string can_dept { get; set; }

        /// <summary>
        /// 
        /// <summary> 
        public string can_pos { get; set; }

        /// <summary>
        /// 
        /// <summary> 
        public string can_location { get; set; }

        /// <summary>
        /// 
        /// <summary> 
        public string can_jobdesc { get; set; }

        /// <summary>
        /// 
        /// <summary> 
        public string can_contents { get; set; }

        /// <summary>
        /// 
        /// <summary> 
        public DateTime? create_dt { get; set; }

        /// <summary>
        /// 
        /// <summary> 
        public int? create_user { get; set; }

        /// <summary>
        /// 
        /// <summary> 
        public DateTime? modify_dt { get; set; }

        /// <summary>
        /// 
        /// <summary> 
        public int? modify_user { get; set; }

        /// <summary>
        /// 
        /// <summary> 
        public DateTime? inorder_dt { get; set; }

        /// <summary>
        /// 
        /// <summary> 
        public int? director_seq { get; set; }

        /// <summary>
        /// 
        /// <summary> 
        public DateTime? director_dt { get; set; }

    }

    public partial class inorder_log
    {
        [NotMapped]
        public string event_name { get; set; }
        [NotMapped]
        public string client_name { get; set; }
        [NotMapped]
        public string contact_name { get; set; }
        [NotMapped]
        public string project_title { get; set; }
        [NotMapped]
        public int? pjt_status { get; set; }
        [NotMapped]
        public DateTime? project_dt { get; set; }
        [NotMapped]
        public string am_names { get; set; }
        [NotMapped]
        public string searcher_names { get; set; }
        [NotMapped]
        public string director_name { get; set; }
        [NotMapped]
        public string director_cp { get; set; }
        [NotMapped]
        public string director_tel { get; set; }
        [NotMapped]
        public string director_email { get; set; }
    }
}
