using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NetFwTypeLib;
namespace Univision.AllowIP
{
    class Program
    {
        
        static void Main(string[] args)
        {
            Console.WriteLine("등록");
            FirewallAPI.AddInboudRule("test2", FirewallAPI.Protocol.Tcp,"192.168.0.1", 80);
            Console.WriteLine("등록 완료");
        }
    }
}
