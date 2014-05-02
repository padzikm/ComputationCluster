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
using System.Threading;

namespace ComputationalClient
{
    class Program
    {
        static String message = "Problem sent successfully.\nComputation is in progress... Press ENTER to check a problem status.";

        //static string defaultProblemFile = "../../../../../doc/SampleProblems/okul12D.vrp";
        static string defaultProblemFile = "C:\\Users\\Kamil\\Documents\\GitHub\\ComputationCluster\\doc\\SampleProblems\\okul12D.vrp";

        static string addressIp;
        static int port;
        static string name;
        static ulong timeout;
        static string pathToFile;
        static byte[] problemBytes;

        static void Main(string[] args)
        {
            string msg = "";
            Client client;
            //bool connected = false;
            
            try
            {
                addressIp = args[0];
                port = int.Parse(args[1]);
                name = args[2];
                timeout = ulong.Parse(args[3]);
                pathToFile = "";//args[4];
                problemBytes = GetBytesFromVrp(pathToFile);
                //Console.WriteLine(addressIp + " " + port + " " + name + " " + timeout + " " + args[4]);
                //ReadData();
                client = new Client(addressIp, port, name, timeout, problemBytes);
            }
            catch (Exception)
            {
                Console.WriteLine("Couldn't work with program parameters. Default one used.");
                client = new Client("localhost", 12345, "dvrp", 100000, GetBytesFromVrp(defaultProblemFile));
            }

            //while (connected == false)
            //{
                //try
                //{
                    client.Start();
                    //Thread.Sleep(6000);
                    //connected = true;
                //}
                //catch (Exception e)
                //{
                //    Console.WriteLine("Some problem occured:\n" + e.Message + "\n\n");
                //    Console.WriteLine("Try again by typing new values (or press ESC to exit):\n\n");
                //    ReadData();
                //    connected = false;
                //}
            //}

            Console.WriteLine("Client created. Start working...\n");
            Console.WriteLine("Type 'stop' to stop client.\n");

            while (msg.ToLower() != "stop")
                msg = Console.ReadLine();   

            Console.WriteLine("Client's work ended. Closing program.");

            client.Stop();
        }

        static void ReadData()
        {
            Console.Write("IP Address:     ");
            addressIp = Console.ReadLine();
            Console.Write("Port number:    ");
            port = int.Parse(Console.ReadLine());
            Console.Write("Problem name:   ");
            name = Console.ReadLine();
            Console.Write("Timeout:        ");
            timeout = ulong.Parse(Console.ReadLine());
            Console.Write("Absolute path:  ");
            pathToFile = Console.ReadLine();
            problemBytes = GetBytesFromVrp(pathToFile);
        }

        static byte[] GetBytesFromVrp(string name)
        {
            StreamReader streamReader = new StreamReader(name);
            var problemString = streamReader.ReadToEnd();
            streamReader.Close();
            Console.WriteLine(problemBytes);
            return DataSerialization.GetBytes(problemString);
        }
    }
}
