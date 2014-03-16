using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace ComputationalClient
{
    class Program
    {
        static void Main(string[] args)
        {
            ComputationalClient client = new ComputationalClient(args[0], ulong.Parse(args[1]), Encoding.UTF8.GetBytes(args[2]));

            Console.WriteLine("Client created. Start working...");

            client.ClientWork("Name of Server?");

            Console.WriteLine("Client's work ended. Closing program.");
        }
    }
}
