using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommunicationServer
{
    public class NodeWorker
    {
        /// <summary>
        /// Sends partial problem to client (if any is available)
        /// </summary>
        public static void Work(ServerNetworkAdapter networkAdapter)
        {
            if (DvrpProblem.ProblemsComputeWaiting.Count > 0 && DvrpProblem.Nodes.Count > 0)
            {
                var tmp = DvrpProblem.ProblemsComputeWaiting.First();
                var pr = DvrpProblem.PartialProblems.First(p => p.Key == tmp.Key);
                var msg = new SolvePartialProblems();
                msg.Id = pr.Key;
                msg.CommonData = DvrpProblem.Problems[pr.Key].Data;
                msg.ProblemType = DvrpProblem.Problems[pr.Key].ProblemType;
                msg.SolvingTimeout = DvrpProblem.Problems[pr.Key].SolvingTimeout;
                msg.SolvingTimeoutSpecified = DvrpProblem.Problems[pr.Key].SolvingTimeoutSpecified;
                if (pr.Value.Count != 0)
                    msg.PartialProblems = pr.Value.ToArray();
                else
                    msg.PartialProblems = new SolvePartialProblemsPartialProblem[]
                    {new SolvePartialProblemsPartialProblem() {Data = new byte[1]}};

                if (networkAdapter.Send(msg))
                    DvrpProblem.ProblemsComputeWaiting.Remove(tmp.Key);
            }            
        }
    }
}
