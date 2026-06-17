using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Univision.Core.Models.DTO.Response.Project
{
    public class WorkSheetMainModel
    {
        public WorkSheetSearchModel search { get; set; }

        public List<uv_division> deptList { get; set; }

        public List<uv_user> userList { get; set; } = new List<uv_user>();
    }

    public class WorkSheetListModel : EntityListViewModel
    {
        public List<WorkSheetModel> list { get; set; }

        public string worksheet_orderTxt { get; set; } = "P.pjt_status";
        public string worksheet_orderOption { get; set; } = "DESC";
    }

    public class WorkSheetModel
    {
        public int p_seq { get; set; }
        public int client_seq { get; set; }
        public string client_name { get; set; }
        public string title { get; set; }
        public int pjt_status { get; set; }
        public int pjt_type { get; set; }
        public DateTime create_dt { get; set; }
        public DateTime modify_dt { get; set; }

        public string am_names { get; set; }
        public string searcher_names { get; set; }

        public int interestCnt { get; set; }

        public string recommandNames { get; set; }
        public string recommandDates { get; set; }
        public int recommandCnt { get; set; }

        public string interviewNames { get; set; }
        public string interviewDates { get; set; }
        public int interviewCnt { get; set; }
    }

    public class WorkSheetInvoiceListModel : EntityListViewModel
    {
        public string invoice_orderTxt { get; set; } = "C.kor_name";
        public string invoice_orderOption { get; set; } = "ASC";

        public List<WorkSheetInvoiceModel> list { get; set; }
    }

    public class WorkSheetInvoiceModel
    {
        public int p_seq { get; set; }
        public int client_seq { get; set; }
        public string client_name { get; set; }
        public string title { get; set; }
        public string am_name { get; set; }
        public string searcher_name { get; set; }
        public DateTime billing_dt { get; set; }
        public int candidate_seq { get; set; }
        public string candidate_name { get; set; }
        public string position_name { get; set; }
        public DateTime schedule_date { get; set; }
        public double billing_money { get; set; }
    }

    public class WorkSheetSearchModel
    {
        public int ud_seq { get; set; } = 0;
        public int uv_seq { get; set; } = 0;

        public string startDt { get; set; } = "";
        public string endDt { get; set; } = "";

        public string invoice_orderTxt { get; set; } = "C.kor_name";
        public string invoice_orderOption { get; set; } = "ASC";

        public string worksheet_orderTxt { get; set; } = "C.kor_name";
        public string worksheet_orderOption { get; set; } = "ASC";
    }
}
