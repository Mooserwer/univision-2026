using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using Univision.Core.Lib;
using Univision.Core.Models;
using Univision.Core.Models.DTO;
using Univision.Core.Models.DTO.Excel;
using Univision.Core.Models.DTO.Request;
using Univision.Core.Models.DTO.Request.Client;
using Univision.Core.Models.DTO.Response.Client;
using Univision.Core.Repositories;
using Univision.Main.Infrastructure;
using Univision.Main.Models.Api;
using Univision.Main.Models.Client;
using Univision.Security;
using Univision.Main.Models.Project;
using Xceed.Words.NET;
using Univision.Core.Models.DTO.Request.Candidate;
using Univision.Core.Models.DTO.Logs;
using Xceed.Document.NET;

namespace Univision.Main.Controllers
{
  public class ClientController : BaseController
  {
    static object _fileObj = new object();

    public ActionResult test()
    {
      return View();
    }

    // GET: Client
    public ActionResult ClientList(ClientSearchModel search, int page = 1)
    {
      int totalCount = 0;

      //한 페이지당 몇개의 행을 보여줄 것인지
      int pageSize = AppPaging.PageLength20;

      ClientRepository cr = new ClientRepository();

      var list = cr.clientList(search, (page - 1) * pageSize, pageSize, out totalCount);

      ClientListViewModel model = new ClientListViewModel
      {
        search = search
          ,
        list = list
          ,
        PagingInfo = new PagingInfo()
        {
          CurrentPage = page,
          ItemsPerPage = pageSize,
          TotalItems = totalCount
        }
      };
      return View(model);
    }

    // GET: Client
    public ActionResult OfflimitList(ClientSearchModel search, int page = 1)
    {
      int totalCount = 0;

      //한 페이지당 몇개의 행을 보여줄 것인지
      int pageSize = 1000;//AppPaging.PageLength20;
      search.offlimit = 9;

      ClientRepository cr = new ClientRepository();

      var list = cr.clientList(search, (page - 1) * pageSize, pageSize, out totalCount);



      ClientListViewModel model = new ClientListViewModel
      {
        search = search
          ,
        list = list
          ,
        PagingInfo = new PagingInfo()
        {
          CurrentPage = page,
          ItemsPerPage = pageSize,
          TotalItems = totalCount
        }
      };
      return View(model);
    }

    public async Task<PartialViewResult> PopFile(int c_seq, int cf_seq = 0)
    {
      ClientRepository cr = new ClientRepository();

      var list = await cr.SelectClientFileAsync(cf_seq);

      ClientFileList model = new ClientFileList()
      {
        c_seq = c_seq,
        cf_seq = cf_seq,
        fileList = list
      };

      return PartialView(model);
    }


    public async Task<PartialViewResult> ContractFileList(int c_seq)
    {

      ClientRepository cr = new ClientRepository();

      var list = await cr.SelectContractFileListAsync(c_seq);

      ContractFileList model = new ContractFileList()
      {
        c_seq = c_seq
          ,
        contractFileList = list

      };

      return PartialView(model);
    }

    public async Task<ActionResult> ClientListExcelDown(ClientSearchModel search)
    {
      try
      {
        ClientRepository pr = new ClientRepository();

        ClientSearchModel search_excel= new ClientSearchModel()
        {
          kor_name = search.kor_name,
          eng_name = search.eng_name,
          ceo = search.ceo,
          biz_num = search.biz_num,
          contact_name = search.contact_name,
          contact_email = search.contact_email,
          am_name = search.am_name,
          create_user = search.create_user,
          is_inorder = search.is_inorder,
          is_foreign = search.is_foreign,
          is_portfolio = search.is_portfolio,
          offlimit = search.offlimit,
          is_contract = search.is_contract,
          createStart = search.createStart,
          createEnd = search.createEnd,
          orderOption = search.orderOption,
          orderTxt = search.orderTxt,
          is_my_client = search.is_my_client,
          business = search.business

        };

        var excel = new Excel<ClientLIstExcelModel>();
        var excelData = await pr.SelectClientListWithoutCountAsync(search);

        var data = excel.WriteExcel(excelData, "ClientList");

        return File(data.FileContents, data.ContentType, data.FileDownloadName);


      }
      catch (Exception e)
      {
        throw e;
      }
    }


    public async Task<ActionResult> PopClientDetailSummary(int c_seq = 0)
    {
      try
      {
        ClientLogRepository log = new ClientLogRepository();

        List<client_log> listSummary = await log.ListSummaryClientLog(c_seq);
        ViewBag.c_seq = c_seq;
        return View(listSummary);
      }
      catch (Exception)
      {

        throw;
      }
    }

    public async Task<ActionResult> ClientDetail(int c_seq = 0)
    {

      string uploadTmpFolder = Utils.ReturnCorpValue(c_seq);

      ClientRepository cr = new ClientRepository();

      var data = await cr.SelectClientWithDetailOneAsync(c_seq);
      //var ClientProjectList = await cr.SelectClientProjectOneAsync(c_seq);
      //var ClientActivityList = await cr.SelectClientActivityOneAsync(c_seq);
      //var ClientContactList = await cr.SelectClientContactOneAsync(c_seq);
      //var ClientTaxList = await cr.SelectClientTaxOneAsync(c_seq);
      //var ClientFileList = await cr.SelectClientFileOneAsync(c_seq);

      ClientCreateModel model = new ClientCreateModel()
      {
        c_seq = data.c_seq
          ,
        log_cnt = data.log_cnt
          ,
        kor_name = data.kor_name
          ,
        eng_name = data.eng_name
          ,
        ceo = data.ceo
          ,
        is_foreign_invest = data.is_foreign_invest
          ,
        is_inorder = data.is_inorder
          ,
        is_portfolio = data.is_portfolio
          ,
        offlimit = (data.offlimit.HasValue ? data.offlimit.Value : 0)
          ,
        offlimit_keyword = data.offlimit_keyword
          ,
        addr1 = data.addr1
          ,
        addr2 = data.addr2
          ,
        is_contract = data.is_contract
          ,
        is_foreign = data.is_foreign
          ,
        foreign_code = data.foreign_code
          ,
        country_name = data.country_name
          ,
        biz_code = data.biz_code
          ,
        biz_type = data.biz_type
          ,
        biz_category = data.biz_category
          ,
        biz_industry = data.biz_industry
          ,
        fix_title = data.fix_title
          ,
        homepage = data.homepage
          ,
        employee_number = data.employee_number
          ,
        sales_amount = data.sales_amount
          ,
        cc_seq = data.cc_seq
          ,
        contract_date = data.contract_date
          ,
        fee_type = data.fee_type
          ,
        deposit_limit = data.deposit_limit
          ,
        guarntee_type = data.guarntee_type
          ,
        is_construct_debut = data.is_construct_debut
          ,
        fix_fee_rate = data.fix_fee_rate
          ,
        contract_comment = data.contract_comment
          ,
        draft_contract_path = data.draft_contract_path
          ,
        manual_contract_path = data.manual_contract_path
          ,
        final_contract_path = data.final_contract_path
          ,
        file_directory = data.file_directory
          ,
        incom_detail = data.incom_detail
          ,
        position_detail = data.position_detail
          ,
        currency_name = data.currcurrency_name
          ,
        biz_industry_code1 = data.biz_industry_code1
          ,
        biz_industry_code2 = data.biz_industry_code2
          ,
        business_name1 = data.business_name1
          ,
        business_name2 = data.business_name2
          ,
        feeValue = data.feeValue
          ,
        create_dt = data.create_dt
          ,
        modify_dt = data.modify_dt
          ,
        create_name = data.create_name
          , 
        bd_user_name = data.bd_user_name
        //data = data
        //, ClientProjectList = ClientProjectList
        //, ClientActivityList = ClientActivityList
        //, ClientContactList = ClientContactList
        //, ClientTaxList = ClientTaxList
        //, ClientFileList = ClientFileList
      };

      model.amList = await cr.SelectClientAmListAsync(c_seq);
      model.CfileList = await cr.SelectContractFileAsync(c_seq, "C");
      model.DfileList = await cr.SelectContractFileAsync(c_seq, "D");
      model.FfileList = await cr.SelectContractFileAsync(c_seq, "F");



      ViewBag.uploadFolder = uploadTmpFolder;

      if (AppIdentity.isExternal == 1)
      {
        ClientEntityRepository cer = new ClientEntityRepository();
        element_open_log eol = await cer.SelectClientOpenLogOneAsync(data.c_seq, AppIdentity.user_seq);
        if (eol == null)
        {
          model.is_external_lock = 1;
          if (!String.IsNullOrEmpty(model.biz_code))
            model.biz_code = Regex.Replace(model.biz_code, "[0-9]", "0");
        }
      }

      return View(model);
    }


    public async Task<ActionResult> NewClientDetail(int c_seq = 0)
    {

      string uploadTmpFolder = Utils.ReturnCorpValue(c_seq);

      ClientRepository cr = new ClientRepository();

      var data = await cr.SelectClientWithDetailOneAsync(c_seq);
      //var ClientProjectList = await cr.SelectClientProjectOneAsync(c_seq);
      //var ClientActivityList = await cr.SelectClientActivityOneAsync(c_seq);
      //var ClientContactList = await cr.SelectClientContactOneAsync(c_seq);
      //var ClientTaxList = await cr.SelectClientTaxOneAsync(c_seq);
      //var ClientFileList = await cr.SelectClientFileOneAsync(c_seq);

      ClientCreateModel model = new ClientCreateModel()
      {
        c_seq = data.c_seq
          ,
        log_cnt = data.log_cnt
          ,
        kor_name = data.kor_name
          ,
        eng_name = data.eng_name
          ,
        ceo = data.ceo
          ,
        is_foreign_invest = data.is_foreign_invest
          ,
        is_inorder = data.is_inorder
          ,
        offlimit = (data.offlimit.HasValue ? data.offlimit.Value : 0)
          ,
        offlimit_keyword = data.offlimit_keyword
          ,
        addr1 = data.addr1
          ,
        addr2 = data.addr2
          ,
        is_contract = data.is_contract
          ,
        is_foreign = data.is_foreign
          ,
        foreign_code = data.foreign_code
          ,
        country_name = data.country_name
          ,
        biz_code = data.biz_code
          ,
        biz_type = data.biz_type
          ,
        biz_category = data.biz_category
          ,
        biz_industry = data.biz_industry
          ,
        fix_title = data.fix_title
          ,
        homepage = data.homepage
          ,
        employee_number = data.employee_number
          ,
        sales_amount = data.sales_amount
          ,
        cc_seq = data.cc_seq
          ,
        contract_date = data.contract_date
          ,
        fee_type = data.fee_type
          ,
        deposit_limit = data.deposit_limit
          ,
        guarntee_type = data.guarntee_type
          ,
        is_construct_debut = data.is_construct_debut
          ,
        fix_fee_rate = data.fix_fee_rate
          ,
        contract_comment = data.contract_comment
          ,
        draft_contract_path = data.draft_contract_path
          ,
        manual_contract_path = data.manual_contract_path
          ,
        final_contract_path = data.final_contract_path
          ,
        file_directory = data.file_directory
          ,
        incom_detail = data.incom_detail
          ,
        position_detail = data.position_detail
          ,
        currency_name = data.currcurrency_name
          ,
        biz_industry_code1 = data.biz_industry_code1
          ,
        biz_industry_code2 = data.biz_industry_code2
          ,
        business_name1 = data.business_name1
          ,
        business_name2 = data.business_name2
          ,
        feeValue = data.feeValue
          ,
        create_dt = data.create_dt
          ,
        modify_dt = data.modify_dt
          ,
        create_name = data.create_name
        //data = data
        //, ClientProjectList = ClientProjectList
        //, ClientActivityList = ClientActivityList
        //, ClientContactList = ClientContactList
        //, ClientTaxList = ClientTaxList
        //, ClientFileList = ClientFileList
      };

      model.amList = await cr.SelectClientAmListAsync(c_seq);
      model.CfileList = await cr.SelectContractFileAsync(c_seq, "C");
      model.DfileList = await cr.SelectContractFileAsync(c_seq, "D");
      model.FfileList = await cr.SelectContractFileAsync(c_seq, "F");



      ViewBag.uploadFolder = uploadTmpFolder;

      if (AppIdentity.isExternal == 1)
      {
        ClientEntityRepository cer = new ClientEntityRepository();
        element_open_log eol = await cer.SelectClientOpenLogOneAsync(data.c_seq, AppIdentity.user_seq);
        if (eol == null)
        {
          model.is_external_lock = 1;
          if (!String.IsNullOrEmpty(model.biz_code))
            model.biz_code = Regex.Replace(model.biz_code, "[0-9]", "0");
        }
      }

      return View(model);
    }

