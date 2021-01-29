using System;
using System.Linq;

namespace Victorina
{
    public class DownloadingFile
    {
        public int FileId { get; }
        public DownloadingFileChunk[] Chunks { get; }

        public DownloadingFile(int fileId, int chunksAmount)
        {
            FileId = fileId;
            Chunks = new DownloadingFileChunk[chunksAmount];
            for (int i = 0; i < chunksAmount; i++)
                Chunks[i] = new DownloadingFileChunk();
        }

        public bool IsDownloaded()
        {
            return Chunks.All(chunk => chunk.IsDownloaded);
        }

        public void SetChunk(int chunkIndex, byte[] bytes)
        {
            Chunks[chunkIndex].Bytes = bytes;
        }

        public bool IsEmpty(int chunkIndex)
        {
            return !Chunks[chunkIndex].IsDownloaded;
        }
        
        public byte[] GetBytes()
        {
            int size = Chunks.Sum(chunk => chunk.Bytes.Length);
            byte[] bytes = new byte[size];
            int nextIndex = 0;
            foreach (DownloadingFileChunk chunk in Chunks)
            {
                Array.Copy(chunk.Bytes, 0, bytes, nextIndex, chunk.Bytes.Length);
                nextIndex += chunk.Bytes.Length;
            }
            return bytes;
        }
    }
}