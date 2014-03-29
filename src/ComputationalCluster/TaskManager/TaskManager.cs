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
            statusThreads = new StatusThread[5];
            foreach (var statusThread in statusThreads.Select(statusThread => new StatusThread()))
            {
                statusThread.HowLong = 0;
                //statusThread.TaskId = registerResponse.Id;
                statusThread.ProblemType = "DVRP";
            }
            networkAdapter.CurrentStatus = new Status { Id = registerResponse.Id, Threads = statusThreads };

            networkAdapter.StartKeepAlive(10000, RecieveProblemData, SendSolvePartialProblems);

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

        private void SendSolvePartialProblems()
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
            networkAdapter.Send(partialProblems, false);
            Console.WriteLine("SendSolvePartialProblems");

        }

        private bool RecieveProblemData()
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

        private bool RecieveMergeRequest()
        {
            try
            {
                //problem = networkAdapter.Receive<Merge>(false);
                if (problem != null) return true;
            }
            catch (Exception)
            {
                return false;
            }
            return false;
        }

        private void SendSolution()
        {
            try
            {
                counter++;
                Solutions solution;
                if (counter > 10)
                    solution = new Solutions { ProblemType = "DVRP", Id = 1, Solutions1 = new[] {new SolutionsSolution{ Type = SolutionsSolutionType.Final} }};
                else
                {
                     solution = new Solutions { ProblemType = "DVRP", Id = 1, Solutions1 = new[] {new SolutionsSolution{ Type = SolutionsSolutionType.Partial} }};
                }
                //TODO Common data?
                networkAdapter.Send(solution, false);
            }
            catch (Exception)
            {
                Console.WriteLine("Cannot send solution to server");
            }

        }

    }
}
