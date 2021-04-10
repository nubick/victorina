using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Victorina
{
    public class AuctionPlayerWidget : MonoBehaviour, IPointerClickHandler
    {
        private PlayerData _player;
        
        public Text Bet;
        public GameObject Selection;

        public void Bind(PlayerData player, AuctionData auctionData, bool isSelected)
        {
            _player = player;
            
            Selection.SetActive(isSelected);
            
            if (auctionData.BettingPlayer == player)
            {
                if (auctionData.Player == player)
                    Bet.text = "Выиграл";
                else
                    Bet.text = "Делает ставку";
            }
            else if (auctionData.Player == player)
            {
                if (auctionData.IsAllIn)
                    Bet.text = "Ва-Банк";
                else
                    Bet.text = auctionData.Bet.ToString();
            }
            else
            {
                if (auctionData.PassedPlayers.Contains(player))
                    Bet.text = "Пас";
                else
                    Bet.text = "Ожидание";
            }
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            MetagameEvents.AuctionPlayerClicked.Publish(_player);
        }
    }
}