    public async Task<ActionResult> ClientContractDetail(int c_seq = 0)
    {
      ClientRepository cr = new ClientRepository();

      var data = await cr.SelectLastClientContractOneAsync(c_seq);
      var files = new List<client_contract_file>();
      var fee_infos = new List<client_annual_income_rate>();

      if (data != null)
      {
        files = await cr.SelectContractFileListAsync(c_seq, data.cc_seq);
        fee_infos = await cr.SelectClientAnnualIncomeRateListAsync(c_seq, data.cc_seq);
      }
      else
      {
        data = new client_contract();
      }



      ClientContractDetailModel model = new ClientContractDetailModel()
      {
        data = data
          ,
        fileList = files
          ,
        feeList = fee_infos
      };

      return View(model);
    }

    public async Task<ActionResult> InvoiceContractDetail(int c_seq = 0)
    {
      ClientRepository cr = new ClientRepository();

      var data = await cr.SelectLastClientContractOneAsync(c_seq);
      var files = new List<client_contract_file>();
      var fee_infos = new List<client_annual_income_rate>();

      if (data != null)
      {
        files = await cr.SelectContractFileListAsync(c_seq, data.cc_seq);
        fee_infos = await cr.SelectClientAnnualIncomeRateListAsync(c_seq, data.cc_seq);
      }
      else
      {
        data = new client_contract();
      }



      ClientContractDetailModel model = new ClientContractDetailModel()
      {
        data = data
          ,
        fileList = files
          ,
        feeList = fee_infos
      };

      return View(model);
    }

    //public async Task<ActionResult> ClientProjectList(ClientProjectSearchModel search, int c_seq, int page = 1)
    //{

    //    int totalCount = 0;
    //    int pageSize = AppPaging.PageLength5;
    //    ClientRepository cr = new ClientRepository();

    //    if (search.excelDown)
    //    {
    //        return RedirectToAction("ClientProjectListExcel", search);
    //    }

    //    var cntData = await cr.SelectClientPjtStatusCountAsync(search, c_seq);
    //    var list = cr.SelectCliProjectLIstAsync(search, c_seq, (page - 1) * pageSize, pageSize, out totalCount);

    //    ClientProjectListModel model = new ClientProjectListModel()
    //    {
    //        search = search,
    //        cntData = cntData,
    //        list = list,
    //        PagingInfo = new PagingInfo()
    //        {
    //            CurrentPage = page
    //            ,
    //            ItemsPerPage = pageSize
    //            ,
    //            TotalItems = totalCount
    //        }
    //    };

    //    return View(model);
    //}

    public PartialViewResult ClientProjectList(int c_seq, int page = 1, int pjt_state = 0, int pjt_page_size = 5)
    {
      int totalCount = 0;

      int pageSize = pjt_page_size;

      ClientRepository cr = new ClientRepository();

      var list = cr.SelectCliProjectLIstAsync(c_seq, pjt_state, (page - 1) * pageSize, pageSize, out totalCount);

      ClientProjectList model = new ClientProjectList()
      {
        c_seq = c_seq
          ,
        projectList = list
          ,
        PagingInfo = new PagingInfo()
        {
          CurrentPage = page,
          ItemsPerPage = pageSize,
          TotalItems = totalCount
        }

      };


      return PartialView(model);
    }

    public async Task<PartialViewResult> ClientActivityList(int c_seq, int page = 1)
    {
      int totalCount = 0;

      int pageSize = AppPaging.PageLength10;

      ClientRepository cr = new ClientRepository();

      var list = cr.SelectCliActivityLIstAsync(c_seq, (page - 1) * pageSize, pageSize, out totalCount);

      ClientActivityList model = new ClientActivityList()
      {
        c_seq = c_seq
          ,
        activityLIst = list
          ,
        PagingInfo = new PagingInfo()
        {
          CurrentPage = page,
          ItemsPerPage = pageSize,
          TotalItems = totalCount
        }

      };
      ViewBag.c_seq = c_seq;
      return PartialView(model);
    }

    public async Task<PartialViewResult> ClientNewActivityList(int c_seq, int type = 0, int page = 1)
    {
      int totalCount = 0;

      int pageSize = AppPaging.PageLength10;

      ClientRepository cr = new ClientRepository();

      var list = cr.SelectNewCliActivityLIstAsync(c_seq, type, (page - 1) * pageSize, pageSize, out totalCount);

      ClientActivityList model = new ClientActivityList()
      {
        c_seq = c_seq
          ,
        type = type
        ,
        activityLIst = list
          ,
        PagingInfo = new PagingInfo()
        {
          CurrentPage = page,
          ItemsPerPage = pageSize,
          TotalItems = totalCount
        }

      };
      ViewBag.c_seq = c_seq;
      return PartialView(model);
    }

    public async Task<ActionResult> ActivityListExcelDown(string korName, string amName, string dateStart, string dateEnd, string searchStart, string searchEnd, string orderOption, string orderTxt)
    {
      try
      {
        ClientRepository pr = new ClientRepository();

        ClientSearchModel search = new ClientSearchModel()
        {
          kor_name = korName,
          am_name = amName,
          dateStart = dateStart,
          dateEnd = dateEnd,
          searchStart = searchStart,
          searchEnd = searchEnd
        };

        var excel = new Excel<ActivityLIstExcelModel>();
        var excelData = await pr.SelectActivityListWithoutCountAsync(search);
        var data = excel.WriteExcel(excelData, "ActivityList");
        return File(data.FileContents, data.ContentType, data.FileDownloadName);

      }
      catch (Exception e)
      {
        throw e;
      }
    }


    public async Task<PartialViewResult> ClientContactList(int c_seq, int page = 1)
    {
      int totalCount = 0;

      int pageSize = AppPaging.PageLength10;

      ClientRepository cr = new ClientRepository();

      var list = cr.SelectCliContactLIstAsync(c_seq, (page - 1) * pageSize, pageSize, out totalCount);

      int is_external_lock = 0;
      if (AppIdentity.isExternal == 1)
      {
        ClientEntityRepository cer = new ClientEntityRepository();

        element_open_log eol = await cer.SelectClientOpenLogOneAsync(c_seq, AppIdentity.user_seq);
        if (eol == null)
        {
          is_external_lock = 1;
          foreach (var data in list)
          {
            if (!String.IsNullOrEmpty(data.phone))
              data.phone = Regex.Replace(data.phone, "[0-9]", "0");

            if (!String.IsNullOrEmpty(data.cell_phone))
              data.cell_phone = Regex.Replace(data.cell_phone, "[0-9]", "0");

            if (!String.IsNullOrEmpty(data.email))
              data.email = Regex.Replace(data.email, "[a-zA-Z0-9]", "*");
          }
        }
      }

      ClientContactList model = new ClientContactList()
      {
        c_seq = c_seq
          ,
        is_external_lock = is_external_lock
          ,
        contactList = list
          ,
        PagingInfo = new PagingInfo()
        {
          CurrentPage = page,
          ItemsPerPage = pageSize,
          TotalItems = totalCount
        }

      };

      return PartialView(model);
    }

    public async Task<PartialViewResult> ClientTaxList(int c_seq, int page = 1)
    {
      int totalCount = 0;

      int pageSize = AppPaging.PageLength10;

      ClientRepository cr = new ClientRepository();

      var list = cr.SelectCliTaxLIstAsync(c_seq, (page - 1) * pageSize, pageSize, out totalCount);

      int is_external_lock = 0;
      if (AppIdentity.isExternal == 1)
      {
        ClientEntityRepository cer = new ClientEntityRepository();

        element_open_log eol = await cer.SelectClientOpenLogOneAsync(c_seq, AppIdentity.user_seq);
        if (eol == null)
        {
          is_external_lock = 1;
          foreach (var data in list)
          {
            if (!String.IsNullOrEmpty(data.phone))
              data.phone = Regex.Replace(data.phone, "[0-9]", "0");

            if (!String.IsNullOrEmpty(data.cell_phone))
              data.cell_phone = Regex.Replace(data.cell_phone, "[0-9]", "0");

            if (!String.IsNullOrEmpty(data.email))
              data.email = Regex.Replace(data.email, "[a-zA-Z0-9]", "*");

            if (!String.IsNullOrEmpty(data.deposit_email))
              data.deposit_email = Regex.Replace(data.deposit_email, "[a-zA-Z0-9]", "*");
          }
        }
      }

      ClientTaxList model = new ClientTaxList()
      {
        c_seq = c_seq
          ,
        is_external_lock = is_external_lock
          ,
        taxList = list
          ,
        PagingInfo = new PagingInfo()
        {
          CurrentPage = page,
          ItemsPerPage = pageSize,
          TotalItems = totalCount
        }

      };

      return PartialView(model);
    }

    public PartialViewResult ClientFileList(int c_seq, int page = 1)
    {
      int totalCount = 0;

      int pageSize = AppPaging.PageLength5;

      ClientRepository cr = new ClientRepository();

      var list = cr.SelectCliFileLIstAsync(c_seq, (page - 1) * pageSize, pageSize, out totalCount);

      ClientFileList model = new ClientFileList()
      {
        c_seq = c_seq
          ,
        fileList = list
          ,
        PagingInfo = new PagingInfo()
        {
          CurrentPage = page,
          ItemsPerPage = pageSize,
          TotalItems = totalCount
        }

      };

      return PartialView(model);
    }

    public PartialViewResult ClientOfferletterList(int c_seq, int page = 1)
    {
      int totalCount = 0;

      int pageSize = AppPaging.PageLength5;

      ClientRepository cr = new ClientRepository();

      var list = cr.SelectCliOfferletterListAsync(c_seq, (page - 1) * pageSize, pageSize, out totalCount);

      ClientOfferletterList model = new ClientOfferletterList()
      {
        c_seq = c_seq,

        offerletterList = list
          ,
        PagingInfo = new PagingInfo()
        {
          CurrentPage = page,
          ItemsPerPage = pageSize,
          TotalItems = totalCount
        }

      };

      return PartialView(model);
    }

