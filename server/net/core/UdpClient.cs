#define UTF16
using System;
using System.Collections.Generic;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.Threading;
namespace ChatServer
{
    public delegate void OnSuperudpReceivedCompleted(ref byte[] buffer);

    class UdpClient
    {
        public EndPoint mremoteEP;
        public UdpServer mudpserver;
        const int BUFFER_SIZE = 65507;
        public byte[] receivebuffer = new byte[BUFFER_SIZE];
        public OnReceivedCompleted OnReceivedCompletePointer = null;
        public UdpClient(UdpServer udpserver, EndPoint remoteendpoint)
        {
            mremoteEP = remoteendpoint;
            mudpserver = udpserver;
        }
        ~UdpClient()
        {
            Console.WriteLine("UdpClient In destructor.");
        }
        public void Send(byte[] buffer)
        {
            if (mudpserver != null)
            {
                mudpserver.UdpListener.SendTo(buffer, mremoteEP);
            }
        }
        public void receivecallback(ref byte[] buffer)
        {
            OnReceivedCompletePointer.Invoke(ref buffer);
        }
    }
}
