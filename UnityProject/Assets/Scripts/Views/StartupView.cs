using Injection;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

namespace Victorina
{
    public class StartupView : ViewBase
    {
        [Inject] private JoinGameView JoinGameView { get; set; }
        [Inject] private CreatePackageGameView CreatePackageGameView { get; set; }
        [Inject] private PackageCrafterView PackageCrafterView { get; set; }
        
        public Text Version;
        public Text SupportMail;
        public Text DiscordInviteLink;
        
        protected override void OnShown()
        {
            Version.text = $"Версия: {Static.DevSettings.GetAppVersion()}";
            SupportMail.text = Static.SupportMail;
            DiscordInviteLink.text = Static.DiscordInviteLink;
        }

        public void OnCreateNewGameButtonClicked()
        {
            SwitchTo(CreatePackageGameView);
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

        public void OnEmailButtonClicked()
        {
            string subject = UnityWebRequest.EscapeURL($"Вумка ({Static.DevSettings.GetAppVersion()}");
            string url = $"mailto:{Static.SupportMail}?subject={subject})";
            Debug.Log($"Open email client: {url}");
            AnalyticsEvents.SupportMailClicked.Publish();
            Application.OpenURL(url);
        }

        public void OnDiscordButtonClicked()
        {
            Debug.Log($"Open discord invite link: {Static.DiscordInviteLink}");
            AnalyticsEvents.DiscordInviteLinkClicked.Publish();
            Application.OpenURL(Static.DiscordInviteLink);
        }
    }
}