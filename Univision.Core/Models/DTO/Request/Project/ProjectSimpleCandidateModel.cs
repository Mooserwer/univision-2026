using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Univision.Core.Models.DTO.Request.Project
{
    public class ProjectSimpleCandidateModel
    {
        public int p_seq { get; set; }
        public int pti_seq { get; set; }
        public int sc_seq { get; set; }
        public string kor_name { get; set; }
        public int gender { get; set; }
        public string birthdate { get; set; }
        public int ex_birthdate { get; set; }
        public string cell_phone { get; set; }
        public string email { get; set; }
        public string sns_url { get; set; }
        public string school { get; set; }
        public string company { get; set; }
        public string memo { get; set; }
    }
}
