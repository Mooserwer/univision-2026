using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Univision.Core.Models;
using Univision.Core.Models.DTO;

namespace Univision.Main.Models.Api
{
  public class RequestPrivacyAgree : EntityListViewModel
  {
    public int is_privacy_agree { get; set; } = 0;
    public string privacy_text { get; set; }
    public int is_provide_agree { get; set; } = 0;
    public string provide_text { get; set; }

    public int client_seq { get; set; }


    public string client_name { get; set; }


    public string send_phone { get; set; }
    public string contents { get; set; }


    public List<receive_info> receive { get; set; } = new List<receive_info>();
  }

}