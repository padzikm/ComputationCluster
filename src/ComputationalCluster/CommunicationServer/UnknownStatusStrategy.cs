using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommunicationServer
{
    class UnknownStatusStrategy : IMessageStrategy
    {
        public void HandleMessage(System.IO.Stream stream, string message, Common.MessageType messageType, DateTime timeout, ref ulong id, out bool keepAlive, out System.Threading.AutoResetEvent waitEvent)
        {
            TimeSpan time, delay;
            TimeSpan span = new TimeSpan(timeout.Hour, timeout.Minute, timeout.Second);
            delay = new TimeSpan(0, 1, 0);
            keepAlive = false;
            waitEvent = null;

            if (!DvrpProblem.IDList.Contains(id))
                return;
            time = DateTime.UtcNow - DvrpProblem.ComponentsLastStatus[id];
            if (time > span + delay)
            {
                DvrpProblem.ComponentsLastStatus.Remove(id);
                DvrpProblem.IDList.Remove(id);
                return;
            }
            keepAlive = true;
        }

        public void HandleWaitEvent(System.IO.Stream stream, ulong id)
        {
            throw new NotImplementedException();
        }
    }
}
