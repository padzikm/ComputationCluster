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
        private TimeSpan timeout;
        private DateTime componentTimeout;
        private MessageStrategyFactory strategyFactory;        

        public Server(IPAddress ipAddress, int port, TimeSpan timeout)
        {
            this.ipAddress = ipAddress;
            this.port = port;
            this.timeout = timeout;
            stop = false;
            strategyFactory = MessageStrategyFactory.Instance;            
            componentTimeout = new DateTime(1, 1, 1, timeout.Hours, timeout.Minutes, timeout.Seconds);
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

        private void HandleConnection(object o) //TODO: handle multiple messages at once from read, change serializer encoding to utf-8, handle closing connections
        {
            Socket soc = (Socket)o;
            Stream stream = new NetworkStream(soc);            
            string msg = string.Empty;
            bool keepAlive = true;
            AutoResetEvent waitEvent;
            ulong id = 0;            
            IMessageStrategy strategy;
            TimeSpan timeLeft, timePassed;
            DateTime dateTime;            

            while (keepAlive)
            {
                try
                {
                    byte[] buffer = new byte[1024];
                    stream.Read(buffer, 0, buffer.Length);
                    msg = MessageSerialization.GetString(buffer).Replace("\0", string.Empty).Trim();
                    Console.WriteLine("Odebrano: \n{0}", msg);
                }
                catch (Exception ex)
                {
                    keepAlive = false;
                    msg = string.Empty;
                    Console.WriteLine(ex.Message);
                }
                finally
                {
                    if (keepAlive)
                    {
                        MessageType msgType = MessageTypeConverter.ConvertToMessageType(msg);
                        strategy = strategyFactory.GetMessageStrategy(msgType, componentTimeout, id);
                        strategy.HandleMessage(stream, msg, msgType, componentTimeout, ref id, out keepAlive, out waitEvent);

                        if (waitEvent != null)
                        {
                            dateTime = DateTime.UtcNow;
                            timeLeft = timeout;
                            while (timeLeft > TimeSpan.Zero)
                            {
                                if (waitEvent.WaitOne(timeLeft))
                                {
                                    strategy = strategyFactory.GetWaitEventStrategy(id);
                                    strategy.HandleWaitEvent(stream, id);

                                    timePassed = DateTime.UtcNow - dateTime;
                                    timeLeft -= timePassed;
                                    dateTime = DateTime.UtcNow;
                                }
                                else
                                    timeLeft = TimeSpan.Zero;
                            }
                        }
                    }
                }
            }

            stream.Close();
            soc.Close();
        }
    }
}