    public async Task<ActionResult> ClientCreate(int c_seq = 0)
    {

      if (AppIdentity.isExternal == 1 && c_seq != 0)
      {
        ClientEntityRepository cer = new ClientEntityRepository();

        element_open_log eol = await cer.SelectClientOpenLogOneAsync(c_seq, AppIdentity.user_seq);
        if (eol == null)
        {
          return RedirectToAction("ClientDetail", "Client", new { c_seq = c_seq });
        }
      }

      ClientRepository cr = new ClientRepository();

      var data = await cr.SelectClientOneAsync(c_seq);
      if (data == null)
        data = new client()
        {
          country_name = "한국",
          foreign_code = "KR",
          bd_user_seq = AppIdentity.user_seq,
          bd_user_name = AppIdentity.name
          
        };

      //var contractDraft = await cr.SelectDraftContract(c_seq, "D");
      //var contractFinal = await cr.SelectFinalContract(c_seq, "F");

      ClientCreateModel model = new ClientCreateModel()
      {
        c_seq = data.c_seq
          ,
        kor_name = data.kor_name
          ,
        eng_name = data.eng_name
          ,
        ceo = data.ceo
          ,
        is_foreign_invest = data.is_foreign_invest
          ,
        is_inorder = data.is_inorder
          ,
        is_portfolio = data.is_portfolio
          ,
        offlimit = (data.offlimit.HasValue ? data.offlimit.Value : 0)
          ,
        offlimit_keyword = data.offlimit_keyword
          ,
        addr1 = data.addr1
          ,
        addr2 = data.addr2
          ,
        is_contract = data.is_contract
          ,
        is_foreign = data.is_foreign
          ,
        foreign_code = data.foreign_code
          ,
        country_name = data.country_name
          ,
        biz_code = data.biz_code
          ,
        biz_type = data.biz_type
          ,
        biz_category = data.biz_category
          ,
        biz_industry = data.biz_industry
          ,
        busiCode1 = data.biz_industry_code1
          ,
        busiCode2 = data.biz_industry_code2
          ,
        fix_title = data.fix_title
          ,
        homepage = data.homepage
          ,
        employee_number = data.employee_number
          ,
        sales_amount = data.sales_amount
          ,
        create_user = data.create_user
          ,
        create_name = data.create_name
          ,
        bd_user_name = data.bd_user_name
          ,
        bd_user_seq = data.bd_user_seq
          ,
        name = data.contact_name
          ,
        gender = data.contact_gender
          ,
        email = data.contact_email
          ,
        phone = data.contact_phone
          ,
        cell_phone = data.contact_cell_phone
          ,
        division = data.contact_division
          ,
        position = data.contact_position
          ,
        tax_name = data.tax_name
          ,
        tax_division = data.tax_division
          ,
        tax_email = data.tax_email
          ,
        tax_phone = data.tax_phone
          ,
        tax_cell_phone = data.tax_cell_phone
          ,
        tax_deposit_email = data.tax_deposit_email
          ,
        tax_deposit_manager = data.tax_deposit_manager
          ,
        business_name1 = data.biz_industry_name1
          ,
        business_name2 = data.biz_industry_name2
          ,
        contact_seq = data.contact_seq
          ,
        tax_seq = data.tax_seq
      };
      if (c_seq != 0)
      {
        model.amList = await cr.SelectClientAmListAsync(c_seq);

      }
      else
      {
        model.amList.Add(new client_manager()
        {
          c_seq = c_seq,
          uv_seq = AppIdentity.user_seq,
          am_name = AppIdentity.name
        });
      }
      return View(model);
      //return View();
    }


    [HttpPost]
    public async Task<JsonResult> CreateContractFile(int c_seq, string file_type, HttpPostedFileBase file)
    {
      try
      {
        FileUpload fiUpload = new FileUpload();
        string path = Server.MapPath("~/UploadedFiles");
        var result = fiUpload.UploadContract(c_seq, path, file);

        if (!result.status)
          return Json(new
          {
            ok = false,
            message = result.statusMessage
          });

        ClientEntityRepository cer = new ClientEntityRepository();

        client_contract_file cr = new client_contract_file()
        {
          c_seq = c_seq,
          file_type = file_type,
          file_dir = Path.Combine(Utils.GetRootUrl(Request), result.dbPath),
          file_origin_path = result.filePath,
          file_path = result.name,
          file_extension = result.extension,
          create_dt = Utils.NowKorea(),
          create_user = AppIdentity.user_seq
        };

        await cer.CreateOrUpdateContractFile(cr, CommonCodes.Create);

        return Json(new
        {
          ok = true,
          message = "이력서를 등록 했습니다.",
          result = cr
        });
      }
      catch (Exception e)
      {
        return Json(new
        {
          ok = false,
          message = "알 수 없는 오류가 발생 했습니다."
        });
      }

    }

    [HttpPost]
    public async Task<JsonResult> CreateTempContractFile(string file_type, string uploadFolder, HttpPostedFileBase file)
    {
      try
      {
        FileUpload fiUpload = new FileUpload();
        string path = Server.MapPath("~/UploadedFiles");
        var rst = fiUpload.UploadTemp(path, "Temp", uploadFolder, file);
        //TODO : 검색엔진에서 파일을 읽어서 주요 정보 추출 프로세스 추가 예정

        can_resume cr = new can_resume()
        {
          file_type = file_type,

          file_dir = rst.dbPath
            ,
          file_origin_path = rst.filePath
            ,
          file_path = rst.name
            ,
          file_extension = rst.extension
        };

        if (!rst.status)
          return Json(new
          {
            ok = false,
            message = rst.statusMessage
            //TODO : 추출한 주요 정보 JSON으로 전달 예정
          });

        return Json(new
        {
          ok = true,
          message = "Upload Complete.",
          result = cr
        });

      }
      catch (Exception e)
      {
        return Json(new
        {
          ok = false,
          message = "[Error]알 수 없는 오류가 발생 했습니다."
        });
      }
    }

    [HttpPost]
    public async Task<JsonResult> UpdateContractFile(string file_origin_path, string file_path, string file_type, string file_extension, int c_seq)
    {
      try
      {
        ClientEntityRepository cr = new ClientEntityRepository();

        FileUpload fi = new FileUpload();

        var result = fi.MoveFile(file_origin_path, Path.Combine(Server.MapPath("~/UploadedFiles"), "client/" + c_seq));
        if (!result.status)
          return Json(new
          {
            ok = false,
            message = result.statusMessage
          });

        client_contract_file cf = new client_contract_file()
        {
          c_seq = c_seq,
          file_type = file_type,
          file_dir = "/UploadFiles/client/" + c_seq + "/" + file_path,
          file_origin_path = file_origin_path,
          file_path = file_path,
          file_extension = file_extension,
          create_dt = Utils.NowKorea(),
          create_user = AppIdentity.user_seq
        };

        await cr.CreateOrUpdateContractFile(cf, CommonCodes.Create);


        return Json(new
        {
          ok = true,
          c_seq = c_seq
        });
      }
      catch (Exception e)
      {
        return Json(new
        {
          ok = false,
          message = "오류발생"
        });
      }


    }


    [HttpPost]
    public async Task<JsonResult> DeleteContractFile(int cf_seq)
    {
      try
      {
        //FileUpload fiUpload = new FileUpload();

        //string path = Server.MapPath("~/UploadedFiles");

        ClientEntityRepository cer = new ClientEntityRepository();

        var cr = await cer.SelectContractFileAsync(cf_seq);

        if (cr == null)
        {
          return Json(new
          {
            ok = false,
            message = "삭제하고자 하는 파일을 찾지 못했습니다."
          });
        }

        cr.remove_dt = Utils.NowKorea();
        cr.remove_user = AppIdentity.user_seq;
        await cer.CreateOrUpdateContractFile(cr, CommonCodes.Delete, AppIdentity.user_seq);

        return Json(new
        {
          ok = true,
          message = "삭제했습니다."
        });


        ////실 파일 삭제
        //if (!fiUpload.DeleteFile(file_origin_path))
        //    return Json(new
        //    {
        //        ok = false
        //        ,
        //        message = "경로에서 삭제하려는 이력서를 찾을 수 없습니다."
        //    });

        //return Json(new
        //{
        //    ok = true,
        //    message = "이력서를 삭제 했습니다."
        //});

      }
      catch (Exception e)
      {
        return Json(new
        {
          ok = false,
          message = "알 수 없는 오류가 발생 했습니다."
        });
      }
    }


    [HttpPost]
    public async Task<JsonResult> SearchCompany(string corpName, string bizNum)
    {
      try
      {
        ClientRepository cr = new ClientRepository();

        var data = await cr.SearchClientOneAsync(corpName, bizNum);

        if (data == null)
        {
          return Json(new
          {
            ok = true,
            message = "신규업체입니다."
          });
        }
        else
        {
          return Json(new
          {
            ok = false,
            message = "동일업체 정보가 있습니다."
          });
        }
      }
      catch (Exception e)
      {
        return Json(new
        {
          ok = false,
          message = "알 수 없는 오류가 발생 했습니다._Controller"
        });
      }
    }

    [HttpPost]
    public async Task<JsonResult> ClientCreate(ClientCreateModel data)
    {
      try
      {
        ClientEntityRepository cr = new ClientEntityRepository();

        ClientCreateUpdateModel model = new ClientCreateUpdateModel();

        string state = "";


        if (data.c_seq == 0)
        {
          model.data = new client()
          {
            kor_name = data.kor_name
              ,
            eng_name = data.eng_name
              ,
            ceo = data.ceo
              ,
            addr1 = data.addr1
              ,
            addr2 = data.addr2
              ,
            is_foreign_invest = data.is_foreign_invest
              ,
            is_inorder = data.is_inorder
              ,
            is_contract = data.is_contract
              ,
            is_portfolio = data.is_portfolio
              ,
            is_foreign = data.is_foreign
              ,
            foreign_code = data.foreign_code
              ,
            country_name = data.country_name
              ,
            biz_code = (!String.IsNullOrEmpty(data.biz_code) ? data.biz_code.Replace("-", "") : "")
              ,
            biz_type = data.biz_type
              ,
            biz_type_code = data.biz_type_code
              ,
            biz_category = data.biz_category
              ,
            biz_category_code = data.biz_category_code
              ,
            biz_industry = data.business_name2
              //, biz_industry_code = data.biz_industry_code                        
              ,
            biz_industry_code1 = data.biz_industry_code1
              ,
            biz_industry_code2 = data.biz_industry_code2
              //business_code1 = data.business_code1,
              //business_code2 = data.business_code2,
              ,
            fix_title = data.fix_title
              ,
            homepage = data.homepage
              ,
            employee_number = data.employee_number
              ,
            sales_amount = data.sales_amount
              ,
            main_contract = data.main_contract
              ,
            offlimit = data.offlimit
              ,
            offlimit_keyword = data.offlimit_keyword
              ,
            create_dt = Utils.NowKorea()
              ,
            create_user = AppIdentity.user_seq
              ,
            modify_dt = Utils.NowKorea()
              ,
            modify_user = AppIdentity.user_seq
              ,
            bd_user_seq = (data.is_inorder == 0 ? data.bd_user_seq : 0)
          };

          state = CommonCodes.Create;


        }
        else
        {
          model.data = await cr.SelectClientOneAsync(data.c_seq);
          model.data.kor_name = data.kor_name;
          model.data.eng_name = data.eng_name;
          model.data.ceo = data.ceo;
          model.data.addr1 = data.addr1;
          model.data.addr2 = data.addr2;
          model.data.is_foreign_invest = data.is_foreign_invest;
          model.data.is_inorder = data.is_inorder;
          model.data.is_portfolio = data.is_portfolio;
          model.data.is_contract = data.is_contract;
          model.data.is_foreign = data.is_foreign;
          model.data.foreign_code = data.foreign_code;
          model.data.country_name = data.country_name;
          model.data.biz_code = (!String.IsNullOrEmpty(data.biz_code) ? data.biz_code.Replace("-", "") : "");
          model.data.biz_type = data.biz_type;
          model.data.biz_type_code = data.biz_type_code;
          model.data.biz_category = data.biz_category;
          model.data.biz_category_code = data.biz_category_code;
          model.data.biz_industry = data.biz_industry;
          model.data.biz_industry_code1 = data.biz_industry_code1;
          model.data.biz_industry_code2 = data.biz_industry_code2;
          model.data.fix_title = data.fix_title;
          model.data.homepage = data.homepage;
          model.data.employee_number = data.employee_number;
          model.data.sales_amount = data.sales_amount;
          model.data.main_contract = data.main_contract;
          model.data.offlimit = data.offlimit;
          model.data.offlimit_keyword = data.offlimit_keyword;
          model.data.modify_dt = Utils.NowKorea();
          model.data.modify_user = AppIdentity.user_seq;
          model.data.bd_user_seq = (data.is_inorder == 0 ? data.bd_user_seq : 0);

          model.deleteAmList = await cr.SelectClientAmListAsync(model.data.c_seq);

          state = CommonCodes.Update;

        }

        foreach (var am in data.amList)
        {
          var manager = new client_manager()
          {
            c_seq = model.data.c_seq
              ,
            uv_seq = am.uv_seq
          };

          model.amList.Add(manager);
        }

        model.data2.c_seq = data.c_seq;
        model.data2.cc_seq = data.contact_seq;
        model.data2.name = data.name;
        model.data2.gender = data.gender;
        model.data2.email = data.email;
        model.data2.phone = data.phone;
        model.data2.cell_phone = data.cell_phone;
        model.data2.division = data.division;
        model.data2.position = data.position;
        model.data2.memo = "";
        model.data2.create_dt = Utils.NowKorea();
        model.data2.create_user = AppIdentity.user_seq;
        model.data2.modify_dt = Utils.NowKorea();
        model.data2.modify_user = AppIdentity.user_seq;


        model.data3.c_seq = data.c_seq;
        model.data3.ctc_seq = data.tax_seq;
        model.data3.division = data.tax_division;
        model.data3.name = data.tax_name;
        model.data3.email = data.tax_email;
        model.data3.phone = data.tax_phone;
        model.data3.cell_phone = data.tax_cell_phone;
        model.data3.deposit_manager = data.tax_deposit_manager;
        model.data3.deposit_email = data.tax_deposit_email;
        model.data3.create_dt = Utils.NowKorea();
        model.data3.create_seq = AppIdentity.user_seq;
        model.data3.modify_dt = Utils.NowKorea();
        model.data3.modify_seq = AppIdentity.user_seq;

        await cr.CreateOrUpdateClient(model, state, AppIdentity.user_seq);

        //인오더에서 생성한 고객사의 경우.
        if (data.i_seq != 0)
        {
          ProjectEntityRepository per = new ProjectEntityRepository();
          InorderCreateUpdateModel inorder = new InorderCreateUpdateModel();
          inorder.data = await per.SelectInorderOneAsync(data.i_seq);

          if (inorder.data != null)
          {
            //고객사 seq 넣어줌.
            inorder.data.c_seq = model.data.c_seq;
            await per.CreateOrUpdateInorder(inorder, CommonCodes.Update, AppIdentity.user_seq);

            inorder_memo memo = new inorder_memo()
            {
              i_seq = inorder.data.i_seq,
              memo = "[" + AppIdentity.name + "]님께서 Order 프로젝트를 등록 했습니다.",
              create_dt = Utils.NowKorea(),
              create_user = AppIdentity.user_seq,
              modify_dt = Utils.NowKorea(),
              modify_user = AppIdentity.user_seq
            };

            await per.CreateOrUpdateInorderMemo(memo, CommonCodes.Create);
          }

        }

        return Json(new
        {
          ok = true,
          c_seq = model.data.c_seq
        });

      }
      catch (Exception e)
      {
        return Json(new
        {
          ok = false,
          message = e.Message
        });
      }
    }





