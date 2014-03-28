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
        /// <summary>
        /// Handle messages from client
        /// </summary>
        /// <param name="stream">Opened stream</param>
        /// <param name="message">XML message</param>
        /// <param name="messageType">XML's message type</param>
        /// <param name="timout">Timeout for keepalive status</param>
        /// <param name="endPoint">Client's network connection details</param>
        void HandleMessage(Stream stream, string message, MessageType messageType, TimeSpan timout, EndPoint endPoint);        
    }
}
