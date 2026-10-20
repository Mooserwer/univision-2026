/* =====================================================================
   [홈페이지 DB] n_privacy_agree / n_privacy_agree_dtl  (개인정보 최소화)
   새 PrivacyCreate 플로우의 공개 동의 페이지(/privacyprovide/{key})용.
   직접 홈페이지 DB에 반영하세요. (기존 home_privacy_agree 와 동일한 '별도 DB' 개념)

   ★ 개인정보 최소화 방침:
     - 대상자 식별정보(이름/연락처/발신주소/동의서 전문/생성자메일 등)는 저장하지 않음.
     - 내부 DB(n_privacy_agree)의 pa_seq 로만 연결되고, 공개 페이지는
       '동의 링크 키(agree_code3)'로 레코드를 찾아 동의 결과(동의일시/IP/동의여부)만 기록.
   ===================================================================== */

-- 동의 마스터 (최소 컬럼) ------------------------------------------------
IF NOT EXISTS (SELECT 1 FROM sys.tables WHERE name = 'n_privacy_agree')
BEGIN
    CREATE TABLE dbo.n_privacy_agree
    (
        pa_seq      INT            NOT NULL,   -- 내부 n_privacy_agree.pa_seq 와 동일 값 (복사, IDENTITY 아님)
        send_type   NVARCHAR(10)   NULL,       -- 발송구분 S:SMS / E:mail
        send_dt     DATETIME       NULL,       -- 발송일자
        agree_code3 NVARCHAR(200)  NULL,       -- 동의 링크 키 (공개 페이지 조회용)
        -- 증빙용(본인확인): 동의 시 공개 페이지에서 입력받아 기록.
        agree_code1 NVARCHAR(100)  NULL,       -- 이름 (평문 유지)
        agree_code2 NVARCHAR(100)  NULL,       -- 생년월일 (해시 - 숫자라 Classic ASP/.NET 해시 일치 용이)
        agree_dt    DATETIME       NULL,       -- 동의일시 (동의 증빙)
        agree_ip    NVARCHAR(50)   NULL,       -- 동의 IP   (동의 증빙)
        create_dt   DATETIME       NULL,       -- 생성일
        -- 스케줄러 동기화 추적: 공개 페이지 동의 결과를 주기적으로 풀링→내부 DB 반영 후 표시.
        is_sync     INT            NOT NULL CONSTRAINT DF_n_pa_home_is_sync DEFAULT(0),  -- 0:미동기화 / 1:동기화완료
        sync_dt     DATETIME       NULL,       -- 동기화 일시
        CONSTRAINT PK_n_privacy_agree_home PRIMARY KEY CLUSTERED (pa_seq)
    );

    CREATE INDEX IX_n_privacy_agree_home_code3 ON dbo.n_privacy_agree (agree_code3);
    -- 스케줄러가 '동의완료 & 미동기화' 건을 빠르게 조회하도록 인덱스
    CREATE INDEX IX_n_privacy_agree_home_sync ON dbo.n_privacy_agree (is_sync, agree_dt);
END
GO

-- 동의 상세 (약관 내용 + 항목별 동의결과. 대상자 PII 없음) ----------------
IF NOT EXISTS (SELECT 1 FROM sys.tables WHERE name = 'n_privacy_agree_dtl')
BEGIN
    CREATE TABLE dbo.n_privacy_agree_dtl
    (
        pad_seq      INT IDENTITY(1,1) NOT NULL,   -- PK
        pa_seq       INT            NOT NULL,       -- 마스터키 (n_privacy_agree.pa_seq)
        agree_type   INT            NULL,           -- 1:수집이용 / 2:제3자제공
        client_seq   INT            NULL,           -- 고객사 코드(제3자 제공 시)
        client_name  NVARCHAR(200)  NULL,           -- 고객사명(제3자 제공받는 회사)
        is_agree     INT            NULL,           -- 동의여부(동의 결과)
        is_optional  INT            NULL,           -- 필수여부
        contents     NVARCHAR(MAX)  NULL,           -- 약관 내용(동의 항목 전문. 개인정보 아님)
        create_dt    DATETIME       NULL,           -- 생성일
        CONSTRAINT PK_n_privacy_agree_dtl_home PRIMARY KEY CLUSTERED (pad_seq)
    );

    CREATE INDEX IX_n_privacy_agree_dtl_home_pa_seq ON dbo.n_privacy_agree_dtl (pa_seq);
END
GO
