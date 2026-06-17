using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Univision.Core.Models.DTO.Response.Statistics
{

    public class BusiJobStatisticsListModel
    {
        public BusiJobSearchModel search { get; set; }
        public List<BusiJobStatisticsModel> list { get; set; }

        public int SumAll { get; set; }
        public int SumSuccess { get; set; }
        public int SumFail { get; set; }
        public double SumMoney { get; set; }

    }

    public class BusiJobStatisticsModel
    {
        public string type { get; set; }
        public string code_name1 { get; set; }
        public string code_name2 { get; set; }
        public int allCnt { get; set; }
        public int successCnt { get; set; }
        public int failCnt { get; set; }
        public double billing_money { get; set; }
    }

    public class BusiJobSearchModel
    {
        public string startDt { get; set; } = "";
        public string endDt { get; set; } = "";

        public string searchOption { get; set; } = "";
        public string searchTxt { get; set; } = "";

        public string orderOption { get; set; } = "ASC";
        public string orderTxt { get; set; } = "A.code_name2";
    }
}
