using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Univision.Core.Models.DTO
{
    //프로젝트 담당자 테이블(Searcher)
    [Table("pjt_contact")]
    public partial class pjt_contact
    {
        /// <summary>
        /// project pk
        /// <summary>
        [Key]
        [Column(Order =1)]
        public int p_seq { get; set; }

        /// <summary>
        /// pk
        /// <summary>
        [Key]
        [Column(Order = 2)]
        public int pc_seq { get; set; }

        /// <summary>
        /// 
        /// <summary>
        public string name { get; set; }

        /// <summary>
        /// 
        /// <summary>
        public string email { get; set; }

        /// <summary>
        /// 
        /// <summary>
        public string phone { get; set; }

        /// <summary>
        /// 
        /// <summary>
        public string cell_phone { get; set; }

        /// <summary>
        /// 
        /// <summary>
        public string division { get; set; }

        /// <summary>
        /// 
        /// <summary>
        public string position { get; set; }

    }

}