    public ActionResult PopClient(ClientSearchModel search, int page = 1)
    {
      int totalCount = 0;

      int pageSize = AppPaging.PageLength10;

      ClientRepository cr = new ClientRepository();

      var list = cr.clientList(search, (page - 1) * pageSize, pageSize, out totalCount);

      ClientListViewModel model = new ClientListViewModel
      {
        search = search
          ,
        list = list
          ,
        PagingInfo = new PagingInfo()
        {
          CurrentPage = page,
          ItemsPerPage = pageSize,
          TotalItems = totalCount
        }
      };
      return View(model);
    }


    public async Task<ActionResult> PopActivity(int cal_seq = 0, int c_seq = 0)
    {
      ClientRepository cr = new ClientRepository();

      var log_data = await cr.SelectActivityWithDetailOneAsync(cal_seq);
      if (log_data == null)
      {
        log_data = new client_activity_log();
        log_data.log_date = DateTime.Now;
      }


      ClientPopupModel model = new ClientPopupModel()
      {
        log_data = log_data
      };


      ViewBag.cSeq = c_seq;
      return View(model);
    }

    public async Task<ActionResult> PopContact(int cc_seq = 0)
    {
      ClientRepository cr = new ClientRepository();

      var data = await cr.SelectContactWithDetailOneAsync(cc_seq);
      if (data == null)
        data = new client_contact();

      ClientPopupModel model = new ClientPopupModel()
      {
        data = data
      };

      return View(model);
    }

    public async Task<ActionResult> PopTax(int ctc_seq = 0)
    {
      ClientRepository cr = new ClientRepository();

      var tax_data = await cr.SelectTexWithDetailOneAsync(ctc_seq);
      if (tax_data == null)
        tax_data = new client_tax_contact();

      ClientPopupModel model = new ClientPopupModel()
      {
        tax_data = tax_data
      };

      return View(model);
    }

    //public async Task<ActionResult> PopFile()
    //{
    //    return View();
    //}


    [HttpPost]
    public async Task<JsonResult> ClientActivityCreate(int cal_seq, int c_seq, string title, string log_comment, DateTime log_date, int is_schedule, int act_type)
    {
      try
      {
        ClientEntityRepository cer = new ClientEntityRepository();

        var activity = await cer.selectActivityOneAsync(cal_seq);

        //신규
        if (activity == null)
        {
          activity = new client_activity_log()
          {
            c_seq = c_seq,
            title = title,
            log_comment = log_comment,
            log_date = log_date,
            is_schedule = is_schedule,
            act_type = act_type,
            create_dt = Utils.NowKorea(),
            create_user = AppIdentity.user_seq,
            modify_dt = Utils.NowKorea(),
            modify_user = AppIdentity.user_seq,
          };

          await cer.CreateOrUpdateActivity(activity, CommonCodes.Create, AppIdentity.user_seq);
        }
        else
        {
          activity.title = title;
          activity.log_comment = log_comment;
          activity.log_date = log_date;
          activity.act_type = act_type;
          activity.is_schedule = is_schedule;
          activity.modify_dt = Utils.NowKorea();
          activity.modify_user = AppIdentity.user_seq;

          await cer.CreateOrUpdateActivity(activity, CommonCodes.Update, AppIdentity.user_seq);
        }

        var clt = await cer.SelectClientOneAsync(c_seq);
        if (clt != null)
        {
          clt.modify_user = AppIdentity.user_seq;
          clt.modify_dt = Utils.NowKorea();

          await cer.UpdateClientOneAsync(clt);
        }

        //스케줄 등록 체크시,
        if (is_schedule == 1)
        {
          ScheduleEntityRepository ser = new ScheduleEntityRepository();
          var sch = await ser.FindScheduleWithClientOneAsync(activity.c_seq, activity.cal_seq);

          ClientRepository cr = new ClientRepository();
          var client = await cr.SelectClientOneAsync(activity.c_seq);
          var amList = await cr.SelectClientAmListAsync(activity.c_seq);

          var attendList = new List<schedule_attend>();

          if (sch == null)
          {
            sch = new schedule()
            {
              type = ScheduleType.share,
              sub_type = ScheduleSubType.client,
              title = "[" + client.kor_name + "]" + activity.title,
              start_date = log_date,
              end_date = log_date.AddMinutes(5),
              contents = log_comment,
              cl_seq = activity.c_seq,
              cal_seq = activity.cal_seq,
              create_dt = Utils.NowKorea(),
              create_user = AppIdentity.user_seq,
              modify_dt = Utils.NowKorea(),
              modify_user = AppIdentity.user_seq,
              bg_color = ScheduleType.shareColor
            };

            foreach (var am in amList)
            {
              schedule_attend at = new schedule_attend()
              {
                uv_seq = am.uv_seq
                  ,
                create_dt = Utils.NowKorea()
                  ,
                create_user = AppIdentity.user_seq
                  ,
                modify_dt = Utils.NowKorea()
                  ,
                modify_user = AppIdentity.user_seq
              };

              attendList.Add(at);
            }

            await ser.CreateOrUpdateSchedule(sch, attendList, null, CommonCodes.Create);
          }
          else
          {
            sch.type = ScheduleType.share;
            sch.sub_type = ScheduleSubType.candidate;
            sch.title = "[" + client.kor_name + "]" + activity.title;
            sch.start_date = log_date;
            sch.end_date = log_date.AddMinutes(5);
            sch.contents = log_comment;
            sch.cl_seq = c_seq;
            sch.cal_seq = cal_seq;
            sch.modify_dt = Utils.NowKorea();
            sch.modify_user = AppIdentity.user_seq;

            var beforeAttendList = await ser.FindScheduleAttendListAsync(sch.s_seq);

            foreach (var am in amList)
            {
              schedule_attend at = new schedule_attend()
              {
                uv_seq = am.uv_seq
                  ,
                create_dt = Utils.NowKorea()
                  ,
                create_user = AppIdentity.user_seq
                  ,
                modify_dt = Utils.NowKorea()
                  ,
                modify_user = AppIdentity.user_seq
              };
            }

            await ser.CreateOrUpdateSchedule(sch, attendList, beforeAttendList, CommonCodes.Update);
          }
        }

        return Json(new
        {
          ok = true
            ,
          message = "저장했습니다."

        });
      }
      catch (Exception e)
      {
        return Json(new
        {
          ok = false
            ,
          message = "저장에 실패했습니다."
        });
      }
    }

    //[HttpPost]
    //public async Task<JsonResult> ClientActivityDelete(int cal_seq)
    //{
    //    try
    //    {
    //        ClientRepository cr = new ClientRepository();

    //        await cr.DeleteClientActivityAsync(cal_seq);

    //        return Json(new
    //        {
    //            ok = true
    //            ,
    //            message = "삭제되었습니다."
    //        });
    //    }
    //    catch (Exception e)
    //    {
    //        return Json(new
    //        {
    //            ok = false
    //            ,
    //            message = "실패했습니다."
    //        });
    //    }
    //}

    public async Task<JsonResult> ClientActivityDelete(int cal_seq)
    {
      try
      {
        ClientEntityRepository cer = new ClientEntityRepository();

        var cf = await cer.selectActivityOneAsync(cal_seq);

        if (cf == null)
        {
          return Json(new
          {
            ok = false
              ,
            message = "삭제하려는 활동내역이 없습니다."
          });
        }

        if (cf.is_schedule == 1)
        {
          ScheduleEntityRepository ser = new ScheduleEntityRepository();
          var sch = await ser.FindScheduleWithClientOneAsync(cf.c_seq, cf.cal_seq);
          var attendList = await ser.FindScheduleAttendListAsync(sch.s_seq);

          await ser.DeleteSchedule(sch, attendList);
        }

        await cer.CreateOrUpdateActivity(cf, CommonCodes.Delete, AppIdentity.user_seq);

        return Json(new
        {
          ok = true
            ,
          message = "활동내역을 삭제하였습니다."
        });
      }
      catch (Exception e)
      {
        return Json(new
        {
          ok = false
            ,
          message = "오류가 발생하였습니다."
        });
      }
    }


    [HttpPost]
    public async Task<JsonResult> ClientContactCreate(string name = "", int gender = 0, string division = "", string position = "", string email = "", string phone = "", string cell_phone = "", string memo = "", int ccSeq = 0, int cSeq = 0, int c_seq = 0)
    {
      try
      {
        ClientRepository cr = new ClientRepository();

        if (ccSeq != 0)
        {
          await cr.UpdateClientContactAsync(ccSeq, c_seq, name, gender, division, position, email, phone, cell_phone, memo, AppIdentity.user_seq);
        }
        else
        {
          await cr.CreateClientContactAsync(c_seq, name, gender, division, position, email, phone, cell_phone, memo);
        }


        return Json(new
        {
          ok = true
            ,
          message = "저장했습니다."

        });
      }
      catch (Exception e)
      {
        return Json(new
        {
          ok = false
            ,
          message = "저장에 실패했습니다."
        });
      }
    }


    [HttpPost]
    public async Task<JsonResult> ClientContactDelete(int cc_seq)
    {
      try
      {
        ClientRepository cr = new ClientRepository();

        await cr.DeleteClientContactAsync(cc_seq, AppIdentity.user_seq);

        return Json(new
        {
          ok = true
            ,
          message = "삭제되었습니다."
        });
      }
      catch (Exception e)
      {
        return Json(new
        {
          ok = false
            ,
          message = "실패했습니다."
        });
      }
    }


    [HttpPost]
    public async Task<JsonResult> ClientTaxCreate(string name = "", string division = "", string email = "", string phone = "", string cell_phone = "", string deposit_manager = "", string deposit_email = "", int c_seq = 0, int ctcSeq = 0)
    {
      try
      {
        ClientRepository cr = new ClientRepository();

        if (ctcSeq != 0)
        {
          await cr.UpdateClientTaxAsync(ctcSeq, c_seq, division, name, email, phone, cell_phone, deposit_manager, deposit_email);
        }
        else
        {
          await cr.CreateClientTaxAsync(c_seq, division, name, email, phone, cell_phone, deposit_manager, deposit_email);
        }


        return Json(new
        {
          ok = true
            ,
          message = "저장했습니다."

        });
      }
      catch (Exception e)
      {
        return Json(new
        {
          ok = false
            ,
          message = "저장에 실패했습니다."
        });
      }
    }


