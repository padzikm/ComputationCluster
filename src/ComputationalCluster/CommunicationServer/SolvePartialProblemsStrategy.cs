using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Common;

namespace CommunicationServer
{
    public class SolvePartialProblemsStrategy : IMessageStrategy
    {
        /// <summary>
        /// Registers divided problem into smaller problems from task
        /// </summary>
        /// <param name="networkAdapter"></param>
        /// <param name="message"></param>
        /// <param name="messageType"></param>
        /// <param name="timout"></param>        
        public void HandleMessage(ServerNetworkAdapter networkAdapter, string message, Common.MessageType messageType, TimeSpan timout)
        {
            SolvePartialProblems partial = MessageSerialization.Deserialize<SolvePartialProblems>(message);

            if (partial == null)
                return;

            DvrpProblem.WaitEvent.WaitOne();

            if(!DvrpProblem.PartialProblems.ContainsKey(partial.Id))
                DvrpProblem.PartialProblems.Add(partial.Id, new List<SolvePartialProblemsPartialProblem>());
            DvrpProblem.PartialProblems[partial.Id].AddRange(partial.PartialProblems);
                        
            DvrpProblem.WaitEvent.Set();
        }
    }
}
