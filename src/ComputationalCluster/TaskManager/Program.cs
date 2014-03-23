﻿using System;
using System.Net;

namespace TaskManager
{
    class Program
    {
        static void Main(string[] args)
        {
            string msg = "";
            Console.WriteLine("Starting Task Manager");
            
            var taskManager = new TaskManager();

            Console.WriteLine("Task Manager created. Start working...\n");
            Console.WriteLine("Type 'stop' to stop Task manager.\n");

            taskManager.Start(IPAddress.Any, 12345);

            while (msg != null && msg.ToLower() != "stop")
                msg = Console.ReadLine();

            taskManager.Close();

            Console.WriteLine("Task manager's work ended. Closing program.");

        }
    }
}
