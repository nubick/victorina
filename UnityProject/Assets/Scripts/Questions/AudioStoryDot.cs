namespace Victorina
{
    public class AudioStoryDot : FileStoryDot
    {
        public override string ToString()
        {
            return $"[asd|{FileId}]";
        }

        public override char ToLetter()
        {
            return 'M';
        }
    }
}