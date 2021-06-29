namespace Victorina
{
    public class PlayerData : SyncData
    {
        public byte PlayerId { get; }

        private string _name;
        public string Name
        {
            get => _name;
            set
            {
                _name = value; 
                MarkAsChanged();
            }
        }

        private bool _isConnected;
        public bool IsConnected
        {
            get => _isConnected;
            set
            {
                _isConnected = value; 
                MarkAsChanged();
            }
        }

        private int _score;
        public int Score
        {
            get => _score;
            set
            {
                _score = value; 
                MarkAsChanged();
            }
        }

        private byte _filesLoadingPercentage;
        public byte FilesLoadingPercentage
        {
            get => _filesLoadingPercentage;
            set
            {
                _filesLoadingPercentage = value; 
                MarkAsChanged();
            }
        }

        public PlayerData(byte playerId)
        {
            PlayerId = playerId;
        }
        
        public override string ToString() => $"[{PlayerId}:{Name}:{Score}]";
        public override int GetHashCode() => PlayerId;
        protected bool Equals(PlayerData other) => PlayerId == other.PlayerId;
        
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
        
        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((PlayerData) obj);
        }
    }
}