using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Univision.Core.Models.DTO
{
    //후보자 종목 코드
    [Table("tempsaved_can_job")]
    public partial class tempsaved_can_job
    {
        /// <summary>
        /// candidate pk
        /// <summary>
        [Key]
        [Column(Order = 1)]
        public int c_seq { get; set; }

        /// <summary>
        /// code_business(산업테이블) code1 
        /// <summary>
        [Key]
        [Column(Order = 2)]
        public double code1 { get; set; }

        /// <summary>
        /// code_business(산업테이블) code2 
        /// <summary>
        [Key]
        [Column(Order = 3)]
        public double code2 { get; set; }

        /// <summary>
        /// code_business(산업테이블) code3 
        /// <summary>
        [Key]
        [Column(Order = 4)]
        public double code3 { get; set; }

        public int order_no { get; set; }

    }

    public partial class tempsaved_can_job
    {
        [NotMapped]
        public string code_name1 { get; set; }
        [NotMapped]
        public string code_name2 { get; set; }
    }

}


