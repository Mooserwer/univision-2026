using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Univision.Core.Models.DTO.Response.Statistics
{
    public class SalesHistoryViewModel
    {
        public SaleHistorySearchModel search { get; set; }

        public List<uv_division> deptList { get; set; }
        public List<uv_user> userList { get; set; }

        public List<PieChartListModel> positionList { get; set; }
        public List<PieChartListModel> feeRateList { get; set; }
        public List<PieChartListModel> businessList { get; set; }
        public List<PieChartListModel> jobList { get; set; }

    }

    public class SalesHistoryListModel : EntityListViewModel
    {
        public SaleHistorySearchModel search { get; set; }

        public SalesHistorySumAmtModel sumData { get; set; }

        public List<SalesHistoryModel> list { get; set; }
    }

    public class SaleHistorySearchModel
    {
        public int ud_seq { get; set; } = 0;
        public int uv_seq { get; set; } = 0;

        public string startDt { get; set; } = "";
        public string endDt { get; set; } = "";
    }
    
    public class PieChartListModel
    {
        public string title { get; set; }
        public int allCnt { get; set; }
        public double billing_money { get; set; }
    }

    public class PieChartModel
    {
        public string title { get; set; }
        public double billing_money { get; set; }
    }

    public class SalesHistorySumAmtModel
    {
        public Int64 sum_annual_income { get; set; }
        public Int64 sum_billing_money { get; set; }
        public Int64 sum_auth_amt { get; set; }
    }

    public class SalesHistoryModel
    {
        public int p_seq { get; set; }
        public int client_seq { get; set; }
        public string client_name { get; set; }
        public string contact_name { get; set; }
        public string am_name { get; set; }
        public string searcher_name { get; set; }
        public int candidate_seq { get; set; }
        public string candidate_name { get; set; }
        public DateTime schedule_date { get; set; }
        public DateTime billing_dt { get; set; }
        public DateTime? deposit_dt { get; set; }
        public int annual_income { get; set; }
        public double fee_rate { get; set; }
        public double billing_money { get; set; }
        public int auth_rate { get; set; }
        public double auth_amt { get; set; }
    }
}
