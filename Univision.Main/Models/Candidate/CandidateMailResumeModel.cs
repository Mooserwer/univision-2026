using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Univision.Core.Models.DTO;

namespace Univision.Main.Models.Candidate
{
    public class CandidateMailResumeListModel
    {
        public string new_mail_key { get; set; }
        public List<CandidateMailResumeModel> resumes { get; set; } = new List<CandidateMailResumeModel>();
    }


    public class CandidateMailResumeModel
    {
        public int dn_seq { get; set; }
        public string resume_type { get; set; }
    }

}