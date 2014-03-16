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
        public static string Receive(Stream stream)
        {
            byte[] buffer = new byte[1024];

            stream.Read(buffer, 0, buffer.Length);

            return MessageSerialization.GetString(buffer);
        }

        public static void Send(Stream stream, string message)
        {

        }
    }
}