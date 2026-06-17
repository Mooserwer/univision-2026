using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Univision.Main.Models.Api
{
    public class ResponseCompany
    {
        /// <summary>
        /// 자료생성년월
        /// </summary>
        public string dataCrtYm { get; set; }

        /// <summary>
        /// 식별번호
        /// </summary>
        public string seq { get; set; }

        /// <summary>
        /// 사업장명
        /// </summary>
        public string wkplNm { get; set; }

        /// <summary>
        /// 사업자등록번호
        /// </summary>
        public string bzowrRgstNo { get; set; }

        /// <summary>
        /// 사업장도로명상세주소
        /// </summary>
        public string wkplRoadNmDtlAddr { get; set; }

        /// <summary>
        /// 사업장가입상태코드
        /// </summary>
        public string wkplJnngStcd { get; set; }

        /// <summary>
        /// 1:법인, 2:개인
        /// </summary>
        public string wkplStylDvcd { get; set; }

        /// <summary>
        /// 시도(행정자치부 법정동 주소코드 참조)
        /// </summary>
        public string ldongAddrMgplDgCd { get; set; }

        /// <summary>
        /// 시군구(행정자치부 법정동 주소코드 참조)
        /// </summary>
        public string ldongAddrMgplSgguCd { get; set; }

        /// <summary>
        /// 읍면동(행정자치부 법정동 주소코드 참조)
        /// </summary>
        public string ldongAddrMgplSgguEmdCd { get; set; }

        /// <summary>
        /// 직원수(가입자수)
        /// </summary>
        public int jnngpCnt { get; set; }
    }
}