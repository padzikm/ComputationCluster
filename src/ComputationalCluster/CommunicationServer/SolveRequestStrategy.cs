using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Common;

namespace CommunicationServer
{
    class SolveRequestStrategy : IMessageStrategy
    {
        public void HandleMessage(System.IO.Stream stream, string message, Common.MessageType messageType, DateTime timeout, ref ulong id, out bool keepAlive, out System.Threading.AutoResetEvent waitEvent)
        {
            SolveRequest request = MessageSerialization.Deserialize<SolveRequest>(message);

            waitEvent = null;

            if (!request.ProblemType.ToLower().Contains("dvrp"))
            {
                id = 0;
                keepAlive = false;
                return;
            }

            keepAlive = true;
            id = DvrpProblem.CreateSaveID();
            DvrpProblem.Problems.Add(id, request);
            DvrpProblem.ProblemsDivideWaiting.Add(id, true);
            SolveRequestResponse response = new SolveRequestResponse() { Id = id };
            ServerNetworkAdapter.Send(stream, response);
            DvrpProblem.TaskEvent.Set();
        }

        public void HandleWaitEvent(System.IO.Stream stream, ulong id)
        {
            throw new NotImplementedException();
        }
    }
}
