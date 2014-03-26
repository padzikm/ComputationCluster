using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Common;

namespace CommunicationServer
{
    interface IMessageStrategy
    {
        void HandleMessage(Stream stream, string message, MessageType messageType, TimeSpan timout, EndPoint endPoint);        
    }
}
