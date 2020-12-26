using UnityEngine;
using UnityEngine.UI;

namespace Victorina
{
    public class PlayerWidget : MonoBehaviour
    {
        public Text PlayerName;
        public Text Score;

        public void Bind(string playerName)
        {
            PlayerName.text = playerName;
            Score.text = "0";
        }
    }
}