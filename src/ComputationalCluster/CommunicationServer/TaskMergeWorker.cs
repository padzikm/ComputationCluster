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
                if (!DvrpProblem.PartialProblems.ContainsKey(id) && DvrpProblem.PartialSolutions.ContainsKey(id) && !DvrpProblem.ProblemSolutions.ContainsKey(id))
                {
                    problemId = id;
                    break;
                }

            if (problemId == 0)
                return;
            
            var partialSolutions = DvrpProblem.PartialSolutions[problemId];
            SolveRequest problem = DvrpProblem.Problems[problemId];
            Solutions response = new Solutions();
            response.Id = problemId;
            response.ProblemType = problem.ProblemType;
            response.CommonData = problem.Data;
            response.Solutions1 = partialSolutions.ToArray();

            if (networkAdapter.Send(response))
                DvrpProblem.SolutionsMerging[taskId].Add(problemId);
        }
    }
}
