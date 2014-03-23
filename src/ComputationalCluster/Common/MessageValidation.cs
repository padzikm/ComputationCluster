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
using System.Reflection;
using System.Xml.Serialization;


namespace Common
{   
    public class MessageValidation
    {
        private static string assemblyNamespaces = "CommunicationProtocolLibrary.XMLSchemas.";
        private static string schemaNamespace = @"http://www.mini.pw.edu.pl/ucc/";

        private static string ConvertMessageTypeToSchemaName(MessageType msgType)
        {            
            switch (msgType)
            {
                case MessageType.DivideProblemMessage: return assemblyNamespaces + "DivideProblemMessage.xsd";
                case MessageType.PartialProblemsMessage: return assemblyNamespaces + "PartialProblemsMessage.xsd";
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
