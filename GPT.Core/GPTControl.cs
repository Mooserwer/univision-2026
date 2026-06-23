using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
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
      
      var requestBody = new Dictionary<string, object>
      {
        { "model", model_name },
        { "messages", messages }
      };

      // 2. model_name 조건에 따라 프로퍼티 동적 추가
      if (model_name.StartsWith("gpt-4"))
      {
        requestBody["temperature"] = 0; // 원하는 값으로 설정
        requestBody["top_p"] = 0;
      }

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

    // ─────────────────────────────────────────────────────────
    //  OpenAI Files API 업로드 (purpose: user_data) → file_id 반환
    //  PDF / DOCX / HWP 등 파일을 텍스트 추출 없이 직접 업로드
    // ─────────────────────────────────────────────────────────
    public async Task<string> UploadFileAsync(string filePath)
    {
      if (String.IsNullOrEmpty(filePath) || !File.Exists(filePath))
        throw new Exception("업로드할 파일을 찾을 수 없습니다: " + filePath);

      using (var form = new MultipartFormDataContent())
      {
        var fileBytes = File.ReadAllBytes(filePath);
        var fileContent = new ByteArrayContent(fileBytes);
        form.Add(fileContent, "file", Path.GetFileName(filePath));
        form.Add(new StringContent("user_data"), "purpose");

        var response = await httpClient.PostAsync("https://api.openai.com/v1/files", form);
        var json = await response.Content.ReadAsStringAsync();
        if (!response.IsSuccessStatusCode)
          throw new Exception($"OpenAI 파일 업로드 오류: {json}");

        var jo = JObject.Parse(json);
        return (string)jo["id"];
      }
    }

    // ─────────────────────────────────────────────────────────
    //  OpenAI 파일 삭제 (사용 후 정리, 실패는 무시)
    // ─────────────────────────────────────────────────────────
    public async Task DeleteFileAsync(string fileId)
    {
      if (String.IsNullOrEmpty(fileId)) return;
      try { await httpClient.DeleteAsync("https://api.openai.com/v1/files/" + fileId); }
      catch { /* 정리 실패는 무시 */ }
    }

    // ─────────────────────────────────────────────────────────
    //  Responses API — 업로드한 파일(file_id) + 지시문으로 분석
    //  (기본 모델 gpt-5.4-mini). 응답의 output_text 를 반환
    // ─────────────────────────────────────────────────────────
    public async Task<string> GetResponseFromFileAsync(string fileId, string instructions, string model_name = "gpt-5.4-mini")
    {
      if (String.IsNullOrEmpty(model_name)) model_name = "gpt-5.4-mini";

      var requestBody = new
      {
        model = model_name,
        input = new[]
        {
          new
          {
            role = "user",
            content = new object[]
            {
              new { type = "input_file", file_id = fileId },
              new { type = "input_text", text = instructions }
            }
          }
        }
      };

      var jsonRequestBody = JsonConvert.SerializeObject(requestBody);
      var httpContent = new StringContent(jsonRequestBody, Encoding.UTF8, "application/json");
      var response = await httpClient.PostAsync("https://api.openai.com/v1/responses", httpContent);
      var jsonResponse = await response.Content.ReadAsStringAsync();

      if (!response.IsSuccessStatusCode)
        throw new Exception($"OpenAI Responses API 오류: {jsonResponse}");

      // output 배열에서 message 타입의 output_text 추출 (reasoning 모델은 앞에 reasoning 항목이 올 수 있음)
      var jo = JObject.Parse(jsonResponse);
      var outputArr = jo["output"] as JArray;
      if (outputArr != null)
      {
        foreach (var item in outputArr)
        {
          if ((string)item["type"] != "message") continue;
          var content = item["content"] as JArray;
          if (content == null) continue;
          foreach (var c in content)
          {
            if ((string)c["type"] == "output_text")
              return ((string)c["text"]).Trim();
          }
        }
      }
      throw new Exception("OpenAI Responses API 빈 응답");
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
