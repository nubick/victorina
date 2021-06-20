using System.Collections.Generic;
using System.Linq;
using Injection;
using MLAPI.Serialization.Pooled;
using UnityEngine;

namespace Victorina.Commands
{
    public class SelectRoundQuestionCommand : Command, INetworkCommand, IServerCommand
    {
        [Inject] private MatchData MatchData { get; set; }
        [Inject] private PlayersBoardSystem PlayersBoardSystem { get; set; }
        [Inject] private PackagePlayStateSystem PlayStateSystem { get; set; }
        [Inject] private PackagePlayStateData PlayStateData { get; set; }
        [Inject] private CommandsSystem CommandsSystem { get; set; }
        [Inject] private TimerSystem TimerSystem { get; set; }
        
        public string QuestionId { get; set; }
        public override CommandType Type => CommandType.SelectRoundQuestion;
        private bool IsOwnerCurrentPlayerOrMaster => Owner == CommandOwner.Master || PlayersBoardSystem.IsCurrentPlayer(OwnerPlayer);

        private NetRoundQuestion GetQuestion(string questionId)
        {
            List<NetRoundQuestion> roundQuestions = MatchData.RoundData.Value.Themes.SelectMany(theme => theme.Questions).ToList();
            return roundQuestions.SingleOrDefault(_ => _.QuestionId == questionId);
        }

        public bool CanSend()
        {
            NetRoundQuestion question = GetQuestion(QuestionId);
            
            if (question.IsAnswered)
            {
                Debug.Log($"Selected question is answered: {question}");
                return false;
            }

            if (!IsOwnerCurrentPlayerOrMaster)
            {
                Debug.Log($"Only Master or Current player can select question: {question}");
                return false;
            }

            return true;
        }
        
        public bool CanExecuteOnServer()
        {
            if (!IsOwnerCurrentPlayerOrMaster)
            {
                Debug.Log($"Master. Validation Error. Player: {OwnerPlayer} is not current. Can't select round question: {QuestionId}");
                return false;
            }

            if (PlayStateData.Type != PlayStateType.Round)
            {
                Debug.Log($"Player: {OwnerPlayer} can't select round question in play state: {PlayStateData}");
                return false;
            }
            
            NetRoundQuestion question = GetQuestion(QuestionId);
            
            if (question == null)
            {
                Debug.Log($"Master. Validation Error. Can't find round question with id: {QuestionId}");
                return false;
            }

            if (question.IsAnswered)
            {
                Debug.Log($"Master. Validation Error. Can't select answered question: {question}");
                return false;
            }

            return true;
        }
        
        public void ExecuteOnServer()
        {
            RoundPlayState roundPlayState = PlayStateData.PlayState as RoundPlayState;
            
            RoundBlinkingPlayState playState = new RoundBlinkingPlayState();
            playState.NetRoundQuestion = GetQuestion(QuestionId);
            playState.RoundNumber = roundPlayState.RoundNumber;
            
            PlayStateSystem.ChangePlayState(playState);
            
            SendSelectQuestionEvents(playState.RoundNumber);

            TimerSystem.RunAfter(3f, CreateStopRoundBlinkingCommand);
        }

        private void CreateStopRoundBlinkingCommand()
        {
            CommandsSystem.AddNewCommand(new StopRoundBlinkingCommand());
        }
        
        private void SendSelectQuestionEvents(int roundNumber)
        {
            int answered = MatchData.RoundData.Value.Themes.SelectMany(theme => theme.Questions).Count(question => question.IsAnswered);
            int notAnswered = MatchData.RoundData.Value.Themes.SelectMany(theme => theme.Questions).Count(question => !question.IsAnswered);

            if (answered == 0)
                AnalyticsEvents.FirstRoundQuestionStart.Publish(roundNumber);
            else if (notAnswered == 1)
                AnalyticsEvents.LastRoundQuestionStart.Publish(roundNumber);
        }

        public void Serialize(PooledBitWriter writer)
        {
            writer.WriteString(QuestionId);
        }

        public void Deserialize(PooledBitReader reader)
        {
            QuestionId = reader.ReadString().ToString();
        }

        public override string ToString()
        {
            return $"[SelectRoundQuestionCommand, QuestionId: {QuestionId}, Owner: {OwnerString}]";
        }
    }
}