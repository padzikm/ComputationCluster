using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Common;

namespace CommunicationServer
{
    class RegisterStrategy : IMessageStrategy
    {
        public void HandleMessage(System.IO.Stream stream, string message, Common.MessageType messageType, DateTime timeout, ref ulong id, out bool keepAlive, out System.Threading.AutoResetEvent waitEvent)
        {
            Register msg = MessageSerialization.Deserialize<Register>(message);
            
            if (msg.SolvableProblems.Where(p => p.ToLower().Contains("dvrp")).Count() == 0)
            {
                id = 0;
                keepAlive = false;
                waitEvent = null;
                return;
            }

            id = DvrpProblem.CreateSaveID();

            if (msg.Type == RegisterType.ComputationalNode)
            {
                DvrpProblem.Nodes.Add(id, msg);
                waitEvent = DvrpProblem.NodeEvent;
            }
            else
            {
                DvrpProblem.Tasks.Add(id, msg);
                waitEvent = DvrpProblem.TaskEvent;
            }

            DvrpProblem.ComponentsLastStatus.Add(id, DateTime.UtcNow);
            keepAlive = true;

            RegisterResponse reponse = new RegisterResponse() { Id = id, Timeout = timeout };
            ServerNetworkAdapter.Send(stream, reponse);
        }

        public void HandleWaitEvent(System.IO.Stream stream, ulong id)
        {
            throw new NotImplementedException();
        }
    }
}
