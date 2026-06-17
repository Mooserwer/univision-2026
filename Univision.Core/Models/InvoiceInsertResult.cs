using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Univision.Core.Models.DTO;

namespace Univision.Core.Models
{
  public class InvoiceInsertResult
  {
    public InvoiceInsertResult(int _resultCode, string _message, invoice_new _new_obj)
    {
      this.ResultCode = _resultCode;
      this.Message = _message;
      this.newObj = _new_obj;
    }

    public int ResultCode { get; set; } // 1: 성공, -1: 에러, -2: 이미 승인됨
    public string Message { get; set; }
    public invoice_new newObj { get; set; }
  }

  public class TEST_InvoiceInsertResult
  {
    public TEST_InvoiceInsertResult(int _resultCode, string _message, test_invoice_new _new_obj)
    {
      this.ResultCode = _resultCode;
      this.Message = _message;
      this.newObj = _new_obj;
    }

    public int ResultCode { get; set; } // 1: 성공, -1: 에러, -2: 이미 승인됨
    public string Message { get; set; }
    public test_invoice_new newObj { get; set; }
  }

  public class InvoiceDeletableResult
  {
    public InvoiceDeletableResult(int _resultCode, string _message, int _new_id)
    {
      this.ResultCode = _resultCode;
      this.Message = _message;
      this.newId = _new_id;
    }

    public int ResultCode { get; set; } // 1: 성공, -1: 에러, -2: 이미 승인됨
    public string Message { get; set; }
    public int newId { get; set; }
  }


}