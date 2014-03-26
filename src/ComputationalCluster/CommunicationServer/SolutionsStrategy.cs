using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommunicationServer
{
    class SolutionsStrategy : IMessageStrategy
    {
        public void HandleMessage(System.IO.Stream stream, string message, Common.MessageType messageType, TimeSpan timout, System.Net.EndPoint endPoint)
        {
            throw new NotImplementedException();
        }
    }
}
