using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Univision.Core.Models.DTO
{
    [Table("schedule_attend")]
    public partial class schedule_attend
    {
        /// <summary>
        /// schedule attend seq
        /// <summary>
        [Key]
        public int sa_seq { get; set; }

        /// <summary>
        /// schedule seq
        /// <summary>
        public int s_seq { get; set; }

        /// <summary>
        /// 공유자 uv_seq
        /// <summary>
        public int uv_seq { get; set; }

        /// <summary>
        /// 생성일
        /// <summary>
        public DateTime create_dt { get; set; }

        /// <summary>
        /// 생성자 uv_seq
        /// <summary>
        public int create_user { get; set; }

        /// <summary>
        /// 수정일
        /// <summary>
        public DateTime? modify_dt { get; set; }

        /// <summary>
        /// 수정자 uv_seq
        /// <summary>
        public int? modify_user { get; set; }
    }
}
