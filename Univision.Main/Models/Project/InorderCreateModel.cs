using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Univision.Core.Models.DTO;

namespace Univision.Main.Models.Project
{
    public class InorderCreateModel : inorder
    {

        public List<inorder_director> drectorList { get; set; } = new List<inorder_director>();
        public List<inorder_memo> memoList { get; set; } = new List<inorder_memo>();
        public List<inorder_file> fileList { get; set; } = new List<inorder_file>();

    }

}