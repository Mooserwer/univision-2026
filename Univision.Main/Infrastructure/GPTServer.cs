using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Linq;
using System.Web;
using System.Threading.Tasks;
using Newtonsoft.Json;


namespace Univision.Main.Infrastructure
{
  public class GPTServer
  {
    private readonly string apiKey;
    private readonly HttpClient httpClient;


    public GPTServer()
    {

      //this.apiKey = ConfigurationManager.AppSettings["apiKey"];
      httpClient = new HttpClient();
      httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {apiKey}");
    }

    public string GetAPIKey()
    {
      return this.apiKey;
    }

    public GPTServer(string apiKey)
    {
      this.apiKey = apiKey;
      httpClient = new HttpClient();
      httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {apiKey}");
    }

    public async Task<string> GetCompletionAsync(List<GptMessage> messages, string model_name = "gpt-4o-mini")
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

    public async Task<dynamic> GetCompletionAsyncRaw(List<GptMessage> messages, string model_name = "gpt-4o-mini")
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
        var result = JsonConvert.DeserializeObject<OpenAIChatCompletionResponse>(jsonResponse);
        //string completionText = result.To;
        //return completionText.Trim();
        return result;
      }
      else
      {
        var errorContent = await response.Content.ReadAsStringAsync();
        //dynamic result = JsonConvert.DeserializeObject(errorContent);
        throw new Exception($"OpenAI API 오류: {errorContent}");
        //return result;
      }
    }

    public async Task<dynamic> GetCompletionAsyncStream(List<GptMessage> messages, string model_name = "gpt-4o-mini")
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
        stream = true
      };

      //var jsonRequestBody = System.Text.Json.JsonSerializer.Serialize(requestBody);

      var jsonRequestBody = JsonConvert.SerializeObject(requestBody);

      var httpContent = new StringContent(jsonRequestBody, Encoding.UTF8, "application/json");

      var response = await httpClient.PostAsync("https://api.openai.com/v1/chat/completions", httpContent);

      if (response.IsSuccessStatusCode)
      {
        return await response.Content.ReadAsStreamAsync();
      }
      // 실패 시 예외 처리
      throw new HttpRequestException("Error: Unable to connect to the API.");
    }
  }



  public class GptMessage
  {
    [JsonProperty("role")]
    public string Role { get; set; }

    [JsonProperty("content")]
    public string Content { get; set; }
  }

  public class OpenAIChatCompletionResponse
  {
    [JsonProperty("id")]
    public string Id { get; set; }
    [JsonProperty("object")]
    public string Object { get; set; }
    [JsonProperty("created")]
    public long Created { get; set; }
    [JsonProperty("model")]
    public string Model { get; set; }
    [JsonProperty("usage")]
    public Usage Usage { get; set; }
    [JsonProperty("choices")]
    public List<Choice> Choices { get; set; }
  }

  public class Usage
  {
    [JsonProperty("prompt_tokens")]
    public int PromptTokens { get; set; }
    [JsonProperty("completion_tokens")]
    public int CompletionTokens { get; set; }
    [JsonProperty("total_tokens")]
    public int TotalTokens { get; set; }
  }

  public class Choice
  {
    [JsonProperty("message")]
    public GptMessage Message { get; set; }
    [JsonProperty("finish_reason")]
    public string FinishReason { get; set; }
    [JsonProperty("index")]
    public int Index { get; set; }
  }

  
}