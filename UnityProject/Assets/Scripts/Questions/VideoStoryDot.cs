namespace Victorina
{
    public class VideoStoryDot : FileStoryDot
    {
        public override string ToString()
        {
            return $"[vsd|{FileId}]";
        }

    }
}