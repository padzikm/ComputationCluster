using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommunicationServer
{
    public class TaskDivideWorker
    {
        /// <summary>
        /// Sends problem to task (if any is available)
        /// </summary>
        public static void Work(ServerNetworkAdapter networkAdapter)
        {
            if (DvrpProblem.ProblemsDivideWaiting.Count > 0 && DvrpProblem.Tasks.Count > 0)
            {
                var divide = DvrpProblem.ProblemsDivideWaiting.First();
                var pr = DvrpProblem.Problems[divide.Key];
                DivideProblem div = new DivideProblem();
                div.Id = divide.Key;
                div.ProblemType = pr.ProblemType;
                div.Data = pr.Data;
                div.ComputationalNodes = (ulong)DvrpProblem.Nodes.Count;                 
                if(networkAdapter.Send(div))
                    DvrpProblem.ProblemsDivideWaiting.Remove(divide.Key);
            }            
        }
    }
}
