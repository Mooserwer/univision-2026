using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Univision.Core.Repositories;

namespace UnivisionCon.IndexRebuild
{
    class Program
    {
        static void Main(string[] args)
        {
            try
            {
                //한달에 한번 새벽에 스케쥴러를 통해 실행시킨다.
                //릴리즈 모드로 빌드 후 생성되는 UnivisionCon.IndexRebuild.Exe를 윈도우 스케쥴러를 통해 등록
                Console.Write("Index Rebuild Start");
                ConsoleRepository cr = new ConsoleRepository();
                cr.IndexRebuild("Univision"); //Database Name
                Console.Write("               End");
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }
    }
}
