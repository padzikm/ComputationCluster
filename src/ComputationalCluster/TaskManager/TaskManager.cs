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
            StartConnection(server, 12345);
            RegisterResponse();
            //RecieveProblemData();
        }
        public void RegisterResponse()
        {
            var registerMessage = new Register();
            registerMessage.Type = RegisterType.TaskManager;
            registerMessage.SolvableProblems = new string[] { "problem A", "problem B" };
            registerMessage.ParallelThreads = (byte)5;
            Send<Register>(registerMessage);
        }

        public void RecieveProblemData()
        {
            try
            {
                problem = this.Recieve<DivideProblem>();
            }
            
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        private void SendSolution()
        {
            var solution = new Solutions();
            solution.ProblemType = "DVRT";
            solution.Id = problem.Id;
            //TODO Common data?
            Send<Solutions>(solution);
        }

        public void Close()
        {
            this.CloseConnection();
        }



    }
}
