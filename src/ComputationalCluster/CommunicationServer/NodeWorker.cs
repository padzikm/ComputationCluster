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
        public static void Work(ulong nodeId, ServerNetworkAdapter networkAdapter)
        {
            foreach (var list in DvrpProblem.PartialProblems.Values)
                foreach (var el in list)
                    if (el.Value == nodeId)
                        return;

            ulong problemId = 0;
            KeyValuePair<SolvePartialProblemsPartialProblem, ulong> partialProblem = new KeyValuePair<SolvePartialProblemsPartialProblem, ulong>();            

            foreach (var list in DvrpProblem.PartialProblems)
            {
                partialProblem = list.Value.FirstOrDefault(p => p.Value == 0);
                
                if (partialProblem.Key != null)
                {
                    list.Value.Remove(partialProblem);
                    problemId = list.Key;
                    break;
                }
            }

            if (partialProblem.Key == null)
                return;

            var msg = new SolvePartialProblems();
            msg.Id = problemId;
            msg.CommonData = DvrpProblem.Problems[problemId].Data;
            msg.ProblemType = DvrpProblem.Problems[problemId].ProblemType;
            msg.SolvingTimeout = DvrpProblem.Problems[problemId].SolvingTimeout;
            msg.SolvingTimeoutSpecified = DvrpProblem.Problems[problemId].SolvingTimeoutSpecified;

            msg.PartialProblems = new SolvePartialProblemsPartialProblem[] { partialProblem.Key };

            if (networkAdapter.Send(msg))
                DvrpProblem.PartialProblems[problemId].Add(new KeyValuePair<SolvePartialProblemsPartialProblem, ulong>(partialProblem.Key, nodeId));                
        }
    }
}
