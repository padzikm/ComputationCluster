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
    class Server
    {
        private IPAddress ipAddress;
        private int port;
        private TcpListener listener;
        private bool stop;
        private Thread currentThread;

        public Server(IPAddress ipAddress, int port)
        {
            this.ipAddress = ipAddress;
            this.port = port;
            stop = false;
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
                    Socket socet = listener.AcceptSocket();
                    Thread thread = new Thread(new ParameterizedThreadStart(HandleConnection));
                    thread.Start(socet);
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

            byte[] buffer = new byte[1024];

            stream.Read(buffer, 0, buffer.Length);            

            string msg = MessageSerialization.GetString(buffer);

            Console.WriteLine("Odebrano: \n{0}", msg);

            //MessageStrategyFactory strategyFactory = MessageStrategyFactory.Instance;
            //IMessageStrategy strategy = strategyFactory.GetMessageStrategy(msg);
            //strategy.HandleMessage(stream, msg);

            stream.Close();
            soc.Close();
        }
    }
}
