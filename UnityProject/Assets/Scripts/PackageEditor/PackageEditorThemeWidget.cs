using UnityEngine;
using UnityEngine.UI;

namespace Victorina
{
    public class PackageEditorThemeWidget : MonoBehaviour
    {
        private Theme _theme;
        
        public Text ThemeName;
        public Color ThemeDefaultColor;
        public Color ThemeSelectedColor;
        public Transform QuestionsRoot;

        public void Bind(Theme theme, bool isSelected)
        {
            _theme = theme;
            ThemeName.text = theme.Name;
            ThemeName.color = isSelected ? ThemeSelectedColor : ThemeDefaultColor;
        }

        public void OnThemeButtonClicked()
        {
            MetagameEvents.EditorThemeClicked.Publish(_theme);
        }
    }
}