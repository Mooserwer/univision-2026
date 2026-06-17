using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Univision.Core.Models.DTO.Request.Project
{
  public class ProjectInvoiceChargeModel
  {
    public string gubun { get; set; }
    public int uv_seq { get; set; }
    public int? ud_seq { get; set; }
    public string name { get; set; }
    public float sales_rate { get; set; }
    public double sales_money { get; set; }
    public double sales_won { get; set; }
    public string comments { get; set; }
  }
}
