using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Common;

namespace CommunicationServer
{
    interface IMessageStrategy
    {
        void HandleMessage(Stream stream, string message, MessageType messageType, DateTime timeout, ref ulong id, out bool keepAlive, out AutoResetEvent waitEvent);
        void HandleWaitEvent(Stream stream, ulong id);
    }
}
