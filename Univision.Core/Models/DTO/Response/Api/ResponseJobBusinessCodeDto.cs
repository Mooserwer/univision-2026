using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Univision.Core.Models.DTO.Response.Api
{
    public class ResponseJobBusinessCodeDto
    {
        public string type { get; set; }

        public float code1 { get; set; }
        public string code_name1 { get; set; }

        public float code2 { get; set; }
        public string code_name2 { get; set; }

        public float code3 { get; set; }
        public string code_name3 { get; set; }
    }

    public class ResponseBusinessCodeDto
    {
        public float code1 { get; set; }
        public string code_name1 { get; set; }

        public float code2 { get; set; }
        public string code_name2 { get; set; }

        public float code3 { get; set; }
        public string code_name3 { get; set; }
    }

    public class ResponseJobCodeDto
    {
        public float code1 { get; set; }
        public string code_name1 { get; set; }

        public float code2 { get; set; }
        public string code_name2 { get; set; }

        public float code3 { get; set; }
        public string code_name3 { get; set; }
    }
}
