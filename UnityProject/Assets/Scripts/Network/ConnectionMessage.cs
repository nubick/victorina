using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

namespace Victorina
{
    [Serializable]
    public class ConnectionMessage
    {
        public string Guid { get; set; }
        public string Name { get; set; }

        public byte[] ToBytes()
        {
            BinaryFormatter binaryFormatter = new BinaryFormatter();
            using MemoryStream memoryStream = new MemoryStream();
            binaryFormatter.Serialize(memoryStream, this);
            return memoryStream.ToArray();
        }

        public static ConnectionMessage FromBytes(byte[] bytes)
        {
            BinaryFormatter binaryFormatter = new BinaryFormatter();
            using MemoryStream memoryStream = new MemoryStream(bytes);
            return (ConnectionMessage) binaryFormatter.Deserialize(memoryStream);
        }
    }
}