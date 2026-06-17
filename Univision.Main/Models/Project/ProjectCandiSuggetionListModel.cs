using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Univision.Core.Models;
using Univision.Core.Models.DTO;

namespace Univision.Main.Models.Project
{
    public class ProjectCandiSuggetionListModel : EntityListViewModel
  {
        public int p_seq { get; set; }

        public int no_business { get; set; }
        public int no_job { get; set; }
        public int totalCnt { get; set; }
        public int currentPage { get; set; }
        public List<candidate> list { get; set; }
    }
}