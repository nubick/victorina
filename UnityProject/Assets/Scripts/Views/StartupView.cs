using Injection;
using UnityEngine.UI;

namespace Victorina
{
    public class StartupView : ViewBase
    {
        [Inject] private JoinGameView JoinGameView { get; set; }
        [Inject] private ServerService ServerService { get; set; }
        [Inject] private PackageCrafterView PackageCrafterView { get; set; }
        
        public Text Version;
        
        protected override void OnShown()
        {
            Version.text = $"Версия: {Static.DevSettings.GetAppVersion()}";
        }

        public void OnCreateNewGameButtonClicked()
        {
            ServerService.StartServer();
        }

        public void OnJoinGameButtonClicked()
        {
            SwitchTo(JoinGameView);
        }

        public void OnEditorButtonClicked()
        {
            AnalyticsEvents.CrafterOpen.Publish();
            SwitchTo(PackageCrafterView);
        }
    }
}