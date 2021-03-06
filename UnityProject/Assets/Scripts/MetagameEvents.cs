using Assets.Scripts.Utils;

namespace Victorina
{
    public static class MetagameEvents
    {
        public static GameEvent<NetworkPlayer> PlayerConnected { get; } = new GameEvent<NetworkPlayer>();
        public static GameEvent PlayerDisconnected { get; } = new GameEvent();
        

        public static GameEvent<NetRoundQuestion> RoundQuestionClicked { get; } = new GameEvent<NetRoundQuestion>();
        public static GameEvent<int> RoundInfoClicked { get; } = new GameEvent<int>();
        public static GameEvent<PlayerData> PlayerBoardWidgetClicked { get; } = new GameEvent<PlayerData>();

        public static GameEvent<PlayerButtonClickData> PlayerButtonClickWidgetClicked { get; } = new GameEvent<PlayerButtonClickData>();
        
        public static GameEvent QuestionTimerStarted { get; } = new GameEvent();
        public static GameEvent QuestionTimerPaused { get; } = new GameEvent();
        

        public static GameEvent MediaRestarted { get; } = new GameEvent();

        //Master events
        public static GameEvent ServerStarted { get; } = new GameEvent();
        public static GameEvent ServerStopped { get; } = new GameEvent();
        
        public static GameEvent TimerRunOut { get; } = new GameEvent();
        
        //Client events
        public static GameEvent<int> ClientFileDownloaded { get; } = new GameEvent<int>();
        public static GameEvent ClientFileRequested { get; } = new GameEvent();
    }
}