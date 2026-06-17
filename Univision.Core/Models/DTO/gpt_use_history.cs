using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Univision.Core.Models.DTO
{
    [Table("gpt_use_history")]
    public class gpt_use_history
    {
        [Key]
        public int guh_seq { get; set; }

        /// <summary>용도 구분 — 1:메이크업  2:AI등록</summary>
        public byte use_type { get; set; }

        /// <summary>원본 파일명 (확장자 포함)</summary>
        public string file_path { get; set; }

        /// <summary>원본 파일 풀경로</summary>
        public string file_full_path { get; set; }

        /// <summary>요청 언어 — KR / EN (메이크업만)</summary>
        public string req_lang { get; set; }

        /// <summary>사용 GPT 모델명 (예: gpt-5.4-mini)</summary>
        public string gpt_model { get; set; }

        /// <summary>기본정보 추출 결과 JSON (NC)</summary>
        public string result_nc { get; set; }

        /// <summary>경력사항 추출 결과 JSON (OC)</summary>
        public string result_oc { get; set; }

        /// <summary>생성된 파일명 (메이크업 완료 시)</summary>
        public string output_file { get; set; }

        /// <summary>생성 파일 다운로드 경로 (메이크업 완료 시)</summary>
        public string output_path { get; set; }

        /// <summary>처리 상태 — 0:처리중  1:완료  2:실패</summary>
        public byte status { get; set; }

        /// <summary>실패 시 오류 메시지</summary>
        public string error_msg { get; set; }

        /// <summary>총 처리 소요 시간(초)</summary>
        public decimal? proc_sec { get; set; }

        /// <summary>신청자 user seq</summary>
        public int create_seq { get; set; }

        /// <summary>신청 일시</summary>
        public DateTime create_dt { get; set; }
    }
}
