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
        [Inject] private GameLobbyView GameLobbyView { get; set; }
        [Inject] private RoundView RoundView { get; set; }
        [Inject] private TextQuestionView TextQuestionView { get; set; }
        [Inject] private ImageStoryDotView ImageStoryDotView { get; set; }
        [Inject] private AudioStoryDotView AudioStoryDotView { get; set; }
        [Inject] private VideoStoryDotView VideoStoryDotView { get; set; }
        [Inject] private AnswerView AnswerView { get; set; }

        [Inject] private MatchData MatchData { get; set; }
        
        public void Initialize()
        {
            foreach(ViewBase view in Object.FindObjectsOfType<ViewBase>())
                view.Content.SetActive(false);
            StartupView.Show();

            MatchData.Phase.SubscribeChanged(RefreshPhase);
            MatchData.CurrentStoryDotIndex.SubscribeChanged(RefreshPhase);
        }

        private void HideAll()
        {
            foreach(ViewBase view in Object.FindObjectsOfType<ViewBase>())
                if(view.IsActive)
                    view.Hide();
        }

        private void RefreshPhase()
        {
            MatchPhase phase = MatchData.Phase.Value;
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
                    case MatchPhase.Answer:
                        AnswerView.Show();
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

            if (MatchData.CurrentStoryDot is ImageStoryDot)
                ImageStoryDotView.Show();
            else if (MatchData.CurrentStoryDot is AudioStoryDot)
                AudioStoryDotView.Show();
            else if (MatchData.CurrentStoryDot is VideoStoryDot)
                VideoStoryDotView.Show();
            else
                TextQuestionView.Show();
        }
    }
}