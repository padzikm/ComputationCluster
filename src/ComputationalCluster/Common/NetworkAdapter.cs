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

    public class NetworkAdapter
    {
        private TcpClient client;
        private NetworkStream stream;

        private const int MaxBufferLenght = 1024;

        /// <summary>
        /// Property of processing status that is sent to server. Contains actual working threads and ID of a task.
        /// </summary>
        public Status CurrentStatus { get; set; }

        /// <summary>
        /// Creates ne Tcp client and open a network stream form it.
        /// </summary>
        /// <param name="serverIpAddress">Specifies IPAddress value of IP which client use to connect to serverName. May be localhost.</param>
        /// <param name="connectionPort">Port that server is listening to.</param>
        public void StartConnection(IPAddress serverIpAddress, int connectionPort)
        {
            client = new TcpClient(serverIpAddress.ToString(), connectionPort);
            stream = client.GetStream();
        }

        /// <summary>
        /// Creates ne Tcp client and open a network stream form it.
        /// </summary>
        /// <param name="serverName"> Specifies string value of IP which client use to connect to serverName. May be localhost. </param>
        /// <param name="port"> Port that server is listening to. </param>
        public void StartConnection(string serverName, int port)
        {
            client = new TcpClient(serverName, port);
            stream = client.GetStream();
        }

        /// <summary>
        /// Closes the previously opened network stream and client.
        /// </summary>
        public void CloseConnection()
        {
            stream.Close();
            client.Close();
        }

        /// <summary>
        /// In newly created thread CurrentStatus are sent due to inform server that component that uses it is alive.
        /// </summary>
        /// <param name="period"> Time at which a message is sent/ </param>
        public void StartKeepAlive(int period)
        {
            var t = new Thread(() =>
            {
                while (true)
                {
                    if (!Send(CurrentStatus))
                        throw new Exception("StartKeepAlive");

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
        /// <returns> True if sending was complete. False otherwise. </returns>
        public bool Send<T>(T message) where T : class
        {
            var xml = MessageSerialization.Serialize(message);
            var data = MessageSerialization.GetBytes(xml);
            stream.Write(data, 0, data.Length);

            Console.WriteLine("Sent {0}", xml);

            return true;
        }


        /// <summary>
        /// Uses newtwork stream to wait (read) for a message for a specific lenght (1024 bytes).  Obtained string is validating
        /// and converting to to type T.
        /// </summary>
        /// <typeparam name="T"> Type of message class that method returns. </typeparam>
        /// <returns> Deserialized message of type T if stream can be read and if serialization is ok. Null otherwise.</returns>
        public T Recieve<T>() where T : class
        {
            if (!stream.CanRead) throw new Exception("Sorry.  You cannot read from this NetworkStream.");
            var readBuffer = new byte[MaxBufferLenght];
            stream.Read(readBuffer, 0, readBuffer.Length);
            var readMessage = MessageSerialization.GetString(readBuffer);
            readMessage = readMessage.Replace("\0", string.Empty).Trim();

            if (MessageValidation.IsMessageValid(MessageTypeConverter.ConvertToMessageType(readMessage), readMessage))
            {
                var deserialized = MessageSerialization.Deserialize<T>(readMessage);
                Console.WriteLine(deserialized);
                return deserialized;
            }
            throw new Exception("Message not valid");
        }
    }
}
