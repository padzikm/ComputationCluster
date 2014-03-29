using System;
using System.Net;

namespace ComputationalNode
{
    class Program
    {
        static void Main(string[] args)
        {
            string msg = "";

            var node = new ComputationalNode("localhost",12345);

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
