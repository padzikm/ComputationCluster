using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Common;

namespace CommunicationServer
{
    class SolutionsStrategy : IMessageStrategy
    {
        public void HandleMessage(System.IO.Stream stream, string message, Common.MessageType messageType, TimeSpan timout, System.Net.EndPoint endPoint)
        {
            Solutions sol = MessageSerialization.Deserialize<Solutions>(message);

            DvrpProblem.WaitEvent.WaitOne();

            if (sol.Solutions1.First().Type == SolutionsSolutionType.Final)
            {
                if (!DvrpProblem.ProblemSolutions.ContainsKey(sol.Id))
                    DvrpProblem.ProblemSolutions.Add(sol.Id, sol);
            }
            else
            {
                if (!DvrpProblem.PartialSolutions.ContainsKey(sol.Id))
                    DvrpProblem.PartialSolutions.Add(sol.Id, new List<SolutionsSolution>());

                DvrpProblem.PartialSolutions[sol.Id].AddRange(sol.Solutions1);
                DvrpProblem.TaskMergeEvent.Set();
            }
            DvrpProblem.WaitEvent.Set();
        }
    }
}
