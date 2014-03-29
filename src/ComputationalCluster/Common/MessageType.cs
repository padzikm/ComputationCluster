using System;
using System.Xml.Linq;

namespace Common
{
    /// <summary>
    /// Enums of messages type present in xml schema.
    /// </summary>
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

    public static class MessageTypeConverter
    {
        /// <summary>
        /// Converts string to MessageType enum type
        /// </summary>
        /// <param name="message"> String to convert. </param>
        /// <returns> Enum type MessageType converted from string. </returns>
        public static MessageType ConvertToMessageType(string message)
        {
            try
            {
                XDocument doc = XDocument.Parse(message);

                return (MessageType) Enum.Parse(typeof (MessageType), doc.Root.Name.LocalName + "Message");
            }
            catch (Exception ex)
            {
                Console.WriteLine("Blad parsera: {0}", ex.Message);
                return MessageType.UnknownMessage;
            }
        }
    }
}
