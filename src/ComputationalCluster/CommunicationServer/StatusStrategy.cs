using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Common;

namespace CommunicationServer
{
    class StatusStrategy : IMessageStrategy
    {
        public void HandleMessage(System.IO.Stream stream, string message, Common.MessageType messageType, DateTime timeout, ref ulong id, out bool keepAlive, out System.Threading.AutoResetEvent waitEvent)
        {
            Status msg = MessageSerialization.Deserialize<Status>(message);
            keepAlive = false;
            waitEvent = null;

            if (!DvrpProblem.IDList.Contains(msg.Id))
                return;
            
            DvrpProblem.ComponentsLastStatus[msg.Id] = DateTime.UtcNow;
            keepAlive = true;
        }

        public void HandleWaitEvent(System.IO.Stream stream, ulong id)
        {
            throw new NotImplementedException();
        }
    }
}
