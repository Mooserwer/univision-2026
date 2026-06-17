using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
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
using Univision.Core.Models.DTO.Request.Candidate;
using Univision.Core.Models.DTO.Response.Candidate;
using Univision.Core.Models.DTO.Response.Project;
using Univision.Core.Models.DTO.Response.Statistics;
using Univision.Core.Models.HomePage;
using Univision.Core.Repositories;
using Univision.Core.Repositories.Homepage;
using Univision.Main.Infrastructure;
using Univision.Main.Infrastructure.Mailing;
using Univision.Main.Models.Api;
using Univision.Main.Models.Candidate;
using Univision.Main.Models.Invoice;
using Univision.Main.Models.Project;
using Univision.Security;
using Xceed.Document.NET;
using Xceed.Words.NET;

namespace Univision.Main.Controllers
{
  public class InvoiceController : BaseController
  {
    /// <summary>
    /// 프로젝트 클라이언트 담당자
    /// </summary>
    /// <param name="c_seq"></param>
    /// <returns></returns>
    public async Task<PartialViewResult> SearchProjectContact(int p_seq, int c_seq)
    {
      ClientRepository cr = new ClientRepository();

      var list = await cr.SelectClientContactListAsync(c_seq);

      ProjectContactListModel model = new ProjectContactListModel()
      {
        p_seq = p_seq,
        list = list
      };

      return PartialView(model);
    }

    [HttpPost] // 새 인보이스 방식용
    public async Task<JsonResult> ProjectNewContactSubmit(client_contact data)
    {
      try
      {

        ClientEntityRepository cer = new ClientEntityRepository();
        var client = await cer.SelectClientOneAsync(data.c_seq);
        if (client == null)
          return Json(new
          {
            ok = false,
            message = "담당자를 등록하려는 고객사 정보를 찾을 수 없습니다."
          });

        data.create_dt = Utils.NowKorea();
        data.create_user = AppIdentity.user_seq;
        data.modify_dt = Utils.NowKorea();
        data.modify_user = AppIdentity.user_seq;

        await cer.CreateOrDeleteCliContact(data, CommonCodes.Create);

        return Json(new
        {
          ok = true
            ,
          cc_seq = data.cc_seq
            ,
          message = "프로젝트 담당자를 저장 했습니다."
        });
      }
      catch (Exception e)
      {
        return Json(new
        {
          ok = false,
          message = "프로젝트 담당자 등록 중, 오류가 발생 했습니다. - " + e.Message
        });
      }
    }

    public async Task<PartialViewResult> SearchProjectTaxContact(int p_seq, int c_seq)
    {
      ClientRepository cr = new ClientRepository();

      var list = await cr.SelectClientTaxContactListAsync(c_seq);

      ProjectTaxContactListModel model = new ProjectTaxContactListModel()
      {
        p_seq = p_seq,
        list = list
      };

      return PartialView(model);
    }

    [HttpPost] // 새로운 계산서 담당자 저장
    public async Task<JsonResult> ProjectNewTaxContactSubmit(client_tax_contact data)
    {
      try
      {
        ClientEntityRepository cer = new ClientEntityRepository();
        var client = await cer.SelectClientOneAsync(data.c_seq);
        if (client == null)
          return Json(new
          {
            ok = false,
            message = "세금계산서 담당자를 등록하려는 고객사 정보를 찾을 수 없습니다."
          });

        data.create_dt = Utils.NowKorea();
        data.create_seq = AppIdentity.user_seq;
        data.modify_dt = Utils.NowKorea();
        data.modify_seq = AppIdentity.user_seq;

        await cer.CreateOrUpdateTaxContact(data, CommonCodes.Create);

        return Json(new
        {
          ok = true
            ,
          ctc_seq = data.ctc_seq
            ,
          message = "전자세금계산서 담당자를 저장 했습니다."
        });
      }
      catch (Exception e)
      {
        return Json(new
        {
          ok = false,
          message = "전자 세금계산서 담당자 등록 중, 오류가 발생 했습니다. - " + e.Message
        });
      }
    }

    /// <summary>
    /// 프로젝트 인보이스 매출분배 담당자 추가.
    /// </summary>
    /// <param name="ud_seq"></param>
    /// <param name="searchTxt"></param>
    /// <returns></returns>

    public async Task<PartialViewResult> AddInvoiceShareUser(int ud_seq = 0, string searchTxt = "")
    {
      ProjectRepository pr = new ProjectRepository();
      AccountRepository ar = new AccountRepository();

      var list = await pr.SelectSearcherList(ud_seq, searchTxt);
      var deptList = await ar.SelectDivisionList();

      SearchProjectSearcherModel model = new SearchProjectSearcherModel()
      {
        ud_seq = ud_seq
          ,
        searchTxt = searchTxt
          ,
        list = list
          ,
        deptList = deptList
      };

      return PartialView(model);
    }

    #region 신 인보이스 

    public async Task<ActionResult> InvoiceNewSelect(int p_seq)
    {
      InvoiceStatusModel model = new InvoiceStatusModel();
      InvoiceRepository ivr = new InvoiceRepository();
      ProjectEntityRepository per = new ProjectEntityRepository();

      var project = await per.SelectProjectOneAsync(p_seq);
      if (project == null)
        project = new project();
      //인보이스 발행되지 않은 후보자 수 (발행 후 취소도 포함)
      //후보자 수가 1 이상이어야 일반인보이스 발행가능
      var hire_cnt = await ivr.SelectHireCandidateCnt(p_seq);

      //기존 발행된 선수금 인보이스 수 
      //후보자 수가 0 이어야 선수금인보이스 발행가능
      var retainer_inv_cnt = await ivr.SelectInvoiceCnt(p_seq, 1);

      //기존 발행된 모든 인보이스 수 (취소 안된)
      //인보이스 수가 1 이상이어야 취소 또는 환불 인보이스 발행가능
      var hire_inv_cnt = await ivr.SelectInvoiceCnt(p_seq, 0);

      var hire_inv_sent_cnt = await ivr.SelectInvoiceCnt(p_seq, 0, 1);
      var retainer_inv_sent_cnt = await ivr.SelectInvoiceCnt(p_seq, 1, 1);
      model.p_seq = project.p_seq;
      model.pjt_type = project.pjt_type;
      model.hire_inv_cnt = hire_cnt;
      model.retainer_inv_cnt = retainer_inv_cnt;
      model.cancel_inv_cnt = (hire_inv_sent_cnt + retainer_inv_sent_cnt);

      return View(model);
    }

    public PartialViewResult PartialInvoiceCreate1_Lang(InvoiceCreateModel model)
    {
      return PartialView(model);
    }

    public PartialViewResult PartialInvoiceCreate2_Cand(InvoiceCreateModel model)
    {
      return PartialView(model);
    }

    public PartialViewResult PartialInvoiceCreate3_Client(InvoiceCreateModel model)
    {
      return PartialView(model);
    }

    public PartialViewResult PartialInvoiceCreate4_Contact(InvoiceCreateModel model)
    {
      return PartialView(model);
    }

    public PartialViewResult PartialInvoiceCreate4_1_PreInvoice(InvoiceCreateModel model)
    {
      return PartialView(model);
    }

    public PartialViewResult PartialInvoiceCreate5_Fee(InvoiceCreateModel model)
    {
      return PartialView(model);
    }

    public PartialViewResult PartialInvoiceCreate6_Content(InvoiceCreateModel model)
    {
      return PartialView(model);
    }

    public PartialViewResult PartialInvoiceCreate7_Print(InvoiceCreateModel model)
    {
      return PartialView(model);
    }

    #region 성공인보이스
    public async Task<ActionResult> InvoiceContengencyCreate(int p_seq, int pic_seq = 0)
    {
      ProjectRepository pr = new ProjectRepository();
      ProjectEntityRepository per = new ProjectEntityRepository();
      ClientRepository cr = new ClientRepository();
      ClientEntityRepository cer = new ClientEntityRepository();

      var data = await per.SelectProjectOneAsync(p_seq);
      //var data = await pr.SelectProjectRepInvoiceOneAsync(p_seq);
      var chargeList = await pr.SelectProjectInvoiceChargeOneAsync(p_seq);
      var client_data = await cer.SelectClientOneAsync(data.c_seq);
      var client_contract = await cer.SelectClientLatestContractOneAsync(data.c_seq);
      var candidateList = await pr.SelectHireNoInvCandidateListAsync(data.p_seq);
      var retainer_amt = await pr.SelectProjectRetainerMoneyAsync(p_seq, 0);
      var client_cert_list = await cr.SelectClientFileByOnlyCseqAsync(data.c_seq);
      var client_cert = client_cert_list.Where(x => x.cf_type == "1").OrderByDescending(x => x.cf_seq).FirstOrDefault();
      var client_agr = client_cert_list.Where(x => x.cf_type == "4").OrderByDescending(x => x.cf_seq).FirstOrDefault();
      var pre_invoice = await pr.SelectProjectInvoiceOneByClientAsync(data.c_seq, 0);
      if (pre_invoice == null)
      {
        pre_invoice = await pr.SelectProjectInvoiceOneByClientAsync(data.c_seq, 2);
      }

      if (pre_invoice == null)
      {
        pre_invoice = await pr.SelectProjectInvoiceOneByClientAsync(data.c_seq, 1);
      }


      if (client_cert == null)
        client_cert = new client_file();
      if (client_agr == null)
        client_agr = new client_file();
      if (candidateList == null)
        candidateList = new List<pjt_recandidate_history>();
      if (client_contract == null)
        client_contract = new client_contract();
      if (chargeList.Count == 1)
      {
        chargeList.FirstOrDefault().sales_rate = 100;
        chargeList.FirstOrDefault().sales_money = 0;
      }

      InvoiceCreateModel model = new InvoiceCreateModel()
      {
        p_seq = data.p_seq,
        pjt_type = data.pjt_type,
        pjt_position_str = data.position_str,
        billing_money = 0,
        bill_currency_cd = data.currency_cd,
        fee_rate = decimal.ToDouble(data.fee_rate.Value),
        billing_type = (client_contract.fee_type == "C" ? 1 : 0),
        invoice_type = (retainer_amt > 0 ? 2 : 0),
        remarks = "",
        retainer_amt = retainer_amt,
        client_seq = client_data.c_seq,
        client_name_kor = client_data.kor_name,
        client_name_eng = client_data.eng_name,
        ceo_kor = client_data.ceo,
        addr_kor = client_data.addr1,
        client_biz_code = client_data.biz_code.Replace(" ", ""),
        vat_type = client_contract.vat_type,

        client_cert_upload_dt = client_cert.create_dt,
        client_cert_file = client_cert.file_path,
        client_cert_seq = client_cert.cf_seq,
        client_agr_upload_dt = client_agr.create_dt,
        client_agr_file = client_agr.file_path,
        client_agr_seq = client_agr.cf_seq,
        is_open_name = 1,
        is_open_annual_income = 1,
        is_po_no = 0,
        chargeList = chargeList,
        candidateList = candidateList,
        pre_invoice = pre_invoice,
        opt_pre_select_seq = pic_seq
      };
      if (data.cc_seq.HasValue && data.cc_seq.Value > 0)
      {
        var contact = await cer.SelectCliContactOneAsync(data.cc_seq.Value);

        model.cc_seq = data.cc_seq.Value;
        model.client_contact_name = contact.name;
        model.client_contact_div = contact.division;
        model.client_contact_tel = contact.phone;
        model.client_contact_tel2 = contact.cell_phone;
        model.client_contact_email = contact.email.Replace(" ", "");
        model.client_contact_pos = contact.position;
        model.client_contact_gender = contact.gender;
      }

      if (data.ctc_seq.HasValue && data.ctc_seq.Value > 0)
      {
        var tax_contact = await cer.SelectCliTaxContactOneAsync(data.ctc_seq.Value);

        model.ctc_seq = data.ctc_seq.Value;
        model.client_etax_name = tax_contact.deposit_manager;
        model.client_etax_email = tax_contact.deposit_email.Replace(" ", ""); ;
        model.client_etax_tel = tax_contact.phone;
        model.client_etax_tel2 = tax_contact.cell_phone;
      }
      if (client_contract.grt_period > 0)
      {
        int grt = client_contract.grt_period.Value;
        if (client_contract.grt_gubun == "개월")
        {
          grt = grt * 30;

        }
        model.guarantee_day = grt;
      }
      model.construct_debut_yn = client_contract.is_construct_debut;
      model.construct_debut_per = client_contract.construct_debut_per.HasValue ? client_contract.construct_debut_per.Value : 0;



      /*
       
       */
      //model.invoice_lang = 0;
      model.billing_dt = Utils.NowKorea();
      //model.project_info = data;

      return View(model);
    }

