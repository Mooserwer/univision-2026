using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Univision.Core.Models;
using Univision.Main.Infrastructure;
using Univision.Main.Models;
using Univision.Main.Models.Api;
using Univision.Security;

namespace Univision.Main.Controllers
{
    [Authorize]
    public class HomeController : BaseController
    {
        public ActionResult Index()
        {
            string email = AppIdentity.email;
            return View();
        }

        public ActionResult companyApi()
        {
            return View();
        }

        public ActionResult ApiPage()
        {
            tempModel model = new tempModel()
            {
                PagingInfo = new PagingInfo
                {
                    CurrentPage = 1
                    ,
                    ItemsPerPage = 10
                    ,
                    TotalItems = 20
                }
            };
            return View(model);
        }

        /// <summary>
        /// 법인 사업장 조회
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<JsonResult> SearchCompanyList(RequestCompany data)
        {
            try
            {
                CallApi api = new CallApi();

                var list = await api.CompanySearchApi(data);

                return Json(new
                {
                    ok = true
                    ,
                    list = list
                });
            }
            catch (Exception e)
            {
                return Json(new
                {
                    ok = false
                    ,
                    message = "fail"
                });
            }
        }

        public ActionResult universityApi()
        {
            return View();
        }

        /// <summary>
        /// 대학조회
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<JsonResult> SearchScoolList(RequestSchool data)
        {
            try
            {
                CallApi api = new CallApi();

                var list = await api.SchoolSearchApi(data);

                return Json(new
                {
                    ok = true
                    ,
                    list = list
                });
            }
            catch (Exception e)
            {
                return Json(new
                {
                    ok = false
                    ,
                    message = "fail"
                });
            }
        }


        
    }
}