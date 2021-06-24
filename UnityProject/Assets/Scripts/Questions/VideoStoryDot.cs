namespace Victorina
{
    public class VideoStoryDot : FileStoryDot
    {
        public override StoryDotType Type => StoryDotType.Video;
        
        public override string ToString()
        {
            return $"[vsd|{FileId}]";
        }
    }
}