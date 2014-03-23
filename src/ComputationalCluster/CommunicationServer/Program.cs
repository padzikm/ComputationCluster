using System;
using System.Net;

namespace CommunicationServer
{   
    class Program
    {        
        static void Main(string[] args)
        {
            var server = new Server(IPAddress.Any, 12345);
            var msg = "";

            server.Start();

            Console.WriteLine("Type 'stop' to stop server");

            while (msg != null && msg.ToLower() != "stop")
                msg = Console.ReadLine();

            server.Stop();            
        }
    }
}
