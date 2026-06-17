using System;
using System.IO;
using System.Text;
using System.Security.Cryptography;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.VisualBasic;
using System.Globalization;
using System.Text.RegularExpressions;

namespace Univision.Main
{
  public class Utils
  {
    public static string StrToHex(string input)
    {
      if (!String.IsNullOrEmpty(input))
        return string.Join("", input.Select(c => ((int)c).ToString("X2")));
      else
        return string.Empty;
    }

    public static string ConvertMoneyToString(double? input)
    {
      if (input.HasValue)
        return string.Format("{0:#,0.##}", input);
      else
        return string.Empty;
    }


    public static bool Chk_Han_Eng(string name)
    {
      byte[] byteArray = Encoding.Default.GetBytes(name);
      int value = Convert.ToInt32(byteArray[0].ToString());
      if (value > 127) //한글
      {
        return true; //한글
      }
      return false; //영문
    }

    public static string ConvertPhoneToString(string input)
    {
      if (input == null || input.Length < 10 || input.Length > 11)
      {
        return input;
      }

      input = input.Replace("-", "");
      if (input.Length == 10)
      {
        string str0 = input.Substring(0, 3);
        string str1 = input.Substring(3, 3);
        string str2 = input.Substring(6, 4);
        return str0 + "-" + str1 + "-" + str2;
      }

      if (input.Length == 11)
      {
        string str0 = input.Substring(0, 3);
        string str1 = input.Substring(3, 4);
        string str2 = input.Substring(7, 4);
        return str0 + "-" + str1 + "-" + str2;
      }

      return input;
    }

    public static string ReturnUniqueValue(int uv_seq, string head = "T")
    {
      var result = default(byte[]);

      using (var stream = new MemoryStream())
      {
        using (var writer = new BinaryWriter(stream, Encoding.UTF8, true))
        {
          writer.Write(DateTime.Now.Ticks);
          writer.Write(uv_seq);
        }

        stream.Position = 0;

        using (var hash = SHA256.Create())
        {
          result = hash.ComputeHash(stream);
        }
      }

      var text = new string[10];
      text[0] = head; // 최초문자 고정
      for (var i = 1; i < text.Length; i++)
      {
        text[i] = result[i].ToString("x2");
      }

      return string.Concat(text);
    }

    public static string ReturnCorpValue(int uv_seq, string head = "C")
    {
      var result = default(byte[]);

      using (var stream = new MemoryStream())
      {
        using (var writer = new BinaryWriter(stream, Encoding.UTF8, true))
        {
          writer.Write(DateTime.Now.Ticks);
          writer.Write(uv_seq);
        }

        stream.Position = 0;

        using (var hash = SHA256.Create())
        {
          result = hash.ComputeHash(stream);
        }
      }

      var text = new string[10];
      text[0] = head; // 최초문자 고정
      for (var i = 1; i < text.Length; i++)
      {
        text[i] = result[i].ToString("x2");
      }

      return string.Concat(text);
    }


    public static string ReturnCautionTypeTxt(int? caution_type)
    {

      try
      {
        if (caution_type.HasValue)
        {
          CandidateCautionType gCode = new CandidateCautionType();

          return gCode.nsCandidateCautionTypes[(int)caution_type];
        }
        else
        {
          return string.Empty;
        }
      }
      catch //(Exception e)
      {
        return string.Empty;
      }
    }

    public static string ReturnAttentionTypeTxt(int? caution_type)
    {

      try
      {
        if (caution_type.HasValue)
        {
          CandidateAttentionType gCode = new CandidateAttentionType();

          return gCode.nsCandidateAttentionTypes[(int)caution_type];
        }
        else
        {
          return string.Empty;
        }
      }
      catch //(Exception e)
      {
        return string.Empty;
      }
    }

    

