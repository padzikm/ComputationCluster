using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommunicationServer
{
    class NodeWorker
    {
        public static void Work()
        {
            while (true)
            {
                DvrpProblem.NodeEvent.WaitOne();
                DvrpProblem.WaitEvent.WaitOne();
                if (DvrpProblem.PartialProblems.Count > 0 && DvrpProblem.Nodes.Count > 0)
                {
                    var pr = DvrpProblem.PartialProblems.First();
                    var msg = new SolvePartialProblems();
                    msg.Id = pr.Key;
                    msg.CommonData = DvrpProblem.Problems[pr.Key].Data;
                    msg.ProblemType = DvrpProblem.Problems[pr.Key].ProblemType;
                    msg.SolvingTimeout = DvrpProblem.Problems[pr.Key].SolvingTimeout;
                    msg.SolvingTimeoutSpecified = DvrpProblem.Problems[pr.Key].SolvingTimeoutSpecified;
                    msg.PartialProblems = pr.Value.ToArray();

                    foreach (var node in DvrpProblem.Nodes)                    
                        ServerNetworkAdapter.Send(DvrpProblem.ComponentsAddress[node.Key], msg);                    
                }
                DvrpProblem.WaitEvent.Set();
            }
        }
    }
}
