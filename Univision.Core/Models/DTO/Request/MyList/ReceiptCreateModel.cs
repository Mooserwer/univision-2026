using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Univision.Core.Models.DTO;

namespace Univision.Core.Models.DTO.Request.MyList
{
    public class ReceiptCreateModel
    {
      public int ra_seq { get; set; }

    /// <summary>
    /// 
    /// <summary>
      public int r_seq { get; set; }

    /// <summary>
    /// 
    /// <summary>
    public int? money01 { get; set; }

    /// <summary>
    /// 
    /// <summary>
    public int? money02 { get; set; }

    public HttpPostedFileBase files { get; set; }

    public List<ReceiptCardCreateDtlModel> list { get; set; } = new List<ReceiptCardCreateDtlModel>();
  }


  public class ReceiptCardCreateDtlModel
  {
    public int rad_seq { get; set; }

    /// <summary>
    /// 
    /// <summary>
    public string comment1 { get; set; }

    /// <summary>
    /// 
    /// <summary>

  }




}