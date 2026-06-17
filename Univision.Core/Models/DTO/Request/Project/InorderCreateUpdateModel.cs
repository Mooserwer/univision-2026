using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Univision.Core.Models.DTO.Request.Candidate
{
    public class InorderCreateUpdateModel
    {
        public inorder data { get; set; } = new inorder();
        public List<inorder_director> drList { get; set; } = new List<inorder_director>();
        public List<inorder_file> fileList { get; set; } = new List<inorder_file>();

    }

    
}
