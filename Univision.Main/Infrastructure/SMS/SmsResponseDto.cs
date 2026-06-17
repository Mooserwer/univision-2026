using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Univision.Main.Infrastructure.SMS
{
    public class SmsSingleSendDto
    {
        public string PhoneSender { get; set; }
        public string PhoneReceiver { get; set; }
        public string Message { get; set; }
        public string Subject { get; set; }
        /// <summary>
        /// SMSType 
        /// 0 : SMS, 1 : LMS
        /// </summary>
        public int smsType { get; set; }
    }

    public class SMSResponseCode
    {
        public static Dictionary<string, string> ResponseCodes = new Dictionary<string, string>();

        static SMSResponseCode()
        {
            ResponseCodes.Add("R000", "성공");
            ResponseCodes.Add("R001", "server busy");
            ResponseCodes.Add("R002", "인증 실패");
            ResponseCodes.Add("R003", "수신자 번호 형식 오류");
            ResponseCodes.Add("R004", "발신자 번호 형식 오류");
            ResponseCodes.Add("R005", "메시지 형식 오류");
            ResponseCodes.Add("R006", "유효하지 않은 TTL");
            ResponseCodes.Add("R007", "유효하지 않은 파라미터 오류");
            ResponseCodes.Add("R008", "스팸 필터링");
            ResponseCodes.Add("R009", "서버 capacity 초과, 재시도 요망");
            ResponseCodes.Add("R010", "등록되지 않은 발신번호 사용");
            ResponseCodes.Add("R011", "발신번호 변작 방지 기준 위반 발신번호 사용");
            ResponseCodes.Add("R012", "해당 서비스유형 전송권한 없음");
            ResponseCodes.Add("R013", "발송 가능 건수 초과");
            ResponseCodes.Add("R999", "알려지지 않은 에러");
            ResponseCodes.Add("R992", "에러코드정의되지않음");
            ResponseCodes.Add("R993", "필수파라미터없음");
            ResponseCodes.Add("9999", "로컬 테스트");
        }
    }

    public enum SmsReportEnum : int
    {
        전송결과확인시작 = 0,
        전송결과확인오류 = 9999,
        성공_단말기에메시지정상도착 = 1000,
        전송시간초과 = 2000,
        전송실패_무선망단 = 2001,
        전송실패_무선망to단말기단 = 2002,
        단말기전원꺼짐 = 2003,
        단말기메시지버퍼풀 = 2004,
        음영지역 = 2005,
        메시지삭제됨 = 2006,
        일시적인단말문제 = 2007,
        전송할수없음 = 3000,
        가입자없음 = 3001,
        성인인증실패 = 3002,
        수신번호형식오류 = 3003,
        단말기서비스일시정지 = 3004,
        단말기호처리상태 = 3005,
        착신거절 = 3006,
        CallbackURL을받을수없는폰 = 3007,
        기타단말기문제 = 3008,
        메시지형식오류 = 3009,
        MMS미지원단말 = 3010,
        서버오류 = 3011,
        스팸 = 3012,
        서비스거부 = 3013,
        기타 = 3014,
        전송경로없음 = 3015,
        첨부파일사이즈제한실패 = 3016,
        휴대폰가입이동통신사의부가서비스 = 3018,
        KISA또는미래부에서발신차단한번호로발신 = 3019,
        CharsetConversionError = 3022

    }
}