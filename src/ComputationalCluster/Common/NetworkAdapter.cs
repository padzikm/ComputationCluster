using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;

namespace Common
{
    public class NetworkAdapter
    {
        private readonly string serverName;
        private readonly int connectionPort;
        private TcpClient client;
        private NetworkStream stream;

        private const int MaxBufferLenght = 1024;

        /// <summary>
        /// Property of processing status that is sent to server. Contains actual working threads and ID of a task.
        /// </summary>
        public Status CurrentStatus { private get; set; }

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
        /// <param name="receiveveHandler">Method handling receive action</param>
        /// <param name="sendhandler">Method handling send action</param>
        public void StartKeepAlive(int period, Func<bool> receiveHandler, Action sendhandler)
        {
            var t = new Thread(() =>
            {
                while (true)
                {
                    client = new TcpClient(serverName, connectionPort);
                    stream = client.GetStream();
                    if (!Send(CurrentStatus, false))
                        break;

                    if (receiveHandler())
                        sendhandler();
                    Thread.Sleep(period);
                }
            });
            t.Start();
        }

        public void StartKeepAlive(int period)
        {
            var t = new Thread(() =>
            {
                while (true)
                {
                    if (!Send(CurrentStatus, true))
                        break;
                    Thread.Sleep(period);
                }
            });
            t.Start();
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

                Console.WriteLine("Sent a message: \n{0}\n\n", xml);

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
                client = new TcpClient(serverName, connectionPort) { ReceiveTimeout = 5000 };
                stream = client.GetStream();
            }

            if (!stream.CanRead) throw new Exception("NetworkStream unavaiable");

            var readBuffer = new byte[MaxBufferLenght];
            stream.Read(readBuffer, 0, readBuffer.Length);

            if (closeConnection)
            {
                stream.Close();
                client.Close();
            }

            var readMessage = MessageSerialization.GetString(readBuffer);
            readMessage = readMessage.Replace("\0", string.Empty).Trim();

            if (!MessageValidation.IsMessageValid(MessageTypeConverter.ConvertToMessageType(readMessage), readMessage))
                throw new Exception("Message not valid");

            var deserialized = MessageSerialization.Deserialize<T>(readMessage);
            Console.WriteLine("Received a message: \n{0}", deserialized);

            return deserialized;
        }
    }
}