    [HttpPost]
    public async Task<JsonResult> InvoiceContengencySubmit(InvoiceCreateModel data)
    {
      try
      {
        bool is_success_ok = false;
        string share_str = String.Empty;

        ProjectEntityRepository per = new ProjectEntityRepository();

        ClientRepository cr = new ClientRepository();
        ProjectRepository pr = new ProjectRepository();

        var project = await pr.SelectProjectOneAsync(data.p_seq);

        pjt_invoice_info info = new pjt_invoice_info()
        {
          p_seq = data.p_seq,
          invoice_lang = data.invoice_lang,
          invoice_type = data.invoice_type,
          billing_dt = data.billing_dt,
          tax_req_dt = data.tax_req_dt,
          base_dt = data.tax_req_dt,
          c_seq = data.c_seq,
          prc_seq = data.prc_seq,
          candidate_name = (data.invoice_lang == 0 ? data.candidate_name_kor : data.candidate_name_eng),
          join_dt = data.join_dt == null ? data.billing_dt : data.join_dt,
          expire_guarantee = data.expire_guarantee,
          candidate_source = data.candidate_source,
          candidate_source_txt = data.candidate_source_txt,
          candidate_position = data.candidate_position,
          candidate_position_txt = data.candidate_position_txt,

          client_name = data.client_name,
          client_ceo = data.client_ceo,
          client_addr1 = data.client_addr1,
          client_biz_code = data.client_biz_code,

          client_contact_name = data.client_contact_name,
          client_contact_email = data.client_contact_email,
          client_contact_phone = data.client_contact_tel,
          etax_name = data.client_etax_name,
          etax_email = data.client_etax_email,
          etax_phone = data.client_etax_tel,

          vat_type = data.vat_type,
          fee_rate = data.fee_rate,
          billing_type = data.billing_type,
          ann_income = data.ann_income,
          annual_income = data.annual_income,
          income_currency_cd = data.income_currency_cd,

          billing_money = data.billing_money,
          bill_currency_cd = data.bill_currency_cd,
          billing_total = data.billing_total,
          //billing_won = data.billing_won,
          billing_amt = data.billing_amt,
          billing_vat = data.billing_vat,

          invoice_title = data.invoice_title,
          invoice_contents = data.invoice_contents,
          deposit_bank_name = data.deposit_bank_name,
          deposit_bank_account = data.deposit_bank_account,
          is_open_name = data.is_open_name,
          is_open_annual_income = data.is_open_annual_income,
          is_po_no = data.is_po_no,
          remarks = data.remarks,

          create_dt = Utils.NowKorea(),
          create_user = AppIdentity.user_seq,
          modify_dt = Utils.NowKorea(),
          modify_user = AppIdentity.user_seq,

          is_deleted = 0
        };

        if (data.bill_currency_cd == "KRW")
        {
          info.billing_won = data.billing_money;
          info.billing_vat_won = data.billing_vat;
          info.billing_total_won = data.billing_total;
          info.ex_rate = 1;
        }

        List<pjt_invoice_sales> sales = new List<pjt_invoice_sales>();
        foreach (var charge in data.chargeList)
        {
          pjt_invoice_sales sale = new pjt_invoice_sales()
          {
            prc_seq = data.prc_seq,
            c_seq = data.c_seq,
            p_seq = data.p_seq,
            uv_seq = charge.uv_seq,
            ud_seq = charge.ud_seq,
            sales_rate = charge.sales_rate,
            sales_money = charge.sales_money,
            comments = charge.comments,
            create_dt = Utils.NowKorea(),
            create_user = AppIdentity.user_seq,
            modify_dt = Utils.NowKorea(),
            modify_user = AppIdentity.user_seq,
          };

          if (data.bill_currency_cd == "KRW")
          {
            sale.sales_won = charge.sales_money;
          }
          share_str += (!String.IsNullOrEmpty(share_str) ? "<br/><br/>" : "") + " - " + charge.name + " ( " + charge.sales_rate.ToString() + "% / " + Utils.ConvertMoneyToString(charge.sales_money) + ") : " + charge.comments;
          sales.Add(sale);
        }

        await per.CreatePjtInvoice(info, sales);


        int sameCnt = await pr.SelectCandidateSameInvoiceCountAsync((int)data.c_seq, project.c_seq);

        //후보자 연봉 및 최종 경력 정보 등록
        CandidateEntityRepository cer = new CandidateEntityRepository();
        ClientEntityRepository clientr = new ClientEntityRepository();

        candidate candidate = new candidate();
        client client = new client();

        //order no 가 0이고, 후보자가 같은 회사의 인보이스로 중복 발행된 건이 있으면 
        //후보자의 연봉 정보를 업데이트 하지 않는다.
        if (sameCnt == 1) // order no == 0 추가 해야 함.
        {
          candidate = await cer.SelectCandidateOneAsync((int)info.c_seq);

          client = await clientr.SelectClientOneAsync(project.c_seq);


          List<can_career> ccareer = await cer.SelectCanCareerListAsync(candidate.c_seq);

          if (ccareer != null)
          {
            if (ccareer.Count > 0)
            {
              foreach (var career in ccareer)
              {
                career.is_work = 0;
                await cer.UpdateCandidateCareerOneAsync(career);
              }
            }
          }

          //후보자 연봉 정보 바인딩
          candidate.hope_salary = data.annual_income;
          candidate.caution_type = 6;
          can_career cc = new can_career()
          {
            c_seq = candidate.c_seq,
            company_name = string.IsNullOrWhiteSpace(client.kor_name) ? client.eng_name : client.kor_name,
            join_dt = ((DateTime)info.join_dt).ToString("yyyy-MM"),
            annual_income = data.annual_income,
            division_name = (project.position_str + "/" + Utils.ReturnPositionCodeTxt(project.position_seq)),
            is_work = 1,
            p_code = project.position_seq
          };

          await cer.CreateCandidateCareerOneAsync(cc);

          CandidateRepository canrep = new CandidateRepository();
          var procMsg = await canrep.SaveCandidateTxtMsg(candidate.c_seq);
          var procMsg2 = await canrep.UpdateCandidateCareerRange(candidate.c_seq);
        }

        //후보자 영문명 저장 해야 할 경우,
        if (data.opt_is_candidate_name_update == 1)
        {
          //후보자 정보가 바인딩 되지 않았다면 조회.
          if (candidate.c_seq == 0)
            candidate = await cer.SelectCandidateOneAsync((int)info.c_seq);

          if (data.invoice_lang == 0)
            candidate.kor_name = data.candidate_name_kor;
          else if (data.invoice_lang == 1)
            candidate.eng_name = data.candidate_name_eng;
        }

        //후보자 정보가 바인딩 되어 있다면, 업데이트(연봉 및 영문명 저장에만 바인딩 되므로)
        //밖으로 빼는 이유는 연봉 저장과 영문명 저장이 동시에 있을 경우, DB 접속을 최소화 하기 위해.
        if (candidate.c_seq != 0)
          await cer.UpdateCandidateOneAsync(candidate);

        //고객사 영문명 저장 해야 할 경우,
        if (data.opt_is_client_update == 1)
        {
          //고객사 정보가 바인딩 되지 않았다면 조회.
          if (client.c_seq == 0)
            client = await clientr.SelectClientOneAsync(project.c_seq);

          if (data.invoice_lang == 0)
            client.kor_name = info.client_name;
          else if (data.invoice_lang == 1)
            client.eng_name = info.client_name;

          if (!String.IsNullOrEmpty(info.client_ceo))
            client.ceo = info.client_ceo;
          if (!String.IsNullOrEmpty(info.client_addr1))
            client.addr1 = info.client_addr1;
          if (!String.IsNullOrEmpty(info.client_biz_code))
            client.biz_code = info.client_biz_code;

          await clientr.UpdateClientOneAsync(client);
        }

        var project_stat = await per.SelectProjectOneAsync(data.p_seq);
        if (data.opt_is_success == 1)
        {
          if (project_stat != null && project_stat.pjt_status != 5)
          {
            project_stat.pjt_status = 5;
            
            if (!project_stat.close_dt.HasValue)
              project_stat.close_dt = Utils.NowKorea();

            project_stat.status_comment = "인보이스 발행으로 자동 성공 처리";
            project_stat.modify_dt = Utils.NowKorea();
            project_stat.modify_user = AppIdentity.user_seq;
            await per.UpdateProjectAsync(project_stat, "U", AppIdentity.user_seq, 1);
            is_success_ok = true;
          }

        }


        AccountRepository ar = new AccountRepository();
        ApiEntityRepository apier = new ApiEntityRepository();
        List<alarm_user> me = new List<alarm_user>();
        List<string> ToArr = new List<string>();
        List<alarm_user> aList = new List<alarm_user>();

        var myuserinfo = await ar.FindUserBySeqAsync(AppIdentity.user_seq);
        MailService mService = new MailService(myuserinfo.email_id, myuserinfo.email_pwd);

        alarm_message aMessage = new alarm_message()
        {
          href_url = "/Project/ProjectDetail?p_seq=" + data.p_seq,
          message = string.Format("[{0}] 인보이스 요청 완료 되었습니다.", project_stat.title),
          create_dt = Utils.NowKorea()
        };

        var userList = await pr.SelectProjectAmListAsync(data.p_seq);

        foreach (var user in userList)
        {
          //이미 리스트에 포함되어 있는 uv_seq라면 넘김.
          if (aList.Where(x => (x.uv_seq == user.uv_seq)).FirstOrDefault() != null)
            continue;

          alarm_user alarm = new alarm_user()
          {
            uv_seq = user.uv_seq
              ,
            is_read = 0
          };

          aList.Add(alarm);


          // 매출 대상자 만 보내기 아닐 경우 AM 추가
          if (data.opt_is_send_only_share == 0)
          {
            //담당자 이메일 추가. (최대 3건이라 건별로 조회)
            var confirmUser = await ar.FindUserBySeqAsync(user.uv_seq);
            ToArr.Add(confirmUser.email);
          }
        }

        var searcherList = await pr.SelectProjectSearcherListAsync(data.p_seq);

        foreach (var user in searcherList)
        {
          //이미 리스트에 포함되어 있는 uv_seq라면 넘김.
          if (aList.Where(x => (x.uv_seq == user.uv_seq)).FirstOrDefault() != null)
            continue;

          alarm_user alarm = new alarm_user()
          {
            uv_seq = user.uv_seq
              ,
            is_read = 0
          };

          aList.Add(alarm);

          // 매출 대상자 만 보내기 아닐 경우 SM 추가
          if (data.opt_is_send_only_share == 0)
          {
            //담당자 이메일 추가. (최대 3건이라 건별로 조회)
            var confirmUser = await ar.FindUserBySeqAsync(user.uv_seq);
            ToArr.Add(confirmUser.email);
          }
        }

        await apier.CreateAlarm(aMessage, aList);

        foreach (var user in sales)
        {
          if (user.sales_rate > 0)
          {
            //담당자 이메일 추가. (최대 3건이라 건별로 조회)
            var confirmUser = await ar.FindUserBySeqAsync(user.uv_seq.Value);
            if (!ToArr.Contains(confirmUser.email))
            {
              ToArr.Add(confirmUser.email);
            }

          }
        }
        ToArr.Add("unico@unicosearch.com");
        //ToArr.Add("planning@unicosearch.com");

        string invoice_type_str = String.Empty;
        string candidate_info_table = String.Empty;
        if (data.invoice_type == 0)
        {
          invoice_type_str = "성공 인보이스";
        }
        else if (data.invoice_type == 2)
        {
          invoice_type_str = "잔금 인보이스";
        }

        candidate_info_table = @"
<tr>
  <th rowspan='4' style='padding: 5px; vertical-align: top; border-top: 1px solid #dee2e6; border: 1px solid #dee2e6 !important; background-color: #007bff; color: #fff'>
    후보자 정보
  </th>
  <th style='padding: 5px; vertical-align: top; border-top: 1px solid #dee2e6; border: 1px solid #dee2e6 !important; background-color: #007bff; color: #fff'>이름</th>
  <td style='padding: 5px; vertical-align: top; border-top: 1px solid #dee2e6; border: 1px solid #dee2e6 !important'>" + info.candidate_name + @"</td>
  <th style='padding: 5px; vertical-align: top; border-top: 1px solid #dee2e6; border: 1px solid #dee2e6 !important; background-color: #007bff; color: #fff'>최종직급</th>
  <td style='padding: 5px; vertical-align: top; border-top: 1px solid #dee2e6; border: 1px solid #dee2e6 !important'>" + info.candidate_position_txt + @"</td>
</tr>
<tr>
  <th style='padding: 5px; vertical-align: top; border-top: 1px solid #dee2e6; border: 1px solid #dee2e6 !important; background-color: #007bff; color: #fff'>입사일</th>
  <td style='padding: 5px; vertical-align: top; border-top: 1px solid #dee2e6; border: 1px solid #dee2e6 !important'>" + Utils.ConvertDateTimeToString(info.join_dt) + @"</td>
  <th style='padding: 5px; vertical-align: top; border-top: 1px solid #dee2e6; border: 1px solid #dee2e6 !important; background-color: #007bff; color: #fff'>후보자소스</th>
  <td style='padding: 5px; vertical-align: top; border-top: 1px solid #dee2e6; border: 1px solid #dee2e6 !important'>" + info.candidate_source_txt + @"</td>
</tr>
<tr>
  <th style='padding: 5px; vertical-align: top; border-top: 1px solid #dee2e6; border: 1px solid #dee2e6 !important; background-color: #007bff; color: #fff'>연봉</th>
  <td colspan='3' style='padding: 5px; vertical-align: top; border-top: 1px solid #dee2e6; border: 1px solid #dee2e6 !important'>" + Utils.ConvertMoneyToString(info.ann_income) + " [" + info.income_currency_cd + @"]</td>
</tr>
<tr>
  <th style='padding: 5px; vertical-align: top; border-top: 1px solid #dee2e6; border: 1px solid #dee2e6 !important; background-color: #007bff; color: #fff'>후보자명 표시</th>
  <td style='padding: 5px; vertical-align: top; border-top: 1px solid #dee2e6; border: 1px solid #dee2e6 !important'>" + (info.is_open_name == 1 ? "표시" : "숨김") + @"</td>
  <th style='padding: 5px; vertical-align: top; border-top: 1px solid #dee2e6; border: 1px solid #dee2e6 !important; background-color: #007bff; color: #fff'>연봉/수수료율 표시</th>
  <td style='padding: 5px; vertical-align: top; border-top: 1px solid #dee2e6; border: 1px solid #dee2e6 !important'>" + (info.is_open_annual_income == 1 ? "표시" : "숨김") + @"</td>
</tr>
<tr>
  <td colspan='5' style='padding: 1px; vertical-align: top; border-top: 1px solid #dee2e6; border: 1px solid #dee2e6 !important'>&nbsp;</td>
</tr>
";

        //메일 Dto
        NewInvoiceCreateDto mailData = new NewInvoiceCreateDto()
        {
          ToArr = ToArr.ToArray()
            ,
          From = new System.Net.Mail.MailAddress(AppIdentity.email, AppIdentity.name)
            ,
          name = AppIdentity.name
            ,
          title = string.Format("[{0}] {1} 요청 건", project.title, invoice_type_str)
            ,
          invoicetype = invoice_type_str
            ,
          billingdt = Utils.ConvertDateTimeToString(data.billing_dt)
          ,
          taxreqdt = Utils.ConvertDateTimeToString(data.tax_req_dt)
          ,
          comment = (!String.IsNullOrEmpty(data.remarks) ? data.remarks.Replace("\r\n", "\n").Replace("\n", "<br />") : "[없음]")
            ,
          clientname = data.client_name
            ,
          ceo = data.client_ceo
            ,
          address = data.client_addr1
            ,
          bizcode = data.client_biz_code
            ,
          language = data.invoice_lang == 0 ? "국문" : "영문"
            ,
          pono = data.is_po_no == 1 ? "<span style='color:red'>필요</span>" : "불필요"
            ,
          candidateinfo = candidate_info_table
            ,
          vattype = "[" + Utils.ReturnVatTypeTxt(data.vat_type) + "]"
            ,
          feerate = (data.billing_type == 1 ? "정액" : data.fee_rate.ToString() + "%")
            ,
          billingamt = Utils.ConvertMoneyToString(data.billing_money)
            ,
          retaineramt = Utils.ConvertMoneyToString(data.retainer_amt * -1)
            ,
          currency = data.bill_currency_cd
            ,
          fee = Utils.ConvertMoneyToString(data.billing_total)
            ,
          amt = Utils.ConvertMoneyToString(data.billing_amt)
            ,
          vat = Utils.ConvertMoneyToString(data.billing_vat)
            ,
          contactname = data.client_contact_name + " " + data.client_contact_pos
            ,
          contactemail = data.client_contact_email
            ,
          contactphone = data.client_contact_tel
            ,
          etaxname = data.client_etax_name
            ,
          etaxmail = data.client_etax_email
            ,
          etaxphone = data.client_etax_tel
            ,
          invoicetitle = data.invoice_title
            ,
          invoicecontents = (!String.IsNullOrEmpty(data.invoice_contents) ? data.invoice_contents.Replace("\r\n", "\n").Replace("\n", "<br />") : "")
            ,
          bankaccount = data.deposit_bank_account
            ,
          bankname = data.deposit_bank_name
            ,
          feeshare = share_str
            ,
          url = "http://univision.unicosearch.com/Project/ProjectDetail?p_seq=" + data.p_seq
        };

        var result_my = mService.SendInvoiceCreateMail(mailData, new NewInvoiceCreateTemplete());

        /*
         * 담당자에게 별도로 전송
         */
        List<string> SupportToArr = new List<string>();

        SupportToArr.Add("narae@unicosearch.com");
        //SupportToArr.Add("claire@unicosearch.com");
        SupportToArr.Add("jhkim@unicosearch.com");

        mailData.ToArr = SupportToArr.ToArray();

        var result_support = mService.SendInvoiceCreateMail(mailData, new NewInvoiceCreateTemplete());


        return Json(new
        {
          ok = true,
          message = "인보이스 발행 완료"
        });
      }
      catch (Exception e)
      {
        return Json(new
        {
          ok = false,
          message = "인보이스 발행 중, 오류가 발생 했습니다. - " + e.Message
        });
      }

    }
    #endregion
    #region 컨설팅
    public async Task<ActionResult> InvoiceConsultingCreate(int p_seq)
    {
      ProjectRepository pr = new ProjectRepository();
      ProjectEntityRepository per = new ProjectEntityRepository();
      ClientRepository cr = new ClientRepository();
      ClientEntityRepository cer = new ClientEntityRepository();

      var data = await per.SelectProjectOneAsync(p_seq);
      //var data = await pr.SelectProjectRepInvoiceOneAsync(p_seq);
      var chargeList = await pr.SelectProjectInvoiceChargeOneAsync(p_seq);
      var client_data = await cer.SelectClientOneAsync(data.c_seq);
      var client_contract = await cer.SelectClientLatestContractOneAsync(data.c_seq);
      var client_cert_list = await cr.SelectClientFileByOnlyCseqAsync(data.c_seq);
      var client_cert = client_cert_list.Where(x => x.cf_type == "1").OrderByDescending(x => x.cf_seq).FirstOrDefault();
      var client_agr = client_cert_list.Where(x => x.cf_type == "4").OrderByDescending(x => x.cf_seq).FirstOrDefault();
      var pre_invoice = await pr.SelectProjectInvoiceOneByClientAsync(data.c_seq, 1);

      if (client_cert == null)
        client_cert = new client_file();
      if (client_agr == null)
        client_agr = new client_file();
      if (client_contract == null)
        client_contract = new client_contract();
      if (chargeList.Count == 1)
      {
        chargeList.FirstOrDefault().sales_rate = 100;
        chargeList.FirstOrDefault().sales_money = 0;
      }

      InvoiceCreateModel model = new InvoiceCreateModel()
      {
        p_seq = data.p_seq,
        pjt_type = data.pjt_type,
        pjt_position_str = data.position_str,
        billing_money = data.exp_salary,
        bill_currency_cd = data.currency_cd,
        fee_rate = 100,
        billing_type = 1,
        invoice_type = 3,
        retainer_amt = 0,
        remarks = "",

        client_seq = client_data.c_seq,
        client_name_kor = client_data.kor_name,
        client_name_eng = client_data.eng_name,
        ceo_kor = client_data.ceo,
        addr_kor = client_data.addr1,
        client_biz_code = client_data.biz_code.Replace(" ", ""),
        vat_type = 1,

        client_cert_upload_dt = client_cert.create_dt,
        client_cert_file = client_cert.file_path,
        client_cert_seq = client_cert.cf_seq,

        client_agr_upload_dt = client_agr.create_dt,
        client_agr_file = client_agr.file_path,
        client_agr_seq = client_agr.cf_seq,

        chargeList = chargeList,
        pre_invoice = pre_invoice,

        opt_is_success = 1
      };
      if (data.cc_seq.HasValue && data.cc_seq.Value > 0)
      {
        var contact = await cer.SelectCliContactOneAsync(data.cc_seq.Value);

        model.cc_seq = data.cc_seq.Value;
        model.client_contact_name = contact.name;
        model.client_contact_div = contact.division;
        model.client_contact_tel = contact.phone;
        model.client_contact_tel2 = contact.cell_phone;
        model.client_contact_email = contact.email.Replace(" ", "");
        model.client_contact_pos = contact.position;
        model.client_contact_gender = contact.gender;
      }

      if (data.ctc_seq.HasValue && data.ctc_seq.Value > 0)
      {
        var tax_contact = await cer.SelectCliTaxContactOneAsync(data.ctc_seq.Value);

        model.ctc_seq = data.ctc_seq.Value;
        model.client_etax_name = tax_contact.deposit_manager;
        model.client_etax_email = tax_contact.deposit_email.Replace(" ", "");
        model.client_etax_tel = tax_contact.phone;
        model.client_etax_tel2 = tax_contact.cell_phone;
      }

      /*
       
       */
      //model.invoice_lang = 0;
      model.billing_dt = Utils.NowKorea();

      return View(model);
    }


