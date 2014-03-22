using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace Common
{
    public enum MessageType
    {
        RegisterMessage,
        RegisterResponseMessage,
        SolveRequestMessage,
        SolveRequestResponseMessage,
        StatusMessage,
        DivideProblemMessage,
        PartialProblemsMessage,
        SolutionRequestMessage,
        SolutionsMessage,
    }

    public class MessageTypeConverter
    {
        public static MessageType ConvertToMessageType(string message)
        {
            XDocument doc = XDocument.Parse(message);

            return (MessageType)Enum.Parse(typeof(MessageType), doc.Root.Name.LocalName + "Message");
        }
    }
}
