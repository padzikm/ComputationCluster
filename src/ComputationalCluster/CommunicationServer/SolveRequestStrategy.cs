using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Common;

namespace CommunicationServer
{
    public class SolveRequestStrategy : IMessageStrategy
    {
        /// <summary>
        /// Register new problem from client
        /// </summary>
        /// <param name="networkAdapter"></param>
        /// <param name="message"></param>
        /// <param name="messageType"></param>
        /// <param name="timeout"></param>        
        public void HandleMessage(ServerNetworkAdapter networkAdapter, string message, MessageType messageType, TimeSpan timeout)
        {
            SolveRequest request = MessageSerialization.Deserialize<SolveRequest>(message);            

            if (request == null || request.Data == null)// || !request.ProblemType.ToLower().Contains("dvrp"))            
                return;
            
            DvrpProblem.WaitEvent.WaitOne();
            ulong id = DvrpProblem.CreateProblemID();
            DvrpProblem.ProblemsID.Add(id);
            DvrpProblem.Problems.Add(id, request);            
            SolveRequestResponse response = new SolveRequestResponse() { Id = id };
            networkAdapter.Send(response);                      
            DvrpProblem.WaitEvent.Set();
        }        
    }
}
