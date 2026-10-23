using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Univision.GetHompage
{
  class Program
  {
    static void Main(string[] args)
    {
      //5분 마다 돌아감
      MainAsync(args).GetAwaiter().GetResult();
    }

    static async Task MainAsync(string[] args)
    {
      HomepageUpdate hu = new HomepageUpdate();
      try
      {
        Console.WriteLine("개인정보 동의 가져오기 시작");
        await hu.UpldatePrivacy();
        Console.WriteLine("개인정보 동의 가져오기 종료");
        Console.WriteLine("SMS전송 결과 가져오기 시작");
        await hu.UpldateSMS();
        Console.WriteLine("SMS전송 결과 가져오기 종료");
      }
      catch (Exception e)
      {
        Console.WriteLine(e.Message);
      }

      // 신규 개인정보 동의(n_privacy_agree) 풀링 - 기존 처리 실패와 무관하게 실행
      try
      {
        Console.WriteLine("신규 개인정보 동의 동기화 시작");
        await hu.UpdateNPrivacy();
        Console.WriteLine("신규 개인정보 동의 동기화 종료");
      }
      catch (Exception e)
      {
        Console.WriteLine(e.Message);
      }

      // [테스트] PrivacyCreateTest 플로우 (*_test 테이블) 동기화 - 운영 처리와 독립 실행
      try
      {
        Console.WriteLine("[테스트] 신규 개인정보 동의 동기화 시작");
        await hu.UpdateNPrivacy(true);
        Console.WriteLine("[테스트] 신규 개인정보 동의 동기화 종료");
      }
      catch (Exception e)
      {
        Console.WriteLine("[테스트] " + e.Message);
      }
      
    }


    
  }
}
