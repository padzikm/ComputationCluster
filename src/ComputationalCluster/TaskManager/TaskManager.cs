using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Threading;
using Common;
using System.Net;
using DVRP;
using UCCTaskSolver;

namespace TaskManager
{
    public class TaskManager
    {
        private DivideProblem problem;
        private Solutions solution;
        private RegisterResponse registerResponse;
        private StatusThread[] statusThreads;
        private readonly TMNetworkAdapter networkAdapter;
        private ulong taskId = 0;
        private TaskSolver taskSolver; 

        public TaskManager(IPAddress serverIp, int port)
        {
            networkAdapter = new TMNetworkAdapter(serverIp, port);

        }

        public TaskManager(string serverName, int port)
        {
            networkAdapter = new TMNetworkAdapter(serverName, port);
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

            statusThreads[0] = new StatusThread { State = StatusThreadState.Idle, HowLong = 0, TaskId = registerResponse.Id, ProblemType = "DVRP" };
            networkAdapter.CurrentStatus = new Status { Id = registerResponse.Id, Threads = statusThreads };
            int timeout = int.Parse(registerResponse.Timeout.Substring(6, 2));
            networkAdapter.StartKeepAliveTask(timeout * 1000, SendPartialProblems, SendFinalSolution, ReceiveDivide, ReceiveSolutions);
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
                List<byte[]> solutions = new List<byte[]>();
                foreach (var partialSolution in solution.Solutions1)
                {
                    solutions.Add(partialSolution.Data);
                }
                taskSolver.MergeSolution(solutions.ToArray());
                
                var solutionToSend = new Solutions
                {
                    ProblemType = "DVRP",
                    Id = 1,
                    
                    Solutions1 = new[] { new SolutionsSolution { Type = SolutionsSolutionType.Final, TaskId = (ulong) id, Data = taskSolver.Solution} }
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
                taskSolver = new DVRP.DVRP(problem.Data);
                var dividedProblems = taskSolver.DivideProblem((int) problem.ComputationalNodes);
                var solvePartialProblemsPartialProblem = new SolvePartialProblemsPartialProblem[dividedProblems.Length];
                for (int i = 0; i < dividedProblems.Length; i++)
                {
                    solvePartialProblemsPartialProblem[i] = new SolvePartialProblemsPartialProblem {Data = dividedProblems[i], TaskId = ++taskId};
                }

                var partialProblems = new SolvePartialProblems
                {
                    CommonData = problem.Data,
                    Id = id,
                    ProblemType = "DVRP",
                    PartialProblems = solvePartialProblemsPartialProblem,
                    SolvingTimeout = 100000,
                    SolvingTimeoutSpecified = true
                };
                networkAdapter.Send(partialProblems, true);
                Console.WriteLine("SendSolvePartialProblems");
            }
            catch (Exception e)
            {
                Console.WriteLine("Cannot send partial problems to server: " + e.Message);
            }

        }

        private void ReceiveDivide(DivideProblem divideProblem)
        {
            problem = divideProblem;
        }

        private void ReceiveSolutions(Solutions solutions)
        {
            this.solution = solutions;
        }

    }
}
