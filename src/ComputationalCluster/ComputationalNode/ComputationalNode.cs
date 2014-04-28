using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.Net;
using System.Net.Sockets;
using System.IO;
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
        private bool working;
        private DVRPTaskSolver solve;
        private byte[] solutions;
        private ulong taskId = 0;
        /// <summary>
        /// Specific constructor for ComputationalNode class. Allows to execute node's correctly.
        /// </summary>
        /// <param name="serverName">String value of a server address name. </param>
        /// <param name="port">Port on which server is listening.</param>
        public ComputationalNode(string serverName, int port)
        {
            if (serverName == null || port < 0)
                throw new ArgumentNullException();

            working = true;
            
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

            working = true;

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
                //nodeThread.Start();
            }
            catch (Exception e)
            {
                Console.WriteLine("Error in node start: {0}", e.Message);
            }
        }

        /// <summary>
        /// Allow to terminate correctly program execution.
        /// </summary>
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

            networkAdapter.StartKeepAlive(timeout * 1000, PartialProblems, Solution);

           
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
                    solve = new DVRPTaskSolver(problem.PartialProblems[0].Data);
                    
                    solutions = solve.Solve(problem.PartialProblems[0].Data, new TimeSpan(100000) );
                    Console.WriteLine("Recive SolvePartialProblems");
                    return true;
                }
                //byte[] solution = Solve(problem, problem.SolvingTimeout);

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
                    Solutions1 = new[] { new SolutionsSolution { Type = SolutionsSolutionType.Partial, Data=solutions, TaskId = ++taskId, TaskIdSpecified = true } },
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
