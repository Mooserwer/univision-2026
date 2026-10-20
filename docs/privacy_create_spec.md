# 개인정보 동의 요청 페이지 (PrivacyCreate) 연동 규격 — 테스트

> 대상: 헤드헌팅 Pro (호출측) 개발
> 목적: 헤드헌팅 Pro 에서 유니코써치 **개인정보 동의 요청 페이지**를 열어, 정보주체에게 동의 요청(SMS/이메일)을 발송
> 결과 수신: 동의 완료 결과는 별도 문서 **「프로필 메모 조회 API (profile-memos)」** 로 가져갑니다.

---

## 1. 전체 흐름

```
① 헤드헌팅 Pro ──(PrivacyCreate 호출: name, phone|email, id)──▶ 유니코써치 동의 요청 페이지
② 담당자: 동의 항목 확인 → [보내기]
③ 유니코써치 ──(동의 링크 SMS 또는 이메일)──▶ 정보주체
④ 정보주체: 링크 접속 → 이름/생년월일 입력 + 동의
⑤ 유니코써치: 동기화(약 2분 주기) → 완료 알림 발송 + 프로필 메모 생성
⑥ 헤드헌팅 Pro ──(GET /profile-memos)──▶ 동의 결과 메모 수신 → 프로필 반영
```

---

## 2. 호출 URL

```
https://univision.unicosearch.com/Partial/PrivacyCreate
```

- 응답은 **HTML 페이지**(동의 요청 화면)입니다. 새 창/팝업/새 탭으로 열어 사용하는 것을 권장합니다.
- 테스트 기간 중에는 **로그인 없이** 열립니다. (정식 오픈 시 로그인 필요로 변경 예정)

---

## 3. 파라미터

| 파라미터 | 타입 | 필수 | 설명 |
|---|---|---|---|
| `name` | string | **필수** | 정보주체 이름 |
| `phone` | string | 조건부 | 휴대폰번호 (예: `01012345678` 또는 `010-1234-5678`) |
| `email` | string | 조건부 | 이메일 주소 |
| `id` | int | **필수** | 헤드헌팅 Pro **프로필 ID (숫자)**. 동의 결과 메모의 `profile_id` 로 그대로 돌아옵니다 |

- `phone` / `email` 은 **둘 중 하나 필수**입니다.
- **발송 방식**: `phone` 이 있으면 **SMS**, 없으면 **이메일**로 발송됩니다. (둘 다 보내면 SMS 우선)
- `id` 는 반드시 **숫자**로 전달해 주세요. 숫자가 아니면 프로필과 연결되지 않아 결과 메모가 생성되지 않습니다.
- 필수값이 누락되면 페이지 상단에 누락 항목이 빨간 안내로 표시되고 발송할 수 없습니다.

---

## 4. 호출 방식 (3가지 모두 지원)

### 4-1. GET (쿼리스트링) — 가장 간단

```
https://univision.unicosearch.com/Partial/PrivacyCreate?name=홍길동&phone=01012345678&id=123456
https://univision.unicosearch.com/Partial/PrivacyCreate?name=홍길동&email=hong@example.com&id=123456
```

- 한글/특수문자는 **URL 인코딩**해서 전달해 주세요.
  예) `name=%ED%99%8D%EA%B8%B8%EB%8F%99` (홍길동), `email=hong%40example.com`

```javascript
const url = "https://univision.unicosearch.com/Partial/PrivacyCreate"
  + "?name=" + encodeURIComponent(name)
  + "&phone=" + encodeURIComponent(phone)
  + "&id=" + profileId;
window.open(url, "privacyCreate", "width=900,height=900");
```

### 4-2. Form POST — URL 에 개인정보를 남기지 않으려면 권장

```html
<form method="post" action="https://univision.unicosearch.com/Partial/PrivacyCreate" target="_blank">
  <input type="hidden" name="name"  value="홍길동" />
  <input type="hidden" name="phone" value="01012345678" />
  <input type="hidden" name="id"    value="123456" />
</form>
```

### 4-3. JSON POST (fetch/XHR)

```http
POST /Partial/PrivacyCreate HTTP/1.1
Host: univision.unicosearch.com
Content-Type: application/json

{ "name": "홍길동", "phone": "01012345678", "id": 123456 }
```

- 응답은 HTML 이므로, 화면 임베드 등 직접 렌더링이 필요한 경우에만 사용하세요.
- **CORS 허용 Origin**: `https://*.rememberapp.co.kr` (https 만 허용, 허용 메서드 `GET, POST, OPTIONS`)

---

## 5. 페이지 동작

1. 파라미터로 받은 정보주체 1명이 **수신자 정보**에 표시됩니다.
2. 동의 항목(기본 모두 선택)
   - 개인정보 수집·이용 동의
   - 개인정보 제3자 제공 동의
3. **[보내기]** → 확인 → 정보주체에게 동의 링크 발송
   - SMS 발신번호: `02-551-2300` (고정)
   - 이메일 발신: 유니코써치 (고정)
4. 발송 완료 시 "동의서를 발송 했습니다." 안내

---

## 6. ⚠️ 테스트 기간 유의사항

| 항목 | 테스트 기간 동작 |
|---|---|
| 로그인 | 불필요 (정식 오픈 시 필요) |
| **실제 수신처** | **입력한 전화번호/이메일과 무관하게 유니코써치 내부 테스트 수신처로 발송됩니다.** (실제 정보주체에게 발송되지 않음) |
| 화면 표시 | 수신자 정보에는 입력한 값이 그대로 표시됩니다 (실제 수신처와 다름) |
| 이름 / 프로필 ID | 입력값 그대로 저장 → 결과 메모의 `profile_id` 로 반환 |
| 동의 진행 | 테스트 수신처에서 유니코써치가 동의를 진행합니다. 일정 조율이 필요하면 연락 바랍니다 |

---

## 7. 테스트 시나리오 (예시)

| # | 호출 | 기대 결과 |
|---|---|---|
| 1 | `name`, `phone`, `id` (GET) | 수신자 SMS 표시 → 보내기 → 발송 완료 안내 |
| 2 | `name`, `email`, `id` (GET) | 수신자 이메일 표시 → 보내기 → 발송 완료 안내 |
| 3 | Form POST / JSON POST | 1·2 와 동일 |
| 4 | `name` 누락 / `phone`·`email` 모두 누락 / `id` 누락 | 상단에 누락 항목 안내 |
| 5 | 동의 완료 후 `GET /profile-memos?after={cursor}` | 해당 `id` 의 메모 수신<br>`[수신완료(yyyy-MM-dd HH:mm)] 수집 및 이용, 제공 https://www.unicosearch.com/agreesuccess/...` |

> 동의 완료 → 메모 생성까지 **최대 약 2~3분** 소요될 수 있습니다.

---

## 8. 체크리스트 (호출측)
- [ ] 호출 URL / 방식(GET·Form POST·JSON POST) 결정
- [ ] `name` 필수, `phone`/`email` 중 하나, `id` 숫자 프로필 ID
- [ ] 한글/특수문자 URL 인코딩 (GET 사용 시)
- [ ] 새 창/팝업으로 열기
- [ ] 결과 수신은 profile-memos API 연동 (별도 문서)
