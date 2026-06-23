/* =====================================================================
   전자결재 v2 마이그레이션  -  참조자(line_type) 추가
   v1(appr_schema.sql)을 이미 반영한 경우에만 실행하세요.
   (신규 설치는 appr_schema.sql 에 이미 포함되어 있습니다.)
   ===================================================================== */

IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE object_id = OBJECT_ID('dbo.APPR_LINE') AND name = 'line_type')
BEGIN
    ALTER TABLE dbo.APPR_LINE ADD line_type INT NOT NULL DEFAULT 0;  -- 0 결재 / 1 참조(열람만)
END
GO
