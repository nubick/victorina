using System;
using System.Linq;

namespace Victorina
{
    public class DownloadingFile
    {
        public int FileId { get; }
        public byte[][] Chunks { get; }

        public DownloadingFile(int fileId, int chunksAmount)
        {
            FileId = fileId;
            Chunks = new byte[chunksAmount][];
        }

        public bool IsFull()
        {
            return Chunks.All(chunk => chunk != null);
        }

        public void SetChunk(int chunkIndex, byte[] bytes)
        {
            Chunks[chunkIndex] = bytes;
        }

        public int GetFirstEmptyChunkIndex()
        {
            for(int index = 0;index<Chunks.Length;index++)
                if (Chunks[index] == null)
                    return index;
            throw new Exception($"File '{FileId}' is full");
        }

        public byte[] GetBytes()
        {
            int size = Chunks.Sum(chunk => chunk.Length);
            byte[] bytes = new byte[size];
            int nextIndex = 0;
            foreach (byte[] chunk in Chunks)
            {
                Array.Copy(chunk, 0, bytes, nextIndex, chunk.Length);
                nextIndex += chunk.Length;
            }
            return bytes;
        }
    }
}