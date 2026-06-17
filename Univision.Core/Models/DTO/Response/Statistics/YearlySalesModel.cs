using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Univision.Core.Models.DTO.Response.Statistics
{
    public class YearlySalesModel
    {
        public int year { get; set; } = 0;
        public int month { get; set; } = 0;
        public int ud_seq { get; set; } = 0;
        public List<uv_division> divisionList { get; set; }
        public List<YearMonthModel> yearList { get; set; }
        public List<YearMonthModel> monthList { get; set; }
        public List<SalesYearlySum> salesYearlySumList { get; set; }
        public List<SalesDivision> salesDivisionList { get; set; }
        public List<SalesPersonal> salesDivisionUserList { get; set; }

        public int pjtSuccessRate { get; set; } = 0;
        public List<PieChartListModel> businessList { get; set; }
        public List<PieChartListModel> feeRateList { get; set; }

    }

    public class SalesYearlySum
    {
        public string billing_dt { get; set; }
        public int total_cnt { get; set; }
        public double sales_money { get; set; }
    }

}
