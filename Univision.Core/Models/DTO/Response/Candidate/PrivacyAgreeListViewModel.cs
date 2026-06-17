using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Univision.Core.Models.DTO.Response.Candidate
{
    public class PrivacyAgreeListViewModel : EntityListViewModel
    {
        public PrivacyAgreeSearchModel search { get; set; }
        public List<privacy_agree> list { get; set; }
    }

    public class PrivacyAgreeSearchModel
    {
        public int uv_seq { get; set; }
        public string searchOption { get; set; } = "";
        public string searchTxt { get; set; } = "";
        public int c_seq { get; set; }
        public int client_seq { get; set; }
    public int is_agree_only { get; set; } = 0;
    public string orderOption { get; set; } = "DESC";
        public string orderTxt { get; set; } = "PA.send_dt";
    }
}
