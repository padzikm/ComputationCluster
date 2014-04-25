using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Common;

namespace CommunicationServer
{
    public class StatusStrategy : IMessageStrategy
    {
        /// <summary>
        /// Keeps alive component, which sent status message
        /// </summary>
        /// <param name="networkAdapter"></param>
        /// <param name="message"></param>
        /// <param name="messageType"></param>
        /// <param name="timeout"></param>        
        public void HandleMessage(ServerNetworkAdapter networkAdapter, string message, MessageType messageType, TimeSpan timeout)
        {
            Status msg = MessageSerialization.Deserialize<Status>(message);            

            if (msg == null)
                return;

            DvrpProblem.WaitEvent.WaitOne();
            if (!DvrpProblem.ComponentsID.Contains(msg.Id))
            {
                DvrpProblem.WaitEvent.Set();
                return;
            }
            DvrpProblem.ComponentsLastStatus[msg.Id] = DateTime.UtcNow;
            if(DvrpProblem.Nodes.ContainsKey(msg.Id))
                NodeWorker.Work(msg.Id, networkAdapter);
            else if (DvrpProblem.Tasks.ContainsKey(msg.Id))
            {
                TaskMergeWorker.Work(msg.Id, networkAdapter);
                TaskDivideWorker.Work(msg.Id, networkAdapter);                
            }
            DvrpProblem.WaitEvent.Set();
        }
    }
}
