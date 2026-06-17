using Dapper;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Univision.Core.Models.DTO;
using Univision.Core.Models.DTO.Response.Api;

namespace Univision.Core.Repositories
{
    public class winformRepository : BaseRepository
    {
        public async Task<int> InsertOrUpdate(CSV_MODEL model)
        {
            int iResult = 0;
            try
            {
               

                using (IDbConnection con = new SqlConnection(base.BaseConnectionString))
                {
                    con.Open();

                    string selectQuery = @"
UPDATE gov_api_company
   SET WKPL_NM				= @WKPL_NM
      ,BZOWR_RGST_NO		= @BZOWR_RGST_NO
      ,DATA_CRT_YM			= @DATA_CRT_YM
      ,WKPL_JNNG_STCD		= @WKPL_JNNG_STCD
      ,ZIP					= @ZIP
      ,WKPL_LTNO_DTL_ADDR	= @WKPL_LTNO_DTL_ADDR
      ,WKPL_ROAD_NM_DTL_ADDR= @WKPL_ROAD_NM_DTL_ADDR
      ,WKPL_STYL_DVCD		= @WKPL_STYL_DVCD
      ,WKPL_INTP_CD			= @WKPL_INTP_CD
      ,VLDT_VL_KRN_NM		= @VLDT_VL_KRN_NM
      ,ADPT_DT				= @ADPT_DT
      ,RRG_DT				= @RRG_DT
      ,SCSN_DT				= @SCSN_DT
      ,JNNGP_CNT			= @JNNGP_CNT
      ,CRRMM_NTC_AMT		= @CRRMM_NTC_AMT
      ,NW_ACQZR_CNT			= @NW_ACQZR_CNT
      ,LSS_JNNGP_CNT		= @LSS_JNNGP_CNT
 WHERE WKPL_NM = @WKPL_NM
 AND BZOWR_RGST_NO = @BZOWR_RGST_NO
 AND WKPL_INTP_CD = @WKPL_INTP_CD


 IF @@ROWCOUNT > 0
 SELECT 1
 ELSE IF @@ROWCOUNT = 0
 BEGIN
 INSERT INTO gov_api_company
           (WKPL_NM
           ,BZOWR_RGST_NO
           ,DATA_CRT_YM
           ,WKPL_JNNG_STCD
           ,ZIP
           ,WKPL_LTNO_DTL_ADDR
           ,WKPL_ROAD_NM_DTL_ADDR
           ,WKPL_STYL_DVCD
           ,WKPL_INTP_CD
           ,VLDT_VL_KRN_NM
           ,ADPT_DT
           ,RRG_DT
           ,SCSN_DT
           ,JNNGP_CNT
           ,CRRMM_NTC_AMT
           ,NW_ACQZR_CNT
           ,LSS_JNNGP_CNT)
     VALUES
           (@WKPL_NM
           ,@BZOWR_RGST_NO
           ,@DATA_CRT_YM
           ,@WKPL_JNNG_STCD
           ,@ZIP
           ,@WKPL_LTNO_DTL_ADDR
           ,@WKPL_ROAD_NM_DTL_ADDR
           ,@WKPL_STYL_DVCD
           ,@WKPL_INTP_CD
           ,@VLDT_VL_KRN_NM
           ,@ADPT_DT
           ,@RRG_DT
           ,@SCSN_DT
           ,@JNNGP_CNT
           ,@CRRMM_NTC_AMT
           ,@NW_ACQZR_CNT
           ,@LSS_JNNGP_CNT)
	SELECT 2
 END";

                    DynamicParameters param = new DynamicParameters();
                    param.Add("@WKPL_NM", model.사업장명, DbType.String);
                    param.Add("@BZOWR_RGST_NO", model.사업자등록번호, DbType.String);
                    param.Add("@DATA_CRT_YM", model.자료생성년월, DbType.String);
                    param.Add("@WKPL_JNNG_STCD", model.사업장가입상태코드, DbType.String);
                    param.Add("@ZIP", model.우편번호, DbType.String);
                    param.Add("@WKPL_LTNO_DTL_ADDR", model.사업장지번상세주소, DbType.String);
                    param.Add("@WKPL_ROAD_NM_DTL_ADDR", model.사업장도로명상세주소, DbType.String);
                    param.Add("@WKPL_STYL_DVCD", model.사업장형태구분코드, DbType.String);
                    param.Add("@WKPL_INTP_CD", model.사업장업종코드, DbType.String);
                    param.Add("@VLDT_VL_KRN_NM", model.사업장업종코드명, DbType.String);
                    param.Add("@ADPT_DT", model.적용일자, DbType.String);
                    param.Add("@RRG_DT", model.재등록일자, DbType.String);
                    param.Add("@SCSN_DT", model.탈퇴일자, DbType.String);
                    param.Add("@JNNGP_CNT", model.가입자수, DbType.Int32);
                    param.Add("@CRRMM_NTC_AMT", model.당월고지금액, DbType.Single);
                    param.Add("@NW_ACQZR_CNT", model.신규취득자수, DbType.Int32);
                    param.Add("@LSS_JNNGP_CNT", model.상실가입자수, DbType.Int32);


                    iResult = await con.ExecuteScalarAsync<int>(selectQuery, param);

                    

                    con.Close();
                }

                return iResult;
            }
            catch //(Exception e)
            {
                return 0;
            }
        }
    }
}
