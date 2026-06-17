using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Univision.Core.Models.DTO.Response.Candidate
{
    public class CandidateDetailViewModel
    {
        public candidate data { get; set; }
        public List<can_resume> resumeList { get; set; }

        public List<can_career> careerList { get; set; }

        public List<can_school> schoolList { get; set; }

        public List<can_business> businessList { get; set; }

        public List<can_job> jobList { get; set; }

        public List<privacy_agree> agreeList { get; set; }
  }

    public class CandidateActivityList : EntityListViewModel
    {
        public int c_seq { get; set; }
        public List<view_can_activity> activityList { get; set; }
    }

    public class CandidateMemoList : EntityListViewModel
    {
        public int c_seq { get; set; }
        public List<can_memo> memoList { get; set; }
    }

    public class CandidateInterviewList : EntityListViewModel
    {
        public int c_seq { get; set; }
        public List<can_interview_sheet> interviewList { get; set; }
    }

    public class CandidateResumeList
    {
        public int c_seq { get; set; }
        public List<can_resume> resumeList { get; set; }
    }
}
