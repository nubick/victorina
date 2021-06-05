using Injection;
using UnityEngine;
using UnityEngine.UI;

namespace Victorina
{
    public class FinalRoundView : ViewBase
    {
        [Inject] private FinalRoundData FinalRoundData { get; set; }
        [Inject] private FinalRoundSystem FinalRoundSystem { get; set; }
        [Inject] private NetworkData NetworkData { get; set; }
        
        public RectTransform ThemeButtonsRoot;
        public FinalRoundThemeButton RemoveThemeButtonPrefab;
        
        [Header("Master")]
        public GameObject MasterPanel;
        public Button AcceptBetsButton;

        public void Initialize()
        {
            MetagameEvents.FinalRoundThemeClicked.Subscribe(OnThemeClicked);
            MetagameEvents.FinalRoundDataChanged.Subscribe(OnFinalRoundDataChanged);
        }
        
        protected override void OnShown()
        {
            RefreshUI(FinalRoundData.Themes, FinalRoundData.RemovedThemes);
        }

        private void OnFinalRoundDataChanged()
        {
            RefreshUI(FinalRoundData.Themes, FinalRoundData.RemovedThemes);
        }
        
        public void RefreshUI(string[] themes, bool[] removedThemes)
        {
            MasterPanel.SetActive(NetworkData.IsMaster);
            AcceptBetsButton.interactable = FinalRoundSystem.IsRemovingFinished();
            
            ClearChild(ThemeButtonsRoot);
            for (int i = 0; i < themes.Length; i++)
            {
                FinalRoundThemeButton button = Instantiate(RemoveThemeButtonPrefab, ThemeButtonsRoot);
                button.Bind(i, themes[i], isEven: i % 2 == 0, isRemoved: removedThemes[i]);
            }
        }

        private void OnThemeClicked(int index)
        {
            FinalRoundSystem.TryRemoveTheme(index);
        }

        public void OnAcceptBetsButtonClicked()
        {
            
        }

        public void OnPassFinalRoundButtonClicked()
        {
            
        }

        public void OnRestartButtonClicked()
        {
            FinalRoundSystem.Reset();
        }
    }
}