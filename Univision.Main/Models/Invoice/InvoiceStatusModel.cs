using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Univision.Core.Models.DTO;

namespace Univision.Main.Models.Invoice
{
  public class InvoiceStatusModel
  {
    public int p_seq { get; set; } = 0;

    public int pjt_type { get; set; } = 0;
    //인보이스 발행 대상자 (인보이스 발행 안된 채용후보자 수)
    public int hire_inv_cnt { get; set; } = 0;

    //선수금 인보이스 여부 ()
    public int retainer_inv_cnt { get; set; } = 0;

    //취소 또는 환불 인보이스 대상 (기존 환불 안된 인보이스 수)
    public int cancel_inv_cnt { get; set; } = 0;

  }

}