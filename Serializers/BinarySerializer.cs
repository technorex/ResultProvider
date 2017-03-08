using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

namespace TechnoRex.ResultProvider.Serializers
{
    public static class BinarySerializer
    {
        public static string Serialize(object obj)
        {
            if (!obj.GetType().IsSerializable)
            {
                return null;
            }

            using (MemoryStream stream = new MemoryStream())
            {
                new BinaryFormatter().Serialize(stream, obj);
                return Convert.ToBase64String(stream.ToArray());
            }
        }
        public static object Deserialize(string str)
        {
            byte[] bytes = Convert.FromBase64String(str);

            using (MemoryStream stream = new MemoryStream(bytes))
            {
                return new BinaryFormatter().Deserialize(stream);
            }
        }

    }
}