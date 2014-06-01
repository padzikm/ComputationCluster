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
        private Stream stream;

        public ServerNetworkAdapter(Stream stream)
        {
            this.stream = stream;
        }

        /// <summary>
        /// Sends message using opened stream
        /// </summary>
        /// <typeparam name="T"></typeparam>        
        /// <param name="message">Message to send</param>
        public virtual bool Send<T>(T message) where T : class 
        {
            try
            {
                string xml = MessageSerialization.Serialize<T>(message);
                Byte[] data = System.Text.Encoding.UTF8.GetBytes(xml);
                stream.Write(data, 0, data.Length);
                stream.Flush();
                Console.WriteLine(@"Wyslano: 
{0}", xml);
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine(@"Blad w send: {0}", ex.Message);
                return false;
            }
        }        
    }
}