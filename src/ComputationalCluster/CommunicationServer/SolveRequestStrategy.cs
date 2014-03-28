using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Common;

namespace CommunicationServer
{
    class SolveRequestStrategy : IMessageStrategy
    {
        public void HandleMessage(System.IO.Stream stream, string message, MessageType messageType, TimeSpan timeout, EndPoint endPoint)
        {
            SolveRequest request = MessageSerialization.Deserialize<SolveRequest>(message);            

            if (!request.ProblemType.ToLower().Contains("dvrp"))            
                return;
            
            DvrpProblem.WaitEvent.WaitOne();
            ulong id = DvrpProblem.CreateSaveProblemID();
            DvrpProblem.Problems.Add(id, request);
            DvrpProblem.ProblemsDivideWaiting.Add(id, true);
            SolveRequestResponse response = new SolveRequestResponse() { Id = id };
            ServerNetworkAdapter.Send(stream, response);
            DvrpProblem.TaskMergeEvent.Set();            
            DvrpProblem.WaitEvent.Set();
        }        
    }
}