    [HttpPost]
    public async Task<JsonResult> InvoiceConsultingSubmit(InvoiceCreateModel data)
    {
      try
      {
        string share_str = String.Empty;

        ProjectEntityRepository per = new ProjectEntityRepository();

        //ClientRepository cr = new ClientRepository();
        ProjectRepository pr = new ProjectRepository();

        var project = await pr.SelectProjectOneAsync(data.p_seq);

        pjt_invoice_info info = new pjt_invoice_info()
        {
          p_seq = data.p_seq,
          invoice_lang = data.invoice_lang,
          invoice_type = data.invoice_type,
          billing_dt = data.billing_dt,
          tax_req_dt = data.tax_req_dt,
          base_dt = data.tax_req_dt,

          client_name = data.client_name,
          client_ceo = data.client_ceo,
          client_addr1 = data.client_addr1,
          client_biz_code = data.client_biz_code,

          client_contact_name = data.client_contact_name,
          client_contact_email = data.client_contact_email,
          client_contact_phone = data.client_contact_tel,
          etax_name = data.client_etax_name,
          etax_email = data.client_etax_email,
          etax_phone = data.client_etax_tel,

          vat_type = data.vat_type,
          fee_rate = data.fee_rate,
          billing_type = data.billing_type,

          billing_money = data.billing_money,
          bill_currency_cd = data.bill_currency_cd,
          billing_total = data.billing_total,
          //billing_won = data.billing_won,
          billing_amt = data.billing_amt,
          billing_vat = data.billing_vat,

          invoice_title = data.invoice_title,
          invoice_contents = data.invoice_contents,
          deposit_bank_name = data.deposit_bank_name,
          deposit_bank_account = data.deposit_bank_account,
          remarks = data.remarks,

          is_open_name = 0,
          is_open_annual_income = 0,
          is_po_no = data.is_po_no,
          expire_guarantee = data.expire_guarantee,

          create_dt = Utils.NowKorea(),
          create_user = AppIdentity.user_seq,
          modify_dt = Utils.NowKorea(),
          modify_user = AppIdentity.user_seq,

          candidate_name = "",

          is_deleted = 0

        };
        if (data.bill_currency_cd == "KRW")
        {
          info.billing_won = data.billing_money;
          info.billing_vat_won = data.billing_vat;
          info.billing_total_won = data.billing_total;
          info.ex_rate = 1;
        }

        List<pjt_invoice_sales> sales = new List<pjt_invoice_sales>();
        foreach (var charge in data.chargeList)
        {
          pjt_invoice_sales sale = new pjt_invoice_sales()
          {
            prc_seq = data.prc_seq,
            c_seq = data.c_seq,
            p_seq = data.p_seq,
            uv_seq = charge.uv_seq,
            ud_seq = charge.ud_seq,
            sales_rate = charge.sales_rate,
            sales_money = charge.sales_money,
            comments = charge.comments,
            create_dt = Utils.NowKorea(),
            create_user = AppIdentity.user_seq,
            modify_dt = Utils.NowKorea(),
            modify_user = AppIdentity.user_seq,
          };

          if (data.bill_currency_cd == "KRW")
          {
            sale.sales_won = charge.sales_money;
          }

          share_str += (!String.IsNullOrEmpty(share_str) ? "<br/><br/>" : "") + " - " + charge.name + " ( " + charge.sales_rate.ToString() + "% / " + Utils.ConvertMoneyToString(charge.sales_money) + ") : " + charge.comments;
          sales.Add(sale);
        }

        await per.CreatePjtInvoice(info, sales);

        if (data.opt_is_client_update == 1)
        {
          try
          {
            ClientEntityRepository cer = new ClientEntityRepository();
            var client = await cer.SelectClientOneAsync(data.client_seq);

            if (data.invoice_lang == 0)
              client.kor_name = info.client_name;
            else if (data.invoice_lang == 1)
              client.eng_name = info.client_name;

            if (!String.IsNullOrEmpty(info.client_ceo))
              client.ceo = info.client_ceo;
            if (!String.IsNullOrEmpty(info.client_addr1))
              client.addr1 = info.client_addr1;
            if (!String.IsNullOrEmpty(info.client_biz_code))
              client.biz_code = info.client_biz_code;

            await cer.UpdateClientOneAsync(client);

          }
          catch
          {
            //고객사 업데이트 실패했지만 기본 진행에는 지장없이 처리
          }
        }

        var project_stat = await per.SelectProjectOneAsync(data.p_seq);
        if (data.opt_is_success == 1)
        {
          if (project_stat != null && project_stat.pjt_status != 5)
          {
            project_stat.pjt_status = 5;
            
            if (!project_stat.close_dt.HasValue)
              project_stat.close_dt = Utils.NowKorea();

            project_stat.status_comment = "인보이스 발행으로 자동 성공 처리";
            project_stat.modify_dt = Utils.NowKorea();
            project_stat.modify_user = AppIdentity.user_seq;
            await per.UpdateProjectAsync(project_stat, "U", AppIdentity.user_seq, 1);
          }

        }

        AccountRepository ar = new AccountRepository();
        List<alarm_user> aList = new List<alarm_user>();
        List<string> ToArr = new List<string>();




        var myuserinfo = await ar.FindUserBySeqAsync(AppIdentity.user_seq);
        MailService mService = new MailService(myuserinfo.email_id, myuserinfo.email_pwd);

        alarm_message aMessage = new alarm_message()
        {
          href_url = "/Project/Project" + (project.pjt_type > 1 ? "Rep" : "") + "Detail?p_seq=" + data.p_seq,
          message = string.Format("[{0}] 컨설팅 인보이스 요청 완료 되었습니다.", project.title),
          create_dt = Utils.NowKorea()
        };
        var userList = await pr.SelectProjectAmListAsync(data.p_seq);

        foreach (var user in userList)
        {
          //이미 리스트에 포함되어 있는 uv_seq라면 넘김.
          if (aList.Where(x => (x.uv_seq == user.uv_seq)).FirstOrDefault() != null)
            continue;

          alarm_user alarm = new alarm_user()
          {
            uv_seq = user.uv_seq
              ,
            is_read = 0
          };

          aList.Add(alarm);


          // 매출 대상자 만 보내기 아닐 경우 AM 추가
          if (data.opt_is_send_only_share == 0)
          {
            //담당자 이메일 추가. (최대 3건이라 건별로 조회)
            var confirmUser = await ar.FindUserBySeqAsync(user.uv_seq);
            ToArr.Add(confirmUser.email);
          }
        }

        var searcherList = await pr.SelectProjectSearcherListAsync(data.p_seq);

        foreach (var user in searcherList)
        {
          //이미 리스트에 포함되어 있는 uv_seq라면 넘김.
          if (aList.Where(x => (x.uv_seq == user.uv_seq)).FirstOrDefault() != null)
            continue;

          alarm_user alarm = new alarm_user()
          {
            uv_seq = user.uv_seq
              ,
            is_read = 0
          };

          aList.Add(alarm);

          // 매출 대상자 만 보내기 아닐 경우 SM 추가
          if (data.opt_is_send_only_share == 0)
          {
            //담당자 이메일 추가. (최대 3건이라 건별로 조회)
            var confirmUser = await ar.FindUserBySeqAsync(user.uv_seq);
            ToArr.Add(confirmUser.email);
          }
        }


        ApiEntityRepository apier = new ApiEntityRepository();
        await apier.CreateAlarm(aMessage, aList);


        foreach (var user in sales)
        {
          if (user.sales_rate > 0)
          {
            //담당자 이메일 추가. (최대 3건이라 건별로 조회)
            var confirmUser = await ar.FindUserBySeqAsync(user.uv_seq.Value);
            if (!ToArr.Contains(confirmUser.email))
            {
              ToArr.Add(confirmUser.email);
            }

          }
        }

        ToArr.Add("unico@unicosearch.com");
        //ToArr.Add("planning@unicosearch.com");

        //메일 Dto
        NewInvoiceCreateDto mailData = new NewInvoiceCreateDto()
        {
          ToArr = ToArr.ToArray()
            ,
          From = new System.Net.Mail.MailAddress(AppIdentity.email, AppIdentity.name)
            ,
          name = AppIdentity.name
            ,
          title = string.Format("[{0}] 컨설팅 인보이스 요청 건", project.title)
            ,
          invoicetype = "컨설팅"
            ,
          billingdt = Utils.ConvertDateTimeToString(data.billing_dt)
          ,
          taxreqdt = Utils.ConvertDateTimeToString(data.tax_req_dt)
          ,
          comment = (!String.IsNullOrEmpty(data.remarks) ? data.remarks.Replace("\r\n", "\n").Replace("\n", "<br />") : "[없음]")
            ,
          clientname = data.client_name
            ,
          ceo = data.client_ceo
            ,
          address = data.client_addr1
            ,
          bizcode = data.client_biz_code
            ,
          language = data.invoice_lang == 0 ? "국문" : "영문"
            ,
          pono = data.is_po_no == 1 ? "<span style='color:red'>필요</span>" : "불필요"
            ,
          candidateinfo = ""
            ,
          vattype = "[" + Utils.ReturnVatTypeTxt(data.vat_type) + "]"
            ,
          feerate = (data.billing_type == 1 ? "정액" : data.fee_rate.ToString() + "%")
            ,
          billingamt = Utils.ConvertMoneyToString(data.billing_money)
            ,
          retaineramt = Utils.ConvertMoneyToString(data.retainer_amt * -1)
            ,
          currency = data.bill_currency_cd
            ,
          fee = Utils.ConvertMoneyToString(data.billing_total)
            ,
          amt = Utils.ConvertMoneyToString(data.billing_amt)
            ,
          vat = Utils.ConvertMoneyToString(data.billing_vat)
            ,
          contactname = data.client_contact_name + " " + data.client_contact_pos
            ,
          contactemail = data.client_contact_email
            ,
          contactphone = data.client_contact_tel
            ,
          etaxname = data.client_etax_name
            ,
          etaxmail = data.client_etax_email
            ,
          etaxphone = data.client_etax_tel
            ,
          invoicetitle = data.invoice_title
            ,
          invoicecontents = (!String.IsNullOrEmpty(data.invoice_contents) ? data.invoice_contents.Replace("\r\n", "\n").Replace("\n", "<br />") : "")
            ,
          bankaccount = data.deposit_bank_account
            ,
          bankname = data.deposit_bank_name
            ,
          feeshare = share_str
            ,
          url = "http://univision.unicosearch.com/Project/Project" + (project.pjt_type > 1 ? "Rep" : "") + "Detail?p_seq=" + data.p_seq
        };

        var result_my = mService.SendInvoiceCreateMail(mailData, new NewInvoiceCreateTemplete());

        /*
         * 담당자에게 별도로 전송
         */
        List<string> SupportToArr = new List<string>();

        SupportToArr.Add("narae@unicosearch.com");
        //SupportToArr.Add("claire@unicosearch.com");
        SupportToArr.Add("jhkim@unicosearch.com");

        mailData.ToArr = SupportToArr.ToArray();

        var result_support = mService.SendInvoiceCreateMail(mailData, new NewInvoiceCreateTemplete());

        return Json(new
        {
          ok = true
        });
      }
      catch (Exception e)
      {
        return Json(new
        {
          ok = false,
          message = "인보이스 발행 중, 오류가 발생 했습니다. - " + e.Message
        });
      }

    }
    #endregion
    #region 선수금
    public async Task<ActionResult> InvoiceRetainerCreate(int p_seq)
    {
      ProjectRepository pr = new ProjectRepository();
      ProjectEntityRepository per = new ProjectEntityRepository();
      ClientRepository cr = new ClientRepository();
      ClientEntityRepository cer = new ClientEntityRepository();

      var data = await per.SelectProjectOneAsync(p_seq);
      //var data = await pr.SelectProjectRepInvoiceOneAsync(p_seq);
      var chargeList = await pr.SelectProjectInvoiceChargeOneAsync(p_seq);
      var client_data = await cer.SelectClientOneAsync(data.c_seq);
      var client_contract = await cer.SelectClientLatestContractOneAsync(data.c_seq);
      var client_cert_list = await cr.SelectClientFileByOnlyCseqAsync(data.c_seq);
      var client_cert = client_cert_list.Where(x => x.cf_type == "1").OrderByDescending(x => x.cf_seq).FirstOrDefault();
      var client_agr = client_cert_list.Where(x => x.cf_type == "4").OrderByDescending(x => x.cf_seq).FirstOrDefault();
      var pre_invoice = await pr.SelectProjectInvoiceOneByClientAsync(data.c_seq, 1);

      if (client_cert == null)
        client_cert = new client_file();
      if (client_agr == null)
        client_agr = new client_file();
      if (client_contract == null)
        client_contract = new client_contract();
      if (chargeList.Count == 1)
      {
        chargeList.FirstOrDefault().sales_rate = 100;
        chargeList.FirstOrDefault().sales_money = 0;
      }

      InvoiceCreateModel model = new InvoiceCreateModel()
      {
        p_seq = data.p_seq,
        pjt_type = data.pjt_type,
        pjt_position_str = data.position_str,
        billing_money = 0,
        bill_currency_cd = data.currency_cd,
        fee_rate = 100,
        billing_type = 1,
        invoice_type = 1,
        retainer_amt = 0,
        remarks = "",

        client_seq = client_data.c_seq,
        client_name_kor = client_data.kor_name,
        client_name_eng = client_data.eng_name,
        ceo_kor = client_data.ceo,
        addr_kor = client_data.addr1,
        client_biz_code = client_data.biz_code.Replace(" ", ""),
        vat_type = 1,

        client_cert_upload_dt = client_cert.create_dt,
        client_cert_file = client_cert.file_path,
        client_cert_seq = client_cert.cf_seq,

        client_agr_upload_dt = client_agr.create_dt,
        client_agr_file = client_agr.file_path,
        client_agr_seq = client_agr.cf_seq,

        chargeList = chargeList,
        pre_invoice = pre_invoice,

        opt_is_success = 0
      };
      if (data.cc_seq.HasValue && data.cc_seq.Value > 0)
      {
        var contact = await cer.SelectCliContactOneAsync(data.cc_seq.Value);

        model.cc_seq = data.cc_seq.Value;
        model.client_contact_name = contact.name;
        model.client_contact_div = contact.division;
        model.client_contact_tel = contact.phone;
        model.client_contact_tel2 = contact.cell_phone;
        model.client_contact_email = contact.email.Replace(" ", "");
        model.client_contact_pos = contact.position;
        model.client_contact_gender = contact.gender;
      }

      if (data.ctc_seq.HasValue && data.ctc_seq.Value > 0)
      {
        var tax_contact = await cer.SelectCliTaxContactOneAsync(data.ctc_seq.Value);

        model.ctc_seq = data.ctc_seq.Value;
        model.client_etax_name = tax_contact.deposit_manager;
        model.client_etax_email = tax_contact.deposit_email.Replace(" ", "");
        model.client_etax_tel = tax_contact.phone;
        model.client_etax_tel2 = tax_contact.cell_phone;
      }

      /*
       
       */
      //model.invoice_lang = 0;
      model.billing_dt = Utils.NowKorea();

      return View(model);
    }

