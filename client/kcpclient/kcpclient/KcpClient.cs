#define LOOPBACKTEST
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Sockets;
using System.Net.Sockets.Kcp;
using System.Buffers;

namespace ConsoleApp2
{
    public delegate void OnUserLevelReceivedCompleted(ref byte[] buffer);
    class KcpClient
    {
        private static Socket m_Socket;            //UDP Socket
        Kcp mKcp;
        public OnUserLevelReceivedCompleted onUserLevelReceivedCompleted;
        IPEndPoint remoteEndPoint;
        public KcpClient(string ip,int port)
        {
            IPAddress ipAd = IPAddress.Parse(ip);//local ip address  "172.16.5.188"
            remoteEndPoint = new IPEndPoint(ipAd, port);//27001
            m_Socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
#if LOOPBACKTEST
            onUserLevelReceivedCompleted += (ref byte[] buffer) => {
#if UTF16
            var str = System.Text.Encoding.Unicode.GetString(buffer);
#else
                var str = System.Text.Encoding.UTF8.GetString(buffer);
#endif
                Console.WriteLine(str);
                //userlevelsend(ref buffer);
            };
#endif
            kcphandle handle = new kcphandle();
            handle.Out = buffer => {
                m_Socket.SendTo(buffer.ToArray(), remoteEndPoint);//low level send
            };
            mKcp = new Kcp((uint)(5598781), handle);
            mKcp.NoDelay(1, 5, 2, 1);//fast
            mKcp.WndSize(64, 64);
            mKcp.SetMtu(512);
            Task.Run(async () =>
            {
                try
                {
                    int updateCount = 0;
                    while (true)
                    {
                        mKcp.Update(DateTime.UtcNow);

                        int len;
                        while ((len = mKcp.PeekSize()) > 0)
                        {
                            var buffer = new byte[len];
                            if (mKcp.Recv(buffer) >= 0)
                            {
                                onUserLevelReceivedCompleted.Invoke(ref buffer);
                            }
                        }
                        await Task.Delay(5);
                        updateCount++;
                        if (updateCount % 1000 == 0)
                        {
                            Console.WriteLine($"KCP ALIVE {updateCount}----");
                        }
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                }

            });
            Task.Run(
                async () =>
                { 
                        EndPoint remoteEP = new IPEndPoint(1, 1);
                        byte[] receivedbuffer = new Byte[65507];
                        await Task.Delay(1);
                    while (true)
                    {
                        try
                        {
                            int bytesReceived = m_Socket.ReceiveFrom(receivedbuffer, ref remoteEP);//here is a block call ,wait until it receive data
                            byte[] validbuffer = new byte[bytesReceived];
                            Array.ConstrainedCopy(receivedbuffer, 0, validbuffer, 0, bytesReceived);
                            mKcp.Input(validbuffer);//buffer is received from low level data
                            //await Task.Delay(1);
                        }
                        catch
                        { 
                        
                        }
                    }

                }
                );
        }
        public void userlevelsend(ref byte[] buffer)
        {
            var res = mKcp.Send(buffer);//send by handle.out
            if (res != 0)
            {
                Console.WriteLine($"kcp send error");
            }
        }
        //        public void rawsend()
        //        {
        //#if UTF16
        //            UnicodeEncoding asen = new UnicodeEncoding();
        //#else
        //            ASCIIEncoding asen = new ASCIIEncoding();
        //#endif
        //            byte[] buffer = asen.GetBytes("hi kcp");
        //            m_Socket.SendTo(buffer.ToArray(), remoteEndPoint);//low level send
        //        }
    }
    internal class kcphandle : IKcpCallback
    {
        public Action<Memory<byte>> Out;
        public void Output(IMemoryOwner<byte> buffer, int avalidLength)
        {
            Out(buffer.Memory.Slice(0, avalidLength));
        }
    }
}
