namespace Victorina
{
    public class ImageStoryDot : FileStoryDot
    {
        public override StoryDotType Type => StoryDotType.Image;
        
        public override string ToString()
        {
            return $"[isd|{FileId}]";
        }
    }
}