    [HttpPost]
    public async Task<JsonResult> InvoiceRetainerSubmit(InvoiceCreateModel data)
    {
      try
      {
        string share_str = String.Empty;

        ProjectEntityRepository per = new ProjectEntityRepository();

        //ClientRepository cr = new ClientRepository();
        ProjectRepository pr = new ProjectRepository();

        var project = await pr.SelectProjectOneAsync(data.p_seq);

        pjt_invoice_info info = new pjt_invoice_info()
        {
          p_seq = data.p_seq,
          invoice_lang = data.invoice_lang,
          invoice_type = data.invoice_type,
          billing_dt = data.billing_dt,
          tax_req_dt = data.tax_req_dt,
          base_dt = data.tax_req_dt,

          client_name = data.client_name,
          client_ceo = data.client_ceo,
          client_addr1 = data.client_addr1,
          client_biz_code = data.client_biz_code,

          client_contact_name = data.client_contact_name,
          client_contact_email = data.client_contact_email,
          client_contact_phone = data.client_contact_tel,
          etax_name = data.client_etax_name,
          etax_email = data.client_etax_email,
          etax_phone = data.client_etax_tel,

          vat_type = data.vat_type,
          fee_rate = data.fee_rate,
          billing_type = data.billing_type,

          billing_money = data.billing_money,
          bill_currency_cd = data.bill_currency_cd,
          billing_total = data.billing_total,
          //billing_won = data.billing_won,
          billing_amt = data.billing_amt,
          billing_vat = data.billing_vat,

          invoice_title = data.invoice_title,
          invoice_contents = data.invoice_contents,
          deposit_bank_name = data.deposit_bank_name,
          deposit_bank_account = data.deposit_bank_account,
          remarks = data.remarks,

          is_open_name = 0,
          is_open_annual_income = 0,
          is_po_no = data.is_po_no,
          expire_guarantee = data.expire_guarantee,

          create_dt = Utils.NowKorea(),
          create_user = AppIdentity.user_seq,
          modify_dt = Utils.NowKorea(),
          modify_user = AppIdentity.user_seq,

          candidate_name = "",

          is_deleted = 0

        };

        if (data.bill_currency_cd == "KRW")
        {
          info.billing_won = data.billing_money;
          info.billing_vat_won = data.billing_vat;
          info.billing_total_won = data.billing_total;
          info.ex_rate = 1;
        }

        List<pjt_invoice_sales> sales = new List<pjt_invoice_sales>();
        foreach (var charge in data.chargeList)
        {
          pjt_invoice_sales sale = new pjt_invoice_sales()
          {
            prc_seq = data.prc_seq,
            c_seq = data.c_seq,
            p_seq = data.p_seq,
            uv_seq = charge.uv_seq,
            ud_seq = charge.ud_seq,
            sales_rate = charge.sales_rate,
            sales_money = charge.sales_money,
            comments = charge.comments,
            create_dt = Utils.NowKorea(),
            create_user = AppIdentity.user_seq,
            modify_dt = Utils.NowKorea(),
            modify_user = AppIdentity.user_seq,
          };

          if (data.bill_currency_cd == "KRW")
          {
            sale.sales_won = charge.sales_money;
          }

          share_str += (!String.IsNullOrEmpty(share_str) ? "<br/><br/>" : "") + " - " + charge.name + " ( " + charge.sales_rate.ToString() + "% / " + Utils.ConvertMoneyToString(charge.sales_money) + ") : " + charge.comments;
          sales.Add(sale);
        }

        await per.CreatePjtInvoice(info, sales);

        if (data.opt_is_client_update == 1)
        {
          try
          {
            ClientEntityRepository cer = new ClientEntityRepository();
            var client = await cer.SelectClientOneAsync(data.client_seq);

            if (data.invoice_lang == 0)
              client.kor_name = info.client_name;
            else if (data.invoice_lang == 1)
              client.eng_name = info.client_name;

            if (!String.IsNullOrEmpty(info.client_ceo))
              client.ceo = info.client_ceo;
            if (!String.IsNullOrEmpty(info.client_addr1))
              client.addr1 = info.client_addr1;
            if (!String.IsNullOrEmpty(info.client_biz_code))
              client.biz_code = info.client_biz_code;

            await cer.UpdateClientOneAsync(client);

          }
          catch
          {
            //고객사 업데이트 실패했지만 기본 진행에는 지장없이 처리
          }
        }

        AccountRepository ar = new AccountRepository();
        List<alarm_user> aList = new List<alarm_user>();
        List<string> ToArr = new List<string>();




        var myuserinfo = await ar.FindUserBySeqAsync(AppIdentity.user_seq);
        MailService mService = new MailService(myuserinfo.email_id, myuserinfo.email_pwd);

        alarm_message aMessage = new alarm_message()
        {
          href_url = "/Project/Project" + (project.pjt_type > 1 ? "Rep" : "") + "Detail?p_seq=" + data.p_seq,
          message = string.Format("[{0}] 선수금 인보이스 요청 완료 되었습니다.", project.title),
          create_dt = Utils.NowKorea()
        };
        var userList = await pr.SelectProjectAmListAsync(data.p_seq);

        foreach (var user in userList)
        {
          //이미 리스트에 포함되어 있는 uv_seq라면 넘김.
          if (aList.Where(x => (x.uv_seq == user.uv_seq)).FirstOrDefault() != null)
            continue;

          alarm_user alarm = new alarm_user()
          {
            uv_seq = user.uv_seq
              ,
            is_read = 0
          };

          aList.Add(alarm);


          // 매출 대상자 만 보내기 아닐 경우 AM 추가
          if (data.opt_is_send_only_share == 0)
          {
            //담당자 이메일 추가. (최대 3건이라 건별로 조회)
            var confirmUser = await ar.FindUserBySeqAsync(user.uv_seq);
            ToArr.Add(confirmUser.email);
          }
        }

        var searcherList = await pr.SelectProjectSearcherListAsync(data.p_seq);

        foreach (var user in searcherList)
        {
          //이미 리스트에 포함되어 있는 uv_seq라면 넘김.
          if (aList.Where(x => (x.uv_seq == user.uv_seq)).FirstOrDefault() != null)
            continue;

          alarm_user alarm = new alarm_user()
          {
            uv_seq = user.uv_seq
              ,
            is_read = 0
          };

          aList.Add(alarm);

          // 매출 대상자 만 보내기 아닐 경우 SM 추가
          if (data.opt_is_send_only_share == 0)
          {
            //담당자 이메일 추가. (최대 3건이라 건별로 조회)
            var confirmUser = await ar.FindUserBySeqAsync(user.uv_seq);
            ToArr.Add(confirmUser.email);
          }
        }


        ApiEntityRepository apier = new ApiEntityRepository();
        await apier.CreateAlarm(aMessage, aList);


        foreach (var user in sales)
        {
          if (user.sales_rate > 0)
          {
            //담당자 이메일 추가. (최대 3건이라 건별로 조회)
            var confirmUser = await ar.FindUserBySeqAsync(user.uv_seq.Value);
            if (!ToArr.Contains(confirmUser.email))
            {
              ToArr.Add(confirmUser.email);
            }

          }
        }

        ToArr.Add("unico@unicosearch.com");
        //ToArr.Add("planning@unicosearch.com");

        //메일 Dto
        NewInvoiceCreateDto mailData = new NewInvoiceCreateDto()
        {
          ToArr = ToArr.ToArray()
            ,
          From = new System.Net.Mail.MailAddress(AppIdentity.email, AppIdentity.name)
            ,
          name = AppIdentity.name
            ,
          title = string.Format("[{0}] 선수금 인보이스 요청 건", project.title)
            ,
          invoicetype = "선수금"
            ,
          billingdt = Utils.ConvertDateTimeToString(data.billing_dt)
          ,
          taxreqdt = Utils.ConvertDateTimeToString(data.tax_req_dt)
          ,
          comment = (!String.IsNullOrEmpty(data.remarks) ? data.remarks.Replace("\r\n", "\n").Replace("\n", "<br />") : "[없음]")
            ,
          clientname = data.client_name
            ,
          ceo = data.client_ceo
            ,
          address = data.client_addr1
            ,
          bizcode = data.client_biz_code
            ,
          language = data.invoice_lang == 0 ? "국문" : "영문"
            ,
          pono = data.is_po_no == 1 ? "<span style='color:red'>필요</span>" : "불필요"
            ,
          candidateinfo = ""
            ,
          vattype = "[" + Utils.ReturnVatTypeTxt(data.vat_type) + "]"
            ,
          feerate = (data.billing_type == 1 ? "정액" : data.fee_rate.ToString() + "%")
            ,
          billingamt = Utils.ConvertMoneyToString(data.billing_money)
            ,
          retaineramt = Utils.ConvertMoneyToString(data.retainer_amt * -1)
            ,
          currency = data.bill_currency_cd
            ,
          fee = Utils.ConvertMoneyToString(data.billing_total)
            ,
          amt = Utils.ConvertMoneyToString(data.billing_amt)
            ,
          vat = Utils.ConvertMoneyToString(data.billing_vat)
            ,
          contactname = data.client_contact_name + " " + data.client_contact_pos
            ,
          contactemail = data.client_contact_email
            ,
          contactphone = data.client_contact_tel
            ,
          etaxname = data.client_etax_name
            ,
          etaxmail = data.client_etax_email
            ,
          etaxphone = data.client_etax_tel
            ,
          invoicetitle = data.invoice_title
            ,
          invoicecontents = (!String.IsNullOrEmpty(data.invoice_contents) ? data.invoice_contents.Replace("\r\n", "\n").Replace("\n", "<br />") : "")
            ,
          bankaccount = data.deposit_bank_account
            ,
          bankname = data.deposit_bank_name
            ,
          feeshare = share_str
            ,
          url = "http://univision.unicosearch.com/Project/Project" + (project.pjt_type > 1 ? "Rep" : "") + "Detail?p_seq=" + data.p_seq
        };

        var result_my = mService.SendInvoiceCreateMail(mailData, new NewInvoiceCreateTemplete());

        /*
         * 담당자에게 별도로 전송
         */
        List<string> SupportToArr = new List<string>();

        SupportToArr.Add("narae@unicosearch.com");
        //SupportToArr.Add("claire@unicosearch.com");
        SupportToArr.Add("jhkim@unicosearch.com");

        mailData.ToArr = SupportToArr.ToArray();

        var result_support = mService.SendInvoiceCreateMail(mailData, new NewInvoiceCreateTemplete());

        return Json(new
        {
          ok = true
        });
      }
      catch (Exception e)
      {
        return Json(new
        {
          ok = false,
          message = "인보이스 발행 중, 오류가 발생 했습니다. - " + e.Message
        });
      }

    }
    #endregion

