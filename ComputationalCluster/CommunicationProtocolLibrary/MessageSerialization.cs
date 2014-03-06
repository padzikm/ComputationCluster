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

namespace CommunicationProtocolLibrary
{
    public class MessageSerialization
    {
        public static string Serialize<T>(T value)
        {
            string serializedXML;

            if (value == null)
                return null;

            try
            {
                XmlSerializer xmlSerializer = new XmlSerializer(typeof(T));
                StringWriter stringWriter = new StringWriter();
                XmlWriter writer = XmlWriter.Create(stringWriter);

                xmlSerializer.Serialize(writer, value);

                serializedXML = stringWriter.ToString();

                writer.Close();
                
                return serializedXML;
            }
            catch
            {
                return null;
            }
        }

        public static T Deserialize<T>(string value) where T : class
        {
            if (value == null)
                return null;

            try
            {

                XmlSerializer xmlSerializer = new XmlSerializer(typeof(T));
                StringReader stringReader = new StringReader(value);
                XmlReader reader = XmlReader.Create(stringReader);

                T result = (T)xmlSerializer.Deserialize(reader);

                return result;
            }
            catch
            {
                return null;
            }
        }        
    }
}
