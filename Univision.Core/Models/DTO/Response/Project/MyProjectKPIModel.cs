using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Univision.Core.Models.DTO.Response.Project
{
    public class MyProjectKPIModel
    {
        public string month_name { get; set; }

        public int pjt_cnt { get; set; }
        public int pjt_ing { get; set; }
        public int pjt_hold { get; set; }
        public int pjt_fail { get; set; }
        public int pjt_comp { get; set; }
        public int pjt_succ { get; set; }

        public int client_cnt { get; set; }
        public int activity_cnt { get; set; }

        public int mem_cnt { get; set; }
        public int inter_cnt { get; set; }
        public int push_cnt { get; set; }
        public int interview_cnt { get; set; }
        public int hire_cnt { get; set; }
    }
}
