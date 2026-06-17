using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Univision.Core.Models.DTO.Response.Client
{
  public class ClientListViewModel : EntityListViewModel
  {
    public ClientSearchModel search { get; set; }
    public ContactSearchModel contactSearch { get; set; }
    public List<client> list { get; set; }
    public List<client_annual_income_rate> incomeList { get; set; }
    public List<client_position_rate> positionList { get; set; }
    public List<code_position> allPosition { get; set; }
  }

  public class ClientSearchModel
  {
    public string kor_name { get; set; } = String.Empty;
    public string eng_name { get; set; }
    public string ceo { get; set; }
    public string contact_name { get; set; }
    public string contact_email { get; set; }
    public string am_name { get; set; }
    public string bd_user { get; set; }
    public string Searcher { get; set; }
    public string biz_num { get; set; } = "";
    public string email { get; set; } = "";
    public string createStart { get; set; } = "";
    public string createEnd { get; set; } = "";
    public string create_user { get; set; } = "";

    public int is_foreign { get; set; } = 9;
    public int offlimit { get; set; } = 0;
    public int is_contract { get; set; } = 9;
    public int is_portfolio { get; set; } = 9;

    public string myClient { get; set; } = "";

    public int searchNum { get; set; } = 10;

    public string phone { get; set; }
    public int cf_seq { get; set; }

    public string activityStart { get; set; }
    public string activityEnd { get; set; }
    public string projectStart { get; set; }
    public string projectEnd { get; set; }

    public string orderOption { get; set; } = "DESC";
    public string orderTxt { get; set; } = "create_dt";
    public int state { get; set; } = 0;
    public bool excelDown { get; set; } = false;

    public string contact_division { get; set; }
    public string contact_position { get; set; }
    public string contact_phone { get; set; }
    public string contact_cell_phone { get; set; }
    public string dateStart { get; set; }
    public string dateEnd { get; set; }
    public string searchStart { get; set; }
    public string searchEnd { get; set; }

    public int is_inorder { get; set; } = 9;

    public bool is_my_client { get; set; } = false;

    public List<code_business2> business { get; set; } = new List<code_business2>();
  }

}
