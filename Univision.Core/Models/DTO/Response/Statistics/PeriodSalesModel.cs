using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Univision.Core.Models.DTO.Response.Statistics
{
    public class PeriodSalesModel
    {
        public int year { get; set; } = 0;
        public int month { get; set; } = 0;
        public int ud_seq { get; set; } = 0;
        public List<uv_division> divisionList { get; set; }
        public List<YearMonthModel> yearList { get; set; }
        public List<YearMonthModel> monthList { get; set; }
        public List<SalesMonthlySum> salesMonthlySumList { get; set; }
        public List<SalesDivision> salesDivisionList { get; set; }
        public List<SalesPersonal> salesDivisionUserList { get; set; }

        public int pjtSuccessRate { get; set; } = 0;
        public List<PieChartListModel> businessList { get; set; }
        public List<PieChartListModel> feeRateList { get; set; }

    }

    public class SalesYearMonth
    {
        public int billing_year { get; set; }
        public int billing_month { get; set; }
    }

    public class SalesMonthlySum
    {
        public string billing_dt { get; set; }
        public int total_cnt { get; set; }
        public double sales_money { get; set; }
        public int deposit_cnt { get; set; }
        public double deposit_amt { get; set; }

        public int total_cnt_acc { get; set; }
        public double sales_money_acc { get; set; }
        public int deposit_cnt_acc { get; set; }
        public double deposit_amt_acc { get; set; }

    }

    public class SalesPersonal
    {
        public string name { get; set; }
        public double sales_money { get; set; }
    }

    public class SalesDivision
    {
        public string ud_name { get; set; }
        public double sales_money { get; set; }
    }

    public class SalesDetailListModel : EntityListViewModel
    {
        public string year { get; set; }
        public int ud_seq { get; set; } = 0;
        public SalesDetailSum sumData { get; set; }
        public List<SalesDetailModel> list { get; set; }
    }

    public class SalesDetailModel
    {
        public int client_seq { get; set; }
        public string client_name { get; set; }
        public string am_name { get; set; }
        public string searcher_name { get; set; }
        public string contact_name { get; set; }
        public int candidate_seq { get; set; }
        public string candidate_name { get; set; }
        public DateTime? join_dt { get; set; }
        public DateTime? billing_dt { get; set; }
        public DateTime? deposit_dt { get; set; }
        public int annual_income { get; set; }
        public double fee_rate { get; set; }
        public double billing_money { get; set; }
        public int sales_rate { get; set; }
        public int sales_money { get; set; }
    }

    public class SalesDetailSum
    {
        public long sum_annual_income { get; set; }
        public long sum_billing_money { get; set; }
        public long sum_sales_money { get; set; }
    }
}
