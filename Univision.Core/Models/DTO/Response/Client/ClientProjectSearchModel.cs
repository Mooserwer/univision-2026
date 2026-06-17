using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Univision.Core.Models.DTO.Response.Client
{
    public class ClientProjectSearchModel
    {
        public string searchOption { get; set; } = "";
        public string searchTxt { get; set; } = "";
        public int state { get; set; } = 0;
        //관심후보 추가 시 이미 추가된 후보자가 있는 프로젝트 제외용
        public int except_c_seq { get; set; } = 0;

        public string orderOption { get; set; } = "DESC";
        public string orderTxt { get; set; } = "kor_name";

        public bool excelDown { get; set; } = false;

        public int pjt_type { get; set; } = 0;
    }
}
