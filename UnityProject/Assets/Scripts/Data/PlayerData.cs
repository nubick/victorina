namespace Victorina
{
    public class PlayerData
    {
        public byte PlayerId { get; }
        public string Name { get; set; }
        public bool IsConnected { get; set; }
        public int Score { get; set; }
        public byte FilesLoadingPercentage { get; set; }

        public PlayerData(byte playerId)
        {
            PlayerId = playerId;
        }
        
        public override string ToString()
        {
            return $"[{PlayerId}:{Name}:{Score}]";
        }

        public override int GetHashCode()
        {
            return PlayerId;
        }
    }
}