    public PartialViewResult ClientCreateFileList(ClientDetailViewModel model)
    {
      return PartialView(model);
    }

    //첨부파일 임시 업로드후 부분뷰 처리
    [HttpPost]
    public PartialViewResult ClientCreateFileList(ClientDetailViewModel model, client_file[] files)
    {
      if (model == null)
      {
        model = new ClientDetailViewModel();
      }

      if (model.fileList == null)
        model.fileList = new List<client_file>();

      if (files != null)
      {
        foreach (client_file file in files)
          model.fileList.Add(file);
      }

      return PartialView(model);
    }

    [HttpPost]
    public async Task<JsonResult> ClientTaxDelete(int ctc_seq)
    {
      try
      {
        ClientRepository cr = new ClientRepository();

        await cr.DeleteClientTaxAsync(ctc_seq);

        return Json(new
        {
          ok = true
            ,
          message = "삭제되었습니다."
        });
      }
      catch (Exception e)
      {
        return Json(new
        {
          ok = false
            ,
          message = "실패했습니다."
        });
      }
    }


    [HttpPost]
    public PartialViewResult ClientRemoveFileList(ClientDetailViewModel model, int file_seq)
    {
      if (model == null)
      {
        model = new ClientDetailViewModel();
      }

      if (model.fileList == null)
        model.fileList = new List<client_file>();


      model.fileList.RemoveAt(file_seq);
      //model.boardFileList[file_seq].file_dir = "";

      return PartialView("BoardCreateFileList", model);
      //return PartialView(model);
    }


    [HttpPost]
    public async Task<JsonResult> CreateTempClientFile(string uploadFolder, HttpPostedFileBase file)
    {
      try
      {
        FileUpload fiUpload = new FileUpload();
        string path = Server.MapPath("~/UploadedFiles");
        var rst = fiUpload.UploadTemp(path, "Temp", uploadFolder, file);


        client_file bf = new client_file()
        {
          directory = rst.dbPath
            ,
          origin_file_path = rst.filePath
            ,
          file_path = rst.name
            ,
          extension = rst.extension
        };

        if (!rst.status)
          return Json(new
          {
            ok = false,
            message = rst.statusMessage
          });

        return Json(new
        {
          ok = true,
          message = "업체파일을 등록 했습니다.",
          result = bf
        });

      }
      catch (Exception e)
      {
        return Json(new
        {
          ok = false,
          message = "알 수 없는 오류가 발생 했습니다."
        });
      }
    }


    //[HttpPost]
    //public PartialViewResult ClientCreateFileList(ClientDetailViewModel model, client_file[] files)
    //{
    //    if (model == null)
    //    {
    //        model = new ClientDetailViewModel();
    //    }

    //    if (model.fileList == null)
    //        model.fileList = new List<client_file>();

    //    if (files != null)
    //    {
    //        foreach (client_file file in files)
    //            model.fileList.Add(file);
    //    }

    //    return PartialView(model);
    //}

    //[HttpPost]
    //public async Task<JsonResult> CreateClientFile(int c_seq, string cf_type, HttpPostedFileBase files)
    //{
    //    try
    //    {
    //        FileUpload fUpload = new FileUpload();
    //        string path = Server.MapPath("~/UploadedFiles");
    //        var result = fUpload.UploadResume(c_seq, path, files);

    //        if (!result.status)
    //            return Json(new
    //            {
    //                ok = false,
    //                message = result.statusMessage
    //            });

    //        ClientEntityRepository cer = new ClientEntityRepository();

    //        client_file cf = new client_file()
    //        {
    //            c_seq = c_seq
    //            , cf_type = cf_type
    //            , directory = Path.Combine(Utils.GetRootUrl(Request), result.dbPath)
    //            , origin_file_path = result.filePath
    //            , file_path = result.name
    //            , extension = result.extension
    //        };

    //        await cer.CreateOrDeleteClientFile(cf, CommonCodes.Create);

    //        return Json(new
    //        {
    //            ok = true,
    //            message = "등록되었습니다."
    //        });
    //    }
    //    catch (Exception e)
    //    {
    //        return Json(new
    //        {
    //            ok = false,
    //            message = "알 수 없는 오류가 발생 했습니다."
    //        });
    //    }
    //}

    public ActionResult ActivityList(ClientHistorySearchModel search, int page = 1)
    {

      int totalCount = 0;
      string[] colorClasses = { "bg-cyan-600", "bg-orange-600", "bg-green-500", "bg-orange-500" };
      Random rand = new Random();

      //한 페이지당 몇개의 행을 보여줄 것인지
      int pageSize = AppPaging.PageLength30;

      ClientRepository cr = new ClientRepository();

      var list = cr.activityList(search, (page - 1) * pageSize, pageSize, out totalCount);
      var history_date_group_list = new List<ClientHistoryDateGroupModel>();

      if (list.Count > 0)
      {
        ClientHistoryDateGroupModel history_date_group = null;
        ClientHistoryGroupModel history_group = null;
        foreach (var item in list)
        {
          if (history_date_group == null || history_date_group.group_date != item.create_dt_str)
          {

            if (history_date_group != null && history_group != null)
            {
              history_date_group.group_list.Add(history_group);
            }

            if (history_date_group != null)
            {
              history_date_group_list.Add(history_date_group);
            }

            history_date_group = new ClientHistoryDateGroupModel()
            {
              group_date = item.create_dt_str
            };

            history_group = null;
          }
          //구간 변경될 경우 
          if (history_group == null || history_group.c_seq != item.c_seq)
          {
            if (history_group != null)
            {
              history_date_group.group_list.Add(history_group);
            }

            history_group = new ClientHistoryGroupModel()
            {
              c_seq = item.c_seq
                ,
              start_dt = item.create_dt
                ,
              end_dt = item.create_dt
                ,
              dot_color = colorClasses[rand.Next(colorClasses.Length)]
            };
          }
          if (history_date_group != null && history_group != null)
          {
            history_group.activityList.Add(item);
            history_group.start_dt = item.create_dt;
          }
        }


        if (history_date_group != null && history_group != null)
        {
          history_date_group.group_list.Add(history_group);
        }

        if (history_date_group != null)
        {
          history_date_group_list.Add(history_date_group);
        }
      }

      ClientHistoryListViewModel model = new ClientHistoryListViewModel
      {
        search = search
          ,
        list = history_date_group_list
          ,
        PagingInfo = new PagingInfo()
        {
          CurrentPage = page,
          ItemsPerPage = pageSize,
          TotalItems = totalCount
        }
      };

      return View(model);
    }


    public ActionResult AttentionList(AttentionSearchModel search, int page = 1)
    {
      int totalCount = 0;

      int pageSize = AppPaging.PageLength10;

      ClientRepository cr = new ClientRepository();

      var list = cr.attentionList(search, (page - 1) * pageSize, pageSize, out totalCount);

      AttentionListViewModel model = new AttentionListViewModel
      {
        search = search
          ,
        list = list
          ,
        PagingInfo = new PagingInfo()
        {
          CurrentPage = page,
          ItemsPerPage = pageSize,
          TotalItems = totalCount
        }
      };
      return View(model);
    }


    public async Task<JsonResult> ClientContractCreate(ClientContractSaveModel model)
    {
      try
      {
        ClientEntityRepository cer = new ClientEntityRepository();

        if (model.contract.cc_seq == 0)
        {
          model.contract.create_dt = Utils.NowKorea();
          model.contract.create_user = AppIdentity.user_seq;
          await cer.CreateOrUpdateClientContract(model.contract, model.annualIncomeList, model.positionRateList, CommonCodes.Create, AppIdentity.user_seq);
        }
        else
        {
          var contract = await cer.SelectClientContractOneAsync(model.contract.cc_seq);
          if (contract == null)
          {
            return Json(new
            {
              ok = false,
              message = "수정하려는 계약정보를 찾을 수 없습니다."
            });
          }

          contract.contract_date = model.contract.contract_date;
          contract.fee_type = model.contract.fee_type;
          contract.deposit_limit = model.contract.deposit_limit;
          contract.guarntee_type = model.contract.guarntee_type;
          contract.is_construct_debut = model.contract.is_construct_debut;
          contract.construct_debut_per = model.contract.construct_debut_per;
          contract.fix_fee_rate = model.contract.fix_fee_rate;
          contract.contract_comment = model.contract.contract_comment;
          contract.modify_dt = Utils.NowKorea();
          contract.modify_user = AppIdentity.user_seq;

          await cer.CreateOrUpdateClientContract(contract, model.annualIncomeList, model.positionRateList, CommonCodes.Update, AppIdentity.user_seq);
        }

        var client = await cer.SelectClientOneAsync(model.contract.c_seq);
        client.is_contract = 1;
        await cer.UpdateClientOneAsync(client);

        return Json(new
        {
          ok = true
            ,
          message = "저장했습니다."

        });
      }
      catch (Exception e)
      {
        return Json(new
        {
          ok = false
            ,
          message = "저장에 실패했습니다-control."
        });
      }
    }

