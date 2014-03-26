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

            //ComputationalClient client = new ComputationalClient(args[0], ulong.Parse(args[1]), Encoding.UTF8.GetBytes(args[2]));

            Client client = new Client("localhost", 12345, "dvrp", 10000, null);

            Console.WriteLine("Client created. Start working...\n");
            Console.WriteLine("Type 'stop' to stop client.\n");

            client.Start();

            while (msg.ToLower() != "stop")
                msg = Console.ReadLine();   

            Console.WriteLine("Client's work ended. Closing program.");
        }
    }
}
