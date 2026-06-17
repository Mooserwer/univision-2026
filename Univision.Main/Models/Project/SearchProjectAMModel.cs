using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Univision.Core.Models;
using Univision.Core.Models.DTO;

namespace Univision.Main.Models.Project
{
    public class SearchProjectAMModel
    {
        public int ud_seq { get; set; } = 0;
        public string searchTxt { get; set; } = "";
        public List<uv_division> deptList { get; set; }
        public List<uv_user> list { get; set; }
    }
}