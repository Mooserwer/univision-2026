using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Univision.Core.Models.DTO.Response.Project
{
  public class PjtProgressCntModel
  {
    public int allCnt { get; set; }
    public int hireOkCnt { get; set; }
    public int hireCnt { get; set; }
    public int interviewOkCnt { get; set; }
    public int interviewCnt { get; set; }
    public int paperOkCnt { get; set; }
    public int recommandCnt { get; set; }
    public int interestCnt { get; set; }
    public int failCnt { get; set; }
    public int selfdropCnt { get; set; }
    public int no_intrestCnt { get; set; }
  }
}
