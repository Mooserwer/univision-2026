using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Univision.Core.Models.DTO.Response.Project
{
    public class ProjectStatusCntModel
    {
        public int allCnt { get; set; }
        public int progressCnt { get; set; }
        public int successCnt { get; set; }
        public int completeCnt { get; set; }
        public int failCnt { get; set; }
        public int waitCnt { get; set; }
    }
}
