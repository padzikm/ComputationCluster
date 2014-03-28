using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Common;

namespace CommunicationServer
{
    class StatusStrategy : IMessageStrategy
    {
        public void HandleMessage(System.IO.Stream stream, string message, MessageType messageType, TimeSpan timeout, EndPoint endPoint)
        {
            Status msg = MessageSerialization.Deserialize<Status>(message);            

            if (msg == null || !DvrpProblem.ComponentsID.Contains(msg.Id))
                return;

            DvrpProblem.WaitEvent.WaitOne();                        
            DvrpProblem.ComponentsLastStatus[msg.Id] = DateTime.UtcNow;
            DvrpProblem.WaitEvent.Set();
        }
    }
}
