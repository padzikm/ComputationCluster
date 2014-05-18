using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Common
{
    public class TMNetworkAdapter : NetworkAdapter
    {

        public TMNetworkAdapter(IPAddress serverIpAddress, int _connectionPort) : base(serverIpAddress, _connectionPort)
        {
        }

        public TMNetworkAdapter(string _serverName, int _connectionPort) : base(_serverName, _connectionPort)
        {
        }

        private Semaphore semaphore = new Semaphore(1, 1);

        /// <summary>
        ///  In newly created thread CurrentStatus is sent due to inform server that component that uses it is alive.
        /// </summary>
        /// <param name="period"> Keepalive timeout </param>
        /// <param name="sendDivide">method handling divide action</param>
        /// <param name="sendMerge">method handling merge action</param>
        public void StartKeepAliveTask(int period, Action<ulong> sendDivide, Action<ulong> sendMerge, Action<DivideProblem> handleDivide, Action<Solutions> handleSolutions)
        {
            
            var t = new Thread(() =>
            {
                while (true)
                {
                    semaphore.WaitOne();
                    client = new TcpClient(serverName, connectionPort);
                    stream = client.GetStream();
                    if (!Send(CurrentStatus, false))
                        break;

                    semaphore.Release();
                    Thread.Sleep(period);
                }

            });
            var checkThread = new Thread(() =>
            {
                try
                {
                    Check(sendDivide, sendMerge, handleDivide, handleSolutions);

                }
                finally
                {

                }
            });
            t.Start();
            checkThread.Start();
        }

        private void Check(Action<ulong> sendDivide, Action<ulong> sendMerge, Action<DivideProblem> handleDivide, Action<Solutions> handleSolutions)
        {
            while (true)
            {
                semaphore.WaitOne();
                var readMessage = ReadMessage();

#if DEBUG
                Console.WriteLine("Odebrano: \n{0}", readMessage);
#endif
                if (MessageValidation.IsMessageValid(MessageTypeConverter.ConvertToMessageType(readMessage), readMessage))
                {
                    var deserialized = MessageSerialization.Deserialize<DivideProblem>(readMessage);

                    if (deserialized != null)
                    {
                        handleDivide(deserialized);
                        sendDivide(deserialized.Id);
                    }
                    else
                    {
                        var deserialized2 = MessageSerialization.Deserialize<Solutions>(readMessage);
                        handleSolutions(deserialized2);
                        if (deserialized2 != null)
                            sendMerge(deserialized2.Id);
                    }
                }
                semaphore.Release();
            }


        }
    }
}
