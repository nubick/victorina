using System;
using System.Text;
using UnityEngine;

namespace Victorina
{
    [Serializable]
    public class ConnectionMessage
    {
        public string Guid;
        public string Name;
        public string ClientVersion;

        public byte[] ToBytes()
        {
            Debug.Log($"ConnectionMessage to bytes, Guid: {Guid}, Name: {Name}, ClientVersion: {ClientVersion}");
            string json = JsonUtility.ToJson(this);
            return Encoding.Unicode.GetBytes(json);
        }

        public static ConnectionMessage FromBytes(byte[] bytes)
        {
            string json = Encoding.Unicode.GetString(bytes);
            Debug.Log($"ConnectionMessage:'{json}'");
            return JsonUtility.FromJson<ConnectionMessage>(json);
        }
    }
}