using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Common;

namespace CommunicationServer
{
    class MessageStrategyFactory
    {
        private static MessageStrategyFactory instance;
        private Dictionary<MessageType, IMessageStrategy> messageStrategies;        

        public static MessageStrategyFactory Instance
        {
            get
            {
                if (instance == null)
                    instance = new MessageStrategyFactory();

                return instance;
            }
        }

        private MessageStrategyFactory()
        {
            messageStrategies = new Dictionary<MessageType, IMessageStrategy>();

            //messageStrategies.Add(MessageType.DivideProblemMessage, );
        }

        public IMessageStrategy GetMessageStrategy(MessageType msgType)
        {
            return messageStrategies.ContainsKey(msgType) ? messageStrategies[msgType] : null;
        }
    }
}
