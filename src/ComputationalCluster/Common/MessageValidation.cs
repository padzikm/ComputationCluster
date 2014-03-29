using System.Collections.Generic;
using System.IO;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Linq;
using System.Reflection;


namespace Common
{   
    public class MessageValidation
    {
        private static readonly string assemblyNamespaces = Assembly.GetExecutingAssembly().GetName().Name + ".XMLSchemas.";
        private const string schemaNamespace = @"http://www.mini.pw.edu.pl/ucc/";

        private static string ConvertMessageTypeToSchemaName(MessageType msgType)
        {            
            switch (msgType)
            {
                case MessageType.DivideProblemMessage: return assemblyNamespaces + "DivideProblemMessage.xsd";
                case MessageType.SolvePartialProblemsMessage: return assemblyNamespaces + "SolvePartialProblemsMessage.xsd";
                case MessageType.RegisterMessage: return assemblyNamespaces + "RegisterMessage.xsd";
                case MessageType.RegisterResponseMessage: return assemblyNamespaces + "RegisterResponseMessage.xsd";
                case MessageType.SolutionRequestMessage: return assemblyNamespaces + "SolutionRequestMessage.xsd";
                case MessageType.SolutionsMessage: return assemblyNamespaces + "SolutionsMessage.xsd";
                case MessageType.SolveRequestMessage: return assemblyNamespaces + "SolveRequestMessage.xsd";
                case MessageType.SolveRequestResponseMessage: return assemblyNamespaces + "SolveRequestResponseMessage.xsd";
                case MessageType.StatusMessage: return assemblyNamespaces + "StartKeepAlive.xsd";
                default: return null;
            }
        }

        /// <summary>
        /// Validates enum type of message with correct XDocument of correct message.
        /// </summary>
        /// <param name="messageType"> Enum message type to validate. </param>
        /// <param name="message"> Correct Message to compare with first input parameter. </param>
        /// <returns> True if message is valid, false otherwise. </returns>
        public static bool IsMessageValid(MessageType messageType, XDocument message)
        {                                              
            List<string> errorList = new List<string>();
            bool isValid = true;

            string schemaFullName = ConvertMessageTypeToSchemaName(messageType);

            Assembly assembly = Assembly.GetExecutingAssembly();
            Stream stream = assembly.GetManifestResourceStream(schemaFullName);
            XmlReader schemaReader = XmlReader.Create(stream);  

            XmlSchemaSet schemas = new XmlSchemaSet();
            schemas.Add(schemaNamespace, schemaReader);            

            message.Validate(schemas, (o, e) =>
            {
                errorList.Add(e.Message);
                isValid = false;
            });            

            return isValid;
        }

        /// <summary>
        /// Validates enum type of message with string to compare.
        /// </summary>
        /// <param name="messageType"> Enum message type to validate. </param>
        /// <param name="message"> Correct string message to compare with first input parameter. </param>
        /// <returns></returns>
        public static bool IsMessageValid(MessageType messageType, string message)
        {
            try
            {
                XDocument xmlDoc = XDocument.Parse(message);
                return IsMessageValid(messageType, xmlDoc);
            }
            catch
            {
                return false;
            }            
        }        
    }
}
