namespace Victorina
{
    public class VideoStoryDot : FileStoryDot
    {
        public override string ToString()
        {
            return $"[vsd|{FileId}]";
        }

        public override char ToLetter()
        {
            return 'V';
        }
    }
}