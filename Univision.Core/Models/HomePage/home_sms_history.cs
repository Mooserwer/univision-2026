using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Univision.Core.Models.HomePage
{
    [Table("sms_history")]
    public partial class home_sms_history
  {
        /// <summary>
        /// sms seq
        /// <summary>
        [Key]
        public int sms_seq { get; set; }

        /// <summary>
        /// 메세지 id (api key 값)
        /// <summary>
        public string msg_id { get; set; }

        /// <summary>
        /// sms 발송타입 (0: sms / 1: lms)
        /// <summary>
        public int sms_type { get; set; }

        /// <summary>
        /// 발신 번호
        /// <summary>
        public string send_number { get; set; }

        /// <summary>
        /// 발신 타입 (0: 휴대전화, 1: 회사번호)
        /// <summary>
        public int send_type { get; set; }

        /// <summary>
        /// 수신번호
        /// <summary>
        public string receive_number { get; set; }

        /// <summary>
        /// 내용
        /// <summary>
        public string contents { get; set; }

        /// <summary>
        /// 내용 byte수
        /// <summary>
        public int contents_len { get; set; }

        /// <summary>
        /// 전송 응답 코드
        /// <summary>
        public string response_code { get; set; }

        /// <summary>
        /// 전송 응답 코드 상세
        /// </summary>
        public string response_desc { get; set; }

        /// <summary>
        /// 생성일
        /// <summary>
        public DateTime create_dt { get; set; }

        /// <summary>
        /// 생성자 uv_seq
        /// <summary>
        public int create_user { get; set; }

    }

    public partial class home_sms_history
    {
        [NotMapped]
        public string user_id { get; set; }

        [NotMapped]
        public List<string> receive_numbers { get; set; }
        
        [NotMapped]
        public string report_code { get; set; }
        
        [NotMapped]
        public string report_desc { get; set; }

        [NotMapped]
        public string report_name { get; set; }

        [NotMapped]
        public string send_name { get; set; }

        [NotMapped]
        public string send_desc { get; set; }
    }
}
