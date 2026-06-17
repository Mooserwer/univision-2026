using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Univision.Core.Models.DTO
{
    //클라이언트 AM
    [Table("client_manager")]
    public partial class client_manager
    {
        /// <summary>
        /// pk
        /// <summary>
        [Key]
        public int seq { get; set; }
        public int c_seq { get; set; }

        public int uv_seq { get; set; }

        [NotMapped]
        public string ud_name { get; set; }

        [NotMapped]
        public string am_name { get; set; }
    }
}


