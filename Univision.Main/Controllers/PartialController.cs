using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Univision.Core.Models;
using Univision.Core.Models.DTO;
using Univision.Core.Models.DTO.Response;
using Univision.Core.Models.DTO.Response.Sms;
using Univision.Core.Models.HomePage;
using Univision.Core.Repositories;
using Univision.Core.Repositories.Homepage;
using Univision.Main.Infrastructure;
using Univision.Main.Infrastructure.Mailing;
using Univision.Main.Infrastructure.SMS;
using Univision.Main.Models.Api;
using Univision.Main.Models.DashBoard;
using Univision.Security;


namespace Univision.Main.Controllers
{
  public class PartialController : BaseController
  {
    /// <summary>
    /// 법인 검색 API ( REST 사용)
    /// </summary>
    /// <param name="companyName"></param>
    /// <returns></returns>
    [HttpPost]
    public async Task<JsonResult> FindCompanyApiRest(string companyName)
    {

      try
      {
        SearchEngineApi asr = new SearchEngineApi();

        var list = await asr.SelectCompanyListAsync(companyName);


        return Json(new
        {
          ok = true,
          list = list
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

    /// <summary>
    /// 법인 검색 API (DLL 사용)
    /// </summary>
    /// <param name="companyName"></param>
    /// <returns></returns>
    [HttpPost]
    public async Task<JsonResult> FindCompanyApi(string companyName)
    {

      try
      {
        SearchEngineRepository asr = new SearchEngineRepository();

        var list = await asr.SelectCompanyListAsync(companyName);


        return Json(new
        {
          ok = true,
          list = list
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
    public async Task<JsonResult> FindExchangeApi(string currency = "", string date = "")
    {

      try
      {
        ExchangeEntityRepository eer = new ExchangeEntityRepository();
        exchange data = new exchange();

        if (!String.IsNullOrWhiteSpace(date))
          data = await eer.SelectExchangeOneAsync(currency, date);
        
        if (data.ex_seq == 0)
          data = await eer.SelectExchangeOneAsync(currency);
          

        return Json(new
        {
          ok = true,
          data = data
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

    /// <summary>
    /// 법인 검색 API
    /// </summary>
    /// <param name="companyName"></param>
    /// <returns></returns>
    [HttpPost]
    public async Task<JsonResult> FindUserEmail(string q = "", string ids = "", string seqs = "")
    {

      try
      {
        AdminRepository asr = new AdminRepository();

        var list = await asr.SelectUserMailListAsync(q, ids, seqs);


        return Json(list);
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
    public async Task<JsonResult> FindKeyword(string q, string old, List<string> job, List<string> busi)
    {

      try
      {
        SearchEngineRepository ser = new SearchEngineRepository();
        /*
        if (!String.IsNullOrEmpty(old))
        {
          old = "'" + old.Replace(", ", "', '") + "'";
        }
        */
        var list = await ser.SelectKeywordListAsync(q, busi, job);


        return Json(new
        {
          list = list
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

    #region 학교, 학과 검색
    /// <summary>
    /// 학교 검색 API
    /// </summary>
    /// <param name="gubun"></param>
    /// <param name="schoolName"></param>
    /// <returns></returns>
    [HttpPost]
    public async Task<JsonResult> FindSchoolApi(string gubun, string sch1, string schoolName)
    {
      try
      {
        if (gubun == "high_list")
        {
          CallApi api = new CallApi();

          RequestSchool data = new RequestSchool()
          {
            gubun = gubun
              ,
            searchSchulNm = schoolName
              ,
            sch1 = sch1
              ,
            perPage = "10"
              ,
            thisPage = "1"
          };

          var list = await api.SchoolSearchApi(data);
          for (var i = 0;i < list.Count; i++)
          {
            list[i].filter_str = list[i].schoolName;
          }

          return Json(new
          {
            list = list
          });
        } 
        else
        {
          ApiEntityRepository aer = new ApiEntityRepository();

          var list_db = await aer.SelectCodeSchoolSearchListAsync();

          List<ResponseSchool> list = new List<ResponseSchool>();

          foreach(var data in list_db)
          {
            list.Add(new ResponseSchool
            {
              schoolName = data.school_name,
              filter_str = data.school_name + ',' + data.search_str
            });
          }

          return Json(new
          {
            list = list
          });
        }
      }
      catch (Exception e)
      {
        return Json(new
        {
          ok = false
        });
      }
    }

    /// <summary>
    /// 학교 검색 API
    /// </summary>
    /// <param name="gubun"></param>
    /// <param name="schoolName"></param>
    /// <returns></returns>
    [HttpPost]
    public async Task<JsonResult> FindClientApi(string client_name, int c_seq)
    {
      try
      {
        ApiRepository ar = new ApiRepository();
        if (!String.IsNullOrEmpty(client_name))
        {
          client_name = client_name.Replace("(주)", "").Replace("(유)", "").Replace("주식회사", "").Replace(" ", "");
        }
        var list = await ar.CodeClientListAsync(client_name, c_seq);

        return Json(new
        {
          list = list
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

    public PartialViewResult SearchClient(string searchTxt, int page = 1)
    {
      ProjectRepository pr = new ProjectRepository();

      int totalCount = 0;

      //한 페이지당 몇개의 행을 보여줄 것인지
      int pageSize = AppPaging.PageLength5;

      var list = pr.SelectClientList(searchTxt, (page - 1) * pageSize, pageSize, out totalCount);

      var model = new SearchClientModel()
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

    /// <summary>
    /// 학과 검색
    /// </summary>
    /// <param name="major_name"></param>
    /// <returns></returns>
    [HttpPost]
    public async Task<JsonResult> FindMajorApi(string major_name, string category1_name = "")
    {
      try
      {
        ApiRepository ar = new ApiRepository();

        var list = await ar.SelectMajorListAsync(category1_name, major_name);

        return Json(new
        {
          list = list
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

    #endregion

    #region 파일업로드

    [HttpPost]
    public PartialViewResult FileUploadApi()
    {
      return PartialView();
    }

    [HttpPost]
    public JsonResult FileUpload(HttpPostedFileBase[] files)
    {

      try
      {
        FileUpload fileUpload = new FileUpload();

        var resultList = fileUpload.Upload(files);

        JsonFiles result = new JsonFiles(resultList);

        //결과 리스트에 값이 없으면 에러.
        bool isEmpty = !resultList.Any();
        if (isEmpty)
        {
          return Json("Error ");
        }
        else
        {
          return Json(result);
        }
      }
      catch (Exception e)
      {
        return Json("Error ");
      }
    }

    #endregion

    #region 산업 API

    /// <summary>
    /// 산업 검색
    /// </summary>
    /// <returns></returns>
    [HttpPost]
    public async Task<PartialViewResult> BusinessApi(List<RequestBusinessCode> data)
    {
      ApiRepository ar = new ApiRepository();

      var list1 = await ar.SelectBusinessCode1ListAsync();

      var list2 = new List<code_business_dtl>();
      //var list3 = new List<code_business3>();

      if (data != null)
      {
        list2 = await ar.SelectBusinessCode2ListAsync(data.FirstOrDefault().code1);
        //list3 = await ar.SelectBusinessCode3ListAsync(data.FirstOrDefault().code1, data.FirstOrDefault().code2);
      }

      BusinessModel model = new BusinessModel()
      {
        code1 = data != null ? data.FirstOrDefault().code1 : 0
          ,
        code2 = data != null ? data.FirstOrDefault().code2 : 0
          ,
        //code3 = data != null ? data.Select(x => x.code3).ToList() : new List<float>()
        //,
        list1 = list1
          ,
        list2 = list2
        //,
        //list3 = list3
      };

      return PartialView(model);

    }

    /// <summary>
    /// 산업 2단계 단건 검색
    /// </summary>
    /// <param name="data"></param>
    /// <returns></returns>
    [HttpPost]
    public async Task<PartialViewResult> BusinessSingleApi(int code1 = 0, int code2 = 0)
    {
      ApiRepository ar = new ApiRepository();

      var list1 = await ar.SelectBusinessCode1ListAsync();

      var list2 = await ar.SelectBusinessCode2ListAsync(code1);

      BusinessModel model = new BusinessModel()
      {
        code1 = code1
          ,
        code2 = code2
          ,
        list1 = list1
          ,
        list2 = list2
      };

      return PartialView(model);

    }

    /// <summary>
    /// 산업 2단계 조회.
    /// </summary>
    /// <param name="code1"></param>
    /// <returns></returns>
    [HttpPost]
    public async Task<JsonResult> BusinessApi2(int code1)
    {
      try
      {
        ApiRepository ar = new ApiRepository();

        var list = await ar.SelectBusinessCode2ListAsync(code1);

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

        });
      }
    }

    [HttpPost]
    public async Task<JsonResult> OfflimitCheckList()
    {
      try
      {
        ClientEntityRepository cer = new ClientEntityRepository();

        var list = await cer.SelectOfflimitCheckListAsync();

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
          ok = false,
          message = e.Message
        });
      }
    }

    /// <summary>
    /// 산업 3단계 조회
    /// </summary>
    /// <param name="code1"></param>
    /// <returns></returns>
    [HttpPost]
    public async Task<JsonResult> BusinessApi3(int code1, int code2)
    {
      try
      {
        ApiRepository ar = new ApiRepository();

        var list = await ar.SelectBusinessCode3ListAsync(code1, code2);

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

        });
      }
    }

    /// <summary>
    /// 산업 키워드 검색(자동완성)
    /// </summary>
    /// <param name="searchValue"></param>
    /// <returns></returns>
    [HttpPost]
    public async Task<JsonResult> SearchBusinessKeywordApi(string searchValue)
    {
      try
      {

        ApiRepository ar = new ApiRepository();

        var list = await ar.SelectBusinessKeywordListAsync(searchValue);

        return Json(new
        {
          list = list
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

    /// <summary>
    /// 산업 키워드 검색(자동완성) 2단계까지만 검색
    /// </summary>
    /// <param name="searchValue"></param>
    /// <returns></returns>
    [HttpPost]
    public async Task<JsonResult> SearchBusinessKeywordApi2(string searchValue)
    {
      try
      {

        ApiRepository ar = new ApiRepository();

        var list = await ar.SelectBusinessKeywordListAsync2(searchValue);

        return Json(new
        {
          list = list
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

    #endregion

    #region 산업 멀티 API

    /// <summary>
    /// 직무·산업 검색
    /// </summary>
    /// <param name="businessData"></param>
    /// <param name="jobData"></param>
    /// <returns></returns>
    [HttpPost]
    public async Task<PartialViewResult> BusinessMultiApi()
    {
      ApiRepository ar = new ApiRepository();

      var businessList1 = await ar.SelectBusinessCode1ListAsync();

      JobBusinessModel model = new JobBusinessModel()
      {
        businessList1 = businessList1
      };

      return PartialView(model);
    }

    [HttpPost]
    public async Task<PartialViewResult> BusinessMultiApi2()
    {
      ApiRepository ar = new ApiRepository();

      var businessList1 = await ar.SelectBusinessCode1ListAsync();

      JobBusinessModel model = new JobBusinessModel()
      {
        businessList1 = businessList1
      };

      return PartialView(model);
    }

    #endregion

    #region 직무 멀티 API

    /// <summary>
    /// 직무·산업 검색
    /// </summary>
    /// <param name="businessData"></param>
    /// <param name="jobData"></param>
    /// <returns></returns>
    [HttpPost]
    public async Task<PartialViewResult> JobMultiApi()
    {
      ApiRepository ar = new ApiRepository();

      var jobList1 = await ar.SelectJobCode1ListAsync();

      JobBusinessModel model = new JobBusinessModel()
      {
        jobList1 = jobList1
      };

      return PartialView(model);
    }

    [HttpPost]
    public async Task<PartialViewResult> JobMultiApi2()
    {
      ApiRepository ar = new ApiRepository();

      var jobList1 = await ar.SelectJobCode1ListAsync();

      JobBusinessModel model = new JobBusinessModel()
      {
        jobList1 = jobList1
      };

      return PartialView(model);
    }

    #endregion

    #region 직무 API

    /// <summary>
    /// 직무 검색
    /// </summary>
    /// <returns></returns>
    [HttpPost]
    public async Task<PartialViewResult> JobApi(List<RequestJobCode> data)
    {
      ApiRepository ar = new ApiRepository();

      var list1 = await ar.SelectJobCode1ListAsync();

      var list2 = new List<code_job_dtl>();
      //var list3 = new List<code_job3>();

      if (data != null)
      {
        list2 = await ar.SelectJobCode2ListAsync(data.FirstOrDefault().code1);
        //list3 = await ar.SelectJobCode3ListAsync(data.FirstOrDefault().code1, data.FirstOrDefault().code2);
      }

      JobModel model = new JobModel()
      {
        code1 = data != null ? data.FirstOrDefault().code1 : 0
          ,
        code2 = data != null ? data.FirstOrDefault().code2 : 0
          //,
          //code3 = data != null ? data.Select(x => x.code3).ToList() : new List<float>()
          ,
        list1 = list1
          ,
        list2 = list2
        //,
        //list3 = list3
      };

      return PartialView(model);

    }

    /// <summary>
    /// 직무 검색 단일선택 및 2단계만 조회 가능.
    /// </summary>
    /// <returns></returns>
    [HttpPost]
    public async Task<PartialViewResult> JobSingleApi(int code1 = 0, int code2 = 0)
    {
      ApiRepository ar = new ApiRepository();

      var list1 = await ar.SelectJobCode1ListAsync();

      var list2 = await ar.SelectJobCode2ListAsync(code1);

      JobModel model = new JobModel()
      {
        code1 = code1
          ,
        code2 = code2
          ,
        list1 = list1
          ,
        list2 = list2
      };

      return PartialView(model);

    }

    /// <summary>
    /// 직무 2단계 조회.
    /// </summary>
    /// <param name="code1"></param>
    /// <returns></returns>
    [HttpPost]
    public async Task<JsonResult> JobApi2(int code1)
    {
      try
      {
        ApiRepository ar = new ApiRepository();

        var list = await ar.SelectJobCode2ListAsync(code1);

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

        });
      }
    }

    /// <summary>
    /// 직무 3단계 조회
    /// </summary>
    /// <param name="code1"></param>
    /// <returns></returns>
    [HttpPost]
    public async Task<JsonResult> JobApi3(int code1, int code2)
    {
      try
      {
        ApiRepository ar = new ApiRepository();

        var list = await ar.SelectJobCode3ListAsync(code1, code2);

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

        });
      }
    }

    /// <summary>
    /// 직무 키워드 검색(자동완성)
    /// </summary>
    /// <param name="searchValue"></param>
    /// <returns></returns>
    [HttpPost]
    public async Task<JsonResult> SearchJobKeywordApi(string searchValue)
    {
      try
      {

        ApiRepository ar = new ApiRepository();

        var list = await ar.SelectJobKeywordListAsync(searchValue);

        return Json(new
        {
          list = list
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

    /// <summary>
    /// 직무 키워드 검색(자동완성) 2단계까지만 검색
    /// </summary>
    /// <param name="searchValue"></param>
    /// <returns></returns>
    [HttpPost]
    public async Task<JsonResult> SearchJobKeywordApi2(string searchValue)
    {
      try
      {

        ApiRepository ar = new ApiRepository();

        var list = await ar.SelectJobKeywordListAsync2(searchValue);

        return Json(new
        {
          list = list
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


    public async Task<ActionResult> CandidateSimpleMemo()
    {

      return PartialView();
    }

    #endregion

    #region 직무·산업 API

    public async Task<PartialViewResult> JobBusiSugg(List<string> curr_job, List<string> curr_busi, List<string> keywords)
    {
      ApiRepository ar = new ApiRepository();


      if (curr_job == null) 
        curr_job = new List<string>();
      if (curr_busi == null)
        curr_busi = new List<string>();
      if (keywords == null)
        keywords = new List<string>();
      
      var list = await ar.SelectCanBusiCodeListAsync(curr_job, curr_busi, keywords);

      JobBusiSuggListModel model = new JobBusiSuggListModel()
      {
        list = list
      };

      return PartialView(model);
    }

    /// <summary>
    /// 직무·산업 검색
    /// </summary>
    /// <param name="businessData"></param>
    /// <param name="jobData"></param>
    /// <returns></returns>
    [HttpPost]
    public async Task<PartialViewResult> JobBusinessApi(List<RequestBusinessCode> businessData, List<RequestJobCode> jobData)
    {
      ApiRepository ar = new ApiRepository();

      var businessList1 = await ar.SelectBusinessCode1ListAsync();

      var jobList1 = await ar.SelectJobCode1ListAsync();

      JobBusinessModel model = new JobBusinessModel()
      {
        businessList1 = businessList1
          ,
        jobList1 = jobList1
      };

      return PartialView(model);
    }

    [HttpPost]
    public async Task<JsonResult> SearchJobBusinessKeywordApi(string searchValue)
    {
      try
      {

        ApiRepository ar = new ApiRepository();

        var list = await ar.SelectJobBusinessKeywordListAsync(searchValue);

        return Json(new
        {
          ok = true,
          list = list
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

    #endregion

    #region 직급/직책

    /// <summary>
    /// 직급/직책 검색
    /// </summary>
    /// <returns></returns>
    [HttpPost]
    public async Task<PartialViewResult> RankPositionApi(int r_code = 0, int p_code = 0)
    {
      ApiRepository ar = new ApiRepository();

      var rankList = await ar.SelectRankCodeListAsync();
      var positionList = await ar.SelectPositionCodeListAsync();

      RankPositionModel model = new RankPositionModel()
      {
        r_code = r_code
          ,
        p_code = p_code
          ,
        rankList = rankList
          ,
        positionList = positionList
      };

      return PartialView(model);

    }

    #endregion

    #region 희망근무지

    [HttpPost]
    public async Task<PartialViewResult> PlaceApi()
    {
      ApiRepository ar = new ApiRepository();

      var addr1List = await ar.SelectAddr1ListAsync();

      PlaceModel model = new PlaceModel()
      {
        addr1List = addr1List
      };

      return PartialView(model);
    }

    public async Task<JsonResult> SelectPlaceAddr2(string code1)
    {
      try
      {
        ApiRepository ar = new ApiRepository();

        var addr2List = await ar.SelectAddr2ListAsync(code1);

        return Json(new
        {
          ok = true,
          list = addr2List
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

    #endregion

    #region 알림 뱃지

    [HttpPost]
    public async Task<JsonResult> GetNewAlram()
    {
      try
      {
        ApiRepository ar = new ApiRepository();

        var list = await ar.SelectAlramBadgeListAsync(AppIdentity.user_seq);

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
        });
      }
    }

    [HttpPost]
    public async Task<JsonResult> GetReceiptStatus()
    {
      try
      {
        ApiRepository ar = new ApiRepository();

        var list = await ar.SelectReceiptCountAsync(AppIdentity.user_seq);

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
        });
      }
    }

    [HttpPost]
    public async Task<JsonResult> GetAttendResult()
    {
      try
      {
        ApiRepository ar = new ApiRepository();
        
        var rst = await ar.SelectAttendCountAsync(AppIdentity.user_seq, Utils.ConvertDateTimeToString(Utils.NowKorea()));

        return Json(new
        {
          ok = true
            ,
          attend = rst
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

    /// <summary>
    /// 알림 단건 읽음 처리
    /// </summary>
    /// <param name="am_seq"></param>
    /// <returns></returns>
    [HttpPost]
    public async Task<JsonResult> AlarmConfirm(int am_seq)
    {
      try
      {

        ApiEntityRepository aer = new ApiEntityRepository();

        var data = await aer.SelectAlarmUserOneAsync(am_seq, AppIdentity.user_seq);

        if (data == null)
          return Json(new
          {
            ok = false,
            message = "선택한 알림 메세지를 찾을 수 없습니다."
          });

        data.is_read = 1;
        data.read_date = Utils.NowKorea();

        await aer.CreateOrUpdateAlarmUserOneAsync(data, CommonCodes.Update);

        var msg = await aer.SelectAlarmMessageOneAsync(am_seq);

        return Json(new
        {
          ok = true,
          url = HttpUtility.UrlEncode(msg.href_url)
        });

      }
      catch (Exception e)
      {
        return Json(new
        {
          ok = false
            ,
          message = "알림 메세지 확인 중 오류가 발생 했습니다."
        });
      }
    }

    /// <summary>
    /// 다중 알림 읽음 처리.
    /// </summary>
    /// <param name="am_seqs"></param>
    /// <returns></returns>
    [HttpPost]
    public async Task<JsonResult> CheckAlarmConfirm(int[] am_seqs)
    {
      try
      {
        ApiEntityRepository aer = new ApiEntityRepository();

        var list = await aer.SelectAlarmUserListAsync(am_seqs, AppIdentity.user_seq);

        if (list == null)
          return Json(new
          {
            ok = false,
            message = "선택한 메세지를 찾을 수 없습니다."
          });

        foreach (var data in list)
        {
          data.is_read = 1;
          data.read_date = Utils.NowKorea();
        }

        await aer.CreateOrUpdateAlarmUserListAsync(list, CommonCodes.Update);

        return Json(new
        {
          ok = true
            ,
          message = "선택하신 메세지를 읽음 표시 했습니다."
        });

      }
      catch (Exception e)
      {
        return Json(new
        {
          ok = false
            ,
          message = "알림 메세지 확인 중 오류가 발생 했습니다."
        });
      }
    }

    #endregion


    /// <summary>
    /// 국가검색 API
    /// </summary>
    /// <param name="countryName"></param>
    /// <returns></returns>
    [HttpPost]
    public async Task<JsonResult> FindCountryApi(string country_name)
    {
      try
      {
        ApiRepository ar = new ApiRepository();

        var list = await ar.SelectCountryNameListAsync(country_name.Replace(" ", ""));

        return Json(new
        {
          list = list
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

    #region SMS 발송

    public PartialViewResult SMSCreate(string receive_number = "")
    {
      ViewBag.myPhoneNumber = AppIdentity.cellphone;
      //ViewBag.companyNumber = ConfigurationManager.AppSettings["companyPhoneNumber"].ToString();
      ViewBag.companyNumber = AppIdentity.tel;
      ViewBag.receive_number = receive_number;

      return PartialView();
    }

    public async Task<PartialViewResult> SMSPrivacyCreate(List<int> c_list, string receive_type= "", int cl_seq = 0)
    {
      CandidateRepository cr = new CandidateRepository();

      ResponsePrivacyAgree model = new ResponsePrivacyAgree();

      if (c_list != null)
      {
        foreach(int c_seq in c_list)
        {
          var cand = await cr.SelectCandidateOneAsync(c_seq);

          if (cand != null)
          {
            receive_info ri = new receive_info();
            ri.c_seq = cand.c_seq;
            ri.can_name = cand.kor_name;
            
            ri.can_email = (!String.IsNullOrEmpty(cand.email1) ? cand.email1 : cand.email2);

            if (c_list.Count == 1 && !String.IsNullOrEmpty(receive_type) && receive_type != "EMAIL")
            {
              
              if (receive_type == "SMS1")
              {
                ri.can_phone = cand.phone;
                ri.can_addr = cand.phone;
                ri.can_type = "SMS";
              }
              else if (receive_type == "SMS2")
              {
                ri.can_phone = cand.cell_phone;
                ri.can_addr = cand.cell_phone;
                ri.can_type = "SMS";
              }


            } else
            {
              string phone_str = String.Empty;
              if (!String.IsNullOrEmpty(cand.phone) && cand.phone.Substring(0, 3) == "010")
              {
                phone_str = cand.phone;
              } else if (!String.IsNullOrEmpty(cand.cell_phone) && cand.cell_phone.Substring(0, 3) == "010")
              {
                phone_str = cand.cell_phone;
              }

              ri.can_phone = phone_str;

              if (!String.IsNullOrEmpty(receive_type) && receive_type == "EMAIL")
              {
                //ri.can_phone = (!String.IsNullOrEmpty(cand.phone) ? cand.phone : cand.cell_phone);
                ri.can_addr = ri.can_email;
                ri.can_type = receive_type;
              } 
              else
              {
                ri.can_addr = (!String.IsNullOrEmpty(phone_str) ? phone_str : ri.can_email);
                ri.can_type = (!String.IsNullOrEmpty(phone_str) ? "SMS" : "EMAIL");
              }
              
            }

            
            model.receive.Add(ri);
          } 
        }
      } 
      
      
      ClientRepository clr = new ClientRepository();

      model.is_privacy_agree = 1;
      var cl = await clr.SelectClientOneAsync(cl_seq);
      if (cl != null)
      {
        model.is_provide_agree = 1;
        model.client_seq = cl.c_seq;
        model.client_name = cl.kor_name;
      }

      ViewBag.MyEmail = AppIdentity.email;
      ViewBag.myPhoneNumber = AppIdentity.cellphone;
      //ViewBag.companyNumber = ConfigurationManager.AppSettings["companyPhoneNumber"].ToString();
      ViewBag.companyNumber = AppIdentity.tel;

      
      return PartialView(model);
    }
    

    public async Task<JsonResult> CreatePrivacy(RequestPrivacyAgree data)
    {
      try
      {
        List<privacy_agree> privacyList = new List<privacy_agree>();
        List<privacy_agree_dtl> privacyDtlList = new List<privacy_agree_dtl>();

        List<home_privacy_agree> home_privacyList = new List<home_privacy_agree>();
        List<home_privacy_agree_dtl> home_privacyDtlList = new List<home_privacy_agree_dtl>();

        SmsEntityRepository ser = new SmsEntityRepository();
        HomepageSmsEntityRepository hser = new HomepageSmsEntityRepository();

        foreach (var r_info in data.receive)
        {
          privacy_agree privacy = new privacy_agree();
          List<privacy_agree_dtl> privacy_dtl = new List<privacy_agree_dtl>();
          
          

          privacy.send_type = r_info.can_type.Substring(0, 1);
          
          if (r_info.can_type == "EMAIL")
            privacy.send_addr = AppIdentity.email;
          else
            privacy.send_addr = data.send_phone;

          privacy.can_name = r_info.can_name;
          privacy.can_contact = r_info.can_addr;
          privacy.c_seq = r_info.c_seq;
          privacy.full_txt = data.contents;
          privacy.create_dt = Utils.NowKorea();
          privacy.create_user = AppIdentity.user_seq;

          if (data.is_privacy_agree == 1)
          {
            privacy_agree_dtl p_dtl = new privacy_agree_dtl();
            p_dtl.agree_type = 1;
            p_dtl.is_optional = 0;
            p_dtl.contents = data.privacy_text;
            p_dtl.create_dt = Utils.NowKorea();
            p_dtl.create_user = AppIdentity.user_seq;

            privacy_dtl.Add(p_dtl);
          }

          if (data.is_provide_agree == 1)
          {
            privacy_agree_dtl p_dtl = new privacy_agree_dtl();
            p_dtl.agree_type = 2;
            p_dtl.is_optional = 0;
            p_dtl.contents = data.provide_text;
            p_dtl.client_seq = data.client_seq;
            p_dtl.client_name = data.client_name;
            p_dtl.create_dt = Utils.NowKorea();
            p_dtl.create_user = AppIdentity.user_seq;

            privacy_dtl.Add(p_dtl);
          }
          
          await ser.CreatePrivacyData(privacy, privacy_dtl);


          home_privacy_agree home_privacy = new home_privacy_agree();
          List<home_privacy_agree_dtl> home_privacy_dtl = new List<home_privacy_agree_dtl>();

          home_privacy.pa_seq = privacy.pa_seq;
          home_privacy.send_type = r_info.can_type.Substring(0, 1);
          home_privacy.agree_code3 = Utils.StrToHex("key_" + privacy.pa_seq.ToString() + "_key");
          home_privacy.send_dt = Utils.NowKorea();
          if (r_info.can_type == "EMAIL")
            home_privacy.send_addr = AppIdentity.email;
          else
            home_privacy.send_addr = data.send_phone;

          home_privacy.can_name = r_info.can_name;
          home_privacy.can_contact = r_info.can_addr;
          home_privacy.c_seq = r_info.c_seq;
          home_privacy.full_txt = data.contents;
          home_privacy.create_dt = Utils.NowKorea();
          home_privacy.create_user = AppIdentity.user_seq;
          home_privacy.create_email = AppIdentity.email;

          if (data.is_privacy_agree == 1)
          {
            home_privacy_agree_dtl home_p_dtl = new home_privacy_agree_dtl();
            home_p_dtl.pa_seq = privacy.pa_seq;
            home_p_dtl.agree_type = 1;
            home_p_dtl.is_optional = 0;
            home_p_dtl.contents = data.privacy_text;
            home_p_dtl.create_dt = Utils.NowKorea();
            home_p_dtl.create_user = AppIdentity.user_seq;

            home_privacy_dtl.Add(home_p_dtl);
          }

          if (data.is_provide_agree == 1)
          {
            home_privacy_agree_dtl home_p_dtl = new home_privacy_agree_dtl();
            home_p_dtl.pa_seq = privacy.pa_seq;
            home_p_dtl.agree_type = 2;
            home_p_dtl.is_optional = 0;
            home_p_dtl.contents = data.provide_text;
            home_p_dtl.client_seq = data.client_seq;
            home_p_dtl.client_name = data.client_name;
            home_p_dtl.create_dt = Utils.NowKorea();
            home_p_dtl.create_user = AppIdentity.user_seq;

            home_privacy_dtl.Add(home_p_dtl);
          }

          await hser.CreatePrivacyData(home_privacy, home_privacy_dtl);
          /*
          privacy.full_txt += 
";
          //"https://www.unicosearch.com/privacyprovide/"
          */

          await SendPrivacy(privacy);

        }

        return Json(new
        {
          ok = true
            ,
          message = "동의서를 발송 했습니다."
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

    public async Task<JsonResult> SendPrivacy(privacy_agree privacy)
    {
      try {
        string key_str = Utils.StrToHex("key_" + privacy.pa_seq.ToString() + "_key");
        string privacy_url = "https://www.unicosearch.com/privacyprovide/" + key_str;
        SmsEntityRepository ser = new SmsEntityRepository();

        if (privacy.send_type == "S")
        {
          SmsService service = new SmsService();
          SmsServiceModel smsDto = new SmsServiceModel();
          

          string add_text = @"안녕하세요 " + privacy.can_name + @" 님 
(주)유니코써치는 인재추천 및 채용절차 진행을 위하여 아래와 같이 개인정보 수집·이용 및 제3자(채용사) 제공에 동의를 받고자 합니다.
제공해 주신 소중한 개인정보는 채용을 위한 절차 이외에 어떤 목적으로도 사용되지 않으며, 동의서 내용을 상세히 확인하신 후 동의여부를 결정하여 진행해 주시기 바랍니다.

아래의 링크주소를 클릭하시면 개인정보 수집 및 제공 동의 화면으로 이동합니다. 



" + privacy_url;
          ;

          smsDto = await service.SendSmsAsyncNew(new SmsSingleSendDto
          {
            Message = privacy.full_txt + add_text
            ,
            PhoneReceiver = privacy.can_contact
            ,
            PhoneSender = privacy.send_addr
            ,
            Subject = String.Empty //"[유니코써치]개인정보 수집 및 제 3자 제공 동의"
          });

          List<sms_history> smsList = new List<sms_history>();
          smsList.Add(new sms_history()
          {
            msg_id = smsDto.msgid,
            sms_type = 0,
            send_number = privacy.send_addr,
            send_type = 0,
            receive_number = privacy.can_contact,
            contents = privacy.full_txt + add_text,
            contents_len = 999,
            response_code = smsDto.response_code,
            response_desc = smsDto.response_desc,
            create_dt = Utils.NowKorea(),
            create_user = AppIdentity.user_seq
          });

          await ser.CreateSms(smsList);
        } else if(privacy.send_type == "E")
        {
          AccountRepository ar = new AccountRepository();
          List<string> ToArr = new List<string>();
          var myuserinfo = await ar.FindUserBySeqAsync(AppIdentity.user_seq);
          MailService mService = new MailService(myuserinfo.email_id, myuserinfo.email_pwd);

          ToArr.Add(privacy.can_contact);

          //메일 Dto
          PrivacyAgreeDto mailData = new PrivacyAgreeDto()
          {
            ToArr = ToArr.ToArray()
              ,
            From = new System.Net.Mail.MailAddress(AppIdentity.email, AppIdentity.name)
              ,
            canname = privacy.can_name
            //'content = privacy.full_txt.Replace("\r\n", "\n").Replace("\n", "<br />")
              ,
            url = privacy_url
          };
          var result_mail = mService.SendPrivacyAgreeMail(mailData, new PrivacyAgreeTemplete());
        }

        privacy.send_dt = Utils.NowKorea();
        privacy.agree_code3 = key_str;
        await ser.UpdatePrivacyMst(privacy);

        return Json(new
        {
          ok = true
              ,
          message = "동의서를 발송 했습니다."
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

    public async Task<JsonResult> SendSms(sms_history data)
    {
      try
      {
        List<sms_history> smsList = new List<sms_history>();

        SmsService service = new SmsService();
        SmsServiceModel smsDto = new SmsServiceModel();

        foreach (var receiveNum in data.receive_numbers)
        {
          //SMS 발송.
          smsDto = await service.SendSmsAsyncNew(new SmsSingleSendDto
          {
            Message = data.contents
              ,
            PhoneReceiver = receiveNum
              ,
            PhoneSender = data.send_number
              ,
            Subject = string.Empty
          });

          //DB에 입력할 SMS 리스트에 추가.
          smsList.Add(new sms_history()
          {
            msg_id = smsDto.msgid,
            sms_type = data.sms_type,
            send_number = data.send_number,
            send_type = data.send_type,
            receive_number = receiveNum,
            contents = data.contents,
            contents_len = data.contents_len,
            response_code = smsDto.response_code,
            response_desc = smsDto.response_desc,
            create_dt = Utils.NowKorea(),
            create_user = AppIdentity.user_seq
          });

        }

        SmsEntityRepository ser = new SmsEntityRepository();
        await ser.CreateSms(smsList);

        return Json(new
        {
          ok = true
            ,
          message = "메세지를 발송 했습니다."
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

    #endregion

    public async Task<PartialViewResult> kpiModal()
    {
      DashboardRepository dr = new DashboardRepository();

      DateTime startDate = Utils.NowKorea().AddDays(1 - Utils.NowKorea().Day);
      DateTime endDate = startDate.AddMonths(1).AddDays(-1);

      string startDt = startDate.ToString("yyyy-MM-dd");
      string endDt = endDate.ToString("yyyy-MM-dd");

      var dayCnt = 0;

      for (int i = 0; i < DateTime.DaysInMonth(startDate.Year, startDate.Month); i++)
      {
        if (startDate.DayOfWeek != DayOfWeek.Saturday && startDate.DayOfWeek != DayOfWeek.Sunday)
          dayCnt++;

        startDate = startDate.AddDays(1);
      }


      DashBoardTopSummaryModel model = new DashBoardTopSummaryModel()
      {
        rCandidateCnt = await dr.CountRegRecandidateMonth(AppIdentity.user_seq, startDt, endDt)
          ,
        intvCandidateCnt = await dr.CountInterviewRecandidateMonth(AppIdentity.user_seq, startDt, endDt)
          ,
        MemoCnt = await dr.CountMemoMonth(AppIdentity.user_seq, startDt, endDt)
          ,
        candidateCnt = await dr.CountRegCandidateMonth(AppIdentity.user_seq, startDt, endDt)
          ,
        candidateUdCnt = await dr.CountUpdateCandidateMonth(AppIdentity.user_seq, startDt, endDt)          
      };


      return PartialView(model);
    }

    #region 개인정보 수정

    public async Task<PartialViewResult> PersonalModify()
    {
      AccountRepository ar = new AccountRepository();
      var user = await ar.FindUserBySeqAsync(AppIdentity.user_seq);

      return PartialView(user);
    }



    [HttpPost]
    public async Task<JsonResult> PersonalModify(uv_user model)
    {
      try
      {
        AdminEntityRepository aer = new AdminEntityRepository();
        var user = await aer.SelectUserOneAsync(model.uv_seq);

        if (user == null)
          return Json(new
          {
            ok = false,
            message = "개인정보 수정하려는 유저가 없습니다."
          });

        //pwd 가 비어있지 않고, pwd확인이랑 같다면 수정.
        if (!string.IsNullOrWhiteSpace(model.pwd) && model.pwd == model.pwd_rp)
          user.pwd = model.pwd;

        user.hp = model.hp;


        await aer.CreateOrUpdateUser(user, CommonCodes.Update, CommonCodes.Update, AppIdentity.user_seq, 1);

        return Json(new
        {
          ok = true,
          message = "개인정보를 수정 했습니다."
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

    #endregion

    public async Task<PartialViewResult> SelectOneUser(string bindObj, int ud_seq = 0, string searchTxt = "")
    {
      ProjectRepository pr = new ProjectRepository();
      AccountRepository ar = new AccountRepository();

      var list = await pr.SelectAMList(ud_seq, searchTxt);
      var deptList = await ar.SelectDivisionList();

      Models.Project.SearchProjectAMModel model = new Models.Project.SearchProjectAMModel()
      {
        ud_seq = ud_seq
          ,
        searchTxt = searchTxt
          ,
        list = list
          ,
        deptList = deptList
      };

      ViewBag.bindObj = bindObj;

      return PartialView(model);
    }
  }
}