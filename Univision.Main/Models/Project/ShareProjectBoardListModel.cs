using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Univision.Core.Models;
using Univision.Core.Models.DTO;

namespace Univision.Main.Models.Project
{
    public class ShareProjectBoardListModel : EntityListViewModel
    {
        public int p_seq { get; set; }
        public List<pjt_director> amList { get; set; }
        public List<pjt_share_board> list { get; set; }
        
    }
}