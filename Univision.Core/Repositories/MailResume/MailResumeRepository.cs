using Dapper;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Univision.Core.Models.DTO;
using Univision.Security;
using Univision.Core.Repositories;
using Univision.Core.Models.DTO.Response.Candidate;

namespace Univision.Core.Repositories.MailResume
{
  public class MailResumeRepository : BaseRepository
  {

    public async Task<List<mail_resume>> SelectReadyMailResumeListAsync(int count = 100, string order = "asc")
    {
      try
      {
        List<mail_resume> list = new List<mail_resume>();

        using (IDbConnection con = new SqlConnection(base.BaseConnectionString))
        {
          con.Open();
          
          if (String.IsNullOrEmpty(order))
          {
            order = " ASC ";
          }

          if (count <= 0)
          {
            count = 1;
          }

          string selectQuery = @" 
SELECT top (@count) A.*
FROM   mail_resume A LEFT JOIN mail_resume_gpt B
                     ON A.dv_timestamp = B.dv_timestamp
WHERE  A.dv_del_yn = 'N'
AND    A.dv_update_state = 'N'
AND    A.dd_email_chk is not null
AND    A.dd_rcv_id_chk is not null
AND    ISNULL(A.no_resume, 'N') <> 'Y'
AND    B.dv_timestamp is null
ORDER BY A.dd_rcv " + order;


          DynamicParameters param = new DynamicParameters();
          param.Add("@count", count, DbType.Int32);
          //param.Add("@order", order, DbType.String);

          var ret = await con.QueryAsync<mail_resume>(selectQuery, param);

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

    public async Task<List<mail_resume>> SelectReadyMailResumeListAsyncDebug(int count = 100, string order = "asc")
    {
      try
      {
        List<mail_resume> list = new List<mail_resume>();

        using (IDbConnection con = new SqlConnection(base.BaseConnectionString))
        {
          con.Open();

          if (String.IsNullOrEmpty(order))
          {
            order = " ASC ";
          }

          if (count <= 0)
          {
            count = 1;
          }

          string selectQuery = @" 
SELECT top (@count) A.*
FROM   mail_resume A LEFT JOIN mail_resume_gpt B
                     ON A.dv_timestamp = B.dv_timestamp
WHERE  A.dv_timestamp = '0000000016238BDC45D7EC489B6BEB7A619A1A8E070060647426C627B642911EA6F7286A5B0B00000000010C000060647426C627B642911EA6F7286A5B0B00034FCC4F0E0000'
ORDER BY dn_idx " + order;


          DynamicParameters param = new DynamicParameters();
          param.Add("@count", count, DbType.Int32);
          //param.Add("@order", order, DbType.String);

          var ret = await con.QueryAsync<mail_resume>(selectQuery, param);

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

    public async Task<List<MailResumeData>> SelectUserMailResumeListAsync(MailResumeSearchModel search)
    {
      try
      {
        List<MailResumeData> list = new List<MailResumeData>();

        using (IDbConnection con = new SqlConnection(base.BaseConnectionString))
        {
          con.Open();

          string selectQuery = @" 
select a.dv_timestamp
       , a.dd_rcv
       , a.dv_snd_name
       , a.dv_snd_addr
       , a.dv_rcv_name
       , a.dv_subject
       , c.c_seq as candidate_seq
       , c.kor_name as candidate_name
       , CONVERT(VARCHAR(04), c.birth_date, 121) as birth_year
       , CASE c.gender WHEN 1 THEN '남' WHEN 2 THEN '여' WHEN 0 THEN '모름' ELSE '' END as gender
       , a.dv_update_state
       , a.dv_del_yn
       , a.dv_del_reason
from mail_resume a left join uv_user u
                   on a.dv_rcv_id = u.user_id
                   left join candidate c
                   on a.dv_cd_no = c.c_seq
                   inner join mail_resume_gpt g
                   on a.dv_timestamp = g.dv_timestamp
where a.dd_email_chk is not null
and   a.dd_rcv_id_chk is not null
and   a.dv_worker_id is null
and   a.dv_del_yn = 'N' 
and   isnull(a.dv_del_reason, '') = ''
and   a.dv_update_state = 'N'
and   u.uv_seq = @uv_seq
and   a.dd_rcv > dateadd(M, -1, getdate())
order by a.dd_rcv desc
";

          DynamicParameters param = new DynamicParameters();
          param.Add("@uv_seq", search.uv_seq, DbType.String);

          var ret = await con.QueryAsync<MailResumeData>(selectQuery, param);

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

    public async Task<List<mail_resume_file>> SelectReadyMailResumeFileListAsync(string timestamp)
    {
      try
      {
        List<mail_resume_file> list = new List<mail_resume_file>();

        using (IDbConnection con = new SqlConnection(base.BaseConnectionString))
        {
          con.Open();

          string selectQuery = @" 
SELECT *, replace(replace(dv_file_path, 'Y:\', '/UploadedFiles/Old/resume_from_mail/'), '\', '/') as file_path,
replace(dv_file_path, 'Y:\', 'D:\UploadFolder\OLD\resume_from_mail\') as file_path2
FROM mail_resume_file
WHERE dv_timestamp = @timestamp
ORDER BY dn_seq
";



          DynamicParameters param = new DynamicParameters();
          param.Add("@timestamp", timestamp, DbType.String);

          var ret = await con.QueryAsync<mail_resume_file>(selectQuery, param);

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


    public async Task<MailResumeData> SelectReadyMailOneAsync(string timestamp)
    {
      try
      {

        using (IDbConnection con = new SqlConnection(base.BaseConnectionString))
        {
          con.Open();

          string selectQuery = @" 
SELECT A.*, c.c_seq as candidate_seq
       , c.kor_name as candidate_name
       , CONVERT(VARCHAR(04), c.birth_date, 121) as birth_year
       , CASE c.gender WHEN 1 THEN '남' WHEN 2 THEN '여' WHEN 0 THEN '모름' ELSE '' END as gender
       , G.gpt_result
FROM  MAIL_RESUME A LEFT JOIN CANDIDATE C
                    ON A.dv_cd_no = C.c_seq
                    LEFT JOIN MAIL_RESUME_GPT G
                    ON A.dv_timestamp = G.dv_timestamp
WHERE A.dv_timestamp = @timestamp
";

          DynamicParameters param = new DynamicParameters();
          param.Add("@timestamp", timestamp, DbType.String);

          var ret = await con.QueryAsync<MailResumeData>(selectQuery, param);

          var data = ret.FirstOrDefault();

          con.Close();

          return data;
        }
      }
      catch (Exception e)
      {
        throw e;
      }
    }





  }
}
