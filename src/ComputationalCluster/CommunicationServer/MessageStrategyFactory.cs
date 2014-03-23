using System.Collections.Generic;
using Common;

namespace CommunicationServer
{
    class MessageStrategyFactory
    {
        private static MessageStrategyFactory instance;
        private readonly Dictionary<MessageType, IMessageStrategy> messageStrategies;        

        public static MessageStrategyFactory Instance
        {
            get { return instance ?? (instance = new MessageStrategyFactory()); }
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
