using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Univision.Main.Models.DashBoard
{
    public class DashBoardTopSummaryModel
    {
        public int rCandidateCnt { get; set; }
        public int intvCandidateCnt { get; set; }
        public int sCandidateCnt { get; set; }
        public int candidateCnt { get; set; }
        public int candidateUdCnt { get; set; }
        public int pjtCnt { get; set; }
        public int MemoCnt { get; set; }
        //public int attendCnt { get; set; }
        //public int nowMonthDateCnt { get; set; }
        public double usedVacation { get; set; }
        public double totalVacation { get; set; }

        public string contract_update_date { get; set; }
        public string kr_intro_update_date { get; set; }
        public string en_intro_update_date { get; set; }
  }
}