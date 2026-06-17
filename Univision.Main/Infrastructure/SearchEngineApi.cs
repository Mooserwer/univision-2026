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
using Univision.Main.Models.KONAN;
using Univision.Core.Models.DTO;


namespace Univision.Main.Infrastructure
{
    public class SearchEngineApi
    {
        private static string serverAddr = "http://220.117.204.140:7577/";
        
        public async Task<List<can_career>> SelectCompanyListAsync(string company_name)
        {
            try
            {
                var client = new RestClient(serverAddr);
                // Build request.
                var req = new RestRequest("/search", Method.GET);
                req.AddHeader("Content-Type", "charset=utf-8");
                //req.AddParameter("serviceKey", datagoApiKey, ParameterType.QueryStringWithoutEncode);

                string select = "*";
                string from = "company_autocomplete.company_autocomplete";
                //string where = HttpUtility.UrlEncode("company_idx='" + company_name + "' allword order by $SIZE(wkpl_nm) asc, $RELEVANCE desc").Replace("+", "%20");
                string where = "company_idx='" + company_name + "' allword synonym order by $RELEVANCE desc";
                string limit = "20";
                req.AddParameter("select", select, ParameterType.QueryString);           
                req.AddParameter("from", from, ParameterType.QueryString);               
                req.AddParameter("where", where, ParameterType.QueryString);             
                req.AddParameter("limit", limit, ParameterType.QueryString);

                //var response = await client.ExecuteTaskAsync<EntityKonanListModel<gov_api_company>>(req);
                var response = await client.ExecuteTaskAsync(req);

                var data = JsonConvert.DeserializeObject<EntityKonanListModel<can_career>>(response.Content);


                List<can_career> list = new List<can_career>();

                if (data.result.rows.Count > 0)
                {
                    foreach(var row in data.result.rows)
                    {
                        list.Add(row.fields);
                    }
                }


                return list;
            }
            catch (Exception e)
            {

                throw e;
            }
        }



    }

}