    public async Task<JsonResult> MakeContractFile(int c_seq, int cc_seq)
    {
      try
      {
        ClientEntityRepository cer = new ClientEntityRepository();

        var client = await cer.SelectClientOneAsync(c_seq);

        if (string.IsNullOrWhiteSpace(client.ceo))
          return Json(new
          {
            ok = false
          ,
            message = "고객사 CEO를 입력해야 합니다."
          });

        if (string.IsNullOrWhiteSpace(client.biz_code))
          return Json(new
          {
            ok = false
          ,
            message = "고객사 사업자등록번호를 입력해야 합니다."
          });

        if (string.IsNullOrWhiteSpace(client.addr1))
          return Json(new
          {
            ok = false
          ,
            message = "고객사 주소를 입력해야 합니다."
          });

        var contract = await cer.SelectClientContractOneAsync(cc_seq);
        var annualList = new List<client_annual_income_rate>();
        var positionList = new List<client_position_rate>();

        string fileName = string.Format("[{0} {1} 국문]Contract_Standard_K.docx", client.kor_name, contract.fee_type == "C" ? "단일수수료" : "표준");
        string renameFileName = "";
        string samplePath = Path.Combine(Server.MapPath("~/UploadedFiles/contractSample/"), "[commonKor]Contract_Standard_K_sample.docx");
        string path = Path.Combine(Server.MapPath("~/UploadedFiles/Client/" + c_seq));
        string file_dir = Path.Combine(Utils.GetRootUrl(Request), path);
        string saveFilePath = Path.Combine(path, fileName);

        if (contract.fee_type == "A" || contract.fee_type == "B")
        {
          annualList = await cer.SelectClientAnnualIncomeRateList(c_seq, cc_seq);
        }

        lock (_fileObj)
        {


          //string file_dir = Path.Combine(Utils.GetRootUrl(Request), path);
          //string file_dir = Request.Url.GetLeftPart(UriPartial.Authority) + "/UploadedFiles/Client/" + c_seq + fileName;

          Directory.CreateDirectory(path);

          string ContractDate = DateTime.Parse(contract.contract_date).ToString("yyyy") + "년 " + DateTime.Parse(contract.contract_date).ToString("MM") + "월 " + DateTime.Parse(contract.contract_date).ToString("dd") + "일";

          using (var document = DocX.Load(samplePath))
          {
            var newDoc = document.Copy();

            string addr1 = "";
            string addr2 = "";

            if (client.addr1.Length > 20)
            {
              addr1 = client.addr1.Substring(0, 20);
              addr2 = client.addr1.Substring(21, client.addr1.Length - 21);
            }
            else
            {
              addr1 = client.addr1;
            }

            foreach (var paragraph in newDoc.Paragraphs)
            {
              if (paragraph.Text.Contains("(client_addr)"))
                paragraph.ReplaceText("(client_addr)", client.addr1);

              if (paragraph.Text.Contains("(client_name)"))
                paragraph.ReplaceText("(client_name)", client.kor_name);

              if (paragraph.Text.Contains("(contract_date)"))
                paragraph.ReplaceText("(contract_date)", ContractDate);


              if (paragraph.Text.Contains("(client_fee_rate)"))
              {
                //연봉별
                if (contract.fee_type == "A")
                {
                  string currencyName = Utils.ReturnCurrencyTxt(annualList.FirstOrDefault().Currency_Name);

                  var normalTable = newDoc.AddTable(annualList.Count + 1, 3);
                  normalTable.Alignment = Alignment.center;
                  normalTable.Design = TableDesign.Custom;
                  normalTable.SetWidthsPercentage(new[] { 55f, 10f, 35f }, 400);
                  normalTable.SetTableCellMargin(TableCellMarginType.bottom, 5f);
                  normalTable.Rows[0].Cells[0].Paragraphs.First().Append("채용 후보자의 초년도 총 보수").Bold().FontSize(10d);
                  normalTable.Rows[0].Cells[1].Paragraphs.First().Append("요율").Bold().FontSize(10d);
                  normalTable.Rows[0].Cells[2].Paragraphs.First().Append("비고").Bold().FontSize(10d);

                  for (int i = 1; i <= annualList.Count; i++)
                  {
                    var data = annualList[i - 1];

                    if (i == 1)
                    {
                      normalTable.Rows[i].Cells[0].Paragraphs.First().Append(Utils.ConvertMoneyToString(data.end_income) + currencyName + " 미만").FontSize(10d);
                    }
                    else
                    {
                      normalTable.Rows[i].Cells[0].Paragraphs.First().Append(Utils.ConvertMoneyToString(data.start_income) + currencyName + " 이상" + Utils.ConvertMoneyToString(data.end_income) + currencyName + " 미만").FontSize(10d);
                    }

                    normalTable.Rows[i].Cells[1].Paragraphs.First().Append(Math.Round((double)data.percentage, 0) + "%").FontSize(10d);
                    normalTable.Rows[i].Cells[2].Paragraphs.First().Append("부가세 별도(수임료의 10 %)").FontSize(10d);
                  }
                  normalTable.MergeCellsInColumn(2, 1, annualList.Count);
                  normalTable.Rows[1].Cells[2].VerticalAlignment = VerticalAlignment.Center;

                  paragraph.ReplaceText("(client_fee_rate)", "고객사가 지불하는 수임료는 후보자 입사일로부터 첫해 년도 총 보수를 기준으로 아래와 같이 청구하며 별도의 부가가치세가 부과된다.");
                  paragraph.InsertTableAfterSelf(normalTable);

                }
                //직급별
                else if (contract.fee_type == "B")
                {
                  var normalTable = newDoc.AddTable(annualList.Count + 1, 3);
                  normalTable.Alignment = Alignment.center;
                  normalTable.Design = TableDesign.Custom;
                  normalTable.SetWidthsPercentage(new[] { 55f, 10f, 35f }, 400);
                  normalTable.SetTableCellMargin(TableCellMarginType.bottom, 5f);
                  normalTable.Rows[0].Cells[0].Paragraphs.First().Append("채용 후보자의 초년도 총 보수").Bold().FontSize(10d);
                  normalTable.Rows[0].Cells[1].Paragraphs.First().Append("요율").Bold().FontSize(10d);
                  normalTable.Rows[0].Cells[2].Paragraphs.First().Append("비고").Bold().FontSize(10d);

                  for (int i = 1; i <= annualList.Count; i++)
                  {
                    var data = annualList[i - 1];

                    if (i == 1)
                    {
                      normalTable.Rows[i].Cells[0].Paragraphs.First().Append(Utils.ReturnPositionCodeTxt(data.start_income) + " ~ " + Utils.ReturnPositionCodeTxt(data.end_income)).FontSize(10d);
                    }
                    else
                    {
                      normalTable.Rows[i].Cells[0].Paragraphs.First().Append(Utils.ReturnPositionCodeTxt(data.start_income) + " ~ " + Utils.ReturnPositionCodeTxt(data.end_income)).FontSize(10d);
                    }

                    normalTable.Rows[i].Cells[1].Paragraphs.First().Append(Math.Round((double)data.percentage, 0) + "%").FontSize(10d);
                    normalTable.Rows[i].Cells[2].Paragraphs.First().Append("부가세 별도(수임료의 10 %)").FontSize(10d);
                  }

                  normalTable.MergeCellsInColumn(2, 1, annualList.Count);
                  normalTable.Rows[1].Cells[2].VerticalAlignment = VerticalAlignment.Center;

                  paragraph.ReplaceText("(client_fee_rate)", "고객사가 지불하는 수임료는 후보자 입사일로부터 첫해 년도 총 보수를 기준으로 아래와 같이 청구하며 별도의 부가가치세가 부과된다.");
                  paragraph.InsertTableAfterSelf(normalTable);
                }
                //고정수수료
                else
                {
                  paragraph.ReplaceText("(client_fee_rate)", "고객사가 지불하는 수임료는 후보자 입사일로부터 첫해 년도 총 보수의 " + contract.fix_fee_rate + "% 로 하며 별도의 부가가치세가 부과된다.");
                }
              }

              if (paragraph.Text.Contains("(is_construct_debut)"))
              {
                if (contract.is_construct_debut == "Y")
                {
                  //paragraph.ReplaceText("(is_construct_debut)", "만약 유니코가 2개월 이내에 적합한 후보자를 재 추천하지 못할 경우, 유니코는 “고객사”로부터 지급받은 금액에서 기초조사비로 30%를 공제하고 잔여금액에서 일할 계산하여 세금계산서를 재발행하고 30일 이내에 수수료를 반환한다. 단, 근무일수 기간 중의 토, 일, 공휴일, 유무급 휴가기간 및 연월차 휴가기간은 근무일수에 포함하며, 보증기간의 월은 30일로 통일한다.\n  * 환불 금액 = 서비스 잔여금액 X { (90일 - 근무일수)÷90일}\n  **서비스잔여금액 = 고객사가 유니코에 지불한 수임료의 70 %");
                  paragraph.ReplaceText("(is_construct_debut)", "만약 유니코가 2개월 이내에 적합한 후보자를 재 추천하지 못할 경우, 유니코는 “고객사”로부터 지급받은 금액에서 기초조사비로 30%를 공제하고 잔여금액에서 일할 계산하여 세금계산서를 재발행하고 30일 이내에 수수료를 반환한다. 단, 근무일수 기간 중의 토, 일, 공휴일, 유무급 휴가기간 및 연월차 휴가기간은 근무일수에 포함하며, 보증기간의 월은 30일로 통일한다.");
                  paragraph.InsertParagraphAfterSelf("   *환불 금액 = 서비스 잔여금액 X { (90일 - 근무일수)÷90일}").FontSize(10d).Bold().Alignment = Alignment.left;
                  paragraph.InsertParagraphAfterSelf("   ** 서비스잔여금액 = 고객사가 유니코에 지불한 수임료의 70%").FontSize(10d).Bold().Alignment = Alignment.left;
                }
                else
                {
                  paragraph.ReplaceText("(is_construct_debut)", "만약 유니코가 2개월 이내에 적합한 후보자를 재 추천하지 못할 경우, 유니코는 “고객사”로부터 지급받은 금액에서 일할 계산하여 세금계산서를 재발행하고 30일 이내에 수수료를 반환한다. 단, 근무일수 기간 중의 토, 일, 공휴일, 유무급 휴가기간 및 연월차 휴가기간은 근무일수에 포함하며, 보증기간의 월은 30일로 통일한다.");
                  paragraph.InsertParagraphAfterSelf("   * 환불 금액 = 지급된 수임료 X { (90일 - 근무일수)÷90일} ").FontSize(10d).Bold().Alignment = Alignment.left;
                }
              }

              if (paragraph.Text.Contains("(client_addr1)"))
                paragraph.ReplaceText("(client_addr1)", addr1);

              if (paragraph.Text.Contains("(client_addr2)"))
                paragraph.ReplaceText("(client_addr2)", addr2);

              if (paragraph.Text.Contains("(client_ceo)"))
                paragraph.ReplaceText("(client_ceo)", client.ceo);
            }


            //이미 생성된 계약 파일이 있을 경우, 기존 파일의 이름을 변경한다.
            if (System.IO.File.Exists(saveFilePath))
            {
              FileUpload fi = new FileUpload();

              FileInfo fInfo = new FileInfo(saveFilePath);
              var dd = fInfo.InitializeLifetimeService();

              renameFileName = "old_" + fInfo.LastWriteTime.ToString("yyyyMMddHHmm") + "_" + fInfo.Name;
              fi.Rename(fInfo, renameFileName);
            }

            newDoc.SaveAs(saveFilePath);

          }
        }

        if (!string.IsNullOrWhiteSpace(renameFileName))
        {
          var beforeFile = await cer.SelectContractFileAsync(c_seq, fileName);

          if (beforeFile != null)
          {
            beforeFile.file_dir = beforeFile.file_dir.Replace(fileName, renameFileName);
            beforeFile.file_origin_path = beforeFile.file_origin_path.Replace(fileName, renameFileName);
            beforeFile.file_path = renameFileName;
          }

          await cer.CreateOrUpdateContractFile(beforeFile, CommonCodes.Update, AppIdentity.user_seq);
        }

        client_contract_file cf = new client_contract_file()
        {
          c_seq = client.c_seq,
          file_type = "C",
          file_dir = Request.Url.GetLeftPart(UriPartial.Authority) + "/UploadedFiles/Client/" + c_seq + fileName,
          file_origin_path = saveFilePath,
          file_path = fileName,
          file_extension = ".docx",
          create_dt = Utils.NowKorea(),
          create_user = AppIdentity.user_seq
        };

        await cer.CreateOrUpdateContractFile(cf, CommonCodes.Create, AppIdentity.user_seq);

        return Json(new
        {
          ok = true
            ,
          message = "계약서 파일을 생성했습니다."
            ,
          cf_seq = cf.cf_seq
        });
      }
      catch (Exception e)
      {
        return Json(new
        {
          ok = false
            ,
          message = "계약서 파일 생성에 실패했습니다.<br/>Error : " + e.Message
        });
      }
    }

    public async Task<JsonResult> CreateAttention(int c_seq)
    {
      try
      {
        ClientEntityRepository cer = new ClientEntityRepository();

        var cf = await cer.SelectCliAttentionOneAsync(c_seq, AppIdentity.user_seq);

        if (cf != null)
        {
          return Json(new
          {
            ok = false
              ,
            message = "이미 관심 업체로 등록된 업체 입니다."
          });
        }

        cf = new client_favorite()
        {
          c_seq = c_seq
            ,
          uv_seq = AppIdentity.user_seq
          //,
          //create_dt = Utils.NowKorea()
        };

        await cer.CreateOrDeleteCliAttention(cf, CommonCodes.Create);

        return Json(new
        {
          ok = true
            ,
          cf_seq = cf.cf_seq
            ,
          message = "관심 업체를 등록 했습니다."
        });
      }
      catch (Exception e)
      {
        return Json(new
        {
          ok = false
            ,
          message = "알 수 없는 오류가 발생 했습니다."
        });
      }
    }

    /// <summary>
    /// 관심후보 삭제
    /// </summary>
    /// <param name="cf_seq"></param>
    /// <returns></returns>
    public async Task<JsonResult> DeleteAttention(int cf_seq)
    {
      try
      {
        ClientEntityRepository cer = new ClientEntityRepository();

        var cf = await cer.SelectCliAttentionOneAsync(cf_seq);

        if (cf == null)
        {
          return Json(new
          {
            ok = false
              ,
            message = "취소하려는 관심 업체를 찾을 수 없습니다."
          });
        }

        await cer.CreateOrDeleteCliAttention(cf, CommonCodes.Delete, AppIdentity.user_seq);

        return Json(new
        {
          ok = true
            ,
          message = "관심 업체를 취소 했습니다."
        });
      }
      catch (Exception e)
      {
        return Json(new
        {
          ok = false
            ,
          message = "알 수 없는 오류가 발생 했습니다."
        });
      }
    }



