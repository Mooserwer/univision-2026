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
    [Table("inorder_director")]
    public partial class inorder_director
    {
        /// <summary>
        /// pk
        /// <summary>
        [Key]
        public int i_seq { get; set; }

        /// <summary>
        /// uv_user pk
        /// <summary>
        public int uv_seq { get; set; }

    }

    public partial class inorder_director
    {
        [NotMapped]
        public string user_name { get; set; }

        [NotMapped]
        public int division_name { get; set; }

    }
}


