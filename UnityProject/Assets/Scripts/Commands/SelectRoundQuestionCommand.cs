using System.Collections.Generic;
using System.Linq;
using Injection;
using MLAPI.Serialization.Pooled;
using UnityEngine;

namespace Victorina.Commands
{
    public class SelectRoundQuestionCommand : Command, INetworkCommand, IServerCommand
    {
        [Inject] private MatchSystem MatchSystem { get; set; }
        [Inject] private MatchData MatchData { get; set; }

        public string QuestionId { get; private set; }
        public override CommandType Type => CommandType.SelectRoundQuestion;
        private bool IsOwnerCurrentPlayerOrMaster => Owner == CommandOwner.Master || MatchSystem.IsCurrentPlayer(OwnerPlayer);
        
        public SelectRoundQuestionCommand() { }
        
        public SelectRoundQuestionCommand(NetRoundQuestion question)
        {
            QuestionId = question.QuestionId;
        }

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
        
        public void ExecuteOnServer()
        {
            NetRoundQuestion netRoundQuestion = GetQuestion(QuestionId);
            MatchSystem.SelectQuestion(netRoundQuestion);
        }
        
        public bool CanExecuteOnServer()
        {
            if (!IsOwnerCurrentPlayerOrMaster)
            {
                Debug.Log($"Master. Validation Error. Player: {OwnerPlayer} is not current. Can't select round question: {QuestionId}");
                return false;
            }

            if (MatchData.Phase.Value != MatchPhase.Round)
            {
                Debug.Log($"Master. Validation Error. Player: {OwnerPlayer} can't select round question in phase: {MatchData.Phase.Value}");
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