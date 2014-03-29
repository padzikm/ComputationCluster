using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Common;

namespace CommunicationServer
{
    public class RegisterStrategy : IMessageStrategy
    {
        /// <summary>
        /// Register new component in problem instance
        /// </summary>
        /// <param name="networkAdapter"></param>
        /// <param name="message"></param>
        /// <param name="messageType"></param>
        /// <param name="timout"></param>        
        public void HandleMessage(ServerNetworkAdapter networkAdapter, string message, MessageType messageType, TimeSpan timout)
        {
            Register msg = MessageSerialization.Deserialize<Register>(message);
            
            if (msg == null || msg.SolvableProblems == null)// || msg.SolvableProblems.Where(p => p.ToLower().Contains("dvrp")).Count() == 0)            
                return;

            DvrpProblem.WaitEvent.WaitOne();
            ulong id = DvrpProblem.CreateSaveComponentID();

            if (msg.Type == RegisterType.ComputationalNode)            
                DvrpProblem.Nodes.Add(id, msg);                            
            else            
                DvrpProblem.Tasks.Add(id, msg);                                                

            DvrpProblem.ComponentsLastStatus.Add(id, DateTime.UtcNow);              
            RegisterResponse reponse = new RegisterResponse() { Id = id, Timeout = timout.ToString()};
            networkAdapter.Send(reponse);            
            DvrpProblem.WaitEvent.Set();
        }
    }
}
