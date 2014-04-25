using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Common;

namespace CommunicationServer
{
    public class SolutionsStrategy : IMessageStrategy
    {
        /// <summary>
        /// Registers new solutions from either node or task
        /// </summary>
        /// <param name="networkAdapter"></param>
        /// <param name="message"></param>
        /// <param name="messageType"></param>
        /// <param name="timout"></param>        
        public void HandleMessage(ServerNetworkAdapter networkAdapter, string message, Common.MessageType messageType, TimeSpan timout)
        {
            Solutions sol = MessageSerialization.Deserialize<Solutions>(message);

            DvrpProblem.WaitEvent.WaitOne();

            if (sol == null || sol.Solutions1 == null || !DvrpProblem.Problems.ContainsKey(sol.Id))
            {
                DvrpProblem.WaitEvent.Set();
                return;
            }


            if (sol.Solutions1.First().Type == SolutionsSolutionType.Final)
            {
                if (!DvrpProblem.ProblemSolutions.ContainsKey(sol.Id))
                    DvrpProblem.ProblemSolutions.Add(sol.Id, sol);

                DvrpProblem.PartialSolutions.Remove(sol.Id);

                foreach (var list in DvrpProblem.SolutionsMerging.Values)
                    if (list.Remove(sol.Id))
                        break;
            }
            else
            {
                if (!DvrpProblem.PartialSolutions.ContainsKey(sol.Id))
                    DvrpProblem.PartialSolutions.Add(sol.Id, new List<SolutionsSolution>());

                DvrpProblem.PartialSolutions[sol.Id].AddRange(sol.Solutions1);

                foreach (var solution in sol.Solutions1)
                    DvrpProblem.PartialProblems[sol.Id].RemoveAll(p => p.Key.TaskId == solution.TaskId);

                if (DvrpProblem.PartialProblems[sol.Id].Count == 0)
                    DvrpProblem.PartialProblems.Remove(sol.Id);
            }
            DvrpProblem.WaitEvent.Set();
        }
    }
}
