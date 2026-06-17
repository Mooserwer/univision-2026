using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Univision.Exchange
{
  class Program
  {
    static async Task Main(string[] args)
    {
      DateTime my_date = DateTime.UtcNow.Date;
      if (args.Length > 0)
      {
        DateTime.TryParse(args[0], out my_date);

      }

      Console.WriteLine("{0} - 환율 수집 중...", my_date.ToString("yyyy-MM-dd"));
      CallApi capi = new CallApi();
      try
      {
        var list_xml = await capi.ExchangeCustomsApiXML(my_date.ToString("yyyyMMdd"));
        Console.WriteLine("{0} 건 발견", list_xml.body.items.item.Count());
        if (list_xml.body.items.item.Count() > 0)
        {
          Console.WriteLine("환율 {0} 건 확인", list_xml.body.items.item.Count());
          List<exchange> exch_list = new List<exchange>();
          foreach (var exg in list_xml.body.items.item)
          {
            Console.WriteLine("{0} 환율 : {1} 작업처리..", exg.currSgn, exg.fxrt);
            try
            {

              string ex_code = exg.currSgn;
              double ex_rate = 1;

              exchange exch = new exchange();
              //}
              exch.ex_date = my_date.ToString("yyyy-MM-dd");
              exch.ex_code = ex_code;
              exch.ex_rate = ex_rate;
              exch.per_won = exg.fxrt;

              exch.read_date = DateTime.Now;

              exch_list.Add(exch);
            }
            catch (Exception e)
            {
              Console.WriteLine("***ERR : {0} 환율 작업 중 오류 발생", exg.currSgn);
              Console.WriteLine(e.Message);
            }

          }

          if (exch_list.Count() > 0)
          {
            try
            {
              Console.WriteLine("환율저장 중...");
              dbConnection dbc = new dbConnection();
              await dbc.SaveData(my_date.ToString("yyyy-MM-dd"), exch_list);
              Console.WriteLine("환율저장 완료");
            }
            catch (Exception e)
            {
              Console.WriteLine("환율저장 중 오류 발생");
            }

          }

        }
        else
        {
          Console.WriteLine("환율 가져올 환율이 없습니다.");
        }

        /*
        var list = await capi.ExchangeApi(my_date.ToString("yyyyMMdd"));

        //ExchangeRepository exer = new ExchangeRepository();
        Console.WriteLine("{0} 건 발견", list.Count());
        if (list.Count() > 0)
        {
          Console.WriteLine("환율 {0} 건 확인", list.Count());
          List<exchange> exch_list = new List<exchange>();
          foreach (var exg in list)
          {
            try
            {
              string ex_code = exg.cur_unit;
              double ex_rate = 1;

              Regex regex = new Regex(@"([A-Z]{3})");
              Match match = regex.Match(exg.cur_unit);
              if (match.Success)
              {
                ex_code = match.Value;
              }

              regex = new Regex(@"([0-9]{3})");
              match = regex.Match(exg.cur_unit);
              if (match.Success)
              {
                ex_rate = double.Parse(match.Value);
              }

              //var exch = await exer.SelectExchangeOneAsync(my_date.ToString("yyyy-MM-dd"), ex_code);
              //if (exch == null)
              // {
              exchange exch = new exchange();
              //}
              exch.ex_date = my_date.ToString("yyyy-MM-dd");
              exch.ex_code = ex_code;
              exch.ex_rate = ex_rate;
              double per_won = 0;
              if (double.TryParse(exg.kftc_deal_bas_r.Replace(",", ""), out per_won))
              {
                exch.per_won = per_won;
              }
              else
              {
                exch.per_won = 0;
              }
              exch.read_date = DateTime.Now;

              exch_list.Add(exch);

              Console.WriteLine("{0} 환율 : {1} 작업처리..", ex_code, exg.kftc_deal_bas_r);

            }
            catch (Exception e)
            {
              Console.WriteLine("***ERR : {0} 환율 작업 중 오류 발생", exg.cur_unit);
              Console.WriteLine(e.Message);
            }

          }

          if (exch_list.Count() > 0)
          {
            try
            {
              Console.WriteLine("환율저장 중...");
              dbConnection dbc = new dbConnection();
              await dbc.SaveData(my_date.ToString("yyyy-MM-dd"), exch_list);
              Console.WriteLine("환율저장 완료");
            }
            catch (Exception e)
            {
              Console.WriteLine("환율저장 중 오류 발생");
            }

          }
        
        }
        else
        {
          Console.WriteLine("환율 가져올 환율이 없습니다.");
        }
        */
        Console.WriteLine("환율작업 종료");
      }
      catch (Exception e)
      {
        Console.WriteLine(e.Message);
      }
    }

  }
}
