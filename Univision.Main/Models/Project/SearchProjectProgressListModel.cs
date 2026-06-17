using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Univision.Core.Models;
using Univision.Core.Models.DTO;
using Univision.Core.Models.DTO.Response.Project;

namespace Univision.Main.Models.Project
{
    public class SearchProjectProgressListModel : EntityListViewModel
    {
        public int p_seq { get; set; }
        public int state { get; set; }
        public string type { get; set; }

        public string orderOption { get; set; } = "DESC";
        public string orderTxt { get; set; } = "A.state";


        public PjtProgressCntModel cntData { get; set; }

        public List<pjt_recandidate_history> list { get; set; }
    }

}