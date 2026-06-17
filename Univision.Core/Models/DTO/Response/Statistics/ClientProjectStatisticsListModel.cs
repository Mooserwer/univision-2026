using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Univision.Core.Models.DTO.Response.Statistics
{
    public class ClientProjectStatisticsListModel : EntityListViewModel
    {
        public ClientProjectStatisticsSearchModel search { get; set; }
        public List<ClientProjectStatisticsModel> list { get; set; }

        public ClientProjectStatisticsModel sumData { get; set; }
    }

    public class ClientProjectStatisticsModel
    {
        public int c_seq { get; set; }
        public string kor_name { get; set; }
        public int allCnt { get; set; }
        public int progressCnt { get; set; }
        public int waitCnt { get; set; }
        public int failCnt { get; set; }
        public int completeCnt { get; set; }
        public int successCnt { get; set; }
        public double billing_money { get; set; }
    }

    public class ClientProjectStatisticsSearchModel
    {
        public string startDt { get; set; } = "";
        public string endDt { get; set; } = "";

        public string searchOption { get; set; } = "";
        public string searchTxt { get; set; } = "";

        public string orderOption { get; set; } = "DESC";
        public string orderTxt { get; set; } = "billing_money";
    }
}
