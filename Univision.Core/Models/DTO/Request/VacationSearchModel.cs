using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Univision.Core.Models.DTO.Request
{
    public class VacationSearchModel
    {
        public int?   searchYear { get; set; }
        public int?   vacationType { get; set; }

        public int? my_confirm { get; set; }
        public string searchOption { get; set; }
        public string searchTxt { get; set; }
        public int uv_seq { get; set; }
    }
}
