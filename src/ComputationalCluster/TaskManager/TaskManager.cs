using System;
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
        private bool isStarted;

        private readonly IPAddress serverIpAddress;
        private readonly int port;


        public TaskManager(IPAddress serverIp, int port)
        {
            serverIpAddress = serverIp;
            this.port = port;
            networkAdapter = new NetworkAdapter(serverIp, port);
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
            networkAdapter.CurrentStatus = new Status {Id = registerResponse.Id, Threads = statusThreads};

            //TODO zero timeout in register response.Timeout?
            networkAdapter.StartKeepAlive(10000, RecieveProblemData, SendSolution);

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

        private bool RecieveProblemData()
        {
            try
            {
                problem = networkAdapter.Receive<DivideProblem>(false);
                if (problem != null) return true;
            }
            catch (Exception e )
            {
                //Console.WriteLine(e);
                return false;
            }
            return false;
        }

        private void SendSolution()
        {
            try
            {
                
                var solution = new Solutions { ProblemType = "DVRP", Id = 1 };
                //TODO Common data?
                networkAdapter.Send(solution, false);
            }
            catch (Exception)
            {
                Console.WriteLine("Cannot send solution to server");
            }

        }

        public void Close()
        {
            isStarted = false;
            //networkAdapter.CloseConnection();
        }



    }
}
