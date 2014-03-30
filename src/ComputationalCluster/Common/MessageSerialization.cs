using System;
using System.Text;
using System.IO;
using System.Xml;
using System.Xml.Serialization;

namespace Common
{
    public class MessageSerialization
    {
        /// <summary>
        /// Serializes specific class (generated from xml respectively) to string including adding header "ucc", and coding utf-8
        /// </summary>
        /// <typeparam name="T"> Type of xml generated class. </typeparam>
        /// <param name="value"> Message to sent - class generated from xml schema. </param>
        /// <returns> Correct string value adequate to input parameter. </returns>
        public static string Serialize<T>(T value) where T : class 
        {
            if (value == null)
                return null;

            try
            {
                XmlSerializer xmlSerializer = new XmlSerializer(typeof(T));
                StringWriter stringWriter = new StringWriter();
                                
                XmlSerializerNamespaces ns = new XmlSerializerNamespaces();
                ns.Add("ucc", "http://www.mini.pw.edu.pl/ucc/");

                xmlSerializer.Serialize(stringWriter, value, ns);

                string serializedXML = stringWriter.ToString().Replace("utf-16", "utf-8");

                MessageType msgType = MessageTypeConverter.ConvertToMessageType(serializedXML);
                if(!MessageValidation.IsMessageValid(msgType, serializedXML))
                    return null;

                return serializedXML;
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex.Message);
                return null;
            }
        }

        /// <summary>
        /// Deserializes string to specific class (generated from xml respectively)
        /// </summary>
        /// <typeparam name="T"> Type of xml generated class. </typeparam>
        /// <param name="value"> String message to convert. </param>
        /// <returns> Deserialized string as a specific class adequate to xml respectively. </returns>
        public static T Deserialize<T>(string value) where T : class
        {
            if (value == null)
                return null;
            try
            {
                MessageType msgType = MessageTypeConverter.ConvertToMessageType(value);
                if (!MessageValidation.IsMessageValid(msgType, value))
                    return null;

                XmlSerializer xmlSerializer = new XmlSerializer(typeof (T));
                StringReader stringReader = new StringReader(value);
                XmlReader reader = XmlReader.Create(stringReader);

                T result = (T) xmlSerializer.Deserialize(reader);

                return result;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return null;
            }
        }

        /// <summary>
        /// Converts string to byte array using utf-8 encoding
        /// </summary>
        /// <param name="str"> String to convert. </param>
        /// <returns> Byte array converted from input string parameter. </returns>
        public static byte[] GetBytes(string str)
        {
            return Encoding.UTF8.GetBytes(str);            
        }

        /// <summary>
        /// Converts byte array to string using utf-8 encoding
        /// </summary>
        /// <param name="bytes"> Byte array to convert. </param>
        /// <returns> String converted from byte array. </returns>
        public static string GetString(byte[] bytes)
        {
            return Encoding.UTF8.GetString(bytes);            
        }
    }
}