    #region 취소(전액환불)
    public async Task<ActionResult> InvoiceCancelCreate(int p_seq, int pre_pii = 0)
    {
      ProjectRepository pr = new ProjectRepository();
      InvoiceRepository ir = new InvoiceRepository();
      ProjectEntityRepository per = new ProjectEntityRepository();
      ClientRepository cr = new ClientRepository();
      ClientEntityRepository cer = new ClientEntityRepository();

      var data = await per.SelectProjectOneAsync(p_seq);
      //var data = await pr.SelectProjectRepInvoiceOneAsync(p_seq);
      var chargeList = await pr.SelectProjectInvoiceChargeOneAsync(p_seq);
      var client_data = await cer.SelectClientOneAsync(data.c_seq);
      var client_contract = await cer.SelectClientLatestContractOneAsync(data.c_seq);
      var pre_invoice_list = await ir.SelectInvoiceListAsyncByProject(p_seq);
      var client_cert_list = await cr.SelectClientFileByOnlyCseqAsync(data.c_seq);
      var client_cert = client_cert_list.Where(x => x.cf_type == "1").OrderByDescending(x => x.cf_seq).FirstOrDefault();
      var client_agr = client_cert_list.Where(x => x.cf_type == "4").OrderByDescending(x => x.cf_seq).FirstOrDefault();
      var pre_invoice = await pr.SelectProjectInvoiceOneByClientAsync(data.c_seq, 5);
      if (pre_invoice == null)
      {
        pre_invoice = await pr.SelectProjectInvoiceOneByClientAsync(data.c_seq, 4);
      }
      if (pre_invoice == null)
      {
        pre_invoice = await pr.SelectProjectInvoiceOneByClientAsync(data.c_seq, 0);
      }


      if (client_cert == null)
        client_cert = new client_file();
      if (client_agr == null)
        client_agr = new client_file();
      if (pre_invoice_list == null)
        pre_invoice_list = new List<pjt_invoice_info>();
      if (client_contract == null)
        client_contract = new client_contract();


      InvoiceCreateModel model = new InvoiceCreateModel()
      {
        p_seq = data.p_seq,
        pjt_type = data.pjt_type,
        pre_pii = pre_pii,
        billing_money = 0,
        bill_currency_cd = data.currency_cd,
        fee_rate = 100,
        billing_type = 1,
        invoice_type = 5,
        remarks = "",
        retainer_amt = 0,
        client_seq = client_data.c_seq,
        client_name_kor = client_data.kor_name,
        client_name_eng = client_data.eng_name,
        ceo_kor = client_data.ceo,
        addr_kor = client_data.addr1,
        client_biz_code = client_data.biz_code.Replace(" ", ""),
        vat_type = client_contract.vat_type,
        client_cert_upload_dt = client_cert.create_dt,
        client_cert_file = client_cert.file_path,
        client_cert_seq = client_cert.cf_seq,
        client_agr_upload_dt = client_agr.create_dt,
        client_agr_file = client_agr.file_path,
        client_agr_seq = client_agr.cf_seq,
        is_po_no = 0,
        pre_invoice_list = pre_invoice_list,
        pre_invoice = pre_invoice
      };
      if (data.cc_seq.HasValue && data.cc_seq.Value > 0)
      {
        var contact = await cer.SelectCliContactOneAsync(data.cc_seq.Value);

        model.cc_seq = data.cc_seq.Value;
        model.client_contact_name = contact.name;
        model.client_contact_div = contact.division;
        model.client_contact_tel = contact.phone;
        model.client_contact_tel2 = contact.cell_phone;
        model.client_contact_email = contact.email.Replace(" ", "");
        model.client_contact_pos = contact.position;
        model.client_contact_gender = contact.gender;
      }

      if (data.ctc_seq.HasValue && data.ctc_seq.Value > 0)
      {
        var tax_contact = await cer.SelectCliTaxContactOneAsync(data.ctc_seq.Value);

        model.ctc_seq = data.ctc_seq.Value;
        model.client_etax_name = tax_contact.deposit_manager;
        model.client_etax_email = tax_contact.deposit_email.Replace(" ", "");
        model.client_etax_tel = tax_contact.phone;
        model.client_etax_tel2 = tax_contact.cell_phone;
      }
      if (client_contract.grt_period > 0)
      {
        int grt = client_contract.grt_period.Value;
        if (client_contract.grt_gubun == "개월")
        {
          grt = grt * 30;

        }
        model.guarantee_day = grt;
      }
      model.construct_debut_yn = client_contract.is_construct_debut;
      model.construct_debut_per = client_contract.construct_debut_per.HasValue ? client_contract.construct_debut_per.Value : 0;
      /*
       
       */
      //model.invoice_lang = 0;
      model.billing_dt = Utils.NowKorea();
      //model.project_info = data;

      return View(model);
    }

