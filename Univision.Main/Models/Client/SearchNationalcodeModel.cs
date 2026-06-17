using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Univision.Core.Models;
using Univision.Core.Models.DTO;


namespace Univision.Main.Models.Client
{
    public class SearchNationalcodeModel : EntityListViewModel
    {
        public string searchTxt { get; set; }
        public List<code_nationality> list { get; set; }
    }
}