using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
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
            get { return instance ?? (instance = new MessageStrategyFactory()); }
        }

        private MessageStrategyFactory()
        {            
            messageStrategies = new Dictionary<MessageType, IMessageStrategy>();
            messageStrategies.Add(MessageType.RegisterMessage, new RegisterStrategy());
            messageStrategies.Add(MessageType.SolveRequestMessage, new SolveRequestStrategy());
            messageStrategies.Add(MessageType.SolutionRequestMessage, new SolutionRequestStrategy());
            messageStrategies.Add(MessageType.StatusMessage, new StatusStrategy());          
            messageStrategies.Add(MessageType.SolutionsMessage, new SolutionsStrategy());
            messageStrategies.Add(MessageType.SolvePartialProblemsMessage, new SolvePartialProblemsStrategy());
        }

        public IMessageStrategy GetMessageStrategy(MessageType msgType)
        {            
            return msgType != MessageType.UnknownMessage ? messageStrategies[msgType] : null;
        }
    }
}
