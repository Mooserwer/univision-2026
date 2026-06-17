using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.Xml.Serialization;
using Univision.Main.Models.Api;

namespace Univision.Main.Infrastructure
{
    public class CallApi
    {
        private static string datagoBaseUrl = "http://apis.data.go.kr/B552015/NpsBplcInfoInqireService";
        private static string datagoApiKey = "4y7Id36pMQbJjD5x4CsNDMkVt4kzVs5Zwm8tqXg7o6RZHIqyvynbcgHfDCsSyorTTI%2F4WZLzMZ46EVyuSKasRA%3D%3D";

        private static string careerBaseUrl = "http://www.career.go.kr/cnet/openapi";
        private static string careerApiKey = "92433d86ab87a445272a9bb2ed206fe9";

        /// <summary>
        /// 법인 리스트 조회
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public async Task<List<ResponseCompany>> CompanySearchApi(RequestCompany data)
        {
            try
            {
                var client = new RestClient(datagoBaseUrl);

                // Build request.
                var req = new RestRequest("/getBassInfoSearch", Method.GET);
                req.AddHeader("Content-Type", "charset=utf-8");

                req.AddParameter("serviceKey", datagoApiKey, ParameterType.QueryStringWithoutEncode);

                if (!string.IsNullOrWhiteSpace(data.ldong_addr_mgpl_dg_cd))
                    req.AddParameter("ldong_addr_mgpl_dg_cd", data.ldong_addr_mgpl_dg_cd, ParameterType.QueryStringWithoutEncode);                //법정동주소광역시도코드

                if (!string.IsNullOrWhiteSpace(data.ldong_addr_mgpl_sggu_cd))
                    req.AddParameter("ldong_addr_mgpl_sggu_cd", data.ldong_addr_mgpl_sggu_cd, ParameterType.QueryStringWithoutEncode);            //법정동주소시군구코드

                if (!string.IsNullOrWhiteSpace(data.ldong_addr_mgpl_sggu_emd_cd))
                    req.AddParameter("ldong_addr_mgpl_sggu_emd_cd", data.ldong_addr_mgpl_sggu_emd_cd, ParameterType.QueryStringWithoutEncode);    //법정동주소읍면동코드

                if (!string.IsNullOrWhiteSpace(data.wkpl_nm))
                    req.AddParameter("wkpl_nm", HttpUtility.UrlEncode(data.wkpl_nm, Encoding.UTF8), ParameterType.QueryStringWithoutEncode);      //사업장명

                if (!string.IsNullOrWhiteSpace(data.bzowr_rgst_no))
                    req.AddParameter("bzowr_rgst_no", data.bzowr_rgst_no, ParameterType.QueryStringWithoutEncode);                                //사업자등록번호

                req.AddParameter("pageNo", data.pageNo);                                                  //페이지번호
                req.AddParameter("numOfRows", data.numOfRows, ParameterType.QueryStringWithoutEncode);                                            //행갯수

                var response = await client.ExecuteTaskAsync<ResponseCompany>(req);

                JObject jObject = JObject.Parse(response.Content);

                JArray jArray = (JArray)jObject["response"]["body"]["items"]["item"];
                List<ResponseCompany> list = jArray.ToObject<List<ResponseCompany>>();

                return list;
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        /// <summary>
        /// 대학교 리스트 조회
        /// </summary>
        /// <returns></returns>
        public async Task<List<ResponseSchool>> SchoolSearchApi(RequestSchool data)
        {
            try
            {
                var client = new RestClient(careerBaseUrl);

                // Build request.
                var req = new RestRequest("/getOpenApi", Method.GET);

                req.AddParameter("apiKey", careerApiKey, ParameterType.QueryStringWithoutEncode);
                req.AddParameter("svcCode", "SCHOOL", ParameterType.QueryStringWithoutEncode);
                req.AddParameter("svcType", "api", ParameterType.QueryStringWithoutEncode);
                req.AddParameter("contentType", "json", ParameterType.QueryStringWithoutEncode);

                if (!string.IsNullOrWhiteSpace(data.gubun))
                    req.AddParameter("gubun", data.gubun, ParameterType.QueryStringWithoutEncode);
                //지역
                if (!string.IsNullOrWhiteSpace(data.region))
                    req.AddParameter("region", data.region, ParameterType.QueryStringWithoutEncode);

                //학교유형1
                if (!string.IsNullOrWhiteSpace(data.sch1))
                    req.AddParameter("sch1", data.sch1, ParameterType.QueryStringWithoutEncode);

                //학교유형2
                if (!string.IsNullOrWhiteSpace(data.sch2))
                    req.AddParameter("sch2", data.sch2, ParameterType.QueryStringWithoutEncode);

                //설립유형(대학교 : 국립, 사립, 공립)
                if (!string.IsNullOrWhiteSpace(data.est))
                    req.AddParameter("est", HttpUtility.UrlEncode(data.est, Encoding.UTF8), ParameterType.QueryStringWithoutEncode);

                //검색어
                if (!string.IsNullOrWhiteSpace(data.searchSchulNm))
                    req.AddParameter("searchSchulNm", data.searchSchulNm, ParameterType.QueryStringWithoutEncode);

                //현재페이지
                req.AddParameter("thisPage", data.thisPage);

                //한페이지당 건수
                req.AddParameter("perPage", data.perPage, ParameterType.QueryStringWithoutEncode);

                var response = await client.ExecuteTaskAsync<ResponseSchool>(req);

                JObject jObject = JObject.Parse(response.Content);

                JArray jArray = (JArray)jObject["dataSearch"]["content"];
                List<ResponseSchool> list = jArray.ToObject<List<ResponseSchool>>();

                return list;
            }
            catch (Exception e)
            {
                throw e;
            }
        }

    }

}