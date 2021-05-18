using Injection;
using UnityEngine;
using UnityEngine.Serialization;

namespace Victorina
{
    public class ThemesSelectionFromBagView : ViewBase
    {
        [Inject] private CrafterData Data { get; set; }
        [Inject] private CrafterBagSystem CrafterBagSystem { get; set; }

        public RectTransform ThemesRoot;
        public BagThemeWidget ThemeWidgetPrefab;
        
        public void Initialize()
        {
            MetagameEvents.CrafterBagThemeClicked.Subscribe(OnThemeClicked);
        }
        
        protected override void OnShown()
        {
            CrafterBagSystem.RefreshBags();
            RefreshUI();
        }

        private void RefreshUI()
        {
            ClearChild(ThemesRoot);
            foreach (Theme theme in Data.BagAllThemes)
            {
                BagThemeWidget themeWidget = Instantiate(ThemeWidgetPrefab, ThemesRoot);
                themeWidget.Bind(theme, Data.BagSelectedThemes.Contains(theme));
            }
        }

        private void OnThemeClicked(Theme theme)
        {
            if (Data.BagSelectedThemes.Contains(theme))
                Data.BagSelectedThemes.Remove(theme);
            else
                Data.BagSelectedThemes.Add(theme);
            RefreshUI();
        }
        
        public void OnAddButtonClicked()
        {
            Debug.Log($"Themes: {Data.SelectedRound.Themes.Count}, selected: {Data.BagSelectedThemes}");
            Data.SelectedRound.Themes.AddRange(Data.BagSelectedThemes);
        }
    }
}