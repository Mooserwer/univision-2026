using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Univision.Core.Models.DTO;

namespace Univision.Core.Models.DTO.Response.MyList
{
  public class ReceiptCocardModel
  {
    /// <summary>
    /// 
    /// <summary>
    public int r_seq { get; set; }
    public int ra_seq { get; set; }

    public string r_type_name { get; set; }
    public string r_month { get; set; }

    public string card_no { get; set; }

    public List<receipt_user_dtl> list { get; set; } = new List<receipt_user_dtl>();
  }


}