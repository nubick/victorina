namespace Victorina
{
    public class NetworkData
    {
        public ClientConnectingState ClientConnectingState { get; set; }
        public bool IsMaster { get; set; }
        public bool IsClient => !IsMaster;
        public byte RegisteredPlayerId { get; set; }
        
        public string LastPlayerName { get; set; }
        public string LastGameCode { get; set; }
    }
}