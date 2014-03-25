using System;
using System.Net;

namespace CommunicationServer
{   
    class Program
    {        
        static void Main(string[] args)
        {
            Server server = new Server(IPAddress.Parse("192.168.0.11"), 12345, new TimeSpan(0, 1, 0));
            string msg = "";

            server.Start();

            Console.WriteLine("Type 'stop' to stop server");

            while (msg != null && msg.ToLower() != "stop")
                msg = Console.ReadLine();

            server.Stop();            
        }
    }
}
