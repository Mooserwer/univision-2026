/* =====================================================================
   [내부 DB] n_privacy_agree / n_privacy_agree_dtl
   새 PrivacyCreate(내부인재풀 개인정보 동의) 플로우용.
   기존 privacy_agree / privacy_agree_dtl 스키마 기반.
   직접 DB에 반영하세요. (내부 Univision DB)
   ===================================================================== */

-- 동의 마스터 ------------------------------------------------------------
IF NOT EXISTS (SELECT 1 FROM sys.tables WHERE name = 'n_privacy_agree')
BEGIN
    CREATE TABLE dbo.n_privacy_agree
    (
        pa_seq       INT IDENTITY(1,1) NOT NULL,   -- PK
        send_type    NVARCHAR(10)   NULL,          -- 발송구분 S:SMS / E:mail
        send_addr    NVARCHAR(100)  NULL,          -- 발신주소(발신번호 또는 발신메일)
        can_name     NVARCHAR(50)   NULL,          -- 대상자명
        can_contact  NVARCHAR(100)  NULL,          -- 연락처(전화번호 또는 이메일)
        profile_id   INT            NULL,          -- 대상 프로필 키 (Remember profile id, R_PROFILE_MEMO.profile_id 와 연결)
        sender_email NVARCHAR(100)  NULL,          -- 발신자(담당자) 이메일 - 동의완료 시 통지 대상 (테스트중 lee.hc@unicosearch.com 고정)
        send_dt      DATETIME       NULL,          -- 발송일자
        full_txt     NVARCHAR(MAX)  NULL,          -- 동의서 전문
        agree_dt     DATETIME       NULL,          -- 동의일자
        agree_ip     NVARCHAR(50)   NULL,          -- 동의 IP
        agree_code1  NVARCHAR(100)  NULL,          -- 본인확인: 이름(평문, 홈페이지 동의 시 입력값 동기화)
        agree_code2  NVARCHAR(100)  NULL,          -- 본인확인: 생년월일(해시, 홈페이지에서 해시 처리된 값 동기화)
        agree_code3  NVARCHAR(200)  NULL,          -- 동의 링크 키
        create_dt    DATETIME       NULL,          -- 생성일
        create_user  INT            NULL,          -- 생성자
        CONSTRAINT PK_n_privacy_agree PRIMARY KEY CLUSTERED (pa_seq)
    );
END
GO

-- 동의 상세 (수집·이용 / 제3자 제공 등 항목별) ---------------------------
IF NOT EXISTS (SELECT 1 FROM sys.tables WHERE name = 'n_privacy_agree_dtl')
BEGIN
    CREATE TABLE dbo.n_privacy_agree_dtl
    (
        pad_seq      INT IDENTITY(1,1) NOT NULL,   -- PK
        pa_seq       INT            NOT NULL,       -- 마스터키 (n_privacy_agree.pa_seq)
        agree_type   INT            NULL,           -- 동의서 구분 (1:수집이용 / 2:제3자제공)
        client_seq   INT            NULL,           -- 고객사 코드
        client_name  NVARCHAR(200)  NULL,           -- 고객사명
        is_agree     INT            NULL,           -- 동의여부
        is_optional  INT            NULL,           -- 필수여부
        contents     NVARCHAR(MAX)  NULL,           -- 동의 항목 내용(약관)
        create_dt    DATETIME       NULL,           -- 생성일
        create_user  INT            NULL,           -- 생성자
        CONSTRAINT PK_n_privacy_agree_dtl PRIMARY KEY CLUSTERED (pad_seq)
    );

    CREATE INDEX IX_n_privacy_agree_dtl_pa_seq ON dbo.n_privacy_agree_dtl (pa_seq);
END
GO
