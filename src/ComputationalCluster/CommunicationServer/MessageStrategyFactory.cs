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
        private IMessageStrategy nodeWaitEventStrategy;
        private IMessageStrategy taskWaitEventStrategy;

        public static MessageStrategyFactory Instance
        {
            get { return instance ?? (instance = new MessageStrategyFactory()); }
        }

        private MessageStrategyFactory()
        {
            nodeWaitEventStrategy = null;
            taskWaitEventStrategy = null;

            messageStrategies = new Dictionary<MessageType, IMessageStrategy>();
            messageStrategies.Add(MessageType.RegisterMessage, new RegisterStrategy());
            messageStrategies.Add(MessageType.SolveRequestMessage, new SolveRequestStrategy());
            messageStrategies.Add(MessageType.SolutionRequestMessage, new SolutionRequestStrategy());
            messageStrategies.Add(MessageType.StatusMessage, new StatusStrategy());
            messageStrategies.Add(MessageType.UnknownMessage, new UnknownStatusStrategy());
        }

        public IMessageStrategy GetMessageStrategy(MessageType msgType, DateTime timeout, ulong id)
        {
            TimeSpan time, delay;
            TimeSpan span = new TimeSpan(timeout.Hour, timeout.Minute, timeout.Second);
            bool keepAlive = false;    
            delay = new TimeSpan(0, 1, 0);
            
            time = DateTime.UtcNow - DvrpProblem.ComponentsLastStatus[id];
            if (time < span + delay)            
                keepAlive = true;            

            return (keepAlive && messageStrategies.ContainsKey(msgType)) ? messageStrategies[msgType] : messageStrategies[MessageType.UnknownMessage];
        }

        public IMessageStrategy GetWaitEventStrategy(ulong id)
        {
            return DvrpProblem.Nodes.ContainsKey(id) ? nodeWaitEventStrategy : taskWaitEventStrategy;
        }        
    }
}
