using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Univision.Core.Models.DTO
{
    //국민연금 기준 법인 사업장 테이블
    [Table("gov_api_company")]
    public partial class gov_api_company
    {
        /// <summary>
        /// pk
        /// <summary>
        [Key]
        public int G_SEQ { get; set; }

        /// <summary>
        /// 사업장명
        /// <summary>
        public string WKPL_NM { get; set; }

        /// <summary>
        /// 사업장명 trim
        /// <summary>
        public string WKPL_NM_trim { get; set; }

        /// <summary>
        /// 사업자등록번호
        /// <summary>
        public string BZOWR_RGST_NO { get; set; }

        /// <summary>
        /// 자료생성년월
        /// <summary>
        public string DATA_CRT_YM { get; set; }

        /// <summary>
        /// 사업장가입상태코드 1:등록2:탈퇴
        /// <summary>
        public string WKPL_JNNG_STCD { get; set; }

        /// <summary>
        /// 우편번호
        /// <summary>
        public string ZIP { get; set; }

        /// <summary>
        /// 사업장지번상세주소
        /// <summary>
        public string WKPL_LTNO_DTL_ADDR { get; set; }

        /// <summary>
        /// 사업장도로명상세주소
        /// <summary>
        public string WKPL_ROAD_NM_DTL_ADDR { get; set; }

        /// <summary>
        /// 사업장형태구분코드 1:법인2:개인
        /// <summary>
        public string WKPL_STYL_DVCD { get; set; }

        /// <summary>
        /// 사업장업종코드 국세청업종 코드 참조
        /// <summary>
        public string WKPL_INTP_CD { get; set; }

        /// <summary>
        /// 사업장업종코드명
        /// <summary>
        public string VLDT_VL_KRN_NM { get; set; }

        /// <summary>
        /// 적용일자
        /// <summary>
        public string ADPT_DT { get; set; }

        /// <summary>
        /// 
        /// <summary>
        public string RRG_DT { get; set; }

        /// <summary>
        /// 재등록일자
        /// <summary>
        public string SCSN_DT { get; set; }

        /// <summary>
        /// 탈퇴일자
        /// <summary>
        public int JNNGP_CNT { get; set; }

        /// <summary>
        /// 가입자수
        /// <summary>
        public long CRRMM_NTC_AMT { get; set; }

        /// <summary>
        /// 당월고지금액
        /// <summary>
        public int NW_ACQZR_CNT { get; set; }

        /// <summary>
        /// 신규취득자수
        /// <summary>
        public int LSS_JNNGP_CNT { get; set; }

    }
}