    public static string ReturnContractFeeTypeTxt(string fee_type)
    {

      try
      {
        if (!string.IsNullOrEmpty(fee_type))
        {
          ContractFeeTypeCode gCode = new ContractFeeTypeCode();

          return gCode.ContractFeeTypeCodes[(string)fee_type];
        }
        else
        {
          return string.Empty;
        }
      }
      catch //(Exception e)
      {
        return string.Empty;
      }
    }


    public static string ReturnGenderTxt(double? gender)
    {

      try
      {
        if (gender.HasValue)
        {
          GenderCode gCode = new GenderCode();

          return gCode.GenderCodes[(double)gender];
        }
        else
        {
          return string.Empty;
        }
      }
      catch //(Exception e)
      {
        return string.Empty;
      }
    }

    public static string ReturnRegTypeTxt(int? reg_type)
    {

      try
      {
        if (reg_type.HasValue)
        {
          RegTypeCode gCode = new RegTypeCode();

          return gCode.RegTypeCodes[(int)reg_type];
        }
        else
        {
          return string.Empty;
        }
      }
      catch //(Exception e)
      {
        return string.Empty;
      }
    }

    public static string ReturnVatTypeTxt(int? vat_type)
    {

      try
      {
        if (vat_type.HasValue)
        {
          VatTypeCode gCode = new VatTypeCode();

          return gCode.VatTypeCodes[(int)vat_type];
        }
        else
        {
          return string.Empty;
        }
      }
      catch (Exception e)
      {
        return string.Empty;
      }
    }


    public static string ReturnRegCDTypeTxt(int? reg_type)
    {

      try
      {
        if (reg_type.HasValue)
        {
          RegCDTypeCode gCode = new RegCDTypeCode();

          return gCode.RegCDTypeCodes[(int)reg_type];
        }
        else
        {
          return string.Empty;
        }
      }
      catch //(Exception e)
      {
        return string.Empty;
      }
    }

