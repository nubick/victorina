namespace Victorina
{
    public class ImageStoryDot : FileStoryDot
    {
        public override string ToString()
        {
            return $"[isd|{FileId}]";
        }
    }
}