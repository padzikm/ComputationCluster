using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Xml.Linq;
using Common;
using System.Net;
using System.Net.Sockets;
using System.IO;

namespace CommunicationServer
{
    public class Server
    {
        private IPAddress ipAddress;
        private int port;
        private TcpListener listener;
        private bool stop;
        private Thread currentThread;                
        private TimeSpan timeout;
        private MessageStrategyFactory strategyFactory;
        private InactiveComponentCollector inactiveComponentCollector;

        public Server(IPAddress ipAddress, int port, TimeSpan timeout)
        {
            this.ipAddress = ipAddress;
            this.port = port;
            this.timeout = timeout;
            stop = false;
            strategyFactory = MessageStrategyFactory.Instance;
            inactiveComponentCollector = new InactiveComponentCollector(timeout, TimeSpan.FromTicks(timeout.Ticks / 2), TimeSpan.FromTicks(timeout.Ticks / 2));
        }

        public void Start()
        {
            if (currentThread != null)
                throw new InvalidOperationException("Server is already running!");

            try
            {
                currentThread = new Thread(Listen);                
                listener = new TcpListener(ipAddress, port);
                listener.Start();
                currentThread.Start();
                inactiveComponentCollector.Start();
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error in server start: {0}", ex.Message);
            }
        }

        public void Stop()
        {
            try
            {
                listener.Stop(); //TODO: do better
                stop = true;
                currentThread.Join();
                currentThread = null;
                inactiveComponentCollector.Stop();
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error in server stop: {0}", ex.Message);
            }
        }

        private void Listen()
        {
            while (!stop)
            {
                try
                {
                    Socket socket = listener.AcceptSocket();
                    Thread thread = new Thread(new ParameterizedThreadStart(HandleConnection));
                    thread.Start(socket);
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Error in server listen: {0}", ex.Message);
                }
            }
        }

        private void HandleConnection(object o)
        {
            Socket soc = (Socket)o;
            Stream stream = new NetworkStream(soc);                        

            try
            {
                ServerNetworkAdapter networkAdapter = new ServerNetworkAdapter(stream);
                byte[] buffer = new byte[1024 * 1000];
                string msg = string.Empty;
                stream.Read(buffer, 0, buffer.Length);                
                msg = MessageSerialization.GetString(buffer).Replace("\0", string.Empty).Trim();
                Console.WriteLine("Odebrano: \n{0}", msg);
                MessageType msgType = MessageTypeConverter.ConvertToMessageType(msg);
                IMessageStrategy strategy = strategyFactory.GetMessageStrategy(msgType);
                if (strategy != null)
                    strategy.HandleMessage(networkAdapter, msg, msgType, timeout);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            finally
            {

                stream.Close();
                soc.Close();
            }
        }
    }
}
