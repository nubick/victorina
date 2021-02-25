namespace Victorina
{
    public class PlayerData
    {
        public ulong Id { get; }
        public string Name { get; set; }
        public int Score { get; set; }
        public byte FilesLoadingPercentage { get; set; }
        
        public PlayerData(ulong id)
        {
            Id = id;
        }

        public override string ToString()
        {
            return $"{Id}:{Name}:{Score}";
        }
    }
}