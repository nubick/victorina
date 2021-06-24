namespace Victorina
{
    public class AudioStoryDot : FileStoryDot
    {
        public override StoryDotType Type => StoryDotType.Audio;
        
        public override string ToString()
        {
            return $"[asd|{FileId}]";
        }
    }
}