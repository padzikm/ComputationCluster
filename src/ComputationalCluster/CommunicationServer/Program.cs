using System;
using System.Net;
using Common;

namespace CommunicationServer
{   
    class Program
    {        
        static void Main(string[] args)
        {            
            IPAddress ipAddress = IPAddress.Any;
            int port = 12345;
            TimeSpan timout = new TimeSpan(0,0,30);

            if (args.Length > 0)
                ipAddress = IPAddress.Parse(args[0]);
            if (args.Length > 1)
                port = int.Parse(args[1]);
            if (args.Length > 2)
            {
                string[] table = args[2].Split(new char[] {':'});
                int hours = 0, minutes = 0, seconds = 30;
                if(table.Length > 0)
                    hours = int.Parse(table[0]);
                if(table.Length > 1)
                    minutes = int.Parse(table[1]);
                if(table.Length > 2)
                    seconds = int.Parse(table[2]);
                timout = new TimeSpan(hours, minutes, seconds);
            }

            Server server = new Server(IPAddress.Any, 12345, new TimeSpan(0,0,30));

            string msg = "";

            server.Start();

            Console.WriteLine("Type 'stop' to stop server");

            while (msg != null && msg.ToLower() != "stop")
                msg = Console.ReadLine();

            server.Stop();            
        }
    }
}
