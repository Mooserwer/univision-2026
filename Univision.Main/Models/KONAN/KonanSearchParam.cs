using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Univision.Main.Models.KONAN
{
    public class KonanSearchParam
    {
        public string select { get; set; }
        public string from { get; set; }
        public string where { get; set; }
        public int limit { get; set; } = 20;
    }

}