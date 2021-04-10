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

        public static bool operator ==(PlayerData player1, PlayerData player2)
        {
            if (ReferenceEquals(player1, null))
                return ReferenceEquals(player2, null);
            
            if (ReferenceEquals(player2, null))
                return false;

            return player1.PlayerId == player2.PlayerId;
        }

        public static bool operator!= (PlayerData player1, PlayerData player2)
        {
            return !(player1 == player2);
        }
        
        protected bool Equals(PlayerData other)
        {
            return PlayerId == other.PlayerId;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((PlayerData) obj);
        }
    }
}