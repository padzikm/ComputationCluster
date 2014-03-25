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

        /// <summary>
        /// Property of processing status that is sent to server. Contains actual working threads and ID of a task.
        /// </summary>
        public Status CurrentStatus { get; set; }
        
        /// <summary>
        /// Creates ne Tcp client and open a network stream form it.
        /// </summary>
        /// <param name="server"> Specifies string value of IP which client use to connect to server. May be localhost. </param>
        /// <param name="port"> Port that server is listening to. </param>
        public void StartConnection(string server, int port)
        {
            client = new TcpClient(server, port);
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
            try
            {
                var xml = MessageSerialization.Serialize(message);
                var data = Encoding.UTF8.GetBytes(xml);
                stream.Write(data, 0, data.Length);
            }
            catch (ArgumentNullException e)
            {
                Console.WriteLine("ArgumentNullException: {0}", e);

            }
            catch (SocketException e)
            {
                Console.WriteLine("SocketException: {0}", e);
            }

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
            if (stream.CanRead)
            {
                var readBuffer = new byte[1024];
                stream.Read(readBuffer, 0, readBuffer.Length);
                var readMessage = readBuffer.ToString();
                if (MessageValidation.IsMessageValid(MessageTypeConverter.ConvertToMessageType(readMessage), readMessage))
                {
                    return MessageSerialization.Deserialize<T>(readBuffer.ToString());
                }
                throw new Exception("Message not valid");
            }
            Console.WriteLine("Sorry.  You cannot read from this NetworkStream.");
            return null;
        }
    }
}
