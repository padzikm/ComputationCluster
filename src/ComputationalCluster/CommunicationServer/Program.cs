using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Xml.Linq;
using Common;
using System.Net;
using System.Net.Sockets;
using System.IO;

namespace CommunicationServer
{   
    class Program
    {        
        static void Main(string[] args)
        {
            Server server = new Server(IPAddress.Any, 12345);
            string msg = "";

            server.Start();

            Console.WriteLine("Type 'stop' to stop server");

            while (msg.ToLower() != "stop")
                msg = Console.ReadLine();

            server.Stop();            
        }
    }
}
