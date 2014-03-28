using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Common;

namespace CommunicationServer
{
    class RegisterStrategy : IMessageStrategy
    {
        public void HandleMessage(System.IO.Stream stream, string message, MessageType messageType, TimeSpan timout, EndPoint endPoint)
        {
            Register msg = MessageSerialization.Deserialize<Register>(message);
            
            if (msg == null || msg.SolvableProblems.Where(p => p.ToLower().Contains("dvrp")).Count() == 0)            
                return;

            DvrpProblem.WaitEvent.WaitOne();
            ulong id = DvrpProblem.CreateSaveComponentID();

            if (msg.Type == RegisterType.ComputationalNode)
            {
                DvrpProblem.Nodes.Add(id, msg);
                //if (DvrpProblem.PartialProblems.Count > 0)
                    DvrpProblem.NodeEvent.Set();
            }
            else
            {
                DvrpProblem.Tasks.Add(id, msg);
                //if (DvrpProblem.ProblemsDivideWaiting.Count > 0)
                    DvrpProblem.TaskDivideEvent.Set();
                //if (DvrpProblem.ProblemsMergeWaiting.Count > 0)
                    DvrpProblem.TaskMergeEvent.Set();
            }

            DvrpProblem.ComponentsLastStatus.Add(id, DateTime.UtcNow);  
            DvrpProblem.ComponentsAddress.Add(id, endPoint);

            RegisterResponse reponse = new RegisterResponse() { Id = id, };
            ServerNetworkAdapter.Send(stream, reponse);            
            DvrpProblem.WaitEvent.Set();
        }
    }
}
