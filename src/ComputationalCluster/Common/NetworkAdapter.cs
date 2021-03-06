﻿using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace Common
{
    public class NetworkAdapter
    {
        protected readonly string serverName;
        protected readonly int connectionPort;
        protected TcpClient client;
        protected NetworkStream stream;

        protected const int MaxBufferLenght = 1024 * 1000;

        /// <summary>
        /// Property of processing status that is sent to server. Contains actual working threads and ID of a task.
        /// </summary>
        public Status CurrentStatus { get; set; }

        /// <summary>
        /// Constructor that takes IPAddress class as first parameter. Stores input data in private variables.
        /// </summary>
        /// <param name="serverIpAddress">Specifies IPAddress value of IP which client use to connect to serverName. May be localhost.</param>
        /// <param name="_connectionPort">Port that server is listening to.</param>
        public NetworkAdapter(IPAddress serverIpAddress, int _connectionPort)
        {
            if (serverIpAddress.ToString() == null || _connectionPort < 0)
                throw new ArgumentNullException();

            serverName = serverIpAddress.ToString();
            connectionPort = _connectionPort;
        }

        /// <summary>
        /// Constructor that takes string variable as first parameter. Stores input data in private variables.
        /// </summary>
        /// <param name="_serverName"> Specifies string value of IP which client use to connect to serverName. May be localhost. </param>
        /// <param name="_connectionPort"> Port that server is listening to. </param>
        public NetworkAdapter(string _serverName, int _connectionPort)
        {
            if (_serverName == null || _connectionPort < 0)
                throw new ArgumentNullException();

            serverName = _serverName;
            connectionPort = _connectionPort;
        }

        /// <summary>
        /// Creates TcpClient and gets a stream from it.
        /// </summary>
        public void StartConnection()
        {
            client = new TcpClient(serverName, connectionPort);

            stream = client.GetStream();
        }

        /// <summary>
        /// Closes previously opened stream from TcpClient ale closes this client too.
        /// </summary>
        public void CloseConnection()
        {
            if (stream != null)
                stream.Close();

            if (client != null)
                client.Close();
        }

        /// <summary>
        /// In newly created thread CurrentStatus is sent due to inform server that component that uses it is alive.
        /// </summary>
        /// <param name="period"> Keepalive timeout </param>
        /// <param name="receiveHandler"> Method handling receive action</param>
        /// <param name="sendhandler">Method handling send action</param>
        public void StartKeepAlive(int period, Func<bool> receiveHandler, Action sendhandler)
        {
            Semaphore semaphore = new Semaphore(1, 1);
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
                    while (true)
                    {
                        semaphore.WaitOne();
                        if (receiveHandler())
                        {
                            sendhandler();
                            semaphore.Release();
                            Thread.CurrentThread.Abort();

                        }
                        semaphore.Release();
                    }

                }
                finally
                {

                }

            });
            t.Start();
            checkThread.Start();
        }

        /// <summary>
        /// Generic method sends serialized and encoded message to network stream.
        /// </summary>
        /// <typeparam name="T"> Type of message class. </typeparam>
        /// <param name="message"> Message to sentm as a instance of T class. </param>
        /// <param name="closeConnection">Indicates whether connections should be recreated and closed</param>
        /// <returns> True if sending was complete. False otherwise. </returns>
        public bool Send<T>(T message, bool closeConnection) where T : class
        {
            try
            {
                if (closeConnection)
                {
                    client = new TcpClient(serverName, connectionPort);
                    stream = client.GetStream();
                }

                var xml = MessageSerialization.Serialize(message);
                var data = MessageSerialization.GetBytes(xml);
                stream.Write(data, 0, data.Length);

                if (closeConnection)
                {
                    stream.Close();
                    client.Close();
                }
#if DEBUG
                Console.WriteLine("Sent a message: \n{0}\n\n", xml);
#endif

                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        /// <summary>
        /// Uses newtwork stream to wait (read) for a message for a specific lenght (1024 bytes).  Obtained string is validating
        /// and converting to to type T.
        /// </summary>
        /// <typeparam name="T"> Type of message class that method returns. </typeparam>
        /// <param name="closeConnection">Indicates whether connections should be recreated and closed</param>
        /// <returns> Deserialized message of type T if stream can be read and if serialization is ok. Null otherwise.</returns>
        public T Receive<T>(bool closeConnection) where T : class
        {


            if (closeConnection)
            {
                client = new TcpClient(serverName, connectionPort);
                stream = client.GetStream();
            }

            if (!stream.CanRead) throw new Exception("NetworkStream unavaiable");

            var readMessage = ReadMessage();

            if (closeConnection)
            {
                stream.Close();
                client.Close();
            }

#if DEBUG
            Console.WriteLine("Odebrano: \n{0}", readMessage);
#endif
            if (readMessage != "")
            {
                if (!MessageValidation.IsMessageValid(MessageTypeConverter.ConvertToMessageType(readMessage), readMessage))
                    throw new Exception("Message not valid");

                var deserialized = MessageSerialization.Deserialize<T>(readMessage);
#if DEBUG
                Console.WriteLine("Received a message: \n{0}", deserialized);
#endif

                return deserialized;
            }
            else
                return null;
        }

        protected string ReadMessage()
        {
            StringBuilder sb = new StringBuilder();
            string readMessage = string.Empty;

            if (readMessage == null) throw new ArgumentNullException("readMessage");
            bool stopRead = false;
            while (!stopRead)
            {
                try
                {
                    byte[] buffer = new byte[1024];
                    stream.Read(buffer, 0, buffer.Length);
                    readMessage = MessageSerialization.GetString(buffer).Replace("\0", string.Empty).Trim();
                    sb.Append(readMessage);
                    if (readMessage == "")
                        stopRead = true;
                }
                catch (Exception e)
                {
                    stopRead = true;
                }
            }
            readMessage = sb.ToString();
            return readMessage;
        }
    }
}