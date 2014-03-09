
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace Common
{
    public class Client
    {
        private TcpClient client;
        private int port;
        private NetworkStream stream;


        public virtual void StartConnection(String server)
        {
            client = new TcpClient(server, port);
        }
        public virtual void CloseConnection()
        {
            client.Close();
        }
        //TODO async or not
        public virtual void Send<T>(T message) where T : class
        {
            try
            {
                stream = client.GetStream();
                string xml = MessageSerialization.Serialize<T>(message);
                Byte[] data = System.Text.Encoding.UTF8.GetBytes(xml);
                stream.Write(data, 0, data.Length);
                stream.Close();
            }
            catch (ArgumentNullException e)
            {
                Console.WriteLine("ArgumentNullException: {0}", e);
            }
            catch (SocketException e)
            {
                Console.WriteLine("SocketException: {0}", e);
            }
        }
        public virtual T Recieve<T>(T message) where T : class
        {
            if (stream.CanRead)
            {
                byte[] readBuffer = new byte[1024];
                stream.BeginRead(readBuffer, 0, readBuffer.Length, null, stream);
                return MessageSerialization.Deserialize<T>(readBuffer.ToString());
            }
            else
            {
                Console.WriteLine("Sorry.  You cannot read from this NetworkStream.");
                return null;
            }
        }



    }
}
