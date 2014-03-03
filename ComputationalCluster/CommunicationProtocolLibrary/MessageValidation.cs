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
        private static string schemaNamespace = "http://www.mini.pw.edu.pl/ucc/";

        private static string ConvertMessageTypeToSchemaPath(MessageType msgType)
        {
            string catalogPath = "/XMLSchemas/";

            switch(msgType)
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

        public static IEnumerable<string> ValidateMessage(MessageType messageType, XDocument message)
        {
            List<string> errorList = new List<string>();

            string schemaPath = ConvertMessageTypeToSchemaPath(messageType);            
            
            XmlSchemaSet schemas = new XmlSchemaSet();
            schemas.Add(schemaNamespace, schemaPath);            

            message.Validate(schemas, (o, e) =>
            {                
                errorList.Add(e.Message);
                
            });            

            return errorList;
        }

        public static IEnumerable<string> ValidateMessage(MessageType messageType, string message)
        {
            XDocument xmlDoc = XDocument.Parse(message);

            return ValidateMessage(messageType, xmlDoc);
        }
    }
}
