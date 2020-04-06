using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Collections.Generic;

//using ChannelidType = System.Byte;//System.Int16
using ChannelidType = System.Int32;//System.uint16

namespace ChatServer
{
   
    class Program
    {

        public static Config config = new Config();
        static void Main(string[] args)
        {
            IPAddress ipAd = IPAddress.Parse(config.configinfor.ipaddress);//local ip address  "172.16.5.188"
            new UdpServer(ipAd, config.configinfor.ttcipport).startlisten();
            Console.WriteLine("listening "+ ipAd.ToString() +":"+ config.configinfor.ttcipport.ToString());
            while (true)
            {
                Thread.Sleep(100);
            }

        }   

    }

}
