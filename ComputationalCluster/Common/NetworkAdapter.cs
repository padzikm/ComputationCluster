
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Common
{
    
    public class NetworkAdapter
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

        //TODO implement keepalive loop - what about this?
        public virtual void StatusMessage(StatusThread[] threads, ulong id, DateTime time)
        {
            Thread t = new Thread(() =>
            {
                while(true)
                {  
                    Status statusMessage = new Status();
                    statusMessage.Id = id;
                    statusMessage.Threads = threads;

                    if(!Send<Status>(statusMessage))
                    {
                        //t.join();
                        return;
                    }
                    
                    Thread.Sleep(time.Millisecond);
                }
            });
            t.Start();
            while (t.ThreadState == ThreadState.Stopped)  // ?
                t.Join();
        }

        //TODO async or not
        public virtual bool Send<T>(T message) where T : class
        {
            try
            {
                stream = client.GetStream();
                string xml = MessageSerialization.Serialize<T>(message);
                Byte[] data = System.Text.Encoding.UTF8.GetBytes(xml);
                stream.Write(data, 0, data.Length);
                stream.Close();
                return true;
            }
            catch (ArgumentNullException e)
            {
                Console.WriteLine("ArgumentNullException: {0}", e);
                return true;
            }
            catch (SocketException e)
            {
                Console.WriteLine("SocketException: {0}", e);
                return false;
            }
        }
        public virtual T Recieve<T>() where T : class
        {
            if (stream.CanRead)
            {
                byte[] readBuffer = new byte[1024];
                stream.BeginRead(readBuffer, 0, readBuffer.Length, null, stream);
                string readMessage = readBuffer.ToString();
                if (MessageValidation.IsMessageValid(MessageTypeConverter.ConvertToMessageType(readMessage), readMessage))
                {
                    return MessageSerialization.Deserialize<T>(readBuffer.ToString());
                }
                else
                    throw new Exception("Message not valid");
                
            }
            else
            {
                Console.WriteLine("Sorry.  You cannot read from this NetworkStream.");
                return null;
            }
        }
    }
}
