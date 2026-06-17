using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Univision.Core.Models.DTO
{
    //간편후보 테이블
    [Table("simple_candidate_history")]
    public partial class simple_candidate_history
    {
        /// <summary>
        /// pk
        /// <summary>
        [Key]
        public int sch_seq { get; set; }
        public int sc_seq { get; set; }
        
        public string change_type { get; set; }

        public string previous_value { get; set; }

        public string new_value { get; set; }
  
    }

}