    public async Task<ActionResult> ContactList(ContactSearchModel search, int page = 1)
    {
      int totalCount = 0;

      int pageSize = AppPaging.PageLength10;

      ClientRepository cr = new ClientRepository();

      var list = cr.contactList(search, (page - 1) * pageSize, pageSize, out totalCount);

      if (AppIdentity.isExternal == 1)
      {
        ClientEntityRepository cer = new ClientEntityRepository();

        foreach (var data in list)
        {
          element_open_log eol = await cer.SelectClientOpenLogOneAsync(data.c_seq, AppIdentity.user_seq);
          if (eol == null)
          {
            data.is_external_lock = 1;
            if (!String.IsNullOrEmpty(data.phone))
              data.phone = Regex.Replace(data.phone, "[0-9]", "0");

            if (!String.IsNullOrEmpty(data.cell_phone))
              data.cell_phone = Regex.Replace(data.cell_phone, "[0-9]", "0");

            if (!String.IsNullOrEmpty(data.email))
              data.email = Regex.Replace(data.email, "[a-zA-Z0-9]", "*");
          }
        }
      }

      ContactListViewModel model = new ContactListViewModel
      {
        search = search
          ,
        list = list
          ,
        PagingInfo = new PagingInfo()
        {
          CurrentPage = page,
          ItemsPerPage = pageSize,
          TotalItems = totalCount
        }
      };
      return View(model);
    }

    public async Task<PartialViewResult> ContactSingleList(int c_seq, int cc_seq, int num)
    {

      ClientRepository cr = new ClientRepository();
      var model = await cr.SelectContactOneAsync(cc_seq);

      if (model != null)
      {
        if (AppIdentity.isExternal == 1)
        {
          ClientEntityRepository cer = new ClientEntityRepository();
          element_open_log eol = await cer.SelectClientOpenLogOneAsync(c_seq, AppIdentity.user_seq);
          if (eol == null)
          {
            model.is_external_lock = 1;
            if (!String.IsNullOrEmpty(model.phone))
              model.phone = Regex.Replace(model.phone, "[0-9]", "0");

            if (!String.IsNullOrEmpty(model.cell_phone))
              model.cell_phone = Regex.Replace(model.cell_phone, "[0-9]", "0");

            if (!String.IsNullOrEmpty(model.email))
              model.email = Regex.Replace(model.email, "[a-zA-Z0-9]", "*");
          }
        }
      }
      ViewBag.num = num;
      return PartialView(model);
    }

    public async Task<ActionResult> ContactListExcelDown(string korName, string engName, string contactName, string contactDivision, string contactPosition, string contactEmail, string amName, string contactPhone, string orderOption, string orderTxt, bool is_my_contact)
    {
      try
      {
        ClientRepository pr = new ClientRepository();

        ContactSearchModel search = new ContactSearchModel()
        {
          kor_name = korName,
          eng_name = engName,
          contact_name = contactName,
          contact_division = contactDivision,
          contact_position = contactPosition,
          contact_email = contactEmail,
          am_name = amName,
          contact_phone = contactPhone,
          orderOption = orderOption,
          orderTxt = orderTxt,
          is_my_contact = is_my_contact
        };

        var excel = new Excel<ContactLIstExcelModel>();
        var excelData = await pr.SelectContactListWithoutCountAsync(search);

        var data = excel.WriteExcel(excelData, "ContactList");

        return File(data.FileContents, data.ContentType, data.FileDownloadName);


      }
      catch (Exception e)
      {
        throw e;
      }
    }

    public async Task<JsonResult> DeleteContact(int cc_seq)
    {
      try
      {
        ClientEntityRepository cer = new ClientEntityRepository();

        var cf = await cer.SelectCliContactOneAsync(cc_seq);

        if (cf == null)
        {
          return Json(new
          {
            ok = false
              ,
            message = "삭제하려는 Contact를 찾을 수 없습니다."
          });
        }

        await cer.CreateOrDeleteCliContact(cf, CommonCodes.Delete, AppIdentity.user_seq);

        return Json(new
        {
          ok = true
            ,
          message = "Contact를 삭제 했습니다."
        });
      }
      catch (Exception e)
      {
        return Json(new
        {
          ok = false
            ,
          message = "알 수 없는 오류가 발생 했습니다."
        });
      }
    }



    public async Task<ActionResult> DormantList(DormantSearchModel search, int page = 1)
    {
      int totalCount = 0;

      //한 페이지당 몇개의 행을 보여줄 것인지
      int pageSize = AppPaging.PageLength20;

      ClientRepository cr = new ClientRepository();

      var list = cr.DorClientList(search, (page - 1) * pageSize, pageSize, out totalCount);

      if (AppIdentity.isExternal == 1)
      {
        ClientEntityRepository cer = new ClientEntityRepository();

        foreach (var data in list)
        {
          element_open_log eol = await cer.SelectClientOpenLogOneAsync(data.c_seq, AppIdentity.user_seq);
          if (eol == null)
          {
            data.is_external_lock = 1;
            if (!String.IsNullOrEmpty(data.contact_phone))
              data.contact_phone = Regex.Replace(data.contact_phone, "[0-9]", "0");
          }
        }
      }


      DormantListViewModel model = new DormantListViewModel
      {
        search = search
          ,
        list = list
          ,
        PagingInfo = new PagingInfo()
        {
          CurrentPage = page,
          ItemsPerPage = pageSize,
          TotalItems = totalCount
        }
      };
      return View(model);
    }


    public PartialViewResult SearchClient(string searchTxt, int page = 1)
    {
      ProjectRepository pr = new ProjectRepository();

      int totalCount = 0;

      //한 페이지당 몇개의 행을 보여줄 것인지
      int pageSize = AppPaging.PageLength5;

      var list = pr.SelectClientList(searchTxt, (page - 1) * pageSize, pageSize, out totalCount);

      var model = new SearchProjectClientModel()
      {
        searchTxt = searchTxt
          ,
        list = list
          ,
        PagingInfo = new PagingInfo()
        {
          CurrentPage = page
              ,
          ItemsPerPage = pageSize
              ,
          TotalItems = totalCount
        }
      };

      return PartialView(model);
    }


    public async Task<ActionResult> PopContractCreate(int c_seq, int cc_seq = 0)
    {
      ClientRepository cr = new ClientRepository();

      var data = await cr.SelectClientContractOneAsync(c_seq, cc_seq);
      var fee_infos = new List<client_annual_income_rate>();

      if (data != null)
      {
        data.contract_date = Utils.ConvertDateTimeToString(Utils.NowKorea());
        fee_infos = await cr.SelectClientAnnualIncomeRateListAsync(c_seq, data.cc_seq);
      }
      else
      {
        data = new client_contract();
        data.fee_type = "A";
        data.is_construct_debut = "Y";
        data.construct_debut_per = 30;
        data.currency_cd = "KRW";
        data.deposit_gubun1 = "계산서를 수령한 날로부터";
        data.deposit_gubun2 = "주";
        data.grt_period = 90;
        data.grt_gubun = "일";
        data.deposit_period = 2;
        data.grt_type = "입사일로 부터";
        data.vat_type = 3;

        fee_infos.Add(new client_annual_income_rate()
        {
          end_income = 50000000,
          percentage = 20
        });

        fee_infos.Add(new client_annual_income_rate()
        {
          start_income = 50000000,
          end_income = 100000000,
          percentage = 25
        });

        fee_infos.Add(new client_annual_income_rate()
        {
          start_income = 100000000,
          percentage = 30
        });
      }

      

      ViewBag.cSeq = c_seq;
      ViewBag.ccSeq = cc_seq;

      ClientContractDetailModel model = new ClientContractDetailModel()
      {
        data = data
          ,
        feeList = fee_infos
      };

      return View(model);
    }

    [HttpPost]
    public async Task<JsonResult> PopContractCreate(ContractCreateModel model)
    {
      try
      {
        ClientEntityRepository cr = new ClientEntityRepository();
        /*
        var data = new client_contract();
        var fee_step = 
        if(model.edit_mode == "A" && model.data.cc_seq > 0)
        {
            data = await cr.SelectClientContractOneAsync(model.data.cc_seq)


        }
        data.contract_date 
        data.contract_comment
        */
        model.data.create_dt = Utils.NowKorea();
        model.data.create_user = AppIdentity.user_seq;

        await cr.CreateContract(model.edit_mode, model.data, model.fee_step, "N", AppIdentity.user_seq);

        return Json(new
        {
          ok = true
            ,
          message = "등록되었습니다."
        });
      }
      catch (Exception e)
      {
        return Json(new
        {
          ok = false
            ,
          message = "알 수 없는 오류가 발생 했습니다." + e.Message
        });
      }
    }


    public ActionResult PopfeeTypeA(int c_seq = 0, int cc_seq = 0, int page = 1)
    {
      ClientRepository cr = new ClientRepository();

      int totalCount = 0;

      //한 페이지당 몇개의 행을 보여줄 것인지
      int pageSize = AppPaging.PageLength30;

      var list = cr.FeetypeListA(c_seq, cc_seq, (page - 1) * pageSize, pageSize, out totalCount);

      ClientListViewModel vm = new ClientListViewModel
      {
        incomeList = list
      };


      ViewBag.cSeq = c_seq;
      ViewBag.ccSeq = cc_seq;
      return View(vm);
    }

    [HttpPost]
    public async Task<JsonResult> CreateTypeAfee(int c_seq, int cc_seq, string currency_name, int incomNum, int[] incomSamt, int[] incomEamt, decimal[] incomPer)
    {
      try
      {
        ClientRepository cr = new ClientRepository();

        await cr.CreateIncomFeeTypeA(c_seq, cc_seq, currency_name, incomNum, incomSamt, incomEamt, incomPer);

        return Json(new
        {
          ok = true
            ,
          message = "등록되었습니다."
        });
      }
      catch (Exception e)
      {
        return Json(new
        {
          ok = false
            ,
          message = "알 수 없는 오류가 발생 했습니다."
        });
      }
    }

    //public ActionResult PopfeeTypeB(int c_seq = 0, int cc_seq = 0)
    //{
    //    ClientRepository cr = new ClientRepository();

    //    int totalCount = 0;
    //    int page = 1;

    //    //한 페이지당 몇개의 행을 보여줄 것인지
    //    int pageSize = AppPaging.PageLength30;

    //    var list = cr.FeetypeListB(c_seq, cc_seq, (page - 1) * pageSize, pageSize, out totalCount);

    //    ClientListViewModel vm = new ClientListViewModel
    //    {
    //        positionList = list
    //    };


    //    //vm.allPosition = cr.SelectPositionListAsync();


    //    ViewBag.cSeq = c_seq;
    //    ViewBag.ccSeq = cc_seq;
    //    return View(vm);
    //}
    public ActionResult PopfeeTypeB(int c_seq = 0, int cc_seq = 0, int page = 1)
    {
      ClientRepository cr = new ClientRepository();

      int totalCount = 0;

      //한 페이지당 몇개의 행을 보여줄 것인지
      int pageSize = AppPaging.PageLength30;

      var list = cr.FeetypeListB(c_seq, cc_seq, (page - 1) * pageSize, pageSize, out totalCount);

      ClientListViewModel vm = new ClientListViewModel
      {
        positionList = list
      };

      //vm.allPosition = cr.SelectPositionListAsync();

      ViewBag.cSeq = c_seq;
      ViewBag.ccSeq = cc_seq;
      return View(vm);
    }

    [HttpPost]
    public async Task<JsonResult> CreateTypeBfee(int c_seq, int cc_seq, int positionNum, string[] positionS, string[] positionE, decimal[] positionPer)
    {
      try
      {
        ClientRepository cr = new ClientRepository();

        cr.CreateIncomFeeTypeB(c_seq, cc_seq, positionNum, positionS, positionE, positionPer);

        return Json(new
        {
          ok = true
            ,
          message = "등록되었습니다."
        });
      }
      catch (Exception e)
      {
        return Json(new
        {
          ok = false
            ,
          message = "알 수 없는 오류가 발생 했습니다."
        });
      }
    }



