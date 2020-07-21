#define LOOPBACKTEST
using System;
using System.Collections.Generic;
using System.Net;
using System.Text;
using System.Net.Sockets.Kcp;
using System.Buffers;
using System.Threading.Tasks;

namespace ChatServer
{
    public delegate void OnUserLevelReceivedCompleted(ref byte[] buffer);
    class KcpClient : UdpClient
    {
        Kcp mKcp;
        public OnUserLevelReceivedCompleted onUserLevelReceivedCompleted;
        public KcpClient(UdpServer udpserver, EndPoint remoteendpoint) : base(udpserver, remoteendpoint)
        {
#if LOOPBACKTEST
            onUserLevelReceivedCompleted += (ref byte[] buffer) => {
#if UTF16
            var str = System.Text.Encoding.Unicode.GetString(buffer);
#else
                var str = System.Text.Encoding.UTF8.GetString(buffer);
#endif
                Console.ForegroundColor = ConsoleColor.Blue;
                Console.WriteLine(str);
                Console.ForegroundColor = ConsoleColor.White;
                userlevelsend(ref buffer);
            };
#endif
            kcphandle handle = new kcphandle();
            OnReceivedCompletePointer = (ref byte[] buffer) => { 
                mKcp.Input(buffer);//buffer is received from low level data
            };
            handle.Out = buffer => {
                mudpserver.UdpListener.SendTo(buffer.ToArray(), mremoteEP);//low level send
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
                        await Task.Delay(1);
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
        }
        public void userlevelsend(ref byte[] buffer)
        {
            var res = mKcp.Send(buffer);//send by handle.out
            if (res != 0)
            {
                Console.WriteLine($"kcp send error");
            }
        }
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
