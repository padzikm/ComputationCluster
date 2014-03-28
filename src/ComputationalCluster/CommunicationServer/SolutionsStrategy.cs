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

            if (sol == null)
                return;

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
            }
            DvrpProblem.WaitEvent.Set();
        }
    }
}
