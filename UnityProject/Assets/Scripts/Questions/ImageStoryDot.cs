namespace Victorina
{
    public class ImageStoryDot : StoryDot
    {
        public string Path { get; }
        public byte[] Bytes { get; set; }

        public ImageStoryDot(string path)
        {
            Path = path;
        }

        public ImageStoryDot(byte[] bytes)
        {
            Bytes = bytes;
        }
    }
}