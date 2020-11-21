using UnityEngine;
using UnityEngine.UI;

namespace Victorina
{
    public class PlayerWidget : MonoBehaviour
    {
        public Text PlayerName;

        public void Bind(NetworkPlayer networkPlayer)
        {
            PlayerName.text = networkPlayer.PlayerName;
        }
    }
}