using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
namespace ConsoleApp2
{
    class Program
    {
        static void Main(string[] args)
        {

            KcpClient kc =  new KcpClient("192.168.0.16",8000);
#if UTF16
            UnicodeEncoding asen = new UnicodeEncoding();
#else
            ASCIIEncoding asen = new ASCIIEncoding();
#endif
            byte[] buffer = asen.GetBytes("hi kcp");
            kc.userlevelsend(ref buffer);
            Console.WriteLine("start");
            while (true)
            {
                Thread.Sleep(100);
            }
        }
    }
}
