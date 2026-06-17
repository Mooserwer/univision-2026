using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Univision.Core.Models;
using Univision.Core.Models.DTO;
using Univision.Core.Models.DTO.Response.Client;

namespace Univision.Main.Models.Client
{
    //public class ClientProjectListModel
    //{
    //}

    public class ClientProjectListModel : EntityListViewModel
    {
        
        public ClientPjtStatusCntModel cntData { get; set; }
        public ClientProjectSearchModel search { get; set; }
        public List<project> list { get; set; }
    }
}