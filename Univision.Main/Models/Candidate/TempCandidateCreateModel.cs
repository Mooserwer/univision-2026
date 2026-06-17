using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Univision.Core.Models.DTO;

namespace Univision.Main.Models.Candidate
{
    public class TempCandidateCreateModel
    {
        public int c_seq { get; set; }
        public int? tempsaved_seq { get; set; }
        public int? manager_seq { get; set; }
        public string manager_name { get; set; }
        public string kor_name { get; set; }
        public string eng_name { get; set; }
        public double? is_foreign { get; set; }
        public DateTime? birth_date { get; set; }
        public double? gender { get; set; }
        public double? ex_birthdate { get; set; }
        public string country_code { get; set; }
        public string addr1 { get; set; }
        public int? ex_addr { get; set; }
        public string phone { get; set; }
        public int? wrong_phone { get; set; }
        public string cell_phone { get; set; }
        public int? wrong_phone2 { get; set; }
        public string email1 { get; set; }
        public string email2 { get; set; }
        public int? hope_salary { get; set; }
        public int? hope_salary2 { get; set; }
        public string keyword { get; set; }
        public string sns_link1 { get; set; }
        public string sns_link2 { get; set; }
        public string sns_link3 { get; set; }

        public int? is_confidential { get; set; }
        public int? is_inactive { get; set; }
        public int? reg_type { get; set; }
        public List<tempsaved_can_school> schoolList { get; set; } = new List<tempsaved_can_school>();
        public List<tempsaved_can_career> companyList { get; set; } = new List<tempsaved_can_career>();
        public List<tempsaved_can_place> placeList { get; set; } = new List<tempsaved_can_place>();
        public List<tempsaved_can_job> jobList { get; set; } = new List<tempsaved_can_job>();
        public List<tempsaved_can_business> busiList { get; set; } = new List<tempsaved_can_business>();
        public List<tempsaved_can_foreign_lan> foreignList { get; set; } = new List<tempsaved_can_foreign_lan>();
        public List<tempsaved_can_resume> resumeList { get; set; } = new List<tempsaved_can_resume>();
        /*
        public can_resume korResume { get; set; }
        public can_resume engResume { get; set; }
        public can_resume etcResume { get; set; }

        public ResumeData resumeData { get; set; }
        */
    }

}