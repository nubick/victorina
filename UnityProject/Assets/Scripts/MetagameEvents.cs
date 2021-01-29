using Assets.Scripts.Utils;

namespace Victorina
{
    public static class MetagameEvents
    {
        public static GameEvent<NetworkPlayer> PlayerConnected { get; } = new GameEvent<NetworkPlayer>();
        public static GameEvent<ulong> PlayerDisconnect { get; } = new GameEvent<ulong>();

        public static GameEvent<NetRoundQuestion> RoundQuestionClicked { get; } = new GameEvent<NetRoundQuestion>();
        public static GameEvent<int> RoundInfoClicked { get; } = new GameEvent<int>();


        //Client events
        public static GameEvent<int> ClientFileDownloaded = new GameEvent<int>();
    }
}