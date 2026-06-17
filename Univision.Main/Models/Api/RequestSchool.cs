using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Univision.Main.Models.Api
{
    public class RequestSchool
    {
        public string gubun { get; set; }

        /// <summary>
        /// 지역
        /// </summary>
        public string region { get; set; }

        /// <summary>
        /// 학교유형1
        /// </summary>
        public string sch1 { get; set; }

        /// <summary>
        /// 학교유형2
        /// </summary>
        public string sch2 { get; set; }

        /// <summary>
        /// 설립유형 (대학교 : 국립, 사립, 공립)
        /// </summary>
        public string est { get; set; }

        /// <summary>
        /// 현재 페이지
        /// </summary>
        public string thisPage { get; set; } = "1";

        /// <summary>
        /// 한 페이지당 건수
        /// </summary>
        public string perPage { get; set; } = "10";

        /// <summary>
        /// 검색어(초등학교, 중학교, 고등학교, 특수학교, 대안학교, 대학교)
        /// </summary>
        public string searchSchulNm { get; set; }
    }

}