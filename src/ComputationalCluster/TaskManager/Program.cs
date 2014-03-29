using System;
using System.Net;

namespace TaskManager
{
    class Program
    {
        static void Main(string[] args)
        {
            string msg = "";
            Console.WriteLine("Starting Task Manager");
            
            var taskManager = new TaskManager(IPAddress.Parse("192.168.0.11"), 12345);

            Console.WriteLine("Task Manager created. Start working...\n");
            Console.WriteLine("Type 'stop' to stop Task manager.\n");

            taskManager.Start();

            while (msg != null && msg.ToLower() != "stop")
                msg = Console.ReadLine();


            Console.WriteLine("Task manager's work ended. Closing program.");

        }
    }
}