    public async Task<ActionResult> PopfeeTypeC(int cc_seq = 0)
    {
      ClientRepository cr = new ClientRepository();

      var data = await cr.SelectFixfeeAsync(cc_seq);
      if (data == null)
        data = new client_contract();

      ClientPopupModel model = new ClientPopupModel()
      {
        contract_data = data
      };

      return View(model);
    }


    public async Task<ActionResult> ClientProjectCopy(int p_seq = 0, string company_name = "")
    {
      ProjectRepository pr = new ProjectRepository();

      var data = await pr.SelectProjectOneAsync(p_seq);
      if (data == null)
        data = new project();

      ProjectCreateModel model = new ProjectCreateModel()
      {
        pjt_type = data.pjt_type,
        c_seq = data.c_seq,
        recruit_number = data.recruit_number,
        is_posting = data.is_posting,
        title = data.title,
        position_seq = data.position_seq,
        position_str = data.position_str,
        experience_type = data.experience_type,
        expreience_number = data.expreience_number,
        edu_code = data.edu_code,
        edu_name = data.edu_name,
        foreign_lang = data.foreign_lang,
        foreign_lang_name = data.foreign_lang_name,
        foreign_level = data.foreign_level,
        assign_task = data.assign_task,
        requirement = data.requirement,
        is_kor_resume = data.is_kor_resume,
        is_eng_resume = data.is_eng_resume,
        is_portfolio = data.is_portfolio,
        is_self_introduction = data.is_self_introduction,
        is_etc = data.is_etc,
        etc_comment = data.etc_comment,
        is_pre_interview = data.is_pre_interview,
        is_number = data.is_number,
        interview_number = data.interview_number,
        is_personality_test = data.is_personality_test,
        gender_type = data.gender_type,
        start_age = data.start_age,
        end_age = data.end_age,
        target_school_nm = data.target_school_nm,
        target_major_nm = data.target_major_nm,
        target_company_nm = data.target_company_nm,
        expected_salary = data.expected_salary,
        currency_cd = data.currency_cd,
        fee_rate = data.fee_rate,
        pjt_status = ProjectStatusCode.progress,
        business_code1 = data.business_code1,
        business_code2 = data.business_code2,
        company_name = company_name
        //create_dt = Utils.NowKorea(),
        //create_user = AppIdentity.user_seq,
        //modify_dt = Utils.NowKorea(),
        //modify_user = AppIdentity.user_seq
      };

      model.amList = await pr.SelectProjectAmListAsync(p_seq);
      model.searcherList = await pr.SelectProjectSearcherListAsync(p_seq);
      return View(model);
    }


    //[HttpPost]
    //public async Task<JsonResult> ClientProjectCopy(ProjectCreateModel data)
    //{
    //    try
    //    {
    //        ProjectEntityRepository per = new ProjectEntityRepository();

    //        ClientCreateUpdateModel model = new ClientCreateUpdateModel();

    //        string state = "";

    //        if (data.p_seq == 0)
    //        {
    //            model.data = new project()
    //            {
    //                pjt_type = data.pjt_type,
    //                c_seq = data.c_seq
    //            };
    //        }
    //    }
    //    catch (Exception e)
    //    {
    //        throw e;
    //    }

    //}


    [HttpPost]
    public async Task<JsonResult> FindClientInfo(int c_seq, int cc_seq, string fee_type)
    {
      try
      {
        ClientRepository cr = new ClientRepository();

        var amList = await cr.SelectClientAmListAsync(c_seq);

        var feeRateList = new Object();
        if (fee_type == ContractFeeType.annual)
        {
          feeRateList = await cr.SelectClientAnnualIncomeRateListAsync(c_seq, cc_seq);
        }

        if (fee_type == ContractFeeType.position)
        {
          feeRateList = await cr.SelectClientPositionRateListAsync(c_seq, cc_seq);
        }

        return Json(new
        {
          ok = true,
          list = amList,
          feeRateList = feeRateList
        });
      }
      catch (Exception e)
      {
        return Json(new
        {
          ok = false,
          message = "회사 AM 리스트를 불러오는 중 오류가 발생 했습니다."
        });
      }
    }

    public PartialViewResult ClientSearch(string searchTxt, int page = 1)
    {
      ClientRepository cr = new ClientRepository();

      int totalCount = 0;

      //한 페이지당 몇개의 행을 보여줄 것인지
      int pageSize = AppPaging.PageLength10;

      var list = cr.SelectClientList(searchTxt, (page - 1) * pageSize, pageSize, out totalCount);

      var model = new SeachClientModel()
      {
        searchTxt = searchTxt,
        list = list,
        PagingInfo = new PagingInfo()
        {
          CurrentPage = page
              ,
          ItemsPerPage = pageSize
              ,
          TotalItems = totalCount
        }
      };

      return PartialView(model);
    }


    public PartialViewResult NationalcodeSearch(string searchTxt = "")
    {
      ClientRepository cr = new ClientRepository();

      var list = cr.SelectNationalcodeList(searchTxt);

      var model = new SearchNationalcodeModel()
      {
        searchTxt = searchTxt,
        list = list
      };

      return PartialView(model);
    }


    [HttpPost]
    public async Task<JsonResult> CreateClientFile(int c_seq, string cf_type, HttpPostedFileBase files)
    {
      try
      {
        FileUpload fiUpload = new FileUpload();
        string path = Server.MapPath("~/UploadedFiles");
        var result = fiUpload.UploadContract(c_seq, path, files);

        if (!result.status)
          return Json(new
          {
            ok = false,
            message = result.statusMessage
          });

        ClientEntityRepository cer = new ClientEntityRepository();

        client_file cr = new client_file()
        {
          c_seq = c_seq,
          cf_type = cf_type,
          directory = Path.Combine(Utils.GetRootUrl(Request), result.dbPath),
          origin_file_path = result.filePath,
          file_path = result.name,
          extension = result.extension,
          create_dt = Utils.NowKorea(),
          create_user = AppIdentity.user_seq
        };

        await cer.CreateOrDeleteClientFile(cr, CommonCodes.Create);

        var clt = await cer.SelectClientOneAsync(c_seq);
        if (clt != null)
        {
          clt.modify_user = AppIdentity.user_seq;
          clt.modify_dt = Utils.NowKorea();

          await cer.UpdateClientOneAsync(clt);
        }


        return Json(new
        {
          ok = true,
          message = "관련파일을 등록 했습니다.",
          result = cr
        });
      }
      catch (Exception e)
      {
        return Json(new
        {
          ok = false,
          message = "알 수 없는 오류가 발생 했습니다."
        });
      }
    }

    [HttpPost]
    public async Task<JsonResult> DeleteClientFile(int c_seq, int cf_seq)
    {
      try
      {
        ClientEntityRepository cer = new ClientEntityRepository();

        var cr = await cer.SelectClientFileOneAsync(c_seq, cf_seq);

        if (cr == null)
          return Json(new
          {
            ok = false
              ,
            message = "DB에서 삭제하려는 파일을 찾을 수 없습니다."
          });
        /*
        FileUpload fiUpload = new FileUpload();

        if (!fiUpload.DeleteFile(cr.origin_file_path))
        {
          return Json(new
          {
            ok = false
                 ,
            message = "경로에서 삭제하려는 파일을 찾을 수 없습니다."
          });
        }
        */
        await cer.CreateOrDeleteClientFile(cr, CommonCodes.Delete, AppIdentity.user_seq);

        return Json(new
        {
          ok = true,
          message = "관련파일을 삭제 했습니다.",
        });
      }
      catch (Exception e)
      {
        return Json(new
        {
          ok = false,
          message = "알 수 없는 오류가 발생 했습니다."
        });
      }
    }

    [HttpPost]
    public async Task<JsonResult> ClientSearchUni(int c_seq, string kor_name)
    {
      try
      {
        ClientRepository cr = new ClientRepository();

        int data = await cr.SelectUniClientAsync(c_seq, kor_name.Replace("(주)", "").Replace("(유)", "").Replace(" ", ""));

        bool valid = data > 0 ? false : true;

        return Json(new
        {
          ok = true,
          data = data,
          valid = valid
        });
      }
      catch (Exception e)
      {
        return Json(new
        {
          ok = false
        });
      }
    }

    [HttpPost]
    public async Task<JsonResult> BizCheck(string biz_code)
    {
      try
      {
        int q = 0;

        if (string.IsNullOrWhiteSpace(biz_code))
          return Json(new
          {
            ok = false
          });

        biz_code = biz_code.Replace("-", "");
        if (biz_code.Length != 10)
        {
          q = 1;
        }

        int sum = 0;
        string checkNo = "137137135";

        for (int i = 0; i < checkNo.Length; i++)
          sum += (int)Char.GetNumericValue(biz_code[i]) * (int)Char.GetNumericValue(checkNo[i]);

        sum += (int)Char.GetNumericValue(biz_code[8]) * 5 / 10;

        sum %= 10;

        if (sum != 0)
          sum = 10 - sum;

        if (sum != (int)Char.GetNumericValue(biz_code[9]))
          q = 2;
        else
          q = 3;


        bool valid = q < 3 ? false : true;

        return Json(new
        {
          ok = true,
          data = biz_code,
          valid = valid
        });
      }
      catch (Exception e)
      {
        return Json(new
        {
          ok = false
        });
      }


    }

    [HttpPost]
    public static bool CheckBizCode(string biz_code)
    {
      biz_code = biz_code.Replace("-", "");
      if (biz_code.Length != 10)
      {
        return false;
      }

      int sum = 0;
      string checkNo = "137137135";

      for (int i = 0; i < checkNo.Length; i++)
        sum += (int)Char.GetNumericValue(biz_code[i]) * (int)Char.GetNumericValue(checkNo[i]);

      sum += (int)Char.GetNumericValue(biz_code[8]) * 5 / 10;

      sum %= 10;

      if (sum != 0)
        sum = 10 - sum;

      if (sum != (int)Char.GetNumericValue(biz_code[9]))
        return false;
      else
        return true;
    }
    /*
    #region 인오더 고객사 생성.

    public async Task<ActionResult> CreateInorderClient(int i_seq)
    {
      ProjectRepository pr = new ProjectRepository();
      var inorder = await pr.SelectInorderOneAsync(i_seq);


      ClientRepository cr = new ClientRepository();

      ClientCreateModel model = new ClientCreateModel()
      {
        c_seq = 0
          ,
        kor_name = inorder.clt_name
          ,
        eng_name = ""
          ,
        ceo = ""
          ,
        is_foreign_invest = 0
          ,
        addr1 = ""
          ,
        addr2 = ""
          ,
        is_contract = 0
          ,
        is_foreign = 0
          ,
        foreign_code = "KR"
          ,
        country_name = "한국"
          ,
        biz_code = ""
          ,
        biz_type = inorder.clt_busi_type
          ,
        biz_category = ""
          ,
        biz_industry = ""
          ,
        busiCode1 = 0
          ,
        busiCode2 = 0
          ,
        fix_title = ""
          ,
        homepage = inorder.clt_url
          ,
        employee_number = 0
          ,
        sales_amount = 0
          ,
        name = inorder.cc_name
          ,
        gender = inorder.cc_gender
          ,
        email = inorder.cc_email
          ,
        phone = inorder.cc_phone
          ,
        cell_phone = inorder.cc_cell_phone
          ,
        division = inorder.cc_division
          ,
        position = inorder.cc_position
          ,
        tax_name = ""
          ,
        tax_division = ""
          ,
        tax_email = ""
          ,
        tax_phone = ""
          ,
        tax_cell_phone = ""
          ,
        tax_deposit_email = ""
          ,
        tax_deposit_manager = ""
          ,
        business_name2 = ""
          ,
        contact_seq = 0
          ,
        tax_seq = 0
          ,
        i_seq = i_seq
      };

      model.amList.Add(new client_manager()
      {
        c_seq = 0,
        uv_seq = (int)inorder.director_seq,
        am_name = inorder.director_name
      });

      return View("ClientCreate", model);
    }

    #endregion
    */
  }
}