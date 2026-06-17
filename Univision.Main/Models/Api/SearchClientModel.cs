using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Univision.Core.Models;
using Univision.Core.Models.DTO;

namespace Univision.Main.Models.Api
{
    public class SearchClientModel : EntityListViewModel
    {
        public string searchTxt { get; set; }
        public List<client> list { get; set; }
    }
}