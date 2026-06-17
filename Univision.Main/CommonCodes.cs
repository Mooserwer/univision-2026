using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Univision.Main
{
  public class CommonCodes
  {
    public const int isUse = 0;
    public const int isDeleted = 1;

    public const string Create = "C";
    public const string Update = "U";
    public const string Delete = "D";

  }

  public class GenderCode
  {
    public const double unknown = 0;
    public const double male = 1;
    public const double female = 2;

    public Dictionary<double, string> GenderCodes = new Dictionary<double, string>();

    public GenderCode()
    {

      GenderCodes.Add(GenderCode.unknown, "성별모름");
      GenderCodes.Add(GenderCode.male, "남");
      GenderCodes.Add(GenderCode.female, "여");
    }
  }

  public class VatTypeCode
  {
    public const int incvat = 0;
    public const int excvat = 1;
    public const int zerovat = 2;
    public const int freevat = 3;

    public Dictionary<int, string> VatTypeCodes = new Dictionary<int, string>();

    public VatTypeCode()
    {

      VatTypeCodes.Add(VatTypeCode.incvat, "VAT포함");
      VatTypeCodes.Add(VatTypeCode.excvat, "VAT별도");
      VatTypeCodes.Add(VatTypeCode.zerovat, "영세율");
      VatTypeCodes.Add(VatTypeCode.freevat, "면세");
    }
  }


  public class ContractFeeTypeCode
  {
    public const string salary_base = "A";
    public const string position_base = "B";
    public const string flat = "C";

    public Dictionary<string, string> ContractFeeTypeCodes = new Dictionary<string, string>();

    public ContractFeeTypeCode()
    {

      ContractFeeTypeCodes.Add(ContractFeeTypeCode.salary_base, "연봉별");
      ContractFeeTypeCodes.Add(ContractFeeTypeCode.position_base, "직급별");
      ContractFeeTypeCodes.Add(ContractFeeTypeCode.flat, "단일요율");
    }
  }

  public class RegCDTypeCode
  {
    public const int direct = 1;
    public const int irdata = 2;
    public const int research = 3;
    public const int referral = 4;    
    public const int etc = 9;

    public Dictionary<int, string> RegCDTypeCodes = new Dictionary<int, string>();

    public RegCDTypeCode()
    {
      RegCDTypeCodes.Add(RegCDTypeCode.direct, "Applicant(직접지원)");
      RegCDTypeCodes.Add(RegCDTypeCode.irdata, "IR data(기업 공시자료)");
      RegCDTypeCodes.Add(RegCDTypeCode.research, "Market research(임원동향조사)");
      RegCDTypeCodes.Add(RegCDTypeCode.referral, "Referral(소개)");
      RegCDTypeCodes.Add(RegCDTypeCode.etc, "Etc");
    }
  }

  public class ExecTypeCode
  {
    public const int id = 1;
    public const int od = 2;
    public const int cm = 3;    
    public const int etc = 9;

    public Dictionary<int, string> ExecTypeCodes = new Dictionary<int, string>();

    public ExecTypeCode()
    {
      ExecTypeCodes.Add(ExecTypeCode.id, "사내");
      ExecTypeCodes.Add(ExecTypeCode.od, "사외");
      ExecTypeCodes.Add(ExecTypeCode.cm, "일반");
      ExecTypeCodes.Add(ExecTypeCode.etc, "기타");
    }
  }

  public class RegTypeCode
  {
    public const int outsearch = 1;
    public const int online = 2;
    public const int applicant = 3;
    public const int outplace = 4;
    public const int linkedin = 5;
    public const int frnd = 8;
    public const int remembercareer = 21;
    public const int saramin = 22;
    public const int jobkorea = 23;

    public const int resume_mail = 91;
    public const int etc = 9;

    public Dictionary<int, string> RegTypeCodes = new Dictionary<int, string>();

    public RegTypeCode()
    {
      RegTypeCodes.Add(RegTypeCode.outsearch, "Out search");
      RegTypeCodes.Add(RegTypeCode.remembercareer, "Online DB - 리멤버");
      RegTypeCodes.Add(RegTypeCode.saramin, "Online DB - 사람인");
      RegTypeCodes.Add(RegTypeCode.jobkorea, "Online DB - 잡코리아");      
      RegTypeCodes.Add(RegTypeCode.online, "Online DB - 그 외 잡포털");
      RegTypeCodes.Add(RegTypeCode.applicant, "Applicant");
      RegTypeCodes.Add(RegTypeCode.outplace, "Outplacement");
      RegTypeCodes.Add(RegTypeCode.linkedin, "Online DB - Linkedin");
      RegTypeCodes.Add(RegTypeCode.frnd, "등록자 지인");
      RegTypeCodes.Add(RegTypeCode.etc, "Etc");
      RegTypeCodes.Add(RegTypeCode.resume_mail, "홈페이지 회원가입");
    }
  }


  public class WorkCode
  {
    public const int notwork = 0;
    public const int work = 1;

    public Dictionary<int, string> WorkCodes = new Dictionary<int, string>();

    public WorkCode()
    {
      WorkCodes.Add(WorkCode.notwork, "퇴사");
      WorkCodes.Add(WorkCode.work, "재직");
    }
  }

  public class ProjectTypeCode
  {
    public const int hire = 1;
    public const int repitation = 2;
    public const int rejoin = 3;
    public const int etc = 4;

    public static Dictionary<int, string> ProjectTypeCodes = new Dictionary<int, string>();

    static ProjectTypeCode()
    {
      ProjectTypeCodes.Add(ProjectTypeCode.hire, "채용");
      ProjectTypeCodes.Add(ProjectTypeCode.repitation, "평판조회");
      ProjectTypeCodes.Add(ProjectTypeCode.rejoin, "재취업");
      ProjectTypeCodes.Add(ProjectTypeCode.etc, "기타");
    }
  }

  /// <summary>
  /// 프로젝트 진행 상태
  /// </summary>
  public class ProjectStatusCode
  {
    public const int progress = 1;
    public const int hold = 2;
    public const int fail = 3;
    public const int complete = 4;
    public const int success = 5;

    public static Dictionary<int, string> ProjectStatusCodes = new Dictionary<int, string>();

    static ProjectStatusCode()
    {
      ProjectStatusCodes.Add(ProjectStatusCode.progress, "진행");
      ProjectStatusCodes.Add(ProjectStatusCode.hold, "보류");
      ProjectStatusCodes.Add(ProjectStatusCode.fail, "종료(취소,실패)");
      ProjectStatusCodes.Add(ProjectStatusCode.complete, "완료");
      ProjectStatusCodes.Add(ProjectStatusCode.success, "성공");
    }
  }

  /// <summary>
  /// 프로젝트 진행 상태
  /// </summary>
  public class ProjectHistoryStateCode
  {
    public const int all = 0;
    public const int interest = 20;
    public const int recommand = 30;
    public const int paper = 40;
    public const int interview = 50;
    public const int interviewOk = 60;
    public const int hireReady = 70;
    public const int hireOk = 80;
    public const int fail = 10;
    public const int selfdrop = 13;
    public const int no_intrest = 15;


    public static Dictionary<int, string> ProjectHistoryStateCodes = new Dictionary<int, string>();

    static ProjectHistoryStateCode()
    {
      ProjectHistoryStateCodes.Add(ProjectHistoryStateCode.all, "전체");
      ProjectHistoryStateCodes.Add(ProjectHistoryStateCode.interest, "잠재후보");
      ProjectHistoryStateCodes.Add(ProjectHistoryStateCode.recommand, "추천");
      ProjectHistoryStateCodes.Add(ProjectHistoryStateCode.paper, "서류통과");
      ProjectHistoryStateCodes.Add(ProjectHistoryStateCode.interview, "면접");
      ProjectHistoryStateCodes.Add(ProjectHistoryStateCode.interviewOk, "면접통과");
      ProjectHistoryStateCodes.Add(ProjectHistoryStateCode.hireReady, "협상&검증");
      ProjectHistoryStateCodes.Add(ProjectHistoryStateCode.hireOk, "입사확정");
      ProjectHistoryStateCodes.Add(ProjectHistoryStateCode.fail, "탈락(Drop)");
      ProjectHistoryStateCodes.Add(ProjectHistoryStateCode.selfdrop, "Self drop");
      ProjectHistoryStateCodes.Add(ProjectHistoryStateCode.no_intrest, "관심없음(Not interested)");

    }
  }

  public class SchoolGraduateState
  {
    public const int graduated = 0;
    public const int graduation = 1;
    public const int attend = 2;
    public const int drop = 3;
    public const int completion = 4;
    public const int rest = 5;

    public static Dictionary<int, string> SchoolGraduateStates = new Dictionary<int, string>();

    static SchoolGraduateState()
    {
      SchoolGraduateStates.Add(SchoolGraduateState.graduated, "졸업");
      SchoolGraduateStates.Add(SchoolGraduateState.graduation, "졸업예정");
      SchoolGraduateStates.Add(SchoolGraduateState.attend, "재학중");
      SchoolGraduateStates.Add(SchoolGraduateState.drop, "중퇴");
      SchoolGraduateStates.Add(SchoolGraduateState.completion, "수료");
      SchoolGraduateStates.Add(SchoolGraduateState.rest, "휴학");
    }
  }

  public class SchoolEducationState
  {
    public const int high = 1;
    public const int college = 2;
    public const int univ = 3;
    public const int master = 4;
    public const int doctor = 5;
    public const int post_doctor = 6;

    public static Dictionary<int, string> SchoolEducationStates = new Dictionary<int, string>();

    static SchoolEducationState()
    {
      SchoolEducationStates.Add(SchoolEducationState.high, "고등");
      SchoolEducationStates.Add(SchoolEducationState.college, "전문대");
      SchoolEducationStates.Add(SchoolEducationState.univ, "학사");
      SchoolEducationStates.Add(SchoolEducationState.master, "석사");
      SchoolEducationStates.Add(SchoolEducationState.doctor, "박사");
      SchoolEducationStates.Add(SchoolEducationState.post_doctor, "PostDoc");
    }
  }

  /// <summary>
  /// 스케줄 타입 (수정될 때 Core의 ScheduleCount.cs 파일 내부의 scheduleType 또한 고쳐야함.
  /// </summary>
  public class ScheduleType
  {
    public const int personal = 0;
    public const int company = 1;
    public const int team = 2;
    public const int share = 3;

    public const string personalColor = "#589ffc";
    public const string companyColor = "#11c26d";
    public const string teamColor = "#0bb2d4";
    public const string shareColor = "#eb6709";


    public static Dictionary<int, string> ScheduleTypes = new Dictionary<int, string>();

    static ScheduleType()
    {
      ScheduleTypes.Add(ScheduleType.personal, "개인");
      ScheduleTypes.Add(ScheduleType.company, "회사");
      ScheduleTypes.Add(ScheduleType.team, "팀");
      ScheduleTypes.Add(ScheduleType.share, "특정인[추가 선택]");
    }
  }

  public class ScheduleSubType
  {
    public const int normal = 1;
    public const int client = 2;
    public const int candidate = 3;
    public const int vacation = 4;

    public static Dictionary<int, string> ScheduleSubTypes = new Dictionary<int, string>();

    static ScheduleSubType()
    {
      ScheduleSubTypes.Add(ScheduleSubType.normal, "일반");
      ScheduleSubTypes.Add(ScheduleSubType.client, "고객사");
      ScheduleSubTypes.Add(ScheduleSubType.candidate, "후보자");
      ScheduleSubTypes.Add(ScheduleSubType.vacation, "휴가");
    }
  }

  public class SMSType
  {
    public const int sms = 0;
    public const int lms = 1;
  }

  public class SMSSendType
  {
    public const int cellphone = 0;
    public const int companyPhone = 1;
  }

  /// <summary>
  /// 클라이언트 계약 수수료 타입
  /// </summary>
  public class ContractFeeType
  {
    public const string annual = "A";
    public const string position = "B";
    public const string fix = "C";

    public static Dictionary<string, string> ContractFeeTypes = new Dictionary<string, string>();

    static ContractFeeType()
    {
      ContractFeeTypes.Add(ContractFeeType.annual, "연봉");
      ContractFeeTypes.Add(ContractFeeType.position, "직급");
      ContractFeeTypes.Add(ContractFeeType.fix, "고정");
    }
  }

  /// <summary>
  /// 후보자 주의 요망사항 타입.
  /// </summary>
  public class CandidateCautionType
  {
    public const int no = 0;
    public const int caution = 1;
    public const int careerCaution = 2;
    public const int moralCaution = 3;
    public const int interviewCaution = 4;
    public const int humanityCaution = 5;
    public const int unicoHire = 6;
    public const int exit = 7;
    public const int blank = 999;
    public const int client = 11;
    public const int confi = 12;
    public const int final = 13;

    public Dictionary<int, string> nsCandidateCautionTypes = new Dictionary<int, string>();
    public static Dictionary<int, string> CandidateCautionTypes = new Dictionary<int, string>();

    static CandidateCautionType()
    {
      CandidateCautionTypes.Add(CandidateCautionType.no, "해당 없음");
      CandidateCautionTypes.Add(CandidateCautionType.client, "현재 고객사의 채용 담당자");
      CandidateCautionTypes.Add(CandidateCautionType.confi, "후보자의 상황이 민감하여 주의가 필요한 경우");
      CandidateCautionTypes.Add(CandidateCautionType.final, "최종면접 예정 혹은 이후 단계");
      CandidateCautionTypes.Add(CandidateCautionType.blank, "===========================================");
      CandidateCautionTypes.Add(CandidateCautionType.caution, "주의요망");
      CandidateCautionTypes.Add(CandidateCautionType.careerCaution, "주의요망(경력 결함)");
      CandidateCautionTypes.Add(CandidateCautionType.moralCaution, "주의요망(도덕 결함)");
      CandidateCautionTypes.Add(CandidateCautionType.interviewCaution, "주의요망(인터뷰 결함)");
      CandidateCautionTypes.Add(CandidateCautionType.humanityCaution, "주의요망(인성 결함)");
      CandidateCautionTypes.Add(CandidateCautionType.unicoHire, "유니코를 통한 입사");
      CandidateCautionTypes.Add(CandidateCautionType.exit, "탈퇴요청");

    }

    public CandidateCautionType()
    {

      nsCandidateCautionTypes.Add(CandidateCautionType.no, "해당 없음");
      nsCandidateCautionTypes.Add(CandidateCautionType.client, "현재 고객사의 채용 담당자");
      nsCandidateCautionTypes.Add(CandidateCautionType.confi, "후보자의 상황이 민감하여 주의가 필요한 경우");
      nsCandidateCautionTypes.Add(CandidateCautionType.final, "최종면접 예정 혹은 이후 단계");
      nsCandidateCautionTypes.Add(CandidateCautionType.blank, "===========================================");
      nsCandidateCautionTypes.Add(CandidateCautionType.caution, "주의요망");
      nsCandidateCautionTypes.Add(CandidateCautionType.careerCaution, "주의요망(경력 결함)");
      nsCandidateCautionTypes.Add(CandidateCautionType.moralCaution, "주의요망(도덕 결함)");
      nsCandidateCautionTypes.Add(CandidateCautionType.interviewCaution, "주의요망(인터뷰 결함)");
      nsCandidateCautionTypes.Add(CandidateCautionType.humanityCaution, "주의요망(인성 결함)");
      nsCandidateCautionTypes.Add(CandidateCautionType.unicoHire, "유니코를 통한 입사");
      nsCandidateCautionTypes.Add(CandidateCautionType.exit, "탈퇴요청");
    }
  }

  public class CandidateAttentionType
  {
    public const int no = 0;
    public const int client = 1;    
    public const int confi = 2;
    public const int final = 3;
    public const int reff = 4;
    public const int etc = 99;

    public Dictionary<int, string> nsCandidateAttentionTypes = new Dictionary<int, string>();
    public static Dictionary<int, string> CandidateAttentionTypes = new Dictionary<int, string>();

    static CandidateAttentionType()
    {
      CandidateAttentionTypes.Add(CandidateAttentionType.no, "해당 없음");      
      CandidateAttentionTypes.Add(CandidateAttentionType.client, "현재 고객사의 채용 담당자");      
      CandidateAttentionTypes.Add(CandidateAttentionType.confi, "후보자의 상황이 민감하여 주의가 필요");
      CandidateAttentionTypes.Add(CandidateAttentionType.final, "최종면접 예정 혹은 이후 단계");
      CandidateAttentionTypes.Add(CandidateAttentionType.reff, "경력, 도덕, 인성 결함, 평판 등의 이슈");
      CandidateAttentionTypes.Add(CandidateAttentionType.etc, "기타");
    }

    public CandidateAttentionType()
    {
      nsCandidateAttentionTypes.Add(CandidateAttentionType.no, "해당 없음");      
      nsCandidateAttentionTypes.Add(CandidateAttentionType.client, "현재 고객사의 채용 담당자");
      nsCandidateAttentionTypes.Add(CandidateAttentionType.confi, "후보자의 상황이 민감하여 주의가 필요");
      nsCandidateAttentionTypes.Add(CandidateAttentionType.final, "최종면접 예정 혹은 이후 단계");
      nsCandidateAttentionTypes.Add(CandidateAttentionType.reff, "경력, 도덕, 인성 결함, 평판 등의 이슈");
      nsCandidateAttentionTypes.Add(CandidateAttentionType.etc, "기타");
    }
  }

  public class MeetingRoomUsage
  {
    public const string client = "client";
    public const string cand = "cand";
    public const string team = "team";
    public const string edu = "edu";
    public const string oth = "oth";

    public static Dictionary<string, string> MeetingRoomUsages = new Dictionary<string, string>();

    static MeetingRoomUsage()
    {
      MeetingRoomUsages.Add(MeetingRoomUsage.client, "고객사미팅");
      MeetingRoomUsages.Add(MeetingRoomUsage.cand, "후보자미팅");
      MeetingRoomUsages.Add(MeetingRoomUsage.team, "팀회의");
      MeetingRoomUsages.Add(MeetingRoomUsage.oth, "교육");
      MeetingRoomUsages.Add(MeetingRoomUsage.oth, "기타");
    }
  }

  /// <summary>
  /// 휴가 타입
  /// </summary>
  public class VacationType
  {
    public const int vacation = 1;
    public const int noReg = 2;
    public const int sick = 3;
    public const int goodsad = 4;
    public const int military = 5;
    public const int home = 10;
    public const int shift_home = 11;
    public const int nomoney = 12;
    public const int special = 13;
    public const int etc = 6;
    public const int outwork = 7;

    public static Dictionary<int, string> VacationTypes = new Dictionary<int, string>();

    static VacationType()
    {
      VacationTypes.Add(VacationType.vacation, "연차");
      VacationTypes.Add(VacationType.noReg, "출근미등록");
      VacationTypes.Add(VacationType.sick, "유급병가");
      VacationTypes.Add(VacationType.goodsad, "경조사");
      VacationTypes.Add(VacationType.military, "예비군");
      VacationTypes.Add(VacationType.home, "재택근무");
      VacationTypes.Add(VacationType.shift_home, "Shift 자택근무");
      VacationTypes.Add(VacationType.nomoney, "무급휴가");
      VacationTypes.Add(VacationType.special, "장기근속 특별휴가");
      VacationTypes.Add(VacationType.etc, "기타");
      VacationTypes.Add(VacationType.outwork, "외근");
    }
  }

  /// <summary>
  /// 직급 코드
  /// </summary>
  public class PositionCode
  {
    public const int ceo = 10;
    //public const int cfo = 20;
    public const int executive = 30;
    //public const int general = 40;
    public const int director = 50;
    //public const int deputy = 60;
    public const int manager = 70;
    public const int assistant = 80;
    //public const int senior = 90;
    public const int staff = 100;
    public const int freelancer = 110;


    public static Dictionary<int, string> PositionCodes = new Dictionary<int, string>();

    static PositionCode()
    {
      PositionCodes.Add(PositionCode.ceo, "최고경영자 (CEO, 지사장)");
      //PositionCodes.Add(PositionCode.cfo, "최고재무경영자");
      PositionCodes.Add(PositionCode.executive, "임원급");
      //PositionCodes.Add(PositionCode.general, "본부장/지사장");
      PositionCodes.Add(PositionCode.director, "부장급");
      //PositionCodes.Add(PositionCode.deputy, "차장급");
      PositionCodes.Add(PositionCode.manager, "중간관리자(과,차장급)");
      PositionCodes.Add(PositionCode.assistant, "사원-대리급");
      //PositionCodes.Add(PositionCode.senior, "경력사원급");
      PositionCodes.Add(PositionCode.staff, "신입사원급");
      PositionCodes.Add(PositionCode.freelancer, "프리랜서(비정규 전문직)");
    }
    /*
    static PositionCode()
    {
      PositionCodes.Add(PositionCode.ceo, "최고 경영자(");
      PositionCodes.Add(PositionCode.cfo, "최고재무경영자");
      PositionCodes.Add(PositionCode.executive, "임원급");
      PositionCodes.Add(PositionCode.general, "본부장/지사장");
      PositionCodes.Add(PositionCode.director, "부장급");
      PositionCodes.Add(PositionCode.deputy, "차장급");
      PositionCodes.Add(PositionCode.manager, "과장급");
      PositionCodes.Add(PositionCode.assistant, "대리급");
      PositionCodes.Add(PositionCode.senior, "경력사원급");
      PositionCodes.Add(PositionCode.staff, "신입사원급");
      PositionCodes.Add(PositionCode.freelancer, "프리랜서(비정규 전문직)");
    }*/
  }

  /// <summary>
  /// 외국어 코드
  /// </summary>
  public class LanguageCode
  {
    public const string chinese = "01";
    public const string english = "02";
    public const string french = "03";
    public const string german = "04";
    public const string japanese = "05";
    public const string russian = "06";
    public const string etc = "99";
    public const string spanish = "07";

    public static Dictionary<string, string> LanguageCodes = new Dictionary<string, string>();

    static LanguageCode()
    {
      LanguageCodes.Add(LanguageCode.chinese, "중국어");
      LanguageCodes.Add(LanguageCode.english, "영어");
      LanguageCodes.Add(LanguageCode.french, "불어");
      LanguageCodes.Add(LanguageCode.german, "독일어");
      LanguageCodes.Add(LanguageCode.japanese, "일본어");
      LanguageCodes.Add(LanguageCode.russian, "러시아어");
      LanguageCodes.Add(LanguageCode.etc, "기타");
      LanguageCodes.Add(LanguageCode.spanish, "스페인어");
    }
  }

  public class LanguageLevelCode
  {
    public const int high = 1;
    public const int middle = 2;
    public const int low = 3;

    public static Dictionary<int, string> LanguageLevelCodes = new Dictionary<int, string>();

    static LanguageLevelCode()
    {
      LanguageLevelCodes.Add(LanguageLevelCode.high, "상");
      LanguageLevelCodes.Add(LanguageLevelCode.middle, "중");
      LanguageLevelCodes.Add(LanguageLevelCode.low, "하");
    }
  }

  /// <summary>
  /// 외부접속 로그 타입
  /// </summary>
  public class ExternalLogType
  {
    //항목 오픈.
    public const int elementOpen = 1;
    //수정 페이지 이동.
    public const int moveModify = 2;
  }

  public class InOrderType
  {

    public const int executive = 1;
    public const int reference = 2;
    public const int assement = 3;
    public const int recruitment = 4;
    public const int outside = 6;

    public static Dictionary<int, string> InOrderTypes = new Dictionary<int, string>();

    static InOrderType()
    {
      InOrderTypes.Add(InOrderType.executive, "Executive Search");
      InOrderTypes.Add(InOrderType.reference, "Reference Check");
      InOrderTypes.Add(InOrderType.assement, "Assessment");
      InOrderTypes.Add(InOrderType.recruitment, "Recruitment Process Outsourcing(RPO)");
      InOrderTypes.Add(InOrderType.outside, "Outside directors");
    }
  }

  public class CurrencyType
  {
    public const string won = "KRW";
    public const string usd = "USD";
    public const string hkd = "HKD";
    public const string yen = "YEN";
    public const string gbp = "GBP";
    public const string dem = "DEM";
    public const string frf = "FRF";
    public const string chf = "CHF";
    public const string eur = "EUR";
    public const string cny = "CNY";
    public const string aud = "AUD";
    public const string nzd = "NZD";
    public const string cad = "CAD";
    public const string thb = "THB";
    public const string twd = "TWD";
    public const string vnd = "VND";

    public static Dictionary<string, string> CurrencyTypes = new Dictionary<string, string>();

    static CurrencyType()
    {
      CurrencyTypes.Add(won, "KRW(원)");
      CurrencyTypes.Add(usd, "USD(미국달러)");
      CurrencyTypes.Add(hkd, "HKD(홍콩달러)");
      CurrencyTypes.Add(yen, "YEN(엔화)");
      CurrencyTypes.Add(gbp, "GBP(파운드)");
      CurrencyTypes.Add(dem, "DEM(마르크)");
      CurrencyTypes.Add(frf, "FRF(프랑스 프랑)");
      CurrencyTypes.Add(chf, "CHF(스위스 프랑)");
      CurrencyTypes.Add(eur, "EUR(유로)");
      CurrencyTypes.Add(cny, "CNY(위안)");
      CurrencyTypes.Add(aud, "AUD(호주달러)");
      CurrencyTypes.Add(nzd, "NZD(뉴질랜드달러)");
      CurrencyTypes.Add(cad, "CAD(캐나다달러)");
      CurrencyTypes.Add(thb, "THB(태국 밧)");
      CurrencyTypes.Add(twd, "TWD(신타이완달러)");
      CurrencyTypes.Add(vnd, "VND(베트남 동)");
    }
  }
}
