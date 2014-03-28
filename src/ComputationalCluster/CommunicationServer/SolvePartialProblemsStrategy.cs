using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Common;

namespace CommunicationServer
{
    class SolvePartialProblemsStrategy : IMessageStrategy
    {
        /// <summary>
        /// Registers divided problem into smaller problems from task
        /// </summary>
        /// <param name="stream"></param>
        /// <param name="message"></param>
        /// <param name="messageType"></param>
        /// <param name="timout"></param>
        /// <param name="endPoint"></param>
        public void HandleMessage(System.IO.Stream stream, string message, Common.MessageType messageType, TimeSpan timout, System.Net.EndPoint endPoint)
        {
            SolvePartialProblems partial = MessageSerialization.Deserialize<SolvePartialProblems>(message);

            if (partial == null)
                return;

            DvrpProblem.WaitEvent.WaitOne();

            if(!DvrpProblem.PartialProblems.ContainsKey(partial.Id))
                DvrpProblem.PartialProblems.Add(partial.Id, new List<SolvePartialProblemsPartialProblem>());
            DvrpProblem.PartialProblems[partial.Id].AddRange(partial.PartialProblems);
            
            DvrpProblem.NodeEvent.Set();
            DvrpProblem.WaitEvent.Set();
        }
    }
}
