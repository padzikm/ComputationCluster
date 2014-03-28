using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
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
            try
            {
                string xml = MessageSerialization.Serialize<T>(message);
                Byte[] data = System.Text.Encoding.UTF8.GetBytes(xml);
                stream.Write(data, 0, data.Length);
                stream.Flush();
                Console.WriteLine("Wyslano: \n{0}", xml);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Blad w send: {0}", ex.Message);
            }
        }

        public static void Send<T>(EndPoint endPoint, T message) where T : class
        {
            try
            {
                IPAddress ip = IPAddress.Parse(((IPEndPoint) endPoint).Address.ToString());
                int port = ((IPEndPoint) endPoint).Port;
                TcpClient client = client = new TcpClient((IPEndPoint) endPoint);
                Stream stream = client.GetStream();
                string xml = MessageSerialization.Serialize<T>(message);
                Byte[] data = System.Text.Encoding.UTF8.GetBytes(xml);
                stream.Write(data, 0, data.Length);
                stream.Flush();
                Console.WriteLine("Wyslano: \n{0}", xml);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Blad w send endpoint: {0}", ex.Message);
            }
        }
    }
}