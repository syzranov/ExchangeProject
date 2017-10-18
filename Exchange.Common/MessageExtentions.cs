using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

namespace Exchange.Common
{
    public static class MessageExtentions
    {
        public static Message Serialize(this object obj)
        {
            using (var ms = new MemoryStream())
            {
                (new BinaryFormatter()).Serialize(ms, obj);
                return new Message { Data = ms.ToArray() };
            }
        }

        public static object Deserialize(this Message msg)
        {
            using (var ms = new MemoryStream(msg.Data))
                return (new BinaryFormatter()).Deserialize(ms);
        }
    }
}