/* =====================================================================
   [내부 DB - 테스트용] n_privacy_agree_test / n_privacy_agree_dtl_test / R_PROFILE_MEMO_TEST
   /Partial/PrivacyCreateTest 테스트 플로우 전용. 운영 테이블과 스키마 동일.
   직접 DB에 반영하세요. (내부 Univision DB)
   테스트 종료 후 DROP 해도 운영에 영향 없음.
   ===================================================================== */

-- 동의 마스터 ------------------------------------------------------------
IF NOT EXISTS (SELECT 1 FROM sys.tables WHERE name = 'n_privacy_agree_test')
BEGIN
    CREATE TABLE dbo.n_privacy_agree_test
    (
        pa_seq       INT IDENTITY(1,1) NOT NULL,
        send_type    NVARCHAR(10)   NULL,
        send_addr    NVARCHAR(100)  NULL,
        can_name     NVARCHAR(50)   NULL,
        can_contact  NVARCHAR(100)  NULL,
        profile_id   INT            NULL,
        sender_email NVARCHAR(100)  NULL,
        send_dt      DATETIME       NULL,
        full_txt     NVARCHAR(MAX)  NULL,
        agree_dt     DATETIME       NULL,
        agree_ip     NVARCHAR(50)   NULL,
        agree_code1  NVARCHAR(100)  NULL,
        agree_code2  NVARCHAR(100)  NULL,
        agree_code3  NVARCHAR(200)  NULL,
        create_dt    DATETIME       NULL,
        create_user  INT            NULL,
        CONSTRAINT PK_n_privacy_agree_test PRIMARY KEY CLUSTERED (pa_seq)
    );
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
        create_user  INT            NULL,
        CONSTRAINT PK_n_privacy_agree_dtl_test PRIMARY KEY CLUSTERED (pad_seq)
    );

    CREATE INDEX IX_n_privacy_agree_dtl_test_pa_seq ON dbo.n_privacy_agree_dtl_test (pa_seq);
END
GO

-- 프로필 메모 (테스트) ----------------------------------------------------
IF NOT EXISTS (SELECT 1 FROM sys.tables WHERE name = 'R_PROFILE_MEMO_TEST')
BEGIN
    CREATE TABLE dbo.R_PROFILE_MEMO_TEST
    (
        memo_id     INT IDENTITY(1,1) NOT NULL,
        profile_id  INT               NOT NULL,
        content     VARCHAR(300)      NULL,
        create_dt   DATETIME          NOT NULL
            CONSTRAINT DF_R_PROFILE_MEMO_TEST_create_dt DEFAULT (GETDATE()),
        CONSTRAINT PK_R_PROFILE_MEMO_TEST PRIMARY KEY CLUSTERED (memo_id)
    );

    CREATE INDEX IX_R_PROFILE_MEMO_TEST_profile_id ON dbo.R_PROFILE_MEMO_TEST (profile_id);
END
GO
