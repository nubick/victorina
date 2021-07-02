namespace Victorina
{
    public class JoinedPlayer
    {
        public string Guid { get; set; }
        public byte PlayerId { get; set; }
        public ulong ClientId { get; set; }
        public string Name { get; set; }
        public bool IsConnected { get; set; }
    }
}