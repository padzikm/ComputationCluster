﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.Threading.Tasks;
using System.Net;
using System.Net.Sockets;
using System.IO;
using Common;

namespace ComputationalNode
{
    class ComputationalNode
    {
        private readonly NetworkAdapter networkAdapter;
        private Thread nodeThread;
        private StatusThread[] threads;
        private RegisterResponse registerResponse;
        private SolvePartialProblems problem;
        private bool working;
        private int port;

        public ComputationalNode(string serverName, int _port)
        {
            if (serverName == null || _port < 0)
                throw new ArgumentNullException();

            port = _port;
            working = true;

            networkAdapter = new NetworkAdapter(serverName, port);

        }

        public void Start()
        {

            if (nodeThread != null)
                throw new InvalidOperationException("Node is already running! Wait for partial problem.\n");

            try
            {
                nodeThread = new Thread(NodeWork);
                nodeThread.Start();
                //nodeThread.Start();
            }
            catch (Exception e)
            {
                Console.WriteLine("Error in node start: {0}", e.Message);
            }
        }

        public void Stop()
        {
            working = false;
            try
            {
                if (nodeThread != null)
                    nodeThread.Abort();
                nodeThread = null;
            }
            catch (Exception e)
            {
                Console.WriteLine("Error in node stop: {0} \n", e.Message);
            }

        }

        private void NodeWork()
        {


            networkAdapter.StartConnection();

            Register();
            RegisterResponse();

            networkAdapter.CloseConnection();

            initThreade();

            networkAdapter.CurrentStatus = new Status { Id = registerResponse.Id, Threads = threads };

            int timeout = 0;           
            string[] time= registerResponse.Timeout.Split(':');          
            timeout += int.Parse(time[2]);
            timeout += 60 * int.Parse(time[1]);
            timeout += 3600 * int.Parse(time[0]);

            var handlers = new Dictionary<Func<bool>, Action>
            {
                {PartialProblems, Solution},
            };

            networkAdapter.StartKeepAlive(timeout * 1000, handlers);
           
        }

        private void Register()
        {
            var registerMessage = new Register()
            {
                Type = RegisterType.ComputationalNode,
                SolvableProblems = new string[] { "DVRP", "DVRP" },
                ParallelThreads = 5
            };
            networkAdapter.Send(registerMessage, false);
        }

        private void RegisterResponse()
        {
            try
            {
                registerResponse = networkAdapter.Receive<RegisterResponse>(false);
                //Console.WriteLine("Receive RegisterResponse");
            }
            catch (Exception)
            {
                Console.WriteLine("Cannot recieve RegisterResponse");
            }

        }

        private bool PartialProblems()
        {
            try
            {
               
                 problem = networkAdapter.Receive<SolvePartialProblems>(false);
                if (problem != null)
                {
                    Console.WriteLine("Recive SolvePartialProblems");
                    return true;
                }
            }
            catch (Exception e )
            {
                Console.WriteLine("Cannot recive SolvePartialProblems {0}",e);
                return false;
            }
            return false;

        }

        private void Solution()
        {

            Thread.Sleep(3000);
            try
            {
                var solution = new Solutions
                {
                    ProblemType = "DVRP", 
                    Id = problem.Id,
                    Solutions1 = new[] { new SolutionsSolution { Type = SolutionsSolutionType.Partial } },
                    CommonData = new byte[5],
                    
                };
                networkAdapter.Send(solution, true);
            }
            catch (Exception)
            {
                Console.WriteLine("Cannot send partial solution to server");
            }

        }

        private void initThreade()
        {
            threads = new StatusThread[] { new StatusThread{HowLong = 0, ProblemType = "DVPR", State = StatusThreadState.Busy }};

        }

       
    }
}
