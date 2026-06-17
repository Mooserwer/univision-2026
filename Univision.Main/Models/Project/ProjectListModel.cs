using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Univision.Core.Models;
using Univision.Core.Models.DTO;
using Univision.Core.Models.DTO.Response.Project;

namespace Univision.Main.Models.Project
{
    public class ProjectListModel
    {
    }

    public class MyProjectListModel : EntityListViewModel
    {
        public List<MyProjectKPIModel> kpiData { get; set; }
        public ProjectStatusCntModel cntData { get; set; }
        public MyProjectSearchModel search { get; set; }
        public List<project> list { get; set; }
    }
}