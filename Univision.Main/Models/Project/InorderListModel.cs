using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Univision.Core.Models;
using Univision.Core.Models.DTO;
using Univision.Core.Models.DTO.Response.Project;

namespace Univision.Main.Models.Project
{
    public class InorderListModel : EntityListViewModel
    {
        public InorderSearchModel search { get; set; }
        public List<inorder> list { get; set; }
    }
}