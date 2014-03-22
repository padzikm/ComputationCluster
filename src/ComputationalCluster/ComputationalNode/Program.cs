using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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

            node.Start("192.168.0.12");

            while (msg.ToLower() != "stop")
                msg = Console.ReadLine();

            node.Stop();


        }
    }
}
