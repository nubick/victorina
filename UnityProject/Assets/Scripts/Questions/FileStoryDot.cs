namespace Victorina
{
    public abstract class FileStoryDot : StoryDot
    {
        public int FileId { get; set; }
        public int ChunksAmount { get; set; }
        public string FileName { get; set; }
        public string Path { get; set; }

        public override bool IsMain => true;
    }
}