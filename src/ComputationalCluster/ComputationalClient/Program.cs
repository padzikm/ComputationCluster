using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Common;
using DvrpUtils;

namespace ComputationalClient
{
    class Program
    {
        static string defaultProblemFile = "E:\\Temp\\DOWNLOAD\\IO\\montemanni\\Dynamisch\\christofides\\c50.vrp";

        static void Main(string[] args)
        {
            string msg = "";
            Client client;

            try
            {
                var addressIp = args[0];
                var port = int.Parse(args[1]);
                var name = args[2];
                var timeout = ulong.Parse(args[3]);
                var problemBytes = GetBytesFromVrp(args[4]);
                //Console.WriteLine(addressIp + " " + port + " " + name + " " + timeout + " " + args[4]);
                client = new Client(addressIp, port, name, timeout, problemBytes);
            }
            catch (Exception)
            {
                Console.WriteLine("Couldn't work with program parameters. Default one used.");
                client = new Client("localhost", 12345, "dvrp", 100000, GetBytesFromVrp(defaultProblemFile));
            }

            Console.WriteLine("Client created. Start working...\n");
            Console.WriteLine("Type 'stop' to stop client.\n");

            client.Start();

            while (msg.ToLower() != "stop")
                msg = Console.ReadLine();   

            Console.WriteLine("Client's work ended. Closing program.");

            client.Stop();
        }

        static byte[] GetBytesFromVrp(string name)
        {
            StreamReader streamReader = new StreamReader(name);
            var problemString = streamReader.ReadToEnd();
            streamReader.Close();
            return DataSerialization.GetBytes(problemString);
        }
    }
}
