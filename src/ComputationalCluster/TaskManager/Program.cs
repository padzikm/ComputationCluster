using System;
using System.Net;
using System.Threading.Tasks;

namespace TaskManager
{
    class Program
    {
        static void Main(string[] args)
        {
            string msg = "";
            Console.WriteLine("Starting Task Manager");

            TaskManager taskManager;

            try
            {
                var adressIp = IPAddress.Parse(args[0]);
                var port = int.Parse(args[1]);
                taskManager = new TaskManager(adressIp, port);
            }
            catch (Exception)
            {
                taskManager = new TaskManager("localhost", 12345);
            }
            

            Console.WriteLine("Task Manager created. Start working...\n");
            Console.WriteLine("Type 'stop' to stop Task manager.\n");

            taskManager.Start();

            while (msg != null && msg.ToLower() != "stop")
                msg = Console.ReadLine();


            Console.WriteLine("Task manager's work ended. Closing program.");

        }
    }
}
