using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Univision.Core.Models;
using Univision.Core.Models.DTO.Request.Project;

namespace Univision.Main.Models.Project
{
    public class ProjectSimpleCandidateListModel : EntityListViewModel
    {
        public int p_seq { get; set; }
        public List<ProjectSimpleCandidateModel> list { get; set; }
    }
}