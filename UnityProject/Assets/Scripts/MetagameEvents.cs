using Assets.Scripts.Data;
using Assets.Scripts.Utils;
using UnityEngine;

namespace Victorina
{
    public static class MetagameEvents
    {
        public static GameEvent<NetworkPlayer> NetworkPlayerSpawned { get; } = new GameEvent<NetworkPlayer>();
        
        public static GameEvent<NetRoundQuestion> RoundQuestionClicked { get; } = new GameEvent<NetRoundQuestion>();
        public static GameEvent<int> RoundInfoClicked { get; } = new GameEvent<int>();
        public static GameEvent<PlayerData> PlayerBoardWidgetClicked { get; } = new GameEvent<PlayerData>();

        public static GameEvent<PlayerButtonClickData> PlayerButtonClickWidgetClicked { get; } = new GameEvent<PlayerButtonClickData>();
        public static GameEvent<PlayerData> AuctionPlayerClicked { get; } = new GameEvent<PlayerData>();
        
        public static GameEvent QuestionTimerStarted { get; } = new GameEvent();
        public static GameEvent QuestionTimerPaused { get; } = new GameEvent();
        
        public static GameEvent MediaRestarted { get; } = new GameEvent();
        
        //Data changed
        public static GameEvent PlayersButtonClickDataChanged { get; } = new GameEvent();
        
        //Master events
        public static GameEvent ServerStarted { get; } = new GameEvent();
        public static GameEvent ServerStopped { get; } = new GameEvent();
        public static GameEvent<byte> MasterClientConnected { get; } = new GameEvent<byte>();
        public static GameEvent MasterClientDisconnected { get; } = new GameEvent();
        
        public static GameEvent TimerRunOut { get; } = new GameEvent();
        
        //Client events
        public static GameEvent ConnectedAsClient { get; } = new GameEvent();
        public static GameEvent DisconnectedAsClient { get; } = new GameEvent();
        
        public static GameEvent<int> ClientFileDownloaded { get; } = new GameEvent<int>();
        public static GameEvent ClientFileRequested { get; } = new GameEvent();
        
        //Package crafter events
        public static GameEvent<Package> CrafterPackageClicked { get; } = new GameEvent<Package>();
        public static GameEvent<Package> CrafterPackageDeleteButtonClicked { get; } = new GameEvent<Package>();
        
        public static GameEvent<Round> CrafterRoundClicked { get; } = new GameEvent<Round>();
        public static GameEvent<Round> CrafterRoundDeleteButtonClicked { get; } = new GameEvent<Round>();
        public static GameEvent<Round> CrafterRoundNameEditRequested { get; } = new GameEvent<Round>();
        
        public static GameEvent<Theme> CrafterThemeClicked { get; } = new GameEvent<Theme>();
        public static GameEvent<Theme> CrafterThemeDeleteButtonClicked { get; } = new GameEvent<Theme>();
        public static GameEvent<Theme> CrafterThemeMoveToBagButtonClicked { get; } = new GameEvent<Theme>();
        public static GameEvent<Theme> CrafterThemeNameEditRequested { get; } = new GameEvent<Theme>();
        
        public static GameEvent<Question> CrafterQuestionClicked { get; } = new GameEvent<Question>();
        public static GameEvent<Question> CrafterQuestionSelected { get; } = new GameEvent<Question>();
        public static GameEvent<Question> CrafterQuestionDeleteButtonClicked { get; } = new GameEvent<Question>();
        public static GameEvent<Question> CrafterQuestionEditRequested { get; } = new GameEvent<Question>();

        public static GameEvent<int> CrafterStoryDotPreviewIndexChanged { get; } = new GameEvent<int>();

        public static GameEvent<DragItemBase> CrafterBeginDrag { get; } = new GameEvent<DragItemBase>();
        public static GameEvent<DragItemBase> CrafterEndDrag { get; } = new GameEvent<DragItemBase>();
        public static GameEvent<Question, Question> CrafterQuestionDropOnQuestion { get; } = new GameEvent<Question, Question>();
        public static GameEvent<Question, Theme> CrafterQuestionDropOnTheme { get; } = new GameEvent<Question, Theme>();
        public static GameEvent<Question, Round> CrafterQuestionDropOnRound { get; } = new GameEvent<Question, Round>();
        public static GameEvent<Theme, Theme> CrafterThemeDropOnTheme { get; } = new GameEvent<Theme, Theme>();
        public static GameEvent<Theme, Round> CrafterThemeDropOnRound { get; } = new GameEvent<Theme, Round>();
        
        public static GameEvent<Theme> CrafterBagThemeClicked { get; } = new GameEvent<Theme>();
        
        //Commands
        public static GameEvent<SoundEffect> PlaySoundEffectCommand { get; } = new GameEvent<SoundEffect>();
    }
}