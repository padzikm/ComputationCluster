using System;
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
    public class MessageSerialization
    {
        public static string Serialize<T>(T value) where T : class 
        {
            if (value == null)
                return null;

            try
            {
                XmlSerializer xmlSerializer = new XmlSerializer(typeof(T));
                StringWriter stringWriter = new StringWriter();
                
                xmlSerializer.Serialize(stringWriter, value);
                
                string serializedXML = stringWriter.ToString();                                
                
                return serializedXML;
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex.Message);
                return null;
            }
        }

        public static T Deserialize<T>(string value) where T : class
        {
            if (value == null)
                return null;

            XmlSerializer xmlSerializer = new XmlSerializer(typeof(T));
            StringReader stringReader = new StringReader(value);
            XmlReader reader = XmlReader.Create(stringReader);

            T result = (T)xmlSerializer.Deserialize(reader);

            return result;
        }

        public static byte[] GetBytes(string str)
        {
            return Encoding.UTF8.GetBytes(str);            
        }

        public static string GetString(byte[] bytes)
        {
            return Encoding.UTF8.GetString(bytes);            
        }
    }
}
