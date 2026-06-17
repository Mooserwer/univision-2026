using Dapper;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Univision.Core.Models.DTO;
using Univision.Core.Models.DTO.Excel;
using Univision.Core.Models.DTO.Response.Api;
using Univision.Core.Models.DTO.Response.Client;
using Univision.Security;





namespace Univision.Core.Repositories
{
  public class ClientRepository : BaseRepository
  {

    /// <summary>
    /// 업체 기본정보
    /// </summary>
    /// <param name="c_seq"></param>
    /// <returns></returns>
    public async Task<client> SelectClientOneAsync(int c_seq)
    {
      try
      {
        using (IDbConnection con = new SqlConnection(base.BaseConnectionString))
        {
          con.Open();


          string selectQuery = @" select
    a.c_seq
	, a.kor_name kor_name
	, a.eng_name eng_name
	, a.ceo ceo
  , a.ceo_eng as ceo_eng
	, a.is_foreign_invest is_foreign_invest
	, a.addr1 addr1
	, a.addr2 addr2
	, a.is_contract is_contract
	, a.is_foreign is_foreign
  , a.is_portfolio is_portfolio
	, a.foreign_code foreign_code
	, a.country_name country_name
	, case when len(a.biz_code) = 12 then a.biz_code
            when len(a.biz_code) = 10 then left(a.biz_code, 3) + '-' + SUBSTRING(a.biz_code, 4, 2) + '-' + SUBSTRING(a.biz_code, 6, 5) 
            else a.biz_code
        end biz_code
	, a.biz_type biz_type
	, a.biz_category biz_category
	, a.biz_industry biz_industry
	, a.biz_industry_code1 AS biz_industry_code1
    , cb1.code_name1 AS biz_industry_name1
    , a.biz_industry_code2 AS biz_industry_code2
    , cb2.code_name2 AS biz_industry_name2
	, a.fix_title fix_title
	, a.homepage homepage
	, a.employee_number employee_number
	, a.sales_amount sales_amount
	, a.create_user create_user
	, b.name contact_name
	, b.gender contact_gender
	, b.email contact_email
	, b.phone contact_phone
	, b.cell_phone contact_cell_phone
	, b.division contact_division
	, b.position contact_position
	, c.name tax_name
	, c.division tax_division
	, c.email tax_email
	, c.phone tax_phone
	, c.cell_phone tax_cell_phone
	, c.deposit_email tax_deposit_email
	, c.deposit_manager tax_deposit_manager
  , d.name as create_name
	, a.biz_industry_code1 biz_industry_code1
	, a.biz_industry_code2 biz_industry_code2
	, c.ctc_seq tax_seq
	, b.cc_seq contact_seq
  , a.is_inorder
  , a.offlimit
  , a.offlimit_keyword
  , a.bd_user_seq
  , bd_u.name as bd_user_name
from
	client a left outer join client_contact b
					on a.c_seq = b.c_seq
			left outer join client_tax_contact c
					on a.c_seq = c.c_seq
            left outer join code_business_mst cb1     
                         on a.biz_industry_code1 = cb1.code1
            left outer join code_business_dtl cb2
					on a.biz_industry_code1 = cb2.code1
					and a.biz_industry_code2 = cb2.code2
            left outer join uv_user d
					on a.create_user = d.uv_seq
          left outer join uv_user bd_u
					on a.bd_user_seq = bd_u.uv_seq
where 
    a.c_seq = @c_seq
";

          DynamicParameters param = new DynamicParameters();
          param.Add("@c_seq", c_seq, DbType.Int32);

          var ret = await con.QueryAsync<client>(selectQuery, param);

          con.Close();

          return ret.FirstOrDefault();
        }
      }
      catch (Exception e)
      {
        throw e;
      }
    }

