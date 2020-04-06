using System;
using System.Collections.Generic;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.Threading;

namespace ChatServer
{
    public delegate void OnReceivedCompleted(ref byte[] buffer);
    class UdpServer
    {
        public Socket UdpListener;
        Dictionary<EndPoint, KcpClient> UdpClientMap ;
        public KcpClient findclient(EndPoint ep)
        {
            bool b = UdpClientMap.ContainsKey(ep);
            if (b)
            {
            }
            else
            {
                UdpClientMap.Add(ep, new KcpClient(this,ep));
                Console.WriteLine("Add UdpServer UdpClientMap size: " + UdpClientMap.Count+"  endpoint:"+ ep.ToString());
            }
            KcpClient outudp;
            UdpClientMap.TryGetValue(ep, out outudp);
            return outudp;
        }
        public void removeclient(EndPoint ep)
        {
            UdpClientMap.Remove(ep);
            Console.WriteLine("remove UdpServer UdpClientMap size: " + UdpClientMap.Count + "  endpoint:" + ep.ToString());
        }
        public UdpServer(IPAddress ipAd, int port)
        {
            IPEndPoint m_LocalEndPoint = new IPEndPoint(ipAd, port);//27001
            UdpListener = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
            UdpListener.Bind(m_LocalEndPoint);
            UdpClientMap = new Dictionary<EndPoint, KcpClient>();
        }
        ~ UdpServer()
        {

        }
        public void startlisten()
        {
            Thread TransferListenerthread = new Thread(new ThreadStart(Listenerthreadwork));
            TransferListenerthread.IsBackground = true;
            TransferListenerthread.Start();
        }
        void Listenerthreadwork()
        {
            byte[] receivedbuffer = new Byte[65507];
            EndPoint remoteEP = new IPEndPoint(1, 1);
            while (true)
            {
                try
                {
                    // int id = Thread.CurrentThread.ManagedThreadId;
                    int bytesReceived = UdpListener.ReceiveFrom(receivedbuffer, ref remoteEP);//here is a block call ,wait until it receive data
                        byte[] validbuffer = new byte[bytesReceived];
                        Array.ConstrainedCopy(receivedbuffer, 0, validbuffer, 0, bytesReceived);
                        findclient(remoteEP).receivecallback(ref validbuffer);
                }
                catch (Exception e)
                {
                    //Console.BackgroundColor = ConsoleColor.Red;
                    //Console.WriteLine(e);
                    //Console.ResetColor();
                }
  

                Thread.Sleep(1);

            }
        }
    }

}
