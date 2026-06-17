using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Univision.Core.Models.DTO;

namespace Univision.Main.Models.Api
{
    public class JobBusinessModel
    {
        public List<code_job_mst> jobList1 { get; set; }
        public List<code_business_mst> businessList1 { get; set; }
    }

    public class BusinessModel
    {
        public float? code1 { get; set; }
        public float? code2 { get; set; }
        //public List<float> code3 { get; set; }

        public List<code_business_mst> list1 { get; set; }
        public List<code_business_dtl> list2 { get; set; }
        //public List<code_business3> list3 { get; set; }
    }

    public class RequestBusinessCode
    {
        public float code1 { get; set; }
        public float code2 { get; set; }
        //public float code3 { get; set; }
    }

    public class JobModel
    {
        public float? code1 { get; set; }
        public float? code2 { get; set; }
        //public List<float> code3 { get; set; }

        public List<code_job_mst> list1 { get; set; }
        public List<code_job_dtl> list2 { get; set; }
        //public List<code_job3> list3 { get; set; }
    }

    public class RequestJobCode
    {
        public float code1 { get; set; }
        public float code2 { get; set; }
        //public float code3 { get; set; }
    }
}