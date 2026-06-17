using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Univision.Core.Models.DTO.Response.Schedult
{
    public class ScheduleCount
    {
        public int allCnt { get; set; }
        public int personalCnt { get; set; }
        public int companyCnt { get; set; }
        public int teamCnt { get; set; }
        public int shareCnt { get; set; }
    }

    public class ScheduleType
    {
        public const int personal = 0;
        public const int company = 1;
        public const int team = 2;
        public const int share = 3;
    }
}
