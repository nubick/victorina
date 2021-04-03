using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Victorina
{
    public class AuctionPlayerWidget : MonoBehaviour, IPointerClickHandler
    {
        public Text Bet;

        public void Bind(PlayerData player, AuctionData auctionData)
        {
            if (auctionData.Player == player)
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
            Debug.Log($"Todo: Implement click handling");
        }
    }
}