    public async Task<ActionResult> InvoiceRefundCreate(int p_seq, int pre_pii = 0 )
    {
      ProjectRepository pr = new ProjectRepository();
      InvoiceRepository ir = new InvoiceRepository();
      ProjectEntityRepository per = new ProjectEntityRepository();
      ClientRepository cr = new ClientRepository();
      ClientEntityRepository cer = new ClientEntityRepository();

      var data = await per.SelectProjectOneAsync(p_seq);
      //var data = await pr.SelectProjectRepInvoiceOneAsync(p_seq);
      var chargeList = await pr.SelectProjectInvoiceChargeOneAsync(p_seq);
      var client_data = await cer.SelectClientOneAsync(data.c_seq);
      var client_contract = await cer.SelectClientLatestContractOneAsync(data.c_seq);
      var pre_invoice_list = await ir.SelectInvoiceListAsyncByProject(p_seq);
      var client_cert_list = await cr.SelectClientFileByOnlyCseqAsync(data.c_seq);
      var client_cert = client_cert_list.Where(x => x.cf_type == "1").OrderByDescending(x => x.cf_seq).FirstOrDefault();
      var client_agr = client_cert_list.Where(x => x.cf_type == "4").OrderByDescending(x => x.cf_seq).FirstOrDefault();
      var pre_invoice = await pr.SelectProjectInvoiceOneByClientAsync(data.c_seq, 5);
      if (pre_invoice == null)
      {
        pre_invoice = await pr.SelectProjectInvoiceOneByClientAsync(data.c_seq, 4);
      }
      if (pre_invoice == null)
      {
        pre_invoice = await pr.SelectProjectInvoiceOneByClientAsync(data.c_seq, 0);
      }


      if (client_cert == null)
        client_cert = new client_file();
      if (client_agr == null)
        client_agr = new client_file();
      if (pre_invoice_list == null)
        pre_invoice_list = new List<pjt_invoice_info>();
      if (client_contract == null)
        client_contract = new client_contract();


      InvoiceCreateModel model = new InvoiceCreateModel()
      {
        p_seq = data.p_seq,
        pjt_type = data.pjt_type,
        pre_pii = pre_pii,
        billing_money = 0,
        bill_currency_cd = data.currency_cd,
        fee_rate = 100,
        billing_type = 1,
        invoice_type = 4,
        remarks = "",
        retainer_amt = 0,
        client_seq = client_data.c_seq,
        client_name_kor = client_data.kor_name,
        client_name_eng = client_data.eng_name,
        ceo_kor = client_data.ceo,
        addr_kor = client_data.addr1,
        client_biz_code = client_data.biz_code.Replace(" ", ""),
        vat_type = client_contract.vat_type,
        client_cert_upload_dt = client_cert.create_dt,
        client_cert_file = client_cert.file_path,
        client_cert_seq = client_cert.cf_seq,
        client_agr_upload_dt = client_agr.create_dt,
        client_agr_file = client_agr.file_path,
        client_agr_seq = client_agr.cf_seq,
        is_po_no = 0,
        pre_invoice_list = pre_invoice_list,
        pre_invoice = pre_invoice
      };
      if (data.cc_seq.HasValue && data.cc_seq.Value > 0)
      {
        var contact = await cer.SelectCliContactOneAsync(data.cc_seq.Value);

        model.cc_seq = data.cc_seq.Value;
        model.client_contact_name = contact.name;
        model.client_contact_div = contact.division;
        model.client_contact_tel = contact.phone;
        model.client_contact_tel2 = contact.cell_phone;
        model.client_contact_email = contact.email.Replace(" ", "");
        model.client_contact_pos = contact.position;
        model.client_contact_gender = contact.gender;
      }

      if (data.ctc_seq.HasValue && data.ctc_seq.Value > 0)
      {
        var tax_contact = await cer.SelectCliTaxContactOneAsync(data.ctc_seq.Value);

        model.ctc_seq = data.ctc_seq.Value;
        model.client_etax_name = tax_contact.deposit_manager;
        model.client_etax_email = tax_contact.deposit_email.Replace(" ", "");
        model.client_etax_tel = tax_contact.phone;
        model.client_etax_tel2 = tax_contact.cell_phone;
      }
      if (client_contract.grt_period > 0)
      {
        int grt = client_contract.grt_period.Value;
        if (client_contract.grt_gubun == "개월")
        {
          grt = grt * 30;

        }
        model.guarantee_day = grt;
      }
      model.construct_debut_yn = client_contract.is_construct_debut;
      model.construct_debut_per = client_contract.construct_debut_per.HasValue ? client_contract.construct_debut_per.Value : 0;
      /*
       
       */
      //model.invoice_lang = 0;
      model.billing_dt = Utils.NowKorea();
      //model.project_info = data;

      return View(model);
    }

