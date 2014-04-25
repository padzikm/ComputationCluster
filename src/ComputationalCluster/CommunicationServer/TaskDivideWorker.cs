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
        public static void Work(ulong taskId, ServerNetworkAdapter networkAdapter)
        {
            if (DvrpProblem.ProblemsDividing[taskId].Count > 0 || DvrpProblem.SolutionsMerging[taskId].Count > 0)
                return;

            ulong problemId = 0;

            var problems = new List<ulong>(DvrpProblem.ProblemsID);

            foreach (var list in DvrpProblem.ProblemsDividing.Values)
                foreach (var el in list)
                    problems.Remove(el);

            foreach (var list in DvrpProblem.SolutionsMerging.Values)
                foreach (var el in list)
                    problems.Remove(el);

            foreach (var id in problems)
                if (!DvrpProblem.PartialProblems.ContainsKey(id) && !DvrpProblem.PartialSolutions.ContainsKey(id) && !DvrpProblem.ProblemSolutions.ContainsKey(id))
                {
                    problemId = id;
                    break;
                }

            if (problemId == 0)
                return;

            var pr = DvrpProblem.Problems[problemId];
            DivideProblem div = new DivideProblem();
            div.Id = problemId;
            div.ProblemType = pr.ProblemType;
            div.Data = pr.Data;
            div.ComputationalNodes = (ulong)DvrpProblem.Nodes.Count;

            if (networkAdapter.Send(div))
                DvrpProblem.ProblemsDividing[taskId].Add(problemId);
        }
    }
}
