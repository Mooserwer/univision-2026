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

namespace Univision.Main.Controllers
{
    public class LogController : Controller
    {
        public async Task<ActionResult> ClientDetailSummary(int c_seq = 0)
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

        public async Task<ActionResult> ClientDetailLog(int c_seq = 0, int log_seq = 0)
        {
            try
            {
                ClientLogRepository log = new ClientLogRepository();
                
                List<client_log> listSummary = await log.ListSummaryClientLog(c_seq);
                client_log data = null;
                List<client_manager_log> manager_log = null;

                if(log_seq > 0)
                {
                    data = await log.SelectClientLog(c_seq, log_seq);
                    manager_log = await log.ListClientManagerLog(c_seq, log_seq);
                }
                
                ViewBag.c_seq = c_seq;
                ViewBag.log_seq = log_seq;
                ViewBag.select = listSummary;
                ViewBag.manager_log = manager_log;
                return View(data);
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        public ActionResult ListClientActivity(int c_seq = 0, int cal_seq = 0, int page = 1)
        {
            try
            {
                ClientLogRepository log = new ClientLogRepository();
                int totalCount = 0;

                int pageSize = AppPaging.PageLength10;
                List<client_activity_log_log> list = log.ListClientActivity(c_seq, cal_seq, (page - 1) * pageSize, pageSize, out totalCount);
                PagingInfo paging = new PagingInfo()
                {
                    CurrentPage = page,
                    ItemsPerPage = pageSize,
                    TotalItems = totalCount
                };
                ViewBag.paging = paging;
                ViewBag.c_seq = c_seq;
                ViewBag.cal_seq = cal_seq;
                return View(list);
            }
            catch (Exception)
            {

                throw;
            }
        }

        public async Task<ActionResult> ActivityDetailLog(int log_seq)
        {
            try
            {
                ClientLogRepository cr = new ClientLogRepository();
                var log_data = await cr.ActivityDetailLog(log_seq);
                return View(log_data);
            }
            catch (Exception)
            {

                throw;
            }
            
        }

        public ActionResult ListClientContactLog(int c_seq = 0, int cc_seq = 0, int page = 1)
        {
            try
            {
                ClientLogRepository log = new ClientLogRepository();
                int totalCount = 0;

                int pageSize = AppPaging.PageLength10;
                List<client_contact_log> list = log.ListClientContactLog(c_seq, cc_seq, (page - 1) * pageSize, pageSize, out totalCount);
                PagingInfo paging = new PagingInfo()
                {
                    CurrentPage = page,
                    ItemsPerPage = pageSize,
                    TotalItems = totalCount
                };
                ViewBag.paging = paging;
                ViewBag.c_seq = c_seq;
                ViewBag.cc_seq = cc_seq;
                return View(list);
            }
            catch (Exception)
            {

                throw;
            }
        }

        public async Task<ActionResult> ClientContactLog(int log_seq = 0)
        {
            try
            {
                ClientLogRepository cr = new ClientLogRepository();
                var data = await cr.ClientContactDetailLog(log_seq);

                return View(data);
            }
            catch (Exception)
            {

                throw;
            }
        }

        public ActionResult ListClientTaxLog(int c_seq = 0, int ctc_seq = 0, int page = 1)
        {
            try
            {
                int totalCount = 0;

                int pageSize = AppPaging.PageLength10;

                ClientLogRepository cr = new ClientLogRepository();

                var list = cr.ListClientTaxLog(c_seq, ctc_seq, (page - 1) * pageSize, pageSize, out totalCount);

                var paging = new PagingInfo()
                {
                    CurrentPage = page,
                    ItemsPerPage = pageSize,
                    TotalItems = totalCount
                };
                ViewBag.c_seq = c_seq;
                ViewBag.ctc_seq = ctc_seq;
                ViewBag.paging = paging;
                return PartialView(list);
            }
            catch (Exception)
            {

                throw;
            }
            
        }

        public async Task<ActionResult> ClientTaxContactLog(int log_seq = 0)
        {
            try
            {
                ClientLogRepository cr = new ClientLogRepository();
                var data = await cr.ClientTaxContactLog(log_seq);

                return View(data);
            }
            catch (Exception)
            {

                throw;
            }           
        }

        public ActionResult ListClientContractLog(int c_seq, int page = 1)
        {
            try
            {
                //int totalCount = 0;

                //int pageSize = AppPaging.PageLength10;

                //ClientLogRepository cr = new ClientLogRepository();

                //var list = cr.ListClientContractLog(c_seq, (page - 1) * pageSize, pageSize, out totalCount);

                //var paging = new PagingInfo()
                //{
                //    CurrentPage = page,
                //    ItemsPerPage = pageSize,
                //    TotalItems = totalCount
                //};
                //ViewBag.c_seq = c_seq;
                //ViewBag.paging = paging;
                return PartialView();
            }
            catch (Exception)
            {

                throw;
            }

        }

        public async Task<ActionResult> InorderLog(int i_seq = 0,  int log_seq = 0)
        {
            try
            {
                ProjectLogRepository log = new ProjectLogRepository();
                var listSummary = await log.ListInorderLogSummary(i_seq);
                inorder_log data = null;
                if(log_seq > 0 )
                    data = await log.SelectInorderLog(log_seq);

                ViewBag.i_seq = i_seq;
                ViewBag.log_seq = log_seq;

                ViewBag.select = listSummary;

                return View(data);
            }
            catch (Exception)
            {

                throw;
            }
        }

        public async Task<ActionResult> ProjectLog(int p_seq = 0, int log_seq = 0)
        {
            try
            {
                ProjectLogRepository log = new ProjectLogRepository();
                var listSummary = await log.ListProjectLogSummary(p_seq);
                project_log data = null;
                List<pjt_director_log> amList = null;
                List<pjt_manager_log> searcherList = null;
                List<pjt_place_log> placeList = null;
                List<pjt_keyword_log> keywordList = null;
                if (log_seq > 0)
                {
                    data = await log.SelectProjectLog(log_seq);
                    amList = await log.ListPjtDirectorLog(log_seq);
                    searcherList = await log.ListPjtManagerLog(log_seq);
                    placeList = await log.ListPjtPlaceLog(log_seq);
                    keywordList = await log.ListPjtKeywordLog(log_seq);

                }
                else
                {
                    amList = new List<pjt_director_log>();
                    searcherList = new List<pjt_manager_log>();
                    placeList = new List<pjt_place_log>();
                    keywordList = new List<pjt_keyword_log>();
                }

                ViewBag.p_seq = p_seq;
                ViewBag.log_seq = log_seq;
                ViewBag.select = listSummary;

                ViewBag.amList = amList;
                ViewBag.searcherList = searcherList;
                ViewBag.placeList = placeList;
                ViewBag.keywordList = keywordList;

                return View(data);
            }
            catch (Exception)
            {

                throw;
            }
        }

        public async Task<ActionResult> CandidateLog(int c_seq = 0, int log_seq = 0)
        {
            try
            {
                CandidateRepository cr = new CandidateRepository();

                CandidateLogRepository log = new CandidateLogRepository();
                candidate_log data = null;
                List<can_school_log> schoolList = null;
                List<can_career_log> careerList = null;
                List<can_place_log> placeList = null;
                List<can_job_code_log> jobList = null;
                List<can_business_code_log> busiList = null;
                List<can_foreign_lan_log> foreignList = null;
                List<can_resume> resumeList = new List<can_resume>();
                List<candidate_log> listsummary = await log.ListCandidateSummary(c_seq);
                if (log_seq > 0 )
                {
                    data = await log.ClientCandidateLog(c_seq, log_seq);
                    schoolList = await log.ListCanSchoolLog(log_seq);
                    careerList = await log.ListCanCareerLog(log_seq);
                    placeList = await log.ListCanPlaceLog(log_seq);
                    jobList = await log.ListCanJobCodeLog(log_seq);
                    busiList = await log.ListCanBusinessCodeLog(log_seq);
                    foreignList = await log.ListCanForeignLanLog(log_seq);
                }
                if (data != null)
                {
                    if (data.is_confidential == 1 && String.IsNullOrEmpty(data.confi_remark))
                    {
                        data.confi_remark = " ";
                    }

                    if (data.is_inactive == 1 && String.IsNullOrEmpty(data.inactive_remark))
                    {
                        data.confi_remark = " ";
                    }
                }
                ViewBag.schoolList = schoolList;
                ViewBag.careerList = careerList;
                ViewBag.placeList = placeList;
                ViewBag.jobList = jobList;
                ViewBag.busiList = busiList;
                ViewBag.foreignList = foreignList;
                ViewBag.c_seq = c_seq;
                ViewBag.log_seq = log_seq;
                ViewBag.select = listsummary;

                return View(data);
            }
            catch (Exception)
            {

                throw;
            }
        }

    }
}