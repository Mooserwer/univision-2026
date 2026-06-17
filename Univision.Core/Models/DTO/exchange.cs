using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Univision.Core.Models.DTO
{
    //인바운드 오더
    [Table("exchange")]
    public partial class exchange
    {
        /// <summary>
        /// 
        /// <summary>
        [Key]
        public int ex_seq { get; set; }

        /// <summary>
        /// 
        /// <summary>
        public string ex_date { get; set; }

        /// <summary>
        /// 
        /// <summary>
        public string ex_code { get; set; }

        /// <summary>
        /// 
        /// <summary>
        public double? per_won { get; set; }

        /// <summary>
        /// 
        /// <summary>
        public double? ex_rate { get; set; }

        /// <summary>
        /// 
        /// <summary>
        public double? per_yesterday { get; set; }

        /// <summary>
        /// 
        /// <summary>
        public double? cross_rate { get; set; }

        /// <summary>
        /// 
        /// <summary>
        public DateTime? read_date { get; set; }
        
    }

}


