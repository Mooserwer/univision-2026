using Dapper;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using Univision.Core.Models;
using Univision.Core.Models.DTO;
using Univision.Core.Models.DTO.Excel;
using Univision.Core.Models.DTO.Request.Project;
using Univision.Core.Models.DTO.Response.Project;
using Univision.Core.Models.HomePage;

namespace Univision.Core.Repositories
{
  public class InvoiceRepository : BaseRepository
  {
    public async Task<int> SelectHireCandidateCnt(int p_seq)
    {
      try
      {
        int hire_cnt = 0;

        using (IDbConnection con = new SqlConnection(base.BaseConnectionString))
        {
          con.Open();

          string selectQuery = @" 
SELECT COUNT(*) as cnt
FROM PROJECT A INNER JOIN (SELECT ROW_NUMBER() OVER(PARTITION BY pic_seq ORDER BY prc_seq DESC) AS l, *
                           FROM PJT_RECANDIDATE_HISTORY
						               WHERE p_seq = @p_seq) B
				       ON  A.p_seq = B.p_seq
				       AND B.l = 1
				       AND B.state = 80
               AND B.is_no_invoice = 0
               LEFT JOIN PJT_INVOICE_INFO C
               ON A.p_seq = C.p_seq
               AND B.prc_seq = C.prc_seq
               AND C.is_deleted = 0
               AND C.invoice_type in (0, 2)
               LEFT JOIN PJT_INVOICE_INFO D
               ON A.p_seq = D.p_seq
               AND C.pii_seq = D.pre_pii
               AND D.is_deleted = 0
               AND D.invoice_type in (5)
WHERE A.p_seq = @p_seq
AND (C.pii_seq is null OR (C.pii_seq is not null AND D.pii_seq is not null))";


          DynamicParameters param = new DynamicParameters();
          param.Add("@p_seq", p_seq, DbType.Int32);

          var ret = await con.QueryAsync<int>(selectQuery, param);

          hire_cnt = ret.FirstOrDefault();

          con.Close();

          return hire_cnt;
        }
      }
      catch (Exception e)
      {
        throw e;
      }
    }


    public async Task<int> SelectInvoiceCnt(int p_seq, int invoice_type = 0, int is_sent = 0)
    {
      try
      {
        int hire_cnt = 0;

        using (IDbConnection con = new SqlConnection(base.BaseConnectionString))
        {
          con.Open();

          string selectQuery = @" 
SELECT COUNT(*) as cnt
FROM PROJECT A INNER JOIN PJT_INVOICE_INFO C
               ON A.p_seq = C.p_seq
               AND C.is_deleted = 0
               AND C.invoice_type = @invoice_type
               LEFT JOIN PJT_INVOICE_INFO D
               ON A.p_seq = D.p_seq
               AND C.pii_seq = D.pre_pii
               AND D.is_deleted = 0
               AND D.invoice_type in (4, 5)
WHERE A.p_seq = @p_seq
AND D.pii_seq is null";
          if (is_sent == 1)
          {
            selectQuery += @" AND C.send_dt is not null ";
          }

          DynamicParameters param = new DynamicParameters();
          param.Add("@p_seq", p_seq, DbType.Int32);
          param.Add("@invoice_type", invoice_type, DbType.Int32);

          var ret = await con.QueryAsync<int>(selectQuery, param);

          hire_cnt = ret.FirstOrDefault();

          con.Close();

          return hire_cnt;
        }
      }
      catch (Exception e)
      {
        throw e;
      }
    }

    public async Task<List<pjt_invoice_info>> SelectInvoiceListAsyncByProject(int p_seq, bool is_show_cancel_refund = false)
    {
      try
      {
        List<pjt_invoice_info> list = new List<pjt_invoice_info>();

        using (IDbConnection con = new SqlConnection(base.BaseConnectionString))
        {
          con.Open();

          string selectQuery = @" 
SELECT C.*
FROM PROJECT A INNER JOIN PJT_INVOICE_INFO C
               ON A.p_seq = C.p_seq
               AND C.is_deleted = 0
               LEFT JOIN PJT_INVOICE_INFO D
               ON A.p_seq = D.p_seq
               AND C.pii_seq = D.pre_pii
               AND D.is_deleted = 0
               AND D.invoice_type in (4, 5)
WHERE A.p_seq = @p_seq
";
          if (!is_show_cancel_refund)
          {
            selectQuery += @"
AND D.pii_seq is null
AND C.invoice_type IN (0, 1, 2, 3)
";

          }
          else
          {

          }


          DynamicParameters param = new DynamicParameters();
          param.Add("@p_seq", p_seq, DbType.Int32);

          var ret = await con.QueryAsync<pjt_invoice_info>(selectQuery, param);

          list = ret.ToList();

          con.Close();

          return list;
        }
      }
      catch (Exception e)
      {
        throw e;
      }
    }

    public async Task<List<ProjectInvoiceChargeModel>> SelectInvoiceSalesListAsync(int pii_seq)
    {
      try
      {
        List<ProjectInvoiceChargeModel> list = new List<ProjectInvoiceChargeModel>();

        using (IDbConnection con = new SqlConnection(base.BaseConnectionString))
        {
          con.Open();

          string selectQuery = @" 
SELECT A.*, U.name, U.ud_seq
FROM PJT_INVOICE_SALES A LEFT JOIN UV_USER U
                         ON A.uv_seq = U.uv_seq
WHERE A.pii_seq = @pii_seq
";
          

          

          DynamicParameters param = new DynamicParameters();
          param.Add("@pii_seq", pii_seq, DbType.Int32);

          var ret = await con.QueryAsync<ProjectInvoiceChargeModel>(selectQuery, param);

          list = ret.ToList();

          con.Close();

          return list;
        }
      }
      catch (Exception e)
      {
        throw e;
      }
    }
  }
}
