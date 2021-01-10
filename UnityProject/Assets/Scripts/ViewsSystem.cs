using System;
using System.Collections;
using Injection;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Victorina
{
    public class ViewsSystem
    {
        [Inject] private StartupView StartupView { get; set; }
        [Inject] private TextQuestionView TextQuestionView { get; set; }
        [Inject] private ImageQuestionView ImageQuestionView { get; set; }
        [Inject] private GameLobbyView GameLobbyView { get; set; }
        [Inject] private RoundView RoundView { get; set; }
        
        [Inject] private MatchData MatchData { get; set; }
        
        public void Initialize()
        {
            foreach(ViewBase view in Object.FindObjectsOfType<ViewBase>())
                view.Content.SetActive(false);
            StartupView.Show();

            MatchData.Phase.SubscribeChanged(() => RefreshPhase(MatchData.Phase.Value));
        }

        private void HideAll()
        {
            foreach(ViewBase view in Object.FindObjectsOfType<ViewBase>())
                if(view.IsActive)
                    view.Hide();
        }

        private void RefreshPhase(MatchPhase phase)
        {
            Debug.Log($"RefreshPhase: {phase}");
            if (phase == MatchPhase.Question)
            {
                RoundView.StartCoroutine(SwitchToQuestionView(MatchData.SelectedRoundQuestion));
            }
            else
            {
                HideAll();
                switch (phase)
                {
                    case MatchPhase.WaitingInLobby:
                        GameLobbyView.Show();
                        break;
                    case MatchPhase.Round:
                        RoundView.Show();
                        break;
                    default:
                        throw new Exception($"Not supported phase: {MatchData.Phase}");
                }
            }
        }

        private IEnumerator SwitchToQuestionView(NetRoundQuestion netRoundQuestion)
        {
            if (RoundView.IsActive)
                yield return RoundView.ShowQuestionBlinkEffect(netRoundQuestion);
            
            HideAll();

            if (MatchData.SelectedQuestion.IsImage)
                ImageQuestionView.Show();
            else
                TextQuestionView.Show();
        }
    }
}