    [HttpPost]
    public async Task<JsonResult> InvoiceRefundSubmit(InvoiceCreateModel data)
    {
      try
      {
        string share_str = String.Empty;

        ProjectEntityRepository per = new ProjectEntityRepository();

        //ClientRepository cr = new ClientRepository();
        ProjectRepository pr = new ProjectRepository();

        var project = await pr.SelectProjectOneAsync(data.p_seq);

        pjt_invoice_info info = new pjt_invoice_info()
        {
          p_seq = data.p_seq,
          pre_pii = data.pre_pii,
          invoice_lang = data.invoice_lang,
          invoice_type = data.invoice_type,
          billing_dt = data.billing_dt,
          tax_req_dt = data.tax_req_dt,
          base_dt = data.tax_req_dt,
          prc_seq = data.prc_seq,
          join_dt = data.join_dt,
          c_seq = data.c_seq,
          client_name = data.client_name,
          client_ceo = data.client_ceo,
          client_addr1 = data.client_addr1,
          client_biz_code = data.client_biz_code,

          client_contact_name = data.client_contact_name,
          client_contact_email = data.client_contact_email,
          client_contact_phone = data.client_contact_tel,
          etax_name = data.client_etax_name,
          etax_email = data.client_etax_email,
          etax_phone = data.client_etax_tel,

          candidate_name = data.candidate_name_kor,
          expire_guarantee = data.expire_guarantee,

          vat_type = data.vat_type,
          fee_rate = data.fee_rate,
          billing_type = data.billing_type,

          billing_money = data.billing_money,
          bill_currency_cd = data.bill_currency_cd,
          billing_total = data.billing_total,
          //billing_won = data.billing_won,
          billing_amt = data.billing_amt,
          billing_vat = data.billing_vat,

          invoice_title = "",
          invoice_contents = "",
          deposit_bank_name = "",
          deposit_bank_account = "",
          remarks = data.remarks,

          is_open_name = 0,
          is_open_annual_income = 0,
          is_po_no = data.is_po_no,
          

          create_dt = Utils.NowKorea(),
          create_user = AppIdentity.user_seq,
          modify_dt = Utils.NowKorea(),
          modify_user = AppIdentity.user_seq,

          

          is_deleted = 0

        };
        if (data.bill_currency_cd == "KRW")
        {
          info.billing_won = data.billing_money;
          info.billing_vat_won = data.billing_vat;
          info.billing_total_won = data.billing_total;
          info.ex_rate = 1;
        }

        List<pjt_invoice_sales> sales = new List<pjt_invoice_sales>();
        foreach (var charge in data.chargeList)
        {
          pjt_invoice_sales sale = new pjt_invoice_sales()
          {
            prc_seq = data.prc_seq,
            c_seq = data.c_seq,
            p_seq = data.p_seq,
            uv_seq = charge.uv_seq,
            ud_seq = charge.ud_seq,
            sales_rate = charge.sales_rate,
            sales_money = charge.sales_money,
            comments = charge.comments,
            create_dt = Utils.NowKorea(),
            create_user = AppIdentity.user_seq,
            modify_dt = Utils.NowKorea(),
            modify_user = AppIdentity.user_seq,
          };

          if (data.bill_currency_cd == "KRW")
          {
            sale.sales_won = charge.sales_money;
          }

          share_str += (!String.IsNullOrEmpty(share_str) ? "<br/><br/>" : "") + " - " + charge.name + " ( " + charge.sales_rate.ToString() + "% / " + Utils.ConvertMoneyToString(charge.sales_money) + ") : " + charge.comments;
          sales.Add(sale);
        }

        await per.CreatePjtInvoice(info, sales);

        if (data.opt_is_client_update == 1)
        {
          try
          {
            ClientEntityRepository cer = new ClientEntityRepository();
            var client = await cer.SelectClientOneAsync(data.client_seq);

            if (data.invoice_lang == 0)
              client.kor_name = info.client_name;
            else if (data.invoice_lang == 1)
              client.eng_name = info.client_name;

            if (!String.IsNullOrEmpty(info.client_ceo))
              client.ceo = info.client_ceo;
            if (!String.IsNullOrEmpty(info.client_addr1))
              client.addr1 = info.client_addr1;
            if (!String.IsNullOrEmpty(info.client_biz_code))
              client.biz_code = info.client_biz_code;

            await cer.UpdateClientOneAsync(client);

          }
          catch
          {
            //고객사 업데이트 실패했지만 기본 진행에는 지장없이 처리
          }
        }

        AccountRepository ar = new AccountRepository();
        List<alarm_user> aList = new List<alarm_user>();
        List<string> ToArr = new List<string>();




        var myuserinfo = await ar.FindUserBySeqAsync(AppIdentity.user_seq);
        MailService mService = new MailService(myuserinfo.email_id, myuserinfo.email_pwd);

        alarm_message aMessage = new alarm_message()
        {
          href_url = "/Project/Project" + (project.pjt_type > 1 ? "Rep" : "") + "Detail?p_seq=" + data.p_seq,
          message = string.Format("[{0}] 환불 인보이스 요청 완료 되었습니다.", project.title),
          create_dt = Utils.NowKorea()
        };
        var userList = await pr.SelectProjectAmListAsync(data.p_seq);

        foreach (var user in userList)
        {
          //이미 리스트에 포함되어 있는 uv_seq라면 넘김.
          if (aList.Where(x => (x.uv_seq == user.uv_seq)).FirstOrDefault() != null)
            continue;

          alarm_user alarm = new alarm_user()
          {
            uv_seq = user.uv_seq
              ,
            is_read = 0
          };

          aList.Add(alarm);


          // 매출 대상자 만 보내기 아닐 경우 AM 추가
          if (data.opt_is_send_only_share == 0)
          {
            //담당자 이메일 추가. (최대 3건이라 건별로 조회)
            var confirmUser = await ar.FindUserBySeqAsync(user.uv_seq);
            ToArr.Add(confirmUser.email);
          }
        }

        var searcherList = await pr.SelectProjectSearcherListAsync(data.p_seq);

        foreach (var user in searcherList)
        {
          //이미 리스트에 포함되어 있는 uv_seq라면 넘김.
          if (aList.Where(x => (x.uv_seq == user.uv_seq)).FirstOrDefault() != null)
            continue;

          alarm_user alarm = new alarm_user()
          {
            uv_seq = user.uv_seq
              ,
            is_read = 0
          };

          aList.Add(alarm);

          // 매출 대상자 만 보내기 아닐 경우 SM 추가
          if (data.opt_is_send_only_share == 0)
          {
            //담당자 이메일 추가. (최대 3건이라 건별로 조회)
            var confirmUser = await ar.FindUserBySeqAsync(user.uv_seq);
            ToArr.Add(confirmUser.email);
          }
        }


        ApiEntityRepository apier = new ApiEntityRepository();
        await apier.CreateAlarm(aMessage, aList);


        foreach (var user in sales)
        {
          if (user.sales_rate > 0)
          {
            //담당자 이메일 추가. (최대 3건이라 건별로 조회)
            var confirmUser = await ar.FindUserBySeqAsync(user.uv_seq.Value);
            if (!ToArr.Contains(confirmUser.email))
            {
              ToArr.Add(confirmUser.email);
            }

          }
        }

        ToArr.Add("unico@unicosearch.com");
        //ToArr.Add("planning@unicosearch.com");

        //메일 Dto
        NewInvoiceCreateDto mailData = new NewInvoiceCreateDto()
        {
          ToArr = ToArr.ToArray()
            ,
          From = new System.Net.Mail.MailAddress(AppIdentity.email, AppIdentity.name)
            ,
          name = AppIdentity.name
            ,
          title = string.Format("[{0}] 환불 인보이스 요청 건", project.title)
            ,
          invoicetype = "환불"
            ,
          billingdt = Utils.ConvertDateTimeToString(data.billing_dt)
          ,
          taxreqdt = Utils.ConvertDateTimeToString(data.tax_req_dt)
          ,
          comment = (!String.IsNullOrEmpty(data.remarks) ? data.remarks.Replace("\r\n", "\n").Replace("\n", "<br />") : "[없음]")
            ,
          clientname = data.client_name
            ,
          ceo = data.client_ceo
            ,
          address = data.client_addr1
            ,
          bizcode = data.client_biz_code
            ,
          language = data.invoice_lang == 0 ? "국문" : "영문"
            ,
          pono = ""
            ,
          candidateinfo = ""
            ,
          vattype = "[" + Utils.ReturnVatTypeTxt(data.vat_type) + "]"
            ,
          feerate = "정액"
            ,
          billingamt = Utils.ConvertMoneyToString(data.billing_money)
            ,
          retaineramt = Utils.ConvertMoneyToString(data.retainer_amt * -1)
            ,
          currency = data.bill_currency_cd
            ,
          fee = Utils.ConvertMoneyToString(data.billing_total)
            ,
          amt = Utils.ConvertMoneyToString(data.billing_amt)
            ,
          vat = Utils.ConvertMoneyToString(data.billing_vat)
            ,
          contactname = data.client_contact_name + " " + data.client_contact_pos
            ,
          contactemail = data.client_contact_email
            ,
          contactphone = data.client_contact_tel
            ,
          etaxname = data.client_etax_name
            ,
          etaxmail = data.client_etax_email
            ,
          etaxphone = data.client_etax_tel
            ,
          invoicetitle = data.invoice_title
            ,
          invoicecontents = ""
            ,
          bankaccount = ""
            ,
          bankname = ""
            ,
          feeshare = share_str
            ,
          url = "http://univision.unicosearch.com/Project/Project" + (project.pjt_type > 1 ? "Rep" : "") + "Detail?p_seq=" + data.p_seq
        };

        var result_my = mService.SendInvoiceCreateMail(mailData, new NewInvoiceCreateTemplete());

        /*
         * 담당자에게 별도로 전송
         */
        List<string> SupportToArr = new List<string>();

        SupportToArr.Add("narae@unicosearch.com");
        //SupportToArr.Add("claire@unicosearch.com");
        SupportToArr.Add("jhkim@unicosearch.com");

        mailData.ToArr = SupportToArr.ToArray();

        var result_support = mService.SendInvoiceCreateMail(mailData, new NewInvoiceCreateTemplete());

        return Json(new
        {
          ok = true
        });
      }
      catch (Exception e)
      {
        return Json(new
        {
          ok = false,
          message = "인보이스 발행 중, 오류가 발생 했습니다. - " + e.Message
        });
      }

    }

