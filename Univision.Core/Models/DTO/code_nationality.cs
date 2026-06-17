using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Univision.Core.Models.DTO
{
    //국민연금 기준 법인 사업장 테이블
    [Table("code_nationality")]
    public partial class code_nationality
    {
        /// <summary>
        /// 국적코드
        /// <summary>        
        public string Nationality_Code { get; set; }

        /// <summary>
        /// 국적영문명
        /// <summary>
        public string En_Country_Name { get; set; }

        /// <summary>
        /// 국적한국명
        /// <summary>
        public string Kr_Country_Name { get; set; }

        /// <summary>
        /// 국적전번
        /// <summary>
        public string Country_Phone_Code { get; set; }

    }
}
