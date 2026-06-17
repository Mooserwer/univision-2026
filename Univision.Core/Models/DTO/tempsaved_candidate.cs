using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Univision.Core.Models.DTO
{
    //후보자 테이블
    [Table("tempsaved_candidate")]
    public partial class tempsaved_candidate
    {
        /// <summary>
        /// pk
        /// <summary>
        [Key]
        public int c_seq { get; set; }

        /// <summary>
        /// uv_user 담당자 pk
        /// <summary>
        public int? manager_seq { get; set; }

        /// <summary>
        /// 한국명
        /// <summary>
        public string kor_name { get; set; }

        /// <summary>
        /// 영문명
        /// <summary>
        public string eng_name { get; set; }

        /// <summary>
        /// 외국인여부(0: 내국인, 1:외국인)
        /// <summary>
        public double? is_foreign { get; set; }

        /// <summary>
        /// 생년월일
        /// <summary>
        public DateTime? birth_date { get; set; }

        /// <summary>
        /// 성별(남자:1 , 여자 : 2)
        /// <summary>
        public double? gender { get; set; }

        /// <summary>
        /// 생년월일 추정 여부(예 : 1 , 아니오:0)
        /// <summary>
        public double? ex_birthdate { get; set; }

        /// <summary>
        /// 국적코드
        /// <summary>
        public string country_code { get; set; }

        /// <summary>
        /// 주소1
        /// <summary>
        public string addr1 { get; set; }

        /// <summary>
        /// 전화번호
        /// <summary>
        public string phone { get; set; }

        /// <summary>
        /// 핸드폰번호
        /// <summary>
        public string cell_phone { get; set; }

        /// <summary>
        /// 이메일1
        /// <summary>
        public string email1 { get; set; }

        /// <summary>
        /// 이메일2
        /// <summary>
        public string email2 { get; set; }

        /// <summary>
        /// 희망연봉
        /// <summary>
        public int? hope_salary { get; set; }

        public int? hope_salary2 { get; set; }

        /// <summary>
        /// 등록일
        /// <summary>
        public DateTime? create_dt { get; set; }

        /// <summary>
        /// 등록자
        /// <summary>
        public int? create_seq { get; set; }

        /// <summary>
        /// 수정일
        /// <summary>
        public DateTime? modify_dt { get; set; }

        /// <summary>
        /// 수정자
        /// <summary>
        public int? modify_seq { get; set; }

        /// <summary>
        /// 후보자 키워드(추가)
        /// <summary>
        public string keyword { get; set; }

        /// <summary>
        /// 해외거주여부(추가)
        /// <summary>
        public int? ex_addr { get; set; }

        /// <summary>
        /// 전화번호 결번여부(추가)
        /// <summary>
        public int? wrong_phone { get; set; }

        /// <summary>
        /// 전화번호2(휴대폰) 결번여부(추가)
        /// <summary>
        public int? wrong_phone2 { get; set; }

        /// <summary>
        /// 컨피덴셜 여부(추가)
        /// <summary>
        public int? is_confidential { get; set; }

        /// <summary>
        /// 인액티브 여부(추가)
        /// <summary>
        public int? is_inactive { get; set; }
        /// <summary>
        /// 등록경로(1 : 아웃써치, 2:온라인DB, 3:잡포탈, 온라인지원자, 4:아웃플레이스먼트, 5:링크드인 , 6:기타
        /// <summary>
        public int? reg_type { get; set; }

        public string sns_link1 { get; set; }
        public string sns_link2 { get; set; }
        public string sns_link3 { get; set; }


    }

    /// <summary>
    /// 리스트에서 쓰임.
    /// </summary>
    public partial class tempsaved_candidate
    {
        /// <summary>
        /// 후보자 최종 학교명
        /// </summary>
        [NotMapped]
        public string school_name { get; set; }

        /// <summary>
        /// 후보자 최종 학교 전공계열
        /// </summary>
        [NotMapped]
        public string category1_name { get; set; }

        [NotMapped]
        public int graduate_status { get; set; }

        /// <summary>
        /// 최종 회사명
        /// </summary>
        [NotMapped]
        public string company_name { get; set; }

        /// <summary>
        /// 부서명
        /// </summary>
        [NotMapped]
        public string division_name { get; set; }

        /// <summary>
        /// 재직 여부
        /// </summary>
        [NotMapped]
        public int is_work { get; set; }

        /// <summary>
        /// 직급 명
        /// </summary>
        [NotMapped]
        public string r_name { get; set; }

        /// <summary>
        /// 직책 명
        /// </summary>
        [NotMapped]
        public string p_name { get; set; }

        /// <summary>
        /// 후보자 직무
        /// </summary>
        [NotMapped]
        public string jobName { get; set; }

        /// <summary>
        /// 후보자 산업
        /// </summary>
        [NotMapped]
        public string busiName { get; set; }

        /// <summary>
        /// 국문이력서 파일 위치
        /// </summary>
        [NotMapped]
        public string kor_file_dir { get; set; }

        /// <summary>
        /// 국문이력서 파일 확장자
        /// </summary>
        [NotMapped]
        public string kor_file_ext { get; set; }

        /// <summary>
        /// 영문이력서 파일 위치
        /// </summary>
        [NotMapped]
        public string eng_file_dir { get; set; }

        /// <summary>
        /// 영문이력서 파일 확장자
        /// </summary>
        [NotMapped]
        public string eng_file_ext { get; set; }

        [NotMapped]
        public int cf_seq { get; set; }

        [NotMapped]
        public string kor_foreign_name { get; set; }

        [NotMapped]
        public string abilityDesc { get; set; }

        [NotMapped]
        public string kor_keyword { get; set; }

        [NotMapped]
        public string eng_keyword { get; set; }
        [NotMapped]
        public string manager_name { get; set; }
    }
}


