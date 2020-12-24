using System;
using Injection;
using Object = UnityEngine.Object;

namespace Victorina
{
    public class ViewsSystem
    {
        [Inject] private StartupView StartupView { get; set; }
        [Inject] private TextQuestionView TextQuestionView { get; set; }
        [Inject] private GameLobbyView GameLobbyView { get; set; }
        
        [Inject] private MatchData MatchData { get; set; }
        
        public void Initialize()
        {
            foreach(ViewBase view in Object.FindObjectsOfType<ViewBase>())
                view.Content.SetActive(false);
            StartupView.Show();
        }

        private void HideAll()
        {
            foreach(ViewBase view in Object.FindObjectsOfType<ViewBase>())
                if(view.IsActive)
                    view.Hide();
        }
        
        public void Refresh()
        {
            HideAll();
            
            switch (MatchData.Phase)
            {
                case MatchPhase.WaitingInLobby:
                    GameLobbyView.Show();
                    break;
                case MatchPhase.Question:
                    TextQuestionView.Show();
                    break;
                default:
                    throw new Exception($"Not supported phase: {MatchData.Phase}");
            }
        }
    }
}