    public static string ReturnExecTypeTxt(int? reg_type)
    {

      try
      {
        if (reg_type.HasValue)
        {
          ExecTypeCode gCode = new ExecTypeCode();

          return gCode.ExecTypeCodes[(int)reg_type];
        }
        else
        {
          return string.Empty;
        }
      }
      catch //(Exception e)
      {
        return string.Empty;
      }
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="workCode"></param>
    /// <returns></returns>
    public static string ReturnWorkCodeTxt(int? workCode)
    {
      try
      {
        WorkCode wCode = new WorkCode();

        if (workCode.HasValue)
          return wCode.WorkCodes[(int)workCode];
        else
          return string.Empty;
      }
      catch //(Exception e)
      {
        return string.Empty;
      }
    }

    /// <summary>
    /// 프로젝트 타입 코드 텍스트 리턴
    /// </summary>
    /// <param name="pjt_type"></param>
    /// <returns></returns>
    public static string ReturnProjectTypeTxt(int? pjt_type)
    {

      try
      {
        if (pjt_type.HasValue)
        {
          return ProjectTypeCode.ProjectTypeCodes[(int)pjt_type];
        }
        else
        {
          return string.Empty;
        }
      }
      catch //(Exception e)
      {
        return string.Empty;
      }
    }

    /// <summary>
    /// 프로젝트 상태 코드 텍스트 리턴
    /// </summary>
    /// <param name="status"></param>
    /// <returns></returns>
    public static string ReturnProjectStatusTxt(int? status)
    {

      try
      {
        if (status.HasValue)
        {
          return ProjectStatusCode.ProjectStatusCodes[(int)status];
        }
        else
        {
          return string.Empty;
        }
      }
      catch //(Exception e)
      {
        return string.Empty;
      }
    }

    public static string ReturnProjectHistoryStateTxt(int? state)
    {

      try
      {
        if (state.HasValue)
        {
          return ProjectHistoryStateCode.ProjectHistoryStateCodes[(int)state];
        }
        else
        {
          return string.Empty;
        }
      }
      catch //(Exception e)
      {
        return string.Empty;
      }
    }

    public static string ReturnSchoolGraduateStateTxt(int? state)
    {

      try
      {
        if (state.HasValue)
        {
          return SchoolGraduateState.SchoolGraduateStates[(int)state];
        }
        else
        {
          return string.Empty;
        }
      }
      catch //(Exception e)
      {
        return string.Empty;
      }
    }


    public static string ReturnSchoolEducationStateTxt(int? gubun)
    {

      try
      {
        if (gubun.HasValue)
        {
          return SchoolEducationState.SchoolEducationStates[(int)gubun];
        }
        else
        {
          return string.Empty;
        }
      }
      catch //(Exception e)
      {
        return string.Empty;
      }
    }

    public static string ReturnFileExtensionImg(string extension)
    {
      try
      {
        string src = "";
        switch (extension)
        {
          case ".doc":
          case ".docx":
            src = "/Images/word.png";
            break;
          case ".pdf":
            src = "/Images/pdf.png";
            break;
          default:

            break;
        }

        return src;
      }
      catch //(Exception e)
      {
        return string.Empty;
      }
    }

    public static string ReturnFileExtensionClass(string extension)
    {
      try
      {
        string fileClass = "";
        switch (extension.ToLower())
        {
          case ".doc":
          case ".docx":
            fileClass = "fa-file-word text-primary";
            break;
          case ".xls":
          case ".xlsx":
            fileClass = "fa-file-excel text-success";
            break;
          case ".ppt":
          case ".pptx":
            fileClass = "fa-file-powerpoint orange-600";
            break;
          case ".pdf":
            fileClass = "fa-file-pdf text-danger";
            break;
          case ".jpg":
          case ".jpeg":
          case ".png":
          case ".gif":
            fileClass = "fa-file-image text-";
            break;
          default:
            fileClass = "fa-file blue-600";
            break;
        }

        return fileClass;
      }
      catch //(Exception e)
      {
        return string.Empty;
      }
    }

    public static string ReturnFALinkClass(string url)
    {
      try
      {
        string fileClass = "";
        if (url.IndexOf("linkedin") >= 0)
        {
          fileClass = "fa-linkedin";
        }
        else if (url.IndexOf("facebook") >= 0)
        {
          fileClass = "fa-facebook";
        }
        else if (url.IndexOf("facebook") >= 0)
        {
          fileClass = "fa-facebook-square";
        }
        else if (url.IndexOf("twitter") >= 0)
        {
          fileClass = "fa-twitter-square";
        }
        else
        {
          fileClass = "fa-thumbtack";
        }


        return fileClass;
      }
      catch //(Exception e)
      {
        return string.Empty;
      }
    }

    /// <summary>
    /// 날짜 표기 타입 변환.(yyyy-MM-dd)
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    public static string ConvertDateTimeToString(DateTime? input)
    {
      return input != null ? input.Value.ToString("yyyy-MM-dd") : string.Empty;
    }

    public static string BirthdayCut(DateTime? input, int cut1 = 2, int cut2 = 2)
    {
      string birth = String.Empty;
      try
      {
        birth = input != null ? input.Value.ToString("yyyy-MM-dd") : string.Empty;
        return birth.Substring(cut1, cut2) + '년';
      }
      catch //(Exception e)
      {
        return String.Empty;
      }

    }


    public static string ConvertDateTime8DigitToString(DateTime? input)
    {
      return input != null ? input.Value.ToString("yyyyMMdd") : string.Empty;
    }

    public static string ConvertDateTimeFormat(string input, string format)
    {
      DateTime date = new DateTime();

      if (DateTime.TryParseExact(input, "dd/MM/yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out date))
        return date.ToString(format);
      else
        return input;
    }

    /// <summary>
    /// 날짜 표기 타입 변환.(yyyy-MM-dd hh:mm:ss)
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    public static string ConvertDateTimeHourToString(DateTime? input)
    {
      return input != null ? input.Value.ToString("yyyy-MM-dd HH:mm:ss") : string.Empty;
    }

    /// <summary>
    /// 날짜 표기 타입 변환.(yyyy-MM-dd hh:mm)
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    public static string ConvertDateTimeHourToString2(DateTime? input)
    {
      return input != null ? input.Value.ToString("yyyy-MM-dd HH:mm") : string.Empty;
    }

    /// <summary>
    /// 날짜 표기 타입 변환.(HH:mm)
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    public static string ConvertHourToString(DateTime? input)
    {
      return input != null ? input.Value.ToString("HH:mm") : string.Empty;
    }


    /// <summary>
    /// 글자수 자름.
    /// </summary>
    /// <param name="value">원문</param>
    /// <param name="maxLength">표기할 글자수</param>
    /// <returns></returns>
    public static string Truncate(string value, int maxLength)
    {
      string truncated = (string.IsNullOrEmpty(value) || value.Length <= maxLength) ? value : value.Substring(0, maxLength) + "...";

      return truncated;
    }

    public static DateTime NowKorea()
    {
      return DateTime.UtcNow.AddHours(9);
    }

    public static double RoundUp(double value, int up)
    {
      return Math.Round(value, up);
    }

    public static string YearSeparate(DateTime? value)
    {
      if (value.HasValue)
      {
        return value.Value.Year.ToString().Substring(2, 2);
      }
      else
        return string.Empty;

    }

    public static string GetRootUrl(HttpRequestBase req)
    {
      return string.Format("{0}://{1}", req.Url.Scheme, req.Headers["host"]);
    }

    public static string Get1ToString(double? ex_birthdate, string str)
    {
      try
      {
        return ex_birthdate == 1 ? str : String.Empty;
      }
      catch //(Exception e)
      {
        return string.Empty;
      }
    }
    public static int GetAge(DateTime? date_birth)
    {
      int age = 0;
      try
      {
        if (date_birth != null)
        {
          var today = DateTime.Today;

          var a = (today.Year * 100 + today.Month) * 100 + today.Day;
          var b = (date_birth.Value.Year * 100 + date_birth.Value.Month) * 100 + date_birth.Value.Day;

          age = (a - b) / 10000;
        }
        return age;

      }
      catch //(Exception e)
      {
        return age;
      }
    }

    public static int GetAge(int year)
    {
      int age = 0;
      try
      {
        if (year > 0)
        {
          
          DateTime date_birth = new DateTime(year, 1, 1);          

          var today = DateTime.Today;

          var a = (today.Year * 100 + today.Month) * 100 + today.Day;
          var b = (date_birth.Year * 100 + date_birth.Month) * 100 + date_birth.Day;

          age = (a - b) / 10000;
        }
        return age;

      }
      catch //(Exception e)
      {
        return age;
      }
    }

    /// <summary>
    /// 정렬 필드 클래스 리턴
    /// </summary>
    /// <param name="compareA"></param>
    /// <param name="compareB"></param>
    /// <param name="orderOption"></param>
    /// <returns></returns>
    public static string ReturnOrderOptionCalss(string compareA, string compareB, string orderOption)
    {
      try
      {
        if (compareA == compareB)
          if (orderOption == "ASC")
            return "fa-sort-up";
          else
            return "fa-sort-down";
        else
          return "fa-sort";

      }
      catch //(Exception e)
      {
        return "fa-sort";
      }
    }


    public static string gfGetPrettyDate(DateTime? data_date)
    {
      const int SECOND = 1;
      const int MINUTE = 60 * SECOND;
      const int HOUR = 60 * MINUTE;
      const int DAY = 24 * HOUR;
      const int MONTH = 30 * DAY;

      if (data_date != null)
      {
        var ts = new TimeSpan(DateTime.Now.Ticks - data_date.Value.Ticks);
        double delta = Math.Abs(ts.TotalSeconds);


        if (delta < 5 * MINUTE)
          return "just now";

        if (delta < 45 * MINUTE)
          return ts.Minutes + " minutes ago";

        if (delta < 90 * MINUTE)
          return "an hour ago";

        if (delta < 24 * HOUR)
          return ts.Hours + " hours ago";

        if (delta < 48 * HOUR)
          return "yesterday";

        if (delta < 30 * DAY)
          return ts.Days + " days ago";

        if (delta < 12 * MONTH)
        {
          int months = Convert.ToInt32(Math.Floor((double)ts.Days / 30));
          return months <= 1 ? "a month ago" : months + " months ago";
        }
        else
        {
          int years = Convert.ToInt32(Math.Floor((double)ts.Days / 365));
          return years <= 1 ? "a year ago" : years + " years ago";
        }
      }
      return string.Empty;
    }

    /// <summary>
    /// 회의실 이용구분 리턴.
    /// </summary>
    /// <param name="usage_cd"></param>
    /// <returns></returns>
    public static string returnMeetingRoomUseType(string usage_cd)
    {
      string useType = "";
      switch (usage_cd)
      {
        case "client":
          useType = "고객사 미팅";
          break;
        case "cand":
          useType = "후보자 미팅";
          break;
        case "team":
          useType = "팀 회의";
          break;
        case "edu":
          useType = "교육";
          break;
        case "oth":
          useType = "기타";
          break;
      }

      return useType;
    }

    /// <summary>
    /// 스케줄 타입 텍스트 리턴.
    /// </summary>
    /// <param name="type"></param>
    /// <returns></returns>
    public static string ReturnScheduleTypeTxt(int type)
    {

      try
      {
        return ScheduleType.ScheduleTypes[(int)type];
      }
      catch //(Exception e)
      {
        return string.Empty;
      }
    }


    /// <summary>
    /// 스케줄 서브타입 텍스트 리턴.
    /// </summary>
    /// <param name="sub_type"></param>
    /// <returns></returns>
    public static string ReturnScheduleSubTypeTxt(int sub_type)
    {

      try
      {
        return ScheduleSubType.ScheduleSubTypes[(int)sub_type];
      }
      catch //(Exception e)
      {
        return string.Empty;
      }
    }


    public static string returnScheduleColorReturn(int type)
    {
      if (type == ScheduleType.personal)
        return ScheduleType.personalColor;
      else if (type == ScheduleType.company)
        return ScheduleType.companyColor;
      else if (type == ScheduleType.team)
        return ScheduleType.teamColor;
      else if (type == ScheduleType.share)
        return ScheduleType.shareColor;
      else
        return string.Empty;

    }

    public static string ConvertBusinessLicenseFormat(string value)
    {
      if (value == null)
        return string.Empty;

      string LicNo = value.Trim().Replace("-", "");

      if (LicNo.Length == 10)
      {
        LicNo = Regex.Replace(LicNo, @"(\w{3})(\w{2})(\w{5})", @"$1-$2-$3");
        return LicNo;
      }
      else
        return value;
    }

    /// <summary>
    /// 프로젝트 status highlight return
    /// </summary>
    /// <param name="state">search class의 state</param>
    /// <param name="baseState">기본 state</param>
    /// <returns></returns>
    public static string ReturnPjtHighlight(int state, int baseState)
    {
      if (state == baseState)
        return "pjtStatusHighlight";
      else
        return "";
    }

    /// <summary>
    /// 휴가 타입 설명 반환.
    /// </summary>
    /// <param name="sub_type"></param>
    /// <returns></returns>
    public static string ReturnVacationTypeTxt(int type)
    {

      try
      {
        return VacationType.VacationTypes[(int)type];
      }
      catch (Exception e)
      {
        return string.Empty;
      }
    }

    /// <summary>
    /// 화폐단위 리턴
    /// </summary>
    /// <param name="currency_cd"></param>
    /// <returns></returns>
    public static string ReturnCurrencyTxt(string currency_cd, string type = "K")
    {
      try
      {
        string currencyTxt = CurrencyType.CurrencyTypes[currency_cd];

        if (type == "K")
          currencyTxt = Regex.Replace(currencyTxt, @"[^가-힣]", "");
        else
          currencyTxt = Regex.Replace(currencyTxt, @"[^a-zA-Z]", "");

        return currencyTxt;
      }
      catch //(Exception e)
      {
        return string.Empty;
      }



      //string currency_txt = "";
      //switch (currency_cd)
      //{
      //    case "WON":
      //        currency_txt = "WON(원)";
      //        break;
      //    case "USD":
      //        currency_txt = "USD(미국달러)";
      //        break;
      //    case "HKD":
      //        currency_txt = "HKD(홍콩달러)";
      //        break;
      //    case "YEN":
      //        currency_txt = "YEN(엔화)";
      //        break;
      //    case "GBP":
      //        currency_txt = "GBP(파운드)";
      //        break;
      //    case "DEM":
      //        currency_txt = "DEM(마르크)";
      //        break;
      //    case "FRF":
      //        currency_txt = "FRF(프랑스 프랑)";
      //        break;
      //    case "CHF":
      //        currency_txt = "CHF(스위스 프랑)";
      //        break;
      //    case "EUR":
      //        currency_txt = "EUR(유로)";
      //        break;
      //    case "CNY":
      //        currency_txt = "CNY(위안)";
      //        break;
      //    case "AUD":
      //        currency_txt = "AUD(호주달러)";
      //        break;
      //    case "NZD":
      //        currency_txt = "NZD(뉴질랜드달러)";
      //        break;
      //    case "CAD":
      //        currency_txt = "CAD(캐나다달러)";
      //        break;
      //    default:
      //        currency_txt = "WON(원)";
      //        break;
      //}

      //return currency_txt;
    }

    /// <summary>
    /// 직급 코드 txt 리턴
    /// </summary>
    /// <param name="position_seq"></param>
    /// <returns></returns>
    public static string ReturnPositionCodeTxt(int? position_seq)
    {

      try
      {
        if (position_seq.HasValue)
          return PositionCode.PositionCodes[(int)position_seq];
        else
          return string.Empty;
      }
      catch //(Exception e)
      {
        return string.Empty;
      }
    }


    public static string ReturnEducationTxt(double? edu_code)
    {
      string edu_txt = "";
      switch (edu_code)
      {
        case 0:
          edu_txt = "학력무관";
          break;
        case 1:
          edu_txt = "고등학교졸업";
          break;
        case 2:
          edu_txt = "대학졸업(2,3년)";
          break;
        case 3:
          edu_txt = "대학교졸업(4년)";
          break;
        case 4:
          edu_txt = "석사졸업";
          break;
        case 5:
          edu_txt = "박사졸업";
          break;
        case 6:
          edu_txt = "고등학교졸업이상";
          break;
        case 7:
          edu_txt = "대학졸업(2,3년)이상";
          break;
        case 8:
          edu_txt = "대학교졸업(4년)이상";
          break;
        case 9:
          edu_txt = "석사졸업이상";
          break;
        default:
          edu_txt = "";
          break;
      }

      return edu_txt;
    }

    /// <summary>
    /// 외국어 코드 리턴
    /// </summary>
    /// <param name="foreign_lang"></param>
    /// <returns></returns>
    public static string ReturnLanguageCodeTxt(string foreign_lang)
    {

      try
      {
        if (!string.IsNullOrWhiteSpace(foreign_lang))
          return LanguageCode.LanguageCodes[foreign_lang];
        else
          return string.Empty;
      }
      catch //(Exception e)
      {
        return string.Empty;
      }
    }


    public static string ReturnLanguageLevelCodeTxt(double? foreign_level)
    {

      try
      {
        if (foreign_level.HasValue)
          return LanguageLevelCode.LanguageLevelCodes[(int)foreign_level];
        else
          return string.Empty;
      }
      catch //(Exception e)
      {
        return string.Empty;
      }
    }

    public static string ReturnInOrderTypeTxt(int? orderType)
    {

      try
      {
        if (orderType.HasValue)
          return InOrderType.InOrderTypes[(int)orderType];
        else
          return string.Empty;
      }
      catch //(Exception e)
      {
        return string.Empty;
      }
    }
  }
}