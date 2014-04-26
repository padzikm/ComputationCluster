using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;

namespace DvrpUtils
{
    public static class DataSerialization
    {
        public static byte[] BinarySerializeObject(object objectToSerialize)
        {
            if (objectToSerialize == null)
                throw new ArgumentNullException("objectToSerialize");

            byte[] serializedObject;

            using (MemoryStream stream = new MemoryStream())
            {
                BinaryFormatter formatter = new BinaryFormatter();
                formatter.Serialize(stream, objectToSerialize);
                serializedObject = stream.ToArray();
            }

            return serializedObject;
        }

        public static T BinaryDeserializeObject<T>(byte[] serializedType)
        {
            if (serializedType == null)
                throw new ArgumentNullException("serializedType");

            if (serializedType.Length.Equals(0))
                throw new ArgumentException("serializedType");

            T deserializedObject;

            using (MemoryStream memoryStream = new MemoryStream(serializedType))
            {
                BinaryFormatter deserializer = new BinaryFormatter();
                deserializedObject = (T)deserializer.Deserialize(memoryStream);
            }

            return deserializedObject;
        }
    }
}
