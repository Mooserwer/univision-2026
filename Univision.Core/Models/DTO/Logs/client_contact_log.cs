using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Univision.Core.Models.DTO.Logs
{
    [Table("client_contact_log")]
    public partial class client_contact_log
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

        public DateTime event_dt { get; set; }
        /// <summary>
        /// 
        /// <summary> 
        public int? event_uv_seq { get; set; }


        /// <summary>
        /// 
        /// <summary> 
        [Key]
        public int cc_seq { get; set; }


        /// <summary>
        /// 
        /// <summary> 
        public int c_seq { get; set; }

      

        /// <summary>
        /// 
        /// <summary> 
        public string name { get; set; }

      
        /// <summary>
        /// 
        /// <summary> 
        public int? gender { get; set; }

       
        /// <summary>
        /// 
        /// <summary> 
        public string email { get; set; }

       

        /// <summary>
        /// 
        /// <summary> 
        public string phone { get; set; }

       

        /// <summary>
        /// 
        /// <summary> 
        public string cell_phone { get; set; }

       

        /// <summary>
        /// 
        /// <summary> 
        public string division { get; set; }

       
        /// <summary>
        /// 
        /// <summary> 
        public string position { get; set; }

        /// <summary>
        /// 
        /// <summary> 
        public string memo { get; set; }

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

    }

    public partial class client_contact_log
    {
        [NotMapped]
        public string event_name { get; set; } = "";

        [NotMapped]
        public string sex { get; set; } = "";

        [NotMapped]
        public string uv_name { get; set; } = "";
    }
}
