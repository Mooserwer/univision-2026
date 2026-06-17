using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Univision.Core.Models.DTO
{
    //학교 테이블
    [Table("code_school")]
    public partial class code_school
    {
        /// <summary>
        /// pk
        /// <summary>
        [Key]
        public int sc_seq { get; set; }

        /// <summary>
        /// api pk
        /// <summary>
        public int? api_seq { get; set; }

        /// <summary>
        /// 학교 명
        /// <summary>
        public string school_name { get; set; }

        /// <summary>
        /// 캠퍼스명
        /// <summary>
        public string campus_name { get; set; }

        /// <summary>
        /// 학교구분
        /// <summary>
        public string gubun { get; set; }

        /// <summary>
        /// 학교타입
        /// <summary>
        public string school_type { get; set; }

        /// <summary>
        /// 
        /// <summary>
        public string est_type { get; set; }

        /// <summary>
        /// 
        /// <summary>
        public string region { get; set; }

        /// <summary>
        /// 
        /// <summary>
        public string adres { get; set; }

        /// <summary>
        /// 
        /// <summary>
        public string college_info { get; set; }

        /// <summary>
        /// 
        /// <summary>
        public string link { get; set; }

    }
}


