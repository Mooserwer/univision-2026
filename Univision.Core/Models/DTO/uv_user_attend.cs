using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Univision.Core.Models.DTO
{
    public partial class uv_user_attend
    {
        /// <summary>
        /// uv_user seq
        /// </summary>
        [Key]
        [Column(Order = 1)]
        public int uv_seq { get; set; }

        /// <summary>
        /// 출근 시간
        /// </summary>
        [Key]
        [Column(Order = 2)]
        public DateTime attend_date { get; set; }
    }

    public partial class uv_user_attend
    {
        [NotMapped]
        public string title { get; set; }
        [NotMapped]
        public string start { get; set; }
    }
}
