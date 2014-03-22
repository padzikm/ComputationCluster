
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
        protected int port;
        private NetworkStream stream;

        public virtual void StartConnection(String server, int _port)
        {
            client = new TcpClient(server, _port);
            stream = client.GetStream();
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

                    if (!Send<Status>(statusMessage))
                        throw new Exception("StatusMessage");
                    
                    Thread.Sleep(time.Millisecond);
                }
            });
            t.Start();
            while (t.ThreadState == ThreadState.Aborted || t.ThreadState == ThreadState.Stopped)  // ?
                t.Abort();
        }

        //TODO rather async ?
        public virtual bool Send<T>(T message) where T : class
        {
            Thread t = new Thread(() =>
            {
                try
                {
                    
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
            });
            t.Start();
            while (t.ThreadState == ThreadState.Aborted || t.ThreadState == ThreadState.Stopped)  // ?
                t.Abort();

            return true;
        }

        //TODO: rather not async
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
