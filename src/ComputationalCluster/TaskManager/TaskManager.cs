using System;
using System.Collections.Generic;
using System.Linq;
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

        //temporary
        private static int counter;
        public TaskManager(IPAddress serverIp, int port)
        {
            networkAdapter = new NetworkAdapter(serverIp, port);

        }

        public TaskManager(string serverName, int port)
        {
            networkAdapter = new NetworkAdapter(serverName, port);
        }

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
            statusThreads = new StatusThread[5];
            for (var i = 0; i < 5; i++)
            {
                statusThreads[i] = new StatusThread { State = StatusThreadState.Busy, HowLong = 1000, TaskId = registerResponse.Id, ProblemType = "DVRP" };
            }
            networkAdapter.CurrentStatus = new Status { Id = registerResponse.Id, Threads = statusThreads };
            var handlers = new Dictionary<Func<bool>, Action>
            {
                {ReceiveProblemData, SendPartialProblems},
                {ReceiveSolutions, SendFinalSolution}
            };
            networkAdapter.StartKeepAlive(10000, handlers);
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

        private bool ReceiveProblemData()
        {
            try
            {
                problem = networkAdapter.Receive<DivideProblem>(false);
                if (problem != null) return true;
            }
            catch (Exception)
            {
                return false;
            }
            return false;
        }

        private bool ReceiveSolutions()
        {
            try
            {
                solution = networkAdapter.Receive<Solutions>(false);
                if (solution != null) return true;
            }
            catch (Exception)
            {
                return false;
            }
            return false;
        }

        private void SendFinalSolution()
        {
            try
            {
                var solutionToSend = new Solutions
                {
                    ProblemType = "DVRP",
                    Id = 1,
                    Solutions1 = new[] { new SolutionsSolution { Type = SolutionsSolutionType.Final } }
                };
                networkAdapter.Send(solutionToSend, true);
            }
            catch (Exception)
            {
                Console.WriteLine("Cannot send final solution to server");
            }

        }


        private void SendPartialProblems()
        {
            try
            {
                var partialProblems = new SolvePartialProblems
                {
                    CommonData = new byte[5],
                    Id = problem.Id,
                    ProblemType = "DVRP",
                    PartialProblems = new SolvePartialProblemsPartialProblem[3],
                    SolvingTimeout = 3,
                    SolvingTimeoutSpecified = true
                };
                networkAdapter.Send(partialProblems, true);
                Console.WriteLine("SendSolvePartialProblems");


            }
            catch (Exception)
            {
                Console.WriteLine("Cannot send partial problems to server");
            }

        }

    }
}
