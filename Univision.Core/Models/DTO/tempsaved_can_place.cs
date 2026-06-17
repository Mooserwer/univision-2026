using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Univision.Core.Models.DTO
{
    //후보자 희망 근무지
    [Table("tempsaved_can_place")]
    public partial class tempsaved_can_place
    {
        /// <summary>
        /// candidate 테이블 seq
        /// <summary>
        public int c_seq { get; set; }

        /// <summary>
        /// 
        /// <summary>
        [Key]
        public int cp_seq { get; set; }

        /// <summary>
        /// 시,도 정보
        /// <summary>
        public string code1 { get; set; }

        /// <summary>
        /// 
        /// <summary>
        public string area1 { get; set; }

        /// <summary>
        /// 구 정보
        /// <summary>
        public string code2 { get; set; }

        /// <summary>
        /// 
        /// <summary>
        public string area2 { get; set; }
    }
}
