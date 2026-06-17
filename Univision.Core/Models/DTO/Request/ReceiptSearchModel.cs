using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Univision.Core.Models.DTO.Request
{
    public class ReceiptSearchModel
  {
        public int?   searchYear { get; set; }
        public int?   type { get; set; }
        public string searchOption { get; set; }
        public string searchTxt { get; set; }
        public int uv_seq { get; set; }
    }
}
