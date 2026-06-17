using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Univision.Core.Models;
using Univision.Core.Models.DTO;
using Univision.Core.Models.DTO.Request;
using Univision.Core.Models.DTO.Response;

namespace Univision.Main.Models.Receipt
{
  public class ReceiptModel : EntityListViewModel
  {
    public ReceiptSearchModel search { get; set; }

    public List<YearMonthModel> yearList { get; set; }

    public List<receipt> list { get; set; }
  }
}
