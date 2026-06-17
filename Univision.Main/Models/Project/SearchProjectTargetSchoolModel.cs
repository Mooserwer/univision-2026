using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Univision.Core.Models;
using Univision.Core.Models.DTO;

namespace Univision.Main.Models.Project
{
    public class SearchProjectTargetSchoolModel : EntityListViewModel
    {
        public int target_school { get; set; }
        public string school_name { get; set; }
        public string gubun { get; set; }

        public List<code_school> list { get; set; }
    }
}