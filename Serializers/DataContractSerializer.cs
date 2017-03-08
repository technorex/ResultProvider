using System;
using System.IO;
using System.Text;
using System.Xml;

namespace TechnoRex.ResultProvider.Serializers
{
    public static class DataContractSerializer
    {
        public static string Serialize(object obj)
        {
            using (MemoryStream memoryStream = new MemoryStream())
            {
                System.Runtime.Serialization.DataContractSerializer serializer = new System.Runtime.Serialization.DataContractSerializer(obj.GetType());
                serializer.WriteObject(memoryStream, obj);
                return Encoding.UTF8.GetString(memoryStream.ToArray());
            }
        }
        public static object Deserialize(string xml, Type toType)
        {
            using (MemoryStream memoryStream = new MemoryStream(Encoding.UTF8.GetBytes(xml)))
            {
                XmlDictionaryReader reader = XmlDictionaryReader.CreateTextReader(memoryStream, Encoding.UTF8, new XmlDictionaryReaderQuotas(), null);
                System.Runtime.Serialization.DataContractSerializer serializer = new System.Runtime.Serialization.DataContractSerializer(toType);
                return serializer.ReadObject(reader);
            }
        }
    }
}