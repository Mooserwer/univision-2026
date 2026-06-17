using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Univision.Core.Models;
using Univision.Core.Models.DTO;

namespace Univision.Main.Models.Project
{
    public class InOrderMemoListModel : EntityListViewModel
    {
        public List<inorder_memo> list { get; set; }
    }
}