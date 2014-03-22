using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommunicationServer
{
    interface IMessageStrategy
    {
        void HandleMessage(Stream stream, string message);
    }
}
