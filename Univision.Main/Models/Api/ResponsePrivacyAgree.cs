using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Univision.Core.Models;
using Univision.Core.Models.DTO;

namespace Univision.Main.Models.Api
{
  public class ResponsePrivacyAgree : EntityListViewModel
  {
    public int is_privacy_agree { get; set; } = 0;
    public int is_provide_agree { get; set; } = 0;

    public int client_seq { get; set; }

    public string client_name { get; set; }




    public List<receive_info> receive { get; set; } = new List<receive_info>();
  }

  public class receive_info
  {
    public int c_seq { get; set; }
    public string can_type { get; set; }
    public string can_name { get; set; }
    public string can_addr { get; set; }
    public string can_phone { get; set; }
    public string can_email { get; set; }
  }
}