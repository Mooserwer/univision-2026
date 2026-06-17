using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Univision.Core.Models.DTO
{
    //api 회사
    [Table("api_company")]
    public partial class api_company
    {
        /// <summary>
        /// pk
        /// <summary>
        [Key]
        public int ac_seq { get; set; }

       /// <summary>
       /// 사업장명
       /// <summary>
       public string wkplNm { get; set; }

       /// <summary>
       /// 사업자 등록번호
       /// <summary>
       public string bzowrRgstNo { get; set; }

       /// <summary>
       /// 사업자도로명상세주소
       /// <summary>
       public string wkplRoadNmDtlAddr { get; set; }

       /// <summary>
       /// 사업장가입상태코드
       /// <summary>
       public float? wkplJnngStcd { get; set; }

       /// <summary>
       /// 법정동주소광역시도코드
       /// <summary>
       public string ldongAddrMgplDgCd { get; set; }

       /// <summary>
       /// 법정동주소시군구코드
       /// <summary>
       public string ldongAddrMgplSgguCd { get; set; }

       /// <summary>
       /// 법정동주소읍면동코드
       /// <summary>
       public string ldongAddrMgplSgguEmdCd { get; set; }

       /// <summary>
       /// 사업장형태구분코드
       /// <summary>
       public float? wkplStylDvcd { get; set; }

       /// <summary>
       /// 사업업종코드
       /// <summary>
       public string wkplIntpCd { get; set; }

       /// <summary>
       /// 사업장업종코드명
       /// <summary>
       public string vldtVlKrnNm { get; set; }

       /// <summary>
       /// 사업장등록일
       /// <summary>
       public string adptDt { get; set; }

       /// <summary>
       /// 사업장탈퇴일
       /// <summary>
       public string scsnDt { get; set; }

       /// <summary>
       /// 가입자수
       /// <summary>
       public string jnngpCnt { get; set; }

       /// <summary>
       /// 당월고지금액
       /// <summary>
       public decimal? crrmmNtcAmt { get; set; }

    }
}


