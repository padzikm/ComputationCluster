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
            ComputationalNode node = new ComputationalNode();
            string msg = "";

            node.StartConnection("12345");

            Console.WriteLine("Type 'stop' to stop node");

            while (msg.ToLower() != "stop")
                msg = Console.ReadLine();

            node.CloseConnection();


        }
    }
}
