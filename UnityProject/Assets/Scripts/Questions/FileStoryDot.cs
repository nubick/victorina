namespace Victorina
{
    public abstract class FileStoryDot : StoryDot
    {
        public int FileId { get; set; }
        public int ChunksAmount { get; set; }
        
        public string SiqPath { get; set; }
    }
}