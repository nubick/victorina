using UnityEngine;
using UnityEngine.UI;

namespace Victorina
{
    public class PlayerBoardWidget : MonoBehaviour
    {
        public Text PlayerName;
        public Text Score;

        public void Bind(string playerName, int score)
        {
            PlayerName.text = playerName;
            Score.text = score.ToString();
        }
    }
}