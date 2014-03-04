using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommunicationProtocolLibrary;
using System.Xml.Linq;

namespace CommunicationServer
{
    class Program
    {        
        static void Main(string[] args)
        {
            string xml = "";

            XDocument doc1 = new XDocument(
                new XElement("Register",
                    new XElement("Child1", "content1"),
                    new XElement("Child3", "content1")
                    //new XElement("Child2", "content1")
                )
            );

            XNamespace ns = @"http://www.mini.pw.edu.pl/ucc/";
            var result = new XDocument(
                new XElement(ns + "RegisterResponse",
                    new XElement(ns + "Id",
                        new XText("34")
                     ),
                     new XElement(ns + "Timeout",
                        new XText("23:56:45")
                     )
                 )
             );

            //IEnumerable<string> list = MessageValidation.IsMessageValid(MessageType.RegisterResponseMessage, result);

            //if (list.Count() == 0)
            //    Console.WriteLine("no errors!");
            //else
            //{
            //    Console.WriteLine("errors!");
            //    foreach (string error in list)
            //        Console.WriteLine(error);
            //}
        }
    }
}
