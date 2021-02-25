using System.Collections.Generic;
using System.Linq;
using Injection;
using UnityEngine;

namespace Victorina
{
    public class MasterDataReceiver
    {
        [Inject] private QuestionAnswerSystem QuestionAnswerSystem { get; set; }
        [Inject] private MatchSystem MatchSystem { get; set; }
        [Inject] private MatchData MatchData { get; set; }
        [Inject] private PlayersBoardSystem PlayersBoardSystem { get; set; }

        public void OnPlayerButtonClickReceived(ulong playerId, float spentSeconds)
        {
            QuestionAnswerSystem.OnPlayerButtonClickReceived(playerId, spentSeconds);
        }

        public void OnCurrentPlayerSelectRoundQuestionReceived(ulong playerId, NetRoundQuestion receivedNetRoundQuestion)
        {
            if (!MatchSystem.IsCurrentPlayer(playerId))
            {
                Debug.Log($"Master. Validation Error. Player: {playerId} is not current. Can't select round question: {receivedNetRoundQuestion}");
                return;
            }

            if (MatchData.Phase.Value != MatchPhase.Round)
            {
                Debug.Log($"Master. Validation Error. Player: {playerId} can't select round question in phase: {MatchData.Phase.Value}");
                return;
            }

            List<NetRoundQuestion> roundQuestions = MatchData.RoundData.Value.Themes.SelectMany(theme => theme.Questions).ToList();
            NetRoundQuestion netRoundQuestion = roundQuestions.SingleOrDefault(_ => _.QuestionId == receivedNetRoundQuestion.QuestionId);

            if (netRoundQuestion == null)
            {
                Debug.Log($"Master. Validation Error. Can't find round question with id: {receivedNetRoundQuestion.QuestionId}");
                return;
            }

            if (netRoundQuestion.IsAnswered)
            {
                Debug.Log($"Master. Validation Error. Can't select answered question: {netRoundQuestion}");
                return;
            }
            
            MatchSystem.SelectQuestion(netRoundQuestion);
        }

        public void OnFilesLoadingPercentageReceived(ulong playerId, byte percentage)
        {
            PlayersBoardSystem.UpdateFilesLoadingPercentage(playerId, percentage);
        }
    }
}