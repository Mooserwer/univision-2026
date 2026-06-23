/* =====================================================================
   전자결재 시스템 (Appr) 테이블 스크립트  -  SQL Server
   직접 DB에 반영하세요. (Univision DB)
   ===================================================================== */

/* 1) 기안 문서 ------------------------------------------------------- */
CREATE TABLE dbo.APPR_DOC
(
    ad_seq          INT IDENTITY(1,1) NOT NULL PRIMARY KEY,
    title           NVARCHAR(300)     NOT NULL,
    content         NVARCHAR(MAX)     NULL,
    drafter_seq     INT               NOT NULL,          -- 기안자 uv_seq
    drafter_name    NVARCHAR(50)      NULL,
    drafter_ud_seq  INT               NULL,              -- 기안자 부서
    doc_status      INT               NOT NULL DEFAULT 0,-- 0 임시저장 / 1 진행중 / 2 승인완료 / 3 반려 / 4 회수
    cur_order       INT               NOT NULL DEFAULT 0,-- 현재 결재 차례(order_no)
    copy_from       INT               NULL,              -- 재기안 원본 ad_seq
    reg_date        DATETIME          NOT NULL DEFAULT GETDATE(),
    mod_date        DATETIME          NULL,
    submit_date     DATETIME          NULL,              -- 상신 일시
    complete_date   DATETIME          NULL,              -- 최종 승인/반려 일시
    is_deleted      BIT               NOT NULL DEFAULT 0
);
CREATE INDEX IX_APPR_DOC_drafter ON dbo.APPR_DOC (drafter_seq, doc_status);

/* 2) 결재선 -------------------------------------------------------- */
CREATE TABLE dbo.APPR_LINE
(
    al_seq            INT IDENTITY(1,1) NOT NULL PRIMARY KEY,
    ad_seq            INT               NOT NULL,
    order_no          INT               NOT NULL,        -- 결재 순서 (1,2,3 ...)
    approver_seq      INT               NOT NULL,        -- 결재자 uv_seq
    approver_name     NVARCHAR(50)      NULL,
    approver_position NVARCHAR(50)      NULL,            -- 직급(표시용)
    line_type         INT               NOT NULL DEFAULT 0, -- 0 결재 / 1 참조(열람만)
    line_status       INT               NOT NULL DEFAULT 0, -- 0 대기 / 1 승인 / 2 반려
    process_date      DATETIME          NULL,
    opinion           NVARCHAR(1000)    NULL             -- 결재 의견/반려 사유
);
CREATE INDEX IX_APPR_LINE_doc      ON dbo.APPR_LINE (ad_seq, order_no);
CREATE INDEX IX_APPR_LINE_approver ON dbo.APPR_LINE (approver_seq, line_status);

/* 3) 첨부파일 ------------------------------------------------------ */
CREATE TABLE dbo.APPR_FILE
(
    af_seq           INT IDENTITY(1,1) NOT NULL PRIMARY KEY,
    ad_seq           INT               NOT NULL,
    file_dir         NVARCHAR(500)     NULL,
    file_origin_path NVARCHAR(500)     NULL,
    file_path        NVARCHAR(300)     NULL,             -- 저장 파일명
    file_extension   NVARCHAR(20)      NULL,
    file_size        BIGINT            NULL,
    reg_date         DATETIME          NOT NULL DEFAULT GETDATE()
);
CREATE INDEX IX_APPR_FILE_doc ON dbo.APPR_FILE (ad_seq);

/* 4) 댓글 ---------------------------------------------------------- */
CREATE TABLE dbo.APPR_COMMENT
(
    ac_seq       INT IDENTITY(1,1) NOT NULL PRIMARY KEY,
    ad_seq       INT               NOT NULL,
    writer_seq   INT               NOT NULL,
    writer_name  NVARCHAR(50)      NULL,
    content      NVARCHAR(2000)    NULL,
    reg_date     DATETIME          NOT NULL DEFAULT GETDATE(),
    is_deleted   BIT               NOT NULL DEFAULT 0
);
CREATE INDEX IX_APPR_COMMENT_doc ON dbo.APPR_COMMENT (ad_seq);
