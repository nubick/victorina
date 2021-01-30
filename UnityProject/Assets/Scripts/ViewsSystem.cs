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
        [Inject] private TextStoryDotView TextStoryDotView { get; set; }
        [Inject] private ImageStoryDotView ImageStoryDotView { get; set; }
        [Inject] private AudioStoryDotView AudioStoryDotView { get; set; }
        [Inject] private VideoStoryDotView VideoStoryDotView { get; set; }

        [Inject] private MasterQuestionPanelView MasterQuestionPanelView { get; set; }
        [Inject] private PlayerButtonView PlayerButtonView { get; set; }
        
        [Inject] private MatchData MatchData { get; set; }
        [Inject] private NetworkData NetworkData { get; set; }
        
        public void Initialize()
        {
            foreach(ViewBase view in Object.FindObjectsOfType<ViewBase>())
                view.Content.SetActive(false);
            StartupView.Show();

            MatchData.Phase.SubscribeChanged(OnMatchPhaseChanged);
            MatchData.QuestionAnsweringData.CurrentStoryDotIndex.SubscribeChanged(OnCurrentStoryDotIndexChanged);
        }

        private void HideAll()
        {
            foreach(ViewBase view in Object.FindObjectsOfType<ViewBase>())
                if(view.IsActive)
                    view.Hide();
        }

        private void OnMatchPhaseChanged()
        {
            MatchPhase phase = MatchData.Phase.Value;
            Debug.Log($"OnPhaseChanged: {phase}");
            
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

        private void OnCurrentStoryDotIndexChanged()
        {
            QuestionPhase phase = MatchData.QuestionAnsweringData.Phase.Value;
            Debug.Log($"OnCurrentStoryDotIndexChanged: {phase}, {MatchData.QuestionAnsweringData.CurrentStoryDotIndex.Value}");
            if (phase == QuestionPhase.ShowQuestion || phase == QuestionPhase.ShowAnswer)
                RoundView.StartCoroutine(SwitchToQuestionView(MatchData.SelectedRoundQuestion));
        }

        private IEnumerator SwitchToQuestionView(NetRoundQuestion netRoundQuestion)
        {
            if (RoundView.IsActive)
                yield return RoundView.ShowQuestionBlinkEffect(netRoundQuestion);

            HideAll();

            if (MatchData.QuestionAnsweringData.CurrentStoryDot is ImageStoryDot)
                ImageStoryDotView.Show();
            else if (MatchData.QuestionAnsweringData.CurrentStoryDot is AudioStoryDot)
                AudioStoryDotView.Show();
            else if (MatchData.QuestionAnsweringData.CurrentStoryDot is VideoStoryDot)
                VideoStoryDotView.Show();
            else
                TextStoryDotView.Show();
            
            if(NetworkData.IsMaster)
                MasterQuestionPanelView.Show();
            else
                PlayerButtonView.Show();
        }
    }
}