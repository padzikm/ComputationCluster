using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Common;
using System.Net;

namespace TaskManager
{
    public class TaskManager : NetworkAdapter
    {
        private DivideProblem problem;
        private StatusThread[] statusThreads;

        public void Start(IPAddress server, int port)
        {
            StartConnection(server, port);
            RegisterResponse();
            CurrentStatus = new Status {Id = 1, Threads = statusThreads};
            StartKeepAlive(6000);
            RecieveProblemData();
        }
        public void RegisterResponse()
        {
            var registerMessage = new Register
            {
                Type = RegisterType.TaskManager,
                SolvableProblems = new[] {"problem A", "problem B"},
                ParallelThreads = 5
            };
            Send(registerMessage);
        }

        public void RecieveProblemData()
        {
            try
            {
                problem = Recieve<DivideProblem>();
            }
            
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        private void SendSolution()
        {
            var solution = new Solutions {ProblemType = "DVRT", Id = problem.Id};
            //TODO Common data?
            Send(solution);
        }

        public void Close()
        {
            this.CloseConnection();
        }



    }
}
