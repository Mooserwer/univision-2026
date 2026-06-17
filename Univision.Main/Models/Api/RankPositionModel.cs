using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Univision.Core.Models.DTO;

namespace Univision.Main.Models.Api
{
    public class RankPositionModel
    {
        public int r_code { get; set; }
        public int p_code { get; set; }

        public List<code_rank> rankList { get; set; }
        public List<code_position> positionList { get; set; }
    }
}