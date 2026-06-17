using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Univision.Main.Models.Api
{
    public class RequestCompany
    {
        /// <summary>
        /// 법정동주소광역시도코드
        /// </summary>
        public string ldong_addr_mgpl_dg_cd { get; set; }

        /// <summary>
        /// 법정동주소시군구코드
        /// </summary>
        public string ldong_addr_mgpl_sggu_cd { get; set; }

        /// <summary>
        /// 법정동주소읍면동코드
        /// </summary>
        public string ldong_addr_mgpl_sggu_emd_cd { get; set; }

        /// <summary>
        /// 사업장명
        /// </summary>
        public string wkpl_nm { get; set; }

        /// <summary>
        /// 사업자등록번호
        /// </summary>
        public string bzowr_rgst_no { get; set; }

        /// <summary>
        /// 페이지번호
        /// </summary>
        public int pageNo { get; set; } = 10;

        /// <summary>
        /// 행갯수
        /// </summary>
        public int numOfRows { get; set; } = 1;
    }
}