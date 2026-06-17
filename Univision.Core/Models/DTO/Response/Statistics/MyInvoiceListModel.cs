using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Univision.Core.Models.DTO.Response.Statistics
{
  public class MyInvoiceListModel
  {
    public MyInvoiceSearchModel search { get; set; }

    public List<uv_user> userList { get; set; }

    public List<MyInvoiceStatistic> list { get; set; }
  }


  public class MyInvoiceSearchModel
  {
    public int uv_seq { get; set; }
    public int ud_seq { get; set; }
    public string startDt { get; set; }
    public string endDt { get; set; }
  }

  public class MyInvoiceStatistic
  {
    public int pii_seq { get; set; }
    public int p_seq { get; set; }
    public string invoice_type_name { get; set; }
    public string invoice_status { get; set; }
    public string invoice_status_cd { get; set; }
    public string billing_dt_str { get; set; }
    public string expire_guarantee_str { get; set; }
    public string pjt_type_str { get; set; }
    public string client_name { get; set; }
    public string title { get; set; }
    public string director_name { get; set; }
    public string manager_name { get; set; }
    public string share_name { get; set; }
    public string billing_money { get; set; }
    public string bill_currency_cd { get; set; }
    public string vat_type_str { get; set; }
    public string fee_rate_str { get; set; }
    public string candidate_name { get; set; }
    public string join_dt_str { get; set; }
    public string ann_income { get; set; }
    public string income_currency_cd { get; set; }


    public int client_seq { get; set; }
    public int candidate_seq { get; set; }
    public string request_user_name { get; set; }
    public string position_str { get; set; }
    public int pjt_type { get; set; }
    public string invoice_no { get; set; }

    public int refund_cnt { get; set; }
    public int cancel_cnt { get; set; }
    public int invoice_type { get; set; }

    public int pic_seq { get; set; }
  }
}
