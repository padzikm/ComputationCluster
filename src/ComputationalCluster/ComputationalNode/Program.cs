using System;
using System.Net;

namespace ComputationalNode
{
    class Program
    {
        static void Main(string[] args)
        {
            string msg = "";

            var node = new ComputationalNode(12345);

            Console.WriteLine("Node created. Start working...\n");
            Console.WriteLine("Type 'stop' to stop node");

            node.Start(IPAddress.Any);

            while (msg.ToLower() != "stop")
                msg = Console.ReadLine();

            node.Stop();


        }
    }
}
