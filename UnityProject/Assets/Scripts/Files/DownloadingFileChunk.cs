namespace Victorina
{
    public class DownloadingFileChunk
    {
        public byte[] Bytes { get; set; }
        public float RequestTime { get; set; }
        public bool IsDownloaded => Bytes != null;
    }
}