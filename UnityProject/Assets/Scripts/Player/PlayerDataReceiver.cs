using Injection;

namespace Victorina
{
    public class PlayerDataReceiver
    {
        [Inject] private MatchData MatchData { get; set; }
        [Inject] private QuestionAnswerData QuestionAnswerData { get; set; }
        [Inject] private DataChangeHandler DataChangeHandler { get; set; }
        [Inject] private PlayerFilesRepository PlayerFilesRepository { get; set; }
        [Inject] private PlayEffectsSystem PlayEffectsSystem { get; set; }

        public void OnReceive(MatchPhase matchPhase)
        {
            MatchData.Phase.Value = matchPhase;
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

        public void OnRoundFileIdsReceived(int[] fileIds, int[] chunksAmounts)
        {
            PlayerFilesRepository.Register(fileIds, chunksAmounts);
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
    }
}