using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.Net;
using System.Net.Sockets;
using System.IO;
using System.Threading;
using System.Diagnostics;
using System.Text;
using Common;
using DvrpUtils;

namespace ComputationalNode
{
    class ComputationalNode
    {
        private readonly NetworkAdapter networkAdapter;
        private Thread nodeThread;
        private StatusThread[] threads;
        private RegisterResponse registerResponse;
        private SolvePartialProblems problem;
        private DVRPTaskSolver DVRPSolver;
        private Dictionary<ulong, byte[]> solutions;
        private ulong taskId = 0;
        private const int availableThreads = 1;
        private ulong computationsTime = 0;
        private bool timeOccured = false;

        /// <summary>
        /// Specific constructor for ComputationalNode class. Allows to execute node's correctly.
        /// </summary>
        /// <param name="serverName">String value of a server address name. </param>
        /// <param name="port">Port on which server is listening.</param>
        public ComputationalNode(string serverName, int port)
        {
            if (serverName == null || port < 0)
                throw new ArgumentNullException();     
            networkAdapter = new NetworkAdapter(serverName, port);
        }

        /// <summary>
        /// Specific constructor for ComputationalNode class. Allows to execute node's correctly.
        /// </summary>
        /// <param name="serverIp">IPAdress of a server address name.</param>
        /// <param name="port">Port on which server is listening.</param>
        public ComputationalNode(IPAddress serverIp, int port)
        {
            if (serverIp == null || port < 0)
                throw new ArgumentNullException();
            networkAdapter = new NetworkAdapter(serverIp, port);
        }

        /// <summary>
        /// This method starts whole Node work. 
        /// </summary>
        public void Start()
        {
            if (nodeThread != null)
                throw new InvalidOperationException("Node is already running! Wait for partial problem.\n");
            try
            {
                nodeThread = new Thread(NodeWork);
                nodeThread.Start();
            }
            catch (Exception e)
            {
                Console.WriteLine(@"Error in node start: {0}", e.Message);
            }
        }

        /// <summary>
        /// Allow to terminate correctly program execution.
        /// </summary>
        public void Stop()
        {
            try
            {
                if (nodeThread != null)
                    nodeThread.Abort();
                nodeThread = null;
            }
            catch (Exception e)
            {
                Console.WriteLine(@"Error in node stop: {0} 
", e.Message);
            }
        }

        private void NodeWork()
        {
            networkAdapter.StartConnection();
            Register();
            RegisterResponse();
            networkAdapter.CloseConnection();

            initThread();
            networkAdapter.CurrentStatus = new Status { Id = registerResponse.Id, Threads = threads };
            int timeout = 0;

            string[] time= registerResponse.Timeout.Split(':');          
            timeout += int.Parse(time[2]);
            timeout += 60 * int.Parse(time[1]);
            timeout += 3600 * int.Parse(time[0]);
            timeout *= 1000;

            networkAdapter.StartKeepAlive(timeout, PartialProblems, Solution);
        }

        private void Register()
        {
            var registerMessage = new Register()
            {
                Type = RegisterType.ComputationalNode,
                SolvableProblems = new string[] { "DVRP" },
                ParallelThreads = availableThreads
            };
            networkAdapter.Send(registerMessage, false);
        }

        private void RegisterResponse()
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
       
        private bool PartialProblems()
        {
            Action action;
            int timeout;
            try
            {
                problem = networkAdapter.Receive<SolvePartialProblems>(false);
               
                if (problem != null && problem.PartialProblems != null)
                {
                    timeout = (int)problem.SolvingTimeout;
                    Console.WriteLine("Dostałem problem, zaczynam liczyć.");
                    DVRPSolver = new DVRPTaskSolver(null);
                   
                    solutions = new Dictionary<ulong, byte[]>();

                    Stopwatch sw = Stopwatch.StartNew();
                    action = solve;
                    ManualResetEvent evt = new ManualResetEvent(false);
                    AsyncCallback cb = delegate { evt.Set(); };
                    IAsyncResult result = action.BeginInvoke(cb, null);
                    if (evt.WaitOne(timeout))
                    {
                        action.EndInvoke(result);
                        timeOccured = false;

                    }
                    else
                    {
                        timeOccured = true;
                        throw new TimeoutException();
                    }

                    sw.Stop();
                    computationsTime =(ulong) sw.Elapsed.TotalMilliseconds;

                    return true;
                }
            }
            catch (Exception e )
            {
                Console.WriteLine(@"Cannot recive SolvePartialProblems {0}",e);
                return false;
            }
            return false;
        }

        private void Solution()
        {
            try
            {
                List<SolutionsSolution> s = new List<SolutionsSolution>();
                foreach(var e in solutions)
                {
                    s.Add(new SolutionsSolution
                    {
                        Type = SolutionsSolutionType.Partial,
                        Data = e.Value,
                        TaskId = e.Key, 
                        TaskIdSpecified = true,
                        ComputationsTime = computationsTime,
                        TimeoutOccured = timeOccured
                    });
                }
   
                var solution = new Solutions
                {
                    ProblemType = problem.ProblemType, 
                    Id = problem.Id,
                    Solutions1 = s.ToArray(),
                    CommonData = problem.CommonData,
                };

                networkAdapter.Send(solution, true);
            }
            catch (Exception)
            {
                Console.WriteLine("Cannot send partial solution to server");
            }
        }

        private void initThread()
        {
            threads = new StatusThread[availableThreads];
            for (int i = 0; i < availableThreads;++i)
            {
                threads[i] = new StatusThread
                {
                    HowLong = 0, 
                    State = StatusThreadState.Idle ,
                };
            }        
        }

         void solve()
        {
            foreach (SolvePartialProblemsPartialProblem problemData in problem.PartialProblems)
            {
                var solution = DVRPSolver.Solve(problemData.Data, TimeSpan.FromMilliseconds((long)problem.SolvingTimeout));
                if (solution != null)
                    solutions.Add(problemData.TaskId, solution);
            }
        }
    }
}
