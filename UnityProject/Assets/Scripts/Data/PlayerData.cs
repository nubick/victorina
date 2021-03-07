namespace Victorina
{
    public class PlayerData
    {
        public string ServerGuid { get; set; }
        
        public ulong Id { get; set; }
        public string Name { get; set; }
        public bool IsConnected { get; set; }
        public int Score { get; set; }
        public byte FilesLoadingPercentage { get; set; }
        
        public override string ToString()
        {
            return $"[{Id}:{Name}:{Score}]";
        }
    }
}