using Assets.Scripts.Utils;

namespace Victorina
{
    public static class MetagameEvents
    {
        public static GameEvent<NetworkPlayer> PlayerConnected { get; } = new GameEvent<NetworkPlayer>();
        public static GameEvent<ulong> PlayerDisconnect { get; } = new GameEvent<ulong>();

        public static GameEvent<NetRoundQuestion> RoundQuestionClicked { get; } = new GameEvent<NetRoundQuestion>();
        public static GameEvent<int> RoundInfoClicked { get; } = new GameEvent<int>();

        public static GameEvent<PlayerButtonClickData> PlayerButtonClickWidgetClicked { get; } = new GameEvent<PlayerButtonClickData>();
        
        public static GameEvent QuestionTimerPauseOn { get; } = new GameEvent();
        public static GameEvent QuestionTimerPauseOff { get; } = new GameEvent();
        
        //Client events
        public static GameEvent<int> ClientFileDownloaded { get; } = new GameEvent<int>();
        public static GameEvent ClientFileRequested { get; } = new GameEvent();
    }
}