/* =====================================================================
   [홈페이지 DB - 테스트용] n_privacy_agree_test / n_privacy_agree_dtl_test
   테스트 동의 페이지(/agreeprovidetest/{key}, /agreesuccesstest/{key}) 전용.
   운영 홈페이지 테이블과 스키마 동일 (개인정보 최소화).
   직접 홈페이지 DB에 반영하세요. 테스트 종료 후 DROP 해도 운영에 영향 없음.
   ===================================================================== */

-- 동의 마스터 ------------------------------------------------------------
IF NOT EXISTS (SELECT 1 FROM sys.tables WHERE name = 'n_privacy_agree_test')
BEGIN
    CREATE TABLE dbo.n_privacy_agree_test
    (
        pa_seq      INT            NOT NULL,   -- 내부 n_privacy_agree_test.pa_seq 복사 (IDENTITY 아님)
        send_type   NVARCHAR(10)   NULL,
        send_dt     DATETIME       NULL,
        agree_code3 NVARCHAR(200)  NULL,
        agree_code1 NVARCHAR(100)  NULL,
        agree_code2 NVARCHAR(100)  NULL,
        agree_dt    DATETIME       NULL,
        agree_ip    NVARCHAR(50)   NULL,
        create_dt   DATETIME       NULL,
        is_sync     INT            NOT NULL CONSTRAINT DF_n_pa_home_test_is_sync DEFAULT(0),
        sync_dt     DATETIME       NULL,
        CONSTRAINT PK_n_privacy_agree_home_test PRIMARY KEY CLUSTERED (pa_seq)
    );

    CREATE INDEX IX_n_privacy_agree_home_test_code3 ON dbo.n_privacy_agree_test (agree_code3);
    CREATE INDEX IX_n_privacy_agree_home_test_sync ON dbo.n_privacy_agree_test (is_sync, agree_dt);
END
GO

-- 동의 상세 --------------------------------------------------------------
IF NOT EXISTS (SELECT 1 FROM sys.tables WHERE name = 'n_privacy_agree_dtl_test')
BEGIN
    CREATE TABLE dbo.n_privacy_agree_dtl_test
    (
        pad_seq      INT IDENTITY(1,1) NOT NULL,
        pa_seq       INT            NOT NULL,
        agree_type   INT            NULL,
        client_seq   INT            NULL,
        client_name  NVARCHAR(200)  NULL,
        is_agree     INT            NULL,
        is_optional  INT            NULL,
        contents     NVARCHAR(MAX)  NULL,
        create_dt    DATETIME       NULL,
        CONSTRAINT PK_n_privacy_agree_dtl_home_test PRIMARY KEY CLUSTERED (pad_seq)
    );

    CREATE INDEX IX_n_privacy_agree_dtl_home_test_pa_seq ON dbo.n_privacy_agree_dtl_test (pa_seq);
END
GO
