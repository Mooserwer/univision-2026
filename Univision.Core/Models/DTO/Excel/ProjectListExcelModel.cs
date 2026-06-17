using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Univision.Core.Models.DTO.Excel
{
  public class ProjectListExcelModel
  {
    public string 상태 { get; set; }
    public string 업체명 { get; set; }
    public string 제목 { get; set; }
    public string 진행상태 { get; set; }
    public string AM { get; set; }
    public string Searcher { get; set; }
    public string 산업 { get; set; }
    public string 생성일 { get; set; }
  }

  public class ProjectListNewExcelModel
  {
    public string 프로젝트_코드 { get; set; }
    public string 고객사_코드 { get; set; }
    public string 고객사_국문명 { get; set; }
    public string 고객사_영문명 { get; set; }
    public string 프로젝트_구분 { get; set; }
    public string 프로젝트_진행상태 { get; set; }
    public string PE_구분 { get; set; }
    public string 구인제목 { get; set; }
    public string 예상_연봉 { get; set; }
    public string 수수료율 { get; set; }
    public string 포지션명 { get; set; }
    public string 직급분류 { get; set; }
    public string 주_산업_대분류 { get; set; }
    public string 주_산업_소분류 { get; set; }
    public string 보조_산업_대분류 { get; set; }
    public string 보조_산업_소분류 { get; set; }
    public string 주_직무_대분류 { get; set; }
    public string 주_직무_소분류 { get; set; }
    public string 보조_직무_대분류 { get; set; }
    public string 보조_직무_소분류 { get; set; }
    public string AM { get; set; }
    public string SM { get; set; }
    public string 최초등록일 { get; set; }
    public string 최종수정일 { get; set; }
    public string 종료일 { get; set; }
    public string 채용후보자정보 { get; set; }
    
  }

  public class ProjectRepListExcelModel
  {
    public string 상태 { get; set; }
    public string 업체명 { get; set; }
    public string 제목 { get; set; }
    public string AM { get; set; }
    public string Searcher { get; set; }
    public string 생성일 { get; set; }
  }

  public class AllProjectListExcelModel
  {
    public string 타입 { get; set; }
    public string 상태 { get; set; }
    public string 업체명 { get; set; }
    public string 제목 { get; set; }
    public string 진행상태 { get; set; }
    public string AM { get; set; }
    public string Searcher { get; set; }
    public string 산업 { get; set; }
    public string 생성일 { get; set; }
  }

  public class ProjectProgressListRawExcelModel
  {
    //▶ 회사 / 이름 / 성별 / 생년 / 학력 / 경력 / 참고사항 / 후보자메모
    public string kor_name { get; set; }
    public string gender { get; set; }
    public int? state { get; set; }
    public string state_name { get; set; }
    public string birth_date { get; set; }
    public string edu { get; set; }
    public string exp { get; set; }
    public string comment { get; set; }
    public string can_memo { get; set; }

  }

  public class ProjectProgressListExcelModel
  {
    //▶ 회사 / 이름 / 성별 / 생년 / 학력 / 경력 / 참고사항 / 후보자메모
    public string 이름 { get; set; }
    public string 성별 { get; set; }
    public string 진행상황 { get; set; }
    public string 생년 { get; set; }
    public string 학력 { get; set; }
    public string 경력 { get; set; }
    public string 참고사항 { get; set; }
    public string 후보자메모 { get; set; }
  }
}
