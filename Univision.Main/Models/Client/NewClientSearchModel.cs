using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Univision.Core.Models.DTO;

namespace Univision.Main.Models.Client
{
    public class NewClientCreateModel
    {
        public string wkplNm { get; set; }
        public string bzowrRgstNo { get; set; }
        public string wkplRoadNmDtlAddr { get; set; }
        public string searchTxt { get; set; }
    }
}