    [HttpPost]
    public async Task<JsonResult> InvoiceCancelSubmit(InvoiceCreateModel data)
    {
      try
      {
        string share_str = String.Empty;

        ProjectEntityRepository per = new ProjectEntityRepository();

        //ClientRepository cr = new ClientRepository();
        ProjectRepository pr = new ProjectRepository();

        var project = await pr.SelectProjectOneAsync(data.p_seq);

        pjt_invoice_info info = new pjt_invoice_info()
        {
          p_seq = data.p_seq,
          pre_pii = data.pre_pii,
          invoice_lang = data.invoice_lang,
          invoice_type = data.invoice_type,
          billing_dt = data.billing_dt,
          tax_req_dt = data.tax_req_dt,
          base_dt = data.tax_req_dt,
          prc_seq = data.prc_seq,
          join_dt = data.join_dt,
          c_seq = data.c_seq,
          client_name = data.client_name,
          client_ceo = data.client_ceo,
          client_addr1 = data.client_addr1,
          client_biz_code = data.client_biz_code,

          client_contact_name = data.client_contact_name,
          client_contact_email = data.client_contact_email,
          client_contact_phone = data.client_contact_tel,
          etax_name = data.client_etax_name,
          etax_email = data.client_etax_email,
          etax_phone = data.client_etax_tel,

          candidate_name = data.candidate_name_kor,
          expire_guarantee = data.expire_guarantee,

          vat_type = data.vat_type,
          fee_rate = data.fee_rate,
          billing_type = data.billing_type,

          billing_money = data.billing_money,
          bill_currency_cd = data.bill_currency_cd,
          billing_total = data.billing_total,
          //billing_won = data.billing_won,
          billing_amt = data.billing_amt,
          billing_vat = data.billing_vat,

          invoice_title = "",
          invoice_contents = "",
          deposit_bank_name = "",
          deposit_bank_account = "",
          remarks = data.remarks,

          is_open_name = 0,
          is_open_annual_income = 0,
          is_po_no = data.is_po_no,


          create_dt = Utils.NowKorea(),
          create_user = AppIdentity.user_seq,
          modify_dt = Utils.NowKorea(),
          modify_user = AppIdentity.user_seq,



          is_deleted = 0

        };
        if (data.bill_currency_cd == "KRW")
        {
          info.billing_won = data.billing_money;
          info.billing_vat_won = data.billing_vat;
          info.billing_total_won = data.billing_total;
          info.ex_rate = 1;
        }

        List<pjt_invoice_sales> sales = new List<pjt_invoice_sales>();
        foreach (var charge in data.chargeList)
        {
          pjt_invoice_sales sale = new pjt_invoice_sales()
          {
            prc_seq = data.prc_seq,
            c_seq = data.c_seq,
            p_seq = data.p_seq,
            uv_seq = charge.uv_seq,
            ud_seq = charge.ud_seq,
            sales_rate = charge.sales_rate,
            sales_money = charge.sales_money,
            comments = charge.comments,
            create_dt = Utils.NowKorea(),
            create_user = AppIdentity.user_seq,
            modify_dt = Utils.NowKorea(),
            modify_user = AppIdentity.user_seq,
          };

          if (data.bill_currency_cd == "KRW")
          {
            sale.sales_won = charge.sales_money;
          }
          share_str += (!String.IsNullOrEmpty(share_str) ? "<br/><br/>" : "") + " - " + charge.name + " ( " + charge.sales_rate.ToString() + "% / " + Utils.ConvertMoneyToString(charge.sales_money) + ") : " + charge.comments;
          sales.Add(sale);
        }

        await per.CreatePjtInvoice(info, sales);

        if (data.opt_is_client_update == 1)
        {
          try
          {
            ClientEntityRepository cer = new ClientEntityRepository();
            var client = await cer.SelectClientOneAsync(data.client_seq);

            if (data.invoice_lang == 0)
              client.kor_name = info.client_name;
            else if (data.invoice_lang == 1)
              client.eng_name = info.client_name;

            if (!String.IsNullOrEmpty(info.client_ceo))
              client.ceo = info.client_ceo;
            if (!String.IsNullOrEmpty(info.client_addr1))
              client.addr1 = info.client_addr1;
            if (!String.IsNullOrEmpty(info.client_biz_code))
              client.biz_code = info.client_biz_code;

            await cer.UpdateClientOneAsync(client);

          }
          catch
          {
            //고객사 업데이트 실패했지만 기본 진행에는 지장없이 처리
          }
        }

        AccountRepository ar = new AccountRepository();
        List<alarm_user> aList = new List<alarm_user>();
        List<string> ToArr = new List<string>();




        var myuserinfo = await ar.FindUserBySeqAsync(AppIdentity.user_seq);
        MailService mService = new MailService(myuserinfo.email_id, myuserinfo.email_pwd);

        alarm_message aMessage = new alarm_message()
        {
          href_url = "/Project/Project" + (project.pjt_type > 1 ? "Rep" : "") + "Detail?p_seq=" + data.p_seq,
          message = string.Format("[{0}] 취소 인보이스 요청 완료 되었습니다.", project.title),
          create_dt = Utils.NowKorea()
        };
        var userList = await pr.SelectProjectAmListAsync(data.p_seq);

        foreach (var user in userList)
        {
          //이미 리스트에 포함되어 있는 uv_seq라면 넘김.
          if (aList.Where(x => (x.uv_seq == user.uv_seq)).FirstOrDefault() != null)
            continue;

          alarm_user alarm = new alarm_user()
          {
            uv_seq = user.uv_seq
              ,
            is_read = 0
          };

          aList.Add(alarm);


          // 매출 대상자 만 보내기 아닐 경우 AM 추가
          if (data.opt_is_send_only_share == 0)
          {
            //담당자 이메일 추가. (최대 3건이라 건별로 조회)
            var confirmUser = await ar.FindUserBySeqAsync(user.uv_seq);
            ToArr.Add(confirmUser.email);
          }
        }

        var searcherList = await pr.SelectProjectSearcherListAsync(data.p_seq);

        foreach (var user in searcherList)
        {
          //이미 리스트에 포함되어 있는 uv_seq라면 넘김.
          if (aList.Where(x => (x.uv_seq == user.uv_seq)).FirstOrDefault() != null)
            continue;

          alarm_user alarm = new alarm_user()
          {
            uv_seq = user.uv_seq
              ,
            is_read = 0
          };

          aList.Add(alarm);

          // 매출 대상자 만 보내기 아닐 경우 SM 추가
          if (data.opt_is_send_only_share == 0)
          {
            //담당자 이메일 추가. (최대 3건이라 건별로 조회)
            var confirmUser = await ar.FindUserBySeqAsync(user.uv_seq);
            ToArr.Add(confirmUser.email);
          }
        }


        ApiEntityRepository apier = new ApiEntityRepository();
        await apier.CreateAlarm(aMessage, aList);


        foreach (var user in sales)
        {
          if (user.sales_rate > 0)
          {
            //담당자 이메일 추가. (최대 3건이라 건별로 조회)
            var confirmUser = await ar.FindUserBySeqAsync(user.uv_seq.Value);
            if (!ToArr.Contains(confirmUser.email))
            {
              ToArr.Add(confirmUser.email);
            }

          }
        }

        ToArr.Add("unico@unicosearch.com");
        //ToArr.Add("planning@unicosearch.com");

        //메일 Dto
        NewInvoiceCreateDto mailData = new NewInvoiceCreateDto()
        {
          ToArr = ToArr.ToArray()
            ,
          From = new System.Net.Mail.MailAddress(AppIdentity.email, AppIdentity.name)
            ,
          name = AppIdentity.name
            ,
          title = string.Format("[{0}] 취소 인보이스 요청 건", project.title)
            ,
          invoicetype = "취소"
            ,
          billingdt = Utils.ConvertDateTimeToString(data.billing_dt)
          ,
          taxreqdt = Utils.ConvertDateTimeToString(data.tax_req_dt)
          ,
          comment = (!String.IsNullOrEmpty(data.remarks) ? data.remarks.Replace("\r\n", "\n").Replace("\n", "<br />") : "[없음]")
            ,
          clientname = data.client_name
            ,
          ceo = data.client_ceo
            ,
          address = data.client_addr1
            ,
          bizcode = data.client_biz_code
            ,
          language = data.invoice_lang == 0 ? "국문" : "영문"
            ,
          pono = ""
            ,
          candidateinfo = ""
            ,
          vattype = "[" + Utils.ReturnVatTypeTxt(data.vat_type) + "]"
            ,
          feerate = "정액"
            ,
          billingamt = Utils.ConvertMoneyToString(data.billing_money)
            ,
          retaineramt = Utils.ConvertMoneyToString(data.retainer_amt * -1)
            ,
          currency = data.bill_currency_cd
            ,
          fee = Utils.ConvertMoneyToString(data.billing_total)
            ,
          amt = Utils.ConvertMoneyToString(data.billing_amt)
            ,
          vat = Utils.ConvertMoneyToString(data.billing_vat)
            ,
          contactname = data.client_contact_name + " " + data.client_contact_pos
            ,
          contactemail = data.client_contact_email
            ,
          contactphone = data.client_contact_tel
            ,
          etaxname = data.client_etax_name
            ,
          etaxmail = data.client_etax_email
            ,
          etaxphone = data.client_etax_tel
            ,
          invoicetitle = data.invoice_title
            ,
          invoicecontents = ""
            ,
          bankaccount = ""
            ,
          bankname = ""
            ,
          feeshare = share_str
            ,
          url = "http://univision.unicosearch.com/Project/Project" + (project.pjt_type > 1 ? "Rep" : "") + "Detail?p_seq=" + data.p_seq
        };

        var result_my = mService.SendInvoiceCreateMail(mailData, new NewInvoiceCreateTemplete());

        /*
         * 담당자에게 별도로 전송
         */
        List<string> SupportToArr = new List<string>();

        SupportToArr.Add("narae@unicosearch.com");
        //SupportToArr.Add("claire@unicosearch.com");
        SupportToArr.Add("jhkim@unicosearch.com");

        mailData.ToArr = SupportToArr.ToArray();

        var result_support = mService.SendInvoiceCreateMail(mailData, new NewInvoiceCreateTemplete());

        return Json(new
        {
          ok = true
        });
      }
      catch (Exception e)
      {
        return Json(new
        {
          ok = false,
          message = "인보이스 발행 중, 오류가 발생 했습니다. - " + e.Message
        });
      }

    }
    #endregion
    #endregion
    [HttpPost]
    public async Task<ActionResult> PartialPreInvoiceShareList(int pii_seq = 0)
    {
      InvoiceRepository ir = new InvoiceRepository();
      
      var model = await ir.SelectInvoiceSalesListAsync(pii_seq);
       

      return View(model);
    }
    

    [HttpPost]
    public async Task<JsonResult> TempInvoiceCreate(InvoiceCreateModel data)
    {
      try
      {
        using (HttpClient client = new HttpClient())
        {
          string url = "http://10.1.2.150:9999/api/invoice/temp_invoice_2025_file_proc.asp";

          //string json = JsonConvert.SerializeObject(data);

          //StringContent content = new StringContent(json, Encoding.UTF8, "application/json");

          var values = new Dictionary<string, string>
          {
            { "p_seq", data.p_seq.ToString() },
            { "c_seq", data.c_seq.ToString() },
            { "billing_money", data.billing_money.ToString() },
            { "bill_currency_cd", data.bill_currency_cd.ToString() },
            { "invoice_type", data.invoice_type.ToString() },
            { "invoice_lang", data.invoice_lang.ToString() },
            { "vat_type", data.vat_type.ToString() },

            { "is_open_name", data.is_open_name.ToString() },
            { "is_open_annual_income", data.is_open_annual_income.ToString() },
            { "candidate_name", data.candidate_name_kor.ToString() },
            { "join_dt", data.join_dt.ToString() },

            { "client_name", data.client_name.ToString() },
            { "client_ceo", data.client_ceo.ToString() },
            { "client_addr1", data.client_addr1.ToString() },
            { "client_biz_code", data.client_biz_code.ToString() },

            { "ann_income", data.ann_income.ToString() },
            { "retainer_amt", data.retainer_amt.ToString() },

            { "invoice_title", data.invoice_title.ToString() },
            { "invoice_contents", data.invoice_contents.ToString() },
            { "deposit_bank_name", data.deposit_bank_name.ToString() },
            { "deposit_bank_account", data.deposit_bank_account.ToString() }
          };
          string responseBody = "";
          var content = new FormUrlEncodedContent(values);
          try
          {
            // Send a POST request to the specified URL with the JSON content
            HttpResponseMessage response = await client.PostAsync(url, content);

            // Ensure the request was successful
            response.EnsureSuccessStatusCode();

            // Read the response content as a string
            responseBody = await response.Content.ReadAsStringAsync();

            JObject jsonResponse = JObject.Parse(responseBody);

            // Output the response body
            //Console.WriteLine(responseBody);
            return Json(new
            {
              ok = (bool)jsonResponse["rtn"],
              file_url = jsonResponse["file_url"].ToString()
            });
          }
          catch (HttpRequestException e)
          {
            // Handle any HTTP request exceptions
            throw new Exception($"Request error: {e.Message}");
          }



        }
      }
      catch (Exception e)
      {
        return Json(new
        {
          ok = false,
          message = "알 수 없는 오류가 발생 했습니다. " + e.Message
        });
      }
    }
  }
}