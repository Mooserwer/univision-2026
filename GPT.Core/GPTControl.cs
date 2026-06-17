using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System.Configuration;

namespace GPT.Core
{
  public class GPTControl
  {
    private readonly string apiKey;
    private readonly HttpClient httpClient;


    public GPTControl() {

      this.apiKey = ConfigurationManager.AppSettings["apiKey"];
      httpClient = new HttpClient();
      httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {apiKey}");
    }

    public string GetAPIKey()
    {
      return this.apiKey;
    }      

    public GPTControl(string apiKey)
    {
      this.apiKey = apiKey;
      httpClient = new HttpClient();
      httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {apiKey}");
    }

    public async Task<string> GetCompletionAsync(List<Message> messages, string model_name = "gpt-4o-mini")
    {
      if (String.IsNullOrEmpty(model_name))
      {
        model_name = "gpt-4o-mini";
      }

      var requestBody = new
      {
        model = model_name, // 사용하고자 하는 모델 이름
        messages = messages,
        //max_tokens = 100, // 생성할 텍스트의 최대 토큰 수
        temperature = 0, // 생성 텍스트의 창의성 수준 
        top_p = 0,
      };

      var jsonRequestBody = JsonConvert.SerializeObject(requestBody);

      var httpContent = new StringContent(jsonRequestBody, Encoding.UTF8, "application/json");

      var response = await httpClient.PostAsync("https://api.openai.com/v1/chat/completions", httpContent);

      if (response.IsSuccessStatusCode)
      {
        var jsonResponse = await response.Content.ReadAsStringAsync();
        dynamic result = JsonConvert.DeserializeObject(jsonResponse);
        string completionText = result.choices[0].message.content;
        return completionText.Trim();
      }
      else
      {
        var errorContent = await response.Content.ReadAsStringAsync();
        throw new Exception($"OpenAI API 오류: {errorContent}");
      }
    }

    public async Task<string> GetCompletionAsyncRaw(List<Message> messages, string model_name = "gpt-4o-mini")
    {
      if (String.IsNullOrEmpty(model_name))
      {
        model_name = "gpt-4o-mini";
      }

      var requestBody = new
      {
        model = model_name, // 사용하고자 하는 모델 이름
        messages = messages,
        //max_tokens = 100, // 생성할 텍스트의 최대 토큰 수
        temperature = 0, // 생성 텍스트의 창의성 수준 
        top_p = 0,
      };

      var jsonRequestBody = JsonConvert.SerializeObject(requestBody);

      var httpContent = new StringContent(jsonRequestBody, Encoding.UTF8, "application/json");

      var response = await httpClient.PostAsync("https://api.openai.com/v1/chat/completions", httpContent);

      if (response.IsSuccessStatusCode)
      {
        var jsonResponse = await response.Content.ReadAsStringAsync();
        //dynamic result = JsonConvert.DeserializeObject(jsonResponse);
        //string completionText = result.To;
        //return completionText.Trim();
        return jsonResponse;
      }
      else
      {
        var errorContent = await response.Content.ReadAsStringAsync();
        //dynamic result = JsonConvert.DeserializeObject(errorContent);
        throw new Exception($"OpenAI API 오류: {errorContent}");
        //return result;
      }
    }




  }



  public class Message
  {
    [JsonProperty("role")]
    public string Role { get; set; }

    [JsonProperty("content")]
    public string Content { get; set; }
  }
}
