# 개인정보 수집·제3자 제공 동의 — 홈페이지(공개 동의 페이지) 연동 규격

> 대상: 홈페이지(Classic ASP) 개발
> 관련 URL 예시: `https://www.unicosearch.com/agreeprovide/6B65795F393239335F6B6579`

---

## 1. 홈페이지의 역할 (요약)

유니비전이 정보주체에게 SMS/이메일로 **동의 링크**를 발송합니다. 정보주체가 그 링크로 들어오면 홈페이지가:

1. 링크의 **키로 동의 레코드를 조회**
2. **약관(동의 항목) 표시**
3. 정보주체가 **동의 체크 + 이름·생년월일 입력** 후 제출
4. **동의 결과를 홈페이지 DB에 저장** (끝)

> ⚠️ **알림(SMS/메일) 발송은 홈페이지에서 하지 않습니다.** 유니비전이 10분마다 홈페이지 DB를 풀링하여 완료 알림과 후처리를 담당합니다. 홈페이지는 **DB 저장까지만** 하면 됩니다.

### 전체 흐름
```
유니비전 ──(동의 링크 SMS/Email)──▶ 정보주체
정보주체 ──(링크 접속)──▶ 홈페이지 공개 페이지
   조회 → 약관 표시 → 이름/생년월일 입력 + 동의 → 홈페이지 DB 저장(is_sync=0)
유니비전(10분 풀링) ──▶ 내부 반영 + 완료 알림 + 메모 기록 + is_sync=1
```

---

## 2. 동의 링크 & 키 해독

링크 형식: `/agreeprovide/{HEXKEY}`

- `HEXKEY` 는 문자열 `key_{pa_seq}_key` 를 **ASCII 16진수**로 인코딩한 값입니다.
- 예시: `pa_seq = 9293`
  - 원문 `key_9293_key`
  - HEX `6B65795F393239335F6B6579`

### 해독 방법 (HEX → pa_seq)
1. HEX 문자열을 2자리씩 끊어 ASCII 문자로 변환 → `key_9293_key`
2. 앞뒤 `key_` / `_key` 제거 → `9293`
3. 정수 `pa_seq` 획득

이 `pa_seq` 로 아래 테이블을 조회합니다. (또는 `agree_code3 = HEXKEY` 로 바로 조회해도 됩니다. `agree_code3` 에 동일 HEX 가 저장되어 있고 인덱스가 있습니다.)

---

## 3. 홈페이지 DB 테이블

유니비전 발송 시, 홈페이지 DB에 아래 2개 테이블의 레코드가 **미리 생성**됩니다. (개인정보 최소화: 이름/연락처 등 식별정보는 저장 안 됨)

### 3-1. `n_privacy_agree` (마스터)
| 컬럼 | 타입 | 발송 시(유니비전) | 동의 시(홈페이지가 기록) |
|---|---|---|---|
| `pa_seq` | int (PK) | 세팅됨 | (조회키) |
| `send_type` | nvarchar(10) | `S`(SMS)/`E`(Email) | - |
| `send_dt` | datetime | 세팅됨 | - |
| `agree_code3` | nvarchar(200) | 링크 키(HEX) | (조회키) |
| `agree_code1` | nvarchar(100) | NULL | **이름(평문)** 입력값 저장 |
| `agree_code2` | nvarchar(100) | NULL | **생년월일(해시)** 저장 (§5) |
| `agree_dt` | datetime | NULL | **동의일시** = 현재시각 |
| `agree_ip` | nvarchar(50) | NULL | **동의자 IP** |
| `create_dt` | datetime | 세팅됨 | - |
| `is_sync` | int | `0` | **그대로 0 유지** (유니비전이 1로 변경) |
| `sync_dt` | datetime | NULL | 건드리지 않음 |

### 3-2. `n_privacy_agree_dtl` (상세 = 동의 항목별)
| 컬럼 | 타입 | 발송 시 | 동의 시 |
|---|---|---|---|
| `pad_seq` | int (PK) | 세팅됨 | - |
| `pa_seq` | int | 세팅됨 | (조회키) |
| `agree_type` | int | `1`=수집·이용 / `2`=제3자 제공 | - |
| `client_name` | nvarchar(200) | 제3자 제공 시 회사명 | - |
| `contents` | nvarchar(max) | **약관 전문** | - |
| `is_agree` | int | NULL | **항목별 동의여부**(동의=1) |
| `is_optional` | int | 필수여부 | - |
| `create_dt` | datetime | 세팅됨 | - |

