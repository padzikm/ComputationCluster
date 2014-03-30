using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommunicationServer
{
    public class TaskMergeWorker
    {
        /// <summary>
        /// Sends partial solutions to task (if any is available)
        /// </summary>
        public static void Work(ServerNetworkAdapter networkAdapter)
        {
            if (DvrpProblem.ProblemsMergeWaiting.Count > 0 && DvrpProblem.Tasks.Count > 0)
            {
                var tmp = DvrpProblem.ProblemsMergeWaiting.First();
                var request = DvrpProblem.PartialSolutions.First(p => p.Key == tmp.Key);
                SolveRequest problem = DvrpProblem.Problems[request.Key];
                Solutions response = new Solutions();
                response.Id = request.Key;
                response.ProblemType = problem.ProblemType;
                response.CommonData = problem.Data;
                if (request.Value != null)
                    response.Solutions1 = request.Value.ToArray();
                else
                    response.Solutions1 = new SolutionsSolution[] { new SolutionsSolution(), };

                if (networkAdapter.Send(response))
                    DvrpProblem.ProblemsMergeWaiting.Remove(request.Key);
            }            
        }
    }
}
