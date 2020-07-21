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

            KcpClient kc =  new KcpClient("192.168.20.96",8000);
#if UTF16
            UnicodeEncoding asen = new UnicodeEncoding();
#else
            ASCIIEncoding asen = new ASCIIEncoding();
#endif
         //   byte[] buffer = asen.GetBytes("hi kcp");
           // kc.userlevelsend(ref buffer);
            Console.WriteLine("start");
            while (true)
            {
                String str = "";
                for (int i = 0; i < 200; i++)
                {
                    int j = i % 10;
                    str += j.ToString();
                }
                byte[] buffer = asen.GetBytes(str);
                //Thread.Sleep(100);
                Console.ReadKey();
                kc.userlevelsend(ref buffer);

            }
        }
    }
}
