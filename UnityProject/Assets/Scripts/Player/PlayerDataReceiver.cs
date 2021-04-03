using System.Linq;
using Injection;

namespace Victorina
{
    public class PlayerDataReceiver
    {
        [Inject] private MatchData MatchData { get; set; }
        [Inject] private MatchSystem MatchSystem { get; set; }
        [Inject] private NetworkData NetworkData { get; set; }
        [Inject] private PlayersBoardSystem PlayersBoardSystem { get; set; }
        [Inject] private QuestionAnswerData QuestionAnswerData { get; set; }
        [Inject] private DataChangeHandler DataChangeHandler { get; set; }
        [Inject] private PlayerFilesRepository PlayerFilesRepository { get; set; }
        [Inject] private PlayerFilesRequestSystem PlayerFilesRequestSystem { get; set; }
        [Inject] private PlayEffectsSystem PlayEffectsSystem { get; set; }
        [Inject] private CatInBagData CatInBagData { get; set; }
        [Inject] private AnsweringTimerData AnsweringTimerData { get; set; }

        public void OnReceiveRegisteredPlayerId(byte playerId)
        {
            NetworkData.RegisteredPlayerId = playerId;
            InitializePlayerData();
        }
        
        public void OnReceive(MatchPhase matchPhase)
        {
            MatchData.Phase.Value = matchPhase;
        }

        private void InitializePlayerData()
        {
            MatchData.IsMeCurrentPlayer = MatchSystem.IsCurrentPlayer(NetworkData.RegisteredPlayerId);
            MatchData.ThisPlayer = PlayersBoardSystem.GetPlayer(NetworkData.RegisteredPlayerId);
        }
        
        public void OnReceive(PlayersBoard playersBoard)
        {
            MatchData.PlayersBoard.Value = playersBoard;
            if (NetworkData.RegisteredPlayerId > 0)
                InitializePlayerData();
        }
        
        public void OnReceive(PlayersButtonClickData data)
        {
            MatchData.QuestionAnswerData.PlayersButtonClickData.Value = data;
        }

        public void OnReceive(QuestionAnswerData data)
        {
            QuestionAnswerData.MasterIntention = data.MasterIntention;
            
            QuestionAnswerData.AnsweringPlayerId = data.AnsweringPlayerId;
            QuestionAnswerData.AnsweringPlayerName = data.AnsweringPlayerName;
            QuestionAnswerData.WrongAnsweredIds = data.WrongAnsweredIds;

            bool wasPhaseChanged = QuestionAnswerData.Phase.Value != data.Phase.Value;
            if (wasPhaseChanged)
                QuestionAnswerData.Phase.Value = data.Phase.Value;

            QuestionAnswerData.TimerState = data.TimerState;
            QuestionAnswerData.TimerResetSeconds = data.TimerResetSeconds;
            QuestionAnswerData.TimerLeftSeconds = data.TimerLeftSeconds;
            
            QuestionAnswerData.CurrentStoryDotIndex = data.CurrentStoryDotIndex;
            
            DataChangeHandler.HandleMasterIntention(QuestionAnswerData);
        }

        public void OnReceiveCatInBagData(bool isPlayerSelected)
        {
            CatInBagData.IsPlayerSelected.Value = isPlayerSelected;
        }
        
        public void OnRoundFileIdsReceived(int[] fileIds, int[] chunksAmounts, int[] priorities)
        {
            PlayerFilesRepository.Register(fileIds, chunksAmounts, priorities);
            PlayerFilesRequestSystem.SendLoadingProgress();
        }
        
        public void OnFileStoryDotReceived(FileStoryDot fileStoryDot)
        {
            PlayerFilesRepository.Register(fileStoryDot.FileId, fileStoryDot.ChunksAmount);
        }
        
        public void OnFileChunkReceived(int fileId, int chunkIndex, byte[] bytes)
        {
            PlayerFilesRepository.AddChunk(fileId, chunkIndex, bytes);
        }

        public void OnReceivePlaySoundEffectCommand(int number)
        {
            PlayEffectsSystem.PlaySound(number);
        }

        public void OnReceiveAnsweringTimerData(bool isRunning, float maxSeconds, float leftSeconds)
        {
            AnsweringTimerData.IsRunning = isRunning;
            AnsweringTimerData.MaxSeconds = maxSeconds;
            AnsweringTimerData.LeftSeconds = leftSeconds;
        }

        public void OnReceiveNetRound(NetRound netRound)
        {
            foreach (NetRoundQuestion roundQuestion in netRound.Themes.SelectMany(theme => theme.Questions))
                roundQuestion.IsDownloadedByMe = roundQuestion.FileIds.All(PlayerFilesRepository.IsDownloaded);

            MatchData.RoundData.Value = netRound;
        }

        public void OnReceiveAuctionData(AuctionData auctionData)
        {
            QuestionAnswerData.AuctionData.Value = auctionData;
        }
    }
}