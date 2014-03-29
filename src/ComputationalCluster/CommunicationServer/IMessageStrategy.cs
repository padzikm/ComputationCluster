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
    public interface IMessageStrategy
    {
        /// <summary>
        /// Handle messages from client
        /// </summary>
        /// <param name="networkAdapter">Adapter with opened stream</param>
        /// <param name="message">XML message</param>
        /// <param name="messageType">XML's message type</param>
        /// <param name="timout">Timeout for keepalive status</param>        
        void HandleMessage(ServerNetworkAdapter networkAdapter, string message, MessageType messageType, TimeSpan timout);        
    }
}
