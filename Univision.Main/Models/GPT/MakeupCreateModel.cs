using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Univision.Core.Models.DTO;

namespace Univision.Main.Models.GPT
{
  public class MakeupCreateModel
  {
    /*
     * 이름
     */
    public string candidate { get; set; }
    public string yob { get; set; }
    public string gender { get; set; }
    public string addr { get; set; }
    public List<string> selfintro { get; set; } = new List<string>();
    public List<MakeupCareerDesc1> core { get; set; } = new List<MakeupCareerDesc1>();
    public List<MakeupEducationModel> education { get; set; } = new List<MakeupEducationModel>();
    public List<MakeupCareerModel> career { get; set; } = new List<MakeupCareerModel>();
    public List<MakeupCertModel> awards { get; set; } = new List<MakeupCertModel>();
    public List<MakeupLearnModel> learns { get; set; } = new List<MakeupLearnModel>();
    public List<MakeupLearnModel> activities { get; set; } = new List<MakeupLearnModel>();
    public List<MakeupLearnModel> overseas { get; set; } = new List<MakeupLearnModel>();
    public List<MakeupSkillModel> skills { get; set; } = new List<MakeupSkillModel>();
    public List<MakeupCertModel> certifications { get; set; } = new List<MakeupCertModel>();
    public List<MakeupLangModel> languages { get; set; } = new List<MakeupLangModel>();
    public List<string> etcs { get; set; } = new List<string>();
  }


  public class MakeupEducationModel
  {
    public string school { get; set; }
    public string major { get; set; }
    public string area { get; set; }
    public string grade { get; set; }
    public string degree { get; set; }
    public string is_grdt { get; set; }
    public string ad_yyyymm { get; set; }
    public string gdt_yyyymm { get; set; }
  }

  public class MakeupCareerModel
  {
    public string company { get; set; }
    [JsonConverter(typeof(StringOrArrayConverter))]
    public List<string> info { get; set; } = new List<string>();
    public string area { get; set; }
    public string dept { get; set; }
    public string pos { get; set; }
    public string is_leader { get; set; }
    public string j_yyyymm { get; set; }
    public string r_yyyymm { get; set; }
    public string r_reason { get; set; }
    public List<MakeupCareerDesc1> desc { get; set; } = new List<MakeupCareerDesc1>();
  }

  [JsonConverter(typeof(CareerDesc1Converter))]
  public class MakeupCareerDesc1
  {
    public string desc { get; set; }
    public List<MakeupCareerDesc2> depth { get; set; } = new List<MakeupCareerDesc2>();
  }

  public class MakeupCareerDesc2
  {
    public string desc { get; set; }
    public List<MakeupCareerDesc3> depth { get; set; } = new List<MakeupCareerDesc3>();
  }

  public class MakeupCareerDesc3
  {
    public string desc { get; set; }
    public List<MakeupCareerDesc4> depth { get; set; } = new List<MakeupCareerDesc4>();
  }

  public class MakeupCareerDesc4
  {
    public string desc { get; set; }
  }

  public class MakeupLearnModel
  {
    public string name { get; set; }
    public string gov { get; set; }
    public string year1 { get; set; }
    public string year2 { get; set; }

  }

  public class MakeupSkillModel
  {
    public string name { get; set; }
    public string desc { get; set; }

  }

  public class MakeupCertModel
  {
    public string name { get; set; }
    public string gov { get; set; }
    public string year { get; set; }

  }

  public class MakeupLangModel
  {
    public string language { get; set; }
    public string level { get; set; }

  }

  /// <summary>
  /// 구버전 호환 — 배열 항목이 string 이면 desc-only 객체로 변환.
  /// (구버전 페이지는 core를 string 배열로 전송, 신버전은 desc/depth 객체 배열로 전송)
  /// </summary>
  public class CareerDesc1Converter : JsonConverter
  {
    public override bool CanConvert(Type objectType) => objectType == typeof(MakeupCareerDesc1);
    public override bool CanWrite => false;

    public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
    {
      if (reader.TokenType == JsonToken.Null) return null;
      if (reader.TokenType == JsonToken.String)
        return new MakeupCareerDesc1 { desc = (string)reader.Value };

      var obj = JObject.Load(reader);
      var item = new MakeupCareerDesc1();
      serializer.Populate(obj.CreateReader(), item);
      return item;
    }

    public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
      => throw new NotImplementedException();
  }

  /// <summary>
  /// 구버전 호환 — string 단일값이면 1개짜리 리스트로 변환.
  /// (구버전 페이지는 career.info를 string으로 전송, 신버전은 string 배열로 전송)
  /// </summary>
  public class StringOrArrayConverter : JsonConverter
  {
    public override bool CanConvert(Type objectType) => objectType == typeof(List<string>);
    public override bool CanWrite => false;

    public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
    {
      if (reader.TokenType == JsonToken.Null) return new List<string>();
      if (reader.TokenType == JsonToken.String)
      {
        var s = (string)reader.Value;
        return string.IsNullOrEmpty(s) ? new List<string>() : new List<string> { s };
      }
      return serializer.Deserialize<List<string>>(reader);
    }

    public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
      => throw new NotImplementedException();
  }

}