namespace Victorina
{
    public class JoinedPlayer
    {
        public byte PlayerId { get; set; }
        public ConnectionMessage ConnectionMessage { get; set; }
        public ulong ClientId { get; set; }
        public bool IsConnected { get; set; }
    }
}