    public List<client> clientList(ClientSearchModel search, int skip, int count, out int totalCount)
    {
      try
      {
        List<client> list = new List<client>();

        using (IDbConnection con = new SqlConnection(base.BaseConnectionString))
        {
          con.Open();
          if (!String.IsNullOrEmpty(search.kor_name))
            search.kor_name = search.kor_name.Replace(" ", "");

          DynamicParameters parameters = new DynamicParameters();
          parameters.Add("@CurrentPage", skip, DbType.Int16, null);
          parameters.Add("@PageSize", count, DbType.Int16, null);
          parameters.Add("@korName", search.kor_name, DbType.String, null);
          //parameters.Add("@engName", search.eng_name.Replace(" ", ""), DbType.String, null);
          parameters.Add("@ceoName", search.ceo, DbType.String, null);
          parameters.Add("@bizNum", search.biz_num, DbType.String, null);
          parameters.Add("@contractName", search.contact_name, DbType.String, null);
          parameters.Add("@Email", search.contact_email, DbType.String, null);
          parameters.Add("@amName", search.am_name, DbType.String, null);
          parameters.Add("@bdUser", search.bd_user, DbType.String, null);
          parameters.Add("@createUser", search.create_user, DbType.String, null);
          parameters.Add("@IsForeign", search.is_foreign, DbType.Int16, null);
          parameters.Add("@isInorder", search.is_inorder, DbType.Int16, null);
          parameters.Add("@isPortfolio", search.is_portfolio, DbType.Int16, null);
          parameters.Add("@OffLimit", search.offlimit, DbType.Int16, null);
          parameters.Add("@IsContract", search.is_contract, DbType.Int16, null);
          parameters.Add("@MyClient", search.myClient, DbType.String, null);
          parameters.Add("@StartDate", search.createStart, DbType.String, null);
          parameters.Add("@EndDate", search.createEnd, DbType.String, null);
          parameters.Add("@AppIdentity", AppIdentity.user_seq, DbType.String, null);

          string query = @"  SELECT CL.c_seq AS c_seq,
       CL.kor_name AS kor_name,
       CL.eng_name AS eng_name,
       CL.ceo AS ceo,
       CL.biz_code AS biz_code,
       CL.is_contract AS is_contract,
       CL.is_foreign AS is_foreign,
       CL.is_portfolio AS is_portfolio,
       CB1.code_name1 AS business_name1,
       CB2.code_name2 AS business_name2,
       CL.offlimit AS offlimit,
       CL.main_contract AS main_contract,
       CL.create_dt AS create_dt,
       CL.modify_dt AS modify_dt,
       CL.is_inorder,
       CL.is_foreign_invest,
       CL.offlimit_keyword,
       BD_U.name as bd_user_name,
       (select count(*) from project where c_seq = cl.c_seq) totalCnt,
	     (select count(*) from project where c_seq = cl.c_seq and pjt_status = 1) progressCnt,
	     (select count(*) from project where c_seq = cl.c_seq and pjt_status = 2) waitCnt,
		   (select count(*) from project where c_seq = cl.c_seq and pjt_status = 3) failCnt,
		   (select count(*) from project where c_seq = cl.c_seq and pjt_status = 4) completeCnt,
		   (select count(*) from project where c_seq = cl.c_seq and pjt_status = 5) successCnt,
       STUFF((SELECT '/' + B.name
                FROM client_manager AS A INNER JOIN uv_user AS B
                                                 ON A.uv_seq = B.uv_seq
               WHERE A.c_seq = CL.c_seq
                 FOR XML PATH('')), 1, 1, '') AS am_name
FROM client AS CL LEFT OUTER JOIN code_business_mst AS CB1
			                   ON CL.biz_industry_code1 = CB1.code1
                  LEFT OUTER JOIN code_business_dtl AS CB2
			                   ON CL.biz_industry_code1 = CB2.code1
			                  AND CL.biz_industry_code2 = CB2.code2
                  LEFT OUTER JOIN uv_user U
			                   ON CL.create_user = U.uv_seq
                  LEFT OUTER JOIN uv_user BD_U
			                   ON CL.bd_user_seq = BD_U.uv_seq
WHERE 1=1 ";


          if (search.is_my_client)
          {
            query += " and CL.c_seq in (select c_seq from client_manager WHERE uv_seq = @AppIdentity ) ";
          }

          if (search.is_inorder != 9)
          {
            query += " AND CL.is_inorder = @isInorder";
          }

          if (search.is_portfolio != 9)
          {
            query += " AND CL.is_portfolio = @isPortfolio";
          }

          if (!string.IsNullOrWhiteSpace(search.kor_name))
          {
            query += " AND (REPLACE(CL.kor_name, ' ', '') LIKE '%' + @korName + '%' OR REPLACE(CL.eng_name, ' ', '') LIKE '%' + @korName + '%' OR CL.offlimit_keyword LIKE '%' + @korName + '%')";
          }

          if (!string.IsNullOrWhiteSpace(search.ceo))
          {
            query += " AND CL.ceo LIKE '%' + @ceoName + '%'";
          }

          if (!string.IsNullOrWhiteSpace(search.biz_num))
          {
            query += " AND CL.biz_code like '%' + @bizNum + '%'";
          }

          //if (search.myClient == "Y")
          //{
          //    query += " and cl.main_contract = @AppIdentity";
          //}

          if (!string.IsNullOrWhiteSpace(search.contact_name))
          {
            query += " AND CL.c_seq in (select c_seq from client_contact where name like '%' + @contractName + '%')";
          }

          if (!string.IsNullOrWhiteSpace(search.contact_email))
          {
            query += " AND CL.c_seq in (select c_seq from client_contact where email like '%' + @Email + '%')";
          }

          if (!string.IsNullOrWhiteSpace(search.am_name))
          {
            query += " and CL.c_seq in (select c_seq from client_manager A left join uv_user B ON A.uv_seq = B.uv_seq WHERE B.name like '%' + @amName + '%') ";
          }

          if (!string.IsNullOrWhiteSpace(search.create_user))
          {
            query += " AND U.name like '%' + @createUser + '%'";
          }

          if (!string.IsNullOrWhiteSpace(search.bd_user))
          {
            query += " AND BD_U.name like '%' + @bdUser + '%'";
          }

          if (!string.IsNullOrWhiteSpace(search.createStart) && (!string.IsNullOrWhiteSpace(search.createEnd)))
          {
            query += " AND CL.create_dt BETWEEN CONVERT(DATETIME, @StartDate + ' 00:00:00') AND CONVERT(DATETIME, @EndDate + ' 23:59:59')";
          }

          if (search.is_foreign != 9)
          {
            query += " AND CL.is_foreign_invest = @IsForeign";
          }

          if (search.offlimit == 9)
          {
            query += " AND CL.offlimit > 0";
          }
          else if (search.offlimit == 1 || search.offlimit == 2 || search.offlimit == 3)
          {
            query += " AND CL.offlimit in (@OffLimit)";
          }

          if (search.is_contract != 9 && search.is_contract == 0)
          {
            query += " AND CL.is_contract not in (1)";
          }
          else if (search.is_contract != 9 && search.is_contract == 1)
          {
            query += " AND CL.is_contract in (1)";
          }

          if (search.business.Count > 0)
          {
            string in_str_all = String.Empty;
            string in_str = String.Empty;
            string in_str_code1 = String.Empty;
            foreach (var code in search.business)
            {
              if (code.code2 <= 10000)
              {
                in_str_code1 += (!String.IsNullOrEmpty(in_str_code1) ? ", " : "") + "'" + code.code2 + "'";
              }
              else
              {
                in_str += (!String.IsNullOrEmpty(in_str) ? ", " : "") + "'" + code.code2 + "'";
              }
            }
            if (!String.IsNullOrEmpty(in_str))
            {
              in_str_all = " CL.biz_industry_code2 in (" + in_str + ") ";
            }
            if (!String.IsNullOrEmpty(in_str_code1))
            {
              in_str_all += (!String.IsNullOrEmpty(in_str_all) ? " OR " : "") + " CL.biz_industry_code1 in (" + in_str_code1 + ") ";
            }

            if (!String.IsNullOrEmpty(in_str_all))
            {
              query += " AND ( " + in_str_all + ")";
            }


          }

          //query += " GROUP BY CL.c_seq, CL.kor_name, CL.eng_name, CL.ceo, CL.biz_code, CL.is_inorder, CL.is_contract, CL.is_foreign, CB1.code_name1, CB2.code_name2, CL.offlimit, CL.main_contract, Cl.create_dt, CL.modify_dt  ";
          query += string.Format(" ORDER BY {0} {1} ", search.orderTxt, search.orderOption);
          query += " OFFSET @CurrentPage ROWS FETCH NEXT @PageSize ROWS ONLY";

          var ret = con.Query<client>(query, parameters);

          query = @"
select count(0) 
FROM client AS CL LEFT OUTER JOIN code_business_mst AS CB1
			                   ON CL.biz_industry_code1 = CB1.code1
                  LEFT OUTER JOIN code_business_dtl AS CB2
			                   ON CL.biz_industry_code1 = CB2.code1
			                  AND CL.biz_industry_code2 = CB2.code2
                  LEFT OUTER JOIN uv_user U
			                   ON CL.create_user = U.uv_seq
                  LEFT OUTER JOIN uv_user BD_U
			                   ON CL.bd_user_seq = BD_U.uv_seq
    where 1=1
";
          if (search.is_my_client)
          {
            query += " and CL.c_seq in (select c_seq from client_manager WHERE uv_seq = @AppIdentity ) ";
          }

          if (search.is_inorder != 9)
          {
            query += " AND CL.is_inorder = @isInorder";
          }

          if (search.is_portfolio != 9)
          {
            query += " AND CL.is_portfolio = @isPortfolio";
          }

          if (!string.IsNullOrWhiteSpace(search.kor_name))
          {
            query += " and (cl.kor_name like '%' + @korName + '%' or cl.eng_name like '%' + @korName + '%' )";
          }

          if (!string.IsNullOrWhiteSpace(search.eng_name))
          {
            query += " and cl.eng_name like '%' + @engName + '%'";
          }

          if (!string.IsNullOrWhiteSpace(search.ceo))
          {
            query += " and cl.ceo like '%' + @ceoName + '%'";
          }

          if (!string.IsNullOrWhiteSpace(search.biz_num))
          {
            query += " and cl.biz_code like '%' + @bizNum + '%'";
          }

          //if (search.myClient == "Y")
          //{
          //    query += " and cl.main_contract = @AppIdentity";
          //}

          if (!string.IsNullOrWhiteSpace(search.contact_name))
          {
            query += " AND CL.c_seq in (select c_seq from client_contact where name like '%' + @contractName + '%')";
          }

          if (!string.IsNullOrWhiteSpace(search.contact_email))
          {
            query += " AND CL.c_seq in (select c_seq from client_contact where email like '%' + @Email + '%')";
          }

          if (!string.IsNullOrWhiteSpace(search.am_name))
          {
            query += " and CL.c_seq in (select c_seq from client_manager A left join uv_user B ON A.uv_seq = B.uv_seq WHERE B.name like '%' + @amName + '%') ";
          }

          if (!string.IsNullOrWhiteSpace(search.create_user))
          {
            query += " and U.name like '%' + @createUser + '%'";
          }

          if (!string.IsNullOrWhiteSpace(search.bd_user))
          {
            query += " AND BD_U.name like '%' + @bdUser + '%'";
          }

          if (!string.IsNullOrWhiteSpace(search.createStart) && (!string.IsNullOrWhiteSpace(search.createEnd)))
          {
            query += " and cl.create_dt between CONVERT(DATETIME, @StartDate + ' 00:00:00') AND CONVERT(DATETIME, @EndDate + ' 23:59:59')";
          }

          if (search.is_foreign != 9)
          {
            query += " and cl.is_foreign = @IsForeign";
          }

          if (search.offlimit == 9)
          {
            query += " AND CL.offlimit > 0";
          }
          else if (search.offlimit == 1 || search.offlimit == 2 || search.offlimit == 3)
          {
            query += " AND CL.offlimit in (@OffLimit)";
          }

          if (search.is_contract != 9)
          {
            query += " and cl.is_contract = @IsContract";
          }

          if (search.business.Count > 0)
          {
            string in_str_all = String.Empty;
            string in_str = String.Empty;
            string in_str_code1 = String.Empty;
            foreach (var code in search.business)
            {
              if (code.code2 <= 10000)
              {
                in_str_code1 += (!String.IsNullOrEmpty(in_str_code1) ? ", " : "") + "'" + code.code2 + "'";
              }
              else
              {
                in_str += (!String.IsNullOrEmpty(in_str) ? ", " : "") + "'" + code.code2 + "'";
              }
            }
            if (!String.IsNullOrEmpty(in_str))
            {
              in_str_all = " CL.biz_industry_code2 in (" + in_str + ") ";
            }
            if (!String.IsNullOrEmpty(in_str_code1))
            {
              in_str_all += (!String.IsNullOrEmpty(in_str_all) ? " OR " : "") + " CL.biz_industry_code1 in (" + in_str_code1 + ") ";
            }

            if (!String.IsNullOrEmpty(in_str_all))
            {
              query += " AND ( " + in_str_all + ")";
            }


          }

          //query += " GROUP BY CL.c_seq, CL.kor_name, CL.eng_name, CL.ceo, CL.biz_code, CL.is_inorder, CL.is_contract, CL.is_foreign, CB1.code_name1, CB2.code_name2, CL.offlimit, CL.main_contract, Cl.create_dt, CL.modify_dt" +
          //query += " ) a";

          totalCount = con.QueryFirstOrDefault<int>(query, parameters);


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

    public List<client> clientListTop5()
    {
      try
      {
        List<client> list = new List<client>();

        using (IDbConnection con = new SqlConnection(base.BaseConnectionString))
        {
          con.Open();

          DynamicParameters parameters = new DynamicParameters();

          string query = @" SELECT TOP 5
	   c_seq
	  ,kor_name
	  ,biz_industry
      ,create_dt
      ,is_inorder
	  ,LTRIM(STUFF((SELECT ', ' + b.name
                      FROM   client_manager a INNER JOIN uv_user b
                                                      ON a.uv_seq = b.uv_seq
	   		         WHERE a.c_seq = aa.c_seq
                       FOR XML PATH('')),1,1,'')) AS am_name
  FROM (SELECT CL.c_seq c_seq
	          ,CL.kor_name kor_name
              ,CL.is_inorder
              ,CB.code_name2 biz_industry
              ,CL.create_dt create_dt
          FROM client AS CL LEFT OUTER JOIN client_manager AS CM
                                         ON CL.c_seq = CM.c_seq
                            LEFT OUTER JOIN uv_user AS US
                                         ON CM.uv_seq = US.uv_seq
                            LEFT OUTER JOIN code_business_dtl AS CB
                                         ON CL.biz_industry_code1 = CB.code1
                                        AND CL.biz_industry_code2 = CB.code2
         WHERE 1 = 1) AS AA
GROUP BY c_seq, kor_name, biz_industry, create_dt, is_inorder
ORDER BY create_dt DESC ";
          var ret = con.Query<client>(query, parameters);

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


    public async Task<List<ClientLIstExcelModel>> SelectClientListWithoutCountAsync(ClientSearchModel search)
    {
      try
      {
        List<ClientLIstExcelModel> list = new List<ClientLIstExcelModel>();

        using (IDbConnection con = new SqlConnection(base.BaseConnectionString))
        {
          con.Open();

          string query = @"
select	
  c_seq As '코드',
	kor_name AS '업체명',
	ceo AS 'CEO',
	biz_code AS 'BizNum',
    stuff((
                select '/' + b.name
                FROM   client_manager a inner join uv_user b
										    on a.uv_seq = b.uv_seq
			    where a.c_seq = aa.c_seq
                FOR XML PATH('')
           ),1,1,'') AS 'AM',
  biz_industry_mst AS '산업대분류',
	biz_industry AS '산업소분류',
    case when is_contract = 1 then 'Y' else 'N' end AS '계약',
	case when is_foreign = 1 then 'Y' else 'N' end AS '외국계',	
	case when offlimit = 1 then '전임직원 컨택금지' when offlimit = 2 then '유니코채용 컨택금지' when offlimit = 3 then '컨택금지(기타사유)' else 'N' end AS 'Offlimit',
  case when is_inorder = 1 then 'Inorder' when is_inorder = 2 then 'All-In-One' when is_inorder = 3 then 'BD Manager' else 'BD' end AS 'Inorder',
  case when is_portfolio = 1 then 'Y' else 'N' end AS 'PE_Portfolio',
	--main_contract,
    create_dt AS '등록일',
	modify_dt AS '수정일',
	total_prj AS '프로젝트',
    success_prj AS '성공',	
    ing_prj AS '진행중',	
	cancel_prj AS '실패'
from (
        select
	        cl.c_seq c_seq,
	        cl.kor_name kor_name,
	        cl.ceo ceo,
	        cl.biz_code biz_code,
	        cl.is_contract is_contract,
	        cl.is_foreign_invest is_foreign,
          cl.is_inorder,
          cl.is_portfolio,
	        --cl.biz_industry biz_industry,
          cm.code_name1 biz_industry_mst,
          cb.code_name2 biz_industry,
	        cl.offlimit offlimit,
	        cl.main_contract main_contract,
            cl.create_dt create_dt,
	        cl.modify_dt modify_dt,
	        (select count(*) from project where c_seq = cl.c_seq) total_prj,
	        (select count(*) from project where c_seq = cl.c_seq and pjt_status = 1) ing_prj,
	        (select count(*) from project where c_seq = cl.c_seq and pjt_status in (4, 5)) success_prj,
	        (select count(*) from project where c_seq = cl.c_seq and pjt_status in (2, 3)) cancel_prj
        from client cl  left outer join code_business_dtl cb
						    on cl.biz_industry_code1 = cb.code1
						    and cl.biz_industry_code2 = cb.code2
              left outer join code_business_mst cm
						    on cl.biz_industry_code1 = cm.code1
						    
        where
            1=1";
          if (search.is_my_client)
          {
            query += " and cm.uv_seq = @AppIdentity";
          }
          if (!string.IsNullOrWhiteSpace(search.kor_name))
          {
            query += " and (cl.kor_name like '%' + @korName + '%' or cl.eng_name like like '%' + @korName + '%') ";
          }

          if (!string.IsNullOrWhiteSpace(search.eng_name))
          {
            query += " and cl.eng_name like '%' + @engName + '%'";
          }

          if (!string.IsNullOrWhiteSpace(search.ceo))
          {
            query += " and cl.ceo like '%' + @ceoName + '%'";
          }

          if (!string.IsNullOrWhiteSpace(search.biz_num))
          {
            query += " and cl.biz_code like '%' + @bizNum + '%'";
          }

          //if (search.myClient == "Y")
          //{
          //    query += " and cl.main_contract = @AppIdentity";
          //}

          if (!string.IsNullOrWhiteSpace(search.contact_name))
          {
            query += " AND CL.c_seq in (select c_seq from client_contact where name like '%' + @contractName + '%')";
          }

          if (!string.IsNullOrWhiteSpace(search.contact_email))
          {
            query += " AND CL.c_seq in (select c_seq from client_contact where email like '%' + @Email + '%')";
          }

          if (!string.IsNullOrWhiteSpace(search.am_name))
          {
            query += " and CL.c_seq in (select c_seq from client_manager A left join uv_user B ON A.uv_seq = B.uv_seq WHERE B.name like '%' + @amName + '%') ";
          }

          if (!string.IsNullOrWhiteSpace(search.create_user))
          {
            query += " and cl.create_user like '%' + @createUser + '%'";
          }

          if (!string.IsNullOrWhiteSpace(search.createStart) && (!string.IsNullOrWhiteSpace(search.createEnd)))
          {
            query += " and cl.create_dt between CONVERT(DATETIME, @StartDate + ' 00:00:00') AND CONVERT(DATETIME, @EndDate + ' 23:59:59')";
          }

          if (search.is_foreign != 9)
          {
            query += " and cl.is_foreign = @IsForeign";
          }

          if (search.is_portfolio != 9)
          {
            query += " and cl.is_portfolio = @IsPortfolio";
          }

          if (search.offlimit != 9 && search.offlimit != 0)
          {
            query += " and cl.offlimit = @OffLimit";
          }

          if (search.is_contract != 9)
          {
            query += " and cl.is_contract = @IsContract";
          }

          if (search.is_inorder != 9)
          {
            query += " and cl.is_inorder = @IsInorder";
          }

          if (search.business.Count > 0)
          {
            string in_str_all = String.Empty;
            string in_str = String.Empty;
            string in_str_code1 = String.Empty;
            foreach (var code in search.business)
            {
              if (code.code2 <= 10000)
              {
                in_str_code1 += (!String.IsNullOrEmpty(in_str_code1) ? ", " : "") + "'" + code.code2 + "'";
              }
              else
              {
                in_str += (!String.IsNullOrEmpty(in_str) ? ", " : "") + "'" + code.code2 + "'";
              }
            }
            if (!String.IsNullOrEmpty(in_str))
            {
              in_str_all = " CL.biz_industry_code2 in (" + in_str + ") ";
            }
            if (!String.IsNullOrEmpty(in_str_code1))
            {
              in_str_all += (!String.IsNullOrEmpty(in_str_all) ? " OR " : "") + " CL.biz_industry_code1 in (" + in_str_code1 + ") ";
            }

            if (!String.IsNullOrEmpty(in_str_all))
            {
              query += " AND ( " + in_str_all + ")";
            }


          }

          query += @" ) aa ";
          query += string.Format(" ORDER BY {0} {1} ", search.orderTxt, search.orderOption);


          /*
GROUP BY
c_seq,
	kor_name,
	ceo,
	biz_code,
	is_contract,
	is_foreign,
	biz_industry,
	offlimit, 
  is_inorder,
	main_contract,
  create_dt,
	modify_dt,
	total_prj,
	ing_prj,
	success_prj,
	cancel_prj ";
          */

          DynamicParameters parameters = new DynamicParameters();
          parameters.Add("@korName", search.kor_name, DbType.String, null);
          parameters.Add("@engName", search.eng_name, DbType.String, null);
          parameters.Add("@ceoName", search.ceo, DbType.String, null);
          parameters.Add("@bizNum", search.biz_num, DbType.String, null);
          parameters.Add("@contractName", search.contact_name, DbType.String, null);
          parameters.Add("@Email", search.contact_email, DbType.String, null);
          parameters.Add("@amName", search.am_name, DbType.String, null);
          parameters.Add("@createUser", search.create_user, DbType.String, null);
          parameters.Add("@IsForeign", search.is_foreign, DbType.Int16, null);
          parameters.Add("@OffLimit", search.offlimit, DbType.Int16, null);
          parameters.Add("@IsInorder", search.is_inorder, DbType.Int16, null);
          parameters.Add("@IsContract", search.is_contract, DbType.Int16, null);
          parameters.Add("@IsPortfolio", search.is_portfolio, DbType.Int16, null);
          parameters.Add("@MyClient", search.myClient, DbType.String, null);
          parameters.Add("@StartDate", search.createStart, DbType.String, null);
          parameters.Add("@EndDate", search.createEnd, DbType.String, null);
          parameters.Add("@AppIdentity", AppIdentity.user_seq, DbType.String, null);

          var ret = await con.QueryAsync<ClientLIstExcelModel>(query, parameters);

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

    public async Task<List<ContactLIstExcelModel>> SelectContactListWithoutCountAsync(ContactSearchModel search)
    {
      try
      {
        List<ContactLIstExcelModel> list = new List<ContactLIstExcelModel>();

        using (IDbConnection con = new SqlConnection(base.BaseConnectionString))
        {
          con.Open();

          DynamicParameters parameters = new DynamicParameters();
          parameters.Add("@korName", search.kor_name, DbType.String, null);
          parameters.Add("@engName", search.eng_name, DbType.String, null);
          parameters.Add("@contactName", search.contact_name, DbType.String, null);
          parameters.Add("@contactDivision", search.contact_division, DbType.String, null);
          parameters.Add("@contactPosition", search.contact_position, DbType.String, null);
          parameters.Add("@Email", search.contact_email, DbType.String, null);
          parameters.Add("@Phone", search.contact_phone, DbType.String, null);
          parameters.Add("@amName", search.am_name, DbType.String, null);
          parameters.Add("@StartDate", search.createStart, DbType.String, null);
          parameters.Add("@EndDate", search.createEnd, DbType.String, null);
          parameters.Add("@AppIdentity", AppIdentity.user_seq, DbType.String, null);

          string query = @"
SELECT
	
	kor_name AS '업체명'
	, eng_name AS '영문명'
	, contact_name AS '이름'
	, contact_division AS '부서'
	, contact_position AS '직급'
	, contact_phone AS '연락처'
	, contact_cell_phone AS '휴대번호'
	, contact_email AS '이메일'
	, memo AS '메모'
    , stuff((
            select b.name + '/'
            FROM   client_manager a inner join uv_user b
										on a.uv_seq = b.uv_seq
			where a.c_seq = tmp.c_seq
            FOR XML PATH('')
       ),1,1,'') AS 'AM'
	, create_dt AS '등록일'
	
FROM (
	SELECT
		cc.cc_seq cc_seq
		, c.c_seq c_seq
		, c.kor_name kor_name
		, c.eng_name eng_name
		, cc.name contact_name
		, cc.division contact_division
		, cc.position contact_position
		, cc.phone contact_phone
		, cc.cell_phone contact_cell_phone
		, cc.email contact_email
		, cc.memo memo
		, cc.create_dt create_dt	
	FROM client_contact cc INNER JOIN client c
								ON cc.c_seq = c.c_seq
							left outer join client_manager cm
								ON c.c_seq = cm.c_seq
							inner join uv_user uu
								ON cm.uv_seq = uu.uv_seq
	WHERE 1=1
";


          if (!string.IsNullOrWhiteSpace(search.kor_name))
          {
            query += " and c.kor_name like '%' + @korName + '%'";
          }

          if (!string.IsNullOrWhiteSpace(search.eng_name))
          {
            query += " and c.kor_name like '%' + @engName + '%'";
          }

          if (!string.IsNullOrWhiteSpace(search.contact_email))
          {
            query += " and cc.email like '%' + @Email + '%'";
          }

          if (!string.IsNullOrWhiteSpace(search.contact_phone))
          {
            query += " and cc.phone like '%' + @Phone + '%'";
          }

          if (!string.IsNullOrWhiteSpace(search.contact_cell_phone))
          {
            query += " and cc.phone like '%' + @cellPhone + '%'";
          }

          if (!string.IsNullOrWhiteSpace(search.am_name))
          {
            query += " and cc.phone like '%' + @cellPhone + '%'";
          }

          if ((!string.IsNullOrWhiteSpace(search.createStart)) && (!string.IsNullOrWhiteSpace(search.createEnd)))
          {
            query += " and cc.create_dt between CONVERT(DATETIME, @StartDate + ' 00:00:00') AND CONVERT(DATETIME, @EndDate + ' 23:59:59')";
          }

          if (search.is_my_contact)
            query += " AND CC.create_user = @AppIdentity";

          query += @" 
GROUP BY
		cc.cc_seq
		, c.c_seq
		, c.kor_name
		, c.eng_name
		, cc.name
		, cc.division
		, cc.position
		, cc.phone
		, cc.cell_phone
		, cc.email
		, cc.memo
		, cc.create_dt
) AS tmp";
          query += string.Format(" ORDER BY {0} {1} ", search.orderTxt, search.orderOption);

          var ret = await con.QueryAsync<ContactLIstExcelModel>(query, parameters);

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
    public async Task<List<client_contract_file>> SelectContractFileListAsync(int c_seq)
    {
      try
      {
        List<client_contract_file> list = new List<client_contract_file>();

        using (IDbConnection con = new SqlConnection(base.BaseConnectionString))
        {
          con.Open();

          string queryString = @"SELECT * FROM client_contract_file WHERE remove_dt is null and c_seq = @cSeq";

          DynamicParameters parameters = new DynamicParameters();
          parameters.Add("@cSeq", c_seq, DbType.Int32);

          var ret = await con.QueryAsync<client_contract_file>(queryString, parameters);

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

    public async Task<List<client_contract_file>> SelectContractFileListAsync(int c_seq, int cc_seq)
    {
      try
      {
        List<client_contract_file> list = new List<client_contract_file>();

        using (IDbConnection con = new SqlConnection(base.BaseConnectionString))
        {
          con.Open();

          string queryString = @"
SELECT * 
FROM client_contract_file 
WHERE remove_dt is null 
AND   c_seq = @c_seq 
AND   cc_seq = @cc_seq ";

          DynamicParameters parameters = new DynamicParameters();
          parameters.Add("@c_seq", c_seq, DbType.Int32);
          parameters.Add("@cc_seq", cc_seq, DbType.Int32);

          var ret = await con.QueryAsync<client_contract_file>(queryString, parameters);

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


    public async Task<List<client_file>> SelectClientFileAsync(int cf_seq)
    {
      try
      {
        List<client_file> list = new List<client_file>();

        using (IDbConnection con = new SqlConnection(base.BaseConnectionString))
        {
          con.Open();

          string queryString = @"SELECT * FROM client_file WHERE cf_seq = @cfSeq";

          DynamicParameters parameters = new DynamicParameters();
          parameters.Add("@cfSeq", cf_seq, DbType.Int32);

          var ret = await con.QueryAsync<client_file>(queryString, parameters);

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

    public async Task<List<client_file>> SelectClientFileByOnlyCseqAsync(int c_seq)
    {
      try
      {
        List<client_file> list = new List<client_file>();

        using (IDbConnection con = new SqlConnection(base.BaseConnectionString))
        {
          con.Open();

          string queryString = @"SELECT * FROM client_file WHERE c_seq = @cSeq";

          DynamicParameters parameters = new DynamicParameters();
          parameters.Add("@cSeq", c_seq, DbType.Int32);

          var ret = await con.QueryAsync<client_file>(queryString, parameters);

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


    public string SelectClientFileInfo(int cf_seq)
    {
      try
      {
        using (IDbConnection con = new SqlConnection(base.BaseConnectionString))
        {
          con.Open();

          string queryString = @"SELECT origin_file_path FROM client_file WHERE cf_seq = @cfSeq";

          DynamicParameters parameters = new DynamicParameters();
          parameters.Add("@cfSeq", cf_seq, DbType.Int32);

          var ret = con.Query<string>(queryString, parameters);

          con.Close();

          return ret.FirstOrDefault();
        }
      }
      catch (Exception e)
      {
        throw e;
      }
    }

    public string SelectContractFileInfo(int cf_seq)
    {
      try
      {

        using (IDbConnection con = new SqlConnection(base.BaseConnectionString))
        {
          con.Open();

          string queryString = @"SELECT file_origin_path FROM client_contract_file WHERE remove_dt is null and cf_seq = @cf_seq";

          DynamicParameters parameters = new DynamicParameters();
          parameters.Add("@cf_seq", cf_seq, DbType.Int32);

          var ret = con.Query<string>(queryString, parameters);

          con.Close();

          return ret.FirstOrDefault();
        }
      }
      catch (Exception e)
      {
        throw e;
      }
    }

    public async Task<int> DeleteClientActivityAsync(int cal_seq)
    {
      try
      {
        using (IDbConnection con = new SqlConnection(base.BaseConnectionString))
        {
          con.Open();
          DynamicParameters parameters = new DynamicParameters();
          parameters.Add("@cal_seq", cal_seq, DbType.Int32);

          string query = @"delete from client_activity_log where cal_seq=@cal_seq";

          int ret = await SqlMapper.ExecuteAsync(con, query, parameters, commandType: CommandType.Text);

          con.Close();

          return ret;
        }
      }
      catch (Exception e)
      {
        throw e;
      }
    }


    public async Task<int> CreateClientContactAsync(int c_seq, string name, int gender, string division, string position, string email, string phone, string cell_phone, string memo)
    {
      try
      {
        using (IDbConnection con = new SqlConnection(base.BaseConnectionString))
        {
          con.Open();
          DynamicParameters parameters = new DynamicParameters();
          parameters.Add("@c_seq", c_seq, DbType.Int32);
          parameters.Add("@name", name, DbType.String, null);
          parameters.Add("@gender", gender, DbType.Int32);
          parameters.Add("@division", division, DbType.String, null);
          parameters.Add("@position", position, DbType.String, null);
          parameters.Add("@email", email, DbType.String, null);
          parameters.Add("@phone", phone, DbType.String, null);
          parameters.Add("@cell_phone", cell_phone, DbType.String, null);
          parameters.Add("@memo", memo, DbType.String, null);
          parameters.Add("@create_user", AppIdentity.user_seq, DbType.String, null);


          string query = @"insert into client_contact(c_seq, name, gender, email, phone, cell_phone, division, position, memo, create_dt, create_user) values(
@c_seq
, @name
, @gender
, @email
, @phone
, @cell_phone
, @division
, @position
, @memo
, getdate()
, @create_user
)";
          int ret = await SqlMapper.ExecuteAsync(con, query, parameters, commandType: CommandType.Text);

          con.Close();

          return ret;

        }
      }
      catch (Exception e)
      {
        throw e;
      }

    }


    public async Task<int> UpdateClientContactAsync(int cc_seq, int c_seq, string name, int gender, string division, string position, string email, string phone, string cell_phone, string memo, int event_user = 0)
    {
      try
      {
        using (IDbConnection con = new SqlConnection(base.BaseConnectionString))
        {
          con.Open();
          DynamicParameters parameters = new DynamicParameters();
          parameters.Add("@cc_seq", cc_seq, DbType.Int32);
          parameters.Add("@c_seq", c_seq, DbType.Int32);
          parameters.Add("@name", name, DbType.String, null);
          parameters.Add("@gender", gender, DbType.Int32);
          parameters.Add("@division", division, DbType.String, null);
          parameters.Add("@position", position, DbType.String, null);
          parameters.Add("@email", email, DbType.String, null);
          parameters.Add("@phone", phone, DbType.String, null);
          parameters.Add("@cell_phone", cell_phone, DbType.String, null);
          parameters.Add("@memo", memo, DbType.String, null);
          parameters.Add("@create_user", AppIdentity.user_seq, DbType.String, null);

          ClientLogRepository log = new ClientLogRepository();
          client_contact data = new client_contact()
          {
            c_seq = c_seq,
            cc_seq = cc_seq,
            name = name,
            gender = gender,
            email = email,
            phone = phone,
            cell_phone = cell_phone,
            division = division,
            position = position,
            memo = memo
          };
          await log.client_contact_log(data, "U", event_user, 1);


          string query = @"update client_contact set 
    c_seq = @c_seq, 
    name = @name, 
    gender = @gender, 
    email = @email, 
    phone = @phone, 
    cell_phone = @cell_phone, 
    division = @division, 
    position = @position, 
    memo = @memo, 
    modify_dt = getdate(), 
    modify_user = @create_user
where cc_seq = @cc_seq
";
          int ret = await SqlMapper.ExecuteAsync(con, query, parameters, commandType: CommandType.Text);

          con.Close();

          return ret;

        }
      }
      catch (Exception e)
      {
        throw e;
      }

    }


    public async Task<int> DeleteClientContactAsync(int cc_seq, int event_user = 0)
    {
      try
      {
        using (IDbConnection con = new SqlConnection(base.BaseConnectionString))
        {
          con.Open();
          DynamicParameters parameters = new DynamicParameters();
          parameters.Add("@cc_seq", cc_seq, DbType.Int32);
          ClientLogRepository log = new ClientLogRepository();

          client_contact data = new client_contact()
          {
            cc_seq = cc_seq
          };
          await log.client_contact_log(data, "D", event_user, 1);
          string query = @"delete from client_contact where cc_seq=@cc_seq";

          int ret = await SqlMapper.ExecuteAsync(con, query, parameters, commandType: CommandType.Text);

          con.Close();

          return ret;
        }
      }
      catch (Exception e)
      {
        throw e;
      }
    }


    public async Task<int> CreateClientTaxAsync(int c_seq, string division, string name, string email, string phone, string cell_phone, string deposit_manager, string deposit_email)
    {
      try
      {
        using (IDbConnection con = new SqlConnection(base.BaseConnectionString))
        {
          con.Open();
          DynamicParameters parameters = new DynamicParameters();
          parameters.Add("@c_seq", c_seq, DbType.Int32);
          parameters.Add("@name", name, DbType.String, null);
          parameters.Add("@division", division, DbType.String, null);
          parameters.Add("@email", email, DbType.String, null);
          parameters.Add("@phone", phone, DbType.String, null);
          parameters.Add("@cell_phone", cell_phone, DbType.String, null);
          parameters.Add("@deposit_manager", deposit_manager, DbType.String, null);
          parameters.Add("@deposit_email", deposit_email, DbType.String, null);
          parameters.Add("@create_user", AppIdentity.user_seq, DbType.String, null);


          string query = @"insert into client_tax_contact(c_seq, division, name, email, phone, cell_phone, deposit_manager, deposit_email, create_dt, create_seq) values(
@c_seq
, @division
, @name
, @email
, @phone
, @cell_phone
, @deposit_manager
, @deposit_email
, getdate()
, @create_user
)";
          int ret = await SqlMapper.ExecuteAsync(con, query, parameters, commandType: CommandType.Text);

          con.Close();

          return ret;

        }
      }
      catch (Exception e)
      {
        throw e;
      }

    }

    public async Task<int> UpdateClientTaxAsync(int ctc_seq, int c_seq, string division, string name, string email, string phone, string cell_phone, string deposit_manager, string deposit_email)
    {
      try
      {
        using (IDbConnection con = new SqlConnection(base.BaseConnectionString))
        {
          con.Open();
          DynamicParameters parameters = new DynamicParameters();
          parameters.Add("@ctc_seq", ctc_seq, DbType.Int32);
          parameters.Add("@c_seq", c_seq, DbType.Int32);
          parameters.Add("@name", name, DbType.String, null);
          parameters.Add("@division", division, DbType.String, null);
          parameters.Add("@email", email, DbType.String, null);
          parameters.Add("@phone", phone, DbType.String, null);
          parameters.Add("@cell_phone", cell_phone, DbType.String, null);
          parameters.Add("@deposit_manager", deposit_manager, DbType.String, null);
          parameters.Add("@deposit_email", deposit_email, DbType.String, null);
          parameters.Add("@create_user", AppIdentity.user_seq, DbType.String, null);

          client_tax_contact data = new client_tax_contact()
          {
            ctc_seq = ctc_seq,
            c_seq = c_seq,
            name = name,
            division = division,
            email = email,
            phone = phone,
            cell_phone = cell_phone,
            deposit_manager = deposit_manager,
            deposit_email = deposit_email
          };

          ClientLogRepository log = new ClientLogRepository();
          await log.client_tax_contact_log(data, 1, "U", AppIdentity.user_seq);
          string query = @"update client_tax_contact set
    c_seq = @c_seq, 
    division = @division, 
    name = @name, 
    email = @email, 
    phone = @phone, 
    cell_phone = @cell_phone, 
    deposit_manager = @deposit_manager, 
    deposit_email = @deposit_email, 
    modify_dt = getdate(), 
    modify_seq = @create_user
where ctc_seq = @ctc_seq
";
          int ret = await SqlMapper.ExecuteAsync(con, query, parameters, commandType: CommandType.Text);

          con.Close();

          return ret;

        }
      }
      catch (Exception e)
      {
        throw e;
      }

    }


    public async Task<int> DeleteClientTaxAsync(int ctc_seq)
    {
      try
      {
        using (IDbConnection con = new SqlConnection(base.BaseConnectionString))
        {
          con.Open();
          DynamicParameters parameters = new DynamicParameters();
          parameters.Add("@ctc_seq", ctc_seq, DbType.Int32);

          string query = @"delete from client_tax_contact where ctc_seq=@ctc_seq";
          client_tax_contact data = new client_tax_contact()
          {
            ctc_seq = ctc_seq,
          };

          ClientLogRepository log = new ClientLogRepository();
          await log.client_tax_contact_log(data, 1, "D", AppIdentity.user_seq);
          int ret = await SqlMapper.ExecuteAsync(con, query, parameters, commandType: CommandType.Text);

          con.Close();

          return ret;
        }
      }
      catch (Exception e)
      {
        throw e;
      }
    }

    public List<client_activity_log> activityList(ClientHistorySearchModel search, int skip, int count, out int totalCount)
    {
      try
      {
        List<client_activity_log> list = new List<client_activity_log>();

        using (IDbConnection con = new SqlConnection(base.BaseConnectionString))
        {
          con.Open();

          DynamicParameters parameters = new DynamicParameters();
          parameters.Add("@CurrentPage", skip, DbType.Int16, null);
          parameters.Add("@PageSize", count, DbType.Int16, null);
          parameters.Add("@search_txt", search.search_txt, DbType.String, null);
          parameters.Add("@user_seq", AppIdentity.user_seq, DbType.Int32);


          string query = @"
SELECT CA.*,
       CL.kor_name,
       CL.eng_name,
       uu.name as create_name,
       CONVERT(VARCHAR(10), CA.create_dt, 121) as create_dt_str
FROM CLIENT_ACTIVITY_LOG CA LEFT OUTER JOIN UV_USER UU
							on ca.create_user = uu.uv_seq
							LEFT OUTER JOIN CLIENT CL
						    on ca.c_seq = cl.c_seq       
WHERE 1=1 ";

          if (!string.IsNullOrWhiteSpace(search.search_txt))
          {
            query += @" AND (cl.kor_name like '%' + @search_txt + '%' 
                              or cl.eng_name like '%' + @search_txt + '%'
                              OR uu.name LIKE '%' + @search_txt + '%' 
                              OR title LIKE '%' + @search_txt + '%' 
                              OR log_comment LIKE '%' + @search_txt + '%' 
                              ) ";
          }
          if (search.is_my_history)
            query += " AND CA.create_user = @user_seq ";


          query += @"ORDER BY CA.create_dt DESC, CA.c_seq DESC, CA.cal_seq DESC 
          OFFSET @CurrentPage ROWS FETCH NEXT @PageSize ROWS ONLY";

          var ret = con.Query<client_activity_log>(query, parameters);


          query = @"
SELECT COUNT(CA.cal_seq)
FROM CLIENT_ACTIVITY_LOG CA LEFT OUTER JOIN UV_USER UU
							on ca.create_user = uu.uv_seq
							LEFT OUTER JOIN CLIENT CL
						    on ca.c_seq = cl.c_seq       
WHERE 1=1 ";

          if (!string.IsNullOrWhiteSpace(search.search_txt))
          {
            query += @" AND (cl.kor_name like '%' + @search_txt + '%' 
                              or cl.eng_name like '%' + @search_txt + '%'
                              OR uu.name LIKE '%' + @search_txt + '%' 
                              OR title LIKE '%' + @search_txt + '%' 
                              OR log_comment LIKE '%' + @search_txt + '%' 
                              ) ";
          }

          if (search.is_my_history)
            query += " AND CA.create_user = @user_seq ";


          totalCount = con.QueryFirstOrDefault<int>(query, parameters);


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


    public async Task<List<ActivityLIstExcelModel>> SelectActivityListWithoutCountAsync(ClientSearchModel search)
    {
      try
      {
        List<ActivityLIstExcelModel> list = new List<ActivityLIstExcelModel>();
        using (IDbConnection con = new SqlConnection(base.BaseConnectionString))
        {
          con.Open();


          string query = @"
SELECT
	kor_name AS '업체명'
	, title AS '제목'
	, cast(log_comment as varchar(255)) AS '내용'
	, log_date AS  '일자'
    , stuff((
            select b.name + '^'
            FROM   client_manager a inner join uv_user b
										on a.uv_seq = b.uv_seq
			where a.c_seq = tmp.c_seq
            FOR XML PATH('')
       ),1,1,'') AS 'AM'
    , create_name AS '등록자'
    , create_dt AS '등록일'
	
FROM (
		SELECT
			ca.cal_seq cal_seq
			, ca.c_seq c_seq
            , cl.kor_name kor_name
			, ca.title title
			, ca.log_comment log_comment
			, ca.log_date log_date
			, ca.create_dt create_dt
			, uu.name create_name
			, ca.modify_dt modify_dt
			, us.name momdify_user	
		FROM client_activity_log ca inner join uv_user uu
										on ca.create_user = uu.uv_seq
									left outer join uv_user us
										on ca.modify_user = us.uv_seq
                                    inner join client cl
										on ca.c_seq = cl.c_seq
		WHERE 1=1 ";


          if (!string.IsNullOrWhiteSpace(search.kor_name))
          {
            query += " and (cl.kor_name like '%' + @korName + '%') or (cl.eng_name like '%' + @korName + '%')";
          }

          if (!string.IsNullOrWhiteSpace(search.am_name))
          {
            query += " and cl.kor_name like '%' + @amName + '%'";
          }

          if ((!string.IsNullOrWhiteSpace(search.dateStart)) && (!string.IsNullOrWhiteSpace(search.dateEnd)))
          {
            query += " and ca.log_date between @dateStart and @dateEnd";
          }

          if ((!string.IsNullOrWhiteSpace(search.createStart)) && (!string.IsNullOrWhiteSpace(search.createEnd)))
          {
            query += " and ca.create_dt between CONVERT(DATETIME, @StartDate + ' 00:00:00') AND CONVERT(DATETIME, @EndDate + ' 23:59:59')";
          }
          query += ") tmp";
          query += @" GROUP BY
	cal_seq
	, c_seq
    , kor_name
	, title
	, cast(log_comment as varchar(255))
	, log_date
	, create_dt
	, create_name
	, modify_dt
	, momdify_user	";
          query += string.Format(" ORDER BY {0} {1} ", search.orderTxt, search.orderOption);

          DynamicParameters parameters = new DynamicParameters();
          parameters.Add("@korName", search.kor_name, DbType.String, null);
          parameters.Add("@amName", search.am_name, DbType.String, null);
          parameters.Add("@dateStart", search.dateStart, DbType.String, null);
          parameters.Add("@dateEnd", search.dateEnd, DbType.String, null);
          parameters.Add("@StartDate", search.createStart, DbType.String, null);
          parameters.Add("@EndDate", search.createEnd, DbType.String, null);
          parameters.Add("@AppIdentity", AppIdentity.user_seq, DbType.String, null);

          var ret = await con.QueryAsync<ActivityLIstExcelModel>(query, parameters);

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

    public async Task<client_contract> SelectLastClientContractOneAsync(int c_seq)
    {
      try
      {
        using (IDbConnection con = new SqlConnection(base.BaseConnectionString))
        {
          con.Open();

          string queryString = @"
select TOP 1 
       A.*
FROM client_contract AS A 
WHERE A.c_seq = @c_seq 
ORDER BY A.cc_seq DESC
";
          /*
           ,CASE WHEN CC.fee_type = 'A' 
                      THEN '연봉별수수료 : ' + STUFF((SELECT ' ' + REPLACE(REPLACE(CONVERT(VARCHAR, CAST(B.start_income AS MONEY), 1), '.00', ''), ' ','')  
                                                                 + '~' + REPLACE(REPLACE(CONVERT(VARCHAR, CAST(B.end_income AS MONEY), 1), '.00', ''), ' ','') 
                                                                 + ' : ' + REPLACE(CONVERT(VARCHAR, B.percentage), ' ', '') + '%' + ' / '
                                                        FROM client_contract AS A INNER JOIN client_annual_income_rate AS B
                                                                                          ON A.c_seq = B.c_seq
                                                                                         AND A.cc_seq = B.cc_seq
                                                       WHERE A.c_seq = C.c_seq
                                                         FOR XML PATH('')), 1, 1, '') 
                      WHEN CC.fee_type = 'B' 
                      THEN '직급별수수료 : ' + STUFF((SELECT ' ' + PS.p_name  + '~' + PE.p_name + ' : ' + REPLACE(CONVERT(VARCHAR, B.rate), ' ', '') + '%' + ' / ' 
                                                        FROM client_contract AS A INNER JOIN client_position_rate AS B
                                                                                          ON A.c_seq = B.c_seq
                                                                                         AND A.cc_seq = B.cc_seq
                                                                                  INNER JOIN code_position AS PS
                                                                                          ON B.start_code_seq = PS.p_code
                                                                                  INNER JOIN code_position AS PE
                                                                                          ON B.end_code_seq = PE.p_code
                                                       WHERE A.c_seq = C.c_seq
                                                         FOR XML PATH('')), 1, 1, '') 
                      WHEN CC.fee_type = 'C'
                      THEN '고정 수수료 : ' + CONVERT(VARCHAR, CC.fix_fee_rate) + '%'
                      ELSE ''
                  END AS feeValue
           */
          DynamicParameters parameters = new DynamicParameters();
          parameters.Add("@c_seq", c_seq, DbType.Int32);

          var ret = await con.QueryAsync<client_contract>(queryString, parameters);

          con.Close();

          return ret.FirstOrDefault();
        }
      }
      catch (Exception e)
      {
        throw e;
      }
    }

    public async Task<client_contract> SelectClientContractOneAsync(int c_seq, int cc_seq)
    {
      try
      {
        using (IDbConnection con = new SqlConnection(base.BaseConnectionString))
        {
          con.Open();

          string queryString = @"
select TOP 1 
       A.*
FROM client_contract AS A 
WHERE A.c_seq = @c_seq ";
          if (cc_seq > 0)
          {
            queryString += " AND   A.cc_seq = @cc_seq ";
          }
          queryString += " ORDER BY A.cc_seq DESC ";

          DynamicParameters parameters = new DynamicParameters();
          parameters.Add("@c_seq", c_seq, DbType.Int32);
          if (cc_seq > 0)
          {
            parameters.Add("@cc_seq", cc_seq, DbType.Int32);
          }

          var ret = await con.QueryAsync<client_contract>(queryString, parameters);

          con.Close();

          return ret.FirstOrDefault();
        }
      }
      catch (Exception e)
      {
        throw e;
      }
    }

    public async Task<client> SelectClientWithDetailOneAsync(int c_seq)
    {
      try
      {
        using (IDbConnection con = new SqlConnection(base.BaseConnectionString))
        {
          con.Open();

          string queryString = @"select C.c_seq
      ,ISNULL(cc.cc_seq, 0) AS cc_seq
	  ,C.kor_name
	  ,C.eng_name
	  ,C.ceo
    ,C.ceo_eng
	  ,C.is_foreign_invest
      ,C.is_inorder
,C.offlimit
,C.offlimit_keyword
	  ,C.addr1
	  ,C.addr2
	  ,C.is_contract
	  ,C.is_foreign
    ,C.is_portfolio
	  ,C.foreign_code
	  ,C.country_name
	  ,C.biz_code
	  ,C.biz_type
	  ,C.biz_category
	  ,C.biz_industry_code1
      ,C.biz_industry_code2
	  ,C.fix_title
	  ,C.homepage
	  ,C.employee_number
	  ,C.sales_amount
      ,C.create_dt
      ,C.modify_dt
	  ,CC.contract_date
      ,CC.fee_type
      ,CC.deposit_limit
	  ,CC.guarntee_type
	  ,CC.is_construct_debut
	  ,ISNULL(cc.fix_fee_rate, 0) AS fix_fee_rate
	  ,CC.contract_comment
      ,CC.draft_contract_path    
      ,CC.manual_contract_path
      ,CC.final_contract_path
      ,CC.file_directory
	  ,(SELECT TOP 1 currency_name FROM client_annual_income_rate WHERE c_seq = C.c_seq AND cc_seq = cc.cc_seq) AS currency_name
      ,CASE WHEN CC.fee_type = 'A' 
            THEN '연봉별수수료 : ' + STUFF((SELECT ' ' + REPLACE(REPLACE(CONVERT(VARCHAR, CAST(B.start_income AS MONEY), 1), '.00', ''), ' ','')  
                                                       + '~' + REPLACE(REPLACE(CONVERT(VARCHAR, CAST(B.end_income AS MONEY), 1), '.00', ''), ' ','') 
                                                       + ' : ' + REPLACE(CONVERT(VARCHAR, B.percentage), ' ', '') + '%' + ' / '
                                              FROM client_contract AS A INNER JOIN client_annual_income_rate AS B
                                                                                ON A.c_seq = B.c_seq
                                                                               AND A.cc_seq = B.cc_seq
                                             WHERE A.c_seq = C.c_seq
                                               FOR XML PATH('')), 1, 1, '') 
            WHEN CC.fee_type = 'B' 
            THEN '직급별수수료 : ' + STUFF((SELECT ' ' + PS.p_name  + '~' + PE.p_name + ' : ' + REPLACE(CONVERT(VARCHAR, B.rate), ' ', '') + '%' + ' / ' 
                                              FROM client_contract AS A INNER JOIN client_position_rate AS B
                                                                                ON A.c_seq = B.c_seq
                                                                               AND A.cc_seq = B.cc_seq
                                                                        INNER JOIN code_position AS PS
                                                                                ON B.start_code_seq = PS.p_code
                                                                        INNER JOIN code_position AS PE
                                                                                ON B.end_code_seq = PE.p_code
                                             WHERE A.c_seq = C.c_seq
                                               FOR XML PATH('')), 1, 1, '') 
            WHEN CC.fee_type = 'C'
            THEN '고정 수수료 : ' + CONVERT(VARCHAR, CC.fix_fee_rate) + '%'
            ELSE ''
        END AS feeValue
      ,CB1.code_name1 AS business_name1
      ,CB2.code_name2 AS business_name2
      ,BD_U.name as bd_user_name
 FROM client AS C LEFT OUTER JOIN client_contract AS CC
                               ON C.c_seq = CC.c_seq
                  LEFT OUTER JOIN code_business_mst AS CB1
                               ON C.biz_industry_code1 = CB1.code1
                  LEFT OUTER JOIN code_business_dtl AS CB2
                               ON C.biz_industry_code1 = CB2.code1
                              AND C.biz_industry_code2 = CB2.code2
                  LEFT OUTER JOIN uv_user BD_U
                               ON C.bd_user_seq = BD_U.uv_seq
    
WHERE C.c_seq = @c_seq 
";
          DynamicParameters parameters = new DynamicParameters();
          parameters.Add("@c_seq", c_seq, DbType.Int32);

          var ret = await con.QueryAsync<client>(queryString, parameters);

          con.Close();

          return ret.FirstOrDefault();
        }
      }
      catch (Exception e)
      {
        throw e;
      }
    }


    public async Task<List<client_contract_file>> SelectContractFileAsync(int c_seq, string file_value)
    {
      try
      {
        List<client_contract_file> list = new List<client_contract_file>();

        using (IDbConnection con = new SqlConnection(base.BaseConnectionString))
        {
          con.Open();

          string queryString = @"SELECT
    a.file_path file_path
    , a.cf_seq cf_seq
    , b.name create_name
    , a.create_dt create_dt
FROM client_contract_file a INNER JOIN uv_user b
                                ON a.create_user = b.uv_seq
WHERE c_seq = @cSeq and file_type = @fileType
ORDER BY a.cf_seq desc
";
          DynamicParameters parameters = new DynamicParameters();
          parameters.Add("@cSeq", c_seq, DbType.Int32);
          parameters.Add("@fileType", file_value, DbType.String, null);

          var ret = await con.QueryAsync<client_contract_file>(queryString, parameters);

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


    public async Task<List<client_manager>> SelectClientAmListAsync(int c_seq)
    {
      try
      {
        List<client_manager> list = new List<client_manager>();

        using (IDbConnection con = new SqlConnection(base.BaseConnectionString))
        {
          con.Open();

          string queryString = @"select
	cm.c_seq c_seq
	, cm.uv_seq uv_seq
	, uu.name am_name
	, ud.ud_name ud_name
from client_manager cm inner join uv_user uu
							on cm.uv_seq = uu.uv_seq
						inner join uv_division ud
							on uu.ud_seq = ud.ud_seq
where cm.c_seq = @cSeq
  and uu.retire_date is null ";

          DynamicParameters parameters = new DynamicParameters();
          parameters.Add("@cSeq", c_seq, DbType.Int32);

          var ret = await con.QueryAsync<client_manager>(queryString, parameters);

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

    /// <summary>
    /// 클라이언트 계약 수수료 연봉별
    /// </summary>
    /// <param name="c_seq"></param>
    /// <param name="cc_seq"></param>
    /// <returns></returns>
    public async Task<List<client_annual_income_rate>> SelectClientAnnualIncomeRateListAsync(int c_seq, int cc_seq)
    {
      try
      {
        List<client_annual_income_rate> list = new List<client_annual_income_rate>();

        using (IDbConnection con = new SqlConnection(base.BaseConnectionString))
        {
          con.Open();

          string queryString = @" 
SELECT A.* 
       ,PS.p_name As start_income_name
       ,PE.p_name As end_income_name
FROM client_annual_income_rate A LEFT JOIN code_position AS PS
                                 ON A.start_income = PS.p_code
                                 LEFT JOIN code_position AS PE
                                 ON A.end_income = PE.p_code

WHERE A.c_seq = @c_seq 
AND A.cc_seq = @cc_seq 
ORDER BY A.start_income, A.percentage";

          DynamicParameters parameters = new DynamicParameters();
          parameters.Add("@c_seq", c_seq, DbType.Int32);
          parameters.Add("@cc_seq", cc_seq, DbType.Int32);

          var ret = await con.QueryAsync<client_annual_income_rate>(queryString, parameters);

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

    /// <summary>
    /// 클라이언트 계약 수수료 직급별
    /// </summary>
    /// <param name="c_seq"></param>
    /// <param name="cc_seq"></param>
    /// <returns></returns>
    public async Task<List<client_position_rate>> SelectClientPositionRateListAsync(int c_seq, int cc_seq)
    {
      try
      {
        List<client_position_rate> list = new List<client_position_rate>();

        using (IDbConnection con = new SqlConnection(base.BaseConnectionString))
        {
          con.Open();

          string queryString = @" SELECT * FROM client_position_rate WHERE c_seq = @c_seq AND cc_seq = @cc_seq ";

          DynamicParameters parameters = new DynamicParameters();
          parameters.Add("@c_seq", c_seq, DbType.Int32);
          parameters.Add("@cc_seq", cc_seq, DbType.Int32);

          var ret = await con.QueryAsync<client_position_rate>(queryString, parameters);

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


    public async Task<ClientPjtStatusCntModel> SelectClientPjtStatusCountAsync(ClientProjectSearchModel search, int c_seq)
    {
      try
      {
        using (IDbConnection con = new SqlConnection(base.BaseConnectionString))
        {
          con.Open();

          string queryString = @" SELECT SUM(allCnt) AS allCnt
      ,SUM(progressCnt) AS progressCnt
      ,SUM(waitCnt) AS waitCnt
      ,SUM(failCnt) AS failCnt
      ,SUM(completeCnt) AS completeCnt
      ,SUM(successCnt) AS successCnt
  FROM (SELECT COUNT(0) AS allCnt
              ,CASE WHEN P.pjt_status = 1 THEN COUNT(P.pjt_status) ELSE 0 END AS progressCnt
              ,CASE WHEN P.pjt_status = 2 THEN COUNT(P.pjt_status) ELSE 0 END AS waitCnt
              ,CASE WHEN P.pjt_status = 3 THEN COUNT(P.pjt_status) ELSE 0 END AS failCnt
              ,CASE WHEN P.pjt_status = 4 THEN COUNT(P.pjt_status) ELSE 0 END AS completeCnt
              ,CASE WHEN P.pjt_status = 5 THEN COUNT(P.pjt_status) ELSE 0 END AS successCnt
         FROM project AS P INNER JOIN Client AS C
                                   ON P.c_seq = C.c_seq
        WHERE P.c_seq = @cSeq ";

          if (!string.IsNullOrWhiteSpace(search.searchTxt))
            queryString += " AND (P.title LIKE '%' + @searchTxt + '%' OR  C.kor_name LIKE '%' + @searchTxt + '%') ";

          queryString += " GROUP BY P.pjt_status) AS A ";

          DynamicParameters parameters = new DynamicParameters();
          parameters.Add("@cSeq", c_seq, DbType.Int32);
          parameters.Add("searchTxt", search.searchTxt, DbType.String);

          var ret = await con.QueryAsync<ClientPjtStatusCntModel>(queryString, parameters);

          con.Close();

          return ret.FirstOrDefault();
        }
      }
      catch (Exception e)
      {
        throw e;
      }
    }




    public List<project> SelectCliProjectLIstAsync(int c_seq, int state, int skip, int count, out int totalCount)
    {
      try
      {
        List<project> list = new List<project>();

        using (IDbConnection con = new SqlConnection(base.BaseConnectionString))
        {
          con.Open();

          string selectQuery = @"
select a.p_seq p_seq
       , a.c_seq
       , a.title title
       , a.pjt_type pjt_type
       , a.pjt_status pjt_status
       , b.name contact_name
       , PB1.code_name1  as business_names1
       , PB2.code_name2  as business_names2
       , PJ1.code_name1  as job_names1
       , PJ2.code_name2  as job_names2
       , a.assign_task assign_task	
       , stuff((select ',' + uv.name
                FROM   pjt_manager pd inner join uv_user uv
                                      on pd.uv_seq = uv.uv_seq
                where pd.p_seq = a.p_seq
                FOR XML PATH('')),1,1,'') AS uv_name
       , stuff((select ',' + uv.name
                FROM   pjt_director pd inner join uv_user uv
                    										on pd.uv_seq = uv.uv_seq
                where pd.p_seq = a.p_seq
                FOR XML PATH('')),1,1,'') AS director
       , a.create_dt
       ,(select count(*) from pjt_recandidate where p_Seq = a.p_Seq) as interest_cnt
       ,(select count(distinct pic_seq) from pjt_recandidate_history where p_Seq = a.p_Seq and state >= 30) as recommend_cnt
       ,(select count(distinct pic_seq) from pjt_recandidate_history where p_Seq = a.p_Seq and state >= 50) as interview_cnt
       ,(select count(distinct pic_seq) from pjt_recandidate_history where p_Seq = a.p_Seq and state >= 70) as nego_cnt
from project a left outer join client_contact b
     					 on a.cc_seq = b.cc_seq
               LEFT OUTER JOIN code_business_mst PB1 
               ON A.business_code1 = PB1.code1
               LEFT OUTER JOIN code_business_dtl PB2
               ON  A.business_code1 = PB2.code1
               AND A.business_code2 = PB2.code2
               LEFT OUTER JOIN code_job_mst PJ1 
               ON A.job_code1 = PJ1.code1
               LEFT OUTER JOIN code_job_dtl PJ2 
               ON  A.job_code1 = PJ2.code1   
               AND A.job_code2 = PJ2.code2   
where a.c_seq = @c_seq";

          if (state > 0)
          {
            selectQuery += @" AND A.pjt_status = @state ";
          }

          selectQuery += @" order by p_seq desc OFFSET @currentPage ROWS FETCH NEXT @pageSize ROWS ONLY ";


          string countQuery = @"select count(*) from project where c_seq = @c_seq ";
          if (state > 0)
          {
            countQuery += @" AND pjt_status = @state ";
          }

          DynamicParameters param = new DynamicParameters();
          param.Add("@c_seq", c_seq, DbType.Int32);
          param.Add("@state", state, DbType.Int32);
          param.Add("@currentPage", skip, DbType.Int32);
          param.Add("@pageSize", count, DbType.Int32);

          DynamicParameters param2 = new DynamicParameters();
          param2.Add("@c_seq", c_seq, DbType.Int32);
          param2.Add("@state", state, DbType.Int32);

          var ret = con.Query<project>(selectQuery, param);
          totalCount = con.QueryFirstOrDefault<int>(countQuery, param2);

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

    public async Task<client> SearchClientOneAsync(string corpName, string bizNum)
    {
      try
      {
        using (IDbConnection con = new SqlConnection(base.BaseConnectionString))
        {
          con.Open();

          string queryString = @"select c_seq from client where kor_name = @corpName and left(biz_code, 6) = @bizNum";

          DynamicParameters parameters = new DynamicParameters();
          parameters.Add("@corpName", corpName, DbType.String);
          parameters.Add("@bizNum", bizNum, DbType.String);

          var ret = await con.QueryAsync<client>(queryString, parameters);

          con.Close();

          return ret.FirstOrDefault();
        }
      }
      catch (Exception e)
      {
        throw e;
      }
    }

    public async Task<List<client_activity_log>> SelectCliActivityLIstAsync(int c_seq)
    {
      try
      {
        List<client_activity_log> list = new List<client_activity_log>();

        using (IDbConnection con = new SqlConnection(base.BaseConnectionString))
        {
          con.Open();

          string selectQuery = @"SELECT CA.*, CONVERT(VARCHAR(10), log_date, 103) AS timelineDate, U.name
   FROM client_activity_log AS CA INNER JOIN uv_user AS U 
                                          ON CA.modify_user = U.uv_seq 
  WHERE CA.c_seq = @c_seq ";

          DynamicParameters param = new DynamicParameters();
          param.Add("@c_seq", c_seq, DbType.Int32);

          var ret = await con.QueryAsync<client_activity_log>(selectQuery, param);

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

    public List<client_activity_log> SelectCliActivityLIstAsync(int c_seq, int skip, int count, out int totalCount)
    {
      try
      {
        List<client_activity_log> list = new List<client_activity_log>();

        using (IDbConnection con = new SqlConnection(base.BaseConnectionString))
        {
          con.Open();

          string selectQuery = @"select
    a.cal_seq cal_seq,
	a.title title, 
	a.log_comment log_comment,
	a.log_date log_date,
    b.name name,
  a. create_user,
	a.create_dt  /*,
    (select count(0) from client_activity_log_log where c_seq = @c_seq and cal_seq = a.cal_seq ) log_cnt */

from client_activity_log a inner join uv_user b
                                    on a.create_user = b.uv_seq
where c_seq = @c_seq
order by cal_seq desc OFFSET @currentPage ROWS FETCH NEXT @pageSize ROWS ONLY
";
          string countQuery = @"select count(*) from client_activity_log where c_seq = @c_seq";

          DynamicParameters param = new DynamicParameters();
          param.Add("@c_seq", c_seq, DbType.Int32);
          param.Add("@currentPage", skip, DbType.Int32);
          param.Add("@pageSize", count, DbType.Int32);

          DynamicParameters param2 = new DynamicParameters();
          param2.Add("@c_seq", c_seq, DbType.Int32);

          var ret = con.Query<client_activity_log>(selectQuery, param);
          totalCount = con.QueryFirstOrDefault<int>(countQuery, param2);

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

    public List<client_activity_log> SelectNewCliActivityLIstAsync(int c_seq, int type, int skip, int count, out int totalCount)
    {
      try
      {
        List<client_activity_log> list = new List<client_activity_log>();

        using (IDbConnection con = new SqlConnection(base.BaseConnectionString))
        {
          con.Open();

          string selectQuery = @"select
    a.cal_seq cal_seq,
	a.title title, 
	a.log_comment log_comment,
	a.log_date log_date,
    b.name name,
  a. create_user,
	a.create_dt,
  a.act_type /*,
    (select count(0) from client_activity_log_log where c_seq = @c_seq and cal_seq = a.cal_seq ) log_cnt */

from client_activity_log a inner join uv_user b
                                    on a.create_user = b.uv_seq
where c_seq = @c_seq ";

          if (type > 0)
          {
            selectQuery += @" AND act_type = @type ";
          }

          selectQuery += @"order by cal_seq desc OFFSET @currentPage ROWS FETCH NEXT @pageSize ROWS ONLY";

          string countQuery = @"select count(*) from client_activity_log where c_seq = @c_seq";
          if (type > 0)
          {
            countQuery += @" AND act_type = @type ";
          }

          DynamicParameters param = new DynamicParameters();
          param.Add("@c_seq", c_seq, DbType.Int32);
          param.Add("@type", type, DbType.Int32);
          param.Add("@currentPage", skip, DbType.Int32);
          param.Add("@pageSize", count, DbType.Int32);

          DynamicParameters param2 = new DynamicParameters();
          param2.Add("@c_seq", c_seq, DbType.Int32);
          param2.Add("@type", type, DbType.Int32);

          var ret = con.Query<client_activity_log>(selectQuery, param);
          totalCount = con.QueryFirstOrDefault<int>(countQuery, param2);

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



    public List<client_contact> SelectCliContactLIstAsync(int c_seq, int skip, int count, out int totalCount)
    {
      try
      {
        List<client_contact> list = new List<client_contact>();

        using (IDbConnection con = new SqlConnection(base.BaseConnectionString))
        {
          con.Open();

          string selectQuery = @"select
	a.cc_seq cc_seq
	, a.name name
	, case when a.gender=1 then '남' else '여' end sex
	, a.division division
	, a.position position
    , a.phone phone
    , a.cell_phone cell_phone
	, a.email email
	, a.memo memo
	, b.name uv_name
	, a.create_dt
    , (select count(0) from client_contact_log where c_seq = @c_seq and cc_seq = a.cc_seq ) log_cnt
from
	client_contact a inner join uv_user b
							on a.create_user = b.uv_seq
where a.c_seq = @c_seq
order by a.cc_seq desc OFFSET @currentPage ROWS FETCH NEXT @pageSize ROWS ONLY
";
          string countQuery = @"select count(*) from client_contact a inner join uv_user b
                                                                                        on a.create_user = b.uv_seq
where c_seq = @c_seq";

          DynamicParameters param = new DynamicParameters();
          param.Add("@c_seq", c_seq, DbType.Int32);
          param.Add("@currentPage", skip, DbType.Int32);
          param.Add("@pageSize", count, DbType.Int32);

          DynamicParameters param2 = new DynamicParameters();
          param2.Add("@c_seq", c_seq, DbType.Int32);

          var ret = con.Query<client_contact>(selectQuery, param);
          totalCount = con.QueryFirstOrDefault<int>(countQuery, param2);

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


    public async Task<client_contact> SelectContactWithDetailOneAsync(int cc_seq)
    {
      try
      {
        using (IDbConnection con = new SqlConnection(base.BaseConnectionString))
        {
          con.Open();

          string queryString = @"select
    cc_seq
	, c_seq
	, name
	, gender
	, email
	, phone
	, cell_phone
	, division
	, position
	, memo
from client_contact 
where cc_seq = @cc_seq
";
          DynamicParameters parameters = new DynamicParameters();
          parameters.Add("@cc_seq", cc_seq, DbType.Int32);

          var ret = await con.QueryAsync<client_contact>(queryString, parameters);

          con.Close();

          return ret.FirstOrDefault();
        }
      }

      catch (Exception e)
      {
        throw e;
      }
    }


    public async Task<client_activity_log> SelectActivityWithDetailOneAsync(int cal_seq)
    {
      try
      {
        using (IDbConnection con = new SqlConnection(base.BaseConnectionString))
        {
          con.Open();

          string queryString = @"select
    cal_seq
	, c_seq
	, title
	, log_comment
	, log_date
  , act_type
    , ISNULL(is_schedule, 0) AS is_schedule
from client_activity_log 
where cal_seq = @cal_seq
";
          DynamicParameters parameters = new DynamicParameters();
          parameters.Add("@cal_seq", cal_seq, DbType.Int32);

          var ret = await con.QueryAsync<client_activity_log>(queryString, parameters);

          con.Close();

          return ret.FirstOrDefault();
        }
      }

      catch (Exception e)
      {
        throw e;
      }
    }

    public async Task<client_tax_contact> SelectTexWithDetailOneAsync(int ctc_seq)
    {
      try
      {
        using (IDbConnection con = new SqlConnection(base.BaseConnectionString))
        {
          con.Open();

          string queryString = @"select
	ctc_seq
	, c_seq
	, division
	, name
	, email
	, phone
	, cell_phone
	, deposit_manager
	, deposit_email
from client_tax_contact 
where ctc_seq = @ctc_seq
";
          DynamicParameters parameters = new DynamicParameters();
          parameters.Add("@ctc_seq", ctc_seq, DbType.Int32);

          var ret = await con.QueryAsync<client_tax_contact>(queryString, parameters);

          con.Close();

          return ret.FirstOrDefault();
        }
      }

      catch (Exception e)
      {
        throw e;
      }
    }



    public List<client_tax_contact> SelectCliTaxLIstAsync(int c_seq, int skip, int count, out int totalCount)
    {
      try
      {
        List<client_tax_contact> list = new List<client_tax_contact>();

        using (IDbConnection con = new SqlConnection(base.BaseConnectionString))
        {
          con.Open();

          string selectQuery = @"select
	a.ctc_seq ctc_seq
    , a.name
	, a.deposit_manager deposit_manager
	, a.division division
	, a.phone phone
    , a.cell_phone cell_phone
	, a.deposit_email deposit_email
    , a.email email
	, a.create_seq create_seq
	, b.name uv_name
	, a.create_dt create_dt
    , (select count(0) from client_tax_contact_log where c_seq = @c_seq and ctc_seq = a.ctc_seq) log_cnt
from
	client_tax_contact a inner join uv_user b
							on a.create_seq = b.uv_seq
where a.c_seq = @c_seq
order by a.ctc_seq desc OFFSET @currentPage ROWS FETCH NEXT @pageSize ROWS ONLY
";
          string countQuery = @"select count(*) from client_tax_contact a inner join uv_user b
                                                                                        on a.create_seq = b.uv_seq
where c_seq = @c_seq and deposit_manager is not null";

          DynamicParameters param = new DynamicParameters();
          param.Add("@c_seq", c_seq, DbType.Int32);
          param.Add("@currentPage", skip, DbType.Int32);
          param.Add("@pageSize", count, DbType.Int32);

          DynamicParameters param2 = new DynamicParameters();
          param2.Add("@c_seq", c_seq, DbType.Int32);

          var ret = con.Query<client_tax_contact>(selectQuery, param);
          totalCount = con.QueryFirstOrDefault<int>(countQuery, param2);

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



    public List<client_file> SelectCliFileLIstAsync(int c_seq, int skip, int count, out int totalCount)
    {
      try
      {
        List<client_file> list = new List<client_file>();

        using (IDbConnection con = new SqlConnection(base.BaseConnectionString))
        {
          con.Open();

          string selectQuery = @" select a.c_seq
	, a.cf_seq cf_seq
    , a.cf_type
	, CASE WHEN a.cf_type = 1 THEN '사업자 등록증' 
           WHEN a.cf_type = 2 THEN '조직도'
           WHEN a.cf_type = 3 THEN '기타'
           WHEN a.cf_type = 4 THEN '계약서'
           ELSE ''
       END AS cf_type_desc
	, a.directory
    , a.origin_file_path
    , a.file_path file_path
    , a.extension
	, b.name uv_name
	, a.create_dt create_dt
from
	client_file a inner join uv_user b
							on a.create_user = b.uv_seq
where a.c_seq = @c_seq
order by a.cf_seq desc OFFSET @currentPage ROWS FETCH NEXT @pageSize ROWS ONLY
";
          string countQuery = @"select count(*) from client_file a inner join uv_user b
                                                                                        on a.create_user = b.uv_seq
where c_seq = @c_seq";

          DynamicParameters param = new DynamicParameters();
          param.Add("@c_seq", c_seq, DbType.Int32);
          param.Add("@currentPage", skip, DbType.Int32);
          param.Add("@pageSize", count, DbType.Int32);

          DynamicParameters param2 = new DynamicParameters();
          param2.Add("@c_seq", c_seq, DbType.Int32);

          var ret = con.Query<client_file>(selectQuery, param);
          totalCount = con.QueryFirstOrDefault<int>(countQuery, param2);

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

    public List<OfferLetterFileModel> SelectCliOfferletterListAsync(int c_seq, int skip, int count, out int totalCount)
    {
      try
      {
        List<OfferLetterFileModel> list = new List<OfferLetterFileModel>();

        using (IDbConnection con = new SqlConnection(base.BaseConnectionString))
        {
          con.Open();

          string selectQuery = @" 
select a.c_seq, a.p_seq, a.title, ca.kor_name, c.file_directory, c.file_origin_path, c.file_path, c.schedule_date
from project a left join pjt_recandidate b
               on a.p_seq = b.p_seq 
               left join (select row_number() over(partition by pic_seq order by prc_seq desc) as l, *
                          from pjt_recandidate_history) C
		       on b.pic_seq = c.pic_seq
		       and c.l = 1
		       and c.state = 80
			   left join candidate ca
			   on c.c_seq = ca.c_seq
where c.pic_seq is not null
and file_path is not null
and a.c_seq = @c_seq
order by c.prc_seq desc 
OFFSET @currentPage ROWS FETCH NEXT @pageSize ROWS ONLY
";
          string countQuery = @"
select count(*) 
from project a left join pjt_recandidate b
               on a.p_seq = b.p_seq 
               left join (select row_number() over(partition by pic_seq order by prc_seq desc) as l, *
                          from pjt_recandidate_history) C
		       on b.pic_seq = c.pic_seq
		       and c.l = 1
		       and c.state = 80
			   left join candidate ca
			   on c.c_seq = ca.c_seq
where c.pic_seq is not null
and file_path is not null
and a.c_seq = @c_seq";

          DynamicParameters param = new DynamicParameters();
          param.Add("@c_seq", c_seq, DbType.Int32);
          param.Add("@currentPage", skip, DbType.Int32);
          param.Add("@pageSize", count, DbType.Int32);

          var ret = con.Query<OfferLetterFileModel>(selectQuery, param);
          totalCount = con.QueryFirstOrDefault<int>(countQuery, param);

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

    //public async Task<List<>>


    public List<client_favorite> attentionList(AttentionSearchModel search, int skip, int count, out int totalCount)
    {
      try
      {
        List<client_favorite> list = new List<client_favorite>();

        using (IDbConnection con = new SqlConnection(base.BaseConnectionString))
        {
          con.Open();

          DynamicParameters parameters = new DynamicParameters();
          parameters.Add("@CurrentPage", skip, DbType.Int16, null);
          parameters.Add("@PageSize", count, DbType.Int16, null);
          parameters.Add("@korName", search.kor_name, DbType.String, null);
          parameters.Add("@engName", search.eng_name, DbType.String, null);
          parameters.Add("@ceoName", search.ceo, DbType.String, null);
          parameters.Add("@bizNum", search.biz_num, DbType.String, null);
          parameters.Add("@contractName", search.contact_name, DbType.String, null);
          parameters.Add("@Email", search.contact_email, DbType.String, null);
          parameters.Add("@amName", search.am_name, DbType.String, null);
          parameters.Add("@createUser", search.create_user, DbType.String, null);
          parameters.Add("@IsForeign", search.is_foreign, DbType.Int16, null);
          parameters.Add("@OffLimit", search.offlimit, DbType.Int16, null);
          parameters.Add("@IsContract", search.is_contract, DbType.Int16, null);
          parameters.Add("@MyClient", search.myClient, DbType.String, null);
          parameters.Add("@StartDate", search.createStart, DbType.String, null);
          parameters.Add("@EndDate", search.createEnd, DbType.String, null);
          parameters.Add("@AppIdentity", AppIdentity.user_seq, DbType.String, null);

          string query = @"
select	
	c_seq,
	kor_name,
	ceo,
	biz_code,
	is_contract,
	is_foreign,
	biz_industry,
	offlimit,
	main_contract,
    create_dt,
	modify_dt,
	total_prj,
	ing_prj,
	success_prj,
	cancel_prj,
	stuff((
            select ' / ' + b.name
            FROM   client_manager a inner join uv_user b
										on a.uv_seq = b.uv_seq
			where a.c_seq = aa.c_seq
            FOR XML PATH('')
       ),1,1,'') AS am_name,
    (select cf_seq from client_favorite where c_seq = aa.c_seq and uv_seq = @AppIdentity) cf_seq
from (
        select
	        cl.c_seq c_seq,
	        cl.kor_name kor_name,
	        cl.ceo ceo,
	        cl.biz_code biz_code,
	        cl.is_contract is_contract,
	        cl.is_foreign is_foreign,
	        cl.biz_industry biz_industry,
	        cl.offlimit offlimit,
	        cl.main_contract main_contract,
            cl.create_dt create_dt,
	        cl.modify_dt modify_dt,
	        (select count(*) from project where c_seq = cl.c_seq) total_prj,
	        (select count(*) from project where c_seq = cl.c_seq and pjt_status = 0) ing_prj,
	        (select count(*) from project where c_seq = cl.c_seq and pjt_status = 1) success_prj,
	        (select count(*) from project where c_seq = cl.c_seq and pjt_status = 2) cancel_prj
        from client cl  left outer join client_contact cc
					        on cl.c_seq = cc.c_seq
                        left outer join client_manager cm
							on cl.c_seq = cm.c_seq
						inner join uv_user us
							on cm.uv_seq = us.uv_seq
        where
            1=1";

          if (!string.IsNullOrWhiteSpace(search.kor_name))
          {
            query += " and cl.kor_name like '%' + @korName + '%'";
          }

          if (!string.IsNullOrWhiteSpace(search.eng_name))
          {
            query += " and cl.eng_name like '%' + @engName + '%'";
          }

          if (!string.IsNullOrWhiteSpace(search.ceo))
          {
            query += " and cl.ceo like '%' + @ceoName + '%'";
          }

          if (!string.IsNullOrWhiteSpace(search.biz_num))
          {
            query += " and cl.biz_code like '%' + @bizNum + '%'";
          }

          //if (search.myClient == "Y")
          //{
          //    query += " and cl.main_contract = @AppIdentity";
          //}

          if (!string.IsNullOrWhiteSpace(search.contact_name))
          {
            query += " and cc.name like '%' + @contractName + '%'";
          }

          if (!string.IsNullOrWhiteSpace(search.contact_email))
          {
            query += " and cc.email like '%' + @Email + '%'";
          }

          if (!string.IsNullOrWhiteSpace(search.am_name))
          {
            query += " and us.name like '%' + @amName + '%'";
          }

          if (!string.IsNullOrWhiteSpace(search.create_user))
          {
            query += " and cl.create_user like '%' + @createUser + '%'";
          }

          if (!string.IsNullOrWhiteSpace(search.createStart) && (!string.IsNullOrWhiteSpace(search.createEnd)))
          {
            query += " and cl.create_dt between CONVERT(DATETIME, @StartDate + ' 00:00:00') AND CONVERT(DATETIME, @EndDate + ' 23:59:59')";
          }

          if (search.is_foreign != 0)
          {
            query += " and cl.is_foreign = @IsForeign";
          }

          if (search.offlimit != 0)
          {
            query += " and cl.offlimit = @OffLimit";
          }

          if (search.is_contract != 0)
          {
            query += " and cl.is_contract = @IsContract";
          }

          query += @" 
) aa
GROUP BY
c_seq,
	kor_name,
	ceo,
	biz_code,
	is_contract,
	is_foreign,
	biz_industry,
	offlimit,
	main_contract,
    create_dt,
	modify_dt,
	total_prj,
	ing_prj,
	success_prj,
	cancel_prj ";
          query += string.Format(" ORDER BY {0} {1} ", search.orderTxt, search.orderOption);
          query += " OFFSET @CurrentPage ROWS FETCH NEXT @PageSize ROWS ONLY";



          var ret = con.Query<client_favorite>(query, parameters);



          query = @"
select count(0) 
from (
    select cl.c_seq
    from client cl  left outer join client_contact cc
					    on cl.c_seq = cc.c_seq
                    left outer join client_manager cm
					    on cl.c_seq = cm.c_seq
				    inner join uv_user us
					    on cm.uv_seq = us.uv_seq
    where 1=1
";

          if (!string.IsNullOrWhiteSpace(search.kor_name))
          {
            query += " and cl.kor_name like '%' + @korName + '%'";
          }

          if (!string.IsNullOrWhiteSpace(search.eng_name))
          {
            query += " and cl.eng_name like '%' + @engName + '%'";
          }

          if (!string.IsNullOrWhiteSpace(search.ceo))
          {
            query += " and cl.ceo like '%' + @ceoName + '%'";
          }

          if (!string.IsNullOrWhiteSpace(search.biz_num))
          {
            query += " and cl.biz_code like '%' + @bizNum + '%'";
          }

          //if (search.myClient == "Y")
          //{
          //    query += " and cl.main_contract = @AppIdentity";
          //}

          if (!string.IsNullOrWhiteSpace(search.contact_name))
          {
            query += " and cc.name like '%' + @contractName + '%'";
          }

          if (!string.IsNullOrWhiteSpace(search.contact_email))
          {
            query += " and cc.email like '%' + @Email + '%'";
          }

          if (!string.IsNullOrWhiteSpace(search.am_name))
          {
            query += " and us.name like '%' + @amName + '%'";
          }

          if (!string.IsNullOrWhiteSpace(search.create_user))
          {
            query += " and cl.create_user like '%' + @createUser + '%'";
          }

          if (!string.IsNullOrWhiteSpace(search.createStart) && (!string.IsNullOrWhiteSpace(search.createEnd)))
          {
            query += " and cl.create_dt between CONVERT(DATETIME, @StartDate + ' 00:00:00') AND CONVERT(DATETIME, @EndDate + ' 23:59:59')";
          }

          if (search.is_foreign != 0)
          {
            query += " and cl.is_foreign = @IsForeign";
          }

          if (search.offlimit != 0)
          {
            query += " and cl.offlimit = @OffLimit";
          }

          if (search.is_contract != 0)
          {
            query += " and cl.is_contract = @IsContract";
          }

          query += " group by cl.c_seq) a";

          totalCount = con.QueryFirstOrDefault<int>(query, parameters);


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


    public List<client_contact> contactList(ContactSearchModel search, int skip, int count, out int totalCount)
    {
      try
      {
        List<client_contact> list = new List<client_contact>();

        using (IDbConnection con = new SqlConnection(base.BaseConnectionString))
        {
          con.Open();

          DynamicParameters parameters = new DynamicParameters();
          parameters.Add("@CurrentPage", skip, DbType.Int16, null);
          parameters.Add("@PageSize", count, DbType.Int16, null);
          parameters.Add("@korName", search.kor_name, DbType.String, null);
          parameters.Add("@engName", search.eng_name, DbType.String, null);
          parameters.Add("@contactName", search.contact_name, DbType.String, null);
          parameters.Add("@contactDivision", search.contact_division, DbType.String, null);
          parameters.Add("@contactPosition", search.contact_position, DbType.String, null);
          parameters.Add("@Email", search.contact_email, DbType.String, null);
          parameters.Add("@Phone", search.contact_phone, DbType.String, null);
          parameters.Add("@cellPhone", search.contact_cell_phone, DbType.String, null);
          parameters.Add("@amName", search.am_name, DbType.String, null);
          parameters.Add("@StartDate", search.createStart, DbType.String, null);
          parameters.Add("@EndDate", search.createEnd, DbType.String, null);
          parameters.Add("@AppIdentity", AppIdentity.user_seq, DbType.String, null);


          string query = @"
SELECT
	cc_seq cc_seq
	, c_seq c_seq
	, kor_name kor_name
	, eng_name eng_name
	, contact_name name
	, contact_division division
	, contact_position position
	, contact_phone phone
	, contact_cell_phone cell_phone
	, contact_email email
	, memo memo
	, create_dt create_dt
	, stuff((
            select ','+b.name
            FROM   client_manager a inner join uv_user b
										on a.uv_seq = b.uv_seq
			where a.c_seq = tmp.c_seq
            FOR XML PATH('')
       ),1,1,'') AS am_name
FROM (
	SELECT
		cc.cc_seq cc_seq
		, c.c_seq c_seq
		, c.kor_name kor_name
		, c.eng_name eng_name
		, cc.name contact_name
		, cc.division contact_division
		, cc.position contact_position
		, cc.phone contact_phone
		, cc.cell_phone contact_cell_phone
		, cc.email contact_email
		, cc.memo memo
		, cc.create_dt create_dt	
	FROM client_contact cc left outer JOIN client c
								ON cc.c_seq = c.c_seq
							left outer join client_manager cm
								ON c.c_seq = cm.c_seq
							left outer join uv_user uu
								ON cm.uv_seq = uu.uv_seq
	WHERE 1=1
";


          if (!string.IsNullOrWhiteSpace(search.kor_name))
          {
            query += " and c.kor_name like '%' + @korName + '%'";
          }

          if (!string.IsNullOrWhiteSpace(search.eng_name))
          {
            query += " and c.eng_name like '%' + @engName + '%'";
          }

          if (!string.IsNullOrWhiteSpace(search.contact_name))
          {
            query += " and cc.name like '%' + @contactName + '%'";
          }

          if (!string.IsNullOrWhiteSpace(search.contact_email))
          {
            query += " and cc.email like '%' + @Email + '%'";
          }

          if (!string.IsNullOrWhiteSpace(search.contact_phone))
          {
            query += " and (cc.phone like '%' + @Phone + '%' or cc.cell_phone like '%' + @Phone + '%')";
          }

          //if (!string.IsNullOrWhiteSpace(search.contact_cell_phone))
          //{
          //    query += " and cc.phone like '%' + @cellPhone + '%'";
          //}

          if (!string.IsNullOrWhiteSpace(search.am_name))
          {
            query += " and cc.phone like '%' + @cellPhone + '%'";
          }

          if (!string.IsNullOrWhiteSpace(search.contact_position))
          {
            query += " and cc.position like '%' + @contactPosition + '%'";
          }

          if (!string.IsNullOrWhiteSpace(search.contact_division))
          {
            query += " and cc.division like '%' + @contactDivision + '%'";
          }


          if ((!string.IsNullOrWhiteSpace(search.createStart)) && (!string.IsNullOrWhiteSpace(search.createEnd)))
          {
            query += " and cc.create_dt between CONVERT(DATETIME, @StartDate + ' 00:00:00') AND CONVERT(DATETIME, @EndDate + ' 23:59:59')";
          }

          if (search.is_my_contact)
            query += " AND CC.create_user = @AppIdentity";

          query += @" 
GROUP BY
		cc.cc_seq
		, c.c_seq
		, c.kor_name
		, c.eng_name
		, cc.name
		, cc.division
		, cc.position
		, cc.phone
		, cc.cell_phone
		, cc.email
		, cc.memo
		, cc.create_dt
) AS tmp";
          query += string.Format(" ORDER BY {0} {1} ", search.orderTxt, search.orderOption);
          query += " OFFSET @CurrentPage ROWS FETCH NEXT @PageSize ROWS ONLY";

          var ret = con.Query<client_contact>(query, parameters);

          query = @"
SELECT
	count(0)
FROM (
	SELECT
		cc.cc_seq cc_seq
		, c.c_seq c_seq
		, c.kor_name kor_name
		, c.eng_name eng_name
		, cc.name contact_name
		, cc.division contact_division
		, cc.position contact_position
		, cc.phone contact_phone
		, cc.cell_phone contact_cell_phone
		, cc.email contact_email
		, cc.memo memo
		, cc.create_dt create_dt	
	FROM client_contact cc left outer JOIN client c
								ON cc.c_seq = c.c_seq
							left outer join client_manager cm
								ON c.c_seq = cm.c_seq
							left outer join uv_user uu
								ON cm.uv_seq = uu.uv_seq
	WHERE 1=1";

          if (!string.IsNullOrWhiteSpace(search.kor_name))
          {
            query += " and c.kor_name like '%' + @korName + '%'";
          }

          if (!string.IsNullOrWhiteSpace(search.eng_name))
          {
            query += " and c.kor_name like '%' + @engName + '%'";
          }

          if (!string.IsNullOrWhiteSpace(search.contact_email))
          {
            query += " and cc.email like '%' + @Email + '%'";
          }

          if (!string.IsNullOrWhiteSpace(search.contact_phone))
          {
            query += " and cc.phone like '%' + @Phone + '%'";
          }

          if (!string.IsNullOrWhiteSpace(search.contact_cell_phone))
          {
            query += " and cc.phone like '%' + @cellPhone + '%'";
          }

          if (!string.IsNullOrWhiteSpace(search.am_name))
          {
            query += " and cc.phone like '%' + @cellPhone + '%'";
          }

          if (!string.IsNullOrWhiteSpace(search.contact_position))
          {
            query += " and cc.position like '%' + @contactPosition + '%'";
          }

          if (!string.IsNullOrWhiteSpace(search.contact_division))
          {
            query += " and cc.division like '%' + @contactDivision + '%'";
          }

          if ((!string.IsNullOrWhiteSpace(search.createStart)) && (!string.IsNullOrWhiteSpace(search.createEnd)))
          {
            query += " and cc.create_dt between CONVERT(DATETIME, @StartDate + ' 00:00:00') AND CONVERT(DATETIME, @EndDate + ' 23:59:59')";
          }

          if (search.is_my_contact)
            query += " AND CC.create_user = @AppIdentity";

          query += @" 
GROUP BY
		cc.cc_seq
		, c.c_seq
		, c.kor_name
		, c.eng_name
		, cc.name
		, cc.division
		, cc.position
		, cc.phone
		, cc.cell_phone
		, cc.email
		, cc.memo
		, cc.create_dt
) AS tmp";

          totalCount = con.QueryFirstOrDefault<int>(query, parameters);


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

    public async Task<client_contact> SelectContactOneAsync(int cc_seq)
    {
      try
      {
        List<client_contact> list = new List<client_contact>();

        using (IDbConnection con = new SqlConnection(base.BaseConnectionString))
        {
          con.Open();

          DynamicParameters parameters = new DynamicParameters();
          parameters.Add("@cc_seq", cc_seq, DbType.Int32, null);


          string query = @"
SELECT
	cc_seq cc_seq
	, c_seq c_seq
	, kor_name kor_name
	, eng_name eng_name
	, contact_name name
	, contact_division division
	, contact_position position
	, contact_phone phone
	, contact_cell_phone cell_phone
	, contact_email email
	, memo memo
	, create_dt create_dt
	, stuff((
            select ','+b.name
            FROM   client_manager a inner join uv_user b
										on a.uv_seq = b.uv_seq
			where a.c_seq = tmp.c_seq
            FOR XML PATH('')
       ),1,1,'') AS am_name
FROM (
	SELECT
		cc.cc_seq cc_seq
		, c.c_seq c_seq
		, c.kor_name kor_name
		, c.eng_name eng_name
		, cc.name contact_name
		, cc.division contact_division
		, cc.position contact_position
		, cc.phone contact_phone
		, cc.cell_phone contact_cell_phone
		, cc.email contact_email
		, cc.memo memo
		, cc.create_dt create_dt	
	FROM client_contact cc left outer JOIN client c
								ON cc.c_seq = c.c_seq
							left outer join client_manager cm
								ON c.c_seq = cm.c_seq
							left outer join uv_user uu
								ON cm.uv_seq = uu.uv_seq
	WHERE 1=1
      AND cc_seq = @cc_seq
    GROUP BY cc.cc_seq, c.c_seq, c.kor_name, c.eng_name, cc.name, cc.division, cc.position, cc.phone, cc.cell_phone, cc.email, cc.memo, cc.create_dt) AS tmp";

          var ret = await con.QueryFirstOrDefaultAsync<client_contact>(query, parameters);

          con.Close();

          return ret;
        }
      }
      catch (Exception e)
      {
        throw e;
      }
    }


    public async Task<List<client_contact>> SelectClientContactListAsync(int c_seq)
    {
      try
      {
        List<client_contact> list = new List<client_contact>();

        using (IDbConnection con = new SqlConnection(base.BaseConnectionString))
        {
          con.Open();

          string selectQuery = @" SELECT * 
  FROM client_contact
 WHERE c_seq = @c_seq ";

          DynamicParameters param = new DynamicParameters();
          param.Add("@c_seq", c_seq, DbType.String);

          var ret = await con.QueryAsync<client_contact>(selectQuery, param);

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

    public async Task<List<client_tax_contact>> SelectClientTaxContactListAsync(int c_seq)
    {
      try
      {
        List<client_tax_contact> list = new List<client_tax_contact>();

        using (IDbConnection con = new SqlConnection(base.BaseConnectionString))
        {
          con.Open();

          string selectQuery = @" SELECT * 
  FROM client_tax_contact
 WHERE c_seq = @c_seq ";

          DynamicParameters param = new DynamicParameters();
          param.Add("@c_seq", c_seq, DbType.String);

          var ret = await con.QueryAsync<client_tax_contact>(selectQuery, param);

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



    public List<client> DorClientList(DormantSearchModel search, int skip, int count, out int totalCount)
    {
      try
      {
        List<client> list = new List<client>();

        using (IDbConnection con = new SqlConnection(base.BaseConnectionString))
        {
          con.Open();

          DynamicParameters parameters = new DynamicParameters();
          parameters.Add("@CurrentPage", skip, DbType.Int16, null);
          parameters.Add("@PageSize", count, DbType.Int16, null);
          parameters.Add("@korName", search.kor_name, DbType.String, null);
          parameters.Add("@ceoName", search.ceo, DbType.String, null);
          parameters.Add("@bizNum", search.biz_num, DbType.String, null);
          parameters.Add("@contractName", search.contact_name, DbType.String, null);
          parameters.Add("@Email", search.contact_email, DbType.String, null);
          parameters.Add("@amName", search.am_name, DbType.String, null);
          parameters.Add("@searcherName", search.Searcher, DbType.String, null);
          string queryString = @"
SELECT b.c_seq
     , b.kor_name
     , b.eng_name
     , b.ceo
     , d.cc_seq
     , d.name as contact_name
     , isnull(d.phone, d.cell_phone) as contact_phone
     , e.code_name1 as business_name1
     , f.code_name2 as business_name2
     , a.dt as lst_client_date
     , c.title as projectTitle
     , c.am_name
     , c.searcher_name
     , c.dt as last_project_dt
     , c.pjt_status
FROM (SELECT c_seq, max(dt) as dt
      FROM (SELECT c_seq, isnull(modify_dt, create_dt) as dt FROM client
            union select c_seq, isnull(modify_dt, create_dt) as dt from client_contact
            union select c_seq, isnull(modify_dt, create_dt) as dt from client_activity_log
            union select c_seq, isnull(modify_dt, create_dt) as dt from client_tax_contact
            union select c_seq, create_dt as dt from client_file) X
      GROUP BY c_seq) A INNER JOIN client b
                        on a.c_seq = b.c_seq
                        left join  (SELECT *
                                         , isnull(modify_dt, create_dt) as dt
                                         , row_number() over(partition by c_seq order by isnull(modify_dt, create_dt) desc) as r
                                         , STUFF((SELECT ',' + B.name
						                                 FROM pjt_director AS A INNER JOIN uv_user AS B
														                                ON A.uv_seq = B.uv_seq
						                                 WHERE A.p_seq = x.p_seq
						                                 FOR XML PATH('')), 1, 1, '') AS am_name
				                                 , STUFF((SELECT ',' + B.name
						                                 FROM pjt_manager AS A INNER JOIN uv_user AS B
														                                ON A.uv_seq = B.uv_seq
						                                 WHERE A.p_seq = x.p_seq
						                                 FOR XML PATH('')), 1, 1, '') AS searcher_name
                                    FROM project as x ) c
                        on a.c_seq = c.c_seq
                        and c.r = 1
                        inner join  (SELECT *
                                         , row_number() over(partition by c_seq order by isnull(modify_dt, create_dt) desc) as r
                                    FROM client_contact ) d
                        on a.c_seq = d.c_seq
                        and d.r = 1
                        left join code_business_mst e
                        on b.biz_industry_code1 = e.code1
                        left join code_business_dtl f
                        on b.biz_industry_code1 = f.code1
                        and b.biz_industry_code2 = f.code2
where (a.dt < DATEADD(MONTH, -6, GETDATE())
and (c.dt < DATEADD(MONTH, -6, GETDATE()) or c.dt is null))

";


          if (!string.IsNullOrWhiteSpace(search.kor_name))
          {
            queryString += " and (b.kor_name like '%' + @korName + '%' or b.eng_name like '%' + @korName + '%'  OR b.offlimit_keyword LIKE '%' + @korName + '%')";
          }


          if (!string.IsNullOrWhiteSpace(search.ceo))
          {
            queryString += " and b.ceo like '%' + @ceoName + '%'";
          }

          if (!string.IsNullOrWhiteSpace(search.biz_num))
          {
            queryString += " and b.biz_code like '%' + @bizNum + '%' ";
          }

          if (!string.IsNullOrWhiteSpace(search.contact_name))
          {
            queryString += " and d.name like '%' + @contractName + '%'";
          }

          if (!string.IsNullOrWhiteSpace(search.contact_email))
          {
            queryString += " and d.email like '%' + @Email + '%'";
          }

          if (!string.IsNullOrWhiteSpace(search.am_name))
          {
            queryString += " and c.am_name like '%' + @amName + '%'";
          }

          if (!string.IsNullOrWhiteSpace(search.Searcher))
          {
            queryString += " and c.searcher_name like '%' + @searcherName + '%'";
          }

          if (search.business.Count > 0)
          {
            string in_str_all = String.Empty;
            string in_str = String.Empty;
            string in_str_code1 = String.Empty;
            foreach (var code in search.business)
            {
              if (code.code2 <= 10000)
              {
                in_str_code1 += (!String.IsNullOrEmpty(in_str_code1) ? ", " : "") + "'" + code.code2 + "'";
              }
              else
              {
                in_str += (!String.IsNullOrEmpty(in_str) ? ", " : "") + "'" + code.code2 + "'";
              }
            }
            if (!String.IsNullOrEmpty(in_str))
            {
              in_str_all = " b.biz_industry_code2 in (" + in_str + ") ";
            }
            if (!String.IsNullOrEmpty(in_str_code1))
            {
              in_str_all += (!String.IsNullOrEmpty(in_str_all) ? " OR " : "") + " b.biz_industry_code1 in (" + in_str_code1 + ") ";
            }

            if (!String.IsNullOrEmpty(in_str_all))
            {
              queryString += " AND ( " + in_str_all + ")";
            }


          }

          queryString += string.Format(" ORDER BY {0} {1} ", search.orderTxt, search.orderOption);
          queryString += " OFFSET @CurrentPage ROWS FETCH NEXT @PageSize ROWS ONLY";

          var ret = con.Query<client>(queryString, parameters);

          queryString = @"
SELECT COUNT(*)
FROM (SELECT c_seq, max(dt) as dt
      FROM (SELECT c_seq, isnull(modify_dt, create_dt) as dt FROM client
            union select c_seq, isnull(modify_dt, create_dt) as dt from client_contact
            union select c_seq, isnull(modify_dt, create_dt) as dt from client_activity_log
            union select c_seq, isnull(modify_dt, create_dt) as dt from client_tax_contact
            union select c_seq, create_dt as dt from client_file) X
      GROUP BY c_seq) A INNER JOIN client b
                        on a.c_seq = b.c_seq
                        left join  (SELECT *
                                         , isnull(modify_dt, create_dt) as dt
                                         , row_number() over(partition by c_seq order by isnull(modify_dt, create_dt) desc) as r
                                         , STUFF((SELECT ',' + B.name
						                                 FROM pjt_director AS A INNER JOIN uv_user AS B
														                                ON A.uv_seq = B.uv_seq
						                                 WHERE A.p_seq = x.p_seq
						                                 FOR XML PATH('')), 1, 1, '') AS am_name
				                                 , STUFF((SELECT ',' + B.name
						                                 FROM pjt_manager AS A INNER JOIN uv_user AS B
														                                ON A.uv_seq = B.uv_seq
						                                 WHERE A.p_seq = x.p_seq
						                                 FOR XML PATH('')), 1, 1, '') AS searcher_name
                                    FROM project as x ) c
                        on a.c_seq = c.c_seq
                        and c.r = 1
                        inner join  (SELECT *
                                         , row_number() over(partition by c_seq order by isnull(modify_dt, create_dt) desc) as r
                                    FROM client_contact ) d
                        on a.c_seq = d.c_seq
                        and d.r = 1
                        left join code_business_dtl e
                        on b.biz_industry_code1 = e.code1
                        and b.biz_industry_code2 = e.code2
where (a.dt < DATEADD(MONTH, -6, GETDATE())
and (c.dt < DATEADD(MONTH, -6, GETDATE()) or c.dt is null))
";
          if (!string.IsNullOrWhiteSpace(search.kor_name))
          {
            queryString += " and (b.kor_name like '%' + @korName + '%' or b.eng_name like '%' + @korName + '%')";
          }


          if (!string.IsNullOrWhiteSpace(search.ceo))
          {
            queryString += " and b.ceo like '%' + @ceoName + '%'";
          }

          if (!string.IsNullOrWhiteSpace(search.biz_num))
          {
            queryString += " and b.biz_code like '%' + @bizNum + '%' ";
          }

          if (!string.IsNullOrWhiteSpace(search.contact_name))
          {
            queryString += " and d.name like '%' + @contractName + '%'";
          }

          if (!string.IsNullOrWhiteSpace(search.contact_email))
          {
            queryString += " and d.email like '%' + @Email + '%'";
          }

          if (!string.IsNullOrWhiteSpace(search.am_name))
          {
            queryString += " and c.am_name like '%' + @amName + '%'";
          }

          if (!string.IsNullOrWhiteSpace(search.Searcher))
          {
            queryString += " and c.searcher_name like '%' + @searcherName + '%'";
          }

          if (search.business.Count > 0)
          {
            string in_str_all = String.Empty;
            string in_str = String.Empty;
            string in_str_code1 = String.Empty;
            foreach (var code in search.business)
            {
              if (code.code2 <= 10000)
              {
                in_str_code1 += (!String.IsNullOrEmpty(in_str_code1) ? ", " : "") + "'" + code.code2 + "'";
              }
              else
              {
                in_str += (!String.IsNullOrEmpty(in_str) ? ", " : "") + "'" + code.code2 + "'";
              }
            }
            if (!String.IsNullOrEmpty(in_str))
            {
              in_str_all = " b.biz_industry_code2 in (" + in_str + ") ";
            }
            if (!String.IsNullOrEmpty(in_str_code1))
            {
              in_str_all += (!String.IsNullOrEmpty(in_str_all) ? " OR " : "") + " b.biz_industry_code1 in (" + in_str_code1 + ") ";
            }

            if (!String.IsNullOrEmpty(in_str_code1))
            {
              queryString += " AND ( " + in_str_all + ")";
            }


          }


          totalCount = con.QueryFirstOrDefault<int>(queryString, parameters);


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




    public async Task<int> CreateClientContractAsync(string contract_date, string deposit_limit, string fee_type, string guarntee_type, string is_construct_debut, string contract_comment, int c_seq, int cc_seq, decimal fix_fee_rate)
    {
      try
      {
        using (IDbConnection con = new SqlConnection(base.BaseConnectionString))
        {
          con.Open();
          DynamicParameters parameters = new DynamicParameters();
          parameters.Add("@c_seq", c_seq, DbType.Int32);
          parameters.Add("@cc_seq", cc_seq, DbType.Int32);
          parameters.Add("@contract_date", contract_date, DbType.String, null);
          parameters.Add("@deposit_limit", deposit_limit, DbType.String, null);
          parameters.Add("@fee_type", fee_type, DbType.String, null);
          parameters.Add("@guarntee_type", guarntee_type, DbType.String, null);
          parameters.Add("@is_construct_debut", is_construct_debut, DbType.String, null);
          parameters.Add("@fixfeeRate", fix_fee_rate, DbType.String, null);
          parameters.Add("@contract_comment", contract_comment, DbType.String, null);
          parameters.Add("@create_user", AppIdentity.user_seq, DbType.String, null);


          string query = @"INSERT INTO client_contract(c_seq, contract_date, fee_type, deposit_limit, guarntee_type, is_construct_debut, fix_fee_rate, contract_comment, create_dt, create_user) values(
@c_seq
, @contract_date
, @fee_type
, @deposit_limit
, @guarntee_type
, @is_construct_debut
, @fixfeeRate
, @contract_comment
, getdate()
, @create_user
)";
          int ret = await SqlMapper.ExecuteAsync(con, query, parameters, commandType: CommandType.Text);

          con.Close();

          return ret;

        }
      }
      catch (Exception e)
      {
        throw e;
      }

    }


    public async Task<int> Create1stFileAsync(int c_seq, string file_dir, string path, string fileName)
    {
      try
      {
        using (IDbConnection con = new SqlConnection(base.BaseConnectionString))
        {
          con.Open();

          DynamicParameters parameters = new DynamicParameters();
          parameters.Add("@cSeq", c_seq, DbType.Int32);
          parameters.Add("@fileDir", file_dir, DbType.String, null);
          parameters.Add("@filePath", path + "\\" + fileName, DbType.String, null);
          parameters.Add("@fileName", fileName, DbType.String, null);
          parameters.Add("@createUser", AppIdentity.user_seq, DbType.Int32);

          string queryString = @"insert into client_contract_file(c_seq, file_type, file_dir, file_origin_path, file_path, file_extension, create_dt, create_user) values(
@cSeq,
'C',
@fileDir,
@filePath,
@fileName,
'.docx',
getdate(),
@createUser
)";
          int ret = await SqlMapper.ExecuteAsync(con, queryString, parameters, commandType: CommandType.Text);

          con.Close();

          return ret;

        }
      }
      catch (Exception e)
      {
        throw e;
      }
    }


    public async Task<int> UpdateClientContractAsync(string contract_date, string deposit_limit, string fee_type, string guarntee_type, string is_construct_debut, string contract_comment, int c_seq, int cc_seq, decimal fix_fee_rate)
    {
      try
      {
        using (IDbConnection con = new SqlConnection(base.BaseConnectionString))
        {
          con.Open();
          DynamicParameters parameters = new DynamicParameters();
          parameters.Add("@c_seq", c_seq, DbType.Int32);
          parameters.Add("@cc_seq", cc_seq, DbType.Int32);
          parameters.Add("@contract_date", contract_date, DbType.String, null);
          parameters.Add("@deposit_limit", deposit_limit, DbType.String, null);
          parameters.Add("@fee_type", fee_type, DbType.String, null);
          parameters.Add("@guarntee_type", guarntee_type, DbType.String, null);
          parameters.Add("@is_construct_debut", is_construct_debut, DbType.String, null);
          parameters.Add("@contract_comment", contract_comment, DbType.String, null);
          parameters.Add("@create_user", AppIdentity.user_seq, DbType.String, null);
          parameters.Add("@fixfeeRate", fix_fee_rate, DbType.String, null);


          string query = @"UPDATE client_contract SET 
    fee_type = @fee_type
    , deposit_limit = @deposit_limit
    , guarntee_type = @guarntee_type
    , is_construct_debut = @is_construct_debut
    , contract_comment = @contract_comment
    , fix_fee_rate = @fixfeeRate
    , modify_dt = getdate()
    , modify_user = @create_user
where cc_seq = @cc_seq
";
          client_contract data = new client_contract()
          {
            c_seq = c_seq,
            cc_seq = cc_seq,
            contract_date = contract_date,
            deposit_limit = deposit_limit,
            fee_type = fee_type,
            guarntee_type = guarntee_type,
            is_construct_debut = is_construct_debut,
            contract_comment = contract_comment,
            fix_fee_rate = decimal.Parse(fix_fee_rate.ToString())
          };

          ClientLogRepository log = new ClientLogRepository();
          await log.client_contract_log(data, 1, "U", AppIdentity.user_seq);


          int ret = await SqlMapper.ExecuteAsync(con, query, parameters, commandType: CommandType.Text);

          con.Close();

          return ret;

        }
      }
      catch (Exception e)
      {
        throw e;
      }
    }




    public List<client_annual_income_rate> FeetypeListA(int c_seq, int cc_seq, int skip, int count, out int totalCount)
    {
      try
      {
        List<client_annual_income_rate> list = new List<client_annual_income_rate>();

        using (IDbConnection con = new SqlConnection(base.BaseConnectionString))
        {
          con.Open();

          DynamicParameters parameters = new DynamicParameters();
          parameters.Add("@CurrentPage", skip, DbType.Int32, null);
          parameters.Add("@PageSize", count, DbType.Int32, null);
          parameters.Add("@cSeq", c_seq, DbType.Int32, null);
          parameters.Add("@ccSeq", cc_seq, DbType.Int32, null);

          string query = @"select
    Currency_Name,
	c_seq,
	cc_seq,
	start_income,
	end_income,
	percentage
from client_annual_income_rate
where c_seq = @cSeq";

          var ret = con.Query<client_annual_income_rate>(query, parameters);

          query = @"select count(0) from client_annual_income_rate where c_seq = @cSeq and cc_seq = @ccSeq";
          totalCount = con.QueryFirstOrDefault<int>(query, parameters);

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

    public async Task CreateIncomFeeTypeA(int c_seq, int cc_seq, string currency_name, int incomNum, int[] incomSamt, int[] incomEamt, decimal[] incomPer)
    {
      try
      {
        using (IDbConnection con = new SqlConnection(base.BaseConnectionString))
        {

          DynamicParameters parameters = new DynamicParameters();
          parameters.Add("@cSeq", c_seq, DbType.Int32, null);
          parameters.Add("@ccSeq", cc_seq, DbType.Int32, null);
          parameters.Add("@currency_name", currency_name, DbType.String, null);

          //string deleteQuery = @"delete from client_contract where fee_type='A' and c_seq = @cSeq";
          //SqlMapper.Execute(con, deleteQuery, parameters, commandType: CommandType.Text);
          ClientLogRepository log = new ClientLogRepository();

          string Delquery = @"delete from client_annual_income_rate where c_Seq=@cSeq and cc_seq=@ccSeq";
          SqlMapper.Execute(con, Delquery, parameters, commandType: CommandType.Text);


          string updateQuery = @"update client_contract set fee_type = 'A' where cc_seq = @ccSeq";


          await log.client_contract_log_fee_type(cc_seq, 1, "U", AppIdentity.user_seq);
          SqlMapper.Execute(con, updateQuery, parameters, commandType: CommandType.Text);


          string query = @"insert into client_annual_income_rate(Currency_Name, c_seq, cc_seq, start_income, end_income, percentage) values( 
                        @currency_name
                        , @c_seq
                        , @cc_seq
                        , @incomSamt
                        , @incomEamt
                        , @incomPer
)";
          con.Open();
          using (var tran = con.BeginTransaction())
          {
            try
            {
              for (var r = 0; r < incomNum; r++)
              {
                try
                {

                  int incomSamtValue = int.Parse(incomSamt.GetValue(r).ToString());
                  int incomEamtValue = int.Parse(incomEamt.GetValue(r).ToString());
                  string incomPerValue = incomPer.GetValue(r).ToString();

                  DynamicParameters parameterInsert = new DynamicParameters();
                  parameterInsert.Add("@c_seq", c_seq, DbType.Int32, null);
                  parameterInsert.Add("@cc_seq", cc_seq, DbType.Int32, null);
                  parameterInsert.Add("@currency_name", currency_name, DbType.String, null);
                  parameterInsert.Add("@incomSamt", incomSamtValue, DbType.Int32, null);
                  parameterInsert.Add("@incomEamt", incomEamtValue, DbType.Int32, null);
                  parameterInsert.Add("@incomPer", incomPerValue, DbType.String, null);

                  SqlMapper.Execute(con, query, parameterInsert, commandType: CommandType.Text, transaction: tran);
                }
                catch (Exception ex)
                {

                  throw ex;
                }
              }
              tran.Commit();
            }
            catch (Exception e)
            {
              tran.Rollback();
              throw e;
            }
          }
          con.Close();
        }
      }
      catch (Exception e)
      {
        throw e;
      }
    }

    public List<client_position_rate> FeetypeListB(int c_seq, int cc_seq, int skip, int count, out int totalCount)
    {
      try
      {
        List<client_position_rate> list = new List<client_position_rate>();

        using (IDbConnection con = new SqlConnection(base.BaseConnectionString))
        {
          con.Open();

          DynamicParameters parameters = new DynamicParameters();
          parameters.Add("@CurrentPage", skip, DbType.Int16, null);
          parameters.Add("@PageSize", count, DbType.Int16, null);
          parameters.Add("@cSeq", c_seq, DbType.Int16, null);
          parameters.Add("@ccSeq", cc_seq, DbType.Int16, null);

          string query = @"select
	c_seq,
	cc_seq,
	start_code_seq,
	end_code_seq,
	rate
from

    client_position_rate
where

    c_seq = @cSeq";

          var ret = con.Query<client_position_rate>(query, parameters);

          query = @"select count(0) from client_position_rate where c_seq = @cSeq and cc_seq = @ccSeq";
          totalCount = con.QueryFirstOrDefault<int>(query, parameters);

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

    public List<code_position> SelectPositionListAsync()
    {
      try
      {
        List<code_position> allPosition = new List<code_position>();

        using (IDbConnection con = new SqlConnection(base.BaseConnectionString))
        {
          con.Open();

          DynamicParameters parameters = new DynamicParameters();


          string queryString = @"SELECT p_code, p_name FROM code_position ORDER BY p_code asc";

          var ret = con.Query<code_position>(queryString, parameters);

          allPosition = ret.ToList();

          con.Close();

          return allPosition;
        }
      }
      catch (Exception e)
      {
        throw e;
      }
    }

    public async Task CreateIncomFeeTypeB(int c_seq, int cc_seq, int positionNum, string[] positionS, string[] positionE, decimal[] positionPer)
    {
      try
      {
        using (IDbConnection con = new SqlConnection(base.BaseConnectionString))
        {

          DynamicParameters parameters = new DynamicParameters();
          parameters.Add("@cSeq", c_seq, DbType.Int32, null);
          parameters.Add("@ccSeq", cc_seq, DbType.Int32, null);

          string Delquery = @"delete from client_position_rate where c_Seq=@cSeq and cc_seq=@ccSeq";
          SqlMapper.Execute(con, Delquery, parameters, commandType: CommandType.Text);


          string updateQuery = @"update client_contract set fee_type = 'B' where cc_seq = @ccSeq";
          ClientLogRepository log = new ClientLogRepository();
          await log.client_contract_log_fee_type(cc_seq, 1, "U", AppIdentity.user_seq);

          SqlMapper.Execute(con, updateQuery, parameters, commandType: CommandType.Text);



          string query = @"insert into client_position_rate(c_seq, cc_seq, start_code_seq, end_code_seq, rate) values( 
                         @c_seq
                        , @cc_seq
                        , @positionS
                        , @positionE
                        , @positionPer
)";
          con.Open();
          using (var tran = con.BeginTransaction())
          {
            try
            {
              for (var r = 0; r < positionNum; r++)
              {
                try
                {

                  string positionSValue = positionS.GetValue(r).ToString();
                  string positionEValue = positionE.GetValue(r).ToString();
                  string positionPerValue = positionPer.GetValue(r).ToString();

                  DynamicParameters parameterInsert = new DynamicParameters();
                  parameterInsert.Add("@c_seq", c_seq, DbType.Int32, null);
                  parameterInsert.Add("@cc_seq", cc_seq, DbType.Int32, null);
                  parameterInsert.Add("@positionS", positionSValue, DbType.String, null);
                  parameterInsert.Add("@positionE", positionEValue, DbType.String, null);
                  parameterInsert.Add("@positionPer", positionPerValue, DbType.String, null);

                  SqlMapper.Execute(con, query, parameterInsert, commandType: CommandType.Text, transaction: tran);
                }
                catch (Exception ex)
                {

                  throw ex;
                }
              }
              tran.Commit();
            }
            catch (Exception e)
            {
              tran.Rollback();
              throw e;
            }
          }
          con.Close();
        }
      }
      catch (Exception e)
      {
        throw e;
      }
    }

    public async Task<client_contract> SelectFixfeeAsync(int cc_seq)
    {
      try
      {
        using (IDbConnection con = new SqlConnection(base.BaseConnectionString))
        {
          con.Open();

          string queryString = @"select fix_fee_rate from client_contract where cc_seq = @cc_seq
";
          DynamicParameters parameters = new DynamicParameters();
          parameters.Add("@cc_seq", cc_seq, DbType.Int32);

          var ret = await con.QueryAsync<client_contract>(queryString, parameters);

          con.Close();

          return ret.FirstOrDefault();
        }
      }

      catch (Exception e)
      {
        throw e;
      }
    }


    public List<gov_api_company> SelectClientList(string searchTxt, int skip, int count, out int totalCount)
    {
      try
      {
        List<gov_api_company> list = new List<gov_api_company>();

        using (IDbConnection con = new SqlConnection(base.BaseConnectionString))
        {
          con.Open();

          string selectQuery = @"SELECT g_seq, wkpl_nm, bzowr_rgst_no, vldt_vl_krn_nm, wkpl_road_nm_dtl_addr  
FROM gov_api_company 
WHERE wkpl_nm like '%' + @searchTxt + '%'
ORDER BY wkpl_nm  
OFFSET @currentPage ROWS FETCH NEXT @pageSize ROWS ONLY ";

          string countQuery = @" SELECT COUNT(0)
  FROM client AS C LEFT OUTER JOIN code_business_dtl AS CB2
                                ON C.biz_industry_code1 = CB2.code1
                               AND C.biz_industry_code2 = CB2.code2
                   LEFT OUTER JOIN client_manager AS CM
                                ON C.c_seq = CM.c_seq
                   LEFT OUTER JOIN uv_user AS U
                            	ON CM.uv_seq = U.uv_seq
 WHERE (C.kor_name LIKE '%' + @searchTxt + '%') OR (C.eng_name LIKE '%' + @searchTxt + '%') ";

          DynamicParameters param = new DynamicParameters();
          param.Add("@searchTxt", searchTxt, DbType.String);
          param.Add("@currentPage", skip, DbType.Int32);
          param.Add("@pageSize", count, DbType.Int32);

          DynamicParameters param2 = new DynamicParameters();
          param2.Add("@searchTxt", searchTxt, DbType.String);

          var ret = con.Query<gov_api_company>(selectQuery, param);
          totalCount = con.QueryFirstOrDefault<int>(countQuery, param2);

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


    public List<code_nationality> SelectNationalcodeList(string searchTxt)
    {
      try
      {
        List<code_nationality> list = new List<code_nationality>();

        using (IDbConnection con = new SqlConnection(base.BaseConnectionString))
        {
          con.Open();

          string queryString = @"SELECT nationality_code, en_country_name, kr_country_name
FROM code_nationality
WHERE 1 = 1 ";

          if (!string.IsNullOrWhiteSpace(searchTxt))
            queryString += " AND en_country_name like '%' + @searchTxt + '%' or kr_country_name like '%' + @searchTxt + '%' ";

          queryString += " ORDER BY kr_country_name ";

          DynamicParameters parameters = new DynamicParameters();
          parameters.Add("@searchTxt", searchTxt, DbType.String);

          var ret = con.Query<code_nationality>(queryString, parameters);

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

    public async Task<int> SelectUniClientAsync(int c_seq, string kor_name)
    {
      try
      {
        //List<client> list = new List<client>();

        using (IDbConnection con = new SqlConnection(base.BaseConnectionString))
        {
          con.Open();

          string queryString = @"select count(*) from client where (REPLACE(REPLACE(REPLACE(kor_name, '(주)', ''), '(유)', ''), ' ', '')=@korName)";
          if (c_seq > 0)
          {
            queryString += " and c_seq <> @c_seq ";
          }
          DynamicParameters parameters = new DynamicParameters();
          parameters.Add("@korName", kor_name, DbType.String);
          parameters.Add("@c_seq", c_seq, DbType.Int32);

          int data = await con.QueryFirstOrDefaultAsync<int>(queryString, parameters);

          //var ret = con.Query<client>(queryString, parameters);

          //list = ret.ToList();

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
