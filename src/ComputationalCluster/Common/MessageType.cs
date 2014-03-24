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
        SolvePartialProblemsMessage,
        SolutionRequestMessage,
        SolutionsMessage,
        UnknownMessage,
    }

    public class MessageTypeConverter
    {
        public static MessageType ConvertToMessageType(string message)
        {
            try
            {
                XDocument doc = XDocument.Parse(message);

                return (MessageType) Enum.Parse(typeof (MessageType), doc.Root.Name.LocalName + "Message");
            }
            catch
            {
                return MessageType.UnknownMessage;
            }
        }
    }
}
