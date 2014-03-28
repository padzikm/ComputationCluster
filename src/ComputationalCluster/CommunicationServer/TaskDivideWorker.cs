using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommunicationServer
{
    class TaskDivideWorker
    {
        /// <summary>
        /// Sends problem to task (if any is available)
        /// </summary>
        public static void Work(ServerNetworkAdapter networkAdapter)
        {
            if (DvrpProblem.Problems.Count > 0 && DvrpProblem.Tasks.Count > 0)
            {
                var pr = DvrpProblem.Problems.First();
                DivideProblem div = new DivideProblem();
                div.Id = pr.Key;
                div.ProblemType = pr.Value.ProblemType;
                div.Data = pr.Value.Data;
                div.ComputationalNodes = (ulong)DvrpProblem.Nodes.Count;
                networkAdapter.Send(div);
            }            
        }
    }
}
