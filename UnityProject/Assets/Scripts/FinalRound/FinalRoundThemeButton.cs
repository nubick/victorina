using UnityEngine;
using UnityEngine.UI;

namespace Victorina
{
    public class FinalRoundThemeButton : MonoBehaviour
    {
        private int _index;
        
        public Image Background;
        public Text ThemeNameText;
        public Image StrikethroughLine;
        public Color OddColor;
        public Color EvenColor;

        public void Bind(int index, string theme, bool isEven, bool isRemoved = false)
        {
            _index = index;
            ThemeNameText.text = theme;
            Background.color = isEven ? EvenColor : OddColor;
            StrikethroughLine.fillAmount = isRemoved ? 1f : 0f;
        }

        public void OnClicked()
        {
            MetagameEvents.FinalRoundThemeClicked.Publish(_index);
        }
    }
}