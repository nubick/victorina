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
        [Inject] private LobbyView LobbyView { get; set; }
        [Inject] private RoundView RoundView { get; set; }
        [Inject] private PlayersBoardView PlayersBoardView { get; set; }
        [Inject] private TextStoryDotView TextStoryDotView { get; set; }
        [Inject] private ImageStoryDotView ImageStoryDotView { get; set; }
        [Inject] private AudioStoryDotView AudioStoryDotView { get; set; }
        [Inject] private VideoStoryDotView VideoStoryDotView { get; set; }
        [Inject] private NoRiskStoryDotView NoRiskStoryDotView { get; set; }
        [Inject] private CatInBagStoryDotView CatInBagStoryDotView { get; set; }

        [Inject] private MasterQuestionPanelView MasterQuestionPanelView { get; set; }
        [Inject] private MasterEffectsView MasterEffectsView { get; set; }
        
        [Inject] private PlayerButtonView PlayerButtonView { get; set; }

        [Inject] private MatchData MatchData { get; set; }
        [Inject] private QuestionAnswerData QuestionAnswerData { get; set; }
        [Inject] private NetworkData NetworkData { get; set; }
        [Inject] private CatInBagData CatInBagData { get; set; }
        
        public void Initialize()
        {
            foreach(ViewBase view in Object.FindObjectsOfType<ViewBase>())
                view.Content.SetActive(false);
            StartupView.Show();

            MatchData.Phase.SubscribeChanged(OnMatchPhaseChanged);
            CatInBagData.IsPlayerSelected.SubscribeChanged(OnCatInBagIsPlayerSelectedChanged);
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
            switch (phase)
            {
                case MatchPhase.WaitingInLobby:
                    ShowLobbyViews();
                    break;
                case MatchPhase.Round:
                    ShowRoundViews();
                    break;
                case MatchPhase.Question:
                    break;
                default:
                    throw new Exception($"Not supported phase: {MatchData.Phase}");
            }
        }
        
        public void StartAnswering()
        {
            RoundView.StartCoroutine(SwitchToQuestionView(MatchData.SelectedRoundQuestion, QuestionAnswerData));
        }
        
        private IEnumerator SwitchToQuestionView(NetRoundQuestion netRoundQuestion, QuestionAnswerData data)
        {
            yield return RoundView.ShowQuestionBlinkEffect(netRoundQuestion);
            UpdateStoryDot(data);
        }

        private void ShowLobbyViews()
        {
            HideAll();
            
            LobbyView.Show();
            PlayersBoardView.Show();
            
            if (NetworkData.IsMaster)
                MasterEffectsView.Show();
        }

        private void ShowRoundViews()
        {
            HideAll();   
            
            RoundView.Show();
            PlayersBoardView.Show();
            
            if (NetworkData.IsMaster)
                MasterEffectsView.Show();
        }

        public void UpdateStoryDot(QuestionAnswerData data)
        {
            HideAll();

            if (data.CurrentStoryDot is ImageStoryDot)
                ImageStoryDotView.Show();
            else if (data.CurrentStoryDot is AudioStoryDot)
                AudioStoryDotView.Show();
            else if (data.CurrentStoryDot is VideoStoryDot)
                VideoStoryDotView.Show();
            else if (data.CurrentStoryDot is NoRiskStoryDot)
                NoRiskStoryDotView.Show();
            else if (data.CurrentStoryDot is CatInBagStoryDot)
            {
                CatInBagStoryDotView.Show();
                if (!CatInBagData.IsPlayerSelected.Value)
                    PlayersBoardView.Show();
            }
            else
                TextStoryDotView.Show();

            if (NetworkData.IsMaster)
            {
                MasterQuestionPanelView.Show();
                MasterEffectsView.Show();
            }

            if (NetworkData.IsClient)
            {
                if (data.CurrentStoryDot is NoRiskStoryDot || data.CurrentStoryDot is CatInBagStoryDot)
                    ;
                else
                    PlayerButtonView.Show();
            }
        }

        public void OnClientDisconnected()
        {
            HideAll();
            StartupView.Show();
        }
        
        private void OnCatInBagIsPlayerSelectedChanged()
        {
            if(CatInBagData.IsPlayerSelected.Value)
                PlayersBoardView.Hide();
        }
    }
}