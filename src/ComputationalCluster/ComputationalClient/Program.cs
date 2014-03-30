using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Common;

namespace ComputationalClient
{
    class Program
    {
        static void Main(string[] args)
        {
            string msg = "";
            Client client;

            try
            {
                var adressIp = IPAddress.Parse(args[0]);
                var port = int.Parse(args[1]);
                client = new Client(adressIp.ToString(), port, "dvrp", 100000, new byte[1]);
            }
            catch (Exception)
            {
                client = new Client("localhost", 12345, "dvrp", 100000, new byte[1]);
            }


            Console.WriteLine("Client created. Start working...\n");
            Console.WriteLine("Type 'stop' to stop client.\n");

            client.Start();

            while (msg.ToLower() != "stop")
                msg = Console.ReadLine();   

            Console.WriteLine("Client's work ended. Closing program.");

            client.Stop();
        }
    }
}
