using System.Collections.Generic;
using System.Linq;
using Injection;
using MLAPI.Serialization.Pooled;
using UnityEngine;

namespace Victorina.Commands
{
    public class SelectRoundQuestionCommand : Command, INetworkCommand, IServerCommand
    {
        [Inject] private PlayersBoardSystem PlayersBoardSystem { get; set; }
        [Inject] private PackagePlayStateSystem PlayStateSystem { get; set; }
        [Inject] private PackagePlayStateData PlayStateData { get; set; }
        [Inject] private CommandsSystem CommandsSystem { get; set; }
        [Inject] private TimerSystem TimerSystem { get; set; }
        
        public string QuestionId { get; set; }
        public override CommandType Type => CommandType.SelectRoundQuestion;
        private bool IsOwnerCurrentPlayerOrMaster => Owner == CommandOwner.Master || PlayersBoardSystem.IsCurrentPlayer(OwnerPlayer);

        public NetRoundQuestion GetNetQuestion(string questionId)
        {
            List<NetRoundQuestion> roundQuestions = PlayStateData.As<RoundPlayState>().NetRound.Themes.SelectMany(theme => theme.Questions).ToList();
            return roundQuestions.SingleOrDefault(_ => _.QuestionId == questionId);
        }

        private bool IsValid(string questionId)
        {
            if (PlayStateData.Type != PlayStateType.Round)
            {
                Debug.Log($"Player: {OwnerPlayer} can't select round question in play state: {PlayStateData}");
                return false;
            }

            if (!IsOwnerCurrentPlayerOrMaster)
            {
                Debug.Log($"Player: {OwnerPlayer} is not current. Can't select round question: {questionId}");
                return false;
            }

            NetRoundQuestion question = GetNetQuestion(questionId);
            
            if (question == null)
            {
                Debug.Log($"Can't find round question with id: {questionId}");
                return false;
            }

            if (question.IsAnswered)
            {
                Debug.Log($"Can't select answered question: {question}");
                return false;
            }
            
            return true;
        }
        
        public bool CanSend()
        {
            return IsValid(QuestionId);
        }
        
        public bool CanExecuteOnServer()
        {
            return IsValid(QuestionId);
       }
        
        public void ExecuteOnServer()
        {
            RoundPlayState roundPlayState = PlayStateData.As<RoundPlayState>();
            
            SendSelectQuestionEvents(roundPlayState.NetRound, roundPlayState.RoundNumber);
            
            RoundBlinkingPlayState blinkingPlayState = new RoundBlinkingPlayState();
            blinkingPlayState.QuestionId = QuestionId;
            blinkingPlayState.RoundNumber = roundPlayState.RoundNumber;
            blinkingPlayState.RoundTypes = roundPlayState.RoundTypes;
            blinkingPlayState.NetRound = roundPlayState.NetRound;
            PlayStateSystem.ChangePlayState(blinkingPlayState);
            
            TimerSystem.RunAfter(1.5f, CreateStopRoundBlinkingCommand);
        }

        private void CreateStopRoundBlinkingCommand()
        {
            CommandsSystem.AddNewCommand(new StopRoundBlinkingCommand());
        }
        
        private void SendSelectQuestionEvents(NetRound netRound, int roundNumber)
        {
            int answered = netRound.Themes.SelectMany(theme => theme.Questions).Count(question => question.IsAnswered);
            int notAnswered = netRound.Themes.SelectMany(theme => theme.Questions).Count(question => !question.IsAnswered);

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