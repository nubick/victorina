using Injection;
using UnityEngine;

namespace Victorina
{
    public class CatInBagSystem
    {
        [Inject] private MatchSystem MatchSystem { get; set; }
        [Inject] private MatchData MatchData { get; set; }
        [Inject] private QuestionAnswerData QuestionAnswerData { get; set; }
        [Inject] private CatInBagData CatInBagData { get; set; }
        [Inject] private NetworkData NetworkData { get; set; }
        [Inject] private PlayersBoardSystem PlayersBoardSystem { get; set; }
        [Inject] private SendToMasterService SendToMasterService { get; set; }
        [Inject] private SendToPlayersService SendToPlayersService { get; set; }

        private bool IsWaitingWhoGiveCatInBag =>
            MatchData.Phase.Value == MatchPhase.Question &&
            QuestionAnswerData.CurrentStoryDot is CatInBagStoryDot &&
            !CatInBagData.IsPlayerSelected.Value;

        public void Initialize()
        {
            MetagameEvents.PlayerBoardWidgetClicked.Subscribe(OnPlayerBoardWidgetClicked);
        }

        private void OnPlayerBoardWidgetClicked(PlayerData playerData)
        {
            if (IsWaitingWhoGiveCatInBag)
            {
                if (!CanGiveToPlayer(playerData.Id))
                {
                    Debug.Log($"Can't give cat in bag to current player: {playerData}");
                    return;
                }

                if (NetworkData.IsMaster)
                {
                    Debug.Log($"Master. Give cat in bag to {playerData}");
                    GiveCatInBag(playerData.Id);
                }
                else
                {
                    if (MatchData.IsMeCurrentPlayer)
                    {
                        SendToMasterService.SendWhoWillGetCatInBag(playerData.Id);
                    }
                    else
                    {
                        Debug.Log("Only Master or current player can select who will take cat in bag.");
                    }
                }
            }
        }

        private bool CanGiveToPlayer(ulong playerId)
        {
            CatInBagStoryDot catInBagStoryDot = QuestionAnswerData.CurrentStoryDot as CatInBagStoryDot;
            return catInBagStoryDot.CanGiveYourself || !MatchSystem.IsCurrentPlayer(playerId);
        }

        public void OnMasterReceiveWhoWillGetCatInBag(ulong senderPlayerId, ulong whoGetPlayerId)
        {
            if (!MatchSystem.IsCurrentPlayer(senderPlayerId))
            {
                Debug.Log($"Master. Validation Error: Can't accept who will get cat in bag from not current player {senderPlayerId}");
                return;
            }

            if (!IsWaitingWhoGiveCatInBag)
            {
                Debug.Log($"Master. Validation Error: Can't accept who will get cat in bag in not waiting this information state from {senderPlayerId}");
                return;
            }

            if (!CanGiveToPlayer(whoGetPlayerId))
            {
                Debug.Log($"Master. Validation Error: Can't give cat in bag to current player {whoGetPlayerId}, from player {senderPlayerId} request");
                return;
            }
            
            Debug.Log($"Master. Give cat in bag to {whoGetPlayerId} from {senderPlayerId} request");
            GiveCatInBag(whoGetPlayerId);
        }

        private void GiveCatInBag(ulong playerId)
        {
            PlayersBoardSystem.MakePlayerCurrent(playerId);
            
            CatInBagData.IsPlayerSelected.Value = true;
            SendToPlayersService.SendCatInBagData(CatInBagData);
        }
    }
}