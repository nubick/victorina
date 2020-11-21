using Assets.Scripts.Utils;

namespace Victorina
{
    public static class MetagameEvents
    {
        public static GameEvent<NetworkPlayer> PlayerConnected { get; } = new GameEvent<NetworkPlayer>();
        public static GameEvent<NetworkPlayer> PlayerDisconnect { get; } = new GameEvent<NetworkPlayer>();
    }
}