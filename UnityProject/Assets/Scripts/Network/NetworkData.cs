namespace Victorina
{
    public class NetworkData
    {
        public ClientConnectingState ClientConnectingState { get; set; }
        public bool IsMaster { get; set; }
        public bool IsClient => !IsMaster;
        public ulong PlayerId { get; set; }
    }
}