using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Univision.Core.Models.DTO
{
    public class CSV_MODEL
    {
        public CSV_MODEL()
        {

        }
        public int 인덱스 { get; set; }
        public string 디비업데이트여부 { get; set; }
        public string 자료생성년월 { get; set; }
        public string 사업장명 { get; set; }
        public string 사업자등록번호 { get; set; }
        public string 사업장가입상태코드 { get; set; }
        public string 우편번호 { get; set; }
        public string 사업장지번상세주소 { get; set; }
        public string 사업장도로명상세주소 { get; set; }
        public string 고객법정동주소코드 { get; set; }
        public string 고객행정동주소코드 { get; set; }
        public string 법정동주소광역시도코드 { get; set; }
        public string 법정동주소광역시시군구코드 { get; set; }
        public string 법정동주소광역시시군구읍면동코드 { get; set; }
        public string 사업장형태구분코드 { get; set; }
        public string 사업장업종코드 { get; set; }
        public string 사업장업종코드명 { get; set; }
        public string 적용일자 { get; set; }
        public string 재등록일자 { get; set; }
        public string 탈퇴일자 { get; set; }
        public int 가입자수 { get; set; }
        public long 당월고지금액 { get; set; }
        public int 신규취득자수 { get; set; }
        public int 상실가입자수 { get; set; }
    }
}
