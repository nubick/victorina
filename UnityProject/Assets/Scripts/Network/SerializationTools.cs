using System;
using System.IO;
using System.Linq;
using MLAPI.Serialization.Pooled;

namespace Victorina
{
    public static class SerializationTools
    {
        public static void SerializeBytesArray(Stream stream, byte[] bytes)
        {
            using PooledBitWriter writer = PooledBitWriter.Get(stream);
            writer.WriteInt32(bytes.Length);
            stream.Write(bytes, 0, bytes.Length);
        }
        
        public static byte[] DeserializeBytesArray(Stream stream)
        {
            using PooledBitReader reader = PooledBitReader.Get(stream);
            int length = reader.ReadInt32();
            byte[] bytes = new byte[length];
            stream.Read(bytes, 0, length);
            return bytes;
        }
        
        public static void SerializeIntsArray(PooledBitWriter writer, int[] ints)
        {
            writer.WriteInt32(ints.Length);
            foreach(int intVal in ints)
                writer.WriteInt32(intVal);
        }

        public static int[] DeserializeIntsArray(PooledBitReader reader)
        {
            int size = reader.ReadInt32();
            int[] array = new int[size];
            for (int i = 0; i < size; i++)
                array[i] = reader.ReadInt32();
            return array;
        }

        public static void SerializeEnumArray<T>(PooledBitWriter writer, T[] enums) where T : Enum
        {
            writer.WriteInt32(enums.Length);
            foreach (T enumItem in enums)
                writer.WriteInt32(Convert.ToInt32(enumItem));
        }

        public static T[] DeserializeEnumArray<T>(PooledBitReader reader) where T : Enum
        {
            int size = reader.ReadInt32();
            T[] enums = new T[size];
            for (int i = 0; i < size; i++)
            {
                int intValue = reader.ReadInt32();
                enums[i] = (T) Enum.ToObject(typeof(T), intValue);
            }
            return enums;
        }

        public static void SerializeBooleansArray(PooledBitWriter writer, bool[] booleans)
        {
            writer.WriteInt32(booleans.Length);
            foreach (bool boolean in booleans)
                writer.WriteBool(boolean);
        }

        public static bool[] DeserializeBooleanArray(PooledBitReader reader)
        {
            int size = reader.ReadInt32();
            bool[] array = new bool[size];
            for (int i = 0; i < size; i++)
                array[i] = reader.ReadBool();
            return array;
        }

        public static void SerializeStringsArray(PooledBitWriter writer, string[] strings)
        {
            writer.WriteInt32(strings.Length);
            foreach (string str in strings)
                writer.WriteString(str);
        }

        public static string[] DeserializeStringsArray(PooledBitReader reader)
        {
            int size = reader.ReadInt32();
            string[] array = new string[size];
            for (int i = 0; i < size; i++)
                array[i] = reader.ReadString().ToString();
            return array;
        }   
    }
}