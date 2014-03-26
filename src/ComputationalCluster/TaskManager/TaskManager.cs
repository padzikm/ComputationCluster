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
        private NetworkAdapter networkAdapter;
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
           
            //networkAdapter.StartConnection(serverIpAddress, port);
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

            networkAdapter.StartKeepAlive(registerResponse.Timeout.Millisecond);
            isStarted = true;

            while (isStarted)
            {
                RecieveProblemData();
                SendSolution();
            }

        }

        private void SendRegisterMessage()
        {
            var registerMessage = new Register
            {
                Type = RegisterType.TaskManager,
                SolvableProblems = new[] { "DVRP", "DVRP" },
                ParallelThreads = 5
            };
            networkAdapter.Send(registerMessage);
            Console.WriteLine("Send Register Message");
        }

        private void RecieveRegisterResponse()
        {
            registerResponse = networkAdapter.Recieve<RegisterResponse>();

        }

        private void RecieveProblemData()
        {
            try
            {
                problem = networkAdapter.Recieve<DivideProblem>();
            }
            
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        private void SendSolution()
        {
            var solution = new Solutions {ProblemType = "DVRP", Id = problem.Id};
            //TODO Common data?
            networkAdapter.Send(solution);
        }

        public void Close()
        {
            isStarted = false;
            //networkAdapter.CloseConnection();
        }



    }
}
