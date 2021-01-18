namespace Victorina
{
    public class AudioStoryDot : StoryDot
    {
        public string Path { get; }
        public string FullPath { get; set; }
        public byte[] Bytes { get; set; }
        
        public AudioStoryDot(string path)
        {
            Path = path;
        }

        public AudioStoryDot(byte[] bytes)
        {
            Bytes = bytes;
        }
    }
}