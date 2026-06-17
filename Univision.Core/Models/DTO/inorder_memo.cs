using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Univision.Core.Models.DTO
{
    //상위 주소 테이블
    [Table("inorder_memo")]
    //인오더 메모 히스토리 
    public partial class inorder_memo
    {
        /// <summary>
        /// pk
        /// <summary>
        [Key]
        public int im_seq { get; set; }

        /// <summary>
        /// inorder pk
        /// <summary>
        public int i_seq { get; set; }

        /// <summary>
        /// 메모
        /// <summary>
        public string memo { get; set; }

        /// <summary>
        /// 
        /// <summary>
        public DateTime? create_dt { get; set; }

        /// <summary>
        /// 
        /// <summary>
        public int? create_user { get; set; }

        /// <summary>
        /// 
        /// <summary>
        public DateTime? modify_dt { get; set; }

        /// <summary>
        /// 
        /// <summary>
        public int? modify_user { get; set; }

    }

    public partial class inorder_memo
    {
        [NotMapped]
        public string create_name { get; set; }
    }
}


