# 프로필 메모 조회 API (profile-memos) 연동 규격

> 대상: 헤드헌팅 Pro (수신측) 개발
> 목적: 유니코써치에서 처리된 **개인정보 동의 결과**를 프로필 메모 형태로 가져가기 위한 조회 API

---

## 1. 개요

헤드헌팅 Pro 에서 유니코써치로 동의 요청을 보낸 뒤, 정보주체가 동의를 완료하면 유니코써치가 해당 프로필(`profile_id`)에 대한 메모를 생성합니다.
수신측은 이 API 를 **주기적으로 호출(폴링)** 하여 새로 생성된 메모를 가져가 프로필에 반영하면 됩니다.

```
헤드헌팅 Pro ──(동의 요청: PrivacyCreate, id=profile_id)──▶ 유니코써치
정보주체 ──(동의 완료)──▶ 유니코써치 (약 2분 주기 동기화 후 메모 생성)
헤드헌팅 Pro ──(GET /profile-memos?after=...)──▶ 메모 수신 → 프로필 반영
```

---

## 2. 요청

| 항목 | 내용 |
|---|---|
| Method | `GET` |
| URL | `{BASE_URL}/profile-memos?after={cursor}&limit={n}` |
| BASE_URL | 별도 전달 |

### 2-1. 인증 (필수)

| 구분 | 내용 |
|---|---|
| Header | `APIKey: {발급키}` — 키 값은 별도 전달 |
| 접속 IP | **허용 IP 에서만 호출 가능** — 호출 서버의 공인(Outbound) IP 를 사전에 전달해 주세요 |

### 2-2. 쿼리 파라미터

| 파라미터 | 타입 | 필수 | 기본값 | 설명 |
|---|---|---|---|---|
| `after` | int | 선택 | `0` | 커서. **마지막으로 받은 `memo_id`**. 이 값보다 큰 메모만 반환. 미지정(0) 시 처음부터 |
| `limit` | int | 선택 | `50` | 한 번에 받을 건수. 최대 `200` (초과 시 200으로 보정) |

### 2-3. 요청 예시

```http
GET /profile-memos?after=0&limit=50 HTTP/1.1
Host: {BASE_URL}
APIKey: {발급키}
```

```bash
curl -H "APIKey: {발급키}" "{BASE_URL}/profile-memos?after=0&limit=50"
```

---

## 3. 응답

### 3-1. 성공 (HTTP 200)

`memo_id` **오름차순** JSON 배열. 새 메모가 없으면 빈 배열 `[]`.

| 필드 | 타입 | 설명 |
|---|---|---|
| `memo_id` | int | 메모 고유번호 (커서로 사용) |
| `profile_id` | int | 대상 프로필 ID (동의 요청 시 전달한 `id`) |
| `content` | string | 메모 내용 (최대 300자) |
| `created_at` | string | 생성일시 (한국시간 KST, `yyyy-MM-ddTHH:mm:ss` 형식) |

```json
[
  {
    "memo_id": 101,
    "profile_id": 327091,
    "content": "[수신완료(2026-10-20 13:46)] 수집 및 이용, 제공 https://www.unicosearch.com/agreesuccess/6B65795F3332373039315F6B6579",
    "created_at": "2026-10-20T13:48:02.193"
  },
  {
    "memo_id": 102,
    "profile_id": 327155,
    "content": "[수신완료(2026-10-20 14:02)] 수집 및 이용 https://www.unicosearch.com/agreesuccess/6B65795F3332373135355F6B6579",
    "created_at": "2026-10-20T14:04:11.507"
  }
]
```

### 3-2. 메모 내용(`content`) 형식

```
[수신완료(동의일시 yyyy-MM-dd HH:mm)] {동의항목} {동의내역 확인 URL}
```

| 동의항목 표기 | 의미 |
|---|---|
| `수집 및 이용` | 개인정보 수집·이용 동의 |
| `제공` | 개인정보 제3자 제공 동의 |
| `수집 및 이용, 제공` | 두 항목 모두 동의 |

- 동의내역 확인 URL(`https://www.unicosearch.com/agreesuccess/...`)은 정보주체의 동의 내역을 확인할 수 있는 페이지입니다.
- 화면에는 `content` 를 **그대로 표시**하는 것을 권장합니다. (형식은 추후 변경될 수 있으므로 문자열 파싱 의존은 지양)

### 3-3. 오류

| 상황 | HTTP | 응답 본문 |
|---|---|---|
| APIKey 누락/불일치 | 403 | `"Invalid API Key"` |
| 허용되지 않은 IP | 403 | `"Not allowed IP Address : {IP}"` |
| 서버 내부 오류 | 200 | `{ "result": -1, "message": "..." }` ← **배열이 아닌 객체** |

> 서버 내부 오류는 HTTP 200 으로 객체가 반환됩니다. 응답이 **배열인지 먼저 확인**한 뒤 처리하고, 객체이면 오류로 간주하여 커서를 갱신하지 말고 다음 주기에 재시도해 주세요.

---

## 4. 권장 연동 방식 (커서 폴링)

1. 최초 1회: `after=0` 으로 호출
2. 응답 배열의 **마지막 `memo_id`** 를 커서로 저장
3. 다음 호출부터 `after={저장한 커서}`
4. 응답 건수가 `limit` 과 같으면 남은 데이터가 있을 수 있으므로 **즉시 이어서 호출**, 빈 배열이면 다음 주기까지 대기
5. 메모 반영이 끝난 뒤에 커서를 저장 (반영 실패 시 커서 미갱신 → 재조회)

```text
cursor = load() ?? 0
loop:
  list = GET /profile-memos?after={cursor}&limit=200
  if list is not array: break   // 오류 → 다음 주기 재시도
  for m in list: apply(m.profile_id, m.content)
  if list.length > 0: cursor = list[last].memo_id; save(cursor)
  if list.length < 200: break
```

- 호출 주기: **5분 내외 권장** (유니코써치 동기화 주기 약 2분)
- `memo_id` 는 증가만 하며 기존 메모는 수정되지 않습니다. 같은 `profile_id` 에 메모가 여러 건 생길 수 있습니다(재동의 등).

---

## 5. 체크리스트 (수신측)
- [ ] BASE_URL / APIKey 수령
- [ ] 호출 서버 공인 IP 전달 (허용 IP 등록)
- [ ] `APIKey` 헤더 포함 호출
- [ ] 커서(`after`) 저장/갱신 로직
- [ ] 응답이 객체(`result: -1`)인 경우 오류 처리
- [ ] `profile_id` 로 프로필 매칭 후 `content` 메모 반영
