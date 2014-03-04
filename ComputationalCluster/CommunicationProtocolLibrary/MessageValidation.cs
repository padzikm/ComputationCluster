using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Linq;
using System.Resources;

namespace CommunicationProtocolLibrary
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
        SolutionsMessage
    }

    public class MessageValidation
    {
        private static string schemaNamespace = @"http://www.mini.pw.edu.pl/ucc/";

        private static string ConvertMessageTypeToSchemaPath(MessageType msgType)
        {
            string catalogPath = "XMLSchemas/";

            switch (msgType)
            {
                case MessageType.DivideProblemMessage: return catalogPath + "DivideProblemMessage.xsd";
                case MessageType.PartialProblemsMessage: return catalogPath + "PartialProblemsMessage.xsd";
                case MessageType.RegisterMessage: return catalogPath + "RegisterMessage.xsd";
                case MessageType.RegisterResponseMessage: return catalogPath + "RegisterResponseMessage.xsd";
                case MessageType.SolutionRequestMessage: return catalogPath + "SolutionRequestMessage.xsd";
                case MessageType.SolutionsMessage: return catalogPath + "SolutionsMessage.xsd";
                case MessageType.SolveRequestMessage: return catalogPath + "SolveRequestMessage.xsd";
                case MessageType.SolveRequestResponseMessage: return catalogPath + "SolveRequestResponseMessage.xsd";
                case MessageType.StatusMessage: return catalogPath + "StatusMessage.xsd";
                default: return null;
            }
        }

        public static bool IsMessageValid(MessageType messageType, XDocument message)
        {
            List<string> errorList = new List<string>();
            bool isValid = true;

            string schemaPath = ConvertMessageTypeToSchemaPath(messageType);

            XmlSchemaSet schemas = new XmlSchemaSet();
            schemas.Add(schemaNamespace, schemaPath);

            message.Validate(schemas, (o, e) =>
            {
                errorList.Add(e.Message);
                isValid = false;
            });            

            return isValid;
        }

        public static bool IsMessageValid(MessageType messageType, string message)
        {
            XDocument xmlDoc = XDocument.Parse(message);

            return IsMessageValid(messageType, xmlDoc);
        }
    }
}
