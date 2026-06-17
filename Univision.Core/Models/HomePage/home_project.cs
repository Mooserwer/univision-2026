using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Univision.Core.Models.HomePage
{
  //프로젝트 테이블
  [Table("project")]
  public partial class home_project
  {
    /// <summary>
    /// pk
    /// <summary>
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.None)]
    public int p_seq { get; set; }

    /// <summary>
    /// 제목
    /// <summary>
    public string title { get; set; }


    /// <summary>
    /// 완료/성공/종료 시 일자
    /// </summary>
    public DateTime? close_dt { get; set; }

    /// <summary>
    /// 프로젝트 진행(재진행) 일자
    /// </summary>
    public DateTime? open_dt { get; set; }

    /// <summary>
    /// 산업코드 1단계
    /// </summary>
    public double? business_code1 { get; set; }

    /// <summary>
    /// 산업코드 2단계
    /// </summary>
    public double? business_code2 { get; set; }

    /// <summary>
    /// 외국계여부
    /// </summary>
    public int is_foreign_invest { get; set; }

    /// <summary>
    /// 직무코드 1단계
    /// </summary>
    public int job_code1 { get; set; }

    /// <summary>
    /// 직무코드 2단계
    /// </summary>
    public int job_code2 { get; set; }

    /// <summary>
    /// 학력 명
    /// <summary>
    public string edu_name { get; set; }

    /// <summary>
    /// 포지션 명
    /// <summary>
    public string position_name { get; set; }

    /// <summary>
    /// 담당업무
    /// <summary>
    public string assign_task { get; set; }

    /// <summary>
    /// 자격요건
    /// <summary>
    public string requirement { get; set; }

    /// <summary>
    /// 경력여부 (신입 : 1, 경력 : 2 , 무관 : 3)
    /// <summary>
    public double? experience_type { get; set; }

    /// <summary>
    /// 경력 년수
    /// <summary>
    public double? expreience_number { get; set; }

    /// <summary>
    /// 경력 근무지역
    /// <summary>
    public string working_area_1 { get; set; }

    /// <summary>
    /// 담당자
    /// <summary>
    public string eu_name { get; set; }

    /// <summary>
    /// 담당자메일
    /// <summary>
    public string eu_email { get; set; }

    /// <summary>
    /// 담당자 전화
    /// <summary>
    public string eu_company_phone { get; set; }

    /// <summary>
    /// 담당자 직급
    /// <summary>
    public string eu_position { get; set; }

    /// <summary>
    /// 
    /// <summary>
    public string interview_process_1 { get; set; }

    /// <summary>
    /// 등록일
    /// <summary>
    public DateTime? create_dt { get; set; }

    /// <summary>
    /// 등록자
    /// <summary>
    public int? create_user { get; set; }

    /// <summary>
    /// 수정일
    /// <summary>
    public DateTime? modify_dt { get; set; }

    /// <summary>
    /// 수정자
    /// <summary>
    public int? modify_user { get; set; }
  }

}


