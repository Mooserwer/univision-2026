/* =====================================================================
   R_PROFILE_MEMO - 프로필 메모 테이블 (헤드헌팅 Pro 내부인재풀)
   직접 DB에 반영하세요. (Remember / Univision DB)
   - memo_id    : 메모 고유 키 (PK, IDENTITY)
   - profile_id : 대상 프로필 키
   - content    : 메모 내용 (varchar(300))
   - create_dt  : 생성일
   ===================================================================== */

IF NOT EXISTS (SELECT 1 FROM sys.tables WHERE name = 'R_PROFILE_MEMO')
BEGIN
    CREATE TABLE dbo.R_PROFILE_MEMO
    (
        memo_id     INT IDENTITY(1,1) NOT NULL,   -- 메모 고유 키
        profile_id  INT               NOT NULL,   -- 대상 프로필 키
        content     VARCHAR(300)      NULL,       -- 메모 내용
        create_dt   DATETIME          NOT NULL
            CONSTRAINT DF_R_PROFILE_MEMO_create_dt DEFAULT (GETDATE()),  -- 생성일
        CONSTRAINT PK_R_PROFILE_MEMO PRIMARY KEY CLUSTERED (memo_id)
    );

    -- 프로필별 조회가 필요할 경우를 위한 보조 인덱스 (커서 페이징은 PK(memo_id) 로 처리됨)
    CREATE INDEX IX_R_PROFILE_MEMO_profile_id ON dbo.R_PROFILE_MEMO (profile_id);
END
GO
