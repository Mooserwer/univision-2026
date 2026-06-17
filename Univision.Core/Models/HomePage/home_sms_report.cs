using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Univision.Core.Models.HomePage
{
    [Table("sms_report")]
    public partial class home_sms_report
    {
        /// <summary>
        /// 발송된 메시지에 부여되는 메시지 아이디
        /// <summary>
        [Key]
        public string msg_id { get; set; }

        /// <summary>
        /// 수신자 번호(국가코드를 제외한 수신번호) ex) 1093446280
       /// <summary>
       public string to { get; set; }

        /// <summary>
        /// 수신자 국가코드
        /// <summary>
        public string to_country { get; set; }

        /// <summary>
        /// 리포트 세부 결과 코드
        /// <summary>
        public string report_code { get; set; }

        /// <summary>
        /// 리포트 세부 결과 설명
        /// <summary>
        public string report_desc { get; set; }

        /// <summary>
        /// Mobile network MCCMNC. (MCC:국가코드, MNC:이통사코드)
        /// ex) 45005 (SKT), 45006 (LGU+), 45008 (KT)
        /// <summary>
        public string net_work { get; set; }

        /// <summary>
        /// 실제 메시지 과금 건수 *Concatenated Message경우, 실제 메시지 과금 된 건수
        /// <summary>
        public string rescnt { get; set; }

        /// <summary>
        /// 단말기 도착 시간 - 한국 표준시 기준 : KST(UTC+9), YYYYMMDDHHMMSS
        /// <summary>
        public string sent_date { get; set; }

        /// <summary>
        /// 여분 필드, 클라이언트 설정 값 - request에서 설정한 ref값 전달
        /// <summary>
        public string @ref { get; set; }

        /// <summary>
        /// 생성일
        /// <summary>
        public DateTime? create_dt { get; set; }

        /// <summary>
        /// 생성자 uv_seq
        /// <summary>
        public int? create_user { get; set; }

    }
}
