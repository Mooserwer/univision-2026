/* =====================================================================
   uv_user - 주간 Shift 자택근무 한도(일) 컬럼 추가
   직접 DB에 반영하세요. (Univision DB)
   NULL 이면 코드에서 기본값 1일로 처리합니다.
   ===================================================================== */

IF NOT EXISTS (SELECT 1 FROM sys.columns
               WHERE object_id = OBJECT_ID('dbo.UV_USER') AND name = 'weekly_shift_limit')
BEGIN
    ALTER TABLE dbo.UV_USER ADD weekly_shift_limit INT NULL;  -- 주간 Shift 자택근무 한도(일)
END
GO
