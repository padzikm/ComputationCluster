using System;
using System.Net;

namespace ComputationalNode
{
    class Program
    {
        static void Main(string[] args)
        {
            string msg = "";
            ComputationalNode node;
            string ipAddress = "localhost";
            int port = 12345;


            if (args.Length > 0)
                ipAddress = args[0];
            if (args.Length > 1)
                port = int.Parse(args[1]);

            node = new ComputationalNode(ipAddress, port);

            Console.WriteLine("Node created. Start working...\n");
            Console.WriteLine("Type 'stop' to stop node");

            node.Start();

            while (msg.ToLower() != "stop")
                msg = Console.ReadLine();

            Console.WriteLine("Node's work ended. Closing program.");

            node.Stop();


        }
    }
}
