using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using Common;

namespace CommunicationServer
{
    public class ServerNetworkAdapter
    {
        public static void Receive(Stream stream, out string message, out MessageType messageType) //TODO: implement this
        {
            byte[] buffer = new byte[1024];

            stream.Read(buffer, 0, buffer.Length);
            message = MessageSerialization.GetString(buffer);            
            messageType = MessageType.UnknownMessage;
        }

        public static void Send<T>(Stream stream, T message) where T : class 
        {            
            string xml = MessageSerialization.Serialize<T>(message);
            Byte[] data = System.Text.Encoding.UTF8.GetBytes(xml);
            stream.Write(data, 0, data.Length); 
            stream.Flush();
        }
    }
}