using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Univision.Core.Models.DTO
{
    //모바일 개인정보 항목 열람 로그
    [Table("element_open_log")]
    public partial class element_open_log
    {
        /// <summary>
        /// 로그 seq
        /// <summary>
        [Key]
        public int log_seq { get; set; }

        /// <summary>
        /// 열람 유저 uv_seq
        /// <summary>
        public int uv_seq { get; set; }

        /// <summary>
        /// 로그 타입 (1: 항목 오픈, 2 : 수정 페이지 이동)
        /// </summary>
        public int log_type { get; set; }

        /// <summary>
        /// 테이블 seq (candidate_seq, client_seq, project_seq 등)
        /// </summary>
        public int element_seq { get; set; }

        /// <summary>
        /// 열람 페이지 명(candidate, client, project 등)
        /// </summary>
        public string page_name { get; set; }

        /// <summary>
        /// 열람데이터 테이블명
        /// <summary>
        public string table_name { get; set; }

        /// <summary>
        /// 열람 항목명
        /// <summary>
        public string item_name { get; set; }

        /// <summary>
        /// 열람 일자
        /// <summary>
        public DateTime log_date { get; set; }

        /// <summary>
        /// 기타정보
        /// </summary>
        public string etc { get; set; }
    }
}
