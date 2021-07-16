using UnityEngine;
using UnityEngine.UI;

namespace Victorina
{
    public class ResultPlayerLineWidget : MonoBehaviour
    {
        public Text ResultLabel;
        public Text Name;
        public Text Score;
        
        public void Bind(string resultLabel, string playerName, int score, Color nameColor)
        {
            ResultLabel.text = resultLabel;
            Name.text = playerName;
            Score.text = score.ToString();
            Name.color = nameColor;
        }
    }
}