---

## 4. 공개 페이지 동작 (단계)

1. URL 의 `HEXKEY` 해독 → `pa_seq`
2. `SELECT * FROM n_privacy_agree WHERE pa_seq = @pa_seq` (없으면 만료/오류 처리)
   - 이미 `agree_dt` 가 있으면 "이미 동의 완료" 안내
3. `SELECT * FROM n_privacy_agree_dtl WHERE pa_seq = @pa_seq ORDER BY agree_type`
   - 각 항목의 `contents`(약관)를 화면에 표시
4. 정보주체 입력 화면:
   - 동의 항목별 **동의 체크박스**
   - **이름**, **생년월일** 입력 (증빙용)
5. 제출 시 저장:
   - `UPDATE n_privacy_agree SET agree_code1=@이름, agree_code2=@생년월일해시, agree_dt=GETDATE(), agree_ip=@IP WHERE pa_seq=@pa_seq`
   - 각 동의 항목: `UPDATE n_privacy_agree_dtl SET is_agree=1 WHERE pad_seq=@pad_seq` (동의한 항목만)
   - **`is_sync` 는 0 그대로 둡니다.**
6. 완료 화면 표시 (알림 발송은 하지 않음)

---

## 5. 생년월일 해시 규격 ★중요

생년월일은 **평문으로 저장하지 않고 해시**로 저장합니다. (개인정보 최소화)

- **입력 정규화**: 숫자 8자리 `YYYYMMDD` (하이픈/공백 제거). 예 `1990-01-01` → `19900101`
- **salt**: `pa_seq` 를 섞습니다 (단독 생년월일 해시는 경우의 수가 적어 역산 쉬움 → 레코드별 salt).
- **공식**:
  ```
  agree_code2 = SHA-256( pa_seq + "|" + YYYYMMDD )   // 소문자 16진수 64자
  ```
  예) `pa_seq=9293`, 생년월일=`19900101` → 해시 입력 문자열 **`9293|19900101`** → SHA-256 → hex
- **인코딩 걱정 없음**: 입력이 전부 ASCII(숫자/`|`)라 Classic ASP(VBScript) ↔ .NET 결과가 동일합니다.
  - Classic ASP 는 **순수 VBScript SHA-256 함수**(COM 컴포넌트 불필요, .asp include)로 계산하면 됩니다. (호스팅 제약 무관)
- 유니비전은 이 해시값을 **그대로 저장/보관**합니다(검증 안 함). 추후 수동 대조가 필요하면 동일 공식으로 재계산합니다.

> 이름(`agree_code1`)은 평문으로 저장합니다.

---

## 6. 유니비전 측 후처리 (참고 — 홈페이지 작업 아님)

유니비전이 **10분마다** (Windows 작업 스케줄러) 홈페이지 DB를 풀링:
- `WHERE is_sync = 0 AND agree_dt IS NOT NULL` 인 건을 가져와
- 내부 DB 반영 + **완료 알림**(정보주체 SMS/Email + 발신자 Email) + **프로필 메모 기록**
- 처리 후 홈페이지 `n_privacy_agree.is_sync = 1`, `sync_dt` 세팅

→ 그래서 홈페이지는 **`is_sync`/`sync_dt` 를 절대 건드리지 않아야** 중복/누락 없이 동기화됩니다.

---

## 7. 체크리스트 (홈페이지 작업)
- [ ] HEXKEY 해독 → pa_seq
- [ ] `n_privacy_agree` / `n_privacy_agree_dtl` 조회 → 약관 표시
- [ ] 이미 동의된 건(`agree_dt` 존재) 처리
- [ ] 이름/생년월일 입력 + 동의 체크 UI
- [ ] 생년월일 SHA-256 해시(§5 공식) — 순수 VBScript 함수
- [ ] 저장: `agree_code1`(이름)/`agree_code2`(해시)/`agree_dt`/`agree_ip` + `n_privacy_agree_dtl.is_agree`
- [ ] `is_sync` 미변경(0 유지)
- [ ] 알림 발송 없음
