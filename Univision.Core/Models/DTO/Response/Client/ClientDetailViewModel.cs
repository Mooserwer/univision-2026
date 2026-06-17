using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Univision.Core.Models.DTO.Response.Client
{
  public class ClientDetailViewModel
  {
    public client data { get; set; }
    //public List<project> projectList { get; set; }
    public List<pjt_recandidate_history> actionList { get; set; }
    public List<client_contact> contactList { get; set; }
    public List<client_tax_contact> taxList { get; set; }
    public List<client_file> fileList { get; set; }
    public List<client_annual_income_rate> list { get; set; }
  }

  public class ClientProjectList : EntityListViewModel
  {
    public int c_seq { get; set; }
    public List<project> projectList { get; set; }
    //public clientprojectstatus cntData { get; set; }
    public ClientProjectSearchModel search { get; set; }

  }

  public class ClientActivityList : EntityListViewModel
  {
    public int c_seq { get; set; }
    public int type { get; set; } = 0;
    public List<client_activity_log> activityLIst { get; set; }
  }

  public class ClientContactList : EntityListViewModel
  {
    public int c_seq { get; set; }
    public int is_external_lock { get; set; } = 0;
    public List<client_contact> contactList { get; set; }
  }

  public class ClientTaxList : EntityListViewModel
  {
    public int c_seq { get; set; }
    public int is_external_lock { get; set; } = 0;
    public List<client_tax_contact> taxList { get; set; }
  }

  public class ClientFileList : EntityListViewModel
  {
    public int c_seq { get; set; }
    public int cf_seq { get; set; }
    public List<client_file> fileList { get; set; }
  }

  public class ClientOfferletterList : EntityListViewModel 
  {
    public int c_seq { get; set; }
    public List<OfferLetterFileModel> offerletterList { get; set; }
  }

  public class ContractFileList
  {
    public int c_seq { get; set; }
    public List<client_contract_file> contractFileList { get; set; }
  }
}
