using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Univision.Main.Models.Api
{
  public class ResponseSchool
  {
    /// <summary>
    /// 전체 검색 결과수
    /// </summary>
    public string totalCount { get; set; }

    /// <summary>
    /// 학교명
    /// </summary>
    public string schoolName { get; set; }

    /// <summary>
    /// 캠퍼스명
    /// </summary>
    public string campusName { get; set; }

    /// <summary>
    /// 학교종류
    /// </summary>
    public string schoolGubun { get; set; }

    /// <summary>
    /// 학교유형
    /// </summary>
    public string schoolType { get; set; }

    /// <summary>
    /// 설립유형
    /// </summary>
    public string estType { get; set; }

    /// <summary>
    /// 지역
    /// </summary>
    public string region { get; set; }

    /// <summary>
    /// 주소
    /// </summary>
    public string adres { get; set; }

    /// <summary>
    /// mycollege 접속 정보(대학교)
    /// </summary>
    public string collegeinfourl { get; set; }

    /// <summary>
    /// 링크 link : 상세페이지 링크, open: 새창으로 띄움
    /// </summary>
    public string link { get; set; }
    public string filter_str { get; set; }
  }
}