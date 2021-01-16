namespace Victorina
{
    public class VideoStoryDot : StoryDot
    {
        public string Path { get; }
        public byte[] Bytes { get; set; }
        
        public VideoStoryDot(string path)
        {
            Path = path;
        }

        public VideoStoryDot(byte[] bytes)
        {
            Bytes = bytes;
        }
    }
}