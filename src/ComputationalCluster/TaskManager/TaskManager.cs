using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Threading;
using Common;
using System.Net;

namespace TaskManager
{
    public class TaskManager
    {
        private DivideProblem problem;
        private Solutions solution;
        private RegisterResponse registerResponse;
        private StatusThread[] statusThreads;
        private readonly NetworkAdapter networkAdapter;

        public TaskManager(IPAddress serverIp, int port)
        {
            networkAdapter = new NetworkAdapter(serverIp, port);

        }

        public TaskManager(string serverName, int port)
        {
            networkAdapter = new NetworkAdapter(serverName, port);
        }
        /// <summary>
        /// Starts new connection between TaskManger and Server
        /// </summary>
        public void Start()
        {
            var thread = new Thread(TaskWork);
            thread.Start();
        }

        private void TaskWork()
        {
            networkAdapter.StartConnection();
            SendRegisterMessage();
            RecieveRegisterResponse();
            networkAdapter.CloseConnection();
            statusThreads = new StatusThread[1];

            statusThreads[0] = new StatusThread { State = StatusThreadState.Busy, HowLong = 1000, TaskId = registerResponse.Id, ProblemType = "DVRP" };
            networkAdapter.CurrentStatus = new Status { Id = registerResponse.Id, Threads = statusThreads };
            int timeout = int.Parse(registerResponse.Timeout.Substring(6, 2));
            networkAdapter.StartKeepAliveTask(timeout * 1000, SendPartialProblems, SendFinalSolution);
        }

        private void SendRegisterMessage()
        {
            var registerMessage = new Register
            {
                Type = RegisterType.TaskManager,
                SolvableProblems = new[] { "DVRP", "DVRP" },
                ParallelThreads = 5
            };
            networkAdapter.Send(registerMessage, false);
            Console.WriteLine("Send Register Message");
        }

        private void RecieveRegisterResponse()
        {
            try
            {
                registerResponse = networkAdapter.Receive<RegisterResponse>(false);
            }
            catch (Exception)
            {

                Console.WriteLine("Cannot recieve RegisterResponse");
            }

        }

        private void SendFinalSolution(ulong id)
        {
            try
            {
                var solutionToSend = new Solutions
                {
                    ProblemType = "DVRP",
                    Id = 1,
                    Solutions1 = new[] { new SolutionsSolution { Type = SolutionsSolutionType.Final, TaskId = (ulong) id, Data = new byte[5]} }
                };
                networkAdapter.Send(solutionToSend, true);
            }
            catch (Exception)
            {
                Console.WriteLine("Cannot send final solution to server");
            }

        }

        private void SendPartialProblems(ulong id)
        {
            try
            {
                var solvePartialProblemsPartialProblem = new SolvePartialProblemsPartialProblem[3];
                for (int i = 0; i < 3; i++)
                {
                    solvePartialProblemsPartialProblem[i] = new SolvePartialProblemsPartialProblem {Data = new byte[5], TaskId = registerResponse.Id};
                }

                var partialProblems = new SolvePartialProblems
                {
                    CommonData = new byte[5],
                    Id = id,
                    ProblemType = "DVRP",
                    PartialProblems = solvePartialProblemsPartialProblem,
                    SolvingTimeout = 3,
                    SolvingTimeoutSpecified = true
                };
                networkAdapter.Send(partialProblems, true);
                Console.WriteLine("SendSolvePartialProblems");


            }
            catch (Exception e)
            {
                Console.WriteLine("Cannot send partial problems to server");
            }

        }

    }
}
