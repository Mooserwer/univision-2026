using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Univision.Core.Models.DTO
{
    public partial class pjt_place
    {
        /// <summary>
        /// 프로젝트 seq
        /// <summary>
        public int p_seq { get; set; }

        /// <summary>
        /// 프로젝트 근무지 seq
        /// <summary>
        [Key]
        public int pp_seq { get; set; }

        /// <summary>
        /// 시, 도 정보
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
