using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Univision.Core.Models.DTO.Response.Statistics
{
    public class UnivisionPjtCandidateListModel
    {
        public UnivisionPjtCandidateSearchModel search { get; set; }

        public List<PjtRecandidateStaticstic> list { get; set; }
    }


    public class UnivisionPjtCandidateSearchModel
    {
        public int status { get; set; }
        public int uv_seq { get; set; }

        public string from_month_str { get; set; }
        public string to_month_str { get; set; }

        public string search_txt { get; set; }
    }


  public class PjtRecandidateStaticstic
  {
    public int p_seq { get; set; }
    public int c_seq { get; set; }
    public string title { get; set; }
    public string pjt_client { get; set; }
    public string kor_name { get; set; }
    public int gender { get; set; }
    public DateTime? birth_date { get; set; }
    public DateTime? schedule_date { get; set; }
  }
}
