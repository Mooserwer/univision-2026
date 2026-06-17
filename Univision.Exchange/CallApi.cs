using Newtonsoft.Json.Linq;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace Univision.Exchange
{
  public class CallApi
  {
    private static string BaseUrl = "https://www.koreaexim.go.kr/site/program/financial/exchangeJSON";
    private static string customsBaseUrl = "http://apis.data.go.kr/1220000/retrieveTrifFxrtInfo/getRetrieveTrifFxrtInfo";
    //private static string ApiKey = "OZv3IR9MrcycxhElDj4am7DDfbZ3ABJY";
    private static string ApiKey = "06LhQZgd9TYu83UimoJDhpljTgqitPvx";
    private static string customsApiKey = "4y7Id36pMQbJjD5x4CsNDMkVt4kzVs5Zwm8tqXg7o6RZHIqyvynbcgHfDCsSyorTTI%2F4WZLzMZ46EVyuSKasRA%3D%3D";
    /// <summary>
    /// 법인 리스트 조회
    /// </summary>
    /// <param name="data"></param>
    /// <returns></returns>
    public async Task<List<ExchangeApiModel>> ExchangeApi(string date = "")
    {
      try
      {
        var client = new RestClient(BaseUrl);

        // Build request.
        var req = new RestRequest("/", Method.GET);
        req.AddHeader("Content-Type", "charset=utf-8");

        req.AddParameter("authkey", ApiKey, ParameterType.QueryStringWithoutEncode);
        req.AddParameter("data", "AP01");

        if (!String.IsNullOrEmpty(date))
        {
          req.AddParameter("searchdate", date);
        }

        var response = await client.ExecuteTaskAsync<ExchangeApiModel>(req);
        //var content = response.Content.TrimStart(new char[] { '[' }).TrimEnd(new char[] { ']' });
        //JObject jObject = JObject.Parse(content);

        Console.WriteLine(response.Content);
        JArray jsonArray = JArray.Parse(response.Content);
        List<ExchangeApiModel> list = jsonArray.ToObject<List<ExchangeApiModel>>();

        return list;
      }
      catch (Exception e)
      {
        throw e;
      }
    }

    public async Task<ExchangeXMLModel> ExchangeCustomsApiXML(string date = "")
    {
      try
      {
        var client = new RestClient(customsBaseUrl);

        // Build request.
        var req = new RestRequest("/", Method.GET);
        req.XmlSerializer = new RestSharp.Serializers.DotNetXmlSerializer();
        req.RequestFormat = DataFormat.Xml;
        //req.RootElement = "response";
        req.AddHeader("Content-Type", "charset=utf-8");

        req.AddParameter("ServiceKey", customsApiKey, ParameterType.QueryStringWithoutEncode);
        req.AddParameter("weekFxrtTpcd", "2");

        if (!String.IsNullOrEmpty(date))
        {
          req.AddParameter("aplyBgnDt", date);
        }

        var response = await client.ExecuteTaskAsync<ExchangeXMLModel>(req);
        //var content = response.Content.TrimStart(new char[] { '[' }).TrimEnd(new char[] { ']' });
        //JObject jObject = JObject.Parse(content);
        Console.WriteLine(response.Content);

        var dotNetXmlDeserializer = new RestSharp.Deserializers.DotNetXmlDeserializer();
        ExchangeXMLModel modelClassObject =
         dotNetXmlDeserializer.Deserialize<ExchangeXMLModel>(response);

        return modelClassObject;
      }
      catch (Exception e)
      {
        throw e;
      }
    }
  }

}